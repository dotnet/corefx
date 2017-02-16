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
        public static int IndexOf(ref char searchSpace, int searchSpaceLength, ref char value, int valueLength)
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            char valueHead = value;
            ref char valueTail = ref Unsafe.Add(ref value, 1);
            int valueTailLength = valueLength - 1;

            int index = 0;
            for (;;)
            {
                Debug.Assert(0 <= index && index <= searchSpaceLength); // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
                int remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;
                if (remainingSearchSpaceLength <= 0)
                    return -1;  // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.

                // Do a quick search for the first element of "value".
                int relativeIndex = IndexOf(ref Unsafe.Add(ref searchSpace, index), valueHead, remainingSearchSpaceLength);
                if (relativeIndex == -1)
                    return -1;
                index += relativeIndex;

                // Found the first element of "value". See if the tail matches.
                if (SequenceEqual(ref Unsafe.Add(ref searchSpace, index + 1), ref valueTail, valueTailLength))
                    return index;  // The tail matched. Return a successful find.

                index++;
            }
        }

        public static int IndexOf(ref char searchSpace, char value, int length)
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

        public unsafe static bool SequenceEqual(ref char first, ref char second, int length)
        {
            Debug.Assert(length >= 0);

            var isMatch = true;

            if (length == 0)
            {
                goto exit;
            }

            length *= 2;

            fixed (char* pFirst = &first)
            fixed (char* pSecond = &second)
            {
                var a = (byte*)pFirst;
                var b = (byte*)pSecond;

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
                    if (*(long*)(a + i) != *(long*)(b + i))
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

                if (length > i && *(short*)(a + i) != *(short*)(b + i))
                {
                    isMatch = false;
                }
        // Don't goto out of fixed block
        exitFixed:;
            }
        exit:
            return isMatch;
        }
    }
}
