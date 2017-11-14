// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadBoolean : Asn1ReaderTests
    {
        [Theory]
        [InlineData(PublicEncodingRules.BER, false, 3, "010100")]
        [InlineData(PublicEncodingRules.BER, true, 3, "010101")]
        // Padded length
        [InlineData(PublicEncodingRules.BER, true, 4, "01810101")]
        [InlineData(PublicEncodingRules.BER, true, 3, "0101FF0500")]
        [InlineData(PublicEncodingRules.CER, false, 3, "0101000500")]
        [InlineData(PublicEncodingRules.CER, true, 3, "0101FF")]
        [InlineData(PublicEncodingRules.DER, false, 3, "010100")]
        [InlineData(PublicEncodingRules.DER, true, 3, "0101FF0500")]
        // Context Specific 0
        [InlineData(PublicEncodingRules.DER, true, 3, "8001FF0500")]
        // Application 31
        [InlineData(PublicEncodingRules.DER, true, 4, "5F1F01FF0500")]
        // Private 253
        [InlineData(PublicEncodingRules.CER, false, 5, "DF817D01000500")]
        public static void ReadBoolean_Success(
            PublicEncodingRules ruleSet,
            bool expectedValue,
            int expectedBytesRead,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            bool value = reader.ReadBoolean();

            if (inputData.Length == expectedBytesRead)
            {
                Assert.False(reader.HasData, "reader.HasData");
            }
            else
            {
                Assert.True(reader.HasData, "reader.HasData");
            }

            if (expectedValue)
            {
                Assert.True(value, "value");
            }
            else
            {
                Assert.False(value, "value");
            }
        }

        [Theory]
        [InlineData("Empty", PublicEncodingRules.DER, "")]
        [InlineData("Empty", PublicEncodingRules.CER, "")]
        [InlineData("Empty", PublicEncodingRules.BER, "")]
        [InlineData("TagOnly", PublicEncodingRules.BER, "01")]
        [InlineData("TagOnly", PublicEncodingRules.CER, "01")]
        [InlineData("TagOnly", PublicEncodingRules.DER, "01")]
        [InlineData("MultiByte TagOnly", PublicEncodingRules.DER, "9F1F")]
        [InlineData("MultiByte TagOnly", PublicEncodingRules.CER, "9F1F")]
        [InlineData("MultiByte TagOnly", PublicEncodingRules.BER, "9F1F")]
        [InlineData("TagAndLength", PublicEncodingRules.BER, "0101")]
        [InlineData("Tag and MultiByteLength", PublicEncodingRules.BER, "01820001")]
        [InlineData("TagAndLength", PublicEncodingRules.CER, "8001")]
        [InlineData("TagAndLength", PublicEncodingRules.DER, "C001")]
        [InlineData("MultiByteTagAndLength", PublicEncodingRules.DER, "9F2001")]
        [InlineData("MultiByteTagAndLength", PublicEncodingRules.CER, "9F2001")]
        [InlineData("MultiByteTagAndLength", PublicEncodingRules.BER, "9F2001")]
        [InlineData("MultiByteTagAndMultiByteLength", PublicEncodingRules.BER, "9F28200001")]
        [InlineData("TooShort", PublicEncodingRules.BER, "0100")]
        [InlineData("TooShort", PublicEncodingRules.CER, "8000")]
        [InlineData("TooShort", PublicEncodingRules.DER, "0100")]
        [InlineData("TooLong", PublicEncodingRules.DER, "C0020000")]
        [InlineData("TooLong", PublicEncodingRules.CER, "01020000")]
        [InlineData("TooLong", PublicEncodingRules.BER, "C081020000")]
        [InlineData("MissingContents", PublicEncodingRules.BER, "C001")]
        [InlineData("MissingContents", PublicEncodingRules.CER, "0101")]
        [InlineData("MissingContents", PublicEncodingRules.DER, "8001")]
        [InlineData("NonCanonical", PublicEncodingRules.DER, "0101FE")]
        [InlineData("NonCanonical", PublicEncodingRules.CER, "800101")]
        [InlineData("Constructed", PublicEncodingRules.BER, "2103010101")]
        [InlineData("Constructed", PublicEncodingRules.CER, "2103010101")]
        [InlineData("Constructed", PublicEncodingRules.DER, "2103010101")]
        [InlineData("WrongTag", PublicEncodingRules.DER, "0400")]
        [InlineData("WrongTag", PublicEncodingRules.CER, "0400")]
        [InlineData("WrongTag", PublicEncodingRules.BER, "0400")]
        public static void ReadBoolean_Failure(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            try
            {
                reader.ReadBoolean();
                Assert.True(false, "CryptographicException was thrown");
            }
            catch (CryptographicException)
            {
            }

            if (inputData.Length == 0)
            {
                // If we started with nothing, where did the data come from?
                Assert.False(reader.HasData, "reader.HasData");
            }
            else
            {
                // Nothing should have moved
                Assert.True(reader.HasData, "reader.HasData");
            }
        }
    }
}
