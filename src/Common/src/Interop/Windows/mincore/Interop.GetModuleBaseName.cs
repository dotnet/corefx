﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Psapi_Obsolete, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, EntryPoint = "K32GetModuleBaseNameW")]
        internal static extern int GetModuleBaseName(SafeProcessHandle processHandle, IntPtr moduleHandle, [Out] StringBuilder baseName, int size);
    }
}
