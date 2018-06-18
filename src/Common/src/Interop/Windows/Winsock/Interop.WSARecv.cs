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
        private static unsafe extern SocketError WSARecv(
            IntPtr socketHandle,
            ref WSABuffer buffer,
            int bufferCount,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine);

        internal static unsafe SocketError WSARecvSingle(
            IntPtr socketHandle,
            ref WSABuffer buffer,
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine)
        {
            // We intentionally do NOT copy this back after the function completes:
            // We don't want to cause a race in async scenarios.
            // The WSABuffer struct should be unchanged anyway.
            WSABuffer localBuffer = buffer;
            return WSARecv(socketHandle, ref localBuffer, 1, out bytesTransferred, ref socketFlags, overlapped, completionRoutine);
        }

        internal static unsafe SocketError WSARecv(
            IntPtr socketHandle,
            Span<WSABuffer> buffers,
            int bufferCount, // this is *not* necessarily #the same as buffers.Length; the field on SocketAsyncEventArgs can be over-sized
            out int bytesTransferred,
            ref SocketFlags socketFlags,
            NativeOverlapped* overlapped,
            IntPtr completionRoutine)
        {
            Debug.Assert(!buffers.IsEmpty);
            return WSARecv(socketHandle, ref MemoryMarshal.GetReference(buffers), bufferCount, out bytesTransferred, ref socketFlags, overlapped, completionRoutine);
        }
    }
}
