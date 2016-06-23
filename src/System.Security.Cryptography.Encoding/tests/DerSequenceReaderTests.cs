// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Test.Cryptography;
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
        
        [Theory]
        [InlineData("170d3135303430313030303030305a", 2015, 4, 1, 0, 0, 0)] // Tests encoding for midnight
        [InlineData("170d3439313233313131353935395a", 2049, 12, 31, 11, 59, 59)]
        [InlineData("170d3531303632383030303330305a", 1951, 06, 28, 0, 3, 0)]
        public static void ReadUtcTime(string hexString, int year, int month, int day, int hour, int minute, int second)
        {
            byte[] payload = hexString.HexToByteArray();
            DateTime representedTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            DerSequenceReader reader = DerSequenceReader.CreateForPayload(payload);
            DateTime decodedTime = reader.ReadUtcTime();
            Assert.Equal(representedTime, decodedTime);
            Assert.Equal(DateTimeKind.Utc, decodedTime.Kind);
        }

        [Theory]
        [InlineData("180f31393932303732323133323130305a", 1992, 07, 22, 13, 21, 00)]
        [InlineData("180f32303138303531383130333731365a", 2018, 05, 18, 10, 37, 16)]
        public static void ReadGeneralizedTime(string hexString, int year, int month, int day, int hour, int minute, int second)
        {
            byte[] payload = hexString.HexToByteArray();
            DateTime representedTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            DerSequenceReader reader = DerSequenceReader.CreateForPayload(payload);
            DateTime decodedTime = reader.ReadGeneralizedTime();
            Assert.Equal(representedTime, decodedTime);
            Assert.Equal(DateTimeKind.Utc, decodedTime.Kind);
        }

        [Theory]
        [InlineData("030504056e9230", "056e9230")] // With unusedbits
        [InlineData("030500056e9237", "056e9237")] // Without unusedbits
        public static void ReadBitString(string hexString, string expected)
        {
            byte[] payload = hexString.HexToByteArray();
            byte[] expectedOctets = expected.HexToByteArray();
            byte[] decoded = DerSequenceReader.CreateForPayload(payload).ReadBitString();
            Assert.Equal(expectedOctets, decoded);
        }

        [Fact]
        public static void FalseEncodedLength_MultiByteLength()
        {
            // This DER encoding follows the usual style of { tag, length, data }, however the encoding for data length
            // here specifies that the content is 672 bytes long (0x82 0x02 0xA0) when in reality it holds 671 bytes
            // that should be encoded as 0x82 0x02 0x9F instead
            byte[] encodedMessage =
                 ("308202a006092a864886f70d010703a08202903082028c020100318202583081c5020100302e301a31183016060355040313"
                + "0f5253414b65795472616e7366657231021031d935fb63e8cfab48a0bf7b397b67c0300d06092a864886f70d010101050004"
                + "818061b4161da3daff2c6c304b8decb021b09ee2523f5162124a6893b077b22a71327c8ab12a82f80472845e274643bfee33"
                + "d34caca6b59fffc66f7fdb2279726f58615258bc3787b479fdfeb4856279e85106d5c271b2f5cadcc8b5622f69cb7e7efd90"
                + "38727c1cb717cb867d2f3e87c3f653cb77837706abb01d40bb22136dac753081c5020100302e301a31183016060355040313"
                + "0f5253414b65795472616e736665723202102bce9f9ece39f98044f0cd2faa9a14e7300d06092a864886f70d010101050004"
                + "818077293ebfd59a4cef9161ef60f082eca1fd7b2e52804992ea5421527bbea35d7abf810d4316e07dfe766f90b221ae34aa"
                + "192e200c26105aba5511c5e168e4cb0bb2996dce730648d5bc8a0005fbb112a80f9a525e266654d4f3de8318abb8f769c387"
                + "e402889354965f05814dcc4a787de1d5442107ab1bf8dcdbeb432d4d70a73081c5020100302e301a31183016060355040313"
                + "0f5253414b65795472616e736665723302104497d870785a23aa4432ed0106ef72a6300d06092a864886f70d010101050004"
                + "81807f2d1e0c34b9c4d8e07cf50107114e10f8c3759eca4bb6385451cf7d3619548b217670e4d9eea0c7a09c513c0e4fc1b1"
                + "978ee2b2aab4c7b04183031d2685bf5ea32b8b48d8eef34743bdf14ba71cde56c97618d48692e59f529cd5a7922caff4ac02"
                + "e5a856a5b28db681b0b508b9761b6fa05a5634742c3542986e4073e7932a302b06092a864886f70d010701301406082a8648"
                + "86f70d030704081ddc958302db22518008d0f4f5bb03b69819").HexToByteArray();

            Assert.Throws<InvalidOperationException>(()=>DerSequenceReader.CreateForPayload(encodedMessage).ReadSequence());
        }

        [Fact]
        public static void FalseEncodedLength_SingleByte()
        {
            // This is tagged to be an OID (0x06) but data is corrupted and holds a false length and no data. This tests
            // length sanity check for reading faulty single byte lengths in DER
            byte[] encodedMessage = ("0601").HexToByteArray();

            Assert.Throws<InvalidOperationException>(() => DerSequenceReader.CreateForPayload(encodedMessage).ReadOid());
        }
    }
}
