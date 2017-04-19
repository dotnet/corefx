// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Sys
    {
        internal const int IPv4AddressBytes = 4;
        internal const int IPv6AddressBytes = 16;

        internal const int MAX_IP_ADDRESS_BYTES = 16;

        internal const int INET_ADDRSTRLEN = 22;
        internal const int INET6_ADDRSTRLEN = 65;

        // NOTE: `_isIPv6` cannot be of type `bool` because `bool` is not a blittable type and this struct is
        //       embedded in other structs for interop purposes.
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct IPAddress : IEquatable<IPAddress>
        {
            public bool IsIPv6
            {
                get { return _isIPv6 != 0; }
                set { _isIPv6 = value ? 1u : 0u; }
            }

            internal fixed byte Address[MAX_IP_ADDRESS_BYTES]; // Buffer to fit an IPv4 or IPv6 address
            private  uint _isIPv6;                             // Non-zero if this is an IPv6 address; zero for IPv4.
            internal uint ScopeId;                             // Scope ID (IPv6 only)

            public bool Equals(IPAddress other)
            {
                int addressByteCount;
                if (IsIPv6)
                {
                    if (!other.IsIPv6)
                    {
                        return false;
                    }
                    if (ScopeId != other.ScopeId)
                    {
                        return false;
                    }

                    addressByteCount = IPv6AddressBytes;
                }
                else
                {
                    if (other.IsIPv6)
                    {
                        return false;
                    }

                    addressByteCount = IPv4AddressBytes;
                }

                fixed (byte* thisAddress = Address)
                {
                    for (int i = 0; i < addressByteCount; i++)
                    {
                        if (thisAddress[i] != other.Address[i])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
