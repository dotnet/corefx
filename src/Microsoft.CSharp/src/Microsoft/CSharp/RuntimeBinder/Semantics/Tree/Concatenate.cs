// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprConcat : ExprWithType
    {
        public ExprConcat(CType type)
            : base(ExpressionKind.Concat ,type)
        {
        }

        public Expr FirstArgument { get; set; }

        public Expr SecondArgument { get; set; }
    }
}
