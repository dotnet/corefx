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
    public class ClientDefaultEncryptionTest
    {
        private DummyTcpServer serverRequireEncryption;
        private DummyTcpServer serverAllowNoEncryption;
        private DummyTcpServer serverNoEncryption;

        [TestInitialize]
        public void StartInternalServers()
        {
            serverRequireEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 400), EncryptionPolicy.RequireEncryption);
            serverAllowNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 401), EncryptionPolicy.AllowNoEncryption);
            serverNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 402), EncryptionPolicy.NoEncryption);
        }

        [TestCleanup]
        public void StopInternalServers()
        {
            serverRequireEncryption.Dispose();
            serverAllowNoEncryption.Dispose();
            serverNoEncryption.Dispose();
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
        public void ClientDefaultEncryption_ServerRequireEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverRequireEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null);
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
        public void ClientDefaultEncryption_ServerAllowNoEncryption_ConnectWithEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverAllowNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null);
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
        public void ClientDefaultEncryption_ServerNoEncryption_NoConnect()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null);
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

