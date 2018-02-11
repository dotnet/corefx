// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class HttpClientHandler_Cancellation_Test : HttpClientTestBase
    {
        [Theory]
        [MemberData(nameof(TwoBoolsAndCancellationMode))]
        public async Task PostAsync_CancelDuringRequestContentSend_TaskCanceledQuickly(bool chunkedTransfer, bool connectionClose, CancellationMode mode)
        {
            if (IsWinHttpHandler || IsNetfxHandler)
            {
                // Issue #27063: hangs / doesn't cancel
                return;
            }

            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                var cts = new CancellationTokenSource();

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    Task serverTask = LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        // Since we won't receive all of the request, just read everything we do get
                        byte[] ignored = new byte[100];
                        while (await stream.ReadAsync(ignored, 0, ignored.Length) > 0);
                        return null;
                    });

                    var preContentSent = new TaskCompletionSource<bool>();
                    var sendPostContent = new TaskCompletionSource<bool>();

                    await ValidateClientCancellationAsync(async () =>
                    {
                        var req = new HttpRequestMessage(HttpMethod.Post, url);
                        req.Content = new DelayedByteContent(2000, 3000, preContentSent, sendPostContent.Task);
                        req.Headers.TransferEncodingChunked = chunkedTransfer;
                        req.Headers.ConnectionClose = connectionClose;

                        Task<HttpResponseMessage> postResponse = client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                        await preContentSent.Task;
                        Cancel(mode, client, cts);
                        await postResponse;
                    });

                    try
                    {
                        sendPostContent.SetResult(true);
                        await serverTask;
                    } catch { }
                });
            }
        }
        
        [Theory]
        [MemberData(nameof(TwoBoolsAndCancellationMode))]
        public async Task GetAsync_CancelDuringResponseHeadersReceived_TaskCanceledQuickly(bool chunkedTransfer, bool connectionClose, CancellationMode mode)
        {
            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                var cts = new CancellationTokenSource();

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    var partialResponseHeadersSent = new TaskCompletionSource<bool>();
                    var clientFinished = new TaskCompletionSource<bool>();

                    Task serverTask = LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        while (!string.IsNullOrEmpty(await reader.ReadLineAsync()));

                        await writer.WriteAsync($"HTTP/1.1 200 OK\r\nDate: {DateTimeOffset.UtcNow:R}\r\n"); // missing final \r\n so headers don't complete

                        partialResponseHeadersSent.TrySetResult(true);
                        await clientFinished.Task;

                        return null;
                    });

                    await ValidateClientCancellationAsync(async () =>
                    {
                        var req = new HttpRequestMessage(HttpMethod.Get, url);
                        req.Headers.ConnectionClose = connectionClose;

                        Task<HttpResponseMessage> getResponse = client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                        await partialResponseHeadersSent.Task;
                        Cancel(mode, client, cts);
                        await getResponse;
                    });

                    try
                    {
                        clientFinished.SetResult(true);
                        await serverTask;
                    } catch { }
                });
            }
        }

        [Theory]
        [MemberData(nameof(TwoBoolsAndCancellationMode))]
        public async Task GetAsync_CancelDuringResponseBodyReceived_Buffered_TaskCanceledQuickly(bool chunkedTransfer, bool connectionClose, CancellationMode mode)
        {
            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                var cts = new CancellationTokenSource();

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    var responseHeadersSent = new TaskCompletionSource<bool>();
                    var clientFinished = new TaskCompletionSource<bool>();

                    Task serverTask = LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        while (!string.IsNullOrEmpty(await reader.ReadLineAsync()));

                        await writer.WriteAsync(
                            $"HTTP/1.1 200 OK\r\n" +
                            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
                            (!chunkedTransfer ? "Content-Length: 20\r\n" : "") +
                            (connectionClose ? "Connection: close\r\n" : "") +
                            $"\r\n123"); // "123" is part of body and could either be chunked size or part of content-length bytes, both incomplete

                        responseHeadersSent.TrySetResult(true);
                        await clientFinished.Task;

                        return null;
                    });

                    await ValidateClientCancellationAsync(async () =>
                    {
                        var req = new HttpRequestMessage(HttpMethod.Get, url);
                        req.Headers.ConnectionClose = connectionClose;

                        Task<HttpResponseMessage> getResponse = client.SendAsync(req, HttpCompletionOption.ResponseContentRead, cts.Token);
                        await responseHeadersSent.Task;
                        await Task.Delay(1); // make it more likely that client will have started processing response body
                        Cancel(mode, client, cts);
                        await getResponse;
                    });

                    try
                    {
                        clientFinished.SetResult(true);
                        await serverTask;
                    } catch { }
                });
            }
        }

        [Theory]
        [MemberData(nameof(ThreeBools))]
        public async Task GetAsync_CancelDuringResponseBodyReceived_Unbuffered_TaskCanceledQuickly(bool chunkedTransfer, bool connectionClose, bool readOrCopyToAsync)
        {
            if (IsNetfxHandler || IsCurlHandler)
            {
                // doesn't cancel
                return;
            }

            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                var cts = new CancellationTokenSource();

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    var clientFinished = new TaskCompletionSource<bool>();

                    Task serverTask = LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        while (!string.IsNullOrEmpty(await reader.ReadLineAsync()));

                        await writer.WriteAsync(
                            $"HTTP/1.1 200 OK\r\n" +
                            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
                            (!chunkedTransfer ? "Content-Length: 20\r\n" : "") +
                            (connectionClose ? "Connection: close\r\n" : "") +
                            $"\r\n");

                        await clientFinished.Task;

                        return null;
                    });

                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    req.Headers.ConnectionClose = connectionClose;
                    Task<HttpResponseMessage> getResponse = client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    await ValidateClientCancellationAsync(async () =>
                    {
                        HttpResponseMessage resp = await getResponse;
                        Stream respStream = await resp.Content.ReadAsStreamAsync();
                        Task readTask = readOrCopyToAsync ?
                            respStream.ReadAsync(new byte[1], 0, 1, cts.Token) :
                            respStream.CopyToAsync(Stream.Null, 10, cts.Token);
                        cts.Cancel();
                        await readTask;
                    });

                    try
                    {
                        clientFinished.SetResult(true);
                        await serverTask;
                    } catch { }
                });
            }
        }

        [Fact]
        public async Task GetAsync_CancelPendingRequests_DoesntCancelReadAsyncOnResponseStream()
        {
            if (IsNetfxHandler)
            {
                // throws ObjectDisposedException as part of Stream.CopyToAsync
                return;
            }
            if (IsCurlHandler)
            {
                // Issue #27065
                // throws OperationCanceledException from Stream.CopyToAsync
                return;
            }

            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    var responseHeadersSent = new TaskCompletionSource<bool>();
                    var clientFinished = new TaskCompletionSource<bool>();

                    var responseContentSegment = new string('s', 3000);
                    int responseSegments = 4;
                    int contentLength = responseContentSegment.Length * responseSegments;

                    Task serverTask = LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        while (!string.IsNullOrEmpty(await reader.ReadLineAsync()));

                        await writer.WriteAsync(
                            $"HTTP/1.1 200 OK\r\n" +
                            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
                            $"Content-Length: {contentLength}\r\n" +
                            $"\r\n");
                        responseHeadersSent.TrySetResult(true);

                        for (int i = 0; i < responseSegments; i++)
                        {
                            if (i > 0) await Task.Delay(1);
                            await writer.WriteAsync(responseContentSegment);
                        }

                        await clientFinished.Task;

                        return null;
                    });


                    using (HttpResponseMessage resp = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    using (Stream respStream = await resp.Content.ReadAsStreamAsync())
                    {
                        await responseHeadersSent.Task;
                        client.CancelPendingRequests(); // should not cancel the operation, as using ResponseHeadersRead
                        var result = new MemoryStream();
                        await respStream.CopyToAsync(result, 10, new CancellationTokenSource().Token);
                        Assert.Equal(contentLength, result.Length);
                    }

                    clientFinished.SetResult(true);
                    await serverTask;
                });
            }
        }

        [Fact]
        public async Task MaxConnectionsPerServer_WaitingConnectionsAreCancelable()
        {
            if (IsWinHttpHandler)
            {
                // Issue #27064:
                // Throws WinHttpException ("The server returned an invalid or unrecognized response")
                // while parsing headers.
                return;
            }
            if (IsNetfxHandler)
            {
                // Throws HttpRequestException wrapping a WebException for the canceled request
                // instead of throwing an OperationCanceledException or a canceled WebException directly.
                return;
            }

            using (HttpClientHandler handler = CreateHttpClientHandler())
            using (HttpClient client = new HttpClient(handler))
            {
                handler.MaxConnectionsPerServer = 1;
                client.Timeout = Timeout.InfiniteTimeSpan;

                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    var serverAboutToBlock = new TaskCompletionSource<bool>();
                    var blockServerResponse = new TaskCompletionSource<bool>();

                    Task serverTask1 = LoopbackServer.AcceptSocketAsync(server, async (socket1, stream1, reader1, writer1) =>
                    {
                        while (!string.IsNullOrEmpty(await reader1.ReadLineAsync()));
                        await writer1.WriteAsync($"HTTP/1.1 200 OK\r\nDate: {DateTimeOffset.UtcNow:R}\r\n");
                        serverAboutToBlock.SetResult(true);
                        await blockServerResponse.Task;
                        await writer1.WriteAsync("Content-Length: 5\r\n\r\nhello");
                        return null;
                    });

                    Task get1 = client.GetAsync(url);
                    await serverAboutToBlock.Task;

                    var cts = new CancellationTokenSource();
                    Task get2 = ValidateClientCancellationAsync(() => client.GetAsync(url, cts.Token));
                    Task get3 = ValidateClientCancellationAsync(() => client.GetAsync(url, cts.Token));

                    Task get4 = client.GetAsync(url);

                    cts.Cancel();
                    await get2;
                    await get3;

                    blockServerResponse.SetResult(true);
                    await new[] { get1, serverTask1 }.WhenAllOrAnyFailed();

                    Task serverTask4 = LoopbackServer.AcceptSocketAsync(server, async (socket2, stream2, reader2, writer2) =>
                    {
                        while (!string.IsNullOrEmpty(await reader2.ReadLineAsync()));
                        await writer2.WriteAsync($"HTTP/1.1 200 OK\r\nDate: {DateTimeOffset.UtcNow:R}\r\nContent-Length: 0\r\n\r\n");
                        return null;
                    });

                    await new[] { get4, serverTask4 }.WhenAllOrAnyFailed();
                });
            }
        }

        private async Task ValidateClientCancellationAsync(Func<Task> clientBodyAsync)
        {
            var stopwatch = Stopwatch.StartNew();
            Exception error = await Record.ExceptionAsync(clientBodyAsync);
            stopwatch.Stop();

            Assert.NotNull(error);

            if (IsNetfxHandler)
            {
                Assert.True(
                    error is WebException we && we.Status == WebExceptionStatus.RequestCanceled ||
                    error is OperationCanceledException,
                    "Expected cancellation exception, got:" + Environment.NewLine + error);
            }
            else
            {
                Assert.True(
                    error is OperationCanceledException,
                    "Expected cancellation exception, got:" + Environment.NewLine + error);
            }

            Assert.True(stopwatch.Elapsed < new TimeSpan(0, 0, 30), $"Elapsed time {stopwatch.Elapsed} should be less than 30 seconds, was {stopwatch.Elapsed.TotalSeconds}");
        }

        private static void Cancel(CancellationMode mode, HttpClient client, CancellationTokenSource cts)
        {
            if ((mode & CancellationMode.Token) != 0)
            {
                cts.Cancel();
            }

            if ((mode & CancellationMode.CancelPendingRequests) != 0)
            {
                client.CancelPendingRequests();
            }

            if ((mode & CancellationMode.DisposeHttpClient) != 0)
            {
                client.Dispose();
            }
        }

        [Flags]
        public enum CancellationMode
        {
            Token = 0x1,
            CancelPendingRequests = 0x2,
            DisposeHttpClient = 0x4
        }

        private static readonly bool[] s_bools = new[] { true, false };

        public static IEnumerable<object[]> TwoBoolsAndCancellationMode() =>
            from first in s_bools
            from second in s_bools
            from mode in new[] { CancellationMode.Token, CancellationMode.CancelPendingRequests, CancellationMode.DisposeHttpClient, CancellationMode.Token | CancellationMode.CancelPendingRequests }
            select new object[] { first, second, mode };

        public static IEnumerable<object[]> ThreeBools() =>
            from first in s_bools
            from second in s_bools
            from third in s_bools
            select new object[] { first, second, third };

        private sealed class DelayedByteContent : HttpContent
        {
            private readonly TaskCompletionSource<bool> _preContentSent;
            private readonly Task _waitToSendPostContent;

            public DelayedByteContent(int preTriggerLength, int postTriggerLength, TaskCompletionSource<bool> preContentSent, Task waitToSendPostContent)
            {
                PreTriggerLength = preTriggerLength;
                _preContentSent = preContentSent;
                _waitToSendPostContent = waitToSendPostContent;
                Content = new byte[preTriggerLength + postTriggerLength];
                new Random().NextBytes(Content);
            }

            public byte[] Content { get; }
            public int PreTriggerLength { get; }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                await stream.WriteAsync(Content, 0, PreTriggerLength);
                _preContentSent.TrySetResult(true);
                await _waitToSendPostContent;
                await stream.WriteAsync(Content, PreTriggerLength, Content.Length - PreTriggerLength);
            }

            protected override bool TryComputeLength(out long length)
            {
                length = Content.Length;
                return true;
            }
        }
    }
}
