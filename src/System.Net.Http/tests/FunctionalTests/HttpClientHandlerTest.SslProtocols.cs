// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Http.Functional.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class HttpClientHandler_SslProtocols_Test
    {
        [Fact]
        public void DefaultProtocols_MatchesExpected()
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Equal(SslProtocols.None, handler.SslProtocols);
            }
        }

        [Theory]
        [InlineData(SslProtocols.None)]
        [InlineData(SslProtocols.Tls)]
        [InlineData(SslProtocols.Tls11)]
        [InlineData(SslProtocols.Tls12)]
        [InlineData(SslProtocols.Tls | SslProtocols.Tls11)]
        [InlineData(SslProtocols.Tls11 | SslProtocols.Tls12)]
        [InlineData(SslProtocols.Tls | SslProtocols.Tls12)]
        [InlineData(SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12)]
        public void SetGetProtocols_Roundtrips(SslProtocols protocols)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.SslProtocols = protocols;
                Assert.Equal(protocols, handler.SslProtocols);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalFact(nameof(BackendSupportsSslConfiguration))]
        public async Task SetProtocols_AfterRequest_ThrowsException()
        {
            using (var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = LoopbackServer.AllowAllCertificates })
            using (var client = new HttpClient(handler))
            {
                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    await TestHelper.WhenAllCompletedOrAnyFailed(
                        LoopbackServer.ReadRequestAndSendResponseAsync(server),
                        client.GetAsync(url));
                });
                Assert.Throws<InvalidOperationException>(() => handler.SslProtocols = SslProtocols.Tls12);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Theory]
        [InlineData(~SslProtocols.None)]
#pragma warning disable 0618 // obsolete warning
        [InlineData(SslProtocols.Ssl2)]
        [InlineData(SslProtocols.Ssl3)]
        [InlineData(SslProtocols.Ssl2 | SslProtocols.Ssl3)]
        [InlineData(SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12)]
#pragma warning restore 0618
        public void DisabledProtocols_SetSslProtocols_ThrowsException(SslProtocols disabledProtocols)
        {
            using (var handler = new HttpClientHandler())
            {
                Assert.Throws<NotSupportedException>(() => handler.SslProtocols = disabledProtocols);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalTheory(nameof(BackendSupportsSslConfiguration))]
        [InlineData(SslProtocols.Tls, false)]
        [InlineData(SslProtocols.Tls, true)]
        [InlineData(SslProtocols.Tls11, false)]
        [InlineData(SslProtocols.Tls11, true)]
        [InlineData(SslProtocols.Tls12, false)]
        [InlineData(SslProtocols.Tls12, true)]
        public async Task GetAsync_AllowedSSLVersion_Succeeds(SslProtocols acceptedProtocol, bool requestOnlyThisProtocol)
        {
            using (var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = LoopbackServer.AllowAllCertificates })
            using (var client = new HttpClient(handler))
            {
                if (requestOnlyThisProtocol)
                {
                    handler.SslProtocols = acceptedProtocol;
                }
                var options = new LoopbackServer.Options { UseSsl = true, SslProtocols = acceptedProtocol };
                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    await TestHelper.WhenAllCompletedOrAnyFailed(
                        LoopbackServer.ReadRequestAndSendResponseAsync(server, options: options),
                        client.GetAsync(url));
                }, options);
            }
        }

        public readonly static object [][] SupportedSSLVersionServers =
        {
            new object[] {"TLSv1.0", Configuration.Http.TLSv10RemoteServer},
            new object[] {"TLSv1.1", Configuration.Http.TLSv11RemoteServer},
            new object[] {"TLSv1.2", Configuration.Http.TLSv12RemoteServer},
        };

        // This test is logically the same as the above test, albeit using remote servers
        // instead of local ones.  We're keeping it for now (as outerloop) because it helps
        // to validate against another SSL implementation that what we mean by a particular
        // TLS version matches that other implementation.
        [OuterLoop] // avoid www.ssllabs.com dependency in innerloop
        [Theory]
        [MemberData(nameof(SupportedSSLVersionServers))]
        public async Task GetAsync_SupportedSSLVersion_Succeeds(string name, string url)
        {
            using (var client = new HttpClient())
            {
                (await client.GetAsync(url)).Dispose();
            }
        }

        public readonly static object[][] NotSupportedSSLVersionServers =
        {
            new object[] {"SSLv2", Configuration.Http.SSLv2RemoteServer},
            new object[] {"SSLv3", Configuration.Http.SSLv3RemoteServer},
        };

        // It would be easy to remove the dependency on these remote servers if we didn't
        // explicitly disallow creating SslStream with SSLv2/3.  Since we explicitly throw
        // when trying to use such an SslStream, we can't stand up a localhost server that
        // only speaks those protocols.
        [OuterLoop] // avoid www.ssllabs.com dependency in innerloop
        [Theory]
        [MemberData(nameof(NotSupportedSSLVersionServers))]
        public async Task GetAsync_UnsupportedSSLVersion_Throws(string name, string url)
        {
            using (var client = new HttpClient())
            {
                await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(url));
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalFact(nameof(BackendSupportsSslConfiguration), nameof(SslDefaultsToTls12))]
        public async Task GetAsync_NoSpecifiedProtocol_DefaultsToTls12()
        {
            using (var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = LoopbackServer.AllowAllCertificates })
            using (var client = new HttpClient(handler))
            {
                var options = new LoopbackServer.Options { UseSsl = true };
                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    await TestHelper.WhenAllCompletedOrAnyFailed(
                        client.GetAsync(url),
                        LoopbackServer.AcceptSocketAsync(server, async (s, stream, reader, writer) =>
                        {
                            Assert.Equal(SslProtocols.Tls12, Assert.IsType<SslStream>(stream).SslProtocol);
                            await LoopbackServer.ReadWriteAcceptedAsync(s, reader, writer);
                            return null;
                        }, options));
                }, options);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ConditionalTheory(nameof(BackendSupportsSslConfiguration))]
        [InlineData(SslProtocols.Tls11, SslProtocols.Tls, typeof(IOException))]
        [InlineData(SslProtocols.Tls12, SslProtocols.Tls11, typeof(IOException))]
        [InlineData(SslProtocols.Tls, SslProtocols.Tls12, typeof(AuthenticationException))]
        public async Task GetAsync_AllowedSSLVersionDiffersFromServer_ThrowsException(
            SslProtocols allowedProtocol, SslProtocols acceptedProtocol, Type exceptedServerException)
        {
            using (var handler = new HttpClientHandler() { SslProtocols = allowedProtocol, ServerCertificateCustomValidationCallback = LoopbackServer.AllowAllCertificates })
            using (var client = new HttpClient(handler))
            {
                var options = new LoopbackServer.Options { UseSsl = true, SslProtocols = acceptedProtocol };
                await LoopbackServer.CreateServerAsync(async (server, url) =>
                {
                    await TestHelper.WhenAllCompletedOrAnyFailed(
                        Assert.ThrowsAsync(exceptedServerException, () => LoopbackServer.ReadRequestAndSendResponseAsync(server, options: options)),
                        Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(url)));
                }, options);
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [ActiveIssue(8538, TestPlatforms.Windows)]
        [Fact]
        public async Task GetAsync_DisallowTls10_AllowTls11_AllowTls12()
        {
            using (var handler = new HttpClientHandler() { SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12, ServerCertificateCustomValidationCallback = LoopbackServer.AllowAllCertificates })
            using (var client = new HttpClient(handler))
            {
                if (BackendSupportsSslConfiguration)
                {
                    LoopbackServer.Options options = new LoopbackServer.Options { UseSsl = true };

                    options.SslProtocols = SslProtocols.Tls;
                    await LoopbackServer.CreateServerAsync(async (server, url) =>
                    {
                        await TestHelper.WhenAllCompletedOrAnyFailed(
                            Assert.ThrowsAsync<IOException>(() => LoopbackServer.ReadRequestAndSendResponseAsync(server, options: options)),
                            Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(url)));
                    }, options);

                    foreach (var prot in new[] { SslProtocols.Tls11, SslProtocols.Tls12 })
                    {
                        options.SslProtocols = prot;
                        await LoopbackServer.CreateServerAsync(async (server, url) =>
                        {
                            await TestHelper.WhenAllCompletedOrAnyFailed(
                                LoopbackServer.ReadRequestAndSendResponseAsync(server, options: options),
                                client.GetAsync(url));
                        }, options);
                    }
                }
                else
                {
                    await Assert.ThrowsAnyAsync<NotSupportedException>(() => client.GetAsync($"http://{Guid.NewGuid().ToString()}/"));
                }
            }
        }

        private static bool SslDefaultsToTls12 => !PlatformDetection.IsWindows7;
        // TLS 1.2 may not be enabled on Win7
        // https://technet.microsoft.com/en-us/library/dn786418.aspx#BKMK_SchannelTR_TLS12

        private static bool BackendSupportsSslConfiguration =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            (CurlSslVersionDescription()?.StartsWith("OpenSSL") ?? false);

        [DllImport("System.Net.Http.Native", EntryPoint = "HttpNative_GetSslVersionDescription")]
        private static extern string CurlSslVersionDescription();
    }
}
