// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define HAVE_TCP_VAR_H 1
#if HAVE_TCP_VAR_H

#include "pal_config.h"
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
    printf("TCP packets received:  %d\n", systemStats->tcps_rcvtotal);
    printf("TCP packets sent:      %d\n", systemStats->tcps_sndtotal);

    retStats->SegmentsReceived = systemStats->tcps_rcvtotal;
    retStats->SegmentsSent = systemStats->tcps_sndtotal;

    free(oldp);
}

#endif // HAVE_TCP_VAR_H
