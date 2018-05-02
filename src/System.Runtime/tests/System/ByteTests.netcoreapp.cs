// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace System.Tests
{
    public partial class ByteTests
    {
        public static IEnumerable<object[]> Parse_ValidWithOffsetCount_TestData()
        {
            foreach (object[] inputs in Parse_Valid_TestData())
            {
                yield return new object[] { inputs[0], 0, ((string)inputs[0]).Length, inputs[1], inputs[2], inputs[3] };
            }

            yield return new object[] { "123", 0, 2, NumberStyles.Integer, null, (byte)12 };
            yield return new object[] { "+123", 0, 2, NumberStyles.Integer, null, (byte)1 };
            yield return new object[] { "+123", 1, 3, NumberStyles.Integer, null, (byte)123 };
            yield return new object[] { "  123  ", 4, 1, NumberStyles.Integer, null, (byte)3 };
            yield return new object[] { "12", 1, 1, NumberStyles.HexNumber, null, (byte)0x2 };
            yield return new object[] { "10", 0, 1, NumberStyles.AllowThousands, null, (byte)1 };
            yield return new object[] { "$100", 0, 2, NumberStyles.Currency, new NumberFormatInfo() { CurrencySymbol = "$" }, (byte)1 };
        }


        [Theory]
        [MemberData(nameof(Parse_ValidWithOffsetCount_TestData))]
        public static void Parse_Span_Valid(string value, int offset, int count, NumberStyles style, IFormatProvider provider, byte expected)
        {
            Assert.Equal(expected, byte.Parse(value.AsSpan(offset, count), style, provider));

            Assert.True(byte.TryParse(value.AsSpan(offset, count), style, provider, out byte result));
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(Parse_Invalid_TestData))]
        public static void Parse_Span_Invalid(string value, NumberStyles style, IFormatProvider provider, Type exceptionType)
        {
            if (value != null)
            {
                Assert.Throws(exceptionType, () => byte.Parse(value.AsSpan(), style, provider));

                Assert.False(byte.TryParse(value.AsSpan(), style, provider, out byte result));
                Assert.Equal(0, result);
            }
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public static void TryFormat(byte i, string format, IFormatProvider provider, string expected)
        {
            char[] actual;
            int charsWritten;

            // Just right
            actual = new char[expected.Length];
            Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten, format, provider));
            Assert.Equal(expected.Length, charsWritten);
            Assert.Equal(expected, new string(actual));

            // Longer than needed
            actual = new char[expected.Length + 1];
            Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten, format, provider));
            Assert.Equal(expected.Length, charsWritten);
            Assert.Equal(expected, new string(actual, 0, charsWritten));

            // Too short
            if (expected.Length > 0)
            {
                actual = new char[expected.Length - 1];
                Assert.False(i.TryFormat(actual.AsSpan(), out charsWritten, format, provider));
                Assert.Equal(0, charsWritten);
            }

            if (format != null)
            {
                // Upper format
                actual = new char[expected.Length];
                Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten, format.ToUpperInvariant(), provider));
                Assert.Equal(expected.Length, charsWritten);
                Assert.Equal(expected.ToUpperInvariant(), new string(actual));

                // Lower format
                actual = new char[expected.Length];
                Assert.True(i.TryFormat(actual.AsSpan(), out charsWritten, format.ToLowerInvariant(), provider));
                Assert.Equal(expected.Length, charsWritten);
                Assert.Equal(expected.ToLowerInvariant(), new string(actual));
            }
        }
    }
}
