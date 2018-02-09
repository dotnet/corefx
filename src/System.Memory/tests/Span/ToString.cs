// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ToStringInt()
        {
            int[] a = { 91, 92, 93 };
            var span = new Span<int>(a);
            Assert.Equal("System.Span<Int32>[3]", span.ToString());
        }

        [Fact]
        public static void ToStringInt_Empty()
        {
            var span = new Span<int>();
            Assert.Equal("System.Span<Int32>[0]", span.ToString());
        }

        [Fact]
        public static unsafe void ToStringChar()
        {
            char[] a = { 'a', 'b', 'c' };
            var span = new Span<char>(a);
            Assert.Equal("abc", span.ToString());

            string testString = "abcdefg";
            ReadOnlySpan<char> readOnlySpan = testString.AsReadOnlySpan();

            fixed (void* ptr = &MemoryMarshal.GetReference(readOnlySpan))
            {
                var temp = new Span<char>(ptr, readOnlySpan.Length);
                Assert.Equal(testString, temp.ToString());
            }
        }

        [Fact]
        public static void ToStringChar_Empty()
        {
            var span = new Span<char>();
            Assert.Equal("", span.ToString());
        }

        [Fact]
        public static void ToStringForSpanOfString()
        {
            string[] a = { "a", "b", "c" };
            var span = new Span<string>(a);
            Assert.Equal("System.Span<String>[3]", span.ToString());
        }

        // This test is only relevant for portable span
#if FEATURE_PORTABLE_SPAN
        [Fact]
#endif
        public static unsafe void ToStringFromString()
        {
            string original = TestHelpers.BuildString(10, 42);

            ReadOnlyMemory<char> readOnlyMemory = original.AsReadOnlyMemory();
            Memory<char> memory = MemoryMarshal.AsMemory(readOnlyMemory);

            Span<char> span = memory.Span;

            string returnedString = span.ToString();
            string returnedStringUsingSlice = span.Slice(0, original.Length).ToString();

            string subString1 = span.Slice(1).ToString();
            string subString2 = span.Slice(0, 2).ToString();
            string subString3 = span.Slice(1, 2).ToString();

            Assert.Equal(original, returnedString);
            Assert.Equal(original, returnedStringUsingSlice);

            Assert.Equal(original.Substring(1), subString1);
            Assert.Equal(original.Substring(0, 2), subString2);
            Assert.Equal(original.Substring(1, 2), subString3);

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
