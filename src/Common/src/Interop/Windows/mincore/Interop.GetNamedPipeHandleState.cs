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
        [DllImport(Libraries.Pipe_L2, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, EntryPoint = "GetNamedPipeHandleStateW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeHandleState(
            SafePipeHandle hNamedPipe,
            out int lpState,
            IntPtr lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            IntPtr lpUserName,
            int nMaxUserNameSize);

        [DllImport(Libraries.Pipe_L2, CharSet = CharSet.Unicode, SetLastError = true, BestFitMapping = false, EntryPoint = "GetNamedPipeHandleStateW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeHandleState(
            SafePipeHandle hNamedPipe,
            IntPtr lpState,
            IntPtr lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            [Out] StringBuilder lpUserName,
            int nMaxUserNameSize);

        [DllImport(Libraries.Pipe_L2, SetLastError = true, EntryPoint="GetNamedPipeHandleStateW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetNamedPipeHandleState(
            SafePipeHandle hNamedPipe,
            IntPtr lpState,
            out int lpCurInstances,
            IntPtr lpMaxCollectionCount,
            IntPtr lpCollectDataTimeout,
            IntPtr lpUserName,
            int nMaxUserNameSize);

    }
}
