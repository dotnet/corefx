// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Collections.Generic;

//
// This file makes NetStandard Reflection's "subclassing" surface area look as much like NetCore as possible so the rest of the code can be written without #if's.
//

namespace System.Reflection.TypeLoading
{
    //
    // For code (usually shared with other repos) that have to interact with "Type" rather than "RoType", some handy extension methods that 
    // "add" the NetCore reflection apis back to NetStandard.
    //
    internal static class NetCoreApiEmulators
    {
#if netstandard
        // On NetStandard, have to do with slower emulations.

        public static bool IsSignatureType(this Type type) => false;
        public static bool IsSZArray(this Type type) => type.IsArray && type.GetArrayRank() == 1 && type.Name.EndsWith("[]");
        public static bool IsVariableBoundArray(this Type type) => type.IsArray && !type.IsSZArray();
        public static bool IsGenericMethodParameter(this Type type) => type.IsGenericParameter && type.DeclaringMethod != null;

        // Signature Types do not exist on NetStandard 2.0 but it's possible we could reach this if a NetCore app uses the NetStandard build
        // of this library.
        public static Type MakeSignatureGenericType(this Type genericTypeDefinition, Type[] typeArguments) => throw new NotSupportedException(SR.NotSupported_MakeGenericType_SignatureTypes);
#else
        // On NetCore, call the real thing.

        public static bool IsSignatureType(this Type type) => type.IsSignatureType;
        public static bool IsSZArray(this Type type) => type.IsSZArray;
        public static bool IsVariableBoundArray(this Type type) => type.IsVariableBoundArray;
        public static bool IsGenericMethodParameter(this Type type) => type.IsGenericMethodParameter;

        // @TODO - https://github.com/dotnet/corefxlab/issues/2443: This should be fixed assuming https://github.com/dotnet/corefx/issues/31798 gets approved.
        public static Type MakeSignatureGenericType(this Type genericTypeDefinition, Type[] typeArguments) => throw new NotSupportedException(SR.NotSupported_MakeGenericType_SignatureTypes);
#endif
    }

    //
    // Another layer of base types. For NetCore, these base types are all but empty. For NetStandard, these base types add the NetCore apis to NetStandard
    // so code interacting with "RoTypes" and friends can happily code to the full NetCore surface area.
    //
    // On NetStandard and pre-2.2 NetCore, the TypeInfo constructor is not exposed so we cannot derive directly from TypeInfo. 
    // But we *can* derive from TypeDelegator which derives from TypeInfo. Since we're overriding (almost) every method, 
    // none of TypeDelegator's own methods get called (and the instance field it has for holding the "underlying Type" goes
    // to waste.) 
    //
    // For future platforms, RoTypeBase's base type should be changed back to TypeInfo. Deriving from TypeDelegator is a hack and
    // causes us to waste an extra pointer-sized field per Type instance. It is also fragile as TypeDelegator could break us in the future
    // by overriding more methods.
    //
    internal abstract class LeveledTypeInfo :
#if netstandard
        TypeDelegator
#else
        TypeInfo
#endif
    {
        protected LeveledTypeInfo() : base() { }

#if netstandard
        // This is an api that TypeDelegator overrides that it needn't have. Since RoType expects to fall through to System.Type's method, we have to reimplement
        // System.Type's behavior here to avoid getting TypeDelegator's method.
        //
        // This is an annoying and fragile requirement as we have to do this for any api that (1) RoType declines to override and (2) TypeDelegator does override.
        // This could be policed by an analyzer that searches RoType's method bodies for non-virtual calls to apis declared on TypeDelegator.
        public override EventInfo[] GetEvents() => GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

        public abstract bool IsGenericTypeParameter { get; }
        public abstract bool IsGenericMethodParameter { get; }
        public abstract bool IsSZArray { get; }
        public abstract bool IsVariableBoundArray { get; }
        public abstract bool IsTypeDefinition { get; }
        public abstract bool IsByRefLike { get; }
        public virtual bool IsSignatureType => false;
        protected abstract MethodInfo GetMethodImpl(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledAssembly : Assembly
    {
#if netstandard
        public abstract Type[] GetForwardedTypes();
#endif // netstandard
    }

    internal abstract class LeveledConstructorInfo : ConstructorInfo
    {
#if netstandard
        public abstract bool IsConstructedGenericMethod { get; }
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledMethodInfo : MethodInfo
    {
#if netstandard
        public abstract bool IsConstructedGenericMethod { get; }
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledEventInfo : EventInfo
    {
#if netstandard
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledFieldInfo : FieldInfo
    {
#if netstandard
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledPropertyInfo : PropertyInfo
    {
#if netstandard
        public abstract bool HasSameMetadataDefinitionAs(MemberInfo other);
#endif // netstandard
    }

    internal abstract class LeveledCustomAttributeData : CustomAttributeData
    {
#if netstandard
        // On NetStandard, AttributeType is declared non-virtually so apps are stuck calling the slow version that builds a constructor.
        public new abstract Type AttributeType { get; }
#else
        // @todo: https://github.com/dotnet/corefxlab/issues/2460 Once the netcore build is building against a contract that declares AttributeType virtually,
        // delete this line. We want RoCustomAttributeData to override the real AttributeType property.
        public new abstract Type AttributeType { get; }
#endif // netstandard
    }
}
