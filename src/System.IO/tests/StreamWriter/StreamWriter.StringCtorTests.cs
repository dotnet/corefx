// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace System.IO.Tests
{
    public class StreamWriter_StringCtorTests
    {
        [Fact]
        public static void NullArgs_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("path", () => new StreamWriter((string)null));
            Assert.Throws<ArgumentNullException>("encoding", () => new StreamWriter("path", false, null));
        }

        [Fact]
        public static void EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StreamWriter(""));
        }

        [Fact]
        public static void NegativeBufferSize_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("bufferSize", () => new StreamWriter("path", false, Encoding.UTF8, -1));
        }

        [Fact]
        public static void CreateStreamWriter()
        {
            string testfile = Path.GetTempFileName();
            string testString = "Hello World!";
            using (StreamWriter writer = new StreamWriter(testfile))
            {
                writer.Write(testString);
            }

            using (StreamReader reader = new StreamReader(testfile))
            {
                Assert.Equal(testString, reader.ReadToEnd());
            }
            File.Delete(testfile);
        }

        public static IEnumerable<object[]> EncodingsToTestStreamWriter()
        {
            yield return new object[] { Encoding.UTF8, "This is UTF8\u00FF" };
            yield return new object[] { Encoding.BigEndianUnicode, "This is BigEndianUnicode\u00FF" };
            yield return new object[] { Encoding.Unicode, "This is Unicode\u00FF" };
        }

        [Theory]
        [MemberData(nameof(EncodingsToTestStreamWriter))]
        public static void TestEncoding(Encoding encoding, string testString)
        {
            string testfile = Path.GetTempFileName();
            using (StreamWriter writer = new StreamWriter(testfile, false, encoding))
            {
                writer.Write(testString);
            }

            using (StreamReader reader = new StreamReader(testfile, encoding))
            {
                Assert.Equal(testString, reader.ReadToEnd());
            }
            File.Delete(testfile);
        }
    }
}
