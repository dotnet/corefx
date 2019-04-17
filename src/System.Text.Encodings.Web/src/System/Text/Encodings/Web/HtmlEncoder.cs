// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.Internal;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// Represents a type used to do HTML encoding.
    /// </summary>
    public abstract class HtmlEncoder : TextEncoder
    {
        /// <summary>
        /// Returns a default built-in instance of <see cref="HtmlEncoder"/>.
        /// </summary>
        public static HtmlEncoder Default
        {
            get { return DefaultHtmlEncoder.Singleton; }
        }

        /// <summary>
        /// Creates a new instance of HtmlEncoder with provided settings.
        /// </summary>
        /// <param name="settings">Settings used to control how the created <see cref="HtmlEncoder"/> encodes, primarily which characters to encode.</param>
        /// <returns>A new instance of the <see cref="HtmlEncoder"/>.</returns>
        public static HtmlEncoder Create(TextEncoderSettings settings)
        {
            return new DefaultHtmlEncoder(settings);
        }

        /// <summary>
        /// Creates a new instance of HtmlEncoder specifying character to be encoded.
        /// </summary>
        /// <param name="allowedRanges">Set of characters that the encoder is allowed to not encode.</param>
        /// <returns>A new instance of the <see cref="HtmlEncoder"/></returns>
        /// <remarks>Some characters in <paramref name="allowedRanges"/> might still get encoded, i.e. this parameter is just telling the encoder what ranges it is allowed to not encode, not what characters it must not encode.</remarks> 
        public static HtmlEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new DefaultHtmlEncoder(allowedRanges);
        }
    }

    internal sealed class DefaultHtmlEncoder : HtmlEncoder
    {
        private const int MaxEncodedScalarLength = 10; // "&#x10FFFF;" is the longest encoded form

        private AllowedCharactersBitmap _allowedCharacters;
        internal static readonly DefaultHtmlEncoder Singleton = new DefaultHtmlEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

        public DefaultHtmlEncoder(TextEncoderSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _allowedCharacters = settings.GetAllowedCharacters();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharacters.ForbidUndefinedCharacters();

            ForbidHtmlCharacters(_allowedCharacters);
        }

        internal static void ForbidHtmlCharacters(AllowedCharactersBitmap allowedCharacters)
        {
            allowedCharacters.ForbidCharacter('<');
            allowedCharacters.ForbidCharacter('>');
            allowedCharacters.ForbidCharacter('&');
            allowedCharacters.ForbidCharacter('\''); // can be used to escape attributes
            allowedCharacters.ForbidCharacter('\"'); // can be used to escape attributes
            allowedCharacters.ForbidCharacter('+'); // technically not HTML-specific, but can be used to perform UTF7-based attacks
        }

        public DefaultHtmlEncoder(params UnicodeRange[] allowedRanges) : this(new TextEncoderSettings(allowedRanges))
        { }

        public override bool RuneMustBeEncoded(Rune value)
        {
            return !_allowedCharacters.IsUnicodeScalarAllowed((uint)value.Value);
        }

        static readonly char[] s_quote = "&quot;".ToCharArray();
        static readonly char[] s_ampersand = "&amp;".ToCharArray();
        static readonly char[] s_lessthan = "&lt;".ToCharArray();
        static readonly char[] s_greaterthan = "&gt;".ToCharArray();

        public override int EncodeSingleRune(Rune value, Span<char> buffer)
        {
            uint scalarValue = (uint)value.Value;
            Span<char> escapedData = stackalloc char[MaxEncodedScalarLength];

            if (scalarValue == '\"')
            {
                escapedData = s_quote;
            }
            else if (scalarValue == '&')
            {
                escapedData = s_ampersand;
            }
            else if (scalarValue == '<')
            {
                escapedData = s_lessthan;
            }
            else if (scalarValue == '>')
            {
                escapedData = s_greaterthan;
            }
            else
            {
                escapedData = escapedData.Slice(WriteEncodedScalarToEndOfScratchBufferAsNumericEntity(scalarValue, escapedData));
            }

            return escapedData.TryCopyTo(buffer) ? escapedData.Length : -1;
        }

        // Writes a scalar value as "&#xABCDEF;" to the end of the scratch buffer,
        // then returns the offset in the scratch buffer where the data begins.
        private static int WriteEncodedScalarToEndOfScratchBufferAsNumericEntity(uint scalarValue, Span<char> buffer)
        {
            Debug.Assert(buffer.Length >= MaxEncodedScalarLength, "Invalid scratch buffer length provided; might write up to 10 chars.");

            int offset = buffer.Length;
            buffer[--offset] = ';';

            do
            {
                buffer[--offset] = HexUtil.UInt32LsbToHexDigit(scalarValue & 0xF);
                scalarValue >>= 4;
                if (scalarValue != 0)
                {
                    buffer[--offset] = HexUtil.UInt32LsbToHexDigit(scalarValue & 0xF);
                    scalarValue >>= 4;
                }
            } while (scalarValue != 0);

            buffer[--offset] = 'x';
            buffer[--offset] = '#';
            buffer[--offset] = '&';

            return offset;
        }
    }
}
