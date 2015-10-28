// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
    public abstract class IPGlobalProperties
    {
        public static IPGlobalProperties GetIPGlobalProperties()
        {
            return IPGlobalPropertiesPal.GetIPGlobalProperties();
        }

        /// <summary>
        /// Gets the Active Udp Listeners on this machine.
        /// </summary>
        public abstract IPEndPoint[] GetActiveUdpListeners();

        /// <summary>
        /// Gets the Active Tcp Listeners on this machine.
        /// </summary>
        public abstract IPEndPoint[] GetActiveTcpListeners();

        /// <summary>
        /// Gets the Active Udp Listeners on this machine.
        /// </summary>
        public abstract TcpConnectionInformation[] GetActiveTcpConnections();

        /// <summary>
        /// Gets the Dynamic Host Configuration Protocol (DHCP) scope name.
        /// </summary>
        public abstract string DhcpScopeName { get; }

        /// <summary>
        /// Gets the domain in which the local computer is registered.
        /// </summary>
        public abstract string DomainName { get; }

        /// <summary>
        /// Gets the host name for the local computer.
        /// </summary>
        public abstract string HostName { get; }

        /// <summary>
        /// Gets a bool value that specifies whether the local computer is acting as a Windows Internet Name Service (WINS) proxy.
        /// </summary>
        public abstract bool IsWinsProxy { get; }

        /// <summary>
        /// Gets the Network Basic Input/Output System (NetBIOS) node type of the local computer.
        /// </summary>
        public abstract NetBiosNodeType NodeType { get; }

        public abstract TcpStatistics GetTcpIPv4Statistics();

        public abstract TcpStatistics GetTcpIPv6Statistics();

        /// <summary>
        /// Provides User Datagram Protocol (UDP) statistical data for the local computer.
        /// </summary>
        public abstract UdpStatistics GetUdpIPv4Statistics();

        public abstract UdpStatistics GetUdpIPv6Statistics();

        /// <summary>
        /// Provides Internet Control Message Protocol (ICMP) version 4 statistical data for the local computer.
        /// </summary>
        public abstract IcmpV4Statistics GetIcmpV4Statistics();

        /// <summary>
        /// Provides Internet Control Message Protocol (ICMP) version 6 statistical data for the local computer.
        /// </summary>
        public abstract IcmpV6Statistics GetIcmpV6Statistics();

        /// <summary>
        /// Provides Internet Protocol (IP) statistical data for the local computer.
        /// </summary>
        public abstract IPGlobalStatistics GetIPv4GlobalStatistics();

        public abstract IPGlobalStatistics GetIPv6GlobalStatistics();

        public virtual Task<UnicastIPAddressInformationCollection> GetUnicastAddressesAsync()
        {
            throw NotImplemented.ByDesignWithMessage(SR.net_MethodNotImplementedException);
        }
    }
}
