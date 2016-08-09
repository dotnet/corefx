using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Security.Tests
{
    public class TlsTestServer
    {
        private readonly X509Certificate _serverCertificate;
        private readonly int _port;
        private readonly IInspectionTest _inspectionTest;

        private const string ResponseStringTemplate =
@"HTTP/1.1 200 OK
Connection: close

<html>
<head>
<title>Test Server</title>
</head>
<body>
<h1>TLS test server</h1>
<h3>TLS Connection information:</h3>
<pre>CipherAlgorithm:        {0}</pre>
<pre>CipherStrength:         {1}</pre>
<pre>HashAlgorithm:          {2}</pre>
<pre>HashStrength:           {3}</pre>
<pre>KeyExchangeAlgorithm:   {4}</pre>
<pre>KeyExchangeStrength:    {5}</pre>
<hr>
<pre>Beast 1/n-1:            {6}</pre>
</body>
</html>

";

        public TlsTestServer(X509Certificate serverCertificate, int port, IInspectionTest test)
        {
            _serverCertificate = serverCertificate;
            _port = port;
            _inspectionTest = test;
            BeastMitigationDetected = false;
        }

        public bool BeastMitigationDetected
        {
            get;
            private set;
        }

        public async Task RunTest()
        {
            IPAddress address = IPAddress.Any;

            var listener = new TcpListener(address, _port);

            listener.Start(1);

            Console.WriteLine("[Server] waiting for connections ({0}:{1})", address, _port);
            bool done = false;

            while (!done)
            {
                try
                {
                    using (TcpClient requestClient = await listener.AcceptTcpClientAsync())
                    {
                        Console.WriteLine("[Server] Client connected.");

                        var inspectionStream = new InspectionNetworkStream(requestClient.GetStream(), _inspectionTest.OnServerSocketOperation);

                        using (var tls = new SslStream(inspectionStream))
                        {
                            await tls.AuthenticateAsServerAsync(
                            _serverCertificate,
                            false,
                            System.Security.Authentication.SslProtocols.Tls,
                            false);

                            Console.WriteLine("[Server] Client authenticated.");

                            done = await HttpConversation(tls);
                        }
                    }
                }
                catch (IOException)
                {
                    // Ignore I/O issues as browsers attempt to connect only to detect crypto information.
                }
            }
        }

        private async Task<bool> HttpConversation(SslStream tls)
        {
            int totalBytesRead = 0;
            int chunks = 0;

            while (totalBytesRead < 5)
            {
                var requestBuffer = new byte[2048];
                int bytesRead = await tls.ReadAsync(requestBuffer, 0, requestBuffer.Length);
                totalBytesRead += bytesRead;

                string requestString = Encoding.UTF8.GetString(requestBuffer, 0, bytesRead);

                Console.WriteLine("[Server] Received {0} bytes: <<<{1}>>>", bytesRead, requestString);
                if (bytesRead == 0)
                {
                    return false;
                }

                if (bytesRead == 1 && chunks == 0)
                {
                    BeastMitigationDetected = true;
                }

                chunks++;
            }

            string responseString = string.Format(
                ResponseStringTemplate, 
                tls.CipherAlgorithm,
                tls.CipherStrength,
                tls.HashAlgorithm,
                tls.HashStrength,
                tls.KeyExchangeAlgorithm,
                tls.KeyExchangeStrength,
                BeastMitigationDetected);

            
            byte[] responseBuffer = Encoding.UTF8.GetBytes(responseString);
            //byte[] responseBuffer = new byte [1]; responseBuffer[0] = (byte)'S';
            
            int batchLength = responseBuffer.Length / 2 + 2;

            await tls.WriteAsync(responseBuffer, 0, batchLength);
            await tls.WriteAsync(responseBuffer, batchLength, responseBuffer.Length - batchLength);

            Console.WriteLine("[Server] Replied with {0} bytes.", responseBuffer.Length);
            return true;
        }
    }
}
