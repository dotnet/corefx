// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace System.Tests
{
    public static class UInt16Tests
    {
        [Fact]
        public static void Ctor_Empty()
        {
            var i = new ushort();
            Assert.Equal(0, i);
        }

        [Fact]
        public static void Ctor_Value()
        {
            ushort i = 41;
            Assert.Equal(41, i);
        }

        [Fact]
        public static void MaxValue()
        {
            Assert.Equal(0xFFFF, ushort.MaxValue);
        }

        [Fact]
        public static void MinValue()
        {
            Assert.Equal(0, ushort.MinValue);
        }

        [Theory]
        [InlineData((ushort)234, (ushort)234, 0)]
        [InlineData((ushort)234, ushort.MinValue, 1)]
        [InlineData((ushort)234, (ushort)0, 1)]
        [InlineData((ushort)234, (ushort)123, 1)]
        [InlineData((ushort)234, (ushort)456, -1)]
        [InlineData((ushort)234, ushort.MaxValue, -1)]
        [InlineData((ushort)234, null, 1)]
        public static void CompareTo(ushort i, object value, int expected)
        {
            if (value is ushort)
            {
                Assert.Equal(expected, Math.Sign(i.CompareTo((ushort)value)));
            }
            IComparable comparable = i;
            Assert.Equal(expected, Math.Sign(comparable.CompareTo(value)));
        }

        [Fact]
        public static void CompareTo_ObjectNotUShort_ThrowsArgumentException()
        {
            IComparable comparable = (ushort)234;
            AssertExtensions.Throws<ArgumentException>(null, () => comparable.CompareTo("a")); // Obj is not a ushort
            AssertExtensions.Throws<ArgumentException>(null, () => comparable.CompareTo(234)); // Obj is not a ushort
        }

        [Theory]
        [InlineData((ushort)789, (ushort)789, true)]
        [InlineData((ushort)788, (ushort)0, false)]
        [InlineData((ushort)0, (ushort)0, true)]
        [InlineData((ushort)789, null, false)]
        [InlineData((ushort)789, "789", false)]
        [InlineData((ushort)789, 789, false)]
        public static void Equals(ushort i1, object obj, bool expected)
        {
            if (obj is ushort)
            {
                Assert.Equal(expected, i1.Equals((ushort)obj));
                Assert.Equal(expected, i1.GetHashCode().Equals(((ushort)obj).GetHashCode()));
                Assert.Equal(i1, i1.GetHashCode());
            }
            Assert.Equal(expected, i1.Equals(obj));
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            NumberFormatInfo emptyFormat = NumberFormatInfo.CurrentInfo;
            yield return new object[] { (ushort)0, "G", emptyFormat, "0" };
            yield return new object[] { (ushort)4567, "G", emptyFormat, "4567" };
            yield return new object[] { ushort.MaxValue, "G", emptyFormat, "65535" };

            yield return new object[] { (ushort)0x2468, "x", emptyFormat, "2468" };
            yield return new object[] { (ushort)2468, "N", emptyFormat, string.Format("{0:N}", 2468.00) };

            NumberFormatInfo customFormat = new NumberFormatInfo();
            customFormat.NegativeSign = "#";
            customFormat.NumberDecimalSeparator = "~";
            customFormat.NumberGroupSeparator = "*";
            yield return new object[] { (ushort)2468, "N", customFormat, "2*468~00" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public static void ToString(ushort i, string format, IFormatProvider provider, string expected)
        {
            // Format should be case insensitive
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
        public static void ToString_InvalidFormat_ThrowsFormatException()
        {
            ushort i = 123;
            Assert.Throws<FormatException>(() => i.ToString("Y")); // Invalid format
            Assert.Throws<FormatException>(() => i.ToString("Y", null)); // Invalid format
        }

        public static IEnumerable<object[]> Parse_Valid_TestData()
        {
            NumberStyles defaultStyle = NumberStyles.Integer;
            NumberFormatInfo emptyFormat = new NumberFormatInfo();

            NumberFormatInfo customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";

            yield return new object[] { "0", defaultStyle, null, (ushort)0 };
            yield return new object[] { "123", defaultStyle, null, (ushort)123 };
            yield return new object[] { "+123", defaultStyle, null, (ushort)123 };
            yield return new object[] { "  123  ", defaultStyle, null, (ushort)123 };
            yield return new object[] { "65535", defaultStyle, null, (ushort)65535 };

            yield return new object[] { "12", NumberStyles.HexNumber, null, (ushort)0x12 };
            yield return new object[] { "1000", NumberStyles.AllowThousands, null, (ushort)1000 };

            yield return new object[] { "123", defaultStyle, emptyFormat, (ushort)123 };

            yield return new object[] { "123", NumberStyles.Any, emptyFormat, (ushort)123 };
            yield return new object[] { "12", NumberStyles.HexNumber, emptyFormat, (ushort)0x12 };
            yield return new object[] { "abc", NumberStyles.HexNumber, emptyFormat, (ushort)0xabc };
            yield return new object[] { "ABC", NumberStyles.HexNumber, null, (ushort)0xabc };
            yield return new object[] { "$1,000", NumberStyles.Currency, customFormat, (ushort)1000 };
        }

        [Theory]
        [MemberData(nameof(Parse_Valid_TestData))]
        public static void Parse(string value, NumberStyles style, IFormatProvider provider, ushort expected)
        {
            ushort result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.True(ushort.TryParse(value, out result));
                Assert.Equal(expected, result);

                Assert.Equal(expected, ushort.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Equal(expected, ushort.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.True(ushort.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(expected, result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Equal(expected, ushort.Parse(value, style));
            }
            Assert.Equal(expected, ushort.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        public static IEnumerable<object[]> Parse_Invalid_TestData()
        {
            NumberStyles defaultStyle = NumberStyles.Integer;

            NumberFormatInfo customFormat = new NumberFormatInfo();
            customFormat.CurrencySymbol = "$";
            customFormat.NumberDecimalSeparator = ".";

            yield return new object[] { null, defaultStyle, null, typeof(ArgumentNullException) };
            yield return new object[] { "", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { " \t \n \r ", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { "Garbage", defaultStyle, null, typeof(FormatException) };

            yield return new object[] { "abc", defaultStyle, null, typeof(FormatException) }; // Hex value
            yield return new object[] { "1E23", defaultStyle, null, typeof(FormatException) }; // Exponent
            yield return new object[] { "(123)", defaultStyle, null, typeof(FormatException) }; // Parentheses
            yield return new object[] { 100.ToString("C0"), defaultStyle, null, typeof(FormatException) }; // Currency
            yield return new object[] { 1000.ToString("N0"), defaultStyle, null, typeof(FormatException) }; // Thousands
            yield return new object[] { 678.90.ToString("F2"), defaultStyle, null, typeof(FormatException) }; // Decimal
            yield return new object[] { "+-123", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { "-+123", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { "+abc", NumberStyles.HexNumber, null, typeof(FormatException) };
            yield return new object[] { "-abc", NumberStyles.HexNumber, null, typeof(FormatException) };

            yield return new object[] { "- 123", defaultStyle, null, typeof(FormatException) };
            yield return new object[] { "+ 123", defaultStyle, null, typeof(FormatException) };

            yield return new object[] { "abc", NumberStyles.None, null, typeof(FormatException) }; // Hex value
            yield return new object[] { "  123  ", NumberStyles.None, null, typeof(FormatException) }; // Trailing and leading whitespace

            yield return new object[] { "678.90", defaultStyle, customFormat, typeof(FormatException) }; // Decimal

            yield return new object[] { "-1", defaultStyle, null, typeof(OverflowException) }; // < min value
            yield return new object[] { "65536", defaultStyle, null, typeof(OverflowException) }; // > max value
            yield return new object[] { "(123)", NumberStyles.AllowParentheses, null, typeof(OverflowException) }; // Parentheses = negative
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            ushort result;
            // If no style is specified, use the (String) or (String, IFormatProvider) overload
            if (style == NumberStyles.Integer)
            {
                Assert.False(ushort.TryParse(value, out result));
                Assert.Equal(default(ushort), result);

                Assert.Throws(exceptionType, () => ushort.Parse(value));

                // If a format provider is specified, but the style is the default, use the (String, IFormatProvider) overload
                if (provider != null)
                {
                    Assert.Throws(exceptionType, () => ushort.Parse(value, provider));
                }
            }

            // If a format provider isn't specified, test the default one, using a new instance of NumberFormatInfo
            Assert.False(ushort.TryParse(value, style, provider ?? new NumberFormatInfo(), out result));
            Assert.Equal(default(ushort), result);

            // If a format provider isn't specified, test the default one, using the (String, NumberStyles) overload
            if (provider == null)
            {
                Assert.Throws(exceptionType, () => ushort.Parse(value, style));
            }
            Assert.Throws(exceptionType, () => ushort.Parse(value, style, provider ?? new NumberFormatInfo()));
        }

        [Theory]
        [InlineData(NumberStyles.HexNumber | NumberStyles.AllowParentheses)]
        [InlineData(unchecked((NumberStyles)0xFFFFFC00))]
        public static void TryParse_InvalidNumberStyle_ThrowsArgumentException(NumberStyles style)
        {
            ushort result = 0;
            Assert.Throws<ArgumentException>(() => ushort.TryParse("1", style, null, out result));
            Assert.Equal(default(ushort), result);

            Assert.Throws<ArgumentException>(() => ushort.Parse("1", style));
            Assert.Throws<ArgumentException>(() => ushort.Parse("1", style, null));
        }
    }
}
