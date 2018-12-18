// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Test.Common;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public sealed class SocketsHttpHandler_HttpClientHandler_Http2_Test : HttpClientTestBase
    {
        protected override bool UseSocketsHttpHandler => true;
        public static bool SupportsAlpn => PlatformDetection.SupportsAlpn;

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_ClientPreface_Sent()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                string connectionPreface = await server.AcceptConnectionAsync();

                Assert.Equal("PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n", connectionPreface);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_InitialSettings_SentAndAcked()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.AcceptConnectionAsync();

                // Receive the initial client settings frame.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.Settings, receivedFrame.Type);

                // Send the initial server settings frame.
                Frame emptySettings = new Frame(0, FrameType.Settings, FrameFlags.None, 0);
                await server.WriteFrameAsync(emptySettings).ConfigureAwait(false);

                // Receive the server settings frame ACK.
                // This doesn't have to be the next frame, as the client is allowed to send before receiving our SETTINGS frame.
                // So, loop until we see it (or the timeout expires)
                while (true)
                {
                    receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    if (receivedFrame.Type == FrameType.Settings && receivedFrame.AckFlag)
                    {
                        break;
                    }
                }
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_DataSentBeforeServerPreface_ProtocolError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.AcceptConnectionAsync();

                // Send a frame despite not having sent the server connection preface.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.Padded, 10, 1);
                await server.WriteFrameAsync(invalidFrame);

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_ServerSendsValidSettingsValues_Success()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task<HttpResponseMessage> sendTask = client.GetAsync(server.Address);

                // Send a bunch of valid SETTINGS values (that won't interfere with processing requests)
                await server.EstablishConnectionAsync(
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

                int streamId = await server.ReadRequestHeaderAsync();

                await server.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await sendTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [ConditionalTheory(nameof(SupportsAlpn))]
        [InlineData(SettingId.MaxFrameSize, 16383)]
        [InlineData(SettingId.MaxFrameSize, 162777216)]
        [InlineData(SettingId.InitialWindowSize, 0x80000000)]
        public async Task Http2_ServerSendsInvalidSettingsValue_ProtocolError(SettingId settingId, uint value)
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                // Send invalid initial SETTINGS value
                await server.EstablishConnectionAsync(new SettingsEntry { SettingId = settingId, Value = value });

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_StreamResetByServer_RequestFails()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.EstablishConnectionAsync();
                await server.ReadRequestHeaderAsync();

                // Send a reset stream frame so that stream 1 moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.Padded, 0x1, 1);
                await server.WriteFrameAsync(resetStream);

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task DataFrame_NoStream_ConnectionError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.EstablishConnectionAsync();
                await server.ReadRequestHeaderAsync();

                // Send a malformed frame (streamId is 0)
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.None, 0, 0);
                await server.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task DataFrame_TooLong_ConnectionError()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.EstablishConnectionAsync();
                await server.ReadRequestHeaderAsync();

                // Send a malformed frame.
                DataFrame invalidFrame = new DataFrame(new byte[Frame.MaxFrameLength + 1], FrameFlags.None, 0, 0);
                await server.WriteFrameAsync(invalidFrame);

                // As this is a connection level error, the client should see the request fail.
                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        [ActiveIssue(31514)]
        public async Task ClosedStream_FrameReceived_ResetsStream()
        {
            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task sendTask = client.GetAsync(server.Address);

                await server.EstablishConnectionAsync();
                await server.ReadRequestHeaderAsync();

                // Send a reset stream frame so that stream 1 moves to a terminal state.
                RstStreamFrame resetStream = new RstStreamFrame(FrameFlags.Padded, 0x1, 1);
                await server.WriteFrameAsync(resetStream);

                // Send a frame on the now-closed stream.
                DataFrame invalidFrame = new DataFrame(new byte[10], FrameFlags.Padded, 10, 1);
                await server.WriteFrameAsync(invalidFrame);

                // Receive a RST_STREAM frame.
                Frame receivedFrame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                Assert.Equal(FrameType.RstStream, receivedFrame.Type);

                await Assert.ThrowsAsync<HttpRequestException>(async () => await sendTask);
            }
        }

        [ConditionalFact(nameof(SupportsAlpn))]
        public async Task Http2_FlowControl_ClientDoesNotExceedWindows()
        {
            const int InitialWindowSize = 65535;
            const int ContentSize = 100_000;

            HttpClientHandler handler = CreateHttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = TestHelper.AllowAllCertificates;
            TestHelper.EnsureHttp2Feature(handler);

            var content = new ByteArrayContent(TestHelper.GenerateRandomContent(ContentSize));

            using (var server = Http2LoopbackServer.CreateServer())
            using (var client = new HttpClient(handler))
            {
                Task<HttpResponseMessage> clientTask = client.PostAsync(server.Address, content);

                await server.EstablishConnectionAsync();

                Frame frame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                int streamId = frame.StreamId;
                Assert.Equal(FrameType.Headers, frame.Type);
                Assert.Equal(FrameFlags.EndHeaders, frame.Flags);

                // Receive up to initial window size
                int bytesReceived = 0;
                while (bytesReceived < InitialWindowSize)
                {
                    frame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    Assert.Equal(streamId, frame.StreamId);
                    Assert.Equal(FrameType.Data, frame.Type);
                    Assert.Equal(FrameFlags.None, frame.Flags);
                    Assert.True(frame.Length > 0);

                    bytesReceived += frame.Length;
                }

                Assert.Equal(InitialWindowSize, bytesReceived);

                // Issue another read. It shouldn't complete yet. Wait a brief period of time to ensure it doesn't complete.
                Task<Frame> readFrameTask = server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window by one. This should still not complete the read.
                await server.WriteFrameAsync(new WindowUpdateFrame(1, 0));

                await Task.Delay(500);

                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window by two. This should complete the read with a single byte.
                await server.WriteFrameAsync(new WindowUpdateFrame(2, streamId));

                frame = await readFrameTask;
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window by two. This should complete the read with a single byte.
                await server.WriteFrameAsync(new WindowUpdateFrame(2, 0));

                frame = await readFrameTask;
                Assert.Equal(1, frame.Length);
                bytesReceived++;

                // Issue another read and ensure it doesn't complete yet.
                readFrameTask = server.ReadFrameAsync(TimeSpan.FromSeconds(30));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase connection window to allow exactly the remaining request size. This should still not complete the read.
                await server.WriteFrameAsync(new WindowUpdateFrame(ContentSize - bytesReceived - 1, 0));

                await Task.Delay(500);
                Assert.False(readFrameTask.IsCompleted);

                // Increase stream window to allow exactly the remaining request size. This should allow the rest of the request to be sent.
                await server.WriteFrameAsync(new WindowUpdateFrame(ContentSize - bytesReceived, streamId));

                frame = await readFrameTask;
                Assert.Equal(streamId, frame.StreamId);
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(FrameFlags.None, frame.Flags);
                Assert.True(frame.Length > 0);

                bytesReceived += frame.Length;

                // Read to end of stream
                while (true)
                {
                    frame = await server.ReadFrameAsync(TimeSpan.FromSeconds(30));
                    if (frame.EndStreamFlag)
                    {
                        break;
                    }

                    Assert.Equal(streamId, frame.StreamId);
                    Assert.Equal(FrameType.Data, frame.Type);
                    Assert.Equal(FrameFlags.None, frame.Flags);
                    Assert.True(frame.Length > 0);

                    bytesReceived += frame.Length;
                }

                Assert.Equal(ContentSize, bytesReceived);

                // Verify EndStream frame
                Assert.Equal(streamId, frame.StreamId);
                Assert.Equal(FrameType.Data, frame.Type);
                Assert.Equal(FrameFlags.EndStream, frame.Flags);
                Assert.True(frame.Length == 0);

                await server.SendDefaultResponseAsync(streamId);

                HttpResponseMessage response = await clientTask;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
