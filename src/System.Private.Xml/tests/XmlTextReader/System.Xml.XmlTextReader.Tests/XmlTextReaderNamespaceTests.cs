// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Xml.Tests
{
    public class XmlTextReaderNamespaceTests
    {
        [Fact]
        public void LookupNamespaceTest()
        {
            XmlTextReader textReader =
                XmlTextReaderTestHelper.CreateReaderWithStringReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>");
            Assert.True(textReader.Read());
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr", "urn:NameSpace"));
            var resolver = textReader as IXmlNamespaceResolver;
            Assert.Equal("urn:NameSpace", resolver.LookupNamespace("ns"));
        }

        [Fact]
        public void LookupPrefixTest()
        {
            XmlTextReader textReader =
                XmlTextReaderTestHelper.CreateReaderWithStringReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>");
            Assert.True(textReader.Read());
            Assert.True(textReader.Read());
            Assert.True(textReader.MoveToAttribute("attr", "urn:NameSpace"));
            var resolver = textReader as IXmlNamespaceResolver;
            Assert.Equal("ns", resolver.LookupPrefix("urn:NameSpace"));
        }

        [Fact]
        public void IXmlResolverGetNamespacesInScopeTest()
        {
            XmlTextReader textReader =
                XmlTextReaderTestHelper.CreateReaderWithStringReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>");
            Assert.True(textReader.Read());
            var resolver = textReader as IXmlNamespaceResolver;
            var expectedOutput = new Dictionary<string, string> { ["ns"] = "urn:NameSpace" };
            Assert.Equal(expectedOutput, resolver.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml));
            expectedOutput.Add("xml", "http://www.w3.org/XML/1998/namespace");
            Assert.Equal(expectedOutput, resolver.GetNamespacesInScope(XmlNamespaceScope.All));
        }

        [Fact]
        public void GetNamespacesInScopeTest()
        {
            XmlTextReader textReader =
                XmlTextReaderTestHelper.CreateReaderWithStringReader(@"<List xmlns:ns='urn:NameSpace'><element1 ns:attr='val'>abc</element1></List>");
            Assert.True(textReader.Read());
            var expectedOutput = new Dictionary<string, string> { ["ns"] = "urn:NameSpace" };
            Assert.Equal(expectedOutput, textReader.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml));
            expectedOutput.Add("xml", "http://www.w3.org/XML/1998/namespace");
            Assert.Equal(expectedOutput, textReader.GetNamespacesInScope(XmlNamespaceScope.All));
        }
    }
}
