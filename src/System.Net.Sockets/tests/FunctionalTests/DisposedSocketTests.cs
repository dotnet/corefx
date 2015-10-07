// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class DisposedSocket
    {
        private readonly static byte[] s_buffer = new byte[1];
        private readonly static IList<ArraySegment<byte>> s_buffers = new List<ArraySegment<byte>> { new ArraySegment<byte>(s_buffer) };
        private readonly static SocketAsyncEventArgs s_eventArgs = new SocketAsyncEventArgs();

        private static Socket GetDisposedSocket(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            using (var socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                return socket;
            }
        }

        private static void TheAsyncCallback(IAsyncResult ar)
        {
        }

        [Fact]
        public void Available_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Available);
        }

        [Fact]
        public void LocalEndPoint_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().LocalEndPoint);
        }

        [Fact]
        public void RemoteEndPoint_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().RemoteEndPoint);
        }

        [Fact]
        public void SetBlocking_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().Blocking = true;
            });
        }

        [Fact]
        public void ExclusiveAddressUse_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ExclusiveAddressUse);
        }

        [Fact]
        public void SetExclusiveAddressUse_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().ExclusiveAddressUse = true;
            });
        }

        [Fact]
        public void ReceiveBufferSize_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveBufferSize);
        }

        [Fact]
        public void SetReceiveBufferSize_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().ReceiveBufferSize = 1;
            });
        }

        [Fact]
        public void SendBufferSize_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendBufferSize);
        }

        [Fact]
        public void SetSendBufferSize_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().SendBufferSize = 1;
            });
        }

        [Fact]
        public void ReceiveTimeout_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveTimeout);
        }

        [Fact]
        public void SetReceiveTimeout_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().ReceiveTimeout = 1;
            });
        }

        [Fact]
        public void SendTimeout_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendTimeout);
        }

        [Fact]
        public void SetSendTimeout_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().SendTimeout = 1;
            });
        }

        [Fact]
        public void LingerState_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().LingerState);
        }

        [Fact]
        public void SetLingerState_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().LingerState = new LingerOption(true, 1);
            });
        }

        [Fact]
        public void NoDelay_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().NoDelay);
        }

        [Fact]
        public void SetNoDelay_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().NoDelay = true;
            });
        }

        [Fact]
        public void Ttl_IPv4_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetwork).Ttl);
        }

        [Fact]
        public void SetTtl_IPv4_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket(AddressFamily.InterNetwork).Ttl = 1;
            });
        }

        [Fact]
        public void Ttl_IPv6_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetworkV6).Ttl);
        }

        [Fact]
        public void SetTtl_IPv6_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket(AddressFamily.InterNetworkV6).Ttl = 1;
            });
        }

        [Fact]
        public void DontFragment_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().DontFragment);
        }

        [Fact]
        public void SetDontFragment_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().DontFragment = true;
            });
        }

        [Fact]
        public void MulticastLoopback_IPv4_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetwork).MulticastLoopback);
        }

        [Fact]
        public void SetMulticastLoopback_IPv4_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket(AddressFamily.InterNetwork).MulticastLoopback = true;
            });
        }

        [Fact]
        public void MulticastLoopback_IPv6_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetworkV6).MulticastLoopback);
        }

        [Fact]
        public void SetMulticastLoopback_IPv6_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket(AddressFamily.InterNetworkV6).MulticastLoopback = true;
            });
        }

        [Fact]
        public void EnableBroadcast_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().EnableBroadcast);
        }

        [Fact]
        public void SetEnableBroadcast_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket().EnableBroadcast = true;
            });
        }

        [Fact]
        public void DualMode_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetworkV6).DualMode);
        }

        [Fact]
        public void SetDualMode_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                GetDisposedSocket(AddressFamily.InterNetworkV6).DualMode = true;
            });
        }

        [Fact]
        public void Bind_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Bind(new IPEndPoint(IPAddress.Loopback, 0)));
        }

        [Fact]
        public void Connect_IPEndPoint_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Connect(new IPEndPoint(IPAddress.Loopback, 1)));
        }

        [Fact]
        public void Connect_IPAddress_Port_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Connect(IPAddress.Loopback, 1));
        }

        [Fact]
        public void Connect_Host_Port_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Connect("localhost", 1));
        }

        [Fact]
        public void Connect_IPAddresses_Port_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Connect(new[] { IPAddress.Loopback }, 1));
        }

        [Fact]
        public void Disconnect_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Disconnect(false));
        }

        [Fact]
        public void Listen_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Listen(1));
        }

        [Fact]
        public void Accept_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Accept());
        }

        [Fact]
        public void Send_Buffer_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffer, s_buffer.Length, SocketFlags.None));
        }

        [Fact]
        public void Send_Buffer_SocketFlags_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffer, SocketFlags.None));
        }

        [Fact]
        public void Send_Buffer_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffer));
        }

        [Fact]
        public void Send_Buffer_Offset_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffer, 0, s_buffer.Length, SocketFlags.None));
        }

        [Fact]
        public void Send_Buffer_Offset_Size_SocketError_Throws_ObjectDisposed()
        {
            SocketError errorCode;
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffer, 0, s_buffer.Length, SocketFlags.None, out errorCode));
        }

        [Fact]
        public void Send_Buffers_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffers));
        }

        [Fact]
        public void Send_Buffers_SocketFlags_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffers, SocketFlags.None));
        }

        [Fact]
        public void Send_Buffers_SocketFlags_SocketError_Throws_ObjectDisposed()
        {
            SocketError errorCode;
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Send(s_buffers, SocketFlags.None, out errorCode));
        }

        [Fact]
        public void SendTo_Buffer_Offset_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendTo(s_buffer, 0, s_buffer.Length, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, 1)));
        }

        [Fact]
        public void SendTo_Buffer_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendTo(s_buffer,  s_buffer.Length, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, 1)));
        }

        [Fact]
        public void SendTo_Buffer_SocketFlags_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendTo(s_buffer, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, 1)));
        }

        [Fact]
        public void SendTo_Buffer_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendTo(s_buffer, new IPEndPoint(IPAddress.Loopback, 1)));
        }

        [Fact]
        public void Receive_Buffer_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffer, s_buffer.Length, SocketFlags.None));
        }

        [Fact]
        public void Receive_Buffer_SocketFlags_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffer, SocketFlags.None));
        }

        [Fact]
        public void Receive_Buffer_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffer));
        }

        [Fact]
        public void Receive_Buffer_Offset_Size_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffer, 0, s_buffer.Length, SocketFlags.None));
        }

        [Fact]
        public void Receive_Buffer_Offset_Size_SocketError_Throws_ObjectDisposed()
        {
            SocketError errorCode;
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffer, 0, s_buffer.Length, SocketFlags.None, out errorCode));
        }

        [Fact]
        public void Receive_Buffers_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffers));
        }

        [Fact]
        public void Receive_Buffers_SocketFlags_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffers, SocketFlags.None));
        }

        [Fact]
        public void Receive_Buffers_SocketFlags_SocketError_Throws_ObjectDisposed()
        {
            SocketError errorCode;
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Receive(s_buffers, SocketFlags.None, out errorCode));
        }

        [Fact]
        public void ReceiveMessageFrom_Throws_ObjectDisposed()
        {
            SocketFlags flags = SocketFlags.None;
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            IPPacketInformation packetInfo;
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveMessageFrom(s_buffer, 0, s_buffer.Length, ref flags, ref remote, out packetInfo));
        }

        [Fact]
        public void ReceiveFrom_Buffer_Offset_Size_Throws_ObjectDisposed()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveFrom(s_buffer, 0, s_buffer.Length, SocketFlags.None, ref remote));
        }

        [Fact]
        public void ReceiveFrom_Buffer_Size_Throws_ObjectDisposed()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveFrom(s_buffer,  s_buffer.Length, SocketFlags.None, ref remote));
        }

        [Fact]
        public void ReceiveFrom_Buffer_SocketFlags_Throws_ObjectDisposed()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveFrom(s_buffer, SocketFlags.None, ref remote));
        }

        [Fact]
        public void ReceiveFrom_Buffer_Throws_ObjectDisposed()
        {
            EndPoint remote = new IPEndPoint(IPAddress.Loopback, 1);
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveFrom(s_buffer, ref remote));
        }

        [Fact]
        public void Shutdown_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Shutdown(SocketShutdown.Both));
        }

        [Fact]
        public void IOControl_Int_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().IOControl(0, s_buffer, s_buffer));
        }

        [Fact]
        public void IOControl_IOControlCode_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().IOControl(IOControlCode.Flush, s_buffer, s_buffer));
        }

        [Fact]
        public void SetIPProtectionLevel_IPv4_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetwork).SetIPProtectionLevel(IPProtectionLevel.Unrestricted));
        }

        [Fact]
        public void SetIPProtectionLevel_IPv6_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket(AddressFamily.InterNetworkV6).SetIPProtectionLevel(IPProtectionLevel.Unrestricted));
        }

        [Fact]
        public void SetSocketOption_Int_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error, 0)); 
        }

        [Fact]
        public void SetSocketOption_Buffer_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error, s_buffer)); 
        }

        [Fact]
        public void SetSocketOption_Bool_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error, true)); 
        }

        [Fact]
        public void SetSocketOption_Object_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 1))); 
        }

        [Fact]
        public void GetSocketOption_Int_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error, 4)); 
        }

        [Fact]
        public void GetSocketOption_Buffer_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error, s_buffer)); 
        }

        [Fact]
        public void GetSocketOption_Object_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger)); 
        }

        [Fact]
        public void Poll_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().Poll(-1, SelectMode.SelectWrite));
        }

        [Fact]
        public void AcceptAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().AcceptAsync(s_eventArgs));
        }

        [Fact]
        public void ConnectAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ConnectAsync(s_eventArgs));
        }

        [Fact]
        public void DisconnectAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().DisconnectAsync(s_eventArgs));
        }

        [Fact]
        public void ReceiveAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveAsync(s_eventArgs));
        }

        [Fact]
        public void ReceiveFromAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveFromAsync(s_eventArgs));
        }

        [Fact]
        public void ReceiveMessageFromAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().ReceiveMessageFromAsync(s_eventArgs));
        }

        [Fact]
        public void SendAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendAsync(s_eventArgs));
        }

        [Fact]
        public void SendPacketsAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendPacketsAsync(s_eventArgs));
        }

        [Fact]
        public void SendToAsync_Throws_ObjectDisposed()
        {
            Assert.Throws<ObjectDisposedException>(() => GetDisposedSocket().SendToAsync(s_eventArgs));
        }
    }
}
