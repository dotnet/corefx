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

    public sealed class SocketsHttpHandler_HttpClientHandler_Http2_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;

        [Fact]
        public async Task Http2_ClientPreface_Sent()
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
        [ActiveIssue(31315)]
        public async Task Http2_DataSentBeforeServerPreface_ProtocolError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();

                // Send a frame despite not having sent the server connection preface.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.Padded, 10, 1);
                await server.WriteFrameAsync(invalidFrame);

                // This currently throws an Http2ProtocolException, but that type is not public.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [Fact]
        [ActiveIssue(31315)]
        public async Task Http2_StreamResetByServer_RequestFails()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Send a reset stream frame so that stream 1 moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.Padded, 0x1, 1);
                await server.WriteFrameAsync(resetStream);

                // This currently throws an IOException.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [Fact]
        [ActiveIssue(31394)]
        public async Task DataFrame_NoStream_ConnectionError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.None, 0, 0);
                await server.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);

                // The server should receive a GOAWAY frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.GoAway, receivedFrame.Type);
            }
        }

        [Fact]
        [ActiveIssue(31520)]
        public async Task DataFrame_TooLong_ConnectionError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[Frame.MaxFrameLength + 1], FrameFlags.None, 0, 0);
                await server.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);

                // The server should receive a GOAWAY frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.GoAway, receivedFrame.Type);
            }
        }

        [Fact]
        [ActiveIssue(31315)]
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

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[0], FrameFlags.Padded, 10, 1);
                await server.WriteFrameAsync(invalidFrame);

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);

                // Receive a RST_STREAM frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.RstStream, receivedFrame.Type);
            }
        }

        [Fact]
        [ActiveIssue(31514)]
        public async Task ClosedStream_FrameReceived_ResetsStream()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = new Http2LoopbackServer(new Http2Options()))
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.CreateServer());

                await server.AcceptConnectionAsync();
                await server.SendConnectionPrefaceAsync();

                // Receive the initial settings frame from the server.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Receive the request header frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                // Send a reset stream frame so that stream 1 moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.Padded, 0x1, 1);
                await server.WriteFrameAsync(resetStream);

                // Send a frame on the now-closed stream.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.Padded, 10, 1);
                await server.WriteFrameAsync(invalidFrame);

                // Receive a RST_STREAM frame.
                receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.RstStream, receivedFrame.Type);

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        public SocketsHttpHandler_HttpClientHandler_Http2_Test()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
        }

        public new void Dispose()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            base.Dispose();
        }
    }
}
