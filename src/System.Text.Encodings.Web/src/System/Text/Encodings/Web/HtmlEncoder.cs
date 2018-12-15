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
    /// Represents a type used to do HTML encoding.
    /// </summary>
    public abstract class HtmlEncoder : TextEncoder
    {
        /// <summary>
        /// Returns a default built-in instance of <see cref="HtmlEncoder"/>.
        /// </summary>
        public static HtmlEncoder Default => DefaultHtmlEncoder.s_singleton;

        /// <summary>
        /// Creates a new instance of HtmlEncoder with provided settings.
        /// </summary>
        /// <param name="settings">Settings used to control how the created <see cref="HtmlEncoder"/> encodes, primarily which characters to encode.</param>
        /// <returns>A new instance of the <see cref="HtmlEncoder"/>.</returns>
        public static HtmlEncoder Create(TextEncoderSettings settings)
            => new DefaultHtmlEncoder(settings);

        /// <summary>
        /// Creates a new instance of HtmlEncoder specifying character to be encoded.
        /// </summary>
        /// <param name="allowedRanges">Set of characters that the encoder is allowed to not encode.</param>
        /// <returns>A new instance of the <see cref="HtmlEncoder"/></returns>
        /// <remarks>Some characters in <paramref name="allowedRanges"/> might still get encoded, i.e. this parameter is just telling the encoder what ranges it is allowed to not encode, not what characters it must not encode.</remarks> 
        public static HtmlEncoder Create(params UnicodeRange[] allowedRanges)
            => new DefaultHtmlEncoder(allowedRanges);
    }

    internal sealed class DefaultHtmlEncoder : HtmlEncoder
    {
        private AllowedCharactersBitmap _allowedCharacters;
        internal static readonly DefaultHtmlEncoder s_singleton = new DefaultHtmlEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

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
            {
                throw new ArgumentNullException(nameof(text));
            }
            return FindFirstCharacterToEncode(new ReadOnlySpan<char>(text, textLength));
        }

        public override int MaxOutputCharactersPerInputCharacter => 10; // "&#x10FFFF;" is the longest encoded form

        static readonly char[] s_quote = "&quot;".ToCharArray();
        static readonly char[] s_ampersand = "&amp;".ToCharArray();
        static readonly char[] s_lessthan = "&lt;".ToCharArray();
        static readonly char[] s_greaterthan = "&gt;".ToCharArray();

        public unsafe override bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return TryEncodeUnicodeScalar(unicodeScalar, new Span<char>(buffer, bufferLength), out numberOfCharactersWritten);
        }

        public override bool TryEncodeUnicodeScalar(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten)
        {
            if (!WillEncode(unicodeScalar))
                return TryWriteScalarAsChar(unicodeScalar, buffer, out numberOfCharactersWritten);
            else if (unicodeScalar == '\"')
                return TryCopyCharacters(s_quote, buffer, out numberOfCharactersWritten);
            else if (unicodeScalar == '&')
                return TryCopyCharacters(s_ampersand, buffer, out numberOfCharactersWritten);
            else if (unicodeScalar == '<')
                return TryCopyCharacters(s_lessthan, buffer, out numberOfCharactersWritten);
            else if (unicodeScalar == '>')
                return TryCopyCharacters(s_greaterthan, buffer, out numberOfCharactersWritten);
            else
                return TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, out numberOfCharactersWritten);
        }

        private static bool TryWriteEncodedScalarAsNumericEntity(int unicodeScalar, Span<char> buffer, out int numberOfCharactersWritten)
        {
            // We're writing the characters in reverse, first determine
            // how many there are
            const int nibbleSize = 4;
            int numberOfHexCharacters = 0;
            int compareUnicodeScalar = unicodeScalar;

            do
            {
                Debug.Assert(numberOfHexCharacters < 8, "Couldn't have written 8 characters out by this point.");
                numberOfHexCharacters++;
                compareUnicodeScalar >>= nibbleSize;
            } while (compareUnicodeScalar != 0);

            numberOfCharactersWritten = numberOfHexCharacters + 4; // four chars are &, #, x, and ;
            Debug.Assert(numberOfHexCharacters > 0, "At least one character should've been written.");

            if (numberOfHexCharacters + 4 > buffer.Length)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            int idx = 0;
            // Finally, write out the HTML-encoded scalar value.
            buffer[idx++] = '&';
            buffer[idx++] = '#';
            buffer[idx] = 'x';

            // Jump to the end of the hex position and write backwards
            idx += numberOfHexCharacters;
            do
            {
                buffer[idx] = HexUtil.Int32LsbToHexDigit(unicodeScalar & 0xF);
                unicodeScalar >>= nibbleSize;
                idx--;
            }
            while (unicodeScalar != 0);

            idx += numberOfHexCharacters + 1;
            buffer[idx] = ';';
            return true;
        }

        public override int FindFirstCharacterToEncode(ReadOnlySpan<char> text)
            => _allowedCharacters.FindFirstCharacterToEncode(text);
    }
}
