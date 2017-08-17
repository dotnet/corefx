// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Encodings.Tests
{
    public partial class EncoderTests
    {
        [Fact]
        public static void GetByteCount_Span_MatchesEncodingByteCount()
        {
            const string Text = "hello world";
            Encoding e = Encoding.UTF8;
            Assert.Equal(e.GetByteCount(Text), e.GetEncoder().GetByteCount(Text.AsReadOnlySpan(), flush: true));
        }

        [Fact]
        public static void GetBytes_Span_MatchesEncodingGetBytes()
        {
            const string Text = "hello world";
            Encoding e = Encoding.UTF8;

            byte[] bytes = new byte[e.GetByteCount(Text)];
            Assert.Equal(bytes.Length, e.GetEncoder().GetBytes(Text.AsReadOnlySpan(), bytes, flush: true));
            Assert.Equal(e.GetBytes(Text), bytes);
        }

        [Fact]
        public static void Convert_Span_MatchesGetBytes()
        {
            const string Text = "hello world";
            Encoding encoding = Encoding.UTF8;
            Encoder encoder = encoding.GetEncoder();
            byte[] bytes;

            bytes = new byte[encoding.GetByteCount(Text)];
            encoder.Convert(Text.AsReadOnlySpan(), bytes.AsSpan().Slice(0, 2), true, out int charsUsed, out int bytesUsed, out bool completed);
            Assert.Equal(encoding.GetBytes(Text).AsSpan().Slice(0, 2).ToArray(), bytes.AsSpan().Slice(0, 2).ToArray());
            Assert.Equal(2, charsUsed);
            Assert.Equal(2, bytesUsed);
            Assert.False(completed);

            bytes = new byte[encoding.GetByteCount(Text)];
            encoder.Convert(Text.AsReadOnlySpan(), bytes, true, out charsUsed, out bytesUsed, out completed);
            Assert.Equal(encoding.GetBytes(Text), bytes);
            Assert.Equal(Text.Length, charsUsed);
            Assert.Equal(bytes.Length, bytesUsed);
            Assert.True(completed);
        }
    }
}
