// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Unicode
{
    /// <summary>
    /// Contains helpers for dealing with Unicode code points.
    /// </summary>
    internal static unsafe partial class UnicodeHelpers
    {
        /// <summary>
        /// Used for invalid Unicode sequences or other unrepresentable values.
        /// </summary>
        private const char UNICODE_REPLACEMENT_CHAR = '\uFFFD';

        /// <summary>
        /// The last code point defined by the Unicode specification.
        /// </summary>
        internal const int UNICODE_LAST_CODEPOINT = 0x10FFFF;

        // This field is only used on big-endian architectures. We don't
        // bother computing it on little-endian architectures.
        private static uint[] _definedCharacterBitmapBigEndian = (BitConverter.IsLittleEndian) ? null : CreateDefinedCharacterBitmapMachineEndian();

        private static uint[] CreateDefinedCharacterBitmapMachineEndian()
        {
            Debug.Assert(!BitConverter.IsLittleEndian);

            // We need to convert little-endian to machine-endian.

            ReadOnlySpan<byte> remainingBitmap = DefinedCharsBitmapSpan;
            uint[] bigEndianData = new uint[remainingBitmap.Length / sizeof(uint)];

            for (int i = 0; i < bigEndianData.Length; i++)
            {
                bigEndianData[i] = BinaryPrimitives.ReadUInt32LittleEndian(remainingBitmap);
                remainingBitmap = remainingBitmap.Slice(sizeof(uint));
            }

            return bigEndianData;
        }

        /// <summary>
        /// A copy of the logic in Rune.DecodeFromUtf16.
        /// </summary>
        public static OperationStatus DecodeScalarValueFromUtf16(ReadOnlySpan<char> source, out uint result, out int charsConsumed)
        {
            const char ReplacementChar = '\uFFFD';

            if (!source.IsEmpty)
            {
                // First, check for the common case of a BMP scalar value.
                // If this is correct, return immediately.

                uint firstChar = source[0];
                if (!UnicodeUtility.IsSurrogateCodePoint(firstChar))
                {
                    result = firstChar;
                    charsConsumed = 1;
                    return OperationStatus.Done;
                }

                // First thing we saw was a UTF-16 surrogate code point.
                // Let's optimistically assume for now it's a high surrogate and hope
                // that combining it with the next char yields useful results.

                if (1 < (uint)source.Length)
                {
                    uint secondChar = source[1];
                    if (UnicodeUtility.IsHighSurrogateCodePoint(firstChar) && UnicodeUtility.IsLowSurrogateCodePoint(secondChar))
                    {
                        // Success! Formed a supplementary scalar value.
                        result = UnicodeUtility.GetScalarFromUtf16SurrogatePair(firstChar, secondChar);
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
                else if (!UnicodeUtility.IsHighSurrogateCodePoint(firstChar))
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

        /// <summary>
        /// A copy of the logic in Rune.DecodeFromUtf8.
        /// </summary>
        public static OperationStatus DecodeScalarValueFromUtf8(ReadOnlySpan<byte> source, out uint result, out int bytesConsumed)
        {
            const char ReplacementChar = '\uFFFD';

            // This method follows the Unicode Standard's recommendation for detecting
            // the maximal subpart of an ill-formed subsequence. See The Unicode Standard,
            // Ch. 3.9 for more details. In summary, when reporting an invalid subsequence,
            // it tries to consume as many code units as possible as long as those code
            // units constitute the beginning of a longer well-formed subsequence per Table 3-7.

            int index = 0;

            // Try reading input[0].

            if ((uint)index >= (uint)source.Length)
            {
                goto NeedsMoreData;
            }

            uint tempValue = source[index];
            if (!UnicodeUtility.IsAsciiCodePoint(tempValue))
            {
                goto NotAscii;
            }

        Finish:

            bytesConsumed = index + 1;
            Debug.Assert(1 <= bytesConsumed && bytesConsumed <= 4); // Valid subsequences are always length [1..4]
            result = tempValue;
            return OperationStatus.Done;

        NotAscii:

            // Per Table 3-7, the beginning of a multibyte sequence must be a code unit in
            // the range [C2..F4]. If it's outside of that range, it's either a standalone
            // continuation byte, or it's an overlong two-byte sequence, or it's an out-of-range
            // four-byte sequence.

            if (!UnicodeUtility.IsInRangeInclusive(tempValue, 0xC2, 0xF4))
            {
                goto FirstByteInvalid;
            }

            tempValue = (tempValue - 0xC2) << 6;

            // Try reading input[1].

            index++;
            if ((uint)index >= (uint)source.Length)
            {
                goto NeedsMoreData;
            }

            // Continuation bytes are of the form [10xxxxxx], which means that their two's
            // complement representation is in the range [-65..-128]. This allows us to
            // perform a single comparison to see if a byte is a continuation byte.

            int thisByteSignExtended = (sbyte)source[index];
            if (thisByteSignExtended >= -64)
            {
                goto Invalid;
            }

            tempValue += (uint)thisByteSignExtended;
            tempValue += 0x80; // remove the continuation byte marker
            tempValue += (0xC2 - 0xC0) << 6; // remove the leading byte marker

            if (tempValue < 0x0800)
            {
                Debug.Assert(UnicodeUtility.IsInRangeInclusive(tempValue, 0x0080, 0x07FF));
                goto Finish; // this is a valid 2-byte sequence
            }

            // This appears to be a 3- or 4-byte sequence. Since per Table 3-7 we now have
            // enough information (from just two code units) to detect overlong or surrogate
            // sequences, we need to perform these checks now.

            if (!UnicodeUtility.IsInRangeInclusive(tempValue, ((0xE0 - 0xC0) << 6) + (0xA0 - 0x80), ((0xF4 - 0xC0) << 6) + (0x8F - 0x80)))
            {
                // The first two bytes were not in the range [[E0 A0]..[F4 8F]].
                // This is an overlong 3-byte sequence or an out-of-range 4-byte sequence.
                goto Invalid;
            }

            if (UnicodeUtility.IsInRangeInclusive(tempValue, ((0xED - 0xC0) << 6) + (0xA0 - 0x80), ((0xED - 0xC0) << 6) + (0xBF - 0x80)))
            {
                // This is a UTF-16 surrogate code point, which is invalid in UTF-8.
                goto Invalid;
            }

            if (UnicodeUtility.IsInRangeInclusive(tempValue, ((0xF0 - 0xC0) << 6) + (0x80 - 0x80), ((0xF0 - 0xC0) << 6) + (0x8F - 0x80)))
            {
                // This is an overlong 4-byte sequence.
                goto Invalid;
            }

            // The first two bytes were just fine. We don't need to perform any other checks
            // on the remaining bytes other than to see that they're valid continuation bytes.

            // Try reading input[2].

            index++;
            if ((uint)index >= (uint)source.Length)
            {
                goto NeedsMoreData;
            }

            thisByteSignExtended = (sbyte)source[index];
            if (thisByteSignExtended >= -64)
            {
                goto Invalid; // this byte is not a UTF-8 continuation byte
            }

            tempValue <<= 6;
            tempValue += (uint)thisByteSignExtended;
            tempValue += 0x80; // remove the continuation byte marker
            tempValue -= (0xE0 - 0xC0) << 12; // remove the leading byte marker

            if (tempValue <= 0xFFFF)
            {
                Debug.Assert(UnicodeUtility.IsInRangeInclusive(tempValue, 0x0800, 0xFFFF));
                goto Finish; // this is a valid 3-byte sequence
            }

            // Try reading input[3].

            index++;
            if ((uint)index >= (uint)source.Length)
            {
                goto NeedsMoreData;
            }

            thisByteSignExtended = (sbyte)source[index];
            if (thisByteSignExtended >= -64)
            {
                goto Invalid; // this byte is not a UTF-8 continuation byte
            }

            tempValue <<= 6;
            tempValue += (uint)thisByteSignExtended;
            tempValue += 0x80; // remove the continuation byte marker
            tempValue -= (0xF0 - 0xE0) << 18; // remove the leading byte marker

            UnicodeDebug.AssertIsValidSupplementaryPlaneScalar(tempValue);
            goto Finish; // this is a valid 4-byte sequence

        FirstByteInvalid:

            index = 1; // Invalid subsequences are always at least length 1.

        Invalid:

            Debug.Assert(1 <= index && index <= 3); // Invalid subsequences are always length 1..3
            bytesConsumed = index;
            result = ReplacementChar;
            return OperationStatus.InvalidData;

        NeedsMoreData:

            Debug.Assert(0 <= index && index <= 3); // Incomplete subsequences are always length 0..3
            bytesConsumed = index;
            result = ReplacementChar;
            return OperationStatus.NeedMoreData;
        }

        /// <summary>
        /// Returns a bitmap of all characters which are defined per the checked-in version
        /// of the Unicode specification.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ReadOnlySpan<uint> GetDefinedCharacterBitmap()
        {
            if (BitConverter.IsLittleEndian)
            {
                // Underlying data is a series of 32-bit little-endian values and is guaranteed
                // properly aligned by the compiler, so we know this is a valid cast byte -> uint.

                return MemoryMarshal.Cast<byte, uint>(DefinedCharsBitmapSpan);
            }
            else
            {
                // Static compiled data was little-endian; we had to create a big-endian
                // representation at runtime.

                return _definedCharacterBitmapBigEndian;
            }
        }

        /// <summary>
        /// Given a UTF-16 character stream, reads the next scalar value from the stream.
        /// Set 'endOfString' to true if 'pChar' points to the last character in the stream.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetScalarValueFromUtf16(char first, char? second, out bool wasSurrogatePair)
        {
            if (!char.IsSurrogate(first))
            {
                wasSurrogatePair = false;
                return first;
            }
            return GetScalarValueFromUtf16Slow(first, second, out wasSurrogatePair);
        }

        private static int GetScalarValueFromUtf16Slow(char first, char? second, out bool wasSurrogatePair)
        {
#if DEBUG
            if (!Char.IsSurrogate(first))
            {
                Debug.Assert(false, "This case should've been handled by the fast path.");
                wasSurrogatePair = false;
                return first;
            }
#endif
            if (char.IsHighSurrogate(first))
            {
                if (second != null)
                {
                    if (char.IsLowSurrogate(second.Value))
                    {
                        // valid surrogate pair - extract codepoint
                        wasSurrogatePair = true;
                        return GetScalarValueFromUtf16SurrogatePair(first, second.Value);
                    }
                    else
                    {
                        // unmatched surrogate - substitute
                        wasSurrogatePair = false;
                        return UNICODE_REPLACEMENT_CHAR;
                    }
                }
                else
                {
                    // unmatched surrogate - substitute
                    wasSurrogatePair = false;
                    return UNICODE_REPLACEMENT_CHAR;
                }
            }
            else
            {
                // unmatched surrogate - substitute
                Debug.Assert(char.IsLowSurrogate(first));
                wasSurrogatePair = false;
                return UNICODE_REPLACEMENT_CHAR;
            }
        }

        /// <summary>
        /// Given a UTF-16 character stream, reads the next scalar value from the stream.
        /// Set 'endOfString' to true if 'pChar' points to the last character in the stream.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetScalarValueFromUtf16(char* pChar, bool endOfString)
        {
            // This method is marked as AggressiveInlining to handle the common case of a non-surrogate
            // character. The surrogate case is handled in the slower fallback code path.
            char thisChar = *pChar;
            return (char.IsSurrogate(thisChar)) ? GetScalarValueFromUtf16Slow(pChar, endOfString) : thisChar;
        }

        private static int GetScalarValueFromUtf16Slow(char* pChar, bool endOfString)
        {
            char firstChar = pChar[0];

            if (!char.IsSurrogate(firstChar))
            {
                Debug.Assert(false, "This case should've been handled by the fast path.");
                return firstChar;
            }
            else if (char.IsHighSurrogate(firstChar))
            {
                if (endOfString)
                {
                    // unmatched surrogate - substitute
                    return UNICODE_REPLACEMENT_CHAR;
                }
                else
                {
                    char secondChar = pChar[1];
                    if (char.IsLowSurrogate(secondChar))
                    {
                        // valid surrogate pair - extract codepoint
                        return GetScalarValueFromUtf16SurrogatePair(firstChar, secondChar);
                    }
                    else
                    {
                        // unmatched surrogate - substitute
                        return UNICODE_REPLACEMENT_CHAR;
                    }
                }
            }
            else
            {
                // unmatched surrogate - substitute
                Debug.Assert(char.IsLowSurrogate(firstChar));
                return UNICODE_REPLACEMENT_CHAR;
            }
        }

        private static int GetScalarValueFromUtf16SurrogatePair(char highSurrogate, char lowSurrogate)
        {
            Debug.Assert(char.IsHighSurrogate(highSurrogate));
            Debug.Assert(char.IsLowSurrogate(lowSurrogate));

            // See http://www.unicode.org/versions/Unicode6.2.0/ch03.pdf, Table 3.5 for the
            // details of this conversion. We don't use Char.ConvertToUtf32 because its exception
            // handling shows up on the hot path, and our caller has already sanitized the inputs.
            return (lowSurrogate & 0x3ff) | (((highSurrogate & 0x3ff) + (1 << 6)) << 10);
        }

        internal static void GetUtf16SurrogatePairFromAstralScalarValue(int scalar, out char highSurrogate, out char lowSurrogate)
        {
            Debug.Assert(0x10000 <= scalar && scalar <= UNICODE_LAST_CODEPOINT);

            // See http://www.unicode.org/versions/Unicode6.2.0/ch03.pdf, Table 3.5 for the
            // details of this conversion. We don't use Char.ConvertFromUtf32 because its exception
            // handling shows up on the hot path, it allocates temporary strings (which we don't want),
            // and our caller has already sanitized the inputs.

            int x = scalar & 0xFFFF;
            int u = scalar >> 16;
            int w = u - 1;
            highSurrogate = (char)(0xD800 | (w << 6) | (x >> 10));
            lowSurrogate = (char)(0xDC00 | (x & 0x3FF));
        }

        /// <summary>
        /// Given a Unicode scalar value, returns the UTF-8 representation of the value.
        /// The return value's bytes should be popped from the LSB.
        /// </summary>
        internal static int GetUtf8RepresentationForScalarValue(uint scalar)
        {
            Debug.Assert(scalar <= UNICODE_LAST_CODEPOINT);

            // See http://www.unicode.org/versions/Unicode6.2.0/ch03.pdf, Table 3.6 for the
            // details of this conversion. We don't use UTF8Encoding since we're encoding
            // a scalar code point, not a UTF16 character sequence.
            if (scalar <= 0x7f)
            {
                // one byte used: scalar 00000000 0xxxxxxx -> byte sequence 0xxxxxxx
                byte firstByte = (byte)scalar;
                return firstByte;
            }
            else if (scalar <= 0x7ff)
            {
                // two bytes used: scalar 00000yyy yyxxxxxx -> byte sequence 110yyyyy 10xxxxxx
                byte firstByte = (byte)(0xc0 | (scalar >> 6));
                byte secondByteByte = (byte)(0x80 | (scalar & 0x3f));
                return ((secondByteByte << 8) | firstByte);
            }
            else if (scalar <= 0xffff)
            {
                // three bytes used: scalar zzzzyyyy yyxxxxxx -> byte sequence 1110zzzz 10yyyyyy 10xxxxxx
                byte firstByte = (byte)(0xe0 | (scalar >> 12));
                byte secondByte = (byte)(0x80 | ((scalar >> 6) & 0x3f));
                byte thirdByte = (byte)(0x80 | (scalar & 0x3f));
                return ((((thirdByte << 8) | secondByte) << 8) | firstByte);
            }
            else
            {
                // four bytes used: scalar 000uuuuu zzzzyyyy yyxxxxxx -> byte sequence 11110uuu 10uuzzzz 10yyyyyy 10xxxxxx
                byte firstByte = (byte)(0xf0 | (scalar >> 18));
                byte secondByte = (byte)(0x80 | ((scalar >> 12) & 0x3f));
                byte thirdByte = (byte)(0x80 | ((scalar >> 6) & 0x3f));
                byte fourthByte = (byte)(0x80 | (scalar & 0x3f));
                return ((((((fourthByte << 8) | thirdByte) << 8) | secondByte) << 8) | firstByte);
            }
        }

        /// <summary>
        /// Returns a value stating whether a character is defined per the checked-in version
        /// of the Unicode specification. Certain classes of characters (control chars,
        /// private use, surrogates, some whitespace) are considered "undefined" for
        /// our purposes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCharacterDefined(char c)
        {
            uint codePoint = (uint)c;
            int index = (int)(codePoint >> 5);
            int offset = (int)(codePoint & 0x1FU);
            return ((GetDefinedCharacterBitmap()[index] >> offset) & 0x1U) != 0;
        }

        /// <summary>
        /// Determines whether the given scalar value is in the supplementary plane and thus
        /// requires 2 characters to be represented in UTF-16 (as a surrogate pair).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSupplementaryCodePoint(int scalar)
        {
            return ((scalar & ~((int)char.MaxValue)) != 0);
        }

        /// <summary>
        /// Returns <see langword="true"/> iff <paramref name="value"/> is a UTF-8 continuation byte;
        /// i.e., has binary representation 10xxxxxx, where x is any bit.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsUtf8ContinuationByte(in byte value)
        {
            // This API takes its input as a readonly ref so that the JIT can emit "cmp ModRM" statements
            // directly rather than bounce a temporary through a register. That is, we want the JIT to be
            // able to emit a single "cmp byte ptr [data], C0h" statement if we're querying a memory location
            // to see if it's a continuation byte. Data that's already enregistered will go through the
            // normal "cmp reg, C0h" code paths, perhaps with some extra unnecessary "movzx" instructions.
            //
            // The below check takes advantage of the two's complement representation of negative numbers.
            // [ 0b1000_0000, 0b1011_1111 ] is [ -127 (sbyte.MinValue), -65 ]

            return ((sbyte)value < -64);
        }
    }
}
