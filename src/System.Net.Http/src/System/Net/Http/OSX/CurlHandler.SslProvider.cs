// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using CURLcode = Interop.Http.CURLcode;

namespace System.Net.Http
{
    internal partial class CurlHandler : HttpMessageHandler
    {
        private static class SslProvider
        {
            internal static void SetSslOptions(EasyRequest easy, ClientCertificateOption clientCertOption)
            {
                Debug.Assert(clientCertOption == ClientCertificateOption.Automatic || clientCertOption == ClientCertificateOption.Manual);

                // Create a client certificate provider if client certs may be used.
                X509Certificate2Collection clientCertificates = easy._handler._clientCertificates;

                if (clientCertOption != ClientCertificateOption.Manual || clientCertificates?.Count > 0)
                {
                    throw new NotImplementedException("Can't handle client certs yet");
                }

                SetSslVersion(easy);

                if (easy._handler.ServerCertificateValidationCallback != null)
                {
                    // If the caller gives no callback things are easy, we just let things happen.
                    // If they give one and the OS call succeeds we could let the callback abort.
                    // The trouble comes if the OS call fails.
                    //  * Disabling VERIFYPEER will allow the chain to be built, so that could be fine.
                    //  * Disabling VERIFYHOST would allow the localhost cert override, but ALSO disables SNI.
                    //
                    // So, a "if it fails, disable an option and try again" means that disabling VERIFYHOST could
                    // change the matched cert (SNI disabled), but VERIFYPEER would work.
                    //
                    // Also, if we disable VERIFYPEER for all server cert vallbacks we'd have a chance to have
                    // no callback fail (OS check) and custom callback report no errors; because the OS did
                    // something we didn't.
                    //
                    // It's a real pickle.
                    //
                    // Allowing a true to become a false is possible by registering for the WRITE callback,
                    // and querying CURLINFO_TLS_SSL_PTR.  (Be sure to check that it's CURLSSLBACKEND_DARWINSSL)
                    throw new NotImplementedException("Intercept the WRITE callback and allow a sniff or success->fail callback.");
                }

                // Curl doesn't expose an option to disable revocation checking for Secure Transport/darwinssl,
                // execpt (perhaps) for disabling VERIFYPEER.
            }

            private static void SetSslVersion(EasyRequest easy)
            {
                // Get the requested protocols.
                SslProtocols protocols = easy._handler.SslProtocols;
                if (protocols == SslProtocols.None)
                {
                    // Let libcurl use its defaults if None is set.
                    return;
                }

                // We explicitly disallow choosing SSL2/3. Make sure they were filtered out.
                Debug.Assert((protocols & ~SecurityProtocol.AllowedSecurityProtocols) == 0,
                    "Disallowed protocols should have been filtered out.");

                // libcurl supports options for either enabling all of the TLS1.* protocols or enabling 
                // just one of them; it doesn't currently support enabling two of the three, e.g. you can't 
                // pick TLS1.1 and TLS1.2 but not TLS1.0, but you can select just TLS1.2.
                Interop.Http.CurlSslVersion curlSslVersion;
                switch (protocols)
                {
                    case SslProtocols.Tls:
                        curlSslVersion = Interop.Http.CurlSslVersion.CURL_SSLVERSION_TLSv1_0;
                        break;
                    case SslProtocols.Tls11:
                        curlSslVersion = Interop.Http.CurlSslVersion.CURL_SSLVERSION_TLSv1_1;
                        break;
                    case SslProtocols.Tls12:
                        curlSslVersion = Interop.Http.CurlSslVersion.CURL_SSLVERSION_TLSv1_2;
                        break;

                    case SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12:
                        curlSslVersion = Interop.Http.CurlSslVersion.CURL_SSLVERSION_TLSv1;
                        break;

                    default:
                        throw new NotSupportedException(SR.net_securityprotocolnotsupported);
                }

                try
                {
                    easy.SetCurlOption(Interop.Http.CURLoption.CURLOPT_SSLVERSION, (long)curlSslVersion);
                }
                catch (CurlException e) when (e.HResult == (int)CURLcode.CURLE_UNKNOWN_OPTION)
                {
                    throw new NotSupportedException(SR.net_securityprotocolnotsupported, e);
                }
            }
        }
    }
}
