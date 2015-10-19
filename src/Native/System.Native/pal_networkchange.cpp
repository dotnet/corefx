// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"

#if HAVE_LINUX_RTNETLINK_H
#include "pal_networkchange.h"
#include "pal_types.h"
#include "pal_utilities.h"

#include <linux/netlink.h>
#include <linux/rtnetlink.h>
#include <net/if.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <sys/uio.h>
#include <unistd.h>

#pragma clang diagnostic ignored "-Wcast-align" // NLMSG_* macros trigger this

extern "C" int CreateNetworkChangeListenerSocket()
{
    sockaddr_nl sa = { };
    sa.nl_family = AF_NETLINK;
    sa.nl_groups = RTMGRP_LINK | RTMGRP_IPV4_IFADDR;
    int sock = socket(AF_NETLINK, SOCK_RAW, NETLINK_ROUTE);
    if (bind(sock, reinterpret_cast<sockaddr*>(&sa), sizeof(sa) != 0))
    {
        return -1;
    }
    return sock;
}

extern "C" int32_t CloseNetworkChangeListenerSocket(int socket)
{
    int result = close(socket);
    return result;
}

extern "C" NetworkChangeKind ReadSingleEvent(int sock)
{
    char buffer[4096];
    iovec iov = { buffer, sizeof(buffer) };
    sockaddr_nl sanl;
    msghdr msg = { reinterpret_cast<void*>(&sanl), sizeof(sockaddr_nl), &iov, 1, NULL, 0, 0};
    nlmsghdr* hdr;
    ssize_t len = recvmsg(sock, &msg, 0);
    if (len == -1)
    {
        // Probably means the socket has been closed.
        // If so, the managed side will ignore the return value.
        return NetworkChangeKind::None;
    }

    for (hdr = reinterpret_cast<nlmsghdr*>(buffer); NLMSG_OK(hdr, len); hdr = NLMSG_NEXT(hdr, len))
    {
        switch (hdr->nlmsg_type)
        {
            case NLMSG_DONE:
                return NetworkChangeKind::None;
            case NLMSG_ERROR:
                return NetworkChangeKind::None;
            case RTM_NEWADDR:
                return NetworkChangeKind::AddressAdded;
            case RTM_DELADDR:
                return NetworkChangeKind::AddressRemoved;
            case RTM_NEWLINK:
                return ReadNewLinkMessage(hdr);
            case RTM_DELLINK:
                return NetworkChangeKind::LinkRemoved;
            default:
                return NetworkChangeKind::None;
        }
    }

    return NetworkChangeKind::None;
}

NetworkChangeKind ReadNewLinkMessage(nlmsghdr* hdr)
{
    assert(hdr != nullptr);
    ifinfomsg* ifimsg;
    ifimsg = reinterpret_cast<ifinfomsg*>(NLMSG_DATA(hdr));
    if(ifimsg->ifi_family == AF_INET)
    {
        if(ifimsg->ifi_flags & IFF_UP)
        {
            return NetworkChangeKind::LinkAdded;
        }
    }

    return NetworkChangeKind::None;
}

#endif // HAVE_LINUX_NETLINK_H
