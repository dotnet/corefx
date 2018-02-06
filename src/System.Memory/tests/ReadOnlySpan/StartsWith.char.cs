// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Threading;
using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ZeroLengthStartsWith_StringComparison()
        {
            var a = new char[3];

            var span = new ReadOnlySpan<char>(a);
            var slice = new ReadOnlySpan<char>(a, 2, 0);
            Assert.True(span.StartsWith(slice, StringComparison.Ordinal));
        }

        [Fact]
        public static void SameSpanStartsWith_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            Assert.True(span.StartsWith(span, StringComparison.Ordinal));
        }

        [Fact]
        public static void LengthMismatchStartsWith_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a, 0, 2);
            var slice = new ReadOnlySpan<char>(a, 0, 3);
            Assert.False(span.StartsWith(slice, StringComparison.Ordinal));
        }

        [Fact]
        public static void StartsWithMatch_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a, 0, 3);
            var slice = new ReadOnlySpan<char>(a, 0, 2);
            Assert.True(span.StartsWith(slice, StringComparison.Ordinal));
        }

        [Fact]
        public static void StartsWithMatchDifferentSpans_StringComparison()
        {
            char[] a = { '4', '5', '6', '7' };
            char[] b = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a, 0, 3);
            var slice = new ReadOnlySpan<char>(b, 0, 3);
            Assert.True(span.StartsWith(slice, StringComparison.Ordinal));
        }

        [Fact]
        public static void StartsWithNoMatch_StringComparison()
        {
            for (int length = 1; length < 32; length++)
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
                    Assert.False(firstSpan.StartsWith(secondSpan, StringComparison.Ordinal));
                }
            }
        }

        [Fact]
        public static void MakeSureNoStartsWithChecksGoOutOfRange_StringComparison()
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
                Assert.True(span1.StartsWith(span2, StringComparison.Ordinal));
            }
        }

        [Fact]
        public static void StartsWithUnknownComparisonType_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.StartsWith(_span, StringComparison.CurrentCulture - 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.StartsWith(_span, StringComparison.OrdinalIgnoreCase + 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.StartsWith(_span, (StringComparison)6));
        }

        [Fact]
        public static void StartsWithMatchNonOrdinal_StringComparison()
        {
            ReadOnlySpan<char> span = new char[] { 'a', 'b', 'c', 'd' };
            ReadOnlySpan<char> value = new char[] { 'a', 'B', 'c' };
            Assert.False(span.StartsWith(value, StringComparison.Ordinal));
            Assert.True(span.StartsWith(value, StringComparison.OrdinalIgnoreCase));

            CultureInfo backupCulture = CultureInfo.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("el-GR");

            span = new char[] { '\u03b1', '\u03b2', '\u03b3', '\u03b4' };  // αβγδ
            value = new char[] { '\u03b1', '\u03b2', '\u03b3' }; // αβγ

            Assert.True(span.StartsWith(value, StringComparison.CurrentCulture));
            Assert.True(span.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            value = new char[] { '\u03b1', '\u0392', '\u03b3' }; // αΒγ
            Assert.False(span.StartsWith(value, StringComparison.CurrentCulture));
            Assert.True(span.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            Thread.CurrentThread.CurrentCulture = backupCulture;

            span = new char[] { '\u0069', '\u00df', '\u0049', '\u03b4' }; // ißIδ
            value = new char[] { '\u0069', '\u0073', '\u0073', '\u0049' }; // issI

            Assert.False(span.StartsWith(value, StringComparison.Ordinal));
            Assert.True(span.StartsWith(value, StringComparison.InvariantCulture));
            Assert.True(span.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            value = new char[] { '\u0049', '\u0073', '\u0073', '\u0049' }; // IssI
            Assert.False(span.StartsWith(value, StringComparison.OrdinalIgnoreCase));
            Assert.False(span.StartsWith(value, StringComparison.InvariantCulture));
            Assert.True(span.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public static void StartsWithNoMatchNonOrdinal_StringComparison()
        {
            ReadOnlySpan<char> span = new char[] { 'a', 'b', 'c', 'd' };
            ReadOnlySpan<char> value = new char[] { 'a', 'D', 'c' };
            Assert.False(span.StartsWith(value, StringComparison.Ordinal));
            Assert.False(span.StartsWith(value, StringComparison.OrdinalIgnoreCase));

            CultureInfo backupCulture = CultureInfo.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("el-GR");

            span = new char[] { '\u03b1', '\u03b2', '\u03b3', '\u03b4' }; // αβγδ
            value = new char[] { '\u03b1', '\u03b4', '\u03b3' }; // αδγ

            Assert.False(span.StartsWith(value, StringComparison.CurrentCulture));
            Assert.False(span.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            value = new char[] { '\u03b1', '\u0394', '\u03b3' }; // αΔγ
            Assert.False(span.StartsWith(value, StringComparison.CurrentCulture));
            Assert.False(span.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            Thread.CurrentThread.CurrentCulture = backupCulture;

            span = new char[] { '\u0069', '\u00df', '\u0049', '\u03b4' }; // ißIδ
            value = new char[] { '\u0069', '\u03b4', '\u03b4', '\u0049' };  // iδδI

            Assert.False(span.StartsWith(value, StringComparison.Ordinal));
            Assert.False(span.StartsWith(value, StringComparison.InvariantCulture));
            Assert.False(span.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));

            value = new char[] { '\u0049', '\u03b4', '\u03b4', '\u0049' }; // IδδI
            Assert.False(span.StartsWith(value, StringComparison.OrdinalIgnoreCase));
            Assert.False(span.StartsWith(value, StringComparison.InvariantCulture));
            Assert.False(span.StartsWith(value, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
