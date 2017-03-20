// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace System.Dynamic.Utils
{
    internal static partial class TypeUtils
    {
        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetNullableType(this Type type)
        {
            Debug.Assert(type != null, "type cannot be null");
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNullableOrReferenceType(this Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static bool IsBool(this Type type)
        {
            return GetNonNullableType(type) == typeof(bool);
        }

        public static bool IsNumeric(this Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsInteger(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return false;
            }
            switch (type.GetTypeCode())
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInteger64(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return false;
            }
            switch (type.GetTypeCode())
            {
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArithmetic(this Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsUnsignedInt(this Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsIntegerOrBool(this Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (type.GetTypeCode())
                {
                    case TypeCode.Int64:
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.UInt64:
                    case TypeCode.UInt32:
                    case TypeCode.UInt16:
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        return true;
                }
            }
            return false;
        }

        public static bool IsNumericOrBool(this Type type)
        {
            return IsNumeric(type) || IsBool(type);
        }

        // Checks if the type is a valid target for an instance call
        public static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            Type targetType = member.DeclaringType;
            if (AreReferenceAssignable(targetType, instanceType))
            {
                return true;
            }
            if (targetType == null)
            {
                return false;
            }
            if (instanceType.IsValueType)
            {
                if (AreReferenceAssignable(targetType, typeof(object)))
                {
                    return true;
                }
                if (AreReferenceAssignable(targetType, typeof(ValueType)))
                {
                    return true;
                }
                if (instanceType.IsEnum && AreReferenceAssignable(targetType, typeof(Enum)))
                {
                    return true;
                }
                // A call to an interface implemented by a struct is legal whether the struct has
                // been boxed or not.
                if (targetType.IsInterface)
                {
                    foreach (Type interfaceType in instanceType.GetTypeInfo().ImplementedInterfaces)
                    {
                        if (AreReferenceAssignable(targetType, interfaceType))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasIdentityPrimitiveOrNullableConversionTo(this Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // Identity conversion
            if (AreEquivalent(source, dest))
            {
                return true;
            }

            // Nullable conversions
            if (IsNullableType(source) && AreEquivalent(dest, GetNonNullableType(source)))
            {
                return true;
            }
            if (IsNullableType(dest) && AreEquivalent(source, GetNonNullableType(dest)))
            {
                return true;
            }
            // Primitive runtime conversions
            // All conversions amongst enum, bool, char, integer and float types
            // (and their corresponding nullable types) are legal except for
            // nonbool==>bool and nonbool==>bool? which are only legal from
            // bool-backed enums.
            if (IsConvertible(source) && IsConvertible(dest))
            {
                return GetNonNullableType(dest) != typeof(bool)
                       || source.IsEnum && source.GetEnumUnderlyingType() == typeof(bool);
            }
            return false;
        }

        public static bool HasReferenceConversionTo(this Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || dest == typeof(void))
            {
                return false;
            }

            Type nnSourceType = GetNonNullableType(source);
            Type nnDestType = GetNonNullableType(dest);

            // Down conversion
            if (nnSourceType.IsAssignableFrom(nnDestType))
            {
                return true;
            }
            // Up conversion
            if (nnDestType.IsAssignableFrom(nnSourceType))
            {
                return true;
            }
            // Interface conversion
            if (source.IsInterface || dest.IsInterface)
            {
                return true;
            }
            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, dest))
                return true;

            // Object conversion
            if (source == typeof(object) || dest == typeof(object))
            {
                return true;
            }
            return false;
        }

        private static bool IsCovariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool IsContravariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool IsInvariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }

        private static bool IsDelegate(Type t)
        {
            Debug.Assert(t != null);
            return t.IsSubclassOf(typeof(MulticastDelegate));
        }

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // There *might* be a legal conversion from a generic delegate type S to generic delegate type  T,
            // provided all of the follow are true:
            //   o Both types are constructed generic types of the same generic delegate type, D<X1,... Xk>.
            //     That is, S = D<S1...>, T = D<T1...>.
            //   o If type parameter Xi is declared to be invariant then Si must be identical to Ti.
            //   o If type parameter Xi is declared to be covariant ("out") then Si must be convertible
            //     to Ti via an identify conversion,  implicit reference conversion, or explicit reference conversion.
            //   o If type parameter Xi is declared to be contravariant ("in") then either Si must be identical to Ti,
            //     or Si and Ti must both be reference types.

            if (!IsDelegate(source) || !IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
                return false;

            Type genericDelegate = source.GetGenericTypeDefinition();

            if (dest.GetGenericTypeDefinition() != genericDelegate)
                return false;

            Type[] genericParameters = genericDelegate.GetGenericArguments();
            Type[] sourceArguments = source.GetGenericArguments();
            Type[] destArguments = dest.GetGenericArguments();

            Debug.Assert(genericParameters != null);
            Debug.Assert(sourceArguments != null);
            Debug.Assert(destArguments != null);
            Debug.Assert(genericParameters.Length == sourceArguments.Length);
            Debug.Assert(genericParameters.Length == destArguments.Length);

            for (int iParam = 0; iParam < genericParameters.Length; ++iParam)
            {
                Type sourceArgument = sourceArguments[iParam];
                Type destArgument = destArguments[iParam];

                Debug.Assert(sourceArgument != null && destArgument != null);

                // If the arguments are identical then this one is automatically good, so skip it.
                if (AreEquivalent(sourceArgument, destArgument))
                {
                    continue;
                }

                Type genericParameter = genericParameters[iParam];

                Debug.Assert(genericParameter != null);

                if (IsInvariant(genericParameter))
                {
                    return false;
                }

                if (IsCovariant(genericParameter))
                {
                    if (!sourceArgument.HasReferenceConversionTo(destArgument))
                    {
                        return false;
                    }
                }
                else if (IsContravariant(genericParameter))
                {
                    if (sourceArgument.IsValueType || destArgument.IsValueType)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsConvertible(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return true;
            }
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasReferenceEquality(Type left, Type right)
        {
            if (left.IsValueType || right.IsValueType)
            {
                return false;
            }

            // If we have an interface and a reference type then we can do
            // reference equality.

            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.

            return left.IsInterface || right.IsInterface ||
                AreReferenceAssignable(left, right) ||
                AreReferenceAssignable(right, left);
        }

        public static bool HasBuiltInEqualityOperator(Type left, Type right)
        {
            // If we have an interface and a reference type then we can do
            // reference equality.
            if (left.IsInterface && !right.IsValueType)
            {
                return true;
            }
            if (right.IsInterface && !left.IsValueType)
            {
                return true;
            }
            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.
            if (!left.IsValueType && !right.IsValueType)
            {
                if (AreReferenceAssignable(left, right) || AreReferenceAssignable(right, left))
                {
                    return true;
                }
            }
            // Otherwise, if the types are not the same then we definitely
            // do not have a built-in equality operator.
            if (!AreEquivalent(left, right))
            {
                return false;
            }
            // We have two identical value types, modulo nullability.  (If they were both the
            // same reference type then we would have returned true earlier.)
            Debug.Assert(left.IsValueType);
            // Equality between struct types is only defined for numerics, bools, enums,
            // and their nullable equivalents.
            Type nnType = GetNonNullableType(left);
            if (nnType == typeof(bool) || IsNumeric(nnType) || nnType.IsEnum)
            {
                return true;
            }
            return false;
        }

        public static bool IsImplicitlyConvertibleTo(this Type source, Type destination)
        {
            return AreEquivalent(source, destination) ||                // identity conversion
                IsImplicitNumericConversion(source, destination) ||
                IsImplicitReferenceConversion(source, destination) ||
                IsImplicitBoxingConversion(source, destination) ||
                IsImplicitNullableConversion(source, destination);
        }

        public static MethodInfo GetUserDefinedCoercionMethod(Type convertFrom, Type convertToType)
        {
            Type nnExprType = GetNonNullableType(convertFrom);
            Type nnConvType = GetNonNullableType(convertToType);

            // try exact match on types
            MethodInfo[] eMethods = nnExprType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            MethodInfo method = FindConversionOperator(eMethods, convertFrom, convertToType);
            if (method != null)
            {
                return method;
            }

            MethodInfo[] cMethods = nnConvType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            method = FindConversionOperator(cMethods, convertFrom, convertToType);
            if (method != null)
            {
                return method;
            }

            if (AreEquivalent(nnExprType, convertFrom) && AreEquivalent(nnConvType, convertToType))
            {
                return null;
            }

            // try lifted conversion
            return FindConversionOperator(eMethods, nnExprType, nnConvType)
                   ?? FindConversionOperator(cMethods, nnExprType, nnConvType)
                   ?? FindConversionOperator(eMethods, nnExprType, convertToType)
                   ?? FindConversionOperator(cMethods, nnExprType, convertToType);
        }

        private static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo)
        {
            foreach (MethodInfo mi in methods)
            {
                if (
                    (mi.Name == "op_Implicit" || mi.Name == "op_Explicit")
                    && AreEquivalent(mi.ReturnType, typeTo)
                    )
                {
                    ParameterInfo[] pis = mi.GetParametersCached();
                    if (pis.Length == 1 && AreEquivalent(pis[0].ParameterType, typeFrom))
                    {
                        return mi;
                    }
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static bool IsImplicitNumericConversion(Type source, Type destination)
        {
            TypeCode tcSource = source.GetTypeCode();
            TypeCode tcDest = destination.GetTypeCode();

            switch (tcSource)
            {
                case TypeCode.SByte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Byte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (tcDest)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Char:
                    switch (tcDest)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Single:
                    return (tcDest == TypeCode.Double);
            }
            return false;
        }

        private static bool IsImplicitReferenceConversion(Type source, Type destination)
        {
            return destination.IsAssignableFrom(source);
        }

        private static bool IsImplicitBoxingConversion(Type source, Type destination)
        {
            if (source.IsValueType && (destination == typeof(object) || destination == typeof(ValueType)))
                return true;
            if (source.IsEnum && destination == typeof(Enum))
                return true;
            return false;
        }

        private static bool IsImplicitNullableConversion(Type source, Type destination)
        {
            if (IsNullableType(destination))
                return IsImplicitlyConvertibleTo(GetNonNullableType(source), GetNonNullableType(destination));
            return false;
        }

        public static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsConstructedGenericType && AreEquivalent(type.GetGenericTypeDefinition(), definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type itype in type.GetTypeInfo().ImplementedInterfaces)
                    {
                        Type found = FindGenericType(definition, itype);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Searches for an operator method on the type. The method must have
        /// the specified signature, no generic arguments, and have the
        /// SpecialName bit set. Also searches inherited operator methods.
        ///
        /// NOTE: This was designed to satisfy the needs of op_True and
        /// op_False, because we have to do runtime lookup for those. It may
        /// not work right for unary operators in general.
        /// </summary>
        public static MethodInfo GetBooleanOperator(Type type, string name)
        {
            do
            {
                MethodInfo result = type.GetAnyStaticMethodValidated(name, new Type[] { type });
                if (result != null && result.IsSpecialName && !result.ContainsGenericParameters)
                {
                    return result;
                }
                type = type.BaseType;
            } while (type != null);
            return null;
        }

        public static Type GetNonRefType(this Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

#if FEATURE_COMPILE
        internal static bool IsUnsigned(this Type type) => IsUnsigned(GetNonNullableType(type).GetTypeCode());

        internal static bool IsUnsigned(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsFloatingPoint(this Type type) => IsFloatingPoint(GetNonNullableType(type).GetTypeCode());

        internal static bool IsFloatingPoint(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }
#endif

    }
}
