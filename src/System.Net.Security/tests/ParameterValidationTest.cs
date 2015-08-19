namespace NCLTest.Security
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;

    [TestClass]
    public class ParameterValidationTest
    {
        private DummyTcpServer remoteServer;

        [TestInitialize]
        public void StartInternalServer()
        {
            remoteServer = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 600), EncryptionPolicy.RequireEncryption);
        }

        [TestCleanup]
        public void StopInternalServer()
        {
            remoteServer.Dispose();
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
        [ExpectedException(typeof(System.ArgumentException))]
        public void SslStreamConstructor_BadEncryptionPolicy_ThrowException()
        {
            SslStream sslStream;
            TcpClient client;

            client = new TcpClient();
            client.Connect(remoteServer.RemoteEndPoint);

            try
            {
                sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, (EncryptionPolicy)100);
                Assert.Fail("SslStream constructor should have thrown exception");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}

