// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///     Indicates the cardinality of the <see cref="Export"/> objects required by an <see cref="ImportDefinition"/>.
    /// </summary>
    public enum ImportCardinality
    {
        /// <summary>
        ///     Zero or one <see cref="Export"/> objects are required by an <see cref="ImportDefinition"/>.
        /// </summary>
        ZeroOrOne = 0,

        /// <summary>
        ///     Exactly one <see cref="Export"/> object is required by an <see cref="ImportDefinition"/>.
        /// </summary>
        ExactlyOne = 1,

        /// <summary>
        ///     Zero or more <see cref="Export"/> objects are required by an <see cref="ImportDefinition"/>.
        /// </summary>
        ZeroOrMore = 2,
    }
}