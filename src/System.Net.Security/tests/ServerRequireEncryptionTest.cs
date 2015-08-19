namespace NCLTest.Security
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    [TestClass]
    public class ServerRequireEncryptionTest
    {
        private DummyTcpServer serverRequireEncryption;

        [TestInitialize]
        public void StartInternalServer()
        {
            serverRequireEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 400), EncryptionPolicy.RequireEncryption);
        }

        [TestCleanup]
        public void StopInternalServer()
        {
            serverRequireEncryption.Dispose();
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
        public void ServerRequireEncryption_ClientRequireEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverRequireEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                sslStream.CipherAlgorithm, sslStream.CipherStrength);
            Assert.IsTrue(sslStream.CipherAlgorithm != CipherAlgorithmType.Null, "Cipher algorithm should not be NULL");
            Assert.IsTrue(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
            sslStream.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void ServerRequireEncryption_ClientAllowNoEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverRequireEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.AllowNoEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                sslStream.CipherAlgorithm, sslStream.CipherStrength);
            Assert.IsTrue(sslStream.CipherAlgorithm != CipherAlgorithmType.Null, "Cipher algorithm should not be NULL");
            Assert.IsTrue(sslStream.CipherStrength > 0, "Cipher strength should be greater than 0");
            sslStream.Dispose();
            client.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void ServerRequireEncryption_ClientNoEncryption_NoConnect()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverRequireEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.NoEncryption);
            try
            {
                sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
                Assert.Fail("SslStream should not have connected");
            }
            finally
            {
                sslStream.Dispose();
                client.Dispose();
            }
        }
    }
}

