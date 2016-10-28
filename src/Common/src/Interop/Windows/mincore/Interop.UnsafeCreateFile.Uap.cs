// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class mincore
    {
        internal static unsafe SafeFileHandle UnsafeCreateFile(
            string lpFileName,
            int dwDesiredAccess,
            System.IO.FileShare dwShareMode,
            ref Interop.mincore.SECURITY_ATTRIBUTES securityAttrs,
            System.IO.FileMode dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile)
        {
            Interop.mincore.CREATEFILE2_EXTENDED_PARAMETERS parameters;
            parameters.dwSize = (uint)Marshal.SizeOf<Interop.mincore.CREATEFILE2_EXTENDED_PARAMETERS>();

            parameters.dwFileAttributes = (uint)dwFlagsAndAttributes & 0x0000FFFF;
            parameters.dwSecurityQosFlags = (uint)dwFlagsAndAttributes & 0x000F0000;
            parameters.dwFileFlags = (uint)dwFlagsAndAttributes & 0xFFF00000;

            parameters.hTemplateFile = hTemplateFile;
            fixed (Interop.mincore.SECURITY_ATTRIBUTES* lpSecurityAttributes = &securityAttrs)
            {
                parameters.lpSecurityAttributes = (IntPtr)lpSecurityAttributes;
                return Interop.mincore.CreateFile2(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, ref parameters);
            }
        }
    }
}
