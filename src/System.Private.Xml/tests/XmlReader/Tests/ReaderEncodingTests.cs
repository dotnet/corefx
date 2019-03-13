// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Xml.Tests
{
    public class ReaderEncodingTests
    {
        [Fact]
        public static void ReadWithSurrogateCharAndInvalidChar()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 0, 34, 1, 0, 62, 100, 60, 47, 97, 62, 10 };
            var reader = XmlReader.Create(new MemoryStream(bytes));

            XmlException ex = Assert.Throws<XmlException>(() => reader.Read());
            Assert.Equal("Invalid character in the given encoding. Line 1, position 4.", ex.Message);
            Assert.Equal(4, ex.LinePosition);
        }

        [Fact]
        public static void ReadWithNormalCharAndInvalidChar()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 65, 0, 0, 0,62, 100, 60, 47, 97, 62, 10 };
            var reader = XmlReader.Create(new MemoryStream(bytes));

            XmlException ex = Assert.Throws<XmlException>(() => reader.Read());
            Assert.Equal("Invalid character in the given encoding. Line 1, position 3.", ex.Message);
            Assert.Equal(3, ex.LinePosition);
        }

        [Fact]
        public static void ReadWithSurrogateCharAndInvalidChar_ValidXmlStructure()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 97, 0, 0, 0, 62, 0, 0, 0, 0, 34, 1, 0, 62, 100, 60, 47, 60, 0, 0, 0, 47, 0, 0, 0, 97, 0, 0, 0, 62, 0, 0, 0 };
            var reader = XmlReader.Create(new MemoryStream(bytes));

            XmlException ex = Assert.Throws<XmlException>(() => reader.Read());
            Assert.Equal("Invalid character in the given encoding. Line 1, position 6.", ex.Message);
            Assert.Equal(6, ex.LinePosition);
        }
        [Fact]
        public static void ReadWithSurrogateCharAsElementName()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 0, 34, 1, 0, 65, 0, 0, 0, 97, 0, 0, 0 };
            var reader = XmlReader.Create(new MemoryStream(bytes));

            XmlException ex = Assert.Throws<XmlException>(() => reader.Read());
            Assert.Equal(2, ex.LinePosition);
        }

        [Fact]
        public static void ReadWithSurrogateChar_ValidXmlStructure()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 97, 0, 0, 0, 62, 0, 0, 0, 0, 34, 1, 0, 60, 0, 0, 0, 47, 0, 0, 0, 97, 0, 0, 0, 62, 0, 0, 0 };
            var reader = XmlReader.Create(new MemoryStream(bytes));

            Assert.True(reader.Read());
        }

        [Fact]
        public static void ReadWithIncompleteBytes()
        {
            var bytes = new byte[] { 60, 0, 0, 0, 97, 0, 0, 0, 65, 0, 0, 0, 97, 62, 10};
            var reader = XmlReader.Create(new MemoryStream(bytes));

            XmlException ex = Assert.Throws<XmlException>(() => reader.Read());        
            Assert.Equal("Data at the root level is invalid. Line 1, position 1.", ex.Message);
        }
    }
}
