/// <summary>
/// Extensions that add the legacy APM Pattern (Begin/End) for generic Streams
/// </summary>
namespace System.Net.Security
{
    using System.Threading.Tasks;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
        
    public static class SslStreamAPMExtensions
    {
        public static IAsyncResult BeginAuthenticateAsClient(
            this SslStream s,
            string targetHost,
            X509CertificateCollection clientCertificates,
            SslProtocols enabledSslProtocols,
            bool checkCertificateRevocation,
            AsyncCallback asyncCallback,
            Object asyncState)
        {
            return s.AuthenticateAsClientAsync(
                targetHost,
                clientCertificates,
                enabledSslProtocols,
                checkCertificateRevocation).ToApm(asyncCallback, asyncState);
        }

        public static void EndAuthenticateAsClient(this SslStream s, IAsyncResult asyncResult)
        {
            Task t = (Task)asyncResult;
            t.GetAwaiter().GetResult();
        }

        public static IAsyncResult BeginAuthenticateAsServer(
            this SslStream s,
            X509Certificate serverCertificate,
            bool clientCertificateRequired,
            SslProtocols enabledSslProtocols,
            bool checkCertificateRevocation,
            AsyncCallback asyncCallback,
            Object asyncState)
        {
            return s.AuthenticateAsServerAsync(
                serverCertificate,
                clientCertificateRequired,
                enabledSslProtocols,
                checkCertificateRevocation).ToApm(asyncCallback, asyncState);
        }

        public static void EndAuthenticateAsServer(this SslStream s, IAsyncResult asyncResult)
        {
            Task t = (Task)asyncResult;
            t.GetAwaiter().GetResult();
        }
    }
}
