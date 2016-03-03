// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    // DtdProcessing enumerations speficies how will an XmlReader handle DTDs in the XML document.
    //
    // Prohibit     The XmlReader will throw an exception when it finds a '<!DOCTYPE' markup.
    // Ignore       The DTD will be ignored. Any reference to a general entity in the XML document 
    //              will cause an exception (except for the predefined entities &lt; &gt; &amp; &quot; and &apos;).
    //              The DocumentType node will not be reported.
    // Parse        The DTD will be parsed and fully processed (entities expanded, default attributes added etc.)
    //
    /// <summary>Specifies the options for processing DTDs. The <see cref="T:System.Xml.DtdProcessing" /> enumeration is used by the <see cref="T:System.Xml.XmlReaderSettings" /> class.</summary>
    public enum DtdProcessing
    {
        /// <summary>Specifies that when a DTD is encountered, an <see cref="T:System.Xml.XmlException" /> is thrown with a message that states that DTDs are prohibited. This is the default behavior.</summary>
        Prohibit,
        /// <summary>Causes the DOCTYPE element to be ignored. No DTD processing occurs. </summary>
        Ignore,
        /// <summary>Used for parsing DTDs.</summary>
        Parse
    }
}
