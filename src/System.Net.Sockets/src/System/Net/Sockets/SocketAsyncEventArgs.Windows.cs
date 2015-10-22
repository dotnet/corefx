// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace System.Net.Sockets
{
    public partial class SocketAsyncEventArgs : EventArgs, IDisposable
    {
        // Struct sizes needed for some custom marshaling.
        internal static readonly int s_controlDataSize = Marshal.SizeOf<Interop.Winsock.ControlData>();
        internal static readonly int s_controlDataIPv6Size = Marshal.SizeOf<Interop.Winsock.ControlDataIPv6>();
        internal static readonly int s_wsaMsgSize = Marshal.SizeOf<Interop.Winsock.WSAMsg>();

        // Buffer,Offset,Count property variables.
        private WSABuffer _wsaBuffer;
        private IntPtr _ptrSingleBuffer;

        // BufferList property variables.
        private WSABuffer[] _wsaBufferArray;
        private bool _bufferListChanged;

        // Internal buffers for WSARecvMsg
        private byte[] _wsaMessageBuffer;
        private GCHandle _wsaMessageBufferGCHandle;
        private IntPtr _ptrWSAMessageBuffer;
        private byte[] _controlBuffer;
        private GCHandle _controlBufferGCHandle;
        private IntPtr _ptrControlBuffer;
        private WSABuffer[] _wsaRecvMsgWSABufferArray;
        private GCHandle _wsaRecvMsgWSABufferArrayGCHandle;
        private IntPtr _ptrWSARecvMsgWSABufferArray;

        // Internal buffer for AcceptEx when Buffer not supplied.
        private IntPtr _ptrAcceptBuffer;

        // Internal SocketAddress buffer
        private GCHandle _socketAddressGCHandle;
        private Internals.SocketAddress _pinnedSocketAddress;
        private IntPtr _ptrSocketAddressBuffer;
        private IntPtr _ptrSocketAddressBufferSize;

        // SendPacketsElements property variables.
        private SendPacketsElement[] _sendPacketsElementsInternal;
        private Interop.Winsock.TransmitPacketsElement[] _sendPacketsDescriptor;
        private int _sendPacketsElementsFileCount;
        private int _sendPacketsElementsBufferCount;

        // Internal variables for SendPackets
        private FileStream[] _sendPacketsFileStreams;
        private SafeHandle[] _sendPacketsFileHandles;
        private IntPtr _ptrSendPacketsDescriptor;

        // Overlapped object related variables.
        private SafeNativeOverlapped _ptrNativeOverlapped;
        private PreAllocatedOverlapped _preAllocatedOverlapped;
        private object[] _objectsToPin;
        private enum PinState
        {
            None = 0,
            NoBuffer,
            SingleAcceptBuffer,
            SingleBuffer,
            MultipleBuffer,
            SendPackets
        }
        private PinState _pinState;
        private byte[] _pinnedAcceptBuffer;
        private byte[] _pinnedSingleBuffer;
        private int _pinnedSingleBufferOffset;
        private int _pinnedSingleBufferCount;

        internal int? SendPacketsDescriptorCount
        {
            get
            {
                return _sendPacketsDescriptor == null ? null : (int?)_sendPacketsDescriptor.Length;
            }
        }

        private void InitializeInternals()
        {
            // Zero tells TransmitPackets to select a default send size.
            _sendPacketsSendSize = 0;
        }

        private void FreeInternals(bool calledFromFinalizer)
        {
            // Free native overlapped data.
            FreeOverlapped(calledFromFinalizer);
        }

        private void SetupSingleBuffer()
        {
            CheckPinSingleBuffer(true);
        }

        private void SetupMultipleBuffers()
        {
            _bufferListChanged = true;
            CheckPinMultipleBuffers();
        }

        private void SetupSendPacketsElements()
        {
            _sendPacketsElementsInternal = null;
        }

        private void InnerComplete()
        {
            CompleteIOCPOperation();
        }

        private unsafe void PrepareIOCPOperation()
        {
            Debug.Assert(_currentSocket != null, "_currentSocket is null");
            Debug.Assert(_currentSocket.SafeHandle != null, "_currentSocket.SafeHandle is null");
            Debug.Assert(!_currentSocket.SafeHandle.IsInvalid, "_currentSocket.SafeHandle is invalid");

            ThreadPoolBoundHandle boundHandle = _currentSocket.SafeHandle.GetOrAllocateThreadPoolBoundHandle();

            NativeOverlapped* overlapped = null;
            if (_preAllocatedOverlapped != null)
            {
                overlapped = boundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::boundHandle#" + Logging.HashString(boundHandle) +
                    "::AllocateNativeOverlapped(m_PreAllocatedOverlapped=" +
                    Logging.HashString(_preAllocatedOverlapped) +
                    "). Returned = " + ((IntPtr)overlapped).ToString("x"));
            }
            else
            {
                overlapped = boundHandle.AllocateNativeOverlapped(CompletionPortCallback, this, null);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::boundHandle#" + Logging.HashString(boundHandle) +
                    "::AllocateNativeOverlapped(pinData=null)" +
                    "). Returned = " + ((IntPtr)overlapped).ToString("x"));
            }

            Debug.Assert(overlapped != null, "NativeOverlapped is null.");
            _ptrNativeOverlapped = new SafeNativeOverlapped(_currentSocket.SafeHandle, overlapped);
        }

        private void CompleteIOCPOperation()
        {
            // TODO: Optimization to remove callbacks if the operations are completed synchronously:
            //       Use SetFileCompletionNotificationModes(FILE_SKIP_COMPLETION_PORT_ON_SUCCESS).

            // If SetFileCompletionNotificationModes(FILE_SKIP_COMPLETION_PORT_ON_SUCCESS) is not set on this handle
            // it is guaranteed that the IOCP operation will be completed in the callback even if Socket.Success was 
            // returned by the Win32 API.

            // Required to allow another IOCP operation for the same handle.
            if (_ptrNativeOverlapped != null)
            {
                _ptrNativeOverlapped.Dispose();
                _ptrNativeOverlapped = null;
            }
        }

        private void InnerStartOperationAccept(bool userSuppliedBuffer)
        {
            if (!userSuppliedBuffer)
            {
                CheckPinSingleBuffer(false);
            }
        }

        internal unsafe SocketError DoOperationAccept(Socket socket, SafeCloseSocket handle, SafeCloseSocket acceptHandle, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            SocketError socketError = SocketError.Success;

            if (!socket.AcceptEx(
                handle,
                acceptHandle,
                (_ptrSingleBuffer != IntPtr.Zero) ? _ptrSingleBuffer : _ptrAcceptBuffer,
                (_ptrSingleBuffer != IntPtr.Zero) ? Count - _acceptAddressBufferCount : 0,
                _acceptAddressBufferCount / 2,
                _acceptAddressBufferCount / 2,
                out bytesTransferred,
                _ptrNativeOverlapped))
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationConnect()
        {
            // ConnectEx uses a sockaddr buffer containing he remote address to which to connect.
            // It can also optionally take a single buffer of data to send after the connection is complete.
            //
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            // The optional buffer is pinned using the Overlapped.UnsafePack method that takes a single object to pin.
            PinSocketAddressBuffer();
            CheckPinNoBuffer();
        }

        internal unsafe SocketError DoOperationConnect(Socket socket, SafeCloseSocket handle, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            SocketError socketError = SocketError.Success;

            if (!socket.ConnectEx(
                handle,
                _ptrSocketAddressBuffer,
                _socketAddress.Size,
                _ptrSingleBuffer,
                Count,
                out bytesTransferred,
                _ptrNativeOverlapped))
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationDisconnect()
        {
            CheckPinNoBuffer();
        }

        private void InnerStartOperationReceive()
        {
            // WWSARecv uses a WSABuffer array describing buffers of data to send.
            //
            // Single and multiple buffers are handled differently so as to optimize
            // performance for the more common single buffer case.  
            //
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            //
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
        }

        internal unsafe SocketError DoOperationReceive(SafeCloseSocket handle, out SocketFlags flags, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            flags = _socketFlags;

            SocketError socketError;
            if (_buffer != null)
            {
                // Single buffer case.
                socketError = Interop.Winsock.WSARecv(
                    handle,
                    ref _wsaBuffer,
                    1,
                    out bytesTransferred,
                    ref flags,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }
            else
            {
                // Multi buffer case.
                socketError = Interop.Winsock.WSARecv(
                    handle,
                    _wsaBufferArray,
                    _wsaBufferArray.Length,
                    out bytesTransferred,
                    ref flags,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }

            if (socketError == SocketError.SocketError)
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationReceiveFrom()
        {
            // WSARecvFrom uses e a WSABuffer array describing buffers in which to 
            // receive data and from which to send data respectively. Single and multiple buffers
            // are handled differently so as to optimize performance for the more common single buffer case.
            //
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            //
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
            //
            // WSARecvFrom and WSASendTo also uses a sockaddr buffer in which to store the address from which the data was received.
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            PinSocketAddressBuffer();
        }

        internal unsafe SocketError DoOperationReceiveFrom(SafeCloseSocket handle, out SocketFlags flags, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            flags = _socketFlags;

            SocketError socketError;
            if (_buffer != null)
            {
                socketError = Interop.Winsock.WSARecvFrom(
                    handle,
                    ref _wsaBuffer,
                    1,
                    out bytesTransferred,
                    ref flags,
                    _ptrSocketAddressBuffer,
                    _ptrSocketAddressBufferSize,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }
            else
            {
                socketError = Interop.Winsock.WSARecvFrom(
                    handle,
                    _wsaBufferArray,
                    _wsaBufferArray.Length,
                    out bytesTransferred,
                    ref flags,
                    _ptrSocketAddressBuffer,
                    _ptrSocketAddressBufferSize,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }

            if (socketError == SocketError.SocketError)
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationReceiveMessageFrom()
        {
            // WSARecvMsg uses a WSAMsg descriptor.
            // The WSAMsg buffer is pinned with a GCHandle to avoid complicating the use of Overlapped.
            // WSAMsg contains a pointer to a sockaddr.  
            // The sockaddr is pinned with a GCHandle to avoid complicating the use of Overlapped.
            // WSAMsg contains a pointer to a WSABuffer array describing data buffers.
            // WSAMsg also contains a single WSABuffer describing a control buffer.
            PinSocketAddressBuffer();

            // Create and pin a WSAMessageBuffer if none already.
            if (_wsaMessageBuffer == null)
            {
                _wsaMessageBuffer = new byte[s_wsaMsgSize];
                _wsaMessageBufferGCHandle = GCHandle.Alloc(_wsaMessageBuffer, GCHandleType.Pinned);
                _ptrWSAMessageBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_wsaMessageBuffer, 0);
            }

            // Create and pin an appropriately sized control buffer if none already
            IPAddress ipAddress = (_socketAddress.Family == AddressFamily.InterNetworkV6 ? _socketAddress.GetIPAddress() : null);
            bool ipv4 = (_currentSocket.AddressFamily == AddressFamily.InterNetwork || (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)); // DualMode
            bool ipv6 = _currentSocket.AddressFamily == AddressFamily.InterNetworkV6;

            if (ipv4 && (_controlBuffer == null || _controlBuffer.Length != s_controlDataSize))
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = new byte[s_controlDataSize];
            }
            else if (ipv6 && (_controlBuffer == null || _controlBuffer.Length != s_controlDataIPv6Size))
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = new byte[s_controlDataIPv6Size];
            }
            if (!_controlBufferGCHandle.IsAllocated)
            {
                _controlBufferGCHandle = GCHandle.Alloc(_controlBuffer, GCHandleType.Pinned);
                _ptrControlBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_controlBuffer, 0);
            }

            // If single buffer we need a pinned 1 element WSABuffer.
            if (_buffer != null)
            {
                if (_wsaRecvMsgWSABufferArray == null)
                {
                    _wsaRecvMsgWSABufferArray = new WSABuffer[1];
                }
                _wsaRecvMsgWSABufferArray[0].Pointer = _ptrSingleBuffer;
                _wsaRecvMsgWSABufferArray[0].Length = _count;
                _wsaRecvMsgWSABufferArrayGCHandle = GCHandle.Alloc(_wsaRecvMsgWSABufferArray, GCHandleType.Pinned);
                _ptrWSARecvMsgWSABufferArray = Marshal.UnsafeAddrOfPinnedArrayElement(_wsaRecvMsgWSABufferArray, 0);
            }
            else
            {
                // Just pin the multi-buffer WSABuffer.
                _wsaRecvMsgWSABufferArrayGCHandle = GCHandle.Alloc(_wsaBufferArray, GCHandleType.Pinned);
                _ptrWSARecvMsgWSABufferArray = Marshal.UnsafeAddrOfPinnedArrayElement(_wsaBufferArray, 0);
            }

            // Fill in WSAMessageBuffer.
            unsafe
            {
                Interop.Winsock.WSAMsg* pMessage = (Interop.Winsock.WSAMsg*)_ptrWSAMessageBuffer; ;
                pMessage->socketAddress = _ptrSocketAddressBuffer;
                pMessage->addressLength = (uint)_socketAddress.Size;
                pMessage->buffers = _ptrWSARecvMsgWSABufferArray;
                if (_buffer != null)
                {
                    pMessage->count = (uint)1;
                }
                else
                {
                    pMessage->count = (uint)_wsaBufferArray.Length;
                }

                if (_controlBuffer != null)
                {
                    pMessage->controlBuffer.Pointer = _ptrControlBuffer;
                    pMessage->controlBuffer.Length = _controlBuffer.Length;
                }
                pMessage->flags = _socketFlags;
            }
        }

        internal unsafe SocketError DoOperationReceiveMessageFrom(Socket socket, SafeCloseSocket handle, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            SocketError socketError = socket.WSARecvMsg(
                handle,
                _ptrWSAMessageBuffer,
                out bytesTransferred,
                _ptrNativeOverlapped,
                IntPtr.Zero);

            if (socketError == SocketError.SocketError)
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationSend()
        {
            // WSASend uses a WSABuffer array describing buffers of data to send.
            //
            // Single and multiple buffers are handled differently so as to optimize
            // performance for the more common single buffer case.  
            //
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            //
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
        }

        internal unsafe SocketError DoOperationSend(SafeCloseSocket handle, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            SocketError socketError;
            if (_buffer != null)
            {
                // Single buffer case.
                socketError = Interop.Winsock.WSASend(
                    handle,
                    ref _wsaBuffer,
                    1,
                    out bytesTransferred,
                    _socketFlags,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }
            else
            {
                // Multi buffer case.
                socketError = Interop.Winsock.WSASend(
                    handle,
                    _wsaBufferArray,
                    _wsaBufferArray.Length,
                    out bytesTransferred,
                    _socketFlags,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }

            if (socketError == SocketError.SocketError)
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        private void InnerStartOperationSendPackets()
        {
            // Prevent mutithreaded manipulation of the list.
            if (_sendPacketsElements != null)
            {
                _sendPacketsElementsInternal = (SendPacketsElement[])_sendPacketsElements.Clone();
            }

            // TransmitPackets uses an array of TRANSMIT_PACKET_ELEMENT structs as
            // descriptors for buffers and files to be sent.  It also takes a send size
            // and some flags.  The TRANSMIT_PACKET_ELEMENT for a file contains a native file handle.
            // This function basically opens the files to get the file handles, pins down any buffers
            // specified and builds the native TRANSMIT_PACKET_ELEMENT array that will be passed
            // to TransmitPackets.

            // Scan the elements to count files and buffers.
            _sendPacketsElementsFileCount = 0;
            _sendPacketsElementsBufferCount = 0;

            Debug.Assert(_sendPacketsElementsInternal != null);

            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe._filePath != null)
                    {
                        _sendPacketsElementsFileCount++;
                    }
                    if (spe._buffer != null && spe._count > 0)
                    {
                        _sendPacketsElementsBufferCount++;
                    }
                }
            }

            // Attempt to open the files if any were given.
            if (_sendPacketsElementsFileCount > 0)
            {
                // Create arrays for streams and handles.
                _sendPacketsFileStreams = new FileStream[_sendPacketsElementsFileCount];
                _sendPacketsFileHandles = new SafeHandle[_sendPacketsElementsFileCount];

                // Loop through the elements attempting to open each files and get its handle.
                int index = 0;
                foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
                {
                    if (spe != null && spe._filePath != null)
                    {
                        Exception fileStreamException = null;
                        try
                        {
                            // Create a FileStream to open the file.
                            _sendPacketsFileStreams[index] =
                                new FileStream(spe._filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        catch (Exception ex)
                        {
                            // Save the exception to throw after closing any previous successful file opens.
                            fileStreamException = ex;
                        }
                        if (fileStreamException != null)
                        {
                            // Got an exception opening a file - do some cleanup then throw.
                            for (int i = 0; i < _sendPacketsElementsFileCount; i++)
                            {
                                // Drop handles.
                                _sendPacketsFileHandles[i] = null;

                                // Close any open streams.
                                if (_sendPacketsFileStreams[i] != null)
                                {
                                    _sendPacketsFileStreams[i].Dispose();
                                    _sendPacketsFileStreams[i] = null;
                                }
                            }
                            throw fileStreamException;
                        }

                        // Get the file handle from the stream.
                        _sendPacketsFileHandles[index] = _sendPacketsFileStreams[index].SafeFileHandle;
                        index++;
                    }
                }
            }

            CheckPinSendPackets();
        }

        internal SocketError DoOperationSendPackets(Socket socket, SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            bool result = socket.TransmitPackets(
                handle,
                _ptrSendPacketsDescriptor,
                _sendPacketsDescriptor.Length,
                _sendPacketsSendSize,
                _ptrNativeOverlapped);

            return result ? SocketError.Success : SocketPal.GetLastSocketError();
        }

        private void InnerStartOperationSendTo()
        {
            // WSASendTo uses a WSABuffer array describing buffers in which to 
            // receive data and from which to send data respectively. Single and multiple buffers
            // are handled differently so as to optimize performance for the more common single buffer case.
            //
            // For a single buffer:
            //   The Overlapped.UnsafePack method is used that takes a single object to pin.
            //   A single WSABuffer that pre-exists in SocketAsyncEventArgs is used.
            //
            // For multiple buffers:
            //   The Overlapped.UnsafePack method is used that takes an array of objects to pin.
            //   An array to reference the multiple buffer is allocated.
            //   An array of WSABuffer descriptors is allocated.
            //
            // WSARecvFrom and WSASendTo also uses a sockaddr buffer in which to store the address from which the data was received.
            // The sockaddr is pinned with a GCHandle to avoid having to use the object array form of UnsafePack.
            PinSocketAddressBuffer();
        }

        internal SocketError DoOperationSendTo(SafeCloseSocket handle, out int bytesTransferred)
        {
            PrepareIOCPOperation();

            SocketError socketError;
            if (_buffer != null)
            {
                // Single buffer case.
                socketError = Interop.Winsock.WSASendTo(
                    handle,
                    ref _wsaBuffer,
                    1,
                    out bytesTransferred,
                    _socketFlags,
                    _ptrSocketAddressBuffer,
                    _socketAddress.Size,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }
            else
            {
                socketError = Interop.Winsock.WSASendTo(
                    handle,
                    _wsaBufferArray,
                    _wsaBufferArray.Length,
                    out bytesTransferred,
                    _socketFlags,
                    _ptrSocketAddressBuffer,
                    _socketAddress.Size,
                    _ptrNativeOverlapped,
                    IntPtr.Zero);
            }

            if (socketError == SocketError.SocketError)
            {
                socketError = SocketPal.GetLastSocketError();
            }

            return socketError;
        }

        // Ensures Overlapped object exists for operations that need no data buffer.
        private void CheckPinNoBuffer()
        {
            // PreAllocatedOverlapped will be reused.
            if (_pinState == PinState.None)
            {
                SetupOverlappedSingle(true);
            }
        }

        // Maintains pinned state of single buffer.
        private void CheckPinSingleBuffer(bool pinUsersBuffer)
        {
            if (pinUsersBuffer)
            {
                // Using app supplied buffer.
                if (_buffer == null)
                {
                    // No user buffer is set so unpin any existing single buffer pinning.
                    if (_pinState == PinState.SingleBuffer)
                    {
                        FreeOverlapped(false);
                    }
                }
                else
                {
                    if (_pinState == PinState.SingleBuffer && _pinnedSingleBuffer == _buffer)
                    {
                        // This buffer is already pinned - update if offset or count has changed.
                        if (_offset != _pinnedSingleBufferOffset)
                        {
                            _pinnedSingleBufferOffset = _offset;
                            _ptrSingleBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offset);
                            _wsaBuffer.Pointer = _ptrSingleBuffer;
                        }
                        if (_count != _pinnedSingleBufferCount)
                        {
                            _pinnedSingleBufferCount = _count;
                            _wsaBuffer.Length = _count;
                        }
                    }
                    else
                    {
                        FreeOverlapped(false);
                        SetupOverlappedSingle(true);
                    }
                }
            }
            else
            {
                // Using internal accept buffer.
                if (!(_pinState == PinState.SingleAcceptBuffer) || !(_pinnedSingleBuffer == _acceptBuffer))
                {
                    // Not already pinned - so pin it.
                    FreeOverlapped(false);
                    SetupOverlappedSingle(false);
                }
            }
        }

        // Ensures Overlapped object exists with appropriate multiple buffers pinned.
        private void CheckPinMultipleBuffers()
        {
            if (_bufferList == null)
            {
                // No buffer list is set so unpin any existing multiple buffer pinning.
                if (_pinState == PinState.MultipleBuffer)
                {
                    FreeOverlapped(false);
                }
            }
            else
            {
                if (!(_pinState == PinState.MultipleBuffer) || _bufferListChanged)
                {
                    // Need to setup a new Overlapped.
                    _bufferListChanged = false;
                    FreeOverlapped(false);
                    try
                    {
                        SetupOverlappedMultiple();
                    }
                    catch (Exception)
                    {
                        FreeOverlapped(false);
                        throw;
                    }
                }
            }
        }

        // Ensures Overlapped object exists with appropriate buffers pinned.
        private void CheckPinSendPackets()
        {
            if (_pinState != PinState.None)
            {
                FreeOverlapped(false);
            }
            SetupOverlappedSendPackets();
        }

        // Ensures appropriate SocketAddress buffer is pinned.
        private void PinSocketAddressBuffer()
        {
            // Check if already pinned.
            if (_pinnedSocketAddress == _socketAddress)
            {
                return;
            }

            // Unpin any existing.
            if (_socketAddressGCHandle.IsAllocated)
            {
                _socketAddressGCHandle.Free();
            }

            // Pin down the new one.
            _socketAddressGCHandle = GCHandle.Alloc(_socketAddress.Buffer, GCHandleType.Pinned);
            _socketAddress.CopyAddressSizeIntoBuffer();
            _ptrSocketAddressBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, 0);
            _ptrSocketAddressBufferSize = Marshal.UnsafeAddrOfPinnedArrayElement(_socketAddress.Buffer, _socketAddress.GetAddressSizeOffset());
            _pinnedSocketAddress = _socketAddress;
        }

        // Cleans up any existing Overlapped object and related state variables.
        private void FreeOverlapped(bool checkForShutdown)
        {
            if (!checkForShutdown || !Environment.HasShutdownStarted)
            {
                // Free the overlapped object.
                if (_ptrNativeOverlapped != null && !_ptrNativeOverlapped.IsInvalid)
                {
                    _ptrNativeOverlapped.Dispose();
                    _ptrNativeOverlapped = null;
                }

                // Free the preallocated overlapped object. This in turn will unpin
                // any pinned buffers.
                if (_preAllocatedOverlapped != null)
                {
                    _preAllocatedOverlapped.Dispose();
                    _preAllocatedOverlapped = null;

                    _pinState = PinState.None;
                    _pinnedAcceptBuffer = null;
                    _pinnedSingleBuffer = null;
                    _pinnedSingleBufferOffset = 0;
                    _pinnedSingleBufferCount = 0;
                }

                // Free any allocated GCHandles.
                if (_socketAddressGCHandle.IsAllocated)
                {
                    _socketAddressGCHandle.Free();
                }

                if (_wsaMessageBufferGCHandle.IsAllocated)
                {
                    _wsaMessageBufferGCHandle.Free();
                }

                if (_wsaRecvMsgWSABufferArrayGCHandle.IsAllocated)
                {
                    _wsaRecvMsgWSABufferArrayGCHandle.Free();
                }

                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
            }
        }

        // Sets up an Overlapped object with either _buffer or _acceptBuffer pinned.
        unsafe private void SetupOverlappedSingle(bool pinSingleBuffer)
        {
            // Pin buffer, get native pointers, and fill in WSABuffer descriptor.
            if (pinSingleBuffer)
            {
                if (_buffer != null)
                {
                    _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _buffer);
                    GlobalLog.Print(
                        "SocketAsyncEventArgs#" + Logging.HashString(this) +
                        "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=true, non-null buffer: " +
                        Logging.HashString(_preAllocatedOverlapped));

                    _pinnedSingleBuffer = _buffer;
                    _pinnedSingleBufferOffset = _offset;
                    _pinnedSingleBufferCount = _count;
                    _ptrSingleBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_buffer, _offset);
                    _ptrAcceptBuffer = IntPtr.Zero;
                    _wsaBuffer.Pointer = _ptrSingleBuffer;
                    _wsaBuffer.Length = _count;
                    _pinState = PinState.SingleBuffer;
                }
                else
                {
                    _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, null);
                    GlobalLog.Print(
                        "SocketAsyncEventArgs#" + Logging.HashString(this) +
                        "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=true, null buffer: " +
                        Logging.HashString(_preAllocatedOverlapped));

                    _pinnedSingleBuffer = null;
                    _pinnedSingleBufferOffset = 0;
                    _pinnedSingleBufferCount = 0;
                    _ptrSingleBuffer = IntPtr.Zero;
                    _ptrAcceptBuffer = IntPtr.Zero;
                    _wsaBuffer.Pointer = _ptrSingleBuffer;
                    _wsaBuffer.Length = _count;
                    _pinState = PinState.NoBuffer;
                }
            }
            else
            {
                _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _acceptBuffer);
                GlobalLog.Print(
                    "SocketAsyncEventArgs#" + Logging.HashString(this) +
                    "::SetupOverlappedSingle: new PreAllocatedOverlapped pinSingleBuffer=false: " +
                    Logging.HashString(_preAllocatedOverlapped));

                _pinnedAcceptBuffer = _acceptBuffer;
                _ptrAcceptBuffer = Marshal.UnsafeAddrOfPinnedArrayElement(_acceptBuffer, 0);
                _ptrSingleBuffer = IntPtr.Zero;
                _pinState = PinState.SingleAcceptBuffer;
            }
        }

        // Sets up an Overlapped object with with multiple buffers pinned.
        unsafe private void SetupOverlappedMultiple()
        {
            ArraySegment<byte>[] tempList = new ArraySegment<byte>[_bufferList.Count];
            _bufferList.CopyTo(tempList, 0);

            // Number of things to pin is number of buffers.
            // Ensure we have properly sized object array.
            if (_objectsToPin == null || (_objectsToPin.Length != tempList.Length))
            {
                _objectsToPin = new object[tempList.Length];
            }

            // Fill in object array.
            for (int i = 0; i < (tempList.Length); i++)
            {
                _objectsToPin[i] = tempList[i].Array;
            }

            if (_wsaBufferArray == null || _wsaBufferArray.Length != tempList.Length)
            {
                _wsaBufferArray = new WSABuffer[tempList.Length];
            }

            // Pin buffers and fill in WSABuffer descriptor pointers and lengths.
            _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _objectsToPin);
            GlobalLog.Print(
                "SocketAsyncEventArgs#" + Logging.HashString(this) + "::SetupOverlappedMultiple: new PreAllocatedOverlapped." +
                Logging.HashString(_preAllocatedOverlapped));

            for (int i = 0; i < tempList.Length; i++)
            {
                ArraySegment<byte> localCopy = tempList[i];
                RangeValidationHelpers.ValidateSegment(localCopy);
                _wsaBufferArray[i].Pointer = Marshal.UnsafeAddrOfPinnedArrayElement(localCopy.Array, localCopy.Offset);
                _wsaBufferArray[i].Length = localCopy.Count;
            }
            _pinState = PinState.MultipleBuffer;
        }

        // Sets up an Overlapped object for SendPacketsAsync.
        unsafe private void SetupOverlappedSendPackets()
        {
            int index;

            // Alloc native descriptor.
            _sendPacketsDescriptor =
                new Interop.Winsock.TransmitPacketsElement[_sendPacketsElementsFileCount + _sendPacketsElementsBufferCount];

            // Number of things to pin is number of buffers + 1 (native descriptor).
            // Ensure we have properly sized object array.
            if (_objectsToPin == null || (_objectsToPin.Length != _sendPacketsElementsBufferCount + 1))
            {
                _objectsToPin = new object[_sendPacketsElementsBufferCount + 1];
            }

            // Fill in objects to pin array. Native descriptor buffer first and then user specified buffers.
            _objectsToPin[0] = _sendPacketsDescriptor;
            index = 1;
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null && spe._buffer != null && spe._count > 0)
                {
                    _objectsToPin[index] = spe._buffer;
                    index++;
                }
            }

            // Pin buffers.
            _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, _objectsToPin);
            GlobalLog.Print(
                "SocketAsyncEventArgs#" + Logging.HashString(this) + "::SetupOverlappedSendPackets: new PreAllocatedOverlapped: " +
                Logging.HashString(_preAllocatedOverlapped));

            // Get pointer to native descriptor.
            _ptrSendPacketsDescriptor = Marshal.UnsafeAddrOfPinnedArrayElement(_sendPacketsDescriptor, 0);

            // Fill in native descriptor.
            int descriptorIndex = 0;
            int fileIndex = 0;
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe._buffer != null && spe._count > 0)
                    {
                        // This element is a buffer.
                        _sendPacketsDescriptor[descriptorIndex].buffer = Marshal.UnsafeAddrOfPinnedArrayElement(spe._buffer, spe._offset);
                        _sendPacketsDescriptor[descriptorIndex].length = (uint)spe._count;
                        _sendPacketsDescriptor[descriptorIndex].flags = (Interop.Winsock.TransmitPacketsElementFlags)spe._flags;
                        descriptorIndex++;
                    }
                    else if (spe._filePath != null)
                    {
                        // This element is a file.
                        _sendPacketsDescriptor[descriptorIndex].fileHandle = _sendPacketsFileHandles[fileIndex].DangerousGetHandle();
                        _sendPacketsDescriptor[descriptorIndex].fileOffset = spe._offset;
                        _sendPacketsDescriptor[descriptorIndex].length = (uint)spe._count;
                        _sendPacketsDescriptor[descriptorIndex].flags = (Interop.Winsock.TransmitPacketsElementFlags)spe._flags;
                        fileIndex++;
                        descriptorIndex++;
                    }
                }
            }

            _pinState = PinState.SendPackets;
        }

        internal void LogBuffer(int size)
        {
            switch (_pinState)
            {
                case PinState.SingleAcceptBuffer:
                    Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", _acceptBuffer, 0, size);
                    break;

                case PinState.SingleBuffer:
                    Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", _buffer, _offset, size);
                    break;

                case PinState.MultipleBuffer:
                    foreach (WSABuffer wsaBuffer in _wsaBufferArray)
                    {
                        Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", wsaBuffer.Pointer, Math.Min(wsaBuffer.Length, size));
                        if ((size -= wsaBuffer.Length) <= 0)
                        {
                            break;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        internal void LogSendPacketsBuffers(int size)
        {
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe._buffer != null && spe._count > 0)
                    {
                        // This element is a buffer.
                        Logging.Dump(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)Buffer", spe._buffer, spe._offset, Math.Min(spe._count, size));
                    }
                    else if (spe._filePath != null)
                    {
                        // This element is a file.
                        Logging.PrintInfo(Logging.Sockets, _currentSocket, "FinishOperation(" + _completedOperation + "Async)", SR.Format(SR.net_log_socket_not_logged_file, spe._filePath));
                    }
                }
            }
        }

        private SocketError FinishOperationAccept(Internals.SocketAddress remoteSocketAddress)
        {
            SocketError socketError;
            IntPtr localAddr;
            int localAddrLength;
            IntPtr remoteAddr;

            try
            {
                _currentSocket.GetAcceptExSockaddrs(
                    _ptrSingleBuffer != IntPtr.Zero ? _ptrSingleBuffer : _ptrAcceptBuffer,
                    _count != 0 ? _count - _acceptAddressBufferCount : 0,
                    _acceptAddressBufferCount / 2,
                    _acceptAddressBufferCount / 2,
                    out localAddr,
                    out localAddrLength,
                    out remoteAddr,
                    out remoteSocketAddress.InternalSize
                    );
                Marshal.Copy(remoteAddr, remoteSocketAddress.Buffer, 0, remoteSocketAddress.Size);

                // Set the socket context.
                IntPtr handle = _currentSocket.SafeHandle.DangerousGetHandle();

                socketError = Interop.Winsock.setsockopt(
                    _acceptSocket.SafeHandle,
                    SocketOptionLevel.Socket,
                    SocketOptionName.UpdateAcceptContext,
                    ref handle,
                    Marshal.SizeOf(handle));

                if (socketError == SocketError.SocketError)
                {
                    socketError = SocketPal.GetLastSocketError();
                }
            }
            catch (ObjectDisposedException)
            {
                socketError = SocketError.OperationAborted;
            }

            return socketError;
        }

        private SocketError FinishOperationConnect()
        {
            SocketError socketError;

            // Update the socket context.
            try
            {
                socketError = Interop.Winsock.setsockopt(
                    _currentSocket.SafeHandle,
                    SocketOptionLevel.Socket,
                    SocketOptionName.UpdateConnectContext,
                    null,
                    0);
                if (socketError == SocketError.SocketError)
                {
                    socketError = SocketPal.GetLastSocketError();
                }
            }
            catch (ObjectDisposedException)
            {
                socketError = SocketError.OperationAborted;
            }

            return socketError;
        }

        private unsafe int GetSocketAddressSize()
        {
            return *(int*)_ptrSocketAddressBufferSize;
        }

        private unsafe void FinishOperationReceiveMessageFrom()
        {
            IPAddress address = null;
            Interop.Winsock.WSAMsg* PtrMessage = (Interop.Winsock.WSAMsg*)Marshal.UnsafeAddrOfPinnedArrayElement(_wsaMessageBuffer, 0);

            if (_controlBuffer.Length == s_controlDataSize)
            {
                // IPv4.
                Interop.Winsock.ControlData controlData = Marshal.PtrToStructure<Interop.Winsock.ControlData>(PtrMessage->controlBuffer.Pointer);
                if (controlData.length != UIntPtr.Zero)
                {
                    address = new IPAddress((long)controlData.address);
                }
                _receiveMessageFromPacketInfo = new IPPacketInformation(((address != null) ? address : IPAddress.None), (int)controlData.index);
            }
            else if (_controlBuffer.Length == s_controlDataIPv6Size)
            {
                // IPv6.
                Interop.Winsock.ControlDataIPv6 controlData = Marshal.PtrToStructure<Interop.Winsock.ControlDataIPv6>(PtrMessage->controlBuffer.Pointer);
                if (controlData.length != UIntPtr.Zero)
                {
                    address = new IPAddress(controlData.address);
                }
                _receiveMessageFromPacketInfo = new IPPacketInformation(((address != null) ? address : IPAddress.IPv6None), (int)controlData.index);
            }
            else
            {
                // Other.
                _receiveMessageFromPacketInfo = new IPPacketInformation();
            }
        }

        private void FinishOperationSendPackets()
        {
            // Close the files if open.
            if (_sendPacketsFileStreams != null)
            {
                for (int i = 0; i < _sendPacketsElementsFileCount; i++)
                {
                    // Drop handles.
                    _sendPacketsFileHandles[i] = null;

                    // Close any open streams.
                    if (_sendPacketsFileStreams[i] != null)
                    {
                        _sendPacketsFileStreams[i].Dispose();
                        _sendPacketsFileStreams[i] = null;
                    }
                }
            }
            _sendPacketsFileStreams = null;
            _sendPacketsFileHandles = null;
        }

        private unsafe void CompletionPortCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
        {
#if DEBUG
            GlobalLog.SetThreadSource(ThreadKinds.CompletionPort);
            using (GlobalLog.SetThreadKind(ThreadKinds.System))
            {
                GlobalLog.Enter(
                    "CompletionPortCallback",
                    "errorCode: " + errorCode + ", numBytes: " + numBytes +
                    ", overlapped#" + ((IntPtr)nativeOverlapped).ToString("x"));
#endif
                SocketFlags socketFlags = SocketFlags.None;
                SocketError socketError = (SocketError)errorCode;

                // This is the same NativeOverlapped* as we already have a SafeHandle for, re-use the original.
                Debug.Assert((IntPtr)nativeOverlapped == _ptrNativeOverlapped.DangerousGetHandle(), "Handle mismatch");

                if (socketError == SocketError.Success)
                {
                    FinishOperationSuccess(socketError, (int)numBytes, socketFlags);
                }
                else
                {
                    if (socketError != SocketError.OperationAborted)
                    {
                        if (_currentSocket.CleanedUp)
                        {
                            socketError = SocketError.OperationAborted;
                        }
                        else
                        {
                            try
                            {
                                // The Async IO completed with a failure.
                                // here we need to call WSAGetOverlappedResult() just so Marshal.GetLastWin32Error() will return the correct error.
                                bool success = Interop.Winsock.WSAGetOverlappedResult(
                                    _currentSocket.SafeHandle,
                                    _ptrNativeOverlapped,
                                    out numBytes,
                                    false,
                                    out socketFlags);
                                socketError = SocketPal.GetLastSocketError();
                            }
                            catch
                            {
                                // _currentSocket.CleanedUp check above does not always work since this code is subject to race conditions.
                                socketError = SocketError.OperationAborted;
                            }
                        }
                    }
                    FinishOperationAsyncFailure(socketError, (int)numBytes, socketFlags);
                }

#if DEBUG
                GlobalLog.Leave("CompletionPortCallback");
            }
#endif
        }
    }
}
