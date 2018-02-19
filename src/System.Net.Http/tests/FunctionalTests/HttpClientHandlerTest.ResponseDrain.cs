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
        // TODO: Add TE test
        // TODO: This is failing on SHH currently

        [Fact]
        public async Task GetAsync_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection()
        {
            const string simpleContent = "Hello world!";
            Console.WriteLine("It's running!");

            await LoopbackServer.CreateClientAndServerAsync(
                async url =>
                {
                    using (var client = CreateHttpClient())
                    {
                        HttpResponseMessage response1 = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                        Assert.Equal(simpleContent.Length, response1.Content.Headers.ContentLength);

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
                        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
                        Assert.Equal(simpleContent.Length, response2.Content.Headers.ContentLength);
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

        // TODO: TE test?
        // TODO: tests with MaxConnections to ensure reuse
#if false
        [Theory]
        [InlineData(1, 5, false)]
        [InlineData(1, 5, true)]
        [InlineData(2, 2, false)]
        [InlineData(2, 2, true)]
        [InlineData(3, 2, false)]
        [InlineData(3, 2, true)]
        [InlineData(3, 5, false)]
        public async Task GetAsync_DisposeBeforeReadingToEnd_DrainsRequestsAndReusesConnection(int maxConnections, int numRequests, bool secure)
        {
            using (HttpClientHandler handler = CreateHttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                // Set MaxConnectionsPerServer to 1.  Otherwise, there may be a delay caused by doing the response drain,
                // which could cause another connection to be established.

                handler.MaxConnectionsPerServer = maxConnections;
                await Task.WhenAll(
                    from i in Enumerable.Range(0, numRequests)
                    select client.GetAsync(secure ? Configuration.Http.RemoteEchoServer : Configuration.Http.SecureRemoteEchoServer));
            }
        }
#endif
    }
}
