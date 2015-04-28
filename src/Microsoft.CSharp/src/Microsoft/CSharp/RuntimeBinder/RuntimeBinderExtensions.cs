﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder
{
    internal static class RuntimeBinderExtensions
    {
        public static bool IsEquivalentTo(this Type t1, Type t2)
        {
            return t1 == t2;
        }

        public static bool IsNullableType(this Type type)
        {
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
                return TypeCode.Empty;
            else if (type == typeof(bool))
                return TypeCode.Boolean;
            else if (type == typeof(char))
                return TypeCode.Char;
            else if (type == typeof(sbyte))
                return TypeCode.SByte;
            else if (type == typeof(byte))
                return TypeCode.Byte;
            else if (type == typeof(short))
                return TypeCode.Int16;
            else if (type == typeof(ushort))
                return TypeCode.UInt16;
            else if (type == typeof(int))
                return TypeCode.Int32;
            else if (type == typeof(uint))
                return TypeCode.UInt32;
            else if (type == typeof(long))
                return TypeCode.Int64;
            else if (type == typeof(ulong))
                return TypeCode.UInt64;
            else if (type == typeof(float))
                return TypeCode.Single;
            else if (type == typeof(double))
                return TypeCode.Double;
            else if (type == typeof(decimal))
                return TypeCode.Decimal;
            else if (type == typeof(System.DateTime))
                return TypeCode.DateTime;
            else if (type == typeof(string))
                return TypeCode.String;
            else if (type.GetTypeInfo().IsEnum)
                return GetTypeCode(Enum.GetUnderlyingType(type));
            else
                return TypeCode.Object;
        }

        // This method is intended as a means to detect when MemberInfos are the same,
        // modulo the fact that they can appear to have different but equivalent local
        // No-PIA types. It is by the symbol table to determine whether
        // or not members have been added to an AggSym or not.
        public static bool IsEquivalentTo(this MemberInfo mi1, MemberInfo mi2)
        {
            if (mi1 == null || mi2 == null)
            {
                return mi1 == null && mi2 == null;
            }

#if UNSUPPORTEDAPI
            if (mi1 == mi2 || (mi1.DeclaringType.IsGenericallyEqual(mi2.DeclaringType) && mi1.MetadataToken == mi2.MetadataToken))
#else
            if (mi1.Equals(mi2))
#endif
            {
                return true;
            }

            if (mi1 is MethodInfo && mi2 is MethodInfo)
            {
                MethodInfo method1 = mi1 as MethodInfo;
                MethodInfo method2 = mi2 as MethodInfo;
                ParameterInfo[] pis1;
                ParameterInfo[] pis2;

                if (method1.IsGenericMethod != method2.IsGenericMethod)
                {
                    return false;
                }

                if (method1.IsGenericMethod)
                {
                    method1 = method1.GetGenericMethodDefinition();
                    method2 = method2.GetGenericMethodDefinition();

                    if (method1.GetGenericArguments().Length != method2.GetGenericArguments().Length)
                    {
                        return false; // Methods of different arity are not equivalent.
                    }
                }

                return method1 != method2
                    && method1.Name == method2.Name
                    && method1.DeclaringType.IsGenericallyEqual(method2.DeclaringType)
                    && method1.ReturnType.IsGenericallyEquivalentTo(method2.ReturnType, method1, method2)
                    && (pis1 = method1.GetParameters()).Length == (pis2 = method2.GetParameters()).Length
                    && Enumerable.All(Enumerable.Zip(pis1, pis2, (pi1, pi2) => pi1.IsEquivalentTo(pi2, method1, method2)), x => x);
            }

            if (mi1 is ConstructorInfo && mi2 is ConstructorInfo)
            {
                ConstructorInfo ctor1 = mi1 as ConstructorInfo;
                ConstructorInfo ctor2 = mi2 as ConstructorInfo;
                ParameterInfo[] pis1;
                ParameterInfo[] pis2;

                return ctor1 != ctor2
                    && ctor1.DeclaringType.IsGenericallyEqual(ctor2.DeclaringType)
                    && (pis1 = ctor1.GetParameters()).Length == (pis2 = ctor2.GetParameters()).Length
                    && Enumerable.All(Enumerable.Zip(pis1, pis2, (pi1, pi2) => pi1.IsEquivalentTo(pi2, ctor1, ctor2)), x => x);
            }

            if (mi1 is PropertyInfo && mi2 is PropertyInfo)
            {
                PropertyInfo prop1 = mi1 as PropertyInfo;
                PropertyInfo prop2 = mi2 as PropertyInfo;

                return prop1 != prop2
                    && prop1.Name == prop2.Name
                    && prop1.DeclaringType.IsGenericallyEqual(prop2.DeclaringType)
                    && prop1.PropertyType.IsGenericallyEquivalentTo(prop2.PropertyType, prop1, prop2)
                    && prop1.GetGetMethod(true).IsEquivalentTo(prop2.GetGetMethod(true))
                    && prop1.GetSetMethod(true).IsEquivalentTo(prop2.GetSetMethod(true));
            }

            return false;
        }

        private static bool IsEquivalentTo(this ParameterInfo pi1, ParameterInfo pi2, MethodBase method1, MethodBase method2)
        {
            if (pi1 == null || pi2 == null)
            {
                return pi1 == null && pi2 == null;
            }

            if (pi1.Equals(pi2))
            {
                return true;
            }

            return pi1.ParameterType.IsGenericallyEquivalentTo(pi2.ParameterType, method1, method2);
        }

        private static bool IsGenericallyEqual(this Type t1, Type t2)
        {
            if (t1 == null || t2 == null)
            {
                return t1 == null && t2 == null;
            }

            if (t1.Equals(t2))
            {
                return true;
            }

            if (t1.IsConstructedGenericType || t2.IsConstructedGenericType)
            {
                Type t1def = t1.IsConstructedGenericType ? t1.GetGenericTypeDefinition() : t1;
                Type t2def = t2.IsConstructedGenericType ? t2.GetGenericTypeDefinition() : t2;

                return t1def.Equals(t2def);
            }

            return false;
        }

        // Compares two types and calls them equivalent if a type parameter equals a type argument.
        // i.e if the inputs are (T, int, C<T>, C<int>) then this will return true.
        private static bool IsGenericallyEquivalentTo(this Type t1, Type t2, MemberInfo member1, MemberInfo member2)
        {
            Debug.Assert(!(member1 is MethodBase) ||
                         !((MethodBase)member1).IsGenericMethod ||
                         (((MethodBase)member1).IsGenericMethodDefinition && ((MethodBase)member2).IsGenericMethodDefinition));

            if (t1.Equals(t2))
            {
                return true;
            }

            // If one of them is a type param and then the other is a real type, then get the type argument in the member
            // or it's declaring type that corresponds to the type param and compare that to the other type.
            if (t1.IsGenericParameter)
            {
                if (t2.IsGenericParameter)
                {
                    // If member's declaring type is not type parameter's declaring type, we assume that it is used as a type argument
                    if (t1.GetTypeInfo().DeclaringMethod == null && member1.DeclaringType.Equals(t1.GetTypeInfo().DeclaringType))
                    {
                        if (!(t2.GetTypeInfo().DeclaringMethod == null && member2.DeclaringType.Equals(t2.GetTypeInfo().DeclaringType)))
                        {
                            return t1.IsTypeParameterEquivalentToTypeInst(t2, member2);
                        }
                    }
                    else if (t2.GetTypeInfo().DeclaringMethod == null && member2.DeclaringType.Equals(t2.GetTypeInfo().DeclaringType))
                    {
                        return t2.IsTypeParameterEquivalentToTypeInst(t1, member1);
                    }

                    // If both of these are type params but didn't compare to be equal then one of them is likely bound to another
                    // open type. Simply disallow such cases.
                    return false;
                }

                return t1.IsTypeParameterEquivalentToTypeInst(t2, member2);
            }
            else if (t2.IsGenericParameter)
            {
                return t2.IsTypeParameterEquivalentToTypeInst(t1, member1);
            }

            // Recurse in for generic types arrays, byref and pointer types.
            if (t1.GetTypeInfo().IsGenericType && t2.GetTypeInfo().IsGenericType)
            {
                var args1 = t1.GetGenericArguments();
                var args2 = t2.GetGenericArguments();

                if (args1.Length == args2.Length)
                {
                    return t1.IsGenericallyEqual(t2) &&
                           Enumerable.All(Enumerable.Zip(args1, args2, (ta1, ta2) => ta1.IsGenericallyEquivalentTo(ta2, member1, member2)), x => x);
                }
            }

            if (t1.IsArray && t2.IsArray)
            {
                return t1.GetArrayRank() == t2.GetArrayRank() &&
                       t1.GetElementType().IsGenericallyEquivalentTo(t2.GetElementType(), member1, member2);
            }

            if ((t1.IsByRef && t2.IsByRef) ||
                (t1.IsPointer && t2.IsPointer))
            {
                return t1.GetElementType().IsGenericallyEquivalentTo(t2.GetElementType(), member1, member2);
            }

            return false;
        }

        private static bool IsTypeParameterEquivalentToTypeInst(this Type typeParam, Type typeInst, MemberInfo member)
        {
            Debug.Assert(typeParam.IsGenericParameter);

            if (typeParam.GetTypeInfo().DeclaringMethod != null)
            {
                // The type param is from a generic method. Since only methods can be generic, anything else
                // here means they are not equivalent.
                if (!(member is MethodBase))
                {
                    return false;
                }

                MethodBase method = (MethodBase)member;
                int position = typeParam.GetTypeInfo().GenericParameterPosition;
                Type[] args = method.IsGenericMethod ? method.GetGenericArguments() : null;

                return args != null &&
                       args.Length > position &&
                       args[position].Equals(typeInst);
            }
            else
            {
                return member.DeclaringType.GetGenericArguments()[typeParam.GetTypeInfo().GenericParameterPosition].Equals(typeInst);
            }
        }


        private static readonly Func<MemberInfo, int> s_GetMetadataTokenSentinel = mi => { throw new InvalidOperationException(); };
        private static Func<MemberInfo, int> s_GetMetadataToken = s_GetMetadataTokenSentinel;

        public static bool HasSameMetadataDefinitionAs(this MemberInfo mi1, MemberInfo mi2)
        {
#if UNSUPPORTEDAPI
            return (mi1.MetadataToken == mi2.MetadataToken) && (mi1.Module == mi2.Module));
#else
            if (!mi1.Module.Equals(mi2.Module))
            {
                return false;
            }

            if ((object)s_GetMetadataToken == (object)s_GetMetadataTokenSentinel)
            {
                // See if MetadataToken property is available.
                Type memberInfo = typeof(MemberInfo);
                PropertyInfo property = memberInfo.GetProperty("MetadataToken", typeof(int), Array.Empty<Type>());

                if ((object)property == null || !property.CanRead)
                {
                    s_GetMetadataToken = null;
                }
                else
                {
                    var parameter = Expression.Parameter(memberInfo);
                    s_GetMetadataToken = Expression.Lambda<Func<MemberInfo, int>>(Expression.Property(parameter, property), new[] { parameter }).Compile();
                }
            }

            if ((object)s_GetMetadataToken != null)
            {
                return s_GetMetadataToken(mi1) == s_GetMetadataToken(mi2);
            }

            return mi1.IsEquivalentTo(mi2);
#endif
        }
    }
}