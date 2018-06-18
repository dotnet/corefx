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
        [MemberData(nameof(GetStringBuilderTestData))]
        public void WriteStringBuilderTest(bool isSynchronized, string testData)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                tw.Write(new StringBuilder(testData));
                tw.Flush();
                Assert.Equal(testData, ctw.Text);
            }
        }

        [Theory]
        [MemberData(nameof(GetStringBuilderTestData))]
        public void WriteLineStringBuilderTest(bool isSynchronized, string testData)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                tw.WriteLine(new StringBuilder(new string(testData)));
                tw.Flush();
                Assert.Equal(testData + tw.NewLine, ctw.Text);
            }
        }

        [Theory]
        [MemberData(nameof(GetStringBuilderTestData))]
        public async void WriteAsyncStringBuilderTest(bool isSynchronized, string testData)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                await tw.WriteAsync(new StringBuilder(testData));
                tw.Flush();
                Assert.Equal(testData, ctw.Text);
            }
        }

        [Theory]
        [MemberData(nameof(GetStringBuilderTestData))]
        public async void WriteLineAsyncStringBuilderTest(bool isSynchronized, string testData)
        {
            using (CharArrayTextWriter ctw = NewTextWriter)
            {
                TextWriter tw = isSynchronized ? TextWriter.Synchronized(ctw) : ctw;
                await tw.WriteLineAsync(new StringBuilder(testData));
                tw.Flush();
                Assert.Equal(testData + tw.NewLine, ctw.Text);
            }
        }

        // Generate data for TextWriter.Write* methods that take a stringBuilder.  
        // We test both the synchronized and unsynchronized variation, on strinbuilder swith 0, small and large values.    
        public static IEnumerable<object[]> GetStringBuilderTestData()
        {
            foreach (string testData in new string[] { "", new string(TestDataProvider.CharData), new string(TestDataProvider.LargeData) })
            {
                foreach (bool isSynchronized in new bool[] { true, false })
                    yield return new object[] { isSynchronized, testData };
            }
        }
    }
}
