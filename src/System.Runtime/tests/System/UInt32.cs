// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class UInt32Tests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var i = new uint();
            Assert.Equal((uint)0, i);
        }

        [Fact]
        public static void TestCtor_Value()
        {
            uint i = 41;
            Assert.Equal((uint)41, i);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(0xFFFFFFFF, uint.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal((uint)0, uint.MinValue);
        }
        
        [Theory]
        [InlineData((uint)234, (uint)234, 0)]
        [InlineData((uint)234, uint.MinValue, 1)]
        [InlineData((uint)234, (uint)0, 1)]
        [InlineData((uint)234, (uint)123, 1)]
        [InlineData((uint)234, (uint)456, -1)]
        [InlineData((uint)234, uint.MaxValue, -1)]
        [InlineData((uint)234, null, 1)]
        public static void TestCompareTo(uint i, object value, int expected)
        {
            if (value is uint)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(i.CompareTo((uint)value)));
            }
            IComparable comparable = i;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (uint)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a uint
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a uint
        }
        
        [Theory]
        [InlineData((uint)789, (uint)789, true)]
        [InlineData((uint)788, (uint)0, false)]
        [InlineData((uint)0, (uint)0, true)]
        [InlineData((uint)789, null, false)]
        [InlineData((uint)789, "789", false)]
        [InlineData((uint)789, 789, false)]
        public static void TestEquals(uint i1, object obj, bool expected)
        {
            if (obj is uint)
            {
                uint i2 = (uint)obj;
                Assert.Equal(expected, i1.Equals(i2));
                Assert.Equal(expected, i1.GetHashCode().Equals(i2.GetHashCode()));
                Assert.Equal((int)i1, i1.GetHashCode());
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

            yield return new object[] { "0", defaultStyle, nullFormat, (uint)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (uint)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (uint)123 };
            yield return new object[] { "4294967295", defaultStyle, nullFormat, 4294967295 };

            yield return new object[] { "12", NumberStyles.HexNumber, nullFormat, (uint)0x12 };
            yield return new object[] { "1000", NumberStyles.AllowThousands, nullFormat, (uint)1000 };

            yield return new object[] { "123", defaultStyle, emptyFormat, (uint)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (uint)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (uint)0x12 };
            yield return new object[] { "abc", NumberStyles.HexNumber, emptyFormat, (uint)0xabc };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, (uint)1000 };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, uint expected)
        {
            uint result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(uint.TryParse(value, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, uint.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, uint.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(uint.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(expected, result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, uint.Parse(value, style));
            }
            Assert.Equal(expected, uint.Parse(value, style, provider ?? new NumberFormatInfo()));
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

            yield return new object[] { "abc", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, nullFormat, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, nullFormat, typeof(FormatException) }; // Thousands
            yield return new object[] { 678.90.ToString("F2"), defaultStyle, nullFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "abc", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "678.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-1", defaultStyle, nullFormat, typeof(OverflowException) }; // < min value
            yield return new object[] { "4294967296", defaultStyle, nullFormat, typeof(OverflowException) }; // > max value
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, nullFormat, typeof(OverflowException) }; // Parentheses = negative
        }

        [Theory, MemberData("Parse_Invalid_TestData")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            uint result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(uint.TryParse(value, out result));
                Assert.Equal(default(uint), result);

                Assert.Throws(exceptionType, () => uint.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => uint.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(uint.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(default(uint), result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => uint.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => uint.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
        
        public static IEnumerable<object[]> ToStringTestData()
        {
            var emptyFormat = NumberFormatInfo.CurrentInfo;
            yield return new object[] { (uint)0, "G", emptyFormat, "0" };
            yield return new object[] { (uint)4567, "G", emptyFormat, "4567" };
            yield return new object[] { uint.MaxValue, "G", emptyFormat, "4294967295" };

            yield return new object[] { (uint)0x2468, "x", emptyFormat, "2468" };
            yield return new object[] { (uint)2468, "N", emptyFormat, string.Format("{0:N}", 2468.00) };

            var customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (uint)2468, "N", customFormat, "2*468~00" };
        }

        [Theory, MemberData("ToStringTestData")]
        public static void TestToString(uint i, string format, IFormatProvider provider, string expected)
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
            uint i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => i.ToString("Y", null)); // Invalid format
        }
    }
}
