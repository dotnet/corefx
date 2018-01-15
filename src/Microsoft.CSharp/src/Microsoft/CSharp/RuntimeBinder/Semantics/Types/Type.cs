// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics
{
    internal abstract class CType
    {
        private protected CType(TypeKind kind)
        {
            TypeKind = kind;
        }

        public bool IsWindowsRuntimeType()
        {
            return (AssociatedSystemType.Attributes & TypeAttributes.WindowsRuntime) == TypeAttributes.WindowsRuntime;
        }

        public bool IsCollectionType()
        {
            if ((AssociatedSystemType.IsGenericType &&
                 (AssociatedSystemType.GetGenericTypeDefinition() == typeof(IList<>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                  AssociatedSystemType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))) ||
                AssociatedSystemType == typeof(System.Collections.IList) ||
                AssociatedSystemType == typeof(System.Collections.ICollection) ||
                AssociatedSystemType == typeof(System.Collections.IEnumerable) ||
                AssociatedSystemType == typeof(System.Collections.Specialized.INotifyCollectionChanged) ||
                AssociatedSystemType == typeof(System.ComponentModel.INotifyPropertyChanged))
            {
                return true;
            }
            return false;
        }

        private Type _associatedSystemType;
        public Type AssociatedSystemType
        {
            get
            {
                if (_associatedSystemType == null)
                {
                    _associatedSystemType = CalculateAssociatedSystemType(this);
                }

                return _associatedSystemType;
            }
        }

        private static Type CalculateAssociatedSystemType(CType src)
        {
            Type result = null;

            switch (src.TypeKind)
            {
                case Semantics.TypeKind.TK_ArrayType:
                    ArrayType a = (ArrayType)src;
                    Type elementType = a.ElementType.AssociatedSystemType;
                    result = a.IsSZArray ? elementType.MakeArrayType() : elementType.MakeArrayType(a.Rank);
                    break;

                case Semantics.TypeKind.TK_NullableType:
                    NullableType n = (NullableType)src;
                    Type underlyingType = n.UnderlyingType.AssociatedSystemType;
                    result = typeof(Nullable<>).MakeGenericType(underlyingType);
                    break;

                case Semantics.TypeKind.TK_PointerType:
                    PointerType p = (PointerType)src;
                    result = p.ReferentType.AssociatedSystemType.MakePointerType();
                    break;

                case Semantics.TypeKind.TK_ParameterModifierType:
                    ParameterModifierType r = (ParameterModifierType)src;
                    Type parameterType = r.ParameterType.AssociatedSystemType;
                    result = parameterType.MakeByRefType();
                    break;

                case Semantics.TypeKind.TK_AggregateType:
                    result = CalculateAssociatedSystemTypeForAggregate((AggregateType)src);
                    break;

                case Semantics.TypeKind.TK_TypeParameterType:
                    TypeParameterType t = (TypeParameterType)src;
                    if (t.IsMethodTypeParameter)
                    {
                        MethodInfo meth = ((MethodSymbol)t.OwningSymbol).AssociatedMemberInfo as MethodInfo;
                        result = meth.GetGenericArguments()[t.IndexInOwnParameters];
                    }
                    else
                    {
                        Type parentType = ((AggregateSymbol)t.OwningSymbol).AssociatedSystemType;
                        result = parentType.GetGenericArguments()[t.IndexInOwnParameters];
                    }
                    break;
            }

            Debug.Assert(result != null || src is AggregateType);
            return result;
        }

        private static Type CalculateAssociatedSystemTypeForAggregate(AggregateType aggtype)
        {
            AggregateSymbol agg = aggtype.OwningAggregate;
            TypeArray typeArgs = aggtype.TypeArgsAll;

            List<Type> list = new List<Type>();

            // Get each type arg.
            for (int i = 0; i < typeArgs.Count; i++)
            {
                // Unnamed type parameter types are just placeholders.
                if (typeArgs[i] is TypeParameterType typeParamArg && typeParamArg.Symbol.name == null)
                {
                    return null;
                }
                list.Add(typeArgs[i].AssociatedSystemType);
            }

            Type[] systemTypeArgs = list.ToArray();
            Type uninstantiatedType = agg.AssociatedSystemType;

            if (uninstantiatedType.IsGenericType)
            {
                try
                {
                    return uninstantiatedType.MakeGenericType(systemTypeArgs);
                }
                catch (ArgumentException)
                {
                    // If the constraints don't work, just return the type without substituting it.
                    return uninstantiatedType;
                }
            }
            return uninstantiatedType;
        }

        public TypeKind TypeKind { get; }

        // This call switches on the kind and dispatches accordingly. This should really only be 
        // used when dereferencing TypeArrays. We should consider refactoring our code to not 
        // need this type of thing - strongly typed handling of TypeArrays would be much better.
        public CType GetBaseOrParameterOrElementType()
        {
            switch (TypeKind)
            {
                case Semantics.TypeKind.TK_ArrayType:
                    return ((ArrayType)this).ElementType;

                case Semantics.TypeKind.TK_PointerType:
                    return ((PointerType)this).ReferentType;

                case Semantics.TypeKind.TK_ParameterModifierType:
                    return ((ParameterModifierType)this).ParameterType;

                case Semantics.TypeKind.TK_NullableType:
                    return ((NullableType)this).UnderlyingType;

                default:
                    return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Given a symbol, determine its fundamental type. This is the type that 
        // indicate how the item is stored and what instructions are used to reference 
        // if. The fundamental types are:
        // one of the integral/float types (includes enums with that underlying type)
        // reference type
        // struct/value type
        public FUNDTYPE fundType()
        {
            switch (TypeKind)
            {
                case Semantics.TypeKind.TK_AggregateType:
                    {
                        AggregateSymbol sym = ((AggregateType)this).OwningAggregate;

                        // Treat enums like their underlying types.
                        if (sym.IsEnum())
                        {
                            sym = sym.GetUnderlyingType().OwningAggregate;
                        }

                        if (sym.IsStruct())
                        {
                            // Struct type could be predefined (int, long, etc.) or some other struct.
                            if (sym.IsPredefined())
                                return PredefinedTypeFacts.GetFundType(sym.GetPredefType());
                            return FUNDTYPE.FT_STRUCT;
                        }
                        return FUNDTYPE.FT_REF;  // Interfaces, classes, delegates are reference types.
                    }

                case Semantics.TypeKind.TK_TypeParameterType:
                    return FUNDTYPE.FT_VAR;

                case Semantics.TypeKind.TK_ArrayType:
                case Semantics.TypeKind.TK_NullType:
                    return FUNDTYPE.FT_REF;

                case Semantics.TypeKind.TK_PointerType:
                    return FUNDTYPE.FT_PTR;

                case Semantics.TypeKind.TK_NullableType:
                    return FUNDTYPE.FT_STRUCT;

                default:
                    return FUNDTYPE.FT_NONE;
            }
        }
        public ConstValKind constValKind()
        {
            if (isPointerLike())
            {
                return ConstValKind.IntPtr;
            }

            switch (fundType())
            {
                case FUNDTYPE.FT_I8:
                case FUNDTYPE.FT_U8:
                    return ConstValKind.Long;
                case FUNDTYPE.FT_STRUCT:
                    // Here we can either have a decimal type, or an enum 
                    // whose fundamental type is decimal.
                    Debug.Assert((getAggregate().IsEnum() && getAggregate().GetUnderlyingType().getPredefType() == PredefinedType.PT_DECIMAL)
                        || (isPredefined() && getPredefType() == PredefinedType.PT_DATETIME)
                        || (isPredefined() && getPredefType() == PredefinedType.PT_DECIMAL));

                    if (isPredefined() && getPredefType() == PredefinedType.PT_DATETIME)
                    {
                        return ConstValKind.Long;
                    }
                    return ConstValKind.Decimal;

                case FUNDTYPE.FT_REF:
                    if (isPredefined() && getPredefType() == PredefinedType.PT_STRING)
                    {
                        return ConstValKind.String;
                    }
                    else
                    {
                        return ConstValKind.IntPtr;
                    }
                case FUNDTYPE.FT_R4:
                    return ConstValKind.Float;
                case FUNDTYPE.FT_R8:
                    return ConstValKind.Double;
                case FUNDTYPE.FT_I1:
                    return ConstValKind.Boolean;
                default:
                    return ConstValKind.Int;
            }
        }
        public CType underlyingType()
        {
            if (this is AggregateType at && at.OwningAggregate.IsEnum())
                return at.OwningAggregate.GetUnderlyingType();
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////////
        // Strips off ArrayType, ParameterModifierType, PointerType, PinnedType and optionally NullableType
        // and returns the result.
        public CType GetNakedType(bool fStripNub)
        {
            for (CType type = this; ;)
            {
                switch (type.TypeKind)
                {
                    default:
                        return type;

                    case Semantics.TypeKind.TK_NullableType:
                        if (!fStripNub)
                            return type;
                        type = type.GetBaseOrParameterOrElementType();
                        break;
                    case Semantics.TypeKind.TK_ArrayType:
                    case Semantics.TypeKind.TK_ParameterModifierType:
                    case Semantics.TypeKind.TK_PointerType:
                        type = type.GetBaseOrParameterOrElementType();
                        break;
                }
            }
        }

        public AggregateSymbol getAggregate()
        {
            Debug.Assert(this is AggregateType);
            return ((AggregateType)this).OwningAggregate;
        }

        public virtual CType StripNubs() => this;

        public virtual CType StripNubs(out bool wasNullable)
        {
            wasNullable = false;
            return this;
        }

        public bool isDelegateType()
        {
            return this is AggregateType at && at.OwningAggregate.IsDelegate();
        }

        ////////////////////////////////////////////////////////////////////////////////
        // A few types are considered "simple" types for purposes of conversions and so
        // on. They are the fundamental types the compiler knows about for operators and
        // conversions.
        public bool isSimpleType()
        {
            return (isPredefined() &&
                    PredefinedTypeFacts.IsSimpleType(getPredefType()));
        }
        public bool isSimpleOrEnum()
        {
            return isSimpleType() || isEnumType();
        }
        public bool isSimpleOrEnumOrString()
        {
            return isSimpleType() || isPredefType(PredefinedType.PT_STRING) || isEnumType();
        }

        private bool isPointerLike()
        {
            return this is PointerType || isPredefType(PredefinedType.PT_INTPTR) || isPredefType(PredefinedType.PT_UINTPTR);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // A few types are considered "numeric" types. They are the fundamental number
        // types the compiler knows about for operators and conversions.
        public bool isNumericType()
        {
            return (isPredefined() &&
                    PredefinedTypeFacts.IsNumericType(getPredefType()));
        }
        public bool isStructOrEnum()
        {
            return this is AggregateType at && (at.OwningAggregate.IsStruct() || at.OwningAggregate.IsEnum()) || this is NullableType;
        }
        public bool isStructType()
        {
            return this is AggregateType at && at.OwningAggregate.IsStruct() || this is NullableType;
        }
        public bool isEnumType()
        {
            return this is AggregateType at && at.OwningAggregate.IsEnum();
        }
        public bool isInterfaceType()
        {
            return this is AggregateType at && at.OwningAggregate.IsInterface();
        }
        public bool isClassType()
        {
            return this is AggregateType at && at.OwningAggregate.IsClass();
        }
        public AggregateType underlyingEnumType()
        {
            Debug.Assert(isEnumType());
            return getAggregate().GetUnderlyingType();
        }
        public bool isUnsigned()
        {
            if (this is AggregateType sym)
            {
                if (sym.isEnumType())
                {
                    sym = sym.underlyingEnumType();
                }
                if (sym.isPredefined())
                {
                    PredefinedType pt = sym.getPredefType();
                    return pt == PredefinedType.PT_UINTPTR || pt == PredefinedType.PT_BYTE || (pt >= PredefinedType.PT_USHORT && pt <= PredefinedType.PT_ULONG);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return this is PointerType;
            }
        }
        public bool isUnsafe()
        {
            // Pointer types are the only unsafe types.
            // Note that generics may not be instantiated with pointer types
            return this is PointerType || this is ArrayType arr && arr.BaseElementType.isUnsafe();
        }
        public bool isPredefType(PredefinedType pt)
        {
            if (this is AggregateType ats)
                return ats.OwningAggregate.IsPredefined() && ats.OwningAggregate.GetPredefType() == pt;
            return (this is VoidType && pt == PredefinedType.PT_VOID);
        }
        public bool isPredefined()
        {
            return this is AggregateType at && at.OwningAggregate.IsPredefined();
        }
        public PredefinedType getPredefType()
        {
            //ASSERT(isPredefined());
            return getAggregate().GetPredefType();
        }

        public bool isStaticClass() => (GetNakedType(false) as AggregateType)?.OwningAggregate?.IsStatic() ?? false;

        // These check for AGGTYPESYMs, TYVARSYMs and others as appropriate.
        public bool IsValType()
        {
            switch (TypeKind)
            {
                case Semantics.TypeKind.TK_TypeParameterType:
                    return ((TypeParameterType)this).IsValueType;
                case Semantics.TypeKind.TK_AggregateType:
                    return ((AggregateType)this).OwningAggregate.IsValueType();
                case Semantics.TypeKind.TK_NullableType:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsNonNubValType()
        {
            switch (TypeKind)
            {
                case Semantics.TypeKind.TK_TypeParameterType:
                    return ((TypeParameterType)this).IsNonNullableValueType;
                case Semantics.TypeKind.TK_AggregateType:
                    return ((AggregateType)this).OwningAggregate.IsValueType();
                case Semantics.TypeKind.TK_NullableType:
                    return false;
                default:
                    return false;
            }
        }
        public virtual bool IsReferenceType => false;
    }
}
