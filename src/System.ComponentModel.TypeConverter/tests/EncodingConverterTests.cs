// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class EncodingConverterTests : ConverterTestBase
    {
        private static readonly EncodingConverter s_converter = new EncodingConverter();

        [Theory]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(char))]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        public static void CanConvertFrom_WithContext(Type sourceType)
        {
            Assert.True(s_converter.CanConvertFrom(TypeConverterTests.s_context, sourceType));
        }

        [Theory]
        [InlineData(1200, "Unicode")]
        [InlineData((short)1200, "Unicode")]
        [InlineData((ushort)1200, "Unicode")]
        [InlineData((ulong)1200, "Unicode")]
        [InlineData((long)1200, "Unicode")]
        [InlineData(28591, "Western European (ISO)")]
        [InlineData(65001, "Unicode (UTF-8)")]
        [InlineData("iso-8859-1", "Western European (ISO)")]
        [InlineData("utf-8", "Unicode (UTF-8)")]
        public static void ConvertFrom_WithContext(object value, string expectedName)
        {
            var encoding = (Encoding)s_converter.ConvertFrom(TypeConverterTests.s_context, null, value);

            Assert.Equal(expectedName, encoding.EncodingName);
        }

        [Theory]
        [InlineData(1200)]
        [InlineData(28591)]
        [InlineData(65001)]
        public static void ConvertToCodePage_WithContext(int expectedCodePage)
        {
            var encoding = Encoding.GetEncoding(expectedCodePage);

            var codePage = s_converter.ConvertTo(TypeConverterTests.s_context, null, encoding, typeof(int));

            Assert.Equal(expectedCodePage, codePage);
        }

        [Theory]
        [InlineData(1200, "utf-16")]
        [InlineData(28591, "iso-8859-1")]
        [InlineData(65001, "utf-8")]
        public static void ConvertToName_WithContext(int codePage, string expectedName)
        {
            var encoding = Encoding.GetEncoding(codePage);

            var name = s_converter.ConvertTo(TypeConverterTests.s_context, null, encoding, typeof(string));

            Assert.Equal(expectedName, name);
        }

        [Fact]
        public static void ConvertToNull_WithContext_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => s_converter.ConvertTo(TypeConverterTests.s_context, null));
        }

        [Theory]
        [InlineData(typeof(Tuple<int>))]
        [InlineData(typeof(Type))]
        [InlineData(typeof(Uri))]
        public static void ConvertToUnsupportedType_WithContext_ThrowsNotSupportedException(Type type)
        {
            Assert.Throws<NotSupportedException>(
                () => s_converter.ConvertTo(TypeConverterTests.s_context, type));
        }

        [Theory]
        [InlineData(1200, "Unicode")]
        [InlineData(65001, "Unicode (UTF-8)")]
        public static void ConvertFromNullable_WithContext(int codePage, string expectedName)
        {
            var encoding = (Encoding)s_converter.ConvertFrom(TypeConverterTests.s_context, null, new Nullable<int>(codePage));

            Assert.Equal(expectedName, encoding.EncodingName);
        }

        [Fact]
        public static void ConvertFromTuple_WithContext_ThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(
                () => s_converter.ConvertFrom(TypeConverterTests.s_context, null, Tuple.Create(1200)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(0.5f)]
        [InlineData(0.5d)]
        [InlineData(float.MaxValue)]
        [InlineData(double.MaxValue)]
        [InlineData(typeof(Type))]
        public static void ConvertFromUnsupportedType_WithContext_ThrowsNotSupportedException(object value)
        {
            Assert.Throws<NotSupportedException>(
                () => s_converter.ConvertFrom(TypeConverterTests.s_context, null, value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(long.MaxValue)]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("1")]
        [InlineData("-1")]
        [InlineData("1200")]
        [InlineData("28591")]
        [InlineData("65001")]
        public static void ConvertFromInvalidEncoding_WithContext_ThrowsFormatException(object encoding)
        {
            Assert.Throws<FormatException>(
                () => s_converter.ConvertFrom(TypeConverterTests.s_context, null, encoding));
        }
    }
}
