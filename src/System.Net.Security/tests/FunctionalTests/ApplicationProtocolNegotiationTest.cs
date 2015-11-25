// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Security.Tests
{
    public class ApplicationProtocolNegotiationTest
    {
        private readonly TimeSpan TestTimeoutSpan = TimeSpan.FromMilliseconds(TestConfiguration.PassingTestTimeoutMilliseconds);
        private const int ApplicationProtocolMismatchCode = unchecked((int) 0x80090367);

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public bool AllowAnyServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            return true;  // allow everything
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ProtocolListEmpty_Throw()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                // Exception thrown by SChannel
                Assert.Throws<AuthenticationException>(() => client.AuthenticateAsClient(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, new string[] {}));
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ProtocolNull_Throw()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                string[] protocolList = new string[] { null };
                Assert.Throws<ArgumentException>(() => client.AuthenticateAsClient(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, protocolList));
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ProtocolEmpty_Throw()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                string[] protocolList = new string[] { "" };
                Assert.Throws<ArgumentException>(() => client.AuthenticateAsClient(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, protocolList));
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ProtocolLongerThan255_Throw()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                string[] protocolList = new string[] {"".PadRight(256, 'A')};
                Assert.Throws<ArgumentException>(() => client.AuthenticateAsClient(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, protocolList));
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ProtocolListLongerThan32k_Throw()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                string[] protocolList = Enumerable.Range(0, 128).Select(number => number.ToString("D3").PadRight(255, 'A')).ToArray();
                Assert.Throws<ArgumentException>(() => client.AuthenticateAsClient(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, protocolList));
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_MatchExists_Connect()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var serverStream = new FakeNetworkStream(true, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var server = new SslStream(serverStream))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, new[] { "h2", "http/1.1" });
                auth[1] = server.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false, new[] { "test", "h2" });
                bool finished = Task.WaitAll(auth, TestTimeoutSpan);
                Assert.True(finished, "Handshake completed in the allotted time");
                Assert.True("h2" == client.NegotiatedApplicationProtocol, "Negotiated application protocol on client should be h2");
                Assert.True("h2" == server.NegotiatedApplicationProtocol, "Negotiated application protocol on server should be h2");
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_NoMatch_ServerThrow()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var serverStream = new FakeNetworkStream(true, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                Task[] auth = new Task[2];
                using (var server = new SslStream(serverStream, false))
                {
                    X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                    auth[0] = client.AuthenticateAsClientAsync(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, new[] { "h2" });
                    auth[1] = server.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false, new[] { "http/1.1" });

                    AuthenticationException exception = Assert.Throws<AuthenticationException>(() => auth[1].GetAwaiter().GetResult());
                    Assert.True(exception.InnerException is Win32Exception, "Server authenticate inner exception should be of type Win32Exception");
                    Assert.True(((Win32Exception)exception.InnerException).NativeErrorCode == ApplicationProtocolMismatchCode, "Server authenticate error code should be ApplicationProtocolMismatch");
                }
                Assert.Throws<IOException>(() => auth[0].GetAwaiter().GetResult());
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ServerRequestedOnly_Connect()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var serverStream = new FakeNetworkStream(true, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var server = new SslStream(serverStream))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false);
                auth[1] = server.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false, new[] { "h2" });

                bool finished = Task.WaitAll(auth, TestTimeoutSpan);
                Assert.True(finished, "Handshake completed in the allotted time");
                Assert.True(null == client.NegotiatedApplicationProtocol, "Negotiated protocol value on client should be null");
                Assert.True(null == server.NegotiatedApplicationProtocol, "Negotiated protocol value on server should be null");
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_ClientRequestedOnly_Connect()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var serverStream = new FakeNetworkStream(true, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var server = new SslStream(serverStream))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, new[] { "h2" });
                auth[1] = server.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false);

                bool finished = Task.WaitAll(auth, TestTimeoutSpan);
                Assert.True(finished, "Handshake completed in the allotted time");
                Assert.True(null == client.NegotiatedApplicationProtocol, "Negotiated protocol value on client should be null");
                Assert.True(null == server.NegotiatedApplicationProtocol, "Negotiated protocol value on server should be null");
            }
        }

        [Fact]
        public void ApplicationProtocolNegotiation_BeforeHandshakeComplete_NegotiatedApplicationProtocolThrows()
        {
            MockNetwork network = new MockNetwork();

            using (var clientStream = new FakeNetworkStream(false, network))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                client.AuthenticateAsClientAsync(certificate.Subject, new X509CertificateCollection(), SslProtocols.Tls12, false, new[] { "h2" });
                Assert.Throws<InvalidOperationException>(() => client.NegotiatedApplicationProtocol);
            }
        }
    }
}
