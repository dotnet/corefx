namespace NCLTest.Security
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;

    [TestClass]
    public class ServerAllowNoEncryptionTest
    {
        private DummyTcpServer serverAllowNoEncryption;

        [TestInitialize]
        public void StartInternalServer()
        {
            serverAllowNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 401), EncryptionPolicy.AllowNoEncryption);
        }

        [TestCleanup]
        public void StopInternalServer()
        {
            serverAllowNoEncryption.Dispose();
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

        [TestMethod]
        public void ServerAllowNoEncryption_ClientRequireEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverAllowNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint, 
                sslStream.CipherAlgorithm, sslStream.CipherStrength);
            Assert.AreNotEqual(CipherAlgorithmType.Null, sslStream.CipherAlgorithm, "Cipher algorithm should not be NULL");
            Assert.IsTrue(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
            sslStream.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void ServerAllowNoEncryption_ClientAllowNoEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverAllowNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.AllowNoEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                sslStream.CipherAlgorithm, sslStream.CipherStrength);
            Assert.AreNotEqual(CipherAlgorithmType.Null, sslStream.CipherAlgorithm, "Cipher algorithm should not be NULL");
            Assert.IsTrue(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
            sslStream.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void ServerAllowNoEncryption_ClientNoEncryption_ConnectWithNoEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            using (client = new TcpClient())
            {
                client.Connect(serverAllowNoEncryption.RemoteEndPoint);

                using (sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.NoEncryption))
                {
                    sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
                    Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                        client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                        sslStream.CipherAlgorithm, sslStream.CipherStrength);

                    CipherAlgorithmType expected = CipherAlgorithmType.Null;
                    Assert.AreEqual(expected, sslStream.CipherAlgorithm, "Cipher algorithm should  be NULL");
                    Assert.AreEqual(0, sslStream.CipherStrength, "Cipher strength should be equal to 0");
                }
            }
        }
    }
}

