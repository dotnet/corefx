// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class SByteTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var i = new sbyte();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtor_Value()
        {
            sbyte i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0x7F, sbyte.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(-0x80, sbyte.MinValue);
        }

        [Theory]
        [InlineData((sbyte)114, (sbyte)114, 0)]
        [InlineData((sbyte)114, sbyte.MinValue, 1)]
        [InlineData((sbyte)114, (sbyte)-123, 1)]
        [InlineData((sbyte)114, (sbyte)0, 1)]
        [InlineData((sbyte)114, (sbyte)123, -1)]
        [InlineData((sbyte)114, sbyte.MaxValue, -1)]
        [InlineData((sbyte)114, null, 1)]
        public static void TestCompareTo(sbyte i, object value, int expected)
        {
            if (value is sbyte)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((sbyte)value)));
            }

            IComparable comparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (sbyte)114;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a sbyte
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a sbyte
        }

        [Theory]
        [InlineData((sbyte)78, (sbyte)78, true)]
        [InlineData((sbyte)78, (sbyte)-78, false)]
        [InlineData((sbyte)78, (sbyte)0, false)]
        [InlineData((sbyte)0, (sbyte)0, true)]
        [InlineData((sbyte)78, null, false)]
        [InlineData((sbyte)78, "78", false)]
        [InlineData((sbyte)78, 78, false)]
        public static void TestEquals(sbyte i1, object obj, bool expected)
        {
            if (obj is sbyte)
            {
                sbyte i2 = (sbyte)obj;
                Assert.Equal(expected, i1.Equals(i2));
                Assert.Equal(expected, i1.GetHashCode().Equals(i2.GetHashCode()));
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;
            var emptyFormat = new NumberFormatInfo();

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "-123", defaultStyle, nullFormat, (sbyte)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (sbyte)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (sbyte)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (sbyte)123 };
            yield return new object[] { "127", defaultStyle, nullFormat, (sbyte)127 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (sbyte)0x12 };
            yield return new object[] { "10", NumberStyles.AllowThousands, nullFormat, (sbyte)10 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, (sbyte)-123 }; // Parentheses = negative

            yield return new object[] { "123", defaultStyle, emptyFormat, (sbyte)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (sbyte)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (sbyte)0x12 };
            yield return new object[] { "$100", NumberStyles.Currency, customFormat, (sbyte)100 };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, sbyte expected)
        {
            sbyte result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(sbyte.TryParse(value, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, sbyte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, sbyte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(sbyte.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(expected, result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, sbyte.Parse(value, style));
            }
            Assert.Equal(expected, sbyte.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Integer;

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, nullFormat, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { " ", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, nullFormat, typeof(FormatException) };

            yield return new object[] { "ab", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, nullFormat, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 67.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "ab", NumberStyles.None, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "67.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-129", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "128", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
        }

        [Theory, MemberData("Parse_Invalid_TestData")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            sbyte result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(sbyte.TryParse(value, out result));
                Assert.Equal(default(sbyte), result);

                Assert.Throws(exceptionType, () => sbyte.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => sbyte.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(sbyte.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(default(sbyte), result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => sbyte.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => sbyte.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> ToStringTestData()
        {
            var emptyFormat = NumberFormatInfo.CurrentInfo;
            yield return new object[] { sbyte.MinValue, "G", emptyFormat, "-128" };
            yield return new object[] { (sbyte)-123, "G", emptyFormat, "-123" };
            yield return new object[] { (sbyte)0, "G", emptyFormat, "0" };
            yield return new object[] { (sbyte)123, "G", emptyFormat, "123" };
            yield return new object[] { sbyte.MaxValue, "G", emptyFormat, "127" };

            yield return new object[] { (sbyte)0x24, "x", emptyFormat, "24" };
            yield return new object[] { (sbyte)24, "N", emptyFormat, string.Format("{0:N}", 24.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (sbyte)-24, "N", customFormat, "#24~00" };
            yield return new object[] { (sbyte)24, "N", customFormat, "24~00" };
        }

        [Theory, MemberData("ToStringTestData")]
        public static void TestToString(sbyte i, string format, IFormatProvider provider, string expected)
        {
            string upperFormat = format.ToUpperInvariant();
            string lowerFormat = format.ToLowerInvariant();

            string upperExpected = expected.ToUpperInvariant();
            string lowerExpected = expected.ToLowerInvariant();

            bool isDefaultProvider = (provider == null || provider == NumberFormatInfo.CurrentInfo);
            if (string.IsNullOrEmpty(format) || format.ToUpperInvariant() == "G")
            {
                if (isDefaultProvider)
                {
                    Assert.Equal(upperExpected, i.ToString());
                    Assert.Equal(upperExpected, i.ToString((IFormatProvider)null));
                }
                Assert.Equal(upperExpected, i.ToString(provider));
            }
            if (isDefaultProvider)
            {
                Assert.Equal(upperExpected, i.ToString(upperFormat));
                Assert.Equal(lowerExpected, i.ToString(lowerFormat));
                Assert.Equal(upperExpected, i.ToString(upperFormat, null));
                Assert.Equal(lowerExpected, i.ToString(lowerFormat, null));
            }
            Assert.Equal(upperExpected, i.ToString(upperFormat, provider));
            Assert.Equal(lowerExpected, i.ToString(lowerFormat, provider));
        }

        [Fact]
        public static void TestToString_Invalid()
        {
            sbyte i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => i.ToString("Y", null)); // Invalid format
        }
    }
}
