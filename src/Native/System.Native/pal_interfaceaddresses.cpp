// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"
#include "pal_utilities.h"
#include "pal_interfaceaddresses.h"
#include "pal_maphardwaretype.h"

#include <assert.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <ifaddrs.h>
#include <net/if.h>
#include <sys/sysctl.h>

static_assert(HAVE_AF_PACKET || HAVE_AF_LINK, "System must have AF_PACKET or AF_LINK.");
#if HAVE_AF_PACKET
#include <linux/if_packet.h>
#elif HAVE_AF_LINK
#include <net/if_dl.h>
#include <net/if_types.h>
#endif

#if HAVE_RT_MSGHDR
#include <net/route.h>
#endif

extern "C" int32_t EnumerateInterfaceAddresses(IPv4AddressFound onIpv4Found,
                                            IPv6AddressFound onIpv6Found,
                                            LinkLayerAddressFound onLinkLayerFound)
{
    ifaddrs *headAddr, *current;
    if (getifaddrs(&headAddr) == -1)
    {
        return -1;
    }

    for (current = headAddr; current != nullptr; current = current->ifa_next)
    {
        uint32_t interfaceIndex = if_nametoindex(current->ifa_name);
        int family = current->ifa_addr->sa_family;
        if (family == AF_INET)
        {
            // IP Address
            IpAddressInfo iai = {};
            iai.InterfaceIndex = interfaceIndex;
            sockaddr_in* sain = reinterpret_cast<sockaddr_in*>(current->ifa_addr);
            memcpy(iai.AddressBytes, &sain->sin_addr.s_addr, sizeof(sain->sin_addr.s_addr));
            iai.NumAddressBytes = NUM_BYTES_IN_IPV4_ADDRESS;

            // Net Mask
            IpAddressInfo maskInfo = {};
            maskInfo.InterfaceIndex = interfaceIndex;
            sockaddr_in* mask_sain = reinterpret_cast<sockaddr_in*>(current->ifa_netmask);
            memcpy(maskInfo.AddressBytes, &mask_sain->sin_addr.s_addr, sizeof(mask_sain->sin_addr.s_addr));
            maskInfo.NumAddressBytes = NUM_BYTES_IN_IPV4_ADDRESS;

            onIpv4Found(current->ifa_name, &iai, &maskInfo);
        }
        else if (family == AF_INET6)
        {
            IpAddressInfo iai = {};
            iai.InterfaceIndex = interfaceIndex;
            sockaddr_in6* sain6 = reinterpret_cast<sockaddr_in6*>(current->ifa_addr);
            memcpy(iai.AddressBytes, sain6->sin6_addr.s6_addr, sizeof(sain6->sin6_addr.s6_addr));
            iai.NumAddressBytes = NUM_BYTES_IN_IPV6_ADDRESS;
            uint32_t scopeId = sain6->sin6_scope_id;
            onIpv6Found(current->ifa_name, &iai, &scopeId);
        }

        // LINUX : AF_PACKET = 17        
#if HAVE_AF_PACKET
        else if (family == AF_PACKET)
        {
            LinkLayerAddressInfo lla = {};
            lla.InterfaceIndex = interfaceIndex;
            sockaddr_ll* sall = reinterpret_cast<sockaddr_ll*>(current->ifa_addr);
            memcpy(&lla.AddressBytes, &sall->sll_addr, sall->sll_halen);
            lla.NumAddressBytes = sall->sll_halen;
            lla.HardwareType = MapHardwareType(sall->sll_hatype);
            onLinkLayerFound(current->ifa_name, &lla);
        }

#elif HAVE_AF_LINK
        // OSX/BSD : AF_LINK = 18
        else if (family == AF_LINK)
        {
            LinkLayerAddressInfo lla = {};
            sockaddr_dl* sadl = reinterpret_cast<sockaddr_dl*>(current->ifa_addr);
            lla.InterfaceIndex = interfaceIndex;
            memcpy(&lla.AddressBytes, reinterpret_cast<uint8_t*>(LLADDR(sadl)), sadl->sdl_alen);
            lla.NumAddressBytes = sadl->sdl_alen;
            lla.HardwareType = MapHardwareType(sadl->sdl_type);
            onLinkLayerFound(current->ifa_name, &lla);

            // Do stuff for OSX
        }
#endif
    }

    freeifaddrs(headAddr);
    return 0;
}

#if HAVE_RT_MSGHDR
extern "C" int32_t EnumerateGatewayAddressesForInterface(uint32_t interfaceIndex, GatewayAddressFound onGatewayFound)
{
    int routeDumpName[] = { CTL_NET, AF_ROUTE, 0, AF_INET, NET_RT_DUMP, 0 };

    size_t byteCount;

    if (sysctl(routeDumpName, 6, nullptr, &byteCount, nullptr, 0) != 0)
    {
        return -1;
    }

    uint8_t* buffer = new uint8_t[byteCount];

    while (sysctl(routeDumpName, 6, buffer, &byteCount, nullptr, 0) != 0)
    {
        delete(buffer);
        buffer = new uint8_t[byteCount];
    }

    rt_msghdr* hdr;
    for (uint8_t* headerBytePtr = buffer;
        static_cast<size_t>(headerBytePtr - buffer) < byteCount;
        headerBytePtr += hdr->rtm_msglen)
    {
        hdr = reinterpret_cast<rt_msghdr*>(headerBytePtr);
        int flags = hdr->rtm_flags;
        int isGateway = flags & RTF_GATEWAY;
        int gatewayPresent = hdr->rtm_addrs & RTA_GATEWAY;

        if (isGateway && gatewayPresent)
        {
            IpAddressInfo iai = { };
            iai.InterfaceIndex = interfaceIndex;
            iai.NumAddressBytes = NUM_BYTES_IN_IPV4_ADDRESS;
            sockaddr_in* sain = reinterpret_cast<sockaddr_in*>(headerBytePtr + sizeof(rt_msghdr));
            sain = sain + 1;
            memcpy(iai.AddressBytes, &sain->sin_addr.s_addr, sizeof(sain->sin_addr.s_addr));
            onGatewayFound(&iai);
        }
    }

    return 0;
}
#endif // HAVE_RT_MSGHDR
