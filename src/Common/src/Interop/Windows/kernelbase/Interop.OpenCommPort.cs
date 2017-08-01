// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class KernelBase
    {
        [DllImport(Libraries.KernelBase, SetLastError = true)]
        internal static extern SafeFileHandle OpenCommPort(
             ulong portNumber,
             int dwDesiredAccess,
             int dwFlagsAndAttributes);
    }
}
