// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal unsafe partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorNew")]
        internal static extern IntPtr MonitorNew();

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorDelete")]
        internal static extern void MonitorDelete(IntPtr monitor);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorAcquire")]
        internal static extern void MonitorAcquire(IntPtr mutex);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorRelease")]
        internal static extern void MonitorRelease(IntPtr mutex);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorWait")]
        internal static extern void MonitorWait(IntPtr monitor);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorTimedWait")]
        internal static extern bool MonitorTimedWait(IntPtr monitor, int timeoutMilliseconds);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_MonitorSignalAndRelease")]
        internal static extern void MonitorSignalAndRelease(IntPtr monitor);
   }
}