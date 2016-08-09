using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Security.Tests
{
    public class TlsTestClient
    {
        private readonly string _server;
        private readonly int _port;
        private readonly IInspectionTest _inspectionTest;

        public TlsTestClient(string server, int port, IInspectionTest test)
        {
            _server = server;
            _port = port;
            _inspectionTest = test;
        }

        public async Task RunTest()
        {
            using (var tcp = new TcpClient())
            {
                Console.WriteLine("[Client] Connecting to {0}:{1}", _server, _port);
                await tcp.ConnectAsync(_server, _port);

                var inspectionStream = new InspectionNetworkStream(tcp.GetStream(), _inspectionTest.OnClientSocketOperation);

                using (var tls = new SslStream(inspectionStream, false, CertificateValidation))
                {
                    Console.WriteLine("[Client] Connected. Authenticating...");
                    await tls.AuthenticateAsClientAsync(_server, null, System.Security.Authentication.SslProtocols.Tls, false);

                    string requestString = "GET / HTTP/1.0\r\nHost: hostwithalongname.test.somehost.abc\r\nUser-Agent: Testing application\r\n\r\n";
                    byte[] requestBuffer = Encoding.UTF8.GetBytes(requestString);
                    //byte[] requestBuffer = new byte[1]; requestBuffer[0] = (byte)'C'; 

                    Console.WriteLine("[Client] Sending request ({0} Bytes)", requestBuffer.Length);

                    await tls.WriteAsync(requestBuffer, 0, requestBuffer.Length);

                    Console.WriteLine("[Client] Waiting for reply...");

                    int bytesRead = 0;
                    int segment = 0;
                    do
                    {
                        segment++;

                        byte[] responseBuffer = new byte[2048];
                        bytesRead = await tls.ReadAsync(responseBuffer, 0, responseBuffer.Length);

                        if (bytesRead > 0)
                        {
                            string responseString = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                            Console.WriteLine("[Client {0}: {2} Bytes] Response: <<<<<{1}>>>>>", segment, responseString, bytesRead);
                        }
                    }
                    while (bytesRead > 0);
                }
            }
        }

        private bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return _inspectionTest.OnCertificateValidation(certificate, chain, sslPolicyErrors);
        }
    }
}
