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
            Name name,
            AggregateSymbol parent,
            TypeArray typeArgsThis,
            AggregateType outerType)
        {
            AggregateType type = new AggregateType();

            type.outerType = outerType;
            type.SetOwningAggregate(parent);
            type.SetTypeArgsThis(typeArgsThis);
            type.SetName(name);

            return type;
        }

        // TypeParameter
        public TypeParameterType CreateTypeParameter(TypeParameterSymbol pSymbol)
        {
            TypeParameterType type = new TypeParameterType();
            type.SetTypeParameterSymbol(pSymbol);
            type.SetName(pSymbol.name);
            Debug.Assert(pSymbol.GetTypeParameterType() == null);
            pSymbol.SetTypeParameterType(type);
            return type;
        }

        // Primitives
        public VoidType CreateVoid() => new VoidType();

        public NullType CreateNull() => new NullType();

        public MethodGroupType CreateMethodGroup() => new MethodGroupType();

        public ArgumentListType CreateArgList() => new ArgumentListType();

        public ErrorType CreateError(
            Name name,
            Name nameText,
            TypeArray typeArgs)
        {
            ErrorType e = new ErrorType();
            e.SetName(name);
            e.nameText = nameText;
            e.typeArgs = typeArgs;
            return e;
        }

        // Derived types - parent is base type
        public ArrayType CreateArray(Name name, CType pElementType, int rank, bool isSZArray)
        {
            ArrayType type = new ArrayType();

            type.SetName(name);
            type.rank = rank;
            type.IsSZArray = isSZArray;
            type.SetElementType(pElementType);
            return type;
        }

        public PointerType CreatePointer(Name name, CType pReferentType)
        {
            PointerType type = new PointerType();
            type.SetName(name);
            type.SetReferentType(pReferentType);
            return type;
        }

        public ParameterModifierType CreateParameterModifier(Name name, CType pParameterType)
        {
            ParameterModifierType type = new ParameterModifierType();
            type.SetName(name);
            type.SetParameterType(pParameterType);
            return type;
        }

        public NullableType CreateNullable(Name name, CType pUnderlyingType, BSYMMGR symmgr, TypeManager typeManager)
        {
            NullableType type = new NullableType();
            type.SetName(name);
            type.SetUnderlyingType(pUnderlyingType);
            type.symmgr = symmgr;
            type.typeManager = typeManager;
            return type;
        }
    }
}
