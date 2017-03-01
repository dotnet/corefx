// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Generic;

namespace System.Net.Sockets
{
    internal unsafe sealed partial class ReceiveMessageOverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        private byte[] _controlBuffer;

        private static readonly int s_controlDataSize = Marshal.SizeOf<Interop.Winsock.ControlData>();
        private static readonly int s_controlDataIPv6Size = Marshal.SizeOf<Interop.Winsock.ControlDataIPv6>();

        internal unsafe byte* GetSocketAddressPtr()
        {
            Debug.Assert(_socketAddress != null);
            return (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, 0);
        }

        internal unsafe int* GetSocketAddressSizePtr()
        {
            Debug.Assert(_socketAddress != null);
            return (int*)Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, _socketAddress.GetAddressSizeOffset());
        }

        internal unsafe int GetSocketAddressSize()
        {
            return *(GetSocketAddressSizePtr());
        }

        internal unsafe byte* GetControlBuffer()
        {
            return (_controlBuffer == null ? null : (byte*) Marshal.UnsafeAddrOfPinnedArrayElement(_controlBuffer, 0));
        }

        internal int GetControlBufferSize()
        {
            return _controlBuffer.Length;
        }

        // SetUnmanagedStructures
        //
        // Fills in overlapped Structures used in an async overlapped Winsock call.
        // These calls are outside the runtime and are unmanaged code, so we need
        // to prepare specific structures and ints that lie in unmanaged memory
        // since the overlapped calls may complete asynchronously.
        internal void SetUnmanagedStructures(byte[] buffer, Internals.SocketAddress socketAddress, SocketFlags flags)
        {
            _socketFlags = flags;
            
            bool ipv4, ipv6;
            Socket.GetIPProtocolInformation(((Socket)AsyncObject).AddressFamily, socketAddress, out ipv4, out ipv6);

            // Prepare control buffer.
            if (ipv4)
            {
                _controlBuffer = new byte[s_controlDataSize];
            }
            else if (ipv6)
            {
                _controlBuffer = new byte[s_controlDataIPv6Size];
            }

            // Pin buffers.
            object[] objectsToPin = new object[(_controlBuffer != null) ? 3 : 2];
            objectsToPin[0] = buffer;

            // Prepare socketaddress buffer.
            _socketAddress = socketAddress;
            _socketAddress.CopyAddressSizeIntoBuffer();
            objectsToPin[1] = _socketAddress.Buffer;

            if (_controlBuffer != null)
            {
                objectsToPin[2] = _controlBuffer;
            }

            base.SetUnmanagedStructures(objectsToPin);
        }

        private unsafe void InitIPPacketInformation()
        {
            IntPtr controlBufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(_controlBuffer, 0);
            if (_controlBuffer.Length == s_controlDataSize)
            {
                // IPv4
                _ipPacketInformation = SocketPal.GetIPPacketInformation((Interop.Winsock.ControlData*)controlBufferPtr);
            }
            else if (_controlBuffer.Length == s_controlDataIPv6Size)
            {
                // IPv6
                _ipPacketInformation = SocketPal.GetIPPacketInformation((Interop.Winsock.ControlDataIPv6*)controlBufferPtr);
            }
            else
            {
                // Other
                _ipPacketInformation = new IPPacketInformation();
            }
        }

        protected override void ForceReleaseUnmanagedStructures()
        {
            base.ForceReleaseUnmanagedStructures();
        }

        internal override object PostCompletion(int numBytes)
        {
            InitIPPacketInformation();
            return base.PostCompletion(numBytes);
        }
    }
}
