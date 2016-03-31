// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Net.Sockets
{
    public partial class SocketAsyncEventArgs : EventArgs, IDisposable
    {
        private int _acceptedFileDescriptor;
        private int _socketAddressSize;
        private SocketFlags _receivedFlags;

        internal int? SendPacketsDescriptorCount { get { return null; } }

        private void InitializeInternals()
        {
            // No-op for *nix.
        }

        private void FreeInternals(bool calledFromFinalizer)
        {
            // No-op for *nix.
        }

        private void SetupSingleBuffer()
        {
            // No-op for *nix.
        }

        private void SetupMultipleBuffers()
        {
            // No-op for *nix.
        }

        private void SetupSendPacketsElements()
        {
            // No-op for *nix.
        }

        private void InnerComplete()
        {
            // No-op for *nix.
        }

        private void InnerStartOperationAccept(bool userSuppliedBuffer)
        {
            _acceptedFileDescriptor = -1;
        }

        private void AcceptCompletionCallback(int acceptedFileDescriptor, byte[] socketAddress, int socketAddressSize, SocketError socketError)
        {
            // TODO: receive bytes on socket if requested

            _acceptedFileDescriptor = acceptedFileDescriptor;
            Debug.Assert(socketAddress == null || socketAddress == _acceptBuffer, $"Unexpected socketAddress: {socketAddress}");
            _acceptAddressBufferCount = socketAddressSize;

            CompletionCallback(0, socketError);
        }

        internal unsafe SocketError DoOperationAccept(Socket socket, SafeCloseSocket handle, SafeCloseSocket acceptHandle, out int bytesTransferred)
        {
            Debug.Assert(acceptHandle == null, $"Unexpected acceptHandle: {acceptHandle}");

            bytesTransferred = 0;

            return handle.AsyncContext.AcceptAsync(_buffer ?? _acceptBuffer, _acceptAddressBufferCount / 2, AcceptCompletionCallback);
        }

        private void InnerStartOperationConnect()
        {
            // No-op for *nix.
        }

        private void ConnectCompletionCallback(SocketError socketError)
        {
            CompletionCallback(0, socketError);
        }

        internal unsafe SocketError DoOperationConnect(Socket socket, SafeCloseSocket handle, out int bytesTransferred)
        {
            bytesTransferred = 0;

            return handle.AsyncContext.ConnectAsync(_socketAddress.Buffer, _socketAddress.Size, ConnectCompletionCallback);
        }

        private void InnerStartOperationDisconnect()
        {
            throw new PlatformNotSupportedException();
        }

        private void TransferCompletionCallback(int bytesTransferred, byte[] socketAddress, int socketAddressSize, SocketFlags receivedFlags, SocketError socketError)
        {
            Debug.Assert(socketAddress == null || socketAddress == _socketAddress.Buffer, $"Unexpected socketAddress: {socketAddress}");
            _socketAddressSize = socketAddressSize;
            _receivedFlags = receivedFlags;

            CompletionCallback(bytesTransferred, socketError);
        }

        private void InnerStartOperationReceive()
        {
            _receivedFlags = System.Net.Sockets.SocketFlags.None;
            _socketAddressSize = 0;
        }

        internal unsafe SocketError DoOperationReceive(SafeCloseSocket handle, out SocketFlags flags, out int bytesTransferred)
        {
            SocketError errorCode;
            if (_buffer != null)
            {
                errorCode = handle.AsyncContext.ReceiveAsync(_buffer, _offset, _count, _socketFlags, TransferCompletionCallback);
            }
            else
            {
                errorCode = handle.AsyncContext.ReceiveAsync(_bufferList, _socketFlags, TransferCompletionCallback);
            }

            flags = _socketFlags;
            bytesTransferred = 0;
            return errorCode;
        }

        private void InnerStartOperationReceiveFrom()
        {
            _receivedFlags = System.Net.Sockets.SocketFlags.None;
            _socketAddressSize = 0;
        }

        internal unsafe SocketError DoOperationReceiveFrom(SafeCloseSocket handle, out SocketFlags flags, out int bytesTransferred)
        {
            SocketError errorCode;
            if (_buffer != null)
            {
                errorCode = handle.AsyncContext.ReceiveFromAsync(_buffer, _offset, _count, _socketFlags, _socketAddress.Buffer, _socketAddress.Size, TransferCompletionCallback);
            }
            else
            {
                errorCode = handle.AsyncContext.ReceiveFromAsync(_bufferList, _socketFlags, _socketAddress.Buffer, _socketAddress.Size, TransferCompletionCallback);
            }

            flags = _socketFlags;
            bytesTransferred = 0;
            return errorCode;
        }

        private void InnerStartOperationReceiveMessageFrom()
        {
            _receiveMessageFromPacketInfo = default(IPPacketInformation);
            _receivedFlags = System.Net.Sockets.SocketFlags.None;
            _socketAddressSize = 0;
        }

        private void ReceiveMessageFromCompletionCallback(int bytesTransferred, byte[] socketAddress, int socketAddressSize, SocketFlags receivedFlags, IPPacketInformation ipPacketInformation, SocketError errorCode)
        {
            Debug.Assert(_socketAddress != null, "Expected non-null _socketAddress");
            Debug.Assert(socketAddress == null || _socketAddress.Buffer == socketAddress, $"Unexpected socketAddress: {socketAddress}");

            _socketAddressSize = socketAddressSize;
            _receivedFlags = receivedFlags;
            _receiveMessageFromPacketInfo = ipPacketInformation;

            CompletionCallback(bytesTransferred, errorCode);
        }

        internal unsafe SocketError DoOperationReceiveMessageFrom(Socket socket, SafeCloseSocket handle, out int bytesTransferred)
        {
            bool isIPv4, isIPv6;
            Socket.GetIPProtocolInformation(socket.AddressFamily, _socketAddress, out isIPv4, out isIPv6);

            bytesTransferred = 0;
            return handle.AsyncContext.ReceiveMessageFromAsync(_buffer, _offset, _count, _socketFlags, _socketAddress.Buffer, _socketAddress.Size, isIPv4, isIPv6, ReceiveMessageFromCompletionCallback);
        }

        private void InnerStartOperationSend()
        {
            _receivedFlags = System.Net.Sockets.SocketFlags.None;
            _socketAddressSize = 0;
        }

        internal unsafe SocketError DoOperationSend(SafeCloseSocket handle, out int bytesTransferred)
        {
            SocketError errorCode;
            if (_buffer != null)
            {
                errorCode = handle.AsyncContext.SendAsync(_buffer, _offset, _count, _socketFlags, TransferCompletionCallback);
            }
            else
            {
                errorCode = handle.AsyncContext.SendAsync(_bufferList, _socketFlags, TransferCompletionCallback);
            }

            bytesTransferred = 0;
            return errorCode;
        }

        private void InnerStartOperationSendPackets()
        {
            throw new PlatformNotSupportedException();
        }

        internal SocketError DoOperationSendPackets(Socket socket, SafeCloseSocket handle)
        {
            throw new PlatformNotSupportedException();
        }

        private void InnerStartOperationSendTo()
        {
            _receivedFlags = System.Net.Sockets.SocketFlags.None;
            _socketAddressSize = 0;
        }

        internal SocketError DoOperationSendTo(SafeCloseSocket handle, out int bytesTransferred)
        {
            SocketError errorCode;
            if (_buffer != null)
            {
                errorCode = handle.AsyncContext.SendToAsync(_buffer, _offset, _count, _socketFlags, _socketAddress.Buffer, _socketAddress.Size, TransferCompletionCallback);
            }
            else
            {
                errorCode = handle.AsyncContext.SendToAsync(_bufferList, _socketFlags, _socketAddress.Buffer, _socketAddress.Size, TransferCompletionCallback);
            }

            bytesTransferred = 0;
            return errorCode;
        }

        internal void LogBuffer(int size)
        {
            // TODO: implement?
        }

        internal void LogSendPacketsBuffers(int size)
        {
            throw new PlatformNotSupportedException();
        }

        private SocketError FinishOperationAccept(Internals.SocketAddress remoteSocketAddress)
        {
            System.Buffer.BlockCopy(_acceptBuffer, 0, remoteSocketAddress.Buffer, 0, _acceptAddressBufferCount);
            _acceptSocket = _currentSocket.CreateAcceptSocket(
                SafeCloseSocket.CreateSocket(_acceptedFileDescriptor),
                _currentSocket._rightEndPoint.Create(remoteSocketAddress));
            return SocketError.Success;
        }

        private SocketError FinishOperationConnect()
        {
            // No-op for *nix.
            return SocketError.Success;
        }

        private unsafe int GetSocketAddressSize()
        {
            return _socketAddressSize;
        }

        private unsafe void FinishOperationReceiveMessageFrom()
        {
            // No-op for *nix.
        }

        private void FinishOperationSendPackets()
        {
            throw new PlatformNotSupportedException();
        }

        private void CompletionCallback(int bytesTransferred, SocketError socketError)
        {
            // TODO: plumb SocketFlags through TransferOperation
            if (socketError == SocketError.Success)
            {
                FinishOperationSuccess(socketError, bytesTransferred, _receivedFlags);
            }
            else
            {
                if (_currentSocket.CleanedUp)
                {
                    socketError = SocketError.OperationAborted;
                }

                FinishOperationAsyncFailure(socketError, bytesTransferred, _receivedFlags);
            }
        }
    }
}
