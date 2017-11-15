// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using Test.Cryptography;
using Xunit;

namespace System.Security.Cryptography.Tests.Asn1
{
    public class WriteNamedBitList : Asn1WriterTests
    {
        [Theory]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "030100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "030100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "030100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "030100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "030100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "030100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "030100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "030100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "030100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "0309000000000000000003",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "03090600000000000000C0",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "030206C0",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "0309060000000001000040",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "03050040000001",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "0309010000000080000002",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "030204B0",
            ReadNamedBitList.X509KeyUsageDescendingCompact.DigitalSignature |
                ReadNamedBitList.X509KeyUsageDescendingCompact.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageDescendingCompact.DataEncipherment)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "030204B0",
            ReadNamedBitList.X509KeyUsageCSharpStyle.DigitalSignature |
                ReadNamedBitList.X509KeyUsageCSharpStyle.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageCSharpStyle.DataEncipherment)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "030204B0",
            ReadNamedBitList.X509KeyUsageWin32.DigitalSignature |
                ReadNamedBitList.X509KeyUsageWin32.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageWin32.DataEncipherment)]
        public static void VerifyWriteNamedBitList(
            PublicEncodingRules ruleSet,
            PublicNamedBitListMode mode,
            string expectedHex,
            object value)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);
            writer.WriteNamedBitList(value, (NamedBitListMode)mode);

            Verify(writer, expectedHex);
        }

        [Theory]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "800100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "820100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "840100",
            ReadNamedBitList.ULongFlags.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "410100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "C30100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "820100",
            ReadNamedBitList.LongFlags.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "820100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "410100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "C30100",
            ReadNamedBitList.X509KeyUsageCSharpStyle.None)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "8009000000000000000003",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "82090600000000000000C0",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "840206C0",
            ReadNamedBitList.ULongFlags.Max | ReadNamedBitList.ULongFlags.AlmostMax)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "4109060000000001000040",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "C3050040000001",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "8209010000000080000002",
            ReadNamedBitList.LongFlags.Max | ReadNamedBitList.LongFlags.Mid)]
        [InlineData(
            PublicEncodingRules.BER,
            PublicNamedBitListMode.NamedZeroIs128BigEndian,
            "820204B0",
            ReadNamedBitList.X509KeyUsageDescendingCompact.DigitalSignature |
                ReadNamedBitList.X509KeyUsageDescendingCompact.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageDescendingCompact.DataEncipherment)]
        [InlineData(
            PublicEncodingRules.CER,
            PublicNamedBitListMode.NamedZeroIsOne,
            "410204B0",
            ReadNamedBitList.X509KeyUsageCSharpStyle.DigitalSignature |
                ReadNamedBitList.X509KeyUsageCSharpStyle.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageCSharpStyle.DataEncipherment)]
        [InlineData(
            PublicEncodingRules.DER,
            PublicNamedBitListMode.NamedZeroIs128LittleEndian,
            "C30204B0",
            ReadNamedBitList.X509KeyUsageWin32.DigitalSignature |
                ReadNamedBitList.X509KeyUsageWin32.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageWin32.DataEncipherment)]
        public static void VerifyWriteNamedBitList_WithTag(
            PublicEncodingRules ruleSet,
            PublicNamedBitListMode mode,
            string expectedHex,
            object value)
        {
            int ruleSetVal = (int)ruleSet;
            int modeVal = (int)mode;
            TagClass tagClass = (TagClass)(byte)((ruleSetVal ^ modeVal) << 6);

            if (tagClass == TagClass.Universal)
                tagClass = TagClass.ContextSpecific;

            Asn1Tag tag = new Asn1Tag(tagClass, ruleSetVal + modeVal);

            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);
            writer.WriteNamedBitList(tag, value, (NamedBitListMode)mode);

            Verify(writer, expectedHex);
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_Generic(PublicEncodingRules ruleSet)
        {
            AsnWriter objWriter = new AsnWriter((AsnEncodingRules)ruleSet);
            AsnWriter genWriter = new AsnWriter((AsnEncodingRules)ruleSet);

            var win32Bits =
                ReadNamedBitList.X509KeyUsageWin32.DigitalSignature |
                ReadNamedBitList.X509KeyUsageWin32.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageWin32.DataEncipherment;

            genWriter.WriteNamedBitList(win32Bits, NamedBitListMode.NamedZeroIs128LittleEndian);
            objWriter.WriteNamedBitList((object)win32Bits, NamedBitListMode.NamedZeroIs128LittleEndian);

            var bigEndianBits =
                ReadNamedBitList.X509KeyUsageDescendingCompact.DigitalSignature |
                ReadNamedBitList.X509KeyUsageDescendingCompact.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageDescendingCompact.DataEncipherment;

            genWriter.WriteNamedBitList(bigEndianBits, NamedBitListMode.NamedZeroIs128BigEndian);
            objWriter.WriteNamedBitList((object)bigEndianBits, NamedBitListMode.NamedZeroIs128BigEndian);

            var csharpBits =
                ReadNamedBitList.X509KeyUsageCSharpStyle.DigitalSignature |
                ReadNamedBitList.X509KeyUsageCSharpStyle.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageCSharpStyle.DataEncipherment;

            genWriter.WriteNamedBitList(csharpBits, NamedBitListMode.NamedZeroIsOne);
            objWriter.WriteNamedBitList((object)csharpBits, NamedBitListMode.NamedZeroIsOne);

            Verify(genWriter, objWriter.Encode().ByteArrayToHex());
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_Generic_WithTag(PublicEncodingRules ruleSet)
        {
            AsnWriter objWriter = new AsnWriter((AsnEncodingRules)ruleSet);
            AsnWriter genWriter = new AsnWriter((AsnEncodingRules)ruleSet);

            Asn1Tag tag = new Asn1Tag(TagClass.ContextSpecific, 52);
            var win32Bits =
                ReadNamedBitList.X509KeyUsageWin32.DigitalSignature |
                ReadNamedBitList.X509KeyUsageWin32.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageWin32.DataEncipherment;

            genWriter.WriteNamedBitList(tag, win32Bits, NamedBitListMode.NamedZeroIs128LittleEndian);
            objWriter.WriteNamedBitList(tag, (object)win32Bits, NamedBitListMode.NamedZeroIs128LittleEndian);

            tag = new Asn1Tag(TagClass.Private, 4);
            var bigEndianBits =
                ReadNamedBitList.X509KeyUsageDescendingCompact.DigitalSignature |
                ReadNamedBitList.X509KeyUsageDescendingCompact.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageDescendingCompact.DataEncipherment;

            genWriter.WriteNamedBitList(tag, bigEndianBits, NamedBitListMode.NamedZeroIs128BigEndian);
            objWriter.WriteNamedBitList(tag, (object)bigEndianBits, NamedBitListMode.NamedZeroIs128BigEndian);

            tag = new Asn1Tag(TagClass.Application, 75);
            var csharpBits =
                ReadNamedBitList.X509KeyUsageCSharpStyle.DigitalSignature |
                ReadNamedBitList.X509KeyUsageCSharpStyle.KeyEncipherment |
                ReadNamedBitList.X509KeyUsageCSharpStyle.DataEncipherment;

            genWriter.WriteNamedBitList(tag, csharpBits, NamedBitListMode.NamedZeroIsOne);
            objWriter.WriteNamedBitList(tag, (object)csharpBits, NamedBitListMode.NamedZeroIsOne);

            Verify(genWriter, objWriter.Encode().ByteArrayToHex());
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_NonNull(PublicEncodingRules ruleSet)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentNullException>(
                "enumValue",
                () => writer.WriteNamedBitList(null, NamedBitListMode.NamedZeroIsOne));

            AssertExtensions.Throws<ArgumentNullException>(
                "enumValue",
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    null,
                    NamedBitListMode.NamedZeroIsOne));
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_EnumRequired(PublicEncodingRules ruleSet)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);

            Assert.Throws<ArgumentException>(
                () => writer.WriteNamedBitList(3, NamedBitListMode.NamedZeroIsOne));

            Assert.Throws<ArgumentException>(
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    3,
                    NamedBitListMode.NamedZeroIsOne));

            Assert.Throws<ArgumentException>(
                () => writer.WriteNamedBitList((object)3, NamedBitListMode.NamedZeroIsOne));

            Assert.Throws<ArgumentException>(
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    (object)3,
                    NamedBitListMode.NamedZeroIsOne));
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_FlagsEnumRequired(PublicEncodingRules ruleSet)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentException>(
                "tEnum",
                () => writer.WriteNamedBitList(
                    NamedBitListMode.NamedZeroIs128BigEndian,
                    NamedBitListMode.NamedZeroIsOne));

            AssertExtensions.Throws<ArgumentException>(
                "tEnum",
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    NamedBitListMode.NamedZeroIs128BigEndian,
                    NamedBitListMode.NamedZeroIsOne));

            AssertExtensions.Throws<ArgumentException>(
                "tEnum",
                () => writer.WriteNamedBitList(
                    (object)NamedBitListMode.NamedZeroIs128BigEndian,
                    NamedBitListMode.NamedZeroIsOne));

            AssertExtensions.Throws<ArgumentException>(
                "tEnum",
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    (object)NamedBitListMode.NamedZeroIs128BigEndian,
                    NamedBitListMode.NamedZeroIsOne));
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_EndOfContents(PublicEncodingRules ruleSet)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentException>(
                "tag",
                () => writer.WriteNamedBitList(
                    Asn1Tag.EndOfContents,
                    StringSplitOptions.RemoveEmptyEntries,
                    NamedBitListMode.NamedZeroIsOne));

            AssertExtensions.Throws<ArgumentException>(
                "tag",
                () => writer.WriteNamedBitList(
                    Asn1Tag.EndOfContents,
                    (object)StringSplitOptions.RemoveEmptyEntries,
                    NamedBitListMode.NamedZeroIsOne));
        }

        [Theory]
        [InlineData(PublicEncodingRules.BER)]
        [InlineData(PublicEncodingRules.CER)]
        [InlineData(PublicEncodingRules.DER)]
        public static void VerifyWriteNamedBitList_LegalModeRequired(PublicEncodingRules ruleSet)
        {
            AsnWriter writer = new AsnWriter((AsnEncodingRules)ruleSet);

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "mode",
                () => writer.WriteNamedBitList(
                    (object)StringSplitOptions.RemoveEmptyEntries,
                    (NamedBitListMode)5));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "mode",
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    (object)StringSplitOptions.RemoveEmptyEntries,
                    (NamedBitListMode)6));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "mode",
                () => writer.WriteNamedBitList(
                    StringSplitOptions.RemoveEmptyEntries,
                    (NamedBitListMode)7));

            AssertExtensions.Throws<ArgumentOutOfRangeException>(
                "mode",
                () => writer.WriteNamedBitList(
                    new Asn1Tag(TagClass.ContextSpecific, 1),
                    StringSplitOptions.RemoveEmptyEntries,
                    (NamedBitListMode)8));
        }
    }
}
