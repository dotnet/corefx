// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    // NOTE: Absolute numbering is utilized in DtdParser. 
    internal enum XmlTokenizedType
    {
        /// <summary>CDATA type.</summary>
        CDATA = 0,
        /// <summary>ID type.</summary>
        ID = 1,
        /// <summary>IDREF type.</summary>
        IDREF = 2,
        /// <summary>IDREFS type.</summary>
        IDREFS = 3,
        /// <summary>ENTITY type.</summary>
        ENTITY = 4,
        /// <summary>ENTITIES type.</summary>
        ENTITIES = 5,
        /// <summary>NMTOKEN type.</summary>
        NMTOKEN = 6,
        /// <summary>NMTOKENS type.</summary>
        NMTOKENS = 7,
        /// <summary>NOTATION type.</summary>
        NOTATION = 8,
        /// <summary>ENUMERATION type.</summary>
        ENUMERATION = 9,
        /// <summary>QName type.</summary>
        QName = 10,
        /// <summary>NCName type.</summary>
        NCName = 11,
        /// <summary>No type.</summary>
        None = 12
    }
}
