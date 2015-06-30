// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


using System.Net;

namespace System.Net.NetworkInformation
{
    //this is the main addressinformation class that contains the ipaddress
    //and other properties
    internal class SystemIPAddressInformation : IPAddressInformation
    {
        IPAddress address;
        internal bool transient = false;
        internal bool dnsEligible = true;

        internal SystemIPAddressInformation(IPAddress address, AdapterAddressFlags flags)
        {
            this.address = address;
            transient = (flags & AdapterAddressFlags.Transient) > 0;
            dnsEligible = (flags & AdapterAddressFlags.DnsEligible) > 0;
        }

        public override IPAddress Address { get { return address; } }

        /// <summary>The address is a cluster address and shouldn't be used by most applications.</summary>
        public override bool IsTransient
        {
            get
            {
                return (transient);
            }
        }

        /// <summary>This address can be used for DNS.</summary>
        public override bool IsDnsEligible
        {
            get
            {
                return (dnsEligible);
            }
        }
    }
}

