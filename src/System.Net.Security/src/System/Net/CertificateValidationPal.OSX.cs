// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace System.Net
{
    internal static partial class CertificateValidationPal
    {
        internal static SslPolicyErrors VerifyCertificateProperties(
            SafeDeleteContext securityContext,
            X509Chain chain,
            X509Certificate2 remoteCertificate,
            bool checkCertName,
            bool isServer,
            string hostName)
        {
            SslPolicyErrors errors = SslPolicyErrors.None;

            if (remoteCertificate == null)
            {
                errors |= SslPolicyErrors.RemoteCertificateNotAvailable;
            }
            else
            {
                if (!chain.Build(remoteCertificate))
                {
                    errors |= SslPolicyErrors.RemoteCertificateChainErrors;
                }

                if (!isServer && checkCertName)
                {
                    SafeDeleteSslContext sslContext = (SafeDeleteSslContext)securityContext;

                    if (!Interop.AppleCrypto.SslCheckHostnameMatch(sslContext.SslContext, hostName))
                    {
                        errors |= SslPolicyErrors.RemoteCertificateNameMismatch;
                    }
                }
            }

            return errors;
        }

        //
        // Extracts a remote certificate upon request.
        //
        internal static X509Certificate2 GetRemoteCertificate(
            SafeDeleteContext securityContext,
            out X509Certificate2Collection remoteCertificateStore)
        {
            remoteCertificateStore = null;

            if (securityContext == null)
            {
                return null;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Enter(securityContext);

            SafeSslHandle sslContext = ((SafeDeleteSslContext)securityContext).SslContext;

            if (sslContext == null)
            {
                return null;
            }

            X509Certificate2 result = null;

            using (SafeX509ChainHandle chainHandle = Interop.AppleCrypto.SslCopyCertChain(sslContext))
            {
                long chainSize = Interop.AppleCrypto.X509ChainGetChainSize(chainHandle);
                X509Certificate2Collection collection = new X509Certificate2Collection();

                for (int i = 0; i < chainSize; i++)
                {
                    IntPtr certHandle = Interop.AppleCrypto.X509ChainGetCertificateAtIndex(chainHandle, i);
                    collection.Add(new X509Certificate2(certHandle));
                }

                if (chainSize > 0)
                {
                    result = collection[0];
                }

                remoteCertificateStore = collection;
            }

            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Log.RemoteCertificate(result);
                NetEventSource.Exit(securityContext, result);
            }

            return result;
        }

        //
        // Used only by client SSL code, never returns null.
        //
        internal static string[] GetRequestCertificateAuthorities(SafeDeleteContext securityContext)
        {
            SafeSslHandle sslContext = ((SafeDeleteSslContext)securityContext).SslContext;

            if (sslContext == null)
            {
                return Array.Empty<string>();
            }

            using (SafeCFArrayHandle dnArray = Interop.AppleCrypto.SslCopyCADistinguishedNames(sslContext))
            {
                long size = Interop.CoreFoundation.CFArrayGetCount(dnArray);

                if (size == 0)
                {
                    return Array.Empty<string>();
                }

                string[] distinguishedNames = new string[size];

                for (int i = 0; i < size; i++)
                {
                    IntPtr element = Interop.CoreFoundation.CFArrayGetValueAtIndex(dnArray, i);

                    using (SafeCFDataHandle cfData = new SafeCFDataHandle(element, ownsHandle: false))
                    {
                        byte[] dnData = Interop.CoreFoundation.CFGetData(cfData);
                        X500DistinguishedName dn = new X500DistinguishedName(dnData);
                        distinguishedNames[i] = dn.Name;
                    }
                }

                return distinguishedNames;
            }
        }

        internal static X509Store EnsureStoreOpened(bool isMachineStore)
        {
            return null;
        }
    }
}
