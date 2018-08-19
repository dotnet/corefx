// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    // https://tools.ietf.org/html/rfc5280#section-4.2.1.11
    //
    // PolicyConstraints ::= SEQUENCE {
    //     requireExplicitPolicy           [0] SkipCerts OPTIONAL,
    //     inhibitPolicyMapping            [1] SkipCerts OPTIONAL
    // }
    //
    // SkipCerts ::= INTEGER (0..MAX)
    [StructLayout(LayoutKind.Sequential)]
    internal struct PolicyConstraintsAsn
    {
        [ExpectedTag(0)]
        [OptionalValue]
        internal uint? RequireExplicitPolicyDepth;

        [ExpectedTag(1)]
        [OptionalValue]
        internal uint? InhibitMappingDepth;
    }
}
