// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class PointerType : CType
    {
        public PointerType()
            : base(TypeKind.TK_PointerType)
        {
        }

        public CType GetReferentType() { return _pReferentType; }
        public void SetReferentType(CType pType) { _pReferentType = pType; }
        private CType _pReferentType;
    }
}
