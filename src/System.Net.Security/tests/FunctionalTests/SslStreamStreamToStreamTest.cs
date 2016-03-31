// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Net.Test.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Security.Tests
{
    public class SslStreamStreamToStreamTest
    {
        private readonly byte[] _sampleMsg = Encoding.UTF8.GetBytes("Sample Test Message");

        [ActiveIssue(4467)]
        [Fact]
        public void SslStream_StreamToStream_Authentication_Success()
        {
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var server = new SslStream(serverStream))
            {
                X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(certificate.GetNameInfo(X509NameType.SimpleName, false));
                auth[1] = server.AuthenticateAsServerAsync(certificate);

                bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Handshake completed in the allotted time");
            }
        }

        [Fact]
        public void SslStream_StreamToStream_Authentication_IncorrectServerName_Fail()
        {
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new SslStream(clientStream))
            using (var server = new SslStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync("incorrectServer");
                auth[1] = server.AuthenticateAsServerAsync(TestConfiguration.GetServerCertificate());

                Assert.Throws<AuthenticationException>(() =>
                {
                    auth[0].GetAwaiter().GetResult();
                });

                auth[1].GetAwaiter().GetResult();
            }
        }

        [Fact]
        public void SslStream_StreamToStream_Successive_ClientWrite_Sync_Success()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var clientSslStream = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var serverSslStream = new SslStream(serverStream))
            {
                bool result = DoHandshake(clientSslStream, serverSslStream);

                Assert.True(result, "Handshake completed.");

                clientSslStream.Write(_sampleMsg);

                serverSslStream.Read(recvBuf, 0, _sampleMsg.Length);

                Assert.True(VerifyOutput(recvBuf, _sampleMsg), "verify first read data is as expected.");

                clientSslStream.Write(_sampleMsg);

                serverSslStream.Read(recvBuf, 0, _sampleMsg.Length);

                Assert.True(VerifyOutput(recvBuf, _sampleMsg), "verify second read data is as expected.");
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SslStream_StreamToStream_LargeWrites_Sync_Success(bool randomizedData)
        {
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer:false))
            using (var serverStream = new VirtualNetworkStream(network, isServer:true))
            using (var clientSslStream = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var serverSslStream = new SslStream(serverStream))
            {
                Assert.True(DoHandshake(clientSslStream, serverSslStream), "Handshake complete");

                byte[] largeMsg = new byte[4096 * 5]; // length longer than max read chunk size (16K + headers)
                if (randomizedData)
                {
                    new Random().NextBytes(largeMsg); // not very compressible
                }
                else
                {
                    for (int i = 0; i < largeMsg.Length; i++)
                    {
                        largeMsg[i] = (byte)i; // very compressible
                    }
                }
                byte[] receivedLargeMsg = new byte[largeMsg.Length];

                // First do a large write and read blocks at a time
                clientSslStream.Write(largeMsg);
                int bytesRead = 0, totalRead = 0;
                while (totalRead < largeMsg.Length &&
                    (bytesRead = serverSslStream.Read(receivedLargeMsg, totalRead, receivedLargeMsg.Length - totalRead)) != 0)
                {
                    totalRead += bytesRead;
                }
                Assert.Equal(receivedLargeMsg.Length, totalRead);
                Assert.Equal(largeMsg, receivedLargeMsg);

                // Then write again and read bytes at a time
                clientSslStream.Write(largeMsg);
                foreach (byte b in largeMsg)
                {
                    Assert.Equal(b, serverSslStream.ReadByte());
                }
            }
        }

        [Fact]
        public void SslStream_StreamToStream_Successive_ClientWrite_Async_Success()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var clientSslStream = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var serverSslStream = new SslStream(serverStream))
            {
                bool result = DoHandshake(clientSslStream, serverSslStream);

                Assert.True(result, "Handshake completed.");

                Task[] tasks = new Task[2];

                tasks[0] = serverSslStream.ReadAsync(recvBuf, 0, _sampleMsg.Length);
                tasks[1] = clientSslStream.WriteAsync(_sampleMsg, 0, _sampleMsg.Length);
                bool finished = Task.WaitAll(tasks, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Send/receive completed in the allotted time");
                Assert.True(VerifyOutput(recvBuf, _sampleMsg), "verify first read data is as expected.");

                tasks[0] = serverSslStream.ReadAsync(recvBuf, 0, _sampleMsg.Length);
                tasks[1] = clientSslStream.WriteAsync(_sampleMsg, 0, _sampleMsg.Length);
                finished = Task.WaitAll(tasks, TestConfiguration.PassingTestTimeoutMilliseconds);
                Assert.True(finished, "Send/receive completed in the allotted time");
                Assert.True(VerifyOutput(recvBuf, _sampleMsg), "verify second read data is as expected.");
            }
        }

        [Fact]
        public void SslStream_StreamToStream_Write_ReadByte_Success()
        {
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer:false))
            using (var serverStream = new VirtualNetworkStream(network, isServer:true))
            using (var clientSslStream = new SslStream(clientStream, false, AllowAnyServerCertificate))
            using (var serverSslStream = new SslStream(serverStream))
            {
                bool result = DoHandshake(clientSslStream, serverSslStream);
                Assert.True(result, "Handshake completed.");

                for (int i = 0; i < 3; i++)
                {
                    clientSslStream.Write(_sampleMsg);
                    foreach (byte b in _sampleMsg)
                    {
                        Assert.Equal(b, serverSslStream.ReadByte());
                    }
                }
            }
        }

        private bool VerifyOutput(byte[] actualBuffer, byte[] expectedBuffer)
        {
            return expectedBuffer.SequenceEqual(actualBuffer);
        }

        private bool AllowAnyServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            SslPolicyErrors expectedSslPolicyErrors = SslPolicyErrors.None;

            if (!Capability.IsTrustedRootCertificateInstalled())
            {
                expectedSslPolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors;
            }

            Assert.Equal(expectedSslPolicyErrors, sslPolicyErrors);

            if (sslPolicyErrors == expectedSslPolicyErrors)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DoHandshake(SslStream clientSslStream, SslStream serverSslStream)
        {
            X509Certificate2 certificate = TestConfiguration.GetServerCertificate();
            Task[] auth = new Task[2];

            auth[0] = clientSslStream.AuthenticateAsClientAsync(certificate.GetNameInfo(X509NameType.SimpleName, false));
            auth[1] = serverSslStream.AuthenticateAsServerAsync(certificate);

            bool finished = Task.WaitAll(auth, TestConfiguration.PassingTestTimeoutMilliseconds);
            return finished;
        }
    }
}
