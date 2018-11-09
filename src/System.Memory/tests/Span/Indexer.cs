// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Xunit;

namespace System.SpanTests
{
    public static partial class IndexerTests
    {
        [Fact]
        public static void IndexerWithIndexTest()
        {
            ReadOnlySpan<char> span = "Hello".AsSpan();
            Assert.Equal('H', span[new Index(0, fromEnd: false)]);
            Assert.Equal('o', span[new Index(1, fromEnd: true)]);
            Assert.Equal(span[new Index(2, fromEnd: false)], span[new Index(span.Length - 2, fromEnd: true)]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                char c;
                ReadOnlySpan<char> s = "Hello".AsSpan();
                c = s[new Index(0, fromEnd: true)];
                return;
            });

            Span<char> span1 = new Span<char>(new char [] { 'H', 'e', 'l', 'l', 'o'});
            Assert.Equal('e', span1[new Index(1, fromEnd: false)]);
            Assert.Equal('l', span1[new Index(2, fromEnd: true)]);
            Assert.Equal(span1[new Index(2, fromEnd: false)], span1[new Index(span.Length - 2, fromEnd: true)]);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                char c;
                Span<char> s = new Span<char>(new char [] { 'H', 'e', 'l', 'l', 'o'});
                c = s[new Index(0, fromEnd: true)];
                return;
            });
        }

        [Fact]
        public static void IndexerWithRangeTest()
        {
            ReadOnlySpan<char> span = "Hello".AsSpan();
            ReadOnlySpan<char> sliced = span[Range.Create(new Index(1, fromEnd: false), new Index(1, fromEnd: true))];
            Assert.True(span.Slice(1, 3) == sliced);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                ReadOnlySpan<char> s = "Hello".AsSpan();
                // Passing wrong start and end to the Range
                ReadOnlySpan<char> s1 = s[Range.Create(new Index(1, fromEnd: true), new Index(1, fromEnd: false))];
                return;
            });

            Span<char> span1 = new Span<char>(new char [] { 'H', 'e', 'l', 'l', 'o'});
            Span<char> sliced1 = span1[Range.Create(new Index(2, fromEnd: false), new Index(1, fromEnd: true))];
            Assert.True(span1.Slice(2, 2) == sliced1);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Span<char> s = new Span<char>(new char [] { 'H', 'e', 'l', 'l', 'o'});
                // Passing wrong start and end to the Range
                Span<char> s1 = s[Range.Create(new Index(0, fromEnd: true), new Index(1, fromEnd: false))];
                return;
            });
        }
    }
}