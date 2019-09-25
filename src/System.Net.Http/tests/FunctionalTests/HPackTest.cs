// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net.Test.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Data;
using System.Runtime.InteropServices.ComTypes;

namespace System.Net.Http.Functional.Tests
{
    public class HPackTest : HttpClientHandlerTestBase
    {
        protected override bool UseSocketsHttpHandler => true;
        protected override bool UseHttp2 => true;

        public HPackTest(ITestOutputHelper output) : base(output)
        {
        }

        private const string LiteralHeaderName = "x-literal-header";
        private const string LiteralHeaderValue = "testing 456";

        [Theory]
        [MemberData(nameof(HeaderEncodingTestData))]
        public async Task HPack_HeaderEncoding(string headerName, string expectedValue, byte[] expectedEncoding)
        {
            await Http2LoopbackServer.CreateClientAndServerAsync(
                async uri =>
                {
                    using HttpClient client = CreateHttpClient();

                    using HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = HttpMethod.Post;
                    request.RequestUri = uri;
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent("testing 123");
                    request.Headers.Add(LiteralHeaderName, LiteralHeaderValue);

                    (await client.SendAsync(request)).Dispose();
                },
                async server =>
                {
                    Http2LoopbackConnection connection = await server.EstablishConnectionAsync();
                    (int streamId, HttpRequestData requestData) = await connection.ReadAndParseRequestHeaderAsync();

                    HttpHeaderData header = requestData.Headers.Single(x => x.Name == headerName);
                    Assert.Equal(expectedValue, header.Value);
                    Assert.True(expectedEncoding.AsSpan().SequenceEqual(header.Raw));

                    await connection.SendDefaultResponseAsync(streamId);
                });
        }

        public static IEnumerable<object[]> HeaderEncodingTestData()
        {
            // Indexed name, indexed value.
            yield return new object[] { ":method", "POST", new byte[] { 0x83 } };
            yield return new object[] { ":path", "/", new byte[] { 0x84 } };

            // Indexed name, literal value.
            byte[] data = new byte[512];
            int offset = HPackEncoder.EncodeHeader(31, "text/plain; charset=utf-8", HPackFlags.HuffmanEncodeValue, data);
            data = data.AsSpan(0, offset).ToArray();
            yield return new object[] { "content-type", "text/plain; charset=utf-8", data };

            // Literal name, literal value.
            data = new byte[512];
            offset = HPackEncoder.EncodeHeader(LiteralHeaderName, LiteralHeaderValue, HPackFlags.HuffmanEncode, data);
            data = data.AsSpan(0, offset).ToArray();
            yield return new object[] { LiteralHeaderName, LiteralHeaderValue, data };
        }
    }
}
