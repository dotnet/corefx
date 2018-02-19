// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Net.Test.Common;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    // TODO: Make tests Outerloop

    public class HttpClientHandler_ResponseDrain_Test : HttpClientTestBase
    {
        // TODO: This is failing on SHH currently

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAsync_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection(bool useTE)
        {
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

                        // Issue another requests.  We'll confirm that it comes on the same connection.
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

#if false
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAsyncWithMaxConnections_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection(int totalSize, int readSize, bool useTE)
        {
            const string simpleContent = "Hello world!";

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();

                    // Set MaxConnectionsPerServer to 1.  This will ensure we will wait for the previous request to drain.
                    handler.MaxConnectionsPerServer = 1;
                    using (var client = new HttpClient(handler))
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                        if (useTE)
                        {
                            request.Headers.TransferEncodingChunked = true;
                        }

                        HttpResponseMessage response1 = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                        if (!useTE)
                        {
                            Assert.Equal(simpleContent.Length, response1.Content.Headers.ContentLength);
                        }

                        // Read part but not all of response
                        Stream responseStream = await response1.Content.ReadAsStreamAsync();
                        await ReadToByteCount(responseStream, readSize);

                        response1.Dispose();

                        // Issue another requests.  We'll confirm that it comes on the same connection.
                        HttpResponseMessage response2 = await client.GetAsync(url);
                        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
                        if (!useTE)
                        {
                            Assert.Equal(simpleContent.Length, response1.Content.Headers.ContentLength);
                        }
                        Assert.Equal(simpleContent, await response2.Content.ReadAsStringAsync());
                    }
                },
                async server =>
                {
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAndSendResponseAsync(content: simpleContent);
                        await connection.ReadRequestHeaderAndSendResponseAsync(content: simpleContent);
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
