// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public static class StringSplitTests
    {
        [Fact]
        public static void SplitInvalidCount()
        {
            const string value = "a,b";
            const int count = -1;
            const StringSplitOptions options = StringSplitOptions.None;

            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(',', count));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(',', count, options));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(new[] { ',' }, count));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(new[] { ',' }, count, options));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(",", count));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(",", count, options));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => value.Split(new[] { "," }, count, options));
        }

        [Fact]
        public static void SplitInvalidOptions()
        {
            const string value = "a,b";
            const int count = int.MaxValue;
            const StringSplitOptions optionsTooLow = StringSplitOptions.None - 1;
            const StringSplitOptions optionsTooHigh = StringSplitOptions.RemoveEmptyEntries + 1;

            Assert.Throws<ArgumentException>(() => value.Split(',', optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(',', optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(',', count, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(',', count, optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { ',' }, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { ',' }, optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { ',' }, count, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { ',' }, count, optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(",", optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(",", optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(",", count, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(",", count, optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { "," }, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { "," }, optionsTooHigh));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { "," }, count, optionsTooLow));
            Assert.Throws<ArgumentException>(() => value.Split(new[] { "," }, count, optionsTooHigh));
        }

        [Fact]
        public static void SplitZeroCountEmptyResult()
        {
            const string value = "a,b";
            const int count = 0;
            const StringSplitOptions options = StringSplitOptions.None;

            string[] expected = new string[0];

            Assert.Equal(expected, value.Split(',', count));
            Assert.Equal(expected, value.Split(',', count, options));
            Assert.Equal(expected, value.Split(new[] { ',' }, count));
            Assert.Equal(expected, value.Split(new[] { ',' }, count, options));
            Assert.Equal(expected, value.Split(",", count));
            Assert.Equal(expected, value.Split(",", count, options));
            Assert.Equal(expected, value.Split(new[] { "," }, count, options));
        }

        [Fact]
        public static void SplitEmptyValueWithRemoveEmptyEntriesOptionEmptyResult()
        {
            string value = string.Empty;
            const int count = int.MaxValue;
            const StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;

            string[] expected = new string[0];

            Assert.Equal(expected, value.Split(',', options));
            Assert.Equal(expected, value.Split(',', count, options));
            Assert.Equal(expected, value.Split(new[] { ',' }, options));
            Assert.Equal(expected, value.Split(new[] { ',' }, count, options));
            Assert.Equal(expected, value.Split(",", options));
            Assert.Equal(expected, value.Split(",", count, options));
            Assert.Equal(expected, value.Split(new[] { "," }, options));
            Assert.Equal(expected, value.Split(new[] { "," }, count, options));
        }

        [Fact]
        public static void SplitOneCountSingleResult()
        {
            const string value = "a,b";
            const int count = 1;
            const StringSplitOptions options = StringSplitOptions.None;

            string[] expected = new[] { value };

            Assert.Equal(expected, value.Split(',', count));
            Assert.Equal(expected, value.Split(',', count, options));
            Assert.Equal(expected, value.Split(new[] { ',' }, count));
            Assert.Equal(expected, value.Split(new[] { ',' }, count, options));
            Assert.Equal(expected, value.Split(",", count));
            Assert.Equal(expected, value.Split(",", count, options));
            Assert.Equal(expected, value.Split(new[] { "," }, count, options));
        }

        [Fact]
        public static void SplitNoMatchSingleResult()
        {
            const string value = "a b";
            const int count = int.MaxValue;
            const StringSplitOptions options = StringSplitOptions.None;

            string[] expected = new[] { value };

            Assert.Equal(expected, value.Split(','));
            Assert.Equal(expected, value.Split(',', options));
            Assert.Equal(expected, value.Split(',', count, options));
            Assert.Equal(expected, value.Split(new[] { ',' }));
            Assert.Equal(expected, value.Split(new[] { ',' }, options));
            Assert.Equal(expected, value.Split(new[] { ',' }, count));
            Assert.Equal(expected, value.Split(new[] { ',' }, count, options));
            Assert.Equal(expected, value.Split(","));
            Assert.Equal(expected, value.Split(",", options));
            Assert.Equal(expected, value.Split(",", count, options));
            Assert.Equal(expected, value.Split(new[] { "," }, options));
            Assert.Equal(expected, value.Split(new[] { "," }, count, options));
        }

        private const int M = int.MaxValue;

        [Theory]
        [InlineData("", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("", ',', 1, StringSplitOptions.None, new[] { "" })]
        [InlineData("", ',', 2, StringSplitOptions.None, new[] { "" })]
        [InlineData("", ',', 3, StringSplitOptions.None, new[] { "" })]
        [InlineData("", ',', 4, StringSplitOptions.None, new[] { "" })]
        [InlineData("", ',', M, StringSplitOptions.None, new[] { "" })]
        [InlineData("", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("", ',', 1, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("", ',', 2, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("", ',', 3, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("", ',', 4, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("", ',', M, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",", ',', 1, StringSplitOptions.None, new[] { "," })]
        [InlineData(",", ',', 2, StringSplitOptions.None, new[] { "", "" })]
        [InlineData(",", ',', 3, StringSplitOptions.None, new[] { "", "" })]
        [InlineData(",", ',', 4, StringSplitOptions.None, new[] { "", "" })]
        [InlineData(",", ',', M, StringSplitOptions.None, new[] { "", "" })]
        [InlineData(",", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "," })]
        [InlineData(",", ',', 2, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', 3, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', 4, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",", ',', M, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",,", ',', 1, StringSplitOptions.None, new[] { ",," })]
        [InlineData(",,", ',', 2, StringSplitOptions.None, new[] { "", ",", })]
        [InlineData(",,", ',', 3, StringSplitOptions.None, new[] { "", "", "" })]
        [InlineData(",,", ',', 4, StringSplitOptions.None, new[] { "", "", "" })]
        [InlineData(",,", ',', M, StringSplitOptions.None, new[] { "", "", "" })]
        [InlineData(",,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",," })]
        [InlineData(",,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",,", ',', M, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("ab", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("ab", ',', 1, StringSplitOptions.None, new[] { "ab" })]
        [InlineData("ab", ',', 2, StringSplitOptions.None, new[] { "ab" })]
        [InlineData("ab", ',', 3, StringSplitOptions.None, new[] { "ab" })]
        [InlineData("ab", ',', 4, StringSplitOptions.None, new[] { "ab" })]
        [InlineData("ab", ',', M, StringSplitOptions.None, new[] { "ab" })]
        [InlineData("ab", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("ab", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "ab" })]
        [InlineData("ab", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "ab" })]
        [InlineData("ab", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "ab" })]
        [InlineData("ab", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "ab" })]
        [InlineData("ab", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "ab" })]
        [InlineData("a,b", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,b", ',', 1, StringSplitOptions.None, new[] { "a,b" })]
        [InlineData("a,b", ',', 2, StringSplitOptions.None, new[] { "a", "b" })]
        [InlineData("a,b", ',', 3, StringSplitOptions.None, new[] { "a", "b" })]
        [InlineData("a,b", ',', 4, StringSplitOptions.None, new[] { "a", "b" })]
        [InlineData("a,b", ',', M, StringSplitOptions.None, new[] { "a", "b" })]
        [InlineData("a,b", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,b", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a,b" })]
        [InlineData("a,b", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,", ',', 1, StringSplitOptions.None, new[] { "a," })]
        [InlineData("a,", ',', 2, StringSplitOptions.None, new[] { "a", "" })]
        [InlineData("a,", ',', 3, StringSplitOptions.None, new[] { "a", "" })]
        [InlineData("a,", ',', 4, StringSplitOptions.None, new[] { "a", "" })]
        [InlineData("a,", ',', M, StringSplitOptions.None, new[] { "a", "" })]
        [InlineData("a,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a," })]
        [InlineData("a,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a" })]
        [InlineData("a,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a" })]
        [InlineData("a,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a" })]
        [InlineData("a,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a" })]
        [InlineData(",b", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",b", ',', 1, StringSplitOptions.None, new[] { ",b" })]
        [InlineData(",b", ',', 2, StringSplitOptions.None, new[] { "", "b" })]
        [InlineData(",b", ',', 3, StringSplitOptions.None, new[] { "", "b" })]
        [InlineData(",b", ',', 4, StringSplitOptions.None, new[] { "", "b" })]
        [InlineData(",b", ',', M, StringSplitOptions.None, new[] { "", "b" })]
        [InlineData(",b", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",b", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",b" })]
        [InlineData(",b", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "b" })]
        [InlineData(",b", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "b" })]
        [InlineData(",b", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "b" })]
        [InlineData(",b", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "b" })]
        [InlineData(",a,b", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",a,b", ',', 1, StringSplitOptions.None, new[] { ",a,b" })]
        [InlineData(",a,b", ',', 2, StringSplitOptions.None, new[] { "", "a,b" })]
        [InlineData(",a,b", ',', 3, StringSplitOptions.None, new[] { "", "a", "b" })]
        [InlineData(",a,b", ',', 4, StringSplitOptions.None, new[] { "", "a", "b" })]
        [InlineData(",a,b", ',', M, StringSplitOptions.None, new[] { "", "a", "b" })]
        [InlineData(",a,b", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",a,b", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",a,b" })]
        [InlineData(",a,b", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData(",a,b", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData(",a,b", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData(",a,b", ',', 5, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,b,", ',', 1, StringSplitOptions.None, new[] { "a,b," })]
        [InlineData("a,b,", ',', 2, StringSplitOptions.None, new[] { "a", "b,", })]
        [InlineData("a,b,", ',', 3, StringSplitOptions.None, new[] { "a", "b", "" })]
        [InlineData("a,b,", ',', 4, StringSplitOptions.None, new[] { "a", "b", "" })]
        [InlineData("a,b,", ',', M, StringSplitOptions.None, new[] { "a", "b", "" })]
        [InlineData("a,b,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,b,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a,b," })]
        [InlineData("a,b,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b," })]
        [InlineData("a,b,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b" })]
        [InlineData("a,b,c", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,b,c", ',', 1, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", ',', 2, StringSplitOptions.None, new[] { "a", "b,c" })]
        [InlineData("a,b,c", ',', 3, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", ',', 4, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", ',', M, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,b,c", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a,b,c" })]
        [InlineData("a,b,c", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b,c", })]
        [InlineData("a,b,c", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("a,,c", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,,c", ',', 1, StringSplitOptions.None, new[] { "a,,c" })]
        [InlineData("a,,c", ',', 2, StringSplitOptions.None, new[] { "a", ",c", })]
        [InlineData("a,,c", ',', 3, StringSplitOptions.None, new[] { "a", "", "c" })]
        [InlineData("a,,c", ',', 4, StringSplitOptions.None, new[] { "a", "", "c" })]
        [InlineData("a,,c", ',', M, StringSplitOptions.None, new[] { "a", "", "c" })]
        [InlineData("a,,c", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,,c", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a,,c" })]
        [InlineData("a,,c", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "c", })]
        [InlineData("a,,c", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "c" })]
        [InlineData("a,,c", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "c" })]
        [InlineData("a,,c", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "c" })]
        [InlineData(",a,b,c", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",a,b,c", ',', 1, StringSplitOptions.None, new[] { ",a,b,c" })]
        [InlineData(",a,b,c", ',', 2, StringSplitOptions.None, new[] { "", "a,b,c" })]
        [InlineData(",a,b,c", ',', 3, StringSplitOptions.None, new[] { "", "a", "b,c" })]
        [InlineData(",a,b,c", ',', 4, StringSplitOptions.None, new[] { "", "a", "b", "c" })]
        [InlineData(",a,b,c", ',', M, StringSplitOptions.None, new[] { "", "a", "b", "c" })]
        [InlineData(",a,b,c", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",a,b,c", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",a,b,c" })]
        [InlineData(",a,b,c", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b,c", })]
        [InlineData(",a,b,c", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData(",a,b,c", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData(",a,b,c", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("a,b,c,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("a,b,c,", ',', 1, StringSplitOptions.None, new[] { "a,b,c," })]
        [InlineData("a,b,c,", ',', 2, StringSplitOptions.None, new[] { "a", "b,c," })]
        [InlineData("a,b,c,", ',', 3, StringSplitOptions.None, new[] { "a", "b", "c,", })]
        [InlineData("a,b,c,", ',', 4, StringSplitOptions.None, new[] { "a", "b", "c", "" })]
        [InlineData("a,b,c,", ',', M, StringSplitOptions.None, new[] { "a", "b", "c", "" })]
        [InlineData("a,b,c,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("a,b,c,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "a,b,c," })]
        [InlineData("a,b,c,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b,c,", })]
        [InlineData("a,b,c,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c," })]
        [InlineData("a,b,c,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("a,b,c,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData(",a,b,c,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",a,b,c,", ',', 1, StringSplitOptions.None, new[] { ",a,b,c," })]
        [InlineData(",a,b,c,", ',', 2, StringSplitOptions.None, new[] { "", "a,b,c," })]
        [InlineData(",a,b,c,", ',', 3, StringSplitOptions.None, new[] { "", "a", "b,c," })]
        [InlineData(",a,b,c,", ',', 4, StringSplitOptions.None, new[] { "", "a", "b", "c," })]
        [InlineData(",a,b,c,", ',', M, StringSplitOptions.None, new[] { "", "a", "b", "c", "" })]
        [InlineData(",a,b,c,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",a,b,c,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",a,b,c," })]
        [InlineData(",a,b,c,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b,c," })]
        [InlineData(",a,b,c,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c," })]
        [InlineData(",a,b,c,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData(",a,b,c,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "a", "b", "c" })]
        [InlineData("first,second", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,second", ',', 1, StringSplitOptions.None, new[] { "first,second" })]
        [InlineData("first,second", ',', 2, StringSplitOptions.None, new[] { "first", "second" })]
        [InlineData("first,second", ',', 3, StringSplitOptions.None, new[] { "first", "second" })]
        [InlineData("first,second", ',', 4, StringSplitOptions.None, new[] { "first", "second" })]
        [InlineData("first,second", ',', M, StringSplitOptions.None, new[] { "first", "second" })]
        [InlineData("first,second", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,second", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first,second" })]
        [InlineData("first,second", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,", ',', 1, StringSplitOptions.None, new[] { "first," })]
        [InlineData("first,", ',', 2, StringSplitOptions.None, new[] { "first", "" })]
        [InlineData("first,", ',', 3, StringSplitOptions.None, new[] { "first", "" })]
        [InlineData("first,", ',', 4, StringSplitOptions.None, new[] { "first", "" })]
        [InlineData("first,", ',', M, StringSplitOptions.None, new[] { "first", "" })]
        [InlineData("first,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first," })]
        [InlineData("first,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first" })]
        [InlineData("first,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first" })]
        [InlineData("first,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first" })]
        [InlineData("first,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first" })]
        [InlineData(",second", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",second", ',', 1, StringSplitOptions.None, new[] { ",second" })]
        [InlineData(",second", ',', 2, StringSplitOptions.None, new[] { "", "second" })]
        [InlineData(",second", ',', 3, StringSplitOptions.None, new[] { "", "second" })]
        [InlineData(",second", ',', 4, StringSplitOptions.None, new[] { "", "second" })]
        [InlineData(",second", ',', M, StringSplitOptions.None, new[] { "", "second" })]
        [InlineData(",second", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",second", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",second" })]
        [InlineData(",second", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "second" })]
        [InlineData(",second", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "second" })]
        [InlineData(",second", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "second" })]
        [InlineData(",second", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "second" })]
        [InlineData(",first,second", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",first,second", ',', 1, StringSplitOptions.None, new[] { ",first,second" })]
        [InlineData(",first,second", ',', 2, StringSplitOptions.None, new[] { "", "first,second" })]
        [InlineData(",first,second", ',', 3, StringSplitOptions.None, new[] { "", "first", "second" })]
        [InlineData(",first,second", ',', 4, StringSplitOptions.None, new[] { "", "first", "second" })]
        [InlineData(",first,second", ',', M, StringSplitOptions.None, new[] { "", "first", "second" })]
        [InlineData(",first,second", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",first,second", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",first,second" })]
        [InlineData(",first,second", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData(",first,second", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData(",first,second", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData(",first,second", ',', 5, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,second,", ',', 1, StringSplitOptions.None, new[] { "first,second," })]
        [InlineData("first,second,", ',', 2, StringSplitOptions.None, new[] { "first", "second,", })]
        [InlineData("first,second,", ',', 3, StringSplitOptions.None, new[] { "first", "second", "" })]
        [InlineData("first,second,", ',', 4, StringSplitOptions.None, new[] { "first", "second", "" })]
        [InlineData("first,second,", ',', M, StringSplitOptions.None, new[] { "first", "second", "" })]
        [InlineData("first,second,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,second,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first,second," })]
        [InlineData("first,second,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second," })]
        [InlineData("first,second,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second" })]
        [InlineData("first,second,third", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,second,third", ',', 1, StringSplitOptions.None, new[] { "first,second,third" })]
        [InlineData("first,second,third", ',', 2, StringSplitOptions.None, new[] { "first", "second,third" })]
        [InlineData("first,second,third", ',', 3, StringSplitOptions.None, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ',', 4, StringSplitOptions.None, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ',', M, StringSplitOptions.None, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,second,third", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first,second,third" })]
        [InlineData("first,second,third", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second,third", })]
        [InlineData("first,second,third", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,,third", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,,third", ',', 1, StringSplitOptions.None, new[] { "first,,third" })]
        [InlineData("first,,third", ',', 2, StringSplitOptions.None, new[] { "first", ",third", })]
        [InlineData("first,,third", ',', 3, StringSplitOptions.None, new[] { "first", "", "third" })]
        [InlineData("first,,third", ',', 4, StringSplitOptions.None, new[] { "first", "", "third" })]
        [InlineData("first,,third", ',', M, StringSplitOptions.None, new[] { "first", "", "third" })]
        [InlineData("first,,third", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,,third", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first,,third" })]
        [InlineData("first,,third", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "third", })]
        [InlineData("first,,third", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "third" })]
        [InlineData("first,,third", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "third" })]
        [InlineData("first,,third", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "third" })]
        [InlineData(",first,second,third", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",first,second,third", ',', 1, StringSplitOptions.None, new[] { ",first,second,third" })]
        [InlineData(",first,second,third", ',', 2, StringSplitOptions.None, new[] { "", "first,second,third" })]
        [InlineData(",first,second,third", ',', 3, StringSplitOptions.None, new[] { "", "first", "second,third" })]
        [InlineData(",first,second,third", ',', 4, StringSplitOptions.None, new[] { "", "first", "second", "third" })]
        [InlineData(",first,second,third", ',', M, StringSplitOptions.None, new[] { "", "first", "second", "third" })]
        [InlineData(",first,second,third", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",first,second,third", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",first,second,third" })]
        [InlineData(",first,second,third", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second,third", })]
        [InlineData(",first,second,third", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData(",first,second,third", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData(",first,second,third", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,second,third,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData("first,second,third,", ',', 1, StringSplitOptions.None, new[] { "first,second,third," })]
        [InlineData("first,second,third,", ',', 2, StringSplitOptions.None, new[] { "first", "second,third," })]
        [InlineData("first,second,third,", ',', 3, StringSplitOptions.None, new[] { "first", "second", "third,", })]
        [InlineData("first,second,third,", ',', 4, StringSplitOptions.None, new[] { "first", "second", "third", "" })]
        [InlineData("first,second,third,", ',', M, StringSplitOptions.None, new[] { "first", "second", "third", "" })]
        [InlineData("first,second,third,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData("first,second,third,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { "first,second,third," })]
        [InlineData("first,second,third,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second,third,", })]
        [InlineData("first,second,third,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third," })]
        [InlineData("first,second,third,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,second,third,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData(",first,second,third,", ',', 0, StringSplitOptions.None, new string[0])]
        [InlineData(",first,second,third,", ',', 1, StringSplitOptions.None, new[] { ",first,second,third," })]
        [InlineData(",first,second,third,", ',', 2, StringSplitOptions.None, new[] { "", "first,second,third," })]
        [InlineData(",first,second,third,", ',', 3, StringSplitOptions.None, new[] { "", "first", "second,third," })]
        [InlineData(",first,second,third,", ',', 4, StringSplitOptions.None, new[] { "", "first", "second", "third," })]
        [InlineData(",first,second,third,", ',', M, StringSplitOptions.None, new[] { "", "first", "second", "third", "" })]
        [InlineData(",first,second,third,", ',', 0, StringSplitOptions.RemoveEmptyEntries, new string[0])]
        [InlineData(",first,second,third,", ',', 1, StringSplitOptions.RemoveEmptyEntries, new[] { ",first,second,third," })]
        [InlineData(",first,second,third,", ',', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second,third," })]
        [InlineData(",first,second,third,", ',', 3, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third," })]
        [InlineData(",first,second,third,", ',', 4, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData(",first,second,third,", ',', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first", "second", "third" })]
        [InlineData("first,second,third", ' ', M, StringSplitOptions.None, new[] { "first,second,third" })]
        [InlineData("first,second,third", ' ', M, StringSplitOptions.RemoveEmptyEntries, new[] { "first,second,third" })]
        [InlineData("Foo Bar Baz", ' ', 2, StringSplitOptions.RemoveEmptyEntries, new[] { "Foo", "Bar Baz" })]
        [InlineData("Foo Bar Baz", ' ', M, StringSplitOptions.None, new[] { "Foo", "Bar", "Baz" })]
        public static void SplitCharSeparator(string value, char separator, int count, StringSplitOptions options, string[] expected)
        {
            Assert.Equal(expected, value.Split(separator, count, options));
            Assert.Equal(expected, value.Split(new[] { separator }, count, options));
            Assert.Equal(expected, value.Split(separator.ToString(), count, options));
            Assert.Equal(expected, value.Split(new[] { separator.ToString() }, count, options));
        }

        [Theory]
        [InlineData("a,b,c", null, M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", "", M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("aaabaaabaaa", "aa", M, StringSplitOptions.None, new[] { "", "ab", "ab", "a" })]
        [InlineData("aaabaaabaaa", "aa", M, StringSplitOptions.RemoveEmptyEntries, new[] { "ab", "ab", "a" })]
        [InlineData("this, is, a, string, with some spaces", ", ", M, StringSplitOptions.None, new[] { "this", "is", "a", "string", "with some spaces" })]
        public static void SplitStringSeparator(string value, string separator, int count, StringSplitOptions options, string[] expected)
        {
            Assert.Equal(expected, value.Split(separator, count, options));
            Assert.Equal(expected, value.Split(new[] { separator }, count, options));
        }

        [Fact]
        public static void SplitNullCharArraySeparator_BindsToCharArrayOverload()
        {
            string value = "a b c";
            string[] expected = new[] { "a", "b", "c" };
            // Ensure Split(null) compiles successfully as a call to Split(char[])
            Assert.Equal(expected, value.Split(null));
        }

        [Theory]
        [InlineData("a b c", null, M, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a b c", new char[0], M, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", null, M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", new char[0], M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ' ', ',' }, M, StringSplitOptions.None, new[] { "this", "", "is", "", "a", "", "string", "", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ',', ' ' }, M, StringSplitOptions.None, new[] { "this", "", "is", "", "a", "", "string", "", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ' ', ',' }, M, StringSplitOptions.RemoveEmptyEntries, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ',', ' ' }, M, StringSplitOptions.RemoveEmptyEntries, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        public static void SplitCharArraySeparator(string value, char[] separators, int count, StringSplitOptions options, string[] expected)
        {
            Assert.Equal(expected, value.Split(separators, count, options));
            Assert.Equal(expected, value.Split(ToStringArray(separators), count, options));
        }

        [Theory]
        [InlineData("a b c", null, M, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a b c", new string[0], M, StringSplitOptions.None, new[] { "a", "b", "c" })]
        [InlineData("a,b,c", null, M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", new string[0], M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", new string[] { null }, M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("a,b,c", new string[] { "" }, M, StringSplitOptions.None, new[] { "a,b,c" })]
        [InlineData("this, is, a, string, with some spaces", new[] { " ", ", " }, M, StringSplitOptions.None, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ", ", " " }, M, StringSplitOptions.None, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { " ", ", " }, M, StringSplitOptions.RemoveEmptyEntries, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        [InlineData("this, is, a, string, with some spaces", new[] { ", ", " " }, M, StringSplitOptions.RemoveEmptyEntries, new[] { "this", "is", "a", "string", "with", "some", "spaces" })]
        public static void SplitStringArraySeparator(string value, string[] separators, int count, StringSplitOptions options, string[] expected)
        {
            Assert.Equal(expected, value.Split(separators, count, options));
        }

        private static string[] ToStringArray(char[] source)
        {
            if (source == null)
                return null;

            string[] result = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i].ToString();
            }
            return result;
        }
    }
}
