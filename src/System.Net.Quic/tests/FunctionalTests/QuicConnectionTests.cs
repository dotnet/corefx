// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Quic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Quic.Tests
{
    public class QuicConnectionTests
    {
        private static ReadOnlyMemory<byte> s_data = Encoding.UTF8.GetBytes("Hello world!");

        [Fact]
        public async Task BasicTest()
        {
            using (QuicListener listener = new QuicListener(new IPEndPoint(IPAddress.Loopback, 0), sslServerAuthenticationOptions: null, mock: true))
            {
                IPEndPoint listenEndPoint = listener.ListenEndPoint;

                await Task.WhenAll(
                    Task.Run(async () =>
                        {
                            // Client code
                            using (QuicConnection connection = new QuicConnection(listenEndPoint, sslClientAuthenticationOptions: null, mock: true))
                            {
                                await connection.ConnectAsync();
                                using (QuicStream stream = connection.CreateBidirectionalStream())
                                {
                                    await stream.WriteAsync(s_data);
                                }
                            }
                        }),
                    Task.Run(async () =>
                        {
                            // Server code
                            using (QuicConnection connection = await listener.AcceptConnectionAsync())
                            {
                                using (QuicStream stream = await connection.AcceptStreamAsync())
                                {
                                    byte[] buffer = new byte[s_data.Length];
                                    int bytesRead = await stream.ReadAsync(buffer);
                                    Assert.Equal(s_data.Length, bytesRead);
                                    Assert.True(s_data.Span.SequenceEqual(buffer));
                                }
                            }
                        }));
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestStreams(bool explicitConnect)
        {
            using (QuicListener listener = new QuicListener(new IPEndPoint(IPAddress.Loopback, 0), sslServerAuthenticationOptions: null, mock: true))
            {
                IPEndPoint listenEndPoint = listener.ListenEndPoint;

                using (QuicConnection clientConnection = new QuicConnection(listenEndPoint, sslClientAuthenticationOptions: null, mock: true))
                {
                    Assert.False(clientConnection.Connected);
                    Assert.Equal(listenEndPoint, clientConnection.RemoteEndPoint);

                    ValueTask connectTask = clientConnection.ConnectAsync();
                    QuicConnection serverConnection = await listener.AcceptConnectionAsync();
                    await connectTask;

                    Assert.True(clientConnection.Connected);
                    Assert.True(serverConnection.Connected);
                    Assert.Equal(listenEndPoint, serverConnection.LocalEndPoint);
                    Assert.Equal(listenEndPoint, clientConnection.RemoteEndPoint);
                    Assert.Equal(clientConnection.LocalEndPoint, serverConnection.RemoteEndPoint);

                    await CreateAndTestBidirectionalStream(clientConnection, serverConnection, explicitConnect);
                    await CreateAndTestBidirectionalStream(serverConnection, clientConnection, explicitConnect);
                    await CreateAndTestUnidirectionalStream(serverConnection, clientConnection, explicitConnect);
                    await CreateAndTestUnidirectionalStream(clientConnection, serverConnection, explicitConnect);
                }
            }
        }

        private static async Task CreateAndTestBidirectionalStream(QuicConnection c1, QuicConnection c2, bool explicitConnect)
        {
            using (QuicStream s1 = c1.CreateBidirectionalStream())
            {
                Assert.True(s1.CanRead);
                Assert.True(s1.CanWrite);
                Assert.False(s1.Connected);
                Assert.Equal(s1.StreamId, -1);

                if (explicitConnect)
                {
                    await s1.ConnectAsync();
                    using (QuicStream s2 = await c2.AcceptStreamAsync())
                    {
                        await TestBidirectionalStream(s1, s2);
                    }
                }
                else
                {
                    ValueTask writeTask = s1.WriteAsync(s_data);
                    using (QuicStream s2 = await c2.AcceptStreamAsync())
                    {
                        await ReceiveDataAsync(s_data, s2);
                        await writeTask;
                        await TestBidirectionalStream(s1, s2);
                    }
                }
            }
        }

        private static async Task CreateAndTestUnidirectionalStream(QuicConnection c1, QuicConnection c2, bool explicitConnect)
        {
            using (QuicStream s1 = c1.CreateUnidirectionalStream())
            {
                Assert.False(s1.CanRead);
                Assert.True(s1.CanWrite);
                Assert.False(s1.Connected);
                Assert.Equal(s1.StreamId, -1);

                if (explicitConnect)
                {
                    await s1.ConnectAsync();
                    using (QuicStream s2 = await c2.AcceptStreamAsync())
                    {
                        await TestUnidirectionalStream(s1, s2);
                    }
                }
                else
                {
                    ValueTask writeTask = s1.WriteAsync(s_data);
                    using (QuicStream s2 = await c2.AcceptStreamAsync())
                    {
                        await ReceiveDataAsync(s_data, s2);
                        await writeTask;
                        await TestUnidirectionalStream(s1, s2);
                    }
                }
            }
        }

        private static async Task TestBidirectionalStream(QuicStream s1, QuicStream s2)
        {
            Assert.True(s1.CanRead);
            Assert.True(s1.CanWrite);
            Assert.True(s2.CanRead);
            Assert.True(s2.CanWrite);
            Assert.True(s1.Connected);
            Assert.True(s2.Connected);
            Assert.Equal(s1.StreamId, s2.StreamId);

            await SendAndReceiveDataAsync(s_data, s1, s2);
            await SendAndReceiveDataAsync(s_data, s2, s1);
            await SendAndReceiveDataAsync(s_data, s2, s1);
            await SendAndReceiveDataAsync(s_data, s1, s2);

            await SendAndReceiveEOFAsync(s1, s2);
            await SendAndReceiveEOFAsync(s2, s1);
        }

        private static async Task TestUnidirectionalStream(QuicStream s1, QuicStream s2)
        {
            Assert.False(s1.CanRead);
            Assert.True(s1.CanWrite);
            Assert.True(s2.CanRead);
            Assert.False(s2.CanWrite);
            Assert.True(s1.Connected);
            Assert.True(s2.Connected);
            Assert.Equal(s1.StreamId, s2.StreamId);

            await SendAndReceiveDataAsync(s_data, s1, s2);
            await SendAndReceiveDataAsync(s_data, s1, s2);

            await SendAndReceiveEOFAsync(s1, s2);
        }

        private static async Task SendAndReceiveDataAsync(ReadOnlyMemory<byte> data, QuicStream s1, QuicStream s2)
        {
            await s1.WriteAsync(data);
            await ReceiveDataAsync(data, s2);
        }

        private static async Task ReceiveDataAsync(ReadOnlyMemory<byte> data, QuicStream s)
        {
            Memory<byte> readBuffer = new byte[data.Length];

            int bytesRead = 0;
            while (bytesRead < data.Length)
            {
                bytesRead += await s.ReadAsync(readBuffer.Slice(bytesRead));
            }

            Assert.True(data.Span.SequenceEqual(readBuffer.Span));
        }

        private static async Task SendAndReceiveEOFAsync(QuicStream s1, QuicStream s2)
        {
            byte[] readBuffer = new byte[1];

            s1.ShutdownWrite();

            int bytesRead = await s2.ReadAsync(readBuffer);
            Assert.Equal(0, bytesRead);

            // Another read should still give EOF
            bytesRead = await s2.ReadAsync(readBuffer);
            Assert.Equal(0, bytesRead);
        }
    }
}
