// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class Rfc3161TimestampToken
    {
        private SignedCms _parsedDocument;

        public Rfc3161TimestampTokenInfo TokenInfo { get; private set; }

        /// <summary>
        /// Get a SignedCms representation of the RFC3161 Timestamp Token.
        /// </summary>
        /// <returns>The SignedCms representation of the RFC3161 Timestamp Token.</returns>
        /// <remarks>
        /// Successive calls to this method return the same object.
        /// The SignedCms class is mutable, but changes to that object are not reflected in the
        /// <see cref="Rfc3161TimestampToken"/> object which produced it.
        /// The value from calling <see cref="SignedCms.Encode"/> can be interpreted again as an
        /// <see cref="Rfc3161TimestampToken"/> via another call to <see cref="TryParse"/>.
        /// </remarks>
        public SignedCms AsSignedCms() => _parsedDocument;

        public bool VerifyData(ReadOnlySpan<byte> data)
        {
            HashAlgorithmName hashAlgorithmName = Helpers.GetDigestAlgorithm(TokenInfo.HashAlgorithmId);

            using (IncrementalHash hasher = IncrementalHash.CreateHash(hashAlgorithmName))
            {
                hasher.AppendData(data);

                // SHA-2-512 is the biggest hash we currently know about.
                Span<byte> stackSpan = stackalloc byte[512 / 8];

                if (hasher.TryGetHashAndReset(stackSpan, out int bytesWritten))
                {
                    return VerifyHash(stackSpan.Slice(0, bytesWritten));
                }

                // Something we understood, but is bigger than 512-bit.
                // Allocate at runtime, trip in a debug build so we can re-evaluate this.
                Debug.Fail(
                    $"TryGetHashAndReset did not fit in {stackSpan.Length} for hash {TokenInfo.HashAlgorithmId.Value}");
                return VerifyHash(hasher.GetHashAndReset());
            }
        }

        public bool VerifyHash(ReadOnlySpan<byte> hash)
        {
            return hash.SequenceEqual(TokenInfo.GetMessageHash().Span);
        }

        public bool CheckCertificate(X509Certificate2 tsaCertificate)
        {
            if (tsaCertificate == null)
            {
                return false;
            }

            Debug.Assert(_parsedDocument != null, "_parsedDocument != null");

            SignerInfoCollection signerInfos = _parsedDocument.SignerInfos;
            Debug.Assert(signerInfos.Count == 1);

            SignerInfo signer = signerInfos[0];
            X509Certificate2 signerCert = signer.Certificate;

            // If we already mapped the signer cert in the CMS object we can't use another
            // input to verify the signature.  So in that case just check that it's the
            // one we already checked in TryParse.
            if (signerCert != null)
            {
                return signerCert.RawData.SequenceEqual(tsaCertificate.RawData);
            }

            EssCertId certId;
            EssCertIdV2 certId2;

            if (!TryGetCertIds(signer, out certId, out certId2))
            {
                Debug.Fail("TryGetCertIds failed, which should have prevented object creation");
                return false;
            }

            return CheckCertificate(tsaCertificate, signer, certId, certId2, TokenInfo);
        }

        private static bool CheckCertificate(
            X509Certificate2 tsaCertificate,
            SignerInfo signer,
            EssCertId certId,
            EssCertIdV2 certId2,
            Rfc3161TimestampTokenInfo tokenInfo)
        {
            Debug.Assert(tsaCertificate != null);
            Debug.Assert(signer != null);
            Debug.Assert(tokenInfo != null);
            // certId and certId2 are allowed to be null, they get checked in CertMatchesIds.

            if (!CertMatchesIds(tsaCertificate, certId, certId2))
            {
                return false;
            }

            // Nothing in RFC3161 actually mentions checking the certificate's validity
            // against the TSTInfo timestamp value, but it seems sensible.
            //
            // Accuracy is ignored here, for better replicability in user code.

            if (tsaCertificate.NotAfter < tokenInfo.Timestamp ||
                tsaCertificate.NotBefore > tokenInfo.Timestamp)
            {
                return false;
            }

            // https://tools.ietf.org/html/rfc3161#section-2.3
            //
            // The TSA MUST sign each time-stamp message with a key reserved
            // specifically for that purpose.  A TSA MAY have distinct private keys,
            // e.g., to accommodate different policies, different algorithms,
            // different private key sizes or to increase the performance. The
            // corresponding certificate MUST contain only one instance of the
            // extended key usage field extension as defined in [RFC2459] Section
            // 4.2.1.13 with KeyPurposeID having value:
            //
            // id-kp-timeStamping. This extension MUST be critical.

            using (var ekuExts = tsaCertificate.Extensions.OfType<X509EnhancedKeyUsageExtension>().GetEnumerator())
            {
                if (!ekuExts.MoveNext())
                {
                    return false;
                }

                X509EnhancedKeyUsageExtension ekuExt = ekuExts.Current;

                if (!ekuExt.Critical)
                {
                    return false;
                }

                bool hasPurpose = false;

                foreach (Oid oid in ekuExt.EnhancedKeyUsages)
                {
                    if (oid.Value == Oids.TimeStampingPurpose)
                    {
                        hasPurpose = true;
                        break;
                    }
                }

                if (!hasPurpose)
                {
                    return false;
                }

                if (ekuExts.MoveNext())
                {
                    return false;
                }
            }

            try
            {
                signer.CheckSignature(new X509Certificate2Collection(tsaCertificate), true);
                return true;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        public static bool TryParse(ReadOnlyMemory<byte> source, out int bytesRead, out Rfc3161TimestampToken token)
        {
            bytesRead = 0;
            token = null;

            try
            {
                ContentInfoAsn contentInfo =
                    AsnSerializer.Deserialize<ContentInfoAsn>(source, AsnEncodingRules.BER, out int bytesActuallyRead);

                // https://tools.ietf.org/html/rfc3161#section-2.4.2
                //
                // A TimeStampToken is as follows.  It is defined as a ContentInfo
                // ([CMS]) and SHALL encapsulate a signed data content type.
                //
                // TimeStampToken::= ContentInfo
                //   --contentType is id-signedData([CMS])
                //   --content is SignedData ([CMS])
                if (contentInfo.ContentType != Oids.Pkcs7Signed)
                {
                    return false;
                }

                SignedCms cms = new SignedCms();
                cms.Decode(source);

                // The fields of type EncapsulatedContentInfo of the SignedData
                // construct have the following meanings:
                //
                // eContentType is an object identifier that uniquely specifies the
                // content type.  For a time-stamp token it is defined as:
                //
                // id-ct-TSTInfo  OBJECT IDENTIFIER ::= { iso(1) member-body(2)
                // us(840) rsadsi(113549) pkcs(1) pkcs-9(9) smime(16) ct(1) 4}
                //
                // eContent is the content itself, carried as an octet string.
                // The eContent SHALL be the DER-encoded value of TSTInfo.
                if (cms.ContentInfo.ContentType.Value != Oids.TstInfo)
                {
                    return false;
                }

                // RFC3161:
                // The time-stamp token MUST NOT contain any signatures other than the
                // signature of the TSA.  The certificate identifier (ESSCertID) of the
                // TSA certificate MUST be included as a signerInfo attribute inside a
                // SigningCertificate attribute.

                // RFC5816 says that ESSCertIDv2 should be allowed instead.

                SignerInfoCollection signerInfos = cms.SignerInfos;

                if (signerInfos.Count != 1)
                {
                    return false;
                }

                SignerInfo signer = signerInfos[0];
                EssCertId certId;
                EssCertIdV2 certId2;

                if (!TryGetCertIds(signer, out certId, out certId2))
                {
                    return false;
                }

                X509Certificate2 signerCert = signer.Certificate;

                if (signerCert == null &&
                    signer.SignerIdentifier.Type == SubjectIdentifierType.IssuerAndSerialNumber)
                {
                    // If the cert wasn't provided, but the identifier was IssuerAndSerialNumber,
                    // and the ESSCertId(V2) has specified an issuerSerial value, ensure it's a match.
                    X509IssuerSerial issuerSerial = (X509IssuerSerial)signer.SignerIdentifier.Value;

                    if (certId?.IssuerSerial != null)
                    {
                        if (!IssuerAndSerialMatch(
                            certId.IssuerSerial.Value,
                            issuerSerial.IssuerName,
                            issuerSerial.SerialNumber))
                        {
                            return false;
                        }
                    }

                    if (certId2?.IssuerSerial != null)
                    {
                        if (!IssuerAndSerialMatch(
                            certId2.IssuerSerial.Value,
                            issuerSerial.IssuerName,
                            issuerSerial.SerialNumber))
                        {
                            return false;
                        }
                    }
                }

                Rfc3161TimestampTokenInfo tokenInfo;

                if (Rfc3161TimestampTokenInfo.TryParse(cms.ContentInfo.Content, out _, out tokenInfo))
                {
                    if (signerCert != null &&
                        !CheckCertificate(signerCert, signer, certId, certId2, tokenInfo))
                    {
                        return false;
                    }

                    token = new Rfc3161TimestampToken
                    {
                        _parsedDocument = cms,
                        TokenInfo = tokenInfo,
                    };

                    bytesRead = bytesActuallyRead;
                    return true;
                }
            }
            catch (CryptographicException)
            {
            }

            return false;
        }

        private static bool IssuerAndSerialMatch(
            CadesIssuerSerial issuerSerial,
            string issuerDirectoryName,
            string serialNumber)
        {
            GeneralName[] issuerNames = issuerSerial.Issuer;

            if (issuerNames == null || issuerNames.Length != 1)
            {
                return false;
            }

            GeneralName requiredName = issuerNames[0];

            if (requiredName.DirectoryName == null)
            {
                return false;
            }

            if (issuerDirectoryName != new X500DistinguishedName(requiredName.DirectoryName.Value.ToArray()).Name)
            {
                return false;
            }

            return serialNumber == issuerSerial.SerialNumber.Span.ToSkiString();
        }

        private static bool IssuerAndSerialMatch(
            CadesIssuerSerial issuerSerial,
            ReadOnlySpan<byte> issuerDirectoryName,
            ReadOnlySpan<byte> serialNumber)
        {
            GeneralName[] issuerNames = issuerSerial.Issuer;

            if (issuerNames == null || issuerNames.Length != 1)
            {
                return false;
            }

            GeneralName requiredName = issuerNames[0];

            if (requiredName.DirectoryName == null)
            {
                return false;
            }

            if (!requiredName.DirectoryName.Value.Span.SequenceEqual(issuerDirectoryName))
            {
                return false;
            }

            return serialNumber.SequenceEqual(issuerSerial.SerialNumber.Span);
        }

        private static bool CertMatchesIds(X509Certificate2 signerCert, EssCertId certId, EssCertIdV2 certId2)
        {
            Debug.Assert(signerCert != null);
            Debug.Assert(certId != null || certId2 != null);
            byte[] serialNumber = null;

            if (certId != null)
            {
                Span<byte> thumbprint = stackalloc byte[20];

                if (!signerCert.TryGetCertHash(HashAlgorithmName.SHA1, thumbprint, out int written) ||
                    written != thumbprint.Length ||
                    !thumbprint.SequenceEqual(certId.Hash.Span))
                {
                    return false;
                }

                if (certId.IssuerSerial != null)
                {
                    serialNumber = signerCert.GetSerialNumber();
                    Array.Reverse(serialNumber);

                    if (!IssuerAndSerialMatch(
                        certId.IssuerSerial.Value,
                        signerCert.IssuerName.RawData,
                        serialNumber))
                    {
                        return false;
                    }
                }
            }

            if (certId2 != null)
            {
                HashAlgorithmName alg;
                // SHA-2-512 is the biggest we know about.
                Span<byte> thumbprint = stackalloc byte[512 / 8];

                try
                {
                    alg = Helpers.GetDigestAlgorithm(certId2.HashAlgorithm.Algorithm);

                    if (signerCert.TryGetCertHash(alg, thumbprint, out int written))
                    {
                        thumbprint = thumbprint.Slice(0, written);
                    }
                    else
                    {
                        Debug.Fail(
                            $"TryGetCertHash did not fit in {thumbprint.Length} for hash {certId2.HashAlgorithm.Algorithm.Value}");

                        thumbprint = signerCert.GetCertHash(alg);
                    }
                }
                catch (CryptographicException)
                {
                    return false;
                }

                if (!thumbprint.SequenceEqual(certId2.Hash.Span))
                {
                    return false;
                }

                if (certId2.IssuerSerial != null)
                {
                    if (serialNumber == null)
                    {
                        serialNumber = signerCert.GetSerialNumber();
                        Array.Reverse(serialNumber);
                    }

                    if (!IssuerAndSerialMatch(
                        certId2.IssuerSerial.Value,
                        signerCert.IssuerName.RawData,
                        serialNumber))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool TryGetCertIds(SignerInfo signer, out EssCertId certId, out EssCertIdV2 certId2)
        {
            // RFC 5035 says that SigningCertificateV2 (contains ESSCertIDv2) is a signed
            // attribute, with OID 1.2.840.113549.1.9.16.2.47, and that it must not be multiply defined.

            // RFC 2634 says that SigningCertificate (contains ESSCertID) is a signed attribute,
            // with OID 1.2.840.113549.1.9.16.2.12, and that it must not be multiply defined.
            certId = null;
            certId2 = null;

            foreach (CryptographicAttributeObject attrSet in signer.SignedAttributes)
            {
                foreach (AsnEncodedData attr in attrSet.Values)
                {
                    if (attr.Oid.Value == Oids.SigningCertificate)
                    {
                        if (certId != null)
                        {
                            return false;
                        }

                        try
                        {
                            SigningCertificateAsn signingCert =
                                AsnSerializer.Deserialize<SigningCertificateAsn>(attr.RawData, AsnEncodingRules.BER);

                            if (signingCert.Certs.Length < 1)
                            {
                                return false;
                            }

                            // The first one is the signing cert, the rest constrain the chain.
                            certId = signingCert.Certs[0];
                        }
                        catch (CryptographicException)
                        {
                            return false;
                        }
                    }

                    if (attr.Oid.Value == Oids.SigningCertificateV2)
                    {
                        if (certId2 != null)
                        {
                            return false;
                        }

                        try
                        {
                            SigningCertificateV2Asn signingCert =
                                AsnSerializer.Deserialize<SigningCertificateV2Asn>(attr.RawData, AsnEncodingRules.BER);

                            if (signingCert.Certs.Length < 1)
                            {
                                return false;
                            }

                            // The first one is the signing cert, the rest constrain the chain.
                            certId2 = signingCert.Certs[0];
                        }
                        catch (CryptographicException)
                        {
                            return false;
                        }
                    }
                }
            }

            return certId2 != null || certId != null;
        }
    }
}
