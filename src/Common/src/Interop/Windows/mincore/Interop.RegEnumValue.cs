// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Registry_L1, CharSet = CharSet.Unicode, BestFitMapping = false, EntryPoint = "RegEnumValueW")]
        internal unsafe static extern int RegEnumValue(
            SafeRegistryHandle hKey,
            int dwIndex,
            char* lpValueName,
            ref int lpcbValueName,
            IntPtr lpReserved_MustBeZero,
            int[] lpType,
            byte[] lpData,
            int[] lpcbData);
    }
}
