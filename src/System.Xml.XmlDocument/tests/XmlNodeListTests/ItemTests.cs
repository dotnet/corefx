// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Xml.Tests
{
    public static class NodeList_ItemTests
    {
        [Fact]
        public static void ItemTest1()
        {
            var xml = @"<a><b/><b c='attr1'/></a>";

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            var xmlNodeList = xmlDocument.GetElementsByTagName("b");

            Assert.Equal(2, xmlNodeList.Count);

            Assert.Equal("b", xmlNodeList.Item(0).Name);
            Assert.Equal(XmlNodeType.Element, xmlNodeList.Item(0).NodeType);
            Assert.Equal(0, xmlNodeList.Item(0).Attributes.Count);

            Assert.Equal("b", xmlNodeList.Item(1).Name);
            Assert.Equal(XmlNodeType.Element, xmlNodeList.Item(1).NodeType);
            Assert.Equal(1, xmlNodeList.Item(1).Attributes.Count);
        }
    }
}
