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

        unsafe static readonly UIntPtr UIntPtrMask4 = sizeof(UIntPtr) == sizeof(uint) ? new UIntPtr(~3u) : new UIntPtr(~((ulong)(3u)));
        unsafe static readonly UIntPtr UIntPtrMask8 = sizeof(UIntPtr) == sizeof(uint) ? new UIntPtr(~7u) : new UIntPtr(~((ulong)(7u)));
        unsafe static readonly UIntPtr UIntPtrMask16 = sizeof(UIntPtr) == sizeof(uint) ? new UIntPtr(~15u) : new UIntPtr(~((ulong)(15u)));
        unsafe static readonly UIntPtr UIntPtrMask32 = sizeof(UIntPtr) == sizeof(uint) ? new UIntPtr(~31u) : new UIntPtr(~((ulong)(31u)));
        unsafe static readonly UIntPtr UIntPtrMask64 = sizeof(UIntPtr) == sizeof(uint) ? new UIntPtr(~63u) : new UIntPtr(~((ulong)(63u)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe static UIntPtr BitwiseAnd(this UIntPtr value, UIntPtr mask)
        {
            return (sizeof(UIntPtr) == sizeof(uint)) 
                ? new UIntPtr((uint)value & (uint)mask)
                : new UIntPtr((ulong)value & (ulong)mask);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe static bool LessThan(this IntPtr index, UIntPtr length)
        {
            return (sizeof(UIntPtr) == sizeof(uint))
                ? (uint)index < (uint)length
                : (ulong)index < (ulong)length;
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
            // TODO: Perhaps do switch casing... 
            // TODO: Bitwise masking generates weird assembly 64-bit, including not inlining calls.

            var i = IntPtr.Zero;
            //while (i.LessThan(byteLength.BitwiseAnd(UIntPtrMask64)))
            while (i.LessThanEqual(byteLength - sizeof(Reg64)))
            {
                Unsafe.As<byte, Reg64>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg64);
                i += sizeof(Reg64);
            }
            //if (i.LessThan(byteLength.BitwiseAnd(UIntPtrMask32)))
            if (i.LessThanEqual(byteLength - sizeof(Reg32)))
            {
                Unsafe.As<byte, Reg32>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg32);
                i += sizeof(Reg32);
            }
            //if (i.LessThan(byteLength.BitwiseAnd(UIntPtrMask16)))
            if (i.LessThanEqual(byteLength - sizeof(Reg16)))
            {
                Unsafe.As<byte, Reg16>(ref Unsafe.Add<byte>(ref b, i)) = default(Reg16);
                i += sizeof(Reg16);
            }
            //if (i.LessThan(byteLength.BitwiseAnd(UIntPtrMask8)))
            if (i.LessThanEqual(byteLength - sizeof(long)))
            {
                Unsafe.As<byte, long>(ref Unsafe.Add<byte>(ref b, i)) = 0;
                i += sizeof(long);
            }
            // JIT: Should elide this if 64-bit
            if (sizeof(IntPtr) == sizeof(int))
            {
                //if (i.LessThan(byteLength.BitwiseAnd(UIntPtrMask4)))
                if (i.LessThanEqual(byteLength - sizeof(int)))
                {
                    Unsafe.As<byte, int>(ref Unsafe.Add<byte>(ref b, i)) = 0;
                    i += sizeof(int);
                }
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
