// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
    internal static class CertificateValidation
    {
        internal static SslPolicyErrors BuildChainAndVerifyProperties(X509Chain chain, X509Certificate2 remoteCertificate, bool checkCertName, string hostName)
        {
            SslPolicyErrors errors = chain.Build(remoteCertificate) ?
                SslPolicyErrors.None :
                SslPolicyErrors.RemoteCertificateChainErrors;

            if (!checkCertName)
            {
                return errors;
            }

            if (string.IsNullOrEmpty(hostName))
            {
                return errors | SslPolicyErrors.RemoteCertificateNameMismatch;
            }

            int hostNameMatch;
            using (SafeX509Handle certHandle = Interop.Crypto.X509Duplicate(remoteCertificate.Handle))
            {
                IPAddress hostnameAsIp;
                if (IPAddress.TryParse(hostName, out hostnameAsIp))
                {
                    byte[] addressBytes = hostnameAsIp.GetAddressBytes();
                    hostNameMatch = Interop.Crypto.CheckX509IpAddress(certHandle, addressBytes, addressBytes.Length, hostName, hostName.Length);
                }
                else
                {
                    // The IdnMapping converts Unicode input into the IDNA punycode sequence.
                    // It also does host case normalization.  The bypass logic would be something
                    // like "all characters being within [a-z0-9.-]+"
                    // Since it's not documented as being thread safe, create a new one each time.
                    string matchName = new IdnMapping().GetAscii(hostName);
                    hostNameMatch = Interop.Crypto.CheckX509Hostname(certHandle, matchName, matchName.Length);
                }
            }

            Debug.Assert(hostNameMatch == 0 || hostNameMatch == 1, $"Expected 0 or 1 from CheckX509Hostname, got {hostNameMatch}");
            return hostNameMatch == 1 ?
                errors :
                errors | SslPolicyErrors.RemoteCertificateNameMismatch;
        }
    }
}
