using Xunit;

namespace System.Xml.Tests
{
    public class XmlNodeReaderGetAttributeTests
    {
        [Fact]
        public void NodeReaderGetAttributeWithEmptyXml()
        {
            var xmlDocument = new XmlDocument();
            var nodeReader = new XmlNodeReader(xmlDocument);
            Assert.Null(nodeReader.GetAttribute(string.Empty));
            Assert.Null(nodeReader.GetAttribute(string.Empty, string.Empty));
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() =>
            {
                nodeReader.GetAttribute(2);
            });
        }

        [Fact]
        public void NodeReaderGetAttributeWithValidXml()
        {
            string xml = "<root attr='val'><child /></root>";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            var nodeReader = new XmlNodeReader(xmlDocument);
            Assert.True(nodeReader.Read());
            Assert.Equal("val", nodeReader.GetAttribute("attr"));
            Assert.Equal("val", nodeReader.GetAttribute("attr", string.Empty));
            Assert.Equal("val", nodeReader.GetAttribute("attr", null));
            Assert.Equal("val", nodeReader.GetAttribute(0));            
        }
    }
}
