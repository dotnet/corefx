﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ZeroLengthEquals_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            var slice = new ReadOnlySpan<char>(a, 2, 0);
            Assert.False(span.Equals(slice, StringComparison.Ordinal));

            Assert.False(span.Equals(slice, StringComparison.CurrentCulture));
            Assert.False(span.Equals(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.False(span.Equals(slice, StringComparison.InvariantCulture));
            Assert.False(span.Equals(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.False(span.Equals(slice, StringComparison.OrdinalIgnoreCase));

            span = new ReadOnlySpan<char>(a, 1, 0);
            Assert.True(span.Equals(slice, StringComparison.Ordinal));

            Assert.True(span.Equals(slice, StringComparison.CurrentCulture));
            Assert.True(span.Equals(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.InvariantCulture));
            Assert.True(span.Equals(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void SameSpanEquals_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            Assert.True(span.Equals(span, StringComparison.Ordinal));

            Assert.True(span.Equals(span, StringComparison.CurrentCulture));
            Assert.True(span.Equals(span, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Equals(span, StringComparison.InvariantCulture));
            Assert.True(span.Equals(span, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Equals(span, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void LengthMismatchEquals_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a, 0, 2);
            var slice = new ReadOnlySpan<char>(a, 0, 3);
            Assert.False(span.Equals(slice, StringComparison.Ordinal));

            Assert.False(span.Equals(slice, StringComparison.CurrentCulture));
            Assert.False(span.Equals(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.False(span.Equals(slice, StringComparison.InvariantCulture));
            Assert.False(span.Equals(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.False(span.Equals(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void EqualsOverlappingMatch_StringComparison()
        {
            char[] a = { '4', '5', '6', '5', '6', '5' };
            var span = new ReadOnlySpan<char>(a, 1, 3);
            var slice = new ReadOnlySpan<char>(a, 3, 3);
            Assert.True(span.Equals(slice, StringComparison.Ordinal));

            Assert.True(span.Equals(slice, StringComparison.CurrentCulture));
            Assert.True(span.Equals(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.InvariantCulture));
            Assert.True(span.Equals(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void EqualsMatchDifferentSpans_StringComparison()
        {
            char[] a = { '4', '5', '6', '7' };
            char[] b = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a, 0, 3);
            var slice = new ReadOnlySpan<char>(b, 0, 3);
            Assert.True(span.Equals(slice, StringComparison.Ordinal));

            Assert.True(span.Equals(slice, StringComparison.CurrentCulture));
            Assert.True(span.Equals(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.InvariantCulture));
            Assert.True(span.Equals(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Equals(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void EqualsNoMatch_StringComparison()
        {
            for (int length = 1; length < 150; length++)
            {
                for (int mismatchIndex = 0; mismatchIndex < length; mismatchIndex++)
                {
                    var first = new char[length];
                    var second = new char[length];
                    for (int i = 0; i < length; i++)
                    {
                        first[i] = second[i] = (char)(i + 1);
                    }

                    second[mismatchIndex] = (char)(second[mismatchIndex] + 1);

                    var firstSpan = new ReadOnlySpan<char>(first);
                    var secondSpan = new ReadOnlySpan<char>(second);
                    Assert.False(firstSpan.Equals(secondSpan, StringComparison.Ordinal));

                    Assert.False(firstSpan.Equals(secondSpan, StringComparison.OrdinalIgnoreCase));
                    
                    // Different behavior depending on OS
                    Assert.Equal(
                        firstSpan.ToString().Equals(secondSpan.ToString(), StringComparison.CurrentCulture),
                        firstSpan.Equals(secondSpan, StringComparison.CurrentCulture));
                    Assert.Equal(
                        firstSpan.ToString().Equals(secondSpan.ToString(), StringComparison.CurrentCultureIgnoreCase),
                        firstSpan.Equals(secondSpan, StringComparison.CurrentCultureIgnoreCase));
                    Assert.Equal(
                        firstSpan.ToString().Equals(secondSpan.ToString(), StringComparison.InvariantCulture),
                        firstSpan.Equals(secondSpan, StringComparison.InvariantCulture));
                    Assert.Equal(
                        firstSpan.ToString().Equals(secondSpan.ToString(), StringComparison.InvariantCultureIgnoreCase),
                        firstSpan.Equals(secondSpan, StringComparison.InvariantCultureIgnoreCase));
                }
            }
        }

        [Fact]
        public static void MakeSureNoEqualsChecksGoOutOfRange_StringComparison()
        {
            for (int length = 0; length < 100; length++)
            {
                var first = new char[length + 2];
                first[0] = (char)99;
                first[length + 1] = (char)99;
                var second = new char[length + 2];
                second[0] = (char)100;
                second[length + 1] = (char)100;
                var span1 = new ReadOnlySpan<char>(first, 1, length);
                var span2 = new ReadOnlySpan<char>(second, 1, length);
                Assert.True(span1.Equals(span2, StringComparison.Ordinal));

                Assert.True(span1.Equals(span2, StringComparison.CurrentCulture));
                Assert.True(span1.Equals(span2, StringComparison.CurrentCultureIgnoreCase));
                Assert.True(span1.Equals(span2, StringComparison.InvariantCulture));
                Assert.True(span1.Equals(span2, StringComparison.InvariantCultureIgnoreCase));
                Assert.True(span1.Equals(span2, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public static void EqualsUnknownComparisonType_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Equals(_span, StringComparison.CurrentCulture - 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Equals(_span, StringComparison.OrdinalIgnoreCase + 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Equals(_span, (StringComparison)6));
        }

        [Theory]
        // CurrentCulture
        [InlineData("Hello", "Hello", StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "hello", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Helloo", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", "Hell", StringComparison.CurrentCulture, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.CurrentCulture, false)]
        [InlineData("", "Hello", StringComparison.CurrentCulture, false)]
        [InlineData("", "", StringComparison.CurrentCulture, true)]
        // CurrentCultureIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "hello", StringComparison.CurrentCultureIgnoreCase, true)]
        [InlineData("Hello", "helloo", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", "hell", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.CurrentCultureIgnoreCase, false)]
        [InlineData("", "", StringComparison.CurrentCultureIgnoreCase, true)]
        // InvariantCulture
        [InlineData("Hello", "Hello", StringComparison.InvariantCulture, true)]
        [InlineData("Hello", "hello", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Helloo", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", "Hell", StringComparison.InvariantCulture, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.InvariantCulture, false)]
        [InlineData("", "Hello", StringComparison.InvariantCulture, false)]
        [InlineData("", "", StringComparison.InvariantCulture, true)]
        // InvariantCultureIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "hello", StringComparison.InvariantCultureIgnoreCase, true)]
        [InlineData("Hello", "Helloo", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", "Hell", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.CurrentCulture, true)]
        [InlineData("Hello", "", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.InvariantCultureIgnoreCase, false)]
        [InlineData("", "", StringComparison.InvariantCultureIgnoreCase, true)]
        // Ordinal
        [InlineData("Hello", "Hello", StringComparison.Ordinal, true)]
        [InlineData("Hello", "hello", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Helloo", StringComparison.Ordinal, false)]
        [InlineData("Hello", "Hell", StringComparison.Ordinal, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.Ordinal, false)]
        [InlineData("Hello", "", StringComparison.Ordinal, false)]
        [InlineData("", "Hello", StringComparison.Ordinal, false)]
        [InlineData("", "", StringComparison.Ordinal, true)]
        // OridinalIgnoreCase
        [InlineData("Hello", "Hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("HELLO", "hello", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("Hello", "Helloo", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "Hell", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", SoftHyphen + "Hello" + SoftHyphen, StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234\u5678", StringComparison.OrdinalIgnoreCase, true)]
        [InlineData("\u1234\u5678", "\u1234\u5679", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1235\u5678", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("\u1234\u5678", "\u1234\u56789\u1234", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("Hello", "", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "Hello", StringComparison.OrdinalIgnoreCase, false)]
        [InlineData("", "", StringComparison.OrdinalIgnoreCase, true)]
        public static void Equals(string s1, string s2, StringComparison comparisonType, bool expected)
        {
            Assert.Equal(expected, s1.AsSpan().Equals(s2.AsSpan(), comparisonType));
        }

        public static IEnumerable<object[]> Equals_EncyclopaediaData()
        {
            yield return new object[] { StringComparison.CurrentCulture, false };
            yield return new object[] { StringComparison.CurrentCultureIgnoreCase, false };
            yield return new object[] { StringComparison.Ordinal, false };
            yield return new object[] { StringComparison.OrdinalIgnoreCase, false };

            // Windows and ICU disagree about how these strings compare in the default locale.
            yield return new object[] { StringComparison.InvariantCulture, PlatformDetection.IsWindows };
            yield return new object[] { StringComparison.InvariantCultureIgnoreCase, PlatformDetection.IsWindows };
        }

        [Theory]
        [MemberData(nameof(Equals_EncyclopaediaData))]
        public static void Equals_Encyclopaedia_ReturnsExpected(StringComparison comparison, bool expected)
        {
            string source = "encyclop\u00e6dia";
            string target = "encyclopaedia";
            CultureInfo backupCulture = CultureInfo.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("se-SE");
            Assert.Equal(expected, source.AsSpan().Equals(target.AsSpan(), comparison));

            Thread.CurrentThread.CurrentCulture = backupCulture;
        }
    }
}
