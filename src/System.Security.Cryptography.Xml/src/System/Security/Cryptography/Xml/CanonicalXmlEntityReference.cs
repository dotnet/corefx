// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
using System.Text;

namespace System.Security.Cryptography.Xml
{
    // the class that provides node subset state and canonicalization function to XmlEntityReference
    internal class CanonicalXmlEntityReference : XmlEntityReference, ICanonicalizableNode
    {
        public CanonicalXmlEntityReference(string name, XmlDocument doc, bool defaultNodeSetInclusionState)
            : base(name, doc)
        {
            IsInNodeSet = defaultNodeSetInclusionState;
        }

        public bool IsInNodeSet { get; set; }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (IsInNodeSet)
                CanonicalizationDispatcher.WriteGenericNode(this, strBuilder, docPos, anc);
        }

        public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (IsInNodeSet)
                CanonicalizationDispatcher.WriteHashGenericNode(this, hash, docPos, anc);
        }
    }
}
