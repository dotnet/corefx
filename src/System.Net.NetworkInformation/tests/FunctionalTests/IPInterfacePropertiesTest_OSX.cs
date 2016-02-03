// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Sockets;
using System.Net.Test.Common;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.NetworkInformation.Tests
{
    [PlatformSpecific(PlatformID.OSX)]
    public class IPInterfacePropertiesTest_OSX
    {
        private readonly ITestOutputHelper _log;

        public IPInterfacePropertiesTest_OSX()
        {
            _log = TestLogging.GetInstance();
        }

        [Fact]
        public void IPInfoTest_AccessAllProperties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                _log.WriteLine("Nic: " + nic.Name);
                _log.WriteLine("- Supports IPv4: " + nic.Supports(NetworkInterfaceComponent.IPv4));
                _log.WriteLine("- Supports IPv6: " + nic.Supports(NetworkInterfaceComponent.IPv6));

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                Assert.NotNull(ipProperties);

                Assert.Throws<PlatformNotSupportedException>(() => ipProperties.AnycastAddresses);

                Assert.Throws<PlatformNotSupportedException>(() => ipProperties.DhcpServerAddresses);

                try
                {
                    Assert.NotNull(ipProperties.DnsAddresses);
                    _log.WriteLine("- Dns Addresses: " + ipProperties.DnsAddresses.Count);
                    foreach (IPAddress dns in ipProperties.DnsAddresses)
                    {
                        _log.WriteLine("-- " + dns.ToString());
                    }
                    Assert.NotNull(ipProperties.DnsSuffix);
                    _log.WriteLine("- Dns Suffix: " + ipProperties.DnsSuffix);
                }
                // If /etc/resolv.conf does not exist, DNS values cannot be retrieved.
                catch (PlatformNotSupportedException) { }

                Assert.NotNull(ipProperties.GatewayAddresses);
                _log.WriteLine("- Gateway Addresses: " + ipProperties.GatewayAddresses.Count);
                foreach (GatewayIPAddressInformation gateway in ipProperties.GatewayAddresses)
                {
                    _log.WriteLine("-- " + gateway.Address.ToString());
                }

                Assert.Throws<PlatformNotSupportedException>(() => ipProperties.IsDnsEnabled);

                Assert.Throws<PlatformNotSupportedException>(() => ipProperties.IsDynamicDnsEnabled);

                Assert.NotNull(ipProperties.MulticastAddresses);
                _log.WriteLine("- Multicast Addresses: " + ipProperties.MulticastAddresses.Count);
                foreach (IPAddressInformation multi in ipProperties.MulticastAddresses)
                {
                    _log.WriteLine("-- " + multi.Address.ToString());
                    Assert.Throws<PlatformNotSupportedException>(() => multi.IsDnsEligible);
                    Assert.Throws<PlatformNotSupportedException>(() => multi.IsTransient);
                }

                Assert.NotNull(ipProperties.UnicastAddresses);
                _log.WriteLine("- Unicast Addresses: " + ipProperties.UnicastAddresses.Count);
                foreach (UnicastIPAddressInformation uni in ipProperties.UnicastAddresses)
                {
                    _log.WriteLine("-- " + uni.Address.ToString());
                    Assert.Throws<PlatformNotSupportedException>(() => uni.AddressPreferredLifetime);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.AddressValidLifetime);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.DhcpLeaseLifetime);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.DuplicateAddressDetectionState);

                    Assert.NotNull(uni.IPv4Mask);
                    _log.WriteLine("--- IPv4 Mask: " + uni.IPv4Mask);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.IsDnsEligible);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.IsTransient);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.PrefixOrigin);
                    Assert.Throws<PlatformNotSupportedException>(() => uni.SuffixOrigin);

                    // Prefix Length
                    Assert.Throws<PlatformNotSupportedException>(() => uni.PrefixLength);
                }

                Assert.Throws<PlatformNotSupportedException>(() => ipProperties.WinsServersAddresses);
            }
        }

        [Fact]
        public void IPInfoTest_AccessAllIPv4Properties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                _log.WriteLine("Nic: " + nic.Name);

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                _log.WriteLine("IPv4 Properties:");

                IPv4InterfaceProperties ipv4Properties = ipProperties.GetIPv4Properties();

                _log.WriteLine("Index: " + ipv4Properties.Index);
                Assert.Throws<PlatformNotSupportedException>(() => ipv4Properties.IsAutomaticPrivateAddressingActive);
                Assert.Throws<PlatformNotSupportedException>(() => ipv4Properties.IsAutomaticPrivateAddressingEnabled);
                Assert.Throws<PlatformNotSupportedException>(() => ipv4Properties.IsDhcpEnabled);
                Assert.Throws<PlatformNotSupportedException>(() => ipv4Properties.IsForwardingEnabled);
                _log.WriteLine("Mtu: " + ipv4Properties.Mtu);
                Assert.Throws<PlatformNotSupportedException>(() => ipv4Properties.UsesWins);
            }
        }

        [Fact]
        public void IPInfoTest_AccessAllIPv6Properties_NoErrors()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                _log.WriteLine("Nic: " + nic.Name);

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                _log.WriteLine("IPv6 Properties:");

                IPv6InterfaceProperties ipv6Properties = ipProperties.GetIPv6Properties();

                if (ipv6Properties == null)
                {
                    _log.WriteLine("IPv6Properties is null");
                    continue;
                }

                _log.WriteLine("Index: " + ipv6Properties.Index);
                _log.WriteLine("Mtu: " + ipv6Properties.Mtu);
                Assert.Throws<PlatformNotSupportedException>(() => ipv6Properties.GetScopeId(ScopeLevel.Link));
            }
        }

        [Fact]
        [Trait("IPv6", "true")]
        public void IPv6ScopeId_AccessAllValues_Success()
        {
            Assert.True(Capability.IPv6Support());

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                _log.WriteLine("Nic: " + nic.Name);

                if (!nic.Supports(NetworkInterfaceComponent.IPv6))
                {
                    continue;
                }

                IPInterfaceProperties ipProperties = nic.GetIPProperties();

                IPv6InterfaceProperties ipv6Properties = ipProperties.GetIPv6Properties();

                Array values = Enum.GetValues(typeof(ScopeLevel));
                foreach (ScopeLevel level in values)
                {
                    Assert.Throws<PlatformNotSupportedException>(() => ipv6Properties.GetScopeId(level));
                }
            }
        }
    }
}
