// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
    internal static class IPAddressUtil
    {
        /// <summary>
        /// Returns a value indicating whether the given IPAddress is a multicast address.
        /// </summary>
        /// <param name="address">The address to test.</param>
        /// <returns>True if the address is a multicast address; false otherwise.</returns>
        public static bool IsMulticast(IPAddress address)
        {
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return address.IsIPv6Multicast;
            }
            else
            {
                byte firstByte = address.GetAddressBytes()[0];
                return firstByte >= 224 && firstByte <= 239;
            }
        }

        /// <summary>
        /// Convert a CIDR prefix length to a subnet mask "255.255.255.0" format.
        /// </summary>
        /// <param name="prefixLength">Length of the prefix.</param>
        /// <param name="family">AddressFamily for the mask.</param>
        /// <returns>netmask corresponsing to prefix length.</returns>
        public static IPAddress PrefixLengthToSubnetMask(byte prefixLength, AddressFamily family)
        {
            Debug.Assert((0 <= prefixLength) && (prefixLength <= 126));
            Debug.Assert((family == AddressFamily.InterNetwork) || (family == AddressFamily.InterNetworkV6));

            Span<byte> addressBytes = (family == AddressFamily.InterNetwork) ?
                stackalloc byte[4] :
                stackalloc byte[16];
            addressBytes.Clear();

            Debug.Assert(prefixLength <= (addressBytes.Length * 8));

            // Enable bits one at a time from left/high to right/low.
            for (int bit = 0; bit < prefixLength; bit++)
            {
                addressBytes[bit / 8] |= (byte)(0x80 >> (bit % 8));
            }

            return new IPAddress(addressBytes);
        }

        /// <summary>
        /// Copies the address bytes out of the given native info's buffer and constructs a new IPAddress.
        /// </summary>
        /// <param name="addressInfo">A pointer to a native IpAddressInfo structure.</param>
        /// <returns>A new IPAddress created with the information in the native structure.</returns>
        public static unsafe IPAddress GetIPAddressFromNativeInfo(Interop.Sys.IpAddressInfo* addressInfo)
        {
            IPAddress ipAddress = new IPAddress(new ReadOnlySpan<byte>(addressInfo->AddressBytes, addressInfo->NumAddressBytes));
            return ipAddress;
        }
    }
}
