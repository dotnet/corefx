// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.X509Certificates.Asn1;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal static class CrlCache
    {
        private static readonly string s_crlDir =
            PersistedFiles.GetUserFeatureDirectory(
                X509Persistence.CryptographyFeatureName,
                X509Persistence.CrlsSubFeatureName);

        private static readonly string s_ocspDir =
            PersistedFiles.GetUserFeatureDirectory(
                X509Persistence.CryptographyFeatureName,
                X509Persistence.OcspSubFeatureName);

        private const ulong X509_R_CERT_ALREADY_IN_HASH_TABLE = 0x0B07D065;

        [ThreadStatic]
        private static HashAlgorithm ts_urlHash;

        public static void AddCrlForCertificate(
            SafeX509Handle cert,
            SafeX509StoreHandle store,
            X509RevocationMode revocationMode,
            DateTime verificationTime,
            ref TimeSpan remainingDownloadTime)
        {
            // In Offline mode, accept any cached CRL we have.
            // "CRL is Expired" is a better match for Offline than "Could not find CRL"
            if (revocationMode != X509RevocationMode.Online)
            {
                verificationTime = DateTime.MinValue;
            }

            string url = GetCdpUrl(cert);

            if (url == null)
            {
                return;
            }

            string crlFileName = GetCrlFileName(cert, url);

            if (AddCachedCrl(crlFileName, store, verificationTime))
            {
                return;
            }

            // Don't do any work if we're over limit or prohibited from fetching new CRLs
            if (remainingDownloadTime <= TimeSpan.Zero ||
                revocationMode != X509RevocationMode.Online)
            {
                return;
            }

            DownloadAndAddCrl(url, crlFileName, store, ref remainingDownloadTime);
        }

        private static bool AddCachedCrl(string crlFileName, SafeX509StoreHandle store, DateTime verificationTime)
        {
            string crlFile = GetCachedCrlPath(crlFileName);

            using (SafeBioHandle bio = Interop.Crypto.BioNewFile(crlFile, "rb"))
            {
                if (bio.IsInvalid)
                {
                    Interop.Crypto.ErrClearError();
                    return false;
                }

                // X509_STORE_add_crl will increase the refcount on the CRL object, so we should still
                // dispose our copy.
                using (SafeX509CrlHandle crl = Interop.Crypto.PemReadBioX509Crl(bio))
                {
                    if (crl.IsInvalid)
                    {
                        Interop.Crypto.ErrClearError();
                        return false;
                    }

                    // If crl.LastUpdate is in the past, downloading a new version isn't really going
                    // to help, since we can't rewind the Internet. So this is just going to fail, but
                    // at least it can fail without using the network.
                    //
                    // If crl.NextUpdate is in the past, try downloading a newer version.
                    DateTime nextUpdate = OpenSslX509CertificateReader.ExtractValidityDateTime(
                        Interop.Crypto.GetX509CrlNextUpdate(crl));

                    // OpenSSL is going to convert our input time to universal, so we should be in Local or
                    // Unspecified (local-assumed).
                    Debug.Assert(
                        verificationTime.Kind != DateTimeKind.Utc,
                        "UTC verificationTime should have been normalized to Local");

                    // In the event that we're to-the-second accurate on the match, OpenSSL will consider this
                    // to be already expired.
                    if (nextUpdate <= verificationTime)
                    {
                        return false;
                    }

                    if (!Interop.Crypto.X509StoreAddCrl(store, crl))
                    {
                        // Ignore error "cert already in store", throw on anything else. In any case the error queue will be cleared.
                        if (X509_R_CERT_ALREADY_IN_HASH_TABLE == Interop.Crypto.ErrPeekLastError())
                        {
                            Interop.Crypto.ErrClearError();
                        }
                        else
                        {
                            throw Interop.Crypto.CreateOpenSslCryptographicException();
                        }
                    }

                    return true;
                }
            }
        }

        private static void DownloadAndAddCrl(
            string url,
            string crlFileName,
            SafeX509StoreHandle store,
            ref TimeSpan remainingDownloadTime)
        {
            // X509_STORE_add_crl will increase the refcount on the CRL object, so we should still
            // dispose our copy.
            using (SafeX509CrlHandle crl = CertificateAssetDownloader.DownloadCrl(url, ref remainingDownloadTime))
            {
                // null is a valid return (e.g. no remainingDownloadTime)
                if (crl != null && !crl.IsInvalid)
                {
                    if (!Interop.Crypto.X509StoreAddCrl(store, crl))
                    {
                        // Ignore error "cert already in store", throw on anything else. In any case the error queue will be cleared.
                        if (X509_R_CERT_ALREADY_IN_HASH_TABLE == Interop.Crypto.ErrPeekLastError())
                        {
                            Interop.Crypto.ErrClearError();
                        }
                        else
                        {
                            throw Interop.Crypto.CreateOpenSslCryptographicException();
                        }
                    }

                    // Saving the CRL to the disk is just a performance optimization for later requests to not
                    // need to use the network again, so failure to save shouldn't throw an exception or mark
                    // the chain as invalid.
                    try
                    {
                        string crlFile = GetCachedCrlPath(crlFileName, mkDir: true);

                        using (SafeBioHandle bio = Interop.Crypto.BioNewFile(crlFile, "wb"))
                        {
                            if (bio.IsInvalid || Interop.Crypto.PemWriteBioX509Crl(bio, crl) == 0)
                            {
                                // No bio, or write failed
                                Interop.Crypto.ErrClearError();
                            }
                        }
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (IOException) { }
                }
            }
        }

        internal static string GetCachedOcspResponseDirectory()
        {
            return s_ocspDir;
        }

        private static string GetCrlFileName(SafeX509Handle cert, string crlUrl)
        {
            // X509_issuer_name_hash returns "unsigned long", which is marshalled as ulong.
            // But it only sets 32 bits worth of data, so force it down to uint just... in case.
            ulong persistentHashLong = Interop.Crypto.X509IssuerNameHash(cert);
            if (persistentHashLong == 0)
            {
                Interop.Crypto.ErrClearError();
            }

            uint persistentHash = unchecked((uint)persistentHashLong);

            if (ts_urlHash == null)
            {
                ts_urlHash = SHA256.Create();
            }

            Span<byte> hash = stackalloc byte[256 >> 3];

            // Endianness isn't important, it just needs to be consistent.
            // (Even if the same storage was used for two different endianness systems it'd stabilize at two files).
            ReadOnlySpan<byte> utf16Url = MemoryMarshal.AsBytes(crlUrl.AsSpan());

            if (!ts_urlHash.TryComputeHash(utf16Url, hash, out int written) || written != hash.Length)
            {
                Debug.Fail("TryComputeHash failed or produced an incorrect length output");
                throw new CryptographicException();
            }

            uint urlHash = MemoryMarshal.Read<uint>(hash);

            // OpenSSL's hashed filename algorithm is the 8-character hex version of the 32-bit value
            // of X509_issuer_name_hash (or X509_subject_name_hash, depending on the context).
            //
            // We mix in an 8-character hex version of the "left"-most bytes of a hash of the URL to
            // disambiguate when one Issuing Authority separates their revocation across independent CRLs.
            return $"{persistentHash:x8}.{urlHash:x8}.crl";
        }

        private static string GetCachedCrlPath(string localFileName, bool mkDir=false)
        {
            if (mkDir)
            {
                Directory.CreateDirectory(s_crlDir);
            }

            return Path.Combine(s_crlDir, localFileName);
        }

        private static string GetCdpUrl(SafeX509Handle cert)
        {
            ArraySegment<byte> crlDistributionPoints =
                OpenSslX509CertificateReader.FindFirstExtension(cert, Oids.CrlDistributionPoints);

            if (crlDistributionPoints.Array == null)
            {
                return null;
            }

            try
            {
                AsnReader reader = new AsnReader(crlDistributionPoints, AsnEncodingRules.DER);
                AsnReader sequenceReader = reader.ReadSequence();
                reader.ThrowIfNotEmpty();

                while (sequenceReader.HasData)
                {
                    DistributionPointAsn.Decode(sequenceReader, out DistributionPointAsn distributionPoint);

                    // Only distributionPoint is supported
                    // Only fullName is supported, nameRelativeToCRLIssuer is for LDAP-based lookup.
                    if (distributionPoint.DistributionPoint.HasValue &&
                        distributionPoint.DistributionPoint.Value.FullName != null)
                    {
                        foreach (GeneralNameAsn name in distributionPoint.DistributionPoint.Value.FullName)
                        {
                            if (name.Uri != null &&
                                Uri.TryCreate(name.Uri, UriKind.Absolute, out Uri uri) &&
                                uri.Scheme == "http")
                            {
                                return name.Uri;
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                // Treat any ASN errors as if the extension was missing.
            }
            finally
            {
                // The data came from a certificate, so it's public.
                CryptoPool.Return(crlDistributionPoints.Array, clearSize: 0);
            }

            return null;
        }
    }
}
