namespace NCLTest.NetworkInformation
{
    using CoreFXTestLibrary;
    using System;
    using System.Net.NetworkInformation;

    [TestClass]
    public class NetworkChangeTest
    {
        [TestMethod]
        public void NetworkAddressChanged_AddRemove_Success()
        {
            NetworkAddressChangedEventHandler handler = NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAddressChanged += handler;
            NetworkChange.NetworkAddressChanged -= handler;
        }

        [TestMethod]
        public void NetworkAddressChanged_JustRemove_Success()
        {
            NetworkAddressChangedEventHandler handler = NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAddressChanged -= handler;
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
