// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Test.Common;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public abstract class HttpClientHandler_Authentication_Test : HttpClientTestBase
    {
        private const string Username = "testusername";
        private const string Password = "testpassword";
        private const string Domain = "testdomain";

        private static readonly NetworkCredential s_credentials = new NetworkCredential(Username, Password, Domain);

        private static readonly Func<HttpClientHandler, Uri, HttpStatusCode, NetworkCredential, Task> s_createAndValidateRequest = async (handler, url, expectedStatusCode, credentials) =>
        {
            handler.Credentials = credentials;

            using (HttpClient client = new HttpClient(handler))
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                Assert.Equal(expectedStatusCode, response.StatusCode);
            }
        };

        [Theory]
        [MemberData(nameof(Authentication_TestData))]
        public async Task HttpClientHandler_Authentication_Succeeds(string authenticateHeader, bool result)
        {
            if (PlatformDetection.IsWindowsNanoServer || (IsCurlHandler && authenticateHeader.ToLowerInvariant().Contains("digest")))
            {
                // TODO: #27113: Fix failing authentication test cases on different httpclienthandlers.
                return;
            }

            var options = new LoopbackServer.Options { Domain = Domain, Username = Username, Password = Password };
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                string serverAuthenticateHeader = $"WWW-Authenticate: {authenticateHeader}\r\n";
                HttpClientHandler handler = CreateHttpClientHandler();
                Task serverTask = result ?
                    server.AcceptConnectionPerformAuthenticationAndCloseAsync(serverAuthenticateHeader) :
                    server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, serverAuthenticateHeader);

                await TestHelper.WhenAllCompletedOrAnyFailedWithTimeout(TestHelper.PassingTestTimeoutMilliseconds,
                    s_createAndValidateRequest(handler, url, result ? HttpStatusCode.OK : HttpStatusCode.Unauthorized, s_credentials), serverTask);
            }, options);
        }

        [Theory]
        [InlineData("WWW-Authenticate: Basic realm=\"hello1\"\r\nWWW-Authenticate: Basic realm=\"hello2\"\r\n")]
        [InlineData("WWW-Authenticate: Basic realm=\"hello\"\r\nWWW-Authenticate: Basic realm=\"hello\"\r\n")]
        [InlineData("WWW-Authenticate: Digest realm=\"hello\", nonce=\"hello\", algorithm=MD5\r\nWWW-Authenticate: Digest realm=\"hello\", nonce=\"hello\", algorithm=MD5\r\n")]
        [InlineData("WWW-Authenticate: Digest realm=\"hello1\", nonce=\"hello\", algorithm=MD5\r\nWWW-Authenticate: Digest realm=\"hello\", nonce=\"hello\", algorithm=MD5\r\n")]
        public async void HttpClientHandler_MultipleAuthenticateHeaders_WithSameAuth_Succeeds(string authenticateHeader)
        {
            if (IsWinHttpHandler)
            {
                // TODO: #27113: Fix failing authentication test cases on different httpclienthandlers.
                return;
            }

            await HttpClientHandler_MultipleAuthenticateHeaders_Succeeds(authenticateHeader);
        }

        [Theory]
        [InlineData("WWW-Authenticate: Basic realm=\"hello\"\r\nWWW-Authenticate: Digest realm=\"hello\", nonce=\"hello\", algorithm=MD5\r\n")]
        [InlineData("WWW-Authenticate: Digest realm=\"hello\", nonce=\"hello\", algorithm=MD5\r\nWWW-Authenticate: Basic realm=\"hello\"\r\n")]
        public async Task HttpClientHandler_MultipleAuthenticateHeaders_Succeeds(string authenticateHeader)
        {
            if (PlatformDetection.IsWindowsNanoServer || (IsCurlHandler && authenticateHeader.Contains("Digest")))
            {
                // TODO: #27113: Fix failing authentication test cases on different httpclienthandlers.
                return;
            }

            var options = new LoopbackServer.Options { Domain = Domain, Username = Username, Password = Password };
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                Task serverTask = server.AcceptConnectionPerformAuthenticationAndCloseAsync(authenticateHeader);
                await TestHelper.WhenAllCompletedOrAnyFailed(s_createAndValidateRequest(handler, url, HttpStatusCode.OK, s_credentials), serverTask);
            }, options);
        }

        [Theory]
        [InlineData("WWW-Authenticate: Basic realm=\"hello\"\r\n")]
        [InlineData("WWW-Authenticate: Digest realm=\"hello\", nonce=\"testnonce\"\r\n")]
        public async void HttpClientHandler_IncorrectCredentials_Fails(string authenticateHeader)
        {
            var options = new LoopbackServer.Options { Domain = Domain, Username = Username, Password = Password };
            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                HttpClientHandler handler = CreateHttpClientHandler();
                Task serverTask = server.AcceptConnectionPerformAuthenticationAndCloseAsync(authenticateHeader);
                await TestHelper.WhenAllCompletedOrAnyFailed(s_createAndValidateRequest(handler, url, HttpStatusCode.Unauthorized, new NetworkCredential("wronguser", "wrongpassword")), serverTask);
            }, options);
        }

        public static IEnumerable<object[]> Authentication_TestData()
        {
            yield return new object[] { "Basic realm=\"testrealm\"", true };
            yield return new object[] { "Basic ", true };
            yield return new object[] { "Basic realm=withoutquotes", true };
            yield return new object[] { "basic ", true };
            yield return new object[] { "bAsiC ", true };
            yield return new object[] { "basic", true };

            // Add digest tests fail on CurlHandler.
            // TODO: #27113: Fix failing authentication test cases on different httpclienthandlers.
            yield return new object[] { "Digest realm=\"testrealm\" nonce=\"testnonce\"", false };
            yield return new object[] { $"Digest realm=\"testrealm\", nonce=\"{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{DateTimeOffset.UtcNow}:XMh;z+$5|`i6Hx}}\", qop=auth-int, algorithm=MD5"))}\"", true };
            yield return new object[] { "Digest realm=\"api@example.org\", qop=\"auth\", algorithm=MD5-sess, nonce=\"5TsQWLVdgBdmrQ0XsxbDODV+57QdFR34I9HAbC/RVvkK\", " +
                    "opaque=\"HRPCssKJSGjCrkzDg8OhwpzCiGPChXYjwrI2QmXDnsOS\", charset=UTF-8, userhash=true", true };
            yield return new object[] { "dIgEsT realm=\"api@example.org\", qop=\"auth\", algorithm=MD5-sess, nonce=\"5TsQWLVdgBdmrQ0XsxbDODV+57QdFR34I9HAbC/RVvkK\", " +
                    "opaque=\"HRPCssKJSGjCrkzDg8OhwpzCiGPChXYjwrI2QmXDnsOS\", charset=UTF-8, userhash=true", true };
            yield return new object[] { $"Basic realm=\"testrealm\", " +
                    $"Digest realm=\"testrealm\", nonce=\"{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{DateTimeOffset.UtcNow}:XMh;z+$5|`i6Hx}}"))}\", algorithm=MD5", true };

            if (PlatformDetection.IsNetCore)
            {
                // These fail on full framework runs.
                // TODO: #27113: Fix failing authentication test cases on different httpclienthandlers.
                yield return new object[] { "Digest realm=\"testrealm1\", nonce=\"testnonce1\" Digest realm=\"testrealm2\", nonce=\"testnonce2\"", false };
                yield return new object[] { "Basic something, Digest something", false };
                yield return new object[] { $"Digest realm=\"testrealm\", nonce=\"testnonce\", algorithm=MD5 " +
                    $"Basic realm=\"testrealm\"", false };
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Basic")]
        [InlineData("Digest")]
        [InlineData("NTLM")]
        [InlineData("Kerberos")]
        [InlineData("Negotiate")]
        public async Task PreAuthenticate_NoPreviousAuthenticatedRequests_NoCredentialsSent(string credCacheScheme)
        {
            if (IsCurlHandler && credCacheScheme != null)
            {
                // When provided with a credential that targets just one auth scheme,
                // libcurl will often proactively send an auth header.
                return;
            }

            const int NumRequests = 3;
            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ConnectionClose = true; // for simplicity of not needing to know every handler's pooling policy
                    handler.PreAuthenticate = true;
                    switch (credCacheScheme)
                    {
                        case null:
                            handler.Credentials = s_credentials;
                            break;

                        default:
                            var cc = new CredentialCache();
                            cc.Add(uri, credCacheScheme, s_credentials);
                            handler.Credentials = cc;
                            break;
                    }

                    for (int i = 0; i < NumRequests; i++)
                    {
                        Assert.Equal("hello world", await client.GetStringAsync(uri));
                    }
                }
            },
            async server =>
            {
                for (int i = 0; i < NumRequests; i++)
                {
                    List<string> headers = await server.AcceptConnectionSendResponseAndCloseAsync(content: "hello world");
                    Assert.All(headers, header => Assert.DoesNotContain("Authorization", header));
                }
            });
        }

        [Theory]
        [InlineData(null, "WWW-Authenticate: Basic realm=\"hello\"\r\n")]
        [InlineData("Basic", "WWW-Authenticate: Basic realm=\"hello\"\r\n")]
        public async Task PreAuthenticate_FirstRequestNoHeaderAndAuthenticates_SecondRequestPreauthenticates(string credCacheScheme, string authResponse)
        {
            if (IsCurlHandler && credCacheScheme != null)
            {
                // When provided with a credential that targets just one auth scheme,
                // libcurl will often proactively send an auth header.
                return;
            }

            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ConnectionClose = true; // for simplicity of not needing to know every handler's pooling policy
                    handler.PreAuthenticate = true;
                    switch (credCacheScheme)
                    {
                        case null:
                            handler.Credentials = s_credentials;
                            break;

                        default:
                            var cc = new CredentialCache();
                            cc.Add(uri, credCacheScheme, s_credentials);
                            handler.Credentials = cc;
                            break;
                    }

                    Assert.Equal("hello world", await client.GetStringAsync(uri));
                    Assert.Equal("hello world", await client.GetStringAsync(uri));
                }
            },
            async server =>
            {
                List<string> headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, authResponse);
                Assert.All(headers, header => Assert.DoesNotContain("Authorization", header));

                for (int i = 0; i < 2; i++)
                {
                    headers = await server.AcceptConnectionSendResponseAndCloseAsync(content: "hello world");
                    Assert.Contains(headers, header => header.Contains("Authorization"));
                }
            });
        }

        [Fact]
        public async Task PreAuthenticate_SuccessfulBasicButThenFails_DoesntLoopInfinitely()
        {
            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ConnectionClose = true; // for simplicity of not needing to know every handler's pooling policy
                    handler.PreAuthenticate = true;
                    handler.Credentials = s_credentials;

                    // First two requests: initially without auth header, then with
                    Assert.Equal("hello world", await client.GetStringAsync(uri));

                    // Attempt preauth, and when that fails, give up.
                    using (HttpResponseMessage resp = await client.GetAsync(uri))
                    {
                        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
                    }
                }
            },
            async server =>
            {
                // First request, no auth header, challenge Basic
                List<string> headers = headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, "WWW-Authenticate: Basic realm=\"hello\"\r\n");
                Assert.All(headers, header => Assert.DoesNotContain("Authorization", header));

                // Second request, contains Basic auth header
                headers = await server.AcceptConnectionSendResponseAndCloseAsync(content: "hello world");
                Assert.Contains(headers, header => header.Contains("Authorization"));

                // Third request, contains Basic auth header but challenges anyway
                headers = headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, "WWW-Authenticate: Basic realm=\"hello\"\r\n");
                Assert.Contains(headers, header => header.Contains("Authorization"));

                if (IsNetfxHandler)
                {
                    // For some reason, netfx tries one more time.
                    headers = headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, "WWW-Authenticate: Basic realm=\"hello\"\r\n");
                    Assert.Contains(headers, header => header.Contains("Authorization"));
                }
            });
        }

        [Fact]
        public async Task PreAuthenticate_SuccessfulBasic_ThenDigestChallenged()
        {
            if (IsWinHttpHandler)
            {
                // WinHttpHandler fails with Unauthorized after the basic preauth fails.
                return;
            }

            if (IsCurlHandler)
            {
                // When provided with a credential that targets just one auth scheme,
                // libcurl will often proactively send an auth header.
                return;
            }

            await LoopbackServer.CreateClientAndServerAsync(async uri =>
            {
                using (HttpClientHandler handler = CreateHttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ConnectionClose = true; // for simplicity of not needing to know every handler's pooling policy
                    handler.PreAuthenticate = true;
                    handler.Credentials = s_credentials;

                    Assert.Equal("hello world", await client.GetStringAsync(uri));
                    Assert.Equal("hello world", await client.GetStringAsync(uri));
                }
            },
            async server =>
            {
                // First request, no auth header, challenge Basic
                List<string> headers = headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, "WWW-Authenticate: Basic realm=\"hello\"\r\n");
                Assert.All(headers, header => Assert.DoesNotContain("Authorization", header));

                // Second request, contains Basic auth header, success
                headers = await server.AcceptConnectionSendResponseAndCloseAsync(content: "hello world");
                Assert.Contains(headers, header => header.Contains("Authorization"));

                // Third request, contains Basic auth header, challenge digest
                headers = await server.AcceptConnectionSendResponseAndCloseAsync(HttpStatusCode.Unauthorized, "WWW-Authenticate: Digest realm=\"hello\", nonce=\"testnonce\"\r\n");
                Assert.Contains(headers, header => header.Contains("Authorization: Basic"));

                // Fourth request, contains Digest auth header, success
                headers = await server.AcceptConnectionSendResponseAndCloseAsync(content: "hello world");
                Assert.Contains(headers, header => header.Contains("Authorization: Digest"));
            });
        }
    }
}
