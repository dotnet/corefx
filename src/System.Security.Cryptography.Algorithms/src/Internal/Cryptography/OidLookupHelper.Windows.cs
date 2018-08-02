// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static class OidLookupHelper
    {
        internal static void VerifyValidHashAlgorithm(string name)
        {
            Oid.FromFriendlyName(name, OidGroup.HashAlgorithm);
        }
    }
}
