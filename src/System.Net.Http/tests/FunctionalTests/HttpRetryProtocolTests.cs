// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System.Net.Sockets;

namespace System.Net.Http.Functional.Tests
{
    public class HttpRetryProtocolTests : HttpClientTestBase
    {
        private static readonly string s_simpleContent = "Hello World\r\n";
        private static readonly string s_simpleResponse =
            $"HTTP/1.1 200 OK\r\n" +
            $"Date: {DateTimeOffset.UtcNow:R}\r\n" +
            $"Content-Length: {s_simpleContent.Length}\r\n" +
            "\r\n" +
            s_simpleContent;

        public static Task CreateServerAndClientAsync(Func<Uri, Task> clientFunc, Func<Socket, Task> serverFunc)
        {
            IPEndPoint ignored;
            return LoopbackServer.CreateServerAsync(async (server, uri) =>
            {
                Task clientTask = clientFunc(uri);
                Task serverTask = serverFunc(server);

                await TestHelper.WhenAllCompletedOrAnyFailed(clientTask, serverTask);

            }, out ignored);
        }

        [Fact]
        public async Task GetAsync_RetryOnConnectionClosed_Success()
        {
            Console.WriteLine("Running");

            await CreateServerAndClientAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    // Send initial request and receive response so connection is established
                    HttpResponseMessage response1 = await client.GetAsync(url);
                    Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                    Assert.Equal(s_simpleContent, await response1.Content.ReadAsStringAsync());

                    // Send second request.  Should reuse same connection.  
                    // The server will close the connection, but HttpClient should retry the request.
                    HttpResponseMessage response2 = await client.GetAsync(url);
                    Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                    Assert.Equal(s_simpleContent, await response1.Content.ReadAsStringAsync());
                }
            },
            async server =>
            {
                await LoopbackServer.AcceptSocketAsync(server, async (s, stream, reader, writer) =>
                {
                    Console.WriteLine("A");

                    // Initial response
                    await LoopbackServer.ReadWriteAcceptedAsync(s, reader, writer, s_simpleResponse);

                    Console.WriteLine("B");

                    // Second response: Read request headers, then close connection
                    await LoopbackServer.ReadWriteAcceptedAsync(s, reader, writer, "");
                    s.Close();

                    Console.WriteLine("D");

                    // Client should reconnect.  Accept that connection and send response.
                    await LoopbackServer.ReadRequestAndSendResponseAsync(server, s_simpleResponse);

                    Console.WriteLine("E");

                    return null;
                });
            });
        }
    }
}
