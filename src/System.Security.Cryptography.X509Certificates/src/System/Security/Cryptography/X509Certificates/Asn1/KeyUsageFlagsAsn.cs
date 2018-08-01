// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace System.Security.Cryptography.X509Certificates.Asn1
{
    /// <summary>Version of X509KeyUsageFlags with reversed bit order.</summary>
    [Flags]
    internal enum KeyUsageFlagsAsn
    {
        None = 0x0000,
        DigitalSignature = 0x0001,
        NonRepudiation = 0x0002,
        KeyEncipherment = 0x0004,
        DataEncipherment = 0x0008,
        KeyAgreement = 0x0010,
        KeyCertSign = 0x0020,
        CrlSign = 0x0040,
        EncipherOnly = 0x0080,
        DecipherOnly = 0x0100,
    }
}

