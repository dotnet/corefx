// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Schema
{
    using System.Xml.Serialization;

    public abstract class XmlSchemaGroupBase : XmlSchemaParticle
    {
        [XmlIgnore]
        public abstract XmlSchemaObjectCollection Items { get; }

        internal abstract void SetItems(XmlSchemaObjectCollection newItems);
    }
}
