// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.Tests
{
    public static partial class DateTimeOffsetTests
    {
        [Fact]
        public static void ToString_ParseSpan_RoundtripsSuccessfully()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedString = expected.ToString();

            Assert.Equal(expectedString, DateTimeOffset.Parse(expectedString.AsSpan()).ToString());
            Assert.Equal(expectedString, DateTimeOffset.Parse(expectedString.AsSpan(), null).ToString());
            Assert.Equal(expectedString, DateTimeOffset.Parse(expectedString.AsSpan(), null, DateTimeStyles.None).ToString());

            Assert.True(DateTimeOffset.TryParse(expectedString.AsSpan(), out DateTimeOffset actual));
            Assert.Equal(expectedString, actual.ToString());
            Assert.True(DateTimeOffset.TryParse(expectedString.AsSpan(), null, DateTimeStyles.None, out actual));
            Assert.Equal(expectedString, actual.ToString());
        }

        [Fact]
        public static void ToString_ParseExactSpan_RoundtripsSuccessfully()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedString = expected.ToString("u");

            Assert.Equal(expectedString, DateTimeOffset.ParseExact(expectedString, "u", null, DateTimeStyles.None).ToString("u"));
            Assert.Equal(expectedString, DateTimeOffset.ParseExact(expectedString, new[] { "u" }, null, DateTimeStyles.None).ToString("u"));

            Assert.True(DateTimeOffset.TryParseExact(expectedString, "u", null, DateTimeStyles.None, out DateTimeOffset actual));
            Assert.Equal(expectedString, actual.ToString("u"));
            Assert.True(DateTimeOffset.TryParseExact(expectedString, new[] { "u" }, null, DateTimeStyles.None, out actual));
            Assert.Equal(expectedString, actual.ToString("u"));
        }

        [Fact]
        public static void TryFormat_ToString_EqualResults()
        {
            DateTimeOffset expected = DateTimeOffset.MaxValue;
            string expectedString = expected.ToString();

            // Just the right amount of space, succeeds
            Span<char> actual = new char[expectedString.Length];
            Assert.True(expected.TryFormat(actual, out int charsWritten));
            Assert.Equal(expectedString.Length, charsWritten);
            Assert.Equal<char>(expectedString.ToCharArray(), actual.ToArray());

            // Too little space, fails
            actual = new char[expectedString.Length - 1];
            Assert.False(expected.TryFormat(actual, out charsWritten));
            Assert.Equal(0, charsWritten);

            // More than enough space, succeeds
            actual = new char[expectedString.Length + 1];
            Assert.True(expected.TryFormat(actual, out charsWritten));
            Assert.Equal(expectedString.Length, charsWritten);
            Assert.Equal<char>(expectedString.ToCharArray(), actual.Slice(0, expectedString.Length).ToArray());
            Assert.Equal(0, actual[actual.Length - 1]);
        }

        [Fact]
        public static void UnixEpoch()
        {
            VerifyDateTimeOffset(DateTimeOffset.UnixEpoch, 1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        }
    }
}
