// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace System.IO.Tests
{
    public class StreamReader_ctorTests
    {
        [Fact]
        public static void StreamReaderNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamReader((Stream)null, true));
        }

        [Fact]
        public static void InputStreamClosed()
        {
            var ms2 = new MemoryStream();
            ms2.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => new StreamReader(ms2, false));
        }

        [Fact]
        public static void CreationFromMemoryStreamWithEncodingFalse()
        {
            var ms2 = new MemoryStream();
            ms2.Write(new byte[] { 65, 66, 67, 68 }, 0, 4);
            ms2.Position = 0;
            var sr2 = new StreamReader(ms2, false);

            Assert.Equal("ABCD", sr2.ReadToEnd());
            sr2.Dispose();
        }

        [Fact]
        public static void CreationFromMemoryStreamWithEncodingTrue()
        {
            var ms2 = new MemoryStream();
            ms2.Write(new byte[] { 65, 66, 67, 68 }, 0, 4);
            ms2.Position = 0;
            var sr2 = new StreamReader(ms2, true);

            Assert.Equal("ABCD", sr2.ReadToEnd());
            sr2.Dispose();
        }

        public static IEnumerable<object[]> EncodingsToTestDetection()
        {
            yield return new object[] { Encoding.BigEndianUnicode };
            yield return new object[] { Encoding.Unicode };
            yield return new object[] { Encoding.UTF32 };
            yield return new object[] { Encoding.UTF8 };

            var bigEndianUTF32 = new UTF32Encoding(bigEndian: true, byteOrderMark: true);
            yield return new object[] { bigEndianUTF32 };
        }

        [Theory]
        [MemberData(nameof(EncodingsToTestDetection))]
        public static void CreationFromMemoryStreamWithEncodingTrueDetectsCorrectEncoding(Encoding expectedEncoding)
        {
            var ms2 = new MemoryStream();
            var encodedBytesWithPreamble = expectedEncoding.GetPreamble().Concat(expectedEncoding.GetBytes("ABCD")).ToArray();
            ms2.Write(encodedBytesWithPreamble);
            ms2.Position = 0;
            var sr2 = new StreamReader(ms2, true);
            var streamContent = sr2.ReadToEnd();

            Assert.Equal("ABCD", streamContent);
            Assert.Equal(expectedEncoding, sr2.CurrentEncoding);
            sr2.Dispose();
        }
    }
}
