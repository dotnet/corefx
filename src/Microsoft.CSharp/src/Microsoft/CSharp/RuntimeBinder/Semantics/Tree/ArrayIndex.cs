// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprArrayIndex : ExprWithType
    {
        public ExprArrayIndex(CType type)
            : base(ExpressionKind.ArrayIndex, type)
        {
        }

        public Expr Array { get; set; }

        public Expr Index { get; set; }
    }
}
