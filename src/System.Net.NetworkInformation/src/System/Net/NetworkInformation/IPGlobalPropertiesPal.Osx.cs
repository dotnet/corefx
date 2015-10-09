namespace System.Net.NetworkInformation
{
    internal static class IPGlobalPropertiesPal
    {
        public static IPGlobalProperties GetIPGlobalProperties()
        {
            return new OsxIPGlobalProperties();
        }
    }
}
