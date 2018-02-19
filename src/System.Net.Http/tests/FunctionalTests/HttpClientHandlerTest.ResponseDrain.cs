// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Test.Common;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    // TODO: Make tests Outerloop

    public class HttpClientHandler_ResponseDrain_Test : HttpClientTestBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetAsync_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection(bool useTE)
        {
            if (UseSocketsHttpHandler)
            {
                // Fails currently
                return;
            }

            const string simpleContent = "Hello world!";

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    using (var client = CreateHttpClient())
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponse(response1, simpleContent.Length, useTE);

                        // Read up to exactly 1 byte before the end of the response
                        Stream responseStream = await response1.Content.ReadAsStreamAsync();
                        byte[] bytes = await ReadToByteCount(responseStream, simpleContent.Length - 1);
                        Assert.Equal(simpleContent.Substring(0, simpleContent.Length - 1), Encoding.ASCII.GetString(bytes));

                        // Introduce a short delay to try to ensure that when we dispose the response,
                        // all response data is available and we can drain synchronously and reuse the connection.
                        await Task.Delay(100);

                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on the same connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponse(response2, simpleContent.Length, useTE);
                        Assert.Equal(simpleContent, await response2.Content.ReadAsStringAsync());
                    }
                },
                async server =>
                {
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        if (useTE)
                        {
                            var response = LoopbackServer.GetSingleChunkHttpResponse(content: simpleContent);
                            await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                            await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                        }
                        else
                        {
                            await connection.ReadRequestHeaderAndSendResponseAsync(content: simpleContent);
                            await connection.ReadRequestHeaderAndSendResponseAsync(content: simpleContent);
                        }
                    });
                });
        }

        // The actual amount of drain that's supported is handler and timing dependent, apparently.
        // These cases are an attempt to provide a "min bar" for draining behavior.

        [Theory]
        [InlineData(10, 0, false)]
        [InlineData(10, 0, true)]
        [InlineData(10, 1, false)]
        [InlineData(10, 1, true)]
        [InlineData(100, 10, false)]
        [InlineData(100, 10, true)]
        [InlineData(1000, 950, false)]
        [InlineData(1000, 950, true)]
        [InlineData(10000, 9500, false)]
        [InlineData(10000, 9500, true)]
        public async Task GetAsyncWithMaxConnections_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection(int totalSize, int readSize, bool useTE)
        {
            if (IsWinHttpHandler && useTE && readSize == 0)
            {
                // WinHttpHandler doesn't try to drain when using TE and no body read.
                return;
            }

            if (UseSocketsHttpHandler)
            {
                // Fails currently
                return;
            }

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain (or fail to)
                    handler.MaxConnectionsPerServer = 1;

                    using (var client = new HttpClient(handler))
                    { 
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponse(response1, totalSize, useTE);

                        // Read part but not all of response
                        Stream responseStream = await response1.Content.ReadAsStreamAsync();
                        await ReadToByteCount(responseStream, readSize);

                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on the same connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponse(response2, totalSize, useTE);
                        Assert.Equal(totalSize, (await response2.Content.ReadAsStringAsync()).Length);
                    }
                },
                async server =>
                {
                    string content = new string('a', totalSize);
                    string response = (useTE ? LoopbackServer.GetSingleChunkHttpResponse(content: content) : LoopbackServer.GetHttpResponse(content: content));
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                        await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                    });
                });
        }

        // See comment above for values here.

        [Theory]
        [InlineData(20000, 0, false)]
        [InlineData(20000, 0, true)]
        [InlineData(40000, 10000, false)]
        [InlineData(40000, 10000, true)]
        [InlineData(100000, 50000, false)]
        [InlineData(100000, 50000, true)]
        public async Task GetAsyncWithMaxConnections_DisposeBeforeReadingToEnd_KillsConnection(int totalSize, int readSize, bool useTE)
        {
            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain (or fail to)
                    handler.MaxConnectionsPerServer = 1;

                    using (var client = new HttpClient(handler))
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponse(response1, totalSize, useTE);

                        // Read part but not all of response
                        Stream responseStream = await response1.Content.ReadAsStreamAsync();
                        await ReadToByteCount(responseStream, readSize);

                        response1.Dispose();

                        // Issue another request.  We'll confirm that it comes on a new connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        ValidateResponse(response2, totalSize, useTE);
                        Assert.Equal(totalSize, (await response2.Content.ReadAsStringAsync()).Length);
                    }
                },
                async server =>
                {
                    string content = new string('a', totalSize);
                    string response = (useTE ? LoopbackServer.GetSingleChunkHttpResponse(content: content) : LoopbackServer.GetHttpResponse(content: content));
                    Task t1 = server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync();
                        try
                        {
                            await connection.Writer.WriteAsync(response);
                        }
                        catch (Exception) { }     // Eat errors from client disconnect.
                    });

                    Task t2 = server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync();
                        try
                        {
                            await connection.Writer.WriteAsync(response);
                        }
                        catch (Exception) { }     // Eat errors from client disconnect.
                    });

                    await TaskTimeoutExtensions.WhenAllOrAnyFailed(new Task[] { t1, t2 });
                });
        }

#if false
        [Fact]
        public async Task StupidTest()
        {
            const string simpleContent = "Hello world!";

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    using (var client = CreateHttpClient())
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        ValidateResponse(response1, simpleContent.Length, true);
                        Assert.Equal(simpleContent, await response1.Content.ReadAsStringAsync());
                    }
                },
                async server =>
                {
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        var response = LoopbackServer.GetSingleChunkHttpResponse(content: simpleContent);
                        Console.WriteLine($"sending response:\r\n{response}");
                        await connection.ReadRequestHeaderAndSendCustomResponseAsync(response);
                    });
                });
        }
#endif

        private static void ValidateResponse(HttpResponseMessage response, int contentLength, bool useTE)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            if (useTE)
            {
                Assert.True(response.Headers.TransferEncodingChunked);
            }
            else
            {
                Assert.Equal(contentLength, response.Content.Headers.ContentLength);
            }
        }

        private static async Task<byte[]> ReadToByteCount(Stream stream, int byteCount)
        {
            byte[] buffer = new byte[byteCount];
            int totalBytesRead = 0;

            while (totalBytesRead < byteCount)
            {
                int bytesRead = await stream.ReadAsync(buffer, totalBytesRead, (byteCount - totalBytesRead));
                if (bytesRead == 0)
                {
                    throw new Exception("Unexpected EOF");
                }

                totalBytesRead += bytesRead;
                if (totalBytesRead > byteCount)
                {
                    throw new Exception("Read more bytes than requested???");
                }
            }

            return buffer;
        }
    }
}
