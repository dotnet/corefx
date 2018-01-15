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
        public TypeParameterType CreateTypeParameter(TypeParameterSymbol pSymbol)
        {
            TypeParameterType type = new TypeParameterType();
            type.SetTypeParameterSymbol(pSymbol);
            Debug.Assert(pSymbol.GetTypeParameterType() == null);
            pSymbol.SetTypeParameterType(type);
            return type;
        }

        // Derived types - parent is base type
        public ArrayType CreateArray(CType pElementType, int rank, bool isSZArray)
        {
            ArrayType type = new ArrayType();

            type.rank = rank;
            type.IsSZArray = isSZArray;
            type.SetElementType(pElementType);
            return type;
        }

        public PointerType CreatePointer(CType pReferentType)
        {
            PointerType type = new PointerType();
            type.SetReferentType(pReferentType);
            return type;
        }

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
