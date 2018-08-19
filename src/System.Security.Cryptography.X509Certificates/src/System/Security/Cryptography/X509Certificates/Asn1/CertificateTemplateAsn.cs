// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // Microsoft extension (1.3.6.1.4.1.311.21.7)
    //
    // TemplateVersion ::= INTEGER (0..4294967295)
    //
    // CertificateTemplate ::= SEQUENCE {
    //     templateID OBJECT IDENTIFIER,
    //     templateMajorVersion TemplateVersion,
    //     templateMinorVersion TemplateVersion OPTIONAL
    // }
    [StructLayout(LayoutKind.Sequential)]
    internal struct CertificateTemplateAsn
    {
        [ObjectIdentifier]
        internal string TemplateID;

        internal uint TemplateMajorVersion;

        [OptionalValue]
        internal uint? TemplateMinorVersion;
    }
}
