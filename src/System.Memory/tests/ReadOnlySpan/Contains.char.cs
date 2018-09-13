// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    // Adapted from IndexOf.byte.cs
    public static partial class ReadOnlySpanTests // .Contains<Char>
    {
        [Fact]
        public static void ZeroLengthContains_Char()
        {
            ReadOnlySpan<char> sp = new ReadOnlySpan<char>(Array.Empty<char>());
            bool found = sp.Contains((char)0);
            Assert.False(found);
        }

        [Fact]
        public static void TestContains_Char()
        {
            for (int length = 0; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }
                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);

                for (int targetIndex = 0; targetIndex < length; targetIndex++)
                {
                    char target = a[targetIndex];
                    bool found = span.Contains(target);
                    Assert.True(found);
                }
            }
        }

        [Fact]
        public static void TestMultipleContains_Char()
        {
            for (int length = 2; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = (char)(i + 1);
                }

                a[length - 1] = (char)200;
                a[length - 2] = (char)200;

                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                bool found = span.Contains((char)200);
                Assert.True(found);
            }
        }

        [Fact]
        public static void MakeSureNoChecksForContainsGoOutOfRange_Char()
        {
            for (int length = 0; length < 100; length++)
            {
                char[] a = new char[length + 2];
                a[0] = '9';
                a[length + 1] = '9';
                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a, 1, length);
                bool found = span.Contains('9');
                Assert.False(found);
            }
        }
    }
}
