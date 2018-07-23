// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Xml.Resolvers;
using Xunit;

namespace System.Xml.XmlResolver.Tests
{
    public class XmlPreloadedResolverConstructionTests
    {
        [Fact]
        public void XmlResolverWithDefaultConstructor()
        {
            var resolver = new XmlPreloadedResolver();
            Assert.Equal(14, resolver.PreloadedUris.Count());
        }

        [Fact]
        public void XmlResolverWithKnownDtdsConstructor()
        {
            var resolver = new XmlPreloadedResolver(XmlKnownDtds.Xhtml10);
            Assert.Equal(12, resolver.PreloadedUris.Count());
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Strict//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Transitional//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Frameset//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Latin 1 for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Symbols for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Special for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent")));

            resolver = new XmlPreloadedResolver(XmlKnownDtds.Rss091);
            Assert.Equal(2, resolver.PreloadedUris.Count());
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//Netscape Communications//DTD RSS 0.91//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://my.netscape.com/publish/formats/rss-0.91.dtd")));
        }

        [Fact]
        public void XmlResolverWithFallbackResolverConstructor()
        {
            var fallbackResolver = new XmlPreloadedResolver(XmlKnownDtds.None);
            var resolver = new XmlPreloadedResolver(fallbackResolver);
            Assert.Equal(14, resolver.PreloadedUris.Count());
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Strict//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Transitional//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Frameset//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Latin 1 for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Symbols for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Special for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//Netscape Communications//DTD RSS 0.91//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://my.netscape.com/publish/formats/rss-0.91.dtd")));
        }

        [Fact]
        public void XmlResolverWithFallbackAndKnownDtdsConstructor()
        {
            var fallbackResolver = new XmlPreloadedResolver();
            var resolver = new XmlPreloadedResolver(fallbackResolver, XmlKnownDtds.Rss091);
            Assert.Equal(2, resolver.PreloadedUris.Count());
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//Netscape Communications//DTD RSS 0.91//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://my.netscape.com/publish/formats/rss-0.91.dtd")));
        }

        [Fact]
        public void XmlResolverWithParameterizedConstructor()
        {
            var fallbackResolver = new XmlPreloadedResolver();
            var resolver = new XmlPreloadedResolver(fallbackResolver, XmlKnownDtds.Xhtml10, null);
            Assert.Equal(12, resolver.PreloadedUris.Count());
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Strict//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Transitional//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//DTD XHTML 1.0 Frameset//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Latin 1 for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Symbols for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("-//W3C//ENTITIES Special for XHTML//EN")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent")));
            Assert.True(resolver.PreloadedUris.Any(u => u.OriginalString.Equals("http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent")));
        }
    }
}
