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
        internal static extern unsafe int WSARecv(
            SafeCloseSocket socketHandle,
            WSABuffer* buffers,
            int bufferCount,
            int* bytesTransferred,
            SocketFlags* socketFlags,
            SafeNativeOverlapped overlapped,
            void* completionRoutine);

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true, EntryPoint = "WSARecv")]
        internal static extern unsafe int WSARecv_Blocking(
            IntPtr socketHandle,
            WSABuffer* buffers,
            int bufferCount,
            int* bytesTransferred,
            SocketFlags* socketFlags,
            SafeNativeOverlapped overlapped,
            void* completionRoutine);
    }
}
