// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, SetLastError=true, CharSet=CharSet.Auto)]
        internal static extern unsafe bool GetOverlappedResult(
            SafeFileHandle hFile,
            NativeOverlapped* lpOverlapped,
            ref int lpNumberOfBytesTransferred,
            bool bWait);
    }
}
