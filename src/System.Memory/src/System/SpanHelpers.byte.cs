// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System
{
    internal static partial class SpanHelpers
    {
        public static int IndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            byte valueHead = value;
            ref byte valueTail = ref Unsafe.Add(ref value, 1);
            int valueTailLength = valueLength - 1;

            int index = 0;
            for (;;)
            {
                Debug.Assert(0 <= index && index <= searchSpaceLength); // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
                int remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;
                if (remainingSearchSpaceLength <= 0)
                    return -1;  // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.

                // Do a quick search for the first element of "value".
                int relativeIndex;
                if (!BitConverter.IsLittleEndian)
                {
                    relativeIndex = IndexOfBigEndian(ref Unsafe.Add(ref searchSpace, index), valueHead, remainingSearchSpaceLength);
                }
                else
                {
                    relativeIndex = IndexOf(ref Unsafe.Add(ref searchSpace, index), valueHead, remainingSearchSpaceLength);
                }

                if (relativeIndex == -1)
                    return -1;
                index += relativeIndex;

                // Found the first element of "value". See if the tail matches.
                if (SequenceEqual(ref Unsafe.Add(ref searchSpace, index + 1), ref valueTail, valueTailLength))
                    return index;  // The tail matched. Return a successful find.

                index++;
            }
        }

        internal static int IndexOfBigEndian(ref byte searchSpace, byte value, int length)
        {
            Debug.Assert(length >= 0);

            int index = -1;
            int remainingLength = length;
            while (remainingLength >= 8)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;

                remainingLength -= 8;
            }

            while (remainingLength >= 4)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;

                remainingLength -= 4;
            }

            while (remainingLength > 0)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    return index;

                remainingLength--;
            }
            return -1;
        }

        public unsafe static bool SequenceEqual(ref byte first, ref byte second, int length)
        {
            Debug.Assert(length >= 0);
            var isMatch = true;

            if (length == 0)
            {
                goto exit;
            }

            fixed (byte* pFirst = &first)
            fixed (byte* pSecond = &second)
            {
                var a = pFirst;
                var b = pSecond;

                if (a == b)
                {
                    goto exitFixed;
                }

                var i = 0;
                if (Vector.IsHardwareAccelerated)
                {
                    while (length - Vector<byte>.Count >= i)
                    {
                        var v0 = Unsafe.Read<Vector<byte>>(a + i);
                        var v1 = Unsafe.Read<Vector<byte>>(b + i);
                        i += Vector<byte>.Count;

                        if (!v0.Equals(v1))
                        {
                            isMatch = false;
                            goto exitFixed;
                        }

                    }
                }

                while (length - sizeof(long) >= i)
                {
                    if(*(long*)(a + i) != *(long*)(b + i))
                    {
                        isMatch = false;
                        goto exitFixed;
                    }

                    i += sizeof(long);
                }

                if (length - sizeof(int) >= i)
                {
                    if (*(int*)(a + i) != *(int*)(b + i))
                    {
                        isMatch = false;
                        goto exitFixed;
                    }

                    i += sizeof(int);
                }

                if (length - sizeof(short) >= i)
                {
                    if (*(short*)(a + i) != *(short*)(b + i))
                    {
                        isMatch = false;
                        goto exitFixed;
                    }

                    i += sizeof(short);
                }

                if (length > i && *(a + i) != *(b + i))
                {
                    isMatch = false;
                }
        // Don't goto out of fixed block
        exitFixed:;
            }
        exit:
            return isMatch;
        }

        public unsafe static int IndexOf(ref byte searchSpace, byte value, int length)
        {
            Debug.Assert(length >= 0);
            var index = -1;
            if (length == 0)
            {
                goto exit;
            }

            fixed (byte* pHaystack = &searchSpace)
            {
                var haystack = pHaystack;
                index = 0;

                if (Vector.IsHardwareAccelerated)
                {
                    if (length - Vector<byte>.Count >= index)
                    {
                        Vector<byte> needles = GetVector(value);
                        do
                        {
                            var flaggedMatches = Vector.Equals(Unsafe.Read<Vector<byte>>(haystack + index), needles);
                            if (flaggedMatches.Equals(Vector<byte>.Zero))
                            {
                                index += Vector<byte>.Count;
                                continue;
                            }

                            index += LocateFirstFoundByte(flaggedMatches);
                            goto exitFixed;

                        } while (length - Vector<byte>.Count >= index);
                    }
                }

                while (length - sizeof(ulong) >= index)
                {
                    var flaggedMatches = SetLowBitsForByteMatch(*(ulong*)(haystack + index), value);
                    if (flaggedMatches == 0)
                    {
                        index += sizeof(ulong);
                        continue;
                    }

                    index += LocateFirstFoundByte(flaggedMatches);
                    goto exitFixed;
                }

                for (; index < length; index++)
                {
                    if (*(haystack + index) == value)
                    {
                        goto exitFixed;
                    }
                }
                // No Matches
                index = -1;
                // Don't goto out of fixed block
                exitFixed:;
            }
            exit:
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LocateFirstFoundByte(Vector<byte> match)
        {
            var vector64 = Vector.AsVectorUInt64(match);
            ulong candidate = 0;
            var i = 0;
            // Pattern unrolled by jit https://github.com/dotnet/coreclr/pull/8001
            for (; i < Vector<ulong>.Count; i++)
            {
                candidate = vector64[i];
                if (candidate == 0) continue;
                break;
            }

            // Single LEA instruction with jitted const (using function result)
            return i * 8 + LocateFirstFoundByte(candidate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LocateFirstFoundByte(ulong match)
        {
            unchecked
            {
                // Flag least significant power of two bit
                var powerOfTwoFlag = match ^ (match - 1);
                // Shift all powers of two into the high byte and extract
                return (int)((powerOfTwoFlag * xorPowerOfTwoToHighByte) >> 57);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SetLowBitsForByteMatch(ulong potentialMatch, byte search)
        {
            unchecked
            {
                var flaggedValue = potentialMatch ^ (byteBroadcastToUlong * search);
                return (
                        (flaggedValue - byteBroadcastToUlong) &
                        ~(flaggedValue) &
                        filterByteHighBitsInUlong
                       ) >> 7;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<byte> GetVector(byte vectorByte)
        {
#if !NETCOREAPP1_2
            // Vector<byte> .ctor doesn't become an intrinsic due to detection issue
            // However this does cause it to become an intrinsic (with additional multiply and reg->reg copy)
            // https://github.com/dotnet/coreclr/issues/7459#issuecomment-253965670
            return Vector.AsVectorByte(new Vector<uint>(vectorByte * 0x01010101u));
#else
            return new Vector<byte>(vectorByte);
#endif
        }

        private const ulong xorPowerOfTwoToHighByte = (0x07ul       |
                                                       0x06ul << 8  |
                                                       0x05ul << 16 |
                                                       0x04ul << 24 |
                                                       0x03ul << 32 |
                                                       0x02ul << 40 |
                                                       0x01ul << 48) + 1;
        private const ulong byteBroadcastToUlong = ~0UL / byte.MaxValue;
        private const ulong filterByteHighBitsInUlong = (byteBroadcastToUlong >> 1) | (byteBroadcastToUlong << (sizeof(ulong) * 8 - 1));
    }
}
