// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Encodings.Tests
{
    public partial class DecoderTests
    {
        [Fact]
        public static void GetCharCount_Span_MatchesEncodingCharCount()
        {
            const string Text = "hello world";
            Encoding e = Encoding.UTF8;
            byte[] textBytes = e.GetBytes(Text);

            Assert.Equal(Text.Length, e.GetDecoder().GetCharCount(textBytes.AsSpan(), flush: true));
        }

        [Fact]
        public static void GetChars_Span_MatchesEncodingGetChars()
        {
            const string Text = "hello world";
            Encoding e = Encoding.UTF8;
            byte[] textBytes = e.GetBytes(Text);

            char[] chars = new char[Text.Length];
            Assert.Equal(chars.Length, e.GetDecoder().GetChars(textBytes.AsReadOnlySpan(), chars.AsSpan(), flush: true));
            Assert.Equal(Text, new string(chars));
        }

        [Fact]
        public static void Convert_Span_MatchesGetChars()
        {
            const string Text = "hello world";
            Encoding e = Encoding.UTF8;
            Decoder decoder = e.GetDecoder();
            byte[] textBytes = e.GetBytes(Text);
            char[] chars;

            chars = new char[Text.Length];
            decoder.Convert(textBytes.AsSpan(), chars.AsSpan().Slice(0, 2), true, out int bytesUsed, out int charsUsed, out bool completed);
            Assert.Equal("he", new string(chars, 0, 2));
            Assert.Equal(2, bytesUsed);
            Assert.Equal(2, charsUsed);
            Assert.False(completed);

            chars = new char[Text.Length];
            decoder.Convert(textBytes.AsSpan(), chars.AsSpan(), true, out bytesUsed, out charsUsed, out completed);
            Assert.Equal(Text, new string(chars));
            Assert.Equal(textBytes.Length, bytesUsed);
            Assert.Equal(Text.Length, charsUsed);
            Assert.True(completed);
        }
    }
}
