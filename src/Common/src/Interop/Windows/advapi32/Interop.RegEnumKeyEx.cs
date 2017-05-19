// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class Advapi32
    {
        [DllImport(Libraries.Advapi32, CharSet = CharSet.Unicode, BestFitMapping = false, EntryPoint = "RegEnumKeyExW")]
        internal static extern unsafe int RegEnumKeyEx(
            SafeRegistryHandle hKey,
            int dwIndex,
            char[] lpName,
            ref int lpcbName,
            int[] lpReserved,
            [Out]StringBuilder lpClass,
            int[] lpcbClass,
            long[] lpftLastWriteTime);
    }
}
