// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
    internal static partial class CertificateHelper
    {
        private const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";

        internal static X509Certificate2 GetEligibleClientCertificate(X509CertificateCollection candidateCerts)
        {
            if (candidateCerts.Count == 0)
            {
                return null;
            }

            var certs = new X509Certificate2Collection();
            certs.AddRange(candidateCerts);

            return GetEligibleClientCertificate(certs);
        }

        internal static X509Certificate2 GetEligibleClientCertificate(X509Certificate2Collection candidateCerts)
        {
            if (candidateCerts.Count == 0)
            {
                return null;
            }

            foreach (X509Certificate2 cert in candidateCerts)
            {
                // Must have private key.
                if (!cert.HasPrivateKey)
                {
                    continue;
                }
                
                // If an extension is missing then it will automatically match.
                bool isMatch = true;

                foreach (X509Extension extension in cert.Extensions)
                {
                    if (extension is X509EnhancedKeyUsageExtension ekus)
                    {
                        isMatch = false;

                        foreach (Oid oid in ekus.EnhancedKeyUsages)
                        {
                            if (oid.Value == ClientAuthenticationOID)
                            {
                                 isMatch = true;
                                 break;
                             }
                         }
                      }
                      else if (extension is X509KeyUsageExtension ku)
                      {
                           const X509KeyUsageFlags requiredUsages = X509KeyUsageFlags.DigitalSignature;

                           isMatch = (ku.KeyUsages & requiredUsages) == requiredUsages;
                      }

                    if (!isMatch)
                    {
                        break;
                    }
                }

                if (isMatch)
                {
                    return cert;
                }
            }

            return null;
        }
    }
}
