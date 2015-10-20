// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Numerics;
using Xunit;

namespace System.Security.Cryptography.Encoding.Tests
{
    public class DerSequenceReaderTests
    {
        [Fact]
        public static void ReadIntegers()
        {
            byte[] derEncoded =
            {
                /* SEQUENCE */     0x30, 23,
                /* INTEGER(0) */   0x02, 0x01, 0x00,
                /* INTEGER(256) */ 0x02, 0x02, 0x01, 0x00,
                /* INTEGER(-1) */  0x02, 0x01, 0xFF,
                /* Big integer */  0x02, 0x0B, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D,
            };

            DerSequenceReader reader = new DerSequenceReader(derEncoded);
            Assert.True(reader.HasData);
            Assert.Equal(23, reader.ContentLength);

            int first = reader.ReadInteger();
            Assert.Equal(0, first);

            int second = reader.ReadInteger();
            Assert.Equal(256, second);

            int third = reader.ReadInteger();
            Assert.Equal(-1, third);

            // Reader reads Big-Endian, BigInteger reads Little-Endian
            byte[] fourthBytes = reader.ReadIntegerBytes();
            Array.Reverse(fourthBytes);
            BigInteger fourth = new BigInteger(fourthBytes);
            Assert.Equal(BigInteger.Parse("3645759592820458633497613"), fourth);

            // And... done.
            Assert.False(reader.HasData);
        }

        [Fact]
        public static void ReadOids()
        {
            byte[] derEncoded =
            {
                // Noise
                0x10, 0x20, 0x30, 0x04, 0x05,

                // Data
                0x30, 27,
                0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x0B,
                0x06, 0x03, 0x55, 0x04, 0x03,
                0x06, 0x09, 0x2B, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x15, 0x07,

                // More noise.
                0x85, 0x71, 0x23, 0x74, 0x01,
            };

            DerSequenceReader reader = new DerSequenceReader(derEncoded, 5, derEncoded.Length - 10);
            Assert.True(reader.HasData);
            Assert.Equal(27, reader.ContentLength);

            Oid first = reader.ReadOid();
            Assert.Equal("1.2.840.113549.1.1.11", first.Value);

            Oid second = reader.ReadOid();
            Assert.Equal("2.5.4.3", second.Value);

            Oid third = reader.ReadOid();
            Assert.Equal("1.3.6.1.4.1.311.21.7", third.Value);

            // And... done.
            Assert.False(reader.HasData);
        }
    }
}
