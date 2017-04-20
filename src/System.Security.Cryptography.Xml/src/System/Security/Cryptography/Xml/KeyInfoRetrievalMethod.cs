// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.Security.Cryptography.Xml
{
    public class KeyInfoRetrievalMethod : KeyInfoClause
    {
        //
        // public constructors
        //

        public KeyInfoRetrievalMethod() { }

        public KeyInfoRetrievalMethod(string strUri)
        {
            Uri = strUri;
        }

        public KeyInfoRetrievalMethod(string strUri, string typeName)
        {
            Uri = strUri;
            Type = typeName;
        }

        //
        // public properties
        //

        public string Uri { get; set; }

        public string Type { get; set; }

        public override XmlElement GetXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            // Create the actual element
            XmlElement retrievalMethodElement = xmlDocument.CreateElement("RetrievalMethod", SignedXml.XmlDsigNamespaceUrl);

            if (!string.IsNullOrEmpty(Uri))
                retrievalMethodElement.SetAttribute("URI", Uri);
            if (!string.IsNullOrEmpty(Type))
                retrievalMethodElement.SetAttribute("Type", Type);

            return retrievalMethodElement;
        }

        public override void LoadXml(XmlElement value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            XmlElement retrievalMethodElement = value;
            Uri = Utils.GetAttribute(value, "URI", SignedXml.XmlDsigNamespaceUrl);
            Type = Utils.GetAttribute(value, "Type", SignedXml.XmlDsigNamespaceUrl);
        }
    }
}
