// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Specifies that an attribute can be used to provide metadata for a type, property, field,
    ///     or method marked with the <see cref="ExportAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,
                    AllowMultiple=false, Inherited=true)]
    public sealed class MetadataAttributeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MetadataAttributeAttribute"/> class.
        /// </summary>
        public MetadataAttributeAttribute()
        {
        }
    }
}