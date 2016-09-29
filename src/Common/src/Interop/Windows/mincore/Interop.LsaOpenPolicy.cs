// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class mincore
    {
        [DllImport(Interop.Libraries.SecurityLsaPolicy, EntryPoint = "LsaOpenPolicy", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint LsaOpenPolicy(string systemName, ref LSA_OBJECT_ATTRIBUTES attributes, int accessMask, out SafeLsaPolicyHandle handle);
    }
}
