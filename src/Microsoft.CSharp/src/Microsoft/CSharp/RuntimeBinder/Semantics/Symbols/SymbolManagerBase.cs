// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class BSYMMGR
    {
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

