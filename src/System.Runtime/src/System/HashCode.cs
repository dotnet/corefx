// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System
{
    public unsafe struct HashCode
    {
        private const uint Queue = 4;
        private const uint Length = 7;

        private const uint Prime1 = 2654435761U;
        private const uint Prime2 = 2246822519U;
        private const uint Prime3 = 3266489917U;
        private const uint Prime4 = 0668265263U;
        private const uint Prime5 = 0374761393U;
        
        // 8x8 = 64 bytes = 1 x86 cache line
        // +----+----+----+----+----+----+----+----+
        // | 00 | 01 | 02 | 03 | 04 | 05 | 06 | 07 |
        // | xxHash State      | Queue        | Le |
        // +----+----+----+----+----+----+----+----+
        private fixed uint _state[8];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        private void Add(int value)
        {
            unchecked
            {
                var val = (uint)value;

                fixed (uint* state = _state)
                {
                    var queue = state + Queue;
                    var position = state[Length] & 0x3;

                    if (position < 3)
                    {
                        queue[position] = val;
                    }
                    else
                    {

                        var sentinel =
                            state[0] | state[1] | state[2] | state[3] | // any of accumulators set?
                            (state[Length] ^ position); // length greater than 3?

                        if (sentinel == 0)
                        {
                            // Initialize

                            state[0] = Prime1 + Prime2;
                            state[1] = Prime2;
                            state[2] = 0;
                            state[3] = (uint)-Prime1;
                        }

                        for (var i = 0; i < 3; i++)
                            state[i] = Round(state[i], queue[i]);
                        state[3] = Round(state[3], val);
                    }

                    state[Length]++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public int ToHashCode()
        {
            unchecked
            {
                fixed (uint* state = _state)
                {
                    var queue = state + Queue;
                    var position = state[Length] & 0x3;

                    // Mix the accumulators (if they contain anything)

                    var hash = state[Length] > 3
                        ? Rol(state[0], 1) + Rol(state[1], 7) + Rol(state[2], 12) + Rol(state[3], 18)
                        : Prime5;

                    hash += state[Length] * 4;

                    // Mix what remains in the queue

                    for (var i = 0; i < position; i++)
                    {
                        hash += queue[i] * Prime3;
                        hash = Rol(hash, 17) * Prime4;
                    }

                    // Final mix

                    hash ^= hash >> 15;
                    hash *= Prime2;
                    hash ^= hash >> 13;
                    hash *= Prime3;
                    hash ^= hash >> 16;

                    return (int)hash;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        private uint Round(uint seed, uint input)
        {
            unchecked
            {
                seed += input * Prime2;
                seed = Rol(seed, 13);
                seed *= Prime1;
                return seed;
            }
        }

#       pragma warning disable 0809
        [Obsolete("Use ToHashCode to retrieve the computed hash code.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => ToHashCode();
#       pragma warning restore 0809

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        private static uint Rol(uint value, int count)
            => (value << count) | (value >> (32 - count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public void Add<T>(T value) => Add(value?.GetHashCode() ?? 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public void Add<T>(T value, IEqualityComparer<T> comparer)
        {
            if (comparer is null) throw new ArgumentNullException(nameof(comparer));
            Add(comparer.GetHashCode(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1>(T1 value1)
        {
            var hc = new HashCode();
            hc.Add(value1);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            hc.Add(value4);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            hc.Add(value4);
            hc.Add(value5);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            hc.Add(value4);
            hc.Add(value5);
            hc.Add(value6);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            hc.Add(value4);
            hc.Add(value5);
            hc.Add(value6);
            hc.Add(value7);
            return hc.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical for inlining across NGen images.")]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            var hc = new HashCode();
            hc.Add(value1);
            hc.Add(value2);
            hc.Add(value3);
            hc.Add(value4);
            hc.Add(value5);
            hc.Add(value6);
            hc.Add(value7);
            hc.Add(value8);
            return hc.ToHashCode();
        }
    }
}
