// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxIPGlobalProperties : IPGlobalProperties
    {
        public override string DhcpScopeName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string DomainName
        {
            get
            {
                return HostInformation.DomainName;
            }
        }

        public override string HostName
        {
            get
            {
                return HostInformation.HostName;
            }
        }

        public override bool IsWinsProxy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override NetBiosNodeType NodeType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override TcpConnectionInformation[] GetActiveTcpConnections()
        {
            throw new NotImplementedException();
        }

        public override IPEndPoint[] GetActiveTcpListeners()
        {
            throw new NotImplementedException();
        }

        public override IPEndPoint[] GetActiveUdpListeners()
        {
            throw new NotImplementedException();
        }

        public override IcmpV4Statistics GetIcmpV4Statistics()
        {
            return new OsxIcmpV4Statistics();
        }

        public override IcmpV6Statistics GetIcmpV6Statistics()
        {
            return new OsxIcmpV6Statistics();
        }

        public override IPGlobalStatistics GetIPv4GlobalStatistics()
        {
            return new OsxIPv4GlobalStatistics();
        }

        public override IPGlobalStatistics GetIPv6GlobalStatistics()
        {
            throw new NotImplementedException();
            //return new OsxIPv6GlobalStatistics();
        }

        public override TcpStatistics GetTcpIPv4Statistics()
        {
            // OSX does not provide separated TCP-IPv4 and TCP-IPv6 stats.
            return new OsxTcpStatistics();
        }

        public override TcpStatistics GetTcpIPv6Statistics()
        {
            // OSX does not provide separated TCP-IPv4 and TCP-IPv6 stats.
            return new OsxTcpStatistics();
        }

        public override UdpStatistics GetUdpIPv4Statistics()
        {
        	// OSX does not provide separated UDP-IPv4 and UDP-IPv6 stats.
            return new OsxUdpStatistics();
        }

        public override UdpStatistics GetUdpIPv6Statistics()
        {
        	// OSX does not provide separated UDP-IPv4 and UDP-IPv6 stats.
            return new OsxUdpStatistics();
        }
    }
}
