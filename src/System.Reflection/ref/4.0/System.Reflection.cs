// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection
{
    public sealed partial class AmbiguousMatchException : System.Exception
    {
        public AmbiguousMatchException() { }
        public AmbiguousMatchException(string message) { }
        public AmbiguousMatchException(string message, System.Exception inner) { }
    }
    public abstract partial class Assembly
    {
        internal Assembly() { }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        public abstract System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DefinedTypes { get; }
        public virtual System.Collections.Generic.IEnumerable<System.Type> ExportedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
        public virtual string FullName { get { return default(string); } }
        public virtual bool IsDynamic { get { return default(bool); } }
        public virtual System.Reflection.Module ManifestModule { get { return default(System.Reflection.Module); } }
        public abstract System.Collections.Generic.IEnumerable<System.Reflection.Module> Modules { get; }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public virtual System.Reflection.ManifestResourceInfo GetManifestResourceInfo(string resourceName) { return default(System.Reflection.ManifestResourceInfo); }
        public virtual string[] GetManifestResourceNames() { return default(string[]); }
        public virtual System.IO.Stream GetManifestResourceStream(string name) { return default(System.IO.Stream); }
        public virtual System.Reflection.AssemblyName GetName() { return default(System.Reflection.AssemblyName); }
        public virtual System.Type GetType(string name) { return default(System.Type); }
        public virtual System.Type GetType(string name, bool throwOnError, bool ignoreCase) { return default(System.Type); }
        public static System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyRef) { return default(System.Reflection.Assembly); }
        public override string ToString() { return default(string); }
    }
    public enum AssemblyContentType
    {
        Default = 0,
        WindowsRuntime = 1,
    }
    public sealed partial class AssemblyName
    {
        public AssemblyName() { }
        public AssemblyName(string assemblyName) { }
        public System.Reflection.AssemblyContentType ContentType { get { return default(System.Reflection.AssemblyContentType); } set { } }
        public string CultureName { get { return default(string); } set { } }
        public System.Reflection.AssemblyNameFlags Flags { get { return default(System.Reflection.AssemblyNameFlags); } set { } }
        public string FullName { get { return default(string); } }
        public string Name { get { return default(string); } set { } }
        public System.Reflection.ProcessorArchitecture ProcessorArchitecture { get { return default(System.Reflection.ProcessorArchitecture); } set { } }
        public System.Version Version { get { return default(System.Version); } set { } }
        public byte[] GetPublicKey() { return default(byte[]); }
        public byte[] GetPublicKeyToken() { return default(byte[]); }
        public void SetPublicKey(byte[] publicKey) { }
        public void SetPublicKeyToken(byte[] publicKeyToken) { }
        public override string ToString() { return default(string); }
    }
    public abstract partial class ConstructorInfo : System.Reflection.MethodBase
    {
        public static readonly string ConstructorName;
        public static readonly string TypeConstructorName;
        internal ConstructorInfo() { }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public virtual object Invoke(object[] parameters) { return default(object); }
    }
    public partial class CustomAttributeData
    {
        internal CustomAttributeData() { }
        public virtual System.Type AttributeType { get { return default(System.Type); } }
        public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument> ConstructorArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument>); } }
        public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument> NamedArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument>); } }
    }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct CustomAttributeNamedArgument
    {
        public bool IsField { get { return default(bool); } }
        public string MemberName { get { return default(string); } }
        public System.Reflection.CustomAttributeTypedArgument TypedValue { get { return default(System.Reflection.CustomAttributeTypedArgument); } }
    }
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct CustomAttributeTypedArgument
    {
        public System.Type ArgumentType { get { return default(System.Type); } }
        public object Value { get { return default(object); } }
    }
    public abstract partial class EventInfo : System.Reflection.MemberInfo
    {
        internal EventInfo() { }
        public virtual System.Reflection.MethodInfo AddMethod { get { return default(System.Reflection.MethodInfo); } }
        public abstract System.Reflection.EventAttributes Attributes { get; }
        public virtual System.Type EventHandlerType { get { return default(System.Type); } }
        public bool IsSpecialName { get { return default(bool); } }
        public virtual System.Reflection.MethodInfo RaiseMethod { get { return default(System.Reflection.MethodInfo); } }
        public virtual System.Reflection.MethodInfo RemoveMethod { get { return default(System.Reflection.MethodInfo); } }
        public virtual void AddEventHandler(object target, System.Delegate handler) { }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public virtual void RemoveEventHandler(object target, System.Delegate handler) { }
    }
    public abstract partial class FieldInfo : System.Reflection.MemberInfo
    {
        internal FieldInfo() { }
        public abstract System.Reflection.FieldAttributes Attributes { get; }
        public abstract System.Type FieldType { get; }
        public bool IsAssembly { get { return default(bool); } }
        public bool IsFamily { get { return default(bool); } }
        public bool IsFamilyAndAssembly { get { return default(bool); } }
        public bool IsFamilyOrAssembly { get { return default(bool); } }
        public bool IsInitOnly { get { return default(bool); } }
        public bool IsLiteral { get { return default(bool); } }
        public bool IsPrivate { get { return default(bool); } }
        public bool IsPublic { get { return default(bool); } }
        public bool IsSpecialName { get { return default(bool); } }
        public bool IsStatic { get { return default(bool); } }
        public override bool Equals(object obj) { return default(bool); }
        public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle) { return default(System.Reflection.FieldInfo); }
        public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.FieldInfo); }
        public override int GetHashCode() { return default(int); }
        public abstract object GetValue(object obj);
        public virtual void SetValue(object obj, object value) { }
    }
    public static partial class IntrospectionExtensions
    {
        public static System.Reflection.TypeInfo GetTypeInfo(this System.Type type) { return default(System.Reflection.TypeInfo); }
    }
    public partial interface IReflectableType
    {
        System.Reflection.TypeInfo GetTypeInfo();
    }
    public partial class LocalVariableInfo
    {
        protected LocalVariableInfo() { }
        public virtual bool IsPinned { get { return default(bool); } }
        public virtual int LocalIndex { get { return default(int); } }
        public virtual System.Type LocalType { get { return default(System.Type); } }
        public override string ToString() { return default(string); }
    }
    public partial class ManifestResourceInfo
    {
        public ManifestResourceInfo(System.Reflection.Assembly containingAssembly, string containingFileName, System.Reflection.ResourceLocation resourceLocation) { }
        public virtual string FileName { get { return default(string); } }
        public virtual System.Reflection.Assembly ReferencedAssembly { get { return default(System.Reflection.Assembly); } }
        public virtual System.Reflection.ResourceLocation ResourceLocation { get { return default(System.Reflection.ResourceLocation); } }
    }
    public abstract partial class MemberInfo
    {
        internal MemberInfo() { }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        public abstract System.Type DeclaringType { get; }
        public virtual System.Reflection.Module Module { get { return default(System.Reflection.Module); } }
        public abstract string Name { get; }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
    }
    public abstract partial class MethodBase : System.Reflection.MemberInfo
    {
        internal MethodBase() { }
        public abstract System.Reflection.MethodAttributes Attributes { get; }
        public virtual System.Reflection.CallingConventions CallingConvention { get { return default(System.Reflection.CallingConventions); } }
        public virtual bool ContainsGenericParameters { get { return default(bool); } }
        public bool IsAbstract { get { return default(bool); } }
        public bool IsAssembly { get { return default(bool); } }
        public bool IsConstructor { get { return default(bool); } }
        public bool IsFamily { get { return default(bool); } }
        public bool IsFamilyAndAssembly { get { return default(bool); } }
        public bool IsFamilyOrAssembly { get { return default(bool); } }
        public bool IsFinal { get { return default(bool); } }
        public virtual bool IsGenericMethod { get { return default(bool); } }
        public virtual bool IsGenericMethodDefinition { get { return default(bool); } }
        public bool IsHideBySig { get { return default(bool); } }
        public bool IsPrivate { get { return default(bool); } }
        public bool IsPublic { get { return default(bool); } }
        public bool IsSpecialName { get { return default(bool); } }
        public bool IsStatic { get { return default(bool); } }
        public bool IsVirtual { get { return default(bool); } }
        public abstract System.Reflection.MethodImplAttributes MethodImplementationFlags { get; }
        public override bool Equals(object obj) { return default(bool); }
        public virtual System.Type[] GetGenericArguments() { return default(System.Type[]); }
        public override int GetHashCode() { return default(int); }
        public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle) { return default(System.Reflection.MethodBase); }
        public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.MethodBase); }
        public abstract System.Reflection.ParameterInfo[] GetParameters();
        public virtual object Invoke(object obj, object[] parameters) { return default(object); }
    }
    public abstract partial class MethodInfo : System.Reflection.MethodBase
    {
        internal MethodInfo() { }
        public virtual System.Reflection.ParameterInfo ReturnParameter { get { return default(System.Reflection.ParameterInfo); } }
        public virtual System.Type ReturnType { get { return default(System.Type); } }
        public virtual System.Delegate CreateDelegate(System.Type delegateType) { return default(System.Delegate); }
        public virtual System.Delegate CreateDelegate(System.Type delegateType, object target) { return default(System.Delegate); }
        public override bool Equals(object obj) { return default(bool); }
        public override System.Type[] GetGenericArguments() { return default(System.Type[]); }
        public virtual System.Reflection.MethodInfo GetGenericMethodDefinition() { return default(System.Reflection.MethodInfo); }
        public override int GetHashCode() { return default(int); }
        public virtual System.Reflection.MethodInfo MakeGenericMethod(params System.Type[] typeArguments) { return default(System.Reflection.MethodInfo); }
    }
    public abstract partial class Module
    {
        internal Module() { }
        public virtual System.Reflection.Assembly Assembly { get { return default(System.Reflection.Assembly); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        public virtual string FullyQualifiedName { get { return default(string); } }
        public virtual string Name { get { return default(string); } }
        public override bool Equals(object o) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public virtual System.Type GetType(string className, bool throwOnError, bool ignoreCase) { return default(System.Type); }
        public override string ToString() { return default(string); }
    }
    public partial class ParameterInfo
    {
        internal ParameterInfo() { }
        public virtual System.Reflection.ParameterAttributes Attributes { get { return default(System.Reflection.ParameterAttributes); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        public virtual object DefaultValue { get { return default(object); } }
        public virtual bool HasDefaultValue { get { return default(bool); } }
        public bool IsIn { get { return default(bool); } }
        public bool IsOptional { get { return default(bool); } }
        public bool IsOut { get { return default(bool); } }
        public bool IsRetval { get { return default(bool); } }
        public virtual System.Reflection.MemberInfo Member { get { return default(System.Reflection.MemberInfo); } }
        public virtual string Name { get { return default(string); } }
        public virtual System.Type ParameterType { get { return default(System.Type); } }
        public virtual int Position { get { return default(int); } }
    }
    public abstract partial class PropertyInfo : System.Reflection.MemberInfo
    {
        internal PropertyInfo() { }
        public abstract System.Reflection.PropertyAttributes Attributes { get; }
        public abstract bool CanRead { get; }
        public abstract bool CanWrite { get; }
        public virtual System.Reflection.MethodInfo GetMethod { get { return default(System.Reflection.MethodInfo); } }
        public bool IsSpecialName { get { return default(bool); } }
        public abstract System.Type PropertyType { get; }
        public virtual System.Reflection.MethodInfo SetMethod { get { return default(System.Reflection.MethodInfo); } }
        public override bool Equals(object obj) { return default(bool); }
        public virtual object GetConstantValue() { return default(object); }
        public override int GetHashCode() { return default(int); }
        public abstract System.Reflection.ParameterInfo[] GetIndexParameters();
        public object GetValue(object obj) { return default(object); }
        public virtual object GetValue(object obj, object[] index) { return default(object); }
        public void SetValue(object obj, object value) { }
        public virtual void SetValue(object obj, object value, object[] index) { }
    }
    public abstract partial class ReflectionContext
    {
        protected ReflectionContext() { }
        public virtual System.Reflection.TypeInfo GetTypeForObject(object value) { return default(System.Reflection.TypeInfo); }
        public abstract System.Reflection.Assembly MapAssembly(System.Reflection.Assembly assembly);
        public abstract System.Reflection.TypeInfo MapType(System.Reflection.TypeInfo type);
    }
    public sealed partial class ReflectionTypeLoadException : System.Exception
    {
        public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions) { }
        public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions, string message) { }
        public System.Exception[] LoaderExceptions { get { return default(System.Exception[]); } }
        public System.Type[] Types { get { return default(System.Type[]); } }
    }
    [System.FlagsAttribute]
    public enum ResourceLocation
    {
        ContainedInAnotherAssembly = 2,
        ContainedInManifestFile = 4,
        Embedded = 1,
    }
    public sealed partial class TargetInvocationException : System.Exception
    {
        public TargetInvocationException(System.Exception inner) { }
        public TargetInvocationException(string message, System.Exception inner) { }
    }
    public sealed partial class TargetParameterCountException : System.Exception
    {
        public TargetParameterCountException() { }
        public TargetParameterCountException(string message) { }
        public TargetParameterCountException(string message, System.Exception inner) { }
    }
    public abstract partial class TypeInfo : System.Reflection.MemberInfo, System.Reflection.IReflectableType
    {
        internal TypeInfo() { }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo> DeclaredConstructors { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.EventInfo> DeclaredEvents { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.EventInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo> DeclaredFields { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo> DeclaredMembers { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> DeclaredMethods { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo>); } }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> DeclaredProperties { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo>); } }
        public virtual System.Type[] GenericTypeParameters { get { return default(System.Type[]); } }
        public virtual System.Collections.Generic.IEnumerable<System.Type> ImplementedInterfaces { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
        public virtual System.Type AsType() { return default(System.Type); }
        public virtual System.Reflection.EventInfo GetDeclaredEvent(string name) { return default(System.Reflection.EventInfo); }
        public virtual System.Reflection.FieldInfo GetDeclaredField(string name) { return default(System.Reflection.FieldInfo); }
        public virtual System.Reflection.MethodInfo GetDeclaredMethod(string name) { return default(System.Reflection.MethodInfo); }
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> GetDeclaredMethods(string name) { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); }
        public virtual System.Reflection.TypeInfo GetDeclaredNestedType(string name) { return default(System.Reflection.TypeInfo); }
        public virtual System.Reflection.PropertyInfo GetDeclaredProperty(string name) { return default(System.Reflection.PropertyInfo); }
        public virtual bool IsAssignableFrom(System.Reflection.TypeInfo typeInfo) { return default(bool); }
        System.Reflection.TypeInfo System.Reflection.IReflectableType.GetTypeInfo() { return default(System.Reflection.TypeInfo); }
    }
}
