// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadSetOf : Asn1ReaderTests
    {
        [Theory]
        [InlineData(PublicEncodingRules.BER, "3100", false, -1)]
        [InlineData(PublicEncodingRules.BER, "31800000", false, -1)]
        [InlineData(PublicEncodingRules.BER, "3183000000", false, -1)]
        [InlineData(PublicEncodingRules.CER, "31800000", false, -1)]
        [InlineData(PublicEncodingRules.DER, "3100", false, -1)]
        [InlineData(PublicEncodingRules.BER, "3100" + "0500", true, -1)]
        [InlineData(PublicEncodingRules.BER, "3102" + "0500", false, 5)]
        [InlineData(PublicEncodingRules.CER, "3180" + "0500" + "0000", false, 5)]
        [InlineData(PublicEncodingRules.CER, "3180" + "010100" + "0000" + "0500", true, 1)]
        [InlineData(PublicEncodingRules.CER, "3180" + "010100" + "0101FF" + "0500" + "0000", false, 1)]
        [InlineData(PublicEncodingRules.DER, "3105" + "0101FF" + "0500", false, 1)]
        public static void ReadSetOf_Success(
            PublicEncodingRules ruleSet,
            string inputHex,
            bool expectDataRemaining,
            int expectedSequenceTagNumber)
        {
            byte[] inputData = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            AsnReader sequence = reader.ReadSetOf();

            if (expectDataRemaining)
            {
                Assert.True(reader.HasData, "reader.HasData");
            }
            else
            {
                Assert.False(reader.HasData, "reader.HasData");
            }

            if (expectedSequenceTagNumber < 0)
            {
                Assert.False(sequence.HasData, "sequence.HasData");
            }
            else
            {
                Assert.True(sequence.HasData, "sequence.HasData");

                Asn1Tag firstTag = sequence.PeekTag();
                Assert.Equal(expectedSequenceTagNumber, firstTag.TagValue);
            }
        }

        [Theory]
        [InlineData("Empty", PublicEncodingRules.BER, "")]
        [InlineData("Empty", PublicEncodingRules.CER, "")]
        [InlineData("Empty", PublicEncodingRules.DER, "")]
        [InlineData("Incomplete Tag", PublicEncodingRules.BER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.CER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.DER, "1F")]
        [InlineData("Missing Length", PublicEncodingRules.BER, "31")]
        [InlineData("Missing Length", PublicEncodingRules.CER, "31")]
        [InlineData("Missing Length", PublicEncodingRules.DER, "31")]
        [InlineData("Primitive Encoding", PublicEncodingRules.BER, "1100")]
        [InlineData("Primitive Encoding", PublicEncodingRules.CER, "1100")]
        [InlineData("Primitive Encoding", PublicEncodingRules.DER, "1100")]
        [InlineData("Definite Length Encoding", PublicEncodingRules.CER, "3100")]
        [InlineData("Indefinite Length Encoding", PublicEncodingRules.DER, "3180" + "0000")]
        [InlineData("Missing Content", PublicEncodingRules.BER, "3101")]
        [InlineData("Missing Content", PublicEncodingRules.DER, "3101")]
        [InlineData("Length Out Of Bounds", PublicEncodingRules.BER, "3105" + "010100")]
        [InlineData("Length Out Of Bounds", PublicEncodingRules.DER, "3105" + "010100")]
        [InlineData("Missing Content - Indefinite", PublicEncodingRules.BER, "3180")]
        [InlineData("Missing Content - Indefinite", PublicEncodingRules.CER, "3180")]
        [InlineData("Missing EoC", PublicEncodingRules.BER, "3180" + "010100")]
        [InlineData("Missing EoC", PublicEncodingRules.CER, "3180" + "010100")]
        [InlineData("Missing Outer EoC", PublicEncodingRules.BER, "3180" + "010100" + ("3180" + "0000"))]
        [InlineData("Missing Outer EoC", PublicEncodingRules.CER, "3180" + "010100" + ("3180" + "0000"))]
        [InlineData("Wrong Tag - Definite", PublicEncodingRules.BER, "3000")]
        [InlineData("Wrong Tag - Definite", PublicEncodingRules.DER, "3000")]
        [InlineData("Wrong Tag - Indefinite", PublicEncodingRules.BER, "3080" + "0000")]
        [InlineData("Wrong Tag - Indefinite", PublicEncodingRules.CER, "3080" + "0000")]
        public static void ReadSetOf_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

                    reader.ReadSetOf();
                });
        }

        [Theory]
        // BER can read out of order (indefinite)
        [InlineData(PublicEncodingRules.BER, "3180" + "0101FF" + "010100" + "0000", true, 1)]
        // BER can read out of order (definite)
        [InlineData(PublicEncodingRules.BER, "3106" + "0101FF" + "010100", true, 1)]
        // CER will not read out of order
        [InlineData(PublicEncodingRules.CER, "3180" + "0500" + "010100" + "0000", false, 1)]
        [InlineData(PublicEncodingRules.CER, "3180" + "0101FF" + "010100" + "0000", false, 1)]
        // CER is happy in order:
        [InlineData(PublicEncodingRules.CER, "3180" + "010100" + "0500" + "0000", true, 5)]
        [InlineData(PublicEncodingRules.CER, "3180" + "010100" + "0101FF" + "0500" + "0000", true, 5)]
        [InlineData(PublicEncodingRules.CER, "3180" + "010100" + "010100" + "0500" + "0000", true, 5)]
        // DER will not read out of order
        [InlineData(PublicEncodingRules.DER, "3106" + "0101FF" + "010100", false, 1)]
        [InlineData(PublicEncodingRules.DER, "3105" + "0500" + "010100", false, 1)]
        // DER is happy in order:
        [InlineData(PublicEncodingRules.DER, "3105" + "010100" + "0500", true, 5)]
        [InlineData(PublicEncodingRules.DER, "3108" + "010100" + "0101FF" + "0500", true, 5)]
        [InlineData(PublicEncodingRules.DER, "3108" + "010100" + "010100" + "0500", true, 5)]
        public static void ReadSetOf_DataSorting(
            PublicEncodingRules ruleSet,
            string inputHex,
            bool expectSuccess,
            int lastTagValue)
        {
            byte[] inputData = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            AsnReader setOf;

            if (expectSuccess)
            {
                setOf = reader.ReadSetOf();
            }
            else
            {
                Assert.Throws<CryptographicException>(
                    () =>
                    {
                        AsnReader alsoReader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
                        alsoReader.ReadSetOf();
                    });

                setOf = reader.ReadSetOf(skipSortOrderValidation: true);
            }

            int lastTag = -1;

            while (setOf.HasData)
            {
                Asn1Tag tag = setOf.PeekTag();
                lastTag = tag.TagValue;

                setOf.SkipValue();
            }

            Assert.Equal(lastTagValue, lastTag);
        }
    }
}
