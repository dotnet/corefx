
using System.Net.Test.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Security.Tests
{
    public class TlsTestBench
    {
        private readonly X509Certificate2 _clientCertificate;
        private readonly X509Certificate2Collection _clientCertificateCollection;
        private readonly X509Certificate2 _serverCertificate;
        private readonly X509Certificate2Collection _serverCertificateCollection;

        public TlsTestBench()
        {
            _serverCertificateCollection = Configuration.Certificates.GetServerCertificateCollection();
            _serverCertificate = Configuration.Certificates.GetServerCertificate();

            _clientCertificateCollection = Configuration.Certificates.GetClientCertificateCollection();
            _clientCertificate = Configuration.Certificates.GetClientCertificate();
        }
        
        //[Fact]
        public async Task TlsTest_Server()
        {
            IInspectionTest sslAlertFilter = new SslAlertsTest();

            var server = new TlsTestServer(_serverCertificate, 4431, sslAlertFilter);
            await server.RunTest();
        }

        //[Fact]
        public async Task TlsTest_Client()
        {
            IInspectionTest sslAlertFilter = new SslAlertsTest();

            var server = new TlsTestClient("localhost", 4431, sslAlertFilter);
            await server.RunTest();
        }
    }
}
