// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Collections.Generic;

namespace System.Dynamic.Utils
{
    // Extensions on System.Type and friends
    internal static partial class TypeExtensions
    {
        // Returns the matching method if the parameter types are reference
        // assignable from the provided type arguments, otherwise null. 
        public static MethodInfo GetAnyStaticMethodValidated(
            this Type type,
            string name,
            Type[] types)
        {
            var method = type.GetAnyStaticMethod(name);

            return method.MatchesArgumentTypes(types) ? method : null;
        }

        /// <summary>
        /// Returns true if the method's parameter types are reference assignable from
        /// the argument types, otherwise false.
        /// 
        /// An example that can make the method return false is that 
        /// typeof(double).GetMethod("op_Equality", ..., new[] { typeof(double), typeof(int) })
        /// returns a method with two double parameters, which doesn't match the provided
        /// argument types.
        /// </summary>
        /// <returns></returns>
        private static bool MatchesArgumentTypes(this MethodInfo mi, Type[] argTypes)
        {
            if (mi == null || argTypes == null)
            {
                return false;
            }
            var ps = mi.GetParameters();

            if (ps.Length != argTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < ps.Length; i++)
            {
                if (!TypeUtils.AreReferenceAssignable(ps[i].ParameterType, argTypes[i]))
                {
                    return false;
                }
            }
            return true;
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

        public static MethodInfo[] GetStaticMethods(this Type type)
        {
            var list = new List<MethodInfo>();
            foreach (var method in type.GetRuntimeMethods())
            {
                if (method.IsStatic)
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }

        public static MethodInfo GetAnyStaticMethod(this Type type, string name)
        {
            foreach (var method in type.GetRuntimeMethods())
            {
                if (method.IsStatic && method.Name == name)
                {
                    return method;
                }
            }
            return null;
        }

        public static MethodInfo[] GetMethodsIgnoreCase(this Type type, BindingFlags flags, string name)
        {
            var list = new List<MethodInfo>();
            foreach (var method in type.GetRuntimeMethods())
            {
                // TODO: Binding flags filter
                if (method.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(method);
                }
            }
            return list.ToArray();
        }
    }
}
