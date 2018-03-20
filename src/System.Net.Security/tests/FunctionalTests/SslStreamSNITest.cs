// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Security.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class SslStreamSNITest
    {
        private static IEnumerable<object[]> HostNameData()
        {
            yield return new object[] { "a" };
            yield return new object[] { "test" };
            yield return new object[] { new string('a', 100) };
        }

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

        [Fact]
        public void SslStream_UnknownHostName_Fails()
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
                Assert.ThrowsAsync<NotSupportedException>(async () => {
                    await server.AuthenticateAsServerAsync(options, cts.Token);
                }).Wait();

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
    }
}
