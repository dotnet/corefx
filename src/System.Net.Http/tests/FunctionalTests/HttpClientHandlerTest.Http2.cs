// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    // Note:  Disposing the HttpClient object automatically disposes the handler within. So, it is not necessary
    // to separately Dispose (or have a 'using' statement) for the handler.
    public abstract class HttpClientHandler_Http2_Test : HttpClientTestBase
    {
        [Fact]
        public async Task Http2_ConnectPrefix_Sent()
        {
            Http2LoopbackServer server = new Http2LoopbackServer(new Http2LoopbackServer.Http2Options());
            SocketsHttpHandler handler = new SocketsHttpHandler();
            handler.MaxHttpVersion = HttpVersion.Version20;

            using (var client = new HttpClient(handler))
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, server.CreateServer());
                request.ProtocolVersion = HttpVersion.Version20;
                Task sendTask = client.SendAsync(request);

                await server.AcceptConnectionAsync().ConfigureAwait(false);

                await server.SendConnectionPrefaceAsync().ConfigureAwait(false);

                List<string> lines = await server.ReadInitialRequestHeadersAsync();

                Assert.True(lines.Contains("Connection: Upgrade, HTTP2-Settings\r\n"));
            }
        }
    }
}