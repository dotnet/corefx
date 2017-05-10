// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization.Xml.Canonicalization.Tests
{
    internal struct XmlAttributeHolder
    {
        private string _prefix;
        private string _ns;
        private string _localName;
        private string _value;

        public static XmlAttributeHolder[] emptyArray = new XmlAttributeHolder[0];

        public XmlAttributeHolder(string prefix, string localName, string ns, string value)
        {
            _prefix = prefix;
            _localName = localName;
            _ns = ns;
            _value = value;
        }
    }
}
