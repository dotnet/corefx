// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

[assembly:System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Reflection.BindingFlags))]

namespace System.Reflection
{
    public static partial class AssemblyExtensions
    {
        public static System.Type[] GetExportedTypes(System.Reflection.Assembly assembly) { throw null; }
        public static System.Reflection.Module[] GetModules(System.Reflection.Assembly assembly) { throw null; }
        public static System.Type[] GetTypes(System.Reflection.Assembly assembly) { throw null; }
    }
    public static partial class EventInfoExtensions
    {
        public static System.Reflection.MethodInfo GetAddMethod(System.Reflection.EventInfo eventInfo) { throw null; }
        public static System.Reflection.MethodInfo GetAddMethod(System.Reflection.EventInfo eventInfo, bool nonPublic) { throw null; }
        public static System.Reflection.MethodInfo GetRaiseMethod(System.Reflection.EventInfo eventInfo) { throw null; }
        public static System.Reflection.MethodInfo GetRaiseMethod(System.Reflection.EventInfo eventInfo, bool nonPublic) { throw null; }
        public static System.Reflection.MethodInfo GetRemoveMethod(System.Reflection.EventInfo eventInfo) { throw null; }
        public static System.Reflection.MethodInfo GetRemoveMethod(System.Reflection.EventInfo eventInfo, bool nonPublic) { throw null; }
    }
    public static partial class MemberInfoExtensions
    {
        public static int GetMetadataToken(this System.Reflection.MemberInfo member) { throw null; }
        public static bool HasMetadataToken(this System.Reflection.MemberInfo member) { throw null; }
    }
    public static partial class MethodInfoExtensions
    {
        public static System.Reflection.MethodInfo GetBaseDefinition(System.Reflection.MethodInfo method) { throw null; }
    }
    public static partial class ModuleExtensions
    {
        public static System.Guid GetModuleVersionId(this System.Reflection.Module module) { throw null; }
        public static bool HasModuleVersionId(this System.Reflection.Module module) { throw null; }
    }
    public static partial class PropertyInfoExtensions
    {
        public static System.Reflection.MethodInfo[] GetAccessors(System.Reflection.PropertyInfo property) { throw null; }
        public static System.Reflection.MethodInfo[] GetAccessors(System.Reflection.PropertyInfo property, bool nonPublic) { throw null; }
        public static System.Reflection.MethodInfo GetGetMethod(System.Reflection.PropertyInfo property) { throw null; }
        public static System.Reflection.MethodInfo GetGetMethod(System.Reflection.PropertyInfo property, bool nonPublic) { throw null; }
        public static System.Reflection.MethodInfo GetSetMethod(System.Reflection.PropertyInfo property) { throw null; }
        public static System.Reflection.MethodInfo GetSetMethod(System.Reflection.PropertyInfo property, bool nonPublic) { throw null; }
    }
    public static partial class TypeExtensions
    {
        public static System.Reflection.ConstructorInfo GetConstructor(System.Type type, System.Type[] types) { throw null; }
        public static System.Reflection.ConstructorInfo[] GetConstructors(System.Type type) { throw null; }
        public static System.Reflection.ConstructorInfo[] GetConstructors(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.MemberInfo[] GetDefaultMembers(System.Type type) { throw null; }
        public static System.Reflection.EventInfo GetEvent(System.Type type, string name) { throw null; }
        public static System.Reflection.EventInfo GetEvent(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.EventInfo[] GetEvents(System.Type type) { throw null; }
        public static System.Reflection.EventInfo[] GetEvents(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.FieldInfo GetField(System.Type type, string name) { throw null; }
        public static System.Reflection.FieldInfo GetField(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.FieldInfo[] GetFields(System.Type type) { throw null; }
        public static System.Reflection.FieldInfo[] GetFields(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Type[] GetGenericArguments(System.Type type) { throw null; }
        public static System.Type[] GetInterfaces(System.Type type) { throw null; }
        public static System.Reflection.MemberInfo[] GetMember(System.Type type, string name) { throw null; }
        public static System.Reflection.MemberInfo[] GetMember(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.MemberInfo[] GetMembers(System.Type type) { throw null; }
        public static System.Reflection.MemberInfo[] GetMembers(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.MethodInfo GetMethod(System.Type type, string name) { throw null; }
        public static System.Reflection.MethodInfo GetMethod(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.MethodInfo GetMethod(System.Type type, string name, System.Type[] types) { throw null; }
        public static System.Reflection.MethodInfo[] GetMethods(System.Type type) { throw null; }
        public static System.Reflection.MethodInfo[] GetMethods(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Type GetNestedType(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Type[] GetNestedTypes(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.PropertyInfo[] GetProperties(System.Type type) { throw null; }
        public static System.Reflection.PropertyInfo[] GetProperties(System.Type type, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.PropertyInfo GetProperty(System.Type type, string name) { throw null; }
        public static System.Reflection.PropertyInfo GetProperty(System.Type type, string name, System.Reflection.BindingFlags bindingAttr) { throw null; }
        public static System.Reflection.PropertyInfo GetProperty(System.Type type, string name, System.Type returnType) { throw null; }
        public static System.Reflection.PropertyInfo GetProperty(System.Type type, string name, System.Type returnType, System.Type[] types) { throw null; }
        public static bool IsAssignableFrom(System.Type type, System.Type c) { throw null; }
        public static bool IsInstanceOfType(System.Type type, object o) { throw null; }
    }
}
