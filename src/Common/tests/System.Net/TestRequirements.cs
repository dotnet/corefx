namespace NCLTest.Common
{
    using CoreFXTestLibrary;
    using System;
    using System.Text;
#if PROJECTN
    using Windows.Networking;
    using Windows.Networking.Connectivity;
#else
    using System.Net.NetworkInformation;
#endif
    public static class TestRequirements
    {
        private static string hostName = null;
        private static string domainName = null;
        
        public static void CheckIPv6Support()
        {
        }

        public static void CheckIPv4Support()
        {
        }

        public static void CheckWDigestAuthAvailable()
        {
            if (!IsWDigestAuthAvailable())
            {
                //Assert.Inconclusive(
                Console.WriteLine(
                    @"When running on Windows Blue, WDigest is disabled. This test requires " +
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\WDigest" +
                    @"\UseLogonCredential DWORD value set to 1.");
            }
        }

        public static bool IsWDigestAuthAvailable()
        {
            return false;
        }

        public static string GetHostName()
        {
            EnsureNetworkSettingsInitialized();
            
            return hostName;
        }
        
        public static string GetDomainName()
        {
            EnsureNetworkSettingsInitialized();
            
            return domainName;
        }
        
        public static bool IsHostDomainJoined()
        {
            return !string.IsNullOrEmpty(GetDomainName());
        }        
        
        private static void EnsureNetworkSettingsInitialized()
        {
            if (hostName == null)
            {
#if PROJECTN            
                var hostNamesList = NetworkInformation.GetHostNames();

                foreach (var entry in hostNamesList)
                {
                    // The first DomainName entry in the GetHostNames() list
                    // is the fdqn of the machine itself.
                    if (entry.Type == HostNameType.DomainName)
                    {
                        var host = entry.ToString();
                        var dot = host.IndexOf('.');
                        if (dot != -1)
                        {
                            // The machine is domain joined.
                            hostName = host.Substring(0, dot);
                            domainName = host.Substring(dot+1);
                        }
                        else
                        {
                            // The machine is not domain joined.
                            hostName = host;
                            domainName = string.Empty;
                        }
                        
                        break;
                    }
                }
#else
                var properties = IPGlobalProperties.GetIPGlobalProperties();
                hostName = properties.HostName;
                domainName = properties.DomainName;
#endif                
            }
        }
        
    }
}
