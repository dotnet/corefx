﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class mincore
    {
        [DllImport(Libraries.ThreadPool, SetLastError = true)]
        internal static unsafe extern SafeThreadPoolIOHandle CreateThreadpoolIo(SafeHandle fl, [MarshalAs(UnmanagedType.FunctionPtr)] NativeIoCompletionCallback pfnio, IntPtr context, IntPtr pcbe);

        [DllImport(Libraries.ThreadPool)]
        internal static unsafe extern void CloseThreadpoolIo(IntPtr pio);

        [DllImport(Libraries.ThreadPool)]
        internal static unsafe extern void StartThreadpoolIo(SafeThreadPoolIOHandle pio);

        [DllImport(Libraries.ThreadPool)]
        internal static unsafe extern void CancelThreadpoolIo(SafeThreadPoolIOHandle pio);
    }

    internal delegate void NativeIoCompletionCallback(IntPtr instance, IntPtr context, IntPtr overlapped, uint ioResult, UIntPtr numberOfBytesTransferred, IntPtr io);
}
