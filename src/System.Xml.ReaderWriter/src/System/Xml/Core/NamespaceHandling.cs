// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    //
    // NamespaceHandling speficies how should the XmlWriter handle namespaces.
    //  

    /// <summary>Specifies whether to remove duplicate namespace declarations in the <see cref="T:System.Xml.XmlWriter" />. </summary>
    [Flags]
    public enum NamespaceHandling
    {
        //
        // Default behavior
        //
        /// <summary>Specifies that duplicate namespace declarations will not be removed.</summary>
        Default = 0x0,

        //
        // Duplicate namespace declarations will be removed
        //
        /// <summary>Specifies that duplicate namespace declarations will be removed. For the duplicate namespace to be removed, the prefix and the namespace must match.</summary>
        OmitDuplicates = 0x1,
    }
}
