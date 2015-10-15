// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"
#include "pal_utilities.h"
#include "pal_maphardwaretype.h"

#include <assert.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <ifaddrs.h>
#include <net/if.h>

static_assert(HAVE_AF_PACKET || HAVE_AF_LINK, "System must have AF_PACKET or AF_LINK.");
#if HAVE_AF_PACKET
#include <linux/if_packet.h>
#elif HAVE_AF_LINK
#include <net/if_dl.h>
#include <net/if_types.h>
#endif

const int NUM_BYTES_IN_IPV4_ADDRESS = 4;
const int NUM_BYTES_IN_IPV6_ADDRESS = 16;

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
