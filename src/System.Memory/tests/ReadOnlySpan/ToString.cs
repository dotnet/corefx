// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        [Fact]
        public static void ToStringInt()
        {
            int[] a = { 91, 92, 93 };
            var span = new ReadOnlySpan<int>(a);
            Assert.Equal("System.ReadOnlySpan<Int32>[3]", span.ToString());
        }

        [Fact]
        public static void ToStringInt_Empty()
        {
            var span = new ReadOnlySpan<int>();
            Assert.Equal("System.ReadOnlySpan<Int32>[0]", span.ToString());
        }

        [Fact]
        public static void ToStringChar()
        {
            char[] a = { 'a', 'b', 'c' };
            var span = new ReadOnlySpan<char>(a);
            Assert.Equal("abc", span.ToString());

            string testString = "abcdefg";
            ReadOnlySpan<char> readOnlySpan = testString.AsReadOnlySpan();
            Assert.Equal(testString, readOnlySpan.ToString());
        }

        [Fact]
        public static void ToStringChar_Empty()
        {
            var span = new ReadOnlySpan<char>();
            Assert.Equal("", span.ToString());
        }

        [Fact]
        public static unsafe void ToStringForSpanOfString()
        {
            string[] a = { "a", "b", "c" };
            var span = new ReadOnlySpan<string>(a);
            Assert.Equal("System.ReadOnlySpan<String>[3]", span.ToString());
        }

        [Fact]
        public static void ToString_SpanOverString()
        {
            string orig = "hello world";
            Assert.Equal(orig, orig.AsReadOnlySpan().ToString());
            Assert.Equal(orig.Substring(0, 5), orig.AsReadOnlySpan(0, 5).ToString());
            Assert.Equal(orig.Substring(5), orig.AsReadOnlySpan(5).ToString());
            Assert.Equal(orig.Substring(1, 3), orig.AsReadOnlySpan(1, 3).ToString());
        }

        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Optimization only applies to portable span.")]
        [Fact]
        public static void ToString_SpanOverFullString_ReturnsOriginal()
        {
            string orig = "hello world";
            Assert.Same(orig, orig.AsReadOnlySpan().ToString());
        }
    }
}
