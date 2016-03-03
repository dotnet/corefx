// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    /// <summary>Specifies the amount of input or output checking that <see cref="T:System.Xml.XmlReader" /> and <see cref="T:System.Xml.XmlWriter" /> objects perform.</summary>
    public enum ConformanceLevel
    {
        // With conformance level Auto an XmlReader or XmlWriter automatically determines whether in incoming XML is an XML fragment or document.
        /// <summary>The <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.Xml.XmlWriter" /> object automatically detects whether document-level or fragment-level checking should be performed, and does the appropriate checking. If you're wrapping another <see cref="T:System.Xml.XmlReader" /> or <see cref="T:System.Xml.XmlWriter" /> object, the outer object doesn't do any additional conformance checking. Conformance checking is left up to the underlying object.See the <see cref="P:System.Xml.XmlReaderSettings.ConformanceLevel" /> and <see cref="P:System.Xml.XmlWriterSettings.ConformanceLevel" /> properties for details on how the compliance level is determined.</summary>
        Auto = 0,

        // Conformance level for XML fragment. An XML fragment can contain any node type that can be a child of an element,
        // plus it can have a single XML declaration as its first node
        /// <summary>The XML data is a well-formed XML fragment, as defined by the W3C.</summary>
        Fragment = 1,

        // Conformance level for XML document as specified in XML 1.0 Specification
        /// <summary>The XML data complies with the rules for a well-formed XML 1.0 document, as defined by the W3C.</summary>
        Document = 2,
    }
}
