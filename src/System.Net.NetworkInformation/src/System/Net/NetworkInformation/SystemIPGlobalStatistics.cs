// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>     
///


using System.Net.Sockets;
using System;
using System.ComponentModel;

namespace System.Net.NetworkInformation
{
    /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics"]/*' />
    /// <summary>IP statistics</summary>
    internal class SystemIPGlobalStatistics : IPGlobalStatistics
    {
        MibIpStats stats = new MibIpStats();

        private SystemIPGlobalStatistics() { }
        internal SystemIPGlobalStatistics(AddressFamily family)
        {
            uint result = UnsafeNetInfoNativeMethods.GetIpStatisticsEx(out stats, family);

            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }
        }

        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ForwardingEnabled"]/*' />
        public override bool ForwardingEnabled { get { return stats.forwardingEnabled; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.DefaultTimeout"]/*' />
        public override int DefaultTtl { get { return (int)stats.defaultTtl; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPackets"]/*' />
        public override long ReceivedPackets { get { return stats.packetsReceived; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsWithHeadersErrors"]/*' />
        public override long ReceivedPacketsWithHeadersErrors { get { return stats.receivedPacketsWithHeaderErrors; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsWithAddressErrors"]/*' />
        public override long ReceivedPacketsWithAddressErrors { get { return stats.receivedPacketsWithAddressErrors; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsForwarded"]/*' />
        public override long ReceivedPacketsForwarded { get { return stats.packetsForwarded; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsWithUnknownProtocol"]/*' />
        public override long ReceivedPacketsWithUnknownProtocol { get { return stats.receivedPacketsWithUnknownProtocols; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsDiscarded"]/*' />
        public override long ReceivedPacketsDiscarded { get { return stats.receivedPacketsDiscarded; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.ReceivedPacketsDelievered"]/*' />
        public override long ReceivedPacketsDelivered { get { return stats.receivedPacketsDelivered; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.OutputPacketRequests"]/*' />
        public override long OutputPacketRequests { get { return stats.packetOutputRequests; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.OutputPacketRoutingDiscards"]/*' />
        public override long OutputPacketRoutingDiscards { get { return stats.outputPacketRoutingDiscards; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.OutputPacketsDiscarded"]/*' />
        public override long OutputPacketsDiscarded { get { return stats.outputPacketsDiscarded; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.OutputPacketsWithNoRoute"]/*' />
        public override long OutputPacketsWithNoRoute { get { return stats.outputPacketsWithNoRoute; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketReassemblyTimeout"]/*' />
        public override long PacketReassemblyTimeout { get { return stats.packetReassemblyTimeout; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketReassembliesRequired"]/*' />
        public override long PacketReassembliesRequired { get { return stats.packetsReassemblyRequired; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketsReassembled"]/*' />
        public override long PacketsReassembled { get { return stats.packetsReassembled; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketReassemblyFailures"]/*' />
        public override long PacketReassemblyFailures { get { return stats.packetsReassemblyFailed; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketsFragmented"]/*' />
        public override long PacketsFragmented { get { return stats.packetsFragmented; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.PacketFragmentFailures"]/*' />
        public override long PacketFragmentFailures { get { return stats.packetsFragmentFailed; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.Interfaces"]/*' />
        public override int NumberOfInterfaces { get { return (int)stats.interfaces; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.IpAddresses"]/*' />
        public override int NumberOfIPAddresses { get { return (int)stats.ipAddresses; } }
        /// <include file='doc\Statistics.uex' path='docs/doc[@for="IPStatistics.Routes"]/*' />
        public override int NumberOfRoutes { get { return (int)stats.routes; } }
    }
}


