// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using System;
using System.Xml;

namespace XmlDocumentTests.XmlDocumentTests
{
    public class CreateProcessingInstruction
    {
        [Fact]
        public static void BasicCreate()
        {
            var xmlDocument = new XmlDocument();
            var newNode = xmlDocument.CreateProcessingInstruction("bar", "foo");

            Assert.Equal("<?bar foo?>", newNode.OuterXml);
            Assert.Equal(XmlNodeType.ProcessingInstruction, newNode.NodeType);
        }
    }
}
