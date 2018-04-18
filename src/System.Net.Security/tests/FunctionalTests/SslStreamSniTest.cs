﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Security.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class SslStreamSniTest
    {
        [Theory]
        [MemberData(nameof(HostNameData))]
        public void SslStream_ClientSendsSNIServerReceives_Ok(string hostName)
        {
            X509Certificate serverCert = Configuration.Certificates.GetSelfSignedServerCertificate();

            WithVirtualConnection((server, client) =>
            {
                Task clientJob = Task.Run(() => {
                    client.AuthenticateAsClient(hostName);
                });

                SslServerAuthenticationOptions options = DefaultServerOptions();

                int timesCallbackCalled = 0;
                options.ServerCertificateSelectionCallback = (sender, actualHostName) =>
                {
                    timesCallbackCalled++;
                    Assert.Equal(hostName, actualHostName);
                    return serverCert;
                };

                var cts = new CancellationTokenSource();
                server.AuthenticateAsServerAsync(options, cts.Token).Wait();

                Assert.Equal(1, timesCallbackCalled);
                clientJob.Wait();
            },
            (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    Assert.Equal(serverCert, certificate);
                    return true;
                });
        }

        [Theory]
        [MemberData(nameof(HostNameData))]
        public void SslStream_ServerCallbackAndLocalCertificateSelectionSet_Throws(string hostName)
        {
            X509Certificate serverCert = Configuration.Certificates.GetSelfSignedServerCertificate();

            int timesCallbackCalled = 0;

            var selectionCallback = new LocalCertificateSelectionCallback((object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] issuers) =>
            {
                Assert.True(false, "LocalCertificateSelectionCallback called when AuthenticateAsServerAsync was expected to fail.");
                return null;
            });

            var validationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                Assert.Equal(serverCert, certificate);
                return true; 
            });

            VirtualNetwork vn = new VirtualNetwork();
            using (VirtualNetworkStream serverStream = new VirtualNetworkStream(vn, isServer: true),
                                        clientStream = new VirtualNetworkStream(vn, isServer: false))
            using (SslStream server = new SslStream(serverStream, false, null, selectionCallback),
                             client = new SslStream(clientStream, leaveInnerStreamOpen: false, validationCallback))
            {
                Task clientJob = Task.Run(() => {
                    client.AuthenticateAsClient(hostName);
                    Assert.True(false, "RemoteCertificateValidationCallback called when AuthenticateAsServerAsync was expected to fail.");
                });

                SslServerAuthenticationOptions options = DefaultServerOptions();
                options.ServerCertificate = serverCert;
                options.ServerCertificateSelectionCallback = (sender, actualHostName) =>
                {
                    timesCallbackCalled++;
                    Assert.Equal(hostName, actualHostName);
                    return serverCert;
                };

                var cts = new CancellationTokenSource();
                Assert.Throws<InvalidOperationException>(() => {server.AuthenticateAsServerAsync(options, cts.Token).Wait();});

                Assert.Equal(0, timesCallbackCalled);
            }
        }

        [Theory]
        [MemberData(nameof(HostNameData))]
        public void SslStream_ServerCallbackNotSet_UsesLocalCertificateSelection(string hostName)
        {
            X509Certificate serverCert = Configuration.Certificates.GetSelfSignedServerCertificate();

            int timesCallbackCalled = 0;

            var selectionCallback = new LocalCertificateSelectionCallback((object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] issuers) =>
            {
                Assert.Equal(targetHost, string.Empty);
                Assert.True(localCertificates.Contains(serverCert));
                timesCallbackCalled++;
                return serverCert;
            });

            var validationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                Assert.Equal(serverCert, certificate);
                return true;
            });

            VirtualNetwork vn = new VirtualNetwork();
            using (VirtualNetworkStream serverStream = new VirtualNetworkStream(vn, isServer: true),
                                        clientStream = new VirtualNetworkStream(vn, isServer: false))
            using (SslStream server = new SslStream(serverStream, false, null, selectionCallback),
                             client = new SslStream(clientStream, leaveInnerStreamOpen: false, validationCallback))
            {
                Task clientJob = Task.Run(() => {
                    client.AuthenticateAsClient(hostName);
                });

                SslServerAuthenticationOptions options = DefaultServerOptions();
                options.ServerCertificate = serverCert;

                var cts = new CancellationTokenSource();
                server.AuthenticateAsServerAsync(options, cts.Token).Wait();

                Assert.Equal(1, timesCallbackCalled);
                clientJob.Wait();
            }
        }

        [Theory]
        [MemberData(nameof(HostNameData))]
        public void SslStream_ServerCallbackReturnsNull_Throws(string hostName)
        {
            X509Certificate serverCert = Configuration.Certificates.GetSelfSignedServerCertificate();

            var selectionCallback = new LocalCertificateSelectionCallback((object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] issuers) =>
            {
                return null;
            });

            var validationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                Assert.True(false, "RemoteCertificateValidationCallback called when AuthenticateAsServerAsync was expected to fail.");
                return true; 
            });

            VirtualNetwork vn = new VirtualNetwork();
            using (VirtualNetworkStream serverStream = new VirtualNetworkStream(vn, isServer: true),
                                        clientStream = new VirtualNetworkStream(vn, isServer: false))
            using (SslStream server = new SslStream(serverStream, false, null, selectionCallback),
                             client = new SslStream(clientStream, leaveInnerStreamOpen: false, validationCallback))
            {
                Task clientJob = Task.Run(() => {
                    client.AuthenticateAsClient(hostName);
                });

                SslServerAuthenticationOptions options = DefaultServerOptions();
                options.ServerCertificate = serverCert;

                var cts = new CancellationTokenSource();
                Assert.Throws<NotSupportedException>(WithAggregateExceptionUnwrapping(() =>
                    server.AuthenticateAsServerAsync(options, cts.Token).Wait()
                ));
            }
        }

        [Fact]
        public void SslStream_NoSniFromClient_CallbackReturnsNull()
        {
            WithVirtualConnection((server, client) =>
            {
                Task clientJob = Task.Run(() => {
                    Assert.Throws<VirtualNetwork.VirtualNetworkConnectionBroken>(()
                        => client.AuthenticateAsClient("test"));
                });

                int timesCallbackCalled = 0;
                SslServerAuthenticationOptions options = DefaultServerOptions();
                options.ServerCertificateSelectionCallback = (sender, actualHostName) =>
                {
                    timesCallbackCalled++;
                    return null;
                };

                var cts = new CancellationTokenSource();
                Assert.Throws<AuthenticationException>(WithAggregateExceptionUnwrapping(() =>
                    server.AuthenticateAsServerAsync(options, cts.Token).Wait()
                ));

                // to break connection so that client is not waiting
                server.Dispose();

                Assert.Equal(1, timesCallbackCalled);

                clientJob.Wait();
            },
            (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                return true;
            });
        }

        private static Action WithAggregateExceptionUnwrapping(Action a)
        {
            return () => {
                try
                {
                    a();
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            };
        }

        private static SslServerAuthenticationOptions DefaultServerOptions()
        {
            return new SslServerAuthenticationOptions()
            {
                ClientCertificateRequired = false,
                EnabledSslProtocols = SslProtocols.Tls,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
            };
        }

        private void WithVirtualConnection(Action<SslStream, SslStream> serverClientConnection, RemoteCertificateValidationCallback clientCertValidate)
        {
            VirtualNetwork vn = new VirtualNetwork();
            using (VirtualNetworkStream serverStream = new VirtualNetworkStream(vn, isServer: true),
                                        clientStream = new VirtualNetworkStream(vn, isServer: false))
            using (SslStream server = new SslStream(serverStream, leaveInnerStreamOpen: false),
                             client = new SslStream(clientStream, leaveInnerStreamOpen: false, clientCertValidate))
            {
                serverClientConnection(server, client);
            }
        }

        private static IEnumerable<object[]> HostNameData()
        {
            yield return new object[] { "a" };
            yield return new object[] { "test" };
            // max allowed hostname length is 63
            yield return new object[] { new string('a', 63) };
            yield return new object[] { "\u017C\u00F3\u0142\u0107 g\u0119\u015Bl\u0105 ja\u017A\u0144. \u7EA2\u70E7. \u7167\u308A\u713C\u304D" };
        }
    }
}
