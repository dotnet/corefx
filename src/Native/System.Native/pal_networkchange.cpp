// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"

#if HAVE_LINUX_RTNETLINK_H
#include "pal_types.h"
#include "pal_networkchange.h"
#include "pal_utilities.h"

#include <unistd.h>
#include <sys/types.h>
#include <sys/uio.h>
#include <sys/socket.h>
#include <net/if.h>
#include <linux/netlink.h>
#include <linux/rtnetlink.h>

#pragma clang diagnostic ignored "-Wcast-align"

extern "C" int CreateNetworkChangeListenerSocket()
{
    struct sockaddr_nl sa = { };
    sa.nl_family = AF_NETLINK;
    sa.nl_groups = RTMGRP_LINK | RTMGRP_IPV4_IFADDR;
    int sock = socket(AF_NETLINK, SOCK_RAW, NETLINK_ROUTE);
    bind(sock, reinterpret_cast<sockaddr*>(&sa), sizeof(sa));
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
        return None;
    }

    for (hdr = reinterpret_cast<nlmsghdr*>(buffer); NLMSG_OK(hdr, len); hdr = NLMSG_NEXT(hdr, len))
    {
        switch (hdr->nlmsg_type)
        {
            case NLMSG_DONE:
                return None;
            case NLMSG_ERROR:
                return None;
            case RTM_NEWADDR:
                return AddressAdded;
            case RTM_DELADDR:
                return AddressRemoved;
            case RTM_NEWLINK:
                return ReadNewLinkMessage(hdr);
            case RTM_DELLINK:
                return LinkRemoved;
            default:
                return None;
        }
    }

    return None;
}

NetworkChangeKind ReadNewLinkMessage(nlmsghdr* hdr)
{
    ifinfomsg* ifimsg;
    ifimsg = reinterpret_cast<ifinfomsg*>(NLMSG_DATA(hdr));
    if(ifimsg->ifi_family == AF_INET)
    {
        if(ifimsg->ifi_flags & IFF_UP)
        {
            return LinkAdded;
        }
    }

    return None;
}

#endif // HAVE_LINUX_NETLINK_H
