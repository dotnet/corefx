// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder
{
    internal interface ICSharpBinder
    {
        CSharpArgumentInfo GetArgumentInfo(int index);

        // This is true for any binder that is eligible to take value type receiver
        // objects as a ref (for mutable operations). Such as calls ("v.M(d)"),
        // and indexers ("v[d] = v[d]"). Note that properties are not here because they
        // are only dispatched dynamically when the receiver is dynamic, and hence boxed.
        bool IsBinderThatCanHaveRefReceiver { get; }

        void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments);

        Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals);

        BindingFlag BindingFlags { get; }

        string Name { get; }

        Type ReturnType { get; }

        int GetGetBinderEquivalenceHash();
        bool IsEquivalentTo(ICSharpBinder other);
    }
}
