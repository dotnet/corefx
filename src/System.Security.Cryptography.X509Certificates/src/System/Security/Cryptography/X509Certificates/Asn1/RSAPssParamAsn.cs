// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // https://tools.ietf.org/html/rfc4055#section-3.1
    //
    // RSASSA-PSS-params ::= SEQUENCE {
    //     hashAlgorithm      [0] HashAlgorithm DEFAULT sha1Identifier,
    //     maskGenAlgorithm   [1] MaskGenAlgorithm DEFAULT mgf1SHA1Identifier,
    //     saltLength         [2] INTEGER DEFAULT 20,
    //     trailerField       [3] INTEGER DEFAULT 1
    // }
    //
    // mgf1SHA1Identifier  AlgorithmIdentifier  ::= { id-mgf1, sha1Identifier }
    // sha1Identifier  AlgorithmIdentifier  ::=  { id-sha1, NULL }
    // (and similar for SHA256/384/512)
    //
    // RFC 5754 says that the NULL for SHA2 (256/384/512) MUST be omitted
    // (https://tools.ietf.org/html/rfc5754#section-2) (and that you MUST
    // be able to read it even if someone wrote it down)
    //
    // Since we
    //  * don't support SHA-1 in this class
    //  * only support MGF-1
    //  * don't support the MGF PRF being different than hashAlgorithm
    //  * use saltLength==hashLength
    //  * don't allow custom trailer
    // we don't have to worry about any of the DEFAULTs. (specify, specify, specify, omit).
    [StructLayout(LayoutKind.Sequential)]
    internal struct RSAPssParamAsn
    {
        [ExpectedTag(0, ExplicitTag = true)]
        internal AlgorithmIdentifierAsn HashAlgorithm;

        [ExpectedTag(1, ExplicitTag = true)]
        internal AlgorithmIdentifierAsn MaskGenAlgorithm;

        [ExpectedTag(2, ExplicitTag = true)]
        [DefaultValue(0x02, 0x01, 0x14)]
        internal uint SaltLength;

        [ExpectedTag(3, ExplicitTag = true)]
        [DefaultValue(0x02, 0x01, 0x01)]
        internal uint TrailerField;
    }
}
