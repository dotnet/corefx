// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal abstract class ExprOperator : ExprWithType
    {
        protected ExprOperator(ExpressionKind kind, CType type)
            : base(kind, type)
        {
            Debug.Assert(kind.IsUnaryOperator() || kind > ExpressionKind.TypeLimit);
        }

        public Expr OptionalUserDefinedCall { get; set; }

        public MethWithInst PredefinedMethodToCall { get; set; }

        public MethPropWithInst UserDefinedCallMethod { get; set; }
    }
}