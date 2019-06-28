// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Linq;
using System.Net.Test.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    public abstract class HttpClientHandlerTest_Http2 : HttpClientHandlerTestBase
    {
        protected override bool UseSocketsHttpHandler => true;
        protected override bool UseHttp2 => true;

        public static bool SupportsAlpn => PlatformDetection.SupportsAlpn;

        public HttpClientHandlerTest_Http2(ITestOutputHelper output) : base(output) { }

        private async Task AssertProtocolErrorAsync(Task task, ProtocolErrors errorCode)
        {
            Exception e = await Assert.ThrowsAsync<HttpRequestException>(() => task);
            if (UseSocketsHttpHandler)
            {
                string text = e.ToString();
                Assert.Contains(((int)errorCode).ToString("x"), text);
                Assert.Contains(
                    Enum.IsDefined(typeof(ProtocolErrors), errorCode) ? errorCode.ToString() : ProtocolErrors.PROTOCOL_ERROR.ToString(),
                    text);
            }
        }

        public enum ProtocolErrors
        {
            NO_ERROR = 0x0,
            PROTOCOL_ERROR = 0x1,
            INTERNAL_ERROR = 0x2,
            FLOW_CONTROL_ERROR = 0x3,
            SETTINGS_TIMEOUT = 0x4,
            STREAM_CLOSED = 0x5,
            FRAME_SIZE_ERROR = 0x6,
            REFUSED_STREAM = 0x7,
            CANCEL = 0x8,
            COMPRESSION_ERROR = 0x9,
            CONNECT_ERROR = 0xa,
            ENHANCE_YOUR_CALM = 0xb,
            INADEQUATE_SECURITY = 0xc,
            HTTP_1_1_REQUIRED = 0xd
        }

        [Fact]
        public async Task Http2_ClientPreface_Sent()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                string connectionPreface = (await server.AcceptConnectionAsync()).PrefixString;

                Assert.Equal("PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n", connectionPreface);
            }
        }

        [Fact]
        public async Task Http2_InitialSettings_SentAndAcked()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.AcceptConnectionAsync();

                // Receive the initial client settings frame.
                Frame receivedFrame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.Settings, receivedFrame.Type);

                // Send the initial server settings frame.
                Frame emptySettings = new Frame(0, FrameType.Settings, FrameFlags.None, 0);
                await connection.WriteFrameAsync(emptySettings).ConfigureAwait(false);

                // Receive the server settings frame ACK.
                // This doesn't have to be the next frame, as the client is allowed to send before receiving our SETTINGS frame.
                // So, loop until we see it (or the timeout expires)
                while (true)
                {
                    receivedFrame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    if (receivedFrame.Type == FrameType.Settings && receivedFrame.AckFlag)
                    {
                        break;
                    }
                }
            }
        }

        [Fact]
        public async Task Http2_DataSentBeforeServerPreface_ProtocolError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.AcceptConnectionAsync();

                // Send a frame despite not having sent the server connection preface.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.Padded, 10, 1);
                await connection.WriteFrameAsync(invalidFrame);

                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);
            }
        }

        [Fact]
        public async Task Http2_NoResponseBody_Success()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(0, (await response.Content.ReadAsByteArrayAsync()).Length);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_ZeroLengthResponseBody_Success()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.SendDefaultResponseHeadersAsync(streamId);

                // Send zero-length body
                var frame = new DataFrame(new byte[0], FrameFlags.EndStream, 0, streamId);
                await connection.WriteFrameAsync(frame);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(0, (await response.Content.ReadAsByteArrayAsync()).Length);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_ServerSendsValidSettingsValues_Success()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                // Send a bunch of valid SETTINGS values (that won't interfere with processing requests)
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync(
                    new SettingsEntry { SettingId = SettingId.HeaderTableSize, Value = 0 },
                    new SettingsEntry { SettingId = SettingId.HeaderTableSize, Value = 1 },
                    new SettingsEntry { SettingId = SettingId.HeaderTableSize, Value = 345678 },
                    new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 0 },
                    new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 1 },
                    new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 4567890 },
                    new SettingsEntry { SettingId = SettingId.MaxConcurrentStreams, Value = 1 },
                    new SettingsEntry { SettingId = SettingId.MaxFrameSize, Value = 16384 },
                    new SettingsEntry { SettingId = SettingId.MaxFrameSize, Value = 16777215 },
                    new SettingsEntry { SettingId = SettingId.MaxHeaderListSize, Value = 0 },
                    new SettingsEntry { SettingId = SettingId.MaxHeaderListSize, Value = 10000000 },
                    new SettingsEntry { SettingId = (SettingId)5678, Value = 1234 });

                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [ActiveIssue(35466)]
        [ConditionalTheory(nameof(SupportsAlpn))]
        [InlineData(SettingId.MaxFrameSize, 16383, ProtocolErrors.PROTOCOL_ERROR, true)]
        [InlineData(SettingId.MaxFrameSize, 162777216, ProtocolErrors.PROTOCOL_ERROR, true)]
        [InlineData(SettingId.InitialWindowSize, 0x80000000, ProtocolErrors.FLOW_CONTROL_ERROR, false)]
        public async Task Http2_ServerSendsInvalidSettingsValue_Error(SettingId settingId, uint value, ProtocolErrors expectedError, bool skipForWinHttp)
        {
            if (IsWinHttpHandler && skipForWinHttp)
            {
                // WinHTTP does not treat these as errors, it seems to ignore the invalid setting.
                return;
            }

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                // Send invalid initial SETTINGS value
                await server.EstablishConnectionAsync(new SettingsEntry { SettingId = settingId, Value = value });

                await AssertProtocolErrorAsync(sendTask, expectedError);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_StreamResetByServerBeforeHeadersSent_RequestFails()
        {
            if (IsWinHttpHandler)
            {
                // WinHTTP does not genenerate an exception here.
                // It seems to ignore a RST_STREAM sent before headers are sent, and continue to wait for HEADERS.
                return;
            }

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send a reset stream frame so that the stream moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.None, (int)ProtocolErrors.INTERNAL_ERROR, streamId);
                await connection.WriteFrameAsync(resetStream);

                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.INTERNAL_ERROR);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_StreamResetByServerAfterHeadersSent_RequestFails()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send response headers
                await connection.SendDefaultResponseHeadersAsync(streamId);

                // Send a reset stream frame so that the stream moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.None, (int)ProtocolErrors.INTERNAL_ERROR, streamId);
                await connection.WriteFrameAsync(resetStream);

                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.INTERNAL_ERROR);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_StreamResetByServerAfterPartialBodySent_RequestFails()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send response headers and partial response body
                await connection.SendDefaultResponseHeadersAsync(streamId);
                DataFrame dataFrame = new DataFrame(new byte[10], FrameFlags.None, 0, streamId);
                await connection.WriteFrameAsync(dataFrame);

                // Send a reset stream frame so that the stream moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.None, (int)ProtocolErrors.INTERNAL_ERROR, streamId);
                await connection.WriteFrameAsync(resetStream);

                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.INTERNAL_ERROR);
            }
        }

        // This test is based on RFC 7540 section 6.1:
        // "If a DATA frame is received whose stream identifier field is 0x0, the recipient MUST
        // respond with a connection error (Section 5.4.1) of type PROTOCOL_ERROR."
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task DataFrame_NoStream_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                await connection.ReadRequestHeaderAsync();

                // Send a malformed frame (streamId is 0)
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.None, 0, 0);
                await connection.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        // This test is based on RFC 7540 section 5.1:
        // "Receiving any frame other than HEADERS or PRIORITY on a stream in this state MUST
        // be treated as a connection error (Section 5.4.1) of type PROTOCOL_ERROR."
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task DataFrame_IdleStream_ConnectionError()
        {
            if (IsWinHttpHandler)
            {
                // WinHTTP does not treat this as an error, it seems to ignore the invalid frame.
                return;
            }

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                await connection.ReadRequestHeaderAsync();

                // Send a data frame on stream 5, which is in the idle state.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.None, 0, 5);
                await connection.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        // The spec does not clearly define how a client should behave when it receives unsolicited
        // headers from the server on an idle stream. We fall back to treating this as a connection
        // level error, as we do for other unexpected frames on idle streams.
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task HeadersFrame_IdleStream_ConnectionError()
        {
            if (IsWinHttpHandler)
            {
                // WinHTTP does not treat this as an error, it seems to ignore the HEADERS frame.
                return;
            }

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send a headers frame on stream 5, which is in the idle state.
                await connection.SendDefaultResponseHeadersAsync(5);

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        private static Frame MakeSimpleHeadersFrame(int streamId, bool endHeaders = false, bool endStream = false) =>
            new HeadersFrame(new byte[] { 0x88 },       // :status: 200
                (endHeaders ? FrameFlags.EndHeaders : FrameFlags.None) | (endStream ? FrameFlags.EndStream : FrameFlags.None),
                0, 0, 0, streamId);

        private static Frame MakeSimpleContinuationFrame(int streamId, bool endHeaders = false) =>
            new ContinuationFrame(new byte[] { 0x88 },       // :status: 200
                (endHeaders ? FrameFlags.EndHeaders : FrameFlags.None),
                0, 0, 0, streamId);

        private static Frame MakeSimpleDataFrame(int streamId, bool endStream = false) =>
            new DataFrame(new byte[] { 0x00 },
                (endStream ? FrameFlags.EndStream : FrameFlags.None),
                0, streamId);

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_ContinuationBeforeHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleContinuationFrame(streamId));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_DataBeforeHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleDataFrame(streamId));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_HeadersAfterHeadersWithoutEndHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_HeadersAfterHeadersAndContinuationWithoutEndHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleContinuationFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_DataAfterHeadersWithoutEndHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleDataFrame(streamId));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task ResponseStreamFrames_DataAfterHeadersAndContinuationWithoutEndHeaders_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                await connection.WriteFrameAsync(MakeSimpleHeadersFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleContinuationFrame(streamId, endHeaders: false));
                await connection.WriteFrameAsync(MakeSimpleDataFrame(streamId));

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        // This test is based on RFC 7540 section 6.8:
        // "An endpoint MUST treat a GOAWAY frame with a stream identifier other than 0x0 as a
        // connection error (Section 5.4.1) of type PROTOCOL_ERROR."
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task GoAwayFrame_NonzeroStream_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                await connection.ReadRequestHeaderAsync();

                // Send a GoAway frame on stream 1.
                GoAwayFrame invalidFrame = new GoAwayFrame(0, (int)ProtocolErrors.ENHANCE_YOUR_CALM, new byte[0], 1);
                await connection.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.PROTOCOL_ERROR);

                // The client should close the connection as this is a fatal connection level error.
                Assert.Null(await connection.ReadFrameAsync(TimeSpan.FromSeconds(30)));
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task GoAwayFrame_NewRequest_NewConnection()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                server.AllowMultipleConnections = true;

                Task<HttpResponseMessage> sendTask1 = client.GetAsync(server.Address);
                Http2LoopbackConnection connection1 = await server.EstablishConnectionAsync();
                int streamId1 = await connection1.ReadRequestHeaderAsync();

                await connection1.SendGoAway(streamId1);

                await connection1.SendDefaultResponseAsync(streamId1);
                HttpResponseMessage response1 = await sendTask1;
                Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

                // New connection should be established after GOAWAY
                Task<HttpResponseMessage> sendTask2 = client.GetAsync(server.Address);
                Http2LoopbackConnection connection2 = await server.EstablishConnectionAsync();
                int streamId2 = await connection2.ReadRequestHeaderAsync();
                await connection2.SendDefaultResponseAsync(streamId2);
                HttpResponseMessage response2 = await sendTask2;
                Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

                await connection1.WaitForConnectionShutdownAsync();
                await connection2.WaitForConnectionShutdownAsync();
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task DataFrame_TooLong_ConnectionError()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                await connection.ReadRequestHeaderAsync();

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[Frame.MaxFrameLength + 1], FrameFlags.None, 0, 0);
                await connection.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await AssertProtocolErrorAsync(sendTask, ProtocolErrors.FRAME_SIZE_ERROR);
            }
        }

        [ConditionalTheory(nameof(SupportsAlpn))]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CompletedResponse_FrameReceived_Ignored(bool sendDataFrame)
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                connection.IgnoreWindowUpdates();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send response and end stream.
                await connection.SendDefaultResponseHeadersAsync(streamId);
                DataFrame dataFrame = new DataFrame(new byte[10], FrameFlags.EndStream, 0, streamId);
                await connection.WriteFrameAsync(dataFrame);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Send a frame on the now-closed stream.
                Frame invalidFrame = ConstructInvalidFrameForClosedStream(streamId, sendDataFrame);
                await connection.WriteFrameAsync(invalidFrame);

                // Pingpong to ensure the frame is processed and ignored
                await connection.PingPong();

                await ValidateConnection(client, server.Address, connection);
            }
        }

        [ConditionalTheory(nameof(SupportsAlpn))]
        [InlineData(true)]
        [InlineData(false)]
        public async Task EmptyResponse_FrameReceived_Ignored(bool sendDataFrame)
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send empty response.
                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Send a frame on the now-closed stream.
                Frame invalidFrame = ConstructInvalidFrameForClosedStream(streamId, sendDataFrame);
                await connection.WriteFrameAsync(invalidFrame);

                // Pingpong to ensure the frame is processed and ignored
                await connection.PingPong();

                await ValidateConnection(client, server.Address, connection);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task CompletedResponse_WindowUpdateFrameReceived_Success()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();

                // Send empty response.
                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Send a frame on the now-closed stream.
                WindowUpdateFrame invalidFrame = new WindowUpdateFrame(1, streamId);
                await connection.WriteFrameAsync(invalidFrame);

                // The client should close the connection.
                await connection.WaitForConnectionShutdownAsync();
            }
        }

        private static Frame ConstructInvalidFrameForClosedStream(int streamId, bool dataFrame)
        {
            if (dataFrame)
            {
                return new DataFrame(new byte[10], FrameFlags.None, 0, streamId);
            }
            else
            {
                byte[] headers = new byte[] { 0x88 };   // Encoding for ":status: 200"
                return new HeadersFrame(headers, FrameFlags.EndHeaders, 0, 0, 0, streamId);
            }
        }

        // Validate that connection is still usable, by sending a request and receiving a response
        private static async Task<int> ValidateConnection(HttpClient client, Uri serverAddress, Http2LoopbackConnection connection)
        {
            Task<HttpResponseMessage> sendTask = client.GetAsync(serverAddress);

            int streamId = await connection.ReadRequestHeaderAsync();
            await connection.SendDefaultResponseAsync(streamId);

            HttpResponseMessage response = await sendTask;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            return streamId;
        }

        public static IEnumerable<object[]> ValidAndInvalidProtocolErrors() =>
            Enum.GetValues(typeof(ProtocolErrors))
            .Cast<ProtocolErrors>()
            .Concat(new[] { (ProtocolErrors)12345 })
            .Select(p => new object[] { p });

        public static IEnumerable<object[]> ValidAndInvalidProtocolErrorsAndBool()
        {
            foreach (object[] args in ValidAndInvalidProtocolErrors())
            {
                yield return args.Append(true).ToArray();
                yield return args.Append(false).ToArray();
            }
        }

        [ConditionalTheory(nameof(SupportsAlpn))]
        [MemberData(nameof(ValidAndInvalidProtocolErrorsAndBool))]
        public async Task ResetResponseStream_FrameReceived_Ignored(ProtocolErrors error, bool dataFrame)
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendDefaultResponseHeadersAsync(streamId);

                // Send a reset stream frame so that stream 1 moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.None, (int)error, streamId);
                await connection.WriteFrameAsync(resetStream);

                await AssertProtocolErrorAsync(sendTask, error);

                // Send a frame on the now-closed stream.
                Frame invalidFrame = ConstructInvalidFrameForClosedStream(streamId, dataFrame);
                await connection.WriteFrameAsync(invalidFrame);

                // Pingpong to ensure the frame is processed and ignored
                await connection.PingPong();

                await ValidateConnection(client, server.Address, connection);
            }
        }

        private static async Task<(int, Http2LoopbackConnection)> EstablishConnectionAndProcessOneRequestAsync(HttpClient client, Http2LoopbackServer server)
        {
            int streamId = -1;

            // Establish connection and send initial request/response to ensure connection is available for subsequent use
            Http2LoopbackConnection connection = null;

            await new[]
            {
                Task.Run(async () =>
                {
                    using (HttpResponseMessage response = await client.GetAsync(server.Address))
                    {
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                        Assert.Equal(0, (await response.Content.ReadAsByteArrayAsync()).Length);
                    }
                }),
                Task.Run(async () =>
                {
                    connection = await server.EstablishConnectionAsync();
                    streamId = await connection.ReadRequestHeaderAsync();
                    await connection.SendDefaultResponseAsync(streamId);
                })
            }.WhenAllOrAnyFailed(TestHelper.PassingTestTimeoutMilliseconds);

            return (streamId, connection);
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task GoAwayFrame_NoPendingStreams_ConnectionClosed()
        {
            using (new Timer(s => Console.WriteLine(GetStateMachineData.Describe(s)), await GetStateMachineData.FetchAsync(), 60_000, 60_000))
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                (int streamId, Http2LoopbackConnection connection) = await EstablishConnectionAndProcessOneRequestAsync(client, server);

                // Send GOAWAY.
                GoAwayFrame goAwayFrame = new GoAwayFrame(streamId, 0, new byte[0], 0);
                await connection.WriteFrameAsync(goAwayFrame);

                // The client should close the connection.
                await connection.WaitForConnectionShutdownAsync();

                // New request should cause a new connection
                await EstablishConnectionAndProcessOneRequestAsync(client, server);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task GoAwayFrame_AllPendingStreamsValid_RequestsSucceedAndConnectionClosed()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                (_, Http2LoopbackConnection connection) = await EstablishConnectionAndProcessOneRequestAsync(client, server);

                // Issue three requests
                Task<HttpResponseMessage> sendTask1 = client.GetAsync(server.Address);
                Task<HttpResponseMessage> sendTask2 = client.GetAsync(server.Address);
                Task<HttpResponseMessage> sendTask3 = client.GetAsync(server.Address);

                // Receive three requests
                int streamId1 = await connection.ReadRequestHeaderAsync();
                int streamId2 = await connection.ReadRequestHeaderAsync();
                int streamId3 = await connection.ReadRequestHeaderAsync();

                Assert.True(streamId1 < streamId2);
                Assert.True(streamId2 < streamId3);

                // Send various partial responses

                // First response: Don't send anything yet

                // Second response: Send headers, no body yet
                await connection.SendDefaultResponseHeadersAsync(streamId2);

                // Third response: Send headers, partial body
                await connection.SendDefaultResponseHeadersAsync(streamId3);
                await connection.SendResponseDataAsync(streamId3, new byte[5], endStream: false);

                // Send a GOAWAY frame that indicates that we will process all three streams
                GoAwayFrame goAwayFrame = new GoAwayFrame(streamId3, 0, new byte[0], 0);
                await connection.WriteFrameAsync(goAwayFrame);

                // Finish sending responses
                await connection.SendDefaultResponseHeadersAsync(streamId1);
                await connection.SendResponseDataAsync(streamId1, new byte[10], endStream: true);
                await connection.SendResponseDataAsync(streamId2, new byte[10], endStream: true);
                await connection.SendResponseDataAsync(streamId3, new byte[5], endStream: true);

                // We will not send any more frames, so send EOF now, and ensure the client handles this properly.
                connection.ShutdownSend();

                // Receive all responses
                HttpResponseMessage response1 = await sendTask1;
                Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                Assert.Equal(10, (await response1.Content.ReadAsByteArrayAsync()).Length);
                HttpResponseMessage response2 = await sendTask2;
                Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
                Assert.Equal(10, (await response2.Content.ReadAsByteArrayAsync()).Length);
                HttpResponseMessage response3 = await sendTask3;
                Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
                Assert.Equal(10, (await response3.Content.ReadAsByteArrayAsync()).Length);

                // Now that all pending responses have been sent, the client should close the connection.
                await connection.WaitForConnectionShutdownAsync();

                // New request should cause a new connection
                await EstablishConnectionAndProcessOneRequestAsync(client, server);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task GoAwayFrame_AbortAllPendingStreams_StreamFailWithExpectedException()
        {
            using (new Timer(s => Console.WriteLine(GetStateMachineData.Describe(s)), await GetStateMachineData.FetchAsync(), 60_000, 60_000))
            using (Http2LoopbackServer server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                (_, Http2LoopbackConnection connection) = await EstablishConnectionAndProcessOneRequestAsync(client, server);

                // Issue three requests
                Task<HttpResponseMessage> sendTask1 = client.GetAsync(server.Address);
                Task<HttpResponseMessage> sendTask2 = client.GetAsync(server.Address);
                Task<HttpResponseMessage> sendTask3 = client.GetAsync(server.Address);

                // Receive three requests
                int streamId1 = await connection.ReadRequestHeaderAsync();
                int streamId2 = await connection.ReadRequestHeaderAsync();
                int streamId3 = await connection.ReadRequestHeaderAsync();

                Assert.InRange(streamId1, int.MinValue, streamId2 - 1);
                Assert.InRange(streamId2, int.MinValue, streamId3 - 1);

                // Send various partial responses

                // First response: Don't send anything yet

                // Second response: Send headers, no body yet
                await connection.SendDefaultResponseHeadersAsync(streamId2);

                // Third response: Send headers, partial body
                await connection.SendDefaultResponseHeadersAsync(streamId3);
                await connection.SendResponseDataAsync(streamId3, new byte[5], endStream: false);

                // Send a GOAWAY frame that indicates that we will abort all the requests.
                var goAwayFrame = new GoAwayFrame(0, (int)ProtocolErrors.ENHANCE_YOUR_CALM, new byte[0], 0);
                await connection.WriteFrameAsync(goAwayFrame);

                // We will not send any more frames, so send EOF now, and ensure the client handles this properly.
                connection.ShutdownSend();

                await AssertProtocolErrorAsync(sendTask1, ProtocolErrors.ENHANCE_YOUR_CALM);
                await AssertProtocolErrorAsync(sendTask2, ProtocolErrors.ENHANCE_YOUR_CALM);
                await AssertProtocolErrorAsync(sendTask3, ProtocolErrors.ENHANCE_YOUR_CALM);

                // Now that all pending responses have been sent, the client should close the connection.
                await connection.WaitForConnectionShutdownAsync();

                // New request should cause a new connection
                await EstablishConnectionAndProcessOneRequestAsync(client, server);
            }
        }

        private static async Task<int> ReadToEndOfStream(Http2LoopbackConnection connection, int streamId)
        {
            int bytesReceived = 0;
            while (true)
            {
                Frame frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                Assert.Equal(streamId, frame.StreamId);
                Assert.Equal(FrameType.Data, frame.Type);

                bytesReceived += frame.Length;

                if (frame.Flags == FrameFlags.EndStream)
                {
                    break;
                }

                Assert.Equal(FrameFlags.None, frame.Flags);
                Assert.True(frame.Length > 0);
            }

            return bytesReceived;
        }

        [ActiveIssue(38799)]
        [OuterLoop("Uses Task.Delay")]
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_FlowControl_ClientDoesNotExceedWindows()
        {
            const int InitialWindowSize = 65535;
            const int ContentSize = 100_000;

            var content = new ByteArrayContent(TestHelper.GenerateRandomContent(ContentSize));

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> clientTask = client.PostAsync(server.Address, content);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                Frame frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                int streamId = frame.StreamId;
                Assert.Equal(FrameType.Headers, frame.Type);
                Assert.Equal(FrameFlags.EndHeaders, frame.Flags);

                // Receive up to initial window size
                int bytesReceived = 0;
                while (bytesReceived < InitialWindowSize)
                {
                    frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    Assert.Equal(streamId, frame.StreamId);
                    Assert.Equal(FrameType.Data, frame.Type);
                    Assert.Equal(FrameFlags.None, frame.Flags);
                    Assert.True(frame.Length > 0);

                    bytesReceived += frame.Length;
                }

                Assert.Equal(InitialWindowSize, bytesReceived);

                // Issue another read. It shouldn't complete yet. Wait a brief period of time to ensure it doesn't complete.
                Task<Frame> readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window by one. This should still not complete the read.
                await connection.WriteFrameAsync(new WindowUpdateFrame(1, 0));

                await Task.Delay(500);

                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window by two. This should complete the read with a single byte.
                await connection.WriteFrameAsync(new WindowUpdateFrame(2, streamId));

                frame = await readFrameTask;
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window by two. This should complete the read with a single byte.
                await connection.WriteFrameAsync(new WindowUpdateFrame(2, 0));

                frame = await readFrameTask;
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window to allow exactly the remaining request size. This should still not complete the read.
                await connection.WriteFrameAsync(new WindowUpdateFrame(ContentSize - bytesReceived - 1, 0));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window to allow exactly the remaining request size. This should allow the rest of the request to be sent.
                await connection.WriteFrameAsync(new WindowUpdateFrame(ContentSize - bytesReceived, streamId));

                frame = await readFrameTask;
                Assert.Equal(streamId, frame.StreamId);
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(FrameFlags.None, frame.Flags);
                Assert.True(frame.Length > 0);

                bytesReceived += frame.Length;

                // Read to end of stream
                bytesReceived += await ReadToEndOfStream(connection, streamId);

                Assert.Equal(ContentSize, bytesReceived);

                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await clientTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [OuterLoop("Uses Task.Delay")]
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_InitialWindowSize_ClientDoesNotExceedWindows()
        {
            const int DefaultInitialWindowSize = 65535;
            const int ContentSize = 100_000;

            var content = new ByteArrayContent(TestHelper.GenerateRandomContent(ContentSize));

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> clientTask = client.PostAsync(server.Address, content);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                // Bump connection window so it won't block the client.
                await connection.WriteFrameAsync(new WindowUpdateFrame(ContentSize - DefaultInitialWindowSize, 0));

                Frame frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                int streamId = frame.StreamId;
                Assert.Equal(FrameType.Headers, frame.Type);
                Assert.Equal(FrameFlags.EndHeaders, frame.Flags);

                // Receive up to initial window size
                int bytesReceived = 0;
                while (bytesReceived < DefaultInitialWindowSize)
                {
                    frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    Assert.Equal(streamId, frame.StreamId);
                    Assert.Equal(FrameType.Data, frame.Type);
                    Assert.Equal(FrameFlags.None, frame.Flags);
                    Assert.True(frame.Length > 0);

                    bytesReceived += frame.Length;
                }

                Assert.Equal(DefaultInitialWindowSize, bytesReceived);

                // Issue another read. It shouldn't complete yet. Wait a brief period of time to ensure it doesn't complete.
                Task<Frame> readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Change SETTINGS_INITIAL_WINDOW_SIZE to 0. This will make the client's credit go negative.
                connection.ExpectSettingsAck();
                await connection.WriteFrameAsync(new SettingsFrame(new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 0 }));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window by one. Client credit will still be negative.
                await connection.WriteFrameAsync(new WindowUpdateFrame(1, streamId));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Change SETTINGS_INITIAL_WINDOW_SIZE to 1. Client credit will still be negative.
                connection.ExpectSettingsAck();
                await connection.WriteFrameAsync(new SettingsFrame(new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 1 }));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window so client credit will be 0.
                await connection.WriteFrameAsync(new WindowUpdateFrame(DefaultInitialWindowSize - 2, streamId));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window by one, so client can now send a single byte.
                await connection.WriteFrameAsync(new WindowUpdateFrame(1, streamId));

                frame = await readFrameTask;
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase SETTINGS_INITIAL_WINDOW_SIZE to 2, so client can now send a single byte.
                connection.ExpectSettingsAck();
                await connection.WriteFrameAsync(new SettingsFrame(new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = 2 }));

                frame = await readFrameTask;
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase SETTINGS_INITIAL_WINDOW_SIZE to be enough that the client can send the rest of the content.
                connection.ExpectSettingsAck();
                await connection.WriteFrameAsync(new SettingsFrame(new SettingsEntry { SettingId = SettingId.InitialWindowSize, Value = ContentSize - (DefaultInitialWindowSize - 1) }));

                frame = await readFrameTask;
                Assert.Equal(streamId, frame.StreamId);
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(FrameFlags.None, frame.Flags);
                Assert.True(frame.Length > 0);

                bytesReceived += frame.Length;

                // Read to end of stream
                bytesReceived += await ReadToEndOfStream(connection, streamId);

                Assert.Equal(ContentSize, bytesReceived);

                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await clientTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [OuterLoop("Uses Task.Delay")]
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_MaxConcurrentStreams_LimitEnforced()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                connection.IgnoreWindowUpdates();

                // Process first request and send response.
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Change MaxConcurrentStreams setting and wait for ack.
                // (We don't want to send any new requests until we receive the ack, otherwise we may have a timing issue.)
                SettingsFrame settingsFrame = new SettingsFrame(new SettingsEntry { SettingId = SettingId.MaxConcurrentStreams, Value = 0 });
                await connection.WriteFrameAsync(settingsFrame);
                Frame settingsAckFrame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.Settings, settingsAckFrame.Type);
                Assert.Equal(FrameFlags.Ack, settingsAckFrame.Flags);

                // Issue two more requests. We shouldn't send either of them.
                sendTask = client.GetAsync(server.Address);
                Task<HttpResponseMessage> sendTask2 = client.GetAsync(server.Address);

                // Issue another read. It shouldn't complete yet. Wait a brief period of time to ensure it doesn't complete.
                Task<Frame> readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Change MaxConcurrentStreams again to allow a single request to come through.
                connection.ExpectSettingsAck();
                settingsFrame = new SettingsFrame(new SettingsEntry { SettingId = SettingId.MaxConcurrentStreams, Value = 1 });
                await connection.WriteFrameAsync(settingsFrame);

                // First request should be sent
                Frame frame = await readFrameTask;
                Assert.Equal(FrameType.Headers, frame.Type);
                streamId = frame.StreamId;

                // Issue another read. Second request should not be sent yet.
                readFrameTask = connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Send response for first request
                await connection.SendDefaultResponseAsync(streamId);
                response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Second request should be sent now
                frame = await readFrameTask;
                Assert.Equal(FrameType.Headers, frame.Type);
                streamId = frame.StreamId;

                // Send response for second request
                await connection.SendDefaultResponseAsync(streamId);
                response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [OuterLoop("Uses Task.Delay")]
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_WaitingForStream_Cancellation()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                connection.IgnoreWindowUpdates();

                // Process first request and send response.
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Change MaxConcurrentStreams setting and wait for ack.
                // (We don't want to send any new requests until we receive the ack, otherwise we may have a timing issue.)
                SettingsFrame settingsFrame = new SettingsFrame(new SettingsEntry { SettingId = SettingId.MaxConcurrentStreams, Value = 0 });
                await connection.WriteFrameAsync(settingsFrame);
                Frame settingsAckFrame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.Settings, settingsAckFrame.Type);
                Assert.Equal(FrameFlags.Ack, settingsAckFrame.Flags);

                // Issue a new request, so that we can cancel it while it waits for a stream.
                var cts = new CancellationTokenSource();
                sendTask = client.GetAsync(server.Address, cts.Token);

                // Make sure that the request makes it to the point where it's waiting for a connection.
                // It's possible that we'll still initiate a cancellation before it makes it to the queue,
                // but it should still behave in the same way if so.
                await Task.Delay(500);

                Stopwatch stopwatch = Stopwatch.StartNew();
                cts.Cancel();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await sendTask);

                // Ensure that the cancellation occurs promptly
                stopwatch.Stop();
                Assert.True(stopwatch.ElapsedMilliseconds < 30000);

                // As the client has not allocated a stream ID when the corresponding request is cancelled,
                // we do not send a RST stream frame.
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_WaitingOnWindowCredit_Cancellation()
        {
            // The goal of this test is to get the client into the state where it has sent the headers,
            // but is waiting on window credit before it will send the body. We then issue a cancellation
            // to ensure the request is cancelled as expected.
            const int InitialWindowSize = 65535;
            const int ContentSize = InitialWindowSize + 1;

            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            var content = new ByteArrayContent(TestHelper.GenerateRandomContent(ContentSize));

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient(handler))
            {
                var cts = new CancellationTokenSource();
                Task<HttpResponseMessage> clientTask = client.PostAsync(server.Address, content, cts.Token);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                Frame frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                int streamId = frame.StreamId;
                Assert.Equal(FrameType.Headers, frame.Type);
                Assert.Equal(FrameFlags.EndHeaders, frame.Flags);

                // Receive up to initial window size
                int bytesReceived = 0;
                while (bytesReceived < InitialWindowSize)
                {
                    frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    Assert.Equal(streamId, frame.StreamId);
                    Assert.Equal(FrameType.Data, frame.Type);
                    Assert.Equal(FrameFlags.None, frame.Flags);
                    Assert.True(frame.Length > 0);

                    bytesReceived += frame.Length;
                }

                // The client is waiting for more credit in order to send the last byte of the
                // request body. Test cancellation at this point.
                Stopwatch stopwatch = Stopwatch.StartNew();

                cts.Cancel();
                await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await clientTask);

                // Ensure that the cancellation occurs promptly
                stopwatch.Stop();
                Assert.True(stopwatch.ElapsedMilliseconds < 30000);

                // The server should receive a RstStream frame.
                frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.RstStream, frame.Type);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_PendingSend_Cancellation()
        {
            // The goal of this test is to get the client into the state where it is sending content,
            // but the send pends because the TCP window is full.
            const int InitialWindowSize = 65535;
            const int ContentSize = InitialWindowSize * 2; // Double the default TCP window size.

            var content = new ByteArrayContent(TestHelper.GenerateRandomContent(ContentSize));

            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                var cts = new CancellationTokenSource();

                Task<HttpResponseMessage> clientTask = client.PostAsync(server.Address, content, cts.Token);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                Frame frame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(30));
                int streamId = frame.StreamId;
                Assert.Equal(FrameType.Headers, frame.Type);
                Assert.Equal(FrameFlags.EndHeaders, frame.Flags);

                // Increase the size of the HTTP/2 Window, so that it is large enough to fill the
                // TCP window when we do not perform any reads on the server side.
                await connection.WriteFrameAsync(new WindowUpdateFrame(InitialWindowSize, streamId));

                // Give the client time to read the window update frame, and for the write to pend.
                await Task.Delay(1000);
                cts.Cancel();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await clientTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_PendingSend_SendsReset()
        {
            var cts = new CancellationTokenSource();

            string content = new string('*', 300);
            var stream = new CustomContent.SlowTestStream(Encoding.UTF8.GetBytes(content), null, count: 10);
            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Version = new Version(2,0);
                    request.Content = new StreamContent(stream);

                    await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await client.SendAsync(request, cts.Token));
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);

                // Cancel client after receiving Headers.
                cts.Cancel();
                Frame frame;
                do
                {
                    frame = await connection.ReadFrameAsync(TimeSpan.FromMilliseconds(TestHelper.PassingTestTimeoutMilliseconds)).ConfigureAwait(false);
                    Assert.NotNull(frame); // We should get Rst before closing connection.
                    Assert.Equal(0, (int)(frame.Flags & FrameFlags.EndStream));
                 } while (frame.Type != FrameType.RstStream);
            });
        }

        [ConditionalTheory(nameof(SupportsAlpn))]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Http2_PendingReceive_SendsReset(bool doRead)
        {
            var cts = new CancellationTokenSource();
            bool isCanceled = false;
            HttpResponseMessage response = null;
            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Version = new Version(2,0);

                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    using (Stream stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (doRead)
                        {
                            try
                            {
                                int readLength;
                                do {
                                    byte[] buffer = new byte[100];

                                    readLength = await stream.ReadAsync(buffer, cts.Token);
                                } while (readLength != 0);
                            }
                            catch (OperationCanceledException) { };
                        }
                        isCanceled = true;
                    }
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);
                _output.WriteLine($"{DateTime.Now} Connection established");
                // Cancel client after receiving Headers.
                await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.OK);

                // Start streaming response
                DataFrame dataFrame = new DataFrame(new byte[100], FrameFlags.None, 0, streamId);
                await connection.WriteFrameAsync(dataFrame);

                // Keep sending data until clients cancels
                while (!isCanceled)
                {
                    if (response != null)
                    {
                        cts.Cancel();
                    }
                    await connection.WriteFrameAsync(dataFrame);
                    await Task.Delay(100);
                }

                _output.WriteLine($"{DateTime.Now} HttpRequest was canceled");
                Frame frame;
                do
                {
                    frame = await connection.ReadFrameAsync(TimeSpan.FromMilliseconds(TestHelper.PassingTestTimeoutMilliseconds)).ConfigureAwait(false);
                    Assert.NotNull(frame); // We should get Rst before closing connection.
                    Assert.Equal(0, (int)(frame.Flags & FrameFlags.EndStream));
                 } while (frame.Type != FrameType.RstStream);
            });
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Dispose_ProcessingRequest_Throws()
        {
            HttpClient client =  CreateHttpClient();
            bool stopSending = false;

            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                string content = new string('*', 300);
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Version = new Version(2,0);
                request.Content = new StreamContent(new CustomContent.SlowTestStream(Encoding.UTF8.GetBytes(content), null, count : 20));
                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                Exception innerException = null;

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    // Dispose client after receiving response headers.
                    client.Dispose();

                    byte[] buffer = new byte[100];
                    try
                    {
                        do
                        {
                            int readLength = await stream.ReadAsync(buffer);
                            Assert.NotEqual(0, readLength);
                        } while (true);
                    }
                    catch (System.IO.IOException e)
                    {
                        Assert.NotNull(e.InnerException);
                        innerException = e.InnerException;
                    }
                    finally
                    {
                        stopSending = true;
                    }
                    Assert.True(innerException is HttpRequestException);
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);
                await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.OK);

                // Start streaming response and wait for client to be disposed.
                byte[] responseBody = new byte[100];
                while (!stopSending)
                {
                    await connection.SendResponseDataAsync(streamId, responseBody, endStream: false);
                    await Task.Delay(100);
                }

                await connection.SendGoAway(streamId);
            });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task PostAsyncExpect100Continue_SendRequest_Ok(bool send100Continue)
        {
            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Version = new Version(2,0);
                    request.Content = new StringContent(new string('*', 3000));
                    request.Headers.ExpectContinue = true;
                    request.Headers.Add("x-test", $"PostAsyncExpect100Continue_SendRequest_Ok({send100Continue}");

                    HttpResponseMessage response = await client.SendAsync(request);
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);
                Assert.Equal("100-continue", requestData.GetSingleHeaderValue("Expect"));

                if (send100Continue)
                {
                    await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.Continue);
                }
                await connection.ReadBodyAsync();
                await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.OK);
                await connection.SendResponseBodyAsync(streamId, Encoding.ASCII.GetBytes("OK"));
                await connection.ShutdownIgnoringErrorsAsync(streamId);
            });
        }

        [Fact]
        public async Task PostAsyncExpect100Continue_LateForbiddenResponse_Ok()
        {
            TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
            string content = new string('*', 300);

            var stream = new CustomContent.SlowTestStream(Encoding.UTF8.GetBytes(content), tsc, trigger : 3, count : 30);

            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Version = new Version(2,0);
                    request.Content = new StreamContent(stream);
                    request.Headers.ExpectContinue = true;
                    request.Headers.Add("x-test", "PostAsyncExpect100Continue_LateForbiddenResponse_Ok");

                    HttpResponseMessage response = await client.SendAsync(request);
                    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);
                Assert.Equal("100-continue", requestData.GetSingleHeaderValue("Expect"));

                // Wait for client so start sending body.
                await tsc.Task.ConfigureAwait(false);
                // And reject content with 403.
                await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.Forbidden);
                await connection.SendResponseBodyAsync(streamId, Encoding.ASCII.GetBytes("no no!"));
                try
                {
                    // Client should send reset.
                    await connection.ReadBodyAsync();
                    Assert.True(false, "Should not be here");
                }
                catch (IOException) { };

                await connection.ShutdownIgnoringErrorsAsync(streamId);
            });
        }

        [Theory]
        [InlineData(true, HttpStatusCode.Forbidden)]
        [InlineData(false, HttpStatusCode.Forbidden)]
        [InlineData(true, HttpStatusCode.OK)]
        [InlineData(false, HttpStatusCode.OK)]
        public async Task SendAsync_ConcurentSendReceive_Ok(bool shouldWait, HttpStatusCode responseCode)
        {
            TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
            string requestContent = new string('*', 300);
            const string responseContent = "SendAsync_ConcurentSendReceive_Ok";
            var stream = new CustomContent.SlowTestStream(Encoding.UTF8.GetBytes(requestContent), tsc, trigger : 1, count : 10);

            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Version = new Version(2,0);
                    request.Content = new StreamContent(stream);

                    HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    Assert.Equal(responseCode, response.StatusCode);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Assert.Equal(responseContent, responseBody);
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);

                // Wait for client so start sending body.
                await tsc.Task.ConfigureAwait(false);

                if (shouldWait)
                {
                    // Read body first before sending back response
                    await connection.ReadBodyAsync();
                }

                await connection.SendResponseHeadersAsync(streamId, endStream: false, responseCode);
                await connection.SendResponseDataAsync(streamId, Encoding.ASCII.GetBytes(responseContent), endStream: false);
                if (!shouldWait)
                {
                    try
                    {
                        // Client should send reset.
                        await connection.ReadBodyAsync();
                        if (responseCode != HttpStatusCode.OK) Assert.True(false, "Should not be here");
                    }
                    catch (IOException) when (responseCode != HttpStatusCode.OK) { };
                }
                var headers = new HttpHeaderData[] { new HttpHeaderData("x-last", "done") };
                await connection.SendResponseHeadersAsync(streamId, endStream: true, isTrailingHeader : true, headers: headers);
                await connection.ShutdownIgnoringErrorsAsync(streamId);
            });
        }

        [Fact]
        public async Task SendAsync_ConcurentSendReceive_Fail()
        {
            TaskCompletionSource<bool> tsc = new TaskCompletionSource<bool>();
            string requestContent = new string('*', 300);
            const string responseContent = "SendAsync_ConcurentSendReceive_Fail";
            var stream = new CustomContent.SlowTestStream(Encoding.UTF8.GetBytes(requestContent), tsc, trigger : 1, count : 50);
            bool stopSending = false;

            await Http2LoopbackServer.CreateClientAndServerAsync(async url =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Version = new Version(2,0);
                    request.Content = new StreamContent(stream);

                    // This should fail either while getting response headers or while reading response body.
                    HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    // Wait for request body start streaming.
                    await tsc.Task.ConfigureAwait(false);
                    // and inject distinct exception on request stream.
                    stream.SetException(new ArithmeticException("Injected test exception"));

                    Exception e = await Assert.ThrowsAsync<HttpRequestException>(() => response.Content.ReadAsStringAsync());
                    Assert.True(e.InnerException is IOException);
                    Assert.True(e.InnerException.InnerException is ArithmeticException);
                    stopSending = true;
                }
            },
            async server =>
            {
                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();

                (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync(readBody : false);
                await connection.SendResponseHeadersAsync(streamId, endStream: false, HttpStatusCode.OK);

                // Wait for client so start sending body.
                await tsc.Task.ConfigureAwait(false);

                int maxCount = 120;
                while (!stopSending && maxCount != 0)
                {
                    await connection.SendResponseDataAsync(streamId, Encoding.ASCII.GetBytes(responseContent), endStream: false);
                    await Task.Delay(500);
                    maxCount --;
                }
                // We should not reach retry limit without failing.
                Assert.NotEqual(0, maxCount);

                var headers = new HttpHeaderData[] { new HttpHeaderData("x-last", "done") };
                await connection.SendResponseHeadersAsync(streamId, endStream: true, isTrailingHeader : true, headers: headers);
                try
                {
                    await connection.SendGoAway(streamId);
                    await connection.WaitForConnectionShutdownAsync();
                }
                catch { };
            });
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_ProtocolMismatch_Throws()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (HttpClient client = CreateHttpClient())
            {
                // Create HTTP/1.1 loopback server and advertise HTTP2 via ALPN.
                await LoopbackServer.CreateServerAsync(async (server, uri) =>
                {
                    // Convert http to https as we are going to negotiate TLS.
                    Task<string> requestTask = client.GetStringAsync(uri.ToString().Replace("http://", "https://"));

                    await server.AcceptConnectionAsync(async connection =>
                    {
                        // negotiate TLS with ALPN H/2
                        var sslStream = new SslStream(connection.Stream, false, delegate { return true; });
                        SslServerAuthenticationOptions options = new SslServerAuthenticationOptions();
                        options.ServerCertificate = Net.Test.Common.Configuration.Certificates.GetServerCertificate();
                        options.ApplicationProtocols = new List<SslApplicationProtocol>() { SslApplicationProtocol.Http2 };
                        options.ApplicationProtocols.Add(SslApplicationProtocol.Http2);
                        // Negotiate TLS.
                        await sslStream.AuthenticateAsServerAsync(options, CancellationToken.None).ConfigureAwait(false);
                        // Send back HTTP/1.1 response
                        await sslStream.WriteAsync(Encoding.ASCII.GetBytes("HTTP/1.1 400 Unrecognized request\r\n\r\n"), CancellationToken.None);
                    });


                    Exception e = await Assert.ThrowsAsync<HttpRequestException>(() => requestTask);
                    Assert.NotNull(e.InnerException);
                    Assert.False(e.InnerException is ObjectDisposedException);
                });
            }
        }

        // rfc7540 8.1.2.3.
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2GetAsync_MultipleStatusHeaders_Throws()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                IList<HttpHeaderData> headers = new HttpHeaderData[] { new HttpHeaderData(":status", "300"), new HttpHeaderData("x-test", "Http2GetAsync_MultipleStatusHeaders_Throws") };
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendResponseHeadersAsync(streamId, endStream : true, headers: headers);
                await Assert.ThrowsAsync<HttpRequestException>(() => sendTask);
            }
        }

        // rfc7540 8.1.2.3.
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2GetAsync_StatusHeaderNotFirst_Throws()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                IList<HttpHeaderData> headers = new HttpHeaderData[] { new HttpHeaderData("x-test", "Http2GetAsync_StatusHeaderNotFirst_Throws"), new HttpHeaderData(":status", "200") };
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendResponseHeadersAsync(streamId, endStream : true, isTrailingHeader : true, headers: headers);

                await Assert.ThrowsAsync<HttpRequestException>(() => sendTask);
            }
        }

        // rfc7540 8.1.2.3.
        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2GetAsync_TrailigPseudo_Throw()
        {
            using (var server = Http2LoopbackServer.CreateServer())
            using (HttpClient client = CreateHttpClient())
            {
                IList<HttpHeaderData> headers = new HttpHeaderData[] { new HttpHeaderData(":path", "http"), new HttpHeaderData("x-test", "Http2GetAsync_TrailigPseudo_Throw") };
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                int streamId = await connection.ReadRequestHeaderAsync();
                await connection.SendDefaultResponseHeadersAsync(streamId);
                await connection.SendResponseDataAsync(streamId, Encoding.ASCII.GetBytes("hello"), endStream: false);
                await connection.SendResponseHeadersAsync(streamId, endStream : true, isTrailingHeader : true, headers: headers);

                await Assert.ThrowsAsync<HttpRequestException>(() => sendTask);
            }
        }

        [Fact]
        public async Task InboundWindowSize_Exceeded_Throw()
        {
            var semaphore = new SemaphoreSlim(0);

            await Http2LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    // An exception will be thrown by either GetAsync or ReadAsStringAsync once
                    // the inbound window size has been exceeded. Which one depends on how quickly
                    // ProcessIncomingFramesAsync() can read data off the socket.
                    Exception requestException = await Assert.ThrowsAsync<HttpRequestException>(async () =>
                    {
                        using HttpClient client = CreateHttpClient();
                        using HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

                        // Keep client open until server is done.
                        await semaphore.WaitAsync(10000);

                        await response.Content.ReadAsStringAsync();
                    });

                    // A Http2ProtocolException will be present somewhere in the inner exceptions.
                    // Its location depends on which method threw the exception.
                    while (requestException?.GetType().FullName.Equals("System.Net.Http.Http2ProtocolException") == false)
                    {
                        requestException = requestException.InnerException;
                    }

                    Assert.NotNull(requestException);
                    Assert.Contains("FLOW_CONTROL_ERROR", requestException.Message);
                },
                async server =>
                {
                    try
                    {
                        (Http2LoopbackConnection connection, SettingsFrame clientSettings) = await server.EstablishConnectionGetSettingsAsync();

                        SettingsEntry clientWindowSizeSetting = clientSettings.Entries.SingleOrDefault(x => x.SettingId == SettingId.InitialWindowSize);
                        int clientWindowSize = clientWindowSizeSetting.SettingId == SettingId.InitialWindowSize ? (int)clientWindowSizeSetting.Value : 65535;

                        // Exceed the window size by 1 byte.
                        ++clientWindowSize;

                        int streamId = await connection.ReadRequestHeaderAsync();

                        // Write the response.
                        await connection.SendDefaultResponseHeadersAsync(streamId);

                        byte[] buffer = new byte[4096];
                        int totalSent = 0;

                        while (totalSent < clientWindowSize)
                        {
                            int sendSize = Math.Min(buffer.Length, clientWindowSize - totalSent);
                            ReadOnlyMemory<byte> sendBuf = buffer.AsMemory(0, sendSize);

                            await connection.SendResponseDataAsync(streamId, sendBuf, endStream: false);
                            totalSent += sendSize;
                        }

                        // Try to read a frame. Should get null if connection reset or RST_STREAM if stream reset.
                        // If client is misbehaving, we'll get an OperationCanceledException due to timeout.
                        try
                        {
                            Frame clientFrame = await connection.ReadFrameAsync(TimeSpan.FromSeconds(5));
                            Assert.True(clientFrame == null || (clientFrame.Type == FrameType.RstStream && clientFrame.StreamId == streamId),
                                "Unexpected frame received from HttpClient; Expected either RST_STREAM or connection reset.");
                        }
                        catch (OperationCanceledException ex)
                        {
                            Assert.True(ex == null, "Stream unexpectedly left open by HttpClient; Expected either RST_STREAM or connection reset.");
                        }
                    }
                    finally
                    {
                        // Shut down client.
                        semaphore.Release();
                    }
                });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task MaxResponseHeadersLength_Exact_Success(bool huffmanEncode)
        {
            await Http2LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();
                    handler.MaxResponseHeadersLength = 1;

                    using HttpClient client = CreateHttpClient(handler);
                    using HttpResponseMessage response = await client.GetAsync(uri);
                },
                async server =>
                {
                    Http2LoopbackConnection con = await server.EstablishConnectionAsync();
                    int streamId = await con.ReadRequestHeaderAsync();

                    await con.SendResponseHeadersAsync(streamId, isTrailingHeader: true, headers: new[]
                    {
                        // 1000 + other strings = 1024
                        new HttpHeaderData(":status", "200", huffmanEncoded: huffmanEncode),
                        new HttpHeaderData("padding-header", new string(' ', 1000), huffmanEncoded: huffmanEncode)
                    });

                    await con.ShutdownIgnoringErrorsAsync(streamId);
                });
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task MaxResponseHeadersLength_Exceeded_Throws(bool huffmanEncode)
        {
            await Http2LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();
                    handler.MaxResponseHeadersLength = 1;

                    using HttpClient client = CreateHttpClient(handler);
                    await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(uri));
                },
                async server =>
                {
                    Http2LoopbackConnection con = await server.EstablishConnectionAsync();
                    int streamId = await con.ReadRequestHeaderAsync();

                    await con.SendResponseHeadersAsync(streamId, isTrailingHeader: true, headers: new[]
                    {
                        // 1001 + other strings = 1025
                        new HttpHeaderData(":status", "200", huffmanEncoded: huffmanEncode),
                        new HttpHeaderData("padding-header", new string(' ', 1001), huffmanEncoded: huffmanEncode)
                    });

                    await con.ShutdownIgnoringErrorsAsync(streamId);
                });
        }

        [Fact]
        public async Task MaxResponseHeadersLength_Malicious_Throws()
        {
            await Http2LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    HttpClientHandler handler = CreateHttpClientHandler();
                    handler.MaxResponseHeadersLength = 1;

                    using HttpClient client = CreateHttpClient(handler);
                    await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(uri));
                },
                async server =>
                {
                    Http2LoopbackConnection con = await server.EstablishConnectionAsync();
                    int streamId = await con.ReadRequestHeaderAsync();

                    // A small malicious/corrupt payload that expands into two 1GB strings. We don't want HPackDecoder to allocate buffers when they exceed MaxResponseHeadersLength.
                    byte[] headerData = new byte[] { 0x88, 0x00, 0x7F, 0x81, 0xFF, 0xFF, 0xFF, 0x03, 0x70, 0x6C, 0x61, 0x69, 0x6E, 0x2D, 0x74, 0x65, 0x78, 0x74, 0x7F, 0x81, 0xFF, 0xFF, 0xFF, 0x03, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61 };
                    HeadersFrame frame = new HeadersFrame(headerData, FrameFlags.EndHeaders | FrameFlags.EndStream, 0, 0, 0, streamId);

                    await con.WriteFrameAsync(frame);
                    await con.ShutdownIgnoringErrorsAsync(streamId);
                });
        }
    }
}
