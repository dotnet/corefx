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

        [StructLayout(LayoutKind.Sequential)]
        struct IntPtrs8
        {
            readonly IntPtr ip0;
            readonly IntPtr ip1;
            readonly IntPtr ip2;
            readonly IntPtr ip3;
            readonly IntPtr ip4;
            readonly IntPtr ip5;
            readonly IntPtr ip6;
            readonly IntPtr ip7;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct IntPtrs4
        {
            readonly IntPtr ip0;
            readonly IntPtr ip1;
            readonly IntPtr ip2;
            readonly IntPtr ip3;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct IntPtrs2
        {
            readonly IntPtr ip0;
            readonly IntPtr ip1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe static bool LessThanEqual(this IntPtr index, UIntPtr length)
        {
            return (sizeof(UIntPtr) == sizeof(uint))
                ? (int)index <= (int)length
                : (long)index <= (long)length;
        }

        public unsafe static void ClearPointerSized(ref byte b, UIntPtr byteLength)
        {
            var i = IntPtr.Zero;
            while (i.LessThanEqual(byteLength - sizeof(IntPtrs8)))
            {
                Unsafe.As<byte, IntPtrs8>(ref Unsafe.Add<byte>(ref b, i)) = default(IntPtrs8);
                i += sizeof(IntPtrs8);
            }
            if (i.LessThanEqual(byteLength - sizeof(IntPtrs4)))
            {
                Unsafe.As<byte, IntPtrs4>(ref Unsafe.Add<byte>(ref b, i)) = default(IntPtrs4);
                i += sizeof(IntPtrs4);
            }
            if (i.LessThanEqual(byteLength - sizeof(IntPtrs2)))
            {
                Unsafe.As<byte, IntPtrs2>(ref Unsafe.Add<byte>(ref b, i)) = default(IntPtrs2);
                i += sizeof(IntPtrs2);
            }
            if (i.LessThanEqual(byteLength - sizeof(IntPtr)))
            {
                Unsafe.As<byte, IntPtr>(ref Unsafe.Add<byte>(ref b, i)) = default(IntPtr);
                i += sizeof(IntPtr);
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
