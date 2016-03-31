// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Security;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    public class HttpClientHandler_ServerCertificates_Test
    {
        [Fact]
        public async Task NoCallback_ValidCertificate_CallbackNotCalled()
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                Assert.Null(handler.ServerCertificateValidationCallback);
                Assert.False(handler.CheckCertificateRevocationList);

                using (HttpResponseMessage response = await client.GetAsync(HttpTestServers.SecureRemoteEchoServer))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                Assert.Throws<InvalidOperationException>(() => handler.ServerCertificateValidationCallback = null);
                Assert.Throws<InvalidOperationException>(() => handler.CheckCertificateRevocationList = false);
            }
        }

        [ConditionalFact(nameof(BackendSupportsCustomCertificateHandling))]
        public async Task UseCallback_NotSecureConnection_CallbackNotCalled()
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                bool callbackCalled = false;
                handler.ServerCertificateValidationCallback = delegate { callbackCalled = true; return true; };

                using (HttpResponseMessage response = await client.GetAsync(HttpTestServers.RemoteEchoServer))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                Assert.False(callbackCalled);
            }
        }

        public static IEnumerable<object[]> UseCallback_ValidCertificate_ExpectedValuesDuringCallback_Urls()
        {
            foreach (bool checkRevocation in new[] { true, false })
            {
                yield return new object[] { HttpTestServers.SecureRemoteEchoServer, checkRevocation };
                yield return new object[] { HttpTestServers.RedirectUriForDestinationUri(true, HttpTestServers.SecureRemoteEchoServer, 1), checkRevocation };
            }
        }

        [ConditionalTheory(nameof(BackendSupportsCustomCertificateHandling))]
        [MemberData(nameof(UseCallback_ValidCertificate_ExpectedValuesDuringCallback_Urls))]
        public async Task UseCallback_ValidCertificate_ExpectedValuesDuringCallback(Uri url, bool checkRevocation)
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                bool callbackCalled = false;
                handler.CheckCertificateRevocationList = checkRevocation;
                handler.ServerCertificateValidationCallback = (request, cert, chain, errors) => {
                    callbackCalled = true;
                    Assert.NotNull(request);
                    Assert.Equal(SslPolicyErrors.None, errors);
                    Assert.True(chain.ChainElements.Count > 0);
                    Assert.NotEmpty(cert.Subject);
                    Assert.Equal(checkRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck, chain.ChainPolicy.RevocationMode);
                    return true;
                };

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                Assert.True(callbackCalled);
            }
        }

        [ConditionalFact(nameof(BackendSupportsCustomCertificateHandling))]
        public async Task UseCallback_CallbackReturnsFailure_ThrowsException()
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                handler.ServerCertificateValidationCallback = delegate { return false; };
                await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer));
            }
        }

        [ConditionalFact(nameof(BackendSupportsCustomCertificateHandling))]
        public async Task UseCallback_CallbackThrowsException_ExceptionPropagates()
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                var e = new DivideByZeroException();
                handler.ServerCertificateValidationCallback = delegate { throw e; };
                Assert.Same(e, await Assert.ThrowsAsync<DivideByZeroException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer)));
            }
        }

        [Theory]
        [InlineData(HttpTestServers.ExpiredCertRemoteServer)]
        [InlineData(HttpTestServers.SelfSignedCertRemoteServer)]
        [InlineData(HttpTestServers.WrongHostNameCertRemoteServer)]
        public async Task NoCallback_BadCertificate_ThrowsException(string url)
        {
            using (var client = new HttpClient())
            {
                await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(url));
            }
        }

        [Fact]
        public async Task NoCallback_RevokedCertificate_NoRevocationChecking_Succeeds()
        {
            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(HttpTestServers.RevokedCertRemoteServer))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [ConditionalFact(nameof(BackendSupportsCustomCertificateHandling))]
        public async Task NoCallback_RevokedCertificate_RevocationChecking_Fails()
        {
            var handler = new HttpClientHandler() { CheckCertificateRevocationList = true };
            using (var client = new HttpClient(handler))
            {
                await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(HttpTestServers.RevokedCertRemoteServer));
            }
        }

        [ConditionalTheory(nameof(BackendSupportsCustomCertificateHandling))]
        [InlineData(HttpTestServers.ExpiredCertRemoteServer, SslPolicyErrors.RemoteCertificateChainErrors)]
        [InlineData(HttpTestServers.SelfSignedCertRemoteServer, SslPolicyErrors.RemoteCertificateChainErrors)]
        [InlineData(HttpTestServers.WrongHostNameCertRemoteServer, SslPolicyErrors.RemoteCertificateNameMismatch)]
        public async Task UseCallback_BadCertificate_ExpectedPolicyErrors(string url, SslPolicyErrors expectedErrors)
        {
            var handler = new HttpClientHandler();
            using (var client = new HttpClient(handler))
            {
                bool callbackCalled = false;

                handler.ServerCertificateValidationCallback = (request, cert, chain, errors) =>
                {
                    callbackCalled = true;
                    Assert.NotNull(request);
                    Assert.NotNull(cert);
                    Assert.NotNull(chain);
                    Assert.Equal(expectedErrors, errors);
                    return true;
                };

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                Assert.True(callbackCalled);
            }
        }

        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task SSLBackendNotSupported_Callback_ThrowsPlatformNotSupportedException()
        {
            using (var client = new HttpClient(new HttpClientHandler() { ServerCertificateValidationCallback = delegate { return true; } }))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer));
            }
        }

        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task SSLBackendNotSupported_Revocation_ThrowsPlatformNotSupportedException()
        {
            using (var client = new HttpClient(new HttpClientHandler() { CheckCertificateRevocationList = true }))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer));
            }
        }

        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task SSLBackendNotSupported_AutomaticClientCerts_ThrowsPlatformNotSupportedException()
        {
            using (var client = new HttpClient(new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Automatic }))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer));
            }
        }

        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task SSLBackendNotSupported_ManualClientCerts_ThrowsPlatformNotSupportedException()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new X509Certificate2());
            using (var client = new HttpClient(handler))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(HttpTestServers.SecureRemoteEchoServer));
            }
        }

        [Fact]
        public async Task PostAsync_Post_ChannelBinding_ConfiguredCorrectly()
        {
            var content = new ChannelBindingAwareContent("Test contest");
            using (var client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(HttpTestServers.SecureRemoteEchoServer, content))
            {
                // Validate status.
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                // Validate the ChannelBinding object exists.
                ChannelBinding channelBinding = content.ChannelBinding;
                Assert.NotNull(channelBinding);

                // Validate the ChannelBinding's validity.
                if (BackendSupportsCustomCertificateHandling)
                {
                    Assert.False(channelBinding.IsInvalid, "Expected valid binding");
                    Assert.NotEqual(IntPtr.Zero, channelBinding.DangerousGetHandle());

                    // Validate the ChannelBinding's description.
                    string channelBindingDescription = channelBinding.ToString();
                    Assert.NotNull(channelBindingDescription);
                    Assert.NotEmpty(channelBindingDescription);
                    Assert.True((channelBindingDescription.Length + 1) % 3 == 0, $"Unexpected length {channelBindingDescription.Length}");
                    for (int i = 0; i < channelBindingDescription.Length; i++)
                    {
                        char c = channelBindingDescription[i];
                        if (i % 3 == 2)
                        {
                            Assert.Equal(' ', c);
                        }
                        else
                        {
                            Assert.True((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'), $"Expected hex, got {c}");
                        }
                    }
                }
                else
                {
                    // Backend doesn't support getting the details to create the CBT.
                    Assert.True(channelBinding.IsInvalid, "Expected invalid binding");
                    Assert.Equal(IntPtr.Zero, channelBinding.DangerousGetHandle());
                    Assert.Null(channelBinding.ToString());
                }
            }
        }

        private static bool BackendSupportsCustomCertificateHandling =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            (CurlSslVersionDescription()?.StartsWith("OpenSSL") ?? false);

        private static bool BackendDoesNotSupportCustomCertificateHandling => !BackendSupportsCustomCertificateHandling;

        [DllImport("System.Net.Http.Native", EntryPoint = "HttpNative_GetSslVersionDescription")]
        private static extern string CurlSslVersionDescription();
    }
}
