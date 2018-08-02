// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static partial class OidLookup
    {
        private static bool ShouldUseCache(OidGroup oidGroup)
        {
            return true;
        }

        private static string NativeOidToFriendlyName(string oid, OidGroup oidGroup, bool fallBackToAllGroups)
        {
            string friendlyName;

            if (OidLookupTable.UnixExtraOidToFriendlyName.TryGetValue(oid, out friendlyName))
            {
                return friendlyName;
            }

            return null;
        }

        private static string NativeFriendlyNameToOid(string friendlyName, OidGroup oidGroup, bool fallBackToAllGroups)
        {
            string oid;

            if (OidLookupTable.UnixExtraFriendlyNameToOid.TryGetValue(friendlyName, out oid))
            {
                return oid;
            }

            return null;
        }
    }
}
