// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    internal static partial class SpanHelpers
    {
        public static unsafe void CopyCore<T>(ref T dst, int dstLength, ref T src, int srcLength)
        {
            IntPtr srcMinusDst = Unsafe.ByteOffset<T>(ref dst, ref src);
            bool srcGreaterThanDst = (sizeof(IntPtr) == sizeof(int)) ? srcMinusDst.ToInt32() >= 0 : srcMinusDst.ToInt64() >= 0;
            IntPtr tailDiff;

            if (srcGreaterThanDst)
            {
                // If the start of source is greater than the start of destination, then we need to calculate
                // the different between the end of destination relative to the start of source.
                tailDiff = Unsafe.ByteOffset<T>(ref Unsafe.Add<T>(ref dst, dstLength), ref src);
            }
            else
            {
                // If the start of source is less than the start of destination, then we need to calculate
                // the different between the end of source relative to the start of destunation.
                tailDiff = Unsafe.ByteOffset<T>(ref Unsafe.Add<T>(ref src, srcLength), ref dst);
            }

            // If the source is entirely before or entirely after the destination and the type inside the span is not
            // itself a reference type or containing reference types, then we can do a simple block copy of the data.
            bool isOverlapped = (sizeof(IntPtr) == sizeof(int)) ? tailDiff.ToInt32() < 0 : tailDiff.ToInt64() < 0;
            if (!isOverlapped && !SpanHelpers.IsReferenceOrContainsReferences<T>())
            {
                ref byte dstBytes = ref Unsafe.As<T, byte>(ref dst);
                ref byte srcBytes = ref Unsafe.As<T, byte>(ref src);
                ulong byteCount = (ulong)length * (ulong)Unsafe.SizeOf<T>();
                ulong index = 0;

                while (index < byteCount)
                {
                    uint blockSize = byteCount > uint.MaxValue ? uint.MaxValue : (uint)byteCount;
                    Unsafe.CopyBlock(
                        ref Unsafe.Add(ref dstBytes, (IntPtr)index),
                        ref Unsafe.Add(ref srcBytes, (IntPtr)index),
                        blockSize);
                    index += blockSize;
                }
            }
            else
            {
                if (srcGreaterThanDst)
                {
                    // Source address greater than or equal to destination address. Can do normal copy.
                    for (int i = 0; i < srcLength; i++)
                    {
                        Unsafe.Add<T>(ref dst, i) = Unsafe.Add<T>(ref src, i);
                    }
                }
                else
                {
                    // Source address less than destination address. Must do backward copy.
                    int i = srcLength;
                    while (i-- != 0)
                    {
                        Unsafe.Add<T>(ref dst, i) = Unsafe.Add<T>(ref src, i);
                    }
                }
            }
        }

        public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength)
            where T : struct, IEquatable<T>
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            T valueHead = value;
            ref T valueTail = ref Unsafe.Add(ref value, 1);
            int valueTailLength = valueLength - 1;

            int index = 0;
            for (;;)
            {
                Debug.Assert(0 <= index && index <= searchSpaceLength); // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
                int remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;
                if (remainingSearchSpaceLength <= 0)
                    break;  // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.

                // Do a quick search for the first element of "value".
                int relativeIndex = IndexOf(ref Unsafe.Add(ref searchSpace, index), valueHead, remainingSearchSpaceLength);
                if (relativeIndex == -1)
                    break;
                index += relativeIndex;

                // Found the first element of "value". See if the tail matches.
                if (SequenceEqual(ref Unsafe.Add(ref searchSpace, index + 1), ref valueTail, valueTailLength))
                    return index;  // The tail matched. Return a successful find.

                index++;
            }
            return -1;
        }

        public static unsafe int IndexOf<T>(ref T searchSpace, T value, int length)
            where T : struct, IEquatable<T>
        {
            Debug.Assert(length >= 0);

            IntPtr index = (IntPtr)0; // Use IntPtr for arithmetic to avoid unnecessary 64->32->64 truncations
            while (length >= 8)
            {
                length -= 8;

                if (value.Equals(Unsafe.Add(ref searchSpace, index)))
                    goto Found;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 1)))
                    goto Found1;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 2)))
                    goto Found2;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 3)))
                    goto Found3;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 4)))
                    goto Found4;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 5)))
                    goto Found5;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 6)))
                    goto Found6;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 7)))
                    goto Found7;

                index += 8;
            }

            if (length >= 4)
            {
                length -= 4;

                if (value.Equals(Unsafe.Add(ref searchSpace, index)))
                    goto Found;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 1)))
                    goto Found1;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 2)))
                    goto Found2;
                if (value.Equals(Unsafe.Add(ref searchSpace, index + 3)))
                    goto Found3;

                index += 4;
            }

            while (length > 0)
            {
                if (value.Equals(Unsafe.Add(ref searchSpace, index)))
                    goto Found;

                index += 1;
                length--;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/9692
            return (int)(byte*)index;
        Found1:
            return (int)(byte*)(index + 1);
        Found2:
            return (int)(byte*)(index + 2);
        Found3:
            return (int)(byte*)(index + 3);
        Found4:
            return (int)(byte*)(index + 4);
        Found5:
            return (int)(byte*)(index + 5);
        Found6:
            return (int)(byte*)(index + 6);
        Found7:
            return (int)(byte*)(index + 7);
        }

        public static bool SequenceEqual<T>(ref T first, ref T second, int length)
            where T : struct, IEquatable<T>
        {
            Debug.Assert(length >= 0);

            if (Unsafe.AreSame(ref first, ref second))
                goto Equal;

            IntPtr index = (IntPtr)0; // Use IntPtr for arithmetic to avoid unnecessary 64->32->64 truncations
            while (length >= 8)
            {
                length -= 8;

                if (!Unsafe.Add(ref first, index).Equals(Unsafe.Add(ref second, index)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 1).Equals(Unsafe.Add(ref second, index + 1)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 2).Equals(Unsafe.Add(ref second, index + 2)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 3).Equals(Unsafe.Add(ref second, index + 3)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 4).Equals(Unsafe.Add(ref second, index + 4)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 5).Equals(Unsafe.Add(ref second, index + 5)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 6).Equals(Unsafe.Add(ref second, index + 6)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 7).Equals(Unsafe.Add(ref second, index + 7)))
                    goto NotEqual;

                index += 8;
            }

            if (length >= 4)
            {
                length -= 4;

                if (!Unsafe.Add(ref first, index).Equals(Unsafe.Add(ref second, index)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 1).Equals(Unsafe.Add(ref second, index + 1)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 2).Equals(Unsafe.Add(ref second, index + 2)))
                    goto NotEqual;
                if (!Unsafe.Add(ref first, index + 3).Equals(Unsafe.Add(ref second, index + 3)))
                    goto NotEqual;

                index += 4;
            }

            while (length > 0)
            {
                if (!Unsafe.Add(ref first, index).Equals(Unsafe.Add(ref second, index)))
                    goto NotEqual;
                index += 1;
                length--;
            }

        Equal:
            return true;

        NotEqual: // Workaround for https://github.com/dotnet/coreclr/issues/9692
            return false;
        }
    }
}
