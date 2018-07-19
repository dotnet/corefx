using Xunit;

namespace System.Xml.Tests
{
    //Unit testing class for all reading related methods
    public class XmlNodeReaderReadTests
    {
        [Fact]
        public void NodeReaderReadWithEmptyDocument()
        {
            var nodeReader = new XmlNodeReader(new XmlDocument());            
            Assert.False(nodeReader.Read());
            Assert.Equal(ReadState.Error, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.None, nodeReader.NodeType);
            Assert.Equal(string.Empty, nodeReader.Name);
            Assert.Equal(string.Empty, nodeReader.LocalName);
            Assert.Equal(0, nodeReader.AttributeCount);
            Assert.False(nodeReader.IsDefault);
            Assert.False(nodeReader.IsEmptyElement);
        }

        [Fact]
        public void NodeReaderReadWithSimpleXml()
        {
            string xml = "<root atri='val'><child /></root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read()); //Root Element Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.Element, nodeReader.NodeType);

            Assert.True(nodeReader.Read()); //Child Element Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.Element, nodeReader.NodeType);
            Assert.True(nodeReader.IsEmptyElement);

            Assert.True(nodeReader.Read()); //End Element Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.EndElement, nodeReader.NodeType);

            Assert.False(nodeReader.Read()); //EOF Read
            Assert.Equal(ReadState.EndOfFile, nodeReader.ReadState);
            Assert.True(nodeReader.EOF);
            Assert.Equal(XmlNodeType.None, nodeReader.NodeType);

            Assert.False(nodeReader.Read()); //EOF Read
        }

        [Fact]
        public void NodeReaderReadWithEntityXml()
        {
            string fst = "<!ENTITY fst 'Sample String'>]>";            
            string dtd = "<!DOCTYPE root[<!ELEMENT root (#PCDATA)>" + fst;
            string xml = dtd + "<root>&fst;</root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read()); //DTD Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.DocumentType, nodeReader.NodeType);

            Assert.True(nodeReader.Read()); //Root Element Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.Element, nodeReader.NodeType);

            Assert.True(nodeReader.Read()); //EntityReference Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.EntityReference, nodeReader.NodeType);
            Assert.False(nodeReader.IsDefault);
            Assert.Equal("fst", nodeReader.Name);
            Assert.Equal("fst", nodeReader.LocalName);
        }

        [Fact]
        public void NodeReaderReadAttributeValueWithEmptyXml()
        {
            var document = new XmlDocument();            
            var nodeReader = new XmlNodeReader(document);
            Assert.False(nodeReader.ReadAttributeValue());
            Assert.False(nodeReader.HasValue);
            Assert.Equal(string.Empty, nodeReader.Value);
        }

        [Fact]
        public void NodeReaderReadAttributeValueWithSimpleXml()
        {
            string xml = "<root><child /></root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());
            Assert.False(nodeReader.ReadAttributeValue());
        }

        [Fact]
        public void NodeReaderReadAttributeValueWithAttributeXml()
        {
            string xml = "<root attr='val'><child /></root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToAttribute("attr"));
            Assert.True(nodeReader.ReadAttributeValue());
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.Text, nodeReader.NodeType);
            Assert.Equal(2, nodeReader.Depth);
            Assert.True(nodeReader.HasValue);
            Assert.Equal("val", nodeReader.Value);
            Assert.Equal(1, nodeReader.AttributeCount);
        }

        [Fact]
        public void NodeReaderReadStringWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.ThrowsAny<InvalidOperationException>(() => {
                nodeReader.ResolveEntity();
            });
            Assert.Equal(string.Empty, nodeReader.ReadString());
        }

        [Fact]
        public void NodeReaderReadStringWithEntityXml()
        {
            string fst = "<!ENTITY fst 'Sample String'>]>";
            string dtd = "<!DOCTYPE root[<!ELEMENT root (#PCDATA)>" + fst;
            string xml = dtd + "<root>&fst;</root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read()); //DTD Read
            Assert.True(nodeReader.Read()); //Root Element Read
            Assert.True(nodeReader.Read()); //EntityReference Read
            Assert.Equal(ReadState.Interactive, nodeReader.ReadState);
            Assert.Equal(XmlNodeType.EntityReference, nodeReader.NodeType);
            nodeReader.ResolveEntity();
            Assert.True(nodeReader.CanResolveEntity);
            Assert.Equal("Sample String", nodeReader.ReadString());
        }

        [Fact]
        public void NodeReaderReadContentAsBase64WithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.Equal(0, nodeReader.ReadContentAsBase64(null, 0, 0));
        }

        [Fact]
        public void NodeReaderReadContentAsBase64WithSimpleXml()
        {
            string xml = "<root attr='val'><child /></root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToAttribute("attr"));
            Assert.True(nodeReader.CanReadBinaryContent);
            Assert.Equal(2, nodeReader.ReadContentAsBase64(new byte[33], 10, 10));
            Assert.Equal(0, nodeReader.ReadContentAsBase64(new byte[33], 10, 10));
        }

        [Fact]
        public void NodeReaderReadContentAsBinHexWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.Equal(0, nodeReader.ReadContentAsBinHex(null, 0, 0));
        }

        [Fact]
        public void NodeReaderReadContentAsBinHexWithSimpleXml()
        {
            string xml = "<root attr='ff0000'><child /></root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToAttribute("attr"));
            Assert.True(nodeReader.CanReadBinaryContent);
            Assert.Equal(3, nodeReader.ReadContentAsBinHex(new byte[33], 10, 10));
            Assert.Equal(0, nodeReader.ReadContentAsBinHex(new byte[33], 10, 10));
        }

        [Fact]
        public void NodeReaderReadElementContentAsBase64WithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.Equal(0, nodeReader.ReadElementContentAsBase64(null, 0, 0));
        }

        [Fact]
        public void NodeReaderReadElementContentAsBase64WithSimpleXml()
        {
            string xml = "<root attr='val'>val</root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());            
            Assert.True(nodeReader.CanReadBinaryContent);
            Assert.Equal(2, nodeReader.ReadElementContentAsBase64(new byte[33], 10, 10));
            Assert.Equal(0, nodeReader.ReadElementContentAsBase64(new byte[33], 10, 10));
        }

        [Fact]
        public void NodeReaderReadElementContentAsBinHexWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.Equal(0, nodeReader.ReadElementContentAsBinHex(null, 0, 0));
        }

        [Fact]
        public void NodeReaderReadElementContentAsBinHexWithSimpleXml()
        {
            string xml = "<root attr='ff0000'>ff0000</root>";
            XmlNodeReader nodeReader = NodeReaderTestHelper.CreateNodeReader(xml);
            Assert.True(nodeReader.Read());            
            Assert.True(nodeReader.CanReadBinaryContent);
            Assert.Equal(3, nodeReader.ReadElementContentAsBinHex(new byte[33], 10, 10));
            Assert.Equal(0, nodeReader.ReadElementContentAsBinHex(new byte[33], 10, 10));
        }    
    }
}
