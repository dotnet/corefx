// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Option placed on a type that controls when the <see cref="CompositionContainer"/> creates 
    /// a new instance of a <see cref="ComposablePart"/>.
    /// </summary>
    public enum CreationPolicy : int
    {
        /// <summary>
        /// Let the <see cref="CompositionContainer"/> choose the most appropriate <see cref="CreationPolicy"/>
        /// for the part given the current context. This is the default <see cref="CreationPolicy"/>, with
        /// the <see cref="CompositionContainer"/> choosing <see cref="CreationPolicy.Shared"/> by default
        /// unless the <see cref="ComposablePart"/> or importer requests <see cref="CreationPolicy.NonShared"/>.
        /// </summary>
        Any = 0,

        /// <summary>
        /// A single shared instance of the associated <see cref="ComposablePart"/> will be created
        /// by the <see cref="CompositionContainer"/> and shared by all requestors.
        /// </summary>
        Shared = 1,

        /// <summary>
        /// A new non-shared instance of the associated <see cref="ComposablePart"/> will be created
        /// by the <see cref="CompositionContainer"/> for every requestor.
        /// </summary>
        NonShared = 2,
    }
}
