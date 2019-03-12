// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
    public sealed class Pkcs12CertBag : Pkcs12SafeBag
    {
        private Oid _certTypeOid;
        private readonly CertBagAsn _decoded;

        public bool IsX509Certificate { get; }

        private Pkcs12CertBag(ReadOnlyMemory<byte> encodedBagValue, CertBagAsn decoded)
            : base(Oids.Pkcs12CertBag, encodedBagValue)
        {
            _decoded = decoded;

            IsX509Certificate = _decoded.CertId == Oids.Pkcs12X509CertBagType;
        }

        /// <summary>
        /// Create a CertBag for a specified certificate type and encoding.
        /// </summary>
        /// <param name="certificateType">The identifier for the certificate type</param>
        /// <param name="encodedCertificate">The encoded value</param>
        /// <remarks>
        /// No validation is done to ensure that the <paramref name="encodedCertificate"/> value is
        /// correct for the indicated <paramref name="certificateType"/>.  Note that for X.509
        /// public-key certificates the correct encoding for a CertBag value is to wrap the
        /// DER-encoded certificate in an OCTET STRING.
        /// </remarks>
        public Pkcs12CertBag(Oid certificateType, ReadOnlyMemory<byte> encodedCertificate)
            : base(
                Oids.Pkcs12CertBag,
                EncodeBagValue(certificateType, encodedCertificate),
                skipCopy: true)
        {
            _certTypeOid = new Oid(certificateType);

            _decoded = CertBagAsn.Decode(EncodedBagValue, AsnEncodingRules.BER);

            IsX509Certificate = _decoded.CertId == Oids.Pkcs12X509CertBagType;
        }

        internal Pkcs12CertBag(X509Certificate2 cert)
            : base(
                Oids.Pkcs12CertBag,
                EncodeBagValue(
                    Oids.Pkcs12X509CertBagType,
                    PkcsPal.Instance.EncodeOctetString(cert.RawData)),
                skipCopy: true)
        {
            _decoded = CertBagAsn.Decode(EncodedBagValue, AsnEncodingRules.BER);

            IsX509Certificate = true;
        }

        public Oid GetCertificateType()
        {
            if (_certTypeOid == null)
            {
                _certTypeOid = new Oid(_decoded.CertId);
            }

            return new Oid(_certTypeOid);
        }

        public ReadOnlyMemory<byte> EncodedCertificate => _decoded.CertValue;

        public X509Certificate2 GetCertificate()
        {
            if (!IsX509Certificate)
            {
                throw new InvalidOperationException(SR.Cryptography_Pkcs12_CertBagNotX509);
            }

            return new X509Certificate2(PkcsHelpers.DecodeOctetString(_decoded.CertValue).ToArray());
        }

        private static byte[] EncodeBagValue(Oid certificateType, ReadOnlyMemory<byte> encodedCertificate)
        {
            if (certificateType == null)
                throw new ArgumentNullException(nameof(certificateType));
            if (certificateType.Value == null)
                throw new CryptographicException(SR.Argument_InvalidOidValue);

            return EncodeBagValue(certificateType.Value, encodedCertificate);
        }

        private static byte[] EncodeBagValue(string certificateType, ReadOnlyMemory<byte> encodedCertificate)
        {
            // Read to ensure that there is precisely one legally encoded value.
            AsnReader reader = new AsnReader(encodedCertificate, AsnEncodingRules.BER);
            reader.ReadEncodedValue();
            reader.ThrowIfNotEmpty();

            // No need to copy encodedCertificate here, because it will be copied into the
            // return value.
            CertBagAsn certBagAsn = new CertBagAsn
            {
                CertId = certificateType,
                CertValue = encodedCertificate,
            };

            using (AsnWriter writer = new AsnWriter(AsnEncodingRules.BER))
            {
                certBagAsn.Encode(writer);
                return writer.Encode();
            }
        }

        internal static Pkcs12CertBag DecodeValue(ReadOnlyMemory<byte> bagValue)
        {
            CertBagAsn decoded = CertBagAsn.Decode(bagValue, AsnEncodingRules.BER);
            return new Pkcs12CertBag(bagValue, decoded);
        }
    }
}
