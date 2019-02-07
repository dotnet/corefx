// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace System.Reflection
{
    internal static class ReflectionServices
    {
        public static string GetDisplayName(Type declaringType, string name)
        {
            if(declaringType == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return declaringType.GetDisplayName() + "." + name;
        }

        public static string GetDisplayName(this MemberInfo member)
        {
            if(member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            switch (member.MemberType)
            {
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:

                    return AttributedModelServices.GetTypeIdentity(((Type)member));
            }

            return GetDisplayName(member.DeclaringType, member.Name);
        }

        internal static bool TryGetGenericInterfaceType(Type instanceType, Type targetOpenInterfaceType, out Type targetClosedInterfaceType)
        {
            // The interface must be open
            if(!targetOpenInterfaceType.IsInterface ||
                !targetOpenInterfaceType.IsGenericTypeDefinition ||
                instanceType.IsGenericTypeDefinition)
            {
                throw new Exception(SR.Diagnostic_InternalExceptionMessage);
            }

            // if instanceType is an interface, we must first check it directly
            if (instanceType.IsInterface &&
                instanceType.IsGenericType &&
                instanceType.UnderlyingSystemType.GetGenericTypeDefinition() == targetOpenInterfaceType.UnderlyingSystemType)
            {
                targetClosedInterfaceType = instanceType;
                return true;
            }

            try
            {
                // Purposefully not using FullName here because it results in a significantly
                //  more expensive implementation of GetInterface, this does mean that we're
                //  takign the chance that there aren't too many types which implement multiple
                //  interfaces by the same name...
                Type targetInterface = instanceType.GetInterface(targetOpenInterfaceType.Name, false);
                if (targetInterface != null &&
                    targetInterface.UnderlyingSystemType.GetGenericTypeDefinition() == targetOpenInterfaceType.UnderlyingSystemType)
                {
                    targetClosedInterfaceType = targetInterface;
                    return true;
                }
            }
            catch (AmbiguousMatchException)
            {
                // If there are multiple with the same name we should not pick any
            }

            targetClosedInterfaceType = null;
            return false;
        }

        internal static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            return type.GetInterfaces().Concat(new Type[] { type }).SelectMany(itf => itf.GetProperties());
        }

        internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            IEnumerable<MethodInfo> declaredMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Type baseType = type.BaseType;
            if (baseType.UnderlyingSystemType != typeof(object))
            {
                return declaredMethods.Concat(baseType.GetAllMethods());
            }
            else
            {
                return declaredMethods;
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            IEnumerable<FieldInfo> declaredFields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            Type baseType = type.BaseType;
            if (baseType.UnderlyingSystemType != typeof(object))
            {
                return declaredFields.Concat(baseType.GetAllFields());
            }
            else
            {
                return declaredFields;
            }
        }

        internal static bool HasBaseclassOf(this Type type, Type baseClass)
        {
            if (type == baseClass)
            {
                return false;
            }

            while (type != null)
            {
                if (type == baseClass)
                    return true;
                type = type.BaseType;
            }
            return false;
        }

    }
}
