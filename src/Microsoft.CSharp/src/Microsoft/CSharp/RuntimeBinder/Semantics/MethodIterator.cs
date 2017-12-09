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
            private SymbolLoader _pSymbolLoader;
            private CSemanticChecker _pSemanticChecker;
            // Inputs.
            private AggregateType _pCurrentType;
            private MethodOrPropertySymbol _pCurrentSym;
            private AggregateDeclaration _pContext;
            private TypeArray _pContainingTypes;
            private CType _pQualifyingType;
            private Name _pName;
            private int _nArity;
            private symbmask_t _mask;
            private EXPRFLAG _flags;
            // Internal state.
            private int _nCurrentTypeCount;
            private bool _bIsCheckingInstanceMethods;
            // Flags for the current sym.
            private bool _bCurrentSymIsBogus;
            private bool _bCurrentSymIsInaccessible;

            public CMethodIterator(CSemanticChecker checker, SymbolLoader symLoader, Name name, TypeArray containingTypes, CType @object, CType qualifyingType, AggregateDeclaration context, bool allowBogusAndInaccessible, bool allowExtensionMethods, int arity, EXPRFLAG flags, symbmask_t mask)
            {
                Debug.Assert(name != null);
                Debug.Assert(symLoader != null);
                Debug.Assert(checker != null);
                Debug.Assert(containingTypes != null);
                Debug.Assert(containingTypes.Count != 0);
                _pSemanticChecker = checker;
                _pSymbolLoader = symLoader;
                _pCurrentType = null;
                _pCurrentSym = null;
                _pName = name;
                _pContainingTypes = containingTypes;
                _pQualifyingType = qualifyingType;
                _pContext = context;
                _nArity = arity;
                _flags = flags;
                _mask = mask;
                _nCurrentTypeCount = 0;
                _bIsCheckingInstanceMethods = true;
                _bCurrentSymIsBogus = false;
                _bCurrentSymIsInaccessible = false;
            }
            public MethodOrPropertySymbol GetCurrentSymbol()
            {
                return _pCurrentSym;
            }
            public AggregateType GetCurrentType()
            {
                return _pCurrentType;
            }
            public bool IsCurrentSymbolInaccessible()
            {
                return _bCurrentSymIsInaccessible;
            }
            public bool IsCurrentSymbolBogus()
            {
                return _bCurrentSymIsBogus;
            }

            public bool MoveNext(bool canIncludeExtensionsInResults) => (_pCurrentType != null || FindNextTypeForInstanceMethods()) && FindNextMethod();

            public bool AtEnd()
            {
                return _pCurrentSym == null;
            }
            private CSemanticChecker GetSemanticChecker()
            {
                return _pSemanticChecker;
            }
            private SymbolLoader GetSymbolLoader()
            {
                return _pSymbolLoader;
            }
            public bool CanUseCurrentSymbol()
            {
                // Make sure that whether we're seeing a ctor is consistent with the flag.
                // The only properties we handle are indexers.
                if (_mask == symbmask_t.MASK_MethodSymbol && (
                        0 == (_flags & EXPRFLAG.EXF_CTOR) != !((MethodSymbol)_pCurrentSym).IsConstructor() ||
                        0 == (_flags & EXPRFLAG.EXF_OPERATOR) != !((MethodSymbol)_pCurrentSym).isOperator) ||
                    _mask == symbmask_t.MASK_PropertySymbol && !(_pCurrentSym is IndexerSymbol))
                {
                    // Get the next symbol.
                    return false;
                }

                // If our arity is non-0, we must match arity with this symbol.
                if (_nArity > 0 & _mask == symbmask_t.MASK_MethodSymbol && ((MethodSymbol)_pCurrentSym).typeVars.Count != _nArity)
                {
                    return false;
                }

                // If this guy's not callable, no good.
                if (!ExpressionBinder.IsMethPropCallable(_pCurrentSym, (_flags & EXPRFLAG.EXF_USERCALLABLE) != 0))
                {
                    return false;
                }

                // Check access. If Sym is not accessible, then let it through and mark it.
                _bCurrentSymIsInaccessible = !GetSemanticChecker().CheckAccess(_pCurrentSym, _pCurrentType, _pContext, _pQualifyingType);

                // Check bogus. If Sym is bogus, then let it through and mark it.
                _bCurrentSymIsBogus = CSemanticChecker.CheckBogus(_pCurrentSym);

                return _bIsCheckingInstanceMethods;
            }

            private bool FindNextMethod()
            {
                while (true)
                {
                    _pCurrentSym = (_pCurrentSym == null
                        ? GetSymbolLoader().LookupAggMember(_pName, _pCurrentType.getAggregate(), _mask)
                        : SymbolLoader.LookupNextSym(_pCurrentSym, _pCurrentType.getAggregate(), _mask)) as MethodOrPropertySymbol;

                    // If we couldn't find a sym, we look up the type chain and get the next type.
                    if (_pCurrentSym == null)
                    {
                        if (_bIsCheckingInstanceMethods)
                        {
                            FindNextTypeForInstanceMethods();
                            if (_pCurrentType == null)
                            {
                                return false;
                            }

                            // Found an instance method.
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
                    _pCurrentType = null;
                    return false;
                }

                _pCurrentType = _pContainingTypes[_nCurrentTypeCount++] as AggregateType;
                return true;
            }
        }
    }
}
