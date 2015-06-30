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

        private int m_LocalBytesTransferred;
        private Socket m_ListenSocket;
        private Socket m_AcceptSocket;

        private int m_AddressBufferLength;
        private byte[] m_Buffer;

        // Constructor. We take in the socket that's creating us, the caller's
        // state object, and the buffer on which the I/O will be performed.
        // We save the socket and state, pin the callers's buffer, and allocate
        // an event for the WaitHandle.
        //
        internal AcceptOverlappedAsyncResult(Socket listenSocket, Object asyncState, AsyncCallback asyncCallback) :
            base(listenSocket, asyncState, asyncCallback)
        {
            m_ListenSocket = listenSocket;
        }

#if !FEATURE_PAL

        //
        // This method will be called by us when the IO completes synchronously and
        // by the ThreadPool when the IO completes asynchronously. (only called on WinNT)
        //

        internal override object PostCompletion(int numBytes)
        {
            SocketError errorCode = (SocketError)ErrorCode;

            SocketAddress remoteSocketAddress = null;
            if (errorCode == SocketError.Success)
            {
                m_LocalBytesTransferred = numBytes;
                if (Logging.On) LogBuffer((long)numBytes);

                //get the endpoint

                remoteSocketAddress = m_ListenSocket.m_RightEndPoint.Serialize();

                IntPtr localAddr;
                int localAddrLength;
                IntPtr remoteAddr;

                //set the socket context
                try
                {
                    m_ListenSocket.GetAcceptExSockaddrs(
                                    Marshal.UnsafeAddrOfPinnedArrayElement(m_Buffer, 0),
                                    m_Buffer.Length - (m_AddressBufferLength * 2),
                                    m_AddressBufferLength,
                                    m_AddressBufferLength,
                                    out localAddr,
                                    out localAddrLength,
                                    out remoteAddr,
                                    out remoteSocketAddress.m_Size
                                    );
                    Marshal.Copy(remoteAddr, remoteSocketAddress.m_Buffer, 0, remoteSocketAddress.m_Size);

                    IntPtr handle = m_ListenSocket.SafeHandle.DangerousGetHandle();

                    errorCode = UnsafeSocketsNativeMethods.OSSOCK.setsockopt(
                        m_AcceptSocket.SafeHandle,
                        SocketOptionLevel.Socket,
                        SocketOptionName.UpdateAcceptContext,
                        ref handle,
                        Marshal.SizeOf(handle));

                    if (errorCode == SocketError.SocketError) errorCode = (SocketError)Marshal.GetLastWin32Error();
                    GlobalLog.Print("AcceptOverlappedAsyncResult#" + Logging.HashString(this) + "::PostCallback() setsockopt handle:" + handle.ToString() + " AcceptSocket:" + Logging.HashString(m_AcceptSocket) + " itsHandle:" + m_AcceptSocket.SafeHandle.DangerousGetHandle().ToString() + " returns:" + errorCode.ToString());
                }
                catch (ObjectDisposedException)
                {
                    errorCode = SocketError.OperationAborted;
                }

                ErrorCode = (int)errorCode;
            }

            if (errorCode == SocketError.Success)
            {
                return m_ListenSocket.UpdateAcceptSocket(m_AcceptSocket, m_ListenSocket.m_RightEndPoint.Create(remoteSocketAddress));
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
            m_AddressBufferLength = addressBufferLength;
            m_Buffer = buffer;
        }

        /*
        // Consider removing.
        internal void SetUnmanagedStructures(byte[] buffer, int addressBufferLength, ref OverlappedCache overlappedCache)
        {
            SetupCache(ref overlappedCache);
            SetUnmanagedStructures(buffer, addressBufferLength);
        }
        */

        void LogBuffer(long size)
        {
            GlobalLog.Assert(Logging.On, "AcceptOverlappedAsyncResult#{0}::LogBuffer()|Logging is off!", Logging.HashString(this));
            IntPtr pinnedBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(m_Buffer, 0);
            if (pinnedBuffer != IntPtr.Zero)
            {
                if (size > -1)
                {
                    Logging.Dump(Logging.Sockets, m_ListenSocket, "PostCompletion", pinnedBuffer, (int)Math.Min(size, (long)m_Buffer.Length));
                }
                else
                {
                    Logging.Dump(Logging.Sockets, m_ListenSocket, "PostCompletion", pinnedBuffer, (int)m_Buffer.Length);
                }
            }
        }

        internal byte[] Buffer
        {
            get
            {
                return m_Buffer;
            }
        }

        internal int BytesTransferred
        {
            get
            {
                return m_LocalBytesTransferred;
            }
        }

        internal Socket AcceptSocket
        {
            set
            {
                m_AcceptSocket = value;
            }
        }
    }
}
