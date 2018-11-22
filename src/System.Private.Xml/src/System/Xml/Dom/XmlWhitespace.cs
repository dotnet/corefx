// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Xml.XPath;

namespace System.Xml
{
    // Represents the text content of an element or attribute.
    public class XmlWhitespace : XmlCharacterData
    {
        protected internal XmlWhitespace(string strData, XmlDocument doc) : base(strData, doc)
        {
            if (!doc.IsLoading && !base.CheckOnData(strData))
                throw new ArgumentException(SR.Xdom_WS_Char);
        }

        // Gets the name of the node.
        public override string Name
        {
            get
            {
                return OwnerDocument.strNonSignificantWhitespaceName;
            }
        }

        // Gets the name of the current node without the namespace prefix.
        public override string LocalName
        {
            get
            {
                return OwnerDocument.strNonSignificantWhitespaceName;
            }
        }

        // Gets the type of the current node.
        public override XmlNodeType NodeType
        {
            get
            {
                return XmlNodeType.Whitespace;
            }
        }

        public override XmlNode ParentNode
        {
            get
            {
                switch (parentNode.NodeType)
                {
                    case XmlNodeType.Document:
                        return base.ParentNode;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        XmlNode parent = parentNode.parentNode;
                        while (parent.IsText)
                        {
                            parent = parent.parentNode;
                        }
                        return parent;
                    default:
                        return parentNode;
                }
            }
        }

        public override string Value
        {
            get
            {
                return Data;
            }

            set
            {
                if (CheckOnData(value))
                    Data = value;
                else
                    throw new ArgumentException(SR.Xdom_WS_Char);
            }
        }

        // Creates a duplicate of this node.
        public override XmlNode CloneNode(bool deep)
        {
            Debug.Assert(OwnerDocument != null);
            return OwnerDocument.CreateWhitespace(Data);
        }

        // Saves the node to the specified XmlWriter.
        public override void WriteTo(XmlWriter w)
        {
            w.WriteWhitespace(Data);
        }

        // Saves all the children of the node to the specified XmlWriter.
        public override void WriteContentTo(XmlWriter w)
        {
            // Intentionally do nothing
        }

        internal override XPathNodeType XPNodeType
        {
            get
            {
                XPathNodeType xnt = XPathNodeType.Whitespace;
                DecideXPNodeTypeForTextNodes(this, ref xnt);
                return xnt;
            }
        }

        internal override bool IsText
        {
            get
            {
                return true;
            }
        }

        public override XmlNode PreviousText
        {
            get
            {
                if (parentNode.IsText)
                {
                    return parentNode;
                }
                return null;
            }
        }
    }
}
