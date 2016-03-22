// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Xml.Serialization.Emit
{
    internal static class TypeExtensions
    {
        public static TypeInfo GetTypeInfo(this System.Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).ToReference();
        }

        public static TypeReference GetEnumUnderlyingType(this Type type)
        {
            throw new NotImplementedException();
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            throw new NotImplementedException();
        }

        internal static ConstructorInfo GetConstructor(this TypeReference type, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            var constructorInfos = type.SystemType.GetConstructors(bindingFlags);
            var constructorInfo = ReflectionUtilities.FilterMethodBases(constructorInfos.Cast<Reflection.MethodBase>().ToArray(), parameterTypes.ToSystemTypes(), ".ctor");
            return constructorInfo != null ? ((Reflection.ConstructorInfo)constructorInfo).ToReference() : null;
        }

        public static ConstructorInfo GetConstructor(this TypeReference type, Type[] types)
            => Reflection.TypeExtensions.GetConstructor(type.SystemType, ToSystemTypes(types)).ToReference();

        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
            => GetConstructor((TypeReference)type, types);

        //public static ConstructorInfo[] GetConstructors(this System.Type type);
        //public static ConstructorInfo[] GetConstructors(this System.Type type, BindingFlags bindingAttr);
        //public static MemberInfo[] GetDefaultMembers(this System.Type type);
        //public static EventInfo GetEvent(this System.Type type, string name);
        //public static EventInfo GetEvent(this System.Type type, string name, BindingFlags bindingAttr);
        //public static EventInfo[] GetEvents(this System.Type type);
        //public static EventInfo[] GetEvents(this System.Type type, BindingFlags bindingAttr);
        //public static FieldInfo GetField(this TypeReference type, string name)

        public static FieldInfo GetField(this TypeReference type, string name, BindingFlags bindingAttr)
            => Reflection.TypeExtensions.GetField(type.SystemType, name, bindingAttr).ToReference();

        // public static FieldInfo[] GetFields(this Type type)
        // public static FieldInfo[] GetFields(this Type type, BindingFlags bindingAttr)

        public static Type[] GetGenericArguments(this Type type)
        {
            throw new NotImplementedException();
        }

        public static Type[] GetInterfaces(this Type type)
        {
            throw new NotImplementedException();
        }

        //public static MemberInfo[] GetMember(this Type type, string name)
        //public static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr)
        //public static MemberInfo[] GetMembers(this Type type)
        //public static MemberInfo[] GetMembers(this Type type, BindingFlags bindingAttr)

        // TODO: casting Type -> TypeReference, could be generic instantiation

        public static MethodInfo GetMethod(this Type type, string name)
            => GetMethod((TypeReference)type, name);

        public static MethodInfo GetMethod(this TypeReference type, string name)
            => Reflection.TypeExtensions.GetMethod(type.SystemType, name).ToReference();

        public static MethodInfo GetMethod(this TypeReference type, string name, Type[] types)
            => Reflection.TypeExtensions.GetMethod(type.SystemType, name, types.ToSystemTypes()).ToReference();
        
        internal static MethodInfo GetMethod(this Type type, string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
            => GetMethod((TypeReference)type, methodName, bindingFlags, parameterTypes);

        internal static MethodInfo GetMethod(this TypeReference type, string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            var methodInfos = type.SystemType.GetMethods(bindingFlags);
            var methodInfo = ReflectionUtilities.FilterMethodBases(methodInfos.Cast<Reflection.MethodBase>().ToArray(), parameterTypes.ToSystemTypes(), methodName);
            return methodInfo != null ? ((Reflection.MethodInfo)methodInfo).ToReference() : null;
        }

        private static System.Type[] ToSystemTypes(this Type[] types)
        {
            if (types.Length == 0)
            {
                return Array.Empty<System.Type>();
            }

            var result = new System.Type[types.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ((TypeReference)types[i]).SystemType;
            }

            return result;
        }

        //public static MethodInfo[] GetMethods(this System.Type type);
        //public static MethodInfo[] GetMethods(this System.Type type, BindingFlags bindingAttr);
        //public static System.Type GetNestedType(this System.Type type, string name, BindingFlags bindingAttr);
        //public static System.Type[] GetNestedTypes(this System.Type type, BindingFlags bindingAttr);
        //public static PropertyInfo[] GetProperties(this System.Type type);
        //public static PropertyInfo[] GetProperties(this System.Type type, BindingFlags bindingAttr);

        public static PropertyInfo GetProperty(this Type type, string name)
            => GetProperty((TypeReference)type, name);

        public static PropertyInfo GetProperty(this TypeReference type, string name)
            => Reflection.TypeExtensions.GetProperty(type.SystemType, name).ToReference();

        // public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        // public static PropertyInfo GetProperty(this Type type, string name, System.Type returnType)
        // public static PropertyInfo GetProperty(this Type type, string name, System.Type returnType, System.Type[] types)

        public static bool IsAssignableFrom(this Type type, Type c)
        {
            throw new NotImplementedException();
        }

        // public static bool IsInstanceOfType(this Type type, object o);

        //public static Attribute GetCustomAttribute(this Assembly element, System.Type attributeType);
        //public static Attribute GetCustomAttribute(this MemberInfo element, System.Type attributeType);
        //public static Attribute GetCustomAttribute(this Module element, System.Type attributeType);
        //public static Attribute GetCustomAttribute(this ParameterInfo element, System.Type attributeType);
        //public static Attribute GetCustomAttribute(this MemberInfo element, System.Type attributeType, bool inherit);
        //public static Attribute GetCustomAttribute(this ParameterInfo element, System.Type attributeType, bool inherit);
        //public static T GetCustomAttribute<T>(this Assembly element) where T : Attribute;
        //public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute;
        //public static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute;
        //public static T GetCustomAttribute<T>(this Module element) where T : Attribute;
        //public static T GetCustomAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute;
        //public static T GetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute;
        //public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element);
        //public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element);
        //public static IEnumerable<Attribute> GetCustomAttributes(this Module element);
        //public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element);
        //public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, bool inherit);
        //public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element, System.Type attributeType);
        //public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, bool inherit);
        //public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, System.Type attributeType);
        //public static IEnumerable<Attribute> GetCustomAttributes(this Module element, System.Type attributeType);
        //public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, System.Type attributeType);
        //public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, System.Type attributeType, bool inherit);
        //public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, System.Type attributeType, bool inherit)
        //public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute;
        //public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute;
        //public static IEnumerable<T> GetCustomAttributes<T>(this Module element) where T : Attribute;
        //public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element) where T : Attribute;
        //public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit) where T : Attribute;
        //public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element, bool inherit) where T : Attribute;
        //public static bool IsDefined(this Assembly element, System.Type attributeType);
        //public static bool IsDefined(this MemberInfo element, System.Type attributeType);
        //public static bool IsDefined(this Module element, System.Type attributeType);
        //public static bool IsDefined(this ParameterInfo element, System.Type attributeType);
        //public static bool IsDefined(this MemberInfo element, System.Type attributeType, bool inherit);
        //public static bool IsDefined(this ParameterInfo element, System.Type attributeType, bool inherit);
    }
}
