// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public partial class TextWriterTests
    {
        [Fact]
        public void WriteCharSpanTest()
        {
            using (CharArrayTextWriter tw = NewTextWriter)
            {
                var rs = new ReadOnlySpan<char>(TestDataProvider.CharData, 4, 6);
                tw.Write(rs);
                Assert.Equal(new string(rs), tw.Text);
            }
        }

        [Fact]
        public void WriteLineCharSpanTest()
        {
            using (CharArrayTextWriter tw = NewTextWriter)
            {
                var rs = new ReadOnlySpan<char>(TestDataProvider.CharData, 4, 6);
                tw.WriteLine(rs);
                Assert.Equal(new string(rs) + tw.NewLine, tw.Text);
            }
        }

        [Fact]
        public async Task WriteCharMemoryTest()
        {
            using (CharArrayTextWriter tw = NewTextWriter)
            {
                var rs = new Memory<char>(TestDataProvider.CharData, 4, 6);
                await tw.WriteAsync(rs);
                Assert.Equal(new string(rs.Span), tw.Text);
            }
        }

        [Fact]
        public async Task WriteLineCharMemoryTest()
        {
            using (CharArrayTextWriter tw = NewTextWriter)
            {
                var rs = new Memory<char>(TestDataProvider.CharData, 4, 6);
                await tw.WriteLineAsync(rs);
                Assert.Equal(new string(rs.Span) + tw.NewLine, tw.Text);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteStringBuilderTest(bool isSynchronized)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                tw.Write(new StringBuilder(new string(TestDataProvider.CharData)));
                tw.Flush();
                Assert.Equal(new string(TestDataProvider.CharData), ctw.Text);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteLineStringBuilderTest(bool isSynchronized)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                tw.Write(new StringBuilder(new string(TestDataProvider.CharData)));
                tw.Flush();
                Assert.Equal(new string(TestDataProvider.CharData), ctw.Text);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]

        public async void WriteAsyncStringBuilderTest(bool isSynchronized)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                await tw.WriteAsync(new StringBuilder(new string(TestDataProvider.CharData)));
                tw.Flush();
                Assert.Equal(new string(TestDataProvider.CharData), ctw.Text);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void WriteLineAsyncStringBuilderTest(bool isSynchronized)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                await tw.WriteLineAsync(new StringBuilder(new string(TestDataProvider.CharData)));
                tw.Flush();
                Assert.Equal(new string(TestDataProvider.CharData) + tw.NewLine, ctw.Text);
            }
        }
    }
}
