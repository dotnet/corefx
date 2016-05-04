// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection.Emit
{
    /// <summary>
    /// Defines and represents a dynamic method that can be compiled, executed, and discarded. Discarded
    /// methods are available for garbage collection.
    /// </summary>
    public sealed partial class DynamicMethod : System.Reflection.MethodInfo
    {
        /// <summary>
        /// Creates a dynamic method that is global to a module, specifying the method name, attributes,
        /// calling convention, return type, parameter types, module, and whether just-in-time (JIT) visibility
        /// checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL)
        /// of the dynamic method.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="attributes">
        /// A bitwise combination of <see cref="MethodAttributes" /> values that specifies
        /// the attributes of the dynamic method. The only combination allowed is
        /// <see cref="MethodAttributes.Public" /> and <see cref="MethodAttributes.Static" />.
        /// </param>
        /// <param name="callingConvention">
        /// The calling convention for the dynamic method. Must be
        /// <see cref="CallingConventions.Standard" />.
        /// </param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="m">
        /// A <see cref="Module" /> representing the module with which the dynamic
        /// method is to be logically associated.
        /// </param>
        /// <param name="skipVisibility">
        /// true to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic
        /// method; otherwise, false.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.-or-
        /// <paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="m" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="attributes" /> is a combination of flags other than
        /// <see cref="MethodAttributes.Public" /> and <see cref="MethodAttributes.Static" />.-or-
        /// <paramref name="callingConvention" /> is not <see cref="CallingConventions.Standard" />.-or-
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Reflection.MethodAttributes attributes, System.Reflection.CallingConventions callingConvention, System.Type returnType, System.Type[] parameterTypes, System.Reflection.Module m, bool skipVisibility) { }
        /// <summary>
        /// Creates a dynamic method, specifying the method name, attributes, calling convention, return
        /// type, parameter types, the type with which the dynamic method is logically associated, and whether
        /// just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft
        /// intermediate language (MSIL) of the dynamic method.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="attributes">
        /// A bitwise combination of <see cref="MethodAttributes" /> values that specifies
        /// the attributes of the dynamic method. The only combination allowed is
        /// <see cref="MethodAttributes.Public" /> and <see cref="MethodAttributes.Static" />.
        /// </param>
        /// <param name="callingConvention">
        /// The calling convention for the dynamic method. Must be
        /// <see cref="CallingConventions.Standard" />.
        /// </param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="owner">
        /// A <see cref="Type" /> with which the dynamic method is logically associated. The
        /// dynamic method has access to all members of the type.
        /// </param>
        /// <param name="skipVisibility">
        /// true to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic
        /// method; otherwise, false.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.
        /// -or-<paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter
        /// of a generic type or method.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="owner" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="attributes" /> is a combination of flags other than
        /// <see cref="MethodAttributes.Public" /> and <see cref="MethodAttributes.Static" />.-or-
        /// <paramref name="callingConvention" /> is not <see cref="CallingConventions.Standard" />.-or-
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Reflection.MethodAttributes attributes, System.Reflection.CallingConventions callingConvention, System.Type returnType, System.Type[] parameterTypes, System.Type owner, bool skipVisibility) { }
        /// <summary>
        /// Initializes an anonymously hosted dynamic method, specifying the method name, return type,
        /// and parameter types.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns
        /// true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes) { }
        /// <summary>
        /// Initializes an anonymously hosted dynamic method, specifying the method name, return type,
        /// parameter types, and whether just-in-time (JIT) visibility checks should be skipped for types and members
        /// accessed by the Microsoft intermediate language (MSIL) of the dynamic method.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="restrictedSkipVisibility">
        /// true to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic
        /// method, with this restriction: the trust level of the assemblies that contain those types and members
        /// must be equal to or less than the trust level of the call stack that emits the dynamic method; otherwise,
        /// false.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns
        /// true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes, bool restrictedSkipVisibility) { }
        /// <summary>
        /// Creates a dynamic method that is global to a module, specifying the method name, return type,
        /// parameter types, and module.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="m">
        /// A <see cref="Module" /> representing the module with which the dynamic
        /// method is to be logically associated.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.
        /// -or-<paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="m" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns
        /// true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes, System.Reflection.Module m) { }
        /// <summary>
        /// Creates a dynamic method that is global to a module, specifying the method name, return type,
        /// parameter types, module, and whether just-in-time (JIT) visibility checks should be skipped for types
        /// and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="m">
        /// A <see cref="Module" /> representing the module with which the dynamic
        /// method is to be logically associated.
        /// </param>
        /// <param name="skipVisibility">
        /// true to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic
        /// method.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.
        /// -or-<paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="m" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is a type for which <see cref="Type.IsByRef" /> returns
        /// true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes, System.Reflection.Module m, bool skipVisibility) { }
        /// <summary>
        /// Creates a dynamic method, specifying the method name, return type, parameter types, and the
        /// type with which the dynamic method is logically associated.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="owner">
        /// A <see cref="Type" /> with which the dynamic method is logically associated. The
        /// dynamic method has access to all members of the type.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.-or-
        /// <paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter
        /// of a generic type or method.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="owner" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is null, or is a type for which <see cref="Type.IsByRef" />
        /// returns true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes, System.Type owner) { }
        /// <summary>
        /// Creates a dynamic method, specifying the method name, return type, parameter types, the type
        /// with which the dynamic method is logically associated, and whether just-in-time (JIT) visibility checks
        /// should be skipped for types and members accessed by the Microsoft intermediate language (MSIL)
        /// of the dynamic method.
        /// </summary>
        /// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be null.</param>
        /// <param name="returnType">
        /// A <see cref="Type" /> object that specifies the return type of the dynamic method,
        /// or null if the method has no return type.
        /// </param>
        /// <param name="parameterTypes">
        /// An array of <see cref="Type" /> objects specifying the types of the parameters of
        /// the dynamic method, or null if the method has no parameters.
        /// </param>
        /// <param name="owner">
        /// A <see cref="Type" /> with which the dynamic method is logically associated. The
        /// dynamic method has access to all members of the type.
        /// </param>
        /// <param name="skipVisibility">
        /// true to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic
        /// method; otherwise, false.
        /// </param>
        /// <exception cref="ArgumentException">
        /// An element of <paramref name="parameterTypes" /> is null or <see cref="Void" />.-or-
        /// <paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter
        /// of a generic type or method.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name" /> is null. -or-<paramref name="owner" /> is null.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="returnType" /> is null, or is a type for which <see cref="Type.IsByRef" />
        /// returns true.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public DynamicMethod(string name, System.Type returnType, System.Type[] parameterTypes, System.Type owner, bool skipVisibility) { }
        /// <summary>
        /// Gets the attributes specified when the dynamic method was created.
        /// </summary>
        /// <returns>
        /// A bitwise combination of the <see cref="MethodAttributes" /> values representing
        /// the attributes for the method.
        /// </returns>
        public override System.Reflection.MethodAttributes Attributes { get { return default(System.Reflection.MethodAttributes); } }
        /// <summary>
        /// Gets the calling convention specified when the dynamic method was created.
        /// </summary>
        /// <returns>
        /// One of the <see cref="CallingConventions" /> values that indicates the
        /// calling convention of the method.
        /// </returns>
        public override System.Reflection.CallingConventions CallingConvention { get { return default(System.Reflection.CallingConventions); } }
        /// <summary>
        /// Gets the type that declares the method, which is always null for dynamic methods.
        /// </summary>
        /// <returns>
        /// Always null.
        /// </returns>
        public override System.Type DeclaringType { get { return default(System.Type); } }
        /// <summary>
        /// Gets or sets a value indicating whether the local variables in the method are zero-initialized.
        /// </summary>
        /// <returns>
        /// true if the local variables in the method are zero-initialized; otherwise, false. The default
        /// is true.
        /// </returns>
        public bool InitLocals { get { return default(bool); } set { } }
        /// <summary>
        /// Gets the name of the dynamic method.
        /// </summary>
        /// <returns>
        /// The simple name of the method.
        /// </returns>
        public override string Name { get { return default(string); } }
        /// <summary>
        /// Gets the return parameter of the dynamic method.
        /// </summary>
        /// <returns>
        /// Always null.
        /// </returns>
        public override System.Reflection.ParameterInfo ReturnParameter { get { return default(System.Reflection.ParameterInfo); } }
        /// <summary>
        /// Gets the type of return value for the dynamic method.
        /// </summary>
        /// <returns>
        /// A <see cref="Type" /> representing the type of the return value of the current method;
        /// <see cref="Void" /> if the method has no return type.
        /// </returns>
        public override System.Type ReturnType { get { return default(System.Type); } }
        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it.
        /// </summary>
        /// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
        /// <returns>
        /// A delegate of the specified type, which can be used to execute the dynamic method.
        /// </returns>
        /// <exception cref="InvalidOperationException">The dynamic method has no method body.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="delegateType" /> has the wrong number of parameters or the wrong parameter
        /// types.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public sealed override System.Delegate CreateDelegate(System.Type delegateType) { return default(System.Delegate); }
        /// <summary>
        /// Completes the dynamic method and creates a delegate that can be used to execute it, specifying
        /// the delegate type and an object the delegate is bound to.
        /// </summary>
        /// <param name="delegateType">
        /// A delegate type whose signature matches that of the dynamic method, minus the first parameter.
        /// </param>
        /// <param name="target">
        /// An object the delegate is bound to. Must be of the same type as the first parameter of the
        /// dynamic method.
        /// </param>
        /// <returns>
        /// A delegate of the specified type, which can be used to execute the dynamic method with the
        /// specified target object.
        /// </returns>
        /// <exception cref="InvalidOperationException">The dynamic method has no method body.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="target" /> is not the same type as the first parameter of the dynamic method,
        /// and is not assignable to that type.-or-<paramref name="delegateType" /> has the wrong number
        /// of parameters or the wrong parameter types.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public sealed override System.Delegate CreateDelegate(System.Type delegateType, object target) { return default(System.Delegate); }
        /// <summary>
        /// Returns a Microsoft intermediate language (MSIL) generator for the method with a default MSIL
        /// stream size of 64 bytes.
        /// </summary>
        /// <returns>
        /// An <see cref="ILGenerator" /> object for the method.
        /// </returns>
        public System.Reflection.Emit.ILGenerator GetILGenerator() { return default(System.Reflection.Emit.ILGenerator); }
        /// <summary>
        /// Returns a Microsoft intermediate language (MSIL) generator for the method with the specified
        /// MSIL stream size.
        /// </summary>
        /// <param name="streamSize">The size of the MSIL stream, in bytes.</param>
        /// <returns>
        /// An <see cref="ILGenerator" /> object for the method, with the specified
        /// MSIL stream size.
        /// </returns>
        [System.Security.SecuritySafeCriticalAttribute]
        public System.Reflection.Emit.ILGenerator GetILGenerator(int streamSize) { return default(System.Reflection.Emit.ILGenerator); }
        /// <summary>
        /// Returns the parameters of the dynamic method.
        /// </summary>
        /// <returns>
        /// An array of <see cref="ParameterInfo" /> objects that represent the parameters
        /// of the dynamic method.
        /// </returns>
        public override System.Reflection.ParameterInfo[] GetParameters() { return default(System.Reflection.ParameterInfo[]); }
        /// <summary>
        /// Returns the signature of the method, represented as a string.
        /// </summary>
        /// <returns>
        /// A string representing the method signature.
        /// </returns>
        public override string ToString() { return default(string); }
    }
}
