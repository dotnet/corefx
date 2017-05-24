// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        private static unsafe extern SocketError WSARecvFrom(
            IntPtr socketHandle,
            WSABuffer* buffers,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            IntPtr socketAddressPointer,
            IntPtr socketAddressSizePointer,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine);

        internal static unsafe SocketError WSARecvFrom(
            IntPtr socketHandle,
            ref WSABuffer buffer,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            IntPtr socketAddressPointer,
            IntPtr socketAddressSizePointer,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine)
        {
            // We intentionally do NOT copy this back after the function completes:
            // We don't want to cause a race in async scenarios.
            // The WSABuffer struct should be unchanged anyway.
            WSABuffer localBuffer = buffer;
            return WSARecvFrom(socketHandle, &localBuffer, bufferCount, out bytesTransferred, ref socketFlags, socketAddressPointer, socketAddressSizePointer, overlapped, completionRoutine);
        }

        internal static unsafe SocketError WSARecvFrom(
            IntPtr socketHandle,
            WSABuffer[] buffers,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            IntPtr socketAddressPointer,
            IntPtr socketAddressSizePointer,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine)
        {
            Debug.Assert(buffers != null && buffers.Length > 0);
            fixed (WSABuffer* buffersPtr = &buffers[0])
            {
                return WSARecvFrom(socketHandle, buffersPtr, bufferCount, out bytesTransferred, ref socketFlags, socketAddressPointer, socketAddressSizePointer, overlapped, completionRoutine);
            }
        }
    }
}
