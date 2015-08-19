namespace NCLTest.Security
{
    using CoreFXTestLibrary;
    using NCLTest.Common;
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Cryptography.X509Certificates;

    [TestClass]
    public class TransportContextTest
    {
        private DummyTcpServer testServer;

        [TestInitialize]
        public void StartInternalServer()
        {
            testServer = new DummyTcpServer(
                new IPEndPoint(IPAddress.Loopback, 400), EncryptionPolicy.RequireEncryption);
        }

        [TestCleanup]
        public void StopInternalServer()
        {
            testServer.Dispose();
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
        public void TransportContext_ConnectToServerWithSsl_GetExpectedChannelBindings()
        {
            TcpClient client = null;
            NetworkStream stream = null;
            SslStream sslStream = null;
            TransportContext context = null;

            try 
	        {
                client = new TcpClient();
                client.Connect(testServer.RemoteEndPoint);

                stream = client.GetStream();
                sslStream = new SslStream(client.GetStream(), false, AllowAnyServerCertificate, null, EncryptionPolicy.RequireEncryption);
                sslStream.AuthenticateAsClient("localhost", null, SslProtocols.Tls, false);

                context = sslStream.TransportContext;
                CheckTransportContext(context);
	        }
            finally
            {
                if (sslStream != null)
                {
                    sslStream.Dispose();
                }
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }

        public void CheckTransportContext(TransportContext context)
        {
            var cbt1 = context.GetChannelBinding(ChannelBindingKind.Endpoint);
            var cbt2 = context.GetChannelBinding(ChannelBindingKind.Unique);
            var cbt3 = context.GetChannelBinding(ChannelBindingKind.Unknown);

            CheckChannelBinding(cbt1);
            CheckChannelBinding(cbt2);
            CheckChannelBinding(cbt3);

            Assert.IsTrue(cbt1 != null, "ChannelBindingKind.Endpoint token data should be returned from SCHANNEL.");
            Assert.IsTrue(cbt2 != null, "ChannelBindingKind.Unique token data should be returned from SCHANNEL.");
            Assert.IsTrue(cbt3 == null, "ChannelBindingKind.Unknown token data should not be returned from SCHANNEL since it does not map to a defined context attribute.");
        }

        public void CheckChannelBinding(ChannelBinding channelBinding)
        {
            try
            {
                if (channelBinding != null)
                {
                    Assert.IsTrue(!channelBinding.IsInvalid, "Channel binding token should be marked as a valid SafeHandle.");
                    Assert.IsTrue(channelBinding.Size > 0, "Number of bytes in a valid channel binding token should be greater than zero.");
                    var bytes = new byte[channelBinding.Size];
                    Marshal.Copy(channelBinding.DangerousGetHandle(), bytes, 0, channelBinding.Size);
                    Assert.AreEqual(channelBinding.Size, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
