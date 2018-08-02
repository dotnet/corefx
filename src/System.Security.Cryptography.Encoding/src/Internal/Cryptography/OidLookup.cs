// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Internal.Cryptography
{
    internal static partial class OidLookup
    {
        private static readonly ConcurrentDictionary<string, string> s_lateBoundOidToFriendlyName =
            new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> s_lateBoundFriendlyNameToOid =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        //
        // Attempts to map a friendly name to an OID. Returns null if not a known name.
        //
        public static string ToFriendlyName(string oid, OidGroup oidGroup, bool fallBackToAllGroups)
        {
            if (oid == null)
                throw new ArgumentNullException(nameof(oid));

            string mappedName;
            bool shouldUseCache = ShouldUseCache(oidGroup);

            // On Unix shouldUseCache is always true, so no matter what OidGroup is passed in the Windows
            // friendly name will be returned.
            //
            // On Windows shouldUseCache is only true for OidGroup.All, because otherwise the OS may filter
            // out the answer based on the group criteria.
            if (shouldUseCache)
            {
                if (OidLookupTable.OidToFriendlyName.TryGetValue(oid, out mappedName) ||
                    OidLookupTable.CompatOids.TryGetValue(oid, out mappedName) ||
                    s_lateBoundOidToFriendlyName.TryGetValue(oid, out mappedName))
                {
                    return mappedName;
                }
            }

            mappedName = NativeOidToFriendlyName(oid, oidGroup, fallBackToAllGroups);

            if (shouldUseCache && mappedName != null)
            {
                s_lateBoundOidToFriendlyName.TryAdd(oid, mappedName);

                // Don't add the reverse here.  Just because oid => name doesn't mean name => oid.
                // And don't bother doing the reverse lookup proactively, just wait until they ask for it.
            }

            return mappedName;
        }

        //
        // Attempts to retrieve the friendly name for an OID. Returns null if not a known or valid OID.
        //
        public static string ToOid(string friendlyName, OidGroup oidGroup, bool fallBackToAllGroups)
        {
            if (friendlyName == null)
                throw new ArgumentNullException(nameof(friendlyName));
            if (friendlyName.Length == 0)
                return null;

            string mappedOid;
            bool shouldUseCache = ShouldUseCache(oidGroup);

            if (shouldUseCache)
            {
                if (OidLookupTable.FriendlyNameToOid.TryGetValue(friendlyName, out mappedOid) ||
                    s_lateBoundFriendlyNameToOid.TryGetValue(friendlyName, out mappedOid))
                {
                    return mappedOid;
                }
            }

            mappedOid = NativeFriendlyNameToOid(friendlyName, oidGroup, fallBackToAllGroups);

            if (shouldUseCache && mappedOid != null)
            {
                s_lateBoundFriendlyNameToOid.TryAdd(friendlyName, mappedOid);

                // Don't add the reverse here.  Friendly Name => OID is a case insensitive search,
                // so the casing provided as input here may not be the 'correct' one.  Just let
                // ToFriendlyName capture the response and cache it itself.
            }

            return mappedOid;
        }
    }
}
