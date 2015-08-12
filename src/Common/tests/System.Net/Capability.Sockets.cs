using System.Net.Sockets;

namespace System.Net.Test.Common
{
    public static partial class Capability
    {
        public static bool IPv6Support()
        {
            return Socket.OSSupportsIPv6;
        }

        public static bool IPv4Support()
        {
            return Socket.OSSupportsIPv4;
        }
    }
}