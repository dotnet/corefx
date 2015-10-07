// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Xml.Tests
{
    public static class SpecifiedTests
    {
        [Fact]
        [OuterLoop]
        public static void AttributeSpecifiedTest()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<elem1 child1='' child2='duu' child3='e1;e2;' child4='a1' child5='goody'> text node two e1; text node three </elem1>");

            var element = (XmlElement)xmlDocument.DocumentElement;

            Assert.True(((XmlAttribute)element.Attributes.Item(0)).Specified);
            Assert.True(((XmlAttribute)element.Attributes.Item(1)).Specified);
            Assert.True(((XmlAttribute)element.Attributes.Item(2)).Specified);
            Assert.True(((XmlAttribute)element.Attributes.Item(3)).Specified);
            Assert.True(((XmlAttribute)element.Attributes.Item(4)).Specified);
        }
    }
}
