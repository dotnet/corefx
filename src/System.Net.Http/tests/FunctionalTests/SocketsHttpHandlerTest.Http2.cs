// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    // Note:  Disposing the HttpClient object automatically disposes the handler within. So, it is not necessary
    // to separately Dispose (or have a 'using' statement) for the handler.
    public abstract class SocketsHttpHandler_HttpClientHandler_Http2_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;

        [Fact]
        public async Task Http2_ClientConnectPreface_Sent()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                string connectionPreface = await server.AcceptConnectionAsync();

                Assert.Equal(connectionPreface, "PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n");
            }
        }

        [Fact]
        [ActiveIssue(31394)]
        public async Task DataFrame_NoStream_Throws()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();
                await server.WriteFrameAsync(new Frame(0, FrameType.Settings, FrameFlags.Ack, 0));

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\n"+receivedFrame);

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\n"+receivedFrame);

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.None, 0, 0);
                Console.WriteLine("\n"+invalidFrame);
                await server.WriteFrameAsync(invalidFrame);

                // Receive a RST_STREAM frame.
                receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\n"+receivedFrame);
                Assert.Equal(FrameType.RstStream, receivedFrame.Type);
            }
        }

        [Fact]
        public async Task DataFrame_PaddingOnly_ResetsStream()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();
                await server.WriteFrameAsync(new Frame(0, FrameType.Settings, FrameFlags.Ack, 0));

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\nServer Recieved:\n"+receivedFrame);

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\nServer Recieved:\n"+receivedFrame);

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[0], FrameFlags.Padded, 10, 1);
                Console.WriteLine("\nServer Sending:\n"+invalidFrame);
                await server.WriteFrameAsync(invalidFrame);

                // Receive a RST_STREAM frame.
                receivedFrame = await server.ReadFrameAsync();
                Console.WriteLine("\nServer Recieved:\n"+receivedFrame);
                Assert.Equal(FrameType.RstStream, receivedFrame.Type);

                await Assert.ThrowsAsync<Exception>(async () => await sendTask);
            }
        }
    }
}