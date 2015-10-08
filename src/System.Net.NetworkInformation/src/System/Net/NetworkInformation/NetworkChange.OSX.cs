namespace System.Net.NetworkInformation
{
    public delegate void NetworkAddressChangedEventHandler(object sender, EventArgs e);

    public delegate void NetworkAvailabilityChangedEventHandler(object sender, NetworkAvailabilityEventArgs e);

    // Linux implementation of NetworkChange
    public class NetworkChange
    {
        static public event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged
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
