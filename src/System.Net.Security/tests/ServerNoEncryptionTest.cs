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
    public class ServerNoEncryptionTest
    {
        private DummyTcpServer serverNoEncryption;

        [TestInitialize]
        public void StartInternalServer()
        {
            serverNoEncryption = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 402), EncryptionPolicy.NoEncryption);
        }

        [TestCleanup]
        public void StopInternalServer()
        {
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
        [ExpectedException(typeof(System.IO.IOException))]
        public void ServerNoEncryption_ClientRequireEncryption_NoConnect()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption);
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

        [TestMethod]
        public void ServerNoEncryption_ClientAllowNoEncryption_ConnectWithNoEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.AllowNoEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);

            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                sslStream.CipherAlgorithm, sslStream.CipherStrength);

            CipherAlgorithmType expected = CipherAlgorithmType.Null;
            Assert.AreEqual(expected, sslStream.CipherAlgorithm, "Cipher algorithm should  be NULL");
            Assert.AreEqual(0, sslStream.CipherStrength, "Cipher strength should be equal to 0");
            sslStream.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void ServerNoEncryption_ClientNoEncryption_ConnectWithNoEncryption()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(serverNoEncryption.RemoteEndPoint);

            sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.NoEncryption);
            sslStream.AuthenticateAsClient("localhost", null, TestConfiguration.DefaultSslProtocols, false);
            Logger.LogInformation("Client({0}) authenticated to server({1}) with encryption cipher: {2} {3}-bit strength",
                client.Client.LocalEndPoint, client.Client.RemoteEndPoint,
                sslStream.CipherAlgorithm, sslStream.CipherStrength);

            CipherAlgorithmType expected = CipherAlgorithmType.Null;
            Assert.AreEqual(expected, sslStream.CipherAlgorithm, "Cipher algorithm should  be NULL");
            Assert.AreEqual(0, sslStream.CipherStrength, "Cipher strength should be equal to 0");
            sslStream.Dispose();
            client.Dispose();
        }
    }
}

