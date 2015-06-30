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
    /// <summary>Tcp specific statistics.</summary>
    internal class SystemTcpStatistics : TcpStatistics
    {
        MibTcpStats stats;

        private SystemTcpStatistics() { }
        internal SystemTcpStatistics(AddressFamily family)
        {
            uint result = UnsafeNetInfoNativeMethods.GetTcpStatisticsEx(out stats, family);

            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }
        }

        public override long MinimumTransmissionTimeout { get { return stats.minimumRetransmissionTimeOut; } }
        public override long MaximumTransmissionTimeout { get { return stats.maximumRetransmissionTimeOut; } }
        public override long MaximumConnections { get { return stats.maximumConnections; } }
        public override long ConnectionsInitiated { get { return stats.activeOpens; } }
        public override long ConnectionsAccepted { get { return stats.passiveOpens; } }//  is this true?  We should check
        public override long FailedConnectionAttempts { get { return stats.failedConnectionAttempts; } }
        public override long ResetConnections { get { return stats.resetConnections; } }
        public override long CurrentConnections { get { return stats.currentConnections; } }
        public override long SegmentsReceived { get { return stats.segmentsReceived; } }
        public override long SegmentsSent { get { return stats.segmentsSent; } }
        public override long SegmentsResent { get { return stats.segmentsResent; } }
        public override long ErrorsReceived { get { return stats.errorsReceived; } }
        public override long ResetsSent { get { return stats.segmentsSentWithReset; } }
        public override long CumulativeConnections { get { return stats.cumulativeConnections; } }
    }
}



