// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    internal static class SR
    {
        internal const string NotSupported_AssemblyCodeBase = "The CodeBase property is not supported on assemblies loaded by a TypeLoader. Use Assembly.Location to find the origin of an Assembly.";
        internal const string NotSupported_SatelliteAssembly = "Satellite assemblies are not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_CaseInsensitive = "Passing true for ignoreCase is not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_InterfaceMapping = "InterfaceMapping is not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_GetBaseDefinition = "GetBaseDefinition() is not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_MDStreamVersion = "MDStreamVersion is not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_ResolvingTokens = "Resolving tokens is not supported on assemblies loaded by a TypeLoader.";
        internal const string NotSupported_MakeGenericType_SignatureTypes = "Passing signature types to this implementation of MakeGenericType() is not supported.";
        internal const string NotSupported_FunctionPointers = "Parsing function pointer types in signatures is not supported.";
        internal const string InvalidOperation_IsSecurity = "This property is not supported on assemblies loaded by a TypeLoader as there is no trust level to evaluate these against.";

        internal const string FileNotFoundAssembly = "Could not find assembly '{0}'. Either explicitly load this assembly using a method such as LoadFromAssemblyPath() or subscribe to the Resolving event with a handler that returns a valid assembly.";

        internal const string NoMetadataInPeImage = "This PE image is not a managed executable.";
        internal const string SpecifiedFileNameInvalid = "The module or file name '{0}' is not specified in the assembly's manifest.";

        internal const string Arg_ReflectionOnlyCA = "The requested operation is invalid on objects loaded by a TypeLoader. Use CustomAttributeData instead.";
        internal const string Arg_ReflectionOnlyInvoke = "It is illegal to invoke a method on a Type loaded by a TypeLoader.";
        internal const string Arg_ReflectionOnlyIllegal = "The requested operation is invalid on objects loaded by a TypeLoader.";
        internal const string Arg_ReflectionOnlyParameterDefaultValue = "It is illegal to request the default value on a ParameterInfo loaded by a TypeLoader. Use RawDefaultValue instead.";

        internal const string ResourceOnlyModule = "It is illegal to perform this operation on a Module that returns true for IsResource()";

        internal const string FileNotFoundModule = "Could not find the module file for '{0}'";
        internal const string FileLoadDuplicateAssemblies = "The assembly '{0}' has already loaded been loaded into this TypeLoader.";

        internal const string Argument_HasToBeArrayClass = "Must be an array type.";
        internal const string InvalidOperation_NotGenericType = "This operation is only valid on generic types.";
        internal const string Arg_NotGenericParameter = "Method may only be called on a Type for which Type.IsGenericParameter is true.";
        internal const string Arg_NotGenericTypeDefinition = "{0} is not a GenericTypeDefinition. MakeGenericType may only be called on a type for which Type.IsGenericTypeDefinition is true.";
        internal const string Argument_GenericArgsCount = "The number of generic arguments provided does not match the number of generic parameters.";

        internal const string TypeNotFound = "Could not find type '{0}' in assembly '{1}'";
        internal const string CoreTypeNotFound = "Could not find core type '{0}'";

        internal const string NoCoreAssemblyDefined = "This operation could not be completed because this TypeLoader's CoreAssemblyName property was not set to a valid assembly name.";

        internal const string BadImageFormat_TypeRefModuleNotInManifest = "Assembly '{0}' contains a type reference 0x{1} that references a module not in the manifest.";
        internal const string BadImageFormat_TypeRefBadScopeType = "Assembly '{0}' contains a type reference 0x{1} that contains an invalid scope token.";

        internal const string Arg_AmbiguousMatchException = "Ambiguous match found.";

        internal const string ExternalAssemblyReturnedByResolveHandler = "An assembly that was not created by this TypeLoader object was returned from its Resolving event handler.";

        internal const string NoInvokeMember = "Types loaded by a TypeLoader cannot pass any of these BindingFlags: InvokeMethod, CreateInstance, GetProperty, SetProperty";

        internal const string MissingCustomAttributeConstructor = "The constructor invoked by a custom attribute cannot be found on type {0}";
        internal const string UnexpectedUnderlyingEnumType = "Enum {0} has invalid underlying type: {1}";

        internal const string ModuleResolveEventReturnedExternalModule = "ModuleResolve handlers may only return Modules loaded by the TypeLoader that loaded the parent assembly.";
        internal const string MakeGenericType_NotLoadedByTypeLoader = "This type {0} was not loaded by the TypeLoader that loaded the generic type or method.";

        internal const string ManifestResourceInfoReferencedBadModule = "A manifest resource entry specified a filename that does not appear in the assembly manifest: '{0}'";

        internal const string Arg_MustBeEnum = "Type provided must be an Enum.";
        internal const string Argument_InvalidEnum = "The Enum type should contain one and only one instance field.";

        internal const string Arg_EnumLitValueNotFound = "Literal value was not found.";
        internal const string Arg_NotGenericMethodDefinition = "{0} is not a GenericMethodDefinition. MakeGenericMethod may only be called on a method for which MethodBase.IsGenericMethodDefinition is true.";

        internal const string TooLateToSetCoreAssemblyName = "This TypeLoader has already loaded the core Assembly using a previously supplied name.";

        internal const string NotAClause = "This ExceptionHandlingClause is not a clause.";
        internal const string NotAFilter = "This ExceptionHandlingClause is not a filter.";

        internal const string Arg_MustBeType = "Type must be a type provided by the TypeLoader.";
        internal const string Arg_EmptyArray = "Array may not be empty.";

        internal const string TypeLoaderDisposed = "This object is no longer valid because the TypeLoader that created it has been disposed.";

        internal const string GenericTypeParamIndexOutOfRange = "A type specification contained an out of range index for a generic type parameter: {0}";
        internal const string GenericMethodParamIndexOutOfRange = "A type specification contained an out of range index for a generic method parameter: {0}";

        internal const string Arg_HTCapacityOverflow = "Hashtable's capacity overflowed and went negative. Check load factor, capacity and the current size of the table.";

        internal static string Format(string resourceFormat, params object[] args) => string.Format(resourceFormat, args);
    }
}
