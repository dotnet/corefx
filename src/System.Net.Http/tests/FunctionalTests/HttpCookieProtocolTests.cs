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

// TODO: Check request after processing to see if the cookie header was added to the collection

namespace System.Net.Http.Functional.Tests
{
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
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

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
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                    Assert.Contains($"Cookie: {cookieName}={cookieValue}", requestLines);
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
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    string expectedHeader = "Cookie: " + string.Join("; ", cookies.Select(c => $"{c.Item1}={c.Item2}").ToArray());

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                    Assert.Contains(expectedHeader, requestLines);
                }
            });
        }

        [Theory]
        [InlineData("CustomCookie", "123")]
        public async Task GetAsync_AddCookieHeader_CookieHeaderSent(string cookieName, string cookieValue)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Cookie", $"{cookieName}={cookieValue}");

                    foreach (var v in requestMessage.Headers.GetValues("Cookie"))
                        Console.WriteLine($"requestMessage Cookie header value: {v}");

                    Task<HttpResponseMessage> getResponseTask = client.SendAsync(requestMessage);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                    Assert.Contains($"Cookie: {cookieName}={cookieValue}", requestLines);
                }
            });
        }

#if false
        // TODO: This is a bad test.

        [Theory]
        [InlineData("CustomCookie", "123")]
        public async Task GetAsync_AddMultipleCookieHeaders_OnlyLastCookieHeaderSent(string cookieName, string cookieValue)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Cookie", $"{cookieName}-1={cookieValue}-1");
                    requestMessage.Headers.Add("Cookie", $"{cookieName}-2={cookieValue}-2");

                    foreach (var v in requestMessage.Headers.GetValues("Cookie"))
                        Console.WriteLine($"requestMessage Cookie header value: {v}");

                    Task<HttpResponseMessage> getResponseTask = client.SendAsync(requestMessage);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                    Assert.Contains($"Cookie: {cookieName}-2={cookieValue}-2", requestLines);
                }
            });
        }
#endif

        [Theory]
        [InlineData("ContainerCookie", "123")]
        public async Task GetAsync_SetCookieContainerAndCookieHeader_BothCookiesSent(string cookieName, string cookieValue)
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(url, new Cookie(cookieName, cookieValue));
                handler.CookieContainer = cookieContainer;

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Cookie", "CustomCookie=456");

                    Task<HttpResponseMessage> getResponseTask = client.SendAsync(requestMessage);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie: ")));

                    var cookies = requestLines.Single(s => s.StartsWith("Cookie: ")).Substring(8).Split(new string[] { "; " }, StringSplitOptions.None);
                    Assert.Equal(2, cookies.Count());
                    Assert.Contains($"{cookieName}={cookieValue}", cookies);
                    Assert.Contains($"CustomCookie=456", cookies);
                }
            });
        }


    }
}
