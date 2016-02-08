// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class DoubleTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var i = new double();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void TestCtor_Value()
        {
            double d = 41;
            Assert.Equal(41, d);

            d = 41.3;
            Assert.Equal(41.3, d);
        }
        
        [Fact]
        public static void TestEpsilon()
        {
            Assert.Equal(4.9406564584124654E-324, double.Epsilon);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal(1.7976931348623157E+308, double.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal(-1.7976931348623157E+308, double.MinValue);
        }

        [Fact]
        public static void TestNaN()
        {
            Assert.Equal(0.0 / 0.0, double.NaN);
        }

        [Fact]
        public static void TestNegativeInfinity()
        {
            Assert.Equal(-1.0 / 0.0, double.NegativeInfinity);
        }

        [Fact]
        public static void TestPositiveInfinity()
        {
            Assert.Equal(1.0 / 0.0, double.PositiveInfinity);
        }

        [Theory]
        [InlineData((double)234, (double)234, 0)]
        [InlineData((double)234, double.MinValue, 1)]
        [InlineData((double)234, (double)-123, 1)]
        [InlineData((double)234, (double)0, 1)]
        [InlineData((double)234, (double)123, 1)]
        [InlineData((double)234, (double)456, -1)]
        [InlineData((double)234, double.MaxValue, -1)]
        [InlineData((double)234, double.NaN, 1)]
        [InlineData(double.NaN, double.NaN, 0)]
        [InlineData(double.NaN, (double)0, -1)]
        [InlineData((double)234, null, 1)]
        public static void TestCompareTo(double d1, object value, int expected)
        {
            if (value is double)
            {
                double d2 = (double)value;
                Assert.Equal(expected, CompareHelper.NormalizeCompare(d1.CompareTo(d2)));
                if (double.IsNaN(d1) || double.IsNaN(d2))
                {
                    Assert.False(d1 >= d2);
                    Assert.False(d1 > d2);
                    Assert.False(d1 <= d2);
                    Assert.False(d1 < d2);
                }
                else
                {
                    if (expected >= 0)
                    {
                        Assert.True(d1 >= d2);
                        Assert.False(d1 < d2);
                    }
                    if (expected > 0)
                    {
                        Assert.True(d1 > d2);
                        Assert.False(d1 <= d2);
                    }
                    if (expected <= 0)
                    {
                        Assert.True(d1 <= d2);
                        Assert.False(d1 > d2);
                    }
                    if (expected < 0)
                    {
                        Assert.True(d1 < d2);
                        Assert.False(d1 >= d2);
                    }
                }
            }
            IComparable comparable = d1;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (double)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo((float)234)); // Obj is not a double
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("234")); // Obj is not a double
        }

        [Theory]
        [InlineData((double)789, (double)789, true)]
        [InlineData((double)789, (double)-789, false)]
        [InlineData((double)789, (double)0, false)]
        [InlineData(double.NaN, double.NaN, true)]
        [InlineData((double)789, (float)789, false)]
        [InlineData((double)789, "789", false)]
        public static void TestEquals(double d1, object value, bool expected)
        {
            if (value is double)
            {
                double d2 = (double)value;
                Assert.Equal(expected, d1.Equals(d2));

                if (double.IsNaN(d1) && double.IsNaN(d2))
                {
                    Assert.Equal(!expected, d1 == d2);
                    Assert.Equal(expected, d1 != d2);
                }
                else
                {
                    Assert.Equal(expected, d1 == d2);
                    Assert.Equal(!expected, d1 != d2);
                }

                Assert.NotEqual(0, d1.GetHashCode());
                Assert.Equal(expected, d1.GetHashCode().Equals(d2.GetHashCode()));
            }
            Assert.Equal(expected, d1.Equals(value));
        }

        [Theory]
        [InlineData(double.PositiveInfinity, true)]
        [InlineData(double.NegativeInfinity, true)]
        [InlineData(double.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsInfinity(double d, bool expected)
        {
            Assert.Equal(expected, double.IsInfinity(d));
        }
        
        [Theory]
        [InlineData(double.PositiveInfinity, false)]
        [InlineData(double.NegativeInfinity, false)]
        [InlineData(double.NaN, true)]
        [InlineData(0.0, false)]
        public static void TestIsNaN(double d, bool expected)
        {
            Assert.Equal(expected, double.IsNaN(d));
        }
                
        [Theory]
        [InlineData(double.NegativeInfinity, true)]
        [InlineData(double.PositiveInfinity, false)]
        [InlineData(double.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsNegativeInfinity(double d, bool expected)
        {
            Assert.Equal(expected, double.IsNegativeInfinity(d));
        }

        [Theory]
        [InlineData(double.PositiveInfinity, true)]
        [InlineData(double.NegativeInfinity, false)]
        [InlineData(double.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsPositiveInfinity(double d, bool expected)
        {
            Assert.Equal(expected, double.IsPositiveInfinity(d));
        }
        
        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            // Defaults: AllowLeadingWhite | AllowTrailingWhite | AllowLeadingSign | AllowDecimalPoint | AllowExponent | AllowThousands
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var emptyFormat = new NumberFormatInfo();

            var customFormat1 = new NumberFormatInfo();
            customFormat1.CurrencySymbol = "$";
            customFormat1.CurrencyGroupSeparator = ",";

            var customFormat2 = new NumberFormatInfo();
            customFormat2.NumberDecimalSeparator = ".";

            NumberFormatInfo invariantFormat = NumberFormatInfo.InvariantInfo;

            yield return new object[] { "-123", defaultStyle, nullFormat, (double)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (double)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (double)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (double)123 };
            yield return new object[] { "567.89", defaultStyle, nullFormat, 567.89 };
            yield return new object[] { "-567.89", defaultStyle, nullFormat, -567.89 };
            yield return new object[] { "1E23", defaultStyle, nullFormat, 1E23 };

            yield return new object[] { "123.1", NumberStyles.AllowDecimalPoint, nullFormat, 123.1 };
            yield return new object[] { 1000.ToString("N0"), NumberStyles.AllowThousands, nullFormat, (double)1000 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (double)123 };
            yield return new object[] { "123.567", NumberStyles.Any, emptyFormat, 123.567 };
            yield return new object[] { "123", NumberStyles.Float, emptyFormat, (double)123 };
            yield return new object[] { "$1000", NumberStyles.Currency, customFormat1, (double)1000 };
            yield return new object[] { "123.123", NumberStyles.Float, customFormat2, 123.123 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, customFormat2, -123 };

            yield return new object[] { "NaN", NumberStyles.Any, invariantFormat, double.NaN };
            yield return new object[] { "Infinity", NumberStyles.Any, invariantFormat, double.PositiveInfinity };
            yield return new object[] { "-Infinity", NumberStyles.Any, invariantFormat, double.NegativeInfinity };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, double expected)
        {
            double d;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.True(double.TryParse(value, out d));
                Assert.Equal(expected, d);

                Assert.Equal(expected, double.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, double.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(double.TryParse(value, style, provider ?? new NumberFormatInfo(), out d));
            Assert.Equal(expected, d);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, double.Parse(value, style));
            }
            Assert.Equal(expected, double.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberFormatInfo nullFormat = null;
            NumberStyles defaultStyle = NumberStyles.Float;

            var customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, nullFormat, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { " ", defaultStyle, nullFormat, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, nullFormat, typeof(FormatException) };

            yield return new object[] { "ab", defaultStyle, nullFormat, typeof(FormatException) }; // Hex value
            yield return new object[] { "(123)", defaultStyle, nullFormat, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, nullFormat, typeof(FormatException) }; // Currency

            yield return new object[] { "123.456", NumberStyles.Integer, nullFormat, typeof(FormatException) }; // Decimal
            yield return new object[] { "  123.456", NumberStyles.None, nullFormat, typeof(FormatException) }; // Leading space
            yield return new object[] { "123.456   ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Leading space
            yield return new object[] { "1E23", NumberStyles.None, nullFormat, typeof(FormatException) }; // Exponent

            yield return new object[] { "ab", NumberStyles.None, nullFormat, typeof(FormatException) }; // Negative hex value
            yield return new object[] { "  123  ", NumberStyles.None, nullFormat, typeof(FormatException) }; // Trailing and leading whitespace
        }

        [Theory, MemberData("Parse_Invalid_TestData")]
        public static void TestParse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            double d;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.False(double.TryParse(value, out d));
                Assert.Equal(default(double), d);

                Assert.Throws(exceptionType, () => double.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => double.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(double.TryParse(value, style, provider ?? new NumberFormatInfo(), out d));
            Assert.Equal(default(double), d);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => double.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => double.Parse(value, style, provider ?? new NumberFormatInfo()));
        }
        
        public static IEnumerable<object[]> ToStringTestData()
        {
            var emptyFormat = NumberFormatInfo.CurrentInfo;
            yield return new object[] { double.MinValue, "G", emptyFormat, "-1.79769313486232E+308" };
            yield return new object[] { (double)-4567, "G", emptyFormat, "-4567" };
            yield return new object[] { -4567.89101, "G", emptyFormat, "-4567.89101" };
            yield return new object[] { (double)0, "G", emptyFormat, "0" };
            yield return new object[] { (double)4567, "G", emptyFormat, "4567" };
            yield return new object[] { 4567.89101, "G", emptyFormat, "4567.89101" };
            yield return new object[] { double.MaxValue, "G", emptyFormat, "1.79769313486232E+308" };

            yield return new object[] { double.Epsilon, "G", emptyFormat, "4.94065645841247E-324" };
            yield return new object[] { double.NaN, "G", emptyFormat, "NaN" };
            
            yield return new object[] { (double)2468, "N", emptyFormat, string.Format("{0:N}", 2468.00) };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customFormat1 = new NumberFormatInfo();
            customFormat1.NumberNegativePattern = 0;
            yield return new object[] { (double)-6310, "G", customFormat1, "-6310" };

            var customFormat2 = new NumberFormatInfo();
            customFormat2.NegativeSign = "#";
            customFormat2.NumberDecimalSeparator = "~";
            customFormat2.NumberGroupSeparator = "*";
            yield return new object[] { (double)-2468, "N", customFormat2, "#2*468~00" };
            yield return new object[] { (double)2468, "N", customFormat2, "2*468~00" };

            var customFormat3 = new NumberFormatInfo();
            customFormat3.NegativeSign = "xx"; // Set to trash to make sure it doesn't show up
            customFormat3.NumberGroupSeparator = "*";
            customFormat3.NumberNegativePattern = 0;
            yield return new object[] { (double)-2468, "N", customFormat3, "(2*468.00)" };

            var invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { double.Epsilon, "G", invariantFormat, "4.94065645841247E-324" };
            yield return new object[] { double.NaN, "G", invariantFormat, "NaN" };
            yield return new object[] { double.PositiveInfinity, "G", invariantFormat, "Infinity" };
            yield return new object[] { double.NegativeInfinity, "G", invariantFormat, "-Infinity" };
        }

        [Theory, MemberData("ToStringTestData")]
        public static void TestToString(double d, string format, IFormatProvider provider, string expected)
        {
            bool isDefaultProvider = (provider == null || provider == NumberFormatInfo.CurrentInfo);
            if (string.IsNullOrEmpty(format) || format.ToUpperInvariant() == "G")
            {
                if (isDefaultProvider)
                {
                    Assert.Equal(expected, d.ToString());
                    Assert.Equal(expected, d.ToString((IFormatProvider)null));
                }
                Assert.Equal(expected, d.ToString(provider));
            }
            if (isDefaultProvider)
            {
                Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant())); // If format is upper case, then exponents are printed in upper case
                Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant())); // If format is lower case, then exponents are printed in upper case
                Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant(), null));
                Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant(), null));
            }
            Assert.Equal(expected.Replace('e', 'E'), d.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected.Replace('E', 'e'), d.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Infinity()
        {
            string positiveInfinityString = double.PositiveInfinity.ToString();
            string negativeInfinityString = double.NegativeInfinity.ToString();

            Assert.True(positiveInfinityString == "Infinity" || positiveInfinityString == "∞");
            Assert.True(negativeInfinityString == "-Infinity" || negativeInfinityString == "-∞");
        }

        [Fact]
        public static void TestToString_Invalid()
        {
            double d = 123;
            Assert.Throws<FormatException>(() => d.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => d.ToString("Y", null)); // Invalid format
        }
    }
}
