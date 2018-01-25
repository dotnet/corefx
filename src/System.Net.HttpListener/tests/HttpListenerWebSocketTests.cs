﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Tests
{
    public class HttpListenerWebSocketTests : IDisposable
    {
        public static bool PartialMessagesSupported => PlatformDetection.ClientWebSocketPartialMessagesSupported;
        public static bool IsNotWindows7 { get; } = !PlatformDetection.IsWindows7;
        public static bool IsNotWindows7AndIsWindowsImplementation => IsNotWindows7 && Helpers.IsWindowsImplementation;

        private HttpListenerFactory Factory { get; }
        private HttpListener Listener { get; }
        private ClientWebSocket Client { get; }
        private Task ClientConnectTask { get; set; }

        public HttpListenerWebSocketTests()
        {
            Factory = new HttpListenerFactory();
            Listener = Factory.GetListener();
            Client = new ClientWebSocket();
        }

        public void Dispose()
        {
            Factory.Dispose();
            Client.Dispose();
        }

        [ConditionalTheory(nameof(PartialMessagesSupported), nameof(IsNotWindows7))]
        [InlineData(WebSocketMessageType.Text, false)]
        [InlineData(WebSocketMessageType.Binary, false)]
        [InlineData(WebSocketMessageType.Text, true)]
        [InlineData(WebSocketMessageType.Binary, true)]
        public async Task SendAsync_SendWholeBuffer_Success(WebSocketMessageType messageType, bool endOfMessage)
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            const string Text = "Hello Web Socket";
            byte[] sentBytes = Encoding.ASCII.GetBytes(Text);

            await context.WebSocket.SendAsync(new ArraySegment<byte>(sentBytes), messageType, endOfMessage, new CancellationToken());

            byte[] receivedBytes = new byte[sentBytes.Length];
            WebSocketReceiveResult result = await ReceiveAllAsync(Client, receivedBytes.Length, receivedBytes);
            Assert.Equal(messageType, result.MessageType);
            Assert.Equal(endOfMessage, result.EndOfMessage);
            Assert.Null(result.CloseStatus);
            Assert.Null(result.CloseStatusDescription);

            Assert.Equal(Text, Encoding.ASCII.GetString(receivedBytes));
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        public async Task SendAsync_NoInnerBuffer_ThrowsArgumentNullException()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await AssertExtensions.ThrowsAsync<ArgumentNullException>("buffer.Array", () => context.WebSocket.SendAsync(new ArraySegment<byte>(), WebSocketMessageType.Text, false, new CancellationToken()));
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData(WebSocketMessageType.Close)]
        [InlineData(WebSocketMessageType.Text - 1)]
        [InlineData(WebSocketMessageType.Binary + 1)]
        public async Task SendAsync_InvalidMessageType_ThrowsArgumentNullException(WebSocketMessageType messageType)
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await AssertExtensions.ThrowsAsync<ArgumentException>("messageType", () => context.WebSocket.SendAsync(new ArraySegment<byte>(), messageType, false, new CancellationToken()));
        }

        [ConditionalFact(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20395, TestPlatforms.AnyUnix)]
        public async Task SendAsync_Disposed_ThrowsObjectDisposedException()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            context.WebSocket.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => context.WebSocket.SendAsync(new ArraySegment<byte>(new byte[10]), WebSocketMessageType.Text, false, new CancellationToken()));
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "WebSocket partial send is not supported on UAP. (#22053)")]
        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData(WebSocketMessageType.Text, false)]
        [InlineData(WebSocketMessageType.Binary, false)]
        [InlineData(WebSocketMessageType.Text, true)]
        [InlineData(WebSocketMessageType.Binary, true)]
        public async Task ReceiveAsync_ReadWholeBuffer_Success(WebSocketMessageType messageType, bool endOfMessage)
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            const string Text = "Hello Web Socket";
            byte[] sentBytes = Encoding.ASCII.GetBytes(Text);

            await Client.SendAsync(new ArraySegment<byte>(sentBytes), messageType, endOfMessage, new CancellationToken());

            byte[] receivedBytes = new byte[sentBytes.Length];
            WebSocketReceiveResult result = await ReceiveAllAsync(context.WebSocket, receivedBytes.Length, receivedBytes);
            Assert.Equal(messageType, result.MessageType);
            Assert.Equal(endOfMessage, result.EndOfMessage);
            Assert.Null(result.CloseStatus);
            Assert.Null(result.CloseStatusDescription);

            Assert.Equal(Text, Encoding.ASCII.GetString(receivedBytes));
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        public async Task ReceiveAsync_NoInnerBuffer_ThrowsArgumentNullException()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            await AssertExtensions.ThrowsAsync<ArgumentNullException>("buffer.Array", () => context.WebSocket.ReceiveAsync(new ArraySegment<byte>(), new CancellationToken()));
        }

        [ConditionalFact(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20395, TestPlatforms.AnyUnix)]
        public async Task ReceiveAsync_Disposed_ThrowsObjectDisposedException()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            context.WebSocket.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => context.WebSocket.ReceiveAsync(new ArraySegment<byte>(new byte[10]), new CancellationToken()));
        }

        public static IEnumerable<object[]> CloseStatus_Valid_TestData()
        {
            yield return new object[] { WebSocketCloseStatus.EndpointUnavailable, "", WebSocketCloseStatus.EndpointUnavailable };
            yield return new object[] { WebSocketCloseStatus.MandatoryExtension, "StatusDescription", WebSocketCloseStatus.MandatoryExtension };
        }

        [ConditionalTheory(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20396, TestPlatforms.AnyUnix)]
        [MemberData(nameof(CloseStatus_Valid_TestData))]
        public async Task CloseOutputAsync_HandshakeStartedFromClient_Success(WebSocketCloseStatus status, string statusDescription, WebSocketCloseStatus expectedCloseStatus)
        {
            // [ActiveIssue(20392, TargetFrameworkMonikers.Netcoreapp)]
            string expectedStatusDescription = statusDescription;
            if (!PlatformDetection.IsFullFramework && statusDescription == null)
            {
                expectedStatusDescription = string.Empty;
            }

            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            // Close the server output.
            Task serverCloseTask = context.WebSocket.CloseOutputAsync(status, statusDescription, new CancellationToken());
            byte[] receivedClientBytes = new byte[10];
            Task<WebSocketReceiveResult> clientReceiveTask = Client.ReceiveAsync(new ArraySegment<byte>(receivedClientBytes), new CancellationToken());

            await Task.WhenAll(serverCloseTask, clientReceiveTask);

            WebSocketReceiveResult clientResult = await clientReceiveTask;
            Assert.Equal(new byte[10], receivedClientBytes);
            Assert.Equal(expectedCloseStatus, clientResult.CloseStatus);

            Assert.Equal(expectedStatusDescription, clientResult.CloseStatusDescription);
            Assert.Equal(WebSocketMessageType.Close, clientResult.MessageType);
            Assert.True(clientResult.EndOfMessage);

            Assert.Null(context.WebSocket.CloseStatus);
            Assert.Null(context.WebSocket.CloseStatusDescription);
            Assert.Equal(WebSocketState.CloseSent, context.WebSocket.State);

            // Trying to send if the socket initiated a close should fail.
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.SendAsync(new ArraySegment<byte>(new byte[10]), WebSocketMessageType.Binary, false, new CancellationToken()));

            // Close the client.
            Task clientCloseTask = Client.CloseAsync(status, statusDescription, new CancellationToken());
            byte[] receivedServerBytes = new byte[10];
            Task<WebSocketReceiveResult> serverReceiveTask = context.WebSocket.ReceiveAsync(new ArraySegment<byte>(receivedServerBytes), new CancellationToken());

            await Task.WhenAll(clientCloseTask, serverReceiveTask);

            WebSocketReceiveResult serverResult = await clientReceiveTask;
            Assert.Equal(new byte[10], receivedServerBytes);
            Assert.Equal(expectedCloseStatus, serverResult.CloseStatus);
            Assert.Equal(expectedStatusDescription, serverResult.CloseStatusDescription);
            Assert.Equal(WebSocketMessageType.Close, serverResult.MessageType);
            Assert.True(serverResult.EndOfMessage);

            Assert.Equal(expectedCloseStatus, context.WebSocket.CloseStatus);
            Assert.Equal(statusDescription, context.WebSocket.CloseStatusDescription);
            Assert.Equal(WebSocketState.Closed, context.WebSocket.State);

            // Trying to read or write if closed should fail.
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.ReceiveAsync(new ArraySegment<byte>(receivedServerBytes), new CancellationToken()));
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.SendAsync(new ArraySegment<byte>(receivedServerBytes), WebSocketMessageType.Binary, false, new CancellationToken()));

            // Trying to close again should be a nop.
            await context.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
            await context.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
        }

        [ConditionalTheory(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20396, TestPlatforms.AnyUnix)]
        [MemberData(nameof(CloseStatus_Valid_TestData))]
        public async Task CloseAsync_HandshakeStartedFromClient_Success(WebSocketCloseStatus status, string statusDescription, WebSocketCloseStatus expectedCloseStatus)
        {
            // [ActiveIssue(20392, TargetFrameworkMonikers.Netcoreapp)]
            string expectedStatusDescription = statusDescription;
            if (!PlatformDetection.IsFullFramework && statusDescription == null)
            {
                expectedStatusDescription = string.Empty;
            }

            HttpListenerWebSocketContext context = await GetWebSocketContext();
            await ClientConnectTask;

            // Close the client output.
            Task clientCloseTask = Client.CloseOutputAsync(status, statusDescription, new CancellationToken());
            byte[] receivedServerBytes = new byte[10];
            Task<WebSocketReceiveResult> serverReceiveTask = context.WebSocket.ReceiveAsync(new ArraySegment<byte>(receivedServerBytes), new CancellationToken());

            await Task.WhenAll(clientCloseTask, serverReceiveTask);

            WebSocketReceiveResult serverResult = await serverReceiveTask;
            Assert.Equal(new byte[10], receivedServerBytes);
            Assert.Equal(expectedCloseStatus, serverResult.CloseStatus);
            Assert.Equal(statusDescription, serverResult.CloseStatusDescription);
            Assert.Equal(WebSocketMessageType.Close, serverResult.MessageType);
            Assert.True(serverResult.EndOfMessage);

            Assert.Equal(expectedCloseStatus, context.WebSocket.CloseStatus);
            Assert.Equal(statusDescription, context.WebSocket.CloseStatusDescription);
            Assert.Equal(WebSocketState.CloseReceived, context.WebSocket.State);

            // Trying to read if the server received a close handshake should fail.
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.ReceiveAsync(new ArraySegment<byte>(receivedServerBytes), new CancellationToken()));

            // Close the server.
            Task serverCloseTask = context.WebSocket.CloseAsync(status, statusDescription, new CancellationToken());

            byte[] receivedClientBytes = new byte[10];
            Task<WebSocketReceiveResult> clientReceiveTask = Client.ReceiveAsync(new ArraySegment<byte>(receivedClientBytes), new CancellationToken());

            await Task.WhenAll(serverCloseTask, clientReceiveTask);

            WebSocketReceiveResult clientResult = await clientReceiveTask;
            Assert.Equal(new byte[10], receivedClientBytes);
            Assert.Equal(expectedCloseStatus, clientResult.CloseStatus);
            Assert.Equal(expectedStatusDescription, clientResult.CloseStatusDescription);
            Assert.Equal(WebSocketMessageType.Close, clientResult.MessageType);
            Assert.True(clientResult.EndOfMessage);

            Assert.Equal(expectedCloseStatus, context.WebSocket.CloseStatus);
            Assert.Equal(statusDescription, context.WebSocket.CloseStatusDescription);
            Assert.Equal(WebSocketState.Closed, context.WebSocket.State);

            // Trying to read or write if closed should fail.
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.ReceiveAsync(new ArraySegment<byte>(receivedServerBytes), new CancellationToken()));
            await Assert.ThrowsAsync<WebSocketException>(() => context.WebSocket.SendAsync(new ArraySegment<byte>(receivedServerBytes), WebSocketMessageType.Binary, false, new CancellationToken()));

            // Trying to close again should be a nop.
            await context.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
            await context.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
        }

        public static IEnumerable<object[]> CloseStatus_Invalid_TestData()
        {
            yield return new object[] { WebSocketCloseStatus.Empty, "StatusDescription", "statusDescription" };
            yield return new object[] { WebSocketCloseStatus.EndpointUnavailable, new string('a', 124), "statusDescription" };
            yield return new object[] { (WebSocketCloseStatus)1006, null, "closeStatus" };
            yield return new object[] { (WebSocketCloseStatus)0, null, "closeStatus" };
            yield return new object[] { (WebSocketCloseStatus)999, null, "closeStatus" };
            yield return new object[] { (WebSocketCloseStatus)1015, null, "closeStatus" };
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [MemberData(nameof(CloseStatus_Invalid_TestData))]
        public async Task CloseAsync_InvalidCloseStatus_ThrowsArgumentException(WebSocketCloseStatus status, string statusDescription, string paramName)
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();

            await Assert.ThrowsAsync<ArgumentException>(paramName, () => context.WebSocket.CloseAsync(status, statusDescription, new CancellationToken()));
            await Assert.ThrowsAsync<ArgumentException>(paramName, () => context.WebSocket.CloseOutputAsync(status, statusDescription, new CancellationToken()));
        }

        [ConditionalFact(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20394, TestPlatforms.AnyUnix)]
        public async Task CloseAsync_AfterDisposed_Nop()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            context.WebSocket.Dispose();

            await context.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
            await context.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
        }

        [ConditionalFact(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20394, TestPlatforms.AnyUnix)]
        public async Task CloseAsync_AfterAborted_Nop()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            context.WebSocket.Abort();

            await context.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
            await context.WebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, new CancellationToken());
        }

        [ConditionalFact(nameof(IsNotWindows7AndIsWindowsImplementation))] // [ActiveIssue(20397, TestPlatforms.AnyUnix)]
        public async Task Dispose_CallAfterDisposed_Nop()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            context.WebSocket.Dispose();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);

            context.WebSocket.Dispose();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);

            context.WebSocket.Abort();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        public async Task Abort_CallAfterAborted_Nop()
        {
            HttpListenerWebSocketContext context = await GetWebSocketContext();
            context.WebSocket.Abort();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);

            context.WebSocket.Abort();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);

            context.WebSocket.Dispose();
            Assert.Equal(WebSocketState.Aborted, context.WebSocket.State);
        }

        private static async Task<WebSocketReceiveResult> ReceiveAllAsync(WebSocket webSocket, int expectedBytes, byte[] buffer)
        {
            int totalReceived = 0;
            WebSocketReceiveResult result = default(WebSocketReceiveResult);
            while (totalReceived < expectedBytes)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                totalReceived += result.Count;
            }
            return new WebSocketReceiveResult(totalReceived, result.MessageType, result.EndOfMessage);
        }

        private async Task<HttpListenerWebSocketContext> GetWebSocketContext(string[] subProtocols = null)
        {
            if (subProtocols != null)
            {
                foreach (string subProtocol in subProtocols)
                {
                    Client.Options.AddSubProtocol(subProtocol);
                }
            }

            var uriBuilder = new UriBuilder(Factory.ListeningUrl) { Scheme = "ws" };
            Task<HttpListenerContext> serverContextTask = Factory.GetListener().GetContextAsync();

            ClientConnectTask = Client.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
            if (ClientConnectTask == await Task.WhenAny(serverContextTask, ClientConnectTask))
            {
                await ClientConnectTask;
                Assert.True(false, "Client should not have completed prior to server sending response");
            }

            HttpListenerContext context = await serverContextTask;
            return await context.AcceptWebSocketAsync(null);
        }
    }
}
