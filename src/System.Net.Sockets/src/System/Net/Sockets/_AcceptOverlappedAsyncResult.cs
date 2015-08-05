// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace System.Net.Sockets
{
    //
    //  AcceptOverlappedAsyncResult - used to take care of storage for async Socket BeginAccept call.
    //
    internal class AcceptOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        //
        // internal class members
        //

        private int _localBytesTransferred;
        private Socket _listenSocket;
        private Socket _acceptSocket;

        private int _addressBufferLength;
        private byte[] _buffer;

        // Constructor. We take in the socket that's creating us, the caller's
        // state object, and the buffer on which the I/O will be performed.
        // We save the socket and state, pin the callers's buffer, and allocate
        // an event for the WaitHandle.
        //
        internal AcceptOverlappedAsyncResult(Socket listenSocket, Object asyncState, AsyncCallback asyncCallback) :
            base(listenSocket, asyncState, asyncCallback)
        {
            _listenSocket = listenSocket;
        }

#if !FEATURE_PAL

        //
        // This method will be called by us when the IO completes synchronously and
        // by the ThreadPool when the IO completes asynchronously. (only called on WinNT)
        //

        internal override object PostCompletion(int numBytes)
        {
            SocketError errorCode = (SocketError)ErrorCode;

            Internals.SocketAddress remoteSocketAddress = null;
            if (errorCode == SocketError.Success)
            {
                _localBytesTransferred = numBytes;
                if (Logging.On) LogBuffer((long)numBytes);

                //get the endpoint

                remoteSocketAddress = IPEndPointExtensions.Serialize(_listenSocket.m_RightEndPoint);

                IntPtr localAddr;
                int localAddrLength;
                IntPtr remoteAddr;

                //set the socket context
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
                                    out remoteSocketAddress.InternalSize
                                    );
                    Marshal.Copy(remoteAddr, remoteSocketAddress.Buffer, 0, remoteSocketAddress.Size);

                    IntPtr handle = _listenSocket.SafeHandle.DangerousGetHandle();

                    errorCode = Interop.Winsock.setsockopt(
                        _acceptSocket.SafeHandle,
                        SocketOptionLevel.Socket,
                        SocketOptionName.UpdateAcceptContext,
                        ref handle,
                        Marshal.SizeOf(handle));

                    if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                    GlobalLog.Print("AcceptOverlappedAsyncResult#" + Logging.HashString(this) + "::PostCallback() setsockopt handle:" + handle.ToString() + " AcceptSocket:" + Logging.HashString(_acceptSocket) + " itsHandle:" + _acceptSocket.SafeHandle.DangerousGetHandle().ToString() + " returns:" + errorCode.ToString());
                }
                catch (ObjectDisposedException)
                {
                    errorCode = SocketError.OperationAborted;
                }

                ErrorCode = (int)errorCode;
            }

            if (errorCode == SocketError.Success)
            {
                return _listenSocket.UpdateAcceptSocket(_acceptSocket, _listenSocket.m_RightEndPoint.Create(remoteSocketAddress));
            }
            else
                return null;
        }

#endif // !FEATURE_PAL


        //
        // SetUnmanagedStructures -
        // Fills in Overlapped Structures used in an Async Overlapped Winsock call
        //   these calls are outside the runtime and are unmanaged code, so we need
        //   to prepare specific structures and ints that lie in unmanaged memory
        //   since the Overlapped calls can be Async
        //
        internal void SetUnmanagedStructures(byte[] buffer, int addressBufferLength)
        {
            // has to be called first to pin memory
            base.SetUnmanagedStructures(buffer);

            //
            // Fill in Buffer Array structure that will be used for our send/recv Buffer
            //
            _addressBufferLength = addressBufferLength;
            _buffer = buffer;
        }

        /*
        // Consider removing.
        internal void SetUnmanagedStructures(byte[] buffer, int addressBufferLength, ref OverlappedCache overlappedCache)
        {
            SetupCache(ref overlappedCache);
            SetUnmanagedStructures(buffer, addressBufferLength);
        }
        */

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

        internal byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }

        internal int BytesTransferred
        {
            get
            {
                return _localBytesTransferred;
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
