// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Apple;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed partial class StorePal
    {
        public static IStorePal FromHandle(IntPtr storeHandle)
        {
            throw new PlatformNotSupportedException();
        }

        public static ILoaderPal FromBlob(byte[] rawData, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
        {
            Debug.Assert(password != null);

            SafeTemporaryKeychainHandle tmpKeychain = Interop.AppleCrypto.CreateTemporaryKeychain();

            try
            {
                SafeCFArrayHandle certs = Interop.AppleCrypto.X509ImportCollection(rawData, password, tmpKeychain);
                return new AppleCertLoader(certs, tmpKeychain);
            }
            catch
            {
                tmpKeychain.Dispose();
                throw;
            }
        }

        public static ILoaderPal FromFile(string fileName, SafePasswordHandle password, X509KeyStorageFlags keyStorageFlags)
        {
            Debug.Assert(password != null);

            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return FromBlob(fileBytes, password, keyStorageFlags);
        }

        public static IExportPal FromCertificate(ICertificatePal cert)
        {
            return new AppleCertificateExporter(cert);
        }

        public static IExportPal LinkFromCertificateCollection(X509Certificate2Collection certificates)
        {
            return new AppleCertificateExporter(certificates);
        }

        public static IStorePal FromSystemStore(string storeName, StoreLocation storeLocation, OpenFlags openFlags)
        {
            throw new NotImplementedException();
        }

        private sealed class AppleCertLoader : ILoaderPal
        {
            private readonly SafeCFArrayHandle _collectionHandle;
            private readonly SafeTemporaryKeychainHandle _tmpKeychain;

            public AppleCertLoader(SafeCFArrayHandle collectionHandle, SafeTemporaryKeychainHandle tmpKeychain)
            {
                _collectionHandle = collectionHandle;
                _tmpKeychain = tmpKeychain;
            }

            public void Dispose()
            {
                _collectionHandle.Dispose();
                _tmpKeychain.Dispose();
            }

            public void MoveTo(X509Certificate2Collection collection)
            {
                long longCount = Interop.CoreFoundation.CFArrayGetCount(_collectionHandle);

                if (longCount > int.MaxValue)
                    throw new CryptographicException();

                int count = (int)longCount;

                // Apple returns things in the opposite order from Windows, so read backwards.
                for (int i = count - 1; i >= 0; i--)
                {
                    IntPtr handle = Interop.CoreFoundation.CFArrayGetValueAtIndex(_collectionHandle, i);

                    if (handle != IntPtr.Zero)
                    {
                        ICertificatePal certPal = CertificatePal.FromHandle(handle, throwOnFail: false);

                        if (certPal != null)
                        {
                            X509Certificate2 cert = new X509Certificate2(certPal);
                            collection.Add(cert);
                        }
                    }
                }
            }
        }

        private sealed class AppleCertificateExporter : IExportPal
        {
            private X509Certificate2Collection _certs;
            private ICertificatePal _singleCertPal;

            public AppleCertificateExporter(ICertificatePal cert)
            {
                _singleCertPal = cert;
            }

            public AppleCertificateExporter(X509Certificate2Collection certs)
            {
                _certs = certs;
            }

            public void Dispose()
            {
                // Don't dispose any of the resources, they're still owned by the caller.
                _singleCertPal = null;
                _certs = null;
            }

            public byte[] Export(X509ContentType contentType, SafePasswordHandle password)
            {
                Debug.Assert(password != null);
                switch (contentType)
                {
                    case X509ContentType.Cert:
                        return ExportX509Der();
                    case X509ContentType.Pfx:
                        return ExportPfx(password);
                    case X509ContentType.Pkcs7:
                        return ExportPkcs7();
                    case X509ContentType.SerializedCert:
                    case X509ContentType.SerializedStore:
                        throw new PlatformNotSupportedException(SR.Cryptography_Unix_X509_SerializedExport);
                    default:
                        throw new CryptographicException(SR.Cryptography_X509_InvalidContentType);
                }
            }

            private byte[] ExportX509Der()
            {
                if (_singleCertPal != null)
                {
                    return _singleCertPal.RawData;
                }

                // Windows/Desktop compatibility: Exporting a collection (or store) as
                // X509ContentType.Cert returns the equivalent of FirstOrDefault(),
                // so anything past _certs[0] is ignored, and an empty collection is
                // null (not an Exception)
                if (_certs.Count == 0)
                {
                    return null;
                }

                return _certs[0].RawData;
            }

            private byte[] ExportPfx(SafePasswordHandle password)
            {
#if LATER
                IntPtr[] certHandles;

                if (_singleCertPal != null)
                {
                    certHandles = new[] { _singleCertPal.Handle };
                }
                else
                {
                    certHandles = new IntPtr[_certs.Count];

                    for (int i = 0; i < _certs.Count; i++)
                    {
                        certHandles[i] = _certs[i].Handle;
                    }
                }

                return Interop.AppleCrypto.X509ExportPfx(certHandles, password);
#else
                throw new NotImplementedException();
#endif
            }

            private byte[] ExportPkcs7()
            {
                IntPtr[] certHandles;

                if (_singleCertPal != null)
                {
                    certHandles = new[] { ((AppleCertificatePal)_singleCertPal).CertificateHandle.DangerousGetHandle() };
                }
                else
                {
                    certHandles = new IntPtr[_certs.Count];

                    for (int i = 0; i < _certs.Count; i++)
                    {
                        AppleCertificatePal pal = (AppleCertificatePal)_certs[i].Pal;
                        certHandles[i] = pal.CertificateHandle.DangerousGetHandle();
                    }
                }

                return Interop.AppleCrypto.X509ExportPkcs7(certHandles);
            }
        }
    }
}
