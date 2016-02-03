// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    //
    // Declaration
    //
    // Base class for NamespaceDeclaration and AggregateDeclaration. Parent is another DECL.
    // Children are DECLs.
    // ----------------------------------------------------------------------------

    internal class Declaration : ParentSymbol
    {
        public NamespaceOrAggregateSymbol bag;
        public Declaration declNext;
    }
}
