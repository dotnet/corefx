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
        internal static unsafe extern SocketError WSARecv(
            SafeCloseSocket socketHandle,
            WSABuffer* buffer,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            SafeNativeOverlapped overlapped,
            IntPtr completionRoutine);

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static unsafe extern SocketError WSARecv(
            IntPtr socketHandle,
            WSABuffer* buffer,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            SafeNativeOverlapped overlapped,
            IntPtr completionRoutine);

        internal static unsafe SocketError WSARecv(
            SafeCloseSocket socketHandle,
            ref WSABuffer buffer,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            SafeNativeOverlapped overlapped,
            IntPtr completionRoutine)
        {
            WSABuffer localBuffer = buffer;
            return WSARecv(socketHandle, &localBuffer, bufferCount, out bytesTransferred, ref socketFlags, overlapped, completionRoutine);
        }

        internal static unsafe SocketError WSARecv(
            SafeCloseSocket socketHandle,
            WSABuffer[] buffers,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            SafeNativeOverlapped overlapped,
            IntPtr completionRoutine)
        {
            fixed (WSABuffer* buffersPtr = &buffers[0])
            { 
                return WSARecv(socketHandle, buffersPtr, bufferCount, out bytesTransferred, ref socketFlags, overlapped, completionRoutine);
            }
        }

        internal static unsafe SocketError WSARecv(
            IntPtr socketHandle,
            WSABuffer[] buffers,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            SafeNativeOverlapped overlapped,
            IntPtr completionRoutine)
        {
            fixed (WSABuffer* buffersPtr = &buffers[0])
            {
                return WSARecv(socketHandle, buffersPtr, bufferCount, out bytesTransferred, ref socketFlags, overlapped, completionRoutine);
            }
        }
    }
}
