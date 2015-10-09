// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"

// These functions are only used for platforms which support
// using sysctl to gather protocol statistics information.
// Currently, this is all keyed off of whether the include tcp_var.h
// exists, but we may want to make this more granular for differnet platforms.

#if HAVE_TCP_VAR_H

#include "pal_utilities.h"
#include "pal_networkstatistics.h"

#include <sys/types.h>
#include <sys/sysctl.h>
#include <sys/socketvar.h>
#include <netinet/ip.h>
#include <netinet/ip_var.h>
#include <netinet/tcp.h>
#include <netinet/tcp_var.h>

extern "C" void GetTcpGlobalStatistics(TcpGlobalStatistics* retStats)
{
    memset(retStats, 0, sizeof(TcpGlobalStatistics));

    uint8_t* oldp = new uint8_t[sizeof(tcpstat)];
    size_t oldlenp = sizeof(ipstat);
    void* newp = nullptr;
    size_t newlen = 0;

    int result = sysctlbyname("net.inet.tcp.stats", oldp, &oldlenp, newp, newlen);
    if (result != 0)
    {
        perror("sysctlbyname failed.");
    }

    tcpstat* systemStats = reinterpret_cast<tcpstat*>(oldp);
    
    retStats->ConnectionsAccepted = systemStats->tcps_accepts;
    retStats->ConnectionsInitiated = systemStats->tcps_connattempt;
    retStats->ErrorsReceived = systemStats->tcps_rcvbadsum + systemStats->tcps_rcvbadoff;
    retStats->FailedConnectionAttempts = systemStats->tcps_connattempt - systemStats->tcps_accepts;
    retStats->SegmentsReceived = systemStats->tcps_rcvtotal;
    retStats->SegmentsResent = systemStats->tcps_sndrexmitpack;
    retStats->SegmentsSent = systemStats->tcps_sndtotal;

    free(oldp);
}

#endif // HAVE_TCP_VAR_H
