
using System.Net.Sockets;
using System;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


namespace System.Net.NetworkInformation
{
    /// <summary>
    /// Provides information specific to a network
    /// interface.
    /// </summary>
    /// <remarks>
    /// <para>Provides information specific to a network interface. A network interface can have more 
    /// than one IPAddress associated with it. We call the native GetAdaptersAddresses api to
    /// prepopulate all of the interface instances and most of their associated information.</para>
    /// </remarks>
    internal class SystemIPInterfaceProperties : IPInterfaceProperties
    {
        //these are valid for all interfaces
        private bool _dnsEnabled = false;
        private bool _dynamicDnsEnabled = false;
        private IPAddressCollection _dnsAddresses = null;
        private UnicastIPAddressInformationCollection _unicastAddresses = null;
        private MulticastIPAddressInformationCollection _multicastAddresses = null;
        private IPAddressInformationCollection _anycastAddresses = null;
        private AdapterFlags _adapterFlags;
        private string _dnsSuffix;
        private SystemIPv4InterfaceProperties _ipv4Properties;
        private SystemIPv6InterfaceProperties _ipv6Properties;
        private IPAddressCollection _winsServersAddresses;
        private GatewayIPAddressInformationCollection _gatewayAddresses;
        private IPAddressCollection _dhcpServers;

        // This constructor is for Vista and newer
        internal SystemIPInterfaceProperties(FIXED_INFO fixedInfo, IpAdapterAddresses ipAdapterAddresses)
        {
            _adapterFlags = ipAdapterAddresses.flags;
            _dnsSuffix = ipAdapterAddresses.dnsSuffix;
            _dnsEnabled = fixedInfo.enableDns;
            _dynamicDnsEnabled = ((ipAdapterAddresses.flags & AdapterFlags.DnsEnabled) > 0);

            _multicastAddresses = SystemMulticastIPAddressInformation.ToMulticastIpAddressInformationCollection(
                IpAdapterAddress.MarshalIpAddressInformationCollection(ipAdapterAddresses.firstMulticastAddress));
            _dnsAddresses = IpAdapterAddress.MarshalIpAddressCollection(ipAdapterAddresses.firstDnsServerAddress);
            _anycastAddresses = IpAdapterAddress.MarshalIpAddressInformationCollection(
                ipAdapterAddresses.firstAnycastAddress);
            _unicastAddresses = SystemUnicastIPAddressInformation.MarshalUnicastIpAddressInformationCollection(
                ipAdapterAddresses.firstUnicastAddress);
            _winsServersAddresses = IpAdapterAddress.MarshalIpAddressCollection(
                ipAdapterAddresses.firstWinsServerAddress);
            _gatewayAddresses = SystemGatewayIPAddressInformation.ToGatewayIpAddressInformationCollection(
                IpAdapterAddress.MarshalIpAddressCollection(ipAdapterAddresses.firstGatewayAddress));

            _dhcpServers = new IPAddressCollection();
            if (ipAdapterAddresses.dhcpv4Server.address != IntPtr.Zero)
                _dhcpServers.InternalAdd(ipAdapterAddresses.dhcpv4Server.MarshalIPAddress());
            if (ipAdapterAddresses.dhcpv6Server.address != IntPtr.Zero)
                _dhcpServers.InternalAdd(ipAdapterAddresses.dhcpv6Server.MarshalIPAddress());

            if ((_adapterFlags & AdapterFlags.IPv4Enabled) != 0)
            {
                _ipv4Properties = new SystemIPv4InterfaceProperties(fixedInfo, ipAdapterAddresses);
            }

            if ((_adapterFlags & AdapterFlags.IPv6Enabled) != 0)
            {
                _ipv6Properties = new SystemIPv6InterfaceProperties(ipAdapterAddresses.ipv6Index,
                    ipAdapterAddresses.mtu, ipAdapterAddresses.zoneIndices);
            }
        }

        public override bool IsDnsEnabled { get { return _dnsEnabled; } }

        public override bool IsDynamicDnsEnabled { get { return _dynamicDnsEnabled; } }

        public override IPv4InterfaceProperties GetIPv4Properties()
        {
            if ((_adapterFlags & AdapterFlags.IPv4Enabled) == 0)
            {
                throw new NetworkInformationException(SocketError.ProtocolNotSupported);
            }
            return _ipv4Properties;
        }

        public override IPv6InterfaceProperties GetIPv6Properties()
        {
            if ((_adapterFlags & AdapterFlags.IPv6Enabled) == 0)
            {
                throw new NetworkInformationException(SocketError.ProtocolNotSupported);
            }
            return _ipv6Properties;
        }

        public override string DnsSuffix
        {
            get
            {
                return _dnsSuffix;
            }
        }

        //returns the addresses specified by the address type.
        public override IPAddressInformationCollection AnycastAddresses
        {
            get
            {
                return _anycastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override UnicastIPAddressInformationCollection UnicastAddresses
        {
            get
            {
                return _unicastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override MulticastIPAddressInformationCollection MulticastAddresses
        {
            get
            {
                return _multicastAddresses;
            }
        }

        //returns the addresses specified by the address type.
        public override IPAddressCollection DnsAddresses
        {
            get
            {
                return _dnsAddresses;
            }
        }

        /// <summary>IP Address of the default gateway.</summary>
        public override GatewayIPAddressInformationCollection GatewayAddresses
        {
            get
            {
                return _gatewayAddresses;
            }
        }

        public override IPAddressCollection DhcpServerAddresses
        {
            get
            {
                return _dhcpServers;
            }
        }

        public override IPAddressCollection WinsServersAddresses
        {
            get
            {
                return _winsServersAddresses;
            }
        }
    }
}
