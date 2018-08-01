// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Xml.Tests
{
    public class XmlTextReaderAttributeTests
    {
        [Fact]
        public void AttributeWithIndexTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val' attr2='val2' attr3=''> abc </element1>");            
            Assert.True(textReader.Read());
            Assert.Equal("val", textReader.GetAttribute(0));
            Assert.Equal("val2", textReader.GetAttribute(1));
            Assert.Equal(string.Empty, textReader.GetAttribute(2));
            Assert.Throws<ArgumentOutOfRangeException>(() => textReader.GetAttribute(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => textReader.GetAttribute(-1));
        }

        [Fact]
        public void MoveToAttributeWithNameTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val' attr2='val2'> abc </element1>");
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr"));            
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val", textReader.Value);

            Assert.True(textReader.MoveToAttribute("attr2"));
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val2", textReader.Value);

            Assert.False(textReader.MoveToAttribute("attr2"));
        }

        [Fact]
        public void MoveToAttributeWithNamespace()
        {
            string inp = "<List xmlns:tp='urn:NameSpace'><element1 tp:attr='val' /><element1 xmlns:rp='urn' rp:attr='val2' /></List>";
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReaderWithStringReader(inp);
            Assert.True(textReader.Read());            
            Assert.True(textReader.Read());            
            Assert.True(textReader.MoveToAttribute("attr", "urn:NameSpace"));
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val", textReader.Value);

            Assert.True(textReader.MoveToAttribute("attr", "urn"));
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val2", textReader.Value);
        }
    }
}
