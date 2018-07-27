// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // https://tools.ietf.org/html/rfc5280#section-4.1
    //
    // TBSCertificate  ::=  SEQUENCE  {
    //     version         [0]  Version DEFAULT v1,
    //     serialNumber         CertificateSerialNumber,
    //     signature            AlgorithmIdentifier,
    //     issuer               Name,
    //     validity             Validity,
    //     subject              Name,
    //     subjectPublicKeyInfo SubjectPublicKeyInfo,
    //     issuerUniqueID  [1]  IMPLICIT UniqueIdentifier OPTIONAL,
    //                          -- If present, version MUST be v2 or v3
    //     subjectUniqueID [2]  IMPLICIT UniqueIdentifier OPTIONAL,
    //                          -- If present, version MUST be v2 or v3
    //      extensions      [3]  Extensions OPTIONAL
    //                          -- If present, version MUST be v3 --
    // }
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class TbsCertificateAsn
    {
        [ExpectedTag(0, ExplicitTag = true)]
        [DefaultValue(0xA0, 0x03, 0x02, 0x01, 0x00)]
        public byte Version;

        [Integer]
        public ReadOnlyMemory<byte> SerialNumber;

        public AlgorithmIdentifierAsn SignatureAlgorithm;

        [AnyValue]
        [ExpectedTag(TagClass.Universal, (int)UniversalTagNumber.SequenceOf)]
        public ReadOnlyMemory<byte> Issuer;

        public ValidityAsn Validity;

        [AnyValue]
        [ExpectedTag(TagClass.Universal, (int)UniversalTagNumber.SequenceOf)]
        public ReadOnlyMemory<byte> Subject;

        public SubjectPublicKeyInfoAsn SubjectPublicKeyInfo;

        [ExpectedTag(1)]
        [OptionalValue]
        [BitString]
        public ReadOnlyMemory<byte>? IssuerUniqueId;

        [ExpectedTag(2)]
        [OptionalValue]
        [BitString]
        public ReadOnlyMemory<byte>? SubjectUniqueId;

        [ExpectedTag(3, ExplicitTag = true)]
        [OptionalValue]
        public X509ExtensionAsn[] Extensions;
    }
}
