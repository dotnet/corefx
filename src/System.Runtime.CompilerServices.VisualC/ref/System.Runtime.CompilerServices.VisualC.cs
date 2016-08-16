// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

//    For changes please talk to WesH or ImmoL.

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Indicates that a method should use the Cdecl calling convention.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvCdecl
    {
        internal CallConvCdecl() { }
    }
    /// <summary>
    /// This calling convention is not supported in this version of the .NET Framework.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvFastcall
    {
        internal CallConvFastcall() { }
    }
    /// <summary>
    /// Indicates that a method should use the StdCall calling convention.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvStdcall
    {
        internal CallConvStdcall() { }
    }
    /// <summary>
    /// Indicates that a method should use the ThisCall calling convention.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvThiscall
    {
        internal CallConvThiscall() { }
    }
    /// <summary>
    /// Indicates that the modified reference type is a boxed value type.
    /// </summary>
    public static partial class IsBoxed
    {
    }
    /// <summary>
    /// Indicates that a modified method argument should be interpreted as having object passed-by-value
    /// semantics. This modifier is applied to reference types.
    /// </summary>
    public static partial class IsByValue
    {
    }
    /// <summary>
    /// Indicates that any copying of values of this type must use the copy constructor provided by
    /// the type.
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public static partial class IsCopyConstructed
    {
    }
    /// <summary>
    /// Indicates that a managed pointer represents a pointer parameter within a method signature.
    /// </summary>
    public static partial class IsExplicitlyDereferenced
    {
    }
    /// <summary>
    /// Indicates that the modified garbage collection reference represents a reference parameter within
    /// a method signature.
    /// </summary>
    public static partial class IsImplicitlyDereferenced
    {
    }
    /// <summary>
    /// Indicates that a modified method is an intrinsic value for which the just-in-time (JIT) compiler
    /// can perform special code generation.
    /// </summary>
    public static partial class IsJitIntrinsic
    {
    }
    /// <summary>
    /// Indicates that a modified integer is a standard C++ long value.
    /// </summary>
    public static partial class IsLong
    {
    }
    /// <summary>
    /// Indicates that a modifier is neither signed nor unsigned.
    /// </summary>
    public static partial class IsSignUnspecifiedByte
    {
    }
    /// <summary>
    /// Indicates that a return type is a user-defined type.
    /// </summary>
    public static partial class IsUdtReturn
    {
    }
    /// <summary>
    /// Applies metadata to an assembly that indicates that a type is an unmanaged type. This class
    /// cannot be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(8), Inherited = true)]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public sealed partial class NativeCppClassAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeCppClassAttribute" />
        /// class.
        /// </summary>
        public NativeCppClassAttribute() { }
    }
    /// <summary>
    /// Specifies that an importing compiler must fully understand the semantics of a type definition,
    /// or refuse to use it.  This class cannot be inherited.
    /// </summary>
    [System.AttributeUsageAttribute((System.AttributeTargets)(1052), AllowMultiple = true, Inherited = false)]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public sealed partial class RequiredAttributeAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAttributeAttribute" />
        /// class.
        /// </summary>
        /// <param name="requiredContract">
        /// A type that an importing compiler must fully understand.This parameter is not supported in
        /// the .NET Framework version 2.0 and later.
        /// </param>
        public RequiredAttributeAttribute(System.Type requiredContract) { }
        /// <summary>
        /// Gets a type that an importing compiler must fully understand.
        /// </summary>
        /// <returns>
        /// A type that an importing compiler must fully understand.
        /// </returns>
        public System.Type RequiredContract { get { return default(System.Type); } }
    }
}
