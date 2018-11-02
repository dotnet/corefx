// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Serial
    {
        [DllImport(Libraries.IOPortsNative, EntryPoint = "SystemIoPortsNative_SerialPortOpen", SetLastError = true)]
        internal static extern SafeSerialDeviceHandle SerialPortOpen(string name);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Read", SetLastError = true)]
        internal static extern unsafe int Read(SafeSerialDeviceHandle fd, byte* buffer, int count);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Write", SetLastError = true)]
        internal static extern unsafe int Write(SafeSerialDeviceHandle fd, byte* buffer, int bufferSize);

        // Following APIs need to take IntPtr as argument since they are called during disposing
        // this is to prevent adding reference and getting ObjectDisposedException

        [DllImport(Libraries.IOPortsNative, EntryPoint = "SystemIoPortsNative_SerialPortClose", SetLastError = true)]
        internal static extern int SerialPortClose(IntPtr handle);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Shutdown")]
        internal static extern Error Shutdown(IntPtr socket, SocketShutdown how);
    }
}
