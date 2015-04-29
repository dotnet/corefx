// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Internal;

namespace System.Text.Encodings.Web
{
    public abstract class TextEncoder
    {
        public abstract bool TryEncodeUnicodeScalar(int unicodeScalar, char[] buffer, int index, out int writtenChars);
        public abstract bool Encodes(int unicodeScalar);

        // all subclasses have the same implementation of this method.
        // but this cannot be made virtual, because it will cause a virtual call to Encodes, and it destroys perf, i.e. makes common scenario 2x slower 
        public abstract int FindFirstCharacterToEncode(string text);

        // this could be a field, but I am trying to make the abstraction pure.
        public abstract int MaxOutputCharsPerInputChar { get; }

        /* The following two methods would improve perf, but complicate the API */

        //public unsafe abstract bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int length, out int writtenChars);
        //public unsafe abstract int FindFirstCharacterToEncode(char* text, int charCount);
    }

    public abstract class HtmlEncoder : TextEncoder
    {
        public static HtmlEncoder Default
        {
            get { return HtmlTextEncoder.Default; }
        }
        public static HtmlEncoder Create(CodePointFilter filter)
        {
            return new HtmlTextEncoder(filter);
        }
        public static HtmlEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new HtmlTextEncoder(allowedRanges);
        }
    }

    public abstract class JavaScriptEncoder : TextEncoder
    {
        public static JavaScriptEncoder Default
        {
            get { return JavaScriptTextEncoder.Default; }
        }
        public static JavaScriptEncoder Create(CodePointFilter filter)
        {
            return new JavaScriptTextEncoder(filter);
        }
        public static JavaScriptEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new JavaScriptTextEncoder(allowedRanges);
        }
    }

    public abstract class UrlEncoder : TextEncoder
    {
        public static UrlEncoder Default
        {
            get { return UrlTextEncoder.Default; }
        }
        public static UrlEncoder Create(CodePointFilter filter)
        {
            return new UrlTextEncoder(filter);
        }
        public static UrlEncoder Create(params UnicodeRange[] allowedRanges)
        {
            return new UrlTextEncoder(allowedRanges);
        }
    }

    internal sealed class HtmlTextEncoder : HtmlEncoder
    {
        private AllowedCharsBitmap _allowedCharsBitmap;

        public readonly static HtmlTextEncoder Default = new HtmlTextEncoder(new CodePointFilter(UnicodeRanges.BasicLatin));

        public HtmlTextEncoder(CodePointFilter filter)
        {
            _allowedCharsBitmap = filter.GetAllowedCharsBitmap();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharsBitmap.ForbidUndefinedCharacters();

            // Forbid characters that are special in HTML.
            // Even though this is a common encoder used by everybody (including URL
            // and JavaScript strings), it's unfortunately common for developers to
            // forget to HTML-encode a string once it has been URL-encoded or
            // JavaScript string-escaped, so this offers extra protection.
            _allowedCharsBitmap.ForbidCharacter('<');
            _allowedCharsBitmap.ForbidCharacter('>');
            _allowedCharsBitmap.ForbidCharacter('&');
            _allowedCharsBitmap.ForbidCharacter('\''); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('\"'); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('+'); // technically not HTML-specific, but can be used to perform UTF7-based attacks
        }

        public HtmlTextEncoder(params UnicodeRange[] allowedRanges) : this(new CodePointFilter(allowedRanges))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Encodes(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar)) return true;            
            return !_allowedCharsBitmap.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int FindFirstCharacterToEncode(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];
                if (!_allowedCharsBitmap.IsCharacterAllowed(character)) { return i; }
            }
            return -1;
        }

        public override int MaxOutputCharsPerInputChar
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return 8;
            }
        }

        static readonly char[] quote = "&quot;".ToCharArray();
        static readonly char[] ampersand = "&amp;".ToCharArray();
        static readonly char[] lessthan = "&lt;".ToCharArray();
        static readonly char[] greaterthan = "&gt;".ToCharArray();

        public override bool TryEncodeUnicodeScalar(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            if (!Encodes(unicodeScalar)) { return unicodeScalar.TryWriteScalarAsChar(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\"') { return quote.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '&') { return ampersand.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '<') { return lessthan.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '>') { return greaterthan.TryCopyCharacters(buffer, index, out writtenChars); }
            else { return TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, index, out writtenChars); }
        }

        // Writes a scalar value as an HTML-encoded numeric entity.
        private unsafe static bool TryWriteEncodedScalarAsNumericEntity(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            // We're building the characters up in reverse
            char* chars = stackalloc char[8 /* "FFFFFFFF" */];
            int numCharsWritten = 0;
            do
            {
                Debug.Assert(numCharsWritten < 8, "Couldn't have written 8 characters out by this point.");
                // Pop off the last nibble
                chars[numCharsWritten++] = HexUtil.ToHexDigit((int)(unicodeScalar & 0xFU));
                unicodeScalar >>= 4;
            } while (unicodeScalar != 0);

            writtenChars = numCharsWritten + 4; // four chars are &, #, x, and ;
            Debug.Assert(numCharsWritten > 0, "At least one character should've been written.");

            if (numCharsWritten + 4 > buffer.Length)
            {
                writtenChars = 0;
                return false;
            }
            // Finally, write out the HTML-encoded scalar value.
            buffer[index++] = '&';
            buffer[index++] = '#';
            buffer[index++] = 'x';
            do
            {
                buffer[index++] = chars[--numCharsWritten];
            }
            while (numCharsWritten != 0);

            buffer[index++] = ';';
            return true;
        }
    } 

    internal sealed class JavaScriptTextEncoder : JavaScriptEncoder
    {
        private AllowedCharsBitmap _allowedCharsBitmap;

        public readonly static JavaScriptTextEncoder Default = new JavaScriptTextEncoder(new CodePointFilter(UnicodeRanges.BasicLatin));

        public JavaScriptTextEncoder(CodePointFilter filter)
        {
            _allowedCharsBitmap = filter.GetAllowedCharsBitmap();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharsBitmap.ForbidUndefinedCharacters();

            // Forbid characters that are special in HTML.
            // Even though this is a common encoder used by everybody (including URL
            // and JavaScript strings), it's unfortunately common for developers to
            // forget to HTML-encode a string once it has been URL-encoded or
            // JavaScript string-escaped, so this offers extra protection.
            _allowedCharsBitmap.ForbidCharacter('<');
            _allowedCharsBitmap.ForbidCharacter('>');
            _allowedCharsBitmap.ForbidCharacter('&');
            _allowedCharsBitmap.ForbidCharacter('\''); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('\"'); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('+'); // technically not HTML-specific, but can be used to perform UTF7-based attacks

            _allowedCharsBitmap.ForbidCharacter('\\');
            _allowedCharsBitmap.ForbidCharacter('/');
        }

        public JavaScriptTextEncoder(params UnicodeRange[] allowedRanges) : this(new CodePointFilter(allowedRanges))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Encodes(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar)) return true;
            return !_allowedCharsBitmap.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int FindFirstCharacterToEncode(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];
                if (!_allowedCharsBitmap.IsCharacterAllowed(character)) { return i; }
            }
            return -1;
        }

        // The worst case encoding is 6 output chars per input char: [input] U+FFFF -> [output] "\uFFFF"
        // We don't need to worry about astral code points since they're represented as encoded
        // surrogate pairs in the output.
        public override int MaxOutputCharsPerInputChar
        {
            get
            {
                return 6;
            }
        }

        static readonly char[] b = @"\b".ToCharArray();
        static readonly char[] t = @"\t".ToCharArray();
        static readonly char[] n = @"\n".ToCharArray();
        static readonly char[] f = @"\f".ToCharArray();
        static readonly char[] r = @"\r".ToCharArray();
        static readonly char[] forward = @"\/".ToCharArray();
        static readonly char[] back = @"\\".ToCharArray();

        // Writes a scalar value as a JavaScript-escaped character (or sequence of characters).
        // See ECMA-262, Sec. 7.8.4, and ECMA-404, Sec. 9
        // http://www.ecma-international.org/ecma-262/5.1/#sec-7.8.4
        // http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        public override bool TryEncodeUnicodeScalar(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            // ECMA-262 allows encoding U+000B as "\v", but ECMA-404 does not.
            // Both ECMA-262 and ECMA-404 allow encoding U+002F SOLIDUS as "\/".
            // (In ECMA-262 this character is a NonEscape character.)
            // HTML-specific characters (including apostrophe and quotes) will
            // be written out as numeric entities for defense-in-depth.
            // See UnicodeEncoderBase ctor comments for more info.

            if (!Encodes(unicodeScalar)) { return unicodeScalar.TryWriteScalarAsChar(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\b') { return b.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\t') { return t.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\n') { return n.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\f') { return f.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\r') { return r.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '/') { return forward.TryCopyCharacters(buffer, index, out writtenChars); }
            else if (unicodeScalar == '\\') { return back.TryCopyCharacters(buffer, index, out writtenChars); }
            else { return TryWriteEncodedScalarAsNumericEntity(unicodeScalar, buffer, index, out writtenChars); }
        }

        // Writes a scalar value as an JavaScript-escaped character (or sequence of characters).
        private unsafe static bool TryWriteEncodedScalarAsNumericEntity(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar))
            {
                // Convert this back to UTF-16 and write out both characters.
                char leadingSurrogate, trailingSurrogate;
                UnicodeHelpers.GetUtf16SurrogatePairFromAstralScalarValue(unicodeScalar, out leadingSurrogate, out trailingSurrogate);
                int leadingSurrogateCharactersWritten;
                if (TryWriteEncodedSingleCharacter(leadingSurrogate, buffer, index, out leadingSurrogateCharactersWritten) &&
                    TryWriteEncodedSingleCharacter(trailingSurrogate, buffer, index + leadingSurrogateCharactersWritten, out writtenChars)
                )
                {
                    writtenChars += leadingSurrogateCharactersWritten;
                    return true;
                }
                else
                {
                    writtenChars = 0;
                    return false;
                }
            }
            else
            {
                // This is only a single character.
                return TryWriteEncodedSingleCharacter(unicodeScalar, buffer, index, out writtenChars);
            }
        }

        // Writes an encoded scalar value (in the BMP) as a JavaScript-escaped character.
        private static bool TryWriteEncodedSingleCharacter(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            Debug.Assert(!UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar), "The incoming value should've been in the BMP.");
            if (buffer.Length - index < 6)
            {
                writtenChars = 0;
                return false;
            }

            // Encode this as 6 chars "\uFFFF".
            buffer[index++] = '\\';
            buffer[index++] = 'u';
            buffer[index++] = HexUtil.ToHexDigit(unicodeScalar >> 12);
            buffer[index++] = HexUtil.ToHexDigit((int)((unicodeScalar >> 8) & 0xFU));
            buffer[index++] = HexUtil.ToHexDigit((int)((unicodeScalar >> 4) & 0xFU));
            buffer[index++] = HexUtil.ToHexDigit((int)(unicodeScalar & 0xFU));

            writtenChars = 6;
            return true;
        }
    }

    internal sealed class UrlTextEncoder : UrlEncoder
    {
        private AllowedCharsBitmap _allowedCharsBitmap;

        public readonly static UrlTextEncoder Default = new UrlTextEncoder(new CodePointFilter(UnicodeRanges.BasicLatin));

        // We perform UTF8 conversion of input, which means that the worst case is
        // 9 output chars per input char: [input] U+FFFF -> [output] "%XX%YY%ZZ".
        // We don't need to worry about astral code points since they consume 2 input
        // chars to produce 12 output chars "%XX%YY%ZZ%WW", which is 6 output chars per input char.
        public override int MaxOutputCharsPerInputChar
        {
            get
            {
                return 9;
            }
        }

        public UrlTextEncoder(CodePointFilter filter)
        {
            _allowedCharsBitmap = filter.GetAllowedCharsBitmap();

            // Forbid codepoints which aren't mapped to characters or which are otherwise always disallowed
            // (includes categories Cc, Cs, Co, Cn, Zs [except U+0020 SPACE], Zl, Zp)
            _allowedCharsBitmap.ForbidUndefinedCharacters();

            // Forbid characters that are special in HTML.
            // Even though this is a common encoder used by everybody (including URL
            // and JavaScript strings), it's unfortunately common for developers to
            // forget to HTML-encode a string once it has been URL-encoded or
            // JavaScript string-escaped, so this offers extra protection.
            _allowedCharsBitmap.ForbidCharacter('<');
            _allowedCharsBitmap.ForbidCharacter('>');
            _allowedCharsBitmap.ForbidCharacter('&');
            _allowedCharsBitmap.ForbidCharacter('\''); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('\"'); // can be used to escape attributes
            _allowedCharsBitmap.ForbidCharacter('+'); // technically not HTML-specific, but can be used to perform UTF7-based attacks

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
            foreach (char c in forbiddenChars)
            {
                _allowedCharsBitmap.ForbidCharacter(c);
            }

            // Specials (U+FFF0 .. U+FFFF) are forbidden by the definition of 'ucschar' above
            for (int i = 0; i < 16; i++)
            {
                _allowedCharsBitmap.ForbidCharacter((char)(0xFFF0 | i));
            }

            // Supplementary characters are forbidden anyway by the base encoder //TODO: make sure it's true after the changes
        }

        public UrlTextEncoder(params UnicodeRange[] allowedRanges) : this(new CodePointFilter(allowedRanges))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Encodes(int unicodeScalar)
        {
            if (UnicodeHelpers.IsSupplementaryCodePoint(unicodeScalar)) return true;
            return !_allowedCharsBitmap.IsUnicodeScalarAllowed(unicodeScalar);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int FindFirstCharacterToEncode(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];
                if (!_allowedCharsBitmap.IsCharacterAllowed(character)) { return i; }
            }
            return -1;
        }

        public override bool TryEncodeUnicodeScalar(int unicodeScalar, char[] buffer, int index, out int writtenChars)
        {
            if (!Encodes(unicodeScalar)) { return unicodeScalar.TryWriteScalarAsChar(buffer, index, out writtenChars); }

            writtenChars = 0;
            uint asUtf8 = (uint)UnicodeHelpers.GetUtf8RepresentationForScalarValue((uint)unicodeScalar);
            do
            {
                char highNibble, lowNibble;
                HexUtil.WriteHexEncodedByte((byte)asUtf8, out highNibble, out lowNibble);
                if (buffer.Length - index < 3)
                {
                    writtenChars = 0;
                    return false;
                }
                buffer[index++] = '%';
                buffer[index++] = highNibble;
                buffer[index++] = lowNibble;

                writtenChars += 3;
            }
            while ((asUtf8 >>= 8) != 0);
            return true;
        }
    }
}
