// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
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
        public static JavaScriptEncoder Default
        {
            get { return DefaultJavaScriptEncoder.Singleton; }
        }

        /// <summary>
        /// Returns a built-in instance of <see cref="JavaScriptEncoder"/> that is less strict about what gets encoded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unlike the <see cref="Default"/>, this encoder instance does not escape HTML-senstive characters like &lt;, &gt;, &amp;, etc. and hence must be used cautiously
        /// (for example, if the output data is within a response whose content-type is known with a charset set to UTF-8).
        /// </para>
        /// <para>
        /// Unlike the <see cref="Default"/>, the quotation mark is encoded as \" rather than \u0022.
        /// </para>
        /// <para>
        /// Unlike the <see cref="Default"/> (which only allows <see cref="UnicodeRanges.BasicLatin"/>), using this encoder instance allows <see cref="UnicodeRanges.All"/> to go through unescaped.
        /// </para>
        /// <para>
        /// Unlike the <see cref="Default"/>, this encoder instance allows some other characters to go through unescaped (for example, '+'), and hence must be used cautiously.
        /// </para>
        /// </remarks>
        public static JavaScriptEncoder UnsafeRelaxedJsonEscaping
        {
            get { return UnsafeRelaxedJavaScriptEncoder.s_singleton; }
        }

        /// <summary>
        /// Creates a new instance of JavaScriptEncoder with provided settings.
        /// </summary>
        /// <param name="settings">Settings used to control how the created <see cref="JavaScriptEncoder"/> encodes, primarily which characters to encode.</param>
        /// <returns>A new instance of the <see cref="JavaScriptEncoder"/>.</returns>
        public static JavaScriptEncoder Create(TextEncoderSettings settings)
        {
            return new DefaultJavaScriptEncoder(settings);
        }

        /// <summary>
        /// Creates a new instance of JavaScriptEncoder specifying character to be encoded.
        /// </summary>
        /// <param name="allowedRanges">Set of characters that the encoder is allowed to not encode.</param>
        /// <returns>A new instance of the <see cref="JavaScriptEncoder"/>.</returns>
        /// <remarks>Some characters in <paramref name="allowedRanges"/> might still get encoded, i.e. this parameter is just telling the encoder what ranges it is allowed to not encode, not what characters it must not encode.</remarks>
        public static JavaScriptEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new DefaultJavaScriptEncoder(allowedRanges);
        }
    }

    internal sealed class DefaultJavaScriptEncoder : JavaScriptEncoder
    {
        private readonly AllowedCharactersBitmap _allowedCharacters;

        internal static readonly DefaultJavaScriptEncoder Singleton = new DefaultJavaScriptEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

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

            // '\' (U+005C REVERSE SOLIDUS) must always be escaped in Javascript / ECMAScript / JSON.
            // '/' (U+002F SOLIDUS) is not Javascript / ECMAScript / JSON-sensitive so doesn't need to be escaped.
            _allowedCharacters.ForbidCharacter('\\');

            // '`' (U+0060 GRAVE ACCENT) is ECMAScript-sensitive (see ECMA-262).
            _allowedCharacters.ForbidCharacter('`');
        }

        public DefaultJavaScriptEncoder(params UnicodeRange[] allowedRanges) : this(new TextEncoderSettings(allowedRanges))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool WillEncode(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar)) return true;
            return !_allowedCharacters.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe override int FindFirstCharacterToEncode(char* text, int textLength)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return _allowedCharacters.FindFirstCharacterToEncode(text, textLength);
        }

        // The worst case encoding is 6 output chars per input char: [input] U+FFFF -> [output] "\uFFFF"
        // We don't need to worry about astral code points since they're represented as encoded
        // surrogate pairs in the output.
        public override int MaxOutputCharactersPerInputCharacter
        {
            get { return 12; } // "\uFFFF\uFFFF" is the longest encoded form
        }

        private static readonly char[] s_b = new char[] { '\\', 'b' };
        private static readonly char[] s_t = new char[] { '\\', 't' };
        private static readonly char[] s_n = new char[] { '\\', 'n' };
        private static readonly char[] s_f = new char[] { '\\', 'f' };
        private static readonly char[] s_r = new char[] { '\\', 'r' };
        private static readonly char[] s_back = new char[] { '\\', '\\' };

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        public unsafe override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
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

            if (!WillEncode(unicodeScalar)) { return TryWriteScalarAsChar(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten); }

            char[] toCopy;
            switch (unicodeScalar)
            {
                case '\b': toCopy = s_b; break;
                case '\t': toCopy = s_t; break;
                case '\n': toCopy = s_n; break;
                case '\f': toCopy = s_f; break;
                case '\r': toCopy = s_r; break;
                case '\\': toCopy = s_back; break;
                default: return TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, bufferLength, out numberOfCharactersWritten);
            }
            return TryCopyCharacters(toCopy, buffer, bufferLength, out numberOfCharactersWritten);
        }

        private static unsafe bool TryWriteEncodedScalarAsNumericEntity(int unicodeScalar, char* buffer, int length, out int numberOfCharactersWritten)
        {
            Debug.Assert(buffer != null && length >= 0);

            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar))
            {
                // Convert this back to UTF-16 and write out both characters.
                char leadingSurrogate, trailingSurrogate;
                UnicodeHelpers.GetUtf16SurrogatePairFromAstralScalarValue(unicodeScalar, out leadingSurrogate, out trailingSurrogate);
                int leadingSurrogateCharactersWritten;
                if (TryWriteEncodedSingleCharacter(leadingSurrogate, buffer, length, out leadingSurrogateCharactersWritten) &&
                    TryWriteEncodedSingleCharacter(trailingSurrogate, buffer + leadingSurrogateCharactersWritten, length - leadingSurrogateCharactersWritten, out numberOfCharactersWritten)
                )
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
                return TryWriteEncodedSingleCharacter(unicodeScalar, buffer, length, out numberOfCharactersWritten);
            }
        }

        // Writes an encoded scalar value (in the BMP) as a JavaScript-escaped character.
        private static unsafe bool TryWriteEncodedSingleCharacter(int unicodeScalar, char* buffer, int length, out int numberOfCharactersWritten)
        {
            Debug.Assert(buffer != null && length >= 0);
            Debug.Assert(!UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar), "The incoming value should've been in the BMP.");

            if (length < 6)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            // Encode this as 6 chars "\uFFFF".
            *buffer = '\\'; buffer++;
            *buffer = 'u'; buffer++;
            *buffer = HexUtil.Int32LsbToHexDigit(unicodeScalar >> 12); buffer++;
            *buffer = HexUtil.Int32LsbToHexDigit((int)((unicodeScalar >> 8) & 0xFU)); buffer++;
            *buffer = HexUtil.Int32LsbToHexDigit((int)((unicodeScalar >> 4) & 0xFU)); buffer++;
            *buffer = HexUtil.Int32LsbToHexDigit((int)(unicodeScalar & 0xFU)); buffer++;

            numberOfCharactersWritten = 6;
            return true;
        }
    }
}
