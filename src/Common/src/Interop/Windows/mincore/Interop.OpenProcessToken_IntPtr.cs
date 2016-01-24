// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System;
using System.Security.Principal;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.ProcessThread_L1, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool OpenProcessToken(
        IntPtr ProcessToken,
        TokenAccessLevels DesiredAccess,
        out SafeTokenHandle TokenHandle);
    }
}
