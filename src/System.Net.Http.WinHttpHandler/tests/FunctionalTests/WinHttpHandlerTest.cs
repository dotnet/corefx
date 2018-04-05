// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

// Can't use "WinHttpHandler.Functional.Tests" in namespace as it won't compile.
// WinHttpHandler is a class and not a namespace and can't be part of namespace paths.
namespace System.Net.Http.WinHttpHandlerFunctional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    // Note:  Disposing the HttpClient object automatically disposes the handler within. So, it is not necessary
    // to separately Dispose (or have a 'using' statement) for the handler.
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "WinHttpHandler not supported on UAP")]
    public class WinHttpHandlerTest
    {
        // TODO: This is a placeholder until GitHub Issue #2383 gets resolved.
        private const string SlowServer = "http://httpbin.org/drip?numbytes=1&duration=1&delay=40&code=200";

        private readonly ITestOutputHelper _output;

        public WinHttpHandlerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void SendAsync_SimpleGet_Success()
        {
            var handler = new WinHttpHandler();
            using (var client = new HttpClient(handler))
            {
                // TODO: This is a placeholder until GitHub Issue #2383 gets resolved.
                var response = client.GetAsync(System.Net.Test.Common.Configuration.Http.RemoteEchoServer).Result;
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                _output.WriteLine(responseContent);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [InlineData(CookieUsePolicy.UseInternalCookieStoreOnly, "cookieName1", "cookieValue1")]
        [InlineData(CookieUsePolicy.UseSpecifiedCookieContainer, "cookieName2", "cookieValue2")]
        public async Task GetAsync_RedirectResponseHasCookie_CookieSentToFinalUri(
            CookieUsePolicy cookieUsePolicy,
            string cookieName,
            string cookieValue)
        {
            Uri uri = System.Net.Test.Common.Configuration.Http.RedirectUriForDestinationUri(false, 302, System.Net.Test.Common.Configuration.Http.RemoteEchoServer, 1);
            var handler = new WinHttpHandler();
            handler.WindowsProxyUsePolicy = WindowsProxyUsePolicy.UseWinInetProxy;
            handler.CookieUsePolicy = cookieUsePolicy;
            if (cookieUsePolicy == CookieUsePolicy.UseSpecifiedCookieContainer)
            {
                handler.CookieContainer = new CookieContainer();
            }

            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add(
                    "X-SetCookie",
                    string.Format("{0}={1};Path=/", cookieName, cookieValue));
                using (HttpResponseMessage httpResponse = await client.GetAsync(uri))
                {
                    string responseText = await httpResponse.Content.ReadAsStringAsync();
                    _output.WriteLine(responseText);
                    Assert.True(JsonMessageContainsKeyValue(responseText, cookieName, cookieValue));
                }
            }            
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        [OuterLoop]
        public async Task SendAsync_SlowServerAndCancel_ThrowsTaskCanceledException()
        {
            var handler = new WinHttpHandler();
            using (var client = new HttpClient(handler))
            {
                var cts = new CancellationTokenSource();
                Task<HttpResponseMessage> t = client.GetAsync(SlowServer, cts.Token);

                await Task.Delay(500);
                cts.Cancel();
                
                AggregateException ag = Assert.Throws<AggregateException>(() => t.Wait());
                Assert.IsType<TaskCanceledException>(ag.InnerException);
            }
        }        
        
        [ActiveIssue(17234)]
        [OuterLoop] // TODO: Issue #11345
        [Fact]
        [OuterLoop]
        public void SendAsync_SlowServerRespondsAfterDefaultReceiveTimeout_ThrowsHttpRequestException()
        {
            var handler = new WinHttpHandler();
            using (var client = new HttpClient(handler))
            {
                Task<HttpResponseMessage> t = client.GetAsync(SlowServer);
                
                AggregateException ag = Assert.Throws<AggregateException>(() => t.Wait());
                Assert.IsType<HttpRequestException>(ag.InnerException);
            }
        }

        [Fact]
        public async Task SendAsync_GetUsingChunkedEncoding_ThrowsHttpRequestException()
        {
            // WinHTTP doesn't support GET requests with a request body that uses
            // chunked encoding. This test pins this behavior and verifies that the
            // error handling is working correctly.
            var server = new Uri("http://www.microsoft.com"); // No network I/O actually happens.
            var request = new HttpRequestMessage(HttpMethod.Get, server);
            request.Content = new StringContent("Request body");
            request.Headers.TransferEncodingChunked = true;

            var handler = new WinHttpHandler();
            using (HttpClient client = new HttpClient(handler))
            {
                HttpRequestException ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(request));
                _output.WriteLine(ex.ToString());
            }
        }

        [OuterLoop]
        [Theory]
        //Take internationalized domain name 'Bangladesh' sample url from http://www.i18nguy.com/markup/idna-examples.html                     
        [InlineData("http://\u09AC\u09BE\u0982\u09B2\u09BE\u09A6\u09C7\u09B6.icom.museum", "icom.museum")]
        [InlineData("http://[::1234]", null)]
        [InlineData("http://[::1234]:8080", null)]
        [InlineData("http://127.0.0.1", null)]
        [InlineData("http://www.microsoft.com", "www.microsoft.com")]        
        public async Task ManualTest_IdnHostName(string requestUri, string requestHost)
        {
            var handler = new WinHttpHandler();
            using (HttpClient client = new HttpClient(handler))
            {
                try
                {
                    var response = await client.GetAsync(requestUri);
                    //We expect only not null requestHost sample reach this point
                    Assert.NotNull(requestHost);
                    Assert.Equal(requestHost, response.RequestMessage.RequestUri.Host);
                }
                catch (HttpRequestException ex)
                {
                    Assert.NotNull(ex.InnerException);
                    Assert.IsAssignableFrom<Win32Exception>(ex.InnerException);

                    //ERROR_INTERNET_INVALID_URL https://msdn.microsoft.com/en-us/library/windows/desktop/aa385465(v=vs.85).aspx
                    Assert.NotEqual(12005, ((Win32Exception)ex.InnerException).NativeErrorCode);

                    /*
                         We expect only connection attempt failed
                         ERROR_INTERNET_CANNOT_CONNECT https://msdn.microsoft.com/en-us/library/windows/desktop/aa385465(v=vs.85).aspx
                    */
                    Assert.Equal(12029, ((Win32Exception)ex.InnerException).NativeErrorCode);
                }
            }
        }

        public static bool JsonMessageContainsKeyValue(string message, string key, string value)
        {
            // TODO: Merge with System.Net.Http TestHelper class as part of GitHub Issue #4989.
            string pattern = string.Format(@"""{0}"": ""{1}""", key, value);
            return message.Contains(pattern);
        }      
    }
}
