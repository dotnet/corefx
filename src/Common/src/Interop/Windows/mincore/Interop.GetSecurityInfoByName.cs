// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class mincore
    {
        [DllImport(Interop.Libraries.SecurityProvider, EntryPoint = "GetNamedSecurityInfoW", CallingConvention = CallingConvention.Winapi, 
            SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern /*DWORD*/ uint GetSecurityInfoByName(string name, /*DWORD*/ uint objectType, /*DWORD*/ uint securityInformation,
            out IntPtr sidOwner,out IntPtr sidGroup, out IntPtr dacl, out IntPtr sacl, out IntPtr securityDescriptor);
    }
}
