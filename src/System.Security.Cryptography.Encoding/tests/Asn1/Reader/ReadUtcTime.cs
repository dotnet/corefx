// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadUtcTime : Asn1ReaderTests
    {
        [Theory]
        // A, B2, C2
        [InlineData(PublicEncodingRules.BER, "17113137303930383130333530332D30373030", 2017, 9, 8, 10, 35, 3, -7, 0)]
        [InlineData(PublicEncodingRules.BER, "17113137303930383130333530332D30303530", 2017, 9, 8, 10, 35, 3, 0, -50)]
        [InlineData(PublicEncodingRules.BER, "17113137303930383130333530332B30373030", 2017, 9, 8, 10, 35, 3, 7, 0)]
        [InlineData(PublicEncodingRules.BER, "17113030303130313030303030302B30303030", 2000, 1, 1, 0, 0, 0, 0, 0)]
        [InlineData(PublicEncodingRules.BER, "17113030303130313030303030302D31343030", 2000, 1, 1, 0, 0, 0, -14, 0)]
        // A, B2, C1 (only legal form for CER or DER)
        [InlineData(PublicEncodingRules.BER, "170D3132303130323233353935395A", 2012, 1, 2, 23, 59, 59, 0, 0)]
        [InlineData(PublicEncodingRules.CER, "170D3439313233313233353935395A", 2049, 12, 31, 23, 59, 59, 0, 0)]
        [InlineData(PublicEncodingRules.DER, "170D3530303130323132333435365A", 1950, 1, 2, 12, 34, 56, 0, 0)]
        // A, B1, C2
        [InlineData(PublicEncodingRules.BER, "170F313730393038313033352D30373030", 2017, 9, 8, 10, 35, 0, -7, 0)]
        [InlineData(PublicEncodingRules.BER, "170F323730393038313033352B30393132", 2027, 9, 8, 10, 35, 0, 9, 12)]
        // A, B1, C1
        [InlineData(PublicEncodingRules.BER, "170B313230313032323335395A", 2012, 1, 2, 23, 59, 0, 0, 0)]
        [InlineData(PublicEncodingRules.BER, "170B343931323331323335395A", 2049, 12, 31, 23, 59, 0, 0, 0)]
        // BER Constructed form
        [InlineData(
            PublicEncodingRules.BER,
            "3780" +
              "04023132" +
              "04023031" +
              "2480" + "040130" + "040132" + "0000" +
              "040432333539" +
              "04830000015A" +
              "0000",
            2012, 1, 2, 23, 59, 0, 0, 0)]
        public static void ParseTime_Valid(
            PublicEncodingRules ruleSet,
            string inputHex,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            int offsetHour,
            int offsetMinute)
        {
            byte[] inputData = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
            DateTimeOffset value = reader.GetUtcTime();

            Assert.Equal(year, value.Year);
            Assert.Equal(month, value.Month);
            Assert.Equal(day, value.Day);
            Assert.Equal(hour, value.Hour);
            Assert.Equal(minute, value.Minute);
            Assert.Equal(second, value.Second);
            Assert.Equal(0, value.Millisecond);
            Assert.Equal(new TimeSpan(offsetHour, offsetMinute, 0), value.Offset);
        }

        [Fact]
        public static void ParseTime_InvalidValue_LegalString()
        {
            byte[] inputData = "17113030303030303030303030302D33333030".HexToByteArray();

            var exception = Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(inputData, AsnEncodingRules.BER);
                    reader.GetUtcTime();
                });

            Assert.NotNull(exception.InnerException);
            Assert.IsType<ArgumentOutOfRangeException>(exception.InnerException);
        }

        [Theory]
        [InlineData(2011, 1912)]
        [InlineData(2012, 2012)]
        [InlineData(2013, 2012)]
        [InlineData(2111, 2012)]
        [InlineData(2112, 2112)]
        [InlineData(2113, 2112)]
        [InlineData(12, 12)]
        [InlineData(99, 12)]
        [InlineData(111, 12)]
        public static void ReadUtcTime_TwoYearMaximum(int maximum, int interpretedYear)
        {
            byte[] inputData = "170D3132303130323233353935395A".HexToByteArray();

            AsnReader reader = new AsnReader(inputData, AsnEncodingRules.BER);
            DateTimeOffset value = reader.GetUtcTime(maximum);

            Assert.Equal(interpretedYear, value.Year);
        }

        [Theory]
        [InlineData("A,B2,C2", PublicEncodingRules.CER, "17113137303930383130333530332D30373030")]
        [InlineData("A,B2,C2", PublicEncodingRules.DER, "17113137303930383130333530332D30373030")]
        [InlineData("A,B1,C2", PublicEncodingRules.CER, "170F313730393038313033352D30373030")]
        [InlineData("A,B1,C2", PublicEncodingRules.DER, "170F313730393038313033352D30373030")]
        [InlineData("A,B1,C1", PublicEncodingRules.CER, "170B313230313032323335395A")]
        [InlineData("A,B1,C1", PublicEncodingRules.DER, "170B313230313032323335395A")]
        [InlineData("A,B1,C1-NotZ", PublicEncodingRules.BER, "170B313230313032323335392B")]
        [InlineData("A,B1,C2-NotPlusMinus", PublicEncodingRules.BER, "170F313730393038313033352C30373030")]
        [InlineData("A,B2,C2-NotPlusMinus", PublicEncodingRules.BER, "17113137303930383130333530332C30373030")]
        [InlineData("A1,B2,C1-NotZ", PublicEncodingRules.DER, "170D3530303130323132333435365B")]
        [InlineData("A,B2,C2-MissingDigit", PublicEncodingRules.BER, "17103137303930383130333530332C303730")]
        [InlineData("A,B2,C2-TooLong", PublicEncodingRules.BER, "17123137303930383130333530332B3037303030")]
        [InlineData("WrongTag", PublicEncodingRules.BER, "1A0D3132303130323233353935395A")]
        public static void ReadUtcTime_Throws(
            string description,
            PublicEncodingRules ruleSet,
            string inputHex)
        {
            byte[] inputData = inputHex.HexToByteArray();

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(inputData, (AsnEncodingRules)ruleSet);
                    reader.GetUtcTime();
                });
        }

        [Fact]
        public static void ReadUtcTime_WayTooBig_Throws()
        {
            // Need to exceed the length that the shared pool will return for 17:
            byte[] inputData = new byte[4097+4];
            inputData[0] = 0x17;
            inputData[1] = 0x82;
            inputData[2] = 0x10;
            inputData[3] = 0x01;

            Assert.Throws<CryptographicException>(
                () =>
                {
                    AsnReader reader = new AsnReader(inputData, AsnEncodingRules.BER);
                    reader.GetUtcTime();
                });
        }
    }
}
