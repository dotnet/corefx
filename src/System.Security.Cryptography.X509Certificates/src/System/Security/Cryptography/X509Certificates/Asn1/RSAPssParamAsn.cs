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
    [StructLayout(LayoutKind.Sequential)]
    internal struct RSAPssParamAsn
    {
        [ExpectedTag(0, ExplicitTag = true)]
        [DefaultValue(0x30, 0x07, 0x06, 0x05, 0x2B, 0x0E, 0x03, 0x02, 0x1A)]
        internal AlgorithmIdentifierAsn HashAlgorithm;

        [ExpectedTag(1, ExplicitTag = true)]
        [DefaultValue(0x30, 0x14, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x08, 0x30, 0x07, 0x06, 0x05, 0x2B, 0x0E, 0x03, 0x02, 0x1A)]
        internal AlgorithmIdentifierAsn MaskGenAlgorithm;

        [ExpectedTag(2, ExplicitTag = true)]
        [DefaultValue(0x02, 0x01, 0x14)]
        internal uint SaltLength;

        [ExpectedTag(3, ExplicitTag = true)]
        [DefaultValue(0x02, 0x01, 0x01)]
        internal uint TrailerField;
    }
}
