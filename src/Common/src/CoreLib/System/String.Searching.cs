// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Globalization;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    public partial class String
    {
        public bool Contains(string value)
        {
            if (value == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.value);

            return SpanHelpers.IndexOf(
                ref _firstChar,
                Length,
                ref value!._firstChar, // TODO-NULLABLE: https://github.com/dotnet/csharplang/issues/538
                value.Length) >= 0;
        }

        public bool Contains(string value, StringComparison comparisonType)
        {
            return (IndexOf(value, comparisonType) >= 0);
        }

        public bool Contains(char value) => SpanHelpers.Contains(ref _firstChar, value, Length);

        public bool Contains(char value, StringComparison comparisonType)
        {
            return IndexOf(value, comparisonType) != -1;
        }

        // Returns the index of the first occurrence of a specified character in the current instance.
        // The search starts at startIndex and runs thorough the next count characters.
        //
        public int IndexOf(char value) => SpanHelpers.IndexOf(ref _firstChar, value, Length);

        public int IndexOf(char value, int startIndex)
        {
            return IndexOf(value, startIndex, this.Length - startIndex);
        }

        public int IndexOf(char value, StringComparison comparisonType)
        {
            switch (comparisonType)
            {
                case StringComparison.CurrentCulture:
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.InvariantCulture:
                case StringComparison.InvariantCultureIgnoreCase:
                    return CompareInfo.Invariant.IndexOf(this, value, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.Ordinal:
                    return IndexOf(value);

                case StringComparison.OrdinalIgnoreCase:
                    return CompareInfo.Invariant.IndexOf(this, value, CompareOptions.OrdinalIgnoreCase);

                default:
                    throw new ArgumentException(SR.NotSupported_StringComparison, nameof(comparisonType));
            }
        }

        public unsafe int IndexOf(char value, int startIndex, int count)
        {
            if ((uint)startIndex > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);

            if ((uint)count > (uint)(Length - startIndex))
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);

            int result = SpanHelpers.IndexOf(ref Unsafe.Add(ref _firstChar, startIndex), value, count);

            return result == -1 ? result : result + startIndex;
        }

        // Returns the index of the first occurrence of any specified character in the current instance.
        // The search starts at startIndex and runs to startIndex + count - 1.
        //
        public int IndexOfAny(char[] anyOf)
        {
            return IndexOfAny(anyOf, 0, this.Length);
        }

        public int IndexOfAny(char[] anyOf, int startIndex)
        {
            return IndexOfAny(anyOf, startIndex, this.Length - startIndex);
        }

        public int IndexOfAny(char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if ((uint)startIndex > (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);

            if ((uint)count > (uint)(Length - startIndex))
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);

            if (anyOf.Length > 0 && anyOf.Length <= 5)
            {
                // The ReadOnlySpan.IndexOfAny extension is vectorized for values of 1 - 5 in length
                var result = new ReadOnlySpan<char>(ref Unsafe.Add(ref _firstChar, startIndex), count).IndexOfAny(anyOf);
                return result == -1 ? result : result + startIndex;
            }
            else if (anyOf.Length > 5)
            {
                // Use Probabilistic Map
                return IndexOfCharArray(anyOf, startIndex, count);
            }
            else // anyOf.Length == 0
            {
                return -1;
            }
        }

        private unsafe int IndexOfCharArray(char[] anyOf, int startIndex, int count)
        {
            // use probabilistic map, see InitializeProbabilisticMap
            ProbabilisticMap map = default;
            uint* charMap = (uint*)&map;

            InitializeProbabilisticMap(charMap, anyOf);

            fixed (char* pChars = &_firstChar)
            {
                char* pCh = pChars + startIndex;

                while (count > 0)
                {
                    int thisChar = *pCh;

                    if (IsCharBitSet(charMap, (byte)thisChar) &&
                        IsCharBitSet(charMap, (byte)(thisChar >> 8)) &&
                        ArrayContains((char)thisChar, anyOf))
                    {
                        return (int)(pCh - pChars);
                    }

                    count--;
                    pCh++;
                }

                return -1;
            }
        }

        private const int PROBABILISTICMAP_BLOCK_INDEX_MASK = 0x7;
        private const int PROBABILISTICMAP_BLOCK_INDEX_SHIFT = 0x3;
        private const int PROBABILISTICMAP_SIZE = 0x8;

        // A probabilistic map is an optimization that is used in IndexOfAny/
        // LastIndexOfAny methods. The idea is to create a bit map of the characters we
        // are searching for and use this map as a "cheap" check to decide if the
        // current character in the string exists in the array of input characters.
        // There are 256 bits in the map, with each character mapped to 2 bits. Every
        // character is divided into 2 bytes, and then every byte is mapped to 1 bit.
        // The character map is an array of 8 integers acting as map blocks. The 3 lsb
        // in each byte in the character is used to index into this map to get the
        // right block, the value of the remaining 5 msb are used as the bit position
        // inside this block. 
        private static unsafe void InitializeProbabilisticMap(uint* charMap, ReadOnlySpan<char> anyOf)
        {
            bool hasAscii = false;
            uint* charMapLocal = charMap; // https://github.com/dotnet/coreclr/issues/14264

            for (int i = 0; i < anyOf.Length; ++i)
            {
                int c = anyOf[i];

                // Map low bit
                SetCharBit(charMapLocal, (byte)c);

                // Map high bit
                c >>= 8;

                if (c == 0)
                {
                    hasAscii = true;
                }
                else
                {
                    SetCharBit(charMapLocal, (byte)c);
                }
            }

            if (hasAscii)
            {
                // Common to search for ASCII symbols. Just set the high value once.
                charMapLocal[0] |= 1u;
            }
        }

        private static bool ArrayContains(char searchChar, char[] anyOf)
        {
            for (int i = 0; i < anyOf.Length; i++)
            {
                if (anyOf[i] == searchChar)
                    return true;
            }

            return false;
        }

        private static unsafe bool IsCharBitSet(uint* charMap, byte value)
        {
            return (charMap[value & PROBABILISTICMAP_BLOCK_INDEX_MASK] & (1u << (value >> PROBABILISTICMAP_BLOCK_INDEX_SHIFT))) != 0;
        }

        private static unsafe void SetCharBit(uint* charMap, byte value)
        {
            charMap[value & PROBABILISTICMAP_BLOCK_INDEX_MASK] |= 1u << (value >> PROBABILISTICMAP_BLOCK_INDEX_SHIFT);
        }

        public int IndexOf(string value)
        {
            return IndexOf(value, StringComparison.CurrentCulture);
        }

        public int IndexOf(string value, int startIndex)
        {
            return IndexOf(value, startIndex, StringComparison.CurrentCulture);
        }

        public int IndexOf(string value, int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > this.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);
            }

            if (count < 0 || count > this.Length - startIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);
            }

            return IndexOf(value, startIndex, count, StringComparison.CurrentCulture);
        }

        public int IndexOf(string value, StringComparison comparisonType)
        {
            return IndexOf(value, 0, this.Length, comparisonType);
        }

        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            return IndexOf(value, startIndex, this.Length - startIndex, comparisonType);
        }

        public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
        {
            // Validate inputs
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (startIndex < 0 || startIndex > this.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);

            if (count < 0 || startIndex > this.Length - count)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);

            if (comparisonType == StringComparison.Ordinal)
            {
                var result = SpanHelpers.IndexOf(
                    ref Unsafe.Add(ref this._firstChar, startIndex),
                    count,
                    ref value._firstChar,
                    value.Length);

                return (result >= 0 ? startIndex : 0) + result;
            }

            switch (comparisonType)
            {
                case StringComparison.CurrentCulture:
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.InvariantCulture:
                case StringComparison.InvariantCultureIgnoreCase:
                    return CompareInfo.Invariant.IndexOf(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.OrdinalIgnoreCase:
                    return CompareInfo.Invariant.IndexOfOrdinal(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType) != CompareOptions.None);

                default:
                    throw new ArgumentException(SR.NotSupported_StringComparison, nameof(comparisonType));
            }
        }

        // Returns the index of the last occurrence of a specified character in the current instance.
        // The search starts at startIndex and runs backwards to startIndex - count + 1.
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        public int LastIndexOf(char value) => SpanHelpers.LastIndexOf(ref _firstChar, value, Length);

        public int LastIndexOf(char value, int startIndex)
        {
            return LastIndexOf(value, startIndex, startIndex + 1);
        }

        public unsafe int LastIndexOf(char value, int startIndex, int count)
        {
            if (Length == 0)
                return -1;

            if ((uint)startIndex >= (uint)Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);

            if ((uint)count > (uint)startIndex + 1)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);

            int startSearchAt = startIndex + 1 - count;
            int result = SpanHelpers.LastIndexOf(ref Unsafe.Add(ref _firstChar, startSearchAt), value, count);

            return result == -1 ? result : result + startSearchAt;
        }

        // Returns the index of the last occurrence of any specified character in the current instance.
        // The search starts at startIndex and runs backwards to startIndex - count + 1.
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        public int LastIndexOfAny(char[] anyOf)
        {
            return LastIndexOfAny(anyOf, this.Length - 1, this.Length);
        }

        public int LastIndexOfAny(char[] anyOf, int startIndex)
        {
            return LastIndexOfAny(anyOf, startIndex, startIndex + 1);
        }

        public unsafe int LastIndexOfAny(char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException(nameof(anyOf));

            if (Length == 0)
                return -1;

            if ((uint)startIndex >= (uint)Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);
            }

            if ((count < 0) || ((count - 1) > startIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);
            }

            if (anyOf.Length > 1)
            {
                return LastIndexOfCharArray(anyOf, startIndex, count);
            }
            else if (anyOf.Length == 1)
            {
                return LastIndexOf(anyOf[0], startIndex, count);
            }
            else // anyOf.Length == 0
            {
                return -1;
            }
        }

        private unsafe int LastIndexOfCharArray(char[] anyOf, int startIndex, int count)
        {
            // use probabilistic map, see InitializeProbabilisticMap
            ProbabilisticMap map = default;
            uint* charMap = (uint*)&map;

            InitializeProbabilisticMap(charMap, anyOf);

            fixed (char* pChars = &_firstChar)
            {
                char* pCh = pChars + startIndex;

                while (count > 0)
                {
                    int thisChar = *pCh;

                    if (IsCharBitSet(charMap, (byte)thisChar) &&
                        IsCharBitSet(charMap, (byte)(thisChar >> 8)) &&
                        ArrayContains((char)thisChar, anyOf))
                    {
                        return (int)(pCh - pChars);
                    }

                    count--;
                    pCh--;
                }

                return -1;
            }
        }

        // Returns the index of the last occurrence of any character in value in the current instance.
        // The search starts at startIndex and runs backwards to startIndex - count + 1.
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        public int LastIndexOf(string value)
        {
            return LastIndexOf(value, this.Length - 1, this.Length, StringComparison.CurrentCulture);
        }

        public int LastIndexOf(string value, int startIndex)
        {
            return LastIndexOf(value, startIndex, startIndex + 1, StringComparison.CurrentCulture);
        }

        public int LastIndexOf(string value, int startIndex, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);
            }

            return LastIndexOf(value, startIndex, count, StringComparison.CurrentCulture);
        }

        public int LastIndexOf(string value, StringComparison comparisonType)
        {
            return LastIndexOf(value, this.Length - 1, this.Length, comparisonType);
        }

        public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            return LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
        }

        public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // Special case for 0 length input strings
            if (this.Length == 0 && (startIndex == -1 || startIndex == 0))
                return (value.Length == 0) ? 0 : -1;

            // Now after handling empty strings, make sure we're not out of range
            if (startIndex < 0 || startIndex > this.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), SR.ArgumentOutOfRange_Index);

            // Make sure that we allow startIndex == this.Length
            if (startIndex == this.Length)
            {
                startIndex--;
                if (count > 0)
                    count--;
            }

            // 2nd half of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_Count);

            // If we are looking for nothing, just return startIndex
            if (value.Length == 0)
                return startIndex;

            switch (comparisonType)
            {
                case StringComparison.CurrentCulture:
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.InvariantCulture:
                case StringComparison.InvariantCultureIgnoreCase:
                    return CompareInfo.Invariant.LastIndexOf(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType));

                case StringComparison.Ordinal:
                case StringComparison.OrdinalIgnoreCase:
                    return CompareInfo.Invariant.LastIndexOfOrdinal(this, value, startIndex, count, GetCaseCompareOfComparisonCulture(comparisonType) != CompareOptions.None);

                default:
                    throw new ArgumentException(SR.NotSupported_StringComparison, nameof(comparisonType));
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = PROBABILISTICMAP_SIZE * sizeof(uint))]
        private struct ProbabilisticMap { }
    }
}
