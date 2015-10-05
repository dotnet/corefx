namespace System.Net.NetworkInformation
{
    public abstract partial class IPGlobalProperties
    {
        public static IPGlobalProperties GetIPGlobalProperties()
        {
            return new SystemIPGlobalProperties();
        }

        internal static IPGlobalProperties InternalGetIPGlobalProperties()
        {
            return new SystemIPGlobalProperties();
        }
    }
}
