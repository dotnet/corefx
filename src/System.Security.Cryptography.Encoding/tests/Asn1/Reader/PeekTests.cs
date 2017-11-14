// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class PeekTests : Asn1ReaderTests
    {
        [Fact]
        public static void ReaderPeekTag_Valid()
        {
            // SEQUENCE(NULL)
            byte[] data = { 0x30, 0x02, 0x05, 0x00 };
            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            Asn1Tag tag = reader.PeekTag();

            Assert.Equal((int)UniversalTagNumber.Sequence, tag.TagValue);
            Assert.True(tag.IsConstructed, "tag.IsConstructed");
            Assert.Equal(TagClass.Universal, tag.TagClass);
        }

        [Fact]
        public static void ReaderPeekTag_Invalid()
        {
            // (UNIVERSAL [continue into next byte])
            byte[] data = { 0x1F };
            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);

            try
            {
                reader.PeekTag();
                Assert.True(false, "CryptographicException was thrown");
            }
            catch (CryptographicException)
            {
            }
        }

        [Fact]
        public static void PeekEncodedValue_Primitive()
        {
            const string EncodedContents = "010203040506";
            const string EncodedValue = "0406" + EncodedContents;

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());

            // It's Peek, so it's reproducible.
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());
        }

        [Fact]
        public static void PeekEncodedValue_Indefinite()
        {
            const string EncodedContents = "040101" + "04050203040506";
            const string EncodedValue = "2480" + EncodedContents + "0000";

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());

            // It's Peek, so it's reproducible.
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());
        }

        [Fact]
        public static void PeekEncodedValue_Corrupt_Throws()
        {
            const string EncodedContents = "040101" + "04050203040506";
            // Constructed bit isn't set, so indefinite length is invalid.
            const string EncodedValue = "0480" + EncodedContents + "0000";

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
                    reader.PeekEncodedValue();
                });
        }

        [Fact]
        public static void PeekContentSpan_Primitive()
        {
            const string EncodedContents = "010203040506";
            const string EncodedValue = "0406" + EncodedContents;

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            Assert.Equal(EncodedContents, reader.PeekContentSpan().ByteArrayToHex());

            // It's Peek, so it's reproducible.
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());
        }

        [Fact]
        public static void PeekContentSpan_Indefinite()
        {
            const string EncodedContents = "040101" + "04050203040506";
            const string EncodedValue = "2480" + EncodedContents + "0000";

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
            Assert.Equal(EncodedContents, reader.PeekContentSpan().ByteArrayToHex());

            // It's Peek, so it's reproducible.
            Assert.Equal(EncodedValue, reader.PeekEncodedValue().ByteArrayToHex());
        }

        [Fact]
        public static void PeekContentSpan_Corrupt_Throws()
        {
            const string EncodedContents = "040101" + "04050203040506";
            // Constructed bit isn't set, so indefinite length is invalid.
            const string EncodedValue = "0480" + EncodedContents + "0000";

            byte[] data = (EncodedValue + "0500").HexToByteArray();

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(data, AsnEncodingRules.BER);
                    reader.PeekContentSpan();
                });
        }
    }
}
