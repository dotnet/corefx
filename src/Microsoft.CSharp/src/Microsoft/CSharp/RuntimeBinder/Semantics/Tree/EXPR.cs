// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal abstract class Expr
    {
        protected Expr(ExpressionKind kind)
        {
            Kind = kind;
        }

        internal object RuntimeObject { get; set; }

        internal CType RuntimeObjectActualType { get; set; }

        public ExpressionKind Kind { get; }

        public EXPRFLAG Flags { get; set; }

        public bool IsOptionalArgument { get; set; }

        public virtual void SetMismatchedStaticBit()
        {
            HasError = true;
        }

        public string ErrorString { get; set; }

        public virtual CType Type => null;

        public bool IsOK => !HasError;

        public bool HasError { get; private set; }

        public void SetError()
        {
            HasError = true;
        }
    }
}
