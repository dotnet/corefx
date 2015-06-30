// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics.Contracts;

namespace System.Net.NetworkInformation
{
    /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation"]/*' />
    /// <summary>Specifies the unicast addresses for an interface.</summary>
    internal class SystemUnicastIPAddressInformation : UnicastIPAddressInformation
    {
        private long dhcpLeaseLifetime;
        private SystemIPAddressInformation innerInfo;
        private IPAddress ipv4Mask;
        private PrefixOrigin prefixOrigin;
        private SuffixOrigin suffixOrigin;
        private DuplicateAddressDetectionState dadState;
        private uint validLifetime;
        private uint preferredLifetime;
        private byte prefixLength;

        internal SystemUnicastIPAddressInformation(IpAdapterUnicastAddress adapterAddress)
        {
            IPAddress ipAddress = adapterAddress.address.MarshalIPAddress();
            this.innerInfo = new SystemIPAddressInformation(ipAddress, adapterAddress.flags);
            this.prefixOrigin = adapterAddress.prefixOrigin;
            this.suffixOrigin = adapterAddress.suffixOrigin;
            this.dadState = adapterAddress.dadState;
            this.validLifetime = adapterAddress.validLifetime;
            this.preferredLifetime = adapterAddress.preferredLifetime;
            this.dhcpLeaseLifetime = adapterAddress.leaseLifetime;

            this.prefixLength = adapterAddress.prefixLength;

            // IPv6 returns 0.0.0.0 for consistancy with XP
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                ipv4Mask = PrefixLengthToSubnetMask(prefixLength, ipAddress.AddressFamily);
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.Address"]/*' />
        public override IPAddress Address { get { return innerInfo.Address; } }


        public override IPAddress IPv4Mask
        {
            get
            {
                // The IPv6 equivilant was never available on XP, and we've kept this behavior for legacy reasons.
                // For IPv6 use PrefixLength instead.
                if (Address.AddressFamily != AddressFamily.InterNetwork)
                {
                    return IPAddress.Any;
                }

                return ipv4Mask;
            }
        }

        public override int PrefixLength
        {
            get
            {
                return prefixLength;
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.Transient"]/*' />
        /// <summary>The address is a cluster address and shouldn't be used by most applications.</summary>
        public override bool IsTransient
        {
            get
            {
                return (innerInfo.IsTransient);
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.DnsEligible"]/*' />
        /// <summary>This address can be used for DNS.</summary>
        public override bool IsDnsEligible
        {
            get
            {
                return (innerInfo.IsDnsEligible);
            }
        }


        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.PrefixOrigin"]/*' />
        public override PrefixOrigin PrefixOrigin
        {
            get
            {
                return prefixOrigin;
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.SuffixOrigin"]/*' />
        public override SuffixOrigin SuffixOrigin
        {
            get
            {
                return suffixOrigin;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.DuplicateAddressDetectionState"]/*' />
        /// <summary>IPv6 only.  Specifies the duplicate address detection state. Only supported
        /// for IPv6. If called on an IPv4 address, will throw a "not supported" exception.</summary>
        public override DuplicateAddressDetectionState DuplicateAddressDetectionState
        {
            get
            {
                return dadState;
            }
        }


        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.ValidLifetime"]/*' />
        /// <summary>Specifies the valid lifetime of the address in seconds.</summary>
        public override long AddressValidLifetime
        {
            get
            {
                return validLifetime;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.PreferredLifetime"]/*' />
        /// <summary>Specifies the prefered lifetime of the address in seconds.</summary>

        public override long AddressPreferredLifetime
        {
            get
            {
                return preferredLifetime;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.PreferredLifetime"]/*' />

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPUnicastAddressInformation.DhcpLeaseLifetime"]/*' />
        /// <summary>Specifies the prefered lifetime of the address in seconds.</summary>
        public override long DhcpLeaseLifetime
        {
            get
            {
                return dhcpLeaseLifetime;
            }
        }

        // Helper method that marshals the addressinformation into the classes
        internal static UnicastIPAddressInformationCollection MarshalUnicastIpAddressInformationCollection(IntPtr ptr)
        {
            UnicastIPAddressInformationCollection addressList = new UnicastIPAddressInformationCollection();

            while (ptr != IntPtr.Zero)
            {
                // Get the address
                IpAdapterUnicastAddress addr = Marshal.PtrToStructure<IpAdapterUnicastAddress>(ptr);
                // Add the address to the list
                addressList.InternalAdd(new SystemUnicastIPAddressInformation(addr));
                // Move to the next address in the list
                ptr = addr.next;
            }

            return addressList;
        }

        // Convert a CIDR prefix length to a subnet mask "255.255.255.0" format
        private static IPAddress PrefixLengthToSubnetMask(byte prefixLength, AddressFamily family)
        {
            Contract.Requires((0 <= prefixLength) && (prefixLength <= 126));
            Contract.Requires((family == AddressFamily.InterNetwork) || (family == AddressFamily.InterNetworkV6));

            byte[] addressBytes;
            if (family == AddressFamily.InterNetwork)
            {
                addressBytes = new byte[4];
            }
            else
            { // v6
                addressBytes = new byte[16];
            }

            Contract.Assert(prefixLength < (addressBytes.Length * 8));

            // Enable bits one at a time from left/high to right/low
            for (int bit = 0; bit < prefixLength; bit++)
            {
                addressBytes[bit / 8] |= (byte)(0x80 >> (bit % 8));
            }

            return new IPAddress(addressBytes);
        }
    }
}
