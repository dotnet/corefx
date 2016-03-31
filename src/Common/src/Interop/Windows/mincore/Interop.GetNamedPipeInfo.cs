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
        [DllImport(Libraries.Pipe_L2, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeInfo(
            SafePipeHandle hNamedPipe,
            out int lpFlags,
            IntPtr lpOutBufferSize,
            IntPtr lpInBufferSize,
            IntPtr lpMaxInstances
        );

        [DllImport(Libraries.Pipe_L2, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeInfo(
            SafePipeHandle hNamedPipe,
            IntPtr lpFlags,
            out int lpOutBufferSize,
            IntPtr lpInBufferSize,
            IntPtr lpMaxInstances
        );

        [DllImport(Libraries.Pipe_L2, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeInfo(
            SafePipeHandle hNamedPipe,
            IntPtr lpFlags,
            IntPtr lpOutBufferSize,
            out int lpInBufferSize,
            IntPtr lpMaxInstances
        );
    }
}
