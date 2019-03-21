// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//// <summary>
//// This is intended to be a template for starting a new component with Bid.
//// </summary>
//
//using System;
//using System.Text;
//using System.Security;
//using System.Reflection;
//using System.Security.Permissions;
//using System.Runtime.InteropServices;
//using System.Runtime.Versioning;

//[module: BidIdentity("MyAssemblyName.1")]
//[module: BidMetaText(":FormatControl: InstanceID='' ")]

//internal static partial class Bid
//{
//    private const string dllName = "BidLdr.dll";

//    //
//    //  Strongly Typed Overloads example.
//    //  Use SignatureGenerator or edit manually in order to create actual set of overloads.
//    //
//    internal static void Trace(string fmtPrintfW, System.Int32 a1) {
//        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
//            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1);
//    }

//    internal static void TraceEx(uint flags, string fmtPrintfW, System.Int32 a1) {
//        if (modID != NoData)
//            NativeMethods.Trace (modID, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW,a1);
//    }

//    //
//    //  Interop calls to pluggable hooks [SuppressUnmanagedCodeSecurity] applied
//    //
//    private static partial class NativeMethods
//    {
//        [ResourceExposure(ResourceScope.None)]
//        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
//        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1);
//    } // Native
//} // Bid

