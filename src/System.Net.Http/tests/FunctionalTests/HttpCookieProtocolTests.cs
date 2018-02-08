﻿// Licensed to the .NET Foundation under one or more agreements.
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
        //
        // Send cookie tests
        //

        private const string s_cookieName = "ABC";
        private const string s_cookieValue = "123";
        private const string s_expectedCookieHeaderValue = "ABC=123";
        private const string s_customCookieHeaderValue = "CustomCookie=456";

        private static CookieContainer CreateSingleCookieContainer(Uri uri)
        {
            var container = new CookieContainer();
            container.Add(uri, new Cookie(s_cookieName, s_cookieValue));
            return container;
        }

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

        [Fact]
        public async Task GetAsync_SetCookieContainer_CookieSent()
        {
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                handler.CookieContainer = CreateSingleCookieContainer(url);

                using (HttpClient client = new HttpClient(handler))
                {
                    Task<HttpResponseMessage> getResponseTask = client.GetAsync(url);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Contains($"Cookie: {s_expectedCookieHeaderValue}", requestLines);
                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                }
            });
        }

        [Fact]
        public async Task GetAsync_SetCookieContainerMultipleCookies_CookiesSent()
        {
            var cookies = new Cookie[]
            {
                new Cookie("hello", "world"),
                new Cookie("foo", "bar"),
                new Cookie("ABC", "123")
            };

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();

                var cookieContainer = new CookieContainer();
                foreach (Cookie c in cookies)
                {
                    cookieContainer.Add(url, c);
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

                    string expectedHeader = "Cookie: " + string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}").ToArray());

                    Assert.Contains(expectedHeader, requestLines);
                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                }
            });
        }

        [Fact]
        public async Task GetAsync_AddCookieHeader_CookieHeaderSent()
        {
            if (IsNetfxHandler)
            {
                // Netfx handler does not support custom cookie header
                return;
            }

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Cookie", s_customCookieHeaderValue);

                    foreach (var v in requestMessage.Headers.GetValues("Cookie"))
                        Console.WriteLine($"requestMessage Cookie header value: {v}");

                    Task<HttpResponseMessage> getResponseTask = client.SendAsync(requestMessage);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Contains($"Cookie: {s_customCookieHeaderValue}", requestLines);
                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie:")));
                }
            });
        }

        [Theory]
        [InlineData("ContainerCookie", "123")]
        public async Task GetAsync_SetCookieContainerAndCookieHeader_BothCookiesSent(string cookieName, string cookieValue)
        {
            if (IsNetfxHandler)
            {
                // Netfx handler does not support custom cookie header
                return;
            }

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                handler.CookieContainer = CreateSingleCookieContainer(url);

                using (HttpClient client = new HttpClient(handler))
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                    requestMessage.Headers.Add("Cookie", s_customCookieHeaderValue);

                    Task<HttpResponseMessage> getResponseTask = client.SendAsync(requestMessage);
                    Task<List<string>> serverTask = LoopbackServer.ReadRequestAndSendResponseAsync(server);
                    await TestHelper.WhenAllCompletedOrAnyFailed(getResponseTask, serverTask);

                    List<string> requestLines = await serverTask;

                    foreach (var s in requestLines)
                        Console.WriteLine(s);

                    Assert.Equal(1, requestLines.Count(s => s.StartsWith("Cookie: ")));

                    var cookies = requestLines.Single(s => s.StartsWith("Cookie: ")).Substring(8).Split(new string[] { "; " }, StringSplitOptions.None);
                    Assert.Contains(s_expectedCookieHeaderValue, cookies);
                    Assert.Contains(s_customCookieHeaderValue, cookies);
                    Assert.Equal(2, cookies.Count());
                }
            });
        }


    }
}
