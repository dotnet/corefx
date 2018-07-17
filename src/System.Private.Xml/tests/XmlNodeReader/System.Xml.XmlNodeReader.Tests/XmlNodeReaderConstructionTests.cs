using Xunit;

namespace System.Xml.Tests
{
    public class XmlNodeReaderConstructionTests
    {
        [Fact]
        public void NodeReaderConstructionWithEmptyDocument()
        {
            var nodeReader = new XmlNodeReader(new XmlDocument());
            Assert.Equal(0, nodeReader.Depth);
            Assert.Equal(ReadState.Initial, nodeReader.ReadState);
            Assert.False(nodeReader.EOF);            
            Assert.Equal(XmlNodeType.None, nodeReader.NodeType);
        }

        [Fact]
        public void NodeReaderConstructionWithNull()
        {
            Assert.ThrowsAny<ArgumentNullException>(() =>
            {
                var nodeReader = new XmlNodeReader(null);
            });
        }
    }
}
