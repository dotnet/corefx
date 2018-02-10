// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    // This is the interface for the BindingContext, which is
    // consumed by the StatementBinder.
    // ----------------------------------------------------------------------------

    internal sealed class BindingContext
    {
        public BindingContext(CSemanticChecker semanticChecker)
        {
            Debug.Assert(semanticChecker != null);
            SemanticChecker = semanticChecker;
        }

        public BindingContext(BindingContext parent)
            : this(parent.SemanticChecker)
        {
            // We copy the context object, but leave checking false.
            ContextForMemberLookup = parent.ContextForMemberLookup;
        }

        public AggregateDeclaration ContextForMemberLookup { get; set; }

        public CSemanticChecker SemanticChecker { get; }

        public bool Checked { get; set; }
    }
}
