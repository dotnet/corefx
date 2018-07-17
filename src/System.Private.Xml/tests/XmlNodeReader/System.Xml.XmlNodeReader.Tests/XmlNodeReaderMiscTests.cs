using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Xml.Tests
{
    public class XmlNodeReaderMiscTests
    {
        [Fact]
        public void NodeReaderCloseWithEmptyXml()
        {
            XmlNodeReader nodeReader = new XmlNodeReader(new XmlDocument());
            nodeReader.Close();
            Assert.Equal(ReadState.Closed, nodeReader.ReadState);
        }

        [Fact]
        public void NodeReaderSkipWithSimpleXml()
        {
            string xml = "<root atri='val'><child /></root>";
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XmlNodeReader nodeReader = new XmlNodeReader(document);
            Assert.True(nodeReader.Read());
            nodeReader.Skip();
            Assert.True(nodeReader.EOF);
            Assert.Equal(ReadState.EndOfFile, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.None, nodeReader.NodeType);
        }

        [Fact]
        public void NodeReaderLookupNamespaceWithEmptyXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);
            Assert.Null(nodeReader.LookupNamespace(string.Empty));            
        }

        [Fact]
        public void NodeReaderLookupNamespaceWithSimpleXml()
        {
            string xml = "<root></root>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDoc);
            nodeReader.Read();
            var namespaceResolver = nodeReader as IXmlNamespaceResolver;
            Assert.Equal(null, namespaceResolver.LookupNamespace("prefix"));
            Assert.Collection(namespaceResolver.GetNamespacesInScope(XmlNamespaceScope.All)
                , kv => Assert.Equal(kv.Key, "xml"));
            Assert.Empty(namespaceResolver.GetNamespacesInScope(XmlNamespaceScope.Local));
        }

        [Fact]
        public void NodeReaderLookupNamespaceWithValidXml()
        {
            string xml = "<book xmlns:bk='urn:samples'> " +
                   "<title>Pride And Prejudice</title>" +
                   "<bk:genre>novel</bk:genre>" +
                   "</book>";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNodeReader nodeReader = new XmlNodeReader(xmlDocument);
            Assert.Equal(string.Empty, nodeReader.LocalName);
            Assert.Equal(string.Empty, nodeReader.Name);
            Assert.Equal(string.Empty, nodeReader.NamespaceURI);
            Assert.Equal(string.Empty, nodeReader.Prefix);
            var namespaceResolver = nodeReader as IXmlNamespaceResolver;
            Assert.Equal(string.Empty, namespaceResolver.LookupNamespace(string.Empty));

            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.Read());
            Assert.Equal(null, nodeReader.LookupNamespace(string.Empty));
            namespaceResolver = nodeReader as IXmlNamespaceResolver;
            Assert.Equal(string.Empty, namespaceResolver.LookupNamespace(string.Empty));

            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.Read());
            Assert.Equal("genre", nodeReader.LocalName);
            Assert.Equal("bk:genre", nodeReader.Name);
            Assert.Equal("urn:samples", nodeReader.LookupNamespace(nodeReader.Prefix));
            Assert.Equal("bk", nodeReader.Prefix);

            namespaceResolver = nodeReader as IXmlNamespaceResolver;
            Assert.Equal("bk", namespaceResolver.LookupPrefix("urn:samples"));
            Assert.Equal("urn:samples", namespaceResolver.LookupNamespace("bk"));
            IDictionary<string, string> namespaces = namespaceResolver.GetNamespacesInScope(XmlNamespaceScope.All);
            Assert.True(namespaces.ContainsKey("bk") && namespaces["bk"].Equals("urn:samples"));
            Assert.True(namespaces.ContainsKey("xml"));
        }    
    }
}
