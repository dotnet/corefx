﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.Resolvers;
using System.Text;
using System.IO;
using Xunit;

namespace System.Xml.XmlResolver.Tests
{
    public class XmlPreloadedResolverAddRemoveTests
    {
        [Fact]
        public void XmlResolverAddWithInvalidData()
        {
            var xmlResolver = new XmlPreloadedResolver();
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(null, new byte[22]));

            byte[] data = null;
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(new Uri("https://html"), data));

            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(null, null, 0, 0));
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(new Uri("https://html"), null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => xmlResolver.Add(new Uri("https://html"), new byte[22], -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => xmlResolver.Add(new Uri("https://html"), new byte[22], 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => xmlResolver.Add(new Uri("https://html"), new byte[11], 5, 20));

            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(null, new MemoryStream()));
            MemoryStream stream = null;
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(new Uri("https://html"), stream));

            string val = null;
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(null, string.Empty));
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Add(new Uri("https://html"), val));
        }

        [Fact]
        public void XmlResolverAddWithValidData()
        {            
            var xmlResolver = new XmlPreloadedResolver(XmlKnownDtds.Xhtml10);                        

            byte[] data = Encoding.ASCII.GetBytes("hello world");
            MemoryStream stream = new MemoryStream(data);
            xmlResolver.Add(new Uri("-//W3C//DTD XHTML 1.0 Transitional//EN", UriKind.RelativeOrAbsolute), stream);
            Stream result = xmlResolver.GetEntity(new Uri("-//W3C//DTD XHTML 1.0 Transitional//EN", UriKind.RelativeOrAbsolute),
                null, typeof(Stream)) as Stream;
            byte[] output = new byte[data.Length];
            result.Read(output, 0, output.Length);
            Assert.Equal(data, output);

            DummyStream dummyStream = new DummyStream(data);
            xmlResolver.Add(new Uri("-//W3C//DTD XHTML 1.0 Strict//EN", UriKind.RelativeOrAbsolute), dummyStream);
            Stream otherResult = xmlResolver.GetEntity(new Uri("-//W3C//DTD XHTML 1.0 Strict//EN", UriKind.RelativeOrAbsolute),
                null, typeof(Stream)) as Stream;
            output = new byte[data.Length];
            otherResult.Read(output, 0, output.Length);
            Assert.Equal(data, output);

        }

        [Fact]
        public void XmlResolverRemoveWithInvalidData()
        {
            var xmlResolver = new XmlPreloadedResolver(XmlKnownDtds.Xhtml10);
            Assert.Throws<ArgumentNullException>(() => xmlResolver.Remove(null));
        }

        [Fact]
        public void XmlResolverRemoveWithValidData()
        {
            var xmlResolver = new XmlPreloadedResolver(XmlKnownDtds.Xhtml10);
            byte[] data = Encoding.ASCII.GetBytes("hello world");
            MemoryStream stream = new MemoryStream(data);
            xmlResolver.Add(new Uri("-//W3C//DTD XHTML 1.0 Transitional//EN", UriKind.RelativeOrAbsolute), stream);
            xmlResolver.Remove(new Uri("-//W3C//DTD XHTML 1.0 Transitional//EN", UriKind.RelativeOrAbsolute));
        }
    }
}
