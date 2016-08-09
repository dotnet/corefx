using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Security.Tests
{
    public interface IInspectionTest
    {
        void OnClientSocketOperation(byte [] buffer, int offset, int count, int bytesRead, InspectionNetworkStream.Direction direction);

        void OnServerSocketOperation(byte[] buffer, int offset, int count, int bytesRead, InspectionNetworkStream.Direction direction);

        bool OnCertificateValidation(X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);
    }
}
