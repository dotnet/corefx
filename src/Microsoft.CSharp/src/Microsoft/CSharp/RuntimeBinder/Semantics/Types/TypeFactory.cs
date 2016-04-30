// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal class TypeFactory
    {
        // Constructor.
        public TypeFactory()
        {
        }

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

            type.SetTypeKind(TypeKind.AggregateType);
            return type;
        }

        // TypeParameter
        public TypeParameterType CreateTypeParameter(TypeParameterSymbol pSymbol)
        {
            TypeParameterType type = new TypeParameterType();
            type.SetTypeParameterSymbol(pSymbol);
            type.SetUnresolved(pSymbol.parent != null && pSymbol.parent.IsAggregateSymbol() && pSymbol.parent.AsAggregateSymbol().IsUnresolved());
            type.SetName(pSymbol.name);

#if CSEE
            type.typeRes = type;
            if (!type.IsUnresolved())
            {
                type.tsRes = ktsImportMax;
            }
#endif // CSEE

            Debug.Assert(pSymbol.GetTypeParameterType() == null);
            pSymbol.SetTypeParameterType(type);

            type.SetTypeKind(TypeKind.TypeParameterType);
            return type;
        }

        // Primitives
        public VoidType CreateVoid()
        {
            VoidType type = new VoidType();
            type.SetTypeKind(TypeKind.VoidType);
            return type;
        }

        public NullType CreateNull()
        {
            NullType type = new NullType();
            type.SetTypeKind(TypeKind.NullType);
            return type;
        }

        public OpenTypePlaceholderType CreateUnit()
        {
            OpenTypePlaceholderType type = new OpenTypePlaceholderType();
            type.SetTypeKind(TypeKind.OpenTypePlaceholderType);
            return type;
        }

        public BoundLambdaType CreateAnonMethod()
        {
            BoundLambdaType type = new BoundLambdaType();
            type.SetTypeKind(TypeKind.BoundLambdaType);
            return type;
        }

        public MethodGroupType CreateMethodGroup()
        {
            MethodGroupType type = new MethodGroupType();
            type.SetTypeKind(TypeKind.MethodGroupType);
            return type;
        }

        public ArgumentListType CreateArgList()
        {
            ArgumentListType type = new ArgumentListType();
            type.SetTypeKind(TypeKind.ArgumentListType);
            return type;
        }

        public ErrorType CreateError(
            Name name,
            CType parent,
            AssemblyQualifiedNamespaceSymbol pParentNS,
            Name nameText,
            TypeArray typeArgs)
        {
            ErrorType e = new ErrorType();
            e.SetName(name);
            e.nameText = nameText;
            e.typeArgs = typeArgs;
            e.SetTypeParent(parent);
            e.SetNSParent(pParentNS);

            e.SetTypeKind(TypeKind.ErrorType);
            return e;
        }

        // Derived types - parent is base type
        public ArrayType CreateArray(Name name, CType pElementType, int rank)
        {
            ArrayType type = new ArrayType();

            type.SetName(name);
            type.rank = rank;
            type.SetElementType(pElementType);

            type.SetTypeKind(TypeKind.ArrayType);
            return type;
        }

        public PointerType CreatePointer(Name name, CType pReferentType)
        {
            PointerType type = new PointerType();
            type.SetName(name);
            type.SetReferentType(pReferentType);

            type.SetTypeKind(TypeKind.PointerType);
            return type;
        }

        public ParameterModifierType CreateParameterModifier(Name name, CType pParameterType)
        {
            ParameterModifierType type = new ParameterModifierType();
            type.SetName(name);
            type.SetParameterType(pParameterType);

            type.SetTypeKind(TypeKind.ParameterModifierType);
            return type;
        }

        public NullableType CreateNullable(Name name, CType pUnderlyingType, BSYMMGR symmgr, TypeManager typeManager)
        {
            NullableType type = new NullableType();
            type.SetName(name);
            type.SetUnderlyingType(pUnderlyingType);
            type.symmgr = symmgr;
            type.typeManager = typeManager;

            type.SetTypeKind(TypeKind.NullableType);
            return type;
        }
    }
}
