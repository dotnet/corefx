// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    // An enumeration for the xml:space scope used in XmlReader and XmlWriter.
    /// <summary>Specifies the current xml:space scope.</summary>
    public enum XmlSpace
    {
        // xml:space scope has not been specified.
        /// <summary>No xml:space scope.</summary>
        None = 0,

        // The xml:space scope is "default".
        /// <summary>The xml:space scope equals default.</summary>
        Default = 1,

        // The xml:space scope is "preserve".
        /// <summary>The xml:space scope equals preserve.</summary>
        Preserve = 2
    }
}
