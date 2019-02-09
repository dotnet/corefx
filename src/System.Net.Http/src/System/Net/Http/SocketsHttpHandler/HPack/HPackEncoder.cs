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
            return EncodeHeader(header.Name, value, null, null, buffer, out length);
        }

        public static bool EncodeHeader(Headers.HeaderDescriptor header, string[] values, string separator, Span<byte> buffer, out int length)
        {
            return EncodeHeader(header.Name, values, separator, buffer, out length);
        }

        public static bool EncodeHeader(string name, string[] values, string separator, Span<byte> buffer, out int length)
        {
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

            if (buffer.Length == 0)
            {
                return false;
            }

            buffer[i++] = 0;

            if (i == buffer.Length)
            {
                return false;
            }

            if (!EncodeString(name, null, null, buffer.Slice(i), out int nameLength, lowercase: true))
            {
                return false;
            }

            i += nameLength;

            if (i >= buffer.Length)
            {
                return false;
            }

            if (!EncodeString(value, values, separator, buffer.Slice(i), out int valueLength, lowercase: false))
            {
                return false;
            }
            i += valueLength;

            length = i;
            return true;
        }

        private static bool EncodeStringPart(string s, Span<byte> buffer, ref int i, bool lowercase)
        {
            const int toLowerMask = 0x20;
            // TODO: use huffman encoding
            for (int j = 0; j < s.Length; j++)
            {
                if (i >= buffer.Length)
                {
                    return false;
                }

                //buffer[i++] = (byte)(s[j] | (lowercase && s[j] >= 'A' && s[j] <= 'Z' ? toLowerMask : 0));
                buffer[i++] = (byte)(s[j] | (lowercase ? toLowerMask : 0));
            }

            return true;
        }

        private static bool EncodeString(string s, string[] parts, string separator, Span<byte> buffer, out int length, bool lowercase)
        {
            int i = 0;
            length = 0;

            if (buffer.Length == 0)
            {
                return false;
            }

            buffer[0] = 0;

            if (s != null)
            {
                i = s.Length;
            }
            else
            {
                // Calculate length of all pars and separators.
                foreach (string part in parts)
                {
                    i += part.Length;
                }
                i += (parts.Length - 1) * separator.Length;
                s = parts[0];
            }

            if (!IntegerEncoder.Encode(i, 7, buffer, out int nameLength))
            {
                return false;
            }
            i = nameLength;

            if (!EncodeStringPart(s, buffer, ref i, lowercase))
            {
                return false;
            }

            // If there are more parts, send them now.
            if (parts != null && parts.Length > 1)
            {
                // If header has multiple parts, write the rest.
                for (int j = 1 ; j < parts.Length; j++)
                {
                    if (!EncodeStringPart(separator, buffer, ref i, lowercase))
                    {
                        return false;
                    }
                    if (!EncodeStringPart(parts[j], buffer, ref i, lowercase))
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
