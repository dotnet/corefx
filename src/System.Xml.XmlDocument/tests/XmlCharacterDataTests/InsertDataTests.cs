// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Xml.Tests
{
    public class InsertDataTests
    {
        [Fact]
        public static void InsertDataAtBeginningOfCdataNode()
        {
            var xmlDocument = new XmlDocument();
            var cdataNode = (XmlCharacterData)xmlDocument.CreateCDataSection("comment");

            cdataNode.InsertData(0, "hello ");

            Assert.Equal("hello comment", cdataNode.Data);
        }

        [Fact]
        public static void InsertDataAtMiddleOfCdataNode()
        {
            var xmlDocument = new XmlDocument();
            var cdataNode = (XmlCharacterData)xmlDocument.CreateCDataSection("comment");

            cdataNode.InsertData(3, " hello ");

            Assert.Equal("com hello ment", cdataNode.Data);
        }

        [Fact]
        public static void InsertDataInEmptyCdataNode()
        {
            var xmlDocument = new XmlDocument();
            var cdataNode = (XmlCharacterData)xmlDocument.CreateCDataSection(null);

            cdataNode.InsertData(0, "hello");

            Assert.Equal("hello", cdataNode.Data);
        }

        [Fact]
        public static void InsertDataBeyondEndOfEmptyCdataNode()
        {
            var xmlDocument = new XmlDocument();
            var cdataNode = (XmlCharacterData)xmlDocument.CreateCDataSection(null);

            Assert.Throws<ArgumentOutOfRangeException>(() => cdataNode.InsertData(1, "hello "));
        }

        [Fact]
        public static void InsertDataBeyondEndOfCdataNodeBigNumber()
        {
            var xmlDocument = new XmlDocument();
            var cdataNode = (XmlCharacterData)xmlDocument.CreateCDataSection("hello");

            Assert.Throws<ArgumentOutOfRangeException>(() => cdataNode.InsertData(10, "hello "));
        }
    }
}
