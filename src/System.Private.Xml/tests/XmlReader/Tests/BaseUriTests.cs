// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace System.Xml.Tests
{
    public class BaseUriTests
    {
        private const string _dummyXml = @"<?xml version=""1.0""?>";

        public static IEnumerable<object[]> GetXmlReaderUrlCreateMethods()
        {
            yield return new [] { new Func<string, XmlReader>(s => XmlReader.Create(s)) };
            yield return new [] { new Func<string, XmlReader>(s => XmlReader.Create(File.OpenRead(s), null, s)) };
            yield return new [] { new Func<string, XmlReader>(s => XmlReader.Create(new StreamReader(File.OpenRead(s)), null, s)) };
            yield return new [] { new Func<string, XmlReader>(s => new XmlTextReader(s)) };
            yield return new [] { new Func<string, XmlReader>(s => new XmlTextReader(s, File.OpenRead(s))) };
            yield return new [] { new Func<string, XmlReader>(s => new XmlTextReader(s, new StreamReader(File.OpenRead(s)))) };
        }

        [Theory]
        [MemberData(nameof(GetXmlReaderUrlCreateMethods))]
        public void CreateWithAbsolutePathGivesAbsoluteBaseUri(Func<string, XmlReader> factory)
        {
            var tempPath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempPath, _dummyXml);
                using (XmlReader reader = factory(tempPath))
                {
                    Assert.True(new Uri(reader.BaseURI).IsAbsoluteUri);
                }
            }
            finally
            {
                File.Delete(tempPath);
            }
        }
    }
}
