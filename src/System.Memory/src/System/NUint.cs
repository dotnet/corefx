// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System
{
    //
    // A makeshift native uint type for build configurations that don't build 32/64-bit specific binaries (and hence, can't alias them to UInt32/UInt64 via an ifdef.)
    //
    internal unsafe struct NUInt
    {
        private readonly void* _value;

        private NUInt(uint value) => _value = (void*)value;
        private NUInt(ulong value) => _value = (void*)value;

        public static implicit operator NUInt(uint value) => new NUInt(value);
        public static implicit operator IntPtr(NUInt value) => (IntPtr)value._value;

        public static explicit operator NUInt(int value) => new NUInt((uint)value);

        public static explicit operator int(NUInt value) => (int)value._value;
        public static explicit operator uint(NUInt value) => (uint)value._value;
        public static explicit operator void* (NUInt value) => value._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator *(NUInt left, uint right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) * right) : new NUInt(((ulong)left._value) * right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator +(NUInt left, uint right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) + right) : new NUInt(((ulong)left._value) + right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator -(NUInt left, uint right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) - right) : new NUInt(((ulong)left._value) - right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator *(NUInt left, NUInt right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) * (uint)right._value) : new NUInt(((ulong)left._value) * (ulong)right._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator +(NUInt left, NUInt right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) + (uint)right._value) : new NUInt(((ulong)left._value) + (ulong)right._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NUInt operator -(NUInt left, NUInt right)
        {
            return (sizeof(IntPtr) == 4) ? new NUInt(((uint)left._value) - (uint)right._value) : new NUInt(((ulong)left._value) - (ulong)right._value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(NUInt left, uint right)
        {
            return (sizeof(IntPtr) == 4) ? ((uint)left._value) >= right : ((ulong)left._value) >= right;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(NUInt left, uint right)
        {
            return (sizeof(IntPtr) == 4) ? ((uint)left._value) <= right : ((ulong)left._value) <= right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(NUInt left, uint right) => !(left <= right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(NUInt left, uint right) => !(left >= right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(NUInt left, NUInt right)
        {
            return (sizeof(IntPtr) == 4) ? ((uint)left._value) <= (uint)right._value : ((ulong)left._value) <= (ulong)right._value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(NUInt left, NUInt right)
        {
            return (sizeof(IntPtr) == 4) ? ((uint)left._value) >= (uint)right._value : ((ulong)left._value) >= (ulong)right._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(NUInt left, NUInt right) => !(left <= right);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(NUInt left, NUInt right) => !(left >= right);

        public static bool operator ==(NUInt left, NUInt right) => left._value == right._value;
        public static bool operator !=(NUInt left, NUInt right) => left._value != right._value;

        public override bool Equals(object obj) => obj is NUInt other && _value == other._value;
        public override int GetHashCode() => new UIntPtr(_value).GetHashCode();

        public override string ToString() => new UIntPtr(_value).ToString();
    }
}
