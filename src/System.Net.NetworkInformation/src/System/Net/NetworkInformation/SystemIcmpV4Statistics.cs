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
    /// <summary>Icmp statistics for IPv4.</summary>
    internal class SystemIcmpV4Statistics : IcmpV4Statistics
    {
        MibIcmpInfo stats;

        internal SystemIcmpV4Statistics()
        {
            uint result = UnsafeNetInfoNativeMethods.GetIcmpStatistics(out stats);

            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }
        }

        public override long MessagesSent { get { return stats.outStats.messages; } }
        public override long MessagesReceived { get { return stats.inStats.messages; } }

        public override long ErrorsSent { get { return stats.outStats.errors; } }
        public override long ErrorsReceived { get { return stats.inStats.errors; } }


        public override long DestinationUnreachableMessagesSent { get { return stats.outStats.destinationUnreachables; } }
        public override long DestinationUnreachableMessagesReceived { get { return stats.inStats.destinationUnreachables; } }

        public override long TimeExceededMessagesSent { get { return stats.outStats.timeExceeds; } }
        public override long TimeExceededMessagesReceived { get { return stats.inStats.timeExceeds; } }

        public override long ParameterProblemsSent { get { return stats.outStats.parameterProblems; } }
        public override long ParameterProblemsReceived { get { return stats.inStats.parameterProblems; } }


        public override long SourceQuenchesSent { get { return stats.outStats.sourceQuenches; } }
        public override long SourceQuenchesReceived { get { return stats.inStats.sourceQuenches; } }


        public override long RedirectsSent { get { return stats.outStats.redirects; } }
        public override long RedirectsReceived { get { return stats.inStats.redirects; } }


        public override long EchoRequestsSent { get { return stats.outStats.echoRequests; } }
        public override long EchoRequestsReceived { get { return stats.inStats.echoRequests; } }

        public override long EchoRepliesSent { get { return stats.outStats.echoReplies; } }
        public override long EchoRepliesReceived { get { return stats.inStats.echoReplies; } }


        public override long TimestampRequestsSent { get { return stats.outStats.timestampRequests; } }
        public override long TimestampRequestsReceived { get { return stats.inStats.timestampRequests; } }

        public override long TimestampRepliesSent { get { return stats.outStats.timestampReplies; } }
        public override long TimestampRepliesReceived { get { return stats.inStats.timestampReplies; } }

        public override long AddressMaskRequestsSent { get { return stats.outStats.addressMaskRequests; } }
        public override long AddressMaskRequestsReceived { get { return stats.inStats.addressMaskRequests; } }

        public override long AddressMaskRepliesSent { get { return stats.outStats.addressMaskReplies; } }
        public override long AddressMaskRepliesReceived { get { return stats.inStats.addressMaskReplies; } }
    }
}


