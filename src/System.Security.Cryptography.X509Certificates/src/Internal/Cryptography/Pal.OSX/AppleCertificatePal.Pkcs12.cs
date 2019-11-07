// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography.Apple;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed partial class AppleCertificatePal : ICertificatePal
    {
        private static ICertificatePal ImportPkcs12(
            byte[] rawData,
            SafePasswordHandle password,
            bool exportable,
            SafeKeychainHandle keychain)
        {
            using (ApplePkcs12Reader reader = new ApplePkcs12Reader(rawData))
            {
                reader.Decrypt(password);

                UnixPkcs12Reader.CertAndKey certAndKey = reader.GetSingleCert();
                AppleCertificatePal pal = (AppleCertificatePal)certAndKey.Cert;

                SafeSecKeyRefHandle safeSecKeyRefHandle =
                    ApplePkcs12Reader.GetPrivateKey(certAndKey.Key);

                AppleCertificatePal newPal;

                using (safeSecKeyRefHandle)
                {
                    // SecItemImport doesn't seem to respect non-exportable import for PKCS#8,
                    // only PKCS#12.
                    //
                    // So, as part of reading this PKCS#12 we now need to write the minimum
                    // PKCS#12 in a normalized form, and ask the OS to import it.
                    if (!exportable && safeSecKeyRefHandle != null)
                    {
                        using (pal)
                        {
                            return ImportPkcs12NonExportable(pal, safeSecKeyRefHandle, password, keychain);
                        }
                    }

                    newPal = pal.MoveToKeychain(keychain, safeSecKeyRefHandle);

                    if (newPal != null)
                    {
                        pal.Dispose();
                    }
                }

                // If no new PAL came back, it means we moved the cert, but had no private key.
                return newPal ?? pal;
            }
        }

        internal static ICertificatePal ImportPkcs12NonExportable(
            AppleCertificatePal cert,
            SafeSecKeyRefHandle privateKey,
            SafePasswordHandle password,
            SafeKeychainHandle keychain)
        {
            Pkcs12SmallExport exporter = new Pkcs12SmallExport(new TempExportPal(cert), privateKey);
            byte[] smallPfx = exporter.Export(X509ContentType.Pkcs12, password);

            SafeSecIdentityHandle identityHandle;
            SafeSecCertificateHandle certHandle = Interop.AppleCrypto.X509ImportCertificate(
                smallPfx,
                X509ContentType.Pkcs12,
                password,
                keychain,
                exportable: false,
                out identityHandle);

            // On Windows and Linux if a PFX uses a LocalKeyId to bind the wrong key to a cert, the
            // nonsensical object of "this cert, that key" is returned.
            //
            // On macOS, because we can't forge CFIdentityRefs without the keychain, we're subject to
            // Apple's more stringent matching of a consistent keypair.
            if (identityHandle.IsInvalid)
            {
                identityHandle.Dispose();
                return new AppleCertificatePal(certHandle);
            }

            certHandle.Dispose();
            return new AppleCertificatePal(identityHandle);
        }

        private sealed class Pkcs12SmallExport : UnixExportProvider
        {
            private readonly SafeSecKeyRefHandle _privateKey;

            internal Pkcs12SmallExport(ICertificatePalCore cert, SafeSecKeyRefHandle privateKey)
                : base(cert)
            {
                Debug.Assert(!privateKey.IsInvalid);
                _privateKey = privateKey;
            }

            protected override byte[] ExportPkcs7() => throw new NotImplementedException();

            protected override byte[] ExportPkcs8(ICertificatePalCore certificatePal, ReadOnlySpan<char> password)
            {
                return AppleCertificatePal.ExportPkcs8(_privateKey, password);
            }
        }

        private sealed class TempExportPal : ICertificatePalCore
        {
            private readonly ICertificatePal _realPal;

            internal TempExportPal(AppleCertificatePal realPal)
            {
                _realPal = realPal;
            }

            public bool HasPrivateKey => true;

            public void Dispose()
            {
                // No-op.
            }

            // Forwarders to make the interface compliant.
            public IntPtr Handle => _realPal.Handle;
            public string Issuer => _realPal.Issuer;
            public string Subject => _realPal.Subject;
            public string LegacyIssuer => _realPal.LegacyIssuer;
            public string LegacySubject => _realPal.LegacySubject;
            public byte[] Thumbprint => _realPal.Thumbprint;
            public string KeyAlgorithm => _realPal.KeyAlgorithm;
            public byte[] KeyAlgorithmParameters => _realPal.KeyAlgorithmParameters;
            public byte[] PublicKeyValue => _realPal.PublicKeyValue;
            public byte[] SerialNumber => _realPal.SerialNumber;
            public string SignatureAlgorithm => _realPal.SignatureAlgorithm;
            public DateTime NotAfter => _realPal.NotAfter;
            public DateTime NotBefore => _realPal.NotBefore;
            public byte[] RawData => _realPal.RawData;
            public byte[] Export(X509ContentType contentType, SafePasswordHandle password) =>
                _realPal.Export(contentType, password);
        }
    }
}
