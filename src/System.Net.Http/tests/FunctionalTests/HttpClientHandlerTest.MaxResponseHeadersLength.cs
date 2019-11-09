// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Test.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public abstract class HttpClientHandler_MaxResponseHeadersLength_Test : HttpClientHandlerTestBase
    {
        public HttpClientHandler_MaxResponseHeadersLength_Test(ITestOutputHelper output) : base(output) { }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void InvalidValue_ThrowsException(int invalidValue)
        {
            using (HttpClientHandler handler = CreateHttpClientHandler())
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>("value", () => handler.MaxResponseHeadersLength = invalidValue);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(65)]
        [InlineData(int.MaxValue)]
        public void ValidValue_SetGet_Roundtrips(int validValue)
        {
            using (HttpClientHandler handler = CreateHttpClientHandler())
            {
                handler.MaxResponseHeadersLength = validValue;
                Assert.Equal(validValue, handler.MaxResponseHeadersLength);
            }
        }

        [Fact]
        public async Task SetAfterUse_Throws()
        {
            await LoopbackServerFactory.CreateClientAndServerAsync(async uri =>
            {
                using HttpClientHandler handler = CreateHttpClientHandler();
                using HttpClient client = CreateHttpClient(handler);

                handler.MaxResponseHeadersLength = 1;
                (await client.GetStreamAsync(uri)).Dispose();
                Assert.Throws<InvalidOperationException>(() => handler.MaxResponseHeadersLength = 1);
            },
            server => server.AcceptConnectionSendResponseAndCloseAsync());
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public async Task InfiniteSingleHeader_ThrowsException()
        {
            if (IsCurlHandler)
            {
                // libcurl fails with an out of memory error
                return;
            }

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (HttpClient client = CreateHttpClient(handler))
                {
                    Task<HttpResponseMessage> getAsync = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    await server.AcceptConnectionAsync(async connection =>
                    {
                        var cts = new CancellationTokenSource();
                        Task serverTask = Task.Run(async delegate
                        {
                            await connection.ReadRequestHeaderAndSendCustomResponseAsync("HTTP/1.1 200 OK\r\nContent-Length: 0\r\nMyInfiniteHeader: ");
                            try
                            {
                                while (!cts.IsCancellationRequested)
                                {
                                    await connection.Writer.WriteAsync(new string('s', 16000));
                                    await Task.Delay(1);
                                }
                            }
                            catch { }
                        });

                        Exception e = await Assert.ThrowsAsync<HttpRequestException>(() => getAsync);
                        cts.Cancel();
                        if (UseSocketsHttpHandler)
                        {
                            Assert.Contains((handler.MaxResponseHeadersLength * 1024).ToString(), e.ToString());
                        }
                        await serverTask;
                    });
                }
            });
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory, MemberData(nameof(ResponseWithManyHeadersData))]
        public async Task ThresholdExceeded_ThrowsException(string responseHeaders, int? maxResponseHeadersLength, bool shouldSucceed)
        {
            if (IsCurlHandler)
            {
                // libcurl often fails with out of memory errors
                return;
            }

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (HttpClient client = CreateHttpClient(handler))
                {
                    if (maxResponseHeadersLength.HasValue)
                    {
                        handler.MaxResponseHeadersLength = maxResponseHeadersLength.Value;
                    }
                    Task<HttpResponseMessage> getAsync = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                    await server.AcceptConnectionAsync(async connection =>
                    {
                        Task serverTask = connection.ReadRequestHeaderAndSendCustomResponseAsync(responseHeaders);

                        if (shouldSucceed)
                        {
                            (await getAsync).Dispose();
                            await serverTask;
                        }
                        else
                        {
                            Exception e = await Assert.ThrowsAsync<HttpRequestException>(() => getAsync);
                            if (UseSocketsHttpHandler)
                            {
                                Assert.Contains((handler.MaxResponseHeadersLength * 1024).ToString(), e.ToString());
                            }
                            try { await serverTask; } catch { }
                        }
                    });
                }
            });
        }

        public static IEnumerable<object[]> ResponseWithManyHeadersData
        {
            get
            {
                foreach (int? max in new int?[] { null, 1, 31, 128 })
                {
                    int actualSize = max.HasValue ? max.Value : 64;

                    yield return new object[] { GenerateLargeResponseHeaders(actualSize * 1024 - 1), max, true }; // Small enough
                    yield return new object[] { GenerateLargeResponseHeaders(actualSize * 1024), max, true }; // Just right
                    yield return new object[] { GenerateLargeResponseHeaders(actualSize * 1024 + 1), max, false }; // Too big
                }
            }
        }

        private static string GenerateLargeResponseHeaders(int responseHeadersSizeInBytes)
        {
            var buffer = new StringBuilder();
            buffer.Append("HTTP/1.1 200 OK\r\n");
            buffer.Append("Content-Length: 0\r\n");
            for (int i = 0; i < 24; i++)
            {
                buffer.Append($"Custom-{i:D4}: 1234567890123456789012345\r\n");
            }
            buffer.Append($"Custom-24: ");
            buffer.Append(new string('c', responseHeadersSizeInBytes - (buffer.Length + 4)));
            buffer.Append("\r\n\r\n");

            string response = buffer.ToString();
            Assert.Equal(responseHeadersSizeInBytes, response.Length);
            return response;
        }
    }
}
