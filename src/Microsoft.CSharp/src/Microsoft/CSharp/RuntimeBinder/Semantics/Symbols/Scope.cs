// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal class Scope : ParentSymbol
    {
        public uint nestingOrder;  // the nesting order of this scopes. outermost == 0
    }
}
