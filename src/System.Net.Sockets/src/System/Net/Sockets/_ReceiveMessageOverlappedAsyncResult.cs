// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Generic;

namespace System.Net.Sockets
{
    //
    //  OverlappedAsyncResult - used to take care of storage for async Socket operation
    //   from the BeginSend, BeginSendTo, BeginReceive, BeginReceiveFrom calls.
    //
    unsafe internal class ReceiveMessageOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        //
        // internal class members
        //
        private UnsafeSocketsNativeMethods.OSSOCK.WSAMsg* m_Message;
        internal SocketAddress SocketAddressOriginal;
        internal SocketAddress m_SocketAddress;
        private WSABuffer* m_WSABuffer;
        private byte[] m_WSABufferArray;
        private byte[] m_ControlBuffer;
        internal byte[] m_MessageBuffer;
        internal SocketFlags m_flags;

        private static readonly int s_ControlDataSize = Marshal.SizeOf<UnsafeSocketsNativeMethods.OSSOCK.ControlData>();
        private static readonly int s_ControlDataIPv6Size = Marshal.SizeOf<UnsafeSocketsNativeMethods.OSSOCK.ControlDataIPv6>();
        private static readonly int s_WSABufferSize = Marshal.SizeOf<WSABuffer>();
        private static readonly int s_WSAMsgSize = Marshal.SizeOf<UnsafeSocketsNativeMethods.OSSOCK.WSAMsg>();

        internal IPPacketInformation m_IPPacketInformation;

        //
        // the following two will be used only on WinNT to enable completion ports
        //
        //
        // Constructor. We take in the socket that's creating us, the caller's
        // state object, and the buffer on which the I/O will be performed.
        // We save the socket and state, pin the callers's buffer, and allocate
        // an event for the WaitHandle.
        //
        internal ReceiveMessageOverlappedAsyncResult(Socket socket, Object asyncState, AsyncCallback asyncCallback) :
            base(socket, asyncState, asyncCallback)
        { }

        internal IntPtr GetSocketAddressSizePtr()
        {
            return Marshal.UnsafeAddrOfPinnedArrayElement(m_SocketAddress.m_Buffer, m_SocketAddress.GetAddressSizeOffset());
        }

        internal SocketAddress SocketAddress
        {
            get
            {
                return m_SocketAddress;
            }
        }


        //
        // SetUnmanagedStructures -
        // Fills in Overlapped Structures used in an Async Overlapped Winsock call
        //   these calls are outside the runtime and are unmanaged code, so we need
        //   to prepare specific structures and ints that lie in unmanaged memory
        //   since the Overlapped calls can be Async
        //

        internal void SetUnmanagedStructures(byte[] buffer, int offset, int size, SocketAddress socketAddress, SocketFlags socketFlags)
        {
            m_MessageBuffer = new byte[s_WSAMsgSize];
            m_WSABufferArray = new byte[s_WSABufferSize];

            //ipv4 or ipv6?
            IPAddress ipAddress = (socketAddress.Family == AddressFamily.InterNetworkV6
                ? socketAddress.GetIPAddress() : null);
            bool ipv4 = (((Socket)AsyncObject).AddressFamily == AddressFamily.InterNetwork
                || (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)); // DualMode
            bool ipv6 = ((Socket)AsyncObject).AddressFamily == AddressFamily.InterNetworkV6;

            //prepare control buffer
            if (ipv4)
            {
                m_ControlBuffer = new byte[s_ControlDataSize];
            }
            else if (ipv6)
            {
                m_ControlBuffer = new byte[s_ControlDataIPv6Size];
            }

            //pin buffers
            object[] objectsToPin = new object[(m_ControlBuffer != null) ? 5 : 4];
            objectsToPin[0] = buffer;
            objectsToPin[1] = m_MessageBuffer;
            objectsToPin[2] = m_WSABufferArray;

            //prepare socketaddress buffer
            m_SocketAddress = socketAddress;
            m_SocketAddress.CopyAddressSizeIntoBuffer();
            objectsToPin[3] = m_SocketAddress.m_Buffer;

            if (m_ControlBuffer != null)
            {
                objectsToPin[4] = m_ControlBuffer;
            }

            base.SetUnmanagedStructures(objectsToPin);

            //prepare data buffer
            m_WSABuffer = (WSABuffer*)Marshal.UnsafeAddrOfPinnedArrayElement(m_WSABufferArray, 0);
            m_WSABuffer->Length = size;
            m_WSABuffer->Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset);


            //setup structure
            m_Message = (UnsafeSocketsNativeMethods.OSSOCK.WSAMsg*)Marshal.UnsafeAddrOfPinnedArrayElement(m_MessageBuffer, 0);
            m_Message->socketAddress = Marshal.UnsafeAddrOfPinnedArrayElement(m_SocketAddress.m_Buffer, 0);
            m_Message->addressLength = (uint)m_SocketAddress.Size;
            m_Message->buffers = Marshal.UnsafeAddrOfPinnedArrayElement(m_WSABufferArray, 0);
            m_Message->count = 1;

            if (m_ControlBuffer != null)
            {
                m_Message->controlBuffer.Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(m_ControlBuffer, 0);
                m_Message->controlBuffer.Length = m_ControlBuffer.Length;
            }

            m_Message->flags = socketFlags;
        }
        
        unsafe private void InitIPPacketInformation()
        {
            IPAddress address = null;

            //ipv4
            if (m_ControlBuffer.Length == s_ControlDataSize)
            {
                UnsafeSocketsNativeMethods.OSSOCK.ControlData controlData = Marshal.PtrToStructure<UnsafeSocketsNativeMethods.OSSOCK.ControlData>(m_Message->controlBuffer.Pointer);
                if (controlData.length != UIntPtr.Zero)
                {
                    address = new IPAddress((long)controlData.address);
                }
                m_IPPacketInformation = new IPPacketInformation(((address != null) ? address : IPAddress.None), (int)controlData.index);
            }
            //ipv6
            else if (m_ControlBuffer.Length == s_ControlDataIPv6Size)
            {
                UnsafeSocketsNativeMethods.OSSOCK.ControlDataIPv6 controlData = Marshal.PtrToStructure<UnsafeSocketsNativeMethods.OSSOCK.ControlDataIPv6>(m_Message->controlBuffer.Pointer);
                if (controlData.length != UIntPtr.Zero)
                {
                    address = new IPAddress(controlData.address);
                }
                m_IPPacketInformation = new IPPacketInformation(((address != null) ? address : IPAddress.IPv6None), (int)controlData.index);
            }
            //other
            else
            {
                m_IPPacketInformation = new IPPacketInformation();
            }
        }


        //
        // This method is called after an asynchronous call is made for the user,
        // it checks and acts accordingly if the IO:
        // 1) completed synchronously.
        // 2) was pended.
        // 3) failed.
        //

        internal void SyncReleaseUnmanagedStructures()
        {
            InitIPPacketInformation();
            ForceReleaseUnmanagedStructures();
        }


        protected override void ForceReleaseUnmanagedStructures()
        {
            m_flags = m_Message->flags;
            base.ForceReleaseUnmanagedStructures();
        }

        internal override object PostCompletion(int numBytes)
        {
            InitIPPacketInformation();
            if (ErrorCode == 0)
            {
                if (Logging.On) LogBuffer(numBytes);
            }
            return (int)numBytes;
        }

        void LogBuffer(int size)
        {
            GlobalLog.Assert(Logging.On, "ReceiveMessageOverlappedAsyncResult#{0}::LogBuffer()|Logging is off!", Logging.HashString(this));
            Logging.Dump(Logging.Sockets, AsyncObject, "PostCompletion", m_WSABuffer->Pointer, Math.Min(m_WSABuffer->Length, size));
        }
    }; // class OverlappedAsyncResult
} // namespace System.Net.Sockets
