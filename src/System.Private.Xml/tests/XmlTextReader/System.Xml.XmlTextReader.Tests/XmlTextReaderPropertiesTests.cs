// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Text;

namespace System.Xml.Tests
{
    public class XmlTextReaderPropertiesTests
    {
        [Fact]
        public void XmlTextReaderHasValueTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val'> abc </element1>");
            Assert.False(textReader.HasValue);
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr"));
            Assert.True(textReader.HasValue);
        }

        [Fact]
        public void XmlTextReaderQuoteCharTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 attr='val'> abc </element1>");
            Assert.Equal('"', textReader.QuoteChar);
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr"));
            Assert.Equal('\'', textReader.QuoteChar);
        }

        [Fact]
        public void XmlTextReaderXmlSpaceTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:space='preserve'> abc </element1>");
            Assert.Equal(XmlSpace.None, textReader.XmlSpace);
            Assert.True(textReader.Read());
            Assert.Equal(XmlSpace.Preserve, textReader.XmlSpace);
        }

        [Fact]
        public void XmlTextReaderXmlLangTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.Equal(string.Empty, textReader.XmlLang);
            Assert.True(textReader.Read());
            Assert.Equal("en-us", textReader.XmlLang);
        }

        [Fact]
        public void XmlTextReaderCanReadBinaryTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.True(textReader.CanReadBinaryContent);
        }

        [Fact]
        public void XmlTextReaderCanReadValueChunkTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.False(textReader.CanReadValueChunk);
        }

        [Fact]
        public void XmlTextReaderNormalizationTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.False(textReader.Normalization);
            Assert.True(textReader.Read());
            textReader.Normalization = true;
            Assert.True(textReader.Normalization);
        }

        [Fact]
        public void XmlTextReaderNamespacesTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.True(textReader.Namespaces);
            textReader.Namespaces = false;
            Assert.False(textReader.Namespaces);
        }

        [Fact]
        public void XmlTextReaderEncodingTest()
        {
            string input = @"<?xml version=""1.0"" encoding=""utf-16""?><List>
            <Employee><ID>1</ID><First>David</First>
              <Last>Smith</Last><Salary>10000</Salary></Employee>            
            </List>";
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader(input, new NameTable());
            Assert.Null(textReader.Encoding);
            Assert.True(textReader.Read());
            Assert.Equal(Encoding.Unicode, textReader.Encoding);
        }

        [Fact]
        public void XmlTextReaderDtdProcessingTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.Equal(DtdProcessing.Parse, textReader.DtdProcessing);
            textReader.DtdProcessing = DtdProcessing.Prohibit;
            Assert.Equal(DtdProcessing.Prohibit, textReader.DtdProcessing);
        }

        [Fact]
        public void XmlTextReaderProhibitDtdTest()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.False(textReader.ProhibitDtd);
            textReader.ProhibitDtd = true;
            Assert.True(textReader.ProhibitDtd);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void XmlTextReaderEntityHandling()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.Equal(EntityHandling.ExpandCharEntities, textReader.EntityHandling);
            textReader.EntityHandling = EntityHandling.ExpandEntities;
            Assert.Equal(EntityHandling.ExpandEntities, textReader.EntityHandling);
        }

        [Fact]
        public void XmlTextReaderCanResolveEntity()
        {
            XmlTextReader textReader = XmlTextReaderTestHelper.CreateReader("<element1 xml:lang='en-us'> abc </element1>");
            Assert.Equal(true, textReader.CanResolveEntity);
        }
    }
}
