// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit;
using System.Net.Http.HPack;
using System.Net.Test.Common;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Net.Http.Unit.Tests.HPack
{
    public class HuffmanDecodingTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void HuffmanDecoding_ValidEncoding_Succeeds(byte[] input)
        {
            // Worst case encoding is 30 bits per input byte, so make the encoded buffer 4 times as big
            byte[] encoded = new byte[input.Length * 4];
            int encodedByteCount = HuffmanEncoder.Encode(input, encoded);

            // Worst case decoding is an output byte per 5 input bits, so make the decoded buffer 2 times as big
            byte[] decoded = new byte[encoded.Length * 2];

            int decodedByteCount = Huffman.Decode(new ReadOnlySpan<byte>(encoded, 0, encodedByteCount), ref decoded);

            Assert.Equal(input.Length, decodedByteCount);
            Assert.Equal(input, decoded.Take(decodedByteCount));
        }

        private static readonly Type s_huffmanDecodingExceptionType = typeof(HttpClient).Assembly.GetType("System.Net.Http.HPack.HuffmanDecodingException");

        [Theory]
        [MemberData(nameof(InvalidEncodingData))]
        public void HuffmanDecoding_InvalidEncoding_Throws(byte[] encoded)
        {
            // Worst case decoding is an output byte per 5 input bits, so make the decoded buffer 2 times as big
            byte[] decoded = new byte[encoded.Length * 2];

            Assert.Throws(s_huffmanDecodingExceptionType, () => Huffman.Decode(encoded, ref decoded));
        }

        // This input sequence will encode to 17 bits, thus offsetting the next character to encode
        // by exactly one bit. We use this below to generate a prefix that encodes all of the possible starting
        // bit offsets for a character, from 0 to 7.
        private static readonly byte[] s_offsetByOneBit = new byte[] { (byte)'c', (byte)'l', (byte)'r' };

        public static IEnumerable<object[]> TestData()
        {
            // Single byte data
            for (int i = 0; i < 256; i++)
            {
                yield return new object[] { new byte[] { (byte)i } };
            }

            // Ensure that decoding every possible value leaves the decoder in a correct state so that
            // a subsequent value can be decoded (here, 'a')
            for (int i = 0; i < 256; i++)
            {
                yield return new object[] { new byte[] { (byte)i, (byte)'a' } };
            }

            // Ensure that every possible bit starting position for every value is encoded properly
            // s_offsetByOneBit encodes to exactly 17 bits, leaving 1 bit for the next byte
            // So by repeating this sequence, we can generate any starting bit position we want.
            byte[] currentPrefix = new byte[0];
            for (int prefixBits = 1; prefixBits <= 8; prefixBits++)
            {
                currentPrefix = currentPrefix.Concat(s_offsetByOneBit).ToArray();

                // Make sure we're actually getting the correct number of prefix bits
                int encodedBits = currentPrefix.Select(b => HuffmanEncoder.s_encodingTable[b].bitLength).Sum();
                Assert.Equal(prefixBits % 8, encodedBits % 8);

                for (int i = 0; i < 256; i++)
                {
                    yield return new object[] { currentPrefix.Concat(new byte[] { (byte)i }.Concat(currentPrefix)).ToArray() };
                }
            }

            // Finally, one really big chunk of randomly generated data.
            byte[] data = new byte[1024 * 1024];
            new Random(42).NextBytes(data);
            yield return new object[] { data };
        }

        public static IEnumerable<object[]> InvalidEncodingData()
        {
            // For encodings greater than 8 bits, truncate one or more bytes to generate an invalid encoding
            byte[] source = new byte[1];
            byte[] destination = new byte[10];
            for (int i = 0; i < 256; i++)
            {
                source[0] = (byte)i;
                int encodedByteCount = HuffmanEncoder.Encode(source, destination);
                if (encodedByteCount > 1)
                {
                    yield return new object[] { destination.Take(encodedByteCount - 1).ToArray() };
                    if (encodedByteCount > 2)
                    {
                        yield return new object[] { destination.Take(encodedByteCount - 2).ToArray() };
                        if (encodedByteCount > 3)
                        {
                            yield return new object[] { destination.Take(encodedByteCount - 3).ToArray() };
                        }
                    }
                }
            }

            // Pad encodings with invalid trailing one bits. This is disallowed.
            byte[] pad1 = new byte[] { 0xFF };
            byte[] pad2 = new byte[] { 0xFF, 0xFF, };
            byte[] pad3 = new byte[] { 0xFF, 0xFF, 0xFF };
            byte[] pad4 = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            for (int i = 0; i < 256; i++)
            {
                source[0] = (byte)i;
                int encodedByteCount = HuffmanEncoder.Encode(source, destination);
                yield return new object[] { destination.Take(encodedByteCount).Concat(pad1).ToArray() };
                yield return new object[] { destination.Take(encodedByteCount).Concat(pad2).ToArray() };
                yield return new object[] { destination.Take(encodedByteCount).Concat(pad3).ToArray() };
                yield return new object[] { destination.Take(encodedByteCount).Concat(pad4).ToArray() };
            }

            // send single EOS
            yield return new object[] { new byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111100 } };

            // send combinations with EOS in the middle
            source = new byte[2];
            destination = new byte[24];
            for (int i = 0; i < 256; i++)
            {
                source[0] = source[1] = (byte)i;
                int encodedByteCount = HuffmanEncoder.Encode(source, destination, injectEOS: true);
                yield return new object[] { destination.Take(encodedByteCount).ToArray() };
            }
        }
    }
}
