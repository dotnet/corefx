// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    /*****************************************************************************
    A GlobalSymbolContext represents the global symbol tables for a compilation. 
    This includes symbols, types, declarations.
    *****************************************************************************/

    internal sealed class GlobalSymbolContext
    {
        public GlobalSymbolContext()
        {
            GlobalSymbols = new BSYMMGR();
            TypeManager = new TypeManager();
        }

        public TypeManager TypeManager { get; }
        public TypeManager GetTypes() { return TypeManager; }
        private BSYMMGR GlobalSymbols { get; }
        public BSYMMGR GetGlobalSymbols() { return GlobalSymbols; }
    }
}
