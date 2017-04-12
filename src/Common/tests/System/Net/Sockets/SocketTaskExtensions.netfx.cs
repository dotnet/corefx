// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

// TODO #17690: netfx is missing the SocketTaskExtensions methods and supporting types.
// This file provides a stand-in to be used in tests so that our tests using these extensions
// can run on netfx in the interim.  Note that these are simple implementations; actual
// implementations will need more optimization.

namespace System.Net.Sockets
{
    public static class SocketTaskExtensions
    {
        public static Task<Socket> AcceptAsync(this Socket socket) =>
            Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, null);

        public static Task<Socket> AcceptAsync(this Socket socket, Socket acceptSocket) =>
            Task.Factory.FromAsync(socket.BeginAccept, socket.EndAccept, acceptSocket, 0, null);

        public static Task ConnectAsync(this Socket socket, EndPoint remoteEndPoint) =>
            Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, remoteEndPoint, null);

        public static Task ConnectAsync(this Socket socket, IPAddress address, int port) =>
            Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, address, port, null);

        public static Task ConnectAsync(this Socket socket, IPAddress[] addresses, int port) =>
            Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, addresses, port, null);

        public static Task ConnectAsync(this Socket socket, string host, int port) =>
            Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, host, port, null);

        public static Task<int> ReceiveAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags) =>
            Task.Factory.FromAsync(
                (callback, state) => socket.BeginReceive(buffer.Array, buffer.Offset, buffer.Count, socketFlags, callback, state),
                socket.EndReceive, null);

        public static Task<int> ReceiveAsync(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags) =>
            Task.Factory.FromAsync(socket.BeginReceive, socket.EndReceive, buffers, socketFlags, null);

        public static Task<SocketReceiveFromResult> ReceiveFromAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint) =>
            Task.Factory.FromAsync(
                (callback, state) => socket.BeginReceiveFrom(buffer.Array, buffer.Offset, buffer.Count, socketFlags, ref remoteEndPoint, callback, state),
                asyncResult => new SocketReceiveFromResult { ReceivedBytes = socket.EndReceiveFrom(asyncResult, ref remoteEndPoint), RemoteEndPoint = remoteEndPoint },
                null);

        public static Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint) =>
            Task.Factory.FromAsync(
                (callback, state) => socket.BeginReceiveMessageFrom(buffer.Array, buffer.Offset, buffer.Count, socketFlags, ref remoteEndPoint, callback, state),
                asyncResult => new SocketReceiveMessageFromResult {
                    ReceivedBytes = socket.EndReceiveMessageFrom(asyncResult, ref socketFlags, ref remoteEndPoint, out IPPacketInformation ipPacket),
                    PacketInformation = ipPacket,
                    RemoteEndPoint = remoteEndPoint,
                    SocketFlags = socketFlags
                }, null);

        public static Task<int> SendAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags) =>
            Task.Factory.FromAsync(
                (callback, state) => socket.BeginSend(buffer.Array, buffer.Offset, buffer.Count, socketFlags, callback, state),
                socket.EndSend,
                null);

        public static Task<int> SendAsync(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags) =>
            Task.Factory.FromAsync(socket.BeginSend, socket.EndSend, buffers, socketFlags, null);

        public static Task<int> SendToAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint) =>
            Task.Factory.FromAsync(
                (callback, state) => socket.BeginSendTo(buffer.Array, buffer.Offset, buffer.Count, socketFlags, remoteEndPoint, callback,  state),
                socket.EndSendTo,
                null);
    }

    public struct SocketReceiveFromResult
    {
        public int ReceivedBytes;
        public EndPoint RemoteEndPoint;
    }

    public struct SocketReceiveMessageFromResult
    {
        public int ReceivedBytes;
        public SocketFlags SocketFlags;
        public EndPoint RemoteEndPoint;
        public IPPacketInformation PacketInformation;
    }
}
