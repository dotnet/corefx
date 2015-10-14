namespace System.Net.NetworkInformation
{
    internal class OsxIPv6InterfaceProperties : UnixIPv6InterfaceProperties
    {
        public OsxIPv6InterfaceProperties(OsxNetworkInterface oni)
            : base(oni)
        {
        }

        public override int Mtu
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            throw new NotImplementedException();
        }
    }
}