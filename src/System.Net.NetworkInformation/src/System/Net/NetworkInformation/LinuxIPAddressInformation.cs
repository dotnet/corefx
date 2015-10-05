namespace System.Net.NetworkInformation
{
    public class LinuxIPAddressInformation : IPAddressInformation
    {
        private IPAddress _address;

        public LinuxIPAddressInformation(IPAddress address)
        {
            _address = address;
        }

        /// Gets the Internet Protocol (IP) address.
        public override IPAddress Address { get { return _address; } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is legal to appear in a Domain Name System (DNS) server database.
        public override bool IsDnsEligible { get { throw new PlatformNotSupportedException(); } }

        /// Gets a bool value that indicates whether the Internet Protocol (IP) address is transient.
        public override bool IsTransient { get { throw new PlatformNotSupportedException(); } }
    }
}