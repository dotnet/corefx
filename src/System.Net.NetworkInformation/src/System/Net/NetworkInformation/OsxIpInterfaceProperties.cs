// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxIpInterfaceProperties : UnixIPInterfaceProperties
    {
        private readonly OsxIPv4InterfaceProperties _ipv4Properties;
        private readonly OsxIPv6InterfaceProperties _ipv6Properties;

        public OsxIpInterfaceProperties(OsxNetworkInterface oni) : base(oni)
        {
            _ipv4Properties = new OsxIPv4InterfaceProperties(oni);
            _ipv6Properties = new OsxIPv6InterfaceProperties(oni);
        }

        public override IPAddressInformationCollection AnycastAddresses
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IPAddressCollection DhcpServerAddresses
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IPAddressCollection DnsAddresses
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string DnsSuffix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override GatewayIPAddressInformationCollection GatewayAddresses
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsDnsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsDynamicDnsEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override IPAddressCollection WinsServersAddresses
        {
            get
            {
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
    }
}
