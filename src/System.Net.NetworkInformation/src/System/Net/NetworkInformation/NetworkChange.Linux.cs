namespace System.Net.NetworkInformation
{
    // Linux implementation of NetworkChange
    public class NetworkChange
    {
        public delegate void NetworkAddressChangedEventHandler(object sender, EventArgs e);

        public delegate void NetworkAvailabilityChangedEventHandler(object sender, NetworkAvailabilityEventArgs e);

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
