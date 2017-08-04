// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public abstract class SendReceive<T> : SocketTestHelperBase<T> where T : SocketHelperBase, new()
    {
        [Theory]
        [InlineData(null, 0, 0)] // null array
        [InlineData(1, -1, 0)] // offset low
        [InlineData(1, 2, 0)] // offset high
        [InlineData(1, 0, -1)] // count low
        [InlineData(1, 1, 2)] // count high
        public async Task InvalidArguments_Throws(int? length, int offset, int count)
        {
            if (length == null && !ValidatesArrayArguments) return;

            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                Type expectedExceptionType = length == null ? typeof(ArgumentNullException) : typeof(ArgumentOutOfRangeException);

                var validBuffer = new ArraySegment<byte>(new byte[1]);
                var invalidBuffer = new FakeArraySegment { Array = length != null ? new byte[length.Value] : null, Offset = offset, Count = count }.ToActual();

                await Assert.ThrowsAsync(expectedExceptionType, () => ReceiveAsync(s, invalidBuffer));
                await Assert.ThrowsAsync(expectedExceptionType, () => ReceiveAsync(s, new List<ArraySegment<byte>> { invalidBuffer }));
                await Assert.ThrowsAsync(expectedExceptionType, () => ReceiveAsync(s, new List<ArraySegment<byte>> { validBuffer, invalidBuffer }));

                await Assert.ThrowsAsync(expectedExceptionType, () => SendAsync(s, invalidBuffer));
                await Assert.ThrowsAsync(expectedExceptionType, () => SendAsync(s, new List<ArraySegment<byte>> { invalidBuffer }));
                await Assert.ThrowsAsync(expectedExceptionType, () => SendAsync(s, new List<ArraySegment<byte>> { validBuffer, invalidBuffer }));
            }
        }

        [ActiveIssue(16945)] // Packet loss, potentially due to other tests running at the same time
        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public async Task SendToRecvFrom_Datagram_UDP(IPAddress loopbackAddress)
        {
            IPAddress leftAddress = loopbackAddress, rightAddress = loopbackAddress;

            // TODO #5185: Harden against packet loss
            const int DatagramSize = 256;
            const int DatagramsToSend = 256;
            const int AckTimeout = 1000;
            const int TestTimeout = 30000;

            var left = new Socket(leftAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            left.BindToAnonymousPort(leftAddress);

            var right = new Socket(rightAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            right.BindToAnonymousPort(rightAddress);

            var leftEndpoint = (IPEndPoint)left.LocalEndPoint;
            var rightEndpoint = (IPEndPoint)right.LocalEndPoint;

            var receiverAck = new SemaphoreSlim(0);
            var senderAck = new SemaphoreSlim(0);

            var receivedChecksums = new uint?[DatagramsToSend];
            Task leftThread = Task.Run(async () =>
            {
                using (left)
                {
                    EndPoint remote = leftEndpoint.Create(leftEndpoint.Serialize());
                    var recvBuffer = new byte[DatagramSize];
                    for (int i = 0; i < DatagramsToSend; i++)
                    {
                        SocketReceiveFromResult result = await ReceiveFromAsync(
                            left, new ArraySegment<byte>(recvBuffer), remote);
                        Assert.Equal(DatagramSize, result.ReceivedBytes);
                        Assert.Equal(rightEndpoint, result.RemoteEndPoint);

                        int datagramId = recvBuffer[0];
                        Assert.Null(receivedChecksums[datagramId]);
                        receivedChecksums[datagramId] = Fletcher32.Checksum(recvBuffer, 0, result.ReceivedBytes);

                        receiverAck.Release();
                        Assert.True(await senderAck.WaitAsync(TestTimeout));
                    }
                }
            });

            var sentChecksums = new uint[DatagramsToSend];
            using (right)
            {
                var random = new Random();
                var sendBuffer = new byte[DatagramSize];
                for (int i = 0; i < DatagramsToSend; i++)
                {
                    random.NextBytes(sendBuffer);
                    sendBuffer[0] = (byte)i;

                    int sent = await SendToAsync(right, new ArraySegment<byte>(sendBuffer), leftEndpoint);

                    Assert.True(await receiverAck.WaitAsync(AckTimeout));
                    senderAck.Release();

                    Assert.Equal(DatagramSize, sent);
                    sentChecksums[i] = Fletcher32.Checksum(sendBuffer, 0, sent);
                }
            }

            await leftThread;
            for (int i = 0; i < DatagramsToSend; i++)
            {
                Assert.NotNull(receivedChecksums[i]);
                Assert.Equal(sentChecksums[i], (uint)receivedChecksums[i]);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(LoopbacksAndBuffers))]
        public async Task SendRecv_Stream_TCP(IPAddress listenAt, bool useMultipleBuffers)
        {
            const int BytesToSend = 123456, ListenBacklog = 1, LingerTime = 1;
            int bytesReceived = 0, bytesSent = 0;
            Fletcher32 receivedChecksum = new Fletcher32(), sentChecksum = new Fletcher32();

            using (var server = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                server.BindToAnonymousPort(listenAt);
                server.Listen(ListenBacklog);

                Task serverProcessingTask = Task.Run(async () =>
                {
                    using (Socket remote = await AcceptAsync(server))
                    {
                        if (!useMultipleBuffers)
                        {
                            var recvBuffer = new byte[256];
                            for (;;)
                            {
                                int received = await ReceiveAsync(remote, new ArraySegment<byte>(recvBuffer));
                                if (received == 0)
                                {
                                    break;
                                }

                                bytesReceived += received;
                                receivedChecksum.Add(recvBuffer, 0, received);
                            }
                        }
                        else
                        {
                            var recvBuffers = new List<ArraySegment<byte>> {
                                new ArraySegment<byte>(new byte[123]),
                                new ArraySegment<byte>(new byte[256], 2, 100),
                                new ArraySegment<byte>(new byte[1], 0, 0),
                                new ArraySegment<byte>(new byte[64], 9, 33)};
                            for (;;)
                            {
                                int received = await ReceiveAsync(remote, recvBuffers);
                                if (received == 0)
                                {
                                    break;
                                }

                                bytesReceived += received;
                                for (int i = 0, remaining = received; i < recvBuffers.Count && remaining > 0; i++)
                                {
                                    ArraySegment<byte> buffer = recvBuffers[i];
                                    int toAdd = Math.Min(buffer.Count, remaining);
                                    receivedChecksum.Add(buffer.Array, buffer.Offset, toAdd);
                                    remaining -= toAdd;
                                }
                            }

                        }
                    }
                });

                EndPoint clientEndpoint = server.LocalEndPoint;
                using (var client = new Socket(clientEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    await ConnectAsync(client, clientEndpoint);

                    var random = new Random();
                    if (!useMultipleBuffers)
                    {
                        var sendBuffer = new byte[512];
                        for (int sent = 0, remaining = BytesToSend; remaining > 0; remaining -= sent)
                        {
                            random.NextBytes(sendBuffer);
                            sent = await SendAsync(client, new ArraySegment<byte>(sendBuffer, 0, Math.Min(sendBuffer.Length, remaining)));
                            bytesSent += sent;
                            sentChecksum.Add(sendBuffer, 0, sent);
                        }
                    }
                    else
                    {
                        var sendBuffers = new List<ArraySegment<byte>> {
                        new ArraySegment<byte>(new byte[23]),
                        new ArraySegment<byte>(new byte[256], 2, 100),
                        new ArraySegment<byte>(new byte[1], 0, 0),
                        new ArraySegment<byte>(new byte[64], 9, 9)};
                        for (int sent = 0, toSend = BytesToSend; toSend > 0; toSend -= sent)
                        {
                            for (int i = 0; i < sendBuffers.Count; i++)
                            {
                                random.NextBytes(sendBuffers[i].Array);
                            }

                            sent = await SendAsync(client, sendBuffers);

                            bytesSent += sent;
                            for (int i = 0, remaining = sent; i < sendBuffers.Count && remaining > 0; i++)
                            {
                                ArraySegment<byte> buffer = sendBuffers[i];
                                int toAdd = Math.Min(buffer.Count, remaining);
                                sentChecksum.Add(buffer.Array, buffer.Offset, toAdd);
                                remaining -= toAdd;
                            }
                        }
                    }

                    client.LingerState = new LingerOption(true, LingerTime);
                    client.Shutdown(SocketShutdown.Send);
                    await serverProcessingTask;
                }

                Assert.Equal(bytesSent, bytesReceived);
                Assert.Equal(sentChecksum.Sum, receivedChecksum.Sum);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public async Task SendRecv_Stream_TCP_LargeMultiBufferSends(IPAddress listenAt)
        {
            using (var listener = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.BindToAnonymousPort(listenAt);
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await client.ConnectAsync(listener.LocalEndPoint);
                using (Socket server = await acceptTask)
                {
                    var sentChecksum = new Fletcher32();
                    var rand = new Random();
                    int bytesToSend = 0;
                    var buffers = new List<ArraySegment<byte>>();
                    const int NumBuffers = 5;
                    for (int i = 0; i < NumBuffers; i++)
                    {
                        var sendBuffer = new byte[12345678];
                        rand.NextBytes(sendBuffer);
                        bytesToSend += sendBuffer.Length - i; // trim off a few bytes to test offset/count
                        sentChecksum.Add(sendBuffer, i, sendBuffer.Length - i);
                        buffers.Add(new ArraySegment<byte>(sendBuffer, i, sendBuffer.Length - i));
                    }

                    Task<int> sendTask = SendAsync(client, buffers);

                    var receivedChecksum = new Fletcher32();
                    int bytesReceived = 0;
                    byte[] recvBuffer = new byte[1024];
                    while (bytesReceived < bytesToSend)
                    {
                        int received = await ReceiveAsync(server, new ArraySegment<byte>(recvBuffer));
                        if (received <= 0)
                        {
                            break;
                        }
                        bytesReceived += received;
                        receivedChecksum.Add(recvBuffer, 0, received);
                    }

                    Assert.Equal(bytesToSend, await sendTask);
                    Assert.Equal(sentChecksum.Sum, receivedChecksum.Sum);
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public async Task SendRecv_Stream_TCP_AlternateBufferAndBufferList(IPAddress listenAt)
        {
            const int BytesToSend = 123456;
            int bytesReceived = 0, bytesSent = 0;
            Fletcher32 receivedChecksum = new Fletcher32(), sentChecksum = new Fletcher32();

            using (var server = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                server.BindToAnonymousPort(listenAt);
                server.Listen(1);

                Task serverProcessingTask = Task.Run(async () =>
                {
                    using (Socket remote = await AcceptAsync(server))
                    {
                        byte[] recvBuffer1 = new byte[256], recvBuffer2 = new byte[256];
                        long iter = 0;
                        while (true)
                        {
                            ArraySegment<byte> seg1 = new ArraySegment<byte>(recvBuffer1), seg2 = new ArraySegment<byte>(recvBuffer2);
                            int received;
                            switch (iter++ % 3)
                            {
                                case 0: // single buffer
                                    received = await ReceiveAsync(remote, seg1);
                                    break;
                                case 1: // buffer list with a single buffer
                                    received = await ReceiveAsync(remote, new List<ArraySegment<byte>> { seg1 });
                                    break;
                                default: // buffer list with multiple buffers
                                    received = await ReceiveAsync(remote, new List<ArraySegment<byte>> { seg1, seg2 });
                                    break;
                            }
                            if (received == 0)
                            {
                                break;
                            }

                            bytesReceived += received;
                            receivedChecksum.Add(recvBuffer1, 0, Math.Min(received, recvBuffer1.Length));
                            if (received > recvBuffer1.Length)
                            {
                                receivedChecksum.Add(recvBuffer2, 0, received - recvBuffer1.Length);
                            }
                        }
                    }
                });

                EndPoint clientEndpoint = server.LocalEndPoint;
                using (var client = new Socket(clientEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    await ConnectAsync(client, clientEndpoint);

                    var random = new Random();
                    byte[] sendBuffer1 = new byte[512], sendBuffer2 = new byte[512];
                    long iter = 0;
                    for (int sent = 0, remaining = BytesToSend; remaining > 0; remaining -= sent)
                    {
                        random.NextBytes(sendBuffer1);
                        random.NextBytes(sendBuffer2);
                        int amountFromSendBuffer1 = Math.Min(sendBuffer1.Length, remaining);
                        switch (iter++ % 3)
                        {
                            case 0: // single buffer
                                sent = await SendAsync(client, new ArraySegment<byte>(sendBuffer1, 0, amountFromSendBuffer1));
                                break;
                            case 1: // buffer list with a single buffer
                                sent = await SendAsync(client, new List<ArraySegment<byte>>
                                {
                                    new ArraySegment<byte>(sendBuffer1, 0, amountFromSendBuffer1)
                                });
                                break;
                            default: // buffer list with multiple buffers
                                sent = await SendAsync(client, new List<ArraySegment<byte>>
                                {
                                    new ArraySegment<byte>(sendBuffer1, 0, amountFromSendBuffer1),
                                    new ArraySegment<byte>(sendBuffer2, 0, Math.Min(sendBuffer2.Length, remaining - amountFromSendBuffer1)),
                                });
                                break;
                        }

                        bytesSent += sent;
                        sentChecksum.Add(sendBuffer1, 0, Math.Min(sent, sendBuffer1.Length));
                        if (sent > sendBuffer1.Length)
                        {
                            sentChecksum.Add(sendBuffer2, 0, sent - sendBuffer1.Length);
                        }
                    }

                    client.Shutdown(SocketShutdown.Send);
                    await serverProcessingTask;
                }

                Assert.Equal(bytesSent, bytesReceived);
                Assert.Equal(sentChecksum.Sum, receivedChecksum.Sum);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(LoopbacksAndBuffers))]
        public async Task SendRecv_Stream_TCP_MultipleConcurrentReceives(IPAddress listenAt, bool useMultipleBuffers)
        {
            using (var server = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                server.BindToAnonymousPort(listenAt);
                server.Listen(1);

                EndPoint clientEndpoint = server.LocalEndPoint;
                using (var client = new Socket(clientEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    Task clientConnect = ConnectAsync(client, clientEndpoint);
                    using (Socket remote = await AcceptAsync(server))
                    {
                        await clientConnect;

                        if (useMultipleBuffers)
                        {
                            byte[] buffer1 = new byte[1], buffer2 = new byte[1], buffer3 = new byte[1], buffer4 = new byte[1], buffer5 = new byte[1];

                            Task<int> receive1 = ReceiveAsync(client, new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer1), new ArraySegment<byte>(buffer2) });
                            Task<int> receive2 = ReceiveAsync(client, new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer3), new ArraySegment<byte>(buffer4) });
                            Task<int> receive3 = ReceiveAsync(client, new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer5) });

                            await Task.WhenAll(
                                SendAsync(remote, new ArraySegment<byte>(new byte[] { 1, 2, 3, 4, 5 })),
                                receive1, receive2, receive3);

                            Assert.True(receive1.Result == 1 || receive1.Result == 2, $"Expected 1 or 2, got {receive1.Result}");
                            Assert.True(receive2.Result == 1 || receive2.Result == 2, $"Expected 1 or 2, got {receive2.Result}");
                            Assert.Equal(1, receive3.Result);

                            if (GuaranteedSendOrdering)
                            {
                                if (receive1.Result == 1 && receive2.Result == 1)
                                {
                                    Assert.Equal(1, buffer1[0]);
                                    Assert.Equal(0, buffer2[0]);
                                    Assert.Equal(2, buffer3[0]);
                                    Assert.Equal(0, buffer4[0]);
                                    Assert.Equal(3, buffer5[0]);
                                }
                                else if (receive1.Result == 1 && receive2.Result == 2)
                                {
                                    Assert.Equal(1, buffer1[0]);
                                    Assert.Equal(0, buffer2[0]);
                                    Assert.Equal(2, buffer3[0]);
                                    Assert.Equal(3, buffer4[0]);
                                    Assert.Equal(4, buffer5[0]);
                                }
                                else if (receive1.Result == 2 && receive2.Result == 1)
                                {
                                    Assert.Equal(1, buffer1[0]);
                                    Assert.Equal(2, buffer2[0]);
                                    Assert.Equal(3, buffer3[0]);
                                    Assert.Equal(0, buffer4[0]);
                                    Assert.Equal(4, buffer5[0]);
                                }
                                else // receive1.Result == 2 && receive2.Result == 2
                                {
                                    Assert.Equal(1, buffer1[0]);
                                    Assert.Equal(2, buffer2[0]);
                                    Assert.Equal(3, buffer3[0]);
                                    Assert.Equal(4, buffer4[0]);
                                    Assert.Equal(5, buffer5[0]);
                                }
                            }
                        }
                        else
                        {
                            var buffer1 = new ArraySegment<byte>(new byte[1]);
                            var buffer2 = new ArraySegment<byte>(new byte[1]);
                            var buffer3 = new ArraySegment<byte>(new byte[1]);

                            Task<int> receive1 = ReceiveAsync(client, buffer1);
                            Task<int> receive2 = ReceiveAsync(client, buffer2);
                            Task<int> receive3 = ReceiveAsync(client, buffer3);

                            await Task.WhenAll(
                                SendAsync(remote, new ArraySegment<byte>(new byte[] { 1, 2, 3 })),
                                receive1, receive2, receive3);

                            Assert.Equal(3, receive1.Result + receive2.Result + receive3.Result);

                            if (GuaranteedSendOrdering)
                            {
                                Assert.Equal(1, buffer1.Array[0]);
                                Assert.Equal(2, buffer2.Array[0]);
                                Assert.Equal(3, buffer3.Array[0]);
                            }
                        }
                    }
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(LoopbacksAndBuffers))]
        public async Task SendRecv_Stream_TCP_MultipleConcurrentSends(IPAddress listenAt, bool useMultipleBuffers)
        {
            using (var server = new Socket(listenAt.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                byte[] sendData = new byte[5000000];
                new Random(42).NextBytes(sendData);

                Func<byte[], int, int, byte[]> slice = (input, offset, count) =>
                {
                    var arr = new byte[count];
                    Array.Copy(input, offset, arr, 0, count);
                    return arr;
                };

                server.BindToAnonymousPort(listenAt);
                server.Listen(1);

                EndPoint clientEndpoint = server.LocalEndPoint;
                using (var client = new Socket(clientEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    Task clientConnect = ConnectAsync(client, clientEndpoint);
                    using (Socket remote = await AcceptAsync(server))
                    {
                        await clientConnect;

                        Task<int> send1, send2, send3;
                        if (useMultipleBuffers)
                        {
                            var bufferList1 = new List<ArraySegment<byte>> { new ArraySegment<byte>(slice(sendData, 0, 1000000)), new ArraySegment<byte>(slice(sendData, 1000000, 1000000)) };
                            var bufferList2 = new List<ArraySegment<byte>> { new ArraySegment<byte>(slice(sendData, 2000000, 1000000)), new ArraySegment<byte>(slice(sendData, 3000000, 1000000)) };
                            var bufferList3 = new List<ArraySegment<byte>> { new ArraySegment<byte>(slice(sendData, 4000000, 1000000)) };

                            send1 = SendAsync(client, bufferList1);
                            send2 = SendAsync(client, bufferList2);
                            send3 = SendAsync(client, bufferList3);
                        }
                        else
                        {
                            var buffer1 = new ArraySegment<byte>(slice(sendData, 0, 2000000));
                            var buffer2 = new ArraySegment<byte>(slice(sendData, 2000000, 2000000));
                            var buffer3 = new ArraySegment<byte>(slice(sendData, 4000000, 1000000));

                            send1 = SendAsync(client, buffer1);
                            send2 = SendAsync(client, buffer2);
                            send3 = SendAsync(client, buffer3);
                        }

                        int receivedTotal = 0;
                        int received;
                        var receiveBuffer = new byte[sendData.Length];
                        while (receivedTotal < receiveBuffer.Length)
                        {
                            if ((received = await ReceiveAsync(remote, new ArraySegment<byte>(receiveBuffer, receivedTotal, receiveBuffer.Length - receivedTotal))) == 0) break;
                            receivedTotal += received;
                        }
                        Assert.Equal(5000000, receivedTotal);
                        if (GuaranteedSendOrdering)
                        {
                            Assert.Equal(sendData, receiveBuffer);
                        }
                    }
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(LoopbacksAndBuffers))]
        public void SendRecvPollSync_TcpListener_Socket(IPAddress listenAt, bool pollBeforeOperation)
        {
            const int BytesToSend = 123456;
            const int ListenBacklog = 1;
            const int TestTimeout = 30000;

            var listener = new TcpListener(listenAt, 0);
            listener.Start(ListenBacklog);
            try
            {
                int bytesReceived = 0;
                var receivedChecksum = new Fletcher32();
                Task serverTask = Task.Run(async () =>
                {
                    using (Socket remote = await listener.AcceptSocketAsync())
                    {
                        var recvBuffer = new byte[256];
                        while (true)
                        {
                            if (pollBeforeOperation)
                            {
                                Assert.True(remote.Poll(-1, SelectMode.SelectRead), "Read poll before completion should have succeeded");
                            }
                            int received = remote.Receive(recvBuffer, 0, recvBuffer.Length, SocketFlags.None);
                            if (received == 0)
                            {
                                Assert.True(remote.Poll(0, SelectMode.SelectRead), "Read poll after completion should have succeeded");
                                break;
                            }

                            bytesReceived += received;
                            receivedChecksum.Add(recvBuffer, 0, received);
                        }
                    }
                });

                int bytesSent = 0;
                var sentChecksum = new Fletcher32();
                Task clientTask = Task.Run(async () =>
                {
                    var clientEndpoint = (IPEndPoint)listener.LocalEndpoint;

                    using (var client = new Socket(clientEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                    {
                        await ConnectAsync(client, clientEndpoint);

                        if (pollBeforeOperation)
                        {
                            Assert.False(client.Poll(TestTimeout, SelectMode.SelectRead), "Expected writer's read poll to fail after timeout");
                        }

                        var random = new Random();
                        var sendBuffer = new byte[512];
                        for (int remaining = BytesToSend, sent = 0; remaining > 0; remaining -= sent)
                        {
                            random.NextBytes(sendBuffer);

                            if (pollBeforeOperation)
                            {
                                Assert.True(client.Poll(-1, SelectMode.SelectWrite), "Write poll should have succeeded");
                            }
                            sent = client.Send(sendBuffer, 0, Math.Min(sendBuffer.Length, remaining), SocketFlags.None);

                            bytesSent += sent;
                            sentChecksum.Add(sendBuffer, 0, sent);
                        }
                    }
                });

                Assert.True(Task.WaitAll(new[] { serverTask, clientTask }, TestTimeout), "Wait timed out");

                Assert.Equal(bytesSent, bytesReceived);
                Assert.Equal(sentChecksum.Sum, receivedChecksum.Sum);
            }
            finally
            {
                listener.Stop();
            }
        }

        [Fact]
        public async Task SendRecv_0ByteReceive_Success()
        {
            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await Task.WhenAll(
                    acceptTask,
                    ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndPoint).Port)));

                using (Socket server = await acceptTask)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        // Have the client do a 0-byte receive.  No data is available, so this should pend.
                        Task<int> receive = ReceiveAsync(client, new ArraySegment<byte>(Array.Empty<byte>()));
                        Assert.False(receive.IsCompleted);
                        Assert.Equal(0, client.Available);

                        // Have the server send 1 byte to the client.
                        Assert.Equal(1, server.Send(new byte[1], 0, 1, SocketFlags.None));
                        Assert.Equal(0, server.Available);

                        // The client should now wake up, getting 0 bytes with 1 byte available.
                        Assert.Equal(0, await receive);
                        Assert.Equal(1, client.Available);

                        // We should be able to do another 0-byte receive that completes immediateliy
                        Assert.Equal(0, await ReceiveAsync(client, new ArraySegment<byte>(new byte[1], 0, 0)));
                        Assert.Equal(1, client.Available);

                        // Then receive the byte
                        Assert.Equal(1, await ReceiveAsync(client, new ArraySegment<byte>(new byte[1])));
                        Assert.Equal(0, client.Available);
                    }
                }
            }
        }

        [Fact]
        public async Task Receive0ByteReturns_WhenPeerDisconnects()
        {
            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await Task.WhenAll(
                    acceptTask,
                    ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndPoint).Port)));

                using (Socket server = await acceptTask)
                {
                    // Have the client do a 0-byte receive.  No data is available, so this should pend.
                    Task<int> receive = ReceiveAsync(client, new ArraySegment<byte>(Array.Empty<byte>()));
                    Assert.False(receive.IsCompleted, $"Task should not have been completed, was {receive.Status}");

                    // Disconnect the client
                    server.Close();

                    // The client should now wake up
                    Assert.Equal(0, await receive);
                }
            }
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 1)]
        public async Task SendRecv_BlockingNonBlocking_LingerTimeout_Success(bool blocking, int lingerTimeout)
        {
            if (UsesSync) return;

            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Blocking = blocking;
                listener.Blocking = blocking;

                client.LingerState = new LingerOption(true, lingerTimeout);
                listener.LingerState = new LingerOption(true, lingerTimeout);

                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await Task.WhenAll(
                    acceptTask,
                    ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndPoint).Port)));

                using (Socket server = await acceptTask)
                {
                    server.Blocking = blocking;
                    server.LingerState = new LingerOption(true, lingerTimeout);

                    Task<int> receive = ReceiveAsync(client, new ArraySegment<byte>(new byte[1]));
                    Assert.Equal(1, await SendAsync(server, new ArraySegment<byte>(new byte[1])));
                    Assert.Equal(1, await receive);
                }
            }
        }

        [Fact]
        [PlatformSpecific(~TestPlatforms.OSX)] // SendBufferSize, ReceiveBufferSize = 0 not supported on OSX.
        public async Task SendRecv_NoBuffering_Success()
        {
            if (UsesSync) return;

            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await Task.WhenAll(
                    acceptTask,
                    ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndPoint).Port)));

                using (Socket server = await acceptTask)
                {
                    client.SendBufferSize = 0;
                    server.ReceiveBufferSize = 0;

                    var sendBuffer = new byte[10000];
                    Task sendTask = SendAsync(client, new ArraySegment<byte>(sendBuffer));

                    int totalReceived = 0;
                    var receiveBuffer = new ArraySegment<byte>(new byte[4096]);
                    while (totalReceived < sendBuffer.Length)
                    {
                        int received = await ReceiveAsync(server, receiveBuffer);
                        if (received <= 0) break;
                        totalReceived += received;
                    }
                    Assert.Equal(sendBuffer.Length, totalReceived);
                    await sendTask;
                }
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public async Task SendRecv_DisposeDuringPendingReceive_ThrowsSocketException()
        {
            if (UsesSync) return; // if sync, can't guarantee call will have been initiated by time of disposal

            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                Task<Socket> acceptTask = AcceptAsync(listener);
                await Task.WhenAll(
                    acceptTask,
                    ConnectAsync(client, new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndPoint).Port)));

                using (Socket server = await acceptTask)
                {
                    Task receiveTask = ReceiveAsync(client, new ArraySegment<byte>(new byte[1]));
                    Assert.False(receiveTask.IsCompleted, "Receive should be pending");

                    client.Dispose();

                    if (DisposeDuringOperationResultsInDisposedException)
                    {
                        await Assert.ThrowsAsync<ObjectDisposedException>(() => receiveTask);
                    }
                    else
                    {
                        var se = await Assert.ThrowsAsync<SocketException>(() => receiveTask);
                        Assert.True(
                            se.SocketErrorCode == SocketError.OperationAborted || se.SocketErrorCode == SocketError.ConnectionAborted,
                            $"Expected {nameof(SocketError.OperationAborted)} or {nameof(SocketError.ConnectionAborted)}, got {se.SocketErrorCode}");
                    }
                }
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)]
        public void SocketSendReceiveBufferSize_SetZero_ThrowsSocketException()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                SocketException e;
                e = Assert.Throws<SocketException>(() => socket.SendBufferSize = 0);
                Assert.Equal(e.SocketErrorCode, SocketError.InvalidArgument);

                e = Assert.Throws<SocketException>(() => socket.ReceiveBufferSize = 0);
                Assert.Equal(e.SocketErrorCode, SocketError.InvalidArgument);
            }
        }
    }

    public sealed class SendReceiveUdpClient : MemberDatas
    {
        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public void SendToRecvFromAsync_Datagram_UDP_UdpClient(IPAddress loopbackAddress)
        {
            IPAddress leftAddress = loopbackAddress, rightAddress = loopbackAddress;

            // TODO #5185: harden against packet loss
            const int DatagramSize = 256;
            const int DatagramsToSend = 256;
            const int AckTimeout = 1000;
            const int TestTimeout = 30000;

            using (var left = new UdpClient(new IPEndPoint(leftAddress, 0)))
            using (var right = new UdpClient(new IPEndPoint(rightAddress, 0)))
            {
                var leftEndpoint = (IPEndPoint)left.Client.LocalEndPoint;
                var rightEndpoint = (IPEndPoint)right.Client.LocalEndPoint;

                var receiverAck = new ManualResetEventSlim();
                var senderAck = new ManualResetEventSlim();

                var receivedChecksums = new uint?[DatagramsToSend];
                int receivedDatagrams = 0;

                Task receiverTask = Task.Run(async () =>
                {
                    for (; receivedDatagrams < DatagramsToSend; receivedDatagrams++)
                    {
                        UdpReceiveResult result = await left.ReceiveAsync();

                        receiverAck.Set();
                        Assert.True(senderAck.Wait(AckTimeout));
                        senderAck.Reset();

                        Assert.Equal(DatagramSize, result.Buffer.Length);
                        Assert.Equal(rightEndpoint, result.RemoteEndPoint);

                        int datagramId = (int)result.Buffer[0];
                        Assert.Null(receivedChecksums[datagramId]);

                        receivedChecksums[datagramId] = Fletcher32.Checksum(result.Buffer, 0, result.Buffer.Length);
                    }
                });

                var sentChecksums = new uint[DatagramsToSend];
                int sentDatagrams = 0;

                Task senderTask = Task.Run(async () =>
                {
                    var random = new Random();
                    var sendBuffer = new byte[DatagramSize];

                    for (; sentDatagrams < DatagramsToSend; sentDatagrams++)
                    {
                        random.NextBytes(sendBuffer);
                        sendBuffer[0] = (byte)sentDatagrams;

                        int sent = await right.SendAsync(sendBuffer, DatagramSize, leftEndpoint);

                        Assert.True(receiverAck.Wait(AckTimeout));
                        receiverAck.Reset();
                        senderAck.Set();

                        Assert.Equal(DatagramSize, sent);
                        sentChecksums[sentDatagrams] = Fletcher32.Checksum(sendBuffer, 0, sent);
                    }
                });

                Assert.True(Task.WaitAll(new[] { receiverTask, senderTask }, TestTimeout));
                for (int i = 0; i < DatagramsToSend; i++)
                {
                    Assert.NotNull(receivedChecksums[i]);
                    Assert.Equal(sentChecksums[i], (uint)receivedChecksums[i]);
                }
            }
        }
    }

    public sealed class SendReceiveListener : MemberDatas
    {
        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [MemberData(nameof(Loopbacks))]
        public void SendRecvAsync_TcpListener_TcpClient(IPAddress listenAt)
        {
            const int BytesToSend = 123456;
            const int ListenBacklog = 1;
            const int LingerTime = 10;
            const int TestTimeout = 30000;

            var listener = new TcpListener(listenAt, 0);
            listener.Start(ListenBacklog);

            int bytesReceived = 0;
            var receivedChecksum = new Fletcher32();
            Task serverTask = Task.Run(async () =>
            {
                using (TcpClient remote = await listener.AcceptTcpClientAsync())
                using (NetworkStream stream = remote.GetStream())
                {
                    var recvBuffer = new byte[256];
                    for (;;)
                    {
                        int received = await stream.ReadAsync(recvBuffer, 0, recvBuffer.Length);
                        if (received == 0)
                        {
                            break;
                        }

                        bytesReceived += received;
                        receivedChecksum.Add(recvBuffer, 0, received);
                    }
                }
            });

            int bytesSent = 0;
            var sentChecksum = new Fletcher32();
            Task clientTask = Task.Run(async () =>
            {
                var clientEndpoint = (IPEndPoint)listener.LocalEndpoint;

                using (var client = new TcpClient(clientEndpoint.AddressFamily))
                {
                    await client.ConnectAsync(clientEndpoint.Address, clientEndpoint.Port);

                    using (NetworkStream stream = client.GetStream())
                    {
                        var random = new Random();
                        var sendBuffer = new byte[512];
                        for (int remaining = BytesToSend, sent = 0; remaining > 0; remaining -= sent)
                        {
                            random.NextBytes(sendBuffer);

                            sent = Math.Min(sendBuffer.Length, remaining);
                            await stream.WriteAsync(sendBuffer, 0, sent);

                            bytesSent += sent;
                            sentChecksum.Add(sendBuffer, 0, sent);
                        }

                        client.LingerState = new LingerOption(true, LingerTime);
                    }
                }
            });

            Assert.True(Task.WaitAll(new[] { serverTask, clientTask }, TestTimeout),
                $"Time out waiting for serverTask ({serverTask.Status}) and clientTask ({clientTask.Status})");

            Assert.Equal(bytesSent, bytesReceived);
            Assert.Equal(sentChecksum.Sum, receivedChecksum.Sum);
        }
    }

    public sealed class SendReceiveSync : SendReceive<SocketHelperSync> { }
    public sealed class SendReceiveSyncForceNonBlocking : SendReceive<SocketHelperSyncForceNonBlocking> { }
    public sealed class SendReceiveApm : SendReceive<SocketHelperApm> { }
    public sealed class SendReceiveTask : SendReceive<SocketHelperTask> { }
    public sealed class SendReceiveEap : SendReceive<SocketHelperEap> { }
}
