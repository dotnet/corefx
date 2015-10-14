// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
    internal abstract class UnixIPInterfaceProperties : IPInterfaceProperties
    {
        private readonly UnicastIPAddressInformationCollection _unicastAddresses;
        private readonly MulticastIPAddressInformationCollection _multicastAddreses;

        public UnixIPInterfaceProperties(UnixNetworkInterface uni)
        {
            _unicastAddresses = GetUnicastAddresses(uni);
            _multicastAddreses = GetMulticastAddresses(uni);
        }

        public sealed override UnicastIPAddressInformationCollection UnicastAddresses { get { return _unicastAddresses; } }

        public sealed override MulticastIPAddressInformationCollection MulticastAddresses { get { return _multicastAddreses; } }

        private UnicastIPAddressInformationCollection GetUnicastAddresses(UnixNetworkInterface uni)
        {
            var collection = new UnicastIPAddressInformationCollection();
            foreach (IPAddress address in uni.Addresses.Where((addr) => !IsMulticast(addr)))
            {
                IPAddress netMask = (address.AddressFamily == AddressFamily.InterNetwork)
                                    ? uni.GetNetMaskForIPv4Address(address)
                                    : IPAddress.Any; // Windows compatibility
                collection.InternalAdd(new UnixUnicastIPAddressInformation(address, netMask));
            }

            return collection;
        }

        private MulticastIPAddressInformationCollection GetMulticastAddresses(UnixNetworkInterface uni)
        {
            var collection = new MulticastIPAddressInformationCollection();
            foreach (IPAddress address in uni.Addresses.Where(IsMulticast))
            {
                collection.InternalAdd(new UnixMulticastIPAddressInformation(address));
            }

            return collection;
        }

        private static bool IsMulticast(IPAddress address)
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
    }
}
