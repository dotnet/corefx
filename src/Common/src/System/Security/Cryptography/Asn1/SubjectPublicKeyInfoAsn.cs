// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Security.Cryptography.Asn1
{
    // https://tools.ietf.org/html/rfc3280#section-4.1
    //
    // SubjectPublicKeyInfo  ::=  SEQUENCE  {
    //   algorithm            AlgorithmIdentifier,
    //   subjectPublicKey     BIT STRING  }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SubjectPublicKeyInfoAsn
    {
        internal AlgorithmIdentifierAsn Algorithm;

        [BitString]
        internal ReadOnlyMemory<byte> SubjectPublicKey;
    }
}
