// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Errors;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    // This is the interface for the BindingContext, which is
    // consumed by the StatementBinder.
    // ----------------------------------------------------------------------------

    internal class BindingContext
    {
        public static BindingContext CreateInstance(
                CSemanticChecker pSemanticChecker,
                ExprFactory exprFactory
            )
        {
            return new BindingContext(
                pSemanticChecker,
                exprFactory);
        }

        private BindingContext(
            CSemanticChecker pSemanticChecker,
            ExprFactory exprFactory
            )
        {
            m_ExprFactory = exprFactory;
            m_pParentDecl = null;
            Debug.Assert(pSemanticChecker != null);
            SemanticChecker = pSemanticChecker;
            SymbolLoader = SemanticChecker.GetSymbolLoader();
            CheckedNormal = false;
            CheckedConstant = false;
        }
        protected BindingContext(BindingContext parent)
        {
            m_ExprFactory = parent.m_ExprFactory;
            UnsafeErrorGiven = parent.UnsafeErrorGiven;
            m_pParentDecl = parent.m_pParentDecl;
            CheckedNormal = parent.CheckedNormal;
            CheckedConstant = parent.CheckedConstant;
            Debug.Assert(parent.SemanticChecker != null);
            SemanticChecker = parent.SemanticChecker;
            SymbolLoader = SemanticChecker.GetSymbolLoader();
        }


        //The SymbolLoader can be retrieved from m_pSemanticChecker,
        //but that is a virtual call that is showing up on the profiler. Retrieve
        //the SymbolLoader once at ruction and return a cached copy.

        // PERFORMANCE: Is this cache still necessary?
        public SymbolLoader SymbolLoader { get; private set; }
        public Declaration m_pParentDecl;
        public Declaration ContextForMemberLookup() { return m_pParentDecl; }

        public bool UnsafeErrorGiven { get; set; }

        // State boolean questions.

        public bool ReportUnsafeErrors()
        {
            return !UnsafeErrorGiven;
        }

        // Members.

        private ExprFactory m_ExprFactory;

        // The rest of the members.

        public CSemanticChecker SemanticChecker { get; }

        public ExprFactory GetExprFactory() { return m_ExprFactory; }

        public bool CheckedNormal { get; set; }
        public bool CheckedConstant { get; set; }
    }
}
