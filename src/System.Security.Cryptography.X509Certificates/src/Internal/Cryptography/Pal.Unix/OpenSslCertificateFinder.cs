// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal
{
    internal sealed class OpenSslCertificateFinder : IFindPal
    {
        private readonly X509Certificate2Collection _findFrom;
        private readonly X509Certificate2Collection _copyTo;
        private readonly bool _validOnly;

        internal OpenSslCertificateFinder(X509Certificate2Collection findFrom, X509Certificate2Collection copyTo, bool validOnly)
        {
            _findFrom = findFrom;
            _copyTo = copyTo;
            _validOnly = validOnly;
        }

        public string NormalizeOid(string maybeOid, OidGroup expectedGroup)
        {
            Oid oid = new Oid(maybeOid);

            // If maybeOid is interpreted to be a FriendlyName, return the OID.
            if (!StringComparer.OrdinalIgnoreCase.Equals(oid.Value, maybeOid))
            {
                return oid.Value;
            }

            FindPal.ValidateOidValue(maybeOid);
            return maybeOid;
        }

        public void FindByThumbprint(byte[] thumbprint)
        {
            FindCore(cert => cert.GetCertHash().ContentsEqual(thumbprint));
        }

        public void FindBySubjectName(string subjectName)
        {
            FindCore(
                cert =>
                {
                    string formedSubject = X500NameEncoder.X500DistinguishedNameDecode(cert.SubjectName.RawData, false, X500DistinguishedNameFlags.None);

                    return formedSubject.IndexOf(subjectName, StringComparison.OrdinalIgnoreCase) >= 0;
                });
        }

        public void FindBySubjectDistinguishedName(string subjectDistinguishedName)
        {
            FindCore(cert => StringComparer.OrdinalIgnoreCase.Equals(subjectDistinguishedName, cert.Subject));
        }

        public void FindByIssuerName(string issuerName)
        {
            FindCore(
                cert =>
                {
                    string formedIssuer = X500NameEncoder.X500DistinguishedNameDecode(cert.IssuerName.RawData, false, X500DistinguishedNameFlags.None);

                    return formedIssuer.IndexOf(issuerName, StringComparison.OrdinalIgnoreCase) >= 0;
                });
        }

        public void FindByIssuerDistinguishedName(string issuerDistinguishedName)
        {
            FindCore(cert => StringComparer.OrdinalIgnoreCase.Equals(issuerDistinguishedName, cert.Issuer));
        }

        public void FindBySerialNumber(BigInteger hexValue, BigInteger decimalValue)
        {
            FindCore(
                cert =>
                {
                    byte[] serialBytes = cert.GetSerialNumber();
                    BigInteger serialNumber = FindPal.PositiveBigIntegerFromByteArray(serialBytes);
                    bool match = hexValue.Equals(serialNumber) || decimalValue.Equals(serialNumber);

                    return match;
                });
        }

        private static DateTime NormalizeDateTime(DateTime dateTime)
        {
            // If it's explicitly UTC, convert it to local before a comparison, since the
            // NotBefore and NotAfter are in Local time.
            //
            // If it was Unknown, assume it was Local.
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime();
            }

            return dateTime;
        }

        public void FindByTimeValid(DateTime dateTime)
        {
            DateTime normalized = NormalizeDateTime(dateTime);

            FindCore(cert => cert.NotBefore <= normalized && normalized <= cert.NotAfter);
        }

        public void FindByTimeNotYetValid(DateTime dateTime)
        {
            DateTime normalized = NormalizeDateTime(dateTime);

            FindCore(cert => cert.NotBefore > normalized);
        }

        public void FindByTimeExpired(DateTime dateTime)
        {
            DateTime normalized = NormalizeDateTime(dateTime);

            FindCore(cert => cert.NotAfter < normalized);
        }

        public void FindByTemplateName(string templateName)
        {
            FindCore(
                cert =>
                {
                    X509Extension ext = FindExtension(cert, Oids.EnrollCertTypeExtension);

                    if (ext != null)
                    {
                        // Try a V1 template structure, just a string:
                        string decodedName = Interop.Crypto.DerStringToManagedString(ext.RawData);

                        // If this doesn't match, maybe a V2 template will
                        if (StringComparer.OrdinalIgnoreCase.Equals(templateName, decodedName))
                        {
                            return true;
                        }
                    }

                    ext = FindExtension(cert, Oids.CertificateTemplate);

                    if (ext != null)
                    {
                        DerSequenceReader reader = new DerSequenceReader(ext.RawData);
                        // SEQUENCE (
                        //     OID oid,
                        //     INTEGER major,
                        //     INTEGER minor OPTIONAL
                        //  )

                        if (reader.PeekTag() == (byte)DerSequenceReader.DerTag.ObjectIdentifier)
                        {
                            Oid oid = reader.ReadOid();

                            if (StringComparer.Ordinal.Equals(templateName, oid.Value))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                });
        }

        public void FindByApplicationPolicy(string oidValue)
        {
            FindCore(
                cert =>
                {
                    X509Extension ext = FindExtension(cert, Oids.EnhancedKeyUsage);

                    if (ext == null)
                    {
                        // A certificate with no EKU is valid for all extended purposes.
                        return true;
                    }

                    var ekuExt = (X509EnhancedKeyUsageExtension)ext;

                    foreach (Oid usageOid in ekuExt.EnhancedKeyUsages)
                    {
                        if (StringComparer.Ordinal.Equals(oidValue, usageOid.Value))
                        {
                            return true;
                        }
                    }

                    // If the certificate had an EKU extension, and the value we wanted was
                    // not present, then it is not valid for that usage.
                    return false;
                });
        }

        public void FindByCertificatePolicy(string oidValue)
        {
            FindCore(
                cert =>
                {
                    X509Extension ext = FindExtension(cert, Oids.CertPolicies);

                    if (ext == null)
                    {
                        // Unlike Application Policy, Certificate Policy is "assume false".
                        return false;
                    }

                    ISet<string> policyOids = CertificatePolicyChain.ReadCertPolicyExtension(ext);
                    return policyOids.Contains(oidValue);
                });
        }

        public void FindByExtension(string oidValue)
        {
            FindCore(cert => FindExtension(cert, oidValue) != null);
        }

        public void FindByKeyUsage(X509KeyUsageFlags keyUsage)
        {
            FindCore(
                cert =>
                {
                    X509Extension ext = FindExtension(cert, Oids.KeyUsage);

                    if (ext == null)
                    {
                        // A certificate with no key usage extension is considered valid for all key usages.
                        return true;
                    }

                    var kuExt = (X509KeyUsageExtension)ext;

                    return (kuExt.KeyUsages & keyUsage) == keyUsage;
                });
        }

        public void FindBySubjectKeyIdentifier(byte[] keyIdentifier)
        {
            FindCore(
                cert =>
                {
                    X509Extension ext = FindExtension(cert, Oids.SubjectKeyIdentifier);
                    byte[] certKeyId;

                    if (ext != null)
                    {
                        // The extension exposes the value as a hexadecimal string, or we can decode here.
                        // Enough parsing has gone on, let's decode.
                        certKeyId = OpenSslX509Encoder.DecodeX509SubjectKeyIdentifierExtension(ext.RawData);
                    }
                    else
                    {
                        // The Desktop/Windows version of this method use CertGetCertificateContextProperty
                        // with a property ID of CERT_KEY_IDENTIFIER_PROP_ID.
                        //
                        // MSDN says that when there's no extension, this method takes the SHA-1 of the
                        // SubjectPublicKeyInfo block, and returns that.
                        //
                        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa376079%28v=vs.85%29.aspx

                        OpenSslX509CertificateReader certPal = (OpenSslX509CertificateReader)cert.Pal;

                        using (HashAlgorithm hash = SHA1.Create())
                        {
                            byte[] publicKeyInfoBytes = Interop.Crypto.OpenSslEncode(
                                Interop.Crypto.GetX509SubjectPublicKeyInfoDerSize,
                                Interop.Crypto.EncodeX509SubjectPublicKeyInfo,
                                certPal.SafeHandle);

                            certKeyId = hash.ComputeHash(publicKeyInfoBytes);
                        }
                    }

                    return keyIdentifier.ContentsEqual(certKeyId);
                });
        }

        public void Dispose()
        {
            // No resources to release, but required by the interface.
        }

        private static X509Extension FindExtension(X509Certificate2 cert, string extensionOid)
        {
            if (cert.Extensions == null || cert.Extensions.Count == 0)
            {
                return null;
            }

            foreach (X509Extension ext in cert.Extensions)
            {
                if (ext != null &&
                    ext.Oid != null &&
                    StringComparer.Ordinal.Equals(extensionOid, ext.Oid.Value))
                {
                    return ext;
                }
            }

            return null;
        }

        private void FindCore(Predicate<X509Certificate2> predicate)
        {
            foreach (X509Certificate2 cert in _findFrom)
            {
                if (predicate(cert))
                {
                    if (!_validOnly || IsCertValid(cert))
                    {
                        OpenSslX509CertificateReader certPal = (OpenSslX509CertificateReader)cert.Pal;
                        _copyTo.Add(new X509Certificate2(certPal.DuplicateHandles()));
                    }
                }
            }
        }

        private static bool IsCertValid(X509Certificate2 cert)
        {
            try
            {
                // This needs to be kept in sync with VerifyCertificateIgnoringErrors in the
                // Windows PAL version (and potentially any other PALs that come about)
                X509Chain chain = new X509Chain
                {
                    ChainPolicy =
                    {
                        RevocationMode = X509RevocationMode.NoCheck,
                        RevocationFlag = X509RevocationFlag.ExcludeRoot
                    }
                };

                return chain.Build(cert);
            }
            catch (CryptographicException)
            {
                return false;
            }
        }
    }
}
