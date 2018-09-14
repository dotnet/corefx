// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection.TypeLoading;

namespace System.Reflection
{
    //
    // A TypeLoader represents a closed universe of Type objects loaded for inspection-only purposes.
    // Each TypeLoader defines its own binding rules and is isolated from all other TypeLoaders.
    //
    // Another way to look at TypeLoaders is as a dictionary binding assembly names to loaded Assembly instances.
    //
    // TypeLoaders treat assemblies strictly as metadata. There are no restrictions on loading assemblies based
    // on target platform, CPU architecture or pointer size. There are no restrictions on the assembly designated
    // as the core assembly ("mscorlib"). 
    //
    // Also, as long as the metadata is "syntactically correct", the TypeLoader strives to report it "as is" (as long it 
    // can do so in a way that can be distinguished from valid data) and refrains from judging whether it's "executable." 
    // This is both for performance reasons (checks cost time) and its intended role as metadata inspection tool.
    // Examples of things that TypeLoaders let go unchecked include creating generic instances that violate generic 
    // parameter constraints, and loading type hierachies that would be unloadable in an actual runtime (deriving from sealed classes,
    // overriding members that don't exist in the ancestor classes, failing to implement all abstract methods, etc.)
    //
    // You cannot invoke methods, set or get field or property values or instantiate objects using 
    // the Type objects from a TypeLoader. You can however, use FieldInfo.GetRawConstantValue(),
    // ParameterInfo.RawDefaultValue and PropertyInfo.GetRawConstantValue(). You can retrieve custom attributes
    // in CustomAttributeData format but not as instantiated custom attributes. The CustomAttributeExtensions
    // extension api will not work with these Types nor will the IsDefined() family of api.
    //
    // There is no binding policy baked into the TypeLoader. You must either manually load all dependencies
    // or subscribe to the Resolving event to load dependencies as needed. The TypeLoader strives to avoid 
    // loading dependencies unless needed. Therefore, it is possible to do useful analysis of an assembly even
    // in the absence of dependencies. For example, retrieving an assembly's name and the names of its (direct)
    // dependencies can be done without having any of those dependencies on hand.
    //
    // To bind assemblies, the TypeLoader raises the Resolving event. You implement the binding algorithm by
    // subscribing to that event. The event should load the requested assembly and return it. To do this,
    // it can use LoadFromAssemblyPath() or one of its variants (LoadFromStream(), LoadFromByteArray()).
    //
    // Once an assembly has been bound, no assembly with the same assembly name identity
    // can be bound again from a different location unless the MVID's are identical.
    //
    // Once loaded, the underlying file may be locked for the duration of the TypeLoader's lifetime. You can
    // release the locks by disposing the TypeLoader object. The behavior of any Type, Assembly or other reflection
    // objects handed out by the TypeLoader is undefined after disposal. Though objects provided by the TypeLoader
    // strive to throw an ObjectDisposedException, this is not guaranteed. Some apis may return fixed or previously
    // cached data. Accessing objects *during* a Dispose may result in a unmanaged access violation and failfast.
    //
    // Comparing Type, Member and Assembly objects:
    //
    //   The right way to compare two Reflection objects dispensed by the TypeLoader are:
    //
    //       m1 == m2
    //       m1.Equals(m2)
    //
    //   but not
    //
    //       object.ReferenceEquals(m1, m2)   // WRONG
    //       (object)m1 == (object)m2         // WRONG
    //
    //   Note that the following descriptions are not literal descriptions of how Equals() is implemented. The TypeLoader
    //   reserves the right to implement Equals() as "object.ReferenceEquals()" and intern the associated objects in such
    //   a way that Equals() works "as if" it were comparing those things.
    //
    // - Each TypeLoader permits only one Assembly instance per assembly identity so equality of assemblies is the same as the
    //   equality of their assembly identity.
    // 
    // - Modules are compared by comparing their containing assemblies and their row indices in the assembly's manifest file table.
    // 
    // - Defined types are compared by comparing their containing modules and their row indices in the module's TypeDefinition table.
    // 
    // - Constructed types (arrays, byrefs, pointers, generic instances) are compared by comparing all of their component types.
    // 
    // - Generic parameter types are compared by comparing their containing Modules and their row indices in the module's GenericParameter table.
    // 
    // - Constructors, methods, fields, events and properties are compared by comparing their declaring types, their row indices in their respective
    //   token tables and their ReflectedType property.
    // 
    // - Parameters are compared by comparing their declaring member and their position index.
    //
    // Multithreading:
    //    
    //   The TypeLoader and the reflection objects it hands out are all multithread-safe and logically immutable, 
    //   with the following provisos:
    //
    //   - Adding (or removing) handlers to the TypeLoader's events is not thread-safe. These should be done prior
    //     to any multithreaded access to the TypeLoader.
    //
    //   - No Loads or inspections of reflection objects can be done during or after disposing the owning TypeLoader.
    //
    // Support for NetCore Reflection apis:
    //
    //   .NETCore added a number of apis (IsSZArray, IsVariableBoundArray, IsTypeDefinition, IsGenericTypeParameter, IsGenericMethodParameter,
    //      HasSameMetadataDefinitionAs, to name a few.) to the Reflection surface area.
    //
    //   The Reflection objects dispensed by TypeLoaders support all the new apis *provided* that you are using the netcore build of System.Reflection.TypeLoader.dll.
    //
    //   If you are using the netstandard build of System.Reflection.TypeLoader.dll, the NetCore-specific apis are not supported. Attempting to invoke
    //   them will generate a NotImplementedException or NullReferenceException (unfortunately, we can't improve the exceptions thrown because
    //   they are being thrown by code this library doesn't control.) Because of this, it is recommended that apps built against NetCore use the NetCore build of
    //   TypeLoader.dll.
    //
    public sealed partial class TypeLoader : IDisposable
    {
        /// <summary>
        /// Creates a new TypeLoader object without setting a core assembly name. The core assembly name should be set by setting the
        /// CoreAssemblyName property.
        /// </summary>
        public TypeLoader() 
            : this(null)
        {
        }

        /// <summary>
        /// Create a new TypeLoader object.
        /// </summary>
        /// <param name="coreAssemblyName">
        /// The name of the assembly that contains the core types such as System.Object. Typically, this would be "mscorlib".
        /// </param>
        public TypeLoader(string coreAssemblyName)
        {
            CoreAssemblyName = coreAssemblyName;
        }

        /// <summary>
        /// Loads an assembly from a specific path on the disk and binds its assembly name to it in the TypeLoader. If a prior
        /// assembly with the same name was already loaded into the TypeLoader, the prior assembly will be returned. If the 
        /// two assemblies do not have the same MVID, this method throws a FileLoadException.
        /// </summary>
        public Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            if (assemblyPath == null)
                throw new ArgumentNullException(nameof(assemblyPath));

            return LoadFromStreamCore(File.OpenRead(assemblyPath));
        }

        /// <summary>
        /// Loads an assembly from a byte array and binds its assembly name to it in the TypeLoader. If a prior
        /// assembly with the same name was already loaded into the TypeLoader, the prior assembly will be returned. If the 
        /// two assemblies do not have the same MVID, this method throws a FileLoadException.
        /// </summary>
        public Assembly LoadFromByteArray(byte[] assembly)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return LoadFromStreamCore(new MemoryStream(assembly));
        }

        /// <summary>
        /// Loads an assembly from a stream and binds its assembly name to it in the TypeLoader. If a prior
        /// assembly with the same name was already loaded into the TypeLoader, the prior assembly will be returned. If the 
        /// two assemblies do not have the same MVID, this method throws a FileLoadException.
        /// 
        /// The TypeLoader takes ownership of the Stream passed into this method. The original owner must not mutate its position, dispose the Stream or 
        /// assume that its position will stay unchanged.
        /// </summary>
        public Assembly LoadFromStream(Stream assembly)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            assembly.Position = 0;
            return LoadFromStreamCore(assembly);
        }

        /// <summary>
        /// Resolves the supplied assembly name to an assembly. If an assembly was previously bound by to this name, that assembly is returned.
        /// Otherwise, the TypeLoader raises the Resolving event. If the event handler returns null, this method throws a FileNotFoundException.
        /// 
        /// Note that this behavior matches the behavior of AssemblyLoadContext.LoadFromAssemblyName() but does not match the behavior of 
        /// Assembly.ReflectionOnlyLoad() (the latter gives up without raising the resolve event.)
        /// </summary>
        public Assembly LoadFromAssemblyName(string assemblyName)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            AssemblyName assemblyNameObject = new AssemblyName(assemblyName);
            RoAssemblyName refName = assemblyNameObject.ToRoAssemblyName();
            return ResolveAssembly(refName);
        }

        /// <summary>
        /// Resolves the supplied assembly name to an assembly. If an assembly was previously bound by to this name, that assembly is returned.
        /// Otherwise, the TypeLoader raises the Resolving event. If the event handler returns null, this method throws a FileNotFoundException.
        /// 
        /// Note that this behavior matches the behavior of AssemblyLoadContext.LoadFromAssemblyName() but does not match the behavior of 
        /// Assembly.ReflectionOnlyLoad(). (the latter gives up without raising the resolve event.)
        /// </summary>
        public Assembly LoadFromAssemblyName(AssemblyName assemblyName)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));

            RoAssemblyName refName = assemblyName.ToRoAssemblyName();
            return ResolveAssembly(refName);
        }

        /// <summary>
        /// Subscribe to this event to define the binding algorithm. The event should use TypeLoader.LoadFromStream(), LoadFromAssemblyPath()
        /// or LoadFromByteArray() to load the requested assembly and return it. 
        /// 
        /// To indicate the failure to find an assembly, the handler should return null rather than throwing an exception. Returning null commits 
        /// the failure so that future attempts to load that name will fail without re-invoking the handler.
        /// 
        /// If the handler throws an exception, the exception will be passed through to the application that invoked the operation that triggered
        /// the binding. The TypeLoader will not catch it and no binding will occur.
        /// 
        /// The handler will generally not be called more than once for the same name, unless two threads race to load the same assembly.
        /// Even in that case, one result will win and be atomically bound to the name.
        /// 
        /// The TypeLoader intentionally performs no ref-def matching on the returned assembly as what constitutes a ref-def match is a policy. 
        /// It is also the kind of arbitrary restriction that TypeLoader strives to avoid.
        /// 
        /// TypeLoaders cannot consume assemblies from other TypeLoaders or other type providers (such as the underlying runtime's own Reflection system.)
        /// If a handler returns such an assembly, the TypeLoader throws a FileLoadException.
        /// </summary>
        public event Func<TypeLoader, AssemblyName, Assembly> Resolving;

        /// <summary>
        /// Returns the assembly name that denotes the "system assembly" that houses the well-known types such as System.Int32.
        /// Typically, this assembly is named "mscorlib", or "netstandard". If the system assembly cannot
        /// be found, many methods, including those that parse method signatures, will throw.
        /// 
        /// The designated core assembly does not need to contain the core types directly. It can type forward them to other assemblies.
        /// Thus, it is perfectly permissible to use the mscorlib facade as the designated core assembly.
        /// 
        /// Note that "System.Runtime" is not an ideal core assembly as it excludes some of the interop-related pseudo-custom attribute types
        /// such as DllImportAttribute. However, it can serve if you have no interest in those attributes. The CustomAttributes api
        /// will skip those attributes if the core assembly does not include the necessary types.
        /// 
        /// Setting the CoreAssemblyName can be deferred until after TypeLoader creation. You can safely use the following apis:
        /// 
        ///    TypeLoader.LoadFromStream(), LoadFromAssemblyPath(), LoadFromByteArray()
        ///    Assembly.GetName(), Assembly.FullName, Assembly.GetReferencedAssemblies()
        ///    Assembly.GetTypes(), Assembly.DefinedTypes, Assembly.GetExportedTypes(), Assembly.GetForwardedTypes()
        ///    Assembly.GetType(string, bool, bool)
        ///    Type.Name, Type.FullName, Type.AssemblyQualifiedName
        ///    
        /// as none of these will trigger an internal search of the core assembly. These may be useful in identifying the core assembly.
        /// 
        /// Once the TypeLoader has used the CoreAssemblyName, however, you can no longer change this property. Attempting to set the CoreAssemblyName
        /// after this point will trigger an InvalidOperationException.
        /// 
        /// If you do not specify a core assembly, or the core assembly cannot be bound or if the core assembly is missing types, this will 
        /// affect the behavior of the TypeLoader as follows:
        /// 
        /// - Apis that need to parse signatures or typespecs and return the results as Types will throw. For example, 
        ///   MethodBase.ReturnType, MethodBase.GetParameters(), Type.BaseType, Type.GetInterfaces().
        /// 
        /// - Apis that need to compare types to well known core types will not throw and the comparison will evaluate to "false."
        ///   For example, if you do not specify a core assembly, Type.IsPrimitive will return false for everything,
        ///   even types named "System.Int32". Similarly, Type.GetTypeCode() will return TypeCode.Object for everything.
        /// 
        /// - If a metadata entity sets flags that surface as a pseudo-custom attribute, and the core assembly does not contain the pseudo-custom attribute
        ///   type, the necessary constructor or any of the parameter types of the constructor, the TypeLoader will not throw. It will omit the pseudo-custom
        ///   attribute from the list of returned attributes.
        /// </summary>
        public string CoreAssemblyName
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(TypeLoader));

                return _userSuppliedCoreAssemblyName;
            }

            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(TypeLoader));

                if (_lazyCommitedCoreAssemblyName != s_committedCoreAssemblyNameSentinel)
                    throw new InvalidOperationException(SR.TooLateToSetCoreAssemblyName);

                if (value != null)
                {
                    // Validate now that the value is a parsable assembly name.
                    new AssemblyName(value);
                }

                _userSuppliedCoreAssemblyName = value;
            }
        }

        private volatile string _userSuppliedCoreAssemblyName;

        /// <summary>
        /// Return an atomic snapshot of the assemblies that have been loaded into the TypeLoader.
        /// </summary>
        public IEnumerable<Assembly> GetAssemblies()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(TypeLoader));

            return _loadedAssemblies.Values;
        }

        /// <summary>
        /// Releases any native resources (such as file locks on assembly files.) After disposal, it is not safe to use
        /// any Assembly objects dispensed by the TypeLoader, nor any Reflection objects dispensed by those Assembly objects.
        /// Though objects provided by the TypeLoader strive to throw an ObjectDisposedException, this is not guaranteed. 
        /// Some apis may return fixed or previously cached data. Accessing objects *during* a Dispose may result in an
        /// unmanaged access violation and failfast.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
