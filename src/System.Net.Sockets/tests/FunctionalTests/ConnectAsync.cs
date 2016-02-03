// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Test.Common;
using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class ConnectAsync
    {
        private readonly ITestOutputHelper _log;

        public ConnectAsync(ITestOutputHelper output)
        {
            _log = TestLogging.GetInstance();
            Assert.True(Capability.IPv4Support() || Capability.IPv6Support());
        }

        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [Fact]
        [Trait("IPv4", "true")]
        public void ConnectAsync_IPv4_Success()
        {
            Assert.True(Capability.IPv4Support());

            AutoResetEvent completed = new AutoResetEvent(false);

            int port;
            using (SocketTestServer.SocketTestServerFactory(IPAddress.Loopback, out port))
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, port);
                args.Completed += OnConnectCompleted;
                args.UserToken = completed;

                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "IPv4: Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);
                }
            }
        }

        [Fact]
        [Trait("IPv6", "true")]
        public void ConnectAsync_IPv6_Success()
        {
            Assert.True(Capability.IPv6Support());

            AutoResetEvent completed = new AutoResetEvent(false);

            int port;
            using (SocketTestServer.SocketTestServerFactory(IPAddress.IPv6Loopback, out port))
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, port);
                args.Completed += OnConnectCompleted;
                args.UserToken = completed;

                using (Socket client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                {
                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "IPv6: Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);
                }
            }
        }
    }
}
