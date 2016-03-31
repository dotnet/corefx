// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    internal static class XmlDocumentEx
    {
        // The only constructors of XmlAttribute are internal and protected so let's use protected attribute to access it
        private class FakeXmlAttribute : XmlAttribute
        {
            public FakeXmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc)
                : base(prefix, localName, namespaceURI, doc)
            { }
        }
        public static XmlAttribute GetNamespaceXml(this XmlDocument xmlDocument)
        {
            XmlAttribute ret = new FakeXmlAttribute(XmlConst.NsXmlNs, XmlConst.NsXml, XmlConst.ReservedNsXmlNs, xmlDocument);
            ret.Value = XmlConst.ReservedNsXml;
            return ret;
        }
    }
}
