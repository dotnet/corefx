// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Why this file exists:
//
// Because the Reflection base types have so many overridable members, it becomes impossible to distinguish
// members we decided not to override vs. those we forgot to override. It would be nice if C# had a construct to 
// tell the reader (and Intellisense) that we've made an explicit decision *not* to override an inherited member, 
// but since it doesn't, we'll make do with this instead.
//
// In DEBUG builds, we'll add a base-delegating override so that it's clear we made an explicit decision
// to accept the base class's implementation and that we don't get Intelli-spammed with the same dozen override suggestions
// every time we add a new subclass or a new abstract to an existing base class. In RELEASE builds, we'll #if'd 
// these out to avoid the extra metadata and runtime cost. That way, every overridable member is accounted 
// for (i.e. the codebase should always be kept in a state where hitting "override" + SPACE never brings up 
// additional suggestions in Intellisense.)
//
// We also do this for types that override abstract members with another abstract override to keep their own Intellisense spam-free.
//
// Why this file is named "UnoverriddenApis":
//
// Because "U" appears late in an alphabetical sort. So when I press F12 to go to a type definition, this file doesn't jump
// ahead of the ones I'm much more likely to be interested in.
//

using System.IO;
using System.Security;
using System.Collections.Generic;

namespace System.Reflection.TypeLoading
{
    internal abstract partial class RoAssembly
    {
#if DEBUG
        // RoAssembly objects are unified in the TypeLoader object. So reference equality is the same as semantic equality.
        public sealed override bool Equals(object o) => base.Equals(o);
        public sealed override int GetHashCode() => base.GetHashCode();

        public sealed override FileStream[] GetFiles() => base.GetFiles();
        public sealed override AssemblyName GetName() => base.GetName();
        public sealed override Type GetType(string name) => base.GetType(name);
        public sealed override Type GetType(string name, bool throwOnError) => base.GetType(name, throwOnError);
        public sealed override IEnumerable<Module> Modules => base.Modules;
        public sealed override SecurityRuleSet SecurityRuleSet => base.SecurityRuleSet;
#endif //DEBUG
    }

    internal abstract partial class RoModule
    {
#if DEBUG
        // RoModule objects are unified in the RoAssembly object. So reference equality is the same as semantic equality.
        public sealed override bool Equals(object o) => base.Equals(o);
        public sealed override int GetHashCode() => base.GetHashCode();

        public sealed override Type GetType(string className) => base.GetType(className);
        public sealed override Type GetType(string className, bool ignoreCase) => base.GetType(className, ignoreCase);
        public sealed override Type[] FindTypes(TypeFilter filter, object filterCriteria) => base.FindTypes(filter, filterCriteria);

#endif //DEBUG
    }

    internal abstract partial class RoType
    {
#if DEBUG
        // RoTypes objects are unified for desktop compat. So reference equality is the same as semantic equality.
        public sealed override bool Equals(object o) => base.Equals(o);
        public sealed override bool Equals(Type o) => base.Equals(o);
        public sealed override int GetHashCode() => base.GetHashCode();

        public sealed override Type[] FindInterfaces(TypeFilter filter, object filterCriteria) => base.FindInterfaces(filter, filterCriteria);
        public sealed override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria) => base.FindMembers(memberType, bindingAttr, filter, filterCriteria);
        public sealed override EventInfo[] GetEvents() => base.GetEvents();
        protected sealed override bool IsContextfulImpl() => base.IsContextfulImpl();
        public sealed override bool IsSubclassOf(Type c) => base.IsSubclassOf(c);
        protected sealed override bool IsMarshalByRefImpl() => base.IsMarshalByRefImpl();
        public sealed override bool IsInstanceOfType(object o) => base.IsInstanceOfType(o);
        public sealed override bool IsSerializable => base.IsSerializable;
        public sealed override bool IsEquivalentTo(Type other) => base.IsEquivalentTo(other); // Note: If we enable COM type equivalence, this is no longer the correct implementation.
        public sealed override bool IsSignatureType => base.IsSignatureType;

        public sealed override IEnumerable<ConstructorInfo> DeclaredConstructors => base.DeclaredConstructors;
        public sealed override IEnumerable<EventInfo> DeclaredEvents => base.DeclaredEvents;
        public sealed override IEnumerable<FieldInfo> DeclaredFields => base.DeclaredFields;
        public sealed override IEnumerable<MemberInfo> DeclaredMembers => base.DeclaredMembers;
        public sealed override IEnumerable<MethodInfo> DeclaredMethods => base.DeclaredMethods;
        public sealed override IEnumerable<TypeInfo> DeclaredNestedTypes => base.DeclaredNestedTypes;
        public sealed override IEnumerable<PropertyInfo> DeclaredProperties => base.DeclaredProperties;

        public sealed override EventInfo GetDeclaredEvent(string name) => base.GetDeclaredEvent(name);
        public sealed override FieldInfo GetDeclaredField(string name) => base.GetDeclaredField(name);
        public sealed override MethodInfo GetDeclaredMethod(string name) => base.GetDeclaredMethod(name);
        public sealed override TypeInfo GetDeclaredNestedType(string name) => base.GetDeclaredNestedType(name);
        public sealed override PropertyInfo GetDeclaredProperty(string name) => base.GetDeclaredProperty(name);

        public sealed override IEnumerable<MethodInfo> GetDeclaredMethods(string name) => base.GetDeclaredMethods(name);

        public sealed override string GetEnumName(object value) => base.GetEnumName(value);
        public sealed override string[] GetEnumNames() => base.GetEnumNames();
        public sealed override bool IsEnumDefined(object value) => base.IsEnumDefined(value);
#endif //DEBUG
    }

    internal abstract partial class RoDefinitionType
    {
#if DEBUG
        public abstract override bool IsGenericTypeDefinition { get; }
        internal abstract override RoModule GetRoModule();
        protected abstract override string ComputeName();
        protected abstract override string ComputeNamespace();
        protected abstract override TypeAttributes ComputeAttributeFlags();
        protected abstract override RoType ComputeDeclaringType();
        internal abstract override bool IsCustomAttributeDefined(ReadOnlySpan<byte> ns, ReadOnlySpan<byte> name);
        internal abstract override CustomAttributeData TryFindCustomAttribute(ReadOnlySpan<byte> ns, ReadOnlySpan<byte> name);
        public abstract override int MetadataToken { get; }
        protected internal abstract override RoType ComputeEnumUnderlyingType();
        internal abstract override IEnumerable<RoType> GetNestedTypesCore(NameFilter filter);
#endif //DEBUG
    }

    internal abstract partial class RoHasElementType
    {
#if DEBUG
        protected abstract override bool IsArrayImpl();
        public abstract override bool IsSZArray { get; }
        public abstract override bool IsVariableBoundArray { get; }
        protected abstract override bool IsByRefImpl();
        protected abstract override bool IsPointerImpl();
        public abstract override int GetArrayRank();
        protected abstract override TypeAttributes ComputeAttributeFlags();
        protected abstract override RoType ComputeBaseTypeWithoutDesktopQuirk();
        protected abstract override IEnumerable<RoType> ComputeDirectlyImplementedInterfaces();
        internal abstract override IEnumerable<ConstructorInfo> GetConstructorsCore(NameFilter filter);
        internal abstract override IEnumerable<MethodInfo> GetMethodsCore(NameFilter filter, Type reflectedType);
#endif //DEBUG
    }

    internal abstract partial class RoGenericParameterType
    {
#if DEBUG
        public abstract override bool IsGenericTypeParameter { get; }
        public abstract override bool IsGenericMethodParameter { get; }
        protected abstract override string ComputeName();
        public abstract override MethodBase DeclaringMethod { get; }
        protected abstract override RoType ComputeDeclaringType();
        internal abstract override RoModule GetRoModule();
        public abstract override IEnumerable<CustomAttributeData> CustomAttributes { get; }
        public abstract override int MetadataToken { get; }
        public abstract override GenericParameterAttributes GenericParameterAttributes { get; }
        internal abstract override bool IsCustomAttributeDefined(ReadOnlySpan<byte> ns, ReadOnlySpan<byte> name);
        internal abstract override CustomAttributeData TryFindCustomAttribute(ReadOnlySpan<byte> ns, ReadOnlySpan<byte> name);
#endif //DEBUG
    }

    internal abstract partial class RoEvent
    {
#if DEBUG
        public sealed override MemberTypes MemberType => base.MemberType;
        public sealed override MethodInfo AddMethod => base.AddMethod;
        public sealed override MethodInfo RemoveMethod => base.RemoveMethod;
        public sealed override MethodInfo RaiseMethod => base.RaiseMethod;
#endif //DEBUG
    }

    internal abstract partial class RoField
    {
#if DEBUG
        public sealed override MemberTypes MemberType => base.MemberType;
#endif //DEBUG
    }

    internal abstract partial class RoMethod
    {
#if DEBUG
        public sealed override MemberTypes MemberType => base.MemberType;
#endif //DEBUG
    }


    internal abstract partial class RoConstructor
    {
#if DEBUG
        public sealed override MemberTypes MemberType => base.MemberType;
        public sealed override Type[] GetGenericArguments() => base.GetGenericArguments();
#endif //DEBUG
    }

    internal abstract partial class RoProperty
    {
#if DEBUG
        public sealed override MemberTypes MemberType => base.MemberType;
        public sealed override MethodInfo GetMethod => base.GetMethod;
        public sealed override MethodInfo SetMethod => base.SetMethod;
        public sealed override object GetValue(object obj, object[] index) => base.GetValue(obj, index);
        public sealed override void SetValue(object obj, object value, object[] index) => base.SetValue(obj, value, index);
#endif //DEBUG
    }

    internal abstract partial class RoCustomAttributeData
    {
#if DEBUG
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();
#endif //DEBUG
    }

    internal abstract partial class RoMethodBody
    {
#if DEBUG
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();
        public sealed override string ToString() => base.ToString();
#endif //DEBUG
    }

    internal sealed partial class RoLocalVariableInfo
    {
#if DEBUG
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();
        public sealed override string ToString() => base.ToString();
#endif //DEBUG
    }

    internal sealed partial class RoExceptionHandlingClause
    {
#if DEBUG
        public sealed override bool Equals(object obj) => base.Equals(obj);
        public sealed override int GetHashCode() => base.GetHashCode();
        public sealed override string ToString() => base.ToString();
#endif //DEBUG
    }
}
