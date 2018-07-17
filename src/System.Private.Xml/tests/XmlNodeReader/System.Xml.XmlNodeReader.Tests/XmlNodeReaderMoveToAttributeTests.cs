using Xunit;

namespace System.Xml.Tests
{
    public class XmlNodeReaderMoveToAttributeTests
    {
        [Fact]
        public void NodeReaderMoveToFirstAttributeWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.False(nodeReader.MoveToFirstAttribute());
        }

        [Fact]
        public void NodeReaderMoveToFirstAttributeWithSimpleXml()
        {
            string xml = "<root></root>";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.True(nodeReader.Read());
            Assert.False(nodeReader.MoveToFirstAttribute());
        }

        [Fact]
        public void NodeReaderMoveToFirstAttributeWithAttributeXml()
        {
            string xml = "<root attr='cal'><child attr='val'></child></root>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var nodeReader = new XmlNodeReader(document);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToFirstAttribute());

            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            Assert.True(nodeReader.MoveToFirstAttribute());
        }

        [Fact]
        public void NodeReaderMoveToNextAttributeWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.False(nodeReader.MoveToNextAttribute());
        }

        [Fact]
        public void NodeReaderMoveToNextAttributeWithSimpleXml()
        {
            string xml = "<root></root>";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.True(nodeReader.Read());
            Assert.False(nodeReader.MoveToNextAttribute());
        }

        [Fact]
        public void NodeReaderMoveToNextAttributeWithAttributeXml()
        {
            string xml = "<root attr='cal' attr2='val'></root>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var nodeReader = new XmlNodeReader(document);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToNextAttribute());

            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            Assert.True(nodeReader.MoveToNextAttribute());
        }

        [Fact]
        public void NodeReaderMoveToAttributeWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.False(nodeReader.MoveToAttribute(string.Empty));
            Assert.False(nodeReader.MoveToAttribute(string.Empty, string.Empty));
            Assert.Throws<ArgumentOutOfRangeException>(() => { nodeReader.MoveToAttribute(0); });
        }

        [Fact]
        public void NodeReaderMoveToAttributeWithSimpleXml()
        {
            string xml = "<root></root>";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.True(nodeReader.Read());
            Assert.False(nodeReader.MoveToAttribute(string.Empty));
            Assert.False(nodeReader.MoveToAttribute(string.Empty, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => { nodeReader.MoveToAttribute(0); });
        }

        [Fact]
        public void NodeReaderMoveToAttributeWithAttributeXml()
        {
            string xml = "<root catr='tal' xmlns:attr='cal' fatr='gal' xmlns:attr2='val'></root>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var nodeReader = new XmlNodeReader(document);
            Assert.True(nodeReader.Read());
            Assert.True(nodeReader.MoveToAttribute("catr"));
            Assert.True(nodeReader.MoveToAttribute("attr", "http://www.w3.org/2000/xmlns/"));
            nodeReader.MoveToAttribute(0);

            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            Assert.True(nodeReader.MoveToAttribute("fatr"));
            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            Assert.True(nodeReader.MoveToAttribute("attr2", "http://www.w3.org/2000/xmlns/"));
            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            nodeReader.MoveToAttribute(1);
        }

        [Fact]
        public void NodeReaderMoveToElementWithEmptyXml()
        {
            var xmlDoc = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDoc);
            Assert.False(nodeReader.MoveToElement());
        }

        [Fact]
        public void NodeReaderMoveToElementWithSimpleXml()
        {
            string xml = "<root attr='cal'><child attr='val'><inner attr='val'></inner></child></root>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var nodeReader = new XmlNodeReader(document);
            Assert.True(nodeReader.Read());
            nodeReader.MoveToFirstAttribute();
            Assert.True(nodeReader.MoveToElement());
            Assert.True(nodeReader.MoveToAttribute("attr"));
            nodeReader.ReadContentAsBase64(new byte[33], 10, 10);
            Assert.True(nodeReader.MoveToElement());
        }
    }
}
