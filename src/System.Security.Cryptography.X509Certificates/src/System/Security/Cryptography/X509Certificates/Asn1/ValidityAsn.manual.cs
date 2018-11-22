// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    internal partial struct ValidityAsn
    {
        public ValidityAsn(DateTimeOffset notBefore, DateTimeOffset notAfter)
        {
            NotBefore = new TimeAsn(notBefore);
            NotAfter = new TimeAsn(notAfter);
        }
    }
}
