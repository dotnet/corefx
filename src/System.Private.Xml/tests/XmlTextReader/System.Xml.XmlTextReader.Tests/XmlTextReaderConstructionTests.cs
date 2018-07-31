// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.IO;

namespace System.Xml.Tests
{
    public class XmlTextReaderConstructionTests
    {
        [Fact]
        public void TextReaderConstructionWithStream()
        {
            var textReader = new XmlTextReader(null as string, new MemoryStream(), new NameTable());
            Assert.NotNull(textReader);            
        }

        [Fact]
        public void TextReaderConstructionWithStringReader()
        {
            XmlTextReader textReader =
                XmlTextReaderTestHelper.CreateReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>", new NameTable());
            Assert.NotNull(textReader);
        }

        [Fact]
        public void TextReaderConstructionWithXmlFragment()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1> abc </element1>");
            Assert.NotNull(textReader);
        }        
    }
}
