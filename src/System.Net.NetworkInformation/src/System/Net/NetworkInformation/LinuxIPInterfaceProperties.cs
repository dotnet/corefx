// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPInterfaceProperties : UnixIPInterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly GatewayIPAddressInformationCollection _gatewayAddresses;
        private readonly LinuxIPv4InterfaceProperties _ipv4Properties;
        private readonly LinuxIPv6InterfaceProperties _ipv6Properties;

        public LinuxIPInterfaceProperties(LinuxNetworkInterface lni) : base(lni)
        {
            _linuxNetworkInterface = lni;
            _gatewayAddresses = GetGatewayAddresses();
            _ipv4Properties = new LinuxIPv4InterfaceProperties(lni);
            _ipv6Properties = new LinuxIPv6InterfaceProperties(lni);
        }

        public override bool IsDynamicDnsEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override IPAddressInformationCollection AnycastAddresses
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override GatewayIPAddressInformationCollection GatewayAddresses { get { return _gatewayAddresses; } }

        public override IPAddressCollection DhcpServerAddresses
        {
            get
            {
                // TODO: Check for the existence of /var/lib/dhcp/dhclient.leases,
                // and use the "option dhcp-server-identifier" element.
                // This should be available per-interface, per-lease.
                throw new NotImplementedException();
            }
        }

        public override IPAddressCollection WinsServersAddresses
        {
            get
            {
                // TODO: Parse /etc/samba/dhcp.conf
                // for 'wins server = <iface>:<name>' items.
                throw new NotImplementedException();
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

        // /proc/net/route contains some information about gateway addresses,
        // and seperates the information about by each interface.
        public GatewayIPAddressInformationCollection GetGatewayAddresses()
        {
            GatewayIPAddressInformationCollection collection = new GatewayIPAddressInformationCollection();
            // Columns are as follows (first-line header):
            // Iface  Destination  Gateway  Flags  RefCnt  Use  Metric  Mask  MTU  Window  IRTT
            string[] fileLines = File.ReadAllLines(NetworkFiles.Ipv4RouteFile);
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
                    collection.InternalAdd(new SimpleGatewayIPAddressInformation(address));
                }
            }

            return collection;
        }
    }
}
