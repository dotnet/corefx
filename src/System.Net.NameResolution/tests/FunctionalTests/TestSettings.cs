namespace NCLTest.NameResolution
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    internal static class TestSettings
    {
        public static readonly string LocalHost = "localhost";

        public static Task<IPAddress> GetLocalIPAddress()
        {
            return ResolveHost(TestSettings.LocalHost, TestSettings.AddressFamily);
        }

        public static AddressFamily AddressFamily
        {
            get
            {
                return AddressFamily.InterNetworkV6;
            }
        }

        private static async Task<IPAddress> ResolveHost(string host, AddressFamily family)
        {
            var hostEntry = await Dns.GetHostEntryAsync(host);

            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == family)
                {
                    Logger.LogInformation("ResolveHost: {0} translated to {1}", host, address);
                    return address;
                }
            }
            return null;
        }
    }
}

