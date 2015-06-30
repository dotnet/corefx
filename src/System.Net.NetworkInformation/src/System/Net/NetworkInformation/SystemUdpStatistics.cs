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
    /// <summary>Udp statistics.</summary>
    internal class SystemUdpStatistics : UdpStatistics
    {
        private MibUdpStats _stats;

        private SystemUdpStatistics() { }
        internal SystemUdpStatistics(AddressFamily family)
        {
            uint result = UnsafeNetInfoNativeMethods.GetUdpStatisticsEx(out _stats, family);

            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }
        }

        public override long DatagramsReceived { get { return _stats.datagramsReceived; } }
        public override long IncomingDatagramsDiscarded { get { return _stats.incomingDatagramsDiscarded; } }
        public override long IncomingDatagramsWithErrors { get { return _stats.incomingDatagramsWithErrors; } }
        public override long DatagramsSent { get { return _stats.datagramsSent; } }
        public override int UdpListeners { get { return (int)_stats.udpListeners; } }
    }
}


