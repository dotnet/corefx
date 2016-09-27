// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using Xunit;

namespace System.IO.Tests
{
    public class StreamReader_StringCtorTests
    {
        [Fact]
        public static void NullArgs_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("path", () => new StreamReader((string)null));
            Assert.Throws<ArgumentNullException>("encoding", () => new StreamReader("path", null, true));
        }

        [Fact]
        public static void EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new StreamReader(""));
        }

        [Fact]
        public static void NegativeBufferSize_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("bufferSize", () => new StreamReader("path", Encoding.UTF8, true, -1));
        }

        [Fact]
        public static void ReadToEnd_False_detectEncodingFromByteOrderMarks()
        {
            string testfile = Path.GetTempFileName();
            File.WriteAllBytes(testfile, new byte[] { 65, 66, 67, 68 });
            using (var sr2 = new StreamReader(testfile, false))
            {
                Assert.Equal("ABCD", sr2.ReadToEnd());
            }
            File.Delete(testfile);
        }

        [Fact]
        public static void ReadToEnd_True_detectEncodingFromByteOrderMarks()
        {
            string testfile = Path.GetTempFileName();
            File.WriteAllBytes(testfile, new byte[] { 65, 66, 67, 68 });
            using (var sr2 = new StreamReader(testfile, true))
            {
                Assert.Equal("ABCD", sr2.ReadToEnd());
            }
            File.Delete(testfile);
        }
    }
}
