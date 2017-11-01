// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Specifies metadata for a type, property, field, or method marked with the
    ///     <see cref="ExportAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, 
                    AllowMultiple = true, Inherited = false)]
    public sealed class ExportMetadataAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportMetadataAttribute"/> with the 
        ///     specified name and metadata value.
        /// </summary>
        /// <param name="name">
        ///     A <see cref="String"/> containing the name of the metadata value; or 
        ///     <see langword="null"/> to set the <see cref="Name"/> property to an empty 
        ///     string ("").
        /// </param>
        /// <param name="value">
        ///     An <see cref="object"/> containing the metadata value. This can be 
        ///     <see langword="null"/>.
        /// </param>
        public ExportMetadataAttribute(string name, object value)
        {
            this.Name = name ?? string.Empty;
            this.Value = value;
        }

        /// <summary>
        ///     Gets the name of the metadata value.
        /// </summary>
        /// <value>
        ///     A <see cref="String"/> containing the name of the metadata value.
        /// </value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the metadata value.
        /// </summary>
        /// <value>
        ///     An <see cref="object"/> containing the metadata value.
        /// </value>
        public object Value
        {
            get;
            private set;
        }

        public bool IsMultiple
        {
            get;
            set;
        }
    }
}