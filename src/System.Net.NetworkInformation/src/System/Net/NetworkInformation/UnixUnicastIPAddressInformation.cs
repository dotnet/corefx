// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    /// Provides information about a network interface's unicast address
    internal class UnixUnicastIPAddressInformation : UnicastIPAddressInformation
    {
        private readonly IPAddress _address;
        private readonly IPAddress _netMask;

        public UnixUnicastIPAddressInformation(IPAddress address, IPAddress netMask)
        {
            _address = address;
            _netMask = netMask;
        }

        public override IPAddress Address { get { return _address; } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is legal to appear in a Domain Name System (DNS) server database.
        public override bool IsDnsEligible { get { throw new PlatformNotSupportedException(); } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is transient.
        public override bool IsTransient { get { throw new PlatformNotSupportedException(); } }

        /// [To be supplied.]
        public override long AddressPreferredLifetime { get { throw new PlatformNotSupportedException(); } }

        /// [To be supplied.]
        public override long AddressValidLifetime { get { throw new PlatformNotSupportedException(); } }

        /// Specifies the amount of time remaining on the Dynamic Host Configuration Protocol (DHCP) lease for this IP address.
        public override long DhcpLeaseLifetime { get { throw new PlatformNotSupportedException(); } }

        /// Gets a value that indicates the state of the duplicate address detection algorithm.
        public override DuplicateAddressDetectionState DuplicateAddressDetectionState { get { throw new PlatformNotSupportedException(); } }

        /// Gets a value that identifies the source of a unicast IP address prefix.
        public override PrefixOrigin PrefixOrigin { get { throw new PlatformNotSupportedException(); } }

        /// Gets a value that identifies the source of a unicast IP address suffix.
        public override SuffixOrigin SuffixOrigin { get { throw new PlatformNotSupportedException(); } }

        public override IPAddress IPv4Mask { get { return _netMask; } }

    }
}
