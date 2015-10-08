// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace XDocumentTests.SDMSample
{
    public class SDM_Misc
    {
        [Fact]
        public void NodeTypes()
        {
            XDocument document = new XDocument();
            XElement element = new XElement("x");
            XText text = new XText("text-value");
            XComment comment = new XComment("comment");
            XProcessingInstruction processingInstruction = new XProcessingInstruction("target", "data");

            Assert.Equal(XmlNodeType.Document, document.NodeType);
            Assert.Equal(XmlNodeType.Element, element.NodeType);
            Assert.Equal(XmlNodeType.Text, text.NodeType);
            Assert.Equal(XmlNodeType.Comment, comment.NodeType);
            Assert.Equal(XmlNodeType.ProcessingInstruction, processingInstruction.NodeType);
        }
    }
}
