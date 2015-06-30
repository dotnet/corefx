// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.NetworkInformation
{
    /// Provides information about a network interface address.
    internal class SystemGatewayIPAddressInformation : GatewayIPAddressInformation
    {
        IPAddress address;

        private SystemGatewayIPAddressInformation(IPAddress address)
        {
            this.address = address;
        }

        /// Gets the Internet Protocol (IP) address.
        public override IPAddress Address
        {
            get
            {
                return address;
            }
        }

        internal static GatewayIPAddressInformationCollection ToGatewayIpAddressInformationCollection(IPAddressCollection addresses)
        {
            GatewayIPAddressInformationCollection gatewayList = new GatewayIPAddressInformationCollection();
            foreach (IPAddress address in addresses)
            {
                gatewayList.InternalAdd(new SystemGatewayIPAddressInformation(address));
            }
            return gatewayList;
        }
    }
}

