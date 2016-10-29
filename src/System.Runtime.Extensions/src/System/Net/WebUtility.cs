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
            if (String.IsNullOrEmpty(value))
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
            if (String.IsNullOrEmpty(value))
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

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.UInt16.TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.UInt16@)", Justification = "UInt16.TryParse guarantees that result is zero if the parse fails.")]
        private static void HtmlDecode(string value, StringBuilder output)
        {
            Debug.Assert(output != null);

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
                                parsedSuccessfully = uint.TryParse(value.Substring(entityOffset + 2, entityLength - 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out parsedValue);
                            }
                            else
                            {
                                parsedSuccessfully = uint.TryParse(value.Substring(entityOffset + 1, entityLength - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out parsedValue);
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
                            string entity = value.Substring(entityOffset, entityLength);
                            i = index; // already looked at everything until semicolon

                            string entityString = HtmlEntities.Lookup(entity);
                            if (entityString != null)
                            {
                                output.Append(entityString);
                                continue;
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

            return ((uint)(code - 'a') <= (uint)('z' - 'a')) ||
                   ((uint)(code - 'A') <= (uint)('Z' - 'A')) ||
                   ((uint)(code - 0x20) <= (uint)('9' - 0x20) && ((1 << (code - 0x20)) & safeSpecialCharMask) != 0) ||
                   (code == (int)'_');
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

            // The list is from https://www.w3.org/TR/html5/syntax.html#named-character-references

            private const int Count = 2125;

            // maps entity strings => unicode chars
            private static readonly LowLevelDictionary<string, string> s_lookupTable =
                new LowLevelDictionary<string, string>(Count, StringComparer.Ordinal)
                {
                    ["Aacute"] = "\x00c1",
                    ["aacute"] = "\x00e1",
                    ["Abreve"] = "\x0102",
                    ["abreve"] = "\x0103",
                    ["ac"] = "\x223e",
                    ["acd"] = "\x223f",
                    ["acE"] = "\x223e\x0333",
                    ["Acirc"] = "\x00c2",
                    ["acirc"] = "\x00e2",
                    ["acute"] = "\x00b4",
                    ["Acy"] = "\x0410",
                    ["acy"] = "\x0430",
                    ["AElig"] = "\x00c6",
                    ["aelig"] = "\x00e6",
                    ["af"] = "\x2061",
                    ["Afr"] = "\xd835\xdd04",
                    ["afr"] = "\xd835\xdd1e",
                    ["Agrave"] = "\x00c0",
                    ["agrave"] = "\x00e0",
                    ["alefsym"] = "\x2135",
                    ["aleph"] = "\x2135",
                    ["Alpha"] = "\x0391",
                    ["alpha"] = "\x03b1",
                    ["Amacr"] = "\x0100",
                    ["amacr"] = "\x0101",
                    ["amalg"] = "\x2a3f",
                    ["AMP"] = "\x0026",
                    ["amp"] = "\x0026",
                    ["And"] = "\x2a53",
                    ["and"] = "\x2227",
                    ["andand"] = "\x2a55",
                    ["andd"] = "\x2a5c",
                    ["andslope"] = "\x2a58",
                    ["andv"] = "\x2a5a",
                    ["ang"] = "\x2220",
                    ["ange"] = "\x29a4",
                    ["angle"] = "\x2220",
                    ["angmsd"] = "\x2221",
                    ["angmsdaa"] = "\x29a8",
                    ["angmsdab"] = "\x29a9",
                    ["angmsdac"] = "\x29aa",
                    ["angmsdad"] = "\x29ab",
                    ["angmsdae"] = "\x29ac",
                    ["angmsdaf"] = "\x29ad",
                    ["angmsdag"] = "\x29ae",
                    ["angmsdah"] = "\x29af",
                    ["angrt"] = "\x221f",
                    ["angrtvb"] = "\x22be",
                    ["angrtvbd"] = "\x299d",
                    ["angsph"] = "\x2222",
                    ["angst"] = "\x00c5",
                    ["angzarr"] = "\x237c",
                    ["Aogon"] = "\x0104",
                    ["aogon"] = "\x0105",
                    ["Aopf"] = "\xd835\xdd38",
                    ["aopf"] = "\xd835\xdd52",
                    ["ap"] = "\x2248",
                    ["apacir"] = "\x2a6f",
                    ["apE"] = "\x2a70",
                    ["ape"] = "\x224a",
                    ["apid"] = "\x224b",
                    ["apos"] = "\x0027",
                    ["ApplyFunction"] = "\x2061",
                    ["approx"] = "\x2248",
                    ["approxeq"] = "\x224a",
                    ["Aring"] = "\x00c5",
                    ["aring"] = "\x00e5",
                    ["Ascr"] = "\xd835\xdc9c",
                    ["ascr"] = "\xd835\xdcb6",
                    ["Assign"] = "\x2254",
                    ["ast"] = "\x002a",
                    ["asymp"] = "\x2248",
                    ["asympeq"] = "\x224d",
                    ["Atilde"] = "\x00c3",
                    ["atilde"] = "\x00e3",
                    ["Auml"] = "\x00c4",
                    ["auml"] = "\x00e4",
                    ["awconint"] = "\x2233",
                    ["awint"] = "\x2a11",
                    ["backcong"] = "\x224c",
                    ["backepsilon"] = "\x03f6",
                    ["backprime"] = "\x2035",
                    ["backsim"] = "\x223d",
                    ["backsimeq"] = "\x22cd",
                    ["Backslash"] = "\x2216",
                    ["Barv"] = "\x2ae7",
                    ["barvee"] = "\x22bd",
                    ["Barwed"] = "\x2306",
                    ["barwed"] = "\x2305",
                    ["barwedge"] = "\x2305",
                    ["bbrk"] = "\x23b5",
                    ["bbrktbrk"] = "\x23b6",
                    ["bcong"] = "\x224c",
                    ["Bcy"] = "\x0411",
                    ["bcy"] = "\x0431",
                    ["bdquo"] = "\x201e",
                    ["becaus"] = "\x2235",
                    ["Because"] = "\x2235",
                    ["because"] = "\x2235",
                    ["bemptyv"] = "\x29b0",
                    ["bepsi"] = "\x03f6",
                    ["bernou"] = "\x212c",
                    ["Bernoullis"] = "\x212c",
                    ["Beta"] = "\x0392",
                    ["beta"] = "\x03b2",
                    ["beth"] = "\x2136",
                    ["between"] = "\x226c",
                    ["Bfr"] = "\xd835\xdd05",
                    ["bfr"] = "\xd835\xdd1f",
                    ["bigcap"] = "\x22c2",
                    ["bigcirc"] = "\x25ef",
                    ["bigcup"] = "\x22c3",
                    ["bigodot"] = "\x2a00",
                    ["bigoplus"] = "\x2a01",
                    ["bigotimes"] = "\x2a02",
                    ["bigsqcup"] = "\x2a06",
                    ["bigstar"] = "\x2605",
                    ["bigtriangledown"] = "\x25bd",
                    ["bigtriangleup"] = "\x25b3",
                    ["biguplus"] = "\x2a04",
                    ["bigvee"] = "\x22c1",
                    ["bigwedge"] = "\x22c0",
                    ["bkarow"] = "\x290d",
                    ["blacklozenge"] = "\x29eb",
                    ["blacksquare"] = "\x25aa",
                    ["blacktriangle"] = "\x25b4",
                    ["blacktriangledown"] = "\x25be",
                    ["blacktriangleleft"] = "\x25c2",
                    ["blacktriangleright"] = "\x25b8",
                    ["blank"] = "\x2423",
                    ["blk12"] = "\x2592",
                    ["blk14"] = "\x2591",
                    ["blk34"] = "\x2593",
                    ["block"] = "\x2588",
                    ["bne"] = "\x003d\x20e5",
                    ["bnequiv"] = "\x2261\x20e5",
                    ["bNot"] = "\x2aed",
                    ["bnot"] = "\x2310",
                    ["Bopf"] = "\xd835\xdd39",
                    ["bopf"] = "\xd835\xdd53",
                    ["bot"] = "\x22a5",
                    ["bottom"] = "\x22a5",
                    ["bowtie"] = "\x22c8",
                    ["boxbox"] = "\x29c9",
                    ["boxDL"] = "\x2557",
                    ["boxDl"] = "\x2556",
                    ["boxdL"] = "\x2555",
                    ["boxdl"] = "\x2510",
                    ["boxDR"] = "\x2554",
                    ["boxDr"] = "\x2553",
                    ["boxdR"] = "\x2552",
                    ["boxdr"] = "\x250c",
                    ["boxH"] = "\x2550",
                    ["boxh"] = "\x2500",
                    ["boxHD"] = "\x2566",
                    ["boxHd"] = "\x2564",
                    ["boxhD"] = "\x2565",
                    ["boxhd"] = "\x252c",
                    ["boxHU"] = "\x2569",
                    ["boxHu"] = "\x2567",
                    ["boxhU"] = "\x2568",
                    ["boxhu"] = "\x2534",
                    ["boxminus"] = "\x229f",
                    ["boxplus"] = "\x229e",
                    ["boxtimes"] = "\x22a0",
                    ["boxUL"] = "\x255d",
                    ["boxUl"] = "\x255c",
                    ["boxuL"] = "\x255b",
                    ["boxul"] = "\x2518",
                    ["boxUR"] = "\x255a",
                    ["boxUr"] = "\x2559",
                    ["boxuR"] = "\x2558",
                    ["boxur"] = "\x2514",
                    ["boxV"] = "\x2551",
                    ["boxv"] = "\x2502",
                    ["boxVH"] = "\x256c",
                    ["boxVh"] = "\x256b",
                    ["boxvH"] = "\x256a",
                    ["boxvh"] = "\x253c",
                    ["boxVL"] = "\x2563",
                    ["boxVl"] = "\x2562",
                    ["boxvL"] = "\x2561",
                    ["boxvl"] = "\x2524",
                    ["boxVR"] = "\x2560",
                    ["boxVr"] = "\x255f",
                    ["boxvR"] = "\x255e",
                    ["boxvr"] = "\x251c",
                    ["bprime"] = "\x2035",
                    ["Breve"] = "\x02d8",
                    ["breve"] = "\x02d8",
                    ["brvbar"] = "\x00a6",
                    ["Bscr"] = "\x212c",
                    ["bscr"] = "\xd835\xdcb7",
                    ["bsemi"] = "\x204f",
                    ["bsim"] = "\x223d",
                    ["bsime"] = "\x22cd",
                    ["bsol"] = "\x005c",
                    ["bsolb"] = "\x29c5",
                    ["bsolhsub"] = "\x27c8",
                    ["bull"] = "\x2022",
                    ["bullet"] = "\x2022",
                    ["bump"] = "\x224e",
                    ["bumpE"] = "\x2aae",
                    ["bumpe"] = "\x224f",
                    ["Bumpeq"] = "\x224e",
                    ["bumpeq"] = "\x224f",
                    ["Cacute"] = "\x0106",
                    ["cacute"] = "\x0107",
                    ["Cap"] = "\x22d2",
                    ["cap"] = "\x2229",
                    ["capand"] = "\x2a44",
                    ["capbrcup"] = "\x2a49",
                    ["capcap"] = "\x2a4b",
                    ["capcup"] = "\x2a47",
                    ["capdot"] = "\x2a40",
                    ["CapitalDifferentialD"] = "\x2145",
                    ["caps"] = "\x2229\xfe00",
                    ["caret"] = "\x2041",
                    ["caron"] = "\x02c7",
                    ["Cayleys"] = "\x212d",
                    ["ccaps"] = "\x2a4d",
                    ["Ccaron"] = "\x010c",
                    ["ccaron"] = "\x010d",
                    ["Ccedil"] = "\x00c7",
                    ["ccedil"] = "\x00e7",
                    ["Ccirc"] = "\x0108",
                    ["ccirc"] = "\x0109",
                    ["Cconint"] = "\x2230",
                    ["ccups"] = "\x2a4c",
                    ["ccupssm"] = "\x2a50",
                    ["Cdot"] = "\x010a",
                    ["cdot"] = "\x010b",
                    ["cedil"] = "\x00b8",
                    ["Cedilla"] = "\x00b8",
                    ["cemptyv"] = "\x29b2",
                    ["cent"] = "\x00a2",
                    ["CenterDot"] = "\x00b7",
                    ["centerdot"] = "\x00b7",
                    ["Cfr"] = "\x212d",
                    ["cfr"] = "\xd835\xdd20",
                    ["CHcy"] = "\x0427",
                    ["chcy"] = "\x0447",
                    ["check"] = "\x2713",
                    ["checkmark"] = "\x2713",
                    ["Chi"] = "\x03a7",
                    ["chi"] = "\x03c7",
                    ["cir"] = "\x25cb",
                    ["circ"] = "\x02c6",
                    ["circeq"] = "\x2257",
                    ["circlearrowleft"] = "\x21ba",
                    ["circlearrowright"] = "\x21bb",
                    ["circledast"] = "\x229b",
                    ["circledcirc"] = "\x229a",
                    ["circleddash"] = "\x229d",
                    ["CircleDot"] = "\x2299",
                    ["circledR"] = "\x00ae",
                    ["circledS"] = "\x24c8",
                    ["CircleMinus"] = "\x2296",
                    ["CirclePlus"] = "\x2295",
                    ["CircleTimes"] = "\x2297",
                    ["cirE"] = "\x29c3",
                    ["cire"] = "\x2257",
                    ["cirfnint"] = "\x2a10",
                    ["cirmid"] = "\x2aef",
                    ["cirscir"] = "\x29c2",
                    ["ClockwiseContourIntegral"] = "\x2232",
                    ["CloseCurlyDoubleQuote"] = "\x201d",
                    ["CloseCurlyQuote"] = "\x2019",
                    ["clubs"] = "\x2663",
                    ["clubsuit"] = "\x2663",
                    ["Colon"] = "\x2237",
                    ["colon"] = "\x003a",
                    ["Colone"] = "\x2a74",
                    ["colone"] = "\x2254",
                    ["coloneq"] = "\x2254",
                    ["comma"] = "\x002c",
                    ["commat"] = "\x0040",
                    ["comp"] = "\x2201",
                    ["compfn"] = "\x2218",
                    ["complement"] = "\x2201",
                    ["complexes"] = "\x2102",
                    ["cong"] = "\x2245",
                    ["congdot"] = "\x2a6d",
                    ["Congruent"] = "\x2261",
                    ["Conint"] = "\x222f",
                    ["conint"] = "\x222e",
                    ["ContourIntegral"] = "\x222e",
                    ["Copf"] = "\x2102",
                    ["copf"] = "\xd835\xdd54",
                    ["coprod"] = "\x2210",
                    ["Coproduct"] = "\x2210",
                    ["COPY"] = "\x00a9",
                    ["copy"] = "\x00a9",
                    ["copysr"] = "\x2117",
                    ["CounterClockwiseContourIntegral"] = "\x2233",
                    ["crarr"] = "\x21b5",
                    ["Cross"] = "\x2a2f",
                    ["cross"] = "\x2717",
                    ["Cscr"] = "\xd835\xdc9e",
                    ["cscr"] = "\xd835\xdcb8",
                    ["csub"] = "\x2acf",
                    ["csube"] = "\x2ad1",
                    ["csup"] = "\x2ad0",
                    ["csupe"] = "\x2ad2",
                    ["ctdot"] = "\x22ef",
                    ["cudarrl"] = "\x2938",
                    ["cudarrr"] = "\x2935",
                    ["cuepr"] = "\x22de",
                    ["cuesc"] = "\x22df",
                    ["cularr"] = "\x21b6",
                    ["cularrp"] = "\x293d",
                    ["Cup"] = "\x22d3",
                    ["cup"] = "\x222a",
                    ["cupbrcap"] = "\x2a48",
                    ["CupCap"] = "\x224d",
                    ["cupcap"] = "\x2a46",
                    ["cupcup"] = "\x2a4a",
                    ["cupdot"] = "\x228d",
                    ["cupor"] = "\x2a45",
                    ["cups"] = "\x222a\xfe00",
                    ["curarr"] = "\x21b7",
                    ["curarrm"] = "\x293c",
                    ["curlyeqprec"] = "\x22de",
                    ["curlyeqsucc"] = "\x22df",
                    ["curlyvee"] = "\x22ce",
                    ["curlywedge"] = "\x22cf",
                    ["curren"] = "\x00a4",
                    ["curvearrowleft"] = "\x21b6",
                    ["curvearrowright"] = "\x21b7",
                    ["cuvee"] = "\x22ce",
                    ["cuwed"] = "\x22cf",
                    ["cwconint"] = "\x2232",
                    ["cwint"] = "\x2231",
                    ["cylcty"] = "\x232d",
                    ["Dagger"] = "\x2021",
                    ["dagger"] = "\x2020",
                    ["daleth"] = "\x2138",
                    ["Darr"] = "\x21a1",
                    ["dArr"] = "\x21d3",
                    ["darr"] = "\x2193",
                    ["dash"] = "\x2010",
                    ["Dashv"] = "\x2ae4",
                    ["dashv"] = "\x22a3",
                    ["dbkarow"] = "\x290f",
                    ["dblac"] = "\x02dd",
                    ["Dcaron"] = "\x010e",
                    ["dcaron"] = "\x010f",
                    ["Dcy"] = "\x0414",
                    ["dcy"] = "\x0434",
                    ["DD"] = "\x2145",
                    ["dd"] = "\x2146",
                    ["ddagger"] = "\x2021",
                    ["ddarr"] = "\x21ca",
                    ["DDotrahd"] = "\x2911",
                    ["ddotseq"] = "\x2a77",
                    ["deg"] = "\x00b0",
                    ["Del"] = "\x2207",
                    ["Delta"] = "\x0394",
                    ["delta"] = "\x03b4",
                    ["demptyv"] = "\x29b1",
                    ["dfisht"] = "\x297f",
                    ["Dfr"] = "\xd835\xdd07",
                    ["dfr"] = "\xd835\xdd21",
                    ["dHar"] = "\x2965",
                    ["dharl"] = "\x21c3",
                    ["dharr"] = "\x21c2",
                    ["DiacriticalAcute"] = "\x00b4",
                    ["DiacriticalDot"] = "\x02d9",
                    ["DiacriticalDoubleAcute"] = "\x02dd",
                    ["DiacriticalGrave"] = "\x0060",
                    ["DiacriticalTilde"] = "\x02dc",
                    ["diam"] = "\x22c4",
                    ["Diamond"] = "\x22c4",
                    ["diamond"] = "\x22c4",
                    ["diamondsuit"] = "\x2666",
                    ["diams"] = "\x2666",
                    ["die"] = "\x00a8",
                    ["DifferentialD"] = "\x2146",
                    ["digamma"] = "\x03dd",
                    ["disin"] = "\x22f2",
                    ["div"] = "\x00f7",
                    ["divide"] = "\x00f7",
                    ["divideontimes"] = "\x22c7",
                    ["divonx"] = "\x22c7",
                    ["DJcy"] = "\x0402",
                    ["djcy"] = "\x0452",
                    ["dlcorn"] = "\x231e",
                    ["dlcrop"] = "\x230d",
                    ["dollar"] = "\x0024",
                    ["Dopf"] = "\xd835\xdd3b",
                    ["dopf"] = "\xd835\xdd55",
                    ["Dot"] = "\x00a8",
                    ["dot"] = "\x02d9",
                    ["DotDot"] = "\x20dc",
                    ["doteq"] = "\x2250",
                    ["doteqdot"] = "\x2251",
                    ["DotEqual"] = "\x2250",
                    ["dotminus"] = "\x2238",
                    ["dotplus"] = "\x2214",
                    ["dotsquare"] = "\x22a1",
                    ["doublebarwedge"] = "\x2306",
                    ["DoubleContourIntegral"] = "\x222f",
                    ["DoubleDot"] = "\x00a8",
                    ["DoubleDownArrow"] = "\x21d3",
                    ["DoubleLeftArrow"] = "\x21d0",
                    ["DoubleLeftRightArrow"] = "\x21d4",
                    ["DoubleLeftTee"] = "\x2ae4",
                    ["DoubleLongLeftArrow"] = "\x27f8",
                    ["DoubleLongLeftRightArrow"] = "\x27fa",
                    ["DoubleLongRightArrow"] = "\x27f9",
                    ["DoubleRightArrow"] = "\x21d2",
                    ["DoubleRightTee"] = "\x22a8",
                    ["DoubleUpArrow"] = "\x21d1",
                    ["DoubleUpDownArrow"] = "\x21d5",
                    ["DoubleVerticalBar"] = "\x2225",
                    ["DownArrow"] = "\x2193",
                    ["Downarrow"] = "\x21d3",
                    ["downarrow"] = "\x2193",
                    ["DownArrowBar"] = "\x2913",
                    ["DownArrowUpArrow"] = "\x21f5",
                    ["DownBreve"] = "\x0311",
                    ["downdownarrows"] = "\x21ca",
                    ["downharpoonleft"] = "\x21c3",
                    ["downharpoonright"] = "\x21c2",
                    ["DownLeftRightVector"] = "\x2950",
                    ["DownLeftTeeVector"] = "\x295e",
                    ["DownLeftVector"] = "\x21bd",
                    ["DownLeftVectorBar"] = "\x2956",
                    ["DownRightTeeVector"] = "\x295f",
                    ["DownRightVector"] = "\x21c1",
                    ["DownRightVectorBar"] = "\x2957",
                    ["DownTee"] = "\x22a4",
                    ["DownTeeArrow"] = "\x21a7",
                    ["drbkarow"] = "\x2910",
                    ["drcorn"] = "\x231f",
                    ["drcrop"] = "\x230c",
                    ["Dscr"] = "\xd835\xdc9f",
                    ["dscr"] = "\xd835\xdcb9",
                    ["DScy"] = "\x0405",
                    ["dscy"] = "\x0455",
                    ["dsol"] = "\x29f6",
                    ["Dstrok"] = "\x0110",
                    ["dstrok"] = "\x0111",
                    ["dtdot"] = "\x22f1",
                    ["dtri"] = "\x25bf",
                    ["dtrif"] = "\x25be",
                    ["duarr"] = "\x21f5",
                    ["duhar"] = "\x296f",
                    ["dwangle"] = "\x29a6",
                    ["DZcy"] = "\x040f",
                    ["dzcy"] = "\x045f",
                    ["dzigrarr"] = "\x27ff",
                    ["Eacute"] = "\x00c9",
                    ["eacute"] = "\x00e9",
                    ["easter"] = "\x2a6e",
                    ["Ecaron"] = "\x011a",
                    ["ecaron"] = "\x011b",
                    ["ecir"] = "\x2256",
                    ["Ecirc"] = "\x00ca",
                    ["ecirc"] = "\x00ea",
                    ["ecolon"] = "\x2255",
                    ["Ecy"] = "\x042d",
                    ["ecy"] = "\x044d",
                    ["eDDot"] = "\x2a77",
                    ["Edot"] = "\x0116",
                    ["eDot"] = "\x2251",
                    ["edot"] = "\x0117",
                    ["ee"] = "\x2147",
                    ["efDot"] = "\x2252",
                    ["Efr"] = "\xd835\xdd08",
                    ["efr"] = "\xd835\xdd22",
                    ["eg"] = "\x2a9a",
                    ["Egrave"] = "\x00c8",
                    ["egrave"] = "\x00e8",
                    ["egs"] = "\x2a96",
                    ["egsdot"] = "\x2a98",
                    ["el"] = "\x2a99",
                    ["Element"] = "\x2208",
                    ["elinters"] = "\x23e7",
                    ["ell"] = "\x2113",
                    ["els"] = "\x2a95",
                    ["elsdot"] = "\x2a97",
                    ["Emacr"] = "\x0112",
                    ["emacr"] = "\x0113",
                    ["empty"] = "\x2205",
                    ["emptyset"] = "\x2205",
                    ["EmptySmallSquare"] = "\x25fb",
                    ["emptyv"] = "\x2205",
                    ["EmptyVerySmallSquare"] = "\x25ab",
                    ["emsp"] = "\x2003",
                    ["emsp13"] = "\x2004",
                    ["emsp14"] = "\x2005",
                    ["ENG"] = "\x014a",
                    ["eng"] = "\x014b",
                    ["ensp"] = "\x2002",
                    ["Eogon"] = "\x0118",
                    ["eogon"] = "\x0119",
                    ["Eopf"] = "\xd835\xdd3c",
                    ["eopf"] = "\xd835\xdd56",
                    ["epar"] = "\x22d5",
                    ["eparsl"] = "\x29e3",
                    ["eplus"] = "\x2a71",
                    ["epsi"] = "\x03b5",
                    ["Epsilon"] = "\x0395",
                    ["epsilon"] = "\x03b5",
                    ["epsiv"] = "\x03f5",
                    ["eqcirc"] = "\x2256",
                    ["eqcolon"] = "\x2255",
                    ["eqsim"] = "\x2242",
                    ["eqslantgtr"] = "\x2a96",
                    ["eqslantless"] = "\x2a95",
                    ["Equal"] = "\x2a75",
                    ["equals"] = "\x003d",
                    ["EqualTilde"] = "\x2242",
                    ["equest"] = "\x225f",
                    ["Equilibrium"] = "\x21cc",
                    ["equiv"] = "\x2261",
                    ["equivDD"] = "\x2a78",
                    ["eqvparsl"] = "\x29e5",
                    ["erarr"] = "\x2971",
                    ["erDot"] = "\x2253",
                    ["Escr"] = "\x2130",
                    ["escr"] = "\x212f",
                    ["esdot"] = "\x2250",
                    ["Esim"] = "\x2a73",
                    ["esim"] = "\x2242",
                    ["Eta"] = "\x0397",
                    ["eta"] = "\x03b7",
                    ["ETH"] = "\x00d0",
                    ["eth"] = "\x00f0",
                    ["Euml"] = "\x00cb",
                    ["euml"] = "\x00eb",
                    ["euro"] = "\x20ac",
                    ["excl"] = "\x0021",
                    ["exist"] = "\x2203",
                    ["Exists"] = "\x2203",
                    ["expectation"] = "\x2130",
                    ["ExponentialE"] = "\x2147",
                    ["exponentiale"] = "\x2147",
                    ["fallingdotseq"] = "\x2252",
                    ["Fcy"] = "\x0424",
                    ["fcy"] = "\x0444",
                    ["female"] = "\x2640",
                    ["ffilig"] = "\xfb03",
                    ["fflig"] = "\xfb00",
                    ["ffllig"] = "\xfb04",
                    ["Ffr"] = "\xd835\xdd09",
                    ["ffr"] = "\xd835\xdd23",
                    ["filig"] = "\xfb01",
                    ["FilledSmallSquare"] = "\x25fc",
                    ["FilledVerySmallSquare"] = "\x25aa",
                    ["fjlig"] = "\x0066\x006a",
                    ["flat"] = "\x266d",
                    ["fllig"] = "\xfb02",
                    ["fltns"] = "\x25b1",
                    ["fnof"] = "\x0192",
                    ["Fopf"] = "\xd835\xdd3d",
                    ["fopf"] = "\xd835\xdd57",
                    ["ForAll"] = "\x2200",
                    ["forall"] = "\x2200",
                    ["fork"] = "\x22d4",
                    ["forkv"] = "\x2ad9",
                    ["Fouriertrf"] = "\x2131",
                    ["fpartint"] = "\x2a0d",
                    ["frac12"] = "\x00bd",
                    ["frac13"] = "\x2153",
                    ["frac14"] = "\x00bc",
                    ["frac15"] = "\x2155",
                    ["frac16"] = "\x2159",
                    ["frac18"] = "\x215b",
                    ["frac23"] = "\x2154",
                    ["frac25"] = "\x2156",
                    ["frac34"] = "\x00be",
                    ["frac35"] = "\x2157",
                    ["frac38"] = "\x215c",
                    ["frac45"] = "\x2158",
                    ["frac56"] = "\x215a",
                    ["frac58"] = "\x215d",
                    ["frac78"] = "\x215e",
                    ["frasl"] = "\x2044",
                    ["frown"] = "\x2322",
                    ["Fscr"] = "\x2131",
                    ["fscr"] = "\xd835\xdcbb",
                    ["gacute"] = "\x01f5",
                    ["Gamma"] = "\x0393",
                    ["gamma"] = "\x03b3",
                    ["Gammad"] = "\x03dc",
                    ["gammad"] = "\x03dd",
                    ["gap"] = "\x2a86",
                    ["Gbreve"] = "\x011e",
                    ["gbreve"] = "\x011f",
                    ["Gcedil"] = "\x0122",
                    ["Gcirc"] = "\x011c",
                    ["gcirc"] = "\x011d",
                    ["Gcy"] = "\x0413",
                    ["gcy"] = "\x0433",
                    ["Gdot"] = "\x0120",
                    ["gdot"] = "\x0121",
                    ["gE"] = "\x2267",
                    ["ge"] = "\x2265",
                    ["gEl"] = "\x2a8c",
                    ["gel"] = "\x22db",
                    ["geq"] = "\x2265",
                    ["geqq"] = "\x2267",
                    ["geqslant"] = "\x2a7e",
                    ["ges"] = "\x2a7e",
                    ["gescc"] = "\x2aa9",
                    ["gesdot"] = "\x2a80",
                    ["gesdoto"] = "\x2a82",
                    ["gesdotol"] = "\x2a84",
                    ["gesl"] = "\x22db\xfe00",
                    ["gesles"] = "\x2a94",
                    ["Gfr"] = "\xd835\xdd0a",
                    ["gfr"] = "\xd835\xdd24",
                    ["Gg"] = "\x22d9",
                    ["gg"] = "\x226b",
                    ["ggg"] = "\x22d9",
                    ["gimel"] = "\x2137",
                    ["GJcy"] = "\x0403",
                    ["gjcy"] = "\x0453",
                    ["gl"] = "\x2277",
                    ["gla"] = "\x2aa5",
                    ["glE"] = "\x2a92",
                    ["glj"] = "\x2aa4",
                    ["gnap"] = "\x2a8a",
                    ["gnapprox"] = "\x2a8a",
                    ["gnE"] = "\x2269",
                    ["gne"] = "\x2a88",
                    ["gneq"] = "\x2a88",
                    ["gneqq"] = "\x2269",
                    ["gnsim"] = "\x22e7",
                    ["Gopf"] = "\xd835\xdd3e",
                    ["gopf"] = "\xd835\xdd58",
                    ["grave"] = "\x0060",
                    ["GreaterEqual"] = "\x2265",
                    ["GreaterEqualLess"] = "\x22db",
                    ["GreaterFullEqual"] = "\x2267",
                    ["GreaterGreater"] = "\x2aa2",
                    ["GreaterLess"] = "\x2277",
                    ["GreaterSlantEqual"] = "\x2a7e",
                    ["GreaterTilde"] = "\x2273",
                    ["Gscr"] = "\xd835\xdca2",
                    ["gscr"] = "\x210a",
                    ["gsim"] = "\x2273",
                    ["gsime"] = "\x2a8e",
                    ["gsiml"] = "\x2a90",
                    ["GT"] = "\x003e",
                    ["Gt"] = "\x226b",
                    ["gt"] = "\x003e",
                    ["gtcc"] = "\x2aa7",
                    ["gtcir"] = "\x2a7a",
                    ["gtdot"] = "\x22d7",
                    ["gtlPar"] = "\x2995",
                    ["gtquest"] = "\x2a7c",
                    ["gtrapprox"] = "\x2a86",
                    ["gtrarr"] = "\x2978",
                    ["gtrdot"] = "\x22d7",
                    ["gtreqless"] = "\x22db",
                    ["gtreqqless"] = "\x2a8c",
                    ["gtrless"] = "\x2277",
                    ["gtrsim"] = "\x2273",
                    ["gvertneqq"] = "\x2269\xfe00",
                    ["gvnE"] = "\x2269\xfe00",
                    ["Hacek"] = "\x02c7",
                    ["hairsp"] = "\x200a",
                    ["half"] = "\x00bd",
                    ["hamilt"] = "\x210b",
                    ["HARDcy"] = "\x042a",
                    ["hardcy"] = "\x044a",
                    ["hArr"] = "\x21d4",
                    ["harr"] = "\x2194",
                    ["harrcir"] = "\x2948",
                    ["harrw"] = "\x21ad",
                    ["Hat"] = "\x005e",
                    ["hbar"] = "\x210f",
                    ["Hcirc"] = "\x0124",
                    ["hcirc"] = "\x0125",
                    ["hearts"] = "\x2665",
                    ["heartsuit"] = "\x2665",
                    ["hellip"] = "\x2026",
                    ["hercon"] = "\x22b9",
                    ["Hfr"] = "\x210c",
                    ["hfr"] = "\xd835\xdd25",
                    ["HilbertSpace"] = "\x210b",
                    ["hksearow"] = "\x2925",
                    ["hkswarow"] = "\x2926",
                    ["hoarr"] = "\x21ff",
                    ["homtht"] = "\x223b",
                    ["hookleftarrow"] = "\x21a9",
                    ["hookrightarrow"] = "\x21aa",
                    ["Hopf"] = "\x210d",
                    ["hopf"] = "\xd835\xdd59",
                    ["horbar"] = "\x2015",
                    ["HorizontalLine"] = "\x2500",
                    ["Hscr"] = "\x210b",
                    ["hscr"] = "\xd835\xdcbd",
                    ["hslash"] = "\x210f",
                    ["Hstrok"] = "\x0126",
                    ["hstrok"] = "\x0127",
                    ["HumpDownHump"] = "\x224e",
                    ["HumpEqual"] = "\x224f",
                    ["hybull"] = "\x2043",
                    ["hyphen"] = "\x2010",
                    ["Iacute"] = "\x00cd",
                    ["iacute"] = "\x00ed",
                    ["ic"] = "\x2063",
                    ["Icirc"] = "\x00ce",
                    ["icirc"] = "\x00ee",
                    ["Icy"] = "\x0418",
                    ["icy"] = "\x0438",
                    ["Idot"] = "\x0130",
                    ["IEcy"] = "\x0415",
                    ["iecy"] = "\x0435",
                    ["iexcl"] = "\x00a1",
                    ["iff"] = "\x21d4",
                    ["Ifr"] = "\x2111",
                    ["ifr"] = "\xd835\xdd26",
                    ["Igrave"] = "\x00cc",
                    ["igrave"] = "\x00ec",
                    ["ii"] = "\x2148",
                    ["iiiint"] = "\x2a0c",
                    ["iiint"] = "\x222d",
                    ["iinfin"] = "\x29dc",
                    ["iiota"] = "\x2129",
                    ["IJlig"] = "\x0132",
                    ["ijlig"] = "\x0133",
                    ["Im"] = "\x2111",
                    ["Imacr"] = "\x012a",
                    ["imacr"] = "\x012b",
                    ["image"] = "\x2111",
                    ["ImaginaryI"] = "\x2148",
                    ["imagline"] = "\x2110",
                    ["imagpart"] = "\x2111",
                    ["imath"] = "\x0131",
                    ["imof"] = "\x22b7",
                    ["imped"] = "\x01b5",
                    ["Implies"] = "\x21d2",
                    ["in"] = "\x2208",
                    ["incare"] = "\x2105",
                    ["infin"] = "\x221e",
                    ["infintie"] = "\x29dd",
                    ["inodot"] = "\x0131",
                    ["Int"] = "\x222c",
                    ["int"] = "\x222b",
                    ["intcal"] = "\x22ba",
                    ["integers"] = "\x2124",
                    ["Integral"] = "\x222b",
                    ["intercal"] = "\x22ba",
                    ["Intersection"] = "\x22c2",
                    ["intlarhk"] = "\x2a17",
                    ["intprod"] = "\x2a3c",
                    ["InvisibleComma"] = "\x2063",
                    ["InvisibleTimes"] = "\x2062",
                    ["IOcy"] = "\x0401",
                    ["iocy"] = "\x0451",
                    ["Iogon"] = "\x012e",
                    ["iogon"] = "\x012f",
                    ["Iopf"] = "\xd835\xdd40",
                    ["iopf"] = "\xd835\xdd5a",
                    ["Iota"] = "\x0399",
                    ["iota"] = "\x03b9",
                    ["iprod"] = "\x2a3c",
                    ["iquest"] = "\x00bf",
                    ["Iscr"] = "\x2110",
                    ["iscr"] = "\xd835\xdcbe",
                    ["isin"] = "\x2208",
                    ["isindot"] = "\x22f5",
                    ["isinE"] = "\x22f9",
                    ["isins"] = "\x22f4",
                    ["isinsv"] = "\x22f3",
                    ["isinv"] = "\x2208",
                    ["it"] = "\x2062",
                    ["Itilde"] = "\x0128",
                    ["itilde"] = "\x0129",
                    ["Iukcy"] = "\x0406",
                    ["iukcy"] = "\x0456",
                    ["Iuml"] = "\x00cf",
                    ["iuml"] = "\x00ef",
                    ["Jcirc"] = "\x0134",
                    ["jcirc"] = "\x0135",
                    ["Jcy"] = "\x0419",
                    ["jcy"] = "\x0439",
                    ["Jfr"] = "\xd835\xdd0d",
                    ["jfr"] = "\xd835\xdd27",
                    ["jmath"] = "\x0237",
                    ["Jopf"] = "\xd835\xdd41",
                    ["jopf"] = "\xd835\xdd5b",
                    ["Jscr"] = "\xd835\xdca5",
                    ["jscr"] = "\xd835\xdcbf",
                    ["Jsercy"] = "\x0408",
                    ["jsercy"] = "\x0458",
                    ["Jukcy"] = "\x0404",
                    ["jukcy"] = "\x0454",
                    ["Kappa"] = "\x039a",
                    ["kappa"] = "\x03ba",
                    ["kappav"] = "\x03f0",
                    ["Kcedil"] = "\x0136",
                    ["kcedil"] = "\x0137",
                    ["Kcy"] = "\x041a",
                    ["kcy"] = "\x043a",
                    ["Kfr"] = "\xd835\xdd0e",
                    ["kfr"] = "\xd835\xdd28",
                    ["kgreen"] = "\x0138",
                    ["KHcy"] = "\x0425",
                    ["khcy"] = "\x0445",
                    ["KJcy"] = "\x040c",
                    ["kjcy"] = "\x045c",
                    ["Kopf"] = "\xd835\xdd42",
                    ["kopf"] = "\xd835\xdd5c",
                    ["Kscr"] = "\xd835\xdca6",
                    ["kscr"] = "\xd835\xdcc0",
                    ["lAarr"] = "\x21da",
                    ["Lacute"] = "\x0139",
                    ["lacute"] = "\x013a",
                    ["laemptyv"] = "\x29b4",
                    ["lagran"] = "\x2112",
                    ["Lambda"] = "\x039b",
                    ["lambda"] = "\x03bb",
                    ["Lang"] = "\x27ea",
                    ["lang"] = "\x27e8",
                    ["langd"] = "\x2991",
                    ["langle"] = "\x27e8",
                    ["lap"] = "\x2a85",
                    ["Laplacetrf"] = "\x2112",
                    ["laquo"] = "\x00ab",
                    ["Larr"] = "\x219e",
                    ["lArr"] = "\x21d0",
                    ["larr"] = "\x2190",
                    ["larrb"] = "\x21e4",
                    ["larrbfs"] = "\x291f",
                    ["larrfs"] = "\x291d",
                    ["larrhk"] = "\x21a9",
                    ["larrlp"] = "\x21ab",
                    ["larrpl"] = "\x2939",
                    ["larrsim"] = "\x2973",
                    ["larrtl"] = "\x21a2",
                    ["lat"] = "\x2aab",
                    ["lAtail"] = "\x291b",
                    ["latail"] = "\x2919",
                    ["late"] = "\x2aad",
                    ["lates"] = "\x2aad\xfe00",
                    ["lBarr"] = "\x290e",
                    ["lbarr"] = "\x290c",
                    ["lbbrk"] = "\x2772",
                    ["lbrace"] = "\x007b",
                    ["lbrack"] = "\x005b",
                    ["lbrke"] = "\x298b",
                    ["lbrksld"] = "\x298f",
                    ["lbrkslu"] = "\x298d",
                    ["Lcaron"] = "\x013d",
                    ["lcaron"] = "\x013e",
                    ["Lcedil"] = "\x013b",
                    ["lcedil"] = "\x013c",
                    ["lceil"] = "\x2308",
                    ["lcub"] = "\x007b",
                    ["Lcy"] = "\x041b",
                    ["lcy"] = "\x043b",
                    ["ldca"] = "\x2936",
                    ["ldquo"] = "\x201c",
                    ["ldquor"] = "\x201e",
                    ["ldrdhar"] = "\x2967",
                    ["ldrushar"] = "\x294b",
                    ["ldsh"] = "\x21b2",
                    ["lE"] = "\x2266",
                    ["le"] = "\x2264",
                    ["LeftAngleBracket"] = "\x27e8",
                    ["LeftArrow"] = "\x2190",
                    ["Leftarrow"] = "\x21d0",
                    ["leftarrow"] = "\x2190",
                    ["LeftArrowBar"] = "\x21e4",
                    ["LeftArrowRightArrow"] = "\x21c6",
                    ["leftarrowtail"] = "\x21a2",
                    ["LeftCeiling"] = "\x2308",
                    ["LeftDoubleBracket"] = "\x27e6",
                    ["LeftDownTeeVector"] = "\x2961",
                    ["LeftDownVector"] = "\x21c3",
                    ["LeftDownVectorBar"] = "\x2959",
                    ["LeftFloor"] = "\x230a",
                    ["leftharpoondown"] = "\x21bd",
                    ["leftharpoonup"] = "\x21bc",
                    ["leftleftarrows"] = "\x21c7",
                    ["LeftRightArrow"] = "\x2194",
                    ["Leftrightarrow"] = "\x21d4",
                    ["leftrightarrow"] = "\x2194",
                    ["leftrightarrows"] = "\x21c6",
                    ["leftrightharpoons"] = "\x21cb",
                    ["leftrightsquigarrow"] = "\x21ad",
                    ["LeftRightVector"] = "\x294e",
                    ["LeftTee"] = "\x22a3",
                    ["LeftTeeArrow"] = "\x21a4",
                    ["LeftTeeVector"] = "\x295a",
                    ["leftthreetimes"] = "\x22cb",
                    ["LeftTriangle"] = "\x22b2",
                    ["LeftTriangleBar"] = "\x29cf",
                    ["LeftTriangleEqual"] = "\x22b4",
                    ["LeftUpDownVector"] = "\x2951",
                    ["LeftUpTeeVector"] = "\x2960",
                    ["LeftUpVector"] = "\x21bf",
                    ["LeftUpVectorBar"] = "\x2958",
                    ["LeftVector"] = "\x21bc",
                    ["LeftVectorBar"] = "\x2952",
                    ["lEg"] = "\x2a8b",
                    ["leg"] = "\x22da",
                    ["leq"] = "\x2264",
                    ["leqq"] = "\x2266",
                    ["leqslant"] = "\x2a7d",
                    ["les"] = "\x2a7d",
                    ["lescc"] = "\x2aa8",
                    ["lesdot"] = "\x2a7f",
                    ["lesdoto"] = "\x2a81",
                    ["lesdotor"] = "\x2a83",
                    ["lesg"] = "\x22da\xfe00",
                    ["lesges"] = "\x2a93",
                    ["lessapprox"] = "\x2a85",
                    ["lessdot"] = "\x22d6",
                    ["lesseqgtr"] = "\x22da",
                    ["lesseqqgtr"] = "\x2a8b",
                    ["LessEqualGreater"] = "\x22da",
                    ["LessFullEqual"] = "\x2266",
                    ["LessGreater"] = "\x2276",
                    ["lessgtr"] = "\x2276",
                    ["LessLess"] = "\x2aa1",
                    ["lesssim"] = "\x2272",
                    ["LessSlantEqual"] = "\x2a7d",
                    ["LessTilde"] = "\x2272",
                    ["lfisht"] = "\x297c",
                    ["lfloor"] = "\x230a",
                    ["Lfr"] = "\xd835\xdd0f",
                    ["lfr"] = "\xd835\xdd29",
                    ["lg"] = "\x2276",
                    ["lgE"] = "\x2a91",
                    ["lHar"] = "\x2962",
                    ["lhard"] = "\x21bd",
                    ["lharu"] = "\x21bc",
                    ["lharul"] = "\x296a",
                    ["lhblk"] = "\x2584",
                    ["LJcy"] = "\x0409",
                    ["ljcy"] = "\x0459",
                    ["Ll"] = "\x22d8",
                    ["ll"] = "\x226a",
                    ["llarr"] = "\x21c7",
                    ["llcorner"] = "\x231e",
                    ["Lleftarrow"] = "\x21da",
                    ["llhard"] = "\x296b",
                    ["lltri"] = "\x25fa",
                    ["Lmidot"] = "\x013f",
                    ["lmidot"] = "\x0140",
                    ["lmoust"] = "\x23b0",
                    ["lmoustache"] = "\x23b0",
                    ["lnap"] = "\x2a89",
                    ["lnapprox"] = "\x2a89",
                    ["lnE"] = "\x2268",
                    ["lne"] = "\x2a87",
                    ["lneq"] = "\x2a87",
                    ["lneqq"] = "\x2268",
                    ["lnsim"] = "\x22e6",
                    ["loang"] = "\x27ec",
                    ["loarr"] = "\x21fd",
                    ["lobrk"] = "\x27e6",
                    ["LongLeftArrow"] = "\x27f5",
                    ["Longleftarrow"] = "\x27f8",
                    ["longleftarrow"] = "\x27f5",
                    ["LongLeftRightArrow"] = "\x27f7",
                    ["Longleftrightarrow"] = "\x27fa",
                    ["longleftrightarrow"] = "\x27f7",
                    ["longmapsto"] = "\x27fc",
                    ["LongRightArrow"] = "\x27f6",
                    ["Longrightarrow"] = "\x27f9",
                    ["longrightarrow"] = "\x27f6",
                    ["looparrowleft"] = "\x21ab",
                    ["looparrowright"] = "\x21ac",
                    ["lopar"] = "\x2985",
                    ["Lopf"] = "\xd835\xdd43",
                    ["lopf"] = "\xd835\xdd5d",
                    ["loplus"] = "\x2a2d",
                    ["lotimes"] = "\x2a34",
                    ["lowast"] = "\x2217",
                    ["lowbar"] = "\x005f",
                    ["LowerLeftArrow"] = "\x2199",
                    ["LowerRightArrow"] = "\x2198",
                    ["loz"] = "\x25ca",
                    ["lozenge"] = "\x25ca",
                    ["lozf"] = "\x29eb",
                    ["lpar"] = "\x0028",
                    ["lparlt"] = "\x2993",
                    ["lrarr"] = "\x21c6",
                    ["lrcorner"] = "\x231f",
                    ["lrhar"] = "\x21cb",
                    ["lrhard"] = "\x296d",
                    ["lrm"] = "\x200e",
                    ["lrtri"] = "\x22bf",
                    ["lsaquo"] = "\x2039",
                    ["Lscr"] = "\x2112",
                    ["lscr"] = "\xd835\xdcc1",
                    ["Lsh"] = "\x21b0",
                    ["lsh"] = "\x21b0",
                    ["lsim"] = "\x2272",
                    ["lsime"] = "\x2a8d",
                    ["lsimg"] = "\x2a8f",
                    ["lsqb"] = "\x005b",
                    ["lsquo"] = "\x2018",
                    ["lsquor"] = "\x201a",
                    ["Lstrok"] = "\x0141",
                    ["lstrok"] = "\x0142",
                    ["LT"] = "\x003c",
                    ["Lt"] = "\x226a",
                    ["lt"] = "\x003c",
                    ["ltcc"] = "\x2aa6",
                    ["ltcir"] = "\x2a79",
                    ["ltdot"] = "\x22d6",
                    ["lthree"] = "\x22cb",
                    ["ltimes"] = "\x22c9",
                    ["ltlarr"] = "\x2976",
                    ["ltquest"] = "\x2a7b",
                    ["ltri"] = "\x25c3",
                    ["ltrie"] = "\x22b4",
                    ["ltrif"] = "\x25c2",
                    ["ltrPar"] = "\x2996",
                    ["lurdshar"] = "\x294a",
                    ["luruhar"] = "\x2966",
                    ["lvertneqq"] = "\x2268\xfe00",
                    ["lvnE"] = "\x2268\xfe00",
                    ["macr"] = "\x00af",
                    ["male"] = "\x2642",
                    ["malt"] = "\x2720",
                    ["maltese"] = "\x2720",
                    ["Map"] = "\x2905",
                    ["map"] = "\x21a6",
                    ["mapsto"] = "\x21a6",
                    ["mapstodown"] = "\x21a7",
                    ["mapstoleft"] = "\x21a4",
                    ["mapstoup"] = "\x21a5",
                    ["marker"] = "\x25ae",
                    ["mcomma"] = "\x2a29",
                    ["Mcy"] = "\x041c",
                    ["mcy"] = "\x043c",
                    ["mdash"] = "\x2014",
                    ["mDDot"] = "\x223a",
                    ["measuredangle"] = "\x2221",
                    ["MediumSpace"] = "\x205f",
                    ["Mellintrf"] = "\x2133",
                    ["Mfr"] = "\xd835\xdd10",
                    ["mfr"] = "\xd835\xdd2a",
                    ["mho"] = "\x2127",
                    ["micro"] = "\x00b5",
                    ["mid"] = "\x2223",
                    ["midast"] = "\x002a",
                    ["midcir"] = "\x2af0",
                    ["middot"] = "\x00b7",
                    ["minus"] = "\x2212",
                    ["minusb"] = "\x229f",
                    ["minusd"] = "\x2238",
                    ["minusdu"] = "\x2a2a",
                    ["MinusPlus"] = "\x2213",
                    ["mlcp"] = "\x2adb",
                    ["mldr"] = "\x2026",
                    ["mnplus"] = "\x2213",
                    ["models"] = "\x22a7",
                    ["Mopf"] = "\xd835\xdd44",
                    ["mopf"] = "\xd835\xdd5e",
                    ["mp"] = "\x2213",
                    ["Mscr"] = "\x2133",
                    ["mscr"] = "\xd835\xdcc2",
                    ["mstpos"] = "\x223e",
                    ["Mu"] = "\x039c",
                    ["mu"] = "\x03bc",
                    ["multimap"] = "\x22b8",
                    ["mumap"] = "\x22b8",
                    ["nabla"] = "\x2207",
                    ["Nacute"] = "\x0143",
                    ["nacute"] = "\x0144",
                    ["nang"] = "\x2220\x20d2",
                    ["nap"] = "\x2249",
                    ["napE"] = "\x2a70\x0338",
                    ["napid"] = "\x224b\x0338",
                    ["napos"] = "\x0149",
                    ["napprox"] = "\x2249",
                    ["natur"] = "\x266e",
                    ["natural"] = "\x266e",
                    ["naturals"] = "\x2115",
                    ["nbsp"] = "\x00a0",
                    ["nbump"] = "\x224e\x0338",
                    ["nbumpe"] = "\x224f\x0338",
                    ["ncap"] = "\x2a43",
                    ["Ncaron"] = "\x0147",
                    ["ncaron"] = "\x0148",
                    ["Ncedil"] = "\x0145",
                    ["ncedil"] = "\x0146",
                    ["ncong"] = "\x2247",
                    ["ncongdot"] = "\x2a6d\x0338",
                    ["ncup"] = "\x2a42",
                    ["Ncy"] = "\x041d",
                    ["ncy"] = "\x043d",
                    ["ndash"] = "\x2013",
                    ["ne"] = "\x2260",
                    ["nearhk"] = "\x2924",
                    ["neArr"] = "\x21d7",
                    ["nearr"] = "\x2197",
                    ["nearrow"] = "\x2197",
                    ["nedot"] = "\x2250\x0338",
                    ["NegativeMediumSpace"] = "\x200b",
                    ["NegativeThickSpace"] = "\x200b",
                    ["NegativeThinSpace"] = "\x200b",
                    ["NegativeVeryThinSpace"] = "\x200b",
                    ["nequiv"] = "\x2262",
                    ["nesear"] = "\x2928",
                    ["nesim"] = "\x2242\x0338",
                    ["NestedGreaterGreater"] = "\x226b",
                    ["NestedLessLess"] = "\x226a",
                    ["NewLine"] = "\x000a",
                    ["nexist"] = "\x2204",
                    ["nexists"] = "\x2204",
                    ["Nfr"] = "\xd835\xdd11",
                    ["nfr"] = "\xd835\xdd2b",
                    ["ngE"] = "\x2267\x0338",
                    ["nge"] = "\x2271",
                    ["ngeq"] = "\x2271",
                    ["ngeqq"] = "\x2267\x0338",
                    ["ngeqslant"] = "\x2a7e\x0338",
                    ["nges"] = "\x2a7e\x0338",
                    ["nGg"] = "\x22d9\x0338",
                    ["ngsim"] = "\x2275",
                    ["nGt"] = "\x226b\x20d2",
                    ["ngt"] = "\x226f",
                    ["ngtr"] = "\x226f",
                    ["nGtv"] = "\x226b\x0338",
                    ["nhArr"] = "\x21ce",
                    ["nharr"] = "\x21ae",
                    ["nhpar"] = "\x2af2",
                    ["ni"] = "\x220b",
                    ["nis"] = "\x22fc",
                    ["nisd"] = "\x22fa",
                    ["niv"] = "\x220b",
                    ["NJcy"] = "\x040a",
                    ["njcy"] = "\x045a",
                    ["nlArr"] = "\x21cd",
                    ["nlarr"] = "\x219a",
                    ["nldr"] = "\x2025",
                    ["nlE"] = "\x2266\x0338",
                    ["nle"] = "\x2270",
                    ["nLeftarrow"] = "\x21cd",
                    ["nleftarrow"] = "\x219a",
                    ["nLeftrightarrow"] = "\x21ce",
                    ["nleftrightarrow"] = "\x21ae",
                    ["nleq"] = "\x2270",
                    ["nleqq"] = "\x2266\x0338",
                    ["nleqslant"] = "\x2a7d\x0338",
                    ["nles"] = "\x2a7d\x0338",
                    ["nless"] = "\x226e",
                    ["nLl"] = "\x22d8\x0338",
                    ["nlsim"] = "\x2274",
                    ["nLt"] = "\x226a\x20d2",
                    ["nlt"] = "\x226e",
                    ["nltri"] = "\x22ea",
                    ["nltrie"] = "\x22ec",
                    ["nLtv"] = "\x226a\x0338",
                    ["nmid"] = "\x2224",
                    ["NoBreak"] = "\x2060",
                    ["NonBreakingSpace"] = "\x00a0",
                    ["Nopf"] = "\x2115",
                    ["nopf"] = "\xd835\xdd5f",
                    ["Not"] = "\x2aec",
                    ["not"] = "\x00ac",
                    ["NotCongruent"] = "\x2262",
                    ["NotCupCap"] = "\x226d",
                    ["NotDoubleVerticalBar"] = "\x2226",
                    ["NotElement"] = "\x2209",
                    ["NotEqual"] = "\x2260",
                    ["NotEqualTilde"] = "\x2242\x0338",
                    ["NotExists"] = "\x2204",
                    ["NotGreater"] = "\x226f",
                    ["NotGreaterEqual"] = "\x2271",
                    ["NotGreaterFullEqual"] = "\x2267\x0338",
                    ["NotGreaterGreater"] = "\x226b\x0338",
                    ["NotGreaterLess"] = "\x2279",
                    ["NotGreaterSlantEqual"] = "\x2a7e\x0338",
                    ["NotGreaterTilde"] = "\x2275",
                    ["NotHumpDownHump"] = "\x224e\x0338",
                    ["NotHumpEqual"] = "\x224f\x0338",
                    ["notin"] = "\x2209",
                    ["notindot"] = "\x22f5\x0338",
                    ["notinE"] = "\x22f9\x0338",
                    ["notinva"] = "\x2209",
                    ["notinvb"] = "\x22f7",
                    ["notinvc"] = "\x22f6",
                    ["NotLeftTriangle"] = "\x22ea",
                    ["NotLeftTriangleBar"] = "\x29cf\x0338",
                    ["NotLeftTriangleEqual"] = "\x22ec",
                    ["NotLess"] = "\x226e",
                    ["NotLessEqual"] = "\x2270",
                    ["NotLessGreater"] = "\x2278",
                    ["NotLessLess"] = "\x226a\x0338",
                    ["NotLessSlantEqual"] = "\x2a7d\x0338",
                    ["NotLessTilde"] = "\x2274",
                    ["NotNestedGreaterGreater"] = "\x2aa2\x0338",
                    ["NotNestedLessLess"] = "\x2aa1\x0338",
                    ["notni"] = "\x220c",
                    ["notniva"] = "\x220c",
                    ["notnivb"] = "\x22fe",
                    ["notnivc"] = "\x22fd",
                    ["NotPrecedes"] = "\x2280",
                    ["NotPrecedesEqual"] = "\x2aaf\x0338",
                    ["NotPrecedesSlantEqual"] = "\x22e0",
                    ["NotReverseElement"] = "\x220c",
                    ["NotRightTriangle"] = "\x22eb",
                    ["NotRightTriangleBar"] = "\x29d0\x0338",
                    ["NotRightTriangleEqual"] = "\x22ed",
                    ["NotSquareSubset"] = "\x228f\x0338",
                    ["NotSquareSubsetEqual"] = "\x22e2",
                    ["NotSquareSuperset"] = "\x2290\x0338",
                    ["NotSquareSupersetEqual"] = "\x22e3",
                    ["NotSubset"] = "\x2282\x20d2",
                    ["NotSubsetEqual"] = "\x2288",
                    ["NotSucceeds"] = "\x2281",
                    ["NotSucceedsEqual"] = "\x2ab0\x0338",
                    ["NotSucceedsSlantEqual"] = "\x22e1",
                    ["NotSucceedsTilde"] = "\x227f\x0338",
                    ["NotSuperset"] = "\x2283\x20d2",
                    ["NotSupersetEqual"] = "\x2289",
                    ["NotTilde"] = "\x2241",
                    ["NotTildeEqual"] = "\x2244",
                    ["NotTildeFullEqual"] = "\x2247",
                    ["NotTildeTilde"] = "\x2249",
                    ["NotVerticalBar"] = "\x2224",
                    ["npar"] = "\x2226",
                    ["nparallel"] = "\x2226",
                    ["nparsl"] = "\x2afd\x20e5",
                    ["npart"] = "\x2202\x0338",
                    ["npolint"] = "\x2a14",
                    ["npr"] = "\x2280",
                    ["nprcue"] = "\x22e0",
                    ["npre"] = "\x2aaf\x0338",
                    ["nprec"] = "\x2280",
                    ["npreceq"] = "\x2aaf\x0338",
                    ["nrArr"] = "\x21cf",
                    ["nrarr"] = "\x219b",
                    ["nrarrc"] = "\x2933\x0338",
                    ["nrarrw"] = "\x219d\x0338",
                    ["nRightarrow"] = "\x21cf",
                    ["nrightarrow"] = "\x219b",
                    ["nrtri"] = "\x22eb",
                    ["nrtrie"] = "\x22ed",
                    ["nsc"] = "\x2281",
                    ["nsccue"] = "\x22e1",
                    ["nsce"] = "\x2ab0\x0338",
                    ["Nscr"] = "\xd835\xdca9",
                    ["nscr"] = "\xd835\xdcc3",
                    ["nshortmid"] = "\x2224",
                    ["nshortparallel"] = "\x2226",
                    ["nsim"] = "\x2241",
                    ["nsime"] = "\x2244",
                    ["nsimeq"] = "\x2244",
                    ["nsmid"] = "\x2224",
                    ["nspar"] = "\x2226",
                    ["nsqsube"] = "\x22e2",
                    ["nsqsupe"] = "\x22e3",
                    ["nsub"] = "\x2284",
                    ["nsubE"] = "\x2ac5\x0338",
                    ["nsube"] = "\x2288",
                    ["nsubset"] = "\x2282\x20d2",
                    ["nsubseteq"] = "\x2288",
                    ["nsubseteqq"] = "\x2ac5\x0338",
                    ["nsucc"] = "\x2281",
                    ["nsucceq"] = "\x2ab0\x0338",
                    ["nsup"] = "\x2285",
                    ["nsupE"] = "\x2ac6\x0338",
                    ["nsupe"] = "\x2289",
                    ["nsupset"] = "\x2283\x20d2",
                    ["nsupseteq"] = "\x2289",
                    ["nsupseteqq"] = "\x2ac6\x0338",
                    ["ntgl"] = "\x2279",
                    ["Ntilde"] = "\x00d1",
                    ["ntilde"] = "\x00f1",
                    ["ntlg"] = "\x2278",
                    ["ntriangleleft"] = "\x22ea",
                    ["ntrianglelefteq"] = "\x22ec",
                    ["ntriangleright"] = "\x22eb",
                    ["ntrianglerighteq"] = "\x22ed",
                    ["Nu"] = "\x039d",
                    ["nu"] = "\x03bd",
                    ["num"] = "\x0023",
                    ["numero"] = "\x2116",
                    ["numsp"] = "\x2007",
                    ["nvap"] = "\x224d\x20d2",
                    ["nVDash"] = "\x22af",
                    ["nVdash"] = "\x22ae",
                    ["nvDash"] = "\x22ad",
                    ["nvdash"] = "\x22ac",
                    ["nvge"] = "\x2265\x20d2",
                    ["nvgt"] = "\x003e\x20d2",
                    ["nvHarr"] = "\x2904",
                    ["nvinfin"] = "\x29de",
                    ["nvlArr"] = "\x2902",
                    ["nvle"] = "\x2264\x20d2",
                    ["nvlt"] = "\x003c\x20d2",
                    ["nvltrie"] = "\x22b4\x20d2",
                    ["nvrArr"] = "\x2903",
                    ["nvrtrie"] = "\x22b5\x20d2",
                    ["nvsim"] = "\x223c\x20d2",
                    ["nwarhk"] = "\x2923",
                    ["nwArr"] = "\x21d6",
                    ["nwarr"] = "\x2196",
                    ["nwarrow"] = "\x2196",
                    ["nwnear"] = "\x2927",
                    ["Oacute"] = "\x00d3",
                    ["oacute"] = "\x00f3",
                    ["oast"] = "\x229b",
                    ["ocir"] = "\x229a",
                    ["Ocirc"] = "\x00d4",
                    ["ocirc"] = "\x00f4",
                    ["Ocy"] = "\x041e",
                    ["ocy"] = "\x043e",
                    ["odash"] = "\x229d",
                    ["Odblac"] = "\x0150",
                    ["odblac"] = "\x0151",
                    ["odiv"] = "\x2a38",
                    ["odot"] = "\x2299",
                    ["odsold"] = "\x29bc",
                    ["OElig"] = "\x0152",
                    ["oelig"] = "\x0153",
                    ["ofcir"] = "\x29bf",
                    ["Ofr"] = "\xd835\xdd12",
                    ["ofr"] = "\xd835\xdd2c",
                    ["ogon"] = "\x02db",
                    ["Ograve"] = "\x00d2",
                    ["ograve"] = "\x00f2",
                    ["ogt"] = "\x29c1",
                    ["ohbar"] = "\x29b5",
                    ["ohm"] = "\x03a9",
                    ["oint"] = "\x222e",
                    ["olarr"] = "\x21ba",
                    ["olcir"] = "\x29be",
                    ["olcross"] = "\x29bb",
                    ["oline"] = "\x203e",
                    ["olt"] = "\x29c0",
                    ["Omacr"] = "\x014c",
                    ["omacr"] = "\x014d",
                    ["Omega"] = "\x03a9",
                    ["omega"] = "\x03c9",
                    ["Omicron"] = "\x039f",
                    ["omicron"] = "\x03bf",
                    ["omid"] = "\x29b6",
                    ["ominus"] = "\x2296",
                    ["Oopf"] = "\xd835\xdd46",
                    ["oopf"] = "\xd835\xdd60",
                    ["opar"] = "\x29b7",
                    ["OpenCurlyDoubleQuote"] = "\x201c",
                    ["OpenCurlyQuote"] = "\x2018",
                    ["operp"] = "\x29b9",
                    ["oplus"] = "\x2295",
                    ["Or"] = "\x2a54",
                    ["or"] = "\x2228",
                    ["orarr"] = "\x21bb",
                    ["ord"] = "\x2a5d",
                    ["order"] = "\x2134",
                    ["orderof"] = "\x2134",
                    ["ordf"] = "\x00aa",
                    ["ordm"] = "\x00ba",
                    ["origof"] = "\x22b6",
                    ["oror"] = "\x2a56",
                    ["orslope"] = "\x2a57",
                    ["orv"] = "\x2a5b",
                    ["oS"] = "\x24c8",
                    ["Oscr"] = "\xd835\xdcaa",
                    ["oscr"] = "\x2134",
                    ["Oslash"] = "\x00d8",
                    ["oslash"] = "\x00f8",
                    ["osol"] = "\x2298",
                    ["Otilde"] = "\x00d5",
                    ["otilde"] = "\x00f5",
                    ["Otimes"] = "\x2a37",
                    ["otimes"] = "\x2297",
                    ["otimesas"] = "\x2a36",
                    ["Ouml"] = "\x00d6",
                    ["ouml"] = "\x00f6",
                    ["ovbar"] = "\x233d",
                    ["OverBar"] = "\x203e",
                    ["OverBrace"] = "\x23de",
                    ["OverBracket"] = "\x23b4",
                    ["OverParenthesis"] = "\x23dc",
                    ["par"] = "\x2225",
                    ["para"] = "\x00b6",
                    ["parallel"] = "\x2225",
                    ["parsim"] = "\x2af3",
                    ["parsl"] = "\x2afd",
                    ["part"] = "\x2202",
                    ["PartialD"] = "\x2202",
                    ["Pcy"] = "\x041f",
                    ["pcy"] = "\x043f",
                    ["percnt"] = "\x0025",
                    ["period"] = "\x002e",
                    ["permil"] = "\x2030",
                    ["perp"] = "\x22a5",
                    ["pertenk"] = "\x2031",
                    ["Pfr"] = "\xd835\xdd13",
                    ["pfr"] = "\xd835\xdd2d",
                    ["Phi"] = "\x03a6",
                    ["phi"] = "\x03c6",
                    ["phiv"] = "\x03d5",
                    ["phmmat"] = "\x2133",
                    ["phone"] = "\x260e",
                    ["Pi"] = "\x03a0",
                    ["pi"] = "\x03c0",
                    ["pitchfork"] = "\x22d4",
                    ["piv"] = "\x03d6",
                    ["planck"] = "\x210f",
                    ["planckh"] = "\x210e",
                    ["plankv"] = "\x210f",
                    ["plus"] = "\x002b",
                    ["plusacir"] = "\x2a23",
                    ["plusb"] = "\x229e",
                    ["pluscir"] = "\x2a22",
                    ["plusdo"] = "\x2214",
                    ["plusdu"] = "\x2a25",
                    ["pluse"] = "\x2a72",
                    ["PlusMinus"] = "\x00b1",
                    ["plusmn"] = "\x00b1",
                    ["plussim"] = "\x2a26",
                    ["plustwo"] = "\x2a27",
                    ["pm"] = "\x00b1",
                    ["Poincareplane"] = "\x210c",
                    ["pointint"] = "\x2a15",
                    ["Popf"] = "\x2119",
                    ["popf"] = "\xd835\xdd61",
                    ["pound"] = "\x00a3",
                    ["Pr"] = "\x2abb",
                    ["pr"] = "\x227a",
                    ["prap"] = "\x2ab7",
                    ["prcue"] = "\x227c",
                    ["prE"] = "\x2ab3",
                    ["pre"] = "\x2aaf",
                    ["prec"] = "\x227a",
                    ["precapprox"] = "\x2ab7",
                    ["preccurlyeq"] = "\x227c",
                    ["Precedes"] = "\x227a",
                    ["PrecedesEqual"] = "\x2aaf",
                    ["PrecedesSlantEqual"] = "\x227c",
                    ["PrecedesTilde"] = "\x227e",
                    ["preceq"] = "\x2aaf",
                    ["precnapprox"] = "\x2ab9",
                    ["precneqq"] = "\x2ab5",
                    ["precnsim"] = "\x22e8",
                    ["precsim"] = "\x227e",
                    ["Prime"] = "\x2033",
                    ["prime"] = "\x2032",
                    ["primes"] = "\x2119",
                    ["prnap"] = "\x2ab9",
                    ["prnE"] = "\x2ab5",
                    ["prnsim"] = "\x22e8",
                    ["prod"] = "\x220f",
                    ["Product"] = "\x220f",
                    ["profalar"] = "\x232e",
                    ["profline"] = "\x2312",
                    ["profsurf"] = "\x2313",
                    ["prop"] = "\x221d",
                    ["Proportion"] = "\x2237",
                    ["Proportional"] = "\x221d",
                    ["propto"] = "\x221d",
                    ["prsim"] = "\x227e",
                    ["prurel"] = "\x22b0",
                    ["Pscr"] = "\xd835\xdcab",
                    ["pscr"] = "\xd835\xdcc5",
                    ["Psi"] = "\x03a8",
                    ["psi"] = "\x03c8",
                    ["puncsp"] = "\x2008",
                    ["Qfr"] = "\xd835\xdd14",
                    ["qfr"] = "\xd835\xdd2e",
                    ["qint"] = "\x2a0c",
                    ["Qopf"] = "\x211a",
                    ["qopf"] = "\xd835\xdd62",
                    ["qprime"] = "\x2057",
                    ["Qscr"] = "\xd835\xdcac",
                    ["qscr"] = "\xd835\xdcc6",
                    ["quaternions"] = "\x210d",
                    ["quatint"] = "\x2a16",
                    ["quest"] = "\x003f",
                    ["questeq"] = "\x225f",
                    ["QUOT"] = "\x0022",
                    ["quot"] = "\x0022",
                    ["rAarr"] = "\x21db",
                    ["race"] = "\x223d\x0331",
                    ["Racute"] = "\x0154",
                    ["racute"] = "\x0155",
                    ["radic"] = "\x221a",
                    ["raemptyv"] = "\x29b3",
                    ["Rang"] = "\x27eb",
                    ["rang"] = "\x27e9",
                    ["rangd"] = "\x2992",
                    ["range"] = "\x29a5",
                    ["rangle"] = "\x27e9",
                    ["raquo"] = "\x00bb",
                    ["Rarr"] = "\x21a0",
                    ["rArr"] = "\x21d2",
                    ["rarr"] = "\x2192",
                    ["rarrap"] = "\x2975",
                    ["rarrb"] = "\x21e5",
                    ["rarrbfs"] = "\x2920",
                    ["rarrc"] = "\x2933",
                    ["rarrfs"] = "\x291e",
                    ["rarrhk"] = "\x21aa",
                    ["rarrlp"] = "\x21ac",
                    ["rarrpl"] = "\x2945",
                    ["rarrsim"] = "\x2974",
                    ["Rarrtl"] = "\x2916",
                    ["rarrtl"] = "\x21a3",
                    ["rarrw"] = "\x219d",
                    ["rAtail"] = "\x291c",
                    ["ratail"] = "\x291a",
                    ["ratio"] = "\x2236",
                    ["rationals"] = "\x211a",
                    ["RBarr"] = "\x2910",
                    ["rBarr"] = "\x290f",
                    ["rbarr"] = "\x290d",
                    ["rbbrk"] = "\x2773",
                    ["rbrace"] = "\x007d",
                    ["rbrack"] = "\x005d",
                    ["rbrke"] = "\x298c",
                    ["rbrksld"] = "\x298e",
                    ["rbrkslu"] = "\x2990",
                    ["Rcaron"] = "\x0158",
                    ["rcaron"] = "\x0159",
                    ["Rcedil"] = "\x0156",
                    ["rcedil"] = "\x0157",
                    ["rceil"] = "\x2309",
                    ["rcub"] = "\x007d",
                    ["Rcy"] = "\x0420",
                    ["rcy"] = "\x0440",
                    ["rdca"] = "\x2937",
                    ["rdldhar"] = "\x2969",
                    ["rdquo"] = "\x201d",
                    ["rdquor"] = "\x201d",
                    ["rdsh"] = "\x21b3",
                    ["Re"] = "\x211c",
                    ["real"] = "\x211c",
                    ["realine"] = "\x211b",
                    ["realpart"] = "\x211c",
                    ["reals"] = "\x211d",
                    ["rect"] = "\x25ad",
                    ["REG"] = "\x00ae",
                    ["reg"] = "\x00ae",
                    ["ReverseElement"] = "\x220b",
                    ["ReverseEquilibrium"] = "\x21cb",
                    ["ReverseUpEquilibrium"] = "\x296f",
                    ["rfisht"] = "\x297d",
                    ["rfloor"] = "\x230b",
                    ["Rfr"] = "\x211c",
                    ["rfr"] = "\xd835\xdd2f",
                    ["rHar"] = "\x2964",
                    ["rhard"] = "\x21c1",
                    ["rharu"] = "\x21c0",
                    ["rharul"] = "\x296c",
                    ["Rho"] = "\x03a1",
                    ["rho"] = "\x03c1",
                    ["rhov"] = "\x03f1",
                    ["RightAngleBracket"] = "\x27e9",
                    ["RightArrow"] = "\x2192",
                    ["Rightarrow"] = "\x21d2",
                    ["rightarrow"] = "\x2192",
                    ["RightArrowBar"] = "\x21e5",
                    ["RightArrowLeftArrow"] = "\x21c4",
                    ["rightarrowtail"] = "\x21a3",
                    ["RightCeiling"] = "\x2309",
                    ["RightDoubleBracket"] = "\x27e7",
                    ["RightDownTeeVector"] = "\x295d",
                    ["RightDownVector"] = "\x21c2",
                    ["RightDownVectorBar"] = "\x2955",
                    ["RightFloor"] = "\x230b",
                    ["rightharpoondown"] = "\x21c1",
                    ["rightharpoonup"] = "\x21c0",
                    ["rightleftarrows"] = "\x21c4",
                    ["rightleftharpoons"] = "\x21cc",
                    ["rightrightarrows"] = "\x21c9",
                    ["rightsquigarrow"] = "\x219d",
                    ["RightTee"] = "\x22a2",
                    ["RightTeeArrow"] = "\x21a6",
                    ["RightTeeVector"] = "\x295b",
                    ["rightthreetimes"] = "\x22cc",
                    ["RightTriangle"] = "\x22b3",
                    ["RightTriangleBar"] = "\x29d0",
                    ["RightTriangleEqual"] = "\x22b5",
                    ["RightUpDownVector"] = "\x294f",
                    ["RightUpTeeVector"] = "\x295c",
                    ["RightUpVector"] = "\x21be",
                    ["RightUpVectorBar"] = "\x2954",
                    ["RightVector"] = "\x21c0",
                    ["RightVectorBar"] = "\x2953",
                    ["ring"] = "\x02da",
                    ["risingdotseq"] = "\x2253",
                    ["rlarr"] = "\x21c4",
                    ["rlhar"] = "\x21cc",
                    ["rlm"] = "\x200f",
                    ["rmoust"] = "\x23b1",
                    ["rmoustache"] = "\x23b1",
                    ["rnmid"] = "\x2aee",
                    ["roang"] = "\x27ed",
                    ["roarr"] = "\x21fe",
                    ["robrk"] = "\x27e7",
                    ["ropar"] = "\x2986",
                    ["Ropf"] = "\x211d",
                    ["ropf"] = "\xd835\xdd63",
                    ["roplus"] = "\x2a2e",
                    ["rotimes"] = "\x2a35",
                    ["RoundImplies"] = "\x2970",
                    ["rpar"] = "\x0029",
                    ["rpargt"] = "\x2994",
                    ["rppolint"] = "\x2a12",
                    ["rrarr"] = "\x21c9",
                    ["Rrightarrow"] = "\x21db",
                    ["rsaquo"] = "\x203a",
                    ["Rscr"] = "\x211b",
                    ["rscr"] = "\xd835\xdcc7",
                    ["Rsh"] = "\x21b1",
                    ["rsh"] = "\x21b1",
                    ["rsqb"] = "\x005d",
                    ["rsquo"] = "\x2019",
                    ["rsquor"] = "\x2019",
                    ["rthree"] = "\x22cc",
                    ["rtimes"] = "\x22ca",
                    ["rtri"] = "\x25b9",
                    ["rtrie"] = "\x22b5",
                    ["rtrif"] = "\x25b8",
                    ["rtriltri"] = "\x29ce",
                    ["RuleDelayed"] = "\x29f4",
                    ["ruluhar"] = "\x2968",
                    ["rx"] = "\x211e",
                    ["Sacute"] = "\x015a",
                    ["sacute"] = "\x015b",
                    ["sbquo"] = "\x201a",
                    ["Sc"] = "\x2abc",
                    ["sc"] = "\x227b",
                    ["scap"] = "\x2ab8",
                    ["Scaron"] = "\x0160",
                    ["scaron"] = "\x0161",
                    ["sccue"] = "\x227d",
                    ["scE"] = "\x2ab4",
                    ["sce"] = "\x2ab0",
                    ["Scedil"] = "\x015e",
                    ["scedil"] = "\x015f",
                    ["Scirc"] = "\x015c",
                    ["scirc"] = "\x015d",
                    ["scnap"] = "\x2aba",
                    ["scnE"] = "\x2ab6",
                    ["scnsim"] = "\x22e9",
                    ["scpolint"] = "\x2a13",
                    ["scsim"] = "\x227f",
                    ["Scy"] = "\x0421",
                    ["scy"] = "\x0441",
                    ["sdot"] = "\x22c5",
                    ["sdotb"] = "\x22a1",
                    ["sdote"] = "\x2a66",
                    ["searhk"] = "\x2925",
                    ["seArr"] = "\x21d8",
                    ["searr"] = "\x2198",
                    ["searrow"] = "\x2198",
                    ["sect"] = "\x00a7",
                    ["semi"] = "\x003b",
                    ["seswar"] = "\x2929",
                    ["setminus"] = "\x2216",
                    ["setmn"] = "\x2216",
                    ["sext"] = "\x2736",
                    ["Sfr"] = "\xd835\xdd16",
                    ["sfr"] = "\xd835\xdd30",
                    ["sfrown"] = "\x2322",
                    ["sharp"] = "\x266f",
                    ["SHCHcy"] = "\x0429",
                    ["shchcy"] = "\x0449",
                    ["SHcy"] = "\x0428",
                    ["shcy"] = "\x0448",
                    ["ShortDownArrow"] = "\x2193",
                    ["ShortLeftArrow"] = "\x2190",
                    ["shortmid"] = "\x2223",
                    ["shortparallel"] = "\x2225",
                    ["ShortRightArrow"] = "\x2192",
                    ["ShortUpArrow"] = "\x2191",
                    ["shy"] = "\x00ad",
                    ["Sigma"] = "\x03a3",
                    ["sigma"] = "\x03c3",
                    ["sigmaf"] = "\x03c2",
                    ["sigmav"] = "\x03c2",
                    ["sim"] = "\x223c",
                    ["simdot"] = "\x2a6a",
                    ["sime"] = "\x2243",
                    ["simeq"] = "\x2243",
                    ["simg"] = "\x2a9e",
                    ["simgE"] = "\x2aa0",
                    ["siml"] = "\x2a9d",
                    ["simlE"] = "\x2a9f",
                    ["simne"] = "\x2246",
                    ["simplus"] = "\x2a24",
                    ["simrarr"] = "\x2972",
                    ["slarr"] = "\x2190",
                    ["SmallCircle"] = "\x2218",
                    ["smallsetminus"] = "\x2216",
                    ["smashp"] = "\x2a33",
                    ["smeparsl"] = "\x29e4",
                    ["smid"] = "\x2223",
                    ["smile"] = "\x2323",
                    ["smt"] = "\x2aaa",
                    ["smte"] = "\x2aac",
                    ["smtes"] = "\x2aac\xfe00",
                    ["SOFTcy"] = "\x042c",
                    ["softcy"] = "\x044c",
                    ["sol"] = "\x002f",
                    ["solb"] = "\x29c4",
                    ["solbar"] = "\x233f",
                    ["Sopf"] = "\xd835\xdd4a",
                    ["sopf"] = "\xd835\xdd64",
                    ["spades"] = "\x2660",
                    ["spadesuit"] = "\x2660",
                    ["spar"] = "\x2225",
                    ["sqcap"] = "\x2293",
                    ["sqcaps"] = "\x2293\xfe00",
                    ["sqcup"] = "\x2294",
                    ["sqcups"] = "\x2294\xfe00",
                    ["Sqrt"] = "\x221a",
                    ["sqsub"] = "\x228f",
                    ["sqsube"] = "\x2291",
                    ["sqsubset"] = "\x228f",
                    ["sqsubseteq"] = "\x2291",
                    ["sqsup"] = "\x2290",
                    ["sqsupe"] = "\x2292",
                    ["sqsupset"] = "\x2290",
                    ["sqsupseteq"] = "\x2292",
                    ["squ"] = "\x25a1",
                    ["Square"] = "\x25a1",
                    ["square"] = "\x25a1",
                    ["SquareIntersection"] = "\x2293",
                    ["SquareSubset"] = "\x228f",
                    ["SquareSubsetEqual"] = "\x2291",
                    ["SquareSuperset"] = "\x2290",
                    ["SquareSupersetEqual"] = "\x2292",
                    ["SquareUnion"] = "\x2294",
                    ["squarf"] = "\x25aa",
                    ["squf"] = "\x25aa",
                    ["srarr"] = "\x2192",
                    ["Sscr"] = "\xd835\xdcae",
                    ["sscr"] = "\xd835\xdcc8",
                    ["ssetmn"] = "\x2216",
                    ["ssmile"] = "\x2323",
                    ["sstarf"] = "\x22c6",
                    ["Star"] = "\x22c6",
                    ["star"] = "\x2606",
                    ["starf"] = "\x2605",
                    ["straightepsilon"] = "\x03f5",
                    ["straightphi"] = "\x03d5",
                    ["strns"] = "\x00af",
                    ["Sub"] = "\x22d0",
                    ["sub"] = "\x2282",
                    ["subdot"] = "\x2abd",
                    ["subE"] = "\x2ac5",
                    ["sube"] = "\x2286",
                    ["subedot"] = "\x2ac3",
                    ["submult"] = "\x2ac1",
                    ["subnE"] = "\x2acb",
                    ["subne"] = "\x228a",
                    ["subplus"] = "\x2abf",
                    ["subrarr"] = "\x2979",
                    ["Subset"] = "\x22d0",
                    ["subset"] = "\x2282",
                    ["subseteq"] = "\x2286",
                    ["subseteqq"] = "\x2ac5",
                    ["SubsetEqual"] = "\x2286",
                    ["subsetneq"] = "\x228a",
                    ["subsetneqq"] = "\x2acb",
                    ["subsim"] = "\x2ac7",
                    ["subsub"] = "\x2ad5",
                    ["subsup"] = "\x2ad3",
                    ["succ"] = "\x227b",
                    ["succapprox"] = "\x2ab8",
                    ["succcurlyeq"] = "\x227d",
                    ["Succeeds"] = "\x227b",
                    ["SucceedsEqual"] = "\x2ab0",
                    ["SucceedsSlantEqual"] = "\x227d",
                    ["SucceedsTilde"] = "\x227f",
                    ["succeq"] = "\x2ab0",
                    ["succnapprox"] = "\x2aba",
                    ["succneqq"] = "\x2ab6",
                    ["succnsim"] = "\x22e9",
                    ["succsim"] = "\x227f",
                    ["SuchThat"] = "\x220b",
                    ["Sum"] = "\x2211",
                    ["sum"] = "\x2211",
                    ["sung"] = "\x266a",
                    ["Sup"] = "\x22d1",
                    ["sup"] = "\x2283",
                    ["sup1"] = "\x00b9",
                    ["sup2"] = "\x00b2",
                    ["sup3"] = "\x00b3",
                    ["supdot"] = "\x2abe",
                    ["supdsub"] = "\x2ad8",
                    ["supE"] = "\x2ac6",
                    ["supe"] = "\x2287",
                    ["supedot"] = "\x2ac4",
                    ["Superset"] = "\x2283",
                    ["SupersetEqual"] = "\x2287",
                    ["suphsol"] = "\x27c9",
                    ["suphsub"] = "\x2ad7",
                    ["suplarr"] = "\x297b",
                    ["supmult"] = "\x2ac2",
                    ["supnE"] = "\x2acc",
                    ["supne"] = "\x228b",
                    ["supplus"] = "\x2ac0",
                    ["Supset"] = "\x22d1",
                    ["supset"] = "\x2283",
                    ["supseteq"] = "\x2287",
                    ["supseteqq"] = "\x2ac6",
                    ["supsetneq"] = "\x228b",
                    ["supsetneqq"] = "\x2acc",
                    ["supsim"] = "\x2ac8",
                    ["supsub"] = "\x2ad4",
                    ["supsup"] = "\x2ad6",
                    ["swarhk"] = "\x2926",
                    ["swArr"] = "\x21d9",
                    ["swarr"] = "\x2199",
                    ["swarrow"] = "\x2199",
                    ["swnwar"] = "\x292a",
                    ["szlig"] = "\x00df",
                    ["Tab"] = "\x0009",
                    ["target"] = "\x2316",
                    ["Tau"] = "\x03a4",
                    ["tau"] = "\x03c4",
                    ["tbrk"] = "\x23b4",
                    ["Tcaron"] = "\x0164",
                    ["tcaron"] = "\x0165",
                    ["Tcedil"] = "\x0162",
                    ["tcedil"] = "\x0163",
                    ["Tcy"] = "\x0422",
                    ["tcy"] = "\x0442",
                    ["tdot"] = "\x20db",
                    ["telrec"] = "\x2315",
                    ["Tfr"] = "\xd835\xdd17",
                    ["tfr"] = "\xd835\xdd31",
                    ["there4"] = "\x2234",
                    ["Therefore"] = "\x2234",
                    ["therefore"] = "\x2234",
                    ["Theta"] = "\x0398",
                    ["theta"] = "\x03b8",
                    ["thetasym"] = "\x03d1",
                    ["thetav"] = "\x03d1",
                    ["thickapprox"] = "\x2248",
                    ["thicksim"] = "\x223c",
                    ["ThickSpace"] = "\x205f\x200a",
                    ["thinsp"] = "\x2009",
                    ["ThinSpace"] = "\x2009",
                    ["thkap"] = "\x2248",
                    ["thksim"] = "\x223c",
                    ["THORN"] = "\x00de",
                    ["thorn"] = "\x00fe",
                    ["Tilde"] = "\x223c",
                    ["tilde"] = "\x02dc",
                    ["TildeEqual"] = "\x2243",
                    ["TildeFullEqual"] = "\x2245",
                    ["TildeTilde"] = "\x2248",
                    ["times"] = "\x00d7",
                    ["timesb"] = "\x22a0",
                    ["timesbar"] = "\x2a31",
                    ["timesd"] = "\x2a30",
                    ["tint"] = "\x222d",
                    ["toea"] = "\x2928",
                    ["top"] = "\x22a4",
                    ["topbot"] = "\x2336",
                    ["topcir"] = "\x2af1",
                    ["Topf"] = "\xd835\xdd4b",
                    ["topf"] = "\xd835\xdd65",
                    ["topfork"] = "\x2ada",
                    ["tosa"] = "\x2929",
                    ["tprime"] = "\x2034",
                    ["TRADE"] = "\x2122",
                    ["trade"] = "\x2122",
                    ["triangle"] = "\x25b5",
                    ["triangledown"] = "\x25bf",
                    ["triangleleft"] = "\x25c3",
                    ["trianglelefteq"] = "\x22b4",
                    ["triangleq"] = "\x225c",
                    ["triangleright"] = "\x25b9",
                    ["trianglerighteq"] = "\x22b5",
                    ["tridot"] = "\x25ec",
                    ["trie"] = "\x225c",
                    ["triminus"] = "\x2a3a",
                    ["TripleDot"] = "\x20db",
                    ["triplus"] = "\x2a39",
                    ["trisb"] = "\x29cd",
                    ["tritime"] = "\x2a3b",
                    ["trpezium"] = "\x23e2",
                    ["Tscr"] = "\xd835\xdcaf",
                    ["tscr"] = "\xd835\xdcc9",
                    ["TScy"] = "\x0426",
                    ["tscy"] = "\x0446",
                    ["TSHcy"] = "\x040b",
                    ["tshcy"] = "\x045b",
                    ["Tstrok"] = "\x0166",
                    ["tstrok"] = "\x0167",
                    ["twixt"] = "\x226c",
                    ["twoheadleftarrow"] = "\x219e",
                    ["twoheadrightarrow"] = "\x21a0",
                    ["Uacute"] = "\x00da",
                    ["uacute"] = "\x00fa",
                    ["Uarr"] = "\x219f",
                    ["uArr"] = "\x21d1",
                    ["uarr"] = "\x2191",
                    ["Uarrocir"] = "\x2949",
                    ["Ubrcy"] = "\x040e",
                    ["ubrcy"] = "\x045e",
                    ["Ubreve"] = "\x016c",
                    ["ubreve"] = "\x016d",
                    ["Ucirc"] = "\x00db",
                    ["ucirc"] = "\x00fb",
                    ["Ucy"] = "\x0423",
                    ["ucy"] = "\x0443",
                    ["udarr"] = "\x21c5",
                    ["Udblac"] = "\x0170",
                    ["udblac"] = "\x0171",
                    ["udhar"] = "\x296e",
                    ["ufisht"] = "\x297e",
                    ["Ufr"] = "\xd835\xdd18",
                    ["ufr"] = "\xd835\xdd32",
                    ["Ugrave"] = "\x00d9",
                    ["ugrave"] = "\x00f9",
                    ["uHar"] = "\x2963",
                    ["uharl"] = "\x21bf",
                    ["uharr"] = "\x21be",
                    ["uhblk"] = "\x2580",
                    ["ulcorn"] = "\x231c",
                    ["ulcorner"] = "\x231c",
                    ["ulcrop"] = "\x230f",
                    ["ultri"] = "\x25f8",
                    ["Umacr"] = "\x016a",
                    ["umacr"] = "\x016b",
                    ["uml"] = "\x00a8",
                    ["UnderBar"] = "\x005f",
                    ["UnderBrace"] = "\x23df",
                    ["UnderBracket"] = "\x23b5",
                    ["UnderParenthesis"] = "\x23dd",
                    ["Union"] = "\x22c3",
                    ["UnionPlus"] = "\x228e",
                    ["Uogon"] = "\x0172",
                    ["uogon"] = "\x0173",
                    ["Uopf"] = "\xd835\xdd4c",
                    ["uopf"] = "\xd835\xdd66",
                    ["UpArrow"] = "\x2191",
                    ["Uparrow"] = "\x21d1",
                    ["uparrow"] = "\x2191",
                    ["UpArrowBar"] = "\x2912",
                    ["UpArrowDownArrow"] = "\x21c5",
                    ["UpDownArrow"] = "\x2195",
                    ["Updownarrow"] = "\x21d5",
                    ["updownarrow"] = "\x2195",
                    ["UpEquilibrium"] = "\x296e",
                    ["upharpoonleft"] = "\x21bf",
                    ["upharpoonright"] = "\x21be",
                    ["uplus"] = "\x228e",
                    ["UpperLeftArrow"] = "\x2196",
                    ["UpperRightArrow"] = "\x2197",
                    ["Upsi"] = "\x03d2",
                    ["upsi"] = "\x03c5",
                    ["upsih"] = "\x03d2",
                    ["Upsilon"] = "\x03a5",
                    ["upsilon"] = "\x03c5",
                    ["UpTee"] = "\x22a5",
                    ["UpTeeArrow"] = "\x21a5",
                    ["upuparrows"] = "\x21c8",
                    ["urcorn"] = "\x231d",
                    ["urcorner"] = "\x231d",
                    ["urcrop"] = "\x230e",
                    ["Uring"] = "\x016e",
                    ["uring"] = "\x016f",
                    ["urtri"] = "\x25f9",
                    ["Uscr"] = "\xd835\xdcb0",
                    ["uscr"] = "\xd835\xdcca",
                    ["utdot"] = "\x22f0",
                    ["Utilde"] = "\x0168",
                    ["utilde"] = "\x0169",
                    ["utri"] = "\x25b5",
                    ["utrif"] = "\x25b4",
                    ["uuarr"] = "\x21c8",
                    ["Uuml"] = "\x00dc",
                    ["uuml"] = "\x00fc",
                    ["uwangle"] = "\x29a7",
                    ["vangrt"] = "\x299c",
                    ["varepsilon"] = "\x03f5",
                    ["varkappa"] = "\x03f0",
                    ["varnothing"] = "\x2205",
                    ["varphi"] = "\x03d5",
                    ["varpi"] = "\x03d6",
                    ["varpropto"] = "\x221d",
                    ["vArr"] = "\x21d5",
                    ["varr"] = "\x2195",
                    ["varrho"] = "\x03f1",
                    ["varsigma"] = "\x03c2",
                    ["varsubsetneq"] = "\x228a\xfe00",
                    ["varsubsetneqq"] = "\x2acb\xfe00",
                    ["varsupsetneq"] = "\x228b\xfe00",
                    ["varsupsetneqq"] = "\x2acc\xfe00",
                    ["vartheta"] = "\x03d1",
                    ["vartriangleleft"] = "\x22b2",
                    ["vartriangleright"] = "\x22b3",
                    ["Vbar"] = "\x2aeb",
                    ["vBar"] = "\x2ae8",
                    ["vBarv"] = "\x2ae9",
                    ["Vcy"] = "\x0412",
                    ["vcy"] = "\x0432",
                    ["VDash"] = "\x22ab",
                    ["Vdash"] = "\x22a9",
                    ["vDash"] = "\x22a8",
                    ["vdash"] = "\x22a2",
                    ["Vdashl"] = "\x2ae6",
                    ["Vee"] = "\x22c1",
                    ["vee"] = "\x2228",
                    ["veebar"] = "\x22bb",
                    ["veeeq"] = "\x225a",
                    ["vellip"] = "\x22ee",
                    ["Verbar"] = "\x2016",
                    ["verbar"] = "\x007c",
                    ["Vert"] = "\x2016",
                    ["vert"] = "\x007c",
                    ["VerticalBar"] = "\x2223",
                    ["VerticalLine"] = "\x007c",
                    ["VerticalSeparator"] = "\x2758",
                    ["VerticalTilde"] = "\x2240",
                    ["VeryThinSpace"] = "\x200a",
                    ["Vfr"] = "\xd835\xdd19",
                    ["vfr"] = "\xd835\xdd33",
                    ["vltri"] = "\x22b2",
                    ["vnsub"] = "\x2282\x20d2",
                    ["vnsup"] = "\x2283\x20d2",
                    ["Vopf"] = "\xd835\xdd4d",
                    ["vopf"] = "\xd835\xdd67",
                    ["vprop"] = "\x221d",
                    ["vrtri"] = "\x22b3",
                    ["Vscr"] = "\xd835\xdcb1",
                    ["vscr"] = "\xd835\xdccb",
                    ["vsubnE"] = "\x2acb\xfe00",
                    ["vsubne"] = "\x228a\xfe00",
                    ["vsupnE"] = "\x2acc\xfe00",
                    ["vsupne"] = "\x228b\xfe00",
                    ["Vvdash"] = "\x22aa",
                    ["vzigzag"] = "\x299a",
                    ["Wcirc"] = "\x0174",
                    ["wcirc"] = "\x0175",
                    ["wedbar"] = "\x2a5f",
                    ["Wedge"] = "\x22c0",
                    ["wedge"] = "\x2227",
                    ["wedgeq"] = "\x2259",
                    ["weierp"] = "\x2118",
                    ["Wfr"] = "\xd835\xdd1a",
                    ["wfr"] = "\xd835\xdd34",
                    ["Wopf"] = "\xd835\xdd4e",
                    ["wopf"] = "\xd835\xdd68",
                    ["wp"] = "\x2118",
                    ["wr"] = "\x2240",
                    ["wreath"] = "\x2240",
                    ["Wscr"] = "\xd835\xdcb2",
                    ["wscr"] = "\xd835\xdccc",
                    ["xcap"] = "\x22c2",
                    ["xcirc"] = "\x25ef",
                    ["xcup"] = "\x22c3",
                    ["xdtri"] = "\x25bd",
                    ["Xfr"] = "\xd835\xdd1b",
                    ["xfr"] = "\xd835\xdd35",
                    ["xhArr"] = "\x27fa",
                    ["xharr"] = "\x27f7",
                    ["Xi"] = "\x039e",
                    ["xi"] = "\x03be",
                    ["xlArr"] = "\x27f8",
                    ["xlarr"] = "\x27f5",
                    ["xmap"] = "\x27fc",
                    ["xnis"] = "\x22fb",
                    ["xodot"] = "\x2a00",
                    ["Xopf"] = "\xd835\xdd4f",
                    ["xopf"] = "\xd835\xdd69",
                    ["xoplus"] = "\x2a01",
                    ["xotime"] = "\x2a02",
                    ["xrArr"] = "\x27f9",
                    ["xrarr"] = "\x27f6",
                    ["Xscr"] = "\xd835\xdcb3",
                    ["xscr"] = "\xd835\xdccd",
                    ["xsqcup"] = "\x2a06",
                    ["xuplus"] = "\x2a04",
                    ["xutri"] = "\x25b3",
                    ["xvee"] = "\x22c1",
                    ["xwedge"] = "\x22c0",
                    ["Yacute"] = "\x00dd",
                    ["yacute"] = "\x00fd",
                    ["YAcy"] = "\x042f",
                    ["yacy"] = "\x044f",
                    ["Ycirc"] = "\x0176",
                    ["ycirc"] = "\x0177",
                    ["Ycy"] = "\x042b",
                    ["ycy"] = "\x044b",
                    ["yen"] = "\x00a5",
                    ["Yfr"] = "\xd835\xdd1c",
                    ["yfr"] = "\xd835\xdd36",
                    ["YIcy"] = "\x0407",
                    ["yicy"] = "\x0457",
                    ["Yopf"] = "\xd835\xdd50",
                    ["yopf"] = "\xd835\xdd6a",
                    ["Yscr"] = "\xd835\xdcb4",
                    ["yscr"] = "\xd835\xdcce",
                    ["YUcy"] = "\x042e",
                    ["yucy"] = "\x044e",
                    ["Yuml"] = "\x0178",
                    ["yuml"] = "\x00ff",
                    ["Zacute"] = "\x0179",
                    ["zacute"] = "\x017a",
                    ["Zcaron"] = "\x017d",
                    ["zcaron"] = "\x017e",
                    ["Zcy"] = "\x0417",
                    ["zcy"] = "\x0437",
                    ["Zdot"] = "\x017b",
                    ["zdot"] = "\x017c",
                    ["zeetrf"] = "\x2128",
                    ["ZeroWidthSpace"] = "\x200b",
                    ["Zeta"] = "\x0396",
                    ["zeta"] = "\x03b6",
                    ["Zfr"] = "\x2128",
                    ["zfr"] = "\xd835\xdd37",
                    ["ZHcy"] = "\x0416",
                    ["zhcy"] = "\x0436",
                    ["zigrarr"] = "\x21dd",
                    ["Zopf"] = "\x2124",
                    ["zopf"] = "\xd835\xdd6b",
                    ["Zscr"] = "\xd835\xdcb5",
                    ["zscr"] = "\xd835\xdccf",
                    ["zwj"] = "\x200d",
                    ["zwnj"] = "\x200c",
                };

            public static string Lookup(string entity)
            {
                string theString;
                s_lookupTable.TryGetValue(entity, out theString);
                return theString;
            }
        }
    }
}
