
    /// <summary><para>
    ///    Provides support for ip configuation information and statistics.
    ///</para></summary>
    ///
namespace System.Net.NetworkInformation {

    using System.Net.Sockets;
    using System;

    /// <summary>
    /// Provides information specific to a network
    /// interface.
    /// </summary>
    /// <remarks>
    /// <para>Provides information specific to a network interface. A network interface can have more 
    /// than one IPAddress associated with it. We call the native GetAdaptersAddresses api to
    /// prepopulate all of the interface instances and most of their associated information.</para>
    /// </remarks>
    internal class SystemIPInterfaceProperties:IPInterfaceProperties {
        //these are valid for all interfaces
        private bool dnsEnabled = false;
        private bool dynamicDnsEnabled = false;
        private IPAddressCollection dnsAddresses = null;
        private UnicastIPAddressInformationCollection unicastAddresses = null;
        private MulticastIPAddressInformationCollection multicastAddresses = null;
        private IPAddressInformationCollection anycastAddresses = null;
        private AdapterFlags adapterFlags;
        private string dnsSuffix;
        private SystemIPv4InterfaceProperties ipv4Properties;
        private SystemIPv6InterfaceProperties ipv6Properties;
        private IPAddressCollection winsServersAddresses;
        private GatewayIPAddressInformationCollection gatewayAddresses;
        private IPAddressCollection dhcpServers;

        // This constructor is for Vista and newer
        internal SystemIPInterfaceProperties(FIXED_INFO fixedInfo, IpAdapterAddresses ipAdapterAddresses) {
            adapterFlags = ipAdapterAddresses.flags;
            dnsSuffix = ipAdapterAddresses.dnsSuffix;
            dnsEnabled = fixedInfo.enableDns;
            dynamicDnsEnabled = ((ipAdapterAddresses.flags & AdapterFlags.DnsEnabled) > 0);

            multicastAddresses = SystemMulticastIPAddressInformation.ToMulticastIpAddressInformationCollection(
                IpAdapterAddress.MarshalIpAddressInformationCollection(ipAdapterAddresses.firstMulticastAddress));
            dnsAddresses = IpAdapterAddress.MarshalIpAddressCollection(ipAdapterAddresses.firstDnsServerAddress);
            anycastAddresses = IpAdapterAddress.MarshalIpAddressInformationCollection(
                ipAdapterAddresses.firstAnycastAddress);
            unicastAddresses = SystemUnicastIPAddressInformation.MarshalUnicastIpAddressInformationCollection(
                ipAdapterAddresses.firstUnicastAddress);
            winsServersAddresses = IpAdapterAddress.MarshalIpAddressCollection(
                ipAdapterAddresses.firstWinsServerAddress);
            gatewayAddresses = SystemGatewayIPAddressInformation.ToGatewayIpAddressInformationCollection(
                IpAdapterAddress.MarshalIpAddressCollection(ipAdapterAddresses.firstGatewayAddress));

            dhcpServers = new IPAddressCollection();
            if (ipAdapterAddresses.dhcpv4Server.address != IntPtr.Zero)
                dhcpServers.InternalAdd(ipAdapterAddresses.dhcpv4Server.MarshalIPAddress());
            if (ipAdapterAddresses.dhcpv6Server.address != IntPtr.Zero)
                dhcpServers.InternalAdd(ipAdapterAddresses.dhcpv6Server.MarshalIPAddress());

            if ((adapterFlags & AdapterFlags.IPv4Enabled) != 0) {
                ipv4Properties = new SystemIPv4InterfaceProperties(fixedInfo, ipAdapterAddresses);
            }

            if ((adapterFlags & AdapterFlags.IPv6Enabled) != 0) {
                ipv6Properties = new SystemIPv6InterfaceProperties(ipAdapterAddresses.ipv6Index, 
                    ipAdapterAddresses.mtu, ipAdapterAddresses.zoneIndices);
            }
        }

        public override bool IsDnsEnabled{get { return dnsEnabled;}}

        public override bool IsDynamicDnsEnabled{get {return dynamicDnsEnabled;}}

        public override IPv4InterfaceProperties GetIPv4Properties(){
            if ((adapterFlags & AdapterFlags.IPv4Enabled) == 0) {
                throw new NetworkInformationException(SocketError.ProtocolNotSupported);
            }
            return ipv4Properties;
        }

        public override IPv6InterfaceProperties GetIPv6Properties(){
            if ((adapterFlags & AdapterFlags.IPv6Enabled) == 0) {
                throw new NetworkInformationException(SocketError.ProtocolNotSupported);
            }
            return ipv6Properties;
        }

        public override string DnsSuffix {
            get {
                return dnsSuffix;
            }
        }
        
        //returns the addresses specified by the address type.
        public override IPAddressInformationCollection AnycastAddresses{
            get{
                return anycastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override UnicastIPAddressInformationCollection UnicastAddresses{
            get{
                return unicastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override MulticastIPAddressInformationCollection MulticastAddresses{
            get{
                return multicastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override IPAddressCollection DnsAddresses{
            get{
                return dnsAddresses;
            }
        }

        /// <summary>IP Address of the default gateway.</summary>
        public override GatewayIPAddressInformationCollection GatewayAddresses{
            get{
                return gatewayAddresses;
            }
        }
                        
        public override IPAddressCollection DhcpServerAddresses{
            get{
                return dhcpServers;
            }
        }

        public override IPAddressCollection WinsServersAddresses{
            get{
                return winsServersAddresses;
            }
        }
    }
}
