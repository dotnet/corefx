// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

#if REGISTRY_ASSEMBLY
using Microsoft.Win32.SafeHandles;
#else
using Internal.Win32.SafeHandles;
#endif

internal partial class Interop
{
    internal partial class Advapi32
    {
        [DllImport(Libraries.Advapi32, CharSet = CharSet.Unicode, BestFitMapping = false, EntryPoint = "RegConnectRegistryW")]
        internal static extern int RegConnectRegistry(string machineName, SafeRegistryHandle key, out SafeRegistryHandle result);
    }
}
