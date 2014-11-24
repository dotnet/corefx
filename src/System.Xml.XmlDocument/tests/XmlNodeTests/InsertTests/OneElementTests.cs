﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using System.Xml;

namespace XmlDocumentTests.XmlNodeTests.InsertTests
{
    public static class OneElementTests
    {
        private static readonly InsertType[] _InsertTypes = new[] { InsertType.Prepend, InsertType.Append };
        private static readonly XmlNodeType[] _XmlNodeTypes = new XmlNodeType[] { XmlNodeType.Whitespace, XmlNodeType.SignificantWhitespace, XmlNodeType.CDATA, XmlNodeType.Text, XmlNodeType.Comment, XmlNodeType.Element /*, XmlNodeType.EntityReference*/ };

        private static void OneTextNodeBase(string xml, InsertType insertType, XmlNodeType nodeType)
        {
            var insertDelegate = TestHelper.CreateInsertFrontOrEnd(insertType);
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(xml);

            var parent = xmlDocument.DocumentElement;
            var child = parent.FirstChild;
            var newChild = TestHelper.CreateNode(xmlDocument, nodeType);
            var expected = (insertType == InsertType.Prepend) ? (newChild.OuterXml + child.OuterXml) : (child.OuterXml + newChild.OuterXml);

            // insert new child
            insertDelegate(parent, newChild);

            // verify
            Assert.Equal(2, parent.ChildNodes.Count);
            Assert.Equal(expected, parent.InnerXml);

            TestHelper.Verify(parent, child, newChild);
            TestHelper.Verify(child, newChild, insertType);

            // delete new child
            parent.RemoveChild(newChild);

            // verify
            Assert.Equal(1, parent.ChildNodes.Count);

            // Verify single child
            Assert.NotNull(parent);
            Assert.NotNull(child);

            Assert.Equal(child, parent.FirstChild);
            Assert.Equal(child, parent.LastChild);
            Assert.Null(child.NextSibling);
            Assert.Null(child.PreviousSibling);

            // delete the last child
            parent.RemoveChild(child);

            // verify
            Assert.Equal(0, parent.ChildNodes.Count);
            Assert.Null(parent.FirstChild);
            Assert.Null(parent.LastChild);
            Assert.False(parent.HasChildNodes);
        }

        [Fact]
        public static void Whitespace()
        {
            var xml = @"<W> 	
        </W>";
            foreach (var insertType in _InsertTypes)
                foreach (var nodeType in _XmlNodeTypes)
                    OneTextNodeBase(xml, insertType, nodeType);
        }

        [Fact]
        public static void SignificantWhitespace()
        {
            var xml = @"<S xml:space=""preserve""> 	</S>";
            foreach (var insertType in _InsertTypes)
                foreach (var nodeType in _XmlNodeTypes)
                    OneTextNodeBase(xml, insertType, nodeType);
        }

        [Fact]
        public static void CDATA()
        {
            var xml = @"<C><![CDATA[ &lt; &amp; <tag> < ! > & </tag> 	 ]]></C>";

            foreach (var insertType in _InsertTypes)
                foreach (var nodeType in _XmlNodeTypes)
                    OneTextNodeBase(xml, insertType, nodeType);
        }

        [Fact]
        public static void Text()
        {
            var xml = @"<T>text</T>";

            foreach (var insertType in _InsertTypes)
                foreach (var nodeType in _XmlNodeTypes)
                    OneTextNodeBase(xml, insertType, nodeType);
        }
    }
}
