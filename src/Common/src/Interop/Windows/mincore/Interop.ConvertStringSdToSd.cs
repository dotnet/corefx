// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class mincore
    {
        [DllImport(Interop.Libraries.SecuritySddl, EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW",
            CallingConvention = CallingConvention.Winapi, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int ConvertStringSdToSd(
            string stringSd,
            /* DWORD */ uint stringSdRevision, 
            out IntPtr resultSd,
            ref uint resultSdLength);
    }
}
