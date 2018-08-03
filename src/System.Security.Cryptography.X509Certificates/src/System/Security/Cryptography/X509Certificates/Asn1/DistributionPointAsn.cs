// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // https://tools.ietf.org/html/rfc5280#section-4.2.1.13
    //
    // DistributionPoint ::= SEQUENCE {
    //     distributionPoint       [0]     DistributionPointName OPTIONAL,
    //     reasons                 [1]     ReasonFlags OPTIONAL,
    //     cRLIssuer               [2]     GeneralNames OPTIONAL
    // }
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class DistributionPointAsn
    {
        [ExpectedTag(0, ExplicitTag = true)]
        [OptionalValue]
        internal DistributionPointNameAsn DistributionPoint;

        [ExpectedTag(1)]
        [OptionalValue]
        internal ReasonFlagsAsn? Reasons;

        [ExpectedTag(2)]
        [OptionalValue]
        internal GeneralNameAsn[] CrlIssuer;
    }

    // DistributionPointName ::= CHOICE {
    //     fullName                [0]     GeneralNames,
    //     nameRelativeToCRLIssuer [1]     RelativeDistinguishedName
    // }
    [Choice]
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class DistributionPointNameAsn
    {
        [ExpectedTag(0)]
        internal GeneralNameAsn[] FullName;

        [ExpectedTag(1)]
        [AnyValue]
        internal ReadOnlyMemory<byte> NameRelativeToCRLIssuer;
    }

    // ReasonFlags ::= BIT STRING {
    //     unused                  (0),
    //     keyCompromise           (1),
    //     cACompromise            (2),
    //     affiliationChanged      (3),
    //     superseded              (4),
    //     cessationOfOperation    (5),
    //     certificateHold         (6),
    //     privilegeWithdrawn      (7),
    //     aACompromise            (8)
    // }
    [Flags]
    internal enum ReasonFlagsAsn
    {
        Unused = 1 << 0,
        KeyCompromise = 1 << 1,
        CACompromise = 1 << 2,
        AffiliationChanged = 1 << 3,
        Superseded = 1 << 4,
        CessationOfOperation = 1 << 5,
        CertificateHold = 1 << 6,
        PrivilegeWithdrawn = 1 << 7,
        AACompromise = 1 << 8,
    }
}
