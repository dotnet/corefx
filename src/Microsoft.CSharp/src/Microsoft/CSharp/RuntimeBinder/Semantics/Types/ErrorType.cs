// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    //
    // ErrorType
    //
    // ErrorType - a symbol representing an error that has been reported.
    // ----------------------------------------------------------------------------

    internal sealed class ErrorType : CType
    {
        public static readonly ErrorType Parentless = new ErrorType();

        private ErrorType()
            : base(TypeKind.TK_ErrorType)
        {
        }

        public ErrorType(Name nameText, TypeArray typeArgs)
            : this()
        {
            Debug.Assert(nameText != null);
            Debug.Assert(typeArgs != null);
            NameText = nameText;
            TypeArgs = typeArgs;
        }

        public Name NameText { get; }

        public TypeArray TypeArgs { get; }

        // ErrorTypes are always either the per-TypeManager singleton ErrorType
        // that has a null nameText and no namespace parent, or else have a
        // non-null nameText and have the root namespace as the namespace parent.
        public bool HasParent => this != Parentless;
    }
}
