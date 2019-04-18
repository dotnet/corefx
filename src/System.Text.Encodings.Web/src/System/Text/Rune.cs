// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text
{
    // In netstandard builds, we cannot use the actual type System.Text.Rune (it's netcoreapp3.0 only).
    // This is a substitute type that has only the very specific capabilities we need for the encoding
    // routines. It's internal so that we don't accidentally expose it via our public surface area.
    internal readonly struct Rune
    {
        internal static readonly Rune ReplacementChar = new Rune(UnicodeUtility.ReplacementChar);

        internal readonly int Value;

        internal Rune(uint value)
        {
            Debug.Assert(value <= 0x10FFFF, "Rune cannot contain a non-scalar value.");
            Debug.Assert(value < 0xD800 || value > 0xDFFF, "Rune cannot contain a non-scalar value.");

            Value = (int)value;
        }

        internal bool IsAscii => UnicodeUtility.IsAsciiCodePoint((uint)Value);

        internal bool IsBmp => UnicodeUtility.IsBmpCodePoint((uint)Value);

        internal static OperationStatus DecodeFromUtf16(ReadOnlySpan<char> source, out Rune result, out int charsConsumed)
        {
            if (!source.IsEmpty)
            {
                // First, check for the common case of a BMP scalar value.
                // If this is correct, return immediately.

                char firstChar = source[0];
                if (TryCreate(firstChar, out result))
                {
                    charsConsumed = 1;
                    return OperationStatus.Done;
                }

                // First thing we saw was a UTF-16 surrogate code point.
                // Let's optimistically assume for now it's a high surrogate and hope
                // that combining it with the next char yields useful results.

                if (1 < (uint)source.Length)
                {
                    char secondChar = source[1];
                    if (TryCreate(firstChar, secondChar, out result))
                    {
                        // Success! Formed a supplementary scalar value.
                        charsConsumed = 2;
                        return OperationStatus.Done;
                    }
                    else
                    {
                        // Either the first character was a low surrogate, or the second
                        // character was not a low surrogate. This is an error.
                        goto InvalidData;
                    }
                }
                else if (!char.IsHighSurrogate(firstChar))
                {
                    // Quick check to make sure we're not going to report NeedMoreData for
                    // a single-element buffer where the data is a standalone low surrogate
                    // character. Since no additional data will ever make this valid, we'll
                    // report an error immediately.
                    goto InvalidData;
                }
            }

            // If we got to this point, the input buffer was empty, or the buffer
            // was a single element in length and that element was a high surrogate char.

            charsConsumed = source.Length;
            result = ReplacementChar;
            return OperationStatus.NeedMoreData;

        InvalidData:

            charsConsumed = 1; // maximal invalid subsequence for UTF-16 is always a single code unit in length
            result = ReplacementChar;
            return OperationStatus.InvalidData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int EncodeToUtf16(Span<char> destination)
        {
            if (!TryEncodeToUtf16(destination, out int charsWritten))
            {
                ThrowDestinationTooShort();
            }

            return charsWritten;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int EncodeToUtf8(Span<byte> destination)
        {
            if (!TryEncodeToUtf8(destination, out int bytesWritten))
            {
                ThrowDestinationTooShort();
            }

            return bytesWritten;
        }

        private static void ThrowDestinationTooShort()
        {
            // This code path should never get hit in practice because the callers always ensure
            // the destination buffer is large enough. So we won't bother creating a proper
            // localized resource string.
            throw new ArgumentException(new ArgumentException().Message, "destination");
        }

        internal static bool TryCreate(char ch, out Rune result)
        {
            uint extendedValue = ch;
            if (!UnicodeUtility.IsSurrogateCodePoint(extendedValue))
            {
                result = new Rune(extendedValue);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        internal static bool TryCreate(char highSurrogate, char lowSurrogate, out Rune result)
        {
            // First, extend both to 32 bits, then calculate the offset of
            // each candidate surrogate char from the start of its range.

            uint highSurrogateOffset = (uint)highSurrogate - 0xD800;
            uint lowSurrogateOffset = (uint)lowSurrogate - 0xDC00;

            // This is a single comparison which allows us to check both for validity at once since
            // both the high surrogate range and the low surrogate range are the same length.
            // If the comparison fails, we call to a helper method to throw the correct exception message.

            if ((highSurrogateOffset | lowSurrogateOffset) <= 0x03FF)
            {
                // The 0x40u << 10 below is to account for uuuuu = wwww + 1 in the surrogate encoding.
                result = new Rune((highSurrogateOffset << 10) + ((uint)lowSurrogate - 0xDC00) + (0x40u << 10));
                return true;
            }
            else
            {
                // Didn't have a high surrogate followed by a low surrogate.
                result = default;
                return false;
            }
        }

        internal bool TryEncodeToUtf16(Span<char> destination, out int charsWritten)
        {
            if (destination.Length >= 1)
            {
                if (IsBmp)
                {
                    destination[0] = (char)Value;
                    charsWritten = 1;
                    return true;
                }
                else if (destination.Length >= 2)
                {
                    UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar((uint)Value, out destination[0], out destination[1]);
                    charsWritten = 2;
                    return true;
                }
            }

            // Destination buffer not large enough

            charsWritten = default;
            return false;
        }

        internal bool TryEncodeToUtf8(Span<byte> destination, out int bytesWritten)
        {
            // The bit patterns below come from the Unicode Standard, Table 3-6.

            if (destination.Length >= 1)
            {
                if (IsAscii)
                {
                    destination[0] = (byte)Value;
                    bytesWritten = 1;
                    return true;
                }

                if (destination.Length >= 2)
                {
                    if ((uint)Value <= 0x7FFu)
                    {
                        // Scalar 00000yyy yyxxxxxx -> bytes [ 110yyyyy 10xxxxxx ]
                        destination[0] = (byte)(((uint)Value + (0b110u << 11)) >> 6);
                        destination[1] = (byte)(((uint)Value & 0x3Fu) + 0x80u);
                        bytesWritten = 2;
                        return true;
                    }

                    if (destination.Length >= 3)
                    {
                        if ((uint)Value <= 0xFFFFu)
                        {
                            // Scalar zzzzyyyy yyxxxxxx -> bytes [ 1110zzzz 10yyyyyy 10xxxxxx ]
                            destination[0] = (byte)(((uint)Value + (0b1110 << 16)) >> 12);
                            destination[1] = (byte)((((uint)Value & (0x3Fu << 6)) >> 6) + 0x80u);
                            destination[2] = (byte)(((uint)Value & 0x3Fu) + 0x80u);
                            bytesWritten = 3;
                            return true;
                        }

                        if (destination.Length >= 4)
                        {
                            // Scalar 000uuuuu zzzzyyyy yyxxxxxx -> bytes [ 11110uuu 10uuzzzz 10yyyyyy 10xxxxxx ]
                            destination[0] = (byte)(((uint)Value + (0b11110 << 21)) >> 18);
                            destination[1] = (byte)((((uint)Value & (0x3Fu << 12)) >> 12) + 0x80u);
                            destination[2] = (byte)((((uint)Value & (0x3Fu << 6)) >> 6) + 0x80u);
                            destination[3] = (byte)(((uint)Value & 0x3Fu) + 0x80u);
                            bytesWritten = 4;
                            return true;
                        }
                    }
                }
            }

            // Destination buffer not large enough

            bytesWritten = default;
            return false;
        }
    }
}
