// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    public sealed class SocketsHttpHandler_HttpProtocolTests : HttpProtocolTests
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpProtocolTests_Dribble : HttpProtocolTests_Dribble
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientTest : HttpClientTest
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_DiagnosticsTest : DiagnosticsTest
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientEKUTest : HttpClientEKUTest
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_DangerousAcceptAllCertificatesValidator_Test : HttpClientHandler_DangerousAcceptAllCertificatesValidator_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_ClientCertificates_Test : HttpClientHandler_ClientCertificates_Test
    {
        public SocketsHttpHandler_HttpClientHandler_ClientCertificates_Test(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_DefaultProxyCredentials_Test : HttpClientHandler_DefaultProxyCredentials_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_MaxConnectionsPerServer_Test : HttpClientHandler_MaxConnectionsPerServer_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_ServerCertificates_Test : HttpClientHandler_ServerCertificates_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_ResponseDrain_Test : HttpClientHandler_ResponseDrain_Test
    {
        protected override bool UseSocketsHttpHandler => true;

        protected override void SetResponseDrainTimeout(HttpClientHandler handler, TimeSpan time)
        {
            SocketsHttpHandler s = (SocketsHttpHandler)GetUnderlyingSocketsHttpHandler(handler);
            Assert.NotNull(s);
            s.ResponseDrainTimeout = time;
        }

        [Fact]
        public void MaxResponseDrainSize_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(1024 * 1024, handler.MaxResponseDrainSize);

                handler.MaxResponseDrainSize = 0;
                Assert.Equal(0, handler.MaxResponseDrainSize);

                handler.MaxResponseDrainSize = int.MaxValue;
                Assert.Equal(int.MaxValue, handler.MaxResponseDrainSize);
            }
        }

        [Fact]
        public void MaxResponseDrainSize_InvalidArgument_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(1024 * 1024, handler.MaxResponseDrainSize);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxResponseDrainSize = -1);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxResponseDrainSize = int.MinValue);

                Assert.Equal(1024 * 1024, handler.MaxResponseDrainSize);
            }
        }

        [Fact]
        public void MaxResponseDrainSize_SetAfterUse_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {
                handler.MaxResponseDrainSize = 1;
                client.GetAsync("http://" + Guid.NewGuid().ToString("N")); // ignoring failure
                Assert.Equal(1, handler.MaxResponseDrainSize);
                Assert.Throws<InvalidOperationException>(() => handler.MaxResponseDrainSize = 1);
            }
        }

        [Fact]
        public void ResponseDrainTimeout_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(TimeSpan.FromSeconds(2), handler.ResponseDrainTimeout);

                handler.ResponseDrainTimeout = TimeSpan.Zero;
                Assert.Equal(TimeSpan.Zero, handler.ResponseDrainTimeout);

                handler.ResponseDrainTimeout = TimeSpan.FromTicks(int.MaxValue);
                Assert.Equal(TimeSpan.FromTicks(int.MaxValue), handler.ResponseDrainTimeout);
            }
        }

        [Fact]
        public void MaxResponseDraiTime_InvalidArgument_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(TimeSpan.FromSeconds(2), handler.ResponseDrainTimeout);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.ResponseDrainTimeout = TimeSpan.FromSeconds(-1));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.ResponseDrainTimeout = TimeSpan.MaxValue);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.ResponseDrainTimeout = TimeSpan.FromSeconds(int.MaxValue));

                Assert.Equal(TimeSpan.FromSeconds(2), handler.ResponseDrainTimeout);
            }
        }

        [Fact]
        public void ResponseDrainTimeout_SetAfterUse_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {
                handler.ResponseDrainTimeout = TimeSpan.FromSeconds(42);
                client.GetAsync("http://" + Guid.NewGuid().ToString("N")); // ignoring failure
                Assert.Equal(TimeSpan.FromSeconds(42), handler.ResponseDrainTimeout);
                Assert.Throws<InvalidOperationException>(() => handler.ResponseDrainTimeout = TimeSpan.FromSeconds(42));
            }
        }

        [OuterLoop]
        [Theory]
        [InlineData(1024 * 1024 * 2, 9_500, 1024 * 1024 * 3, ContentMode.ContentLength)]
        [InlineData(1024 * 1024 * 2, 9_500, 1024 * 1024 * 3, ContentMode.SingleChunk)]
        [InlineData(1024 * 1024 * 2, 9_500, 1024 * 1024 * 13, ContentMode.BytePerChunk)]
        public async Task GetAsyncWithMaxConnections_DisposeBeforeReadingToEnd_DrainsRequestsUnderMaxDrainSizeAndReusesConnection(int totalSize, int readSize, int maxDrainSize, ContentMode mode)
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    var handler = new SocketsHttpHandler();
                    handler.MaxResponseDrainSize = maxDrainSize;
                    handler.ResponseDrainTimeout = Timeout.InfiniteTimeSpan;

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain (or fail to)
                    handler.MaxConnectionsPerServer = 1;

                    using (var client = new HttpClient(handler))
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponseHeaders(response1, totalSize, mode);

                        // Read part but not all of response
                        Stream responseStream = await response1.Content.ReadAsStreamAsync();
                        await ReadToByteCount(responseStream, readSize);

                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on the same connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponseHeaders(response2, totalSize, mode);
                        Assert.Equal(totalSize, (await response2.Content.ReadAsStringAsync()).Length);
                    }
                },
                async server =>
                {
                    string content = new string('a', totalSize);
                    string response = GetResponseForContentMode(content, mode);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                        await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                    });
                });
        }

        [OuterLoop]
        [Theory]
        [InlineData(100_000, 0,  ContentMode.ContentLength)]
        [InlineData(100_000, 0, ContentMode.SingleChunk)]
        [InlineData(100_000, 0, ContentMode.BytePerChunk)]
        public async Task GetAsyncWithMaxConnections_DisposeLargerThanMaxDrainSize_KillsConnection(int totalSize, int maxDrainSize, ContentMode mode)
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    var handler = new SocketsHttpHandler();
                    handler.MaxResponseDrainSize = maxDrainSize;
                    handler.ResponseDrainTimeout = Timeout.InfiniteTimeSpan;

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain (or fail to)
                    handler.MaxConnectionsPerServer = 1;

                    using (var client = new HttpClient(handler))
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponseHeaders(response1, totalSize, mode);
                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on a new connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponseHeaders(response2, totalSize, mode);
                        Assert.Equal(totalSize, (await response2.Content.ReadAsStringAsync()).Length);
                    }
                },
                async server =>
                {
                    string content = new string('a', totalSize);
                    string response = GetResponseForContentMode(content, mode);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync();
                        try
                        {
                            await connection.Writer.WriteAsync(response);
                        }
                        catch (Exception) { }     // Eat errors from client disconnect.

                        await server.AcceptConnectionSendCustomResponseAndCloseAsync(response);
                    });
                });
        }

        [OuterLoop]
        [Theory]
        [InlineData(ContentMode.ContentLength)]
        [InlineData(ContentMode.SingleChunk)]
        [InlineData(ContentMode.BytePerChunk)]
        public async Task GetAsyncWithMaxConnections_DrainTakesLongerThanTimeout_KillsConnection(ContentMode mode)
        {
            const int ContentLength = 10_000;

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    var handler = new SocketsHttpHandler();
                    handler.MaxResponseDrainSize = int.MaxValue;
                    handler.ResponseDrainTimeout = TimeSpan.FromMilliseconds(1);

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain (or fail to)
                    handler.MaxConnectionsPerServer = 1;

                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = Timeout.InfiniteTimeSpan;

                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponseHeaders(response1, ContentLength, mode);
                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on a new connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponseHeaders(response2, ContentLength, mode);
                        Assert.Equal(ContentLength, (await response2.Content.ReadAsStringAsync()).Length);
                    }
                },
                async server =>
                {
                    string content = new string('a', ContentLength);
                    string response = GetResponseForContentMode(content, mode);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync();
                        try
                        {
                            // Write out only part of the response
                            await connection.Writer.WriteAsync(response.Substring(0, response.Length / 2));
                        }
                        catch (Exception) { }     // Eat errors from client disconnect.

                        await server.AcceptConnectionSendCustomResponseAndCloseAsync(response);
                    });
                });
        }
    }

    public sealed class SocketsHttpHandler_PostScenarioTest : PostScenarioTest
    {
        public SocketsHttpHandler_PostScenarioTest(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_ResponseStreamTest : ResponseStreamTest
    {
        public SocketsHttpHandler_ResponseStreamTest(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_SslProtocols_Test : HttpClientHandler_SslProtocols_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_SchSendAuxRecordHttpTest : SchSendAuxRecordHttpTest
    {
        public SocketsHttpHandler_SchSendAuxRecordHttpTest(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientMiniStress : HttpClientMiniStress
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandlerTest : HttpClientHandlerTest
    {
        public SocketsHttpHandler_HttpClientHandlerTest(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_DefaultCredentialsTest : DefaultCredentialsTest
    {
        public SocketsHttpHandler_DefaultCredentialsTest(ITestOutputHelper output) : base(output) { }
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_IdnaProtocolTests : IdnaProtocolTests
    {
        protected override bool UseSocketsHttpHandler => true;
        protected override bool SupportsIdna => true;
    }

    public sealed class SocketsHttpHandler_HttpRetryProtocolTests : HttpRetryProtocolTests
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpCookieProtocolTests : HttpCookieProtocolTests
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_Cancellation_Test : HttpClientHandler_Cancellation_Test
    {
        protected override bool UseSocketsHttpHandler => true;

        [Fact]
        public void ConnectTimeout_Default()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.ConnectTimeout);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(int.MaxValue + 1L)]
        public void ConnectTimeout_InvalidValues(long ms)
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.ConnectTimeout = TimeSpan.FromMilliseconds(ms));
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(int.MaxValue - 1)]
        [InlineData(int.MaxValue)]
        public void ConnectTimeout_ValidValues_Roundtrip(long ms)
        {
            using (var handler = new SocketsHttpHandler())
            {
                handler.ConnectTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.ConnectTimeout);
            }
        }

        [Fact]
        public void ConnectTimeout_SetAfterUse_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {
                handler.ConnectTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
                client.GetAsync("http://" + Guid.NewGuid().ToString("N")); // ignoring failure
                Assert.Equal(TimeSpan.FromMilliseconds(int.MaxValue), handler.ConnectTimeout);
                Assert.Throws<InvalidOperationException>(() => handler.ConnectTimeout = TimeSpan.FromMilliseconds(1));
            }
        }

        [OuterLoop]
        [Fact]
        public async Task ConnectTimeout_TimesOutSSLAuth_Throws()
        {
            var releaseServer = new TaskCompletionSource<bool>();
            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (var handler = new SocketsHttpHandler())
                using (var invoker = new HttpMessageInvoker(handler))
                {
                    handler.ConnectTimeout = TimeSpan.FromSeconds(1);

                    var sw = Stopwatch.StartNew();
                    await Assert.ThrowsAsync<OperationCanceledException>(() =>
                        invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                            new UriBuilder(uri) { Scheme = "https" }.ToString()), default));
                    sw.Stop();

                    Assert.InRange(sw.ElapsedMilliseconds, 500, 30_000);
                    releaseServer.SetResult(true);
                }
            }, server => releaseServer.Task); // doesn't establish SSL connection
        }


        [Fact]
        public void Expect100ContinueTimeout_Default()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(TimeSpan.FromSeconds(1), handler.Expect100ContinueTimeout);
            }
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(int.MaxValue + 1L)]
        public void Expect100ContinueTimeout_InvalidValues(long ms)
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => handler.Expect100ContinueTimeout = TimeSpan.FromMilliseconds(ms));
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(int.MaxValue - 1)]
        [InlineData(int.MaxValue)]
        public void Expect100ContinueTimeout_ValidValues_Roundtrip(long ms)
        {
            using (var handler = new SocketsHttpHandler())
            {
                handler.Expect100ContinueTimeout = TimeSpan.FromMilliseconds(ms);
                Assert.Equal(TimeSpan.FromMilliseconds(ms), handler.Expect100ContinueTimeout);
            }
        }

        [Fact]
        public void Expect100ContinueTimeout_SetAfterUse_Throws()
        {
            using (var handler = new SocketsHttpHandler())
            using (var client = new HttpClient(handler))
            {
                handler.Expect100ContinueTimeout = TimeSpan.FromMilliseconds(int.MaxValue);
                client.GetAsync("http://" + Guid.NewGuid().ToString("N")); // ignoring failure
                Assert.Equal(TimeSpan.FromMilliseconds(int.MaxValue), handler.Expect100ContinueTimeout);
                Assert.Throws<InvalidOperationException>(() => handler.Expect100ContinueTimeout = TimeSpan.FromMilliseconds(1));
            }
        }

        [OuterLoop("Incurs significant delay")]
        [Fact]
        public async Task Expect100Continue_WaitsExpectedPeriodOfTimeBeforeSendingContent()
        {
            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (var handler = new SocketsHttpHandler())
                using (var invoker = new HttpMessageInvoker(handler))
                {
                    TimeSpan delay = TimeSpan.FromSeconds(3);
                    handler.Expect100ContinueTimeout = delay;

                    var tcs = new TaskCompletionSource<bool>();
                    var content = new SetTcsContent(new MemoryStream(new byte[1]), tcs);
                    var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
                    request.Headers.ExpectContinue = true;

                    var sw = Stopwatch.StartNew();
                    (await invoker.SendAsync(request, default)).Dispose();
                    sw.Stop();

                    Assert.InRange(sw.Elapsed, delay - TimeSpan.FromSeconds(.5), delay * 10); // arbitrary wiggle room
                }
            }, async server =>
            {
                await server.AcceptConnectionAsync(async connection =>
                {
                    await connection.ReadRequestHeaderAsync();
                    await connection.Reader.ReadAsync(new char[1]);
                    await connection.SendResponseAsync();
                });
            });
        }

        private sealed class SetTcsContent : StreamContent
        {
            private readonly TaskCompletionSource<bool> _tcs;

            public SetTcsContent(Stream stream, TaskCompletionSource<bool> tcs) : base(stream) => _tcs = tcs;

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                _tcs.SetResult(true);
                return base.SerializeToStreamAsync(stream, context);
            }
        }
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_MaxResponseHeadersLength_Test : HttpClientHandler_MaxResponseHeadersLength_Test
    {
        protected override bool UseSocketsHttpHandler => true;
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_Authentication_Test : HttpClientHandler_Authentication_Test
    {
        protected override bool UseSocketsHttpHandler => true;

        [Theory]
        [MemberData(nameof(Authentication_SocketsHttpHandler_TestData))]
        public async void SocketsHttpHandler_Authentication_Succeeds(string authenticateHeader, bool result)
        {
            await HttpClientHandler_Authentication_Succeeds(authenticateHeader, result);
        }

        public static IEnumerable<object[]> Authentication_SocketsHttpHandler_TestData()
        {
            // These test cases pass on SocketsHttpHandler, fail everywhere else.
            // TODO: #28065: Investigate failing authentication test cases on WinHttpHandler & CurlHandler.
            yield return new object[] { "Basic realm=\"testrealm1\" basic realm=\"testrealm1\"", true };
            yield return new object[] { "Basic something digest something", true };
            yield return new object[] { "Digest ", false };
            yield return new object[] { "Digest realm=withoutquotes, nonce=withoutquotes", false };
            yield return new object[] { "Digest realm=\"testrealm\", nonce=\"testnonce\", algorithm=\"myown\"", false };
        }
    }

    public sealed class SocketsHttpHandler_ConnectionUpgrade_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;

        [Fact]
        public async Task UpgradeConnection_ReturnsReadableAndWritableStream()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    // We need to use ResponseHeadersRead here, otherwise we will hang trying to buffer the response body.
                    Task<HttpResponseMessage> getResponseTask = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        Task<List<string>> serverTask = connection.ReadRequestHeaderAndSendCustomResponseAsync($"HTTP/1.1 101 Switching Protocols\r\nDate: {DateTimeOffset.UtcNow:R}\r\n\r\n");

                        await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                        using (Stream clientStream = await (await getResponseTask).Content.ReadAsStreamAsync())
                        {
                            // Boolean properties returning correct values
                            Assert.True(clientStream.CanWrite);
                            Assert.True(clientStream.CanRead);
                            Assert.False(clientStream.CanSeek);

                            // Not supported operations
                            Assert.Throws<NotSupportedException>(() => clientStream.Length);
                            Assert.Throws<NotSupportedException>(() => clientStream.Position);
                            Assert.Throws<NotSupportedException>(() => clientStream.Position = 0);
                            Assert.Throws<NotSupportedException>(() => clientStream.Seek(0, SeekOrigin.Begin));
                            Assert.Throws<NotSupportedException>(() => clientStream.SetLength(0));

                            // Invalid arguments
                            var nonWritableStream = new MemoryStream(new byte[1], false);
                            var disposedStream = new MemoryStream();
                            disposedStream.Dispose();
                            Assert.Throws<ArgumentNullException>(() => clientStream.CopyTo(null));
                            Assert.Throws<ArgumentOutOfRangeException>(() => clientStream.CopyTo(Stream.Null, 0));
                            Assert.Throws<ArgumentNullException>(() => { clientStream.CopyToAsync(null, 100, default); });
                            Assert.Throws<ArgumentOutOfRangeException>(() => { clientStream.CopyToAsync(Stream.Null, 0, default); });
                            Assert.Throws<ArgumentOutOfRangeException>(() => { clientStream.CopyToAsync(Stream.Null, -1, default); });
                            Assert.Throws<NotSupportedException>(() => { clientStream.CopyToAsync(nonWritableStream, 100, default); });
                            Assert.Throws<ObjectDisposedException>(() => { clientStream.CopyToAsync(disposedStream, 100, default); });
                            Assert.Throws<ArgumentNullException>(() => clientStream.Read(null, 0, 100));
                            Assert.Throws<ArgumentOutOfRangeException>(() => clientStream.Read(new byte[1], -1, 1));
                            Assert.ThrowsAny<ArgumentException>(() => clientStream.Read(new byte[1], 2, 1));
                            Assert.Throws<ArgumentOutOfRangeException>(() => clientStream.Read(new byte[1], 0, -1));
                            Assert.ThrowsAny<ArgumentException>(() => clientStream.Read(new byte[1], 0, 2));
                            Assert.Throws<ArgumentNullException>(() => clientStream.BeginRead(null, 0, 100, null, null));
                            Assert.Throws<ArgumentOutOfRangeException>(() => clientStream.BeginRead(new byte[1], -1, 1, null, null));
                            Assert.ThrowsAny<ArgumentException>(() => clientStream.BeginRead(new byte[1], 2, 1, null, null));
                            Assert.Throws<ArgumentOutOfRangeException>(() => clientStream.BeginRead(new byte[1], 0, -1, null, null));
                            Assert.ThrowsAny<ArgumentException>(() => clientStream.BeginRead(new byte[1], 0, 2, null, null));
                            Assert.Throws<ArgumentNullException>(() => clientStream.EndRead(null));
                            Assert.Throws<ArgumentNullException>(() => { clientStream.ReadAsync(null, 0, 100, default); });
                            Assert.Throws<ArgumentOutOfRangeException>(() => { clientStream.ReadAsync(new byte[1], -1, 1, default); });
                            Assert.ThrowsAny<ArgumentException>(() => { clientStream.ReadAsync(new byte[1], 2, 1, default); });
                            Assert.Throws<ArgumentOutOfRangeException>(() => { clientStream.ReadAsync(new byte[1], 0, -1, default); });
                            Assert.ThrowsAny<ArgumentException>(() => { clientStream.ReadAsync(new byte[1], 0, 2, default); });

                            // Validate writing APIs on clientStream

                            clientStream.WriteByte((byte)'!');
                            clientStream.Write(new byte[] { (byte)'\r', (byte)'\n' }, 0, 2);
                            Assert.Equal("!", await connection.Reader.ReadLineAsync());

                            clientStream.Write(new Span<byte>(new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'\r', (byte)'\n' }));
                            Assert.Equal("hello", await connection.Reader.ReadLineAsync());

                            await clientStream.WriteAsync(new byte[] { (byte)'w', (byte)'o', (byte)'r', (byte)'l', (byte)'d', (byte)'\r', (byte)'\n' }, 0, 7);
                            Assert.Equal("world", await connection.Reader.ReadLineAsync());

                            await clientStream.WriteAsync(new Memory<byte>(new byte[] { (byte)'a', (byte)'n', (byte)'d', (byte)'\r', (byte)'\n' }, 0, 5));
                            Assert.Equal("and", await connection.Reader.ReadLineAsync());

                            await Task.Factory.FromAsync(clientStream.BeginWrite, clientStream.EndWrite, new byte[] { (byte)'b', (byte)'e', (byte)'y', (byte)'o', (byte)'n', (byte)'d', (byte)'\r', (byte)'\n' }, 0, 8, null);
                            Assert.Equal("beyond", await connection.Reader.ReadLineAsync());

                            clientStream.Flush();
                            await clientStream.FlushAsync();

                            // Validate reading APIs on clientStream
                            await connection.Stream.WriteAsync(Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz"));
                            var buffer = new byte[1];

                            Assert.Equal('a', clientStream.ReadByte());

                            Assert.Equal(1, clientStream.Read(buffer, 0, 1));
                            Assert.Equal((byte)'b', buffer[0]);

                            Assert.Equal(1, clientStream.Read(new Span<byte>(buffer, 0, 1)));
                            Assert.Equal((byte)'c', buffer[0]);

                            Assert.Equal(1, await clientStream.ReadAsync(buffer, 0, 1));
                            Assert.Equal((byte)'d', buffer[0]);

                            Assert.Equal(1, await clientStream.ReadAsync(new Memory<byte>(buffer, 0, 1)));
                            Assert.Equal((byte)'e', buffer[0]);

                            Assert.Equal(1, await Task.Factory.FromAsync(clientStream.BeginRead, clientStream.EndRead, buffer, 0, 1, null));
                            Assert.Equal((byte)'f', buffer[0]);

                            var ms = new MemoryStream();
                            Task copyTask = clientStream.CopyToAsync(ms);

                            string bigString = string.Concat(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 1000));
                            Task lotsOfDataSent = connection.Socket.SendAsync(Encoding.ASCII.GetBytes(bigString), SocketFlags.None);
                            connection.Socket.Shutdown(SocketShutdown.Send);
                            await copyTask;
                            await lotsOfDataSent;
                            Assert.Equal("ghijklmnopqrstuvwxyz" + bigString, Encoding.ASCII.GetString(ms.ToArray()));
                        }
                    });
                }
            });
        }
    }

    public sealed class SocketsHttpHandler_Connect_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;

        [Fact]
        public async Task ConnectMethod_Success()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("CONNECT"), url);
                    request.Headers.Host = "foo.com:345";

                    // We need to use ResponseHeadersRead here, otherwise we will hang trying to buffer the response body.
                    Task<HttpResponseMessage> responseTask = client.SendAsync(request,  HttpCompletionOption.ResponseHeadersRead);

                    await server.AcceptConnectionAsync(async connection =>
                    {
                        // Verify that Host header exist and has same value and URI authority.
                        List<string> lines = await connection.ReadRequestHeaderAsync().ConfigureAwait(false);
                        string authority = lines[0].Split()[1];
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("Host:",StringComparison.InvariantCultureIgnoreCase))
                            {
                                Assert.Equal(line, "Host: foo.com:345");
                                break;
                            }
                        }

                        Task serverTask = connection.SendResponseAsync(HttpStatusCode.OK);
                        await TestHelper.WhenAllCompletedOrAnyFailed(responseTask, serverTask).ConfigureAwait(false);

                        using (Stream clientStream = await (await responseTask).Content.ReadAsStreamAsync())
                        {
                            Assert.True(clientStream.CanWrite);
                            Assert.True(clientStream.CanRead);
                            Assert.False(clientStream.CanSeek);

                            TextReader clientReader = new StreamReader(clientStream);
                            TextWriter clientWriter = new StreamWriter(clientStream) { AutoFlush = true };
                            TextReader serverReader = connection.Reader;
                            TextWriter serverWriter = connection.Writer;

                            const string helloServer = "hello server";
                            const string helloClient = "hello client";
                            const string goodbyeServer = "goodbye server";
                            const string goodbyeClient = "goodbye client";

                            clientWriter.WriteLine(helloServer);
                            Assert.Equal(helloServer, serverReader.ReadLine());
                            serverWriter.WriteLine(helloClient);
                            Assert.Equal(helloClient, clientReader.ReadLine());
                            clientWriter.WriteLine(goodbyeServer);
                            Assert.Equal(goodbyeServer, serverReader.ReadLine());
                            serverWriter.WriteLine(goodbyeClient);
                            Assert.Equal(goodbyeClient, clientReader.ReadLine());
                        }
                    });
                }
            });
        }

        [Fact]
        public async Task ConnectMethod_Fails()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("CONNECT"), url);
                    request.Headers.Host = "foo.com:345";
                    // We need to use ResponseHeadersRead here, otherwise we will hang trying to buffer the response body.
                    Task<HttpResponseMessage> responseTask = client.SendAsync(request,  HttpCompletionOption.ResponseHeadersRead);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        Task<List<string>> serverTask = connection.ReadRequestHeaderAndSendResponseAsync(HttpStatusCode.Forbidden, content: "error");

                        await TestHelper.WhenAllCompletedOrAnyFailed(responseTask, serverTask);
                        HttpResponseMessage response = await responseTask;

                        Assert.True(response.StatusCode ==  HttpStatusCode.Forbidden);
                    });
                }
            });
        }
    }

    public sealed class SocketsHttpHandler_HttpClientHandler_ConnectionPooling_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;

        // TODO: ISSUE #27272
        // Currently the subsequent tests sometimes fail/hang with WinHttpHandler / CurlHandler.
        // In theory they should pass with any handler that does appropriate connection pooling.
        // We should understand why they sometimes fail there and ideally move them to be
        // used by all handlers this test project tests.

        [Fact]
        public async Task MultipleIterativeRequests_SameConnectionReused()
        {
            using (HttpClient client = CreateHttpClient())
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);
                var ep = (IPEndPoint)listener.LocalEndPoint;
                var uri = new Uri($"http://{ep.Address}:{ep.Port}/");

                string responseBody =
                    "HTTP/1.1 200 OK\r\n" +
                    $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
                    "Content-Length: 0\r\n" +
                    "\r\n";

                Task<string> firstRequest = client.GetStringAsync(uri);
                using (Socket server = await listener.AcceptAsync())
                using (var serverStream = new NetworkStream(server, ownsSocket: false))
                using (var serverReader = new StreamReader(serverStream))
                {
                    while (!string.IsNullOrWhiteSpace(await serverReader.ReadLineAsync()));
                    await server.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(responseBody)), SocketFlags.None);
                    await firstRequest;

                    Task<Socket> secondAccept = listener.AcceptAsync(); // shouldn't complete

                    Task<string> additionalRequest = client.GetStringAsync(uri);
                    while (!string.IsNullOrWhiteSpace(await serverReader.ReadLineAsync()));
                    await server.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(responseBody)), SocketFlags.None);
                    await additionalRequest;

                    Assert.False(secondAccept.IsCompleted, $"Second accept should never complete");
                }
            }
        }

        [OuterLoop("Incurs a delay")]
        [Fact]
        public async Task ServerDisconnectsAfterInitialRequest_SubsequentRequestUsesDifferentConnection()
        {
            using (HttpClient client = CreateHttpClient())
            {
                await LoopbackServer.CreateServerAsync(async (server, uri) =>
                {
                    // Make multiple requests iteratively.
                    for (int i = 0; i < 2; i++)
                    {
                        Task<string> request = client.GetStringAsync(uri);
                        await server.AcceptConnectionSendResponseAndCloseAsync();
                        await request;

                        if (i == 0)
                        {
                            await Task.Delay(2000); // give client time to see the closing before next connect
                        }
                    }
                });
            }
        }

        [Fact]
        public async Task ServerSendsGarbageAfterInitialRequest_SubsequentRequestUsesDifferentConnection()
        {
            using (HttpClient client = CreateHttpClient())
            {
                await LoopbackServer.CreateServerAsync(async (server, uri) =>
                {
                    // Make multiple requests iteratively.
                    for (int i = 0; i < 2; i++)
                    {
                        Task<string> request = client.GetStringAsync(uri);
                        string response = LoopbackServer.GetHttpResponse() + "here is a bunch of garbage";
                        await server.AcceptConnectionSendCustomResponseAndCloseAsync(response);
                        await request;
                    }
                });
            }
        }

        [Fact]
        public async Task ServerSendsConnectionClose_SubsequentRequestUsesDifferentConnection()
        {
            using (HttpClient client = CreateHttpClient())
            {
                await LoopbackServer.CreateServerAsync(async (server, uri) =>
                {
                    string responseBody =
                        "HTTP/1.1 200 OK\r\n" +
                        $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
                        "Content-Length: 0\r\n" +
                        "Connection: close\r\n" +
                        "\r\n";

                    // Make first request.
                    Task<string> request1 = client.GetStringAsync(uri);
                    await server.AcceptConnectionAsync(async connection1 =>
                    {
                        await connection1.ReadRequestHeaderAndSendCustomResponseAsync(responseBody);
                        await request1;

                        // Make second request and expect it to be served from a different connection.
                        Task<string> request2 = client.GetStringAsync(uri);
                        await server.AcceptConnectionAsync(async connection2 =>
                        {
                            await connection2.ReadRequestHeaderAndSendCustomResponseAsync(responseBody);
                            await request2;
                        });
                    });
                });
            }
        }

        [Theory]
        [InlineData("PooledConnectionLifetime")]
        [InlineData("PooledConnectionIdleTimeout")]
        public async Task SmallConnectionTimeout_SubsequentRequestUsesDifferentConnection(string timeoutPropertyName)
        {
            using (var handler = new SocketsHttpHandler())
            {
                switch (timeoutPropertyName)
                {
                    case "PooledConnectionLifetime": handler.PooledConnectionLifetime = TimeSpan.FromMilliseconds(1); break;
                    case "PooledConnectionIdleTimeout": handler.PooledConnectionLifetime = TimeSpan.FromMilliseconds(1); break;
                    default: throw new ArgumentOutOfRangeException(nameof(timeoutPropertyName));
                }

                using (HttpClient client = new HttpClient(handler))
                {
                    await LoopbackServer.CreateServerAsync(async (server, uri) =>
                    {
                        // Make first request.
                        Task<string> request1 = client.GetStringAsync(uri);
                        await server.AcceptConnectionAsync(async connection =>
                        {
                            await connection.ReadRequestHeaderAndSendResponseAsync();
                            await request1;

                            // Wait a small amount of time before making the second request, to give the first request time to timeout.
                            await Task.Delay(100);

                            // Make second request and expect it to be served from a different connection.
                            Task<string> request2 = client.GetStringAsync(uri);
                            await server.AcceptConnectionAsync(async connection2 =>
                            {
                                await connection2.ReadRequestHeaderAndSendResponseAsync();
                                await request2;
                            });
                        });
                    });
                }
            }
        }

        [OuterLoop]
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ConnectionsPooledThenDisposed_NoUnobservedTaskExceptions(bool secure)
        {
            RemoteInvoke(async secureString =>
            {
                var releaseServer = new TaskCompletionSource<bool>();
                await LoopbackServer.CreateClientAndServerAsync(async uri =>
                {
                    using (var handler = new SocketsHttpHandler())
                    using (var client = new HttpClient(handler))
                    {
                        handler.SslOptions.RemoteCertificateValidationCallback = delegate { return true; };
                        handler.PooledConnectionLifetime = TimeSpan.FromMilliseconds(1);

                        var exceptions = new List<Exception>();
                        TaskScheduler.UnobservedTaskException += (s, e) => exceptions.Add(e.Exception);

                        await client.GetStringAsync(uri);
                        await Task.Delay(10); // any value >= the lifetime
                        Task ignored = client.GetStringAsync(uri); // force the pool to look for the previous connection and find it's too old
                        await Task.Delay(100); // give some time for the connection close to fail pending reads

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        // Note that there are race conditions here such that we may not catch every failure,
                        // and thus could have some false negatives, but there won't be any false positives.
                        Assert.True(exceptions.Count == 0, string.Concat(exceptions));

                        releaseServer.SetResult(true);
                    }
                }, server => server.AcceptConnectionAsync(async connection =>
                {
                    await connection.ReadRequestHeaderAndSendResponseAsync(content: "hello world");
                    await releaseServer.Task;
                }),
                new LoopbackServer.Options { UseSsl = bool.Parse(secureString) });
                return SuccessExitCode;
            }, secure.ToString()).Dispose();
        }
    }

    public sealed class SocketsHttpHandler_PublicAPIBehavior_Test
    {
        private static async Task IssueRequestAsync(HttpMessageHandler handler)
        {
            using (var c = new HttpMessageInvoker(handler, disposeHandler: false))
                await Assert.ThrowsAnyAsync<Exception>(() =>
                    c.SendAsync(new HttpRequestMessage(HttpMethod.Get, new Uri("/shouldquicklyfail", UriKind.Relative)), default));
        }

        [Fact]
        public void AllowAutoRedirect_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.True(handler.AllowAutoRedirect);

                handler.AllowAutoRedirect = true;
                Assert.True(handler.AllowAutoRedirect);

                handler.AllowAutoRedirect = false;
                Assert.False(handler.AllowAutoRedirect);
            }
        }

        [Fact]
        public void AutomaticDecompression_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(DecompressionMethods.None, handler.AutomaticDecompression);

                handler.AutomaticDecompression = DecompressionMethods.GZip;
                Assert.Equal(DecompressionMethods.GZip, handler.AutomaticDecompression);

                handler.AutomaticDecompression = DecompressionMethods.Deflate;
                Assert.Equal(DecompressionMethods.Deflate, handler.AutomaticDecompression);

                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                Assert.Equal(DecompressionMethods.GZip | DecompressionMethods.Deflate, handler.AutomaticDecompression);
            }
        }

        [Fact]
        public void CookieContainer_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                CookieContainer container = handler.CookieContainer;
                Assert.Same(container, handler.CookieContainer);

                var newContainer = new CookieContainer();
                handler.CookieContainer = newContainer;
                Assert.Same(newContainer, handler.CookieContainer);
            }
        }

        [Fact]
        public void Credentials_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Null(handler.Credentials);

                var newCredentials = new NetworkCredential("username", "password");
                handler.Credentials = newCredentials;
                Assert.Same(newCredentials, handler.Credentials);
            }
        }

        [Fact]
        public void DefaultProxyCredentials_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Null(handler.DefaultProxyCredentials);

                var newCredentials = new NetworkCredential("username", "password");
                handler.DefaultProxyCredentials = newCredentials;
                Assert.Same(newCredentials, handler.DefaultProxyCredentials);
            }
        }

        [Fact]
        public void MaxAutomaticRedirections_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(50, handler.MaxAutomaticRedirections);

                handler.MaxAutomaticRedirections = int.MaxValue;
                Assert.Equal(int.MaxValue, handler.MaxAutomaticRedirections);

                handler.MaxAutomaticRedirections = 1;
                Assert.Equal(1, handler.MaxAutomaticRedirections);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxAutomaticRedirections = 0);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxAutomaticRedirections = -1);
            }
        }

        [Fact]
        public void MaxConnectionsPerServer_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(int.MaxValue, handler.MaxConnectionsPerServer);

                handler.MaxConnectionsPerServer = int.MaxValue;
                Assert.Equal(int.MaxValue, handler.MaxConnectionsPerServer);

                handler.MaxConnectionsPerServer = 1;
                Assert.Equal(1, handler.MaxConnectionsPerServer);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxConnectionsPerServer = 0);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxConnectionsPerServer = -1);
            }
        }

        [Fact]
        public void MaxResponseHeadersLength_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(64, handler.MaxResponseHeadersLength);

                handler.MaxResponseHeadersLength = int.MaxValue;
                Assert.Equal(int.MaxValue, handler.MaxResponseHeadersLength);

                handler.MaxResponseHeadersLength = 1;
                Assert.Equal(1, handler.MaxResponseHeadersLength);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxResponseHeadersLength = 0);
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxResponseHeadersLength = -1);
            }
        }

        [Fact]
        public void PreAuthenticate_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.False(handler.PreAuthenticate);

                handler.PreAuthenticate = false;
                Assert.False(handler.PreAuthenticate);

                handler.PreAuthenticate = true;
                Assert.True(handler.PreAuthenticate);
            }
        }

        [Fact]
        public void PooledConnectionIdleTimeout_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(TimeSpan.FromMinutes(2), handler.PooledConnectionIdleTimeout);

                handler.PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan;
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.PooledConnectionIdleTimeout);

                handler.PooledConnectionIdleTimeout = TimeSpan.FromSeconds(0);
                Assert.Equal(TimeSpan.FromSeconds(0), handler.PooledConnectionIdleTimeout);

                handler.PooledConnectionIdleTimeout = TimeSpan.FromSeconds(1);
                Assert.Equal(TimeSpan.FromSeconds(1), handler.PooledConnectionIdleTimeout);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.PooledConnectionIdleTimeout = TimeSpan.FromSeconds(-2));
            }
        }

        [Fact]
        public void PooledConnectionLifetime_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.PooledConnectionLifetime);

                handler.PooledConnectionLifetime = Timeout.InfiniteTimeSpan;
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.PooledConnectionLifetime);

                handler.PooledConnectionLifetime = TimeSpan.FromSeconds(0);
                Assert.Equal(TimeSpan.FromSeconds(0), handler.PooledConnectionLifetime);

                handler.PooledConnectionLifetime = TimeSpan.FromSeconds(1);
                Assert.Equal(TimeSpan.FromSeconds(1), handler.PooledConnectionLifetime);

                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.PooledConnectionLifetime = TimeSpan.FromSeconds(-2));
            }
        }

        [Fact]
        public void Properties_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                IDictionary<string, object> props = handler.Properties;
                Assert.NotNull(props);
                Assert.Empty(props);

                props.Add("hello", "world");
                Assert.Equal(1, props.Count);
                Assert.Equal("world", props["hello"]);
            }
        }

        [Fact]
        public void Proxy_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.Null(handler.Proxy);

                var proxy = new WebProxy();
                handler.Proxy = proxy;
                Assert.Same(proxy, handler.Proxy);
            }
        }

        [Fact]
        public void SslOptions_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                SslClientAuthenticationOptions options = handler.SslOptions;
                Assert.NotNull(options);

                Assert.True(options.AllowRenegotiation);
                Assert.Null(options.ApplicationProtocols);
                Assert.Equal(X509RevocationMode.NoCheck, options.CertificateRevocationCheckMode);
                Assert.Null(options.ClientCertificates);
                Assert.Equal(SslProtocols.None, options.EnabledSslProtocols);
                Assert.Equal(EncryptionPolicy.RequireEncryption, options.EncryptionPolicy);
                Assert.Null(options.LocalCertificateSelectionCallback);
                Assert.Null(options.RemoteCertificateValidationCallback);
                Assert.Null(options.TargetHost);

                Assert.Same(options, handler.SslOptions);

                var newOptions = new SslClientAuthenticationOptions();
                handler.SslOptions = newOptions;
                Assert.Same(newOptions, handler.SslOptions);
            }
        }

        [Fact]
        public void UseCookies_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.True(handler.UseCookies);

                handler.UseCookies = true;
                Assert.True(handler.UseCookies);

                handler.UseCookies = false;
                Assert.False(handler.UseCookies);
            }
        }

        [Fact]
        public void UseProxy_GetSet_Roundtrips()
        {
            using (var handler = new SocketsHttpHandler())
            {
                Assert.True(handler.UseProxy);

                handler.UseProxy = false;
                Assert.False(handler.UseProxy);

                handler.UseProxy = true;
                Assert.True(handler.UseProxy);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AfterDisposeSendAsync_GettersUsable_SettersThrow(bool dispose)
        {
            using (var handler = new SocketsHttpHandler())
            {
                Type expectedExceptionType;
                if (dispose)
                {
                    handler.Dispose();
                    expectedExceptionType = typeof(ObjectDisposedException);
                }
                else
                {
                    await IssueRequestAsync(handler);
                    expectedExceptionType = typeof(InvalidOperationException);
                }

                Assert.True(handler.AllowAutoRedirect);
                Assert.Equal(DecompressionMethods.None, handler.AutomaticDecompression);
                Assert.NotNull(handler.CookieContainer);
                Assert.Null(handler.Credentials);
                Assert.Null(handler.DefaultProxyCredentials);
                Assert.Equal(50, handler.MaxAutomaticRedirections);
                Assert.Equal(int.MaxValue, handler.MaxConnectionsPerServer);
                Assert.Equal(64, handler.MaxResponseHeadersLength);
                Assert.False(handler.PreAuthenticate);
                Assert.Equal(TimeSpan.FromMinutes(2), handler.PooledConnectionIdleTimeout);
                Assert.Equal(Timeout.InfiniteTimeSpan, handler.PooledConnectionLifetime);
                Assert.NotNull(handler.Properties);
                Assert.Null(handler.Proxy);
                Assert.NotNull(handler.SslOptions);
                Assert.True(handler.UseCookies);
                Assert.True(handler.UseProxy);

                Assert.Throws(expectedExceptionType, () => handler.AllowAutoRedirect = false);
                Assert.Throws(expectedExceptionType, () => handler.AutomaticDecompression = DecompressionMethods.GZip);
                Assert.Throws(expectedExceptionType, () => handler.CookieContainer = new CookieContainer());
                Assert.Throws(expectedExceptionType, () => handler.Credentials = new NetworkCredential("anotheruser", "anotherpassword"));
                Assert.Throws(expectedExceptionType, () => handler.DefaultProxyCredentials = new NetworkCredential("anotheruser", "anotherpassword"));
                Assert.Throws(expectedExceptionType, () => handler.MaxAutomaticRedirections = 2);
                Assert.Throws(expectedExceptionType, () => handler.MaxConnectionsPerServer = 2);
                Assert.Throws(expectedExceptionType, () => handler.MaxResponseHeadersLength = 2);
                Assert.Throws(expectedExceptionType, () => handler.PreAuthenticate = false);
                Assert.Throws(expectedExceptionType, () => handler.PooledConnectionIdleTimeout = TimeSpan.FromSeconds(2));
                Assert.Throws(expectedExceptionType, () => handler.PooledConnectionLifetime = TimeSpan.FromSeconds(2));
                Assert.Throws(expectedExceptionType, () => handler.Proxy = new WebProxy());
                Assert.Throws(expectedExceptionType, () => handler.SslOptions = new SslClientAuthenticationOptions());
                Assert.Throws(expectedExceptionType, () => handler.UseCookies = false);
                Assert.Throws(expectedExceptionType, () => handler.UseProxy = false);
            }
        }
    }

    public sealed class SocketsHttpHandler_ExternalConfiguration_Test : HttpClientTestBase
    {
        private const string EnvironmentVariableSettingName = "DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER";
        private const string AppContextSettingName = "System.Net.Http.UseSocketsHttpHandler";

        private static bool UseSocketsHttpHandlerEnvironmentVariableIsNotSet =>
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariableSettingName));

        [ConditionalTheory(nameof(UseSocketsHttpHandlerEnvironmentVariableIsNotSet))]
        [InlineData("true", true)]
        [InlineData("TRUE", true)]
        [InlineData("tRuE", true)]
        [InlineData("1", true)]
        [InlineData("0", false)]
        [InlineData("false", false)]
        [InlineData("helloworld", true)]
        [InlineData("", true)]
        public void HttpClientHandler_SettingEnvironmentVariableChangesDefault(string envVarValue, bool expectedUseSocketsHandler)
        {
            RemoteInvoke((innerEnvVarValue, innerExpectedUseSocketsHandler) =>
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableSettingName, innerEnvVarValue);
                using (var handler = new HttpClientHandler())
                {
                    Assert.Equal(bool.Parse(innerExpectedUseSocketsHandler), IsSocketsHttpHandler(handler));
                }
                return SuccessExitCode;
            }, envVarValue, expectedUseSocketsHandler.ToString()).Dispose();
        }

        [Fact]
        public void HttpClientHandler_SettingAppContextChangesDefault()
        {
            RemoteInvoke(() =>
            {
                AppContext.SetSwitch(AppContextSettingName, isEnabled: true);
                using (var handler = new HttpClientHandler())
                {
                    Assert.True(IsSocketsHttpHandler(handler));
                }

                AppContext.SetSwitch(AppContextSettingName, isEnabled: false);
                using (var handler = new HttpClientHandler())
                {
                    Assert.False(IsSocketsHttpHandler(handler));
                }

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void HttpClientHandler_AppContextOverridesEnvironmentVariable()
        {
            RemoteInvoke(() =>
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableSettingName, "true");
                using (var handler = new HttpClientHandler())
                {
                    Assert.True(IsSocketsHttpHandler(handler));
                }

                AppContext.SetSwitch(AppContextSettingName, isEnabled: false);
                using (var handler = new HttpClientHandler())
                {
                    Assert.False(IsSocketsHttpHandler(handler));
                }

                AppContext.SetSwitch(AppContextSettingName, isEnabled: true);
                Environment.SetEnvironmentVariable(EnvironmentVariableSettingName, null);
                using (var handler = new HttpClientHandler())
                {
                    Assert.True(IsSocketsHttpHandler(handler));
                }

                return SuccessExitCode;
            }).Dispose();
        }
    }
}
