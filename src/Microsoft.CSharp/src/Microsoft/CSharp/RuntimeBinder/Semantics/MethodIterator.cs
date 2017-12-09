// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal partial class CMemberLookupResults
    {
        public partial class CMethodIterator
        {
            private readonly SymbolLoader _pSymbolLoader;
            private readonly CSemanticChecker _pSemanticChecker;
            // Inputs.
            private readonly AggregateDeclaration _pContext;
            private readonly TypeArray _pContainingTypes;
            private readonly CType _pQualifyingType;
            private readonly Name _pName;
            private readonly int _nArity;
            private readonly symbmask_t _mask;
            private readonly EXPRFLAG _flags;
            // Internal state.
            private int _nCurrentTypeCount;

            public CMethodIterator(CSemanticChecker checker, SymbolLoader symLoader, Name name, TypeArray containingTypes, CType qualifyingType, AggregateDeclaration context, int arity, EXPRFLAG flags, symbmask_t mask)
            {
                Debug.Assert(name != null);
                Debug.Assert(symLoader != null);
                Debug.Assert(checker != null);
                Debug.Assert(containingTypes != null);
                Debug.Assert(containingTypes.Count != 0);
                _pSemanticChecker = checker;
                _pSymbolLoader = symLoader;
                CurrentType = null;
                CurrentSymbol = null;
                _pName = name;
                _pContainingTypes = containingTypes;
                _pQualifyingType = qualifyingType;
                _pContext = context;
                _nArity = arity;
                _flags = flags;
                _mask = mask;
                _nCurrentTypeCount = 0;
                IsCurrentSymbolBogus = false;
                IsCurrentSymbolInaccessible = false;
            }

            public MethodOrPropertySymbol CurrentSymbol { get; private set; }

            public AggregateType CurrentType { get; private set; }

            public bool IsCurrentSymbolInaccessible { get; private set; }

            public bool IsCurrentSymbolBogus { get; private set; }

            public bool MoveNext() => (CurrentType != null || FindNextTypeForInstanceMethods()) && FindNextMethod();

            public bool AtEnd => CurrentSymbol == null;

            public bool CanUseCurrentSymbol
            {
                get
                {
                    // Make sure that whether we're seeing a ctor is consistent with the flag.
                    // The only properties we handle are indexers.
                    if (_mask == symbmask_t.MASK_MethodSymbol && (
                            0 == (_flags & EXPRFLAG.EXF_CTOR) != !((MethodSymbol)CurrentSymbol).IsConstructor() ||
                            0 == (_flags & EXPRFLAG.EXF_OPERATOR) != !((MethodSymbol)CurrentSymbol).isOperator) ||
                        _mask == symbmask_t.MASK_PropertySymbol && !(CurrentSymbol is IndexerSymbol))
                    {
                        // Get the next symbol.
                        return false;
                    }

                    // If our arity is non-0, we must match arity with this symbol.
                    if (_nArity > 0 & _mask == symbmask_t.MASK_MethodSymbol && ((MethodSymbol)CurrentSymbol).typeVars.Count != _nArity)
                    {
                        return false;
                    }

                    // If this guy's not callable, no good.
                    if (!ExpressionBinder.IsMethPropCallable(CurrentSymbol, (_flags & EXPRFLAG.EXF_USERCALLABLE) != 0))
                    {
                        return false;
                    }

                    // Check access. If Sym is not accessible, then let it through and mark it.
                    IsCurrentSymbolInaccessible = !_pSemanticChecker.CheckAccess(CurrentSymbol, CurrentType, _pContext, _pQualifyingType);

                    // Check bogus. If Sym is bogus, then let it through and mark it.
                    IsCurrentSymbolBogus = CSemanticChecker.CheckBogus(CurrentSymbol);

                    return true;
                }
            }

            private bool FindNextMethod()
            {
                for (;;)
                {
                    CurrentSymbol = (CurrentSymbol == null
                        ? _pSymbolLoader.LookupAggMember(_pName, CurrentType.getAggregate(), _mask)
                        : SymbolLoader.LookupNextSym(CurrentSymbol, CurrentType.getAggregate(), _mask)) as MethodOrPropertySymbol;

                    // If we couldn't find a sym, we look up the type chain and get the next type.
                    if (CurrentSymbol == null)
                    {
                        if (!FindNextTypeForInstanceMethods())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Note that we do not filter the current symbol for the user. They must do that themselves.
                        // This is because for instance, BindGrpToArgs wants to filter on arguments before filtering
                        // on bogosity.

                        // If we're here, we're good to go.

                        return true;
                    }
                }
            }

            private bool FindNextTypeForInstanceMethods()
            {
                if (_nCurrentTypeCount >= _pContainingTypes.Count)
                {
                    // No more types to check.
                    CurrentType = null;
                    return false;
                }

                CurrentType = _pContainingTypes[_nCurrentTypeCount++] as AggregateType;
                return true;
            }
        }
    }
}
