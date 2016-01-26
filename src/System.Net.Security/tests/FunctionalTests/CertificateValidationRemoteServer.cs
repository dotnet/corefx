// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
using System.Net.Tests;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Security.Tests
{
    public class CertificateValidationRemoteServer
    {
        [Fact]
        public async Task CertificateValidationRemoteServer_EndToEnd_Ok()
        {
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                await client.ConnectAsync(HttpTestServers.Host, 443);

                using (SslStream sslStream = new SslStream(client.GetStream(), false, RemoteHttpsCertValidation, null))
                {
                    await sslStream.AuthenticateAsClientAsync(HttpTestServers.Host);
                }
            }
        }

        private bool RemoteHttpsCertValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the local machine doesn't trust this remote machine, it will either be
            // * UntrustedRoot (it sent the whole chain, I don't trust it)
            // * PartialChain (it didn't send the root, and we couldn't complete it)
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            {
                X509ChainStatusFlags[] permittedFlags = new[]
                {
                    X509ChainStatusFlags.UntrustedRoot,
                    X509ChainStatusFlags.PartialChain,
                };

                Assert.Contains(chain.ChainStatus[0].Status, permittedFlags);
            }
            else
            {
                Assert.Equal(SslPolicyErrors.None, sslPolicyErrors);
            }

            return true;
        }
    }
}
