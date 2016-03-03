// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml
{
    // Specifies how whitespace is handled in XmlTextReader.
    internal enum WhitespaceHandling
    {
        // Return all Whitespace and SignificantWhitespace nodes. This is the default.
        /// <summary>Return Whitespace and SignificantWhitespace nodes. This is the default.</summary>
        All = 0,

        // Return just SignificantWhitespace, i.e. whitespace nodes that are in scope of xml:space="preserve"
        /// <summary>Return SignificantWhitespace nodes only.</summary>
        Significant = 1,

        // Do not return any Whitespace or SignificantWhitespace nodes.
        /// <summary>Return no Whitespace and no SignificantWhitespace nodes.</summary>
        None = 2
    }
}
