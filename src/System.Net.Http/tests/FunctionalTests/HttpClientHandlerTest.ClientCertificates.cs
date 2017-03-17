// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class HttpClientHandler_ClientCertificates_Test
    {
        [Fact]
        public void ClientCertificateOptions_Default()
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Equal(ClientCertificateOption.Manual, handler.ClientCertificateOptions);
            }
        }

        [Theory]
        [InlineData((ClientCertificateOption)2)]
        [InlineData((ClientCertificateOption)(-1))]
        public void ClientCertificateOptions_InvalidArg_ThrowsException(ClientCertificateOption option)
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Throws<ArgumentOutOfRangeException>("value", () => handler.ClientCertificateOptions = option);
            }
        }

        [Theory]
        [InlineData(ClientCertificateOption.Automatic)]
        [InlineData(ClientCertificateOption.Manual)]
        public void ClientCertificateOptions_ValueArg_Roundtrips(ClientCertificateOption option)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = option;
                Assert.Equal(option, handler.ClientCertificateOptions);
            }
        }

        [Fact]
        public void ClientCertificates_ClientCertificateOptionsAutomatic_ThrowsException()
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                Assert.Throws<InvalidOperationException>(() => handler.ClientCertificates);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task Automatic_SSLBackendNotSupported_ThrowsPlatformNotSupportedException()
        {
            using (var client = new HttpClient(new HttpClientHandler() { ClientCertificateOptions = ClientCertificateOption.Automatic }))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(Configuration.Http.SecureRemoteEchoServer));
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalFact(nameof(BackendDoesNotSupportCustomCertificateHandling))]
        public async Task Manual_SSLBackendNotSupported_ThrowsPlatformNotSupportedException()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(Configuration.Certificates.GetClientCertificate());
            using (var client = new HttpClient(handler))
            {
                await Assert.ThrowsAsync<PlatformNotSupportedException>(() => client.GetAsync(Configuration.Http.SecureRemoteEchoServer));
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ActiveIssue(9543)] // fails sporadically with 'WinHttpException : The server returned an invalid or unrecognized response' or 'TaskCanceledException : A task was canceled'
        [ConditionalTheory(nameof(BackendSupportsCustomCertificateHandling))]
        [InlineData(6, false)]
        [InlineData(3, true)]
        public async Task Manual_CertificateSentMatchesCertificateReceived_Success(
            int numberOfRequests,
            bool reuseClient) // validate behavior with and without connection pooling, which impacts client cert usage
        {
            var options = new LoopbackServer.Options { UseSsl = true };

            Func<X509Certificate2, HttpClient> createClient = (cert) =>
            {
                var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = delegate { return true; } };
                handler.ClientCertificates.Add(cert);
                return new HttpClient(handler);
            };

            Func<HttpClient, Socket, Uri, X509Certificate2, Task> makeAndValidateRequest = async (client, server, url, cert) =>
            {
                await TestHelper.WhenAllCompletedOrAnyFailed(
                    client.GetStringAsync(url),
                    LoopbackServer.AcceptSocketAsync(server, async (socket, stream, reader, writer) =>
                    {
                        SslStream sslStream = Assert.IsType<SslStream>(stream);
                        Assert.Equal(cert, sslStream.RemoteCertificate);
                        await LoopbackServer.ReadWriteAcceptedAsync(socket, reader, writer);
                        return null;
                    }, options));
            };

            await LoopbackServer.CreateServerAsync(async (server, url) =>
            {
                if (reuseClient)
                {
                    using (X509Certificate2 cert = Configuration.Certificates.GetClientCertificate())
                    {
                        using (HttpClient client = createClient(cert))
                        {
                            for (int i = 0; i < numberOfRequests; i++)
                            {
                                await makeAndValidateRequest(client, server, url, cert);

                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < numberOfRequests; i++)
                    {
                        using (X509Certificate2 cert = Configuration.Certificates.GetClientCertificate())
                        {
                            using (HttpClient client = createClient(cert))
                            {
                                await makeAndValidateRequest(client, server, url, cert);
                            }
                        }

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }, options);
        }

        private static bool BackendSupportsCustomCertificateHandling =>
            HttpClientHandler_ServerCertificates_Test.BackendSupportsCustomCertificateHandling;

        private static bool BackendDoesNotSupportCustomCertificateHandling => !BackendSupportsCustomCertificateHandling;
    }
}
