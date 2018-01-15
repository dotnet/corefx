// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
                    Debug.Assert((getAggregate().IsEnum() && getAggregate().GetUnderlyingType().PredefinedType == Syntax.PredefinedType.PT_DECIMAL)
                        || (IsPredefined && PredefinedType == Syntax.PredefinedType.PT_DATETIME)
                        || (IsPredefined && PredefinedType == Syntax.PredefinedType.PT_DECIMAL));

                    if (IsPredefined && PredefinedType == Syntax.PredefinedType.PT_DATETIME)
                    {
                        return ConstValKind.Long;
                    }
                    return ConstValKind.Decimal;

                case FUNDTYPE.FT_REF:
                    if (IsPredefined && PredefinedType == Syntax.PredefinedType.PT_STRING)
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

        public virtual bool IsDelegateType => false;

        ////////////////////////////////////////////////////////////////////////////////
        // A few types are considered "simple" types for purposes of conversions and so
        // on. They are the fundamental types the compiler knows about for operators and
        // conversions.
        public virtual bool IsSimpleType => false;

        public virtual bool IsSimpleOrEnum => false;

        public virtual bool IsSimpleOrEnumOrString => false;

        private bool isPointerLike()
        {
            return this is PointerType || IsPredefType(Syntax.PredefinedType.PT_INTPTR) || IsPredefType(Syntax.PredefinedType.PT_UINTPTR);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // A few types are considered "numeric" types. They are the fundamental number
        // types the compiler knows about for operators and conversions.
        public virtual bool IsNumericType => false;

        public virtual bool IsStructOrEnum => false;

        public virtual bool IsStructType => false;

        public virtual bool IsEnumType => false;

        public virtual bool IsInterfaceType => false;

        public virtual bool IsClassType => false;

        [ExcludeFromCodeCoverage] // Should only be called through override.
        public virtual AggregateType UnderlyingEnumType => throw Error.InternalCompilerError();

        public bool isUnsigned()
        {
            if (this is AggregateType sym)
            {
                if (sym.IsEnumType)
                {
                    sym = sym.UnderlyingEnumType;
                }

                if (sym.IsPredefined)
                {
                    PredefinedType pt = sym.PredefinedType;
                    return pt == Syntax.PredefinedType.PT_UINTPTR || pt == Syntax.PredefinedType.PT_BYTE || (pt >= Syntax.PredefinedType.PT_USHORT && pt <= Syntax.PredefinedType.PT_ULONG);
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

        // Pointer types (or arrays of them) are the only unsafe types.
        // Note that generics may not be instantiated with pointer types
        public virtual bool IsUnsafe() => false;

        public virtual bool IsPredefType(PredefinedType pt) => false;

        public virtual bool IsPredefined => false;

        [ExcludeFromCodeCoverage] // Should only be called through override.
        public virtual PredefinedType PredefinedType => throw Error.InternalCompilerError();

        public virtual bool IsStaticClass => false;

        // These check for AGGTYPESYMs, TYVARSYMs and others as appropriate.
        public virtual bool IsValueType => false;

        public virtual bool IsNonNullableValueType => false;

        public virtual bool IsReferenceType => false;
    }
}
