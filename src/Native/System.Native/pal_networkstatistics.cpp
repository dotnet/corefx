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
#include "pal_errno.h"

#include <sys/types.h>
#include <sys/sysctl.h>
#include <sys/socketvar.h>
#include <netinet/ip.h>
#include <netinet/ip_var.h>
#include <netinet/tcp.h>
#include <netinet/tcp_var.h>
#include <netinet/tcp_fsm.h>
#include <netinet/udp.h>
#include <netinet/udp_var.h>
#include <netinet/ip_icmp.h>
#include <netinet/icmp_var.h>
#include <netinet/icmp6.h>
#include <errno.h>

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
    retStats->CumulativeConnections = systemStats.tcps_connects;
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

extern "C" int32_t GetEstimatedTcpConnectionCount()
{
    int32_t count;
    ReadSysctlVar("net.inet.tcp.pcbcount", &count);
    return count;
}

size_t GetEstimatedXtcbSize()
{
    void* oldp = nullptr;
    void* newp = nullptr;
    size_t oldlenp, newlen = 0;

    sysctlbyname("net.inet.tcp.pcblist", oldp, &oldlenp, newp, newlen);
    return oldlenp;
}

extern "C" int32_t GetActiveTcpConnectionInfos(NativeTcpConnectionInformation* infos, int32_t* infoCount)
{
    size_t estimatedXtcbSize = GetEstimatedXtcbSize();
    uint8_t* buffer = new uint8_t[estimatedXtcbSize];
    void* newp = nullptr;
    size_t newlen = 0;

    while (sysctlbyname("net.inet.tcp.pcblist", buffer, &estimatedXtcbSize, newp, newlen) != 0)
    {
        delete buffer;
        estimatedXtcbSize = estimatedXtcbSize * 2;
        buffer = new uint8_t[estimatedXtcbSize];
    }

    int32_t count = static_cast<int32_t>(estimatedXtcbSize / sizeof(xtcpcb));
    if (count > *infoCount)
    {
        // Not enough space in caller-supplied buffer.
        *infoCount = count;
        return -1;
    }
    *infoCount = count;

    //  sizeof(xtcpcb) == 524
    tcpcb tcp_pcb;
    inpcb in_pcb;
    xinpgen* xHeadPtr;
    int32_t connectionIndex = -1;
    xHeadPtr = reinterpret_cast<xinpgen*>(buffer);
    for (xHeadPtr = reinterpret_cast<xinpgen*>(reinterpret_cast<uint8_t*>(xHeadPtr) + xHeadPtr->xig_len);
            xHeadPtr->xig_len >= sizeof(xtcpcb);
            xHeadPtr = reinterpret_cast<xinpgen*>(reinterpret_cast<uint8_t*>(xHeadPtr) + xHeadPtr->xig_len))
    {
        connectionIndex++;
        xtcpcb* head_xtcpb = reinterpret_cast<xtcpcb*>(xHeadPtr);

        tcp_pcb = head_xtcpb->xt_tp;
        in_pcb = head_xtcpb->xt_inp;

        NativeTcpConnectionInformation* ntci = &infos[connectionIndex];
        ntci->State = MapTcpState(tcp_pcb.t_state);

        uint8_t vflag = in_pcb.inp_vflag; // INP_IPV4 or INP_IPV6
        bool isIpv4 = (vflag & INP_IPV4) == INP_IPV4;
        if (isIpv4)
        {
            memcpy(&ntci->LocalEndPoint.AddressBytes, &in_pcb.inp_laddr.s_addr, 4);
            memcpy(&ntci->RemoteEndPoint.AddressBytes, &in_pcb.inp_faddr.s_addr, 4);
            ntci->LocalEndPoint.NumAddressBytes = 4;
            ntci->RemoteEndPoint.NumAddressBytes = 4;
        }
        else
        {
            memcpy(&ntci->LocalEndPoint.AddressBytes, &in_pcb.in6p_laddr.s6_addr, 16);
            memcpy(&ntci->RemoteEndPoint.AddressBytes, &in_pcb.in6p_faddr.s6_addr, 16);
            ntci->LocalEndPoint.NumAddressBytes = 16;
            ntci->RemoteEndPoint.NumAddressBytes = 16;
        }

        ntci->LocalEndPoint.Port = in_pcb.inp_lport;
        ntci->RemoteEndPoint.Port = in_pcb.inp_fport;
    }

    return 0;
}

TcpState MapTcpState(int tcpState)
{
    switch (tcpState)
    {
        case TCPS_CLOSED:
            return Closed;
        case TCPS_LISTEN:
            return Listen;
        case TCPS_SYN_SENT:
            return SynSent;
        case TCPS_SYN_RECEIVED:
            return SynReceived;
        case TCPS_ESTABLISHED:
            return Established;
        case TCPS_CLOSE_WAIT:
            return CloseWait;
        case TCPS_FIN_WAIT_1:
            return FinWait1;
        case TCPS_CLOSING:
            return Closing;
        case TCPS_FIN_WAIT_2:
            return FinWait2;
        case TCPS_TIME_WAIT:
            return TimeWait;
        default:
            return Unknown;
    }
}

#endif // HAVE_TCP_VAR_H
