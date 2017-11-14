// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadNull : Asn1ReaderTests
    {
        [Theory]
        [InlineData(PublicEncodingRules.BER, "0500")]
        [InlineData(PublicEncodingRules.CER, "0500")]
        [InlineData(PublicEncodingRules.DER, "0500")]
        [InlineData(PublicEncodingRules.BER, "0583000000")]
        public static void ReadNull_Success(PublicEncodingRules ruleSet, string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            reader.ReadNull();
            Assert.False(reader.HasData, "reader.HasData");
        }

        [Theory]
        [InlineData("Long length", PublicEncodingRules.CER, "0583000000")]
        [InlineData("Long length", PublicEncodingRules.DER, "0583000000")]
        [InlineData("Constructed definite length", PublicEncodingRules.BER, "2500")]
        [InlineData("Constructed definite length", PublicEncodingRules.DER, "2500")]
        [InlineData("Constructed indefinite length", PublicEncodingRules.BER, "25800000")]
        [InlineData("Constructed indefinite length", PublicEncodingRules.CER, "25800000")]
        [InlineData("No length", PublicEncodingRules.BER, "05")]
        [InlineData("No length", PublicEncodingRules.CER, "05")]
        [InlineData("No length", PublicEncodingRules.DER, "05")]
        [InlineData("No data", PublicEncodingRules.BER, "")]
        [InlineData("No data", PublicEncodingRules.CER, "")]
        [InlineData("No data", PublicEncodingRules.DER, "")]
        [InlineData("NonEmpty", PublicEncodingRules.BER, "050100")]
        [InlineData("NonEmpty", PublicEncodingRules.CER, "050100")]
        [InlineData("NonEmpty", PublicEncodingRules.DER, "050100")]
        [InlineData("Incomplete length", PublicEncodingRules.BER, "0581")]
        public static void ReadNull_Throws(string description, PublicEncodingRules ruleSet, string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

                    reader.ReadNull();
                });
        }
    }
}
