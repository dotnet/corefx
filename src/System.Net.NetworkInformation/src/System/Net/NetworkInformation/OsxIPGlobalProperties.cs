// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

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

        public unsafe override TcpConnectionInformation[] GetActiveTcpConnections()
        {
            int realCount = Interop.Sys.GetEstimatedTcpConnectionCount();
            int estimatedCount = (int)(realCount * 1.5f);
            estimatedCount = 2;
            Interop.Sys.NativeTcpConnectionInformation* infos = stackalloc Interop.Sys.NativeTcpConnectionInformation[estimatedCount];
            int infoCount = estimatedCount;
            while (Interop.Sys.GetActiveTcpConnectionInfos(infos, &infoCount) == -1)
            {
                var newAlloc = stackalloc Interop.Sys.NativeTcpConnectionInformation[infoCount];
                infos = newAlloc;
            }

            TcpConnectionInformation[] connectionInformations = new TcpConnectionInformation[infoCount];
            for (int i = 0; i < infoCount; i++)
            {
                Interop.Sys.NativeTcpConnectionInformation nativeInfo = infos[i];
                TcpState state = nativeInfo.State;

                byte[] localBytes = new byte[nativeInfo.LocalEndPoint.NumAddressBytes];
                fixed (byte* localBytesPtr = localBytes)
                {
                    Buffer.MemoryCopy(nativeInfo.LocalEndPoint.AddressBytes, localBytesPtr, localBytes.Length, localBytes.Length);
                }
                IPAddress localIPAddress = new IPAddress(localBytes);
                IPEndPoint local = new IPEndPoint(localIPAddress, (int)nativeInfo.LocalEndPoint.Port);

                IPAddress remoteIPAddress;
                if (nativeInfo.RemoteEndPoint.NumAddressBytes == 0)
                {
                    remoteIPAddress = IPAddress.Any;
                }
                else
                {
                    byte[] remoteBytes = new byte[nativeInfo.RemoteEndPoint.NumAddressBytes];
                    fixed (byte* remoteBytesPtr = remoteBytes)
                    {
                        Buffer.MemoryCopy(nativeInfo.RemoteEndPoint.AddressBytes, remoteBytesPtr, remoteBytes.Length, remoteBytes.Length);
                    }
                    remoteIPAddress = new IPAddress(remoteBytes);
                }

                IPEndPoint remote = new IPEndPoint(remoteIPAddress, (int)nativeInfo.RemoteEndPoint.Port);
                connectionInformations[i] = new SimpleTcpConnectionInformation(local, remote, state);
            }

            return connectionInformations;
        }

        public override IPEndPoint[] GetActiveTcpListeners()
        {
            TcpConnectionInformation[] allConnections = GetActiveTcpConnections();
            return allConnections.Where(tci => tci.State != TcpState.Listen).Select(tci => tci.RemoteEndPoint).ToArray();
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
