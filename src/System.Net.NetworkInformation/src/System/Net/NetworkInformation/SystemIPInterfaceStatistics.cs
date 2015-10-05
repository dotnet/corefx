// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>     
///


namespace System.Net.NetworkInformation
{
    internal class SystemIPInterfaceStatistics : IPInterfaceStatistics
    {
        private Interop.IpHlpApi.MibIfRow2 _ifRow;

        internal SystemIPInterfaceStatistics(long index)
        {
            _ifRow = GetIfEntry2(index);
        }

        public override long OutputQueueLength { get { return (long)_ifRow.outQLen; } }
        public override long BytesSent { get { return (long)_ifRow.outOctets; } }
        public override long BytesReceived { get { return (long)_ifRow.inOctets; } }
        public override long UnicastPacketsSent { get { return (long)_ifRow.outUcastPkts; } }
        public override long UnicastPacketsReceived { get { return (long)_ifRow.inUcastPkts; } }
        public override long NonUnicastPacketsSent { get { return (long)_ifRow.outNUcastPkts; } }
        public override long NonUnicastPacketsReceived { get { return (long)_ifRow.inNUcastPkts; } }
        public override long IncomingPacketsDiscarded { get { return (long)_ifRow.inDiscards; } }
        public override long OutgoingPacketsDiscarded { get { return (long)_ifRow.outDiscards; } }
        public override long IncomingPacketsWithErrors { get { return (long)_ifRow.inErrors; } }
        public override long OutgoingPacketsWithErrors { get { return (long)_ifRow.outErrors; } }
        public override long IncomingUnknownProtocolPackets { get { return (long)_ifRow.inUnknownProtos; } }

        internal static Interop.IpHlpApi.MibIfRow2 GetIfEntry2(long index)
        {
            Interop.IpHlpApi.MibIfRow2 ifRow = new Interop.IpHlpApi.MibIfRow2();
            if (index == 0)
            {
                return ifRow;
            }
            ifRow.interfaceIndex = (uint)index;
            uint result = Interop.IpHlpApi.GetIfEntry2(ref ifRow);
            if (result != Interop.IpHlpApi.ERROR_SUCCESS)
            {
                throw new NetworkInformationException((int)result);
            }
            return ifRow;
        }
    }

    /// Despite the naming, the results are not IPv4 specific.
    /// Do not use this class, use SystemIPInterfaceStatistics instead.
    /// <summary>IP statistics</summary>
    internal class SystemIPv4InterfaceStatistics : IPv4InterfaceStatistics
    {
        private Interop.IpHlpApi.MibIfRow2 _ifRow;

        internal SystemIPv4InterfaceStatistics(long index)
        {
            _ifRow = SystemIPInterfaceStatistics.GetIfEntry2(index);
        }

        public override long OutputQueueLength { get { return (long)_ifRow.outQLen; } }
        public override long BytesSent { get { return (long)_ifRow.outOctets; } }
        public override long BytesReceived { get { return (long)_ifRow.inOctets; } }
        public override long UnicastPacketsSent { get { return (long)_ifRow.outUcastPkts; } }
        public override long UnicastPacketsReceived { get { return (long)_ifRow.inUcastPkts; } }
        public override long NonUnicastPacketsSent { get { return (long)_ifRow.outNUcastPkts; } }
        public override long NonUnicastPacketsReceived { get { return (long)_ifRow.inNUcastPkts; } }
        public override long IncomingPacketsDiscarded { get { return (long)_ifRow.inDiscards; } }
        public override long OutgoingPacketsDiscarded { get { return (long)_ifRow.outDiscards; } }
        public override long IncomingPacketsWithErrors { get { return (long)_ifRow.inErrors; } }
        public override long OutgoingPacketsWithErrors { get { return (long)_ifRow.outErrors; } }
        public override long IncomingUnknownProtocolPackets { get { return (long)_ifRow.inUnknownProtos; } }
    }
}


