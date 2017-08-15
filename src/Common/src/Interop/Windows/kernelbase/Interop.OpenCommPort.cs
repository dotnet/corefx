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
// Disable Pinvoke analyzer errors, needed until buildtools can be updated to remove kernel32.dll!OpenCommPort
// Tracking issue https://github.com/dotnet/corefx/issues/23264
#pragma warning disable BCL0015
        [DllImport(Libraries.KernelBase, SetLastError = true)]
        internal static extern SafeFileHandle OpenCommPort(
             uint uPortNumber,
             int dwDesiredAccess,
             int dwFlagsAndAttributes);
#pragma warning restore BCL0015
    }
}
