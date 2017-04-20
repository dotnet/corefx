// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Text;

namespace System.Security.Cryptography.Xml
{
    // the class that provides node subset state and canonicalization function to XmlComment
    internal class CanonicalXmlComment : XmlComment, ICanonicalizableNode
    {
        public CanonicalXmlComment(string comment, XmlDocument doc, bool defaultNodeSetInclusionState, bool includeComments)
            : base(comment, doc)
        {
            IsInNodeSet = defaultNodeSetInclusionState;
            IncludeComments = includeComments;
        }

        public bool IsInNodeSet { get; set; }

        public bool IncludeComments { get; private set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (!IsInNodeSet || !IncludeComments)
                return;

            if (docPos == DocPosition.AfterRootElement)
                strBuilder.Append((char)10);
            strBuilder.Append("<!--");
            strBuilder.Append(Value);
            strBuilder.Append("-->");
            if (docPos == DocPosition.BeforeRootElement)
                strBuilder.Append((char)10);
        }

        public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (!IsInNodeSet || !IncludeComments)
                return;

            UTF8Encoding utf8 = new UTF8Encoding(false);
            byte[] rgbData = utf8.GetBytes("(char) 10");
            if (docPos == DocPosition.AfterRootElement)
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes("<!--");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes(Value);
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            rgbData = utf8.GetBytes("-->");
            hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            if (docPos == DocPosition.BeforeRootElement)
            {
                rgbData = utf8.GetBytes("(char) 10");
                hash.TransformBlock(rgbData, 0, rgbData.Length, rgbData, 0);
            }
        }
    }
}
