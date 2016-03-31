// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents the set of binary operation flags in C# for use with <see cref="CSharpBinaryOperationBinder" /> instances.
    /// Instances of this enum are generated by the C# compiler.
    /// </summary>
    internal enum CSharpBinaryOperationFlags
    {
        None = 0,

        /// <summary>
        /// The operation is a binary compound operation on a member access.
        /// </summary>
        MemberAccess = 1,

        /// <summary>
        /// The operation is a logical binary operation.
        /// </summary>
        LogicalOperation = 2,
    }
}

