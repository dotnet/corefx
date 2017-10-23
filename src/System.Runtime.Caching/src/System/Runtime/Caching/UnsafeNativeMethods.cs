// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Runtime.Caching
{
    [SuppressUnmanagedCodeSecurity]
    //[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage", Justification = "Grandfathered suppression from original caching code checkin")]
    [SecurityCritical]
    internal static class UnsafeNativeMethods
    {
        private const string KERNEL32 = "KERNEL32.DLL";

        /*
         * KERNEL32.DLL
         */
        [DllImport(KERNEL32, CharSet = CharSet.Unicode)]
        internal extern static int GlobalMemoryStatusEx(ref MEMORYSTATUSEX memoryStatusEx);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MEMORYSTATUSEX
    {
        internal int dwLength;
        internal int dwMemoryLoad;
        internal long ullTotalPhys;
        internal long ullAvailPhys;
        internal long ullTotalPageFile;
        internal long ullAvailPageFile;
        internal long ullTotalVirtual;
        internal long ullAvailVirtual;
        internal long ullAvailExtendedVirtual;
        internal void Init()
        {
            dwLength = Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }
}
