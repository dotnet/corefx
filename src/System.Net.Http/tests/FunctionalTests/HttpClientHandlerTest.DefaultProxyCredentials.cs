// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class HttpClientHandler_DefaultProxyCredentials_Test : HttpClientTestBase
    {
        [Fact]
        public void Default_Get_Null()
        {
            using (HttpClientHandler handler = CreateHttpClientHandler())
            {
                Assert.Null(handler.DefaultProxyCredentials);
            }
        }

        [Fact]
        public void SetGet_Roundtrips()
        {
            using (HttpClientHandler handler = CreateHttpClientHandler())
            {
                var creds = new NetworkCredential("username", "password", "domain");

                handler.DefaultProxyCredentials = null;
                Assert.Null(handler.DefaultProxyCredentials);

                handler.DefaultProxyCredentials = creds;
                Assert.Same(creds, handler.DefaultProxyCredentials);

                handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                Assert.Same(CredentialCache.DefaultCredentials, handler.DefaultProxyCredentials);
            }
        }

        [ActiveIssue(20010, TargetFrameworkMonikers.Uap)]
        [Fact]
        public async Task ProxyExplicitlyProvided_DefaultCredentials_Ignored()
        {
            var rightCreds = new NetworkCredential("rightusername", "rightpassword");
            var wrongCreds = new NetworkCredential("wrongusername", "wrongpassword");
            string expectCreds = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{rightCreds.UserName}:{rightCreds.Password}"));

            await LoopbackServer.CreateServerAsync(async (proxyServer, proxyUrl) =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    handler.Proxy = new UseSpecifiedUriWebProxy(proxyUrl, rightCreds);
                    handler.DefaultProxyCredentials = wrongCreds;

                    Task<HttpResponseMessage> responseTask = client.GetAsync(Configuration.Http.RemoteEchoServer);
                    await proxyServer.AcceptConnectionAsync(async connection =>
                    {
                        await connection.ReadRequestHeaderAsync().ConfigureAwait(false);
                        if (connection.GetRequestHeaderValue("Proxy-Authorization") == null)
                        {
                            await connection.SendResponseAsync(HttpStatusCode.ProxyAuthenticationRequired, "Proxy-Authenticate: Basic\r\n");
                            lines = await connection.ReadRequestHeaderAsync().ConfigureAwait(false);
                        }
                        // Verify that we got rightCreds instead of wrongCreds (or nothing).
                        Assert.Equal(expectCreds, connection.GetRequestHeaderValue("Proxy-Authorization"));

                        Task serverTask = connection.SendResponseAsync(HttpStatusCode.OK);

                        await TestHelper.WhenAllCompletedOrAnyFailed(serverTask, responseTask).ConfigureAwait(false);
                        HttpResponseMessage response = await responseTask.ConfigureAwait(false);

                        Assert.True(response.StatusCode ==  HttpStatusCode.OK);
                    });
                };
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [ActiveIssue(25640, TestPlatforms.Windows)] // TODO It should be enabled for SocketsHttpHandler on all platforms
        public async void ProxySetViaEnvironmentVariable_DefaultProxyCredentialsUsed(bool useProxy)
        {
            const string ExpectedUsername = "rightusername";
            const string ExpectedPassword = "rightpassword";
            LoopbackServer.Options options = new LoopbackServer.Options { IsProxy = true, Username = ExpectedUsername, Password = ExpectedPassword };

            await LoopbackServer.CreateServerAsync(async (proxyServer, proxyUri) =>
            {
                // libcurl will read a default proxy from the http_proxy environment variable.  Ensure that when it does,
                // our default proxy credentials are used.  To avoid messing up anything else in this process, we run the
                // test in another process.
                var psi = new ProcessStartInfo();
                Task<List<string>> proxyTask = null;

                if (useProxy)
                {
                    proxyTask = proxyServer.AcceptConnectionPerformAuthenticationAndCloseAsync("Proxy-Authenticate: Basic realm=\"NetCore\"\r\n");
                    psi.Environment.Add("http_proxy", $"http://{proxyUri.Host}:{proxyUri.Port}");
                }

                RemoteInvoke(async (useProxyString, useSocketsHttpHandlerString) =>
                {
                    using (HttpClientHandler handler = CreateHttpClientHandler(useSocketsHttpHandlerString))
                    using (var client = new HttpClient(handler))
                    {
                        var creds = new NetworkCredential(ExpectedUsername, ExpectedPassword);
                        handler.DefaultProxyCredentials = creds;
                        handler.UseProxy = bool.Parse(useProxyString);

                        HttpResponseMessage response = await client.GetAsync(Configuration.Http.RemoteEchoServer);
                        // Correctness of user and password is done in server part.
                        Assert.True(response.StatusCode ==  HttpStatusCode.OK);
                    }
                    return SuccessExitCode;
                }, useProxy.ToString(), UseSocketsHttpHandler.ToString(), new RemoteInvokeOptions { StartInfo = psi });

                if (useProxy)
                {      
                    await proxyTask;
                }
            }, options);
        }

        // The purpose of this test is mainly to validate the .NET Framework OOB System.Net.Http implementation
        // since it has an underlying dependency to WebRequest. While .NET Core implementations of System.Net.Http
        // are not using any WebRequest code, the test is still useful to validate correctness.
        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public async Task ProxyNotExplicitlyProvided_DefaultCredentialsSet_DefaultWebProxySetToNull_Success()
        {
            WebRequest.DefaultWebProxy = null;

            using (HttpClientHandler handler = CreateHttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                handler.DefaultProxyCredentials = new NetworkCredential("UsernameNotUsed", "PasswordNotUsed");
                HttpResponseMessage response = await client.GetAsync(Configuration.Http.RemoteEchoServer);

                Assert.Equal(response.StatusCode, HttpStatusCode.OK);
            }
        }
    }
}
