// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.Security.Cryptography.Xml
{
    // This is for generic, unknown nodes
    public class KeyInfoNode : KeyInfoClause
    {
        //
        // public constructors
        //

        public KeyInfoNode() { }

        public KeyInfoNode(XmlElement node)
        {
            Value = node;
        }

        //
        // public properties
        //

        public XmlElement Value { get; set; }

        //
        // public methods
        //

        public override XmlElement GetXml()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;
            return GetXml(xmlDocument);
        }

        internal override XmlElement GetXml(XmlDocument xmlDocument)
        {
            return xmlDocument.ImportNode(Value, true) as XmlElement;
        }

        public override void LoadXml(XmlElement value)
        {
            Value = value;
        }
    }
}
