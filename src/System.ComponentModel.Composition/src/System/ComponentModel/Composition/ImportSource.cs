// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Option placed on an import to determine how composition searches for exports.
    /// </summary>
    public enum ImportSource : int
    {
        /// <summary>
        /// The import can be satisfied with values from the current or parent (or other ancestor) containers  (scopes)
        /// </summary>
        Any = 0,

        /// <summary>
        /// The import can be satisfied with values from the current container (scope) 
        /// </summary>
        Local = 1,

        /// <summary>
        /// The import can only be satisfied with values from the parent container (or other ancestor containers) (scopes) 
        /// </summary>
        NonLocal = 2

    }
}
