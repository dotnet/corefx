// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection.Emit
{
    /// <summary>
    /// Helps build custom attributes.
    /// </summary>
    public partial class CustomAttributeBuilder
    {
        /// <summary>
        /// Initializes an instance of the CustomAttributeBuilder class given the constructor for the custom
        /// attribute and the arguments to the constructor.
        /// </summary>
        /// <param name="con">The constructor for the custom attribute.</param>
        /// <param name="constructorArgs">The arguments to the constructor of the custom attribute.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="con" /> is static or private.-or- The number of supplied arguments does not
        /// match the number of parameters of the constructor as required by the calling convention of
        /// the constructor.-or- The type of supplied argument does not match the type of the parameter
        /// declared in the constructor. -or-A supplied argument is a reference type other than <see cref="String" />
        /// or <see cref="Type" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="con" /> or <paramref name="constructorArgs" /> is null.
        /// </exception>
        public CustomAttributeBuilder(System.Reflection.ConstructorInfo con, object[] constructorArgs) { }
        /// <summary>
        /// Initializes an instance of the CustomAttributeBuilder class given the constructor for the custom
        /// attribute, the arguments to the constructor, and a set of named field/value pairs.
        /// </summary>
        /// <param name="con">The constructor for the custom attribute.</param>
        /// <param name="constructorArgs">The arguments to the constructor of the custom attribute.</param>
        /// <param name="namedFields">Named fields of the custom attribute.</param>
        /// <param name="fieldValues">Values for the named fields of the custom attribute.</param>
        /// <exception cref="ArgumentException">
        /// The lengths of the <paramref name="namedFields" /> and <paramref name="fieldValues" /> arrays
        /// are different.-or- <paramref name="con" /> is static or private.-or- The number of supplied
        /// arguments does not match the number of parameters of the constructor as required by the calling
        /// convention of the constructor.-or- The type of supplied argument does not match the type of
        /// the parameter declared in the constructor.-or- The types of the field values do not match
        /// the types of the named fields.-or- The field does not belong to the same class or base class
        /// as the constructor. -or-A supplied argument or named field is a reference type other than
        /// <see cref="String" /> or <see cref="Type" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">One of the parameters is null.</exception>
        public CustomAttributeBuilder(System.Reflection.ConstructorInfo con, object[] constructorArgs, System.Reflection.FieldInfo[] namedFields, object[] fieldValues) { }
        /// <summary>
        /// Initializes an instance of the CustomAttributeBuilder class given the constructor for the custom
        /// attribute, the arguments to the constructor, and a set of named property or value pairs.
        /// </summary>
        /// <param name="con">The constructor for the custom attribute.</param>
        /// <param name="constructorArgs">The arguments to the constructor of the custom attribute.</param>
        /// <param name="namedProperties">Named properties of the custom attribute.</param>
        /// <param name="propertyValues">Values for the named properties of the custom attribute.</param>
        /// <exception cref="ArgumentException">
        /// The lengths of the <paramref name="namedProperties" /> and <paramref name="propertyValues" />
        /// arrays are different.-or- <paramref name="con" /> is static or private.-or- The number
        /// of supplied arguments does not match the number of parameters of the constructor as required
        /// by the calling convention of the constructor.-or- The type of supplied argument does not match
        /// the type of the parameter declared in the constructor.-or- The types of the property values
        /// do not match the types of the named properties.-or- A property has no setter method.-or- The
        /// property does not belong to the same class or base class as the constructor. -or-A supplied
        /// argument or named property is a reference type other than <see cref="String" /> or
        /// <see cref="Type" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">One of the parameters is null.</exception>
        public CustomAttributeBuilder(System.Reflection.ConstructorInfo con, object[] constructorArgs, System.Reflection.PropertyInfo[] namedProperties, object[] propertyValues) { }
        /// <summary>
        /// Initializes an instance of the CustomAttributeBuilder class given the constructor for the custom
        /// attribute, the arguments to the constructor, a set of named property or value pairs, and a set
        /// of named field or value pairs.
        /// </summary>
        /// <param name="con">The constructor for the custom attribute.</param>
        /// <param name="constructorArgs">The arguments to the constructor of the custom attribute.</param>
        /// <param name="namedProperties">Named properties of the custom attribute.</param>
        /// <param name="propertyValues">Values for the named properties of the custom attribute.</param>
        /// <param name="namedFields">Named fields of the custom attribute.</param>
        /// <param name="fieldValues">Values for the named fields of the custom attribute.</param>
        /// <exception cref="ArgumentException">
        /// The lengths of the <paramref name="namedProperties" /> and <paramref name="propertyValues" />
        /// arrays are different.-or- The lengths of the <paramref name="namedFields" /> and <paramref name="fieldValues" />
        /// arrays are different.-or- <paramref name="con" /> is static or private.-or-
        /// The number of supplied arguments does not match the number of parameters of the constructor
        /// as required by the calling convention of the constructor.-or- The type of supplied argument
        /// does not match the type of the parameter declared in the constructor.-or- The types of the
        /// property values do not match the types of the named properties.-or- The types of the field
        /// values do not match the types of the corresponding field types.-or- A property has no setter.-or-
        /// The property or field does not belong to the same class or base class as the constructor.
        /// -or-A supplied argument, named property, or named field is a reference type other than <see cref="String" />
        /// or <see cref="Type" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">One of the parameters is null.</exception>
        public CustomAttributeBuilder(System.Reflection.ConstructorInfo con, object[] constructorArgs, System.Reflection.PropertyInfo[] namedProperties, object[] propertyValues, System.Reflection.FieldInfo[] namedFields, object[] fieldValues) { }
    }
    /// <summary>
    /// Generates Microsoft intermediate language (MSIL) instructions.
    /// </summary>
    public partial class ILGenerator
    {
        internal ILGenerator() { }
        /// <summary>
        /// Gets the current offset, in bytes, in the Microsoft intermediate language (MSIL) stream that
        /// is being emitted by the <see cref="ILGenerator" />.
        /// </summary>
        /// <returns>
        /// The offset in the MSIL stream at which the next instruction will be emitted.
        /// </returns>
        public virtual int ILOffset { get { return default(int); } }
        /// <summary>
        /// Begins a catch block.
        /// </summary>
        /// <param name="exceptionType">The <see cref="Type" /> object that represents the exception.</param>
        /// <exception cref="ArgumentException">The catch block is within a filtered exception.</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exceptionType" /> is null, and the exception filter block has not returned
        /// a value that indicates that finally blocks should be run until this catch block is located.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The Microsoft intermediate language (MSIL) being generated is not currently in an exception
        /// block.
        /// </exception>
        public virtual void BeginCatchBlock(System.Type exceptionType) { }
        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The Microsoft intermediate language (MSIL) being generated is not currently in an exception
        /// block. -or-This <see cref="ILGenerator" /> belongs to a
        /// <see cref="DynamicMethod" />.
        /// </exception>
        public virtual void BeginExceptFilterBlock() { }
        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <returns>
        /// The label for the end of the block. This will leave you in the correct place to execute finally
        /// blocks or to finish the try.
        /// </returns>
        public virtual System.Reflection.Emit.Label BeginExceptionBlock() { return default(System.Reflection.Emit.Label); }
        /// <summary>
        /// Begins an exception fault block in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The MSIL being generated is not currently in an exception block. -or-This
        /// <see cref="ILGenerator" /> belongs to a <see cref="DynamicMethod" />.
        /// </exception>
        public virtual void BeginFaultBlock() { }
        /// <summary>
        /// Begins a finally block in the Microsoft intermediate language (MSIL) instruction stream.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The MSIL being generated is not currently in an exception block.
        /// </exception>
        public virtual void BeginFinallyBlock() { }
        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This <see cref="ILGenerator" /> belongs to a
        /// <see cref="DynamicMethod" />.
        /// </exception>
        public virtual void BeginScope() { }
        /// <summary>
        /// Declares a local variable of the specified type.
        /// </summary>
        /// <param name="localType">
        /// A <see cref="Type" /> object that represents the type of the local variable.
        /// </param>
        /// <returns>
        /// The declared local variable.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="localType" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The containing type has been created by the <see cref="TypeBuilder.CreateType" />
        /// method.
        /// </exception>
        public virtual System.Reflection.Emit.LocalBuilder DeclareLocal(System.Type localType) { return default(System.Reflection.Emit.LocalBuilder); }
        /// <summary>
        /// Declares a local variable of the specified type, optionally pinning the object referred to
        /// by the variable.
        /// </summary>
        /// <param name="localType">A <see cref="Type" /> object that represents the type of the local variable.</param>
        /// <param name="pinned">true to pin the object in memory; otherwise, false.</param>
        /// <returns>
        /// A <see cref="LocalBuilder" /> object that represents the local variable.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="localType" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The containing type has been created by the <see cref="TypeBuilder.CreateType" />
        /// method.-or-The method body of the enclosing method has been created by the
        /// <see cref="MethodBuilder.CreateMethodBody(Byte[],Int32)" /> method.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The method with which this <see cref="ILGenerator" /> is associated
        /// is not represented by a <see cref="MethodBuilder" />.
        /// </exception>
        public virtual System.Reflection.Emit.LocalBuilder DeclareLocal(System.Type localType, bool pinned) { return default(System.Reflection.Emit.LocalBuilder); }
        /// <summary>
        /// Declares a new label.
        /// </summary>
        /// <returns>
        /// Returns a new label that can be used as a token for branching.
        /// </returns>
        public virtual System.Reflection.Emit.Label DefineLabel() { return default(System.Reflection.Emit.Label); }
        /// <summary>
        /// Puts the specified instruction onto the stream of instructions.
        /// </summary>
        /// <param name="opcode">The Microsoft Intermediate Language (MSIL) instruction to be put onto the stream.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode) { }
        /// <summary>
        /// Puts the specified instruction and character argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The character argument pushed onto the stream immediately after the instruction.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, byte arg) { }
        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream. Defined in the OpCodes enumeration.</param>
        /// <param name="arg">The numerical argument pushed onto the stream immediately after the instruction.</param>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, double arg) { }
        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="arg">The Int argument pushed onto the stream immediately after the instruction.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, short arg) { }
        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The numerical argument pushed onto the stream immediately after the instruction.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, int arg) { }
        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The numerical argument pushed onto the stream immediately after the instruction.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, long arg) { }
        /// <summary>
        /// Puts the specified instruction and metadata token for the specified constructor onto the Microsoft
        /// intermediate language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="con">A ConstructorInfo representing a constructor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="con" /> is null. This exception is new in the .NET Framework 4.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.ConstructorInfo con) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream and leaves
        /// space to include a label when fixes are done.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="label">The label to which to branch from this location.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.Emit.Label label) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream and leaves
        /// space to include a label when fixes are done.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="labels">
        /// The array of label objects to which to branch from this location. All of the labels will be
        /// used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="con" /> is null. This exception is new in the .NET Framework 4.
        /// </exception>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.Emit.Label[] labels) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed
        /// by the index of the given local variable.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="local">A local variable.</param>
        /// <exception cref="ArgumentException">
        /// The parent method of the <paramref name="local" /> parameter does not match the method associated
        /// with this <see cref="ILGenerator" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="local" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="opcode" /> is a single-byte instruction, and <paramref name="local" /> represents
        /// a local variable with an index greater than Byte.MaxValue.
        /// </exception>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.Emit.LocalBuilder local) { }
        /// <summary>
        /// Puts the specified instruction and a signature token onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="signature">A helper for constructing a signature token.</param>
        /// <exception cref="ArgumentNullException"><paramref name="signature" /> is null.</exception>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.Emit.SignatureHelper signature) { }
        /// <summary>
        /// Puts the specified instruction and metadata token for the specified field onto the Microsoft
        /// intermediate language (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="field">A FieldInfo representing a field.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.FieldInfo field) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed
        /// by the metadata token for the given method.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="meth">A MethodInfo representing a method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="meth" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// <paramref name="meth" /> is a generic method for which the
        /// <see cref="MethodInfo.IsGenericMethodDefinition" /> property is false.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Reflection.MethodInfo meth) { }
        /// <summary>
        /// Puts the specified instruction and character argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The character argument pushed onto the stream immediately after the instruction.</param>
        [System.CLSCompliantAttribute(false)]
        public void Emit(System.Reflection.Emit.OpCode opcode, sbyte arg) { }
        /// <summary>
        /// Puts the specified instruction and numerical argument onto the Microsoft intermediate language
        /// (MSIL) stream of instructions.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="arg">The Single argument pushed onto the stream immediately after the instruction.</param>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, float arg) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed
        /// by the metadata token for the given string.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be emitted onto the stream.</param>
        /// <param name="str">The String to be emitted.</param>
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, string str) { }
        /// <summary>
        /// Puts the specified instruction onto the Microsoft intermediate language (MSIL) stream followed
        /// by the metadata token for the given type.
        /// </summary>
        /// <param name="opcode">The MSIL instruction to be put onto the stream.</param>
        /// <param name="cls">A Type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cls" /> is null.</exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void Emit(System.Reflection.Emit.OpCode opcode, System.Type cls) { }
        /// <summary>
        /// Puts a call or callvirt instruction onto the Microsoft intermediate language (MSIL) stream
        /// to call a varargs method.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream. Must be
        /// <see cref="OpCodes.Call" />, <see cref="OpCodes.Callvirt" />, or
        /// <see cref="OpCodes.Newobj" />.
        /// </param>
        /// <param name="methodInfo">The varargs method to be called.</param>
        /// <param name="optionalParameterTypes">
        /// The types of the optional arguments if the method is a varargs method; otherwise, null.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="opcode" /> does not specify a method call.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The calling convention for the method is not varargs, but optional parameter types are supplied.
        /// This exception is thrown in the .NET Framework versions 1.0 and 1.1, In subsequent versions, no
        /// exception is thrown.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void EmitCall(System.Reflection.Emit.OpCode opcode, System.Reflection.MethodInfo methodInfo, System.Type[] optionalParameterTypes) { }
        /// <summary>
        /// Puts a <see cref="OpCodes.Calli" /> instruction onto the Microsoft
        /// intermediate language (MSIL) stream, specifying a managed calling convention for the indirect
        /// call.
        /// </summary>
        /// <param name="opcode">
        /// The MSIL instruction to be emitted onto the stream. Must be
        /// <see cref="OpCodes.Calli" />.
        /// </param>
        /// <param name="callingConvention">The managed calling convention to be used.</param>
        /// <param name="returnType">The <see cref="Type" /> of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments for varargs calls.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="optionalParameterTypes" /> is not null, but <paramref name="callingConvention" />
        /// does not include the <see cref="CallingConventions.VarArgs" /> flag.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void EmitCalli(System.Reflection.Emit.OpCode opcode, System.Reflection.CallingConventions callingConvention, System.Type returnType, System.Type[] parameterTypes, System.Type[] optionalParameterTypes) { }
        /// <summary>
        /// Emits the Microsoft intermediate language (MSIL) necessary to call
        /// <see cref="Overload:System.Console.WriteLine" /> with the given local variable.
        /// </summary>
        /// <param name="localBuilder">The local variable whose value is to be written to the console.</param>
        /// <exception cref="ArgumentException">
        /// The type of <paramref name="localBuilder" /> is <see cref="TypeBuilder" />
        /// or <see cref="EnumBuilder" />, which are not supported. -or-There
        /// is no overload of <see cref="Overload:System.Console.WriteLine" /> that accepts the type of
        /// <paramref name="localBuilder" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="localBuilder" /> is null.</exception>
        public virtual void EmitWriteLine(System.Reflection.Emit.LocalBuilder localBuilder) { }
        /// <summary>
        /// Emits the Microsoft intermediate language (MSIL) necessary to call
        /// <see cref="Overload:System.Console.WriteLine" /> with the given field.
        /// </summary>
        /// <param name="fld">The field whose value is to be written to the console.</param>
        /// <exception cref="ArgumentException">
        /// There is no overload of the <see cref="Overload:System.Console.WriteLine" /> method that accepts
        /// the type of the specified field.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="fld" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The type of the field is <see cref="TypeBuilder" /> or
        /// <see cref="EnumBuilder" />, which are not supported.
        /// </exception>
        public virtual void EmitWriteLine(System.Reflection.FieldInfo fld) { }
        /// <summary>
        /// Emits the Microsoft intermediate language (MSIL) to call <see cref="Overload:System.Console.WriteLine" />
        /// with a string.
        /// </summary>
        /// <param name="value">The string to be printed.</param>
        public virtual void EmitWriteLine(string value) { }
        /// <summary>
        /// Ends an exception block.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The end exception block occurs in an unexpected place in the code stream.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The Microsoft intermediate language (MSIL) being generated is not currently in an exception
        /// block.
        /// </exception>
        public virtual void EndExceptionBlock() { }
        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// This <see cref="ILGenerator" /> belongs to a
        /// <see cref="DynamicMethod" />.
        /// </exception>
        public virtual void EndScope() { }
        /// <summary>
        /// Marks the Microsoft intermediate language (MSIL) stream's current position with the given label.
        /// </summary>
        /// <param name="loc">The label for which to set an index.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="loc" /> represents an invalid index into the label array.-or- An index for
        /// <paramref name="loc" /> has already been defined.
        /// </exception>
        public virtual void MarkLabel(System.Reflection.Emit.Label loc) { }
        /// <summary>
        /// Emits an instruction to throw an exception.
        /// </summary>
        /// <param name="excType">The class of the type of exception to throw.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="excType" /> is not the <see cref="Exception" /> class or a derived
        /// class of <see cref="Exception" />.-or- The type does not have a default constructor.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="excType" /> is null.</exception>
        public virtual void ThrowException(System.Type excType) { }
        /// <summary>
        /// Specifies the namespace to be used in evaluating locals and watches for the current active
        /// lexical scope.
        /// </summary>
        /// <param name="usingNamespace">
        /// The namespace to be used in evaluating locals and watches for the current active lexical scope
        /// </param>
        /// <exception cref="ArgumentException">Length of <paramref name="usingNamespace" /> is zero.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="usingNamespace" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// This <see cref="ILGenerator" /> belongs to a
        /// <see cref="DynamicMethod" />.
        /// </exception>
        public virtual void UsingNamespace(string usingNamespace) { }
    }
    /// <summary>
    /// Represents a label in the instruction stream. Label is used in conjunction with the
    /// <see cref="ILGenerator" /> class.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct Label
    {
        /// <summary>
        /// Checks if the given object is an instance of Label and is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with this Label instance.</param>
        /// <returns>
        /// Returns true if <paramref name="obj" /> is an instance of Label and is equal to this object;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Indicates whether the current instance is equal to the specified <see cref="Label" />.
        /// </summary>
        /// <param name="obj">The <see cref="Label" /> to compare to the current instance.</param>
        /// <returns>
        /// true if the value of <paramref name="obj" /> is equal to the value of the current instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(System.Reflection.Emit.Label obj) { return default(bool); }
        /// <summary>
        /// Generates a hash code for this instance.
        /// </summary>
        /// <returns>
        /// Returns a hash code for this instance.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Indicates whether two <see cref="Label" /> structures are equal.
        /// </summary>
        /// <param name="a">The <see cref="Label" /> to compare to <paramref name="b" />.</param>
        /// <param name="b">The <see cref="Label" /> to compare to <paramref name="a" />.</param>
        /// <returns>
        /// true if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator ==(System.Reflection.Emit.Label a, System.Reflection.Emit.Label b) { return default(bool); }
        /// <summary>
        /// Indicates whether two <see cref="Label" /> structures are not equal.
        /// </summary>
        /// <param name="a">The <see cref="Label" /> to compare to <paramref name="b" />.</param>
        /// <param name="b">The <see cref="Label" /> to compare to <paramref name="a" />.</param>
        /// <returns>
        /// true if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator !=(System.Reflection.Emit.Label a, System.Reflection.Emit.Label b) { return default(bool); }
    }
    /// <summary>
    /// Represents a local variable within a method or constructor.
    /// </summary>
    public sealed partial class LocalBuilder : System.Reflection.LocalVariableInfo
    {
        internal LocalBuilder() { }
        /// <summary>
        /// Gets a value indicating whether the object referred to by the local variable is pinned in memory.
        /// </summary>
        /// <returns>
        /// true if the object referred to by the local variable is pinned in memory; otherwise, false.
        /// </returns>
        public override bool IsPinned { get { return default(bool); } }
        /// <summary>
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        /// <returns>
        /// An integer value that represents the order of declaration of the local variable within the
        /// method body.
        /// </returns>
        public override int LocalIndex { get { return default(int); } }
        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        /// <returns>
        /// The <see cref="Type" /> of the local variable.
        /// </returns>
        public override System.Type LocalType { get { return default(System.Type); } }
    }
    /// <summary>
    /// Creates or associates parameter information.
    /// </summary>
    public partial class ParameterBuilder
    {
        internal ParameterBuilder() { }
        /// <summary>
        /// Retrieves the attributes for this parameter.
        /// </summary>
        /// <returns>
        /// Read-only. Retrieves the attributes for this parameter.
        /// </returns>
        public virtual int Attributes { get { return default(int); } }
        /// <summary>
        /// Retrieves whether this is an input parameter.
        /// </summary>
        /// <returns>
        /// Read-only. Retrieves whether this is an input parameter.
        /// </returns>
        public bool IsIn { get { return default(bool); } }
        /// <summary>
        /// Retrieves whether this parameter is optional.
        /// </summary>
        /// <returns>
        /// Read-only. Specifies whether this parameter is optional.
        /// </returns>
        public bool IsOptional { get { return default(bool); } }
        /// <summary>
        /// Retrieves whether this parameter is an output parameter.
        /// </summary>
        /// <returns>
        /// Read-only. Retrieves whether this parameter is an output parameter.
        /// </returns>
        public bool IsOut { get { return default(bool); } }
        /// <summary>
        /// Retrieves the name of this parameter.
        /// </summary>
        /// <returns>
        /// Read-only. Retrieves the name of this parameter.
        /// </returns>
        public virtual string Name { get { return default(string); } }
        /// <summary>
        /// Retrieves the signature position for this parameter.
        /// </summary>
        /// <returns>
        /// Read-only. Retrieves the signature position for this parameter.
        /// </returns>
        public virtual int Position { get { return default(int); } }
        /// <summary>
        /// Sets the default value of the parameter.
        /// </summary>
        /// <param name="defaultValue">The default value of this parameter.</param>
        /// <exception cref="ArgumentException">
        /// The parameter is not one of the supported types.-or-The type of <paramref name="defaultValue" />
        /// does not match the type of the parameter.-or-The parameter is of type <see cref="Object" />
        /// or other reference type, <paramref name="defaultValue" /> is not null, and the value cannot
        /// be assigned to the reference type.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public virtual void SetConstant(object defaultValue) { }
        /// <summary>
        /// Set a custom attribute using a specified custom attribute blob.
        /// </summary>
        /// <param name="con">The constructor for the custom attribute.</param>
        /// <param name="binaryAttribute">A byte blob representing the attributes.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="con" /> or <paramref name="binaryAttribute" /> is null.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public void SetCustomAttribute(System.Reflection.ConstructorInfo con, byte[] binaryAttribute) { }
        /// <summary>
        /// Set a custom attribute using a custom attribute builder.
        /// </summary>
        /// <param name="customBuilder">An instance of a helper class to define the custom attribute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="con" /> is null.</exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public void SetCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder customBuilder) { }
    }
    /// <summary>
    /// Provides methods for building signatures.
    /// </summary>
    public sealed partial class SignatureHelper
    {
        internal SignatureHelper() { }
        /// <summary>
        /// Adds an argument to the signature.
        /// </summary>
        /// <param name="clsArgument">The type of the argument.</param>
        /// <exception cref="ArgumentException">The signature has already been finished.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="clsArgument" /> is null.</exception>
        public void AddArgument(System.Type clsArgument) { }
        /// <summary>
        /// Adds an argument of the specified type to the signature, specifying whether the argument is
        /// pinned.
        /// </summary>
        /// <param name="argument">The argument type.</param>
        /// <param name="pinned">true if the argument is pinned; otherwise, false.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument" /> is null.</exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public void AddArgument(System.Type argument, bool pinned) { }
        /// <summary>
        /// Adds an argument to the signature, with the specified custom modifiers.
        /// </summary>
        /// <param name="argument">The argument type.</param>
        /// <param name="requiredCustomModifiers">
        /// An array of types representing the required custom modifiers for the argument, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the argument has no required custom modifiers, specify null.
        /// </param>
        /// <param name="optionalCustomModifiers">
        /// An array of types representing the optional custom modifiers for the argument, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the argument has no optional custom modifiers, specify null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="argument" /> is null. -or-An element of <paramref name="requiredCustomModifiers" />
        /// or <paramref name="optionalCustomModifiers" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The signature has already been finished. -or-One of the specified custom modifiers is an array
        /// type.-or-One of the specified custom modifiers is an open generic type. That is, the
        /// <see cref="Type.ContainsGenericParameters" /> property is true for the custom modifier.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public void AddArgument(System.Type argument, System.Type[] requiredCustomModifiers, System.Type[] optionalCustomModifiers) { }
        /// <summary>
        /// Adds a set of arguments to the signature, with the specified custom modifiers.
        /// </summary>
        /// <param name="arguments">The types of the arguments to be added.</param>
        /// <param name="requiredCustomModifiers">
        /// An array of arrays of types. Each array of types represents the required custom modifiers
        /// for the corresponding argument, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsBoxed" />. If a particular argument has
        /// no required custom modifiers, specify null instead of an array of types. If none of the arguments
        /// have required custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <param name="optionalCustomModifiers">
        /// An array of arrays of types. Each array of types represents the optional custom modifiers
        /// for the corresponding argument, such as <see cref="Runtime.CompilerServices.IsConst" />
        /// or <see cref="Runtime.CompilerServices.IsBoxed" />. If a particular argument has
        /// no optional custom modifiers, specify null instead of an array of types. If none of the arguments
        /// have optional custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// An element of <paramref name="arguments" /> is null. -or-One of the specified custom modifiers
        /// is null. (However, null can be specified for the array of custom modifiers for any argument.)
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The signature has already been finished. -or-One of the specified custom modifiers is an array
        /// type.-or-One of the specified custom modifiers is an open generic type. That is, the
        /// <see cref="Type.ContainsGenericParameters" /> property is true for the custom modifier.
        /// -or-The size of <paramref name="requiredCustomModifiers" /> or <paramref name="optionalCustomModifiers" />
        /// does not equal the size of <paramref name="arguments" />.
        /// </exception>
        public void AddArguments(System.Type[] arguments, System.Type[][] requiredCustomModifiers, System.Type[][] optionalCustomModifiers) { }
        /// <summary>
        /// Marks the end of a vararg fixed part. This is only used if the caller is creating a vararg
        /// signature call site.
        /// </summary>
        public void AddSentinel() { }
        /// <summary>
        /// Checks if this instance is equal to the given object.
        /// </summary>
        /// <param name="obj">The object with which this instance should be compared.</param>
        /// <returns>
        /// true if the given object is a SignatureHelper and represents the same signature; otherwise,
        /// false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Returns a signature helper for a field.
        /// </summary>
        /// <param name="mod">The dynamic module that contains the field for which the SignatureHelper is requested.</param>
        /// <returns>
        /// The SignatureHelper object for a field.
        /// </returns>
        public static System.Reflection.Emit.SignatureHelper GetFieldSigHelper(System.Reflection.Module mod) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Creates and returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// Returns the hash code based on the name.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns a signature helper for a local variable.
        /// </summary>
        /// <returns>
        /// A <see cref="SignatureHelper" /> for a local variable.
        /// </returns>
        public static System.Reflection.Emit.SignatureHelper GetLocalVarSigHelper() { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a local variable.
        /// </summary>
        /// <param name="mod">
        /// The dynamic module that contains the local variable for which the SignatureHelper is requested.
        /// </param>
        /// <returns>
        /// The SignatureHelper object for a local variable.
        /// </returns>
        public static System.Reflection.Emit.SignatureHelper GetLocalVarSigHelper(System.Reflection.Module mod) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a method given the method's calling convention and return type.
        /// </summary>
        /// <param name="callingConvention">The calling convention of the method.</param>
        /// <param name="returnType">
        /// The return type of the method, or null for a void return type (Sub procedure in Visual Basic).
        /// </param>
        /// <returns>
        /// The SignatureHelper object for a method.
        /// </returns>
        public static System.Reflection.Emit.SignatureHelper GetMethodSigHelper(System.Reflection.CallingConventions callingConvention, System.Type returnType) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a method given the method's module, calling convention, and
        /// return type.
        /// </summary>
        /// <param name="mod">
        /// The <see cref="ModuleBuilder" /> that contains the method for which
        /// the SignatureHelper is requested.
        /// </param>
        /// <param name="callingConvention">The calling convention of the method.</param>
        /// <param name="returnType">
        /// The return type of the method, or null for a void return type (Sub procedure in Visual Basic).
        /// </param>
        /// <returns>
        /// The SignatureHelper object for a method.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="mod" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mod" /> is not a <see cref="ModuleBuilder" />.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public static System.Reflection.Emit.SignatureHelper GetMethodSigHelper(System.Reflection.Module mod, System.Reflection.CallingConventions callingConvention, System.Type returnType) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a method with a standard calling convention, given the method's
        /// module, return type, and argument types.
        /// </summary>
        /// <param name="mod">
        /// The <see cref="ModuleBuilder" /> that contains the method for which
        /// the SignatureHelper is requested.
        /// </param>
        /// <param name="returnType">
        /// The return type of the method, or null for a void return type (Sub procedure in Visual Basic).
        /// </param>
        /// <param name="parameterTypes">
        /// The types of the arguments of the method, or null if the method has no arguments.
        /// </param>
        /// <returns>
        /// The SignatureHelper object for a method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mod" /> is null.-or-An element of <paramref name="parameterTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mod" /> is not a <see cref="ModuleBuilder" />.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public static System.Reflection.Emit.SignatureHelper GetMethodSigHelper(System.Reflection.Module mod, System.Type returnType, System.Type[] parameterTypes) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a property, given the dynamic module that contains the property,
        /// the calling convention, the property type, the property arguments, and custom modifiers for the
        /// return type and arguments.
        /// </summary>
        /// <param name="mod">
        /// The <see cref="ModuleBuilder" /> that contains the property for which
        /// the <see cref="SignatureHelper" /> is requested.
        /// </param>
        /// <param name="callingConvention">The calling convention of the property accessors.</param>
        /// <param name="returnType">The property type.</param>
        /// <param name="requiredReturnTypeCustomModifiers">
        /// An array of types representing the required custom modifiers for the return type, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the return type has no required custom modifiers, specify null.
        /// </param>
        /// <param name="optionalReturnTypeCustomModifiers">
        /// An array of types representing the optional custom modifiers for the return type, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the return type has no optional custom modifiers, specify null.
        /// </param>
        /// <param name="parameterTypes">The types of the property's arguments, or null if the property has no arguments.</param>
        /// <param name="requiredParameterTypeCustomModifiers">
        /// An array of arrays of types. Each array of types represents the required custom modifiers for
        /// the corresponding argument of the property. If a particular argument has no required custom modifiers,
        /// specify null instead of an array of types. If the property has no arguments, or if none of the
        /// arguments have required custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <param name="optionalParameterTypeCustomModifiers">
        /// An array of arrays of types. Each array of types represents the optional custom modifiers for
        /// the corresponding argument of the property. If a particular argument has no optional custom modifiers,
        /// specify null instead of an array of types. If the property has no arguments, or if none of the
        /// arguments have optional custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <returns>
        /// A <see cref="SignatureHelper" /> object for a property.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mod" /> is null.-or-An element of <paramref name="parameterTypes" /> is null.
        /// -or-One of the specified custom modifiers is null. (However, null can be specified for the
        /// array of custom modifiers for any argument.)
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The signature has already been finished. -or-<paramref name="mod" /> is not a
        /// <see cref="ModuleBuilder" />.-or-One of the specified custom modifiers is an array type.-or-One of the specified custom
        /// modifiers is an open generic type. That is, the
        /// <see cref="Type.ContainsGenericParameters" /> property is true for the custom modifier.-or-The size of
        /// <paramref name="requiredParameterTypeCustomModifiers" /> or <paramref name="optionalParameterTypeCustomModifiers" /> does not equal the size of
        /// <paramref name="parameterTypes" />.
        /// </exception>
        [System.Security.SecuritySafeCriticalAttribute]
        public static System.Reflection.Emit.SignatureHelper GetPropertySigHelper(System.Reflection.Module mod, System.Reflection.CallingConventions callingConvention, System.Type returnType, System.Type[] requiredReturnTypeCustomModifiers, System.Type[] optionalReturnTypeCustomModifiers, System.Type[] parameterTypes, System.Type[][] requiredParameterTypeCustomModifiers, System.Type[][] optionalParameterTypeCustomModifiers) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a property, given the dynamic module that contains the property,
        /// the property type, and the property arguments.
        /// </summary>
        /// <param name="mod">
        /// The <see cref="ModuleBuilder" /> that contains the property for which
        /// the <see cref="SignatureHelper" /> is requested.
        /// </param>
        /// <param name="returnType">The property type.</param>
        /// <param name="parameterTypes">The argument types, or null if the property has no arguments.</param>
        /// <returns>
        /// A <see cref="SignatureHelper" /> object for a property.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mod" /> is null.-or-An element of <paramref name="parameterTypes" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="mod" /> is not a <see cref="ModuleBuilder" />.
        /// </exception>
        public static System.Reflection.Emit.SignatureHelper GetPropertySigHelper(System.Reflection.Module mod, System.Type returnType, System.Type[] parameterTypes) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Returns a signature helper for a property, given the dynamic module that contains the property,
        /// the property type, the property arguments, and custom modifiers for the return type and arguments.
        /// </summary>
        /// <param name="mod">
        /// The <see cref="ModuleBuilder" /> that contains the property for which
        /// the <see cref="SignatureHelper" /> is requested.
        /// </param>
        /// <param name="returnType">The property type.</param>
        /// <param name="requiredReturnTypeCustomModifiers">
        /// An array of types representing the required custom modifiers for the return type, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the return type has no required custom modifiers, specify null.
        /// </param>
        /// <param name="optionalReturnTypeCustomModifiers">
        /// An array of types representing the optional custom modifiers for the return type, such as
        /// <see cref="Runtime.CompilerServices.IsConst" /> or <see cref="Runtime.CompilerServices.IsBoxed" />.
        /// If the return type has no optional custom modifiers, specify null.
        /// </param>
        /// <param name="parameterTypes">The types of the property's arguments, or null if the property has no arguments.</param>
        /// <param name="requiredParameterTypeCustomModifiers">
        /// An array of arrays of types. Each array of types represents the required custom modifiers for
        /// the corresponding argument of the property. If a particular argument has no required custom modifiers,
        /// specify null instead of an array of types. If the property has no arguments, or if none of the
        /// arguments have required custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <param name="optionalParameterTypeCustomModifiers">
        /// An array of arrays of types. Each array of types represents the optional custom modifiers for
        /// the corresponding argument of the property. If a particular argument has no optional custom modifiers,
        /// specify null instead of an array of types. If the property has no arguments, or if none of the
        /// arguments have optional custom modifiers, specify null instead of an array of arrays.
        /// </param>
        /// <returns>
        /// A <see cref="SignatureHelper" /> object for a property.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mod" /> is null.-or-An element of <paramref name="parameterTypes" /> is null.
        /// -or-One of the specified custom modifiers is null. (However, null can be specified for the
        /// array of custom modifiers for any argument.)
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The signature has already been finished. -or-<paramref name="mod" /> is not a
        /// <see cref="ModuleBuilder" />.-or-One of the specified custom modifiers is an array type.-or-One of the specified custom
        /// modifiers is an open generic type. That is, the
        /// <see cref="Type.ContainsGenericParameters" /> property is true for the custom modifier.-or-The size of
        /// <paramref name="requiredParameterTypeCustomModifiers" /> or <paramref name="optionalParameterTypeCustomModifiers" /> does not equal the size of
        /// <paramref name="parameterTypes" />.
        /// </exception>
        public static System.Reflection.Emit.SignatureHelper GetPropertySigHelper(System.Reflection.Module mod, System.Type returnType, System.Type[] requiredReturnTypeCustomModifiers, System.Type[] optionalReturnTypeCustomModifiers, System.Type[] parameterTypes, System.Type[][] requiredParameterTypeCustomModifiers, System.Type[][] optionalParameterTypeCustomModifiers) { return default(System.Reflection.Emit.SignatureHelper); }
        /// <summary>
        /// Adds the end token to the signature and marks the signature as finished, so no further tokens
        /// can be added.
        /// </summary>
        /// <returns>
        /// Returns a byte array made up of the full signature.
        /// </returns>
        public byte[] GetSignature() { return default(byte[]); }
        /// <summary>
        /// Returns a string representing the signature arguments.
        /// </summary>
        /// <returns>
        /// Returns a string representing the arguments of this signature.
        /// </returns>
        public override string ToString() { return default(string); }
    }
}
