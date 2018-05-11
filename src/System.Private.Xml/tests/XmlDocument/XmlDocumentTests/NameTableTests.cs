// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Xml.Tests
{
    public class NameTableTests
    {
        [Fact]
        public static void RespectManuallyAddedReference()
        {
            // Test depends that the string used here is also automatically added to XmlDocument XmlNameTable
            string targetStr = "#DoCument".ToLowerInvariant(); // Avoid interning
            var d = new XmlDocument();
            string defaultXmlDocumentTargetStr = d.NameTable.Get(targetStr);
            Assert.NotSame(targetStr, defaultXmlDocumentTargetStr);
            Assert.Equal(targetStr, defaultXmlDocumentTargetStr);

            // Create a NameTable to be shared and ensure that it is using the same reference as the test
            var nt = new NameTable();
            nt.Add(targetStr);
            Assert.Same(targetStr, nt.Get(targetStr));

            // The one added earlier should be the actual reference
            d = new XmlDocument(nt);
            Assert.Same(targetStr, d.Name);
        }
    }
}
