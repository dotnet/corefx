// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.Tests
{
    //When add new tests make sure to add checks for both string and span APIs where relevant.
    public partial class StringTests : RemoteExecutorTestBase
    {        
        [Fact]
        public static void ZeroLengthContains_StringComparison()
        {
            var a = new char[3];

            //this Contains overload doesn't exist on netfx
            string s1 = new string(a);
            string s2 = new string(a, 2, 0);
            Assert.True(s1.Contains(s2, StringComparison.Ordinal));

            Assert.True(s1.Contains(s2, StringComparison.CurrentCulture));
            Assert.True(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCulture));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

            s1 = string.Empty;
            Assert.True(s1.Contains(s2, StringComparison.Ordinal));

            Assert.True(s1.Contains(s2, StringComparison.CurrentCulture));
            Assert.True(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCulture));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

            var span = new ReadOnlySpan<char>(a);
            var emptySlice = new ReadOnlySpan<char>(a, 2, 0);
            Assert.True(span.Contains(emptySlice, StringComparison.Ordinal));

            Assert.True(span.Contains(emptySlice, StringComparison.CurrentCulture));
            Assert.True(span.Contains(emptySlice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Contains(emptySlice, StringComparison.InvariantCulture));
            Assert.True(span.Contains(emptySlice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Contains(emptySlice, StringComparison.OrdinalIgnoreCase));

            span = ReadOnlySpan<char>.Empty;
            Assert.True(span.Contains(emptySlice, StringComparison.Ordinal));

            Assert.True(span.Contains(emptySlice, StringComparison.CurrentCulture));
            Assert.True(span.Contains(emptySlice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Contains(emptySlice, StringComparison.InvariantCulture));
            Assert.True(span.Contains(emptySlice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Contains(emptySlice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void SameSpanContains_StringComparison()
        {
            string s1 = "456";

            //this Contains overload doesn't exist on netfx
            Assert.True(s1.Contains(s1, StringComparison.Ordinal));

            Assert.True(s1.Contains(s1, StringComparison.CurrentCulture));
            Assert.True(s1.Contains(s1, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(s1.Contains(s1, StringComparison.InvariantCulture));
            Assert.True(s1.Contains(s1, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(s1.Contains(s1, StringComparison.OrdinalIgnoreCase));

            ReadOnlySpan<char> span = s1.AsSpan();
            Assert.True(span.Contains(span, StringComparison.Ordinal));

            Assert.True(span.Contains(span, StringComparison.CurrentCulture));
            Assert.True(span.Contains(span, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Contains(span, StringComparison.InvariantCulture));
            Assert.True(span.Contains(span, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Contains(span, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void LengthMismatchContains_StringComparison()
        {
            string value = "456";

            //this Contains overload doesn't exist on netfx
            string s1 = value.Substring(0, 2);
            string s2 = value.Substring(0, 3);
            Assert.False(s1.Contains(s2, StringComparison.Ordinal));

            Assert.False(s1.Contains(s2, StringComparison.CurrentCulture));
            Assert.False(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
            Assert.False(s1.Contains(s2, StringComparison.InvariantCulture));
            Assert.False(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
            Assert.False(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

            ReadOnlySpan<char> span = value.AsSpan(0, 2);
            ReadOnlySpan<char> slice = value.AsSpan(0, 3);
            Assert.False(span.Contains(slice, StringComparison.Ordinal));

            Assert.False(span.Contains(slice, StringComparison.CurrentCulture));
            Assert.False(span.Contains(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.False(span.Contains(slice, StringComparison.InvariantCulture));
            Assert.False(span.Contains(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.False(span.Contains(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void ContainsMatch_StringComparison()
        {
            string value = "456";

            //this Contains overload doesn't exist on netfx
            string s1 = value.Substring(0, 3);
            string s2 = value.Substring(0, 2);
            Assert.True(s1.Contains(s2, StringComparison.Ordinal));

            Assert.True(s1.Contains(s2, StringComparison.CurrentCulture));
            Assert.True(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCulture));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

            ReadOnlySpan<char> span = value.AsSpan(0, 3);
            ReadOnlySpan<char> slice = value.AsSpan(0, 2);
            Assert.True(span.Contains(slice, StringComparison.Ordinal));

            Assert.True(span.Contains(slice, StringComparison.CurrentCulture));
            Assert.True(span.Contains(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Contains(slice, StringComparison.InvariantCulture));
            Assert.True(span.Contains(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Contains(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void ContainsMatchDifferentSpans_StringComparison()
        {
            string value1 = "4567";
            string value2 = "456";

            //this Contains overload doesn't exist on netfx
            string s1 = value1.Substring(0, 3);
            string s2 = value2.Substring(0, 3);
            Assert.True(s1.Contains(s2, StringComparison.Ordinal));

            Assert.True(s1.Contains(s2, StringComparison.CurrentCulture));
            Assert.True(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCulture));
            Assert.True(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

            ReadOnlySpan<char> span = value1.AsSpan(0, 3);
            ReadOnlySpan<char> slice = value2.AsSpan(0, 3);
            Assert.True(span.Contains(slice, StringComparison.Ordinal));

            Assert.True(span.Contains(slice, StringComparison.CurrentCulture));
            Assert.True(span.Contains(slice, StringComparison.CurrentCultureIgnoreCase));
            Assert.True(span.Contains(slice, StringComparison.InvariantCulture));
            Assert.True(span.Contains(slice, StringComparison.InvariantCultureIgnoreCase));
            Assert.True(span.Contains(slice, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public static void ContainsNoMatch_StringComparison()
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

                    //this Contains overload doesn't exist on netfx
                    string s1 = new string(first);
                    string s2 = new string(second);
                    Assert.False(s1.Contains(s2, StringComparison.Ordinal));

                    Assert.False(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

                    // Different behavior depending on OS	
                    Assert.Equal(
                        s1.ToString().StartsWith(s2.ToString(), StringComparison.CurrentCulture),
                        s1.Contains(s2, StringComparison.CurrentCulture));
                    Assert.Equal(
                        s1.ToString().StartsWith(s2.ToString(), StringComparison.CurrentCulture),
                        s1.Contains(s2, StringComparison.CurrentCulture));
                    Assert.Equal(
                        s1.ToString().StartsWith(s2.ToString(), StringComparison.InvariantCulture),
                        s1.Contains(s2, StringComparison.InvariantCulture));
                    Assert.Equal(
                        s1.ToString().StartsWith(s2.ToString(), StringComparison.InvariantCultureIgnoreCase),
                        s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));

                    var firstSpan = new ReadOnlySpan<char>(first);
                    var secondSpan = new ReadOnlySpan<char>(second);
                    Assert.False(firstSpan.Contains(secondSpan, StringComparison.Ordinal));

                    Assert.False(firstSpan.Contains(secondSpan, StringComparison.OrdinalIgnoreCase));

                    // Different behavior depending on OS
                    Assert.Equal(
                        firstSpan.ToString().StartsWith(secondSpan.ToString(), StringComparison.CurrentCulture),
                        firstSpan.Contains(secondSpan, StringComparison.CurrentCulture));
                    Assert.Equal(
                        firstSpan.ToString().StartsWith(secondSpan.ToString(), StringComparison.CurrentCulture),
                        firstSpan.Contains(secondSpan, StringComparison.CurrentCulture));
                    Assert.Equal(
                        firstSpan.ToString().StartsWith(secondSpan.ToString(), StringComparison.InvariantCulture),
                        firstSpan.Contains(secondSpan, StringComparison.InvariantCulture));
                    Assert.Equal(
                        firstSpan.ToString().StartsWith(secondSpan.ToString(), StringComparison.InvariantCultureIgnoreCase),
                        firstSpan.Contains(secondSpan, StringComparison.InvariantCultureIgnoreCase));
                }
            }
        }

        [Fact]
        public static void MakeSureNoContainsChecksGoOutOfRange_StringComparison()
        {
            for (int length = 0; length < 100; length++)
            {
                var first = new char[length + 2];
                first[0] = (char)99;
                first[length + 1] = (char)99;
                var second = new char[length + 2];
                second[0] = (char)100;
                second[length + 1] = (char)100;

                //this Contains overload doesn't exist on netfx
                string s1 = new string(first, 1, length);
                string s2 = new string(second, 1, length);
                Assert.True(s1.Contains(s2, StringComparison.Ordinal));

                Assert.True(s1.Contains(s2, StringComparison.CurrentCulture));
                Assert.True(s1.Contains(s2, StringComparison.CurrentCultureIgnoreCase));
                Assert.True(s1.Contains(s2, StringComparison.InvariantCulture));
                Assert.True(s1.Contains(s2, StringComparison.InvariantCultureIgnoreCase));
                Assert.True(s1.Contains(s2, StringComparison.OrdinalIgnoreCase));

                var span1 = new ReadOnlySpan<char>(first, 1, length);
                var span2 = new ReadOnlySpan<char>(second, 1, length);
                Assert.True(span1.Contains(span2, StringComparison.Ordinal));

                Assert.True(span1.Contains(span2, StringComparison.CurrentCulture));
                Assert.True(span1.Contains(span2, StringComparison.CurrentCultureIgnoreCase));
                Assert.True(span1.Contains(span2, StringComparison.InvariantCulture));
                Assert.True(span1.Contains(span2, StringComparison.InvariantCultureIgnoreCase));
                Assert.True(span1.Contains(span2, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public static void ContainsUnknownComparisonType_StringComparison()
        {
            string s = "456";

            //this Contains overload doesn't exist on netfx
            Assert.Throws<ArgumentException>(() => s.Contains(s, StringComparison.CurrentCulture - 1));
            Assert.Throws<ArgumentException>(() => s.Contains(s, StringComparison.OrdinalIgnoreCase + 1));
            Assert.Throws<ArgumentException>(() => s.Contains(s, (StringComparison)6));

            ReadOnlySpan<char> span = s.AsSpan();
            SpanTestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Contains(_span, StringComparison.CurrentCulture - 1));
            SpanTestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Contains(_span, StringComparison.OrdinalIgnoreCase + 1));
            SpanTestHelpers.AssertThrows<ArgumentException, char>(span, (_span) => _span.Contains(_span, (StringComparison)6));
        }
    }
}
