// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices; // Do not remove. This is necessary for netstandard, since this file is mirrored into corefx

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

#if !netstandard11
using System.Numerics;
#endif

namespace System
{
    internal static partial class SpanHelpers
    {
        public static int IndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength)
            where T : IEquatable<T>
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            T valueHead = value;
            ref T valueTail = ref Unsafe.Add(ref value, 1);
            int valueTailLength = valueLength - 1;

            int index = 0;
            for (; ; )
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
            where T : IEquatable<T>
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

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
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

        public static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, int length)
            where T : IEquatable<T>
        {
            Debug.Assert(length >= 0);

            T lookUp;
            int index = 0;
            while ((length - index) >= 8)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;
                lookUp = Unsafe.Add(ref searchSpace, index + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, index + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, index + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, index + 4);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found4;
                lookUp = Unsafe.Add(ref searchSpace, index + 5);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found5;
                lookUp = Unsafe.Add(ref searchSpace, index + 6);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found6;
                lookUp = Unsafe.Add(ref searchSpace, index + 7);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found7;

                index += 8;
            }

            if ((length - index) >= 4)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;
                lookUp = Unsafe.Add(ref searchSpace, index + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, index + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, index + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found3;

                index += 4;
            }

            while (index < length)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;

                index++;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return index;
        Found1:
            return index + 1;
        Found2:
            return index + 2;
        Found3:
            return index + 3;
        Found4:
            return index + 4;
        Found5:
            return index + 5;
        Found6:
            return index + 6;
        Found7:
            return index + 7;
        }

        public static int IndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length)
            where T : IEquatable<T>
        {
            Debug.Assert(length >= 0);

            T lookUp;
            int index = 0;
            while ((length - index) >= 8)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;
                lookUp = Unsafe.Add(ref searchSpace, index + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, index + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, index + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, index + 4);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found4;
                lookUp = Unsafe.Add(ref searchSpace, index + 5);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found5;
                lookUp = Unsafe.Add(ref searchSpace, index + 6);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found6;
                lookUp = Unsafe.Add(ref searchSpace, index + 7);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found7;

                index += 8;
            }

            if ((length - index) >= 4)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;
                lookUp = Unsafe.Add(ref searchSpace, index + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, index + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, index + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found3;

                index += 4;
            }

            while (index < length)
            {
                lookUp = Unsafe.Add(ref searchSpace, index);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;

                index++;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return index;
        Found1:
            return index + 1;
        Found2:
            return index + 2;
        Found3:
            return index + 3;
        Found4:
            return index + 4;
        Found5:
            return index + 5;
        Found6:
            return index + 6;
        Found7:
            return index + 7;
        }

        public static int IndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength)
            where T : IEquatable<T>
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            int index = -1;
            for (int i = 0; i < valueLength; i++)
            {
                var tempIndex = IndexOf(ref searchSpace, Unsafe.Add(ref value, i), searchSpaceLength);
                if ((uint)tempIndex < (uint)index)
                {
                    index = tempIndex;
                    // Reduce space for search, cause we don't care if we find the search value after the index of a previously found value
                    searchSpaceLength = tempIndex;

                    if (index == 0)
                        break;
                }
            }
            return index;
        }

        public static int LastIndexOf<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength)
            where T : IEquatable<T>
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            T valueHead = value;
            ref T valueTail = ref Unsafe.Add(ref value, 1);
            int valueTailLength = valueLength - 1;

            int index = 0;
            for (; ; )
            {
                Debug.Assert(0 <= index && index <= searchSpaceLength); // Ensures no deceptive underflows in the computation of "remainingSearchSpaceLength".
                int remainingSearchSpaceLength = searchSpaceLength - index - valueTailLength;
                if (remainingSearchSpaceLength <= 0)
                    break;  // The unsearched portion is now shorter than the sequence we're looking for. So it can't be there.

                // Do a quick search for the first element of "value".
                int relativeIndex = LastIndexOf(ref searchSpace, valueHead, remainingSearchSpaceLength);
                if (relativeIndex == -1)
                    break;

                // Found the first element of "value". See if the tail matches.
                if (SequenceEqual(ref Unsafe.Add(ref searchSpace, relativeIndex + 1), ref valueTail, valueTailLength))
                    return relativeIndex;  // The tail matched. Return a successful find.

                index += remainingSearchSpaceLength - relativeIndex;
            }
            return -1;
        }

        public static int LastIndexOf<T>(ref T searchSpace, T value, int length)
            where T : IEquatable<T>
        {
            Debug.Assert(length >= 0);

            while (length >= 8)
            {
                length -= 8;

                if (value.Equals(Unsafe.Add(ref searchSpace, length + 7)))
                    goto Found7;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 6)))
                    goto Found6;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 5)))
                    goto Found5;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 4)))
                    goto Found4;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 3)))
                    goto Found3;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 2)))
                    goto Found2;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 1)))
                    goto Found1;
                if (value.Equals(Unsafe.Add(ref searchSpace, length)))
                    goto Found;
            }

            if (length >= 4)
            {
                length -= 4;

                if (value.Equals(Unsafe.Add(ref searchSpace, length + 3)))
                    goto Found3;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 2)))
                    goto Found2;
                if (value.Equals(Unsafe.Add(ref searchSpace, length + 1)))
                    goto Found1;
                if (value.Equals(Unsafe.Add(ref searchSpace, length)))
                    goto Found;
            }

            while (length > 0)
            {
                length--;

                if (value.Equals(Unsafe.Add(ref searchSpace, length)))
                    goto Found;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return length;
        Found1:
            return length + 1;
        Found2:
            return length + 2;
        Found3:
            return length + 3;
        Found4:
            return length + 4;
        Found5:
            return length + 5;
        Found6:
            return length + 6;
        Found7:
            return length + 7;
        }

        public static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, int length)
            where T : IEquatable<T>
        {
            Debug.Assert(length >= 0);

            T lookUp;
            while (length >= 8)
            {
                length -= 8;

                lookUp = Unsafe.Add(ref searchSpace, length + 7);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found7;
                lookUp = Unsafe.Add(ref searchSpace, length + 6);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found6;
                lookUp = Unsafe.Add(ref searchSpace, length + 5);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found5;
                lookUp = Unsafe.Add(ref searchSpace, length + 4);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found4;
                lookUp = Unsafe.Add(ref searchSpace, length + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, length + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, length + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;
            }

            if (length >= 4)
            {
                length -= 4;

                lookUp = Unsafe.Add(ref searchSpace, length + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, length + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, length + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;
            }

            while (length > 0)
            {
                length--;

                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp))
                    goto Found;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return length;
        Found1:
            return length + 1;
        Found2:
            return length + 2;
        Found3:
            return length + 3;
        Found4:
            return length + 4;
        Found5:
            return length + 5;
        Found6:
            return length + 6;
        Found7:
            return length + 7;
        }

        public static int LastIndexOfAny<T>(ref T searchSpace, T value0, T value1, T value2, int length)
            where T : IEquatable<T>
        {
            Debug.Assert(length >= 0);

            T lookUp;
            while (length >= 8)
            {
                length -= 8;

                lookUp = Unsafe.Add(ref searchSpace, length + 7);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found7;
                lookUp = Unsafe.Add(ref searchSpace, length + 6);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found6;
                lookUp = Unsafe.Add(ref searchSpace, length + 5);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found5;
                lookUp = Unsafe.Add(ref searchSpace, length + 4);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found4;
                lookUp = Unsafe.Add(ref searchSpace, length + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, length + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, length + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;
            }

            if (length >= 4)
            {
                length -= 4;

                lookUp = Unsafe.Add(ref searchSpace, length + 3);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found3;
                lookUp = Unsafe.Add(ref searchSpace, length + 2);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found2;
                lookUp = Unsafe.Add(ref searchSpace, length + 1);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found1;
                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;
            }

            while (length > 0)
            {
                length--;

                lookUp = Unsafe.Add(ref searchSpace, length);
                if (value0.Equals(lookUp) || value1.Equals(lookUp) || value2.Equals(lookUp))
                    goto Found;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return length;
        Found1:
            return length + 1;
        Found2:
            return length + 2;
        Found3:
            return length + 3;
        Found4:
            return length + 4;
        Found5:
            return length + 5;
        Found6:
            return length + 6;
        Found7:
            return length + 7;
        }

        public static int LastIndexOfAny<T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength)
            where T : IEquatable<T>
        {
            Debug.Assert(searchSpaceLength >= 0);
            Debug.Assert(valueLength >= 0);

            if (valueLength == 0)
                return 0;  // A zero-length sequence is always treated as "found" at the start of the search space.

            int index = -1;
            for (int i = 0; i < valueLength; i++)
            {
                var tempIndex = LastIndexOf(ref searchSpace, Unsafe.Add(ref value, i), searchSpaceLength);
                if (tempIndex > index)
                    index = tempIndex;
            }
            return index;
        }

        public static bool SequenceEqual<T>(ref T first, ref T second, int length)
            where T : IEquatable<T>
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

        NotEqual: // Workaround for https://github.com/dotnet/coreclr/issues/13549
            return false;
        }

        public static int SequenceCompareTo<T>(ref T first, int firstLength, ref T second, int secondLength)
            where T : IComparable<T>
        {
            Debug.Assert(firstLength >= 0);
            Debug.Assert(secondLength >= 0);

            var minLength = firstLength;
            if (minLength > secondLength)
                minLength = secondLength;
            for (int i = 0; i < minLength; i++)
            {
                int result = Unsafe.Add(ref first, i).CompareTo(Unsafe.Add(ref second, i));
                if (result != 0)
                    return result;
            }
            return firstLength.CompareTo(secondLength);
        }
    }
}
