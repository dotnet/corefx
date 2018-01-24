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
            var span = new ReadOnlySpan<char>(Array.Empty<char>());
            Assert.True(span.IsWhiteSpace());
        }

        [Fact]
        public static void IsWhiteSpaceTrueLatin1()
        {
            Random rand = new Random(42);
            for (int length = 0; length < 32; length++)
            {
                char[] a = new char[length];
                for (int i = 0; i < length; i++)
                {
                    a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length - 1)];
                }
                var span = new Span<char>(a);
                Assert.True(span.AsReadOnlySpan().IsWhiteSpace());

                for (int i = 0; i < s_whiteSpaceCharacters.Length - 1; i++)
                {
                    span.Fill(s_whiteSpaceCharacters[i]);
                    Assert.True(span.AsReadOnlySpan().IsWhiteSpace());
                }
            }
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
                var span = new ReadOnlySpan<char>(a);
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
                for (int i = 0; i < length; i++)
                {
                    a[i] = s_whiteSpaceCharacters[rand.Next(0, s_whiteSpaceCharacters.Length)];
                }
                var span = new Span<char>(a);

                // first character is not a white-space character
                a[0] = 'a';
                Assert.False(span.AsReadOnlySpan().IsWhiteSpace());
                a[0] = ' ';

                // last character is not a white-space character
                a[length - 1] = 'a';
                Assert.False(span.AsReadOnlySpan().IsWhiteSpace());
                a[length - 1] = ' ';

                // character in the middle is not a white-space character
                a[length/2] = 'a';
                Assert.False(span.AsReadOnlySpan().IsWhiteSpace());
                a[length/2] = ' ';

                // no character is a white-space character
                span.Fill('a');
                Assert.False(span.AsReadOnlySpan().IsWhiteSpace());
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
