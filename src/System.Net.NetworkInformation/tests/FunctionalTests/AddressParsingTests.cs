// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Xunit;

namespace System.Net.NetworkInformation.Tests
{
    public class AddressParsingTests : FileCleanupTestBase
    {
        [Fact]
        public void HexIPAddressParsing()
        {
            Assert.Equal(IPAddress.Parse("10.105.128.1"), StringParsingHelpers.ParseHexIPAddress("0180690A"));
            Assert.Equal(IPAddress.Parse("103.69.35.1"), StringParsingHelpers.ParseHexIPAddress("01234567"));
            Assert.Equal(IPAddress.Parse("152.186.220.254"), StringParsingHelpers.ParseHexIPAddress("FEDCBA98"));

            Assert.Equal(IPAddress.Parse("::"), StringParsingHelpers.ParseHexIPAddress("00000000000000000000000000000000"));
            Assert.Equal(IPAddress.Parse("::1"), StringParsingHelpers.ParseHexIPAddress("00000000000000000000000001000000"));
            Assert.Equal(IPAddress.Parse("fec0::1"), StringParsingHelpers.ParseHexIPAddress("0000C0FE000000000000000001000000"));
            Assert.Equal(IPAddress.Parse("fe80::222:222"), StringParsingHelpers.ParseHexIPAddress("000080FE000000000000000022022202"));
            Assert.Equal(IPAddress.Parse("fe80::215:5dff:fe00:402"), StringParsingHelpers.ParseHexIPAddress("000080FE00000000FF5D1502020400FE"));
        }

        [Fact]
        public void GatewayAddressParsing()
        {
            string fileName = GetTestFilePath();
            FileUtil.NormalizeLineEndings("NetworkFiles/route", fileName);
            List<GatewayIPAddressInformation> gatewayAddresses = StringParsingHelpers.ParseGatewayAddressesFromRouteFile(fileName, "wlan0");
            Assert.Equal(3, gatewayAddresses.Count);

            Assert.Equal(IPAddress.Parse("10.105.128.1"), gatewayAddresses[0].Address);
            Assert.Equal(IPAddress.Parse("103.69.35.1"), gatewayAddresses[1].Address);
            Assert.Equal(IPAddress.Parse("152.186.220.254"), gatewayAddresses[2].Address);
        }

        [Fact]
        public void DhcpServerAddressParsing()
        {
            string fileName = GetTestFilePath();
            FileUtil.NormalizeLineEndings("NetworkFiles/dhclient.leases", fileName);
            List<IPAddress> dhcpServerAddresses = StringParsingHelpers.ParseDhcpServerAddressesFromLeasesFile(fileName, "wlan0");
            Assert.Equal(1, dhcpServerAddresses.Count);
            Assert.Equal(IPAddress.Parse("10.105.128.4"), dhcpServerAddresses[0]);
        }

        [Fact]
        public void WinsServerAddressParsing()
        {
            string fileName = GetTestFilePath();
            FileUtil.NormalizeLineEndings("NetworkFiles/smb.conf", fileName);

            List<IPAddress> winsServerAddresses = StringParsingHelpers.ParseWinsServerAddressesFromSmbConfFile(fileName);
            Assert.Equal(1, winsServerAddresses.Count);
            Assert.Equal(IPAddress.Parse("255.1.255.1"), winsServerAddresses[0]);
        }
    }
}
