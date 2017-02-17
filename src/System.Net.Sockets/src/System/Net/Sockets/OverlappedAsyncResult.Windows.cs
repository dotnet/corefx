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
    // OverlappedAsyncResult
    //
    // This class is used to take care of storage for async Socket operation
    // from the BeginSend, BeginSendTo, BeginReceive, BeginReceiveFrom calls.
    internal partial class OverlappedAsyncResult : BaseOverlappedAsyncResult
    {
        internal unsafe byte* GetSocketAddressPtr()
        {
            Debug.Assert(_socketAddress != null);
            return (byte*) Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, 0);
        }

        internal unsafe int* GetSocketAddressSizePtr()
        {
            Debug.Assert(_socketAddress != null);
            return (int*) Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, _socketAddress.GetAddressSizeOffset());
        }

        internal unsafe int GetSocketAddressSize()
        {
            return *(GetSocketAddressSizePtr());
        }

        // SetUnmanagedStructures
        //
        // Fills in overlapped structures used in an async overlapped Winsock call.
        // These calls are outside the runtime and are unmanaged code, so we need
        // to prepare specific structures and ints that lie in unmanaged memory
        // since the overlapped calls may complete asynchronously.
        internal void SetUnmanagedStructures(byte[] buffer, Internals.SocketAddress socketAddress)
        {
            // Fill in Buffer Array structure that will be used for our send/recv Buffer
            _socketAddress = socketAddress;
            if (_socketAddress != null)
            {
                object[] objectsToPin = null;
                objectsToPin = new object[2];
                objectsToPin[0] = buffer;

                _socketAddress.CopyAddressSizeIntoBuffer();
                objectsToPin[1] = _socketAddress.Buffer;

                base.SetUnmanagedStructures(objectsToPin);
            }
            else
            {
                base.SetUnmanagedStructures(buffer);
            }
        }

        internal void SetUnmanagedStructures(IList<ArraySegment<byte>> buffers)
        {
            // Fill in Buffer Array structure that will be used for our send/recv Buffer.
            // Make sure we don't let the app mess up the buffer array enough to cause
            // corruption.
            int count = buffers.Count;
            ArraySegment<byte>[] buffersCopy = new ArraySegment<byte>[count];

            for (int i = 0; i < count; i++)
            {
                buffersCopy[i] = buffers[i];
                RangeValidationHelpers.ValidateSegment(buffersCopy[i]);
            }

            object[] objectsToPin = new object[count];
            for (int i = 0; i < count; i++)
            {
                objectsToPin[i] = buffersCopy[i].Array;
            }

            base.SetUnmanagedStructures(objectsToPin);
        }
    }
}
