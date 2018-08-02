// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static class OidLookupHelper
    {
        internal static void VerifyValidHashAlgorithm(string name)
        {
            if (FromFriendlyName(name) == null)
                throw new CryptographicException(SR.Cryptography_Oid_InvalidName);
        }

        private static string FromFriendlyName(string oid)
        {
            string mappedName;
            if (OidLookupTable.FriendlyNameToOid.TryGetValue(oid, out mappedName) ||
                OidLookupTable.UnixExtraFriendlyNameToOid.TryGetValue(oid, out mappedName))
            {
                return mappedName;
            }

            return null;
        }
    }
}
