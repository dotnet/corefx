// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    /////////////////////////////////////////////////////////////////////////////////

    internal sealed class TypeParameterType : CType
    {
        public TypeParameterType(TypeParameterSymbol symbol)
            : base(TypeKind.TK_TypeParameterType)
        {
            Debug.Assert(symbol.GetTypeParameterType() == null);
            TypeParameterSymbol = symbol;
            symbol.SetTypeParameterType(this);
        }

        public TypeParameterSymbol TypeParameterSymbol { get; }

        public ParentSymbol GetOwningSymbol() { return TypeParameterSymbol.parent; }

        // Forward calls into the symbol.
        public Name Name => TypeParameterSymbol.name;

        public bool Covariant { get { return TypeParameterSymbol.Covariant; } }
        public bool Invariant { get { return TypeParameterSymbol.Invariant; } }
        public bool Contravariant { get { return TypeParameterSymbol.Contravariant; } }
        public bool IsValueType() { return TypeParameterSymbol.IsValueType(); }
        public bool IsReferenceType() { return TypeParameterSymbol.IsReferenceType(); }
        public bool IsNonNullableValueType() { return TypeParameterSymbol.IsNonNullableValueType(); }
        public bool HasNewConstraint() { return TypeParameterSymbol.HasNewConstraint(); }
        public bool HasRefConstraint() { return TypeParameterSymbol.HasRefConstraint(); }
        public bool HasValConstraint() { return TypeParameterSymbol.HasValConstraint(); }
        public bool IsMethodTypeParameter() { return TypeParameterSymbol.IsMethodTypeParameter(); }
        public int GetIndexInOwnParameters() { return TypeParameterSymbol.GetIndexInOwnParameters(); }
        public int GetIndexInTotalParameters() { return TypeParameterSymbol.GetIndexInTotalParameters(); }
        public TypeArray GetBounds() { return TypeParameterSymbol.GetBounds(); }
    }
}
