﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadBMPString : Asn1ReaderTests
    {
        public static IEnumerable<object[]> ValidEncodingData { get; } =
            new object[][]
            {
                new object[]
                {
                    PublicEncodingRules.BER,
                    "1E1A004A006F0068006E00200051002E00200053006D006900740068",
                    "John Q. Smith",
                },
                new object[]
                {
                    PublicEncodingRules.CER,
                    "1E1A004A006F0068006E00200051002E00200053006D006900740068",
                    "John Q. Smith",
                },
                new object[]
                {
                    PublicEncodingRules.DER,
                    "1E1A004A006F0068006E00200051002E00200053006D006900740068",
                    "John Q. Smith",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "3E80" + "041A004A006F0068006E00200051002E00200053006D006900740068" + "0000",
                    "John Q. Smith",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "3E1C" + "041A004A006F0068006E00200051002E00200053006D006900740068",
                    "John Q. Smith",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "1E00",
                    "",
                },
                new object[]
                {
                    PublicEncodingRules.CER,
                    "1E00",
                    "",
                },
                new object[]
                {
                    PublicEncodingRules.DER,
                    "1E00",
                    "",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "3E00",
                    "",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "3E80" + "0000",
                    "",
                },
                new object[]
                {
                    PublicEncodingRules.BER,
                    "3E80" +
                      "2480" +
                        // "Dr."
                        "040600440072002E" +
                        // " & "
                        "0406002000260020" +
                        // "Mrs."
                        "0408004D00720073002E" +
                        "0000" +
                      // " "
                      "04020020" +
                      "2480" +
                        "2410" +
                          // "Smith"
                          "040A0053006D006900740068" +
                          // hyphen (U+2010)
                          "04022010" +
                        "0000" +
                      // "Jones"
                      "040A004A006F006E00650073" +
                      "2480" +
                        // " "
                        "04020020" +
                        "2480" +
                          // The next two bytes are U+FE60, small ampersand
                          // Since UCS-2 would always chunk evenly under CER the odds of
                          // misaligned data are low in reality, but maybe some BER encoder
                          // chunks odd, so a split scenario could still happen.
                          "0401FE" +
                          "040160" +
                          "0000" +
                        // " "
                        "04020020" +
                        // "children"
                        "0410006300680069006C006400720065006E" +
                        "0000" +
                      "0000",
                    "Dr. & Mrs. Smith\u2010Jones \uFE60 children",
                },
            };

        [Theory]
        [MemberData(nameof(ValidEncodingData))]
        public static void GetBMPString_Success(
            PublicEncodingRules ruleSet,
            string inputHex,
            string expectedValue)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            string value = reader.GetCharacterString(UniversalTagNumber.BMPString);

            Assert.Equal(expectedValue, value);
        }

        [Theory]
        [MemberData(nameof(ValidEncodingData))]
        public static void TryCopyBMPString(
            PublicEncodingRules ruleSet,
            string inputHex,
            string expectedValue)
        {
            byte[] inputData = inputHex.HexToByteArray();
            char[] output = new char[expectedValue.Length];

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            bool copied;
            int charsWritten;

            if (output.Length > 0)
            {
                output[0] = 'a';

                copied = reader.TryCopyBMPString(
                    output.AsSpan().Slice(0, expectedValue.Length - 1),
                    out charsWritten);

                Assert.False(copied, "reader.TryCopyBMPString - too short");
                Assert.Equal(0, charsWritten);
                Assert.Equal('a', output[0]);
            }

            copied = reader.TryCopyBMPString(
                output,
                out charsWritten);

            Assert.True(copied, "reader.TryCopyBMPString");

            string actualValue = new string(output, 0, charsWritten);
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [MemberData(nameof(ValidEncodingData))]
        public static void TryCopyBMPStringBytes(
            PublicEncodingRules ruleSet,
            string inputHex,
            string expectedString)
        {
            byte[] inputData = inputHex.HexToByteArray();
            string expectedHex = Text.Encoding.BigEndianUnicode.GetBytes(expectedString).ByteArrayToHex();
            byte[] output = new byte[expectedHex.Length / 2];

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            bool copied;
            int bytesWritten;

            if (output.Length > 0)
            {
                output[0] = 32;

                copied = reader.TryCopyBMPStringBytes(output.AsSpan().Slice(0, output.Length - 1),
                    out bytesWritten);

                Assert.False(copied, "reader.TryCopyBMPStringBytes - too short");
                Assert.Equal(0, bytesWritten);
                Assert.Equal(32, output[0]);
            }

            copied = reader.TryCopyBMPStringBytes(output,
                out bytesWritten);

            Assert.True(copied, "reader.TryCopyBMPStringBytes");

            Assert.Equal(
                expectedHex,
                new ReadOnlySpan<byte>(output, 0, bytesWritten).ByteArrayToHex());

            Assert.Equal(output.Length, bytesWritten);
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER, "1E020020", true)]
        [InlineData(PublicEncodingRules.BER, "3E80" + "04020020" + "0000", false)]
        [InlineData(PublicEncodingRules.BER, "3E04" + "04020020", false)]
        public static void TryGetBMPStringBytes(
            PublicEncodingRules ruleSet,
            string inputHex,
            bool expectSuccess)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            bool got = reader.TryGetBMPStringBytes(out ReadOnlyMemory<byte> contents);

            if (expectSuccess)
            {
                Assert.True(got, "reader.TryGetBMPStringBytes");

                unsafe
                {
                    fixed (byte* inputPtr = &inputData[2])
                    fixed (byte* matchPtr = &contents.Span.DangerousGetPinnableReference())
                    {
                        Assert.Equal((IntPtr)inputPtr, (IntPtr)matchPtr);
                    }
                }
            }
            else
            {
                Assert.False(got, "reader.TryGetBMPStringBytes");
                Assert.True(contents.IsEmpty, "contents.IsEmpty");
            }
        }

        [Theory]
        [InlineData("Incomplete Tag", PublicEncodingRules.BER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.CER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.DER, "1F")]
        [InlineData("Missing Length", PublicEncodingRules.BER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.CER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.DER, "1E")]
        [InlineData("Missing Contents", PublicEncodingRules.BER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.CER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.DER, "1E02")]
        [InlineData("Length Too Long", PublicEncodingRules.BER, "1E0600480069")]
        [InlineData("Length Too Long", PublicEncodingRules.CER, "1E0600480069")]
        [InlineData("Length Too Long", PublicEncodingRules.DER, "1E0600480069")]
        [InlineData("Constructed Form", PublicEncodingRules.DER, "3E0404020049")]
        public static void TryGetBMPStringBytes_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () =>
                {
                    reader.TryGetBMPStringBytes(out ReadOnlyMemory<byte> contents);
                });
        }

        [Theory]
        [InlineData("Empty", PublicEncodingRules.BER, "")]
        [InlineData("Empty", PublicEncodingRules.CER, "")]
        [InlineData("Empty", PublicEncodingRules.DER, "")]
        [InlineData("Incomplete Tag", PublicEncodingRules.BER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.CER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.DER, "1F")]
        [InlineData("Missing Length", PublicEncodingRules.BER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.CER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.DER, "1E")]
        [InlineData("Missing Contents", PublicEncodingRules.BER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.CER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.DER, "1E02")]
        [InlineData("Missing Contents - Constructed", PublicEncodingRules.BER, "3E02")]
        [InlineData("Missing Contents - Constructed Indef", PublicEncodingRules.BER, "3E80")]
        [InlineData("Missing Contents - Constructed Indef", PublicEncodingRules.CER, "3E80")]
        [InlineData("Length Too Long", PublicEncodingRules.BER, "1E034869")]
        [InlineData("Length Too Long", PublicEncodingRules.CER, "1E034869")]
        [InlineData("Length Too Long", PublicEncodingRules.DER, "1E034869")]
        [InlineData("Definite Constructed Form", PublicEncodingRules.CER, "3E03040149")]
        [InlineData("Definite Constructed Form", PublicEncodingRules.DER, "3E03040149")]
        [InlineData("Indefinite Constructed Form - Short Payload", PublicEncodingRules.CER, "3E800401490000")]
        [InlineData("Indefinite Constructed Form", PublicEncodingRules.DER, "3E800401490000")]
        [InlineData("No nested content", PublicEncodingRules.CER, "3E800000")]
        [InlineData("No EoC", PublicEncodingRules.BER, "3E80" + "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.BER, "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.CER, "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.DER, "04024869")]
        [InlineData("Wrong Tag - Constructed", PublicEncodingRules.BER, "240404024869")]
        [InlineData("Wrong Tag - Constructed Indef", PublicEncodingRules.BER, "2480" + "04024869" + "0000")]
        [InlineData("Wrong Tag - Constructed Indef", PublicEncodingRules.CER, "2480" + "04024869" + "0000")]
        [InlineData("Wrong Tag - Constructed", PublicEncodingRules.DER, "240404024869")]
        [InlineData("Nested Bad Tag", PublicEncodingRules.BER, "3E04" + "1E024869")]
        [InlineData("Nested context-specific", PublicEncodingRules.BER, "3E04800400FACE")]
        [InlineData("Nested context-specific (indef)", PublicEncodingRules.BER, "3E80800400FACE0000")]
        [InlineData("Nested context-specific (indef)", PublicEncodingRules.CER, "3E80800400FACE0000")]
        [InlineData("Nested Length Too Long", PublicEncodingRules.BER, "3E07" + ("2402" + "0404") + "04020049")]
        [InlineData("Nested Simple Length Too Long", PublicEncodingRules.BER, "3E03" + "040548656C6C6F")]
        [InlineData("Constructed EndOfContents", PublicEncodingRules.BER, "3E8020000000")]
        [InlineData("Constructed EndOfContents", PublicEncodingRules.CER, "3E8020000000")]
        [InlineData("NonEmpty EndOfContents", PublicEncodingRules.BER, "3E80000100")]
        [InlineData("NonEmpty EndOfContents", PublicEncodingRules.CER, "3E80000100")]
        [InlineData("LongLength EndOfContents", PublicEncodingRules.BER, "3E80008100")]
        public static void TryCopyBMPStringBytes_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            byte[] outputData = new byte[inputData.Length + 1];
            outputData[0] = 252;

            int bytesWritten = -1;

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () =>
                {
                    reader.TryCopyBMPStringBytes(outputData, out bytesWritten);
                });

            Assert.Equal(-1, bytesWritten);
            Assert.Equal(252, outputData[0]);
        }

        private static void TryCopyBMPString_Throws(PublicEncodingRules ruleSet, byte[] inputData)
        {
            char[] outputData = new char[inputData.Length + 1];
            outputData[0] = 'a';

            int bytesWritten = -1;
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () =>
                {
                    reader.TryCopyBMPString(
                        outputData,
                        out bytesWritten);
                });

            Assert.Equal(-1, bytesWritten);
            Assert.Equal('a', outputData[0]);
        }

        [Theory]
        [InlineData("Incomplete Tag", PublicEncodingRules.BER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.CER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.DER, "1F")]
        [InlineData("Missing Length", PublicEncodingRules.BER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.CER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.DER, "1E")]
        [InlineData("Missing Contents", PublicEncodingRules.BER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.CER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.DER, "1E02")]
        [InlineData("Length Too Long", PublicEncodingRules.BER, "1E0600480069")]
        [InlineData("Length Too Long", PublicEncodingRules.CER, "1E0600480069")]
        [InlineData("Length Too Long", PublicEncodingRules.DER, "1E0600480069")]
        [InlineData("Constructed Form", PublicEncodingRules.DER, "3E0404020049")]
        [InlineData("Bad BMP value (odd length)", PublicEncodingRules.BER, "1E0120")]
        [InlineData("Bad BMP value (high surrogate)", PublicEncodingRules.BER, "1E02D800")]
        [InlineData("Bad BMP value (high private surrogate)", PublicEncodingRules.BER, "1E02DB81")]
        [InlineData("Bad BMP value (low surrogate)", PublicEncodingRules.BER, "1E02DC00")]
        [InlineData("Wrong Tag", PublicEncodingRules.BER, "04024869")]
        public static void GetBMPString_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () =>
                {
                    reader.GetCharacterString(UniversalTagNumber.BMPString);
                });
        }

        [Theory]
        [InlineData("Empty", PublicEncodingRules.BER, "")]
        [InlineData("Empty", PublicEncodingRules.CER, "")]
        [InlineData("Empty", PublicEncodingRules.DER, "")]
        [InlineData("Incomplete Tag", PublicEncodingRules.BER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.CER, "1F")]
        [InlineData("Incomplete Tag", PublicEncodingRules.DER, "1F")]
        [InlineData("Missing Length", PublicEncodingRules.BER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.CER, "1E")]
        [InlineData("Missing Length", PublicEncodingRules.DER, "1E")]
        [InlineData("Missing Contents", PublicEncodingRules.BER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.CER, "1E02")]
        [InlineData("Missing Contents", PublicEncodingRules.DER, "1E02")]
        [InlineData("Missing Contents - Constructed", PublicEncodingRules.BER, "3E02")]
        [InlineData("Missing Contents - Constructed Indef", PublicEncodingRules.BER, "3E80")]
        [InlineData("Missing Contents - Constructed Indef", PublicEncodingRules.CER, "3E80")]
        [InlineData("Length Too Long", PublicEncodingRules.BER, "1E034869")]
        [InlineData("Length Too Long", PublicEncodingRules.CER, "1E034869")]
        [InlineData("Length Too Long", PublicEncodingRules.DER, "1E034869")]
        [InlineData("Definite Constructed Form", PublicEncodingRules.CER, "3E03040149")]
        [InlineData("Definite Constructed Form", PublicEncodingRules.DER, "3E03040149")]
        [InlineData("Indefinite Constructed Form - Short Payload", PublicEncodingRules.CER, "3E800401490000")]
        [InlineData("Indefinite Constructed Form", PublicEncodingRules.DER, "3E800401490000")]
        [InlineData("No nested content", PublicEncodingRules.CER, "3E800000")]
        [InlineData("No EoC", PublicEncodingRules.BER, "3E80" + "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.BER, "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.CER, "04024869")]
        [InlineData("Wrong Tag - Primitive", PublicEncodingRules.DER, "04024869")]
        [InlineData("Wrong Tag - Constructed", PublicEncodingRules.BER, "240404024869")]
        [InlineData("Wrong Tag - Constructed Indef", PublicEncodingRules.BER, "2480" + "04024869" + "0000")]
        [InlineData("Wrong Tag - Constructed Indef", PublicEncodingRules.CER, "2480" + "04024869" + "0000")]
        [InlineData("Wrong Tag - Constructed", PublicEncodingRules.DER, "240404024869")]
        [InlineData("Nested Bad Tag", PublicEncodingRules.BER, "3E04" + "1E024869")]
        [InlineData("Nested context-specific", PublicEncodingRules.BER, "3E04800400FACE")]
        [InlineData("Nested context-specific (indef)", PublicEncodingRules.BER, "3E80800400FACE0000")]
        [InlineData("Nested context-specific (indef)", PublicEncodingRules.CER, "3E80800400FACE0000")]
        [InlineData("Nested Length Too Long", PublicEncodingRules.BER, "3E07" + ("2402" + "0404") + "04020049")]
        [InlineData("Nested Simple Length Too Long", PublicEncodingRules.BER, "3E03" + "040548656C6C6F")]
        [InlineData("Constructed EndOfContents", PublicEncodingRules.BER, "3E8020000000")]
        [InlineData("Constructed EndOfContents", PublicEncodingRules.CER, "3E8020000000")]
        [InlineData("NonEmpty EndOfContents", PublicEncodingRules.BER, "3E80000100")]
        [InlineData("NonEmpty EndOfContents", PublicEncodingRules.CER, "3E80000100")]
        [InlineData("LongLength EndOfContents", PublicEncodingRules.BER, "3E80008100")]
        [InlineData("Bad BMP value (odd length)", PublicEncodingRules.BER, "1E0120")]
        [InlineData("Bad BMP value (high surrogate)", PublicEncodingRules.BER, "1E02D800")]
        [InlineData("Bad BMP value (high private surrogate)", PublicEncodingRules.BER, "1E02DB81")]
        [InlineData("Bad BMP value (low surrogate)", PublicEncodingRules.BER, "1E02DC00")]
        public static void TryCopyBMPString_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();
            TryCopyBMPString_Throws(ruleSet, inputData);
        }

        [Fact]
        public static void TryCopyBMPString_Throws_CER_NestedTooLong()
        {
            // CER says that the maximum encoding length for a BMPString primitive
            // is 1000.
            //
            // This test checks it for a primitive contained within a constructed.
            //
            // So we need 04 [1001] { 1001 0x00s }
            // 1001 => 0x3E9, so the length encoding is 82 03 E9.
            // 1001 + 3 + 1 == 1005
            //
            // Plus a leading 3E 80 (indefinite length constructed)
            // and a trailing 00 00 (End of contents)
            // == 1009
            byte[] input = new byte[1009];
            // CONSTRUCTED BMPSTRING (indefinite)
            input[0] = 0x3E;
            input[1] = 0x80;
            // OCTET STRING (1001)
            input[2] = 0x04;
            input[3] = 0x82;
            input[4] = 0x03;
            input[5] = 0xE9;
            // EOC implicit since the byte[] initializes to zeros

            TryCopyBMPString_Throws(PublicEncodingRules.CER, input);
        }

        [Fact]
        public static void TryCopyBMPString_Throws_CER_NestedTooShortIntermediate()
        {
            // CER says that the maximum encoding length for a BMPString primitive
            // is 1000, and in the constructed form the lengths must be
            // [ 1000, 1000, 1000, ..., len%1000 ]
            //
            // So 1000, 2, 2 is illegal.
            //
            // 3E 80 (indefinite constructed BMP string)
            //    04 82 03 08 (octet string, 1000 bytes)
            //       [1000 content bytes]
            //    04 02 (octet string, 2 bytes)
            //       [2 content bytes]
            //    04 02 (octet string, 2 bytes)
            //       [2 content bytes]
            //    00 00 (end of contents)
            // Looks like 1,016 bytes.
            byte[] input = new byte[1016];
            // CONSTRUCTED BMP STRING (indefinite)
            input[0] = 0x3E;
            input[1] = 0x80;
            // OCTET STRING (1000)
            input[2] = 0x03;
            input[3] = 0x82;
            input[4] = 0x03;
            input[5] = 0xE8;
            // OCTET STRING (2)
            input[1006] = 0x04;
            input[1007] = 0x02;
            // OCTET STRING (2)
            input[1010] = 0x04;
            input[1011] = 0x02;
            // EOC implicit since the byte[] initializes to zeros

            TryCopyBMPString_Throws(PublicEncodingRules.CER, input);
        }

        [Fact]
        public static void TryCopyBMPStringBytes_Success_CER_MaxPrimitiveLength()
        {
            // CER says that the maximum encoding length for a BMPString primitive
            // is 1000.
            //
            // So we need 1E [1000] { 1000 anythings }
            // 1000 => 0x3E8, so the length encoding is 82 03 E8.
            // 1000 + 3 + 1 == 1004
            byte[] input = new byte[1004];
            input[0] = 0x1E;
            input[1] = 0x82;
            input[2] = 0x03;
            input[3] = 0xE8;

            // Content
            input[4] = 0x65;
            input[5] = 0x65;
            input[1002] = 0x61;
            input[1003] = 0x61;

            byte[] output = new byte[1000];

            AsnReader reader = new AsnReader(input, AsnEncodingRules.CER);
            bool success = reader.TryCopyBMPStringBytes(output, out int bytesWritten);

            Assert.True(success, "reader.TryCopyBMPStringBytes");
            Assert.Equal(1000, bytesWritten);

            Assert.Equal(
                input.AsReadOnlySpan().Slice(4).ByteArrayToHex(),
                output.ByteArrayToHex());
        }

        [Fact]
        public static void TryCopyBMPStringBytes_Success_CER_MinConstructedLength()
        {
            // CER says that the maximum encoding length for a BMPString primitive
            // is 1000, and that a constructed form must be used for values greater
            // than 1000 bytes, with segments dividing up for each thousand
            // [1000, 1000, ..., len%1000].
            //
            // So our smallest constructed form is 1001 bytes, [1000, 1]
            //
            // 3E 80 (indefinite constructed BMPString)
            //    04 82 03 E9 (primitive octet string, 1000 bytes)
            //       [1000 content bytes]
            //    04 01 (primitive octet string, 1 byte)
            //       pp
            //    00 00 (end of contents, 0 bytes)
            // 1011 total.
            byte[] input = new byte[1011];
            int offset = 0;
            // CONSTRUCTED BMPSTRING (Indefinite)
            input[offset++] = 0x3E;
            input[offset++] = 0x80;
            // OCTET STRING (1000)
            input[offset++] = 0x04;
            input[offset++] = 0x82;
            input[offset++] = 0x03;
            input[offset++] = 0xE8;

            // Primitive 1: (65 65 :: 61 61) (1000)
            input[offset++] = 0x65;
            input[offset] = 0x65;
            offset += 997;
            input[offset++] = 0x61;
            input[offset++] = 0x61;

            // OCTET STRING (1)
            input[offset++] = 0x04;
            input[offset++] = 0x01;

            // Primitive 2: One more byte
            input[offset] = 0x2E;

            byte[] expected = new byte[1001];
            offset = 0;
            expected[offset++] = 0x65;
            expected[offset] = 0x65;
            offset += 997;
            expected[offset++] = 0x61;
            expected[offset++] = 0x61;
            expected[offset] = 0x2E;

            byte[] output = new byte[1001];

            AsnReader reader = new AsnReader(input, AsnEncodingRules.CER);
            bool success = reader.TryCopyBMPStringBytes(output, out int bytesWritten);

            Assert.True(success, "reader.TryCopyBMPStringBytes");
            Assert.Equal(1001, bytesWritten);

            Assert.Equal(
                expected.ByteArrayToHex(),
                output.ByteArrayToHex());
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void TagMustBeCorrect_Universal(PublicEncodingRules ruleSet)
        {
            byte[] inputData = { 0x1E, 4, 0, (byte)'h', 0, (byte)'i' };
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentException>(
                "expectedTag",
                () => reader.TryGetBMPStringBytes(Asn1Tag.Null, out _));

            Assert.True(reader.HasData, "HasData after bad universal tag");

            Assert.Throws<CryptographicException>(
                () => reader.TryGetBMPStringBytes(new Asn1Tag(TagClass.ContextSpecific, 0), out _));

            Assert.True(reader.HasData, "HasData after wrong tag");

            Assert.True(reader.TryGetBMPStringBytes(out ReadOnlyMemory<byte> value));
            Assert.Equal("00680069", value.ByteArrayToHex());
            Assert.False(reader.HasData, "HasData after read");
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void TagMustBeCorrect_Custom(PublicEncodingRules ruleSet)
        {
            byte[] inputData = { 0x87, 2, 0x20, 0x10 };
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentException>(
                "expectedTag",
                () => reader.TryGetBMPStringBytes(Asn1Tag.Null, out _));

            Assert.True(reader.HasData, "HasData after bad universal tag");

            Assert.Throws<CryptographicException>(() => reader.TryGetBMPStringBytes(out _));

            Assert.True(reader.HasData, "HasData after default tag");

            Assert.Throws<CryptographicException>(
                () => reader.TryGetBMPStringBytes(new Asn1Tag(TagClass.Application, 0), out _));

            Assert.True(reader.HasData, "HasData after wrong custom class");

            Assert.Throws<CryptographicException>(
                () => reader.TryGetBMPStringBytes(new Asn1Tag(TagClass.ContextSpecific, 1), out _));

            Assert.True(reader.HasData, "HasData after wrong custom tag value");

            Assert.True(
                reader.TryGetBMPStringBytes(
                    new Asn1Tag(TagClass.ContextSpecific, 7),
                    out ReadOnlyMemory<byte> value));

            Assert.Equal("2010", value.ByteArrayToHex());
            Assert.False(reader.HasData, "HasData after reading value");
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER, "1E022010", PublicTagClass.Universal, 30)]
        [InlineData(PublicEncodingRules.CER, "1E022010", PublicTagClass.Universal, 30)]
        [InlineData(PublicEncodingRules.DER, "1E022010", PublicTagClass.Universal, 30)]
        [InlineData(PublicEncodingRules.BER, "8002FE60", PublicTagClass.ContextSpecific, 0)]
        [InlineData(PublicEncodingRules.CER, "4C02FE60", PublicTagClass.Application, 12)]
        [InlineData(PublicEncodingRules.DER, "DF8A4602FE60", PublicTagClass.Private, 1350)]
        public static void ExpectedTag_IgnoresConstructed(
            PublicEncodingRules ruleSet,
            string inputHex,
            PublicTagClass tagClass,
            int tagValue)
        {
            byte[] inputData = inputHex.HexToByteArray();
            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.True(
                reader.TryGetBMPStringBytes(
                    new Asn1Tag((TagClass)tagClass, tagValue, true),
                    out ReadOnlyMemory<byte> val1));

            Assert.False(reader.HasData);

            reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);

            Assert.True(
                reader.TryGetBMPStringBytes(
                    new Asn1Tag((TagClass)tagClass, tagValue, false),
                    out ReadOnlyMemory<byte> val2));

            Assert.False(reader.HasData);

            Assert.Equal(val1.ByteArrayToHex(), val2.ByteArrayToHex());
        }
    }
}
