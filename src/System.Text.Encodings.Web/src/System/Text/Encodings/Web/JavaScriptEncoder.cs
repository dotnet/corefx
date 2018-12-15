// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Internal;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// Represents a type used to do JavaScript encoding/escaping.
    /// </summary>
    public abstract class JavaScriptEncoder : TextEncoder
    {
        /// <summary>
        /// Returns a default built-in instance of <see cref="JavaScriptEncoder"/>.
        /// </summary>
        public static JavaScriptEncoder Default => DefaultJavaScriptEncoder.s_singleton;

        /// <summary>
        /// Creates a new instance of JavaScriptEncoder with provided settings.
        /// </summary>
        /// <param name="settings">Settings used to control how the created <see cref="JavaScriptEncoder"/> encodes, primarily which characters to encode.</param>
        /// <returns>A new instance of the <see cref="JavaScriptEncoder"/>.</returns>
        public static JavaScriptEncoder Create(TextEncoderSettings settings)
            => new DefaultJavaScriptEncoder(settings);

        /// <summary>
        /// Creates a new instance of JavaScriptEncoder specifying character to be encoded.
        /// </summary>
        /// <param name="allowedRanges">Set of characters that the encoder is allowed to not encode.</param>
        /// <returns>A new instance of the <see cref="JavaScriptEncoder"/>.</returns>
        /// <remarks>Some characters in <paramref name="allowedRanges"/> might still get encoded, i.e. this parameter is just telling the encoder what ranges it is allowed to not encode, not what characters it must not encode.</remarks> 
        public static JavaScriptEncoder Create(params UnicodeRange[] allowedRanges)
            => new DefaultJavaScriptEncoder(allowedRanges);
    }

    internal sealed class DefaultJavaScriptEncoder : JavaScriptEncoder
    {
        private AllowedCharactersBitmap _allowedCharacters;

        internal static readonly DefaultJavaScriptEncoder s_singleton = new DefaultJavaScriptEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

        public DefaultJavaScriptEncoder(TextEncoderSettings filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            _allowedCharacters = filter.GetAllowedCharacters();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharacters.ForbidUndefinedCharacters();

            // Forbid characters that are special in HTML.
            // Even though this is a not HTML encoder, 
            // it's unfortunately common for developers to
            // forget to HTML-encode a string once it has been JS-encoded,
            // so this offers extra protection.
            DefaultHtmlEncoder.ForbidHtmlCharacters(_allowedCharacters);

            _allowedCharacters.ForbidCharacter('\\');
            _allowedCharacters.ForbidCharacter('/');

            // Forbid GRAVE ACCENT \u0060 character.
            _allowedCharacters.ForbidCharacter('`');
        }

        public DefaultJavaScriptEncoder(params UnicodeRange[] allowedRanges) : this(new TextEncoderSettings(allowedRanges))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool WillEncode(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar))
                return true;
            return !_allowedCharacters.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe override int FindFirstCharacterToEncode(char* text, int textLength)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            return FindFirstCharacterToEncode(new ReadOnlySpan<char>(text, textLength));
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
        private static readonly char[] s_forward = new char[] { '\\', '/' };
        private static readonly char[] s_back = new char[] { '\\', '\\' };

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        public unsafe override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return TryEncodeUnicodeScalar(unicodeScalar, new Span<char>(buffer, bufferLength), out numberOfCharactersWritten);
        }

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        public override bool TryEncodeUnicodeScalar(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten)
        {
            // ECMA-262 allows encoding U+000B as "\v", but ECMA-404 does not.
            // Both ECMA-262 and ECMA-404 allow encoding U+002F SOLIDUS as "\/".
            // (In ECMA-262 this character is a NonEscape character.)
            // HTML-specific characters (including apostrophe and quotes) will
            // be written out as numeric entities for defense-in-depth.
            // See UnicodeEncoderBase ctor comments for more info.

            if (!WillEncode(unicodeScalar))
                return TryWriteScalarAsChar(unicodeScalar, buffer, out numberOfCharactersWritten);

            ReadOnlySpan<char> toCopy;
            switch (unicodeScalar)
            {
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
                case '/':
                    toCopy = s_forward;
                    break;
                case '\\':
                    toCopy = s_back;
                    break;
                default:
                    return TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, out numberOfCharactersWritten);
            }

            if (!toCopy.TryCopyTo(buffer))
            {
                numberOfCharactersWritten = 0;
                return false;
            }
            numberOfCharactersWritten = toCopy.Length;
            return true;
        }

        private static bool TryWriteEncodedScalarAsNumericEntity(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar))
            {
                // Convert this back to UTF-16 and write out both characters.
                UnicodeHelpers.GetUtf16SurrogatePairFromAstralScalarValue(unicodeScalar, out char leadingSurrogate, out char trailingSurrogate);

                if (TryWriteEncodedSingleCharacter(leadingSurrogate, buffer, out int leadingSurrogateCharactersWritten) &&
                    TryWriteEncodedSingleCharacter(trailingSurrogate, buffer.Slice(leadingSurrogateCharactersWritten), out numberOfCharactersWritten))
                {
                    numberOfCharactersWritten += leadingSurrogateCharactersWritten;
                    return true;
                }
                else
                {
                    numberOfCharactersWritten = 0;
                    return false;
                }
            }
            else
            {
                // This is only a single character.
                return TryWriteEncodedSingleCharacter(unicodeScalar, buffer, out numberOfCharactersWritten);
            }
        }

        // Writes an encoded scalar value (in the BMP) as a JavaScript-escaped character.
        private static bool TryWriteEncodedSingleCharacter(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten)
        {
            Debug.Assert(!UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar), "The incoming value should've been in the BMP.");

            if (buffer.Length < 6)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            // Encode this as 6 chars "\uFFFF".
            buffer[0] = '\\';
            buffer[1] = 'u';
            buffer[2] = HexUtil.Int32LsbToHexDigit(unicodeScalar >> 12);
            buffer[3] = HexUtil.Int32LsbToHexDigit((int)((unicodeScalar >> 8) & 0xFU));
            buffer[4] = HexUtil.Int32LsbToHexDigit((int)((unicodeScalar >> 4) & 0xFU));
            buffer[5] = HexUtil.Int32LsbToHexDigit((int)(unicodeScalar & 0xFU));

            numberOfCharactersWritten = 6;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int FindFirstCharacterToEncode(ReadOnlySpan<char> text)
            => _allowedCharacters.FindFirstCharacterToEncode(text);
    }
}
