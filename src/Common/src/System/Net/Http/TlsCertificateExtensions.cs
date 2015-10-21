// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Http
{
    /// <summary>
    /// Defines Extension methods which are meant to be re-used across WinHttp & UnixHttp Handlers
    /// </summary>
    internal static class TLSCertificateExtensions
    {
        private const string ClientCertificateOid = "1.3.6.1.5.5.7.3.2";

        /// <summary>
        ///   returns true if the X509 Certificate can be used  as SSL Client Certificate.
        /// </summary>
        private static bool IsClientCertificate(X509Certificate2 cert)
        {
            Debug.Assert(cert != null, "certificate cannot be null");

            bool foundEku = false;
            bool foundKeyUsages = false;
            bool isClientAuth = true;
            bool isDigitalSignature = true;
            foreach(X509Extension extension in cert.Extensions)
            {
                // check if the extension is an enhanced usage ext.
                // But do this only if needed. No point going over it, if we already have established that our cert has the
                // required extension.
                if (!foundEku)
                {
                    X509EnhancedKeyUsageExtension enhancedUsageExt = extension as X509EnhancedKeyUsageExtension;
                    if (enhancedUsageExt != null)
                    {
                        foundEku = true;
                        isClientAuth = false;
                        foreach(Oid oid in enhancedUsageExt.EnhancedKeyUsages)
                        {
                            if (string.Equals(ClientCertificateOid, oid.Value))
                            {
                                isClientAuth = true;
                                break;
                            }
                        }
                    }
                }

                /// Check if the extension is a key usage extension.
                /// No point going over it if we have already established that out cert has digital signature
                if (!foundKeyUsages)
                {
                    X509KeyUsageExtension usageExt = extension as X509KeyUsageExtension;
                    if (usageExt != null)
                    {
                        foundKeyUsages = true;
                        isDigitalSignature = (usageExt.KeyUsages & X509KeyUsageFlags.DigitalSignature) != 0;
                    }
                }

                if (foundKeyUsages && foundEku)
                {
                    break;
                }
            }

            return isClientAuth && isDigitalSignature;
        }

        /// <summary>
        ///   Returns a new collection containing valid client certificates from the given X509Certificate2Collection
        /// </summary>
        internal static bool TryFindClientCertificate(this X509Certificate2Collection certificates,
                                                      ISet<string> allowedIssuers,
                                                      out X509Certificate2 clientCertificate,
                                                      out X509Chain clientCertChain)
        {
            clientCertificate = null;
            clientCertChain = null;
            if (certificates == null)
            {
                return false;
            }

            DateTime now = DateTime.Now;
            foreach(X509Certificate2 cert in certificates)
            {
                if (cert.HasPrivateKey && now > cert.NotBefore && now < cert.NotAfter)
                {
                    if (IsClientCertificate(cert))
                    {
                        X509Chain chain = new X509Chain();
                        //TODO (#2204) set it to AllFlag when 2204 is fixed.
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                        chain.ChainPolicy.ApplicationPolicy.Add(new Oid(ClientCertificateOid));
                        if (!chain.Build(cert))
                        {
                            // TODO: uncomment 'continue' based on decision around 2204

                            // Ideally, we would like to set VerificationFlags = AllFlags
                            // But we are getting "work in progress" exception here when verificationFlags is anything but NoFlag
                            // continue;
                        }

                        if (chain.ChainElements.Count > 0)
                        {
                            if (allowedIssuers.Count == 0)
                            {
                                clientCertificate = cert;
                                clientCertChain = chain;
                                return true;
                            }

                            X509Certificate2 trustAnchor = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                            if (allowedIssuers.Contains(trustAnchor.SubjectName.Name))
                            {
                                clientCertificate = cert;
                                clientCertChain = chain;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
