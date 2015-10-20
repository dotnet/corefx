// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace System.Net.Sockets.Tests
{
    // TODO:
    //
    // End*:
    // - Invalid asyncresult type
    // - asyncresult from different object
    // - asyncresult with end already called
    public class ArgumentValidation
    {
        private readonly static byte[] s_buffer = new byte[1];
        private readonly static IList<ArraySegment<byte>> s_buffers = new List<ArraySegment<byte>> { new ArraySegment<byte>(s_buffer) };
        private readonly static Socket s_ipv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly static Socket s_ipv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

        private static void TheAsyncCallback(IAsyncResult ar)
        {
        }

        private static Socket GetSocket(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            Debug.Assert(addressFamily == AddressFamily.InterNetwork || addressFamily == AddressFamily.InterNetworkV6);
            return addressFamily == AddressFamily.InterNetwork ? s_ipv4Socket : s_ipv6Socket;
        }

        [Fact]
        public void BeginAccept_NegativeReceiveSize_Throws_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginAccept(null, -1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginAccept_NotBound_Throws_InvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => GetSocket().BeginAccept(null, 0, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginAccept_NotListening_Throws_InvalidOperation()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                Assert.Throws<InvalidOperationException>(() => socket.BeginAccept(null, 0, TheAsyncCallback, null));
            }
        }

        [Fact]
        public void EndAccept_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndAccept(null));
        }

        [Fact]
        public void BeginConnect_EndPoint_NullEndPoint_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginConnect((EndPoint)null, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_EndPoint_ListeningSocket_Throws_InvalidOperation()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                socket.Listen(1);
                Assert.Throws<InvalidOperationException>(() => socket.BeginConnect(new IPEndPoint(IPAddress.Loopback, 1), TheAsyncCallback, null));
            }
        }

        [Fact]
        public void BeginConnect_EndPoint_AddressFamily_Throws_NotSupported()
        {
            Assert.Throws<NotSupportedException>(() => GetSocket(AddressFamily.InterNetwork).BeginConnect(new DnsEndPoint("localhost", 1, AddressFamily.InterNetworkV6), TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_Host_NullHost_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginConnect((string)null, 1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_Host_InvalidPort_Throws_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect("localhost", -1, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect("localhost", 65536, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_Host_ListeningSocket_Throws_InvalidOperation()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                socket.Listen(1);
                Assert.Throws<InvalidOperationException>(() => socket.BeginConnect("localhost", 1, TheAsyncCallback, null));
            }
        }

        [Fact]
        public void BeginConnect_IPAddress_NullIPAddress_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginConnect((IPAddress)null, 1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddress_InvalidPort_Throws_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect(IPAddress.Loopback, -1, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect(IPAddress.Loopback, 65536, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddress_AddressFamily_Throws_NotSupported()
        {
            Assert.Throws<NotSupportedException>(() => GetSocket(AddressFamily.InterNetwork).BeginConnect(IPAddress.IPv6Loopback, 1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddresses_NullIPAddresses_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginConnect((IPAddress[])null, 1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddresses_EmptyIPAddresses_Throws_Argument()
        {
            Assert.Throws<ArgumentException>(() => GetSocket().BeginConnect(new IPAddress[0], 1, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddresses_InvalidPort_Throws_ArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect(new[] { IPAddress.Loopback }, -1, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginConnect(new[] { IPAddress.Loopback }, 65536, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginConnect_IPAddresses_ListeningSocket_Throws_InvalidOperation()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                socket.Listen(1);
                Assert.Throws<InvalidOperationException>(() => socket.BeginConnect(new[] { IPAddress.Loopback }, 1, TheAsyncCallback, null));
            }
        }

        [Fact]
        public void EndConnect_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndConnect(null));
        }

        [Fact]
        public void EndDisconnect_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndDisconnect(null));
        }

        [Fact]
        public void BeginSend_Buffer_NullBuffer_Throws_ArgumentNull()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginSend(null, 0, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSend_Buffer_InvalidOffset_Throws_ArgumentOutOfRange()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSend(s_buffer, -1, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSend(s_buffer, s_buffer.Length + 1, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSend_Buffer_InvalidCount_Throws_ArgumentOutOfRange()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSend(s_buffer, 0, -1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSend(s_buffer, 0, s_buffer.Length + 1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSend(s_buffer, s_buffer.Length, 1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSend_Buffers_NullBuffers_Throws_ArgumentNull()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginSend(null, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSend_Buffers_EmptyBuffers_Throws_Argument()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentException>(() => GetSocket().BeginSend(new List<ArraySegment<byte>>(), SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void EndSend_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndSend(null));
        }

        [Fact]
        public void BeginSendTo_NullBuffer_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginSendTo(null, 0, 0, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, 1), TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSendTo_NullEndPoint_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginSendTo(s_buffer, 0, 0, SocketFlags.None, null, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSendTo_InvalidOffset_Throws_ArgumentOutOfRange()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSendTo(s_buffer, -1, s_buffer.Length, SocketFlags.None, endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSendTo(s_buffer, s_buffer.Length + 1, s_buffer.Length, SocketFlags.None, endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginSendTo_InvalidSize_Throws_ArgumentOutOfRange()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSendTo(s_buffer, 0, -1, SocketFlags.None, endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSendTo(s_buffer, 0, s_buffer.Length + 1, SocketFlags.None, endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginSendTo(s_buffer, s_buffer.Length, 1, SocketFlags.None, endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void EndSendTo_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndSendTo(null));
        }

        [Fact]
        public void BeginReceive_Buffer_NullBuffer_Throws_ArgumentNull()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceive(null, 0, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceive_Buffer_InvalidOffset_Throws_ArgumentOutOfRange()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceive(s_buffer, -1, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceive(s_buffer, s_buffer.Length + 1, 0, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceive_Buffer_InvalidCount_Throws_ArgumentOutOfRange()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceive(s_buffer, 0, -1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceive(s_buffer, 0, s_buffer.Length + 1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceive(s_buffer, s_buffer.Length, 1, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceive_Buffers_NullBuffers_Throws_ArgumentNull()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceive(null, SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceive_Buffers_EmptyBuffers_Throws_Argument()
        {
            SocketError errorCode;
            Assert.Throws<ArgumentException>(() => GetSocket().BeginReceive(new List<ArraySegment<byte>>(), SocketFlags.None, out errorCode, TheAsyncCallback, null));
        }

        [Fact]
        public void EndReceive_NullAsyncResult_Throws_ArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndReceive(null));
        }

        [Fact]
        public void BeginReceiveFrom_NullBuffer_Throws_ArgumentNull()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceiveFrom(null, 0, 0, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveFrom_NullEndPoint_Throws_ArgumentNull()
        {
            EndPoint endpoint = null;
            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceiveFrom(s_buffer, 0, 0, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveFrom_AddressFamily_Throws_Argument()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.IPv6Loopback, 1);
            Assert.Throws<ArgumentException>(() => GetSocket(AddressFamily.InterNetwork).BeginReceiveFrom(s_buffer, 0, 0, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveFrom_InvalidOffset_Throws_ArgumentOutOfRange()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveFrom(s_buffer, -1, s_buffer.Length, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveFrom(s_buffer, s_buffer.Length + 1, s_buffer.Length, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveFrom_InvalidSize_Throws_ArgumentOutOfRange()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveFrom(s_buffer, 0, -1, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveFrom(s_buffer, 0, s_buffer.Length + 1, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveFrom(s_buffer, s_buffer.Length, 1, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveFrom_NotBound_Throws_InvalidOperation()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<InvalidOperationException>(() => GetSocket().BeginReceiveFrom(s_buffer, 0, 0, SocketFlags.None, ref endpoint, TheAsyncCallback, null));
        }

        [Fact]
        public void EndReceiveFrom_NullAsyncResult_Throws_ArgumentNull()
        {
            EndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ArgumentNullException>(() => GetSocket().EndReceiveFrom(null, ref endpoint));
        }

        [Fact]
        public void BeginReceiveMessageFrom_NullBuffer_Throws_ArgumentNull()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);

            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceiveMessageFrom(null, 0, 0, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveMessageFrom_NullEndPoint_Throws_ArgumentNull()
        {
            EndPoint remote = null;

            Assert.Throws<ArgumentNullException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, 0, 0, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveMessageFrom_AddressFamily_Throws_Argument()
        {
            EndPoint remote = new IPEndPoint(IPAddress.IPv6Loopback, 1);

            Assert.Throws<ArgumentException>(() => GetSocket(AddressFamily.InterNetwork).BeginReceiveMessageFrom(s_buffer, 0, 0, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveMessageFrom_InvalidOffset_Throws_ArgumentOutOfRange()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, -1, s_buffer.Length, SocketFlags.None, ref remote, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, s_buffer.Length + 1, s_buffer.Length, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveMessageFrom_InvalidSize_Throws_ArgumentOutOfRange()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, 0, -1, SocketFlags.None, ref remote, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, 0, s_buffer.Length + 1, SocketFlags.None, ref remote, TheAsyncCallback, null));
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, s_buffer.Length, 1, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void BeginReceiveMessageFrom_NotBound_Throws_InvalidOperation()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);

            Assert.Throws<InvalidOperationException>(() => GetSocket().BeginReceiveMessageFrom(s_buffer, 0, 0, SocketFlags.None, ref remote, TheAsyncCallback, null));
        }

        [Fact]
        public void EndReceiveMessageFrom_NullEndPoint_Throws_ArgumentNull()
        {
            SocketFlags flags = SocketFlags.None;
            EndPoint remote = null;
            IPPacketInformation packetInfo;

            Assert.Throws<ArgumentNullException>(() => GetSocket().EndReceiveMessageFrom(null, ref flags, ref remote, out packetInfo));
        }

        [Fact]
        public void EndReceiveMessageFrom_AddressFamily_Throws_Argument()
        {
            SocketFlags flags = SocketFlags.None;
            EndPoint remote = new IPEndPoint(IPAddress.IPv6Loopback, 1);
            IPPacketInformation packetInfo;

            Assert.Throws<ArgumentException>(() => GetSocket(AddressFamily.InterNetwork).EndReceiveMessageFrom(null, ref flags, ref remote, out packetInfo));
        }

        [Fact]
        public void EndReceiveMessageFrom_NullAsyncResult_Throws_ArgumentNull()
        {
            SocketFlags flags = SocketFlags.None;
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            IPPacketInformation packetInfo;

            Assert.Throws<ArgumentNullException>(() => GetSocket().EndReceiveMessageFrom(null, ref flags, ref remote, out packetInfo));
        }
    }
}
