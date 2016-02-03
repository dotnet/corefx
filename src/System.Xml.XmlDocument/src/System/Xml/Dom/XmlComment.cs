// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Xml
{
    // Represents the content of an XML comment.
    public class XmlComment : XmlCharacterData
    {
        protected internal XmlComment(string comment, XmlDocument doc) : base(comment, doc)
        {
        }

        // Gets the name of the node.
        public override String Name
        {
            get { return OwnerDocument.strCommentName; }
        }

        // Gets the name of the current node without the namespace prefix.
        public override String LocalName
        {
            get { return OwnerDocument.strCommentName; }
        }

        // Gets the type of the current node.
        public override XmlNodeType NodeType
        {
            get { return XmlNodeType.Comment; }
        }

        // Creates a duplicate of this node.
        public override XmlNode CloneNode(bool deep)
        {
            Debug.Assert(OwnerDocument != null);
            return OwnerDocument.CreateComment(Data);
        }

        // Saves the node to the specified XmlWriter.
        public override void WriteTo(XmlWriter w)
        {
            w.WriteComment(Data);
        }

        // Saves all the children of the node to the specified XmlWriter.
        public override void WriteContentTo(XmlWriter w)
        {
            // Intentionally do nothing
        }
    }
}

