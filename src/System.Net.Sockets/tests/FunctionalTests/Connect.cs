// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public abstract class Connect<T> : SocketTestHelperBase<T> where T : SocketHelperBase, new()
    {
        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public void Connect_Success(IPAddress listenAt)
        {
            int port;
            using (SocketTestServer.SocketTestServerFactory(SocketImplementationType.Async, listenAt, out port))
            {
                using (Socket client = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    Task connectTask = ConnectAsync(client, new IPEndPoint(listenAt, port));
                    Assert.True(connectTask.Wait(TestSettings.PassingTestTimeout), "IPv4: Timed out while waiting for connection");
                    Assert.True(client.Connected);
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public void Connect_MultipleIPAddresses_Success(IPAddress listenAt)
        {
            if (!SupportsMultiConnect)
                return;

            int port;
            using (SocketTestServer.SocketTestServerFactory(SocketImplementationType.Async, listenAt, out port))
            using (Socket client = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                Task connectTask = MultiConnectAsync(client, new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback }, port);
                Assert.True(connectTask.Wait(TestSettings.PassingTestTimeout), "Timed out while waiting for connection");
                Assert.True(client.Connected);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        [ActiveIssue(22765, TestPlatforms.AnyUnix)]
        public async Task Connect_OnConnectedSocket_Fails()
        {
            int port;
            using (SocketTestServer.SocketTestServerFactory(SocketImplementationType.Async, IPAddress.Loopback, out port))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                await ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, port));

                // In the sync case, we throw a derived exception here, so need to use ThrowsAnyAsync
                SocketException se = await Assert.ThrowsAnyAsync<SocketException>(() => ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, port)));
                Assert.Equal(SocketError.IsConnected, se.SocketErrorCode);
            }
        }

        [PlatformSpecific(TestPlatforms.Windows)] // Unix currently does not support Disconnect
        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public async Task Connect_AfterDisconnect_Fails()
        {
            int port;
            using (SocketTestServer.SocketTestServerFactory(SocketImplementationType.Async, IPAddress.Loopback, out port))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                await ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, port));
                client.Disconnect(reuseSocket: false);

                if (ConnectAfterDisconnectResultsInInvalidOperationException)
                {
                    await Assert.ThrowsAsync<InvalidOperationException>(() => ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, port)));
                }
                else
                {
                    SocketException se = await Assert.ThrowsAsync<SocketException>(() => ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, port)));
                    Assert.Equal(SocketError.IsConnected, se.SocketErrorCode);
                }
            }
        }
    }

    public sealed class ConnectSync : Connect<SocketHelperSync> { }
    public sealed class ConnectSyncForceNonBlocking : Connect<SocketHelperSyncForceNonBlocking> { }
    public sealed class ConnectApm : Connect<SocketHelperApm> { }
    public sealed class ConnectTask : Connect<SocketHelperTask> { }
    public sealed class ConnectEap : Connect<SocketHelperEap> { }
}
