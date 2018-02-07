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
        public static void ToStringForSpanOfString()
        {
            string[] a = { "a", "b", "c" };
            var span = new ReadOnlySpan<string>(a);
            Assert.Equal("System.ReadOnlySpan<String>[3]", span.ToString());
        }

        [Fact]
        public static unsafe void ToStringFromString()
        {
            string original = TestHelpers.BuildString(10, 42);
            ReadOnlySpan<char> span = original.AsReadOnlySpan();

            string returnedString = span.ToString();
            string returnedStringUsingSlice = span.Slice(0, original.Length).ToString();

            string subString1 = span.Slice(1).ToString();
            string subString2 = span.Slice(0, 2).ToString();
            string subString3 = span.Slice(1, 2).ToString();

            Assert.Equal("RDDNEGSNET", returnedString);
            Assert.Equal("RDDNEGSNET", returnedStringUsingSlice);

            Assert.Equal("DDNEGSNET", subString1);
            Assert.Equal("RD", subString2);
            Assert.Equal("DD", subString3);

            fixed (char* pOriginal = original)
            fixed (char* pString1 = returnedString)
            fixed (char* pString2 = returnedStringUsingSlice)
            {
                Assert.Equal((int)pOriginal, (int)pString1);
                Assert.Equal((int)pOriginal, (int)pString2);
            }

            fixed (char* pOriginal = original)
            fixed (char* pSubString1 = subString1)
            fixed (char* pSubString2 = subString2)
            fixed (char* pSubString3 = subString3)
            {
                Assert.NotEqual((int)pOriginal, (int)pSubString1);
                Assert.NotEqual((int)pOriginal, (int)pSubString2);
                Assert.NotEqual((int)pOriginal, (int)pSubString3);
                Assert.NotEqual((int)pSubString1, (int)pSubString2);
                Assert.NotEqual((int)pSubString1, (int)pSubString3);
                Assert.NotEqual((int)pSubString2, (int)pSubString3);
            }
        }
    }
}
