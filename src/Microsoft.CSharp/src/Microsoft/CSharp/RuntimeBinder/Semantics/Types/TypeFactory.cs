// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal sealed class TypeFactory
    {
        // Aggregate
        public AggregateType CreateAggregateType(
            AggregateSymbol parent, TypeArray typeArgsThis, AggregateType outerType) =>
            new AggregateType(parent, typeArgsThis, outerType);

        // TypeParameter
        public TypeParameterType CreateTypeParameter(TypeParameterSymbol pSymbol) => new TypeParameterType(pSymbol);

        // Derived types - parent is base type
        public ArrayType CreateArray(CType pElementType, int rank, bool isSZArray) => new ArrayType(pElementType, rank, isSZArray);

        public PointerType CreatePointer(CType pReferentType) => new PointerType(pReferentType);

        public ParameterModifierType CreateParameterModifier(CType pParameterType)
        {
            ParameterModifierType type = new ParameterModifierType();
            type.SetParameterType(pParameterType);
            return type;
        }

        public NullableType CreateNullable(CType pUnderlyingType, BSYMMGR symmgr, TypeManager typeManager)
        {
            NullableType type = new NullableType();
            type.SetUnderlyingType(pUnderlyingType);
            type.symmgr = symmgr;
            type.typeManager = typeManager;
            return type;
        }
    }
}
