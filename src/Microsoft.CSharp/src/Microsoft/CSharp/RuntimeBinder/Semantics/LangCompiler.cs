// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class LangCompiler : CSemanticChecker
    {
        private SymbolLoader _symbolLoader;

        ////////////////////////////////////////////////////////////////////////////////
        // Construct a compiler. All the real work is done in the Init() routine. This 
        // primary initializes all the sub-components.

        public LangCompiler(NameManager pNameMgr)
        {
            GlobalSymbolContext globalSymbolContext = new GlobalSymbolContext(pNameMgr);
            _symbolLoader = new SymbolLoader(
                globalSymbolContext, new ErrorHandling(new UserStringBuilder(globalSymbolContext)));
        }

        public override SymbolLoader SymbolLoader { get { return _symbolLoader; } }
        public override SymbolLoader GetSymbolLoader() { return _symbolLoader; }
    }
}
