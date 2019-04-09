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
        internal static class CommErrors
        {
            internal const int CE_RXOVER = 0x01;
            internal const int CE_OVERRUN = 0x02;
            internal const int CE_PARITY = 0x04;
            internal const int CE_FRAME = 0x08;
            internal const int CE_TXFULL = 0x100;
        }

        [DllImport(Libraries.Kernel32, SetLastError=true, CharSet=CharSet.Auto)]
        internal static extern bool ClearCommError(
            SafeFileHandle hFile,
            ref int lpErrors,
            ref COMSTAT lpStat);

        [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool ClearCommError(
            SafeFileHandle hFile,
            ref int lpErrors,
            IntPtr lpStat);
    }
}