// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.NetworkInformation
{
    /// Provides information about a network interface address.
    internal class LinuxGatewayIPAddressInformation : GatewayIPAddressInformation
    {
        private readonly IPAddress _address;

        public LinuxGatewayIPAddressInformation(IPAddress address)
        {
            _address = address;
        }

        /// Gets the Internet Protocol (IP) address.
        public override IPAddress Address
        {
            get
            {
                return _address;
            }
        }
    }
}
