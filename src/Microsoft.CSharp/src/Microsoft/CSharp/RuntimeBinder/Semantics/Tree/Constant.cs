// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class EXPRCONSTANT : EXPR
    {
        private EXPR OptionalConstructorCall;
        public EXPR GetOptionalConstructorCall() { return OptionalConstructorCall; }
        public void SetOptionalConstructorCall(EXPR value) { OptionalConstructorCall = value; }

        private bool IsZero
        {
            get
            {
                return Val.IsZero(this.type.constValKind());
            }
        }
        public bool isZero() { return IsZero; }

        public ConstVal Val { get; set; }

        public ulong getU64Value() { return Val.UInt64Val; }
        public long getI64Value() { return I64Value; }
        public long I64Value
        {
            get
            {
                FUNDTYPE ft = type.fundType();
                switch (ft)
                {
                    case FUNDTYPE.FT_I8:
                    case FUNDTYPE.FT_U8:
                        return Val.Int64Val;
                    case FUNDTYPE.FT_U4:
                        return Val.UInt32Val;
                    case FUNDTYPE.FT_I1:
                    case FUNDTYPE.FT_I2:
                    case FUNDTYPE.FT_I4:
                    case FUNDTYPE.FT_U1:
                    case FUNDTYPE.FT_U2:
                        return Val.Int32Val;
                    default:
                        Debug.Assert(false, "Bad fundType in getI64Value");
                        return 0;
                }
            }
        }
    }
}
