// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
    /// Provides information about a network interface's unicast address
    internal class UnixUnicastIPAddressInformation : UnicastIPAddressInformation
    {
        private readonly IPAddress _address;
        private readonly int _prefixLength;

        public UnixUnicastIPAddressInformation(IPAddress address, int prefixLength)
        {
            _address = address;
            _prefixLength = prefixLength;
        }

        public override IPAddress Address { get { return _address; } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is legal to appear in a Domain Name System (DNS) server database.
        public override bool IsDnsEligible { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is transient.
        public override bool IsTransient { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// [To be supplied.]
        public override long AddressPreferredLifetime { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// [To be supplied.]
        public override long AddressValidLifetime { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// Specifies the amount of time remaining on the Dynamic Host Configuration Protocol (DHCP) lease for this IP address.
        public override long DhcpLeaseLifetime { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// Gets a value that indicates the state of the duplicate address detection algorithm.
        public override DuplicateAddressDetectionState DuplicateAddressDetectionState { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }
        /// Gets a value that identifies the source of a unicast IP address prefix.
        public override PrefixOrigin PrefixOrigin { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        /// Gets a value that identifies the source of a unicast IP address suffix.
        public override SuffixOrigin SuffixOrigin { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        public override IPAddress IPv4Mask
        {
            get
            {
                // The IPv6 equivalent was never available on down-level platforms.
                // We've kept this behavior for legacy reasons. For IPv6 use PrefixLength instead.
                if (Address.AddressFamily != AddressFamily.InterNetwork || _prefixLength == 0)
                {
                    return IPAddress.Any;
                }

                return PrefixLengthToSubnetMask((byte)_prefixLength, AddressFamily.InterNetwork);
            }
        }

        public override int PrefixLength { get { return _prefixLength; } }
    }
}
