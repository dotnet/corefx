// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Xml.Tests
{
    public class XmlTextReaderAttributeTests
    {
        [Fact]
        public void XmlTextReaderGetAttributeWithIndexTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val'> abc </element1>");            
            Assert.True(textReader.Read());
            Assert.Equal("val", textReader.GetAttribute(0));
        }

        [Fact]
        public void XmlTextReaderMoveToAttributeWithNameTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val'> abc </element1>");
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr"));
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val", textReader.Value);
        }

        [Fact]
        public void XmlTextReaderMoveToAttributeWithNamespace()
        {
            XmlTextReader textReader = 
                XmlTextReaderTestHelper.CreateReader("<List xmlns:tp='urn:NameSpace'><element1 tp:attr='val'>abc</element1></List>", new NameTable());
            Assert.True(textReader.Read());            
            Assert.True(textReader.Read());            
            Assert.True(textReader.MoveToAttribute("attr", "urn:NameSpace"));
            Assert.Equal(XmlNodeType.Attribute, textReader.NodeType);
            Assert.Equal("val", textReader.Value);
        }
    }
}
