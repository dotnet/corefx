// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Text;

namespace System.Security.Cryptography.Xml
{
    // the class that provides node subset state and canonicalization function to XmlText
    internal class CanonicalXmlText : XmlText, ICanonicalizableNode
    {
        public CanonicalXmlText(string strData, XmlDocument doc, bool defaultNodeSetInclusionState)
            : base(strData, doc)
        {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (IsInNodeSet)
                strBuilder.Append(Utils.EscapeTextData(Value));
        }

        public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (IsInNodeSet)
            {
                UTF8Encoding utf8 = new UTF8Encoding(false);
                byte[] rgbData = utf8.GetBytes(Utils.EscapeTextData(Value));
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }
}
