// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection
{
    /// <summary>
    /// The exception that is thrown when binding to a member results in more than one member matching
    /// the binding criteria. This class cannot be inherited.
    /// </summary>
    public sealed partial class AmbiguousMatchException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousMatchException" />
        /// class with an empty message string and the root cause exception set to null.
        /// </summary>
        public AmbiguousMatchException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousMatchException" />
        /// class with its message string set to the given message and the root cause exception set
        /// to null.
        /// </summary>
        /// <param name="message">A string indicating the reason this exception was thrown.</param>
        public AmbiguousMatchException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbiguousMatchException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public AmbiguousMatchException(string message, System.Exception inner) { }
    }
    /// <summary>
    /// Represents an assembly, which is a reusable, versionable, and self-describing building block
    /// of a common language runtime application.
    /// </summary>
    public abstract partial class Assembly : System.Reflection.ICustomAttributeProvider
    {
        internal Assembly() { }
        /// <summary>
        /// Gets a collection that contains this assembly's custom attributes.
        /// </summary>
        /// <returns>
        /// A collection that contains this assembly's custom attributes.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        /// <summary>
        /// Gets a collection of the types defined in this assembly.
        /// </summary>
        /// <returns>
        /// A collection of the types defined in this assembly.
        /// </returns>
        public abstract System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DefinedTypes { get; }
        /// <summary>
        /// Gets a collection of the public types defined in this assembly that are visible outside the
        /// assembly.
        /// </summary>
        /// <returns>
        /// A collection of the public types defined in this assembly that are visible outside the assembly.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Type> ExportedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
        /// <summary>
        /// Gets the entry point of this assembly.
        /// </summary>
        /// <returns>
        /// An object that represents the entry point of this assembly. If no entry point is found (for
        /// example, the assembly is a DLL), null is returned.
        /// </returns>
        public virtual MethodInfo EntryPoint { get { return default(MethodInfo); } }
        /// <summary>
        /// Gets the display name of the assembly.
        /// </summary>
        /// <returns>
        /// The display name of the assembly.
        /// </returns>
        public virtual string FullName { get { return default(string); } }
        /// <summary>
        /// Gets a value that indicates whether the current assembly was generated dynamically in the current
        /// process by using reflection emit.
        /// </summary>
        /// <returns>
        /// true if the current assembly was generated dynamically in the current process; otherwise, false.
        /// </returns>
        public virtual bool IsDynamic { get { return default(bool); } }
        /// <summary>
        /// Gets the module that contains the manifest for the current assembly.
        /// </summary>
        /// <returns>
        /// The module that contains the manifest for the assembly.
        /// </returns>
        public virtual System.Reflection.Module ManifestModule { get { return default(System.Reflection.Module); } }
        /// <summary>
        /// Gets a collection that contains the modules in this assembly.
        /// </summary>
        /// <returns>
        /// A collection that contains the modules in this assembly.
        /// </returns>
        public abstract System.Collections.Generic.IEnumerable<System.Reflection.Module> Modules { get; }
        /// <summary>
        /// Determines whether this assembly and the specified object are equal.
        /// </summary>
        /// <param name="o">The object to compare with this instance.</param>
        /// <returns>
        /// true if <paramref name="o" /> is equal to this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object o) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns information about how the given resource has been persisted.
        /// </summary>
        /// <param name="resourceName">The case-sensitive name of the resource.</param>
        /// <returns>
        /// An object that is populated with information about the resource's topology, or null if the
        /// resource is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="resourceName" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="resourceName" /> parameter is an empty string ("").
        /// </exception>
        public virtual System.Reflection.ManifestResourceInfo GetManifestResourceInfo(string resourceName) { return default(System.Reflection.ManifestResourceInfo); }
        /// <summary>
        /// Returns the names of all the resources in this assembly.
        /// </summary>
        /// <returns>
        /// An array that contains the names of all the resources.
        /// </returns>
        public virtual string[] GetManifestResourceNames() { return default(string[]); }
        /// <summary>
        /// Loads the specified manifest resource from this assembly.
        /// </summary>
        /// <param name="name">The case-sensitive name of the manifest resource being requested.</param>
        /// <returns>
        /// The manifest resource; or null if no resources were specified during compilation or if the
        /// resource is not visible to the caller.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> parameter is null.</exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="name" /> parameter is an empty string ("").
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="IO.IOException" />, instead.A file that was found could not be loaded.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException"><paramref name="name" /> was not found.</exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="name" /> is not a valid assembly.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// Resource length is greater than <see cref="Int64.MaxValue" />.
        /// </exception>
        public virtual System.IO.Stream GetManifestResourceStream(string name) { return default(System.IO.Stream); }
        /// <summary>
        /// Gets an <see cref="AssemblyName" /> for this assembly.
        /// </summary>
        /// <returns>
        /// An object that contains the fully parsed display name for this assembly.
        /// </returns>
        public virtual System.Reflection.AssemblyName GetName() { return default(System.Reflection.AssemblyName); }
        /// <summary>
        /// Gets the <see cref="Type" /> object with the specified name in the assembly instance.
        /// </summary>
        /// <param name="name">The full name of the type.</param>
        /// <returns>
        /// An object that represents the specified class, or null if the class is not found.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="name" /> is invalid.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="name" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="IO.IOException" />, instead.<paramref name="name" /> requires a dependent
        /// assembly that was found but could not be loaded.-or-The current assembly was loaded into the
        /// reflection-only context, and <paramref name="name" /> requires a dependent assembly that was
        /// not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual System.Type GetType(string name) { return default(System.Type); }
        /// <summary>
        /// Gets the <see cref="Type" /> object with the specified name in the assembly instance,
        /// with the options of ignoring the case, and of throwing an exception if the type is not found.
        /// </summary>
        /// <param name="name">The full name of the type.</param>
        /// <param name="throwOnError">true to throw an exception if the type is not found; false to return null.</param>
        /// <param name="ignoreCase">true to ignore the case of the type name; otherwise, false.</param>
        /// <returns>
        /// An object that represents the specified class.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name" /> is invalid.-or- The length of <paramref name="name" /> exceeds 1024
        /// characters.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="TypeLoadException">
        /// <paramref name="throwOnError" /> is true, and the type cannot be found.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="name" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="name" /> requires a dependent assembly that was found but could not be loaded.-or-The
        /// current assembly was loaded into the reflection-only context, and <paramref name="name" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual System.Type GetType(string name, bool throwOnError, bool ignoreCase) { return default(System.Type); }
        /// <summary>
        /// Loads an assembly given its <see cref="AssemblyName" />.
        /// </summary>
        /// <param name="assemblyRef">The object that describes the assembly to be loaded.</param>
        /// <returns>
        /// The loaded assembly.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyRef" /> is null.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="assemblyRef" /> is not found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="IO.IOException" />, instead.A file that was found could not be loaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="assemblyRef" /> is not a valid assembly. -or-Version 2.0 or later of the common
        /// language runtime is currently loaded and <paramref name="assemblyRef" /> was compiled with
        /// a later version.
        /// </exception>
        public static System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyRef) { return default(System.Reflection.Assembly); }
        /// <summary>
        /// Gets the process executable in the default application domain. In other application domains,
        /// this is the first executable that was executed by <see cref="AppDomain.ExecuteAssembly(String)" />.
        /// </summary>
        /// <returns>
        /// The assembly that is the process executable in the default application domain, or the first
        /// executable that was executed by <see cref="AppDomain.ExecuteAssembly(String)" />.
        /// Can return null when called from unmanaged code.
        /// </returns>
        public static System.Reflection.Assembly GetEntryAssembly() { return default(System.Reflection.Assembly); }
        /// <summary>
        /// Gets the full path or UNC location of the loaded file that contains the manifest.
        /// </summary>
        /// <returns>
        /// The location of the loaded file that contains the manifest. If the loaded file was shadow-copied,
        /// the location is that of the file after being shadow-copied. If the assembly is loaded from
        /// a byte array, such as when using the <see cref="Assembly.Load(Byte[])" />
        /// method overload, the value returned is an empty string ("").
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The current assembly is a dynamic assembly, represented by an
        /// <see cref="Emit.AssemblyBuilder" /> object.
        /// </exception>
        public virtual string Location { get { return default(string); } }
        /// <summary>
        /// Returns the full name of the assembly, also known as the display name.
        /// </summary>
        /// <returns>
        /// The full name of the assembly, or the class name if the full name of the assembly cannot be
        /// determined.
        /// </returns>
        public override string ToString() { return default(string); }
        /// <summary>
        /// Gets the location of the assembly as specified originally, for example, in an
        /// <see cref="AssemblyName" /> object.
        /// </summary>
        /// <returns>
        /// The location of the assembly as specified originally.
        /// </returns>
        public virtual string CodeBase { get { return default(string); } }
        /// <summary>
        /// Gets a string representing the version of the common language runtime (CLR) saved in the file
        /// containing the manifest.
        /// </summary>
        /// <returns>
        /// The CLR version folder name. This is not a full path.
        /// </returns>
        public virtual string ImageRuntimeVersion { get { return default(string); } }
        /// <summary>
        /// Locates the specified type from this assembly and creates an instance of it using the system
        /// activator, using case-sensitive search.
        /// </summary>
        /// <param name="typeName">The <see cref="Type.FullName" /> of the type to locate.</param>
        /// <returns>
        /// An instance of the specified type created with the default constructor; or null if <paramref name="typeName" />
        /// is not found. The type is resolved using the default binder, without specifying
        /// culture or activation attributes, and with <see cref="BindingFlags" />
        /// set to Public or Instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="typeName" /> is an empty string ("") or a string beginning with a null character.-or-The
        /// current assembly was loaded into the reflection-only context.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="typeName" /> is null.</exception>
        /// <exception cref="MissingMethodException">No matching constructor was found.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="typeName" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="typeName" /> requires a dependent assembly that was found but could not be
        /// loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="typeName" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="typeName" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="typeName" /> requires a dependent assembly that was compiled for a version
        /// of the runtime that is later than the currently loaded version.
        /// </exception>
        public object CreateInstance(string typeName) { return default(object); }
        /// <summary>
        /// Locates the specified type from this assembly and creates an instance of it using the system
        /// activator, with optional case-sensitive search.
        /// </summary>
        /// <param name="typeName">The <see cref="Type.FullName" /> of the type to locate.</param>
        /// <param name="ignoreCase">true to ignore the case of the type name; otherwise, false.</param>
        /// <returns>
        /// An instance of the specified type created with the default constructor; or null if <paramref name="typeName" />
        /// is not found. The type is resolved using the default binder, without specifying
        /// culture or activation attributes, and with <see cref="BindingFlags" />
        /// set to Public or Instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="typeName" /> is an empty string ("") or a string beginning with a null character.
        /// -or-The current assembly was loaded into the reflection-only context.
        /// </exception>
        /// <exception cref="MissingMethodException">No matching constructor was found.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="typeName" /> is null.</exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="typeName" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="typeName" /> requires a dependent assembly that was found but could not be
        /// loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="typeName" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="typeName" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="typeName" /> requires a dependent assembly that was compiled for a version
        /// of the runtime that is later than the currently loaded version.
        /// </exception>
        public object CreateInstance(string typeName, bool ignoreCase) { return default(object); }
        /// <summary>
        /// Creates the name of a type qualified by the display name of its assembly.
        /// </summary>
        /// <param name="assemblyName">The display name of an assembly.</param>
        /// <param name="typeName">The full name of a type.</param>
        /// <returns>
        /// The full name of the type qualified by the display name of the assembly.
        /// </returns>
        public static string CreateQualifiedName(string assemblyName, string typeName) { return default(string); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit) { return default(object[]); }
        /// <summary>
        /// Gets the public types defined in this assembly that are visible outside the assembly.
        /// </summary>
        /// <returns>
        /// An array that represents the types defined in this assembly that are visible outside the assembly.
        /// </returns>
        /// <exception cref="NotSupportedException">The assembly is a dynamic assembly.</exception>
        public virtual Type[] GetExportedTypes() { return default(Type[]); }
        /// <summary>
        /// Gets the <see cref="AssemblyName" /> objects for all the assemblies referenced
        /// by this assembly.
        /// </summary>
        /// <returns>
        /// An array that contains the fully parsed display names of all the assemblies referenced by this
        /// assembly.
        /// </returns>
        public virtual AssemblyName[] GetReferencedAssemblies() { return default(AssemblyName[]); }
        /// <summary>
        /// Gets the <see cref="Type" /> object with the specified name in the assembly instance
        /// and optionally throws an exception if the type is not found.
        /// </summary>
        /// <param name="name">The full name of the type.</param>
        /// <param name="throwOnError">true to throw an exception if the type is not found; false to return null.</param>
        /// <returns>
        /// An object that represents the specified class.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name" /> is invalid.-or- The length of <paramref name="name" /> exceeds 1024
        /// characters.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="TypeLoadException">
        /// <paramref name="throwOnError" /> is true, and the type cannot be found.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="name" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="name" /> requires a dependent assembly that was found but could not be loaded.-or-The
        /// current assembly was loaded into the reflection-only context, and <paramref name="name" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="name" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="name" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual Type GetType(string name, bool throwOnError) { return default(Type); }
        /// <summary>
        /// Gets the types defined in this assembly.
        /// </summary>
        /// <returns>
        /// An array that contains all the types that are defined in this assembly.
        /// </returns>
        /// <exception cref="ReflectionTypeLoadException">
        /// The assembly contains one or more types that cannot be loaded. The array returned by the
        /// <see cref="ReflectionTypeLoadException.Types" /> property of this exception
        /// contains a <see cref="Type" /> object for each type that was loaded and null for
        /// each type that could not be loaded, while the <see cref="ReflectionTypeLoadException.LoaderExceptions" />
        /// property contains an exception for each type that could not be loaded.
        /// </exception>
        public virtual Type[] GetTypes() { return default(Type[]); }
        bool System.Reflection.ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit) { return default(bool); }

    }
    /// <summary>
    /// Provides information about the type of code contained in an assembly.
    /// </summary>
    public enum AssemblyContentType
    {
        /// <summary>
        /// The assembly contains .NET Framework code.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The assembly contains Windows Runtime code.
        /// </summary>
        WindowsRuntime = 1,
    }
    /// <summary>
    /// Describes an assembly's unique identity in full.
    /// </summary>
    public sealed partial class AssemblyName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyName" /> class.
        /// </summary>
        public AssemblyName() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyName" /> class with
        /// the specified display name.
        /// </summary>
        /// <param name="assemblyName">
        /// The display name of the assembly, as returned by the <see cref="FullName" />
        /// property.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyName" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyName" /> is a zero length string.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="IO.IOException" />, instead.The referenced assembly could not be found,
        /// or could not be loaded.
        /// </exception>
        public AssemblyName(string assemblyName) { }
        /// <summary>
        /// Gets or sets a value that indicates what type of content the assembly contains.
        /// </summary>
        /// <returns>
        /// A value that indicates what type of content the assembly contains.
        /// </returns>
        public System.Reflection.AssemblyContentType ContentType { get { return default(System.Reflection.AssemblyContentType); } set { } }
        /// <summary>
        /// Gets or sets the name of the culture associated with the assembly.
        /// </summary>
        /// <returns>
        /// The culture name.
        /// </returns>
        public string CultureName { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets the attributes of the assembly.
        /// </summary>
        /// <returns>
        /// A value that represents the attributes of the assembly.
        /// </returns>
        public System.Reflection.AssemblyNameFlags Flags { get { return default(System.Reflection.AssemblyNameFlags); } set { } }
        /// <summary>
        /// Gets the full name of the assembly, also known as the display name.
        /// </summary>
        /// <returns>
        /// A string that is the full name of the assembly, also known as the display name.
        /// </returns>
        public string FullName { get { return default(string); } }
        /// <summary>
        /// Gets or sets the simple name of the assembly. This is usually, but not necessarily, the file
        /// name of the manifest file of the assembly, minus its extension.
        /// </summary>
        /// <returns>
        /// The simple name of the assembly.
        /// </returns>
        public string Name { get { return default(string); } set { } }
        /// <summary>
        /// Gets or sets a value that identifies the processor and bits-per-word of the platform targeted
        /// by an executable.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that identifies the processor and bits-per-word of the platform
        /// targeted by an executable.
        /// </returns>
        public System.Reflection.ProcessorArchitecture ProcessorArchitecture { get { return default(System.Reflection.ProcessorArchitecture); } set { } }
        /// <summary>
        /// Gets or sets the major, minor, build, and revision numbers of the assembly.
        /// </summary>
        /// <returns>
        /// An object that represents the major, minor, build, and revision numbers of the assembly.
        /// </returns>
        public System.Version Version { get { return default(System.Version); } set { } }
        /// <summary>
        /// Gets the public key of the assembly.
        /// </summary>
        /// <returns>
        /// A byte array that contains the public key of the assembly.
        /// </returns>
        /// <exception cref="Security.SecurityException">
        /// A public key was provided (for example, by using the
        /// <see cref="SetPublicKey(Byte[])" /> method), but no public key token was provided.
        /// </exception>
        public byte[] GetPublicKey() { return default(byte[]); }
        /// <summary>
        /// Gets the public key token, which is the last 8 bytes of the SHA-1 hash of the public key under
        /// which the application or assembly is signed.
        /// </summary>
        /// <returns>
        /// A byte array that contains the public key token.
        /// </returns>
        public byte[] GetPublicKeyToken() { return default(byte[]); }
        /// <summary>
        /// Sets the public key identifying the assembly.
        /// </summary>
        /// <param name="publicKey">A byte array containing the public key of the assembly.</param>
        public void SetPublicKey(byte[] publicKey) { }
        /// <summary>
        /// Sets the public key token, which is the last 8 bytes of the SHA-1 hash of the public key under
        /// which the application or assembly is signed.
        /// </summary>
        /// <param name="publicKeyToken">A byte array containing the public key token of the assembly.</param>
        public void SetPublicKeyToken(byte[] publicKeyToken) { }
        /// <summary>
        /// Returns the full name of the assembly, also known as the display name.
        /// </summary>
        /// <returns>
        /// The full name of the assembly, or the class name if the full name cannot be determined.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Specifies flags that control binding and the way in which the search for members and types
    /// is conducted by reflection.
    /// </summary>
    [Flags]
    public enum BindingFlags
    {
        /// <summary>
        /// Specifies that reflection should create an instance of the specified type. Calls the constructor
        /// that matches the given arguments. The supplied member name is ignored. If the type of lookup is
        /// not specified, (Instance | Public) will apply. It is not possible to call a type initializer.This
        /// flag is passed to an InvokeMember method to invoke a constructor.
        /// </summary>
        CreateInstance = 512,
        /// <summary>
        /// Specifies that only members declared at the level of the supplied type's hierarchy should be
        /// considered. Inherited members are not considered.
        /// </summary>
        DeclaredOnly = 2,
        /// <summary>
        /// Specifies that no binding flags are defined.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Specifies that public and protected static members up the hierarchy should be returned. Private
        /// static members in inherited classes are not returned. Static members include fields, methods, events,
        /// and properties. Nested types are not returned.
        /// </summary>
        FlattenHierarchy = 64,
        /// <summary>
        /// Specifies that the value of the specified field should be returned.This flag is passed to an
        /// InvokeMember method to get a field value.
        /// </summary>
        GetField = 1024,
        /// <summary>
        /// Specifies that the value of the specified property should be returned.This flag is passed to
        /// an InvokeMember method to invoke a property getter.
        /// </summary>
        GetProperty = 4096,
        /// <summary>
        /// Specifies that the case of the member name should not be considered when binding.
        /// </summary>
        IgnoreCase = 1,
        /// <summary>
        /// Specifies that instance members are to be included in the search.
        /// </summary>
        Instance = 4,
        /// <summary>
        /// Specifies that a method is to be invoked. This must not be a constructor or a type initializer.This
        /// flag is passed to an InvokeMember method to invoke a method.
        /// </summary>
        InvokeMethod = 256,
        /// <summary>
        /// Specifies that non-public members are to be included in the search.
        /// </summary>
        NonPublic = 32,
        /// <summary>
        /// Specifies that public members are to be included in the search.
        /// </summary>
        Public = 16,
        /// <summary>
        /// Specifies that the value of the specified field should be set.This flag is passed to an InvokeMember
        /// method to set a field value.
        /// </summary>
        SetField = 2048,
        /// <summary>
        /// Specifies that the value of the specified property should be set. For COM properties, specifying
        /// this binding flag is equivalent to specifying PutDispProperty and PutRefDispProperty.This flag
        /// is passed to an InvokeMember method to invoke a property setter.
        /// </summary>
        SetProperty = 8192,
        /// <summary>
        /// Specifies that static members are to be included in the search.
        /// </summary>
        Static = 8,
    }
    /// <summary>
    /// Discovers the attributes of a class constructor and provides access to constructor metadata.
    /// </summary>
    public abstract partial class ConstructorInfo : System.Reflection.MethodBase
    {
        /// <summary>
        /// Represents the name of the class constructor method as it is stored in metadata. This name
        /// is always ".ctor". This field is read-only.
        /// </summary>
        public static readonly string ConstructorName;
        /// <summary>
        /// Represents the name of the type constructor method as it is stored in metadata. This name is
        /// always ".cctor". This property is read-only.
        /// </summary>
        public static readonly string TypeConstructorName;
        internal ConstructorInfo() { }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Invokes the constructor reflected by the instance that has the specified parameters, providing
        /// default values for the parameters not commonly used.
        /// </summary>
        /// <param name="parameters">
        /// An array of values that matches the number, order and type (under the constraints of the default
        /// binder) of the parameters for this constructor. If this constructor takes no parameters, then
        /// use either an array with zero elements or null, as in Object[] parameters = new Object[0]. Any object
        /// in this array that is not explicitly initialized with a value will contain the default value for that
        /// object type. For reference-type elements, this value is null. For value-type elements, this value
        /// is 0, 0.0, or false, depending on the specific element type.
        /// </param>
        /// <returns>
        /// An instance of the class associated with the constructor.
        /// </returns>
        /// <exception cref="MemberAccessException">
        /// The class is abstract.-or- The constructor is a class initializer.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The constructor is private or protected,
        /// and the caller lacks <see cref="Security.Permissions.ReflectionPermissionFlag.MemberAccess" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="parameters" /> array does not contain values that match the types accepted
        /// by this constructor.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// The invoked constructor throws an exception.
        /// </exception>
        /// <exception cref="TargetParameterCountException">
        /// An incorrect number of parameters was passed.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Creation of <see cref="TypedReference" />, <see cref="ArgIterator" />, and
        /// <see cref="RuntimeArgumentHandle" /> types is not supported.
        /// </exception>
        /// <exception cref="Security.SecurityException">
        /// The caller does not have the necessary code access permission.
        /// </exception>
        public virtual object Invoke(object[] parameters) { return default(object); }
        /// <summary>
        /// Gets a <see cref="MemberTypes" /> value indicating that this member is
        /// a constructor.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating that this member is a constructor.
        /// </returns>
        public override MemberTypes MemberType { get { return default(MemberTypes); } }
    }
    /// <summary>
    /// Provides access to custom attribute data for assemblies, modules, types, members and parameters
    /// that are loaded into the reflection-only context.
    /// </summary>
    public partial class CustomAttributeData
    {
        internal CustomAttributeData() { }
        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        /// <returns>
        /// The type of the attribute.
        /// </returns>
        public virtual System.Type AttributeType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the list of positional arguments specified for the attribute instance represented by
        /// the <see cref="CustomAttributeData" /> object.
        /// </summary>
        /// <returns>
        /// A collection of structures that represent the positional arguments specified for the custom
        /// attribute instance.
        /// </returns>
        public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument> ConstructorArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeTypedArgument>); } }
        /// <summary>
        /// Gets the list of named arguments specified for the attribute instance represented by the
        /// <see cref="CustomAttributeData" /> object.
        /// </summary>
        /// <returns>
        /// A collection of structures that represent the named arguments specified for the custom attribute
        /// instance.
        /// </returns>
        public virtual System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument> NamedArguments { get { return default(System.Collections.Generic.IList<System.Reflection.CustomAttributeNamedArgument>); } }
        /// <summary>
        /// Gets a <see cref="ConstructorInfo" /> object that represents the constructor
        /// that would have initialized the custom attribute.
        /// </summary>
        /// <returns>
        /// An object that represents the constructor that would have initialized the custom attribute
        /// represented by the current instance of the <see cref="CustomAttributeData" />
        /// class.
        /// </returns>
        public virtual ConstructorInfo Constructor { get { return default(ConstructorInfo); } }
        /// <summary>
        /// Returns a list of <see cref="CustomAttributeData" /> objects representing
        /// data about the attributes that have been applied to the target assembly.
        /// </summary>
        /// <param name="target">The assembly whose custom attribute data is to be retrieved.</param>
        /// <returns>
        /// A list of objects that represent data about the attributes that have been applied to the target
        /// assembly.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target" /> is null.</exception>
        public static System.Collections.Generic.IList<CustomAttributeData> GetCustomAttributes(Assembly target) { return default(System.Collections.Generic.IList<CustomAttributeData>); }
        /// <summary>
        /// Returns a list of <see cref="CustomAttributeData" /> objects representing
        /// data about the attributes that have been applied to the target member.
        /// </summary>
        /// <param name="target">The member whose attribute data is to be retrieved.</param>
        /// <returns>
        /// A list of objects that represent data about the attributes that have been applied to the target
        /// member.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target" /> is null.</exception>
        public static System.Collections.Generic.IList<CustomAttributeData> GetCustomAttributes(MemberInfo target) { return default(System.Collections.Generic.IList<CustomAttributeData>); }
        /// <summary>
        /// Returns a list of <see cref="CustomAttributeData" /> objects representing
        /// data about the attributes that have been applied to the target module.
        /// </summary>
        /// <param name="target">The module whose custom attribute data is to be retrieved.</param>
        /// <returns>
        /// A list of objects that represent data about the attributes that have been applied to the target
        /// module.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target" /> is null.</exception>
        public static System.Collections.Generic.IList<CustomAttributeData> GetCustomAttributes(Module target) { return default(System.Collections.Generic.IList<CustomAttributeData>); }
        /// <summary>
        /// Returns a list of <see cref="CustomAttributeData" /> objects representing
        /// data about the attributes that have been applied to the target parameter.
        /// </summary>
        /// <param name="target">The parameter whose attribute data is to be retrieved.</param>
        /// <returns>
        /// A list of objects that represent data about the attributes that have been applied to the target
        /// parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="target" /> is null.</exception>
        public static System.Collections.Generic.IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target) { return default(System.Collections.Generic.IList<CustomAttributeData>); }
    }
    /// <summary>
    /// Represents a named argument of a custom attribute in the reflection-only context.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct CustomAttributeNamedArgument
    {
        /// <summary>
        /// Gets a value that indicates whether the named argument is a field.
        /// </summary>
        /// <returns>
        /// true if the named argument is a field; otherwise, false.
        /// </returns>
        public bool IsField { get { return default(bool); } }
        /// <summary>
        /// Gets the name of the attribute member that would be used to set the named argument.
        /// </summary>
        /// <returns>
        /// The name of the attribute member that would be used to set the named argument.
        /// </returns>
        public string MemberName { get { return default(string); } }
        /// <summary>
        /// Gets a <see cref="CustomAttributeTypedArgument" /> structure that can
        /// be used to obtain the type and value of the current named argument.
        /// </summary>
        /// <returns>
        /// A structure that can be used to obtain the type and value of the current named argument.
        /// </returns>
        public System.Reflection.CustomAttributeTypedArgument TypedValue { get { return default(System.Reflection.CustomAttributeTypedArgument); } }
        /// <summary>
        /// Tests whether two <see cref="CustomAttributeNamedArgument" /> structures
        /// are equivalent.
        /// </summary>
        /// <param name="left">The structure to the left of the equality operator.</param>
        /// <param name="right">The structure to the right of the equality operator.</param>
        /// <returns>
        /// true if the two <see cref="CustomAttributeNamedArgument" /> structures
        /// are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right) { return default(bool); }
        /// <summary>
        /// Tests whether two <see cref="CustomAttributeNamedArgument" /> structures
        /// are different.
        /// </summary>
        /// <param name="left">The structure to the left of the inequality operator.</param>
        /// <param name="right">The structure to the right of the inequality operator.</param>
        /// <returns>
        /// true if the two <see cref="CustomAttributeNamedArgument" /> structures
        /// are different; otherwise, false.
        /// </returns>
        public static bool operator !=(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right) { return default(bool); }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns a string that consists of the argument name, the equal sign, and a string representation
        /// of the argument value.
        /// </summary>
        /// <returns>
        /// A string that consists of the argument name, the equal sign, and a string representation of
        /// the argument value.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Represents an argument of a custom attribute in the reflection-only context, or an element
    /// of an array argument.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct CustomAttributeTypedArgument
    {
        /// <summary>
        /// Gets the type of the argument or of the array argument element.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> object representing the type of the argument or of the array
        /// element.
        /// </returns>
        public System.Type ArgumentType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the value of the argument for a simple argument or for an element of an array argument;
        /// gets a collection of values for an array argument.
        /// </summary>
        /// <returns>
        /// An object that represents the value of the argument or element, or a generic
        /// <see cref="Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="CustomAttributeTypedArgument" /> objects that represent
        /// the values of an array-type argument.
        /// </returns>
        public object Value { get { return default(object); } }
        /// <summary>
        /// Tests whether two <see cref="CustomAttributeTypedArgument" /> structures
        /// are equivalent.
        /// </summary>
        /// <param name="left">
        /// The <see cref="CustomAttributeTypedArgument" /> structure to the left
        /// of the equality operator.
        /// </param>
        /// <param name="right">
        /// The <see cref="CustomAttributeTypedArgument" /> structure to the right
        /// of the equality operator.
        /// </param>
        /// <returns>
        /// true if the two <see cref="CustomAttributeTypedArgument" /> structures
        /// are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right) { return default(bool); }
        /// <summary>
        /// Tests whether two <see cref="CustomAttributeTypedArgument" /> structures
        /// are different.
        /// </summary>
        /// <param name="left">
        /// The <see cref="CustomAttributeTypedArgument" /> structure to the left
        /// of the inequality operator.
        /// </param>
        /// <param name="right">
        /// The <see cref="CustomAttributeTypedArgument" /> structure to the right
        /// of the inequality operator.
        /// </param>
        /// <returns>
        /// true if the two <see cref="CustomAttributeTypedArgument" /> structures
        /// are different; otherwise, false.
        /// </returns>
        public static bool operator !=(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right) { return default(bool); }
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same
        /// value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns a string consisting of the argument name, the equal sign, and a string representation
        /// of the argument value.
        /// </summary>
        /// <returns>
        /// A string consisting of the argument name, the equal sign, and a string representation of the
        /// argument value.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Discovers the attributes of an event and provides access to event metadata.
    /// </summary>
    public abstract partial class EventInfo : System.Reflection.MemberInfo
    {
        internal EventInfo() { }
        /// <summary>
        /// Gets the <see cref="MethodInfo" /> object for the
        /// <see cref="AddEventHandler(Object,Delegate)" /> method of the event, including non-public methods.
        /// </summary>
        /// <returns>
        /// The <see cref="MethodInfo" /> object for the
        /// <see cref="AddEventHandler(Object,Delegate)" /> method.
        /// </returns>
        public virtual System.Reflection.MethodInfo AddMethod { get { return default(System.Reflection.MethodInfo); } }
        /// <summary>
        /// Gets the attributes for this event.
        /// </summary>
        /// <returns>
        /// The read-only attributes for this event.
        /// </returns>
        public abstract System.Reflection.EventAttributes Attributes { get; }
        /// <summary>
        /// Gets the Type object of the underlying event-handler delegate associated with this event.
        /// </summary>
        /// <returns>
        /// A read-only Type object representing the delegate event handler.
        /// </returns>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual System.Type EventHandlerType { get { return default(System.Type); } }
        /// <summary>
        /// Gets a value indicating whether the event is multicast.
        /// </summary>
        /// <returns>
        /// true if the delegate is an instance of a multicast delegate; otherwise, false.
        /// </returns>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual bool IsMulticast { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the EventInfo has a name with a special meaning.
        /// </summary>
        /// <returns>
        /// true if this event has a special name; otherwise, false.
        /// </returns>
        public bool IsSpecialName { get { return default(bool); } }
        /// <summary>
        /// Gets the method that is called when the event is raised, including non-public methods.
        /// </summary>
        /// <returns>
        /// The method that is called when the event is raised.
        /// </returns>
        public virtual System.Reflection.MethodInfo RaiseMethod { get { return default(System.Reflection.MethodInfo); } }
        /// <summary>
        /// Gets the MethodInfo object for removing a method of the event, including non-public methods.
        /// </summary>
        /// <returns>
        /// The MethodInfo object for removing a method of the event.
        /// </returns>
        public virtual System.Reflection.MethodInfo RemoveMethod { get { return default(System.Reflection.MethodInfo); } }
        /// <summary>
        /// Adds an event handler to an event source.
        /// </summary>
        /// <param name="target">The event source.</param>
        /// <param name="handler">Encapsulates a method or methods to be invoked when the event is raised by the target.</param>
        /// <exception cref="InvalidOperationException">The event does not have a public add accessor.</exception>
        /// <exception cref="ArgumentException">The handler that was passed in cannot be used.</exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The caller does not have access permission
        /// to the member.
        /// </exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The <paramref name="target" /> parameter is null and the event is not static.-or-
        /// The <see cref="EventInfo" /> is not declared on the target.
        /// </exception>
        public virtual void AddEventHandler(object target, System.Delegate handler) { }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Removes an event handler from an event source.
        /// </summary>
        /// <param name="target">The event source.</param>
        /// <param name="handler">The delegate to be disassociated from the events raised by target.</param>
        /// <exception cref="InvalidOperationException">The event does not have a public remove accessor.</exception>
        /// <exception cref="ArgumentException">The handler that was passed in cannot be used.</exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The <paramref name="target" /> parameter is null and the event is not static.-or-
        /// The <see cref="EventInfo" /> is not declared on the target.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The caller does not have access permission
        /// to the member.
        /// </exception>
        public virtual void RemoveEventHandler(object target, System.Delegate handler) { }
        /// <summary>
        /// Returns the method used to add an event handler delegate to the event source.
        /// </summary>
        /// <returns>
        /// A <see cref="MethodInfo" /> object representing the method used to add
        /// an event handler delegate to the event source.
        /// </returns>
        public MethodInfo GetAddMethod() { return default(MethodInfo); }
        /// <summary>
        /// When overridden in a derived class, retrieves the MethodInfo object for the
        /// <see cref="AddEventHandler(Object,Delegate)" /> method of the event, specifying whether to return non-public methods.
        /// </summary>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false.</param>
        /// <returns>
        /// A <see cref="MethodInfo" /> object representing the method used to add
        /// an event handler delegate to the event source.
        /// </returns>
        /// <exception cref="MethodAccessException">
        /// <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is
        /// non-public, and the caller does not have permission to reflect on non-public methods.
        /// </exception>
        public abstract MethodInfo GetAddMethod(bool nonPublic);
        /// <summary>
        /// Returns the method that is called when the event is raised.
        /// </summary>
        /// <returns>
        /// The method that is called when the event is raised.
        /// </returns>
        public MethodInfo GetRaiseMethod() { return default(MethodInfo); }
        /// <summary>
        /// When overridden in a derived class, returns the method that is called when the event is raised,
        /// specifying whether to return non-public methods.
        /// </summary>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false.</param>
        /// <returns>
        /// A MethodInfo object that was called when the event was raised.
        /// </returns>
        /// <exception cref="MethodAccessException">
        /// <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is
        /// non-public, and the caller does not have permission to reflect on non-public methods.
        /// </exception>
        public abstract MethodInfo GetRaiseMethod(bool nonPublic);
        /// <summary>
        /// Returns the method used to remove an event handler delegate from the event source.
        /// </summary>
        /// <returns>
        /// A <see cref="MethodInfo" /> object representing the method used to remove
        /// an event handler delegate from the event source.
        /// </returns>
        public MethodInfo GetRemoveMethod() { return default(MethodInfo); }
        /// <summary>
        /// When overridden in a derived class, retrieves the MethodInfo object for removing a method of
        /// the event, specifying whether to return non-public methods.
        /// </summary>
        /// <param name="nonPublic">true if non-public methods can be returned; otherwise, false.</param>
        /// <returns>
        /// A <see cref="MethodInfo" /> object representing the method used to remove
        /// an event handler delegate from the event source.
        /// </returns>
        /// <exception cref="MethodAccessException">
        /// <paramref name="nonPublic" /> is true, the method used to add an event handler delegate is
        /// non-public, and the caller does not have permission to reflect on non-public methods.
        /// </exception>
        public abstract MethodInfo GetRemoveMethod(bool nonPublic);
        /// <summary>
        /// Gets a <see cref="MemberTypes" /> value indicating that this member is
        /// an event.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating that this member is an event.
        /// </returns>
        public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }

    }
    /// <summary>
    /// Discovers the attributes of a field and provides access to field metadata.
    /// </summary>
    public abstract partial class FieldInfo : System.Reflection.MemberInfo
    {
        internal FieldInfo() { }
        /// <summary>
        /// Gets the attributes associated with this field.
        /// </summary>
        /// <returns>
        /// The FieldAttributes for this field.
        /// </returns>
        public abstract System.Reflection.FieldAttributes Attributes { get; }
        /// <summary>
        /// Gets the type of this field object.
        /// </summary>
        /// <returns>
        /// The type of this field object.
        /// </returns>
        public abstract System.Type FieldType { get; }
        /// <summary>
        /// Gets a value indicating whether the potential visibility of this field is described by
        /// <see cref="FieldAttributes.Assembly" />; that is, the field is visible at most
        /// to other types in the same assembly, and is not visible to derived types outside the assembly.
        /// </summary>
        /// <returns>
        /// true if the visibility of this field is exactly described by
        /// <see cref="FieldAttributes.Assembly" />; otherwise, false.
        /// </returns>
        public bool IsAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the visibility of this field is described by
        /// <see cref="FieldAttributes.Family" />; that is, the field is visible only within its class and derived classes.
        /// </summary>
        /// <returns>
        /// true if access to this field is exactly described by <see cref="FieldAttributes.Family" />
        /// ; otherwise, false.
        /// </returns>
        public bool IsFamily { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the visibility of this field is described by
        /// <see cref="FieldAttributes.FamANDAssem" />; that is, the field can be accessed from derived classes, but only if they are in the same
        /// assembly.
        /// </summary>
        /// <returns>
        /// true if access to this field is exactly described by
        /// <see cref="FieldAttributes.FamANDAssem" />; otherwise, false.
        /// </returns>
        public bool IsFamilyAndAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the potential visibility of this field is described by
        /// <see cref="FieldAttributes.FamORAssem" />; that is, the field can be accessed
        /// by derived classes wherever they are, and by classes in the same assembly.
        /// </summary>
        /// <returns>
        /// true if access to this field is exactly described by
        /// <see cref="FieldAttributes.FamORAssem" />; otherwise, false.
        /// </returns>
        public bool IsFamilyOrAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the field can only be set in the body of the constructor.
        /// </summary>
        /// <returns>
        /// true if the field has the InitOnly attribute set; otherwise, false.
        /// </returns>
        public bool IsInitOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the value is written at compile time and cannot be changed.
        /// </summary>
        /// <returns>
        /// true if the field has the Literal attribute set; otherwise, false.
        /// </returns>
        public bool IsLiteral { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the field is private.
        /// </summary>
        /// <returns>
        /// true if the field is private; otherwise; false.
        /// </returns>
        public bool IsPrivate { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the field is public.
        /// </summary>
        /// <returns>
        /// true if this field is public; otherwise, false.
        /// </returns>
        public bool IsPublic { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the corresponding SpecialName attribute is set in the
        /// <see cref="FieldAttributes" /> enumerator.
        /// </summary>
        /// <returns>
        /// true if the SpecialName attribute is set in <see cref="FieldAttributes" />
        /// ; otherwise, false.
        /// </returns>
        public bool IsSpecialName { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the field is static.
        /// </summary>
        /// <returns>
        /// true if this field is static; otherwise, false.
        /// </returns>
        public bool IsStatic { get { return default(bool); } }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Gets a <see cref="FieldInfo" /> for the field represented by the specified
        /// handle.
        /// </summary>
        /// <param name="handle">
        /// A <see cref="RuntimeFieldHandle" /> structure that contains the handle to the internal
        /// metadata representation of a field.
        /// </param>
        /// <returns>
        /// A <see cref="FieldInfo" /> object representing the field specified by
        /// <paramref name="handle" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="handle" /> is invalid.</exception>
        public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle) { return default(System.Reflection.FieldInfo); }
        /// <summary>
        /// Gets a <see cref="FieldInfo" /> for the field represented by the specified
        /// handle, for the specified generic type.
        /// </summary>
        /// <param name="handle">
        /// A <see cref="RuntimeFieldHandle" /> structure that contains the handle to the internal
        /// metadata representation of a field.
        /// </param>
        /// <param name="declaringType">
        /// A <see cref="RuntimeTypeHandle" /> structure that contains the handle to the generic
        /// type that defines the field.
        /// </param>
        /// <returns>
        /// A <see cref="FieldInfo" /> object representing the field specified by
        /// <paramref name="handle" />, in the generic type specified by <paramref name="declaringType" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="handle" /> is invalid.-or-<paramref name="declaringType" /> is not compatible
        /// with <paramref name="handle" />. For example, <paramref name="declaringType" /> is the runtime
        /// type handle of the generic type definition, and <paramref name="handle" /> comes from a constructed
        /// type. See Remarks.
        /// </exception>
        public static System.Reflection.FieldInfo GetFieldFromHandle(System.RuntimeFieldHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.FieldInfo); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Gets an array of types that identify the optional custom modifiers of the field.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the optional custom modifiers
        /// of the current field, such as <see cref="Runtime.CompilerServices.IsConst" />.
        /// </returns>
        public virtual Type[] GetOptionalCustomModifiers() { return default(Type[]); }
        /// <summary>
        /// Returns a literal value associated with the field by a compiler.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> that contains the literal value associated with the field.
        /// If the literal value is a class type with an element value of zero, the return value is null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Constant table in unmanaged metadata does not contain a constant value for the current
        /// field.
        /// </exception>
        /// <exception cref="FormatException">
        /// The type of the value is not one of the types permitted by the Common Language Specification
        /// (CLS). See the ECMA Partition II specification Metadata Logical Format: Other Structures, Element
        /// Types used in Signatures.
        /// </exception>
        /// <exception cref="NotSupportedException">The constant value for the field is not set.</exception>
        public virtual object GetRawConstantValue() { return default(object); }
        /// <summary>
        /// Gets an array of types that identify the required custom modifiers of the property.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the required custom modifiers
        /// of the current property, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsImplicitlyDereferenced" />.
        /// </returns>
        public virtual Type[] GetRequiredCustomModifiers() { return default(Type[]); }
        /// <summary>
        /// When overridden in a derived class, returns the value of a field supported by a given object.
        /// </summary>
        /// <param name="obj">The object whose field value will be returned.</param>
        /// <returns>
        /// An object containing the value of the field reflected by this instance.
        /// </returns>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The field is non-static and <paramref name="obj" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// A field is marked literal, but the field does not have one of the accepted literal types.
        /// </exception>
        /// <exception cref="FieldAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The caller does not have permission
        /// to access this field.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The method is neither declared nor inherited by the class of <paramref name="obj" />.
        /// </exception>
        public abstract object GetValue(object obj);
        /// <summary>
        /// Gets a <see cref="MemberTypes" /> value indicating that this member is
        /// a field.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating that this member is a field.
        /// </returns>
        public override MemberTypes MemberType { get { return default(MemberTypes); } }
        /// <summary>
        /// Sets the value of the field supported by the given object.
        /// </summary>
        /// <param name="obj">The object whose field value will be set.</param>
        /// <param name="value">The value to assign to the field.</param>
        /// <exception cref="FieldAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The caller does not have permission
        /// to access this field.
        /// </exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The <paramref name="obj" /> parameter is null and the field is an instance field.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The field does not exist on the object.-or- The <paramref name="value" /> parameter cannot
        /// be converted and stored in the field.
        /// </exception>
        public virtual void SetValue(object obj, object value) { }

    }
    /// <summary>
    /// Provides custom attributes for reflection objects that support them.
    /// </summary>
    public interface ICustomAttributeProvider
    {
        /// <summary>
        /// Returns an array of all of the custom attributes defined on this member, excluding named attributes,
        /// or an empty array if there are no custom attributes.
        /// </summary>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// An array of Objects representing custom attributes, or an empty array.
        /// </returns>
        /// <exception cref="TypeLoadException">The custom attribute type cannot be loaded.</exception>
        /// <exception cref="AmbiguousMatchException">
        /// There is more than one attribute of type <paramref name="attributeType" /> defined on this
        /// member.
        /// </exception>
        object[] GetCustomAttributes(bool inherit);
        /// <summary>
        /// Returns an array of custom attributes defined on this member, identified by type, or an empty
        /// array if there are no custom attributes of that type.
        /// </summary>
        /// <param name="attributeType">The type of the custom attributes.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// An array of Objects representing custom attributes, or an empty array.
        /// </returns>
        /// <exception cref="TypeLoadException">The custom attribute type cannot be loaded.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="attributeType" /> is null.</exception>
        object[] GetCustomAttributes(Type attributeType, bool inherit);
        /// <summary>
        /// Indicates whether one or more instance of <paramref name="attributeType" /> is defined on
        /// this member.
        /// </summary>
        /// <param name="attributeType">The type of the custom attributes.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>
        /// true if the <paramref name="attributeType" /> is defined on this member; false otherwise.
        /// </returns>
        bool IsDefined(Type attributeType, bool inherit);
    }
    /// <summary>
    /// Contains methods for converting <see cref="Type" /> objects.
    /// </summary>
    public static partial class IntrospectionExtensions
    {
        /// <summary>
        /// Returns the <see cref="TypeInfo" /> representation of the specified type.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>
        /// The converted object.
        /// </returns>
        public static System.Reflection.TypeInfo GetTypeInfo(this System.Type type) { return default(System.Reflection.TypeInfo); }
    }
    /// <summary>
    /// The exception that is thrown in
    /// <see cref="Type.FindMembers(MemberTypes,BindingFlags,MemberFilter,Object)" /> when the filter criteria is not valid for the type of filter you are using.
    /// </summary>
    public partial class InvalidFilterCriteriaException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFilterCriteriaException" />
        /// class with the default properties.
        /// </summary>
        public InvalidFilterCriteriaException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFilterCriteriaException" />
        /// class with the given HRESULT and message string.
        /// </summary>
        /// <param name="message">The message text for the exception.</param>
        public InvalidFilterCriteriaException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFilterCriteriaException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public InvalidFilterCriteriaException(string message, Exception inner) { }
    }
    /// <summary>
    /// Represents a type that you can reflect over.
    /// </summary>
    public partial interface IReflectableType
    {
        /// <summary>
        /// Retrieves an object that represents this type.
        /// </summary>
        /// <returns>
        /// An object that represents this type.
        /// </returns>
        System.Reflection.TypeInfo GetTypeInfo();
    }
    /// <summary>
    /// Discovers the attributes of a local variable and provides access to local variable metadata.
    /// </summary>
    public partial class LocalVariableInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalVariableInfo" /> class.
        /// </summary>
        protected LocalVariableInfo() { }
        /// <summary>
        /// Gets a <see cref="Boolean" /> value that indicates whether the object referred to
        /// by the local variable is pinned in memory.
        /// </summary>
        /// <returns>
        /// true if the object referred to by the variable is pinned in memory; otherwise, false.
        /// </returns>
        public virtual bool IsPinned { get { return default(bool); } }
        /// <summary>
        /// Gets the index of the local variable within the method body.
        /// </summary>
        /// <returns>
        /// An integer value that represents the order of declaration of the local variable within the
        /// method body.
        /// </returns>
        public virtual int LocalIndex { get { return default(int); } }
        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        /// <returns>
        /// The type of the local variable.
        /// </returns>
        public virtual System.Type LocalType { get { return default(System.Type); } }
        /// <summary>
        /// Returns a user-readable string that describes the local variable.
        /// </summary>
        /// <returns>
        /// A string that displays information about the local variable, including the type name, index,
        /// and pinned status.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Provides access to manifest resources, which are XML files that describe application dependencies.
    /// </summary>
    public partial class ManifestResourceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestResourceInfo" />
        /// class for a resource that is contained by the specified assembly and file, and that has the
        /// specified location.
        /// </summary>
        /// <param name="containingAssembly">The assembly that contains the manifest resource.</param>
        /// <param name="containingFileName">
        /// The name of the file that contains the manifest resource, if the file is not the same as the
        /// manifest file.
        /// </param>
        /// <param name="resourceLocation">
        /// A bitwise combination of enumeration values that provides information about the location of
        /// the manifest resource.
        /// </param>
        public ManifestResourceInfo(System.Reflection.Assembly containingAssembly, string containingFileName, System.Reflection.ResourceLocation resourceLocation) { }
        /// <summary>
        /// Gets the name of the file that contains the manifest resource, if it is not the same as the
        /// manifest file.
        /// </summary>
        /// <returns>
        /// The manifest resource's file name.
        /// </returns>
        public virtual string FileName { get { return default(string); } }
        /// <summary>
        /// Gets the containing assembly for the manifest resource.
        /// </summary>
        /// <returns>
        /// The manifest resource's containing assembly.
        /// </returns>
        public virtual System.Reflection.Assembly ReferencedAssembly { get { return default(System.Reflection.Assembly); } }
        /// <summary>
        /// Gets the manifest resource's location.
        /// </summary>
        /// <returns>
        /// A bitwise combination of <see cref="Reflection.ResourceLocation" /> flags that indicates
        /// the location of the manifest resource.
        /// </returns>
        public virtual System.Reflection.ResourceLocation ResourceLocation { get { return default(System.Reflection.ResourceLocation); } }
    }
    /// <summary>
    /// Represents a delegate that is used to filter a list of members represented in an array of
    /// <see cref="MemberInfo" /> objects.
    /// </summary>
    /// <param name="m">The <see cref="MemberInfo" /> object to which the filter is applied.</param>
    /// <param name="filterCriteria">An arbitrary object used to filter the list.</param>
    /// <returns>
    /// true to include the member in the filtered list; otherwise false.
    /// </returns>
    public delegate bool MemberFilter(MemberInfo m, object filterCriteria);
    /// <summary>
    /// Obtains information about the attributes of a member and provides access to member metadata.
    /// </summary>
    public abstract partial class MemberInfo : System.Reflection.ICustomAttributeProvider
    {
        internal MemberInfo() { }
        /// <summary>
        /// Gets a collection that contains this member's custom attributes.
        /// </summary>
        /// <returns>
        /// A collection that contains this member's custom attributes.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        /// <summary>
        /// Gets the class that declares this member.
        /// </summary>
        /// <returns>
        /// The Type object for the class that declares this member.
        /// </returns>
        public abstract System.Type DeclaringType { get; }
        /// <summary>
        /// Gets a value that identifies a metadata element.
        /// </summary>
        /// <returns>
        /// A value which, in combination with <see cref="Module" />, uniquely
        /// identifies a metadata element.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="MemberInfo" /> represents an array method, such
        /// as Address, on an array type whose element type is a dynamic type that has not been completed.
        /// To get a metadata token in this case, pass the <see cref="MemberInfo" />
        /// object to the <see cref="Emit.ModuleBuilder.GetMethodToken(MethodInfo)" />
        /// method; or use the
        /// <see cref="Emit.ModuleBuilder.GetArrayMethodToken(Type,String,CallingConventions,Type,Type[])" />  method to get the token directly, instead of using the
        /// <see cref="Emit.ModuleBuilder.GetArrayMethod(Type,String,CallingConventions,Type,Type[])" /> method to get a <see cref="MethodInfo" /> first.
        /// </exception>
        public virtual int MetadataToken { get { return default(int); } }
        /// <summary>
        /// Gets the module in which the type that declares the member represented by the current
        /// <see cref="MemberInfo" /> is defined.
        /// </summary>
        /// <returns>
        /// The <see cref="Reflection.Module" /> in which the type that declares the member represented
        /// by the current <see cref="MemberInfo" /> is defined.
        /// </returns>
        /// <exception cref="NotImplementedException">This method is not implemented.</exception>
        public virtual System.Reflection.Module Module { get { return default(System.Reflection.Module); } }
        /// <summary>
        /// Gets the name of the current member.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> containing the name of this member.
        /// </returns>
        public abstract string Name { get; }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// When overridden in a derived class, gets a <see cref="MemberTypes" />
        /// value indicating the type of the member — method, constructor, event, and so on.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating the type of member.
        /// </returns>
        public abstract MemberTypes MemberType { get; }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit) { return default(object[]); }
        bool System.Reflection.ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit) { return default(bool); }
    }
    /// <summary>
    /// Marks each type of member that is defined as a derived class of <see cref="MemberInfo" />.
    /// </summary>
    [Flags]
    public enum MemberTypes
    {
        /// <summary>
        /// Specifies that the member is a constructor
        /// </summary>
        Constructor = 0x01,
        /// <summary>
        /// Specifies that the member is an event.
        /// </summary>
        Event = 0x02,
        /// <summary>
        /// Specifies that the member is a field.
        /// </summary>
        Field = 0x04,
        /// <summary>
        /// Specifies that the member is a method.
        /// </summary>
        Method = 0x08,
        /// <summary>
        /// Specifies that the member is a property.
        /// </summary>
        Property = 0x10,
        /// <summary>
        /// Specifies that the member is a type.
        /// </summary>
        TypeInfo = 0x20,
        /// <summary>
        /// Specifies that the member is a custom member type.
        /// </summary>
        Custom = 0x40,
        /// <summary>
        /// Specifies that the member is a nested type.
        /// </summary>
        NestedType = 0x80,
        /// <summary>
        /// Specifies all member types.
        /// </summary>
        All = Constructor | Event | Field | Method | Property | TypeInfo | NestedType,
    }
    /// <summary>
    /// Provides information about methods and constructors.
    /// </summary>
    public abstract partial class MethodBase : System.Reflection.MemberInfo
    {
        internal MethodBase() { }
        /// <summary>
        /// Gets the attributes associated with this method.
        /// </summary>
        /// <returns>
        /// One of the <see cref="MethodAttributes" /> values.
        /// </returns>
        public abstract System.Reflection.MethodAttributes Attributes { get; }
        /// <summary>
        /// Gets a value indicating the calling conventions for this method.
        /// </summary>
        /// <returns>
        /// The <see cref="CallingConventions" /> for this method.
        /// </returns>
        public virtual System.Reflection.CallingConventions CallingConvention { get { return default(System.Reflection.CallingConventions); } }
        /// <summary>
        /// Gets a value indicating whether the generic method contains unassigned generic type parameters.
        /// </summary>
        /// <returns>
        /// true if the current <see cref="MethodBase" /> object represents a generic
        /// method that contains unassigned generic type parameters; otherwise, false.
        /// </returns>
        public virtual bool ContainsGenericParameters { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is abstract.
        /// </summary>
        /// <returns>
        /// true if the method is abstract; otherwise, false.
        /// </returns>
        public bool IsAbstract { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the potential visibility of this method or constructor is
        /// described by <see cref="MethodAttributes.Assembly" />; that is, the method
        /// or constructor is visible at most to other types in the same assembly, and is not visible
        /// to derived types outside the assembly.
        /// </summary>
        /// <returns>
        /// true if the visibility of this method or constructor is exactly described by
        /// <see cref="MethodAttributes.Assembly" />; otherwise, false.
        /// </returns>
        public bool IsAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is a constructor.
        /// </summary>
        /// <returns>
        /// true if this method is a constructor represented by a <see cref="ConstructorInfo" />
        /// object (see note in Remarks about <see cref="Emit.ConstructorBuilder" />
        /// objects); otherwise, false.
        /// </returns>
        public bool IsConstructor { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the visibility of this method or constructor is described
        /// by <see cref="MethodAttributes.Family" />; that is, the method or constructor
        /// is visible only within its class and derived classes.
        /// </summary>
        /// <returns>
        /// true if access to this method or constructor is exactly described by
        /// <see cref="MethodAttributes.Family" />; otherwise, false.
        /// </returns>
        public bool IsFamily { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the visibility of this method or constructor is described
        /// by <see cref="MethodAttributes.FamANDAssem" />; that is, the method or
        /// constructor can be called by derived classes, but only if they are in the same assembly.
        /// </summary>
        /// <returns>
        /// true if access to this method or constructor is exactly described by
        /// <see cref="MethodAttributes.FamANDAssem" />; otherwise, false.
        /// </returns>
        public bool IsFamilyAndAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the potential visibility of this method or constructor is
        /// described by <see cref="MethodAttributes.FamORAssem" />; that is, the
        /// method or constructor can be called by derived classes wherever they are, and by classes in
        /// the same assembly.
        /// </summary>
        /// <returns>
        /// true if access to this method or constructor is exactly described by
        /// <see cref="MethodAttributes.FamORAssem" />; otherwise, false.
        /// </returns>
        public bool IsFamilyOrAssembly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this method is final.
        /// </summary>
        /// <returns>
        /// true if this method is final; otherwise, false.
        /// </returns>
        public bool IsFinal { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is generic.
        /// </summary>
        /// <returns>
        /// true if the current <see cref="MethodBase" /> represents a generic method;
        /// otherwise, false.
        /// </returns>
        public virtual bool IsGenericMethod { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is a generic method definition.
        /// </summary>
        /// <returns>
        /// true if the current <see cref="MethodBase" /> object represents the definition
        /// of a generic method; otherwise, false.
        /// </returns>
        public virtual bool IsGenericMethodDefinition { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether only a member of the same kind with exactly the same signature
        /// is hidden in the derived class.
        /// </summary>
        /// <returns>
        /// true if the member is hidden by signature; otherwise, false.
        /// </returns>
        public bool IsHideBySig { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this member is private.
        /// </summary>
        /// <returns>
        /// true if access to this method is restricted to other members of the class itself; otherwise,
        /// false.
        /// </returns>
        public bool IsPrivate { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this is a public method.
        /// </summary>
        /// <returns>
        /// true if this method is public; otherwise, false.
        /// </returns>
        public bool IsPublic { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this method has a special name.
        /// </summary>
        /// <returns>
        /// true if this method has a special name; otherwise, false.
        /// </returns>
        public bool IsSpecialName { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is static.
        /// </summary>
        /// <returns>
        /// true if this method is static; otherwise, false.
        /// </returns>
        public bool IsStatic { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the method is virtual.
        /// </summary>
        /// <returns>
        /// true if this method is virtual; otherwise, false.
        /// </returns>
        public bool IsVirtual { get { return default(bool); } }
        /// <summary>
        /// Gets the <see cref="MethodImplAttributes" /> flags that specify the attributes
        /// of a method implementation.
        /// </summary>
        /// <returns>
        /// The method implementation flags.
        /// </returns>
        public abstract System.Reflection.MethodImplAttributes MethodImplementationFlags { get; }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns an array of <see cref="Type" /> objects that represent the type arguments
        /// of a generic method or the type parameters of a generic method definition.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that represent the type arguments of a generic
        /// method or the type parameters of a generic method definition. Returns an empty array if the
        /// current method is not a generic method.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The current object is a <see cref="ConstructorInfo" />. Generic constructors
        /// are not supported in the .NET Framework version 2.0. This exception is the default behavior
        /// if this method is not overridden in a derived class.
        /// </exception>
        public virtual System.Type[] GetGenericArguments() { return default(System.Type[]); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Gets method information by using the method's internal metadata representation (handle).
        /// </summary>
        /// <param name="handle">The method's handle.</param>
        /// <returns>
        /// A MethodBase containing information about the method.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="handle" /> is invalid.</exception>
        public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle) { return default(System.Reflection.MethodBase); }
        /// <summary>
        /// Gets a <see cref="MethodBase" /> object for the constructor or method
        /// represented by the specified handle, for the specified generic type.
        /// </summary>
        /// <param name="handle">A handle to the internal metadata representation of a constructor or method.</param>
        /// <param name="declaringType">A handle to the generic type that defines the constructor or method.</param>
        /// <returns>
        /// A <see cref="MethodBase" /> object representing the method or constructor
        /// specified by <paramref name="handle" />, in the generic type specified by <paramref name="declaringType" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="handle" /> is invalid.</exception>
        public static System.Reflection.MethodBase GetMethodFromHandle(System.RuntimeMethodHandle handle, System.RuntimeTypeHandle declaringType) { return default(System.Reflection.MethodBase); }
        /// <summary>
        /// When overridden in a derived class, gets the parameters of the specified method or constructor.
        /// </summary>
        /// <returns>
        /// An array of type ParameterInfo containing information that matches the signature of the method
        /// (or constructor) reflected by this MethodBase instance.
        /// </returns>
        public abstract System.Reflection.ParameterInfo[] GetParameters();
        /// <summary>
        /// Invokes the method or constructor represented by the current instance, using the specified
        /// parameters.
        /// </summary>
        /// <param name="obj">
        /// The object on which to invoke the method or constructor. If a method is static, this argument
        /// is ignored. If a constructor is static, this argument must be null or an instance of the class that
        /// defines the constructor.
        /// </param>
        /// <param name="parameters">
        /// An argument list for the invoked method or constructor. This is an array of objects with the
        /// same number, order, and type as the parameters of the method or constructor to be invoked.
        /// If there are no parameters, <paramref name="parameters" /> should be null.If the method or
        /// constructor represented by this instance takes a ref parameter (ByRef in Visual Basic), no
        /// special attribute is required for that parameter in order to invoke the method or constructor
        /// using this function. Any object in this array that is not explicitly initialized with a value
        /// will contain the default value for that object type. For reference-type elements, this value
        /// is null. For value-type elements, this value is 0, 0.0, or false, depending on the specific
        /// element type.
        /// </param>
        /// <returns>
        /// An object containing the return value of the invoked method, or null in the case of a constructor.Elements
        /// of the <paramref name="parameters" /> array that represent parameters declared with the ref
        /// or out keyword may also be modified.
        /// </returns>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The <paramref name="obj" /> parameter is null and the method is not static.-or-
        /// The method is not declared or inherited by the class of <paramref name="obj" />. -or-A static
        /// constructor is invoked, and <paramref name="obj" /> is neither null nor an instance of the
        /// class that declared the constructor.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The elements of the <paramref name="parameters" />array do not match the signature of the
        /// method or constructor reflected by this instance.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// The invoked method or constructor throws an exception. -or-The current instance is a
        /// <see cref="Emit.DynamicMethod" /> that contains unverifiable code. See the
        /// "Verification" section in Remarks for <see cref="Emit.DynamicMethod" />.
        /// </exception>
        /// <exception cref="TargetParameterCountException">
        /// The <paramref name="parameters" /> array does not have the correct number of arguments.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.The caller does not have permission
        /// to execute the method or constructor that is represented by the current instance.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The type that declares the method is an open generic type. That is, the
        /// <see cref="Type.ContainsGenericParameters" /> property returns true for the declaring type.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The current instance is a <see cref="Emit.MethodBuilder" />.
        /// </exception>
        public virtual object Invoke(object obj, object[] parameters) { return default(object); }
        /// <summary>
        /// When overridden in a derived class, returns the <see cref="MethodImplAttributes" />
        /// flags.
        /// </summary>
        /// <returns>
        /// The MethodImplAttributes flags.
        /// </returns>
        public abstract MethodImplAttributes GetMethodImplementationFlags();
    }
    /// <summary>
    /// Discovers the attributes of a method and provides access to method metadata.
    /// </summary>
    public abstract partial class MethodInfo : System.Reflection.MethodBase
    {
        internal MethodInfo() { }
        /// <summary>
        /// When overridden in a derived class, returns the <see cref="MethodInfo" />
        /// object for the method on the direct or indirect base class in which the method represented
        /// by this instance was first declared.
        /// </summary>
        /// <returns>
        /// A <see cref="MethodInfo" /> object for the first implementation of this
        /// method.
        /// </returns>
        public abstract MethodInfo GetBaseDefinition();
        /// <summary>
        /// Gets a <see cref="ParameterInfo" /> object that contains information about
        /// the return type of the method, such as whether the return type has custom modifiers.
        /// </summary>
        /// <returns>
        /// A <see cref="ParameterInfo" /> object that contains information about
        /// the return type.
        /// </returns>
        /// <exception cref="NotImplementedException">This method is not implemented.</exception>
        public virtual System.Reflection.ParameterInfo ReturnParameter { get { return default(System.Reflection.ParameterInfo); } }
        /// <summary>
        /// Gets the return type of this method.
        /// </summary>
        /// <returns>
        /// The return type of this method.
        /// </returns>
        public virtual System.Type ReturnType { get { return default(System.Type); } }
        /// <summary>
        /// Creates a delegate of the specified type from this method.
        /// </summary>
        /// <param name="delegateType">The type of the delegate to create.</param>
        /// <returns>
        /// The delegate for this method.
        /// </returns>
        public virtual System.Delegate CreateDelegate(System.Type delegateType) { return default(System.Delegate); }
        /// <summary>
        /// Creates a delegate of the specified type with the specified target from this method.
        /// </summary>
        /// <param name="delegateType">The type of the delegate to create.</param>
        /// <param name="target">The object targeted by the delegate.</param>
        /// <returns>
        /// The delegate for this method.
        /// </returns>
        public virtual System.Delegate CreateDelegate(System.Type delegateType, object target) { return default(System.Delegate); }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns an array of <see cref="Type" /> objects that represent the type arguments
        /// of a generic method or the type parameters of a generic method definition.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that represent the type arguments of a generic
        /// method or the type parameters of a generic method definition. Returns an empty array if the
        /// current method is not a generic method.
        /// </returns>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public override System.Type[] GetGenericArguments() { return default(System.Type[]); }
        /// <summary>
        /// Returns a <see cref="MethodInfo" /> object that represents a generic method
        /// definition from which the current method can be constructed.
        /// </summary>
        /// <returns>
        /// A <see cref="MethodInfo" /> object representing a generic method definition
        /// from which the current method can be constructed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The current method is not a generic method. That is,
        /// <see cref="MethodInfo.IsGenericMethod" /> returns false.
        /// </exception>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public virtual System.Reflection.MethodInfo GetGenericMethodDefinition() { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic
        /// method definition, and returns a <see cref="MethodInfo" /> object representing
        /// the resulting constructed method.
        /// </summary>
        /// <param name="typeArguments">
        /// An array of types to be substituted for the type parameters of the current generic method definition.
        /// </param>
        /// <returns>
        /// A <see cref="MethodInfo" /> object that represents the constructed method
        /// formed by substituting the elements of <paramref name="typeArguments" /> for the type parameters
        /// of the current generic method definition.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="MethodInfo" /> does not represent a generic method
        /// definition. That is, <see cref="MethodInfo.IsGenericMethodDefinition" />
        /// returns false.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typeArguments" /> is null.-or- Any element of <paramref name="typeArguments" />
        /// is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in <paramref name="typeArguments" /> is not the same as the number
        /// of type parameters of the current generic method definition.-or- An element of <paramref name="typeArguments" />
        /// does not satisfy the constraints specified for the corresponding type parameter of the
        /// current generic method definition.
        /// </exception>
        /// <exception cref="NotSupportedException">This method is not supported.</exception>
        public virtual System.Reflection.MethodInfo MakeGenericMethod(params System.Type[] typeArguments) { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Gets a <see cref="MemberTypes" /> value indicating that this member is
        /// a method.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating that this member is a method.
        /// </returns>
        public override MemberTypes MemberType { get { return default(MemberTypes); } }
        /// <summary>
        /// Gets the custom attributes for the return type.
        /// </summary>
        /// <returns>
        /// An ICustomAttributeProvider object representing the custom attributes for the return type.
        /// </returns>
        public abstract System.Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes { get; }
    }
    /// <summary>
    /// Performs reflection on a module.
    /// </summary>
    public abstract partial class Module : System.Reflection.ICustomAttributeProvider
    {
        internal Module() { }
        /// <summary>
        /// Gets the appropriate <see cref="Reflection.Assembly" /> for this instance of
        /// <see cref="Module" />.
        /// </summary>
        /// <returns>
        /// An Assembly object.
        /// </returns>
        public virtual System.Reflection.Assembly Assembly { get { return default(System.Reflection.Assembly); } }
        /// <summary>
        /// Gets a collection that contains this module's custom attributes.
        /// </summary>
        /// <returns>
        /// A collection that contains this module's custom attributes.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        /// <summary>
        /// Gets a string representing the fully qualified name and path to this module.
        /// </summary>
        /// <returns>
        /// The fully qualified module name.
        /// </returns>
        /// <exception cref="Security.SecurityException">The caller does not have the required permissions.</exception>
        public virtual string FullyQualifiedName { get { return default(string); } }
        /// <summary>
        /// Gets a String representing the name of the module with the path removed.
        /// </summary>
        /// <returns>
        /// The module name with no path.
        /// </returns>
        public virtual string Name { get { return default(string); } }
        /// <summary>
        /// Determines whether this module and the specified object are equal.
        /// </summary>
        /// <param name="o">The object to compare with this instance.</param>
        /// <returns>
        /// true if <paramref name="o" /> is equal to this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object o) { return default(bool); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns the specified type, specifying whether to make a case-sensitive search of the module
        /// and whether to throw an exception if the type cannot be found.
        /// </summary>
        /// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
        /// <param name="throwOnError">true to throw an exception if the type cannot be found; false to return null.</param>
        /// <param name="ignoreCase">true for case-insensitive search; otherwise, false.</param>
        /// <returns>
        /// A <see cref="Type" /> object representing the specified type, if the type is declared
        /// in this module; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="className" /> is null.</exception>
        /// <exception cref="TargetInvocationException">
        /// The class initializers are invoked and an exception is thrown.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="className" /> is a zero-length string.
        /// </exception>
        /// <exception cref="TypeLoadException">
        /// <paramref name="throwOnError" /> is true, and the type cannot be found.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="className" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="className" /> requires a dependent assembly that was found but could not be
        /// loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="className" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="className" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual System.Type GetType(string className, bool throwOnError, bool ignoreCase) { return default(System.Type); }
        /// <summary>
        /// Returns the name of the module.
        /// </summary>
        /// <returns>
        /// A String representing the name of this module.
        /// </returns>
        public override string ToString() { return default(string); }
        /// <summary>
        /// A TypeFilter object that filters the list of types defined in this module based upon the name.
        /// This field is case-sensitive and read-only.
        /// </summary>
        public static readonly TypeFilter FilterTypeName;
        /// <summary>
        /// A TypeFilter object that filters the list of types defined in this module based upon the name.
        /// This field is case-insensitive and read-only.
        /// </summary>
        public static readonly TypeFilter FilterTypeNameIgnoreCase;
        /// <summary>
        /// Gets a universally unique identifier (UUID) that can be used to distinguish between two versions
        /// of a module.
        /// </summary>
        /// <returns>
        /// A <see cref="Guid" /> that can be used to distinguish between two versions of a module.
        /// </returns>
        public virtual Guid ModuleVersionId { get { return default(Guid); } }
        /// <summary>
        /// Gets a string representing the name of the module.
        /// </summary>
        /// <returns>
        /// The module name.
        /// </returns>
        public virtual string ScopeName { get { return default(string); } }
        /// <summary>
        /// Returns an array of classes accepted by the given filter and filter criteria.
        /// </summary>
        /// <param name="filter">The delegate used to filter the classes.</param>
        /// <param name="filterCriteria">An Object used to filter the classes.</param>
        /// <returns>
        /// An array of type Type containing classes that were accepted by the filter.
        /// </returns>
        /// <exception cref="ReflectionTypeLoadException">
        /// One or more classes in a module could not be loaded.
        /// </exception>
        public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria) { return default(Type[]); }
        /// <summary>
        /// Returns a field having the specified name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>
        /// A FieldInfo object having the specified name, or null if the field does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> parameter is null.</exception>
        public FieldInfo GetField(string name) { return default(FieldInfo); }
        /// <summary>
        /// Returns a field having the specified name and binding attributes.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="bindingAttr">One of the BindingFlags bit flags used to control the search.</param>
        /// <returns>
        /// A FieldInfo object having the specified name and binding attributes, or null if the field does
        /// not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> parameter is null.</exception>
        public virtual FieldInfo GetField(string name, BindingFlags bindingAttr) { return default(FieldInfo); }
        /// <summary>
        /// Returns the global fields defined on the module.
        /// </summary>
        /// <returns>
        /// An array of <see cref="FieldInfo" /> objects representing the global fields
        /// defined on the module; if there are no global fields, an empty array is returned.
        /// </returns>
        public FieldInfo[] GetFields() { return default(FieldInfo[]); }
        /// <summary>
        /// Returns the global fields defined on the module that match the specified binding flags.
        /// </summary>
        /// <param name="bindingFlags">
        /// A bitwise combination of <see cref="BindingFlags" /> values that limit
        /// the search.
        /// </param>
        /// <returns>
        /// An array of type <see cref="FieldInfo" /> representing the global fields
        /// defined on the module that match the specified binding flags; if no global fields match the
        /// binding flags, an empty array is returned.
        /// </returns>
        public virtual FieldInfo[] GetFields(BindingFlags bindingFlags) { return default(FieldInfo[]); }
        /// <summary>
        /// Returns a method having the specified name.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <returns>
        /// A MethodInfo object having the specified name, or null if the method does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public MethodInfo GetMethod(string name) { return default(MethodInfo); }
        /// <summary>
        /// Returns a method having the specified name and parameter types.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="types">The parameter types to search for.</param>
        /// <returns>
        /// A MethodInfo object in accordance with the specified criteria, or null if the method does not
        /// exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null, <paramref name="types" /> is null, or <paramref name="types" />
        /// (i) is null.
        /// </exception>
        public MethodInfo GetMethod(string name, Type[] types) { return default(MethodInfo); }
        /// <summary>
        /// Returns the global methods defined on the module.
        /// </summary>
        /// <returns>
        /// An array of <see cref="MethodInfo" /> objects representing all the global
        /// methods defined on the module; if there are no global methods, an empty array is returned.
        /// </returns>
        public MethodInfo[] GetMethods() { return default(MethodInfo[]); }
        /// <summary>
        /// Returns the global methods defined on the module that match the specified binding flags.
        /// </summary>
        /// <param name="bindingFlags">
        /// A bitwise combination of <see cref="BindingFlags" /> values that limit
        /// the search.
        /// </param>
        /// <returns>
        /// An array of type <see cref="MethodInfo" /> representing the global methods
        /// defined on the module that match the specified binding flags; if no global methods match the
        /// binding flags, an empty array is returned.
        /// </returns>
        public virtual MethodInfo[] GetMethods(BindingFlags bindingFlags) { return default(MethodInfo[]); }
        /// <summary>
        /// Returns the specified type, performing a case-sensitive search.
        /// </summary>
        /// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
        /// <returns>
        /// A Type object representing the given type, if the type is in this module; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="className" /> is null.</exception>
        /// <exception cref="TargetInvocationException">
        /// The class initializers are invoked and an exception is thrown.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="className" /> is a zero-length string.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="className" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="className" /> requires a dependent assembly that was found but could not be
        /// loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="className" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="className" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual Type GetType(string className) { return default(Type); }
        /// <summary>
        /// Returns the specified type, searching the module with the specified case sensitivity.
        /// </summary>
        /// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
        /// <param name="ignoreCase">true for case-insensitive search; otherwise, false.</param>
        /// <returns>
        /// A Type object representing the given type, if the type is in this module; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="className" /> is null.</exception>
        /// <exception cref="TargetInvocationException">
        /// The class initializers are invoked and an exception is thrown.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="className" /> is a zero-length string.
        /// </exception>
        /// <exception cref="IO.FileNotFoundException">
        /// <paramref name="className" /> requires a dependent assembly that could not be found.
        /// </exception>
        /// <exception cref="IO.FileLoadException">
        /// <paramref name="className" /> requires a dependent assembly that was found but could not be
        /// loaded.-or-The current assembly was loaded into the reflection-only context, and <paramref name="className" />
        /// requires a dependent assembly that was not preloaded.
        /// </exception>
        /// <exception cref="BadImageFormatException">
        /// <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.
        /// -or-<paramref name="className" /> requires a dependent assembly which was compiled for a version
        /// of the runtime later than the currently loaded version.
        /// </exception>
        public virtual Type GetType(string className, bool ignoreCase) { return default(Type); }
        /// <summary>
        /// Returns all the types defined within this module.
        /// </summary>
        /// <returns>
        /// An array of type Type containing types defined within the module that is reflected by this
        /// instance.
        /// </returns>
        /// <exception cref="ReflectionTypeLoadException">
        /// One or more classes in a module could not be loaded.
        /// </exception>
        /// <exception cref="Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual Type[] GetTypes() { return default(Type[]); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit) { return default(object[]); }
        bool System.Reflection.ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit) { return default(bool); }
    }
    /// <summary>
    /// Discovers the attributes of a parameter and provides access to parameter metadata.
    /// </summary>
    public partial class ParameterInfo : System.Reflection.ICustomAttributeProvider
    {
        internal ParameterInfo() { }
        /// <summary>
        /// Gets the attributes for this parameter.
        /// </summary>
        /// <returns>
        /// A ParameterAttributes object representing the attributes for this parameter.
        /// </returns>
        public virtual System.Reflection.ParameterAttributes Attributes { get { return default(System.Reflection.ParameterAttributes); } }
        /// <summary>
        /// Gets a collection that contains this parameter's custom attributes.
        /// </summary>
        /// <returns>
        /// A collection that contains this parameter's custom attributes.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData> CustomAttributes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.CustomAttributeData>); } }
        /// <summary>
        /// Gets a value indicating the default value if the parameter has a default value.
        /// </summary>
        /// <returns>
        /// The default value of the parameter, or <see cref="DBNull.Value" /> if the parameter
        /// has no default value.
        /// </returns>
        public virtual object DefaultValue { get { return default(object); } }
        /// <summary>
        /// Gets a value that indicates whether this parameter has a default value.
        /// </summary>
        /// <returns>
        /// true if this parameter has a default value; otherwise, false.
        /// </returns>
        public virtual bool HasDefaultValue { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this is an input parameter.
        /// </summary>
        /// <returns>
        /// true if the parameter is an input parameter; otherwise, false.
        /// </returns>
        public bool IsIn { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        /// <returns>
        /// true if the parameter is optional; otherwise, false.
        /// </returns>
        public bool IsOptional { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this is an output parameter.
        /// </summary>
        /// <returns>
        /// true if the parameter is an output parameter; otherwise, false.
        /// </returns>
        public bool IsOut { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether this is a Retval parameter.
        /// </summary>
        /// <returns>
        /// true if the parameter is a Retval; otherwise, false.
        /// </returns>
        public bool IsRetval { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating the member in which the parameter is implemented.
        /// </summary>
        /// <returns>
        /// The member which implanted the parameter represented by this <see cref="ParameterInfo" />.
        /// </returns>
        public virtual System.Reflection.MemberInfo Member { get { return default(System.Reflection.MemberInfo); } }
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <returns>
        /// The simple name of this parameter.
        /// </returns>
        public virtual string Name { get { return default(string); } }
        /// <summary>
        /// Gets the Type of this parameter.
        /// </summary>
        /// <returns>
        /// The Type object that represents the Type of this parameter.
        /// </returns>
        public virtual System.Type ParameterType { get { return default(System.Type); } }
        /// <summary>
        /// Gets the zero-based position of the parameter in the formal parameter list.
        /// </summary>
        /// <returns>
        /// An integer representing the position this parameter occupies in the parameter list.
        /// </returns>
        public virtual int Position { get { return default(int); } }
        /// <summary>
        /// Gets the optional custom modifiers of the parameter.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the optional custom modifiers
        /// of the current parameter, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsImplicitlyDereferenced" />.
        /// </returns>
        public virtual Type[] GetOptionalCustomModifiers() { return default(Type[]); }
        /// <summary>
        /// Gets the required custom modifiers of the parameter.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the required custom modifiers
        /// of the current parameter, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsImplicitlyDereferenced" />.
        /// </returns>
        public virtual Type[] GetRequiredCustomModifiers() { return default(Type[]); }
        /// <summary>
        /// Gets a value indicating the default value if the parameter has a default value.
        /// </summary>
        /// <returns>
        /// The default value of the parameter, or <see cref="DBNull.Value" /> if the parameter
        /// has no default value.
        /// </returns>
        public virtual object RawDefaultValue { get { return default(object); } }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(bool inherit) { return default(object[]); }
        object[] System.Reflection.ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit) { return default(object[]); }
        bool System.Reflection.ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit) { return default(bool); }
    }
    /// <summary>
    /// Attaches a modifier to parameters so that binding can work with parameter signatures in which
    /// the types have been modified.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ParameterModifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModifier" /> structure
        /// representing the specified number of parameters.
        /// </summary>
        /// <param name="parameterCount">The number of parameters.</param>
        /// <exception cref="ArgumentException"><paramref name="parameterCount" /> is negative.</exception>
        public ParameterModifier(int parameterCount) { }
        /// <summary>
        /// Gets or sets a value that specifies whether the parameter at the specified index position
        /// is to be modified by the current <see cref="ParameterModifier" />.
        /// </summary>
        /// <param name="index">The index position of the parameter whose modification status is being examined or set.</param>
        /// <returns>
        /// true if the parameter at this index position is to be modified by this
        /// <see cref="ParameterModifier" />; otherwise, false.
        /// </returns>
        public bool this[int index] { get { return default(bool); } set { } }
    }
    /// <summary>
    /// Discovers the attributes of a property and provides access to property metadata.
    /// </summary>
    public abstract partial class PropertyInfo : System.Reflection.MemberInfo
    {
        internal PropertyInfo() { }
        /// <summary>
        /// Gets the attributes for this property.
        /// </summary>
        /// <returns>
        /// The attributes of this property.
        /// </returns>
        public abstract System.Reflection.PropertyAttributes Attributes { get; }
        /// <summary>
        /// Gets a value indicating whether the property can be read.
        /// </summary>
        /// <returns>
        /// true if this property can be read; otherwise, false.
        /// </returns>
        public abstract bool CanRead { get; }
        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <returns>
        /// true if this property can be written to; otherwise, false.
        /// </returns>
        public abstract bool CanWrite { get; }
        /// <summary>
        /// Gets the get accessor for this property.
        /// </summary>
        /// <returns>
        /// The get accessor for this property.
        /// </returns>
        public virtual System.Reflection.MethodInfo GetMethod { get { return default(System.Reflection.MethodInfo); } }
        /// <summary>
        /// Gets a value indicating whether the property is the special name.
        /// </summary>
        /// <returns>
        /// true if this property is the special name; otherwise, false.
        /// </returns>
        public bool IsSpecialName { get { return default(bool); } }
        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        /// <returns>
        /// The type of this property.
        /// </returns>
        public abstract System.Type PropertyType { get; }
        /// <summary>
        /// Gets the set accessor for this property.
        /// </summary>
        /// <returns>
        /// The set accessor for this property, or null if the property is read-only.
        /// </returns>
        public virtual System.Reflection.MethodInfo SetMethod { get { return default(System.Reflection.MethodInfo); } }
        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance, or null.</param>
        /// <returns>
        /// true if <paramref name="obj" /> equals the type and value of this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns a literal value associated with the property by a compiler.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> that contains the literal value associated with the property.
        /// If the literal value is a class type with an element value of zero, the return value is null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Constant table in unmanaged metadata does not contain a constant value for the current
        /// property.
        /// </exception>
        /// <exception cref="FormatException">
        /// The type of the value is not one of the types permitted by the Common Language Specification
        /// (CLS). See the ECMA Partition II specification, Metadata.
        /// </exception>
        public virtual object GetConstantValue() { return default(object); }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// When overridden in a derived class, returns an array of all the index parameters for the property.
        /// </summary>
        /// <returns>
        /// An array of type ParameterInfo containing the parameters for the indexes. If the property is
        /// not indexed, the array has 0 (zero) elements.
        /// </returns>
        public abstract System.Reflection.ParameterInfo[] GetIndexParameters();
        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be returned.</param>
        /// <returns>
        /// The property value of the specified object.
        /// </returns>
        public object GetValue(object obj) { return default(object); }
        /// <summary>
        /// Returns the property value of a specified object with optional index values for indexed properties.
        /// </summary>
        /// <param name="obj">The object whose property value will be returned.</param>
        /// <param name="index">
        /// Optional index values for indexed properties. The indexes of indexed properties are zero-based.
        /// This value should be null for non-indexed properties.
        /// </param>
        /// <returns>
        /// The property value of the specified object.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="index" /> array does not contain the type of arguments needed.-or- The
        /// property's get accessor is not found.
        /// </exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The object does not match the target type, or a property is an instance property
        /// but <paramref name="obj" /> is null.
        /// </exception>
        /// <exception cref="TargetParameterCountException">
        /// The number of parameters in <paramref name="index" /> does not match the number of parameters
        /// the indexed property takes.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.There was an illegal attempt to access
        /// a private or protected method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// An error occurred while retrieving the property value. For example, an index value specified
        /// for an indexed property is out of range. The <see cref="Exception.InnerException" />
        /// property indicates the reason for the error.
        /// </exception>
        public virtual object GetValue(object obj, object[] index) { return default(object); }
        /// <summary>
        /// Sets the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        /// <exception cref="ArgumentException">
        /// The property's set accessor is not found. -or-<paramref name="value" /> cannot be converted
        /// to the type of <see cref="PropertyType" />.
        /// </exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The type of <paramref name="obj" /> does not match the target type, or a property
        /// is an instance property but <paramref name="obj" /> is null.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.There was an illegal attempt to access
        /// a private or protected method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// An error occurred while setting the property value. The <see cref="Exception.InnerException" />
        /// property indicates the reason for the error.
        /// </exception>
        public void SetValue(object obj, object value) { }
        /// <summary>
        /// Sets the property value of a specified object with optional index values for index properties.
        /// </summary>
        /// <param name="obj">The object whose property value will be set.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="index">
        /// Optional index values for indexed properties. This value should be null for non-indexed properties.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The <paramref name="index" /> array does not contain the type of arguments needed.-or- The
        /// property's set accessor is not found. -or-<paramref name="value" /> cannot be converted to
        /// the type of <see cref="PropertyType" />.
        /// </exception>
        /// <exception cref="TargetException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch <see cref="Exception" />
        /// instead.The object does not match the target type, or a property is an instance property
        /// but <paramref name="obj" /> is null.
        /// </exception>
        /// <exception cref="TargetParameterCountException">
        /// The number of parameters in <paramref name="index" /> does not match the number of parameters
        /// the indexed property takes.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception,
        /// <see cref="MemberAccessException" />, instead.There was an illegal attempt to access
        /// a private or protected method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// An error occurred while setting the property value. For example, an index value specified
        /// for an indexed property is out of range. The <see cref="Exception.InnerException" />
        /// property indicates the reason for the error.
        /// </exception>
        public virtual void SetValue(object obj, object value, object[] index) { }
        /// <summary>
        /// Gets a <see cref="MemberTypes" /> value indicating that this member is
        /// a property.
        /// </summary>
        /// <returns>
        /// A <see cref="MemberTypes" /> value indicating that this member is a property.
        /// </returns>
        public override MemberTypes MemberType { get { return default(MemberTypes); } }
        /// <summary>
        /// Returns an array whose elements reflect the public get and set accessors of the property reflected
        /// by the current instance.
        /// </summary>
        /// <returns>
        /// An array of <see cref="MethodInfo" /> objects that reflect the public
        /// get and set accessors of the property reflected by the current instance, if found; otherwise,
        /// this method returns an array with zero (0) elements.
        /// </returns>
        public MethodInfo[] GetAccessors() { return default(MethodInfo[]); }
        /// <summary>
        /// Returns an array whose elements reflect the public and, if specified, non-public get and set
        /// accessors of the property reflected by the current instance.
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether non-public methods should be returned in the returned array. true if non-public
        /// methods are to be included; otherwise, false.
        /// </param>
        /// <returns>
        /// An array whose elements reflect the get and set accessors of the property reflected by the
        /// current instance. If <paramref name="nonPublic" /> is true, this array contains public and
        /// non-public get and set accessors. If <paramref name="nonPublic" /> is false, this array contains
        /// only public get and set accessors. If no accessors with the specified visibility are found,
        /// this method returns an array with zero (0) elements.
        /// </returns>
        public abstract MethodInfo[] GetAccessors(bool nonPublic);
        /// <summary>
        /// Returns the public get accessor for this property.
        /// </summary>
        /// <returns>
        /// A MethodInfo object representing the public get accessor for this property, or null if the
        /// get accessor is non-public or does not exist.
        /// </returns>
        public MethodInfo GetGetMethod() { return default(MethodInfo); }
        /// <summary>
        /// When overridden in a derived class, returns the public or non-public get accessor for this
        /// property.
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether a non-public get accessor should be returned. true if a non-public accessor
        /// is to be returned; otherwise, false.
        /// </param>
        /// <returns>
        /// A MethodInfo object representing the get accessor for this property, if <paramref name="nonPublic" />
        /// is true. Returns null if <paramref name="nonPublic" /> is false and the get accessor is
        /// non-public, or if <paramref name="nonPublic" /> is true but no get accessors exist.
        /// </returns>
        /// <exception cref="Security.SecurityException">
        /// The requested method is non-public and the caller does not have
        /// <see cref="Security.Permissions.ReflectionPermission" /> to reflect on this non-public method.
        /// </exception>
        public abstract MethodInfo GetGetMethod(bool nonPublic);
        /// <summary>
        /// Returns the public set accessor for this property.
        /// </summary>
        /// <returns>
        /// The MethodInfo object representing the Set method for this property if the set accessor is
        /// public, or null if the set accessor is not public.
        /// </returns>
        public MethodInfo GetSetMethod() { return default(MethodInfo); }
        /// <summary>
        /// When overridden in a derived class, returns the set accessor for this property.
        /// </summary>
        /// <param name="nonPublic">
        /// Indicates whether the accessor should be returned if it is non-public. true if a non-public
        /// accessor is to be returned; otherwise, false.
        /// </param>
        /// <returns>
        /// This property's Set method, or null, as shown in the following table.Value Condition The Set
        /// method for this property. The set accessor is public.-or- <paramref name="nonPublic" /> is
        /// true and the set accessor is non-public. null<paramref name="nonPublic" /> is true, but the
        /// property is read-only.-or- <paramref name="nonPublic" /> is false and the set accessor is
        /// non-public.-or- There is no set accessor.
        /// </returns>
        /// <exception cref="Security.SecurityException">
        /// The requested method is non-public and the caller does not have
        /// <see cref="Security.Permissions.ReflectionPermission" /> to reflect on this non-public method.
        /// </exception>
        public abstract MethodInfo GetSetMethod(bool nonPublic);
        /// <summary>
        /// Returns an array of types representing the optional custom modifiers of the property.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the optional custom modifiers
        /// of the current property, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsImplicitlyDereferenced" />.
        /// </returns>
        public virtual Type[] GetOptionalCustomModifiers() { return default(Type[]); }
        /// <summary>
        /// Returns a literal value associated with the property by a compiler.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> that contains the literal value associated with the property.
        /// If the literal value is a class type with an element value of zero, the return value is null.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Constant table in unmanaged metadata does not contain a constant value for the current
        /// property.
        /// </exception>
        /// <exception cref="FormatException">
        /// The type of the value is not one of the types permitted by the Common Language Specification
        /// (CLS). See the ECMA Partition II specification, Metadata Logical Format: Other Structures, Element
        /// Types used in Signatures.
        /// </exception>
        public virtual object GetRawConstantValue() { return default(object); }
        /// <summary>
        /// Returns an array of types representing the required custom modifiers of the property.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Type" /> objects that identify the required custom modifiers
        /// of the current property, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsImplicitlyDereferenced" />.
        /// </returns>
        public virtual Type[] GetRequiredCustomModifiers() { return default(Type[]); }
    }
    /// <summary>
    /// Represents a context that can provide reflection objects.
    /// </summary>
    public abstract partial class ReflectionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionContext" /> class.
        /// </summary>
        protected ReflectionContext() { }
        /// <summary>
        /// Gets the representation of the type of the specified object in this reflection context.
        /// </summary>
        /// <param name="value">The object to represent.</param>
        /// <returns>
        /// An object that represents the type of the specified object.
        /// </returns>
        public virtual System.Reflection.TypeInfo GetTypeForObject(object value) { return default(System.Reflection.TypeInfo); }
        /// <summary>
        /// Gets the representation, in this reflection context, of an assembly that is represented by
        /// an object from another reflection context.
        /// </summary>
        /// <param name="assembly">The external representation of the assembly to represent in this context.</param>
        /// <returns>
        /// The representation of the assembly in this reflection context.
        /// </returns>
        public abstract System.Reflection.Assembly MapAssembly(System.Reflection.Assembly assembly);
        /// <summary>
        /// Gets the representation, in this reflection context, of a type represented by an object from
        /// another reflection context.
        /// </summary>
        /// <param name="type">The external representation of the type to represent in this context.</param>
        /// <returns>
        /// The representation of the type in this reflection context..
        /// </returns>
        public abstract System.Reflection.TypeInfo MapType(System.Reflection.TypeInfo type);
    }
    /// <summary>
    /// The exception that is thrown by the <see cref="Module.GetTypes" /> method
    /// if any of the classes in a module cannot be loaded. This class cannot be inherited.
    /// </summary>
    public sealed partial class ReflectionTypeLoadException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionTypeLoadException" />
        /// class with the given classes and their associated exceptions.
        /// </summary>
        /// <param name="classes">
        /// An array of type Type containing the classes that were defined in the module and loaded. This
        /// array can contain null reference (Nothing in Visual Basic) values.
        /// </param>
        /// <param name="exceptions">
        /// An array of type Exception containing the exceptions that were thrown by the class loader.
        /// The null reference (Nothing in Visual Basic) values in the <paramref name="classes" /> array
        /// line up with the exceptions in this <paramref name="exceptions" /> array.
        /// </param>
        public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionTypeLoadException" />
        /// class with the given classes, their associated exceptions, and exception descriptions.
        /// </summary>
        /// <param name="classes">
        /// An array of type Type containing the classes that were defined in the module and loaded. This
        /// array can contain null reference (Nothing in Visual Basic) values.
        /// </param>
        /// <param name="exceptions">
        /// An array of type Exception containing the exceptions that were thrown by the class loader.
        /// The null reference (Nothing in Visual Basic) values in the <paramref name="classes" /> array
        /// line up with the exceptions in this <paramref name="exceptions" /> array.
        /// </param>
        /// <param name="message">A String describing the reason the exception was thrown.</param>
        public ReflectionTypeLoadException(System.Type[] classes, System.Exception[] exceptions, string message) { }
        /// <summary>
        /// Gets the array of exceptions thrown by the class loader.
        /// </summary>
        /// <returns>
        /// An array of type Exception containing the exceptions thrown by the class loader. The null
        /// values in the <see cref="Types" /> array of
        /// this instance line up with the exceptions in this array.
        /// </returns>
        public System.Exception[] LoaderExceptions { get { return default(System.Exception[]); } }
        /// <summary>
        /// Gets the array of classes that were defined in the module and loaded.
        /// </summary>
        /// <returns>
        /// An array of type Type containing the classes that were defined in the module and loaded. This
        /// array can contain some null values.
        /// </returns>
        public System.Type[] Types { get { return default(System.Type[]); } }
    }
    /// <summary>
    /// Specifies the resource location.
    /// </summary>
    [System.FlagsAttribute]
    public enum ResourceLocation
    {
        /// <summary>
        /// Specifies that the resource is contained in another assembly.
        /// </summary>
        ContainedInAnotherAssembly = 2,
        /// <summary>
        /// Specifies that the resource is contained in the manifest file.
        /// </summary>
        ContainedInManifestFile = 4,
        /// <summary>
        /// Specifies an embedded (that is, non-linked) resource.
        /// </summary>
        Embedded = 1,
    }
    /// <summary>
    /// Represents the exception that is thrown when an attempt is made to invoke an invalid target.
    /// </summary>
    public partial class TargetException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetException" /> class
        /// with an empty message and the root cause of the exception.
        /// </summary>
        public TargetException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetException" /> class
        /// with the given message and the root cause exception.
        /// </summary>
        /// <param name="message">A String describing the reason why the exception occurred.</param>
        public TargetException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetException" /> class
        /// with a specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public TargetException(string message, Exception inner) { }
    }
    /// <summary>
    /// The exception that is thrown by methods invoked through reflection. This class cannot be inherited.
    /// </summary>
    public sealed partial class TargetInvocationException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInvocationException" />
        /// class with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public TargetInvocationException(System.Exception inner) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInvocationException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public TargetInvocationException(string message, System.Exception inner) { }
    }
    /// <summary>
    /// The exception that is thrown when the number of parameters for an invocation does not match
    /// the number expected. This class cannot be inherited.
    /// </summary>
    public sealed partial class TargetParameterCountException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterCountException" />
        /// class with an empty message string and the root cause of the exception.
        /// </summary>
        public TargetParameterCountException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterCountException" />
        /// class with its message string set to the given message and the root cause exception.
        /// </summary>
        /// <param name="message">A String describing the reason this exception was thrown.</param>
        public TargetParameterCountException(string message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetParameterCountException" />
        /// class with a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception. If the <paramref name="inner" />
        /// parameter is not null, the current exception is raised in a catch block that handles the inner
        /// exception.
        /// </param>
        public TargetParameterCountException(string message, System.Exception inner) { }
    }
    /// <summary>
    /// Filters the classes represented in an array of <see cref="Type" /> objects.
    /// </summary>
    /// <param name="m">The Type object to which the filter is applied.</param>
    /// <param name="filterCriteria">An arbitrary object used to filter the list.</param>
    /// <returns>
    /// true to include the <see cref="Type" /> in the filtered list; otherwise false.
    /// </returns>
    public delegate bool TypeFilter(Type m, Object filterCriteria);
    /// <summary>
    /// Represents type declarations for class types, interface types, array types, value types, enumeration
    /// types, type parameters, generic type definitions, and open or closed constructed generic types.
    /// </summary>
    public abstract partial class TypeInfo : System.Reflection.MemberInfo, System.Reflection.IReflectableType
    {
        internal TypeInfo() { }
        /// <summary>
        /// Returns the current type as a <see cref="Type" /> object.
        /// </summary>
        /// <returns>
        /// The current type.
        /// </returns>
        public virtual System.Type AsType() { return default(System.Type); }
        /// <summary>
        /// Gets a collection of the constructors declared by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the constructors declared by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo> DeclaredConstructors { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo>); } }
        /// <summary>
        /// Gets a collection of the events defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the events defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.EventInfo> DeclaredEvents { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.EventInfo>); } }
        /// <summary>
        /// Gets a collection of the fields defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the fields defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo> DeclaredFields { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.FieldInfo>); } }
        /// <summary>
        /// Gets a collection of the members defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the members defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo> DeclaredMembers { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MemberInfo>); } }
        /// <summary>
        /// Gets a collection of the methods defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the methods defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> DeclaredMethods { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); } }
        /// <summary>
        /// Gets a collection of the nested types defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of nested types defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo> DeclaredNestedTypes { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.TypeInfo>); } }
        /// <summary>
        /// Gets a collection of the properties defined by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the properties defined by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo> DeclaredProperties { get { return default(System.Collections.Generic.IEnumerable<System.Reflection.PropertyInfo>); } }
        public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria) { return default(Type[]); }
        public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria) { return default(MemberInfo[]); }
        /// <summary>
        /// Gets an array of the generic type parameters of the current instance.
        /// </summary>
        /// <returns>
        /// An array that contains the current instance's generic type parameters, or an array of
        /// <see cref="Array.Length" /> zero if the current instance has no generic type parameters.
        /// </returns>
        public virtual System.Type[] GenericTypeParameters { get { return default(System.Type[]); } }
        public ConstructorInfo GetConstructor(Type[] types) { return default(ConstructorInfo); }
        public ConstructorInfo[] GetConstructors() { return default(ConstructorInfo[]); }
        public virtual ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) { return default(ConstructorInfo[]); }
        /// <summary>
        /// Returns an object that represents the specified public event declared by the current type.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <returns>
        /// An object that represents the specified event, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Reflection.EventInfo GetDeclaredEvent(string name) { return default(System.Reflection.EventInfo); }
        /// <summary>
        /// Returns an object that represents the specified public field declared by the current type.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>
        /// An object that represents the specified field, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Reflection.FieldInfo GetDeclaredField(string name) { return default(System.Reflection.FieldInfo); }
        /// <summary>
        /// Returns an object that represents the specified public method declared by the current type.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <returns>
        /// An object that represents the specified method, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Reflection.MethodInfo GetDeclaredMethod(string name) { return default(System.Reflection.MethodInfo); }
        /// <summary>
        /// Returns a collection that contains all public methods declared on the current type that match
        /// the specified name.
        /// </summary>
        /// <param name="name">The method name to search for.</param>
        /// <returns>
        /// A collection that contains methods that match <paramref name="name" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo> GetDeclaredMethods(string name) { return default(System.Collections.Generic.IEnumerable<System.Reflection.MethodInfo>); }
        /// <summary>
        /// Returns an object that represents the specified public nested type declared by the current
        /// type.
        /// </summary>
        /// <param name="name">The name of the nested type.</param>
        /// <returns>
        /// An object that represents the specified nested type, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Reflection.TypeInfo GetDeclaredNestedType(string name) { return default(System.Reflection.TypeInfo); }
        /// <summary>
        /// Returns an object that represents the specified public property declared by the current type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// An object that represents the specified property, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        public virtual System.Reflection.PropertyInfo GetDeclaredProperty(string name) { return default(System.Reflection.PropertyInfo); }
        public virtual MemberInfo[] GetDefaultMembers() { return default(MemberInfo[]); }
        public virtual string GetEnumName(object value) { return default(string); }
        public virtual string[] GetEnumNames() { return default(string[]); }
        public virtual Type GetEnumUnderlyingType() { return default(Type); }
        public virtual Array GetEnumValues() { return default(Array); }
        public EventInfo GetEvent(string name) { return default(EventInfo); }
        public virtual EventInfo GetEvent(string name, BindingFlags bindingAttr) { return default(EventInfo); }
        public virtual EventInfo[] GetEvents() { return default(EventInfo[]); }
        public virtual EventInfo[] GetEvents(BindingFlags bindingAttr) { return default(EventInfo[]); }
        public FieldInfo GetField(string name) { return default(FieldInfo); }
        public virtual FieldInfo GetField(string name, BindingFlags bindingAttr) { return default(FieldInfo); }
        public FieldInfo[] GetFields() { return default(FieldInfo[]); }
        public virtual FieldInfo[] GetFields(BindingFlags bindingAttr) { return default(FieldInfo[]); }
        public virtual Type[] GetGenericArguments() { return default(Type[]); }
        public Type GetInterface(string name) { return default(Type); }
        public virtual Type GetInterface(string name, bool ignoreCase) { return default(Type); }
        public virtual Type[] GetInterfaces() { return default(Type[]); }
        public MemberInfo[] GetMember(string name) { return default(MemberInfo[]); }
        public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr) { return default(MemberInfo[]); }
        public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr) { return default(MemberInfo[]); }
        public MemberInfo[] GetMembers() { return default(MemberInfo[]); }
        public virtual MemberInfo[] GetMembers(BindingFlags bindingAttr) { return default(MemberInfo[]); }
        public MethodInfo GetMethod(string name) { return default(MethodInfo); }
        public MethodInfo GetMethod(string name, BindingFlags bindingAttr) { return default(MethodInfo); }
        public MethodInfo GetMethod(string name, Type[] types) { return default(MethodInfo); }
        public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers) { return default(MethodInfo); }
        public MethodInfo[] GetMethods() { return default(MethodInfo[]); }
        public virtual MethodInfo[] GetMethods(BindingFlags bindingAttr) { return default(MethodInfo[]); }
        public Type GetNestedType(string name) { return default(Type); }
        public virtual Type GetNestedType(string name, BindingFlags bindingAttr) { return default(Type); }
        public Type[] GetNestedTypes() { return default(Type[]); }
        public virtual Type[] GetNestedTypes(BindingFlags bindingAttr) { return default(Type[]); }
        public PropertyInfo[] GetProperties() { return default(PropertyInfo[]); }
        public virtual PropertyInfo[] GetProperties(BindingFlags bindingAttr) { return default(PropertyInfo[]); }
        public PropertyInfo GetProperty(string name) { return default(PropertyInfo); }
        public PropertyInfo GetProperty(string name, BindingFlags bindingAttr) { return default(PropertyInfo); }
        public PropertyInfo GetProperty(string name, Type returnType) { return default(PropertyInfo); }
        public PropertyInfo GetProperty(string name, Type returnType, Type[] types) { return default(PropertyInfo); }
        public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers) { return default(PropertyInfo); }
        public PropertyInfo GetProperty(string name, Type[] types) { return default(PropertyInfo); }
        System.Reflection.TypeInfo System.Reflection.IReflectableType.GetTypeInfo() { return default(System.Reflection.TypeInfo); }
        /// <summary>
        /// Gets a collection of the interfaces implemented by the current type.
        /// </summary>
        /// <returns>
        /// A collection of the interfaces implemented by the current type.
        /// </returns>
        public virtual System.Collections.Generic.IEnumerable<System.Type> ImplementedInterfaces { get { return default(System.Collections.Generic.IEnumerable<System.Type>); } }
        public virtual bool IsAssignableFrom(Type c) { return default(bool); }
        /// <summary>
        /// Returns a value that indicates whether the specified type can be assigned to the current type.
        /// </summary>
        /// <param name="typeInfo">The type to check.</param>
        /// <returns>
        /// true if the specified type can be assigned to this type; otherwise, false.
        /// </returns>
        public virtual bool IsAssignableFrom(System.Reflection.TypeInfo typeInfo) { return default(bool); }
        public virtual bool IsEnumDefined(object value) { return default(bool); }
        public virtual bool IsInstanceOfType(object o) { return default(bool); }
        public virtual System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute { get { return default(System.Runtime.InteropServices.StructLayoutAttribute); } }
        public ConstructorInfo TypeInitializer { get { return default(ConstructorInfo); } }
        public virtual Type UnderlyingSystemType { get { return default(Type); } }
        public override System.Reflection.MemberTypes MemberType { get { return default(System.Reflection.MemberTypes); } }
    }
}
