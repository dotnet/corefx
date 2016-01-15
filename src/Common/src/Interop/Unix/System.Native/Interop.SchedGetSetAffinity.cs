// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_SchedSetAffinity", SetLastError = true)]
        internal static extern int SchedSetAffinity(int pid, ref IntPtr mask);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_SchedGetAffinity", SetLastError = true)]
        internal static extern int SchedGetAffinity(int pid, out IntPtr mask);
    }
}
