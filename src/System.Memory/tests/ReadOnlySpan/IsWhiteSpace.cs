// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        private static readonly char[] s_whiteSpaceCharacters = { (char)9, (char)10, (char)11, (char)12, (char)13, (char)32, (char)133, (char)160, (char)5760 };

        [Fact]
        public static void ZeroLengthIsWhiteSpace()
        {
            ReadOnlySpan<char> span = new ReadOnlySpan<char>(Array.Empty<char>());
            Assert.True(span.IsWhiteSpace());
        }

        [Fact]
        public static void IsWhiteSpaceTrue()
        {
            Random rand = new Random(42);
            for (int length = 0; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length)];
                }
                ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                Assert.True(span.IsWhiteSpace());
            }
        }

        [Fact]
        public static void IsWhiteSpaceFalse()
        {
            Random rand = new Random(42);
            for (int length = 1; length < 32; length++)
            {
                char[] a = new char[length];
                
                // first character is not a white-space character
                {
                    for (int i = 0; i < length; i++)
                    {
                        a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length)];
                    }
                    a[0] = 'a';
                    ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                    Assert.False(span.IsWhiteSpace());
                }

                // last character is not a white-space character
                {
                    for (int i = 0; i < length; i++)
                    {
                        a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length)];
                    }
                    a[length - 1] = 'a';
                    ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                    Assert.False(span.IsWhiteSpace());
                }

                // character in the middle is not a white-space character
                {
                    for (int i = 0; i < length; i++)
                    {
                        a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length)];
                    }
                    a[length/2] = 'a';
                    ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                    Assert.False(span.IsWhiteSpace());
                }

                // no character is a white-space character
                {
                    for (int i = 0; i < length; i++)
                    {
                        a[i] = 'a';
                    }
                    ReadOnlySpan<char> span = new ReadOnlySpan<char>(a);
                    Assert.False(span.IsWhiteSpace());
                }

            }
        }

        [Fact]
        public static void MakeSureNoIsWhiteSpaceChecksGoOutOfRange()
        {
            for (int length = 3; length < 64; length++)
            {
                char[] first = new char[length];
                first[0] = ' ';
                first[length - 1] = ' ';
                var span = new ReadOnlySpan<char>(first, 1, length - 2);
                Assert.False(span.IsWhiteSpace());
            }
        }
    }
}
