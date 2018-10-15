// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Sockets;
using System.Net.Test.Common;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.NetworkInformation.Tests
{
    public class IPGlobalPropertiesTest
    {
        private readonly ITestOutputHelper _log;
        public static readonly object[][] Loopbacks = new[]
        {
            new object[] { IPAddress.Loopback },
            new object[] { IPAddress.IPv6Loopback },
        };

        public IPGlobalPropertiesTest()
        {
            _log = TestLogging.GetInstance();
        }

        [Fact]
        public void IPGlobalProperties_AccessAllMethods_NoErrors()
        {
            IPGlobalProperties gp = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] activeConnections = gp.GetActiveTcpConnections();
            IPEndPoint[] tcpListeners = gp.GetActiveTcpListeners();
            IPEndPoint[] udpListeners = gp.GetActiveUdpListeners();

            IPGlobalStatistics v4IpStat = gp.GetIPv4GlobalStatistics();
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // OSX does not provide IPv6  stats.
                IPGlobalStatistics v6IpStat = gp.GetIPv6GlobalStatistics();
            }

            IcmpV4Statistics v4IcmpStat = gp.GetIcmpV4Statistics();
            IcmpV6Statistics v6IcmpStat = gp.GetIcmpV6Statistics();
            TcpStatistics v4TcpSTat = gp.GetTcpIPv4Statistics();
            TcpStatistics v6TcpStat = gp.GetTcpIPv6Statistics();
            UdpStatistics v4UdpStat = gp.GetUdpIPv4Statistics();
            UdpStatistics v6UdpStat = gp.GetUdpIPv6Statistics();
        }

        [Theory]
        [MemberData(nameof(Loopbacks))]
        public void IPGlobalProperties_TcpListeners_Succeed(IPAddress address)
        {
            using (var server = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(new IPEndPoint(address, 0));
                server.Listen(1);
                _log.WriteLine("listening on {0}", server.LocalEndPoint);

                IPEndPoint[] tcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                bool found = false;
                foreach (IPEndPoint ep in tcpListeners)
                {
                    if (ep.Equals(server.LocalEndPoint))
                    {
                        found = true;
                        break;
                    }
                }

                Assert.True(found);
            }
        }

        [Theory]
        [MemberData(nameof(Loopbacks))]
        public async Task IPGlobalProperties_TcpActiveConnections_Succeed(IPAddress address)
        {
            using (var server = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(new IPEndPoint(address, 0));
                server.Listen(1);
                _log.WriteLine("listening on {0}", server.LocalEndPoint);

                await client.ConnectAsync(server.LocalEndPoint);

                TcpConnectionInformation[] tcpCconnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                bool found = false;
                foreach (TcpConnectionInformation ti in tcpCconnections)
                {
                    if (ti.LocalEndPoint.Equals(client.LocalEndPoint) && ti.RemoteEndPoint.Equals(client.RemoteEndPoint) &&
                         (ti.State == TcpState.Established))
                    {
                        found = true;
                        break;
                    }
                }

                Assert.True(found);
            }
        }

        [Fact]
        public void IPGlobalProperties_TcpActiveConnections_NotListening()
        {
            TcpConnectionInformation[] tcpCconnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            foreach (TcpConnectionInformation ti in tcpCconnections)
            {
                Assert.NotEqual(TcpState.Listen, ti.State);
            }
        }
    }
}
