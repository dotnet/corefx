// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography
{
    public enum X509SelectionFlag
    {
        SingleSelection = 0x00,
        MultiSelection = 0x01
    }

    internal static class X509Utils
    {
        internal static SafeCertContextHandle GetCertContext(X509Certificate2 certificate)
        {
            SafeCertContextHandle safeCertContext = CAPI.CertDuplicateCertificateContext(certificate.Handle);
            GC.KeepAlive(certificate);
            return safeCertContext;
        }

        internal static SafeCertStoreHandle ExportToMemoryStore(X509Certificate2Collection collection)
        {
            SafeCertStoreHandle safeCertStoreHandle = SafeCertStoreHandle.InvalidHandle;

            // we always want to use CERT_STORE_ENUM_ARCHIVED_FLAG since we want to preserve the collection in this operation.
            // By default, Archived certificates will not be included.

            safeCertStoreHandle = CAPI.CertOpenStore(new IntPtr(CAPI.CERT_STORE_PROV_MEMORY),
                                                     CAPI.X509_ASN_ENCODING | CAPI.PKCS_7_ASN_ENCODING,
                                                     IntPtr.Zero,
                                                     CAPI.CERT_STORE_ENUM_ARCHIVED_FLAG | CAPI.CERT_STORE_CREATE_NEW_FLAG,
                                                     null);

            if (safeCertStoreHandle == null || safeCertStoreHandle.IsInvalid)
                throw new CryptographicException(Marshal.GetLastWin32Error());

            //
            // We use CertAddCertificateLinkToStore to keep a link to the original store, so any property changes get
            // applied to the original store. This has a limit of 99 links per cert context however.
            //

            foreach (X509Certificate2 x509 in collection)
            {
                if (!CAPI.CertAddCertificateLinkToStore(safeCertStoreHandle,
                                                        X509Utils.GetCertContext(x509),
                                                        CAPI.CERT_STORE_ADD_ALWAYS,
                                                        SafeCertContextHandle.InvalidHandle))
                    throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            return safeCertStoreHandle;
        }

        internal static X509Certificate2Collection GetCertificates(SafeCertStoreHandle safeCertStoreHandle)
        {
            X509Certificate2Collection collection = new X509Certificate2Collection();
            IntPtr pEnumContext = CAPI.CertEnumCertificatesInStore(safeCertStoreHandle, IntPtr.Zero);
            while (pEnumContext != IntPtr.Zero)
            {
                X509Certificate2 certificate = new X509Certificate2(pEnumContext);
                collection.Add(certificate);
                pEnumContext = CAPI.CertEnumCertificatesInStore(safeCertStoreHandle, pEnumContext);
            }
            return collection;
        }
    }
}
