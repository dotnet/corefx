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
        public static BindingContext CreateInstance(CSemanticChecker semanticChecker, ExprFactory exprFactory)
        {
            Debug.Assert(semanticChecker != null);
            return new BindingContext(semanticChecker, exprFactory);
        }

        public static BindingContext CreateInstance(BindingContext parent, bool checkedNormal, bool checkedConstant)
        {
            Debug.Assert(parent.SemanticChecker != null);
            return new BindingContext(parent, checkedNormal, checkedConstant);
        }

        private BindingContext(CSemanticChecker semanticChecker, ExprFactory exprFactory)
        {
            ExprFactory = exprFactory;
            SemanticChecker = semanticChecker;
            SymbolLoader = semanticChecker.GetSymbolLoader();
        }

        private BindingContext(BindingContext parent, bool checkedNormal, bool checkedConstant)
        {
            ExprFactory = parent.ExprFactory;
            ReportUnsafeErrors = parent.ReportUnsafeErrors;
            ContextForMemberLookup = parent.ContextForMemberLookup;
            CheckedNormal = parent.CheckedNormal;
            CheckedConstant = parent.CheckedConstant;
            SymbolLoader = (SemanticChecker = parent.SemanticChecker).GetSymbolLoader();
            CheckedNormal = checkedNormal;
            CheckedConstant = checkedConstant;
        }


        //The SymbolLoader can be retrieved from m_pSemanticChecker,
        //but that is a virtual call that is showing up on the profiler. Retrieve
        //the SymbolLoader once at construction and return a cached copy.

        // PERFORMANCE: Is this cache still necessary?
        public SymbolLoader SymbolLoader { get; }

        public Declaration ContextForMemberLookup { get; set; }

        // State boolean questions.

        public bool ReportUnsafeErrors { get; set; } = true;

        // Members.

        public CSemanticChecker SemanticChecker { get; }

        public ExprFactory ExprFactory { get; }

        public bool CheckedNormal { get; set; }

        public bool CheckedConstant { get; set; }
    }
}
