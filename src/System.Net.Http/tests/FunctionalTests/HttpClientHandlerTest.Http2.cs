// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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
            Http2LoopbackServer server = new Http2LoopbackServer(new Http2Options());
            HttpClientHandler handler = CreateHttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                await client.GetAsync(server.CreateServer())
            }
        }
    }
}