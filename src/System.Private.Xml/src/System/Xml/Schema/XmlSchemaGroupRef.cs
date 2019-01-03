// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Schema
{
    using System.Xml.Serialization;

    public class XmlSchemaGroupRef : XmlSchemaParticle
    {
        private XmlQualifiedName _refName = XmlQualifiedName.Empty;
        private XmlSchemaGroupBase _particle;
        private XmlSchemaGroup _refined;

        [XmlAttribute("ref")]
        public XmlQualifiedName RefName
        {
            get { return _refName; }
            set { _refName = (value == null ? XmlQualifiedName.Empty : value); }
        }

        [XmlIgnore]
        public XmlSchemaGroupBase Particle
        {
            get { return _particle; }
        }

        internal void SetParticle(XmlSchemaGroupBase value)
        {
            _particle = value;
        }

        [XmlIgnore]
        internal XmlSchemaGroup Redefined
        {
            get { return _refined; }
            set { _refined = value; }
        }
    }
}
