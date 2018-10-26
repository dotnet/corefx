﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class HuffmanDecodingTests
    {
        delegate int DecodeDelegate(ReadOnlySpan<byte> src, Span<byte> dst);

        private static readonly DecodeDelegate s_decodeDelegate = GetDecodeDelegate();

        private static DecodeDelegate GetDecodeDelegate()
        {
            Assembly assembly = typeof(HttpClient).Assembly;
            Type huffmanType = assembly.GetType("System.Net.Http.HPack.Huffman");
            MethodInfo decodeMethod = huffmanType.GetMethod("Decode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return (DecodeDelegate)Delegate.CreateDelegate(typeof(DecodeDelegate), decodeMethod);
        }

        private static int Decode(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            return s_decodeDelegate(source, destination);
        }

        private static readonly (uint code, int bitLength)[] s_encodingTable = new (uint code, int bitLength)[]
        {
            (0b11111111_11000000_00000000_00000000, 13),
            (0b11111111_11111111_10110000_00000000, 23),
            (0b11111111_11111111_11111110_00100000, 28),
            (0b11111111_11111111_11111110_00110000, 28),
            (0b11111111_11111111_11111110_01000000, 28),
            (0b11111111_11111111_11111110_01010000, 28),
            (0b11111111_11111111_11111110_01100000, 28),
            (0b11111111_11111111_11111110_01110000, 28),
            (0b11111111_11111111_11111110_10000000, 28),
            (0b11111111_11111111_11101010_00000000, 24),
            (0b11111111_11111111_11111111_11110000, 30),
            (0b11111111_11111111_11111110_10010000, 28),
            (0b11111111_11111111_11111110_10100000, 28),
            (0b11111111_11111111_11111111_11110100, 30),
            (0b11111111_11111111_11111110_10110000, 28),
            (0b11111111_11111111_11111110_11000000, 28),
            (0b11111111_11111111_11111110_11010000, 28),
            (0b11111111_11111111_11111110_11100000, 28),
            (0b11111111_11111111_11111110_11110000, 28),
            (0b11111111_11111111_11111111_00000000, 28),
            (0b11111111_11111111_11111111_00010000, 28),
            (0b11111111_11111111_11111111_00100000, 28),
            (0b11111111_11111111_11111111_11111000, 30),
            (0b11111111_11111111_11111111_00110000, 28),
            (0b11111111_11111111_11111111_01000000, 28),
            (0b11111111_11111111_11111111_01010000, 28),
            (0b11111111_11111111_11111111_01100000, 28),
            (0b11111111_11111111_11111111_01110000, 28),
            (0b11111111_11111111_11111111_10000000, 28),
            (0b11111111_11111111_11111111_10010000, 28),
            (0b11111111_11111111_11111111_10100000, 28),
            (0b11111111_11111111_11111111_10110000, 28),
            (0b01010000_00000000_00000000_00000000, 6),
            (0b11111110_00000000_00000000_00000000, 10),
            (0b11111110_01000000_00000000_00000000, 10),
            (0b11111111_10100000_00000000_00000000, 12),
            (0b11111111_11001000_00000000_00000000, 13),
            (0b01010100_00000000_00000000_00000000, 6),
            (0b11111000_00000000_00000000_00000000, 8),
            (0b11111111_01000000_00000000_00000000, 11),
            (0b11111110_10000000_00000000_00000000, 10),
            (0b11111110_11000000_00000000_00000000, 10),
            (0b11111001_00000000_00000000_00000000, 8),
            (0b11111111_01100000_00000000_00000000, 11),
            (0b11111010_00000000_00000000_00000000, 8),
            (0b01011000_00000000_00000000_00000000, 6),
            (0b01011100_00000000_00000000_00000000, 6),
            (0b01100000_00000000_00000000_00000000, 6),
            (0b00000000_00000000_00000000_00000000, 5),
            (0b00001000_00000000_00000000_00000000, 5),
            (0b00010000_00000000_00000000_00000000, 5),
            (0b01100100_00000000_00000000_00000000, 6),
            (0b01101000_00000000_00000000_00000000, 6),
            (0b01101100_00000000_00000000_00000000, 6),
            (0b01110000_00000000_00000000_00000000, 6),
            (0b01110100_00000000_00000000_00000000, 6),
            (0b01111000_00000000_00000000_00000000, 6),
            (0b01111100_00000000_00000000_00000000, 6),
            (0b10111000_00000000_00000000_00000000, 7),
            (0b11111011_00000000_00000000_00000000, 8),
            (0b11111111_11111000_00000000_00000000, 15),
            (0b10000000_00000000_00000000_00000000, 6),
            (0b11111111_10110000_00000000_00000000, 12),
            (0b11111111_00000000_00000000_00000000, 10),
            (0b11111111_11010000_00000000_00000000, 13),
            (0b10000100_00000000_00000000_00000000, 6),
            (0b10111010_00000000_00000000_00000000, 7),
            (0b10111100_00000000_00000000_00000000, 7),
            (0b10111110_00000000_00000000_00000000, 7),
            (0b11000000_00000000_00000000_00000000, 7),
            (0b11000010_00000000_00000000_00000000, 7),
            (0b11000100_00000000_00000000_00000000, 7),
            (0b11000110_00000000_00000000_00000000, 7),
            (0b11001000_00000000_00000000_00000000, 7),
            (0b11001010_00000000_00000000_00000000, 7),
            (0b11001100_00000000_00000000_00000000, 7),
            (0b11001110_00000000_00000000_00000000, 7),
            (0b11010000_00000000_00000000_00000000, 7),
            (0b11010010_00000000_00000000_00000000, 7),
            (0b11010100_00000000_00000000_00000000, 7),
            (0b11010110_00000000_00000000_00000000, 7),
            (0b11011000_00000000_00000000_00000000, 7),
            (0b11011010_00000000_00000000_00000000, 7),
            (0b11011100_00000000_00000000_00000000, 7),
            (0b11011110_00000000_00000000_00000000, 7),
            (0b11100000_00000000_00000000_00000000, 7),
            (0b11100010_00000000_00000000_00000000, 7),
            (0b11100100_00000000_00000000_00000000, 7),
            (0b11111100_00000000_00000000_00000000, 8),
            (0b11100110_00000000_00000000_00000000, 7),
            (0b11111101_00000000_00000000_00000000, 8),
            (0b11111111_11011000_00000000_00000000, 13),
            (0b11111111_11111110_00000000_00000000, 19),
            (0b11111111_11100000_00000000_00000000, 13),
            (0b11111111_11110000_00000000_00000000, 14),
            (0b10001000_00000000_00000000_00000000, 6),
            (0b11111111_11111010_00000000_00000000, 15),
            (0b00011000_00000000_00000000_00000000, 5),
            (0b10001100_00000000_00000000_00000000, 6),
            (0b00100000_00000000_00000000_00000000, 5),
            (0b10010000_00000000_00000000_00000000, 6),
            (0b00101000_00000000_00000000_00000000, 5),
            (0b10010100_00000000_00000000_00000000, 6),
            (0b10011000_00000000_00000000_00000000, 6),
            (0b10011100_00000000_00000000_00000000, 6),
            (0b00110000_00000000_00000000_00000000, 5),
            (0b11101000_00000000_00000000_00000000, 7),
            (0b11101010_00000000_00000000_00000000, 7),
            (0b10100000_00000000_00000000_00000000, 6),
            (0b10100100_00000000_00000000_00000000, 6),
            (0b10101000_00000000_00000000_00000000, 6),
            (0b00111000_00000000_00000000_00000000, 5),
            (0b10101100_00000000_00000000_00000000, 6),
            (0b11101100_00000000_00000000_00000000, 7),
            (0b10110000_00000000_00000000_00000000, 6),
            (0b01000000_00000000_00000000_00000000, 5),
            (0b01001000_00000000_00000000_00000000, 5),
            (0b10110100_00000000_00000000_00000000, 6),
            (0b11101110_00000000_00000000_00000000, 7),
            (0b11110000_00000000_00000000_00000000, 7),
            (0b11110010_00000000_00000000_00000000, 7),
            (0b11110100_00000000_00000000_00000000, 7),
            (0b11110110_00000000_00000000_00000000, 7),
            (0b11111111_11111100_00000000_00000000, 15),
            (0b11111111_10000000_00000000_00000000, 11),
            (0b11111111_11110100_00000000_00000000, 14),
            (0b11111111_11101000_00000000_00000000, 13),
            (0b11111111_11111111_11111111_11000000, 28),
            (0b11111111_11111110_01100000_00000000, 20),
            (0b11111111_11111111_01001000_00000000, 22),
            (0b11111111_11111110_01110000_00000000, 20),
            (0b11111111_11111110_10000000_00000000, 20),
            (0b11111111_11111111_01001100_00000000, 22),
            (0b11111111_11111111_01010000_00000000, 22),
            (0b11111111_11111111_01010100_00000000, 22),
            (0b11111111_11111111_10110010_00000000, 23),
            (0b11111111_11111111_01011000_00000000, 22),
            (0b11111111_11111111_10110100_00000000, 23),
            (0b11111111_11111111_10110110_00000000, 23),
            (0b11111111_11111111_10111000_00000000, 23),
            (0b11111111_11111111_10111010_00000000, 23),
            (0b11111111_11111111_10111100_00000000, 23),
            (0b11111111_11111111_11101011_00000000, 24),
            (0b11111111_11111111_10111110_00000000, 23),
            (0b11111111_11111111_11101100_00000000, 24),
            (0b11111111_11111111_11101101_00000000, 24),
            (0b11111111_11111111_01011100_00000000, 22),
            (0b11111111_11111111_11000000_00000000, 23),
            (0b11111111_11111111_11101110_00000000, 24),
            (0b11111111_11111111_11000010_00000000, 23),
            (0b11111111_11111111_11000100_00000000, 23),
            (0b11111111_11111111_11000110_00000000, 23),
            (0b11111111_11111111_11001000_00000000, 23),
            (0b11111111_11111110_11100000_00000000, 21),
            (0b11111111_11111111_01100000_00000000, 22),
            (0b11111111_11111111_11001010_00000000, 23),
            (0b11111111_11111111_01100100_00000000, 22),
            (0b11111111_11111111_11001100_00000000, 23),
            (0b11111111_11111111_11001110_00000000, 23),
            (0b11111111_11111111_11101111_00000000, 24),
            (0b11111111_11111111_01101000_00000000, 22),
            (0b11111111_11111110_11101000_00000000, 21),
            (0b11111111_11111110_10010000_00000000, 20),
            (0b11111111_11111111_01101100_00000000, 22),
            (0b11111111_11111111_01110000_00000000, 22),
            (0b11111111_11111111_11010000_00000000, 23),
            (0b11111111_11111111_11010010_00000000, 23),
            (0b11111111_11111110_11110000_00000000, 21),
            (0b11111111_11111111_11010100_00000000, 23),
            (0b11111111_11111111_01110100_00000000, 22),
            (0b11111111_11111111_01111000_00000000, 22),
            (0b11111111_11111111_11110000_00000000, 24),
            (0b11111111_11111110_11111000_00000000, 21),
            (0b11111111_11111111_01111100_00000000, 22),
            (0b11111111_11111111_11010110_00000000, 23),
            (0b11111111_11111111_11011000_00000000, 23),
            (0b11111111_11111111_00000000_00000000, 21),
            (0b11111111_11111111_00001000_00000000, 21),
            (0b11111111_11111111_10000000_00000000, 22),
            (0b11111111_11111111_00010000_00000000, 21),
            (0b11111111_11111111_11011010_00000000, 23),
            (0b11111111_11111111_10000100_00000000, 22),
            (0b11111111_11111111_11011100_00000000, 23),
            (0b11111111_11111111_11011110_00000000, 23),
            (0b11111111_11111110_10100000_00000000, 20),
            (0b11111111_11111111_10001000_00000000, 22),
            (0b11111111_11111111_10001100_00000000, 22),
            (0b11111111_11111111_10010000_00000000, 22),
            (0b11111111_11111111_11100000_00000000, 23),
            (0b11111111_11111111_10010100_00000000, 22),
            (0b11111111_11111111_10011000_00000000, 22),
            (0b11111111_11111111_11100010_00000000, 23),
            (0b11111111_11111111_11111000_00000000, 26),
            (0b11111111_11111111_11111000_01000000, 26),
            (0b11111111_11111110_10110000_00000000, 20),
            (0b11111111_11111110_00100000_00000000, 19),
            (0b11111111_11111111_10011100_00000000, 22),
            (0b11111111_11111111_11100100_00000000, 23),
            (0b11111111_11111111_10100000_00000000, 22),
            (0b11111111_11111111_11110110_00000000, 25),
            (0b11111111_11111111_11111000_10000000, 26),
            (0b11111111_11111111_11111000_11000000, 26),
            (0b11111111_11111111_11111001_00000000, 26),
            (0b11111111_11111111_11111011_11000000, 27),
            (0b11111111_11111111_11111011_11100000, 27),
            (0b11111111_11111111_11111001_01000000, 26),
            (0b11111111_11111111_11110001_00000000, 24),
            (0b11111111_11111111_11110110_10000000, 25),
            (0b11111111_11111110_01000000_00000000, 19),
            (0b11111111_11111111_00011000_00000000, 21),
            (0b11111111_11111111_11111001_10000000, 26),
            (0b11111111_11111111_11111100_00000000, 27),
            (0b11111111_11111111_11111100_00100000, 27),
            (0b11111111_11111111_11111001_11000000, 26),
            (0b11111111_11111111_11111100_01000000, 27),
            (0b11111111_11111111_11110010_00000000, 24),
            (0b11111111_11111111_00100000_00000000, 21),
            (0b11111111_11111111_00101000_00000000, 21),
            (0b11111111_11111111_11111010_00000000, 26),
            (0b11111111_11111111_11111010_01000000, 26),
            (0b11111111_11111111_11111111_11010000, 28),
            (0b11111111_11111111_11111100_01100000, 27),
            (0b11111111_11111111_11111100_10000000, 27),
            (0b11111111_11111111_11111100_10100000, 27),
            (0b11111111_11111110_11000000_00000000, 20),
            (0b11111111_11111111_11110011_00000000, 24),
            (0b11111111_11111110_11010000_00000000, 20),
            (0b11111111_11111111_00110000_00000000, 21),
            (0b11111111_11111111_10100100_00000000, 22),
            (0b11111111_11111111_00111000_00000000, 21),
            (0b11111111_11111111_01000000_00000000, 21),
            (0b11111111_11111111_11100110_00000000, 23),
            (0b11111111_11111111_10101000_00000000, 22),
            (0b11111111_11111111_10101100_00000000, 22),
            (0b11111111_11111111_11110111_00000000, 25),
            (0b11111111_11111111_11110111_10000000, 25),
            (0b11111111_11111111_11110100_00000000, 24),
            (0b11111111_11111111_11110101_00000000, 24),
            (0b11111111_11111111_11111010_10000000, 26),
            (0b11111111_11111111_11101000_00000000, 23),
            (0b11111111_11111111_11111010_11000000, 26),
            (0b11111111_11111111_11111100_11000000, 27),
            (0b11111111_11111111_11111011_00000000, 26),
            (0b11111111_11111111_11111011_01000000, 26),
            (0b11111111_11111111_11111100_11100000, 27),
            (0b11111111_11111111_11111101_00000000, 27),
            (0b11111111_11111111_11111101_00100000, 27),
            (0b11111111_11111111_11111101_01000000, 27),
            (0b11111111_11111111_11111101_01100000, 27),
            (0b11111111_11111111_11111111_11100000, 28),
            (0b11111111_11111111_11111101_10000000, 27),
            (0b11111111_11111111_11111101_10100000, 27),
            (0b11111111_11111111_11111101_11000000, 27),
            (0b11111111_11111111_11111101_11100000, 27),
            (0b11111111_11111111_11111110_00000000, 27),
            (0b11111111_11111111_11111011_10000000, 26)
        };

        // Encoded values are 30 bits at most, so are stored in the table in a uint.
        // Convert to ulong here and put the encoded value in the most significant bits.
        // This makes the encoding logic below simpler.
        private static (ulong code, int bitLength) GetEncodedValue(byte b)
        {
            (uint code, int bitLength) = s_encodingTable[b];
            return (((ulong)code) << 32, bitLength);
        }

        private static int Encode(byte[] source, byte[] destination, bool injectEOS)
        {
            ulong currentBits = 0;  // We can have 7 bits of rollover plus 30 bits for the next encoded value, so use a ulong
            int currentBitCount = 0;
            int dstOffset = 0;

            for (int i = 0; i < source.Length; i++)
            {
                (ulong code, int bitLength) = GetEncodedValue(source[i]);

                // inject EOS if instructed to
                if (injectEOS)
                {
                    code |= (ulong)0b11111111_11111111_11111111_11111100 << (32 - bitLength);
                    bitLength += 30;
                    injectEOS = false;
                }

                currentBits |= code >> currentBitCount;
                currentBitCount += bitLength;

                while (currentBitCount >= 8)
                {
                    destination[dstOffset++] = (byte)(currentBits >> 56);
                    currentBits = currentBits << 8;
                    currentBitCount -= 8;
                }
            }

            // Fill any trailing bits with ones, per RFC
            if (currentBitCount > 0)
            {
                currentBits |= 0xFFFFFFFFFFFFFFFF >> currentBitCount;
                destination[dstOffset++] = (byte)(currentBits >> 56);
            }

            return dstOffset;
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void HuffmanDecoding_ValidEncoding_Succeeds(byte[] input)
        {
            // Worst case encoding is 30 bits per input byte, so make the encoded buffer 4 times as big
            byte[] encoded = new byte[input.Length * 4];
            int encodedByteCount = Encode(input, encoded, false);

            // Worst case decoding is an output byte per 5 input bits, so make the decoded buffer 2 times as big
            byte[] decoded = new byte[encoded.Length * 2];

            int decodedByteCount = Decode(new ReadOnlySpan<byte>(encoded, 0, encodedByteCount), decoded);

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

            Assert.Throws(s_huffmanDecodingExceptionType, () => Decode(encoded, decoded));
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
                int encodedBits = currentPrefix.Select(b => s_encodingTable[b].bitLength).Sum();
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
                int encodedByteCount = Encode(source, destination, false);
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
                int encodedByteCount = Encode(source, destination, false);
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
                int encodedByteCount = Encode(source, destination, true);
                yield return new object[] { destination.Take(encodedByteCount).ToArray() };
            }
        }
    }
}
