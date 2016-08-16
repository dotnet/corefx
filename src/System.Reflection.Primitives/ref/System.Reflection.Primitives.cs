// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Reflection
{
    /// <summary>
    /// Defines the valid calling conventions for a method.
    /// </summary>
    [System.FlagsAttribute]
    public enum CallingConventions
    {
        /// <summary>
        /// Specifies that either the Standard or the VarArgs calling convention may be used.
        /// </summary>
        Any = 3,
        /// <summary>
        /// Specifies that the signature is a function-pointer signature, representing a call to an instance
        /// or virtual method (not a static method). If ExplicitThis is set, HasThis must also be set. The
        /// first argument passed to the called method is still a this pointer, but the type of the first argument
        /// is now unknown. Therefore, a token that describes the type (or class) of the this pointer is explicitly
        /// stored into its metadata signature.
        /// </summary>
        ExplicitThis = 64,
        /// <summary>
        /// Specifies an instance or virtual method (not a static method). At run-time, the called method
        /// is passed a pointer to the target object as its first argument (the this pointer). The signature
        /// stored in metadata does not include the type of this first argument, because the method is known and
        /// its owner class can be discovered from metadata.
        /// </summary>
        HasThis = 32,
        /// <summary>
        /// Specifies the default calling convention as determined by the common language runtime. Use
        /// this calling convention for static methods. For instance or virtual methods use HasThis.
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Specifies the calling convention for methods with variable arguments.
        /// </summary>
        VarArgs = 2,
    }
    /// <summary>
    /// Specifies the attributes of an event.
    /// </summary>
    [System.FlagsAttribute]
    public enum EventAttributes
    {
        /// <summary>
        /// Specifies that the event has no attributes.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the common language runtime should check name encoding.
        /// </summary>
        RTSpecialName = 1024,
        /// <summary>
        /// Specifies that the event is special in a way described by the name.
        /// </summary>
        SpecialName = 512,
    }
    /// <summary>
    /// Specifies flags that describe the attributes of a field.
    /// </summary>
    [System.FlagsAttribute]
    public enum FieldAttributes
    {
        /// <summary>
        /// Specifies that the field is accessible throughout the assembly.
        /// </summary>
        Assembly = 3,
        /// <summary>
        /// Specifies that the field is accessible only by subtypes in this assembly.
        /// </summary>
        FamANDAssem = 2,
        /// <summary>
        /// Specifies that the field is accessible only by type and subtypes.
        /// </summary>
        Family = 4,
        /// <summary>
        /// Specifies that the field is accessible by subtypes anywhere, as well as throughout this assembly.
        /// </summary>
        FamORAssem = 5,
        /// <summary>
        /// Specifies the access level of a given field.
        /// </summary>
        FieldAccessMask = 7,
        /// <summary>
        /// Specifies that the field has a default value.
        /// </summary>
        HasDefault = 32768,
        /// <summary>
        /// Specifies that the field has marshaling information.
        /// </summary>
        HasFieldMarshal = 4096,
        /// <summary>
        /// Specifies that the field has a relative virtual address (RVA). The RVA is the location of the
        /// method body in the current image, as an address relative to the start of the image file in which
        /// it is located.
        /// </summary>
        HasFieldRVA = 256,
        /// <summary>
        /// Specifies that the field is initialized only, and can be set only in the body of a constructor.
        /// </summary>
        InitOnly = 32,
        /// <summary>
        /// Specifies that the field's value is a compile-time (static or early bound) constant. Any attempt
        /// to set it throws a <see cref="FieldAccessException" />.
        /// </summary>
        Literal = 64,
        /// <summary>
        /// Specifies that the field does not have to be serialized when the type is remoted.
        /// </summary>
        NotSerialized = 128,
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        PinvokeImpl = 8192,
        /// <summary>
        /// Specifies that the field is accessible only by the parent type.
        /// </summary>
        Private = 1,
        /// <summary>
        /// Specifies that the field cannot be referenced.
        /// </summary>
        PrivateScope = 0,
        /// <summary>
        /// Specifies that the field is accessible by any member for whom this scope is visible.
        /// </summary>
        Public = 6,
        /// <summary>
        /// Specifies that the common language runtime (metadata internal APIs) should check the name encoding.
        /// </summary>
        RTSpecialName = 1024,
        /// <summary>
        /// Specifies a special method, with the name describing how the method is special.
        /// </summary>
        SpecialName = 512,
        /// <summary>
        /// Specifies that the field represents the defined type, or else it is per-instance.
        /// </summary>
        Static = 16,
    }
    /// <summary>
    /// Describes the constraints on a generic type parameter of a generic type or method.
    /// </summary>
    [System.FlagsAttribute]
    public enum GenericParameterAttributes
    {
        /// <summary>
        /// The generic type parameter is contravariant. A contravariant type parameter can appear as a
        /// parameter type in method signatures.
        /// </summary>
        Contravariant = 2,
        /// <summary>
        /// The generic type parameter is covariant. A covariant type parameter can appear as the result
        /// type of a method, the type of a read-only field, a declared base type, or an implemented interface.
        /// </summary>
        Covariant = 1,
        /// <summary>
        /// A type can be substituted for the generic type parameter only if it has a parameterless constructor.
        /// </summary>
        DefaultConstructorConstraint = 16,
        /// <summary>
        /// There are no special flags.
        /// </summary>
        None = 0,
        /// <summary>
        /// A type can be substituted for the generic type parameter only if it is a value type and is
        /// not nullable.
        /// </summary>
        NotNullableValueTypeConstraint = 8,
        /// <summary>
        /// A type can be substituted for the generic type parameter only if it is a reference type.
        /// </summary>
        ReferenceTypeConstraint = 4,
        /// <summary>
        /// Selects the combination of all special constraint flags. This value is the result of using
        /// logical OR to combine the following flags:
        /// <see cref="DefaultConstructorConstraint" />, <see cref="ReferenceTypeConstraint" />,
        /// and <see cref="NotNullableValueTypeConstraint" />.
        /// </summary>
        SpecialConstraintMask = 28,
        /// <summary>
        /// Selects the combination of all variance flags. This value is the result of using logical OR
        /// to combine the following flags: <see cref="Contravariant" />
        /// and <see cref="Covariant" />.
        /// </summary>
        VarianceMask = 3,
    }
    /// <summary>
    /// Specifies flags for method attributes. These flags are defined in the corhdr.h file.
    /// </summary>
    [System.FlagsAttribute]
    public enum MethodAttributes
    {
        /// <summary>
        /// Indicates that the class does not provide an implementation of this method.
        /// </summary>
        Abstract = 1024,
        /// <summary>
        /// Indicates that the method is accessible to any class of this assembly.
        /// </summary>
        Assembly = 3,
        /// <summary>
        /// Indicates that the method can only be overridden when it is also accessible.
        /// </summary>
        CheckAccessOnOverride = 512,
        /// <summary>
        /// Indicates that the method is accessible to members of this type and its derived types that
        /// are in this assembly only.
        /// </summary>
        FamANDAssem = 2,
        /// <summary>
        /// Indicates that the method is accessible only to members of this class and its derived classes.
        /// </summary>
        Family = 4,
        /// <summary>
        /// Indicates that the method is accessible to derived classes anywhere, as well as to any class
        /// in the assembly.
        /// </summary>
        FamORAssem = 5,
        /// <summary>
        /// Indicates that the method cannot be overridden.
        /// </summary>
        Final = 32,
        /// <summary>
        /// Indicates that the method has security associated with it. Reserved flag for runtime use only.
        /// </summary>
        HasSecurity = 16384,
        /// <summary>
        /// Indicates that the method hides by name and signature; otherwise, by name only.
        /// </summary>
        HideBySig = 128,
        /// <summary>
        /// Retrieves accessibility information.
        /// </summary>
        MemberAccessMask = 7,
        /// <summary>
        /// Indicates that the method always gets a new slot in the vtable.
        /// </summary>
        NewSlot = 256,
        /// <summary>
        /// Indicates that the method implementation is forwarded through PInvoke (Platform Invocation
        /// Services).
        /// </summary>
        PinvokeImpl = 8192,
        /// <summary>
        /// Indicates that the method is accessible only to the current class.
        /// </summary>
        Private = 1,
        /// <summary>
        /// Indicates that the member cannot be referenced.
        /// </summary>
        PrivateScope = 0,
        /// <summary>
        /// Indicates that the method is accessible to any object for which this object is in scope.
        /// </summary>
        Public = 6,
        /// <summary>
        /// Indicates that the method calls another method containing security code. Reserved flag for
        /// runtime use only.
        /// </summary>
        RequireSecObject = 32768,
        /// <summary>
        /// Indicates that the method will reuse an existing slot in the vtable. This is the default behavior.
        /// </summary>
        ReuseSlot = 0,
        /// <summary>
        /// Indicates that the common language runtime checks the name encoding.
        /// </summary>
        RTSpecialName = 4096,
        /// <summary>
        /// Indicates that the method is special. The name describes how this method is special.
        /// </summary>
        SpecialName = 2048,
        /// <summary>
        /// Indicates that the method is defined on the type; otherwise, it is defined per instance.
        /// </summary>
        Static = 16,
        /// <summary>
        /// Indicates that the managed method is exported by thunk to unmanaged code.
        /// </summary>
        UnmanagedExport = 8,
        /// <summary>
        /// Indicates that the method is virtual.
        /// </summary>
        Virtual = 64,
        /// <summary>
        /// Retrieves vtable attributes.
        /// </summary>
        VtableLayoutMask = 256,
    }
    /// <summary>
    /// Specifies flags for the attributes of a method implementation.
    /// </summary>
    public enum MethodImplAttributes
    {
        /// <summary>
        /// Specifies that the method should be inlined wherever possible.
        /// </summary>
        AggressiveInlining = 256,
        /// <summary>
        /// Specifies flags about code type.
        /// </summary>
        CodeTypeMask = 3,
        /// <summary>
        /// Specifies that the method is not defined.
        /// </summary>
        ForwardRef = 16,
        /// <summary>
        /// Specifies that the method implementation is in Microsoft intermediate language (MSIL).
        /// </summary>
        IL = 0,
        /// <summary>
        /// Specifies an internal call.
        /// </summary>
        InternalCall = 4096,
        /// <summary>
        /// Specifies that the method is implemented in managed code.
        /// </summary>
        Managed = 0,
        /// <summary>
        /// Specifies whether the method is implemented in managed or unmanaged code.
        /// </summary>
        ManagedMask = 4,
        /// <summary>
        /// Specifies that the method implementation is native.
        /// </summary>
        Native = 1,
        /// <summary>
        /// Specifies that the method cannot be inlined.
        /// </summary>
        NoInlining = 8,
        /// <summary>
        /// Specifies that the method is not optimized by the just-in-time (JIT) compiler or by native
        /// code generation (see Ngen.exe) when debugging possible code generation problems.
        /// </summary>
        NoOptimization = 64,
        /// <summary>
        /// Specifies that the method implementation is in Optimized Intermediate Language (OPTIL).
        /// </summary>
        OPTIL = 2,
        /// <summary>
        /// Specifies that the method signature is exported exactly as declared.
        /// </summary>
        PreserveSig = 128,
        /// <summary>
        /// Specifies that the method implementation is provided by the runtime.
        /// </summary>
        Runtime = 3,
        /// <summary>
        /// Specifies that the method is single-threaded through the body. Static methods (Shared in Visual
        /// Basic) lock on the type, whereas instance methods lock on the instance. You can also use the C#
        /// lock statement or the Visual Basic SyncLock statement for this purpose.
        /// </summary>
        Synchronized = 32,
        /// <summary>
        /// Specifies that the method is implemented in unmanaged code.
        /// </summary>
        Unmanaged = 4,
    }
    /// <summary>
    /// Defines the attributes that can be associated with a parameter. These are defined in CorHdr.h.
    /// </summary>
    [System.FlagsAttribute]
    public enum ParameterAttributes
    {
        /// <summary>
        /// Specifies that the parameter has a default value.
        /// </summary>
        HasDefault = 4096,
        /// <summary>
        /// Specifies that the parameter has field marshaling information.
        /// </summary>
        HasFieldMarshal = 8192,
        /// <summary>
        /// Specifies that the parameter is an input parameter.
        /// </summary>
        In = 1,
        /// <summary>
        /// Specifies that the parameter is a locale identifier (lcid).
        /// </summary>
        Lcid = 4,
        /// <summary>
        /// Specifies that there is no parameter attribute.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the parameter is optional.
        /// </summary>
        Optional = 16,
        /// <summary>
        /// Specifies that the parameter is an output parameter.
        /// </summary>
        Out = 2,
        /// <summary>
        /// Specifies that the parameter is a return value.
        /// </summary>
        Retval = 8,
    }
    /// <summary>
    /// Defines the attributes that can be associated with a property. These attribute values are defined
    /// in corhdr.h.
    /// </summary>
    [System.FlagsAttribute]
    public enum PropertyAttributes
    {
        /// <summary>
        /// Specifies that the property has a default value.
        /// </summary>
        HasDefault = 4096,
        /// <summary>
        /// Specifies that no attributes are associated with a property.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the metadata internal APIs check the name encoding.
        /// </summary>
        RTSpecialName = 1024,
        /// <summary>
        /// Specifies that the property is special, with the name describing how the property is special.
        /// </summary>
        SpecialName = 512,
    }
    /// <summary>
    /// Specifies type attributes.
    /// </summary>
    [System.FlagsAttribute]
    public enum TypeAttributes
    {
        /// <summary>
        /// Specifies that the type is abstract.
        /// </summary>
        Abstract = 128,
        /// <summary>
        /// LPTSTR is interpreted as ANSI.
        /// </summary>
        AnsiClass = 0,
        /// <summary>
        /// LPTSTR is interpreted automatically.
        /// </summary>
        AutoClass = 131072,
        /// <summary>
        /// Specifies that class fields are automatically laid out by the common language runtime.
        /// </summary>
        AutoLayout = 0,
        /// <summary>
        /// Specifies that calling static methods of the type does not force the system to initialize the
        /// type.
        /// </summary>
        BeforeFieldInit = 1048576,
        /// <summary>
        /// Specifies that the type is a class.
        /// </summary>
        Class = 0,
        /// <summary>
        /// Specifies class semantics information; the current class is contextful (else agile).
        /// </summary>
        ClassSemanticsMask = 32,
        /// <summary>
        /// LPSTR is interpreted by some implementation-specific means, which includes the possibility
        /// of throwing a <see cref="NotSupportedException" />. Not used in the Microsoft implementation
        /// of the .NET Framework.
        /// </summary>
        CustomFormatClass = 196608,
        /// <summary>
        /// Used to retrieve non-standard encoding information for native interop. The meaning of the values
        /// of these 2 bits is unspecified. Not used in the Microsoft implementation of the .NET Framework.
        /// </summary>
        CustomFormatMask = 12582912,
        /// <summary>
        /// Specifies that class fields are laid out at the specified offsets.
        /// </summary>
        ExplicitLayout = 16,
        /// <summary>
        /// Type has security associate with it.
        /// </summary>
        HasSecurity = 262144,
        /// <summary>
        /// Specifies that the class or interface is imported from another module.
        /// </summary>
        Import = 4096,
        /// <summary>
        /// Specifies that the type is an interface.
        /// </summary>
        Interface = 32,
        /// <summary>
        /// Specifies class layout information.
        /// </summary>
        LayoutMask = 24,
        /// <summary>
        /// Specifies that the class is nested with assembly visibility, and is thus accessible only by
        /// methods within its assembly.
        /// </summary>
        NestedAssembly = 5,
        /// <summary>
        /// Specifies that the class is nested with assembly and family visibility, and is thus accessible
        /// only by methods lying in the intersection of its family and assembly.
        /// </summary>
        NestedFamANDAssem = 6,
        /// <summary>
        /// Specifies that the class is nested with family visibility, and is thus accessible only by methods
        /// within its own type and any derived types.
        /// </summary>
        NestedFamily = 4,
        /// <summary>
        /// Specifies that the class is nested with family or assembly visibility, and is thus accessible
        /// only by methods lying in the union of its family and assembly.
        /// </summary>
        NestedFamORAssem = 7,
        /// <summary>
        /// Specifies that the class is nested with private visibility.
        /// </summary>
        NestedPrivate = 3,
        /// <summary>
        /// Specifies that the class is nested with public visibility.
        /// </summary>
        NestedPublic = 2,
        /// <summary>
        /// Specifies that the class is not public.
        /// </summary>
        NotPublic = 0,
        /// <summary>
        /// Specifies that the class is public.
        /// </summary>
        Public = 1,
        /// <summary>
        /// Runtime should check name encoding.
        /// </summary>
        RTSpecialName = 2048,
        /// <summary>
        /// Specifies that the class is concrete and cannot be extended.
        /// </summary>
        Sealed = 256,
        /// <summary>
        /// Specifies that class fields are laid out sequentially, in the order that the fields were emitted
        /// to the metadata.
        /// </summary>
        SequentialLayout = 8,
        /// <summary>
        /// Specifies that the class can be serialized.
        /// </summary>
        Serializable = 8192,
        /// <summary>
        /// Specifies that the class is special in a way denoted by the name.
        /// </summary>
        SpecialName = 1024,
        /// <summary>
        /// Used to retrieve string information for native interoperability.
        /// </summary>
        StringFormatMask = 196608,
        /// <summary>
        /// LPTSTR is interpreted as UNICODE.
        /// </summary>
        UnicodeClass = 65536,
        /// <summary>
        /// Specifies type visibility information.
        /// </summary>
        VisibilityMask = 7,
        /// <summary>
        /// Specifies a Windows Runtime type.
        /// </summary>
        WindowsRuntime = 16384,
    }
}
namespace System.Reflection.Emit
{
    /// <summary>
    /// Describes how an instruction alters the flow of control.
    /// </summary>
    public enum FlowControl
    {
        /// <summary>
        /// Branch instruction.
        /// </summary>
        Branch = 0,
        /// <summary>
        /// Break instruction.
        /// </summary>
        Break = 1,
        /// <summary>
        /// Call instruction.
        /// </summary>
        Call = 2,
        /// <summary>
        /// Conditional branch instruction.
        /// </summary>
        Cond_Branch = 3,
        /// <summary>
        /// Provides information about a subsequent instruction. For example, the Unaligned instruction
        /// of Reflection.Emit.Opcodes has FlowControl.Meta and specifies that the subsequent pointer instruction
        /// might be unaligned.
        /// </summary>
        Meta = 4,
        /// <summary>
        /// Normal flow of control.
        /// </summary>
        Next = 5,
        /// <summary>
        /// Return instruction.
        /// </summary>
        Return = 7,
        /// <summary>
        /// Exception throw instruction.
        /// </summary>
        Throw = 8,
    }
    /// <summary>
    /// Describes an intermediate language (IL) instruction.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct OpCode
    {
        /// <summary>
        /// The flow control characteristics of the intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The type of flow control.
        /// </returns>
        public System.Reflection.Emit.FlowControl FlowControl { get { return default(System.Reflection.Emit.FlowControl); } }
        /// <summary>
        /// The name of the intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The name of the IL instruction.
        /// </returns>
        public string Name { get { return default(string); } }
        /// <summary>
        /// The type of intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The type of intermediate language (IL) instruction.
        /// </returns>
        public System.Reflection.Emit.OpCodeType OpCodeType { get { return default(System.Reflection.Emit.OpCodeType); } }
        /// <summary>
        /// The operand type of an intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The operand type of an IL instruction.
        /// </returns>
        public System.Reflection.Emit.OperandType OperandType { get { return default(System.Reflection.Emit.OperandType); } }
        /// <summary>
        /// The size of the intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The size of the IL instruction.
        /// </returns>
        public int Size { get { return default(int); } }
        /// <summary>
        /// How the intermediate language (IL) instruction pops the stack.
        /// </summary>
        /// <returns>
        /// Read-only. The way the IL instruction pops the stack.
        /// </returns>
        public System.Reflection.Emit.StackBehaviour StackBehaviourPop { get { return default(System.Reflection.Emit.StackBehaviour); } }
        /// <summary>
        /// How the intermediate language (IL) instruction pushes operand onto the stack.
        /// </summary>
        /// <returns>
        /// Read-only. The way the IL instruction pushes operand onto the stack.
        /// </returns>
        public System.Reflection.Emit.StackBehaviour StackBehaviourPush { get { return default(System.Reflection.Emit.StackBehaviour); } }
        /// <summary>
        /// Gets the numeric value of the intermediate language (IL) instruction.
        /// </summary>
        /// <returns>
        /// Read-only. The numeric value of the IL instruction.
        /// </returns>
        public short Value { get { return default(short); } }
        /// <summary>
        /// Tests whether the given object is equal to this Opcode.
        /// </summary>
        /// <param name="obj">The object to compare to this object.</param>
        /// <returns>
        /// true if <paramref name="obj" /> is an instance of Opcode and is equal to this object; otherwise,
        /// false.
        /// </returns>
        public override bool Equals(object obj) { return default(bool); }
        /// <summary>
        /// Indicates whether the current instance is equal to the specified <see cref="OpCode" />.
        /// </summary>
        /// <param name="obj">The <see cref="OpCode" /> to compare to the current instance.</param>
        /// <returns>
        /// true if the value of <paramref name="obj" /> is equal to the value of the current instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(System.Reflection.Emit.OpCode obj) { return default(bool); }
        /// <summary>
        /// Returns the generated hash code for this Opcode.
        /// </summary>
        /// <returns>
        /// Returns the hash code for this instance.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Indicates whether two <see cref="OpCode" /> structures are equal.
        /// </summary>
        /// <param name="a">The <see cref="OpCode" /> to compare to <paramref name="b" />.</param>
        /// <param name="b">The <see cref="OpCode" /> to compare to <paramref name="a" />.</param>
        /// <returns>
        /// true if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator ==(System.Reflection.Emit.OpCode a, System.Reflection.Emit.OpCode b) { return default(bool); }
        /// <summary>
        /// Indicates whether two <see cref="OpCode" /> structures are not equal.
        /// </summary>
        /// <param name="a">The <see cref="OpCode" /> to compare to <paramref name="b" />.</param>
        /// <param name="b">The <see cref="OpCode" /> to compare to <paramref name="a" />.</param>
        /// <returns>
        /// true if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, false.
        /// </returns>
        public static bool operator !=(System.Reflection.Emit.OpCode a, System.Reflection.Emit.OpCode b) { return default(bool); }
        /// <summary>
        /// Returns this Opcode as a <see cref="String" />.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="String" /> containing the name of this Opcode.
        /// </returns>
        public override string ToString() { return default(string); }
    }
    /// <summary>
    /// Provides field representations of the Microsoft Intermediate Language (MSIL) instructions
    /// for emission by the <see cref="ILGenerator" /> class members (such
    /// as <see cref="ILGenerator.Emit(OpCode)" />).
    /// </summary>
    public partial class OpCodes
    {
        internal OpCodes() { }
        /// <summary>
        /// Adds two values and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Add;
        /// <summary>
        /// Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Add_Ovf;
        /// <summary>
        /// Adds two unsigned integer values, performs an overflow check, and pushes the result onto the
        /// evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Add_Ovf_Un;
        /// <summary>
        /// Computes the bitwise AND of two values and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode And;
        /// <summary>
        /// Returns an unmanaged pointer to the argument list of the current method.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Arglist;
        /// <summary>
        /// Transfers control to a target instruction if two values are equal.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Beq;
        /// <summary>
        /// Transfers control to a target instruction (short form) if two values are equal.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Beq_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than or equal to the
        /// second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bge;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than or
        /// equal to the second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bge_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bge_Un;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the
        /// second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bge_Un_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bgt;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the
        /// second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bgt_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bgt_Un;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the
        /// second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bgt_Un_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second
        /// value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ble;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal
        /// to the second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ble_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second
        /// value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ble_Un;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal
        /// to the second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ble_Un_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Blt;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the
        /// second value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Blt_S;
        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Blt_Un;
        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the
        /// second value, when comparing unsigned integer values or unordered float values.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Blt_Un_S;
        /// <summary>
        /// Transfers control to a target instruction when two unsigned integer values or unordered float
        /// values are not equal.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bne_Un;
        /// <summary>
        /// Transfers control to a target instruction (short form) when two unsigned integer values or
        /// unordered float values are not equal.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Bne_Un_S;
        /// <summary>
        /// Converts a value type to an object reference (type O).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Box;
        /// <summary>
        /// Unconditionally transfers control to a target instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Br;
        /// <summary>
        /// Unconditionally transfers control to a target instruction (short form).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Br_S;
        /// <summary>
        /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point
        /// has been tripped.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Break;
        /// <summary>
        /// Transfers control to a target instruction if <paramref name="value" /> is false, a null reference
        /// (Nothing in Visual Basic), or zero.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Brfalse;
        /// <summary>
        /// Transfers control to a target instruction if <paramref name="value" /> is false, a null reference,
        /// or zero.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Brfalse_S;
        /// <summary>
        /// Transfers control to a target instruction if <paramref name="value" /> is true, not null,
        /// or non-zero.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Brtrue;
        /// <summary>
        /// Transfers control to a target instruction (short form) if <paramref name="value" /> is true,
        /// not null, or non-zero.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Brtrue_S;
        /// <summary>
        /// Calls the method indicated by the passed method descriptor.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Call;
        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments
        /// described by a calling convention.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Calli;
        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Callvirt;
        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Castclass;
        /// <summary>
        /// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation
        /// stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ceq;
        /// <summary>
        /// Compares two values. If the first value is greater than the second, the integer value 1 (int32)
        /// is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Cgt;
        /// <summary>
        /// Compares two unsigned or unordered values. If the first value is greater than the second, the
        /// integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the
        /// evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Cgt_Un;
        /// <summary>
        /// Throws <see cref="ArithmeticException" /> if value is not a finite number.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ckfinite;
        /// <summary>
        /// Compares two values. If the first value is less than the second, the integer value 1 (int32)
        /// is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Clt;
        /// <summary>
        /// Compares the unsigned or unordered values <paramref name="value1" /> and <paramref name="value2" />.
        /// If <paramref name="value1" /> is less than <paramref name="value2" />, then the integer
        /// value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the
        /// evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Clt_Un;
        /// <summary>
        /// Constrains the type on which a virtual method call is made.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Constrained;
        /// <summary>
        /// Converts the value on top of the evaluation stack to native int.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_I;
        /// <summary>
        /// Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_I1;
        /// <summary>
        /// Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_I2;
        /// <summary>
        /// Converts the value on top of the evaluation stack to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_I4;
        /// <summary>
        /// Converts the value on top of the evaluation stack to int64.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_I8;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed native int, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed native int, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int8 and extends it to
        /// int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I1;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to
        /// int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I1_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int16 and extending it
        /// to int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I2;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int16 and extends it
        /// to int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I2_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int32, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I4;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int32, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I4_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to signed int64, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I8;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int64, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_I8_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned native int, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to
        /// int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U1;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it
        /// to int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U1_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int16 and extends it
        /// to int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U2;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it
        /// to int32, throwing <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U2_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int32, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U4;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U4_Un;
        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int64, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U8;
        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing
        /// <see cref="OverflowException" /> on overflow.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_Ovf_U8_Un;
        /// <summary>
        /// Converts the unsigned integer value on top of the evaluation stack to float32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_R_Un;
        /// <summary>
        /// Converts the value on top of the evaluation stack to float32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_R4;
        /// <summary>
        /// Converts the value on top of the evaluation stack to float64.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_R8;
        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned native int, and extends it to
        /// native int.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_U;
        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_U1;
        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_U2;
        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_U4;
        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Conv_U8;
        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Cpblk;
        /// <summary>
        /// Copies the value type located at the address of an object (type &amp;, * or native int) to
        /// the address of the destination object (type &amp;, * or native int).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Cpobj;
        /// <summary>
        /// Divides two values and pushes the result as a floating-point (type F) or quotient (type int32)
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Div;
        /// <summary>
        /// Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Div_Un;
        /// <summary>
        /// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the
        /// evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Dup;
        /// <summary>
        /// Transfers control from the filter clause of an exception back to the Common Language Infrastructure
        /// (CLI) exception handler.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Endfilter;
        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to the Common
        /// Language Infrastructure (CLI) exception handler.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Endfinally;
        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Initblk;
        /// <summary>
        /// Initializes each field of the value type at a specified address to a null reference or a 0
        /// of the appropriate primitive type.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Initobj;
        /// <summary>
        /// Tests whether an object reference (type O) is an instance of a particular class.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Isinst;
        /// <summary>
        /// Exits current method and jumps to specified method.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Jmp;
        /// <summary>
        /// Loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg;
        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg_0;
        /// <summary>
        /// Loads the argument at index 1 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg_1;
        /// <summary>
        /// Loads the argument at index 2 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg_2;
        /// <summary>
        /// Loads the argument at index 3 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg_3;
        /// <summary>
        /// Loads the argument (referenced by a specified short form index) onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarg_S;
        /// <summary>
        /// Load an argument address onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarga;
        /// <summary>
        /// Load an argument address, in short form, onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldarga_S;
        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4;
        /// <summary>
        /// Pushes the integer value of 0 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_0;
        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_1;
        /// <summary>
        /// Pushes the integer value of 2 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_2;
        /// <summary>
        /// Pushes the integer value of 3 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_3;
        /// <summary>
        /// Pushes the integer value of 4 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_4;
        /// <summary>
        /// Pushes the integer value of 5 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_5;
        /// <summary>
        /// Pushes the integer value of 6 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_6;
        /// <summary>
        /// Pushes the integer value of 7 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_7;
        /// <summary>
        /// Pushes the integer value of 8 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_8;
        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_M1;
        /// <summary>
        /// Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I4_S;
        /// <summary>
        /// Pushes a supplied value of type int64 onto the evaluation stack as an int64.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_I8;
        /// <summary>
        /// Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_R4;
        /// <summary>
        /// Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldc_R8;
        /// <summary>
        /// Loads the element at a specified array index onto the top of the evaluation stack as the type
        /// specified in the instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem;
        /// <summary>
        /// Loads the element with type native int at a specified array index onto the top of the evaluation
        /// stack as a native int.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_I;
        /// <summary>
        /// Loads the element with type int8 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_I1;
        /// <summary>
        /// Loads the element with type int16 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_I2;
        /// <summary>
        /// Loads the element with type int32 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_I4;
        /// <summary>
        /// Loads the element with type int64 at a specified array index onto the top of the evaluation
        /// stack as an int64.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_I8;
        /// <summary>
        /// Loads the element with type float32 at a specified array index onto the top of the evaluation
        /// stack as type F (float).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_R4;
        /// <summary>
        /// Loads the element with type float64 at a specified array index onto the top of the evaluation
        /// stack as type F (float).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_R8;
        /// <summary>
        /// Loads the element containing an object reference at a specified array index onto the top of
        /// the evaluation stack as type O (object reference).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_Ref;
        /// <summary>
        /// Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_U1;
        /// <summary>
        /// Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_U2;
        /// <summary>
        /// Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation
        /// stack as an int32.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelem_U4;
        /// <summary>
        /// Loads the address of the array element at a specified array index onto the top of the evaluation
        /// stack as type &amp; (managed pointer).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldelema;
        /// <summary>
        /// Finds the value of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldfld;
        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldflda;
        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a specific method
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldftn;
        /// <summary>
        /// Loads a value of type native int as a native int onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_I;
        /// <summary>
        /// Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_I1;
        /// <summary>
        /// Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_I2;
        /// <summary>
        /// Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_I4;
        /// <summary>
        /// Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_I8;
        /// <summary>
        /// Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_R4;
        /// <summary>
        /// Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_R8;
        /// <summary>
        /// Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_Ref;
        /// <summary>
        /// Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_U1;
        /// <summary>
        /// Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_U2;
        /// <summary>
        /// Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldind_U4;
        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldlen;
        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc;
        /// <summary>
        /// Loads the local variable at index 0 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc_0;
        /// <summary>
        /// Loads the local variable at index 1 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc_1;
        /// <summary>
        /// Loads the local variable at index 2 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc_2;
        /// <summary>
        /// Loads the local variable at index 3 onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc_3;
        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack, short form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloc_S;
        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloca;
        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack, short
        /// form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldloca_S;
        /// <summary>
        /// Pushes a null reference (type O) onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldnull;
        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldobj;
        /// <summary>
        /// Pushes the value of a static field onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldsfld;
        /// <summary>
        /// Pushes the address of a static field onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldsflda;
        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldstr;
        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldtoken;
        /// <summary>
        /// Pushes an unmanaged pointer (type native int) to the native code implementing a particular
        /// virtual method associated with a specified object onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ldvirtftn;
        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a specific target
        /// instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Leave;
        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a target instruction
        /// (short form).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Leave_S;
        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address
        /// (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Localloc;
        /// <summary>
        /// Pushes a typed reference to an instance of a specific type onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Mkrefany;
        /// <summary>
        /// Multiplies two values and pushes the result on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Mul;
        /// <summary>
        /// Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation
        /// stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Mul_Ovf;
        /// <summary>
        /// Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto
        /// the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Mul_Ovf_Un;
        /// <summary>
        /// Negates a value and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Neg;
        /// <summary>
        /// Pushes an object reference to a new zero-based, one-dimensional array whose elements are of
        /// a specific type onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Newarr;
        /// <summary>
        /// Creates a new object or a new instance of a value type, pushing an object reference (type O)
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Newobj;
        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed although a processing
        /// cycle can be consumed.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Nop;
        /// <summary>
        /// Computes the bitwise complement of the integer value on top of the stack and pushes the result
        /// onto the evaluation stack as the same type.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Not;
        /// <summary>
        /// Compute the bitwise complement of the two integer values on top of the stack and pushes the
        /// result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Or;
        /// <summary>
        /// Removes the value currently on top of the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Pop;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix1;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix2;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix3;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix4;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix5;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix6;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefix7;
        /// <summary>
        /// This is a reserved instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Prefixref;
        /// <summary>
        /// Specifies that the subsequent array address operation performs no type check at run time, and
        /// that it returns a managed pointer whose mutability is restricted.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Readonly;
        /// <summary>
        /// Retrieves the type token embedded in a typed reference.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Refanytype;
        /// <summary>
        /// Retrieves the address (type &amp;) embedded in a typed reference.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Refanyval;
        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Rem;
        /// <summary>
        /// Divides two unsigned values and pushes the remainder onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Rem_Un;
        /// <summary>
        /// Returns from the current method, pushing a return value (if present) from the callee's evaluation
        /// stack onto the caller's evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Ret;
        /// <summary>
        /// Rethrows the current exception.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Rethrow;
        /// <summary>
        /// Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the
        /// result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Shl;
        /// <summary>
        /// Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Shr;
        /// <summary>
        /// Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing
        /// the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Shr_Un;
        /// <summary>
        /// Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Sizeof;
        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Starg;
        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index,
        /// short form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Starg_S;
        /// <summary>
        /// Replaces the array element at a given index with the value on the evaluation stack, whose type
        /// is specified in the instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem;
        /// <summary>
        /// Replaces the array element at a given index with the native int value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_I;
        /// <summary>
        /// Replaces the array element at a given index with the int8 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_I1;
        /// <summary>
        /// Replaces the array element at a given index with the int16 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_I2;
        /// <summary>
        /// Replaces the array element at a given index with the int32 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_I4;
        /// <summary>
        /// Replaces the array element at a given index with the int64 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_I8;
        /// <summary>
        /// Replaces the array element at a given index with the float32 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_R4;
        /// <summary>
        /// Replaces the array element at a given index with the float64 value on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_R8;
        /// <summary>
        /// Replaces the array element at a given index with the object ref value (type O) on the evaluation
        /// stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stelem_Ref;
        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stfld;
        /// <summary>
        /// Stores a value of type native int at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_I;
        /// <summary>
        /// Stores a value of type int8 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_I1;
        /// <summary>
        /// Stores a value of type int16 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_I2;
        /// <summary>
        /// Stores a value of type int32 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_I4;
        /// <summary>
        /// Stores a value of type int64 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_I8;
        /// <summary>
        /// Stores a value of type float32 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_R4;
        /// <summary>
        /// Stores a value of type float64 at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_R8;
        /// <summary>
        /// Stores a object reference value at a supplied address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stind_Ref;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at a specified index.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at index 0.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc_0;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at index 1.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc_1;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at index 2.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc_2;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at index 3.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc_3;
        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it in a the local variable
        /// list at <paramref name="index" /> (short form).
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stloc_S;
        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stobj;
        /// <summary>
        /// Replaces the value of a static field with a value from the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Stsfld;
        /// <summary>
        /// Subtracts one value from another and pushes the result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Sub;
        /// <summary>
        /// Subtracts one integer value from another, performs an overflow check, and pushes the result
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Sub_Ovf;
        /// <summary>
        /// Subtracts one unsigned integer value from another, performs an overflow check, and pushes the
        /// result onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Sub_Ovf_Un;
        /// <summary>
        /// Implements a jump table.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Switch;
        /// <summary>
        /// Performs a postfixed method call instruction such that the current method's stack frame is
        /// removed before the actual call instruction is executed.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Tailcall;
        /// <summary>
        /// Throws the exception object currently on the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Throw;
        /// <summary>
        /// Indicates that an address currently atop the evaluation stack might not be aligned to the natural
        /// size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk
        /// instruction.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Unaligned;
        /// <summary>
        /// Converts the boxed representation of a value type to its unboxed form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Unbox;
        /// <summary>
        /// Converts the boxed representation of a type specified in the instruction to its unboxed form.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Unbox_Any;
        /// <summary>
        /// Specifies that an address currently atop the evaluation stack might be volatile, and the results
        /// of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Volatile;
        /// <summary>
        /// Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result
        /// onto the evaluation stack.
        /// </summary>
        public static readonly System.Reflection.Emit.OpCode Xor;
        /// <summary>
        /// Returns true or false if the supplied opcode takes a single byte argument.
        /// </summary>
        /// <param name="inst">An instance of an Opcode object.</param>
        /// <returns>
        /// True or false.
        /// </returns>
        public static bool TakesSingleByteArgument(System.Reflection.Emit.OpCode inst) { return default(bool); }
    }
    /// <summary>
    /// Describes the types of the Microsoft intermediate language (MSIL) instructions.
    /// </summary>
    public enum OpCodeType
    {
        /// <summary>
        /// These are Microsoft intermediate language (MSIL) instructions that are used as a synonym for
        /// other MSIL instructions. For example, ldarg.0 represents the ldarg instruction with an argument of
        /// 0.
        /// </summary>
        Macro = 1,
        /// <summary>
        /// Describes a reserved Microsoft intermediate language (MSIL) instruction.
        /// </summary>
        Nternal = 2,
        /// <summary>
        /// Describes a Microsoft intermediate language (MSIL) instruction that applies to objects.
        /// </summary>
        Objmodel = 3,
        /// <summary>
        /// Describes a prefix instruction that modifies the behavior of the following instruction.
        /// </summary>
        Prefix = 4,
        /// <summary>
        /// Describes a built-in instruction.
        /// </summary>
        Primitive = 5,
    }
    /// <summary>
    /// Describes the operand type of Microsoft intermediate language (MSIL) instruction.
    /// </summary>
    public enum OperandType
    {
        /// <summary>
        /// The operand is a 32-bit integer branch target.
        /// </summary>
        InlineBrTarget = 0,
        /// <summary>
        /// The operand is a 32-bit metadata token.
        /// </summary>
        InlineField = 1,
        /// <summary>
        /// The operand is a 32-bit integer.
        /// </summary>
        InlineI = 2,
        /// <summary>
        /// The operand is a 64-bit integer.
        /// </summary>
        InlineI8 = 3,
        /// <summary>
        /// The operand is a 32-bit metadata token.
        /// </summary>
        InlineMethod = 4,
        /// <summary>
        /// No operand.
        /// </summary>
        InlineNone = 5,
        /// <summary>
        /// The operand is a 64-bit IEEE floating point number.
        /// </summary>
        InlineR = 7,
        /// <summary>
        /// The operand is a 32-bit metadata signature token.
        /// </summary>
        InlineSig = 9,
        /// <summary>
        /// The operand is a 32-bit metadata string token.
        /// </summary>
        InlineString = 10,
        /// <summary>
        /// The operand is the 32-bit integer argument to a switch instruction.
        /// </summary>
        InlineSwitch = 11,
        /// <summary>
        /// The operand is a FieldRef, MethodRef, or TypeRef token.
        /// </summary>
        InlineTok = 12,
        /// <summary>
        /// The operand is a 32-bit metadata token.
        /// </summary>
        InlineType = 13,
        /// <summary>
        /// The operand is 16-bit integer containing the ordinal of a local variable or an argument.
        /// </summary>
        InlineVar = 14,
        /// <summary>
        /// The operand is an 8-bit integer branch target.
        /// </summary>
        ShortInlineBrTarget = 15,
        /// <summary>
        /// The operand is an 8-bit integer.
        /// </summary>
        ShortInlineI = 16,
        /// <summary>
        /// The operand is a 32-bit IEEE floating point number.
        /// </summary>
        ShortInlineR = 17,
        /// <summary>
        /// The operand is an 8-bit integer containing the ordinal of a local variable or an argumenta.
        /// </summary>
        ShortInlineVar = 18,
    }
    /// <summary>
    /// Specifies one of two factors that determine the memory alignment of fields when a type is marshaled.
    /// </summary>
    public enum PackingSize
    {
        /// <summary>
        /// The packing size is 1 byte.
        /// </summary>
        Size1 = 1,
        /// <summary>
        /// The packing size is 128 bytes.
        /// </summary>
        Size128 = 128,
        /// <summary>
        /// The packing size is 16 bytes.
        /// </summary>
        Size16 = 16,
        /// <summary>
        /// The packing size is 2 bytes.
        /// </summary>
        Size2 = 2,
        /// <summary>
        /// The packing size is 32 bytes.
        /// </summary>
        Size32 = 32,
        /// <summary>
        /// The packing size is 4 bytes.
        /// </summary>
        Size4 = 4,
        /// <summary>
        /// The packing size is 64 bytes.
        /// </summary>
        Size64 = 64,
        /// <summary>
        /// The packing size is 8 bytes.
        /// </summary>
        Size8 = 8,
        /// <summary>
        /// The packing size is not specified.
        /// </summary>
        Unspecified = 0,
    }
    /// <summary>
    /// Describes how values are pushed onto a stack or popped off a stack.
    /// </summary>
    public enum StackBehaviour
    {
        /// <summary>
        /// No values are popped off the stack.
        /// </summary>
        Pop0 = 0,
        /// <summary>
        /// Pops one value off the stack.
        /// </summary>
        Pop1 = 1,
        /// <summary>
        /// Pops 1 value off the stack for the first operand, and 1 value of the stack for the second operand.
        /// </summary>
        Pop1_pop1 = 2,
        /// <summary>
        /// Pops a 32-bit integer off the stack.
        /// </summary>
        Popi = 3,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, and a value off the stack for the
        /// second operand.
        /// </summary>
        Popi_pop1 = 4,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, and a 32-bit integer off the stack
        /// for the second operand.
        /// </summary>
        Popi_popi = 5,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, a 32-bit integer off the stack for
        /// the second operand, and a 32-bit integer off the stack for the third operand.
        /// </summary>
        Popi_popi_popi = 7,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, and a 64-bit integer off the stack
        /// for the second operand.
        /// </summary>
        Popi_popi8 = 6,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, and a 32-bit floating point number
        /// off the stack for the second operand.
        /// </summary>
        Popi_popr4 = 8,
        /// <summary>
        /// Pops a 32-bit integer off the stack for the first operand, and a 64-bit floating point number
        /// off the stack for the second operand.
        /// </summary>
        Popi_popr8 = 9,
        /// <summary>
        /// Pops a reference off the stack.
        /// </summary>
        Popref = 10,
        /// <summary>
        /// Pops a reference off the stack for the first operand, and a value off the stack for the second
        /// operand.
        /// </summary>
        Popref_pop1 = 11,
        /// <summary>
        /// Pops a reference off the stack for the first operand, and a 32-bit integer off the stack for
        /// the second operand.
        /// </summary>
        Popref_popi = 12,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a 32-bit integer off the stack for the third operand.
        /// </summary>
        Popref_popi_pop1 = 28,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a value off the stack for the third operand.
        /// </summary>
        Popref_popi_popi = 13,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a 64-bit integer off the stack for the third operand.
        /// </summary>
        Popref_popi_popi8 = 14,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a 32-bit integer off the stack for the third operand.
        /// </summary>
        Popref_popi_popr4 = 15,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a 64-bit floating point number off the stack for the third operand.
        /// </summary>
        Popref_popi_popr8 = 16,
        /// <summary>
        /// Pops a reference off the stack for the first operand, a value off the stack for the second
        /// operand, and a reference off the stack for the third operand.
        /// </summary>
        Popref_popi_popref = 17,
        /// <summary>
        /// No values are pushed onto the stack.
        /// </summary>
        Push0 = 18,
        /// <summary>
        /// Pushes one value onto the stack.
        /// </summary>
        Push1 = 19,
        /// <summary>
        /// Pushes 1 value onto the stack for the first operand, and 1 value onto the stack for the second
        /// operand.
        /// </summary>
        Push1_push1 = 20,
        /// <summary>
        /// Pushes a 32-bit integer onto the stack.
        /// </summary>
        Pushi = 21,
        /// <summary>
        /// Pushes a 64-bit integer onto the stack.
        /// </summary>
        Pushi8 = 22,
        /// <summary>
        /// Pushes a 32-bit floating point number onto the stack.
        /// </summary>
        Pushr4 = 23,
        /// <summary>
        /// Pushes a 64-bit floating point number onto the stack.
        /// </summary>
        Pushr8 = 24,
        /// <summary>
        /// Pushes a reference onto the stack.
        /// </summary>
        Pushref = 25,
        /// <summary>
        /// Pops a variable off the stack.
        /// </summary>
        Varpop = 26,
        /// <summary>
        /// Pushes a variable onto the stack.
        /// </summary>
        Varpush = 27,
    }
}
