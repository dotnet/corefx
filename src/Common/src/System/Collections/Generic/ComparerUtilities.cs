// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    internal static class EqualityComparer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DefaultEquals<T>(T x, T y)
        {
            return IsWellKnownType<T>() ?
                DefaultEqualsWellKnown(x, y) :
                EqualityComparer<T>.Default.Equals(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DefaultEqualsWellKnown<T>(T x, T y)
        {
            Debug.Assert(IsWellKnownType<T>());
            return DefaultEqualsWellKnown1(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DefaultEqualsWellKnown1<T>(T x, T y)
        {
            if (default(T) != null)
            {
                if (typeof(T) == typeof(byte))
                {
                    return (byte)(object)x == (byte)(object)y;
                }
                if (typeof(T) == typeof(sbyte))
                {
                    return (sbyte)(object)x == (sbyte)(object)y;
                }
                if (typeof(T) == typeof(short))
                {
                    return (short)(object)x == (short)(object)y;
                }
                if (typeof(T) == typeof(ushort))
                {
                    return (ushort)(object)x == (ushort)(object)y;
                }
                if (typeof(T) == typeof(int))
                {
                    return (int)(object)x == (int)(object)y;
                }
                if (typeof(T) == typeof(uint))
                {
                    return (uint)(object)x == (uint)(object)y;
                }
            }

            return DefaultEqualsWellKnown2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DefaultEqualsWellKnown2<T>(T x, T y)
        {
            if (default(T) != null)
            {
                if (typeof(T) == typeof(long))
                {
                    return (long)(object)x == (long)(object)y;
                }
                if (typeof(T) == typeof(ulong))
                {
                    return (ulong)(object)x == (ulong)(object)y;
                }
                if (typeof(T) == typeof(IntPtr))
                {
                    return (IntPtr)(object)x == (IntPtr)(object)y;
                }
                if (typeof(T) == typeof(UIntPtr))
                {
                    return (UIntPtr)(object)x == (UIntPtr)(object)y;
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return (string)(object)x == (string)(object)y;
                }
            }

            Debug.Fail($"{typeof(T)} is a well-known type, but we don't recognize it.");
            return default(bool);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastEquals<T>(this IEqualityComparer<T> equalityComparer, T x, T y)
        {
            Debug.Assert(equalityComparer != null);

            if (IsWellKnownType<T>() && equalityComparer == EqualityComparer<T>.Default)
            {
                return DefaultEqualsWellKnown(x, y);
            }

            return equalityComparer.Equals(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastGetHashCode<T>(this IEqualityComparer<T> equalityComparer, T obj)
        {
            Debug.Assert(equalityComparer != null);

            // TODO: For .NET Framework, Enum.GetHashCode() boxes. Check default(T) == null?
            // (Nullable enums will still box?)
            if (equalityComparer == EqualityComparer<T>.Default)
            {
                return obj?.GetHashCode() ?? 0;
            }

            return equalityComparer.GetHashCode(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsWellKnownType<T>()
        {
            if (default(T) != null)
            {
                return
                    typeof(T) == typeof(byte) ||
                    typeof(T) == typeof(sbyte) ||
                    typeof(T) == typeof(short) ||
                    typeof(T) == typeof(ushort) ||
                    typeof(T) == typeof(int) ||
                    typeof(T) == typeof(uint) ||
                    typeof(T) == typeof(long) ||
                    typeof(T) == typeof(ulong) ||
                    typeof(T) == typeof(IntPtr) ||
                    typeof(T) == typeof(UIntPtr);
            }
            else
            {
                return typeof(T) == typeof(string);
            }
        }
    }
}
