// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CSharp.RuntimeBinder.Errors;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    // ----------------------------------------------------------------------------
    //
    // NullableType
    //
    // A "derived" type representing Nullable<T>. The base type T is the parent.
    //
    // ----------------------------------------------------------------------------

    internal sealed class NullableType : CType
    {
        private AggregateType ats;

        public NullableType(CType underlyingType, BSYMMGR symmgr, TypeManager typeManager)
            : base(TypeKind.TK_NullableType)
        {
            UnderlyingType = underlyingType;
            SymbolManager = symmgr;
            TypeManager = typeManager;
        }

        public CType UnderlyingType { get; }

        private BSYMMGR SymbolManager { get; }

        private TypeManager TypeManager { get; }

        public AggregateType GetAts()
        {
            if (ats == null)
            {
                AggregateSymbol aggNullable = TypeManager.GetNullable();
                CType typePar = UnderlyingType;
                CType[] typeParArray = { typePar };
                TypeArray ta = SymbolManager.AllocParams(1, typeParArray);
                ats = TypeManager.GetAggregate(aggNullable, ta);
            }

            return ats;
        }

        public override CType StripNubs() => UnderlyingType;

        public override CType StripNubs(out bool wasNullable)
        {
            wasNullable = true;
            return UnderlyingType;
        }
    }
}
