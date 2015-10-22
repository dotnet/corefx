﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal static class NetworkFiles
    {
        // Paths to specific directories and files
        public const string InterfaceListingFile = "/proc/net/dev";
        public const string SnmpV4StatsFile = "/proc/net/snmp";
        public const string SnmpV6StatsFile = "/proc/net/snmp6";
        public const string Ipv4ConfigFolder = "/proc/sys/net/ipv4/conf";
        public const string Ipv6ConfigFolder = "/proc/sys/net/ipv6/conf";
        public const string Ipv4RouteFile = "/proc/net/route";
        public const string Ipv6RouteFile = "/proc/net/ipv6_route";
        public const string SockstatFile = "/proc/net/sockstat";
        public const string Sockstat6File = "/proc/net/sockstat6";
        public const string SysClassNetFolder = "/sys/class/net";
        public const string EtcResolvConfFile = "/etc/resolv.conf";
        public const string Tcp4ConnectionsFile = "/proc/net/tcp";
        public const string Tcp6ConnectionsFile = "/proc/net/tcp6";
        public const string Udp4ConnectionsFile = "/proc/net/udp";
        public const string Udp6ConnectionsFile = "/proc/net/udp6";
        public const string DHClientLeasesFile = "/var/lib/dhcp/dhclient.leases";
        public const string SmbConfFile = "/etc/smb/smb.conf";

        // Individual file names
        public const string AllNetworkInterfaceFileName = "all";
        public const string DefaultNetworkInterfaceFileName = "default";
        public const string FlagsFileName = "flags";
        public const string ForwardingFileName = "forwarding";
        public const string MtuFileName = "mtu";
        public const string OperstateFileName = "operstate";
        public const string SpeedFileName = "speed";
        public const string TransmitQueueLengthFileName = "tx_queue_len";
    }
}
