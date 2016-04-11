// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class HttpClientHandler_Timeouts_Test
    {
        [Fact]
        public void Timeouts_DefaultValue_Infinite()
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.ConnectTimeout);
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.SendTimeout);
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.ReceiveHeadersTimeout);
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.ReceiveDataTimeout);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(int.MaxValue + 1L)]
        public void Timeouts_InvalidValues(long ms)
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.ConnectTimeout = TimeSpan.FromMilliseconds(ms));
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.SendTimeout = TimeSpan.FromMilliseconds(ms));
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.ReceiveHeadersTimeout = TimeSpan.FromMilliseconds(ms));
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.ReceiveDataTimeout = TimeSpan.FromMilliseconds(ms));
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(int.MaxValue - 1)]
        [InlineData(int.MaxValue)]
        public void Timeouts_ValidValues_Roundtrip(long ms)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ConnectTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.ConnectTimeout);

                handler.SendTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.SendTimeout);

                handler.ReceiveHeadersTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.ReceiveHeadersTimeout);

                handler.ReceiveDataTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.ReceiveDataTimeout);
            }
        }

        [Fact]
        public async Task Timeouts_SetBeforeUse_Succeeds_SetAfterUse_Throws()
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(int.MaxValue);

                handler.ConnectTimeout = timeout;
                handler.SendTimeout = timeout;
                handler.ReceiveHeadersTimeout = timeout;
                handler.ReceiveDataTimeout = timeout;

                await client.GetAsync(HttpTestServers.RemoteEchoServer);

                Assert.Equal(timeout, handler.ConnectTimeout);
                Assert.Equal(timeout, handler.SendTimeout);
                Assert.Equal(timeout, handler.ReceiveHeadersTimeout);
                Assert.Equal(timeout, handler.ReceiveDataTimeout);

                Assert.Throws<InvalidOperationException>(() => handler.ConnectTimeout = TimeSpan.FromMilliseconds(1));
                Assert.Throws<InvalidOperationException>(() => handler.SendTimeout = TimeSpan.FromMilliseconds(1));
                Assert.Throws<InvalidOperationException>(() => handler.ReceiveHeadersTimeout = TimeSpan.FromMilliseconds(1));
                Assert.Throws<InvalidOperationException>(() => handler.ReceiveDataTimeout = TimeSpan.FromMilliseconds(1));
            }
        }

        [Fact]
        public Task ConnectTimeout_TimesOut_Throws()
        {
            return CreateServer(async (server, uri, handler, client) =>
            {
                const int NumBacklogSockets = 16; // must be larger than OS' queue length when using Listen(1).
                var socketBacklog = new List<Socket>(NumBacklogSockets);
                try
                {
                    handler.ConnectTimeout = TimeSpan.FromMilliseconds(10);

                    // Listen's backlog is only advisory; the OS may actually allow a larger backlog than that.
                    // As such, create a bunch of clients to connect to the endpoint so that our actual request
                    // will timeout while trying to connect.
                    for (int i = 0; i < NumBacklogSockets; i++)
                    {
                        var tmpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        var ignored = tmpClient.ConnectAsync((IPEndPoint)server.LocalEndPoint);
                        socketBacklog.Add(tmpClient);
                    }

                    // Make the actual connection.  It should timeout in connect.
                    var sw = Stopwatch.StartNew();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetAsync(uri));
                    sw.Stop();

                    Assert.InRange(sw.ElapsedMilliseconds, 1, 10 * 1000); // allow a very wide range
                }
                finally
                {
                    foreach (Socket c in socketBacklog)
                    {
                        c.Dispose();
                    }
                }
            });
        }

        [Fact]
        public Task SendTimeout_TimesOut_Throws()
        {
            return CreateServer(async (server, uri, handler, client) =>
            {
                handler.SendTimeout = TimeSpan.FromMilliseconds(10);

                var sw = Stopwatch.StartNew();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.PostAsync(uri, new StringContent("This body won't be received")));
                sw.Stop();

                Assert.InRange(sw.ElapsedMilliseconds, 1, 10 * 1000); // allow a very wide range
            });
        }

        [Fact]
        public Task ReceiveHeadersTimeout_TimesOut_Throws()
        {
            return CreateServer(async (server, uri, handler, client) =>
            {
                handler.ReceiveHeadersTimeout = TimeSpan.FromSeconds(1);

                Task<HttpResponseMessage> getAsync = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

                using (Socket conn = server.Accept())
                using (Stream serverStream = new NetworkStream(conn, ownsSocket: false))
                using (StreamReader reader = new StreamReader(serverStream, Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && !string.IsNullOrEmpty(line)) ;

                    byte[] headerDataNoEnd = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n");
                    serverStream.Write(headerDataNoEnd, 0, headerDataNoEnd.Length);

                    var sw = Stopwatch.StartNew();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => getAsync);
                    sw.Stop();

                    Assert.InRange(sw.ElapsedMilliseconds, 1, 10 * 1000); // allow a very wide range
                }
            });
        }

        [Fact]
        public Task ReceiveDataTimeout_TimesOut_Throws()
        {
            return CreateServer(async (server, uri, handler, client) =>
            {
                handler.ReceiveDataTimeout = TimeSpan.FromMilliseconds(10);

                Task<HttpResponseMessage> getAsync = client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);

                using (Socket conn = server.Accept())
                using (Stream serverStream = new NetworkStream(conn, ownsSocket: false))
                using (StreamReader reader = new StreamReader(serverStream, Encoding.ASCII))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && !string.IsNullOrEmpty(line)) ;

                    byte[] bodyData = Encoding.ASCII.GetBytes("This is the body.");
                    byte[] headerData = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Length: {bodyData.Length}\r\n\r\n");
                    serverStream.Write(headerData, 0, headerData.Length);
                    serverStream.Write(bodyData, 0, bodyData.Length / 2);

                    var sw = Stopwatch.StartNew();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => getAsync);
                    sw.Stop();

                    Assert.InRange(sw.ElapsedMilliseconds, 1, 10 * 1000); // allow a very wide range
                }
            });
        }

        private static async Task CreateServer(Func<Socket, Uri, HttpClientHandler, HttpClient, Task> func)
        {
            using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                server.Listen(1);

                IPEndPoint ep = (IPEndPoint)server.LocalEndPoint;
                var uri = new Uri($"http://{ep.Address}:{ep.Port}");

                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    await func(server, uri, handler, client).ConfigureAwait(false);
                }
            }
        }

    }
}
