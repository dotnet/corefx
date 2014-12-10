// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using System;
using System.Xml;

namespace XmlDocumentTests.XmlDocumentTests
{
    public class ImportNodeTests
    {
        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportNullNode()
        {
            var xmlDocument = new XmlDocument();

            Assert.Throws<InvalidOperationException>(() => xmlDocument.ImportNode(null, false));
            Assert.Throws<InvalidOperationException>(() => xmlDocument.ImportNode(null, true));
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportDocumentNode()
        {
            var xmlDocument = new XmlDocument();
            var toImport = new XmlDocument();

            Assert.Throws<InvalidOperationException>(() => xmlDocument.ImportNode(toImport, true));
            Assert.Throws<InvalidOperationException>(() => xmlDocument.ImportNode(toImport, false));
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportDocumentFragment()
        {
            var tempDoc = new XmlDocument();
            var nodeToImport = tempDoc.CreateDocumentFragment();

            nodeToImport.AppendChild(tempDoc.CreateElement("A1"));
            nodeToImport.AppendChild(tempDoc.CreateComment("comment"));
            nodeToImport.AppendChild(tempDoc.CreateProcessingInstruction("PI", "donothing"));

            var xmlDocument = new XmlDocument();
            var node = xmlDocument.ImportNode(nodeToImport, true);

            Assert.Equal(xmlDocument, node.OwnerDocument);
            Assert.Equal(XmlNodeType.DocumentFragment, node.NodeType);
            Assert.Equal(nodeToImport.OuterXml, node.OuterXml);
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportWhiteSpace()
        {
            var whitespace = "        ";
            var tempDoc = new XmlDocument();
            var nodeToImport = tempDoc.CreateWhitespace(whitespace);

            var xmlDocument = new XmlDocument();
            var node = xmlDocument.ImportNode(nodeToImport, true);

            Assert.Equal(xmlDocument, node.OwnerDocument);
            Assert.Equal(XmlNodeType.Whitespace, node.NodeType);
            Assert.Equal(whitespace, node.Value);
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportSignificantWhitespace()
        {
            var whitespace = "        \t";
            var tempDoc = new XmlDocument();
            var nodeToImport = tempDoc.CreateSignificantWhitespace(whitespace);

            var xmlDocument = new XmlDocument();
            var node = xmlDocument.ImportNode(nodeToImport, true);

            Assert.Equal(xmlDocument, node.OwnerDocument);
            Assert.Equal(XmlNodeType.SignificantWhitespace, node.NodeType);
            Assert.Equal(whitespace, node.Value);
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportElementDeepFalse()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<elem1 child1='' child2='duu' child3='e1;e2;' child4='a1' child5='goody'> text node two text node three </elem1>");

            var doc = new XmlDocument();
            var imported = doc.ImportNode(xmlDocument.DocumentElement, false);


            Assert.Equal(xmlDocument.DocumentElement.Name, imported.Name);
            Assert.Equal(xmlDocument.DocumentElement.Value, imported.Value);
            Assert.Equal(xmlDocument.DocumentElement.Prefix, imported.Prefix);
            Assert.Equal(xmlDocument.DocumentElement.NamespaceURI, imported.NamespaceURI);
            Assert.Equal(xmlDocument.DocumentElement.LocalName, imported.LocalName);

            Assert.Equal(0, imported.ChildNodes.Count);
            Assert.Equal(xmlDocument.DocumentElement.Attributes.Count, imported.Attributes.Count);
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportElementDeepTrue()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<elem1 child1='' child2='duu' child3='e1;e2;' child4='a1' child5='goody'> text node two text node three </elem1>");

            var doc = new XmlDocument();
            var imported = doc.ImportNode(xmlDocument.DocumentElement, true);


            Assert.Equal(xmlDocument.DocumentElement.Name, imported.Name);
            Assert.Equal(xmlDocument.DocumentElement.Value, imported.Value);
            Assert.Equal(xmlDocument.DocumentElement.Prefix, imported.Prefix);
            Assert.Equal(xmlDocument.DocumentElement.NamespaceURI, imported.NamespaceURI);
            Assert.Equal(xmlDocument.DocumentElement.LocalName, imported.LocalName);

            Assert.Equal(xmlDocument.DocumentElement.ChildNodes.Count, imported.ChildNodes.Count);
            Assert.Equal(xmlDocument.DocumentElement.Attributes.Count, imported.Attributes.Count);
        }

        //[Fact] https://github.com/dotnet/corefx/issues/208
        public static void ImportAttributeDeepFalse()
        {
            var xmlDocument = new XmlDocument();
            var attr = xmlDocument.CreateAttribute("child", "asdf");
            attr.Value = "valuehere";

            var doc = new XmlDocument();
            var imported = doc.ImportNode(attr, false);

            Assert.Equal(attr.Name, imported.Name);
            Assert.Equal(attr.Value, imported.Value);
            Assert.Equal(attr.Prefix, imported.Prefix);
            Assert.Equal(attr.NamespaceURI, imported.NamespaceURI);
            Assert.Equal(attr.LocalName, imported.LocalName);

            Assert.Equal(1, imported.ChildNodes.Count);
        }
    }
}
