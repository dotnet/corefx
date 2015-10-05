namespace System.Net.NetworkInformation
{
    // Linux implementation of NetworkChange
    public class NetworkChange
    {
        static public event NetworkAddressChangedEventHandler NetworkAddressChanged
        {
            add
            {
                throw new PlatformNotSupportedException();
            }
            remove
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
