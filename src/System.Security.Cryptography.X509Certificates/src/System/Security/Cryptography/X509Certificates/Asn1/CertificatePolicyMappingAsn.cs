// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // https://tools.ietf.org/html/rfc5280#section-4.2.1.5
    //
    // PolicyMappings ::= SEQUENCE SIZE (1..MAX) OF SEQUENCE {
    //     issuerDomainPolicy      CertPolicyId,
    //     subjectDomainPolicy     CertPolicyId
    // }
    //
    // CertPolicyId ::= OBJECT IDENTIFIER
    [StructLayout(LayoutKind.Sequential)]
    internal struct CertificatePolicyMappingAsn
    {
        [ObjectIdentifier]
        public string IssuerDomainPolicy;
        [ObjectIdentifier]
        public string SubjectDomainPolicy;
    }
}
