// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Internal;
using System.Text.Unicode;

#if NETCOREAPP
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace System.Text.Encodings.Web
{
    internal sealed class UnsafeRelaxedJavaScriptEncoder : JavaScriptEncoder
    {
        private readonly AllowedCharactersBitmap _allowedCharacters;

        internal static readonly UnsafeRelaxedJavaScriptEncoder s_singleton = new UnsafeRelaxedJavaScriptEncoder();

        private UnsafeRelaxedJavaScriptEncoder()
        {
            var filter = new TextEncoderSettings(UnicodeRanges.All);

            _allowedCharacters = filter.GetAllowedCharacters();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharacters.ForbidUndefinedCharacters();

            // '"' (U+0022 QUOTATION MARK) must always be escaped in Javascript / ECMAScript / JSON.
            _allowedCharacters.ForbidCharacter('\"'); // can be used to escape attributes

            // '\' (U+005C REVERSE SOLIDUS) must always be escaped in Javascript / ECMAScript / JSON.
            // '/' (U+002F SOLIDUS) is not Javascript / ECMAScript / JSON-sensitive so doesn't need to be escaped.
            _allowedCharacters.ForbidCharacter('\\');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool WillEncode(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar))
            {
                return true;
            }

            Debug.Assert(unicodeScalar >= char.MinValue && unicodeScalar <= char.MaxValue);

            return !_allowedCharacters.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            int idx = 0;

#if NETCOREAPP
            if (Sse2.IsSupported)
            {
                short* startingAddress = (short*)text;
                while (textLength - 8 >= idx)
                {
                    Debug.Assert(startingAddress >= text && startingAddress <= (text + textLength - 8));

                    // Load the next 8 characters.
                    Vector128<short> sourceValue = Sse2.LoadVector128(startingAddress);

                    Vector128<short> mask = Sse2Helper.CreateAsciiMask(sourceValue);
                    int index = Sse2.MoveMask(mask.AsByte());

                    if (index != 0)
                    {
                        // At least one of the following 8 characters is non-ASCII.
                        int processNextEight = idx + 8;
                        Debug.Assert(processNextEight <= textLength);
                        for (; idx < processNextEight; idx++)
                        {
                            Debug.Assert((text + idx) <= (text + textLength));
                            if (!_allowedCharacters.IsCharacterAllowed(*(text + idx)))
                            {
                                goto Return;
                            }
                        }
                        startingAddress += 8;
                    }
                    else
                    {
                        // Check if any of the 8 characters need to be escaped.
                        mask = Sse2Helper.CreateEscapingMask_UnsafeRelaxedJavaScriptEncoder(sourceValue);

                        index = Sse2.MoveMask(mask.AsByte());
                        // If index == 0, that means none of the 8 characters needed to be escaped.
                        // TrailingZeroCount is relatively expensive, avoid it if possible.
                        if (index != 0)
                        {
                            // Found at least one character that needs to be escaped, figure out the index of
                            // the first one found that needed to be escaped within the 8 characters.
                            Debug.Assert(index > 0 && index <= 65_535);
                            int tzc = BitOperations.TrailingZeroCount(index);
                            Debug.Assert(tzc % 2 == 0 && tzc >= 0 && tzc <= 16);
                            idx += tzc >> 1;
                            goto Return;
                        }
                        idx += 8;
                        startingAddress += 8;
                    }
                }

                // Process the remaining characters.
                Debug.Assert(textLength - idx < 8);
            }
#endif

            for (; idx < textLength; idx++)
            {
                Debug.Assert((text + idx) <= (text + textLength));
                if (!_allowedCharacters.IsCharacterAllowed(*(text + idx)))
                {
                    goto Return;
                }
            }

            idx = -1; // All characters are allowed.

        Return:
            return idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe int FindFirstCharacterToEncodeUtf8(ReadOnlySpan<byte> utf8Text)
        {
            fixed (byte* ptr = utf8Text)
            {
                int idx = 0;

#if NETCOREAPP
                if (Sse2.IsSupported)
                {
                    sbyte* startingAddress = (sbyte*)ptr;
                    while (utf8Text.Length - 16 >= idx)
                    {
                        Debug.Assert(startingAddress >= ptr && startingAddress <= (ptr + utf8Text.Length - 16));

                        // Load the next 16 bytes.
                        Vector128<sbyte> sourceValue = Sse2.LoadVector128(startingAddress);

                        // Check for ASCII text. Any byte that's not in the ASCII range will already be negative when
                        // casted to signed byte.
                        int index = Sse2.MoveMask(sourceValue);

                        if (index != 0)
                        {
                            // At least one of the following 16 bytes is non-ASCII.

                            int processNextSixteen = idx + 16;
                            Debug.Assert(processNextSixteen <= utf8Text.Length);

                            while (idx < processNextSixteen)
                            {
                                Debug.Assert((ptr + idx) <= (ptr + utf8Text.Length));

                                if (UnicodeUtility.IsAsciiCodePoint(ptr[idx]))
                                {
                                    if (!_allowedCharacters.IsUnicodeScalarAllowed(ptr[idx]))
                                    {
                                        goto Return;
                                    }
                                    idx++;
                                }
                                else
                                {
                                    OperationStatus opStatus = UnicodeHelpers.DecodeScalarValueFromUtf8(utf8Text.Slice(idx), out uint nextScalarValue, out int utf8BytesConsumedForScalar);

                                    Debug.Assert(nextScalarValue <= int.MaxValue);
                                    if (opStatus != OperationStatus.Done || WillEncode((int)nextScalarValue))
                                    {
                                        goto Return;
                                    }

                                    Debug.Assert(opStatus == OperationStatus.Done);
                                    idx += utf8BytesConsumedForScalar;
                                }
                            }
                            startingAddress = (sbyte*)ptr + idx;
                        }
                        else
                        {
                            // Check if any of the 16 bytes need to be escaped.
                             Vector128<sbyte> mask = Sse2Helper.CreateEscapingMask_UnsafeRelaxedJavaScriptEncoder(sourceValue);

                            index = Sse2.MoveMask(mask);
                            // If index == 0, that means none of the 16 bytes needed to be escaped.
                            // TrailingZeroCount is relatively expensive, avoid it if possible.
                            if (index != 0)
                            {
                                // Found at least one byte that needs to be escaped, figure out the index of
                                // the first one found that needed to be escaped within the 16 bytes.
                                Debug.Assert(index > 0 && index <= 65_535);
                                int tzc = BitOperations.TrailingZeroCount(index);
                                Debug.Assert(tzc >= 0 && tzc <= 16);
                                idx += tzc;
                                goto Return;
                            }
                            idx += 16;
                            startingAddress += 16;
                        }
                    }

                    // Process the remaining bytes.
                    Debug.Assert(utf8Text.Length - idx < 16);
                }
#endif

                while (idx < utf8Text.Length)
                {
                    Debug.Assert((ptr + idx) <= (ptr + utf8Text.Length));

                    if (UnicodeUtility.IsAsciiCodePoint(ptr[idx]))
                    {
                        if (!_allowedCharacters.IsUnicodeScalarAllowed(ptr[idx]))
                        {
                            goto Return;
                        }
                        idx++;
                    }
                    else
                    {
                        OperationStatus opStatus = UnicodeHelpers.DecodeScalarValueFromUtf8(utf8Text.Slice(idx), out uint nextScalarValue, out int utf8BytesConsumedForScalar);

                        Debug.Assert(nextScalarValue <= int.MaxValue);
                        if (opStatus != OperationStatus.Done || WillEncode((int)nextScalarValue))
                        {
                            goto Return;
                        }

                        Debug.Assert(opStatus == OperationStatus.Done);
                        idx += utf8BytesConsumedForScalar;
                    }
                }
                Debug.Assert(idx == utf8Text.Length);

                idx = -1; // All bytes are allowed.

            Return:
                return idx;
            }
        }

        // The worst case encoding is 6 output chars per input char: [input] U+FFFF -> [output] "\uFFFF"
        // We don't need to worry about astral code points since they're represented as encoded
        // surrogate pairs in the output.
        public override int MaxOutputCharactersPerInputCharacter => 12; // "\uFFFF\uFFFF" is the longest encoded form

        private static readonly char[] s_b = new char[] { '\\', 'b' };
        private static readonly char[] s_t = new char[] { '\\', 't' };
        private static readonly char[] s_n = new char[] { '\\', 'n' };
        private static readonly char[] s_f = new char[] { '\\', 'f' };
        private static readonly char[] s_r = new char[] { '\\', 'r' };
        private static readonly char[] s_back = new char[] { '\\', '\\' };
        private static readonly char[] s_doubleQuote = new char[] { '\\', '"' };

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            // ECMA-262 allows encoding U+000B as "\v", but ECMA-404 does not.
            // Both ECMA-262 and ECMA-404 allow encoding U+002F SOLIDUS as "\/"
            // (in ECMA-262 this character is a NonEscape character); however, we
            // don't encode SOLIDUS by default unless the caller has provided an
            // explicit bitmap which does not contain it. In this case we'll assume
            // that the caller didn't want a SOLIDUS written to the output at all,
            // so it should be written using "\u002F" encoding.
            // HTML-specific characters (including apostrophe and quotes) will
            // be written out as numeric entities for defense-in-depth.
            // See UnicodeEncoderBase ctor comments for more info.

            if (!WillEncode(unicodeScalar))
            {
                return TryWriteScalarAsChar(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
            }

            char[] toCopy;
            switch (unicodeScalar)
            {
                case '\"':
                    toCopy = s_doubleQuote;
                    break;
                case '\b':
                    toCopy = s_b;
                    break;
                case '\t':
                    toCopy = s_t;
                    break;
                case '\n':
                    toCopy = s_n;
                    break;
                case '\f':
                    toCopy = s_f;
                    break;
                case '\r':
                    toCopy = s_r;
                    break;
                case '\\':
                    toCopy = s_back;
                    break;
                default:
                    return JavaScriptEncoderHelper.TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
            }
            return TryCopyCharacters(toCopy, buffer, bufferLength, out numberOfCharactersWritten);
        }
    }
}
