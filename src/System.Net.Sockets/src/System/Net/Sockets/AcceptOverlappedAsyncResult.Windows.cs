// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace System.Net.Sockets
{
    // AcceptOverlappedAsyncResult - used to take care of storage for async Socket BeginAccept call.
    internal partial class AcceptOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        private Socket _acceptSocket;
        private int _addressBufferLength;

        // This method will be called by us when the IO completes synchronously and
        // by the ThreadPool when the IO completes asynchronously. (only called on WinNT)
        internal override object PostCompletion(int numBytes)
        {
            SocketError errorCode = (SocketError)ErrorCode;

            Internals.SocketAddress remoteSocketAddress = null;
            if (errorCode == SocketError.Success)
            {
                _localBytesTransferred = numBytes;
                if (Logging.On)
                {
                    LogBuffer((long)numBytes);
                }

                // get the endpoint
                remoteSocketAddress = IPEndPointExtensions.Serialize(_listenSocket._rightEndPoint);

                IntPtr localAddr;
                int localAddrLength;
                IntPtr remoteAddr;

                // set the socket context
                try
                {
                    _listenSocket.GetAcceptExSockaddrs(
                        Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0),
                        _buffer.Length - (_addressBufferLength * 2),
                        _addressBufferLength,
                        _addressBufferLength,
                        out localAddr,
                        out localAddrLength,
                        out remoteAddr,
                        out remoteSocketAddress.InternalSize);

                    Marshal.Copy(remoteAddr, remoteSocketAddress.Buffer, 0, remoteSocketAddress.Size);

                    IntPtr handle = _listenSocket.SafeHandle.DangerousGetHandle();

                    errorCode = Interop.Winsock.setsockopt(
                        _acceptSocket.SafeHandle,
                        SocketOptionLevel.Socket,
                        SocketOptionName.UpdateAcceptContext,
                        ref handle,
                        Marshal.SizeOf(handle));

                    if (errorCode == SocketError.SocketError)
                    {
                        errorCode = (SocketError)Marshal.GetLastWin32Error();
                    }
                    GlobalLog.Print("AcceptOverlappedAsyncResult#" + Logging.HashString(this) + "::PostCallback() setsockopt handle:" + handle.ToString() + " AcceptSocket:" + Logging.HashString(_acceptSocket) + " itsHandle:" + _acceptSocket.SafeHandle.DangerousGetHandle().ToString() + " returns:" + errorCode.ToString());
                }
                catch (ObjectDisposedException)
                {
                    errorCode = SocketError.OperationAborted;
                }

                ErrorCode = (int)errorCode;
            }

            if (errorCode != SocketError.Success)
            {
                return null;
            }

            return _listenSocket.UpdateAcceptSocket(_acceptSocket, _listenSocket._rightEndPoint.Create(remoteSocketAddress));
        }

        // SetUnmanagedStructures
        //
        // This method fills in overlapped structures used in an asynchronous 
        // overlapped Winsock call. These calls are outside the runtime and are
        // unmanaged code, so we need to prepare specific structures and ints that
        // lie in unmanaged memory since the overlapped calls may complete asynchronously.
        internal void SetUnmanagedStructures(byte[] buffer, int addressBufferLength)
        {
            // has to be called first to pin memory
            base.SetUnmanagedStructures(buffer);

            // Fill in Buffer Array structure that will be used for our send/recv Buffer
            _addressBufferLength = addressBufferLength;
            _buffer = buffer;
        }

        private void LogBuffer(long size)
        {
            GlobalLog.Assert(Logging.On, "AcceptOverlappedAsyncResult#{0}::LogBuffer()|Logging is off!", Logging.HashString(this));
            IntPtr pinnedBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, 0);
            if (pinnedBuffer != IntPtr.Zero)
            {
                if (size > -1)
                {
                    Logging.Dump(Logging.Sockets, _listenSocket, "PostCompletion", pinnedBuffer, (int)Math.Min(size, (long)_buffer.Length));
                }
                else
                {
                    Logging.Dump(Logging.Sockets, _listenSocket, "PostCompletion", pinnedBuffer, (int)_buffer.Length);
                }
            }
        }

        internal Socket AcceptSocket
        {
            set
            {
                _acceptSocket = value;
            }
        }
    }
}
