// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class ExprQuestionMark : Expr
    {
        public ExprQuestionMark()
            : base(ExpressionKind.QuestionMark)
        {
        }

        public Expr TestExpression { get; set; }

        public ExprBinOp Consequence { get; set; }

        public override CType Type => Consequence.Type ?? Consequence.OptionalLeftChild.Type;
    }
}
