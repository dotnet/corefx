// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
    // TODO #4052: Optimize delegate allocations.
    public static class SocketTaskExtensions
    {
        public static Task<Socket> AcceptAsync(this Socket socket)
        {
            return Task<Socket>.Factory.FromAsync(socket.BeginAccept(null, null), socket.EndAccept);
        }

        public static Task<Socket> AcceptAsync(this Socket socket, Socket acceptSocket)
        {
            return Task<Socket>.Factory.FromAsync(socket.BeginAccept(acceptSocket, 0, null, null), socket.EndAccept);
        }

        public static Task ConnectAsync(this Socket socket, EndPoint remoteEndPoint)
        {
            return Task.Factory.FromAsync(socket.BeginConnect(remoteEndPoint, null, null), socket.EndConnect);
        }

        public static Task ConnectAsync(this Socket socket, IPAddress address, int port)
        {
            return Task.Factory.FromAsync(socket.BeginConnect(address, port, null, null), socket.EndConnect);
        }

        public static Task ConnectAsync(this Socket socket, IPAddress[] addresses, int port)
        {
            return Task.Factory.FromAsync(socket.BeginConnect(addresses, port, null, null), socket.EndConnect);
        }

        public static Task ConnectAsync(this Socket socket, string host, int port)
        {
            return Task.Factory.FromAsync(socket.BeginConnect(host, port, null, null), socket.EndConnect);
        }

        public static Task<int> ReceiveAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(
                socket.BeginReceive(buffer.Array, buffer.Offset, buffer.Count, socketFlags, null, null),
                socket.EndReceive);
        }

        public static Task<int> ReceiveAsync(
            this Socket socket, 
            IList<ArraySegment<byte>> buffers, 
            SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(
                socket.BeginReceive(buffers, socketFlags, null, null),
                socket.EndReceive);
        }

        public static Task<SocketReceiveFromResult> ReceiveFromAsync(
            this Socket socket, 
            ArraySegment<byte> buffer, 
            SocketFlags socketFlags, 
            EndPoint remoteEndPoint)
        {
            return Task<SocketReceiveFromResult>.Factory.FromAsync(
                socket.BeginReceiveFrom(
                    buffer.Array, 
                    buffer.Offset, 
                    buffer.Count, 
                    socketFlags, 
                    ref remoteEndPoint, 
                    null, 
                    null),
                asyncResult => {
                    int bytesReceived = socket.EndReceiveFrom(asyncResult, ref remoteEndPoint);

                    return new SocketReceiveFromResult() {
                        ReceivedBytes = bytesReceived,
                        RemoteEndPoint = remoteEndPoint };
                });
        }

        public static Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(
            this Socket socket, 
            ArraySegment<byte> buffer, 
            SocketFlags socketFlags, 
            EndPoint remoteEndPoint)
        {
            return Task<SocketReceiveMessageFromResult>.Factory.FromAsync(
                socket.BeginReceiveMessageFrom(
                    buffer.Array, 
                    buffer.Offset, 
                    buffer.Count, 
                    socketFlags, 
                    ref remoteEndPoint, 
                    null, 
                    null),
                asyncResult => {
                    IPPacketInformation ipPacket;
                    int bytesReceived = socket.EndReceiveMessageFrom(
                        asyncResult, 
                        ref socketFlags, 
                        ref remoteEndPoint, 
                        out ipPacket);

                    return new SocketReceiveMessageFromResult() {
                        PacketInformation = ipPacket,
                        ReceivedBytes = bytesReceived,
                        RemoteEndPoint = remoteEndPoint,
                        SocketFlags = socketFlags };
                });
        }

        public static Task<int> SendAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(
                socket.BeginSend(
                    buffer.Array, 
                    buffer.Offset, 
                    buffer.Count, 
                    socketFlags, 
                    null, 
                    null),
                socket.EndSend);
        }

        public static Task<int> SendAsync(
            this Socket socket, 
            IList<ArraySegment<byte>> buffers, 
            SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(socket.BeginSend(buffers, socketFlags, null, null), socket.EndSend);
        }

        public static Task<int> SendToAsync(
            this Socket socket, 
            ArraySegment<byte> buffer, 
            SocketFlags socketFlags, 
            EndPoint remoteEndPoint)
        {
            return Task<int>.Factory.FromAsync(
                socket.BeginSendTo(
                    buffer.Array, 
                    buffer.Offset, 
                    buffer.Count, 
                    socketFlags, 
                    remoteEndPoint, 
                    null, 
                    null),
                socket.EndSendTo);
        }
    }
}
