﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Test.Common;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.NetworkInformation.Tests
{
    public class NetworkInterfaceBasicTest
    {
        private readonly ITestOutputHelper _log;

        public NetworkInterfaceBasicTest()
        {
            _log = TestLogging.GetInstance();
        }

        [Fact]
        public void BasicTest_GetNetworkInterfaces_AtLeastOne()
        {
            Assert.NotEqual<int>(0, NetworkInterface.GetAllNetworkInterfaces().Length);
        }

        [Fact]
        public void BasicTest_AccessInstanceProperties_NoExceptions()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                _log.WriteLine("- NetworkInterface -");
                _log.WriteLine("Name: " + nic.Name);
                _log.WriteLine("Description: " + nic.Description);
                _log.WriteLine("ID: " + nic.Id);
                _log.WriteLine("IsReceiveOnly: " + nic.IsReceiveOnly);
                _log.WriteLine("Type: " + nic.NetworkInterfaceType);
                _log.WriteLine("Status: " + nic.OperationalStatus);
                _log.WriteLine("Speed: " + nic.Speed);
                Assert.True(nic.Speed >= 0, "Overflow");
                _log.WriteLine("SupportsMulticast: " + nic.SupportsMulticast);
                _log.WriteLine("GetPhysicalAddress(): " + nic.GetPhysicalAddress());
            }
        }

        [Fact]
        [Trait("IPv4", "true")]
        public void BasicTest_StaticLoopbackIndex_MatchesLoopbackNetworkInterface()
        {
            Assert.True(Capability.IPv4Support());

            _log.WriteLine("Loopback IPv4 index: " + NetworkInterface.LoopbackInterfaceIndex);

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.Equals(IPAddress.Loopback))
                    {
                        Assert.Equal<int>(nic.GetIPProperties().GetIPv4Properties().Index,
                            NetworkInterface.LoopbackInterfaceIndex);
                        return; // Only check IPv4 loopback
                    }
                }
            }
        }

        [Fact]
        [Trait("IPv4", "true")]
        public void BasicTest_StaticLoopbackIndex_ExceptionIfV4NotSupported()
        {
            Assert.True(Capability.IPv4Support());

            _log.WriteLine("Loopback IPv4 index: " + NetworkInterface.LoopbackInterfaceIndex);
        }

        [Fact]
        [Trait("IPv6", "true")]
        public void BasicTest_StaticIPv6LoopbackIndex_MatchesLoopbackNetworkInterface()
        {
            Assert.True(Capability.IPv6Support());

            _log.WriteLine("Loopback IPv6 index: " + NetworkInterface.IPv6LoopbackInterfaceIndex);

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.Equals(IPAddress.IPv6Loopback))
                    {
                        Assert.Equal<int>(
                            nic.GetIPProperties().GetIPv6Properties().Index,
                            NetworkInterface.IPv6LoopbackInterfaceIndex);

                        return; // Only check IPv6 loopback.
                    }
                }
            }
        }

        [Fact]
        [Trait("IPv6", "true")]
        public void BasicTest_StaticIPv6LoopbackIndex_ExceptionIfV6NotSupported()
        {
            Assert.True(Capability.IPv6Support());
            _log.WriteLine("Loopback IPv6 index: " + NetworkInterface.IPv6LoopbackInterfaceIndex);
        }

        [Fact]
        public void BasicTest_GetIPInterfaceStatistics_Success()
        {
            // This API is not actually IPv4 specific.
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceStatistics stats = nic.GetIPStatistics();

                _log.WriteLine("- Stats for : " + nic.Name);
                _log.WriteLine("BytesReceived: " + stats.BytesReceived);
                _log.WriteLine("BytesSent: " + stats.BytesSent);
                _log.WriteLine("IncomingPacketsDiscarded: " + stats.IncomingPacketsDiscarded);
                _log.WriteLine("IncomingPacketsWithErrors: " + stats.IncomingPacketsWithErrors);
                _log.WriteLine("IncomingUnknownProtocolPackets: " + stats.IncomingUnknownProtocolPackets);
                _log.WriteLine("NonUnicastPacketsReceived: " + stats.NonUnicastPacketsReceived);
                _log.WriteLine("NonUnicastPacketsSent: " + stats.NonUnicastPacketsSent);
                _log.WriteLine("OutgoingPacketsDiscarded: " + stats.OutgoingPacketsDiscarded);
                _log.WriteLine("OutgoingPacketsWithErrors: " + stats.OutgoingPacketsWithErrors);
                _log.WriteLine("OutputQueueLength: " + stats.OutputQueueLength);
                _log.WriteLine("UnicastPacketsReceived: " + stats.UnicastPacketsReceived);
                _log.WriteLine("UnicastPacketsSent: " + stats.UnicastPacketsSent);
            }
        }

        [Fact]
        public void BasicTest_GetIsNetworkAvailable_Success()
        {
            Assert.True(NetworkInterface.GetIsNetworkAvailable());
        }
    }
}
