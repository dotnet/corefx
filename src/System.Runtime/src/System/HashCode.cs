// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace System
{
    public struct HashCode
    {
#   if SYSTEM_HASHCODE_TESTVECTORS
        private static readonly uint _seed = 0;
#   else
        private static readonly uint _seed = unchecked((uint)new Random().Next(int.MinValue, int.MaxValue));
#   endif

        private const uint Prime1 = 2654435761U;
        private const uint Prime2 = 2246822519U;
        private const uint Prime3 = 3266489917U;
        private const uint Prime4 = 0668265263U;
        private const uint Prime5 = 0374761393U;

        private uint _v1, _v2, _v3, _v4;
        private uint _queue1, _queue2, _queue3;
        private uint _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1>(T1 value1)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);

                var hash = MixEmptyState();
                hash += 4;

                hash = QueueRound(hash, hc1);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);

                var hash = MixEmptyState();
                hash += 8;

                hash = QueueRound(hash, hc1);
                hash = QueueRound(hash, hc2);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);

                var hash = MixEmptyState();
                hash += 12;

                hash = QueueRound(hash, hc1);
                hash = QueueRound(hash, hc2);
                hash = QueueRound(hash, hc3);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);
                var hc4 = (uint)(value4?.GetHashCode() ?? 0);

                Initialize(out var v1, out var v2, out var v3, out var v4);

                v1 = Round(v1, hc1);
                v2 = Round(v2, hc2);
                v3 = Round(v3, hc3);
                v4 = Round(v4, hc4);

                var hash = MixState(v1, v2, v3, v4);
                hash += 16;

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);
                var hc4 = (uint)(value4?.GetHashCode() ?? 0);
                var hc5 = (uint)(value5?.GetHashCode() ?? 0);

                Initialize(out var v1, out var v2, out var v3, out var v4);

                v1 = Round(v1, hc1);
                v2 = Round(v2, hc2);
                v3 = Round(v3, hc3);
                v4 = Round(v4, hc4);

                var hash = MixState(v1, v2, v3, v4);
                hash += 20;

                hash = QueueRound(hash, hc5);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);
                var hc4 = (uint)(value4?.GetHashCode() ?? 0);
                var hc5 = (uint)(value5?.GetHashCode() ?? 0);
                var hc6 = (uint)(value6?.GetHashCode() ?? 0);

                Initialize(out var v1, out var v2, out var v3, out var v4);

                v1 = Round(v1, hc1);
                v2 = Round(v2, hc2);
                v3 = Round(v3, hc3);
                v4 = Round(v4, hc4);

                var hash = MixState(v1, v2, v3, v4);
                hash += 24;

                hash = QueueRound(hash, hc5);
                hash = QueueRound(hash, hc6);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);
                var hc4 = (uint)(value4?.GetHashCode() ?? 0);
                var hc5 = (uint)(value5?.GetHashCode() ?? 0);
                var hc6 = (uint)(value6?.GetHashCode() ?? 0);
                var hc7 = (uint)(value7?.GetHashCode() ?? 0);

                Initialize(out var v1, out var v2, out var v3, out var v4);

                v1 = Round(v1, hc1);
                v2 = Round(v2, hc2);
                v3 = Round(v3, hc3);
                v4 = Round(v4, hc4);

                var hash = MixState(v1, v2, v3, v4);
                hash += 28;

                hash = QueueRound(hash, hc5);
                hash = QueueRound(hash, hc6);
                hash = QueueRound(hash, hc7);

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
        {
            unchecked
            {
                var hc1 = (uint)(value1?.GetHashCode() ?? 0);
                var hc2 = (uint)(value2?.GetHashCode() ?? 0);
                var hc3 = (uint)(value3?.GetHashCode() ?? 0);
                var hc4 = (uint)(value4?.GetHashCode() ?? 0);
                var hc5 = (uint)(value5?.GetHashCode() ?? 0);
                var hc6 = (uint)(value6?.GetHashCode() ?? 0);
                var hc7 = (uint)(value7?.GetHashCode() ?? 0);
                var hc8 = (uint)(value8?.GetHashCode() ?? 0);

                Initialize(out var v1, out var v2, out var v3, out var v4);

                v1 = Round(v1, hc1);
                v2 = Round(v2, hc2);
                v3 = Round(v3, hc3);
                v4 = Round(v4, hc4);

                v1 = Round(v1, hc5);
                v2 = Round(v2, hc6);
                v3 = Round(v3, hc7);
                v4 = Round(v4, hc8);

                var hash = MixState(v1, v2, v3, v4);
                hash += 32;

                hash = MixFinal(hash);
                return (int)hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Rol(uint value, int count)
            => (value << count) | (value >> (32 - count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
        {
            unchecked
            {
                v1 = _seed + Prime1 + Prime2;
                v2 = _seed + Prime2;
                v3 = _seed + 0;
                v4 = _seed - Prime1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Round(uint hash, uint input)
        {
            unchecked
            {
                hash += input * Prime2;
                hash = Rol(hash, 13);
                hash *= Prime1;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint QueueRound(uint hash, uint queuedValue)
        {
            unchecked
            {
                hash += queuedValue * Prime3;
                return Rol(hash, 17) * Prime4;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixState(uint v1, uint v2, uint v3, uint v4)
        {
            return Rol(v1, 1) + Rol(v2, 7) + Rol(v3, 12) + Rol(v4, 18);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixEmptyState()
        {
            return _seed + Prime5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixFinal(uint hash)
        {
            unchecked
            {
                hash ^= hash >> 15;
                hash *= Prime2;
                hash ^= hash >> 13;
                hash *= Prime3;
                hash ^= hash >> 16;
                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(T value)
        {
            Add(value?.GetHashCode() ?? 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<T>(T value, IEqualityComparer<T> comparer)
        {
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
            Add(comparer.GetHashCode(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(int value)
        {
            unchecked
            {
                var val = (uint)value;
                var position = _length & 0x3;

                switch (position)
                {
                    case 0:
                        _queue1 = val;
                        break;
                    case 1:
                        _queue2 = val;
                        break;
                    case 2:
                        _queue3 = val;
                        break;
                    default:

                        var sentinel =
                            _v1 | _v2 | _v3 | _v4 | // any of accumulators set?
                            (_length ^ position); // length greater than 3?

                        if (sentinel == 0)
                            Initialize(out _v1, out _v2, out _v3, out _v4);

                        _v1 = Round(_v1, _queue1);
                        _v2 = Round(_v2, _queue2);
                        _v3 = Round(_v3, _queue3);
                        _v4 = Round(_v4, val);

                        break;
                }

                _length++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToHashCode()
        {
            unchecked
            {
                var position = _length & 0x3;

                // If the length is less than 3, _v1 -> _v4 don't contain
                // anything yet. xxHash32 treats this differently.

                var hash = (_length ^ position) == 0
                    ? MixEmptyState()
                    : MixState(_v1, _v2, _v3, _v4);

                hash += _length * 4;

                // Mix what remains in the queue

                switch (position)
                {
                    case 1:
                        hash = QueueRound(hash, _queue1);
                        break;

                    case 2:
                        hash = QueueRound(hash, _queue1);
                        hash = QueueRound(hash, _queue2);
                        break;

                    case 3:
                        hash = QueueRound(hash, _queue1);
                        hash = QueueRound(hash, _queue2);
                        hash = QueueRound(hash, _queue3);
                        break;
                }

                // Final mix

                hash = MixFinal(hash);

                return (int)hash;
            }
        }

#pragma warning disable 0809

        [Obsolete("Use ToHashCode to retrieve the computed hash code.", error: true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => throw new NotImplementedException();

#pragma warning restore 0809

    }
}
