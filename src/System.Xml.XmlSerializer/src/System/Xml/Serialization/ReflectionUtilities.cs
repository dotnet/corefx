// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Xml.Serialization
{
    internal static class ReflectionUtilities
    {
        public static bool TypeSequenceEqual(Type[] seq1, Type[] seq2)
        {
            if (seq1 == null || seq2 == null || seq1.Length != seq2.Length)
                return false;
            for (int i = 0; i < seq1.Length; i++)
            {
                if (!seq1[i].Equals(seq2[i]) && !seq1[i].IsAssignableFrom(seq2[i]))
                    return false;
            }
            return true;
        }

        public static MethodBase FilterMethodBases(MethodBase[] methodBases, Type[] parameterTypes, string methodName)
        {
            if (methodBases == null || string.IsNullOrEmpty(methodName))
                return null;

            var matchedMethods = methodBases.Where(method => method.Name.Equals(methodName));
            matchedMethods = matchedMethods.Where(method => TypeSequenceEqual(method.GetParameters().Select(param => param.ParameterType).ToArray(), parameterTypes));
            return matchedMethods.FirstOrDefault();
        }

        private struct ModuleMemberMap
        {
            public readonly Emit.Module Module;
            public readonly Dictionary<MemberInfo, Emit.MemberInfo> Map;

            public ModuleMemberMap(Emit.Module module, Dictionary<MemberInfo, Emit.MemberInfo> map)
            {
                Module = module;
                Map = map;
            }
        }

        // TODO: better map (weak)
        private static readonly Dictionary<Module, ModuleMemberMap> s_map = new Dictionary<Module, ModuleMemberMap>();
        
        public static Emit.TypeReference ToReference(this Type type) => ToReference(type.GetTypeInfo());
        public static Emit.TypeReference ToReference(this TypeInfo member) => (Emit.TypeReference)ToReference((MemberInfo)member);
        public static Emit.FieldReference ToReference(this FieldInfo member) => (Emit.FieldReference)ToReference((MemberInfo)member);
        public static Emit.MethodReference ToReference(this MethodInfo member) => (Emit.MethodReference)ToReference((MemberInfo)member);
        public static Emit.ConstructorReference ToReference(this ConstructorInfo member) => (Emit.ConstructorReference)ToReference((MemberInfo)member);
        public static Emit.PropertyReference ToReference(this PropertyInfo member) => (Emit.PropertyReference)ToReference((MemberInfo)member);

        public static Emit.MemberInfo ToReference(this MemberInfo member)
        {
            Emit.MemberInfo memberRef;
            ModuleMemberMap moduleMap;
            if (!s_map.TryGetValue(member.Module, out moduleMap))
            {
                moduleMap = new ModuleMemberMap(new Emit.ModuleReference(member.Module), new Dictionary<MemberInfo, Emit.MemberInfo>());
                s_map.Add(member.Module, moduleMap); 
            }
            else if (moduleMap.Map.TryGetValue(member, out memberRef))
            {
                return memberRef;
            }

            var type = member as TypeInfo;
            if (type != null)
            {
                // generic type parameter can't have cross-assembly reference:
                if (type.IsGenericParameter)
                {
                    throw new InvalidOperationException();
                }

                // top-level type:
                if (type.DeclaringType == null)
                {
                    return new Emit.TypeReference(moduleMap.Module, type);
                }
            }

            var containingTypeRef = member.DeclaringType?.GetTypeInfo().ToReference();
            if (containingTypeRef == null)
            {
                // global member:
                throw new NotSupportedException();
            }

            var method = member as MethodInfo;
            if (method != null)
            {
                return new Emit.MethodReference(containingTypeRef, method);
            }

            var field = member as FieldInfo;
            if (field != null)
            {
                return new Emit.FieldReference(containingTypeRef, field);
            }

            var ctor = member as ConstructorInfo;
            if (ctor != null)
            {
                return new Emit.ConstructorReference(containingTypeRef, ctor);
            }

            var property = member as PropertyInfo;
            if (property != null)
            {
                return new Emit.PropertyReference(containingTypeRef, property);
            }

            throw Emit.ExceptionUtilities.Unreachable;
        }
    }
}
