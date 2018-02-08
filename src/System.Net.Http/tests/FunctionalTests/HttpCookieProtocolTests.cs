// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    // TODO: Add managed test class

    public class HttpCookieProtocolTests : HttpClientTestBase
    {
        [Fact]
        public async Task GetAsync_DefaultCoookieContainer_NoCookieSent()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                using (HttpClient client = CreateHttpClient())
                {
                    Task<HttpResponseMessage> getResponseTask = client.GetAsync(url);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    Assert.Equal(0, requestLines.Count(s => s.StartsWith("Cookie:")));
                }
            });
        }

        [Theory]     
        [InlineData("cookieName1", "cookieValue1")]
        [InlineData("ABC", "XYZ")]
        public async Task GetAsync_SetCookieContainer_CookieSent(string cookieName, string cookieValue)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(url, new Cookie(cookieName, cookieValue));
                handler.CookieContainer = cookieContainer;

                using (HttpClient client = new HttpClient(handler))
                {
                    Task<HttpResponseMessage> getResponseTask = client.GetAsync(url);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);
                    
                    Assert.Equal(1, requestLines.Count(s => s == $"Cookie: {cookieName}={cookieValue}"));
                }
            });
        }

        [Fact]
        public async Task GetAsync_SetCookieContainerMultipleCookies_CookiesSent()
        {
            List<(string, string)> cookies = new List<(string, string)>()
            {
                ("hello", "world"),
                ("foo", "bar"),
                ("ABC", "123")
            };

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();

                var cookieContainer = new CookieContainer();
                foreach ((string cookieName, string cookieValue) in cookies)
                {
                    cookieContainer.Add(url, new Cookie(cookieName, cookieValue));
                }

                handler.CookieContainer = cookieContainer;

                using (HttpClient client = new HttpClient(handler))
                {
                    Task<HttpResponseMessage> getResponseTask = client.GetAsync(url);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    string expectedHeader = string.Join("; ", cookies.Select(c => $"{c.Item1}={c.Item2}").ToArray());

                    //                    Console.WriteLine("Expected: ")
                    Assert.Equal(1, requestLines.Count(s => s == $"Cookie: {expectedHeader}"));
                }
            });
        }
    }
}
