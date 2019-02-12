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
            Debug.Assert(value != null);
            return EncodeHeader(name, value, null, null, buffer, out length);
        }

        public static bool EncodeHeader(Headers.HeaderDescriptor header, string value, Span<byte> buffer, out int length)
        {
            Debug.Assert(value != null);
            return EncodeHeader(header.Name, value, null, null, buffer, out length);
        }

        public static bool EncodeHeader(Headers.HeaderDescriptor header, string[] values, string separator, Span<byte> buffer, out int length)
        {
            Debug.Assert(values != null);
            Debug.Assert(values.Length > 0);
            return EncodeHeader(header.Name, values, separator, buffer, out length);
        }

        public static bool EncodeHeader(string name, string[] values, string separator, Span<byte> buffer, out int length)
        {
            Debug.Assert(values != null);
            Debug.Assert(values.Length > 0);
            if (values.Length == 1)
            {
                return EncodeHeader(name, values[0], null, null, buffer, out length);
            }

            // When we have more values, separator must be provided.
            Debug.Assert(separator != null);
            return EncodeHeader(name, null, values, separator, buffer, out length);
        }

        private static bool EncodeHeader(string name, string value, string[] values, string separator, Span<byte> buffer, out int length)
        {
            int i = 0;
            length = 0;

            // We need at least \0 and twice length plus one octet string
            if (buffer.Length < 5)
            {
                return false;
            }

            buffer[i++] = 0;

            if (!EncodeHeaderName(name, buffer.Slice(i), out int nameLength))
            {
                return false;
            }
            i += nameLength;

            if (!EncodeHeaderValue(value, values, separator, buffer.Slice(i), out int valueLength))
            {
                return false;
            }
            i += valueLength;

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

            // TODO: use huffman encoding
            for (int j = 0; j < s.Length; j++)
            {
                // TODO Use ASCII ToUpper when #34144 is ready.
                buffer[currentIndex++] = (byte)(s[j] | (s[j] >= 'A' && s[j] <= 'Z' ? ToLowerMask : 0));
            }
            nameLength = currentIndex;

            return true;
        }

        // Encode fragment or header value without writing out length.
        private static bool EncodeStringPart(string s, Span<byte> buffer, ref int currentIndex)
        {
            if (s.Length >= buffer.Length)
            {
                return false;
            }

            int i = 0;
            for (int j = 0; j < s.Length; j++)
            {
                // TODO add validation for valid characters #35165.
                buffer[i++] = (byte)(s[j]);
            }
            currentIndex += i;

            return true;
        }

        private static bool EncodeHeaderValue(string s, string[] parts, string separator, Span<byte> buffer, out int length)
        {
            int i = 0;
            length = 0;

            // \0, length and at least one byte of value
            if (buffer.Length < 3)
            {
                return false;
            }

            if (s != null)
            {
                i = s.Length;
            }
            else
            {
                // Calculate length of all parts and separators.
                foreach (string part in parts)
                {
                    i += part.Length;
                }

                i += (parts.Length - 1) * separator.Length;
                s = parts[0];
            }

            if (!IntegerEncoder.Encode(i, 7, buffer, out int valueLength))
            {
                return false;
            }

            i = valueLength;

            if (!EncodeStringPart(s, buffer.Slice(i), ref i))
            {
                return false;
            }

            // If there are more parts, send them now.
            if (parts != null && parts.Length > 1)
            {
                // If header has multiple parts, write the rest.
                for (int j = 1 ; j < parts.Length; j++)
                {
                    if (!EncodeStringPart(separator, buffer.Slice(i), ref i))
                    {
                        return false;
                    }
                    if (!EncodeStringPart(parts[j], buffer.Slice(i), ref i))
                    {
                        return false;
                    }
                }
            }

            length = i;
            return true;
        }
    }
}
