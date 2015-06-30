
namespace System.Net
{
    public static class HostInformation
    {
        public static string DomainName
        {
            get
            {
                var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

                return properties.DomainName;
            }
        }
    }
}
