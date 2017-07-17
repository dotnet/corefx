// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprMethodInfo : ExprWithType
    {
        public ExprMethodInfo(CType type, MethodSymbol method, AggregateType methodType, TypeArray methodParameters)
            : base(ExpressionKind.MethodInfo, type)
        {
            Method = new MethWithInst(method, methodType, methodParameters);
        }

        public MethWithInst Method { get; }
    }
}
