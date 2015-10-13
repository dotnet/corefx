namespace System.Net.NetworkInformation
{
    internal class OsxIpInterfaceProperties : UnixIPInterfaceProperties
    {
        private readonly string _name;

        public OsxIpInterfaceProperties(OsxNetworkInterface oni) : base(oni)
        {
            _name = oni.Name;
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
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override IPv6InterfaceProperties GetIPv6Properties()
        {
            throw new NotImplementedException();
        }
    }
}
