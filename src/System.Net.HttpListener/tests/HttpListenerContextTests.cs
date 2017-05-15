// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Tests
{
    public class HttpListenerContextTests
    {
        public static bool IsNotWindows7 { get; } = !PlatformDetection.IsWindows7 && PlatformDetection.IsNotOneCoreUAP;

        public static IEnumerable<object[]> SubProtocol_TestData()
        {
            yield return new object[] { new string[0], null };

            yield return new object[] { new string[] { "MyProtocol1" }, null };
            yield return new object[] { new string[] { "MyProtocol" }, "MyProtocol" };
            yield return new object[] { new string[] { "MyProtocol" }, "myPROTOCOL" };

            yield return new object[] { new string[] { "MyProtocol1", "MyProtocol2" }, null };
            yield return new object[] { new string[] { "MyProtocol1", "MyProtocol2" }, "MyProtocol2" };
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [MemberData(nameof(SubProtocol_TestData))]
        public async Task AcceptWebSocketAsync_ValidSubProtocol_Success(string[] clientProtocols, string serverProtocol)
        {
            await GetWebSocketContext((socket, context) =>
            {
                HttpListenerWebSocketContext socketContext = context.AcceptWebSocketAsync(serverProtocol).Result;
                Assert.Equal(serverProtocol, socketContext.WebSocket.SubProtocol);
            }, subProtocols: clientProtocols);
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        // The managed implementation doesn't validate that the socket is actually a web socket.
        // This means that HttpListener can connect to a Socket masquerading as a web socket.
        [ActiveIssue(18128, TestPlatforms.AnyUnix)]
        public async Task AcceptWebSocketAsync_SocketSpoofingAsWebSocket_ThrowsWebSocketException()
        {
            await GetSocketContext(new string[] { "Connection: Upgrade", "Upgrade: websocket", "Sec-WebSocket-Version: 13", "Sec-WebSocket-Key: Key" }, context =>
            {
                Assert.ThrowsAsync<WebSocketException>(() => context.AcceptWebSocketAsync(null)).Wait();
            });
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        public async Task AcceptWebSocketAsync_UnsupportedProtocol_ThrowsWebSocketException()
        {
            await GetWebSocketContext((socket, context) =>
            {
                Assert.ThrowsAsync<WebSocketException>(() => context.AcceptWebSocketAsync("MyOtherProtocol")).Wait();
            }, subProtocols: new string[] { "MyProtocol" });
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData("Connection: ")]
        [InlineData("Connection: Connection\r\nUpgrade: ")]
        [InlineData("Connection: Test1\r\nUpgrade: Test2")]
        [InlineData("Connection: Upgrade\r\nUpgrade: Test2")]
        [InlineData("Connection: Upgrade\r\nUpgrade: websocket")]
        [InlineData("Connection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Version: ")]
        [InlineData("Connection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Version: 1")]
        [InlineData("Connection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Version: 13")]
        [InlineData("Connection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Version: 13\r\nSec-WebSocket-Key: ")]
        [InlineData("UnknownHeader: random")]
        public async Task AcceptWebSocketAsync_InvalidHeaders_ThrowsWebSocketException(string headers)
        {
            await GetSocketContext(new string[] { headers }, context =>
            {
                Assert.ThrowsAsync<WebSocketException>(() => context.AcceptWebSocketAsync(null)).Wait();
            });
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("random(text")]
        [InlineData("random)text")]
        [InlineData("random<text")]
        [InlineData("random>text")]
        [InlineData("random@text")]
        [InlineData("random,text")]
        [InlineData("random;text")]
        [InlineData("random:text")]
        [InlineData("random\\text")]
        [InlineData("random\"text")]
        [InlineData("random/text")]
        [InlineData("random[text")]
        [InlineData("random]text")]
        [InlineData("random?text")]
        [InlineData("random=text")]
        [InlineData("random{text")]
        [InlineData("random}text")]
        [InlineData("\x19")]
        [InlineData("\x7f")]
        public async Task AcceptWebSocketAsync_InvalidSubProtocol_ThrowsArgumentException(string subProtocol)
        {
            await GetWebSocketContext((socket, context) =>
            {
                Assert.ThrowsAsync<ArgumentException>("subProtocol", () => context.AcceptWebSocketAsync(subProtocol)).Wait();
            });
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData("!")]
        [InlineData("#")]
        [InlineData("YouDontKnowMe")]
        public async Task AcceptWebSocketAsync_NoSuchSubProtocol_ThrowsWebSocketException(string subProtocol)
        {
            await GetWebSocketContext((socket, context) =>
            {
                Assert.ThrowsAsync<WebSocketException>(() => context.AcceptWebSocketAsync(subProtocol)).Wait();
            });
        }

        [ConditionalFact(nameof(IsNotWindows7))]
        public async Task AcceptWebSocketAsync_InvalidKeepAlive_ThrowsWebSocketException()
        {
            await GetWebSocketContext((socket, context) =>
            {
                TimeSpan keepAlive = TimeSpan.FromMilliseconds(-2);
                Assert.ThrowsAsync<ArgumentOutOfRangeException>("keepAliveInterval", () => context.AcceptWebSocketAsync(null, keepAlive)).Wait();
            });
        }

        [ConditionalTheory(nameof(IsNotWindows7))]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(255)]
        [InlineData(64 * 1024 + 1)]
        public async Task AcceptWebSocketAsync_InvalidReceiveBufferSize_ThrowsWebSocketException(int receiveBufferSize)
        {
            await GetWebSocketContext((socket, context) =>
            {
                Assert.ThrowsAsync<ArgumentOutOfRangeException>("receiveBufferSize", () => context.AcceptWebSocketAsync(null, receiveBufferSize, TimeSpan.MaxValue)).Wait();
            });
        }

        private static async Task GetSocketContext(string[] headers, Action<HttpListenerContext> contextAction)
        {
            using (HttpListenerFactory factory = new HttpListenerFactory())
            using (Socket client = factory.GetConnectedSocket())
            {
                client.Send(factory.GetContent("1.1", "POST", null, "Text", headers, true));

                HttpListener listener = factory.GetListener();
                contextAction(await listener.GetContextAsync());
            }
        }

        private static async Task GetWebSocketContext(Action<ClientWebSocket, HttpListenerContext> contextAction, string[] subProtocols = null)
        {
            using (HttpListenerFactory factory = new HttpListenerFactory())
            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                if (subProtocols != null)
                {
                    foreach (string subProtocol in subProtocols)
                    {
                        clientWebSocket.Options.AddSubProtocol(subProtocol);
                    }
                }

                var uriBuilder = new UriBuilder(factory.ListeningUrl) { Scheme = "ws" };
                Task<HttpListenerContext> serverContextTask = factory.GetListener().GetContextAsync();

                Task clientConnectTask = clientWebSocket.ConnectAsync(uriBuilder.Uri, CancellationToken.None);
                contextAction(clientWebSocket, await serverContextTask);
            }
        }
    }
}
