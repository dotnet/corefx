// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, EntryPoint = "CreateFile2", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        internal unsafe static extern SafeFileHandle CreateFile2(
            string lpFileName,
            int dwDesiredAccess,
            System.IO.FileShare dwShareMode,
            System.IO.FileMode dwCreationDisposition,
            [In] ref CREATEFILE2_EXTENDED_PARAMETERS parameters);

        private static unsafe SafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            System.IO.FileShare dwShareMode,
            Interop.Kernel32.SECURITY_ATTRIBUTES* securityAttrs,
            System.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            Interop.Kernel32.CREATEFILE2_EXTENDED_PARAMETERS parameters;
            parameters.dwSize = (uint)Marshal.SizeOf<Interop.Kernel32.CREATEFILE2_EXTENDED_PARAMETERS>();

            parameters.dwFileAttributes = (uint)dwFlagsAndAttributes & 0x0000FFFF;
            parameters.dwSecurityQosFlags = (uint)dwFlagsAndAttributes & 0x000F0000;
            parameters.dwFileFlags = (uint)dwFlagsAndAttributes & 0xFFF00000;

            parameters.hTemplateFile = hTemplateFile;
            parameters.lpSecurityAttributes = securityAttrs;
            
            return Interop.Kernel32.CreateFile2(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, ref parameters);
        }

        internal static unsafe SafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            System.IO.FileShare dwShareMode,
            ref Interop.Kernel32.SECURITY_ATTRIBUTES securityAttrs,
            System.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            fixed (Interop.Kernel32.SECURITY_ATTRIBUTES* lpSecurityAttributes = &securityAttrs)
            {
                return CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
            }
        }

        internal static unsafe SafeFileHandle CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            System.IO.FileShare dwShareMode,
            System.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes)
        {
            return CreateFile(lpFileName, dwDesiredAccess, dwShareMode, null, dwCreationDisposition, dwFlagsAndAttributes, IntPtr.Zero);
        }
    }
}
