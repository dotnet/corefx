// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


using System.Net;

namespace System.Net.NetworkInformation
{
    /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation"]/*' />
    /// <summary>Specifies the Multicast addresses for an interface.</summary>
    /// </platnote>
    internal class SystemMulticastIPAddressInformation : MulticastIPAddressInformation
    {
        private SystemIPAddressInformation _innerInfo;

        private SystemMulticastIPAddressInformation()
        {
        }

        public SystemMulticastIPAddressInformation(SystemIPAddressInformation addressInfo)
        {
            _innerInfo = addressInfo;
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.Address"]/*' />
        public override IPAddress Address { get { return _innerInfo.Address; } }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.Transient"]/*' />
        /// <summary>The address is a cluster address and shouldn't be used by most applications.</summary>
        public override bool IsTransient
        {
            get
            {
                return (_innerInfo.IsTransient);
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPAddressInformation.DnsEligible"]/*' />
        /// <summary>This address can be used for DNS.</summary>
        public override bool IsDnsEligible
        {
            get
            {
                return (_innerInfo.IsDnsEligible);
            }
        }


        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.PrefixOrigin"]/*' />
        public override PrefixOrigin PrefixOrigin
        {
            get
            {
                return PrefixOrigin.Other;
            }
        }

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.SuffixOrigin"]/*' />
        public override SuffixOrigin SuffixOrigin
        {
            get
            {
                return SuffixOrigin.Other;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.DuplicateAddressDetectionState"]/*' />
        /// <summary>IPv6 only.  Specifies the duplicate address detection state. Only supported
        /// for IPv6. If called on an IPv4 address, will throw a "not supported" exception.</summary>
        public override DuplicateAddressDetectionState DuplicateAddressDetectionState
        {
            get
            {
                return DuplicateAddressDetectionState.Invalid;
            }
        }


        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.ValidLifetime"]/*' />
        /// <summary>Specifies the valid lifetime of the address in seconds.</summary>
        public override long AddressValidLifetime
        {
            get
            {
                return 0;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.PreferredLifetime"]/*' />
        /// <summary>Specifies the prefered lifetime of the address in seconds.</summary>

        public override long AddressPreferredLifetime
        {
            get
            {
                return 0;
            }
        }
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.PreferredLifetime"]/*' />

        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="IPMulticastAddressInformation.DhcpLeaseLifetime"]/*' />
        /// <summary>Specifies the prefered lifetime of the address in seconds.</summary>
        public override long DhcpLeaseLifetime
        {
            get
            {
                return 0;
            }
        }


        internal static MulticastIPAddressInformationCollection ToMulticastIpAddressInformationCollection(IPAddressInformationCollection addresses)
        {
            MulticastIPAddressInformationCollection multicastList = new MulticastIPAddressInformationCollection();
            foreach (IPAddressInformation addressInfo in addresses)
            {
                multicastList.InternalAdd(new SystemMulticastIPAddressInformation((SystemIPAddressInformation)addressInfo));
            }
            return multicastList;
        }
    }
}

