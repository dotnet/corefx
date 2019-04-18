// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.Internal;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// Represents a type used to do JavaScript encoding/escaping.
    /// </summary>
    public abstract class JavaScriptEncoder : TextEncoder
    {
#if BUILDING_FOR_NETSTANDARD
        // Don't allow anybody outside of our assembly to subclass this on netstandard
        internal JavaScriptEncoder()
        {
        }
#endif

        /// <summary>
        /// Returns a default built-in instance of <see cref="JavaScriptEncoder"/>.
        /// </summary>
        public static JavaScriptEncoder Default
        {
            get { return DefaultJavaScriptEncoder.Singleton; }
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
        private const int MaxEncodedScalarLength = 12; // "\uFFFF\uFFFF" is the longest encoded form

        private AllowedCharactersBitmap _allowedCharacters;

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

            _allowedCharacters.ForbidCharacter('\\');
            _allowedCharacters.ForbidCharacter('/');

            // Forbid GRAVE ACCENT \u0060 character.
            _allowedCharacters.ForbidCharacter('`');
        }

        public DefaultJavaScriptEncoder(params UnicodeRange[] allowedRanges) : this(new TextEncoderSettings(allowedRanges))
        { }

#if !BUILDING_FOR_NETSTANDARD
        public override bool RuneMustBeEncoded(Rune value)
#else
        internal override bool RuneMustBeEncoded(Rune value) // Rune is internal on netstandard
#endif
        {
            return !_allowedCharacters.IsUnicodeScalarAllowed((uint)value.Value);
        }

        static readonly char[] s_b = new char[] { '\\', 'b' };
        static readonly char[] s_t = new char[] { '\\', 't' };
        static readonly char[] s_n = new char[] { '\\', 'n' };
        static readonly char[] s_f = new char[] { '\\', 'f' };
        static readonly char[] s_r = new char[] { '\\', 'r' };
        static readonly char[] s_forward = new char[] { '\\', '/' };
        static readonly char[] s_back = new char[] { '\\', '\\' };

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
#if !BUILDING_FOR_NETSTANDARD
        public override int EncodeSingleRune(Rune value, Span<char> buffer)
#else
        internal override int EncodeSingleRune(Rune value, Span<char> buffer) // Rune is internal on netstandard
#endif
        {
            // ECMA-262 allows encoding U+000B as "\v", but ECMA-404 does not.
            // Both ECMA-262 and ECMA-404 allow encoding U+002F SOLIDUS as "\/".
            // (In ECMA-262 this character is a NonEscape character.)
            // HTML-specific characters (including apostrophe and quotes) will
            // be written out as numeric entities for defense-in-depth.
            // See UnicodeEncoderBase ctor comments for more info.

            Span<char> escapedData = stackalloc char[MaxEncodedScalarLength];

            switch (value.Value)
            {
                case '\b':
                    escapedData = s_b;
                    break;
                case '\t':
                    escapedData = s_t;
                    break;
                case '\n':
                    escapedData = s_n;
                    break;
                case '\f':
                    escapedData = s_f;
                    break;
                case '\r':
                    escapedData = s_r;
                    break;
                case '/':
                    escapedData = s_forward;
                    break;
                case '\\':
                    escapedData = s_back;
                    break;
                default:
                    escapedData = escapedData.Slice(0, WriteRuneAsEncodedNumericEntity(value, escapedData));
                    break;
            }

            return escapedData.TryCopyTo(buffer) ? escapedData.Length : -1;
        }

        // Writes a scalar value as "\uAAAA" to the scratch buffer, then returns the number
        // of chars written.
        private static int WriteRuneAsEncodedNumericEntity(Rune value, Span<char> buffer)
        {
            Debug.Assert(buffer.Length >= MaxEncodedScalarLength, "Invalid scratch buffer length provided; might write up to 12 chars.");

            Span<char> scalarAsChars = stackalloc char[2];
            int scalarCharCount = value.EncodeToUtf16(scalarAsChars);

            // BMP chars are also handled by the below

            buffer[0] = '\\';
            buffer[1] = 'u';

#if !BUILDING_FOR_NETSTANDARD
            bool succeeded = ((uint)scalarAsChars[0]).TryFormat(buffer.Slice(2), out _, "X4");
            Debug.Assert(succeeded);
#else
            char firstChar = scalarAsChars[0];
            HexUtil.ByteToHexDigits((byte)firstChar, out buffer[4], out buffer[5]);
            HexUtil.ByteToHexDigits((byte)(firstChar >> 8), out buffer[2], out buffer[3]);
#endif

            // If we actually got a surrogate pair, write out the second component now.

            if (scalarCharCount > 1)
            {
                buffer[6] = '\\';
                buffer[7] = 'u';

#if !BUILDING_FOR_NETSTANDARD
                succeeded = ((uint)scalarAsChars[1]).TryFormat(buffer.Slice(8), out _, "X4");
                Debug.Assert(succeeded);
#else
                char secondChar = scalarAsChars[1];
                HexUtil.ByteToHexDigits((byte)secondChar, out buffer[10], out buffer[11]);
                HexUtil.ByteToHexDigits((byte)(secondChar >> 8), out buffer[8], out buffer[9]);
#endif
            }

            return scalarCharCount * 6; // each UTF-16 code unit gets turned into 6 chars after escaping
        }
    }
}
