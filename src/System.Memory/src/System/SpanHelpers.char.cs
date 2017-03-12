// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard10
using System.Numerics;
#endif

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

        public static int IndexOf(ref char searchSpace, char value, int length)
        {
            Debug.Assert(length >= 0);

            int index = -1;
            int remainingLength = length;
            while (remainingLength >= 8)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;

                remainingLength -= 8;
            }

            if (remainingLength >= 4)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;

                remainingLength -= 4;
            }

            while (remainingLength > 0)
            {
                if (value == Unsafe.Add(ref searchSpace, ++index))
                    goto Found;

                remainingLength--;
            }
            return -1;

        Found: // Workaround for https://github.com/dotnet/coreclr/issues/9692
            return index;
        }

        public static unsafe bool SequenceEqual(ref char firstAsChar, ref char secondAsChar, int length)
        {
            Debug.Assert(length >= 0);

            ref byte first = ref Unsafe.As<char, byte>(ref firstAsChar);
            ref byte second = ref Unsafe.As<char, byte>(ref secondAsChar);

            if (Unsafe.AreSame(ref first, ref second))
                goto Equal;

            IntPtr i = (IntPtr)0; // Use IntPtr and byte* for arithmetic to avoid unnecessary 64->32->64 truncations
            IntPtr n = (IntPtr)length + length; // sizeof(char) * length;

#if !netstandard10
            if (Vector.IsHardwareAccelerated && (byte*)n >= (byte*)Vector<byte>.Count)
            {
                n -= Vector<byte>.Count;
                while ((byte*)n > (byte*)i)
                {
                    if (Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref first, i)) !=
                        Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref second, i)))
                    {
                        goto NotEqual;
                    }
                    i += Vector<byte>.Count;
                }
                return Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref first, n)) ==
                       Unsafe.ReadUnaligned<Vector<byte>>(ref Unsafe.AddByteOffset(ref second, n));
            }
#endif

            if ((byte*)n >= (byte*)sizeof(UIntPtr))
            {
                n -= sizeof(UIntPtr);
                while ((byte*)n > (byte*)i)
                {
                    if (Unsafe.ReadUnaligned<UIntPtr>(ref Unsafe.AddByteOffset(ref first, i)) !=
                        Unsafe.ReadUnaligned<UIntPtr>(ref Unsafe.AddByteOffset(ref second, i)))
                    {
                        goto NotEqual;
                    }
                    i += sizeof(UIntPtr);
                }
                return Unsafe.ReadUnaligned<UIntPtr>(ref Unsafe.AddByteOffset(ref first, n)) ==
                       Unsafe.ReadUnaligned<UIntPtr>(ref Unsafe.AddByteOffset(ref second, n));
            }

            while ((byte*)n > (byte*)i)
            {
                if (Unsafe.As<byte, char>(ref Unsafe.AddByteOffset(ref first, i)) !=
                    Unsafe.As<byte, char>(ref Unsafe.AddByteOffset(ref second, i)))
                {
                    goto NotEqual;
                }
                i += sizeof(char);
            }

        Equal:
            return true;

        NotEqual: // Workaround for https://github.com/dotnet/coreclr/issues/9692
            return false;
        }
    }
}
