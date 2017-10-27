// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Place on a type that should not be discovered as a <see cref="ComposablePart" /> in
    ///     a <see cref="ComposablePartCatalog" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PartNotDiscoverableAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PartNotDiscoverableAttribute"/> class.
        /// </summary>
        public PartNotDiscoverableAttribute()
        {
        }
    }
}
