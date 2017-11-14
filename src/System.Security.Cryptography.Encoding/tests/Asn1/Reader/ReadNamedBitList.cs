// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public sealed class ReadNamedBitList : Asn1ReaderTests
    {
        [Flags]
        public enum X509KeyUsageCSharpStyle
        {
            None = 0,
            DigitalSignature = 1,
            NonRepudiation = 1 << 1,
            KeyEncipherment = 1 << 2,
            DataEncipherment = 1 << 3,
            KeyAgreement = 1 << 4,
            KeyCertSign = 1 << 5,
            CrlSign = 1 << 6,
            EncipherOnly = 1 << 7,
            DecipherOnly = 1 << 8,
        }

        [Flags]
        public enum X509KeyUsageWin32
        {
            None = 0x0000,
            DigitalSignature = 0x0080,
            NonRepudiation = 0x0040,
            KeyEncipherment = 0x0020,
            DataEncipherment = 0x0010,
            KeyAgreement = 0x0008,
            KeyCertSign = 0x0004,
            CrlSign = 0x0002,
            EncipherOnly = 0x0001,
            DecipherOnly = 0x8000,
        }

        [Flags]
        public enum X509KeyUsageDescendingCompact : ushort
        {
            None = 0x0000,
            DigitalSignature = 0x8000,
            NonRepudiation = 0x4000,
            KeyEncipherment = 0x2000,
            DataEncipherment = 0x1000,
            KeyAgreement = 0x0800,
            KeyCertSign = 0x0400,
            CrlSign = 0x0200,
            EncipherOnly = 0x0100,
            DecipherOnly = 0x0080,
        }

        [Flags]
        public enum X509KeyUsageDescending
        {
            None = 0x000000000,
            DigitalSignature = unchecked((int)0x80000000),
            NonRepudiation = 0x40000000,
            KeyEncipherment = 0x20000000,
            DataEncipherment = 0x10000000,
            KeyAgreement = 0x08000000,
            KeyCertSign = 0x04000000,
            CrlSign = 0x02000000,
            EncipherOnly = 0x01000000,
            DecipherOnly = 0x00800000,
        }

        [Flags]
        public enum ULongFlags : ulong
        {
            None = 0,
            Min = 1,
            Mid = 1L << 32,
            AlmostMax = 1L << 62,
            Max = 1UL << 63,
        }

        [Flags]
        public enum LongFlags : long
        {
            None = 0,
            Mid = 1L << 32,
            Max = 1L << 62,
            Min = long.MinValue,
        }

        [Theory]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.None,
            "030100")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.DecipherOnly | X509KeyUsageCSharpStyle.KeyCertSign,
            "0303070480")]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.KeyAgreement,
            "03020008")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            typeof(X509KeyUsageWin32),
            X509KeyUsageWin32.None,
            "030100")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            typeof(X509KeyUsageWin32),
            X509KeyUsageWin32.DecipherOnly | X509KeyUsageWin32.DataEncipherment,
            "0303071080")]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            typeof(X509KeyUsageWin32),
            X509KeyUsageWin32.CrlSign | X509KeyUsageWin32.KeyCertSign,
            "03020106")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(X509KeyUsageDescendingCompact),
            X509KeyUsageDescendingCompact.None,
            "030100")]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(X509KeyUsageDescendingCompact),
            X509KeyUsageDescendingCompact.EncipherOnly | X509KeyUsageDescendingCompact.DecipherOnly,
            "0303070180")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(X509KeyUsageDescendingCompact),
            X509KeyUsageDescendingCompact.KeyAgreement | X509KeyUsageDescendingCompact.KeyCertSign,
            "0302020C")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(X509KeyUsageDescending),
            X509KeyUsageDescending.EncipherOnly | X509KeyUsageDescending.DecipherOnly,
            "0303070180")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(X509KeyUsageDescendingCompact),
            X509KeyUsageDescendingCompact.KeyAgreement | X509KeyUsageDescendingCompact.KeyCertSign,
            "0302020C")]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(LongFlags),
            LongFlags.Mid | LongFlags.Max,
            "0309010000000080000002")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(LongFlags),
            LongFlags.Mid | LongFlags.Min,
            "0309000000000080000001")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(LongFlags),
            LongFlags.Min | LongFlags.Max,
            "0309000000000000000003")]
        // BER: Unused bits are unmapped, regardless of value.
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.DecipherOnly | X509KeyUsageCSharpStyle.KeyCertSign,
            "030307048F")]
        // BER: Trailing zeros are permitted.
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.DecipherOnly | X509KeyUsageCSharpStyle.KeyCertSign | X509KeyUsageCSharpStyle.DataEncipherment,
            "03050014800000")]
        // BER: Trailing 0-bits don't have to be declared "unused"
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(X509KeyUsageCSharpStyle),
            X509KeyUsageCSharpStyle.DecipherOnly | X509KeyUsageCSharpStyle.KeyCertSign | X509KeyUsageCSharpStyle.DataEncipherment,
            "0303001480")]
        public static void VerifyReadNamedBitListEncodings(
            PublicEncodingRules ruleSet,
            PublicNamedBitListMode mode,
            Type enumType,
            long enumValue,
            string inputHex)
        {
            byte[] inputBytes = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputBytes, (AsnEncodingRules)ruleSet);
            Enum readValue = reader.GetNamedBitListValue(enumType, (NamedBitListMode)mode);

            Assert.Equal(Enum.ToObject(enumType, enumValue), readValue);
        }

        [Theory]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(ULongFlags),
            ULongFlags.Mid | ULongFlags.Max,
            "0309000000000080000001")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            typeof(ULongFlags),
            ULongFlags.Min | ULongFlags.Mid,
            "0306078000000080")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            typeof(ULongFlags),
            ULongFlags.Mid | ULongFlags.Max,
            "0309070000000001000080")]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            typeof(ULongFlags),
            ULongFlags.Min | ULongFlags.Mid,
            "0306000100000001")]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(ULongFlags),
            ULongFlags.Mid | ULongFlags.Max,
            "03050080000001")]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            typeof(ULongFlags),
            ULongFlags.Min | ULongFlags.Mid,
            "0309000000000100000001")]
        public static void VerifyReadNamedBitListEncodings_ULong(
            PublicEncodingRules ruleSet,
            PublicNamedBitListMode mode,
            Type enumType,
            ulong enumValue,
            string inputHex)
        {
            byte[] inputBytes = inputHex.HexToByteArray();

            AsnReader reader = new AsnReader(inputBytes, (AsnEncodingRules)ruleSet);
            Enum readValue = reader.GetNamedBitListValue(enumType, (NamedBitListMode)mode);

            Assert.Equal(Enum.ToObject(enumType, enumValue), readValue);
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyGenericReadNamedBitList(PublicEncodingRules ruleSet)
        {
            string inputHex = "0309000000000100000001" + "0309010000000080000002";
            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            ULongFlags uLongFlags = reader.GetNamedBitListValue<ULongFlags>(NamedBitListMode.NamedZeroIs128BigEndian);
            LongFlags longFlags = reader.GetNamedBitListValue<LongFlags>(NamedBitListMode.NamedZeroIsOne);

            //Assert.False(reader.HasData);
            Assert.Equal(ULongFlags.Mid | ULongFlags.Min, uLongFlags);
            Assert.Equal(LongFlags.Mid | LongFlags.Max, longFlags);
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void ReadNamedBitList_RequiresFlags(PublicEncodingRules ruleSet)
        {
            string inputHex = "030100";
            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentException>(
                "tFlagsEnum",
                () => reader.GetNamedBitListValue<NamedBitListMode>(NamedBitListMode.NamedZeroIsOne));

            Assert.True(reader.HasData, "reader.HasData");
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void ReadNamedBitList_DataOutOfRange(PublicEncodingRules ruleSet)
        {
            string inputHex = "0309000000000100000001";

            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            Assert.Throws<Exception>(
                () => reader.GetNamedBitListValue<X509KeyUsageCSharpStyle>(NamedBitListMode.NamedZeroIsOne));

            Assert.True(reader.HasData, "reader.HasData");
        }

        [Theory]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void ReadNamedBitList_ExcessiveBytes(PublicEncodingRules ruleSet)
        {
            string inputHex = "03050014800000";

            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () => reader.GetNamedBitListValue<X509KeyUsageCSharpStyle>(NamedBitListMode.NamedZeroIsOne));

            Assert.True(reader.HasData, "reader.HasData");
        }

        [Theory]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void ReadNamedBitList_ExcessiveBits(PublicEncodingRules ruleSet)
        {
            string inputHex = "0303061480";

            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            Assert.Throws<CryptographicException>(
                () => reader.GetNamedBitListValue<X509KeyUsageCSharpStyle>(NamedBitListMode.NamedZeroIsOne));

            Assert.True(reader.HasData, "reader.HasData");
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void ReadNamedBitList_RequiresValidMode(PublicEncodingRules ruleSet)
        {
            string inputHex = "0303071480";
            AsnReader reader = new AsnReader(inputHex.HexToByteArray(), (AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "mode",
                () => reader.GetNamedBitListValue<X509KeyUsageCSharpStyle>((NamedBitListMode)5));
        }
    }
}
