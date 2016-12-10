// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System
{
    internal static class SpanHelpers
    {
        /// <summary>
        /// Computes "start + index * sizeof(T)", using the unsigned IntPtr-sized multiplication for 32 and 64 bits.
        ///
        /// Assumptions:
        ///     Start and index are non-negative, and already pre-validated to be within the valid range of their containing Span.
        ///
        ///     If the byte length (Span.Length * sizeof(T)) does an unsigned overflow (i.e. the buffer wraps or is too big to fit within the address space),
        ///     the behavior is undefined.
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Add<T>(this IntPtr start, int index)
        {
            Debug.Assert(start.ToInt64() >= 0);
            Debug.Assert(index >= 0);

            unsafe
            {
                if (sizeof(IntPtr) == sizeof(int))
                {
                    // 32-bit path.
                    uint byteLength = (uint)index * (uint)Unsafe.SizeOf<T>();
                    return (IntPtr)(((byte*)start) + byteLength);
                }
                else
                {
                    // 64-bit path.
                    ulong byteLength = (ulong)index * (ulong)Unsafe.SizeOf<T>();
                    return (IntPtr)(((byte*)start) + byteLength);
                }
            }
        }

        /// <summary>
        /// Determine if a type is eligible for storage in unmanaged memory. TODO: To be replaced by a ContainsReference() api.
        /// </summary>
        public static bool IsReferenceFree<T>() => PerTypeValues<T>.IsReferenceFree;

        private static bool IsReferenceFreeCore<T>()
        {
            // Under the JIT, these become constant-folded.
            if (typeof(T) == typeof(byte))
                return true;
            if (typeof(T) == typeof(sbyte))
                return true;
            if (typeof(T) == typeof(bool))
                return true;
            if (typeof(T) == typeof(char))
                return true;
            if (typeof(T) == typeof(short))
                return true;
            if (typeof(T) == typeof(ushort))
                return true;
            if (typeof(T) == typeof(int))
                return true;
            if (typeof(T) == typeof(uint))
                return true;
            if (typeof(T) == typeof(long))
                return true;
            if (typeof(T) == typeof(ulong))
                return true;
            if (typeof(T) == typeof(IntPtr))
                return true;
            if (typeof(T) == typeof(UIntPtr))
                return true;

            return IsReferenceFreeCoreSlow(typeof(T));
        }

        private static bool IsReferenceFreeCoreSlow(Type type)
        {
            if (type.GetTypeInfo().IsPrimitive) // This is hopefully the common case. All types that return true for this are value types w/out embedded references.
                return true;

            if (!type.GetTypeInfo().IsValueType)
                return false;

            // If type is a Nullable<> of something, unwrap it first.
            Type underlyingNullable = Nullable.GetUnderlyingType(type);
            if (underlyingNullable != null)
                type = underlyingNullable;

            if (type.GetTypeInfo().IsEnum)
                return true;

            foreach (FieldInfo field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.IsStatic)
                    continue;
                if (!IsReferenceFreeCoreSlow(field.FieldType))
                    return false;
            }
            return true;
        }

        [StructLayout(LayoutKind.Sequential, Size = 64)]
        private struct Reg64 { }
        [StructLayout(LayoutKind.Sequential, Size = 32)]
        private struct Reg32 { }
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private struct Reg16 { }

        public static void MemSetClear(ref byte b, int byteLength)
        {
            var ptrSize = Unsafe.SizeOf<IntPtr>();
            int i = 0;
            if (byteLength >= ptrSize)
            {
                // Zero IntPtr used as a known location to be used for aligning `ref` location
                var zero = IntPtr.Zero;
                IntPtr byteOffset = Unsafe.ByteOffset(ref b, ref Unsafe.As<IntPtr, byte>(ref zero));

                // IntPtr does not support arithmetic so need to go through hoops and loops to make mask
                IntPtr byteOffsetAligned = ptrSize == 4
                    ? new IntPtr((int)byteOffset & ~(ptrSize - 1))
                    : new IntPtr((long)byteOffset & ~((long)(ptrSize - 1)));

                // Align to be sure we do not tear an object reference
                int bytesBeforeReferenceAlignment = ptrSize == 4
                    ? (int)((int)byteOffset - (int)byteOffsetAligned)
                    : (int)((long)byteOffset - (long)byteOffsetAligned);
                bytesBeforeReferenceAlignment = bytesBeforeReferenceAlignment > byteLength
                    ? byteLength : bytesBeforeReferenceAlignment;

                while (i < bytesBeforeReferenceAlignment)
                {
                    Unsafe.Add<byte>(ref b, i) = 0;
                    ++i;
                }
                // TODO: Perhaps do switch casing...
                while (i < (byteLength - 64))
                {
                    Unsafe.As<byte, Reg64>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg64);
                    i += 64;
                }
                if (i < (byteLength - 32))
                {
                    Unsafe.As<byte, Reg32>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg32);
                    i += 32;
                }
                if (i < (byteLength - 16))
                {
                    Unsafe.As<byte, Reg16>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg16);
                    i += 16;
                }
                if (i < (byteLength - 8))
                {
                    Unsafe.As<byte, long>(ref Unsafe.Add<byte>(ref b, i)) = default(long);
                    i += 8;
                }
                if (i < (byteLength - ptrSize))
                {
                    Unsafe.As<byte, IntPtr>(ref Unsafe.Add<byte>(ref b, i)) = default(IntPtr);
                    i += ptrSize;
                }
            }
            while (i < byteLength)
            {
                Unsafe.Add<byte>(ref b, i) = 0;
                ++i;
            }
        }

        public static class PerTypeValues<T>
        {
            //
            // Latch to ensure that excruciatingly expensive validation check for constructing a Span around a raw pointer is done
            // only once per type (unless of course, the validation fails.)
            //
            // false == not yet computed or found to be not reference free.
            // true == confirmed reference free
            //
            public static readonly bool IsReferenceFree = IsReferenceFreeCore<T>();

            public static readonly T[] EmptyArray = new T[0];

            public static readonly IntPtr ArrayAdjustment = MeasureArrayAdjustment();

            // Array header sizes are a runtime implementation detail and aren't the same across all runtimes. (The CLR made a tweak after 4.5, and Mono has an extra Bounds pointer.)
            private static IntPtr MeasureArrayAdjustment()
            {
                T[] sampleArray = new T[1];
                return Unsafe.ByteOffset<T>(ref Unsafe.As<Pinnable<T>>(sampleArray).Data, ref sampleArray[0]);
            }
        }
    }
}
