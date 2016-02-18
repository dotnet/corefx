// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class SingleTests
    {
        [Fact]
        public static void TestCtor_Empty()
        {
            var f = new float();
            Assert.Equal(0, f);
        }

        [Fact]
        public static void TestCtor_Value()
        {
            float f = 41;
            Assert.Equal(41, f);

            f = 41.3f;
            Assert.Equal(41.3f, f);
        }

        [Fact]
        public static void TestEpsilon()
        {
            Assert.Equal((float)1.4e-45, float.Epsilon);
        }

        [Fact]
        public static void TestMaxValue()
        {
            Assert.Equal((float)3.40282346638528859e+38, float.MaxValue);
        }

        [Fact]
        public static void TestMinValue()
        {
            Assert.Equal((float)-3.40282346638528859e+38, float.MinValue);
        }

        [Fact]
        public static void TestNaN()
        {
            Assert.Equal((float)0.0 / (float)0.0, float.NaN);
        }

        [Fact]
        public static void TestNegativeInfinity()
        {
            Assert.Equal((float)-1.0 / (float)0.0, float.NegativeInfinity);
        }

        [Fact]
        public static void TestPositiveInfinity()
        {
            Assert.Equal((float)1.0 / (float)0.0, float.PositiveInfinity);
        }

        [Theory]
        [InlineData((float)234, (float)234, 0)]
        [InlineData((float)234, float.MinValue, 1)]
        [InlineData((float)234, (float)-123, 1)]
        [InlineData((float)234, (float)0, 1)]
        [InlineData((float)234, (float)123, 1)]
        [InlineData((float)234, (float)456, -1)]
        [InlineData((float)234, float.MaxValue, -1)]
        [InlineData((float)234, float.NaN, 1)]
        [InlineData(float.NaN, float.NaN, 0)]
        [InlineData(float.NaN, (float)0, -1)]
        [InlineData((float)234, null, 1)]
        public static void TestCompareTo(float f1, object value, int expected)
        {
            if (value is float)
            {
                float f2 = (float)value;
                Assert.Equal(expected, CompareHelper.NormalizeCompare(f1.CompareTo(f2)));
                if (float.IsNaN(f1) || float.IsNaN(f2))
                {
                    Assert.False(f1 >= f2);
                    Assert.False(f1 > f2);
                    Assert.False(f1 <= f2);
                    Assert.False(f1 < f2);
                }
                else
                {
                    if (expected >= 0)
                    {
                        Assert.True(f1 >= f2);
                        Assert.False(f1 < f2);
                    }
                    if (expected > 0)
                    {
                        Assert.True(f1 > f2);
                        Assert.False(f1 <= f2);
                    }
                    if (expected <= 0)
                    {
                        Assert.True(f1 <= f2);
                        Assert.False(f1 > f2);
                    }
                    if (expected < 0)
                    {
                        Assert.True(f1 < f2);
                        Assert.False(f1 >= f2);
                    }
                }
            }
            IComparable comparable = f1;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(value)));
        }

        [Fact]
        public static void TestCompareTo_Invalid()
        {
            IComparable comparable = (float)234;
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo((double)234)); // Obj is not a float
            Assert.Throws<ArgumentException>(null, () => comparable.CompareTo("234")); // Obj is not a float
        }

        [Theory]
        [InlineData((float)789, (float)789, true)]
        [InlineData((float)789, (float)-789, false)]
        [InlineData((float)789, (float)0, false)]
        [InlineData(float.NaN, float.NaN, true)]
        [InlineData((float)789, (double)789, false)]
        [InlineData((float)789, "789", false)]
        public static void TestEquals(float f1, object value, bool expected)
        {
            if (value is float)
            {
                float f2 = (float)value;
                Assert.Equal(expected, f1.Equals(f2));

                if (float.IsNaN(f1) && float.IsNaN(f2))
                {
                    Assert.Equal(!expected, f1 == f2);
                    Assert.Equal(expected, f1 != f2);
                }
                else
                {
                    Assert.Equal(expected, f1 == f2);
                    Assert.Equal(!expected, f1 != f2);
                }

                Assert.NotEqual(0, f1.GetHashCode());
                Assert.Equal(expected, f1.GetHashCode().Equals(f2.GetHashCode()));
            }
            Assert.Equal(expected, f1.Equals(value));
        }

        [Theory]
        [InlineData(float.PositiveInfinity, true)]
        [InlineData(float.NegativeInfinity, true)]
        [InlineData(float.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsInfinity(f));
        }
        
        [Theory]
        [InlineData(float.NegativeInfinity, false)]
        [InlineData(float.PositiveInfinity, false)]
        [InlineData(float.NaN, true)]
        [InlineData(0.0, false)]
        public static void TestIsNaN(float f, bool expected)
        {
            Assert.Equal(expected, float.IsNaN(f));
        }

        [Theory]
        [InlineData(float.NegativeInfinity, true)]
        [InlineData(float.PositiveInfinity, false)]
        [InlineData(float.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsNegativeInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsNegativeInfinity(f));
        }

        [Theory]
        [InlineData(float.PositiveInfinity, true)]
        [InlineData(float.NegativeInfinity, false)]
        [InlineData(float.NaN, false)]
        [InlineData(0.0, false)]
        public static void TestIsPositiveInfinity(float f, bool expected)
        {
            Assert.Equal(expected, float.IsPositiveInfinity(f));
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

            yield return new object[] { "-123", defaultStyle, nullFormat, (float)-123 };
            yield return new object[] { "0", defaultStyle, nullFormat, (float)0 };
            yield return new object[] { "123", defaultStyle, nullFormat, (float)123 };
            yield return new object[] { "  123  ", defaultStyle, nullFormat, (float)123 };
            yield return new object[] { "567.89", defaultStyle, nullFormat, (float)567.89 };
            yield return new object[] { "-567.89", defaultStyle, nullFormat, (float)-567.89 };
            yield return new object[] { "1E23", defaultStyle, nullFormat, (float)1E23 };

            yield return new object[] { "123.1", NumberStyles.AllowDecimalPoint, nullFormat, (float)123.1 };
            yield return new object[] { 1000.ToString("N0"), NumberStyles.AllowThousands, nullFormat, (float)1000 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (float)123 };
            yield return new object[] { "123.567", NumberStyles.Any, emptyFormat, 123.567 };
            yield return new object[] { "123", NumberStyles.Float, emptyFormat, (float)123 };
            yield return new object[] { "$1000", NumberStyles.Currency, customFormat1, (float)1000 };
            yield return new object[] { "123.123", NumberStyles.Float, customFormat2, (float)123.123 };
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, customFormat2, (float)-123 };

            yield return new object[] { "NaN", NumberStyles.Any, invariantFormat, float.NaN };
            yield return new object[] { "Infinity", NumberStyles.Any, invariantFormat, float.PositiveInfinity };
            yield return new object[] { "-Infinity", NumberStyles.Any, invariantFormat, float.NegativeInfinity };
        }

        [Theory, MemberData("Parse_Valid_TestData")]
        public static void TestParse(string value, NumberStyles style, IFormatProvider provider, float expected)
        {
            float f;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.True(float.TryParse(value, out f));
                Assert.Equal(expected, f);

                Assert.Equal(expected, float.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, float.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(float.TryParse(value, style, provider ?? new NumberFormatInfo(), out f));
            Assert.Equal(expected, f);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, float.Parse(value, style));
            }
            Assert.Equal(expected, float.Parse(value, style, provider ?? new NumberFormatInfo()));
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
            float f;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Float)
            {
                Assert.False(float.TryParse(value, out f));
                Assert.Equal(default(float), f);

                Assert.Throws(exceptionType, () => float.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => float.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(float.TryParse(value, style, provider ?? new NumberFormatInfo(), out f));
            Assert.Equal(default(float), f);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => float.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => float.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> ToStringTestData()
        {
            var emptyFormat = NumberFormatInfo.CurrentInfo;
            yield return new object[] { float.MinValue, "G", emptyFormat, "-3.402823E+38" };
            yield return new object[] { (float)-4567, "G", emptyFormat, "-4567" };
            yield return new object[] { (float)-4567.89101, "G", emptyFormat, "-4567.891" };
            yield return new object[] { (float)0, "G", emptyFormat, "0" };
            yield return new object[] { (float)4567, "G", emptyFormat, "4567" };
            yield return new object[] { (float)4567.89101, "G", emptyFormat, "4567.891" };
            yield return new object[] { float.MaxValue, "G", emptyFormat, "3.402823E+38" };

            yield return new object[] { float.Epsilon, "G", emptyFormat, "1.401298E-45" };
            yield return new object[] { float.NaN, "G", emptyFormat, "NaN" };

            yield return new object[] { (float)2468, "N", emptyFormat, string.Format("{0:N}", 2468.00) };

            // Changing the negative pattern doesn't do anything without also passing in a format string
            var customFormat1 = new NumberFormatInfo();
            customFormat1.NumberNegativePattern = 0;
            yield return new object[] { (float)-6310, "G", customFormat1, "-6310" };

            var customFormat2 = new NumberFormatInfo();
            customFormat2.NegativeSign = "#";
            customFormat2.NumberDecimalSeparator = "~";
            customFormat2.NumberGroupSeparator = "*";
            yield return new object[] { (float)-2468, "N", customFormat2, "#2*468~00" };
            yield return new object[] { (float)2468, "N", customFormat2, "2*468~00" };

            var customFormat3 = new NumberFormatInfo();
            customFormat3.NegativeSign = "xx"; // Set to trash to make sure it doesn't show up
            customFormat3.NumberGroupSeparator = "*";
            customFormat3.NumberNegativePattern = 0;
            yield return new object[] { (float)-2468, "N", customFormat3, "(2*468.00)" };

            var invariantFormat = NumberFormatInfo.InvariantInfo;
            yield return new object[] { float.Epsilon, "G", invariantFormat, "1.401298E-45" };
            yield return new object[] { float.NaN, "G", invariantFormat, "NaN" };
            yield return new object[] { float.PositiveInfinity, "G", invariantFormat, "Infinity" };
            yield return new object[] { float.NegativeInfinity, "G", invariantFormat, "-Infinity" };
        }

        [Theory, MemberData("ToStringTestData")]
        public static void TestToString(float f, string format, IFormatProvider provider, string expected)
        {
            bool isDefaultProvider = (provider == null || provider == NumberFormatInfo.CurrentInfo);
            if (string.IsNullOrEmpty(format) || format.ToUpperInvariant() == "G")
            {
                if (isDefaultProvider)
                {
                    Assert.Equal(expected, f.ToString());
                    Assert.Equal(expected, f.ToString((IFormatProvider)null));
                }
                Assert.Equal(expected, f.ToString(provider));
            }
            if (isDefaultProvider)
            {
                Assert.Equal(expected.Replace('e', 'E'), f.ToString(format.ToUpperInvariant())); // If format is upper case, then exponents are printed in upper case
                Assert.Equal(expected.Replace('E', 'e'), f.ToString(format.ToLowerInvariant())); // If format is lower case, then exponents are printed in lower case
                Assert.Equal(expected.Replace('e', 'E'), f.ToString(format.ToUpperInvariant(), null));
                Assert.Equal(expected.Replace('E', 'e'), f.ToString(format.ToLowerInvariant(), null));
            }
            Assert.Equal(expected.Replace('e', 'E'), f.ToString(format.ToUpperInvariant(), provider));
            Assert.Equal(expected.Replace('E', 'e'), f.ToString(format.ToLowerInvariant(), provider));
        }

        [Fact]
        public static void TestToString_Infinity()
        {
            string positiveInfinityString = float.PositiveInfinity.ToString();
            string negativeInfinityString = float.NegativeInfinity.ToString();

            Assert.True(positiveInfinityString == "Infinity" || positiveInfinityString == "∞");
            Assert.True(negativeInfinityString == "-Infinity" || negativeInfinityString == "-∞");
        }

        [Fact]
        public static void TestToString_Invalid()
        {
            float f = 123;
            Assert.Throws<FormatException>(() => f.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => f.ToString("Y", null)); // Invalid format
        }
    }
}
