// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    public partial class SocketAsyncEventArgs : EventArgs, IDisposable
    {
        // Struct sizes needed for some custom marshaling.
        internal static readonly int s_controlDataSize = Marshal.SizeOf<Interop.Winsock.ControlData>();
        internal static readonly int s_controlDataIPv6Size = Marshal.SizeOf<Interop.Winsock.ControlDataIPv6>();

        // Single buffer GCHandle
        private GCHandle _singleBufferGCHandle;

        // Multiple buffer GCHandle list
        private List<GCHandle> _multipleBufferGCHandles;
        
        // Internal buffers for WSARecvMsg
        private byte[] _controlBuffer;
        private GCHandle _controlBufferGCHandle;

        // SocketAddress GCHandle
        private GCHandle _socketAddressBufferGCHandle;

        // SendPacketsElements property variables.
        private SendPacketsElement[] _sendPacketsElementsInternal;
        private int _sendPacketsElementsFileCount;
        private int _sendPacketsElementsBufferCount;

        // Internal variables for SendPackets
        private FileStream[] _sendPacketsFileStreams;
        private SafeHandle[] _sendPacketsFileHandles;

        // Overlapped object related variables.
        private SafeNativeOverlapped _ptrNativeOverlapped;
        private PreAllocatedOverlapped _preAllocatedOverlapped;

        private enum PinState
        {
            NoBuffer = 0,
            SingleAcceptBuffer,
            SingleBuffer,
            MultipleBuffer,
            SendPackets
        }
        private PinState _pinState;

        internal int? SendPacketsDescriptorCount
        {
            get
            {
                return _sendPacketsElementsInternal == null ? null : (int?)_sendPacketsElementsFileCount + _sendPacketsElementsBufferCount;
            }
        }

        private void InitializeInternals()
        {
            // Zero tells TransmitPackets to select a default send size.
            _sendPacketsSendSize = 0;

            AllocateOverlapped();
        }

        private void FreeInternals()
        {
            ClearPinState();

            // Free the overlapped object.
            if (_ptrNativeOverlapped != null && !_ptrNativeOverlapped.IsInvalid)
            {
                _ptrNativeOverlapped.Dispose();
                _ptrNativeOverlapped = null;
            }

            // Dispose the pre-allocated overlapped
            Debug.Assert(_preAllocatedOverlapped != null);
            _preAllocatedOverlapped.Dispose();
            _preAllocatedOverlapped = null;

            // Unpin socketAddress, if necessary
            if (_socketAddressBufferGCHandle.IsAllocated)
            {
                Debug.Assert(_socketAddressBufferGCHandle.Target == _socketAddress.Buffer);
                _socketAddressBufferGCHandle.Free();
            }

            // Unpin controlBuffer, if necessary
            if (_controlBufferGCHandle.IsAllocated)
            {
                Debug.Assert(_controlBufferGCHandle.Target == _controlBuffer);
                _controlBufferGCHandle.Free();
            }
        }

        private unsafe void AllocateOverlapped()
        {
            _preAllocatedOverlapped = new PreAllocatedOverlapped(CompletionPortCallback, this, null);
        }

        private void ClearPinState()
        {
            // Our buffer state has changed.
            // Unpin any buffers we have pinned currently.
            switch (_pinState)
            {
                case PinState.NoBuffer:
                    break;

                case PinState.SingleBuffer:
                case PinState.SingleAcceptBuffer:
                    Debug.Assert(_singleBufferGCHandle.IsAllocated);
                    _singleBufferGCHandle.Free();
                    break;

                case PinState.MultipleBuffer:
                case PinState.SendPackets:
                    for (int i = 0; i < _multipleBufferGCHandles.Count; i++)
                    {
                        Debug.Assert(_multipleBufferGCHandles[i].IsAllocated);
                        _multipleBufferGCHandles[i].Free();
                    }
                    _multipleBufferGCHandles.Clear();
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            _pinState = PinState.NoBuffer;
        }

        private void SetupSingleBuffer()
        {
            CheckPinSingleBuffer(true);
        }

        private void SetupMultipleBuffers()
        {
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

            ThreadPoolBoundHandle boundHandle = _currentSocket.GetOrAllocateThreadPoolBoundHandle();

            NativeOverlapped* overlapped = boundHandle.AllocateNativeOverlapped(_preAllocatedOverlapped);
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"boundHandle:{boundHandle}, PreAllocatedOverlapped:{_preAllocatedOverlapped}, Returned:{(IntPtr)overlapped}");

            Debug.Assert(overlapped != null, "NativeOverlapped is null.");

            // If we already have a SafeNativeOverlapped SafeHandle and it's associated with the same
            // socket (due to the last operation that used this SocketAsyncEventArgs using the same socket),
            // then we can reuse the same SafeHandle object.  Otherwise, this is either the first operation
            // or the last operation was with a different socket, so create a new SafeHandle.
            if (_ptrNativeOverlapped?.SocketHandle == _currentSocket.SafeHandle)
            {
                _ptrNativeOverlapped.ReplaceHandle(overlapped);
            }
            else
            {
                _ptrNativeOverlapped?.Dispose();
                _ptrNativeOverlapped = new SafeNativeOverlapped(_currentSocket.SafeHandle, overlapped);
            }
        }

        private SocketError ProcessIOCPResult(bool success, int bytesTransferred)
        {
            if (success)
            {
                // Synchronous success.
                if (_currentSocket.SafeHandle.SkipCompletionPortOnSuccess)
                {
                    // The socket handle is configured to skip completion on success, 
                    // so we can set the results right now.
                    FinishOperationSyncSuccess(bytesTransferred, SocketFlags.None);
                    return SocketError.Success;
                }

                // Socket handle is going to post a completion to the completion port (may have done so already).
                // Return pending and we will continue in the completion port callback.
                return SocketError.IOPending;
            }

            // Get the socket error (which may be IOPending)
            SocketError errorCode = SocketPal.GetLastSocketError();

            if (errorCode == SocketError.IOPending)
            {
                return errorCode;
            }

            FinishOperationSyncFailure(errorCode, bytesTransferred, SocketFlags.None);

            // Note, the overlapped will be release in CompleteIOCPOperation below, for either success or failure
            return errorCode;
        }

        private void CompleteIOCPOperation()
        {
            // Required to allow another IOCP operation for the same handle.  We release the native overlapped
            // in the safe handle, but keep the safe handle object around so as to be able to reuse it
            // for other operations.
            _ptrNativeOverlapped?.FreeNativeOverlapped();
        }

        private static unsafe byte* GetPinnedBufferPointer(byte[] buffer, int offset, GCHandle gcHandle)
        {
            Debug.Assert(gcHandle.IsAllocated);
            Debug.Assert(gcHandle.Target == buffer);

            return (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset);
        }

        private unsafe byte* GetPinnedSingleBufferPointer()
        {
            return GetPinnedBufferPointer(_buffer, _offset, _singleBufferGCHandle);
        }

        private unsafe byte* GetPinnedAcceptBufferPointer()
        {
            return GetPinnedBufferPointer(_acceptBuffer, 0, _singleBufferGCHandle);
        }

        private unsafe void PopulateWSABuffersArray(WSABuffer* wsaBuffers)
        {
            Debug.Assert(_pinState == PinState.MultipleBuffer);

            int bufferCount = _bufferListInternal.Count;
            for (int i = 0; i < bufferCount; i++)
            {
                ArraySegment<byte> buffer = _bufferListInternal[i];
                wsaBuffers[i] = new WSABuffer(
                    GetPinnedBufferPointer(buffer.Array, buffer.Offset, _multipleBufferGCHandles[i]),
                    buffer.Count);
            }
        }

        private void InnerStartOperationAccept(bool userSuppliedBuffer)
        {
            if (!userSuppliedBuffer)
            {
                CheckPinSingleBuffer(false);
            }
        }

        internal unsafe SocketError DoOperationAccept(Socket socket, SafeCloseSocket handle, SafeCloseSocket acceptHandle)
        {
            PrepareIOCPOperation();

            byte* bufferPtr;
            int receiveDataLength = 0;
            if (_buffer == null)
            {
                bufferPtr = GetPinnedAcceptBufferPointer();
            }
            else
            {
                bufferPtr = GetPinnedSingleBufferPointer();
                receiveDataLength = _count - _acceptAddressBufferCount;
            }

            int bytesTransferred;
            bool success = socket.AcceptEx(
                handle,
                acceptHandle,
                bufferPtr,
                receiveDataLength,
                _acceptAddressBufferCount / 2,
                _acceptAddressBufferCount / 2,
                &bytesTransferred,
                _ptrNativeOverlapped);

            return ProcessIOCPResult(success, bytesTransferred);
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

        internal unsafe SocketError DoOperationConnect(Socket socket, SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int bytesTransferred;

            byte* bufferPtr = null;
            int count = 0;
            if (_buffer != null)
            {
                bufferPtr = GetPinnedSingleBufferPointer();
                count = _count;
            }

            bool success = socket.ConnectEx(
                handle,
                GetSocketAddressBufferPointer(),
                _socketAddress.Size,
                bufferPtr,
                count,
                &bytesTransferred,
                _ptrNativeOverlapped);

            return ProcessIOCPResult(success, bytesTransferred);
        }

        private void InnerStartOperationDisconnect()
        {
            CheckPinNoBuffer();
        }

        internal SocketError DoOperationDisconnect(Socket socket, SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            bool success = socket.DisconnectEx(
                    handle,
                    _ptrNativeOverlapped,
                    (int)(DisconnectReuseSocket ? TransmitFileOptions.ReuseSocket : 0),
                    0);

            return ProcessIOCPResult(success, 0);
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

        internal unsafe SocketError DoOperationReceive(SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int result;
            int bytesTransferred;
            SocketFlags flags = _socketFlags;
            if (_buffer != null)
            {
                // Single buffer case.
                WSABuffer wsaBuffer = new WSABuffer(GetPinnedSingleBufferPointer(), _count);
                result = Interop.Winsock.WSARecv(
                    handle,
                    &wsaBuffer,
                    1,
                    &bytesTransferred,
                    &flags,
                    _ptrNativeOverlapped,
                    null);
            }
            else
            {
                // Multi buffer case.
                int bufferCount = _bufferListInternal.Count;
                if (bufferCount <= WSABuffer.StackAllocLimit)
                {
                    WSABuffer* wsaBuffers = stackalloc WSABuffer[bufferCount];
                    PopulateWSABuffersArray(wsaBuffers);

                    result = Interop.Winsock.WSARecv(
                        handle,
                        wsaBuffers,
                        bufferCount,
                        &bytesTransferred,
                        &flags,
                        _ptrNativeOverlapped,
                        null);
                }
                else
                {
                    WSABuffer[] wsaHeapBuffers = new WSABuffer[bufferCount];
                    fixed (WSABuffer* wsaBuffers = wsaHeapBuffers)
                    {
                        PopulateWSABuffersArray(wsaBuffers);

                        result = Interop.Winsock.WSARecv(
                            handle,
                            wsaBuffers,
                            bufferCount,
                            &bytesTransferred,
                            &flags,
                            _ptrNativeOverlapped,
                            null);
                    }
                }
            }

            return ProcessIOCPResult(result == 0, bytesTransferred);
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

        internal unsafe SocketError DoOperationReceiveFrom(SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int result;
            int bytesTransferred;
            SocketFlags flags = _socketFlags;
            if (_buffer != null)
            {
                // Single buffer case.
                WSABuffer wsaBuffer = new WSABuffer(GetPinnedSingleBufferPointer(), _count);
                result = Interop.Winsock.WSARecvFrom(
                    handle,
                    &wsaBuffer,
                    1,
                    &bytesTransferred,
                    &flags,
                    GetSocketAddressBufferPointer(),
                    GetSocketAddressSizePointer(),
                    _ptrNativeOverlapped,
                    null);
            }
            else
            {
                // Multi buffer case.
                int bufferCount = _bufferListInternal.Count;
                if (bufferCount <= WSABuffer.StackAllocLimit)
                {
                    WSABuffer* wsaBuffers = stackalloc WSABuffer[bufferCount];
                    PopulateWSABuffersArray(wsaBuffers);

                    result = Interop.Winsock.WSARecvFrom(
                        handle,
                        wsaBuffers,
                        bufferCount,
                        &bytesTransferred,
                        &flags,
                        GetSocketAddressBufferPointer(),
                        GetSocketAddressSizePointer(),
                        _ptrNativeOverlapped,
                        null);
                }
                else
                {
                    WSABuffer[] wsaHeapBuffers = new WSABuffer[bufferCount];
                    fixed (WSABuffer* wsaBuffers = wsaHeapBuffers)
                    {
                        PopulateWSABuffersArray(wsaBuffers);

                        result = Interop.Winsock.WSARecvFrom(
                            handle,
                            wsaBuffers,
                            bufferCount,
                            &bytesTransferred,
                            &flags,
                            GetSocketAddressBufferPointer(),
                            GetSocketAddressSizePointer(),
                            _ptrNativeOverlapped,
                            null);
                    }                    
                }
            }

            return ProcessIOCPResult(result == 0, bytesTransferred);
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
                _controlBufferGCHandle = GCHandle.Alloc(_controlBuffer, GCHandleType.Pinned);
            }
            else if (ipv6 && (_controlBuffer == null || _controlBuffer.Length != s_controlDataIPv6Size))
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = new byte[s_controlDataIPv6Size];
                _controlBufferGCHandle = GCHandle.Alloc(_controlBuffer, GCHandleType.Pinned);
            }
            else
            {
                if (_controlBufferGCHandle.IsAllocated)
                {
                    _controlBufferGCHandle.Free();
                }
                _controlBuffer = null;
            }
        }

        internal unsafe SocketError DoOperationReceiveMessageFrom(Socket socket, SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int result;
            int bytesTransferred;

            WSABuffer wsaControlBuffer = (_controlBuffer == null ? default(WSABuffer) :
                new WSABuffer(GetPinnedBufferPointer(_controlBuffer, 0, _controlBufferGCHandle), _controlBuffer.Length));

            if (_buffer != null)
            {
                // Single buffer case.
                WSABuffer wsaBuffer = new WSABuffer(GetPinnedSingleBufferPointer(), _count);
                Interop.Winsock.WSAMsg wsaMsg = new Interop.Winsock.WSAMsg(
                    GetSocketAddressBufferPointer(),
                    _socketAddress.Size,
                    &wsaBuffer, 
                    1,
                    wsaControlBuffer,
                    _socketFlags);

                result = socket.WSARecvMsg(
                    handle,
                    &wsaMsg,
                    &bytesTransferred,
                    _ptrNativeOverlapped,
                    null);
            }
            else
            {
                // Multi buffer case.
                int bufferCount = _bufferListInternal.Count;
                if (bufferCount <= WSABuffer.StackAllocLimit)
                {
                    WSABuffer* wsaBuffers = stackalloc WSABuffer[bufferCount];
                    PopulateWSABuffersArray(wsaBuffers);
                    Interop.Winsock.WSAMsg wsaMsg = new Interop.Winsock.WSAMsg(
                        GetSocketAddressBufferPointer(),
                        _socketAddress.Size,
                            wsaBuffers, 
                        bufferCount,
                        wsaControlBuffer,
                        _socketFlags);

                    result = socket.WSARecvMsg(
                        handle,
                        &wsaMsg,
                        &bytesTransferred,
                        _ptrNativeOverlapped,
                        null);
                }
                else
                {
                    WSABuffer[] wsaHeapBuffers = new WSABuffer[bufferCount];
                    fixed (WSABuffer* wsaBuffers = wsaHeapBuffers)
                    {
                        PopulateWSABuffersArray(wsaBuffers);
                        Interop.Winsock.WSAMsg wsaMsg = new Interop.Winsock.WSAMsg(
                            GetSocketAddressBufferPointer(),
                            _socketAddress.Size,
                            wsaBuffers, 
                            bufferCount,
                            wsaControlBuffer,
                            _socketFlags);

                        result = socket.WSARecvMsg(
                            handle,
                            &wsaMsg,
                            &bytesTransferred,
                            _ptrNativeOverlapped,
                            null);
                    }
                }
            }

            return ProcessIOCPResult(result == 0, bytesTransferred);
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

        internal unsafe SocketError DoOperationSend(SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int result;
            int bytesTransferred;
            if (_buffer != null)
            {
                // Single buffer case.
                WSABuffer wsaBuffer = new WSABuffer(GetPinnedSingleBufferPointer(), _count);
                result = Interop.Winsock.WSASend(
                    handle,
                    &wsaBuffer,
                    1,
                    &bytesTransferred,
                    _socketFlags,
                    _ptrNativeOverlapped,
                    null);
            }
            else
            {
                // Multi buffer case.
                int bufferCount = _bufferListInternal.Count;
                if (bufferCount <= WSABuffer.StackAllocLimit)
                {
                    WSABuffer* wsaBuffers = stackalloc WSABuffer[bufferCount];
                    PopulateWSABuffersArray(wsaBuffers);

                    result = Interop.Winsock.WSASend(
                        handle,
                        wsaBuffers,
                        bufferCount,
                        &bytesTransferred,
                        _socketFlags,
                        _ptrNativeOverlapped,
                        null);
                }
                else
                {
                    WSABuffer[] wsaHeapBuffers = new WSABuffer[bufferCount];
                    fixed (WSABuffer* wsaBuffers = wsaHeapBuffers)
                    {
                        PopulateWSABuffersArray(wsaBuffers);

                        result = Interop.Winsock.WSASend(
                            handle,
                            wsaBuffers,
                            bufferCount,
                            &bytesTransferred,
                            _socketFlags,
                            _ptrNativeOverlapped,
                            null);
                    }
                }
            }

            return ProcessIOCPResult(result == 0, bytesTransferred);
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

        private unsafe void PopulateTransmitPacketElementsArray(Interop.Winsock.TransmitPacketsElement* packetElements)
        {
            int descriptorIndex = 0;
            int fileIndex = 0;
            int bufferIndex = 0;
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe._buffer != null && spe._count > 0)
                    {
                        // This element is a buffer.
                        packetElements[descriptorIndex].buffer = (byte*) GetPinnedBufferPointer(spe._buffer, spe._offset, _multipleBufferGCHandles[bufferIndex]);
                        packetElements[descriptorIndex].length = (uint)spe._count;
                        packetElements[descriptorIndex].flags = (Interop.Winsock.TransmitPacketsElementFlags)spe._flags;
                        bufferIndex++;
                        descriptorIndex++;
                    }
                    else if (spe._filePath != null)
                    {
                        // This element is a file.
                        packetElements[descriptorIndex].fileHandle = _sendPacketsFileHandles[fileIndex].DangerousGetHandle();
                        packetElements[descriptorIndex].fileOffset = spe._offset;
                        packetElements[descriptorIndex].length = (uint)spe._count;
                        packetElements[descriptorIndex].flags = (Interop.Winsock.TransmitPacketsElementFlags)spe._flags;
                        fileIndex++;
                        descriptorIndex++;
                    }
                }
            }
        }

        internal unsafe SocketError DoOperationSendPackets(Socket socket, SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            bool result;

            int elementCount = _sendPacketsElementsFileCount + _sendPacketsElementsBufferCount;
            if (elementCount <= Interop.Winsock.TransmitPacketsElement.StackAllocLimit)
            {
                Interop.Winsock.TransmitPacketsElement* packetElements = stackalloc Interop.Winsock.TransmitPacketsElement[elementCount];
                PopulateTransmitPacketElementsArray(packetElements);

                result = socket.TransmitPackets(
                    handle,
                    packetElements,
                    elementCount,
                    _sendPacketsSendSize,
                    _ptrNativeOverlapped,
                    _sendPacketsFlags);
            }
            else
            {
                Interop.Winsock.TransmitPacketsElement[] packetElementsHeap = new Interop.Winsock.TransmitPacketsElement[elementCount];
                fixed (Interop.Winsock.TransmitPacketsElement* packetElements = packetElementsHeap)
                {
                    PopulateTransmitPacketElementsArray(packetElements);

                    result = socket.TransmitPackets(
                        handle,
                        packetElements,
                        elementCount,
                        _sendPacketsSendSize,
                        _ptrNativeOverlapped,
                        _sendPacketsFlags);
                }
            }

            return ProcessIOCPResult(result, 0);
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

        internal unsafe SocketError DoOperationSendTo(SafeCloseSocket handle)
        {
            PrepareIOCPOperation();

            int result;
            int bytesTransferred;
            if (_buffer != null)
            {
                // Single buffer case.
                WSABuffer wsaBuffer = new WSABuffer(GetPinnedSingleBufferPointer(), _count);
                result = Interop.Winsock.WSASendTo(
                    handle,
                    &wsaBuffer,
                    1,
                    &bytesTransferred,
                    _socketFlags,
                    GetSocketAddressBufferPointer(),
                    _socketAddress.Size,
                    _ptrNativeOverlapped,
                    null);
            }
            else
            {
                // Multi buffer case.
                int bufferCount = _bufferListInternal.Count;
                if (bufferCount <= WSABuffer.StackAllocLimit)
                {
                    WSABuffer* wsaBuffers = stackalloc WSABuffer[bufferCount];
                    PopulateWSABuffersArray(wsaBuffers);

                    result = Interop.Winsock.WSASendTo(
                        handle,
                        wsaBuffers,
                        bufferCount,
                        &bytesTransferred,
                        _socketFlags,
                        GetSocketAddressBufferPointer(),
                        _socketAddress.Size,
                        _ptrNativeOverlapped,
                        null);
                }
                else
                {
                    WSABuffer[] wsaHeapBuffers = new WSABuffer[bufferCount];
                    fixed (WSABuffer* wsaBuffers = wsaHeapBuffers)
                    {
                        PopulateWSABuffersArray(wsaBuffers);

                        result = Interop.Winsock.WSASendTo(
                            handle,
                            wsaBuffers,
                            bufferCount,
                            &bytesTransferred,
                            _socketFlags,
                            GetSocketAddressBufferPointer(),
                            _socketAddress.Size,
                            _ptrNativeOverlapped,
                            null);
                    }
                }
            }

            return ProcessIOCPResult(result == 0, bytesTransferred);
        }

        private void CheckPinNoBuffer()
        {
            if (_pinState == PinState.NoBuffer)
            {
                PinSingleBuffer(true);
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
                        ClearPinState();
                    }
                }
                else
                {
                    if (_pinState == PinState.SingleBuffer)
                    {
                        Debug.Assert(_singleBufferGCHandle.IsAllocated);
                        if (_singleBufferGCHandle.Target == _buffer)
                        {
                            // This buffer is already pinned 
                            return;
                        }
                    }

                    ClearPinState();
                    PinSingleBuffer(true);
                }
            }
            else
            {
                if (_pinState == PinState.SingleAcceptBuffer)
                {
                    Debug.Assert(_singleBufferGCHandle.IsAllocated);
                    if (_singleBufferGCHandle.Target == _acceptBuffer)
                    {
                        // Accept buffer is already pinned
                        return;
                    }
                }

                ClearPinState();
                PinSingleBuffer(false);
            }
        }

        // Ensures Overlapped object exists with appropriate multiple buffers pinned.
        private void CheckPinMultipleBuffers()
        {
            if (_bufferListInternal == null || _bufferListInternal.Count == 0)
            {
                // No buffer list is set so unpin any existing multiple buffer pinning.
                if (_pinState == PinState.MultipleBuffer)
                {
                    ClearPinState();
                }
            }
            else
            {
                // Need to setup a new Overlapped.
                ClearPinState();
                try
                {
                    PinMultipleBuffers();
                }
                catch (Exception)
                {
                    ClearPinState();
                    throw;
                }
            }
        }

        // Ensures Overlapped object exists with appropriate buffers pinned.
        private void CheckPinSendPackets()
        {
            if (_pinState != PinState.NoBuffer)
            {
                ClearPinState();
            }
            PinSendPacketsBuffers();
        }

        // Ensures appropriate SocketAddress buffer is pinned.
        private void PinSocketAddressBuffer()
        {
            // Check if already pinned.
            if (_socketAddressBufferGCHandle.IsAllocated)
            {
                if (_socketAddressBufferGCHandle.Target == _socketAddress)
                {
                    // Already pinned 
                    return;
                }

                // Unpin previous
                _socketAddressBufferGCHandle.Free();
            }

            // Pin down the new one.
            _socketAddressBufferGCHandle = GCHandle.Alloc(_socketAddress.Buffer, GCHandleType.Pinned);
            _socketAddress.CopyAddressSizeIntoBuffer();
        }

        private unsafe void PinSingleBuffer(bool useSingleBuffer)
        {
            // Pin buffer, get native pointers, and fill in WSABuffer descriptor.
            if (useSingleBuffer)
            {
                if (_buffer != null)
                {
                    Debug.Assert(!_singleBufferGCHandle.IsAllocated);
                    _singleBufferGCHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

                    _pinState = PinState.SingleBuffer;
                }
                else
                {
                    _pinState = PinState.NoBuffer;
                }
            }
            else
            {
                // Indicates we should use the accept buffer
                Debug.Assert(!_singleBufferGCHandle.IsAllocated);
                _singleBufferGCHandle = GCHandle.Alloc(_acceptBuffer, GCHandleType.Pinned);

                _pinState = PinState.SingleAcceptBuffer;
            }
        }

        private unsafe void PinMultipleBuffers()
        {
            int bufferCount = _bufferListInternal.Count;

            if (_multipleBufferGCHandles == null)
            {
                _multipleBufferGCHandles = new List<GCHandle>(bufferCount);
            }
            else
            {
                _multipleBufferGCHandles.Clear();
            }

            // Pin buffers.
            for (int i = 0; i < bufferCount; i++)
            {
                _multipleBufferGCHandles.Add(GCHandle.Alloc(_bufferListInternal[i].Array, GCHandleType.Pinned));
            }

            _pinState = PinState.MultipleBuffer;
        }

        // Sets up an Overlapped object for SendPacketsAsync.
        private unsafe void PinSendPacketsBuffers()
        {
            int bufferCount = _sendPacketsElementsBufferCount;

            if (_multipleBufferGCHandles == null)
            {
                _multipleBufferGCHandles = new List<GCHandle>(bufferCount);
            }
            else
            {
                _multipleBufferGCHandles.Clear();
            }

            // Pin user specified buffers.
            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null && spe._buffer != null && spe._count > 0)
                {
                    _multipleBufferGCHandles.Add(GCHandle.Alloc(spe._buffer, GCHandleType.Pinned));
                }
            }

            _pinState = PinState.SendPackets;
        }

        private unsafe byte* GetSocketAddressBufferPointer()
        {
            return GetPinnedBufferPointer(_socketAddress.Buffer, 0, _socketAddressBufferGCHandle);
        }

        private unsafe int* GetSocketAddressSizePointer()
        {
            return (int*)GetPinnedBufferPointer(_socketAddress.Buffer, _socketAddress.GetAddressSizeOffset(), _socketAddressBufferGCHandle);
        }

        private unsafe int GetSocketAddressSize()
        {
            return *(GetSocketAddressSizePointer());
        }

        internal void LogBuffer(int size)
        {
            if (!NetEventSource.IsEnabled) return;

            switch (_pinState)
            {
                case PinState.SingleAcceptBuffer:
                    NetEventSource.DumpBuffer(this, _acceptBuffer, 0, size);
                    break;

                case PinState.SingleBuffer:
                    NetEventSource.DumpBuffer(this, _buffer, _offset, size);
                    break;

                case PinState.MultipleBuffer:
                    foreach (ArraySegment<byte> buffer in _bufferListInternal)
                    {
                        NetEventSource.DumpBuffer(this, buffer.Array, buffer.Offset, Math.Min(buffer.Count, size));
                        if ((size -= buffer.Count) <= 0)
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
            if (!NetEventSource.IsEnabled) return;

            foreach (SendPacketsElement spe in _sendPacketsElementsInternal)
            {
                if (spe != null)
                {
                    if (spe._buffer != null && spe._count > 0)
                    {
                        // This element is a buffer.
                        NetEventSource.DumpBuffer(this, spe._buffer, spe._offset, Math.Min(spe._count, size));
                    }
                    else if (spe._filePath != null)
                    {
                        // This element is a file.
                        NetEventSource.NotLoggedFile(spe._filePath, _currentSocket, _completedOperation);
                    }
                }
            }
        }

        private unsafe SocketError FinishOperationAccept(Internals.SocketAddress remoteSocketAddress)
        {
            SocketError socketError;
            byte* localAddr;
            int localAddrLength;
            byte* remoteAddr;
            int remoteAddrLength;

            try
            {
                byte* bufferPtr;
                int receiveDataLength = 0;
                if (_buffer == null)
                {
                    bufferPtr = GetPinnedAcceptBufferPointer();
                }
                else
                {
                    bufferPtr = GetPinnedSingleBufferPointer();
                    receiveDataLength = _count - _acceptAddressBufferCount;
                }

                _currentSocket.GetAcceptExSockaddrs(
                    bufferPtr,
                    receiveDataLength,
                    _acceptAddressBufferCount / 2,
                    _acceptAddressBufferCount / 2,
                    &localAddr,
                    &localAddrLength,
                    &remoteAddr,
                    &remoteAddrLength);

                remoteSocketAddress.InternalSize = remoteAddrLength;
                Marshal.Copy((IntPtr)remoteAddr, remoteSocketAddress.Buffer, 0, remoteSocketAddress.Size);

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

        private unsafe void FinishOperationReceiveMessageFrom()
        {
            byte* controlBufferPtr = GetPinnedBufferPointer(_controlBuffer, 0, _controlBufferGCHandle);

            if (_controlBuffer.Length == s_controlDataSize)
            {
                // IPv4.
                _receiveMessageFromPacketInfo = SocketPal.GetIPPacketInformation((Interop.Winsock.ControlData*)controlBufferPtr);
            }
            else if (_controlBuffer.Length == s_controlDataIPv6Size)
            {
                // IPv6.
                _receiveMessageFromPacketInfo = SocketPal.GetIPPacketInformation((Interop.Winsock.ControlDataIPv6*)controlBufferPtr);
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
            DebugThreadTracking.SetThreadSource(ThreadKinds.CompletionPort);
            using (DebugThreadTracking.SetThreadKind(ThreadKinds.System))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Enter(this, $"errorCode:{errorCode}, numBytes:{numBytes}, overlapped:{(IntPtr)nativeOverlapped}");
#endif
                SocketFlags socketFlags = SocketFlags.None;
                SocketError socketError = (SocketError)errorCode;

                // This is the same NativeOverlapped* as we already have a SafeHandle for, re-use the original.
                Debug.Assert((IntPtr)nativeOverlapped == _ptrNativeOverlapped.DangerousGetHandle(), "Handle mismatch");

                if (socketError == SocketError.Success)
                {
                    FinishOperationAsyncSuccess((int)numBytes, SocketFlags.None);
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
                                // here we need to call WSAGetOverlappedResult() just so GetLastSocketError() will return the correct error.
                                bool success = Interop.Winsock.WSAGetOverlappedResult(
                                    _currentSocket.SafeHandle,
                                    _ptrNativeOverlapped,
                                    &numBytes,
                                    false,
                                    &socketFlags);

                                if (success)
                                {
                                    NetEventSource.Fail(this, $"WSAGetOverlappedResult unexpectedly succeeded. errorCode:{errorCode} numBytes:{numBytes}");
                                }

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
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
            }
#endif
        }
    }
}
