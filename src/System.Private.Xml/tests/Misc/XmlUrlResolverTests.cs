// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Xunit;

namespace System.Xml.Tests
{
    public class XmlUriResolverTests
    {
        [Fact]
        public void Resolving_RelativeBase_Throws()
        {
            var resolver = new XmlUrlResolver();
            Assert.Throws<NotSupportedException>(() => resolver.ResolveUri(new Uri(",", UriKind.Relative), "test.xml"));
        }

        [Theory]
        [MemberData(nameof(GetBaseUriAndPath))]
        public void Resolving_LocalPath_Ok(Uri baseUri, string path)
        {
            var resolver = new XmlUrlResolver();
            Uri resolvedUri = resolver.ResolveUri(baseUri, path);

            Assert.Equal(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, path)), resolvedUri.LocalPath);
            Assert.True(resolvedUri.LocalPath.EndsWith(path.Replace('/', Path.DirectorySeparatorChar)));
        }

        public static IEnumerable<object[]> GetBaseUriAndPath()
        {
            var currDirWithDirSeparator = Environment.CurrentDirectory + Path.DirectorySeparatorChar;

            var baseUris = new Uri[]
            {
                new Uri(currDirWithDirSeparator, UriKind.Absolute),
                new Uri(currDirWithDirSeparator, UriKind.RelativeOrAbsolute),
                null
            };

            foreach (Uri baseUri in baseUris)
            {
                yield return new object[] { baseUri, "f#/t/ë/test.xml" };
                yield return new object[] { baseUri, "/f#/t/ë/t#st.xml" };
                yield return new object[] { baseUri, "/f#/ã/ë/tëst.xml" };
                yield return new object[] { baseUri, "u/t/c/test.xml" };
                yield return new object[] { baseUri, "u/t/c/t#st.xml" };
                yield return new object[] { baseUri, "/u/t/c/tëst.xml" };
                yield return new object[] { baseUri, "test.xml" };
                yield return new object[] { baseUri, "t#st.xml" };
                yield return new object[] { baseUri, "tëst.xml" };
            }
        }
    }
}
