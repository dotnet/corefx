// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Threading;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.CoreFile_L2, EntryPoint = "ReadDirectoryChangesW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern bool ReadDirectoryChangesW(
            SafeFileHandle hDirectory, 
            byte[] lpBuffer, 
            int nBufferLength, 
            [MarshalAs(UnmanagedType.Bool)] bool bWatchSubtree, 
            int dwNotifyFilter, 
            out int lpBytesReturned,
            NativeOverlapped* lpOverlapped, 
            IntPtr lpCompletionRoutine);
    }
}
