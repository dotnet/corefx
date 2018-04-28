// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Don't entity encode high chars (160 to 256)
#define ENTITY_ENCODE_HIGH_ASCII_CHARS

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Net
{
    public static class WebUtility
    {
        // some consts copied from Char / CharUnicodeInfo since we don't have friend access to those types
        private const char HIGH_SURROGATE_START = '\uD800';
        private const char LOW_SURROGATE_START = '\uDC00';
        private const char LOW_SURROGATE_END = '\uDFFF';
        private const int UNICODE_PLANE00_END = 0x00FFFF;
        private const int UNICODE_PLANE01_START = 0x10000;
        private const int UNICODE_PLANE16_END = 0x10FFFF;

        private const int UnicodeReplacementChar = '\uFFFD';

        #region HtmlEncode / HtmlDecode methods

        private static readonly char[] s_htmlEntityEndingChars = new char[] { ';', '&' };

        public static string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Don't create StringBuilder if we don't have anything to encode
            int index = IndexOfHtmlEncodingChars(value, 0);
            if (index == -1)
            {
                return value;
            }

            StringBuilder sb = StringBuilderCache.Acquire(value.Length);
            HtmlEncode(value, index, sb);
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public static void HtmlEncode(string value, TextWriter output)
        {
            output.Write(HtmlEncode(value));
        }

        private static unsafe void HtmlEncode(string value, int index, StringBuilder output)
        {
            Debug.Assert(value != null);
            Debug.Assert(output != null);
            Debug.Assert(0 <= index && index <= value.Length, "0 <= index && index <= value.Length");

            int cch = value.Length - index;
            fixed (char* str = value)
            {
                char* pch = str;
                while (index-- > 0)
                {
                    output.Append(*pch++);
                }

                for (; cch > 0; cch--, pch++)
                {
                    char ch = *pch;
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '<':
                                output.Append("&lt;");
                                break;
                            case '>':
                                output.Append("&gt;");
                                break;
                            case '"':
                                output.Append("&quot;");
                                break;
                            case '\'':
                                output.Append("&#39;");
                                break;
                            case '&':
                                output.Append("&amp;");
                                break;
                            default:
                                output.Append(ch);
                                break;
                        }
                    }
                    else
                    {
                        int valueToEncode = -1; // set to >= 0 if needs to be encoded

#if ENTITY_ENCODE_HIGH_ASCII_CHARS
                        if (ch >= 160 && ch < 256)
                        {
                            // The seemingly arbitrary 160 comes from RFC
                            valueToEncode = ch;
                        }
                        else
#endif // ENTITY_ENCODE_HIGH_ASCII_CHARS
                        if (Char.IsSurrogate(ch))
                        {
                            int scalarValue = GetNextUnicodeScalarValueFromUtf16Surrogate(ref pch, ref cch);
                            if (scalarValue >= UNICODE_PLANE01_START)
                            {
                                valueToEncode = scalarValue;
                            }
                            else
                            {
                                // Don't encode BMP characters (like U+FFFD) since they wouldn't have
                                // been encoded if explicitly present in the string anyway.
                                ch = (char)scalarValue;
                            }
                        }

                        if (valueToEncode >= 0)
                        {
                            // value needs to be encoded
                            output.Append("&#");
                            output.Append(valueToEncode.ToString(CultureInfo.InvariantCulture));
                            output.Append(';');
                        }
                        else
                        {
                            // write out the character directly
                            output.Append(ch);
                        }
                    }
                }
            }
        }

        public static string HtmlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Don't create StringBuilder if we don't have anything to encode
            if (!StringRequiresHtmlDecoding(value))
            {
                return value;
            }

            StringBuilder sb = StringBuilderCache.Acquire(value.Length);
            HtmlDecode(value, sb);
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        public static void HtmlDecode(string value, TextWriter output)
        {
            output.Write(HtmlDecode(value));
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.UInt16.TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.UInt16@)", Justification = "UInt16.TryParse guarantees that result is zero if the parse fails.")]
        private static void HtmlDecode(string value, StringBuilder output)
        {
            Debug.Assert(output != null);

            ReadOnlySpan<char> valueSpan = value.AsSpan();
            int l = value.Length;
            for (int i = 0; i < l; i++)
            {
                char ch = value[i];

                if (ch == '&')
                {
                    // We found a '&'. Now look for the next ';' or '&'. The idea is that
                    // if we find another '&' before finding a ';', then this is not an entity,
                    // and the next '&' might start a real entity (VSWhidbey 275184)
                    int index = value.IndexOfAny(s_htmlEntityEndingChars, i + 1);
                    if (index > 0 && value[index] == ';')
                    {
                        int entityOffset = i + 1;
                        int entityLength = index - entityOffset;

                        if (entityLength > 1 && value[entityOffset] == '#')
                        {
                            // The # syntax can be in decimal or hex, e.g.
                            //      &#229;  --> decimal
                            //      &#xE5;  --> same char in hex
                            // See http://www.w3.org/TR/REC-html40/charset.html#entities

                            bool parsedSuccessfully;
                            uint parsedValue;
                            if (value[entityOffset + 1] == 'x' || value[entityOffset + 1] == 'X')
                            {
                                parsedSuccessfully = uint.TryParse(valueSpan.Slice(entityOffset + 2, entityLength - 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out parsedValue);
                            }
                            else
                            {
                                parsedSuccessfully = uint.TryParse(valueSpan.Slice(entityOffset + 1, entityLength - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue);
                            }

                            if (parsedSuccessfully)
                            {
                                // decoded character must be U+0000 .. U+10FFFF, excluding surrogates
                                parsedSuccessfully = ((parsedValue < HIGH_SURROGATE_START) || (LOW_SURROGATE_END < parsedValue && parsedValue <= UNICODE_PLANE16_END));
                            }

                            if (parsedSuccessfully)
                            {
                                if (parsedValue <= UNICODE_PLANE00_END)
                                {
                                    // single character
                                    output.Append((char)parsedValue);
                                }
                                else
                                {
                                    // multi-character
                                    char leadingSurrogate, trailingSurrogate;
                                    ConvertSmpToUtf16(parsedValue, out leadingSurrogate, out trailingSurrogate);
                                    output.Append(leadingSurrogate);
                                    output.Append(trailingSurrogate);
                                }

                                i = index; // already looked at everything until semicolon
                                continue;
                            }
                        }
                        else
                        {
                            ReadOnlySpan<char> entity = valueSpan.Slice(entityOffset, entityLength);
                            i = index; // already looked at everything until semicolon

                            char entityChar = HtmlEntities.Lookup(entity);
                            if (entityChar != (char)0)
                            {
                                ch = entityChar;
                            }
                            else
                            {
                                output.Append('&');
                                output.Append(entity);
                                output.Append(';');
                                continue;
                            }
                        }
                    }
                }

                output.Append(ch);
            }
        }

        private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos)
        {
            Debug.Assert(0 <= startPos && startPos <= s.Length, "0 <= startPos && startPos <= s.Length");

            int cch = s.Length - startPos;
            fixed (char* str = s)
            {
                for (char* pch = &str[startPos]; cch > 0; pch++, cch--)
                {
                    char ch = *pch;
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '<':
                            case '>':
                            case '"':
                            case '\'':
                            case '&':
                                return s.Length - cch;
                        }
                    }
#if ENTITY_ENCODE_HIGH_ASCII_CHARS
                    else if (ch >= 160 && ch < 256)
                    {
                        return s.Length - cch;
                    }
#endif // ENTITY_ENCODE_HIGH_ASCII_CHARS
                    else if (Char.IsSurrogate(ch))
                    {
                        return s.Length - cch;
                    }
                }
            }

            return -1;
        }

        #endregion

        #region UrlEncode implementation
        
        private static void GetEncodedBytes(byte[] originalBytes, int offset, int count, byte[] expandedBytes)
        {
            int pos = 0;
            int end = offset + count;
            Debug.Assert(offset < end && end <= originalBytes.Length);
            for (int i = offset; i < end; i++)
            {
#if DEBUG
                // Make sure we never overwrite any bytes if originalBytes and
                // expandedBytes refer to the same array
                if (originalBytes == expandedBytes)
                {
                    Debug.Assert(i >= pos);
                }
#endif

                byte b = originalBytes[i];
                char ch = (char)b;
                if (IsUrlSafeChar(ch))
                {
                    expandedBytes[pos++] = b;
                }
                else if (ch == ' ')
                {
                    expandedBytes[pos++] = (byte)'+';
                }
                else
                {
                    expandedBytes[pos++] = (byte)'%';
                    expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
            }
        }

#endregion

#region UrlEncode public methods

        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Already shipped public API; code moved here as part of API consolidation")]
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            int safeCount = 0;
            int spaceCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (IsUrlSafeChar(ch))
                {
                    safeCount++;
                }
                else if (ch == ' ')
                {
                    spaceCount++;
                }
            }

            int unexpandedCount = safeCount + spaceCount;
            if (unexpandedCount == value.Length)
            {
                if (spaceCount != 0)
                {
                    // Only spaces to encode
                    return value.Replace(' ', '+');
                }

                // Nothing to expand
                return value;
            }

            int byteCount = Encoding.UTF8.GetByteCount(value);
            int unsafeByteCount = byteCount - unexpandedCount;
            int byteIndex = unsafeByteCount * 2;

            // Instead of allocating one array of length `byteCount` to store
            // the UTF-8 encoded bytes, and then a second array of length 
            // `3 * byteCount - 2 * unexpandedCount`
            // to store the URL-encoded UTF-8 bytes, we allocate a single array of
            // the latter and encode the data in place, saving the first allocation.
            // We store the UTF-8 bytes to the end of this array, and then URL encode to the
            // beginning of the array.
            byte[] newBytes = new byte[byteCount + byteIndex];
            Encoding.UTF8.GetBytes(value, 0, value.Length, newBytes, byteIndex);
            
            GetEncodedBytes(newBytes, byteIndex, byteCount, newBytes);
            return Encoding.UTF8.GetString(newBytes);
        }

        public static byte[] UrlEncodeToBytes(byte[] value, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(value, offset, count))
            {
                return null;
            }

            bool foundSpaces = false;
            int unsafeCount = 0;

            // count them first
            for (int i = 0; i < count; i++)
            {
                char ch = (char)value[offset + i];

                if (ch == ' ')
                    foundSpaces = true;
                else if (!IsUrlSafeChar(ch))
                    unsafeCount++;
            }

            // nothing to expand?
            if (!foundSpaces && unsafeCount == 0)
            {
                var subarray = new byte[count];
                Buffer.BlockCopy(value, offset, subarray, 0, count);
                return subarray;
            }

            // expand not 'safe' characters into %XX, spaces to +s
            byte[] expandedBytes = new byte[count + unsafeCount * 2];
            GetEncodedBytes(value, offset, count, expandedBytes);
            return expandedBytes;
        }

#endregion

#region UrlDecode implementation

        private static string UrlDecodeInternal(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            int count = value.Length;
            UrlDecoder helper = new UrlDecoder(count, encoding);

            // go through the string's chars collapsing %XX and
            // appending each char as char, with exception of %XX constructs
            // that are appended as bytes
            bool needsDecodingUnsafe = false;
            bool needsDecodingSpaces = false;
            for (int pos = 0; pos < count; pos++)
            {
                char ch = value[pos];

                if (ch == '+')
                {
                    needsDecodingSpaces = true;
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    int h1 = HexToInt(value[pos + 1]);
                    int h2 = HexToInt(value[pos + 2]);

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        byte b = (byte)((h1 << 4) | h2);
                        pos += 2;

                        // don't add as char
                        helper.AddByte(b);
                        needsDecodingUnsafe = true;
                        continue;
                    }
                }

                if ((ch & 0xFF80) == 0)
                    helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                else
                    helper.AddChar(ch);
            }
            
            if (!needsDecodingUnsafe)
            {
                if (needsDecodingSpaces)
                {
                    // Only spaces to decode
                    return value.Replace('+', ' ');
                }

                // Nothing to decode
                return value;
            }

            return helper.GetString();
        }

        private static byte[] UrlDecodeInternal(byte[] bytes, int offset, int count)
        {
            if (!ValidateUrlEncodingParameters(bytes, offset, count))
            {
                return null;
            }

            int decodedBytesCount = 0;
            byte[] decodedBytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                int pos = offset + i;
                byte b = bytes[pos];

                if (b == '+')
                {
                    b = (byte)' ';
                }
                else if (b == '%' && i < count - 2)
                {
                    int h1 = HexToInt((char)bytes[pos + 1]);
                    int h2 = HexToInt((char)bytes[pos + 2]);

                    if (h1 >= 0 && h2 >= 0)
                    {     // valid 2 hex chars
                        b = (byte)((h1 << 4) | h2);
                        i += 2;
                    }
                }

                decodedBytes[decodedBytesCount++] = b;
            }

            if (decodedBytesCount < decodedBytes.Length)
            {
                Array.Resize(ref decodedBytes, decodedBytesCount);
            }

            return decodedBytes;
        }

#endregion

#region UrlDecode public methods


        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Already shipped public API; code moved here as part of API consolidation")]
        public static string UrlDecode(string encodedValue)
        {
            return UrlDecodeInternal(encodedValue, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(byte[] encodedValue, int offset, int count)
        {
            return UrlDecodeInternal(encodedValue, offset, count);
        }

#endregion

#region Helper methods

        // similar to Char.ConvertFromUtf32, but doesn't check arguments or generate strings
        // input is assumed to be an SMP character
        private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
        {
            Debug.Assert(UNICODE_PLANE01_START <= smpChar && smpChar <= UNICODE_PLANE16_END);

            int utf32 = (int)(smpChar - UNICODE_PLANE01_START);
            leadingSurrogate = (char)((utf32 / 0x400) + HIGH_SURROGATE_START);
            trailingSurrogate = (char)((utf32 % 0x400) + LOW_SURROGATE_START);
        }

        private static unsafe int GetNextUnicodeScalarValueFromUtf16Surrogate(ref char* pch, ref int charsRemaining)
        {
            // invariants
            Debug.Assert(charsRemaining >= 1);
            Debug.Assert(Char.IsSurrogate(*pch));

            if (charsRemaining <= 1)
            {
                // not enough characters remaining to resurrect the original scalar value
                return UnicodeReplacementChar;
            }

            char leadingSurrogate = pch[0];
            char trailingSurrogate = pch[1];

            if (Char.IsSurrogatePair(leadingSurrogate, trailingSurrogate))
            {
                // we're going to consume an extra char
                pch++;
                charsRemaining--;

                // below code is from Char.ConvertToUtf32, but without the checks (since we just performed them)
                return (((leadingSurrogate - HIGH_SURROGATE_START) * 0x400) + (trailingSurrogate - LOW_SURROGATE_START) + UNICODE_PLANE01_START);
            }
            else
            {
                // unmatched surrogate
                return UnicodeReplacementChar;
            }
        }

        private static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
            (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
            (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
            -1;
        }

        private static char IntToHex(int n)
        {
            Debug.Assert(n < 0x10);

            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'A');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsUrlSafeChar(char ch)
        {
            // Set of safe chars, from RFC 1738.4 minus '+'
            /*
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
                return true;

            switch (ch)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                case '*':
                case '(':
                case ')':
                    return true;
            }

            return false;
            */
            // Optimized version of the above:

            int code = (int)ch;

            const int safeSpecialCharMask = 0x03FF0000 | // 0..9
                1 << ((int)'!' - 0x20) | // 0x21
                1 << ((int)'(' - 0x20) | // 0x28
                1 << ((int)')' - 0x20) | // 0x29
                1 << ((int)'*' - 0x20) | // 0x2A
                1 << ((int)'-' - 0x20) | // 0x2D
                1 << ((int)'.' - 0x20); // 0x2E

            unchecked
            {
                return ((uint)(code - 'a') <= (uint)('z' - 'a')) ||
                       ((uint)(code - 'A') <= (uint)('Z' - 'A')) ||
                       ((uint)(code - 0x20) <= (uint)('9' - 0x20) && ((1 << (code - 0x20)) & safeSpecialCharMask) != 0) ||
                       (code == (int)'_');
            }
        }

        private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
        {
            if (bytes == null && count == 0)
                return false;
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (offset < 0 || offset > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0 || offset + count > bytes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return true;
        }

        private static bool StringRequiresHtmlDecoding(string s)
        {
            // this string requires html decoding if it contains '&' or a surrogate character
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '&' || Char.IsSurrogate(c))
                {
                    return true;
                }
            }
            return false;
        }

#endregion

        // Internal struct to facilitate URL decoding -- keeps char buffer and byte buffer, allows appending of either chars or bytes
        private struct UrlDecoder
        {
            private int _bufferSize;

            // Accumulate characters in a special array
            private int _numChars;
            private char[] _charBuffer;

            // Accumulate bytes for decoding into characters in a special array
            private int _numBytes;
            private byte[] _byteBuffer;

            // Encoding to convert chars to bytes
            private Encoding _encoding;

            private void FlushBytes()
            {
                Debug.Assert(_numBytes > 0);
                if (_charBuffer == null)
                    _charBuffer = new char[_bufferSize];

                _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
                _numBytes = 0;
            }

            internal UrlDecoder(int bufferSize, Encoding encoding)
            {
                _bufferSize = bufferSize;
                _encoding = encoding;

                _charBuffer = null; // char buffer created on demand

                _numChars = 0;
                _numBytes = 0;
                _byteBuffer = null; // byte buffer created on demand
            }

            internal void AddChar(char ch)
            {
                if (_numBytes > 0)
                    FlushBytes();

                if (_charBuffer == null)
                    _charBuffer = new char[_bufferSize];

                _charBuffer[_numChars++] = ch;
            }

            internal void AddByte(byte b)
            {
                if (_byteBuffer == null)
                    _byteBuffer = new byte[_bufferSize];

                _byteBuffer[_numBytes++] = b;
            }

            internal String GetString()
            {
                if (_numBytes > 0)
                    FlushBytes();

                Debug.Assert(_numChars > 0);
                return new string(_charBuffer, 0, _numChars);
            }
        }

        // helper class for lookup of HTML encoding entities
        private static class HtmlEntities
        {
#if DEBUG
            static HtmlEntities()
            {
                // Make sure the initial capacity for s_lookupTable is correct
                Debug.Assert(s_lookupTable.Count == Count, $"There should be {Count} HTML entities, but {nameof(s_lookupTable)} has {s_lookupTable.Count} of them.");
            }
#endif

            // The list is from http://www.w3.org/TR/REC-html40/sgml/entities.html, except for &apos;, which
            // is defined in http://www.w3.org/TR/2008/REC-xml-20081126/#sec-predefined-ent.

            private const int Count = 253;

            // maps entity strings => unicode chars
            private static readonly LowLevelDictionary<ulong, char> s_lookupTable =
                new LowLevelDictionary<ulong, char>(Count)
                {
                    [1953461617] = '\x0022', // "quot"
                    [7368033] = '\x0026', // "amp"
                    [1936683105] = '\x0027', // "apos"
                    [29804] = '\x003c', // "lt"
                    [29799] = '\x003e', // "gt"
                    [1886610030] = '\x00a0', // "nbsp"
                    [465525302633] = '\x00a1', // "iexcl"
                    [1953391971] = '\x00a2', // "cent"
                    [431349919600] = '\x00a3', // "pound"
                    [121381990856035] = '\x00a4', // "curren"
                    [7234937] = '\x00a5', // "yen"
                    [125762589323874] = '\x00a6', // "brvbar"
                    [1952671091] = '\x00a7', // "sect"
                    [7105909] = '\x00a8', // "uml"
                    [2037411683] = '\x00a9', // "copy"
                    [1717858927] = '\x00aa', // "ordf"
                    [478711734636] = '\x00ab', // "laquo"
                    [7630702] = '\x00ac', // "not"
                    [7956595] = '\x00ad', // "shy"
                    [6776178] = '\x00ae', // "reg"
                    [1919115629] = '\x00af', // "macr"
                    [6776164] = '\x00b0', // "deg"
                    [121416367565936] = '\x00b1', // "plusmn"
                    [846230899] = '\x00b2', // "sup2"
                    [863008115] = '\x00b3', // "sup3"
                    [435745547105] = '\x00b4', // "acute"
                    [478660487533] = '\x00b5', // "micro"
                    [1634886000] = '\x00b6', // "para"
                    [128021774494061] = '\x00b7', // "middot"
                    [465624655203] = '\x00b8', // "cedil"
                    [829453683] = '\x00b9', // "sup1"
                    [1835299439] = '\x00ba', // "ordm"
                    [478711734642] = '\x00bb', // "raquo"
                    [57386725372518] = '\x00bc', // "frac14"
                    [55187702116966] = '\x00bd', // "frac12"
                    [57395315307110] = '\x00be', // "frac34"
                    [128038972256617] = '\x00bf', // "iquest"
                    [111559115433793] = '\x00c0', // "Agrave"
                    [111550860058945] = '\x00c1', // "Aacute"
                    [427121271617] = '\x00c2', // "Acirc"
                    [111481989985345] = '\x00c3', // "Atilde"
                    [1819112769] = '\x00c4', // "Auml"
                    [444234035777] = '\x00c5', // "Aring"
                    [444150334785] = '\x00c6', // "AElig"
                    [119199911732035] = '\x00c7', // "Ccedil"
                    [111559115433797] = '\x00c8', // "Egrave"
                    [111550860058949] = '\x00c9', // "Eacute"
                    [427121271621] = '\x00ca', // "Ecirc"
                    [1819112773] = '\x00cb', // "Euml"
                    [111559115433801] = '\x00cc', // "Igrave"
                    [111550860058953] = '\x00cd', // "Iacute"
                    [427121271625] = '\x00ce', // "Icirc"
                    [1819112777] = '\x00cf', // "Iuml"
                    [4740165] = '\x00d0', // "ETH"
                    [111481989985358] = '\x00d1', // "Ntilde"
                    [111559115433807] = '\x00d2', // "Ograve"
                    [111550860058959] = '\x00d3', // "Oacute"
                    [427121271631] = '\x00d4', // "Ocirc"
                    [111481989985359] = '\x00d5', // "Otilde"
                    [1819112783] = '\x00d6', // "Ouml"
                    [495622908276] = '\x00d7', // "times"
                    [114844765025103] = '\x00d8', // "Oslash"
                    [111559115433813] = '\x00d9', // "Ugrave"
                    [111550860058965] = '\x00da', // "Uacute"
                    [427121271637] = '\x00db', // "Ucirc"
                    [1819112789] = '\x00dc', // "Uuml"
                    [111550860058969] = '\x00dd', // "Yacute"
                    [336388376660] = '\x00de', // "THORN"
                    [444150348403] = '\x00df', // "szlig"
                    [111559115433825] = '\x00e0', // "agrave"
                    [111550860058977] = '\x00e1', // "aacute"
                    [427121271649] = '\x00e2', // "acirc"
                    [111481989985377] = '\x00e3', // "atilde"
                    [1819112801] = '\x00e4', // "auml"
                    [444234035809] = '\x00e5', // "aring"
                    [444150343009] = '\x00e6', // "aelig"
                    [119199911732067] = '\x00e7', // "ccedil"
                    [111559115433829] = '\x00e8', // "egrave"
                    [111550860058981] = '\x00e9', // "eacute"
                    [427121271653] = '\x00ea', // "ecirc"
                    [1819112805] = '\x00eb', // "euml"
                    [111559115433833] = '\x00ec', // "igrave"
                    [111550860058985] = '\x00ed', // "iacute"
                    [427121271657] = '\x00ee', // "icirc"
                    [1819112809] = '\x00ef', // "iuml"
                    [6845541] = '\x00f0', // "eth"
                    [111481989985390] = '\x00f1', // "ntilde"
                    [111559115433839] = '\x00f2', // "ograve"
                    [111550860058991] = '\x00f3', // "oacute"
                    [427121271663] = '\x00f4', // "ocirc"
                    [111481989985391] = '\x00f5', // "otilde"
                    [1819112815] = '\x00f6', // "ouml"
                    [111481940502884] = '\x00f7', // "divide"
                    [114844765025135] = '\x00f8', // "oslash"
                    [111559115433845] = '\x00f9', // "ugrave"
                    [111550860058997] = '\x00fa', // "uacute"
                    [427121271669] = '\x00fb', // "ucirc"
                    [1819112821] = '\x00fc', // "uuml"
                    [111550860059001] = '\x00fd', // "yacute"
                    [474366306420] = '\x00fe', // "thorn"
                    [1819112825] = '\x00ff', // "yuml"
                    [444150334799] = '\x0152', // "OElig"
                    [444150343023] = '\x0153', // "oelig"
                    [121424939410259] = '\x0160', // "Scaron"
                    [121424939410291] = '\x0161', // "scaron"
                    [1819112793] = '\x0178', // "Yuml"
                    [1718578790] = '\x0192', // "fnof"
                    [1668442467] = '\x02c6', // "circ"
                    [435476523380] = '\x02dc', // "tilde"
                    [418364025921] = '\x0391', // "Alpha"
                    [1635018050] = '\x0392', // "Beta"
                    [418447712583] = '\x0393', // "Gamma"
                    [418565088580] = '\x0394', // "Delta"
                    [31084758854496325] = '\x0395', // "Epsilon"
                    [1635018074] = '\x0396', // "Zeta"
                    [6386757] = '\x0397', // "Eta"
                    [418564630612] = '\x0398', // "Theta"
                    [1635020617] = '\x0399', // "Iota"
                    [418498240843] = '\x039a', // "Kappa"
                    [107083775959372] = '\x039b', // "Lambda"
                    [30029] = '\x039c', // "Mu"
                    [30030] = '\x039d', // "Nu"
                    [26968] = '\x039e', // "Xi"
                    [31084784522980687] = '\x039f', // "Omicron"
                    [26960] = '\x03a0', // "Pi"
                    [7301202] = '\x03a1', // "Rho"
                    [418447321427] = '\x03a3', // "Sigma"
                    [7692628] = '\x03a4', // "Tau"
                    [31084758854496341] = '\x03a5', // "Upsilon"
                    [6907984] = '\x03a6', // "Phi"
                    [6907971] = '\x03a7', // "Chi"
                    [6910800] = '\x03a8', // "Psi"
                    [418346528079] = '\x03a9', // "Omega"
                    [418364025953] = '\x03b1', // "alpha"
                    [1635018082] = '\x03b2', // "beta"
                    [418447712615] = '\x03b3', // "gamma"
                    [418565088612] = '\x03b4', // "delta"
                    [31084758854496357] = '\x03b5', // "epsilon"
                    [1635018106] = '\x03b6', // "zeta"
                    [6386789] = '\x03b7', // "eta"
                    [418564630644] = '\x03b8', // "theta"
                    [1635020649] = '\x03b9', // "iota"
                    [418498240875] = '\x03ba', // "kappa"
                    [107083775959404] = '\x03bb', // "lambda"
                    [30061] = '\x03bc', // "mu"
                    [30062] = '\x03bd', // "nu"
                    [27000] = '\x03be', // "xi"
                    [31084784522980719] = '\x03bf', // "omicron"
                    [26992] = '\x03c0', // "pi"
                    [7301234] = '\x03c1', // "rho"
                    [112568633354611] = '\x03c2', // "sigmaf"
                    [418447321459] = '\x03c3', // "sigma"
                    [7692660] = '\x03c4', // "tau"
                    [31084758854496373] = '\x03c5', // "upsilon"
                    [6908016] = '\x03c6', // "phi"
                    [6908003] = '\x03c7', // "chi"
                    [6910832] = '\x03c8', // "psi"
                    [418346528111] = '\x03c9', // "omega"
                    [7888463084717959284] = '\x03d1', // "thetasym"
                    [448445771893] = '\x03d2', // "upsih"
                    [7760240] = '\x03d6', // "piv"
                    [1886613093] = '\x2002', // "ensp"
                    [1886612837] = '\x2003', // "emsp"
                    [123641075951732] = '\x2009', // "thinsp"
                    [1785624442] = '\x200c', // "zwnj"
                    [6977402] = '\x200d', // "zwj"
                    [7172716] = '\x200e', // "lrm"
                    [7171186] = '\x200f', // "rlm"
                    [448612361326] = '\x2013', // "ndash"
                    [448612361325] = '\x2014', // "mdash"
                    [478711739244] = '\x2018', // "lsquo"
                    [478711739250] = '\x2019', // "rsquo"
                    [478711734899] = '\x201a', // "sbquo"
                    [478711735404] = '\x201c', // "ldquo"
                    [478711735410] = '\x201d', // "rdquo"
                    [478711735394] = '\x201e', // "bdquo"
                    [125779852091748] = '\x2020', // "dagger"
                    [125779852091716] = '\x2021', // "Dagger"
                    [1819047266] = '\x2022', // "bull"
                    [123598092920168] = '\x2026', // "hellip"
                    [119200063579504] = '\x2030', // "permil"
                    [435627324016] = '\x2032', // "prime"
                    [435627323984] = '\x2033', // "Prime"
                    [122550204068716] = '\x2039', // "lsaquo"
                    [122550204068722] = '\x203a', // "rsaquo"
                    [435644099695] = '\x203e', // "oline"
                    [465792234086] = '\x2044', // "frasl"
                    [1869772133] = '\x20ac', // "euro"
                    [435526135145] = '\x2111', // "image"
                    [123636629988727] = '\x2118', // "weierp"
                    [1818322290] = '\x211c', // "real"
                    [435475804788] = '\x2122', // "trade"
                    [30814309007584353] = '\x2135', // "alefsym"
                    [1920098668] = '\x2190', // "larr"
                    [1920098677] = '\x2191', // "uarr"
                    [1920098674] = '\x2192', // "rarr"
                    [1920098660] = '\x2193', // "darr"
                    [1920098664] = '\x2194', // "harr"
                    [491545260643] = '\x21b5', // "crarr"
                    [1920090476] = '\x21d0', // "lArr"
                    [1920090485] = '\x21d1', // "uArr"
                    [1920090482] = '\x21d2', // "rArr"
                    [1920090468] = '\x21d3', // "dArr"
                    [1920090472] = '\x21d4', // "hArr"
                    [119212747157350] = '\x2200', // "forall"
                    [1953653104] = '\x2202', // "part"
                    [500152498277] = '\x2203', // "exist"
                    [521644567909] = '\x2205', // "empty"
                    [418430214510] = '\x2207', // "nabla"
                    [1852404585] = '\x2208', // "isin"
                    [474215640942] = '\x2209', // "notin"
                    [26990] = '\x220b', // "ni"
                    [1685025392] = '\x220f', // "prod"
                    [7173491] = '\x2211', // "sum"
                    [495891409261] = '\x2212', // "minus"
                    [128038905278316] = '\x2217', // "lowast"
                    [426969948530] = '\x221a', // "radic"
                    [1886351984] = '\x221d', // "prop"
                    [474214723177] = '\x221e', // "infin"
                    [6778465] = '\x2220', // "ang"
                    [6581857] = '\x2227', // "and"
                    [29295] = '\x2228', // "or"
                    [7364963] = '\x2229', // "cap"
                    [7370083] = '\x222a', // "cup"
                    [7630441] = '\x222b', // "int"
                    [57610315589748] = '\x2234', // "there4"
                    [7170419] = '\x223c', // "sim"
                    [1735290723] = '\x2245', // "cong"
                    [482873013089] = '\x2248', // "asymp"
                    [25966] = '\x2260', // "ne"
                    [508575445349] = '\x2261', // "equiv"
                    [25964] = '\x2264', // "le"
                    [25959] = '\x2265', // "ge"
                    [6452595] = '\x2282', // "sub"
                    [7370099] = '\x2283', // "sup"
                    [1651864430] = '\x2284', // "nsub"
                    [1700951411] = '\x2286', // "sube"
                    [1701868915] = '\x2287', // "supe"
                    [495891279983] = '\x2295', // "oplus"
                    [126879464518767] = '\x2297', // "otimes"
                    [1886545264] = '\x22a5', // "perp"
                    [1953457267] = '\x22c5', // "sdot"
                    [465624720236] = '\x2308', // "lceil"
                    [465624720242] = '\x2309', // "rceil"
                    [125822936311404] = '\x230a', // "lfloor"
                    [125822936311410] = '\x230b', // "rfloor"
                    [1735287148] = '\x2329', // "lang"
                    [1735287154] = '\x232a', // "rang"
                    [8023916] = '\x25ca', // "loz"
                    [126879312998515] = '\x2660', // "spades"
                    [495573101667] = '\x2663', // "clubs"
                    [126943972386152] = '\x2665', // "hearts"
                    [495756339556] = '\x2666', // "diams"
                    // To compute the key for new entries, use:
                    // string entity = "diams"; ulong key = 0; for (int i = 0; i < entity.Length; i++) key |= (ulong)entity[i] << 8 * i; Console.WriteLine(key);
                };

            public static char Lookup(ReadOnlySpan<char> entity)
            {
                // To avoid an allocation, keys of type "ulong" are used in the lookup table.
                // Since all entity strings comprise 8 characters or less and are ASCII-only, they "fit" into an ulong (8 bytes).
                // The ulong key is the reversed single-byte character representation of the actual entity string.
                if (entity.Length <= 8)
                {
                    ulong key = 0;
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] > 0xFF)
                        {
                            return default;
                        }

                        key |= (ulong)entity[i] << 8 * i;
                    }

                    s_lookupTable.TryGetValue(key, out char result);
                    return result;
                }
                else
                {
                    // Currently, there are no entities that are longer than 8 characters.
                    return default;
                } 
            }
        }
    }
}
