// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

        public bool IsWindowsRuntimeType => (AssociatedSystemType.Attributes & TypeAttributes.WindowsRuntime) != 0;

        [ExcludeFromCodeCoverage] // Should only be called through override.
        public virtual Type AssociatedSystemType => throw Error.InternalCompilerError();

        public TypeKind TypeKind { get; }

        public virtual CType BaseOrParameterOrElementType => null;

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

                    case TypeKind.TK_NullableType:
                        if (!fStripNub)
                        {
                            goto default;
                        }

                        goto case TypeKind.TK_ArrayType;

                    case TypeKind.TK_ArrayType:
                    case TypeKind.TK_ParameterModifierType:
                    case TypeKind.TK_PointerType:
                        type = type.BaseOrParameterOrElementType;
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

        public virtual bool IsUnsigned => false;

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
