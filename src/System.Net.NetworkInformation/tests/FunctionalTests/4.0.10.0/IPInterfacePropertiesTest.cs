namespace NCLTest.NetworkInformation
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using NCLTest.Common;

    [TestClass]
    public class IPInterfacePropertiesTest
    {
        [TestMethod]
        public void IPInfoTest_AccessAllProperties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Logger.LogInformation("Nic: " + nic.Name);
                Logger.LogInformation("- Supports IPv4: " + nic.Supports(NetworkInterfaceComponent.IPv4));
                Logger.LogInformation("- Supports IPv6: " + nic.Supports(NetworkInterfaceComponent.IPv6));

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                Assert.IsNotNull(ipProperties, "IpProperties");

                Assert.IsNotNull(ipProperties.AnycastAddresses, "AnycastAddresses");
                Logger.LogInformation("- Anycast Addresses: " + ipProperties.AnycastAddresses.Count);
                foreach (IPAddressInformation anyAddr in ipProperties.AnycastAddresses)
                {
                    Logger.LogInformation("-- " + anyAddr.Address.ToString());
                    Logger.LogInformation("--- Dns Eligible: " + anyAddr.IsDnsEligible);
                    Logger.LogInformation("--- Transient: " + anyAddr.IsTransient);
                }

                Assert.IsNotNull(ipProperties.DhcpServerAddresses, "DhcpServerAddresses");
                Logger.LogInformation("- Dhcp Server Addresses: " + ipProperties.DhcpServerAddresses.Count);
                foreach (IPAddress dhcp in ipProperties.DhcpServerAddresses)
                {
                    Logger.LogInformation("-- " + dhcp.ToString());
                }

                Assert.IsNotNull(ipProperties.DnsAddresses, "DnsAddresses");
                Logger.LogInformation("- Dns Addresses: " + ipProperties.DnsAddresses.Count);
                foreach (IPAddress dns in ipProperties.DnsAddresses)
                {
                    Logger.LogInformation("-- " + dns.ToString());
                }

                Assert.IsNotNull(ipProperties.DnsSuffix, "DnsSuffix");
                Logger.LogInformation("- Dns Suffix: " + ipProperties.DnsSuffix);

                Assert.IsNotNull(ipProperties.GatewayAddresses, "GatewayAddresses");
                Logger.LogInformation("- Gateway Addresses: " + ipProperties.GatewayAddresses.Count);
                foreach (GatewayIPAddressInformation gateway in ipProperties.GatewayAddresses)
                {
                    Logger.LogInformation("-- " + gateway.Address.ToString());
                }

                Logger.LogInformation("- Dns Enabled: " + ipProperties.IsDnsEnabled);
                
                Logger.LogInformation("- Dynamic Dns Enabled: " + ipProperties.IsDynamicDnsEnabled);

                Assert.IsNotNull(ipProperties.MulticastAddresses, "MulticastAddresses");
                Logger.LogInformation("- Multicast Addresses: " + ipProperties.MulticastAddresses.Count);
                foreach (IPAddressInformation multi in ipProperties.MulticastAddresses)
                {
                    Logger.LogInformation("-- " + multi.Address.ToString());
                    Logger.LogInformation("--- Dns Eligible: " + multi.IsDnsEligible);
                    Logger.LogInformation("--- Transient: " + multi.IsTransient);
                }

                Assert.IsNotNull(ipProperties.UnicastAddresses, "UnicastAddresses");
                Logger.LogInformation("- Unicast Addresses: " + ipProperties.UnicastAddresses.Count);
                foreach (UnicastIPAddressInformation uni in ipProperties.UnicastAddresses)
                {
                    Logger.LogInformation("-- " + uni.Address.ToString());
                    Logger.LogInformation("--- Preferred Lifetime: " + uni.AddressPreferredLifetime);
                    Logger.LogInformation("--- Valid Lifetime: " + uni.AddressValidLifetime);
                    Logger.LogInformation("--- Dhcp lease Lifetime: " + uni.DhcpLeaseLifetime);
                    Logger.LogInformation("--- Duplicate Address Detection State: " + uni.DuplicateAddressDetectionState);

                    Assert.IsNotNull(uni.IPv4Mask, "IPv4Mask");
                    Logger.LogInformation("--- IPv4 Mask: " + uni.IPv4Mask);
                    Logger.LogInformation("--- Dns Eligible: " + uni.IsDnsEligible);
                    Logger.LogInformation("--- Transient: " + uni.IsTransient);
                    Logger.LogInformation("--- Prefix Origin: " + uni.PrefixOrigin);
                    Logger.LogInformation("--- Suffix Origin: " + uni.SuffixOrigin);

                    // Prefix Length
                    
                    Logger.LogInformation("--- Prefix Length: " + uni.PrefixLength);
                    Assert.AreNotEqual(0, uni.PrefixLength, "PrefixLength");
                }

                Assert.IsNotNull(ipProperties.WinsServersAddresses, "WinsServersAddresses");
                Logger.LogInformation("- Wins Addresses: " + ipProperties.WinsServersAddresses.Count);
                foreach (IPAddress wins in ipProperties.WinsServersAddresses)
                {
                    Logger.LogInformation("-- " + wins.ToString());
                }
            }
        }

        [TestMethod]
        public void IPInfoTest_AccessAllIPv4Properties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Logger.LogInformation("Nic: " + nic.Name);

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                Logger.LogInformation("IPv4 Properties:");

                if (!nic.Supports(NetworkInterfaceComponent.IPv4))
                {
                    try
                    {
                        IPv4InterfaceProperties throwAway = ipProperties.GetIPv4Properties();
                        Assert.Fail("Should have thrown ProtocolNotSupported");
                    }
                    catch (NetworkInformationException nie)
                    {
                        Assert.AreEqual(SocketError.ProtocolNotSupported, (SocketError)nie.ErrorCode);
                        continue;
                    }
                }

                IPv4InterfaceProperties ipv4Properties = ipProperties.GetIPv4Properties();

                Logger.LogInformation("Index: " + ipv4Properties.Index);
                Logger.LogInformation("IsAutomaticPrivateAddressingActive: " + ipv4Properties.IsAutomaticPrivateAddressingActive);
                Logger.LogInformation("IsAutomaticPrivateAddressingEnabled: " + ipv4Properties.IsAutomaticPrivateAddressingEnabled);
                Logger.LogInformation("IsDhcpEnabled: " + ipv4Properties.IsDhcpEnabled);
                Logger.LogInformation("IsForwardingEnabled: " + ipv4Properties.IsForwardingEnabled);
                Logger.LogInformation("Mtu: " + ipv4Properties.Mtu);
                Logger.LogInformation("UsesWins: " + ipv4Properties.UsesWins);
            }
        }

        [TestMethod]
        public void IPInfoTest_AccessAllIPv6Properties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Logger.LogInformation("Nic: " + nic.Name);

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                Logger.LogInformation("IPv6 Properties:");

                if (!nic.Supports(NetworkInterfaceComponent.IPv6))
                {
                    try
                    {
                        IPv6InterfaceProperties throwAway = ipProperties.GetIPv6Properties();
                        Assert.Fail("Should have thrown ProtocolNotSupported");
                    }
                    catch (NetworkInformationException nie)
                    {
                        Assert.AreEqual(SocketError.ProtocolNotSupported, (SocketError)nie.ErrorCode);
                        continue;
                    }
                }

                IPv6InterfaceProperties ipv6Properties = ipProperties.GetIPv6Properties();
                
                if (ipv6Properties == null)
                {
                    Logger.LogInformation("IPv6Properties is null");
                    continue;
                }

                Logger.LogInformation("Index: " + ipv6Properties.Index);
                Logger.LogInformation("Mtu: " + ipv6Properties.Mtu);
                Logger.LogInformation("ScopeID: " + ipv6Properties.GetScopeId(ScopeLevel.Link));
            }
        }
        
        [TestMethod]
        public void IPv6ScopeId_GetLinkLevel_MatchesIndex()
        {
            TestRequirements.CheckIPv6Support();

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                if (!nic.Supports(NetworkInterfaceComponent.IPv6))
                {
                    continue;
                }

                IPv6InterfaceProperties ipv6Properties = ipProperties.GetIPv6Properties();
                // This is not officially guaranteed by windows, but it's what gets used.
                Assert.AreEqual(ipv6Properties.Index, ipv6Properties.GetScopeId(ScopeLevel.Link), "ScopeID");
            }
        }
        [TestMethod]
        public void IPv6ScopeId_AccessAllValues_Success()
        {
            TestRequirements.CheckIPv6Support();

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Logger.LogInformation("Nic: " + nic.Name);

                if (!nic.Supports(NetworkInterfaceComponent.IPv6))
                {
                    continue;
                }

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                Logger.LogInformation("- IPv6 Scope levels:");

                IPv6InterfaceProperties ipv6Properties = ipProperties.GetIPv6Properties();

                Array values = Enum.GetValues(typeof(ScopeLevel));
                foreach (ScopeLevel level in values)
                {
                    Logger.LogInformation("-- Level: " + level + "; " + ipv6Properties.GetScopeId(level));
                }
            }
        }
    }
}
