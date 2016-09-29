// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecvFrom(
            [In] SafeCloseSocket socketHandle,
            [In] ref WSABuffer buffer,
            [In] int bufferCount,
            [Out] out int bytesTransferred,
            [In, Out] ref SocketFlags socketFlags,
            [In] IntPtr socketAddressPointer,
            [In] IntPtr socketAddressSizePointer,
            [In] SafeHandle overlapped,
            [In] IntPtr completionRoutine);

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecvFrom(
            [In] SafeCloseSocket socketHandle,
            [In, Out] WSABuffer[] buffers,
            [In] int bufferCount,
            [Out] out int bytesTransferred,
            [In, Out] ref SocketFlags socketFlags,
            [In] IntPtr socketAddressPointer,
            [In] IntPtr socketAddressSizePointer,
            [In] SafeNativeOverlapped overlapped,
            [In] IntPtr completionRoutine);
    }
}
