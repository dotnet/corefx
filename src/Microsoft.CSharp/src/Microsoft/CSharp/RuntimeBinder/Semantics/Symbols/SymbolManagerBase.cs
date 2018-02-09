// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class BSYMMGR
    {
        // Special nullable members.
        public PropertySymbol propNubValue;
        public MethodSymbol methNubCtor;

        public BSYMMGR()
        {
            ////////////////////////////////////////////////////////////////////////////////
            // Build the data structures needed to make FPreLoad fast. Make sure the 
            // namespaces are created. Compute and sort hashes of the NamespaceSymbol * value and type
            // name (sans arity indicator).

            for (int i = 0; i < (int)PredefinedType.PT_COUNT; ++i)
            {
                NamespaceSymbol ns = NamespaceSymbol.Root;
                string name = PredefinedTypeFacts.GetName((PredefinedType)i);
                int start = 0;
                while (start < name.Length)
                {
                    int iDot = name.IndexOf('.', start);
                    if (iDot == -1)
                        break;
                    string sub = (iDot > start) ? name.Substring(start, iDot - start) : name.Substring(start);
                    Name nm = NameManager.Add(sub);
                    ns = LookupGlobalSymCore(nm, ns, symbmask_t.MASK_NamespaceSymbol) as NamespaceSymbol ?? SymFactory.CreateNamespace(nm, ns);
                    start += sub.Length + 1;
                }
            }
        }

        public static BetterType CompareTypes(TypeArray ta1, TypeArray ta2)
        {
            if (ta1 == ta2)
            {
                return BetterType.Same;
            }
            if (ta1.Count != ta2.Count)
            {
                // The one with more parameters is more specific.
                return ta1.Count > ta2.Count ? BetterType.Left : BetterType.Right;
            }

            BetterType nTot = BetterType.Neither;

            for (int i = 0; i < ta1.Count; i++)
            {
                CType type1 = ta1[i];
                CType type2 = ta2[i];
                BetterType nParam = BetterType.Neither;

            LAgain:
                if (type1.TypeKind != type2.TypeKind)
                {
                    if (type1 is TypeParameterType)
                    {
                        nParam = BetterType.Right;
                    }
                    else if (type2 is TypeParameterType)
                    {
                        nParam = BetterType.Left;
                    }
                }
                else
                {
                    switch (type1.TypeKind)
                    {
                        default:
                            Debug.Assert(false, "Bad kind in CompareTypes");
                            break;
                        case TypeKind.TK_TypeParameterType:
                            break;

                        case TypeKind.TK_PointerType:
                        case TypeKind.TK_ParameterModifierType:
                        case TypeKind.TK_ArrayType:
                        case TypeKind.TK_NullableType:
                            type1 = type1.BaseOrParameterOrElementType;
                            type2 = type2.BaseOrParameterOrElementType;
                            goto LAgain;

                        case TypeKind.TK_AggregateType:
                            nParam = CompareTypes(((AggregateType)type1).TypeArgsAll, ((AggregateType)type2).TypeArgsAll);
                            break;
                    }
                }

                if (nParam == BetterType.Right || nParam == BetterType.Left)
                {
                    if (nTot == BetterType.Same || nTot == BetterType.Neither)
                    {
                        nTot = nParam;
                    }
                    else if (nParam != nTot)
                    {
                        return BetterType.Neither;
                    }
                }
            }

            return nTot;
        }

        public static Symbol LookupGlobalSymCore(Name name, ParentSymbol parent, symbmask_t kindmask) => SymbolStore.LookupSym(name, parent, kindmask);

        public static Symbol LookupAggMember(Name name, AggregateSymbol agg, symbmask_t mask) => SymbolStore.LookupSym(name, agg, mask);

        public static Symbol LookupNextSym(Symbol sym, ParentSymbol parent, symbmask_t kindmask)
        {
            Debug.Assert(sym.parent == parent);

            sym = sym.nextSameName;
            Debug.Assert(sym == null || sym.parent == parent);

            // Keep traversing the list of symbols with same name and parent.
            while (sym != null)
            {
                if ((kindmask & sym.mask()) > 0)
                    return sym;

                sym = sym.nextSameName;
                Debug.Assert(sym == null || sym.parent == parent);
            }

            return null;
        }
    }
}

