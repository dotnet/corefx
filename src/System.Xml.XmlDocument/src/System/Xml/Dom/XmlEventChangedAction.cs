// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Xml
{
    // Specifies the type of node change
    public enum XmlNodeChangedAction
    {
        // A node is being inserted in the tree.
        Insert = 0,

        // A node is being removed from the tree.
        Remove = 1,

        // A node value is being changed.
        Change = 2
    }
}
