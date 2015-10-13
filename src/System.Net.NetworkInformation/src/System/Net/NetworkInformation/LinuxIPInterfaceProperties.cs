// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPInterfaceProperties : UnixIPInterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly string _dnsSuffix;
        private readonly IPAddressCollection _dnsAddresses;
        private readonly GatewayIPAddressInformationCollection _gatewayAddresses;
        private readonly LinuxIPv4InterfaceProperties _ipv4Properties;
        private readonly LinuxIPv6InterfaceProperties _ipv6Properties;

        public LinuxIPInterfaceProperties(LinuxNetworkInterface lni) : base(lni)
        {
            _linuxNetworkInterface = lni;
            _dnsSuffix = GetDnsSuffix();
            _dnsAddresses = GetDnsAddresses();
            _gatewayAddresses = GetGatewayAddresses();
            _ipv4Properties = new LinuxIPv4InterfaceProperties(lni);
            _ipv6Properties = new LinuxIPv6InterfaceProperties(lni);
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
