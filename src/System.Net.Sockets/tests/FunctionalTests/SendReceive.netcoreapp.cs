// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public sealed class SendReceiveSpanSync : SendReceive<SocketHelperSpanSync> { }
    public sealed class SendReceiveSpanSyncForceNonBlocking : SendReceive<SocketHelperSpanSyncForceNonBlocking> { }
    public sealed class SendReceiveMemoryArrayTask : SendReceive<SocketHelperMemoryArrayTask>
    {
        [Fact]
        public async Task Precanceled_Throws()
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.BindToAnonymousPort(IPAddress.Loopback);
                listener.Listen(1);

                await client.ConnectAsync(listener.LocalEndPoint);
                using (Socket server = await listener.AcceptAsync())
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await server.SendAsync((ReadOnlyMemory<byte>)new byte[0], SocketFlags.None, cts.Token));
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await server.ReceiveAsync((Memory<byte>)new byte[0], SocketFlags.None, cts.Token));
                }
            }
        }

        [Fact]
        public async Task TCP_ReceiveCanceledDuringOperation_Throws()
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.BindToAnonymousPort(IPAddress.Loopback);
                listener.Listen(1);

                await client.ConnectAsync(listener.LocalEndPoint);
                using (Socket server = await listener.AcceptAsync())
                {
                    ValueTask<int> vt1 = default, vt2 = default;
                    CancellationTokenSource cts;

                    for (int len = 0; len < 2; len++)
                    {
                        cts = new CancellationTokenSource();
                        vt1 = server.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                        Assert.False(vt1.IsCompleted);
                        cts.Cancel();
                        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt1);
                    }

                    for (int len = 0; len < 2; len++)
                    {
                        cts = new CancellationTokenSource();
                        vt1 = server.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                        vt2 = server.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                        Assert.False(vt1.IsCompleted);
                        Assert.False(vt2.IsCompleted);
                        cts.Cancel();
                        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt1);
                        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt2);
                    }

                    await server.SendAsync((ReadOnlyMemory<byte>)new byte[1], SocketFlags.None);
                    Assert.Equal(1, await client.ReceiveAsync((Memory<byte>)new byte[10], SocketFlags.None));
                }
            }
        }

        [Fact]
        public async Task UDP_ReceiveCanceledDuringOperation_Throws()
        {
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                client.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                ValueTask<int> vt1 = default, vt2 = default;
                CancellationTokenSource cts;

                for (int len = 0; len < 2; len++)
                {
                    cts = new CancellationTokenSource();
                    vt1 = client.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                    Assert.False(vt1.IsCompleted);
                    cts.Cancel();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt1);
                }

                for (int len = 0; len < 2; len++)
                {
                    cts = new CancellationTokenSource();
                    vt1 = client.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                    vt2 = client.ReceiveAsync((Memory<byte>)new byte[len], SocketFlags.None, cts.Token);
                    Assert.False(vt1.IsCompleted);
                    Assert.False(vt2.IsCompleted);
                    cts.Cancel();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt1);
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await vt2);
                }
            }
        }

        [Fact]
        public async Task DisposedSocket_ThrowsOperationCanceledException()
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.BindToAnonymousPort(IPAddress.Loopback);
                listener.Listen(1);

                await client.ConnectAsync(listener.LocalEndPoint);
                using (Socket server = await listener.AcceptAsync())
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    server.Shutdown(SocketShutdown.Both);
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await server.SendAsync((ReadOnlyMemory<byte>)new byte[0], SocketFlags.None, cts.Token));
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await server.ReceiveAsync((Memory<byte>)new byte[0], SocketFlags.None, cts.Token));
                }
            }
        }
    }
    public sealed class SendReceiveMemoryNativeTask : SendReceive<SocketHelperMemoryNativeTask> { }
}
