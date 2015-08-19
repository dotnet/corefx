namespace NCLTest.Security
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    [TestClass]
    public class ClientAsyncAuthenticateTest
    {
        private const SslProtocols AllSslProtocols =
            SslProtocols.Ssl2
            | SslProtocols.Ssl3
            | SslProtocols.Tls
            | SslProtocols.Tls11
            | SslProtocols.Tls12;

        private static readonly SslProtocols[] EachSslProtocol = new SslProtocols[] 
        { 
            SslProtocols.Ssl2,
            SslProtocols.Ssl3, 
            SslProtocols.Tls, 
            SslProtocols.Tls11, 
            SslProtocols.Tls12,
        };

        [TestMethod]
        public void ClientAsyncAuthenticate_ServerRequireEncryption_ConnectWithEncryption()
        {
            ClientAsyncSslHelper(EncryptionPolicy.RequireEncryption);
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void ClientAsyncAuthenticate_ServerNoEncryption_NoConnect()
        {
            ClientAsyncSslHelper(EncryptionPolicy.NoEncryption);
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_EachProtocol_Success()
        {
            foreach (SslProtocols protocol in EachSslProtocol)
            {
                ClientAsyncSslHelper(protocol, protocol);
            }
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_MismatchProtocols_Fails()
        {
            foreach (SslProtocols serverProtocol in EachSslProtocol)
            {
                foreach (SslProtocols clientProtocol in EachSslProtocol)
                {
                    if (serverProtocol != clientProtocol)
                    {
                        try
                        {
                            ClientAsyncSslHelper(serverProtocol, clientProtocol);
                            Assert.Fail(serverProtocol + "; " + clientProtocol);
                        }
                        catch (AuthenticationException) { }
                        catch (IOException) { }
                    }
                }
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(System.ComponentModel.Win32Exception))]
        public void ClientAsyncAuthenticate_Ssl2Tls12ServerSsl2Client_Fails()
        {
            // Ssl2 and Tls 1.2 are mutually exclusive.
            ClientAsyncSslHelper(SslProtocols.Ssl2 | SslProtocols.Tls12, SslProtocols.Ssl2);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ComponentModel.Win32Exception))]
        public void ClientAsyncAuthenticate_Ssl2Tls12ServerTls12Client_Fails()
        {
            // Ssl2 and Tls 1.2 are mutually exclusive.
            ClientAsyncSslHelper(SslProtocols.Ssl2 | SslProtocols.Tls12, SslProtocols.Tls12);
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_Ssl2ServerSsl2Tls12Client_Success()
        {
            ClientAsyncSslHelper(SslProtocols.Ssl2, SslProtocols.Ssl2 | SslProtocols.Tls12);
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_Tls12ServerSsl2Tls12Client_Success()
        {
            ClientAsyncSslHelper(SslProtocols.Tls12, SslProtocols.Ssl2 | SslProtocols.Tls12);
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_AllServerAllClient_Success()
        {
            // Drop Ssl2, it's incompatible with Tls 1.2
            SslProtocols sslProtocols = AllSslProtocols & ~SslProtocols.Ssl2;
            ClientAsyncSslHelper(sslProtocols, sslProtocols);
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_AllServerVsIndividualClientProtocols_Success()
        {
            foreach (SslProtocols clientProtocol in EachSslProtocol)
            {
                if (clientProtocol != SslProtocols.Ssl2) // Incompatible with Tls 1.2
                {
                    ClientAsyncSslHelper(clientProtocol, AllSslProtocols);
                }
            }
        }

        [TestMethod]
        public void ClientAsyncAuthenticate_IndividualServerVsAllClientProtocols_Success()
        {
            SslProtocols clientProtocols = AllSslProtocols & ~SslProtocols.Ssl2; // Incompatible with Tls 1.2
            foreach (SslProtocols serverProtocol in EachSslProtocol)
            {
                if (serverProtocol != SslProtocols.Ssl2) // Incompatible with Tls 1.2
                {
                    ClientAsyncSslHelper(clientProtocols, serverProtocol);
                    // Cached Tls creds fail when used against Tls servers of higher versions.
                    // Servers are not expected to dynamically change versions.
                    // Not available in ProjectK / N: FlushSslSessionCache();
                }
            }
        }

        #region Helpers

        private void ClientAsyncSslHelper(EncryptionPolicy encryptionPolicy)
        {
            ClientAsyncSslHelper(encryptionPolicy, TestConfiguration.DefaultSslProtocols, TestConfiguration.DefaultSslProtocols);
        }

        private void ClientAsyncSslHelper(SslProtocols clientSslProtocols, SslProtocols serverSslProtocols)
        {
            ClientAsyncSslHelper(EncryptionPolicy.RequireEncryption, clientSslProtocols, serverSslProtocols);
        }

        private void ClientAsyncSslHelper(EncryptionPolicy encryptionPolicy, SslProtocols clientSslProtocols, 
            SslProtocols serverSslProtocols)
        {
            Logger.LogInformation("Server: " + serverSslProtocols + "; Client: " + clientSslProtocols);
            
            IPEndPoint endPoint = new IPEndPoint(IPAddress.IPv6Loopback, 0);
            using (DummyTcpServer server = new DummyTcpServer(endPoint, encryptionPolicy))
            {
                server.SslProtocols = serverSslProtocols;
                using (TcpClient client = new TcpClient(AddressFamily.InterNetworkV6))
                {
                    client.Connect(server.RemoteEndPoint);

                    SslStream sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null);

                    IAsyncResult async = sslStream.BeginAuthenticateAsClient("localhost", null, clientSslProtocols, false, null, null);
                    Assert.IsTrue(async.AsyncWaitHandle.WaitOne(10000), "Timed Out");
                    sslStream.EndAuthenticateAsClient(async);

                    Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                        client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                        sslStream.CipherAlgorithm, sslStream.CipherStrength);
                    Assert.IsTrue(sslStream.CipherAlgorithm != CipherAlgorithmType.Null, "Cipher algorithm should not be NULL");
                    Assert.IsTrue(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
                                        
                    sslStream.Dispose();
                }
            }
        }
        
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public bool AllowAnyServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            return true;  // allow everything
        }
        #endregion Helpers
    }
}

