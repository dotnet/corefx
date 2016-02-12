// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

//    For changes please talk to WesH or ImmoL.

namespace System.Runtime.CompilerServices
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvCdecl
    {
        internal CallConvCdecl() { }
    }
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvFastcall
    {
        internal CallConvFastcall() { }
    }
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvStdcall
    {
        internal CallConvStdcall() { }
    }
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class CallConvThiscall
    {
        internal CallConvThiscall() { }
    }
    public static partial class IsBoxed
    {
    }
    public static partial class IsByValue
    {
    }
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public static partial class IsCopyConstructed
    {
    }
    public static partial class IsExplicitlyDereferenced
    {
    }
    public static partial class IsImplicitlyDereferenced
    {
    }
    public static partial class IsJitIntrinsic
    {
    }
    public static partial class IsLong
    {
    }
    public static partial class IsSignUnspecifiedByte
    {
    }
    public static partial class IsUdtReturn
    {
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(8), Inherited = true)]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public sealed partial class NativeCppClassAttribute : System.Attribute
    {
        public NativeCppClassAttribute() { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1052), AllowMultiple = true, Inherited = false)]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public sealed partial class RequiredAttributeAttribute : System.Attribute
    {
        public RequiredAttributeAttribute(System.Type requiredContract) { }
        public System.Type RequiredContract { get { return default(System.Type); } }
    }
}
