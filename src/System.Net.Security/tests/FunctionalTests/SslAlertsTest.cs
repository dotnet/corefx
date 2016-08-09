using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Security.Tests
{
    class SslAlertsTest : IInspectionTest
    {
        private int _clientRecordsReadCount = 0;
        private int _clientRecordsWrittenCount = 0;

        private int _serverRecordsReadCount = 0;
        private int _serverRecordsWrittenCount = 0;

        public bool OnCertificateValidation(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void OnClientSocketOperation(byte[] buffer, int offset, int count, int bytesRead, InspectionNetworkStream.Direction direction)
        {
            switch (direction)
            {
                case InspectionNetworkStream.Direction.Read:
                    _clientRecordsReadCount++;
                    Console.WriteLine("[ClientInspection]: Read {0}", _clientRecordsReadCount);
                    
                    // TODO: Causes Hang
                    //buffer[5] = 123;

                    break;
                case InspectionNetworkStream.Direction.Write:
                    _clientRecordsWrittenCount++;
                    Console.WriteLine("[ClientInspection]: Write {0}", _clientRecordsWrittenCount);

                    if (_clientRecordsWrittenCount == 1)
                    {
                        // Client Hello corrupted: Client throws. OpenSSL sends alert.
                        //buffer[offset + count - 1] = (byte)'X';
                    }
                    break;

                default:
                    break;
            }
        }

        public void OnServerSocketOperation(byte[] buffer, int offset, int count, int bytesRead, InspectionNetworkStream.Direction direction)
        {
            switch (direction)
            {
                case InspectionNetworkStream.Direction.Read:
                    _serverRecordsReadCount++;
                    Console.WriteLine("[ServerInspection]: Read {0}", _serverRecordsReadCount);
                    

                    break;
                case InspectionNetworkStream.Direction.Write:
                    _serverRecordsWrittenCount++;
                    Console.WriteLine("[ServerInspection]: Write {0}", _serverRecordsWrittenCount);

                    if (_serverRecordsWrittenCount == 1)
                    {
                        //Console.WriteLine("[ServerInspection]: Corrupting buffer");

                        // Server Hello/Certificate - Server throws.
                        //buffer[offset + count - 6] = (byte)'X';
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
