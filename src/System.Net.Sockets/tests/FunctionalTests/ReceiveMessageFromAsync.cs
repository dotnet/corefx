// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public class ReceiveMessageFromAsync
    {
        public void OnCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void Success_IPv4()
        {
            ManualResetEvent completed = new ManualResetEvent(false);

            if (Socket.OSSupportsIPv4)
            {
                using (Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    int port = receiver.BindToAnonymousPort(IPAddress.Loopback);
                    receiver.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sender.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                    for (int i = 0; i < TestSettings.UDPRedundancy; i++)
                    {
                        sender.SendTo(new byte[1024], new IPEndPoint(IPAddress.Loopback, port));
                    }

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    args.SetBuffer(new byte[1024], 0, 1024);
                    args.Completed += OnCompleted;
                    args.UserToken = completed;

                    bool pending = receiver.ReceiveMessageFromAsync(args);
                    if (!pending)
                    {
                        OnCompleted(null, args);
                    }

                    Assert.True(completed.WaitOne(TestSettings.PassingTestTimeout), "Timeout while waiting for connection");

                    Assert.Equal(1024, args.BytesTransferred);
                    Assert.Equal(sender.LocalEndPoint, args.RemoteEndPoint);
                    Assert.Equal(((IPEndPoint)sender.LocalEndPoint).Address, args.ReceiveMessageFromPacketInfo.Address);

                    sender.Dispose();
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void Success_IPv6()
        {
            ManualResetEvent completed = new ManualResetEvent(false);

            if (Socket.OSSupportsIPv6)
            {
                using (Socket receiver = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp))
                {
                    int port = receiver.BindToAnonymousPort(IPAddress.IPv6Loopback);
                    receiver.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.PacketInformation, true);

                    Socket sender = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                    sender.Bind(new IPEndPoint(IPAddress.IPv6Loopback, 0));

                    for (int i = 0; i < TestSettings.UDPRedundancy; i++)
                    {
                        sender.SendTo(new byte[1024], new IPEndPoint(IPAddress.IPv6Loopback, port));
                    }

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
                    args.SetBuffer(new byte[1024], 0, 1024);
                    args.Completed += OnCompleted;
                    args.UserToken = completed;

                    bool pending = receiver.ReceiveMessageFromAsync(args);
                    if (!pending)
                    {
                        OnCompleted(null, args);
                    }

                    Assert.True(completed.WaitOne(TestSettings.PassingTestTimeout), "Timeout while waiting for connection");

                    Assert.Equal(1024, args.BytesTransferred);
                    Assert.Equal(sender.LocalEndPoint, args.RemoteEndPoint);
                    Assert.Equal(((IPEndPoint)sender.LocalEndPoint).Address, args.ReceiveMessageFromPacketInfo.Address);

                    sender.Dispose();
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Task_Success(bool ipv4)
        {
            AddressFamily family = ipv4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6;
            IPAddress loopback = ipv4 ? IPAddress.Loopback : IPAddress.IPv6Loopback;

            using (Socket receiver = new Socket(family, SocketType.Dgram, ProtocolType.Udp))
            using (Socket sender = new Socket(family, SocketType.Dgram, ProtocolType.Udp))
            {
                int port = receiver.BindToAnonymousPort(loopback);
                receiver.SetSocketOption(ipv4 ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.PacketInformation, true);

                sender.Bind(new IPEndPoint(loopback, 0));

                for (int i = 0; i < TestSettings.UDPRedundancy; i++)
                {
                    sender.SendTo(new byte[1024], new IPEndPoint(loopback, port));
                }

                SocketReceiveMessageFromResult result = await receiver.ReceiveMessageFromAsync(
                    new ArraySegment<byte>(new byte[1024], 0, 1024), SocketFlags.None,
                    new IPEndPoint(ipv4 ? IPAddress.Any : IPAddress.IPv6Any, 0));
                Assert.Equal(1024, result.ReceivedBytes);
                Assert.Equal(sender.LocalEndPoint, result.RemoteEndPoint);
                Assert.Equal(((IPEndPoint)sender.LocalEndPoint).Address, result.PacketInformation.Address);
            }
        }
    }
}
