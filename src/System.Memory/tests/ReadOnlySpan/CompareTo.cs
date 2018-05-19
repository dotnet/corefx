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
        public static void CompareToNoMatch_StringComparison()
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
                    Assert.True(0 > firstSpan.CompareTo(secondSpan, StringComparison.Ordinal));

                    // Due to differences in the implementation, the exact result of CompareTo will not necessarily match with string.Compare.
                    // However, the sign will match, which is what defines correctness.
                    Assert.Equal(
                        Math.Sign(string.Compare(firstSpan.ToString(), secondSpan.ToString(), StringComparison.OrdinalIgnoreCase)),
                        Math.Sign(firstSpan.CompareTo(secondSpan, StringComparison.OrdinalIgnoreCase)));

                    Assert.Equal(
                        string.Compare(firstSpan.ToString(), secondSpan.ToString(), StringComparison.CurrentCulture),
                        firstSpan.CompareTo(secondSpan, StringComparison.CurrentCulture));
                    Assert.Equal(
                        string.Compare(firstSpan.ToString(), secondSpan.ToString(), StringComparison.CurrentCultureIgnoreCase),
                        firstSpan.CompareTo(secondSpan, StringComparison.CurrentCultureIgnoreCase));
                    Assert.Equal(
                        string.Compare(firstSpan.ToString(), secondSpan.ToString(), StringComparison.InvariantCulture),
                        firstSpan.CompareTo(secondSpan, StringComparison.InvariantCulture));
                    Assert.Equal(
                        string.Compare(firstSpan.ToString(), secondSpan.ToString(), StringComparison.InvariantCultureIgnoreCase),
                        firstSpan.CompareTo(secondSpan, StringComparison.InvariantCultureIgnoreCase));
                }
            }
        }

        [Fact]
        public static void MakeSureNoCompareToChecksGoOutOfRange_StringComparison()
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
                Assert.Equal(0, span1.CompareTo(span2, StringComparison.Ordinal));

                Assert.Equal(0, span1.CompareTo(span2, StringComparison.CurrentCulture));
                Assert.Equal(0, span1.CompareTo(span2, StringComparison.CurrentCultureIgnoreCase));
                Assert.Equal(0, span1.CompareTo(span2, StringComparison.InvariantCulture));
                Assert.Equal(0, span1.CompareTo(span2, StringComparison.InvariantCultureIgnoreCase));
                Assert.Equal(0, span1.CompareTo(span2, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public static void CompareToUnknownComparisonType_StringComparison()
        {
            char[] a = { '4', '5', '6' };
            var span = new ReadOnlySpan<char>(a);
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.CompareTo(_span, StringComparison.CurrentCulture - 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.CompareTo(_span, StringComparison.OrdinalIgnoreCase + 1));
            TestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.CompareTo(_span, (StringComparison)6));
        }

    }
}
