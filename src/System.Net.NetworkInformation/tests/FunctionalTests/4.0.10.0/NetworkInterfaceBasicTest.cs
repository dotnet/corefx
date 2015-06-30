namespace NCLTest.NetworkInformation
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using NCLTest.Common;

    [TestClass]
    public class NetworkInterfaceBasicTest
    {
        [TestMethod]
        public void BasicTest_GetNetworkInterfaces_AtLeastOne()
        {
            Assert.AreNotEqual<int>(0, NetworkInterface.GetAllNetworkInterfaces().Length, "No Interfaces returned");
        }

        [TestMethod]
        public void BasicTest_AccessInstanceProperties_NoExceptions()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Logger.LogInformation("- NetworkInterface -");
                Logger.LogInformation("Name: " + nic.Name);
                Logger.LogInformation("Description: " + nic.Description);
                Logger.LogInformation("ID: " + nic.Id);
                Logger.LogInformation("IsReceiveOnly: " + nic.IsReceiveOnly);
                Logger.LogInformation("Type: " + nic.NetworkInterfaceType);
                Logger.LogInformation("Status: " + nic.OperationalStatus);
                Logger.LogInformation("Speed: " + nic.Speed);
                Assert.IsTrue(nic.Speed >= 0, "Overflow");
                Logger.LogInformation("SupportsMulticast: " + nic.SupportsMulticast);
                Logger.LogInformation("GetPhysicalAddress(): " + nic.GetPhysicalAddress());
            }
        }

        [TestMethod]
        public void BasicTest_StaticLoopbackIndex_MatchesLoopbackNetworkInterface()
        {
            TestRequirements.CheckIPv4Support();

            Logger.LogInformation("Loopback IPv4 index: " + NetworkInterface.LoopbackInterfaceIndex);
            
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.Equals(IPAddress.Loopback))
                    {
                        Assert.AreEqual<int>(nic.GetIPProperties().GetIPv4Properties().Index, 
                            NetworkInterface.LoopbackInterfaceIndex, "Loopback Index mismatch");
                        return; // Only check IPv4 loopback
                    }
                }
            }
        }

        [TestMethod]
        public void BasicTest_StaticLoopbackIndex_ExceptionIfV4NotSupported()
        {
            TestRequirements.CheckIPv4Support();
            Logger.LogInformation("Loopback IPv4 index: " + NetworkInterface.LoopbackInterfaceIndex);
        }

        [TestMethod]
        public void BasicTest_StaticIPv6LoopbackIndex_MatchesLoopbackNetworkInterface()
        {
            TestRequirements.CheckIPv6Support();

            Logger.LogInformation("Loopback IPv6 index: " + NetworkInterface.IPv6LoopbackInterfaceIndex);

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.Equals(IPAddress.IPv6Loopback))
                    {
                        Assert.AreEqual<int>(nic.GetIPProperties().GetIPv6Properties().Index, 
                            NetworkInterface.IPv6LoopbackInterfaceIndex, "Loopback Index mismatch");
                        return; // Only check IPv6 loopback
                    }
                }
            }
        }

        [TestMethod]
        public void BasicTest_StaticIPv6LoopbackIndex_ExceptionIfV6NotSupported()
        {
            TestRequirements.CheckIPv6Support();
            Logger.LogInformation("Loopback IPv6 index: " + NetworkInterface.IPv6LoopbackInterfaceIndex);
        }

        [TestMethod]
        public void BasicTest_GetIPv4InterfaceStatistics_Success()
        {
            // This API is not actually IPv4 specific.
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPv4InterfaceStatistics stats = nic.GetIPv4Statistics();

                Logger.LogInformation("- Stats for : " + nic.Name);
                Logger.LogInformation("BytesReceived: " + stats.BytesReceived);
                Logger.LogInformation("BytesSent: " + stats.BytesSent);
                Logger.LogInformation("IncomingPacketsDiscarded: " + stats.IncomingPacketsDiscarded);
                Logger.LogInformation("IncomingPacketsWithErrors: " + stats.IncomingPacketsWithErrors);
                Logger.LogInformation("IncomingUnknownProtocolPackets: " + stats.IncomingUnknownProtocolPackets);
                Logger.LogInformation("NonUnicastPacketsReceived: " + stats.NonUnicastPacketsReceived);
                Logger.LogInformation("NonUnicastPacketsSent: " + stats.NonUnicastPacketsSent);
                Logger.LogInformation("OutgoingPacketsDiscarded: " + stats.OutgoingPacketsDiscarded);
                Logger.LogInformation("OutgoingPacketsWithErrors: " + stats.OutgoingPacketsWithErrors);
                Logger.LogInformation("OutputQueueLength: " + stats.OutputQueueLength);
                Logger.LogInformation("UnicastPacketsReceived: " + stats.UnicastPacketsReceived);
                Logger.LogInformation("UnicastPacketsSent: " + stats.UnicastPacketsSent);
            }
        }
        
        [TestMethod]
        public void BasicTest_CompareGetIPv4InterfaceStatisticsWithGetIPInterfaceStatictics_Success()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // These APIs call the same native API that returns more than just IPv4 results
                // so we added GetIPStatistics() as a more accurate name.
                IPv4InterfaceStatistics v4stats = nic.GetIPv4Statistics();
                IPInterfaceStatistics stats = nic.GetIPStatistics();

                // Verify that the by the second call the stats should have increased, or at least not decreased.
                Assert.IsTrue(v4stats.BytesReceived <= stats.BytesReceived, "BytesReceived did not increase");
                Assert.IsTrue(v4stats.BytesSent <= stats.BytesSent, "BytesSent decreased");
                Assert.IsTrue(v4stats.IncomingPacketsDiscarded <= stats.IncomingPacketsDiscarded,
                    "IncomingPacketsDiscarded decreased");
                Assert.IsTrue(v4stats.IncomingPacketsWithErrors <= stats.IncomingPacketsWithErrors,
                    "IncomingPacketsWithErrors decreased");
                Assert.IsTrue(v4stats.IncomingUnknownProtocolPackets <= stats.IncomingUnknownProtocolPackets,
                    "IncomingUnknownProtocolPackets decreased");
                Assert.IsTrue(v4stats.NonUnicastPacketsReceived <= stats.NonUnicastPacketsReceived,
                    "NonUnicastPacketsReceived decreased");
                Assert.IsTrue(v4stats.NonUnicastPacketsSent <= stats.NonUnicastPacketsSent,
                    "NonUnicastPacketsSent decreased");
                Assert.IsTrue(v4stats.OutgoingPacketsDiscarded <= stats.OutgoingPacketsDiscarded,
                    "OutgoingPacketsDiscarded decreased");
                Assert.IsTrue(v4stats.OutgoingPacketsWithErrors <= stats.OutgoingPacketsWithErrors,
                    "OutgoingPacketsWithErrors decreased");
                Assert.IsTrue(v4stats.OutputQueueLength <= stats.OutputQueueLength,
                    "OutputQueueLength decreased");
                Assert.IsTrue(v4stats.UnicastPacketsReceived <= stats.UnicastPacketsReceived,
                    "UnicastPacketsReceived decreased");
                Assert.IsTrue(v4stats.UnicastPacketsSent <= stats.UnicastPacketsSent,
                    "UnicastPacketsSent decreased");
            }
        }

        [TestMethod]
        public void BasicTest_GetIsNetworkAvailable_Success()
        {
            Assert.IsTrue(NetworkInterface.GetIsNetworkAvailable());
        }
    }
}
