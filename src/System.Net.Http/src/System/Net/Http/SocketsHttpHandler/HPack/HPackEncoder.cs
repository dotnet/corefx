// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Http;

namespace System.Net.Http.HPack
{
    internal static class HPackEncoder
    {
        // Things we should add:
        // * Static table encoding
        //          This should be based off some sort of "known headers/values" scheme, so we don't need
        //          to re-lookup header names each time.
        // * Huffman encoding
        //      
        // Things we should consider adding:
        // * Dynamic table encoding.
        //          This would make the encoder stateful, which complicates things significantly.
        //          Additionally, it's not clear exactly what strings we would add to the dynamic table
        //          without some additional guidance from the user about this.
        //          So for now, don't do dynamic encoding.

        public static bool EncodeHeader(string name, string value, Span<byte> buffer, out int length)
        {
            int i = 0;
            length = 0;
            int valueLength = value.Length;

            // We need at least \0 and twice length plus one octet string
            if (buffer.Length < 3 + name.Length + valueLength)
            {
                return false;
            }

            buffer[i++] = 0;
            if (!EncodeHeaderName(name, buffer.Slice(i), out int encodedLength))
            {
                return false;
            }

            i += encodedLength;

            if (!IntegerEncoder.Encode(valueLength, 7, buffer.Slice(i), out encodedLength))
            {
                return false;
            }

            i += encodedLength;

            if (valueLength > 0)
            {
                if (!EncodeStringPart(value, buffer.Slice(i), out encodedLength))
                {
                    return false;
                }

                i += encodedLength;
            }

            length = i;
            return true;
        }

        public static bool EncodeHeader(string name, string[] values, string separator, Span<byte> buffer, out int length)
        {
            int i = 0;
            int valueLength = 0;
            length = 0;

            if (values == null || values.Length == 0)
            {
                return EncodeHeader(name, null, buffer, out length);
            }
            else if (values.Length == 1)
            {
                return EncodeHeader(name, values[0], buffer, out length);
            }

            // Calculate length of all parts and separators.
            foreach (string part in values)
            {
                valueLength += part.Length;
            }

            valueLength  += (values.Length - 1) * separator.Length;
            if (buffer.Length < 3 + name.Length + valueLength)
            {
                return false;
            }

            buffer[i++] = 0;

            if (!EncodeHeaderName(name, buffer.Slice(i), out int encodedLength))
            {
                return false;
            }

            i += encodedLength;

            if (!IntegerEncoder.Encode(valueLength, 7, buffer.Slice(i), out encodedLength))
            {
                return false;
            }

            i += encodedLength;

            if (valueLength == 0)
            {
                // We are done if there is no value.
                length = i;
                return true;
            }

            encodedLength = 0;
            for (int j = 0; j < values.Length; j++)
            {
                if (j != 0 && !EncodeStringPart(separator, buffer.Slice(i), out encodedLength))
                {
                        return false;
                }

                i += encodedLength;

                if (!EncodeStringPart(values[j], buffer.Slice(i), out encodedLength))
                {
                    return false;
                }

                i += encodedLength;
            }

            length = i;
            return true;
        }

        // Encode header name. It needs to be lower cased and validity was checked when created.
        private static bool EncodeHeaderName(string s, Span<byte> buffer, out int nameLength)
        {
            const int ToLowerMask = 0x20;
            nameLength = 0;

            if (!IntegerEncoder.Encode(s.Length, 7, buffer, out int currentIndex))
            {
                return false;
            }

            if (currentIndex + s.Length >= buffer.Length)
            {
                return false;
            }

            for (int j = 0; j < s.Length; j++)
            {
                // TODO Use ASCII ToUpper when #34144 is ready.
                buffer[currentIndex++] = (byte)(s[j] | (s[j] >= 'A' && s[j] <= 'Z' ? ToLowerMask : 0));
            }

            nameLength = currentIndex;

            return true;
        }

        // Encode fragment or header value without writing out length.
        private static bool EncodeStringPart(string s, Span<byte> buffer, out int encodedLength)
        {
            encodedLength = 0;

            if (s.Length > buffer.Length)
            {
                return false;
            }

            int i = 0;
            for (int j = 0; j < s.Length; j++)
            {
                if ((c & 0xFF80) != 0)
                {
                    throw new HttpRequestException(SR.net_http_request_invalid_char_encoding);
                }
                buffer[i++] = (byte)c[j];
            }

            encodedLength = i;

            return true;
        }
    }
}
