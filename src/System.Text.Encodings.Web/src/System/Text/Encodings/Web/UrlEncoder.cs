// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Internal;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// Represents a type used to do URL encoding.
    /// </summary>
    public abstract class UrlEncoder : TextEncoder
    {
        /// <summary>
        /// Returns a default built-in instance of <see cref="UrlEncoder"/>.
        /// </summary>
        public static UrlEncoder Default
        {
            get { return DefaultUrlEncoder.Singleton; }
        }

        /// <summary>
        /// Creates a new instance of UrlEncoder with provided settings.
        /// </summary>
        /// <param name="settings">Settings used to control how the created <see cref="UrlEncoder"/> encodes, primarily which characters to encode.</param>
        /// <returns>A new instance of the <see cref="UrlEncoder"/>.</returns>
        public static UrlEncoder Create(TextEncoderSettings settings)
        {
            return new DefaultUrlEncoder(settings);
        }

        /// <summary>
        /// Creates a new instance of UrlEncoder specifying character to be encoded.
        /// </summary>
        /// <param name="allowedRanges">Set of characters that the encoder is allowed to not encode.</param>
        /// <returns>A new instance of the <see cref="UrlEncoder"/>.</returns>
        /// <remarks>Some characters in <paramref name="allowedRanges"/> might still get encoded, i.e. this parameter is just telling the encoder what ranges it is allowed to not encode, not what characters it must not encode.</remarks> 
        public static UrlEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new DefaultUrlEncoder(allowedRanges);
        }
    }

    internal sealed class DefaultUrlEncoder : UrlEncoder
    {
        private const int MaxEncodedScalarLength = 12; // "%XX%YY%ZZ%WW" is the longest encoded form (hex-escaped UTF-8 representation of non-BMP scalar)

        private AllowedCharactersBitmap _allowedCharacters;

        internal static readonly DefaultUrlEncoder Singleton = new DefaultUrlEncoder(new TextEncoderSettings(UnicodeRanges.BasicLatin));

        public DefaultUrlEncoder(TextEncoderSettings filter)
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
            // forget to HTML-encode a string once it has been URL-encoded,
            // so this offers extra protection.
            DefaultHtmlEncoder.ForbidHtmlCharacters(_allowedCharacters);

            // Per RFC 3987, Sec. 2.2, we want encodings that are safe for
            // four particular components: 'isegment', 'ipath-noscheme',
            // 'iquery', and 'ifragment'. The relevant definitions are below.
            //
            //    ipath-noscheme = isegment-nz-nc *( "/" isegment )
            // 
            //    isegment       = *ipchar
            // 
            //    isegment-nz-nc = 1*( iunreserved / pct-encoded / sub-delims
            //                         / "@" )
            //                   ; non-zero-length segment without any colon ":"
            //
            //    ipchar         = iunreserved / pct-encoded / sub-delims / ":"
            //                   / "@"
            // 
            //    iquery         = *( ipchar / iprivate / "/" / "?" )
            // 
            //    ifragment      = *( ipchar / "/" / "?" )
            // 
            //    iunreserved    = ALPHA / DIGIT / "-" / "." / "_" / "~" / ucschar
            // 
            //    ucschar        = %xA0-D7FF / %xF900-FDCF / %xFDF0-FFEF
            //                   / %x10000-1FFFD / %x20000-2FFFD / %x30000-3FFFD
            //                   / %x40000-4FFFD / %x50000-5FFFD / %x60000-6FFFD
            //                   / %x70000-7FFFD / %x80000-8FFFD / %x90000-9FFFD
            //                   / %xA0000-AFFFD / %xB0000-BFFFD / %xC0000-CFFFD
            //                   / %xD0000-DFFFD / %xE1000-EFFFD
            // 
            //    pct-encoded    = "%" HEXDIG HEXDIG
            // 
            //    sub-delims     = "!" / "$" / "&" / "'" / "(" / ")"
            //                   / "*" / "+" / "," / ";" / "="
            //
            // The only common characters between these four components are the
            // intersection of 'isegment-nz-nc' and 'ipchar', which is really
            // just 'isegment-nz-nc' (colons forbidden).
            // 
            // From this list, the base encoder already forbids "&", "'", "+",
            // and we'll additionally forbid "=" since it has special meaning
            // in x-www-form-urlencoded representations.
            //
            // This means that the full list of allowed characters from the
            // Basic Latin set is:
            // ALPHA / DIGIT / "-" / "." / "_" / "~" / "!" / "$" / "(" / ")" / "*" / "," / ";" / "@"

            const string forbiddenChars = @" #%/:=?[\]^`{|}"; // chars from Basic Latin which aren't already disallowed by the base encoder
            foreach (char character in forbiddenChars)
            {
                _allowedCharacters.ForbidCharacter(character);
            }

            // Specials (U+FFF0 .. U+FFFF) are forbidden by the definition of 'ucschar' above
            for (int i = 0; i < 16; i++)
            {
                _allowedCharacters.ForbidCharacter((char)(0xFFF0 | i));
            }
        }

        public DefaultUrlEncoder(params UnicodeRange[] allowedRanges) : this(new TextEncoderSettings(allowedRanges))
        { }

        public override bool RuneMustBeEncoded(Rune value)
        {
            return !_allowedCharacters.IsUnicodeScalarAllowed((uint)value.Value);
        }

        public override int EncodeSingleRune(Rune value, Span<char> buffer)
        {
            Span<byte> scalarAsUtf8Bytes = stackalloc byte[4]; // max 4 UTF-8 bytes per scalar
            Span<char> escapedData = stackalloc char[MaxEncodedScalarLength];

            int utf8ByteLength = value.EncodeToUtf8(scalarAsUtf8Bytes);

            // For each UTF-8 byte, write "%XX"

            for (int i = 0; i < utf8ByteLength; i++)
            {
                int escapedDataIndex = 3 * i;
                escapedData[escapedDataIndex] = '%';
                HexUtil.ByteToHexDigits(scalarAsUtf8Bytes[i], out escapedData[escapedDataIndex + 1], out escapedData[escapedDataIndex + 2]);
            }

            escapedData = escapedData.Slice(0, utf8ByteLength * 3);
            return escapedData.TryCopyTo(buffer) ? escapedData.Length : -1;
        }
    }
}
