using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml.Tests
{
    internal static class NodeReaderTestHelper
    {
        internal static XmlNodeReader CreateNodeReader(string xml)
        {            
            var document = new XmlDocument();
            document.LoadXml(xml);
            var nodeReader = new XmlNodeReader(document);
            return nodeReader;
        }
    }
}
