// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Psapi_Obsolete, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "K32GetModuleInformation")]
        internal static extern bool GetModuleInformation(SafeProcessHandle processHandle, IntPtr moduleHandle, NtModuleInfo ntModuleInfo, int size);

        [StructLayout(LayoutKind.Sequential)]
        internal class NtModuleInfo
        {
            internal IntPtr BaseOfDll = (IntPtr)0;
            internal int SizeOfImage = 0;
            internal IntPtr EntryPoint = (IntPtr)0;
        }
    }
}
