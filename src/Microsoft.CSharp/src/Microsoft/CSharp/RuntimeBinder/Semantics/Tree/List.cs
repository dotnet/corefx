// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprList : Expr
    {
        public ExprList(Expr optionalElement, Expr optionalNextListNode)
            : base(ExpressionKind.List)
        {
            OptionalElement = optionalElement;
            OptionalNextListNode = optionalNextListNode;
        }

        public Expr OptionalElement { get; set; }

        public Expr OptionalNextListNode { get; set; }
    }
}
