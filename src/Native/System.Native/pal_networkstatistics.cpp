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
#include <netinet/udp.h>
#include <netinet/udp_var.h>
#include <netinet/ip_icmp.h>
#include <netinet/icmp_var.h>
#include <netinet/icmp6.h>

template <class RetType>
int32_t ReadSysctlVar(const char* name, RetType* value)
{
    size_t oldlenp = sizeof(RetType);
    void* newp = nullptr;
    size_t newlen = 0;
    return sysctlbyname(name, value, &oldlenp, newp, newlen);
}

extern "C" int32_t GetTcpGlobalStatistics(TcpGlobalStatistics* retStats)
{
    assert(retStats != nullptr);

    tcpstat systemStats;
    if (ReadSysctlVar("net.inet.tcp.stats", &systemStats))
    {
        memset(retStats, 0, sizeof(TcpGlobalStatistics)); // out parameter must be initialized.
        return -1;
    }

    retStats->ConnectionsAccepted = systemStats.tcps_accepts;
    retStats->ConnectionsInitiated = systemStats.tcps_connattempt;
    retStats->ErrorsReceived = systemStats.tcps_rcvbadsum + systemStats.tcps_rcvbadoff;
    retStats->FailedConnectionAttempts = systemStats.tcps_connattempt - systemStats.tcps_accepts;
    retStats->SegmentsReceived = systemStats.tcps_rcvtotal;
    retStats->SegmentsResent = systemStats.tcps_sndrexmitpack;
    retStats->SegmentsSent = systemStats.tcps_sndtotal;

    if (ReadSysctlVar("net.inet.tcp.pcbcount", &retStats->CurrentConnections))
    {
        retStats->CurrentConnections = 0;
        return -1;
    }
    printf("Current Connections: %d\n", retStats->CurrentConnections);

    return 0;
}

extern "C" int32_t GetIPv4GlobalStatistics(IPv4GlobalStatistics* retStats)
{
    assert(retStats != nullptr);

    ipstat systemStats;
    if (ReadSysctlVar("net.inet.ip.stats", &systemStats))
    {
        memset(retStats, 0, sizeof(IPv4GlobalStatistics)); // out parameter must be initialized.
        return -1;
    }

    retStats->OutboundPackets = systemStats.ips_localout;
    retStats->OutputPacketsNoRoute = systemStats.ips_noroute;
    retStats->CantFrags = systemStats.ips_cantfrag;
    retStats->DatagramsFragmented = systemStats.ips_fragmented;
    retStats->PacketsReassembled = systemStats.ips_reassembled;
    retStats->TotalPacketsReceived = systemStats.ips_total;
    retStats->PacketsDelivered = systemStats.ips_delivered;
    retStats->PacketsDiscarded = systemStats.ips_total - systemStats.ips_delivered;
    retStats->PacketsForwarded = systemStats.ips_forward;
    retStats->BadAddress = systemStats.ips_badaddr;
    retStats->BadHeader = systemStats.ips_badhlen; // Also include badaddr?
    retStats->UnknownProtos = systemStats.ips_noproto;

    if (ReadSysctlVar("net.inet.ip.ttl", &retStats->DefaultTtl))
    {
        retStats->DefaultTtl = 0;
        retStats->Forwarding = 0;
        return -1;
    }
    if (ReadSysctlVar("net.inet.ip.forwarding", &retStats->Forwarding))
    {
        retStats->Forwarding = 0;
        return -1;
    }

    return 0;
}

extern "C" int32_t GetIPv6GlobalStatistics(IPv6GlobalStatistics* retStats)
{
    memset(retStats, 0, sizeof(IPv6GlobalStatistics));
    return -1;
}

extern "C" int32_t GetUdpGlobalStatistics(UdpGlobalStatistics* retStats)
{
    assert(retStats != nullptr);

    udpstat systemStats;
    if (ReadSysctlVar("net.inet.udp.stats", &systemStats))
    {
        memset(retStats, 0, sizeof(UdpGlobalStatistics)); // out parameter must be initialized.
        return -1;
    }

    retStats->DatagramsReceived = systemStats.udps_ipackets;
    retStats->DatagramsSent = systemStats.udps_opackets;
    retStats->IncomingDiscarded = systemStats.udps_noport;
    retStats->IncomingErrors = systemStats.udps_hdrops + systemStats.udps_badsum + systemStats.udps_badlen;

    // This may contain both UDP4 and UDP6 listeners.
    if (ReadSysctlVar("net.inet.udp.pcbcount", &retStats->UdpListeners))
    {
        retStats->UdpListeners = 0;
        return -1;
    }

    return 0;
}

extern "C" int32_t GetIcmpv4GlobalStatistics(Icmpv4GlobalStatistics* retStats)
{
    assert(retStats != nullptr);

    icmpstat systemStats;
    if (ReadSysctlVar("net.inet.icmp.stats", &systemStats))
    {
        memset(retStats, 0, sizeof(Icmpv4GlobalStatistics));
        return -1;
    }

    u_int32_t* inHist = systemStats.icps_inhist;
    u_int32_t* outHist = systemStats.icps_outhist;

    retStats->AddressMaskRepliesReceived = inHist[ICMP_MASKREPLY];
    retStats->AddressMaskRepliesSent = outHist[ICMP_MASKREPLY];
    retStats->AddressMaskRequestsReceived = inHist[ICMP_MASKREQ];
    retStats->AddressMaskRequestsSent = outHist[ICMP_MASKREQ];
    retStats->DestinationUnreachableMessagesReceived = inHist[ICMP_UNREACH];
    retStats->DestinationUnreachableMessagesSent = outHist[ICMP_UNREACH];
    retStats->EchoRepliesReceived = inHist[ICMP_ECHOREPLY];
    retStats->EchoRepliesSent = outHist[ICMP_ECHOREPLY];
    retStats->EchoRequestsReceived = inHist[ICMP_ECHO];
    retStats->EchoRequestsSent = outHist[ICMP_ECHO];
    retStats->ParameterProblemsReceived = inHist[ICMP_PARAMPROB];
    retStats->ParameterProblemsSent = outHist[ICMP_PARAMPROB];
    retStats->RedirectsReceived = inHist[ICMP_REDIRECT];
    retStats->RedirectsSent = outHist[ICMP_REDIRECT];
    retStats->SourceQuenchesReceived = inHist[ICMP_SOURCEQUENCH];
    retStats->SourceQuenchesSent = outHist[ICMP_SOURCEQUENCH];
    retStats->TimeExceededMessagesReceived = inHist[ICMP_TIMXCEED];
    retStats->TimeExceededMessagesSent = outHist[ICMP_TIMXCEED];
    retStats->TimestampRepliesReceived = inHist[ICMP_TSTAMPREPLY];
    retStats->TimestampRepliesSent = outHist[ICMP_TSTAMPREPLY];
    retStats->TimestampRequestsReceived = inHist[ICMP_TSTAMP];
    retStats->TimestampRequestsSent = outHist[ICMP_TSTAMP];

    return 0;
}

extern "C" int32_t GetIcmpv6GlobalStatistics(Icmpv6GlobalStatistics* retStats)
{
    assert(retStats != nullptr);

    icmp6stat systemStats;
    if (ReadSysctlVar("net.inet6.icmp6.stats", &systemStats))
    {
        memset(retStats, 0, sizeof(Icmpv6GlobalStatistics));
        return -1;
    }

    uint64_t* inHist = systemStats.icp6s_inhist;
    uint64_t* outHist = systemStats.icp6s_outhist;

    retStats->DestinationUnreachableMessagesReceived = inHist[ICMP6_DST_UNREACH];
    retStats->DestinationUnreachableMessagesSent = outHist[ICMP6_DST_UNREACH];
    retStats->EchoRepliesReceived = inHist[ICMP6_ECHO_REPLY];
    retStats->EchoRepliesSent = outHist[ICMP6_ECHO_REPLY];
    retStats->EchoRequestsReceived = inHist[ICMP6_ECHO_REQUEST];
    retStats->EchoRequestsSent = outHist[ICMP6_ECHO_REQUEST];
    retStats->MembershipQueriesReceived = inHist[ICMP6_MEMBERSHIP_QUERY];
    retStats->MembershipQueriesSent = outHist[ICMP6_MEMBERSHIP_QUERY];
    retStats->MembershipReductionsReceived = inHist[ICMP6_MEMBERSHIP_REDUCTION];
    retStats->MembershipReductionsSent = outHist[ICMP6_MEMBERSHIP_REDUCTION];
    retStats->MembershipReportsReceived = inHist[ICMP6_MEMBERSHIP_REPORT];
    retStats->MembershipReportsSent = outHist[ICMP6_MEMBERSHIP_REPORT];
    retStats->NeighborAdvertisementsReceived = inHist[ND_NEIGHBOR_ADVERT];
    retStats->NeighborAdvertisementsSent = outHist[ND_NEIGHBOR_ADVERT];
    retStats->NeighborSolicitsReceived = inHist[ND_NEIGHBOR_SOLICIT];
    retStats->NeighborSolicitsSent = outHist[ND_NEIGHBOR_SOLICIT];
    retStats->PacketTooBigMessagesReceived = inHist[ICMP6_PACKET_TOO_BIG];
    retStats->PacketTooBigMessagesSent = outHist[ICMP6_PACKET_TOO_BIG];
    retStats->ParameterProblemsReceived = inHist[ICMP6_PARAM_PROB];
    retStats->ParameterProblemsSent = outHist[ICMP6_PARAM_PROB];
    retStats->RedirectsReceived = inHist[ND_REDIRECT];
    retStats->RedirectsSent = outHist[ND_REDIRECT];
    retStats->RouterAdvertisementsReceived = inHist[ND_ROUTER_ADVERT];
    retStats->RouterAdvertisementsSent = outHist[ND_ROUTER_ADVERT];
    retStats->RouterSolicitsReceived = inHist[ND_ROUTER_SOLICIT];
    retStats->RouterSolicitsSent = outHist[ND_ROUTER_SOLICIT];
    retStats->TimeExceededMessagesReceived = inHist[ICMP6_TIME_EXCEEDED];
    retStats->TimeExceededMessagesSent = outHist[ICMP6_TIME_EXCEEDED];

    return 0;
}

#endif // HAVE_TCP_VAR_H
