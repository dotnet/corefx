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
        MibIfRow2 ifRow;

        internal SystemIPInterfaceStatistics(long index)
        {
            ifRow = GetIfEntry2(index);
        }

        public override long OutputQueueLength { get { return (long)ifRow.outQLen; } }
        public override long BytesSent { get { return (long)ifRow.outOctets; } }
        public override long BytesReceived { get { return (long)ifRow.inOctets; } }
        public override long UnicastPacketsSent { get { return (long)ifRow.outUcastPkts; } }
        public override long UnicastPacketsReceived { get { return (long)ifRow.inUcastPkts; } }
        public override long NonUnicastPacketsSent { get { return (long)ifRow.outNUcastPkts; } }
        public override long NonUnicastPacketsReceived { get { return (long)ifRow.inNUcastPkts; } }
        public override long IncomingPacketsDiscarded { get { return (long)ifRow.inDiscards; } }
        public override long OutgoingPacketsDiscarded { get { return (long)ifRow.outDiscards; } }
        public override long IncomingPacketsWithErrors { get { return (long)ifRow.inErrors; } }
        public override long OutgoingPacketsWithErrors { get { return (long)ifRow.outErrors; } }
        public override long IncomingUnknownProtocolPackets { get { return (long)ifRow.inUnknownProtos; } }

        internal static MibIfRow2 GetIfEntry2(long index)
        {
            MibIfRow2 ifRow = new MibIfRow2();
            if (index == 0)
            {
                return ifRow;
            }
            ifRow.interfaceIndex = (uint)index;
            uint result = UnsafeNetInfoNativeMethods.GetIfEntry2(ref ifRow);
            if (result != IpHelperErrors.Success)
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
        MibIfRow2 ifRow;

        internal SystemIPv4InterfaceStatistics(long index)
        {
            ifRow = SystemIPInterfaceStatistics.GetIfEntry2(index);
        }

        public override long OutputQueueLength { get { return (long)ifRow.outQLen; } }
        public override long BytesSent { get { return (long)ifRow.outOctets; } }
        public override long BytesReceived { get { return (long)ifRow.inOctets; } }
        public override long UnicastPacketsSent { get { return (long)ifRow.outUcastPkts; } }
        public override long UnicastPacketsReceived { get { return (long)ifRow.inUcastPkts; } }
        public override long NonUnicastPacketsSent { get { return (long)ifRow.outNUcastPkts; } }
        public override long NonUnicastPacketsReceived { get { return (long)ifRow.inNUcastPkts; } }
        public override long IncomingPacketsDiscarded { get { return (long)ifRow.inDiscards; } }
        public override long OutgoingPacketsDiscarded { get { return (long)ifRow.outDiscards; } }
        public override long IncomingPacketsWithErrors { get { return (long)ifRow.inErrors; } }
        public override long OutgoingPacketsWithErrors { get { return (long)ifRow.outErrors; } }
        public override long IncomingUnknownProtocolPackets { get { return (long)ifRow.inUnknownProtos; } }
    }
}


