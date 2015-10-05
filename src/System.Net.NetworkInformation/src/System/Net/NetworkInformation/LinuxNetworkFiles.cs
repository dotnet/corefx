namespace System.Net.NetworkInformation
{
    internal static class LinuxNetworkFiles
    {
        public const string InterfaceListingFile = "/proc/net/dev";
        public const string SnmpV4StatsFile = "/proc/net/snmp";
        public const string SnmpV6StatsFile = "/proc/net/snmp6";
        public const string Ipv4ConfigFolder = "/proc/sys/net/ipv4/conf";
        public const string Ipv6ConfigFolder = "/proc/sys/net/ipv6/conf";
        public const string Ipv4RouteFile = "/proc/net/route";
        public const string Ipv6RouteFile = "/proc/net/ipv6_route";
        public const string SockstatFile = "/proc/net/sockstat";
        public const string Sockstat6File = "/proc/net/sockstat6";
        public const string SysClassNetFolder = "/sys/class/net";
        public const string ProcSysNetFolder = "/proc/sys/net";
        public const string EtcResolvConfFile = "/etc/resolv.conf";
        public const string Tcp4ConnectionsFile = "/proc/net/tcp";
        public const string Tcp6ConnectionsFile = "/proc/net/tcp6";
        public const string Udp4ConnectionsFile = "/proc/net/udp";
        public const string Udp6ConnectionsFile = "/proc/net/udp6";
    }
}
