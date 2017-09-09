// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public partial class WriteTests
    {
        [Fact]
        public void WriteReadOnlySpanTest()
        {
            char[] chArr = setupArray();
            ReadOnlySpan<char> readSpan = new ReadOnlySpan<char>(chArr);

            Stream ms = CreateStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr;

            sw.Write(readSpan);
            sw.Flush();
            ms.Position = 0;
            sr = new StreamReader(ms);

            for (int i = 0; i < chArr.Length; i++)
            {
                Assert.Equal((int)chArr[i], sr.Read());
            }
            ms.Dispose();
        }

        [Fact]
        public void WriteLineReadOnlySpanTest()
        {
            char[] chArr = setupArray();
            ReadOnlySpan<char> readSpan = new ReadOnlySpan<char>(chArr);

            Stream ms = CreateStream();
            StreamWriter sw = new StreamWriter(ms);
            StreamReader sr;

            sw.Write(readSpan);
            sw.Flush();
            ms.Position = 0;
            sr = new StreamReader(ms);

            string readData = sr.ReadLine();
            Assert.Equal(new string(chArr), readData);

            ms.Dispose();
        }
    }
}
