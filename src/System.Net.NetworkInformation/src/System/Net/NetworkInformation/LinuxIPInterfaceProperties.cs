// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPInterfaceProperties : IPInterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly string _dnsSuffix;
        private readonly UnicastIPAddressInformationCollection _unicastAddresses;
        private readonly MulticastIPAddressInformationCollection _multicastAddreses;
        private readonly IPAddressCollection _dnsAddresses;
        private readonly GatewayIPAddressInformationCollection _gatewayAddresses;
        private readonly LinuxIPv4InterfaceProperties _ipv4Properties;
        private readonly LinuxIPv6InterfaceProperties _ipv6Properties;

        public LinuxIPInterfaceProperties(LinuxNetworkInterface lni)
        {
            _linuxNetworkInterface = lni;
            _dnsSuffix = GetDnsSuffix();
            _unicastAddresses = GetUnicastAddresses();
            _multicastAddreses = GetMulticastAddresses();
            _dnsAddresses = GetDnsAddresses();
            _gatewayAddresses = GetGatewayAddresses();
            _ipv4Properties = new LinuxIPv4InterfaceProperties(_linuxNetworkInterface);
            _ipv6Properties = new LinuxIPv6InterfaceProperties(_linuxNetworkInterface);
        }

        public override bool IsDnsEnabled
        {
            get
            {
                // /etc/nsswitchc.conf may be of use here.
                // The absence of "dns" in that file could indicate that DNS is not enabled.
                return DnsAddresses.Count > 0;
            }
        }

        public override string DnsSuffix { get { return _dnsSuffix; } }

        public override bool IsDynamicDnsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override UnicastIPAddressInformationCollection UnicastAddresses { get { return _unicastAddresses; } }

        public override MulticastIPAddressInformationCollection MulticastAddresses { get { return _multicastAddreses; } }

        public override IPAddressInformationCollection AnycastAddresses
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override IPAddressCollection DnsAddresses { get { return _dnsAddresses; } }

        public override GatewayIPAddressInformationCollection GatewayAddresses { get { return _gatewayAddresses; } }

        public override IPAddressCollection DhcpServerAddresses
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IPAddressCollection WinsServersAddresses
        {
            get
            {
                // TODO: Can we return false? Is this Windows-specific?
                throw new PlatformNotSupportedException();
            }
        }

        public override IPv4InterfaceProperties GetIPv4Properties()
        {
            return _ipv4Properties;
        }

        public override IPv6InterfaceProperties GetIPv6Properties()
        {
            return _ipv6Properties;
        }

        private static string GetDnsSuffix()
        {
            string data = File.ReadAllText(LinuxNetworkFiles.EtcResolvConfFile);
            RowConfigReader rcr = new RowConfigReader(data);
            string dnsSuffix;

            if (rcr.TryGetNextValue("search", out dnsSuffix))
            {
                return dnsSuffix;
            }
            else
            {
                return string.Empty;
            }
        }

        private UnicastIPAddressInformationCollection GetUnicastAddresses()
        {
            var collection = new UnicastIPAddressInformationCollection();
            foreach (IPAddress address in _linuxNetworkInterface.Addresses.Where((addr) => !IsMulticast(addr)))
            {
                IPAddress netMask = (address.AddressFamily == AddressFamily.InterNetwork)
                                    ? _linuxNetworkInterface.GetNetMaskForIPv4Address(address)
                                    : IPAddress.Any; // Windows compatibility
                collection.InternalAdd(new LinuxUnicastIPAddressInformation(address, netMask));
            }

            return collection;
        }

        private MulticastIPAddressInformationCollection GetMulticastAddresses()
        {
            var collection = new MulticastIPAddressInformationCollection();
            foreach (IPAddress address in _linuxNetworkInterface.Addresses.Where(IsMulticast))
            {
                collection.InternalAdd(new LinuxMulticastIPAddressInformation(address));
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

        private static IPAddressCollection GetDnsAddresses()
        {
            // Parse /etc/resolv.conf for all of the "nameserver" entries.
            // These are the DNS servers the machine is configured to use.
            string data = File.ReadAllText(LinuxNetworkFiles.EtcResolvConfFile);
            RowConfigReader rcr = new RowConfigReader(data);
            InternalIPAddressCollection addresses = new InternalIPAddressCollection();

            string addressString = null;
            while (rcr.TryGetNextValue("nameserver", out addressString))
            {
                IPAddress parsedAddress;
                if (IPAddress.TryParse(addressString, out parsedAddress))
                {
                    addresses.InternalAdd(parsedAddress);
                }
            }

            return addresses;
        }

        // /proc/net/route contains some information about gateway addresses,
        // and seperates the information about by each interface.
        // ** TODO: /proc/net/ipv6_route contains some other routing-related information,
        //    but I don't believe it exposes any additional gateway IP addresses. **
        public GatewayIPAddressInformationCollection GetGatewayAddresses()
        {
            GatewayIPAddressInformationCollection collection = new GatewayIPAddressInformationCollection();
            // Columns are as follows (first-line header):
            // Iface  Destination  Gateway  Flags  RefCnt  Use  Metric  Mask  MTU  Window  IRTT
            string[] fileLines = File.ReadAllLines(LinuxNetworkFiles.Ipv4RouteFile);
            foreach (string line in fileLines)
            {
                if (line.StartsWith(_linuxNetworkInterface.Name))
                {
                    StringParser parser = new StringParser(line, '\t', skipEmpty: true);
                    parser.MoveNext();
                    parser.MoveNextOrFail();
                    string gatewayIPHex = parser.MoveAndExtractNext();
                    long addressValue = Convert.ToInt64(gatewayIPHex, 16);
                    IPAddress address = new IPAddress(addressValue);
                    collection.InternalAdd(new LinuxGatewayIPAddressInformation(address));
                }
            }

            return collection;
        }
    }
}
