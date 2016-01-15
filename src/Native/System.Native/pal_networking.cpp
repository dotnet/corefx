// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"
#include "pal_networking.h"
#include "pal_utilities.h"

#if HAVE_ALLOCA_H
#include <alloca.h>
#endif
#include <arpa/inet.h>
#include <assert.h>
#if HAVE_EPOLL
#include <sys/epoll.h>
#elif HAVE_KQUEUE
#include <sys/types.h>
#include <sys/event.h>
#include <sys/time.h>
#endif
#include <errno.h>
#include <netdb.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <sys/socket.h>
#if defined(__APPLE__) && __APPLE__
#include <sys/socketvar.h>
#endif
#include <unistd.h>
#include <vector>

#if !HAVE_IN_PKTINFO
// On platforms, such as FreeBSD, where in_pktinfo
// is not available, fallback to custom definition
// with required members.
struct in_pktinfo
{
    in_addr ipi_addr;
};
#define IP_PKTINFO IP_RECVDSTADDR
#endif

#if !defined(IPV6_ADD_MEMBERSHIP) && defined(IPV6_JOIN_GROUP)
#define IPV6_ADD_MEMBERSHIP IPV6_JOIN_GROUP
#endif

#if !defined(IPV6_DROP_MEMBERSHIP) && defined(IPV6_LEAVE_GROUP)
#define IPV6_DROP_MEMBERSHIP IPV6_LEAVE_GROUP
#endif

enum
{
#if defined(__APPLE__) && __APPLE__
    LINGER_OPTION_NAME = SO_LINGER_SEC
#else
    LINGER_OPTION_NAME = SO_LINGER,
#endif
};

enum
{
    HOST_ENTRY_HANDLE_ADDRINFO = 1,
    HOST_ENTRY_HANDLE_HOSTENT = 2,
};

enum
{
    INET6_ADDRSTRLEN_MANAGED = 65 // Managed code has a longer max IPv6 string length
};

static_assert(PAL_HOST_NOT_FOUND == HOST_NOT_FOUND, "");
static_assert(PAL_TRY_AGAIN == TRY_AGAIN, "");
static_assert(PAL_NO_RECOVERY == NO_RECOVERY, "");
static_assert(PAL_NO_DATA == NO_DATA, "");
static_assert(PAL_NO_ADDRESS == NO_ADDRESS, "");
static_assert(sizeof(uint8_t) == sizeof(char), ""); // We make casts from uint8_t to char so make sure it's legal

// NOTE: clang has trouble with offsetof nested inside of static_assert. Instead, store
//       the necessary field offsets in constants.
const int OffsetOfIOVectorBase = offsetof(IOVector, Base);
const int OffsetOfIOVectorCount = offsetof(IOVector, Count);
const int OffsetOfIovecBase = offsetof(iovec, iov_base);
const int OffsetOfIovecLen = offsetof(iovec, iov_len);

// We require that IOVector have the same layout as iovec.
static_assert(sizeof(IOVector) == sizeof(iovec), "");
static_assert(sizeof(decltype(IOVector::Base)) == sizeof(decltype(iovec::iov_base)), "");
static_assert(OffsetOfIOVectorBase == OffsetOfIovecBase, "");
static_assert(sizeof(decltype(IOVector::Count)) == sizeof(decltype(iovec::iov_len)), "");
static_assert(OffsetOfIOVectorCount == OffsetOfIovecLen, "");

template <typename T>
constexpr T Min(T left, T right)
{
    return left < right ? left : right;
}

template <typename T>
constexpr T Max(T left, T right)
{
    return left > right ? left : right;
}

static int IpStringToAddressHelper(const uint8_t* address, const uint8_t* port, bool isIPv6, addrinfo*& info)
{
    assert(address != nullptr);

    addrinfo hint = {.ai_family = isIPv6 ? AF_INET6 : AF_INET, .ai_flags = AI_NUMERICHOST | AI_NUMERICSERV};

    info = nullptr;
    return getaddrinfo(reinterpret_cast<const char*>(address), reinterpret_cast<const char*>(port), &hint, &info);
}

static void ConvertByteArrayToIn6Addr(in6_addr& addr, const uint8_t* buffer, int32_t bufferLength)
{
#if HAVE_IN6_U
    assert(bufferLength == ARRAY_SIZE(addr.__in6_u.__u6_addr8));
    memcpy(addr.__in6_u.__u6_addr8, buffer, UnsignedCast(bufferLength));
#else
    assert(bufferLength == ARRAY_SIZE(addr.__u6_addr.__u6_addr8));
    memcpy(addr.__u6_addr.__u6_addr8, buffer, UnsignedCast(bufferLength));
#endif
}

static void ConvertIn6AddrToByteArray(uint8_t* buffer, int32_t bufferLength, const in6_addr& addr)
{
#if HAVE_IN6_U
    assert(bufferLength == ARRAY_SIZE(addr.__in6_u.__u6_addr8));
    memcpy(buffer, addr.__in6_u.__u6_addr8, UnsignedCast(bufferLength));
#else
    assert(bufferLength == ARRAY_SIZE(addr.__u6_addr.__u6_addr8));
    memcpy(buffer, addr.__u6_addr.__u6_addr8, UnsignedCast(bufferLength));
#endif
}

static void ConvertByteArrayToSockAddrIn6(sockaddr_in6& addr, const uint8_t* buffer, int32_t bufferLength)
{
    ConvertByteArrayToIn6Addr(addr.sin6_addr, buffer, bufferLength);

    // Mark that this is INET6
    addr.sin6_family = AF_INET6;
}

static void ConvertByteArrayToInAddr(in_addr& addr, const uint8_t* buffer, int32_t bufferLength)
{
    assert(bufferLength == NUM_BYTES_IN_IPV4_ADDRESS);
    (void)bufferLength; // Silence compiler warnings about unused variables on release mode

    addr.s_addr = *reinterpret_cast<const uint32_t*>(buffer); // Send back in network byte order.
}

static void ConvertInAddrToByteArray(uint8_t* buffer, int32_t bufferLength, const in_addr& addr)
{
    assert(bufferLength == NUM_BYTES_IN_IPV4_ADDRESS);
    (void)bufferLength; // Silence compiler warnings about unused variables on release mode

    *reinterpret_cast<uint32_t*>(buffer) = addr.s_addr; // Send back in network byte order.
}

static void ConvertByteArrayToSockAddrIn(sockaddr_in& addr, const uint8_t* buffer, int32_t bufferLength)
{
    ConvertByteArrayToInAddr(addr.sin_addr, buffer, bufferLength);

    addr.sin_family = AF_INET;
}

static int32_t ConvertGetAddrInfoAndGetNameInfoErrorsToPal(int32_t error)
{
    switch (error)
    {
        case 0:
            return 0;
        case EAI_AGAIN:
            return PAL_EAI_AGAIN;
        case EAI_BADFLAGS:
            return PAL_EAI_BADFLAGS;
#ifdef EAI_FAIL
        case EAI_FAIL:
            return PAL_EAI_FAIL;
#endif
        case EAI_FAMILY:
            return PAL_EAI_FAMILY;
        case EAI_NONAME:
#ifdef EAI_NODATA
        case EAI_NODATA:
#endif
            return PAL_EAI_NONAME;
    }

    assert(false && "Unknown AddrInfo error flag");
    return -1;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t IPv6StringToAddress(const uint8_t* address, const uint8_t* port, uint8_t* buffer, int32_t bufferLength, uint32_t* scope)
{
    return SystemNative_IPv6StringToAddress(address, port, buffer, bufferLength, scope);
}

extern "C" int32_t
SystemNative_IPv6StringToAddress(const uint8_t* address, const uint8_t* port, uint8_t* buffer, int32_t bufferLength, uint32_t* scope)
{
    assert(buffer != nullptr);
    assert(bufferLength == NUM_BYTES_IN_IPV6_ADDRESS);
    assert(scope != nullptr);

    // Call our helper to do the getaddrinfo call for us; once we have the info, copy what we need
    addrinfo* info;
    int32_t result = IpStringToAddressHelper(address, port, true, info);
    if (result == 0)
    {
        sockaddr_in6* addr = reinterpret_cast<sockaddr_in6*>(info->ai_addr);
        ConvertIn6AddrToByteArray(buffer, bufferLength, addr->sin6_addr);
        *scope = addr->sin6_scope_id;

        freeaddrinfo(info);
    }

    return ConvertGetAddrInfoAndGetNameInfoErrorsToPal(result);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t IPv4StringToAddress(const uint8_t* address, uint8_t* buffer, int32_t bufferLength, uint16_t* port)
{
    return SystemNative_IPv4StringToAddress(address, buffer, bufferLength, port);
}

extern "C" int32_t SystemNative_IPv4StringToAddress(const uint8_t* address, uint8_t* buffer, int32_t bufferLength, uint16_t* port)
{
    assert(buffer != nullptr);
    assert(bufferLength == NUM_BYTES_IN_IPV4_ADDRESS);
    assert(port != nullptr);

    // Call our helper to do the getaddrinfo call for us; once we have the info, copy what we need
    addrinfo* info;
    int32_t result = IpStringToAddressHelper(address, nullptr, false, info);
    if (result == 0)
    {
        sockaddr_in* addr = reinterpret_cast<sockaddr_in*>(info->ai_addr);
        ConvertInAddrToByteArray(buffer, bufferLength, addr->sin_addr);
        *port = addr->sin_port;

        freeaddrinfo(info);
    }

    return ConvertGetAddrInfoAndGetNameInfoErrorsToPal(result);
}

static void AppendScopeIfNecessary(uint8_t* string, int32_t stringLength, uint32_t scope)
{
    assert(scope != 0);

    // Find the scope ID, if it exists
    int i;
    for (i = 0; i < stringLength && string[i] != '\0'; i++)
    {
        if (string[i] == '%')
        {
            // Found a scope ID. Assume it's correct and return.
            return;
        }
    }

    auto capacity = static_cast<size_t>(stringLength - i);
    int n = snprintf(reinterpret_cast<char*>(&string[i]), capacity, "%%%d", scope);
    assert(static_cast<size_t>(n) < capacity);
    (void)n; // Silence an unused variable warning in release mode
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t IPAddressToString(
    const uint8_t* address, int32_t addressLength, bool isIPv6, uint8_t* string, int32_t stringLength, uint32_t scope)
{
    return SystemNative_IPAddressToString(address, addressLength, isIPv6, string, stringLength, scope);
}

extern "C" int32_t SystemNative_IPAddressToString(
    const uint8_t* address, int32_t addressLength, bool isIPv6, uint8_t* string, int32_t stringLength, uint32_t scope)
{
    assert(address != nullptr);
    assert((addressLength == NUM_BYTES_IN_IPV6_ADDRESS) || (addressLength == NUM_BYTES_IN_IPV4_ADDRESS));
    assert(string != nullptr);

    // These constants differ per platform so the managed side uses the bigger value; therefore, check that
    // the length is between the two lengths
    assert((stringLength >= INET_ADDRSTRLEN) && (stringLength <= INET6_ADDRSTRLEN_MANAGED));

    socklen_t len = UnsignedCast(stringLength);

    sockaddr_in inAddr;
    sockaddr_in6 in6Addr;
    const sockaddr* addr;
    socklen_t addrLen;

    if (!isIPv6)
    {
        ConvertByteArrayToSockAddrIn(inAddr, address, addressLength);
        addr = reinterpret_cast<const sockaddr*>(&inAddr);
        addrLen = sizeof(inAddr);
    }
    else
    {
        in6Addr.sin6_scope_id = scope;
        ConvertByteArrayToSockAddrIn6(in6Addr, address, addressLength);
        addr = reinterpret_cast<const sockaddr*>(&in6Addr);
        addrLen = sizeof(in6Addr);
    }

    int result = getnameinfo(addr, addrLen, reinterpret_cast<char*>(string), len, nullptr, 0, NI_NUMERICHOST);
    if (result != 0)
    {
        return ConvertGetAddrInfoAndGetNameInfoErrorsToPal(result);
    }

    // Some platforms do not append unknown scope IDs, but the managed code wants this behavior.
    if (isIPv6 && scope != 0)
    {
        AppendScopeIfNecessary(string, stringLength, scope);
    }

    return 0;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetHostEntryForName(const uint8_t* address, HostEntry* entry)
{
    return SystemNative_GetHostEntryForName(address, entry);
}

extern "C" int32_t SystemNative_GetHostEntryForName(const uint8_t* address, HostEntry* entry)
{
    if (address == nullptr || entry == nullptr)
    {
        return PAL_EAI_BADARG;
    }

    // Get all address families and the canonical name
    addrinfo hint = {.ai_family = AF_UNSPEC, .ai_flags = AI_CANONNAME};

    addrinfo* info = nullptr;
    int result = getaddrinfo(reinterpret_cast<const char*>(address), nullptr, &hint, &info);
    if (result != 0)
    {
        return ConvertGetAddrInfoAndGetNameInfoErrorsToPal(result);
    }

    *entry = {.CanonicalName = nullptr,
              .Aliases = nullptr,
              .AddressListHandle = reinterpret_cast<void*>(info),
              .IPAddressCount = 0,
              .HandleType = HOST_ENTRY_HANDLE_ADDRINFO};

    // Find the canonical name for this host (if any) and count the number of IP end points.
    for (addrinfo* ai = info; ai != nullptr; ai = ai->ai_next)
    {
        // If we haven't found a canonical name yet and this addrinfo has one, copy it
        if ((entry->CanonicalName == nullptr) && (ai->ai_canonname != nullptr))
        {
            entry->CanonicalName = reinterpret_cast<uint8_t*>(ai->ai_canonname);
        }

        if (ai->ai_family == AF_INET || ai->ai_family == AF_INET6)
        {
            entry->IPAddressCount++;
        }
    }

    return PAL_EAI_SUCCESS;
}

static int ConvertGetHostErrorPlatformToPal(int error)
{
    switch (error)
    {
        case HOST_NOT_FOUND:
            return PAL_HOST_NOT_FOUND;

        case TRY_AGAIN:
            return PAL_TRY_AGAIN;

        case NO_RECOVERY:
            return PAL_NO_RECOVERY;

        case NO_DATA:
            return PAL_NO_DATA;

        default:
            assert(false && "Unknown gethostbyname/gethostbyaddr error code");
            return PAL_HOST_NOT_FOUND;
    }
}

static void ConvertHostEntPlatformToPal(HostEntry& hostEntry, hostent& entry)
{
    hostEntry = {.CanonicalName = reinterpret_cast<uint8_t*>(entry.h_name),
                 .Aliases = reinterpret_cast<uint8_t**>(entry.h_aliases),
                 .AddressListHandle = reinterpret_cast<void*>(&entry),
                 .IPAddressCount = 0,
                 .HandleType = HOST_ENTRY_HANDLE_HOSTENT};

    for (int i = 0; entry.h_addr_list[i] != nullptr; i++)
    {
        hostEntry.IPAddressCount++;
    }
}

#if HAVE_GETHOSTBYNAME_R
static int GetHostByNameHelper(const uint8_t* hostname, hostent** entry)
{
    assert(hostname != nullptr);
    assert(entry != nullptr);

    size_t scratchLen = 512;

    for (;;)
    {
        uint8_t* buffer = reinterpret_cast<uint8_t*>(malloc(sizeof(hostent) + scratchLen));
        if (buffer == nullptr)
        {
            return PAL_NO_MEM;
        }

        hostent* result = reinterpret_cast<hostent*>(buffer);
        char* scratch = reinterpret_cast<char*>(&buffer[sizeof(hostent)]);

        int getHostErrno;
        int err =
            gethostbyname_r(reinterpret_cast<const char*>(hostname), result, scratch, scratchLen, entry, &getHostErrno);
        switch (err)
        {
            case 0:
                *entry = result;
                return 0;

            case ERANGE:
                free(buffer);
                scratchLen *= 2;
                break;

            default:
                free(buffer);
                *entry = nullptr;
                return getHostErrno;
        }
    }
}
#endif

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetHostByName(const uint8_t* hostname, HostEntry* entry)
{
    return SystemNative_GetHostByName(hostname, entry);
}

extern "C" int32_t SystemNative_GetHostByName(const uint8_t* hostname, HostEntry* entry)
{
    if (hostname == nullptr || entry == nullptr)
    {
        return PAL_BAD_ARG;
    }

    hostent* hostEntry = nullptr;
    int error = 0;

#if HAVE_THREAD_SAFE_GETHOSTBYNAME_AND_GETHOSTBYADDR
    hostEntry = gethostbyname(reinterpret_cast<const char*>(hostname));
    error = h_errno;
#elif HAVE_GETHOSTBYNAME_R
    error = GetHostByNameHelper(hostname, &hostEntry);
#else
#error Platform does not provide thread-safe gethostbyname
#endif

    if (hostEntry == nullptr)
    {
        return ConvertGetHostErrorPlatformToPal(error);
    }

    ConvertHostEntPlatformToPal(*entry, *hostEntry);
    return PAL_SUCCESS;
}

#if HAVE_GETHOSTBYADDR_R
static int GetHostByAddrHelper(const uint8_t* addr, const socklen_t addrLen, int type, hostent** entry)
{
    assert(addr != nullptr);
    assert(addrLen >= 0);
    assert(entry != nullptr);

    size_t scratchLen = 512;

    for (;;)
    {
        uint8_t* buffer = reinterpret_cast<uint8_t*>(malloc(sizeof(hostent) + scratchLen));
        if (buffer == nullptr)
        {
            return PAL_NO_MEM;
        }

        hostent* result = reinterpret_cast<hostent*>(buffer);
        char* scratch = reinterpret_cast<char*>(&buffer[sizeof(hostent)]);

        int getHostErrno;
        int err = gethostbyaddr_r(addr, addrLen, type, result, scratch, scratchLen, entry, &getHostErrno);
        switch (err)
        {
            case 0:
                *entry = result;
                return 0;

            case ERANGE:
                free(buffer);
                scratchLen *= 2;
                break;

            default:
                free(buffer);
                *entry = nullptr;
                return getHostErrno;
        }
    }
}
#endif

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetHostByAddress(const IPAddress* address, HostEntry* entry)
{
    return SystemNative_GetHostByAddress(address, entry);
}

extern "C" int32_t SystemNative_GetHostByAddress(const IPAddress* address, HostEntry* entry)
{
    if (address == nullptr || entry == nullptr)
    {
        return PAL_BAD_ARG;
    }

    uint8_t* addr = nullptr;
    socklen_t addrLen = 0;
    int type = AF_UNSPEC;

    in_addr inAddr = {};
    in6_addr in6Addr = {};

    if (!address->IsIPv6)
    {
        ConvertByteArrayToInAddr(inAddr, address->Address, NUM_BYTES_IN_IPV4_ADDRESS);
        addr = reinterpret_cast<uint8_t*>(&inAddr);
        addrLen = sizeof(inAddr);
        type = AF_INET;
    }
    else
    {
        ConvertByteArrayToIn6Addr(in6Addr, address->Address, NUM_BYTES_IN_IPV6_ADDRESS);
        addr = reinterpret_cast<uint8_t*>(&in6Addr);
        addrLen = sizeof(in6Addr);
        type = AF_INET6;
    }

    hostent* hostEntry = nullptr;
    int error = 0;

#if HAVE_THREAD_SAFE_GETHOSTBYNAME_AND_GETHOSTBYADDR
    hostEntry = gethostbyaddr(addr, addrLen, type);
    error = h_errno;
#elif HAVE_GETHOSTBYADDR_R
    error = GetHostByAddrHelper(addr, addrLen, type, &hostEntry);
#else
#error Platform does not provide thread-safe gethostbyname
#endif

    if (hostEntry == nullptr)
    {
        return ConvertGetHostErrorPlatformToPal(error);
    }

    ConvertHostEntPlatformToPal(*entry, *hostEntry);
    return PAL_SUCCESS;
}

static int32_t GetNextIPAddressFromAddrInfo(addrinfo** info, IPAddress* endPoint)
{
    assert(info != nullptr);
    assert(endPoint != nullptr);

    for (addrinfo* ai = *info; ai != nullptr; ai = ai->ai_next)
    {
        switch (ai->ai_family)
        {
            case AF_INET:
            {
                auto* inetSockAddr = reinterpret_cast<sockaddr_in*>(ai->ai_addr);

                ConvertInAddrToByteArray(endPoint->Address, NUM_BYTES_IN_IPV4_ADDRESS, inetSockAddr->sin_addr);
                endPoint->IsIPv6 = 0;
                break;
            }

            case AF_INET6:
            {
                auto* inet6SockAddr = reinterpret_cast<sockaddr_in6*>(ai->ai_addr);

                ConvertIn6AddrToByteArray(endPoint->Address, NUM_BYTES_IN_IPV6_ADDRESS, inet6SockAddr->sin6_addr);
                endPoint->IsIPv6 = 1;
                endPoint->ScopeId = inet6SockAddr->sin6_scope_id;
                break;
            }

            default:
                // Skip non-IPv4 and non-IPv6 addresses
                continue;
        }

        *info = ai->ai_next;
        return PAL_EAI_SUCCESS;
    }

    return PAL_EAI_NOMORE;
}

static int32_t GetNextIPAddressFromHostEnt(hostent** hostEntry, IPAddress* address)
{
    assert(hostEntry != nullptr);
    assert(address != nullptr);

    hostent* entry = *hostEntry;
    if (*entry->h_addr_list == nullptr)
    {
        return PAL_EAI_NOMORE;
    }

    switch (entry->h_addrtype)
    {
        case AF_INET:
        {
            auto* inAddr = reinterpret_cast<in_addr*>(entry->h_addr_list[0]);

            ConvertInAddrToByteArray(address->Address, NUM_BYTES_IN_IPV4_ADDRESS, *inAddr);
            address->IsIPv6 = 0;
            break;
        }

        case AF_INET6:
        {
            auto* in6Addr = reinterpret_cast<in6_addr*>(entry->h_addr_list[0]);

            ConvertIn6AddrToByteArray(address->Address, NUM_BYTES_IN_IPV6_ADDRESS, *in6Addr);
            address->IsIPv6 = 1;
            address->ScopeId = 0;
            break;
        }

        default:
            return PAL_EAI_NOMORE;
    }

    entry->h_addr_list = &entry->h_addr_list[1];
    return PAL_EAI_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetNextIPAddress(const HostEntry* hostEntry, void** addressListHandle, IPAddress* endPoint)
{
    return SystemNative_GetNextIPAddress(hostEntry, addressListHandle, endPoint);
}

extern "C" int32_t SystemNative_GetNextIPAddress(const HostEntry* hostEntry, void** addressListHandle, IPAddress* endPoint)
{
    if (hostEntry == nullptr || addressListHandle == nullptr || endPoint == nullptr)
    {
        return PAL_EAI_BADARG;
    }

    switch (hostEntry->HandleType)
    {
        case HOST_ENTRY_HANDLE_ADDRINFO:
            return GetNextIPAddressFromAddrInfo(reinterpret_cast<addrinfo**>(addressListHandle), endPoint);

        case HOST_ENTRY_HANDLE_HOSTENT:
            return GetNextIPAddressFromHostEnt(reinterpret_cast<hostent**>(addressListHandle), endPoint);

        default:
            return PAL_EAI_BADARG;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" void FreeHostEntry(HostEntry* entry)
{
    return SystemNative_FreeHostEntry(entry);
}

extern "C" void SystemNative_FreeHostEntry(HostEntry* entry)
{
    if (entry != nullptr)
    {
        switch (entry->HandleType)
        {
            case HOST_ENTRY_HANDLE_ADDRINFO:
            {
                auto* ai = reinterpret_cast<addrinfo*>(entry->AddressListHandle);
                freeaddrinfo(ai);
                break;
            }

            case HOST_ENTRY_HANDLE_HOSTENT:
            {
#if !HAVE_THREAD_SAFE_GETHOSTBYNAME_AND_GETHOSTBYADDR
                free(entry->AddressListHandle);
#endif
                break;
            }

            default:
                break;
        }
    }
}

inline int32_t ConvertGetNameInfoFlagsToPal(int32_t flags)
{
    int32_t outFlags = 0;
    if ((flags & NI_NAMEREQD) == NI_NAMEREQD)
    {
        outFlags |= PAL_NI_NAMEREQD;
    }
    if ((flags & NI_NUMERICHOST) == NI_NUMERICHOST)
    {
        outFlags |= PAL_NI_NUMERICHOST;
    }

    return outFlags;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetNameInfo(const uint8_t* address,
    int32_t addressLength,
    bool isIPv6,
    uint8_t* host,
    int32_t hostLength,
    uint8_t* service,
    int32_t serviceLength,
    int32_t flags)
{
    return SystemNative_GetNameInfo(address,
        addressLength,
        isIPv6,
        host,
        hostLength,
        service,
        serviceLength,
        flags);
}

extern "C" int32_t SystemNative_GetNameInfo(const uint8_t* address,
                               int32_t addressLength,
                               bool isIPv6,
                               uint8_t* host,
                               int32_t hostLength,
                               uint8_t* service,
                               int32_t serviceLength,
                               int32_t flags)
{
    assert(address != nullptr);
    assert(addressLength > 0);
    assert((host != nullptr) || (service != nullptr));
    assert((hostLength > 0) || (serviceLength > 0));

    int32_t nativeFlags = ConvertGetNameInfoFlagsToPal(flags);
    int32_t result;

    if (isIPv6)
    {
        sockaddr_in6 addr = {};
        ConvertByteArrayToSockAddrIn6(addr, address, addressLength);
        result = getnameinfo(reinterpret_cast<const sockaddr*>(&addr),
                             sizeof(sockaddr_in6),
                             reinterpret_cast<char*>(host),
                             UnsignedCast(hostLength),
                             reinterpret_cast<char*>(service),
                             UnsignedCast(serviceLength),
                             nativeFlags);
    }
    else
    {
        sockaddr_in addr = {};
        ConvertByteArrayToSockAddrIn(addr, address, addressLength);
        result = getnameinfo(reinterpret_cast<const sockaddr*>(&addr),
                             sizeof(sockaddr_in),
                             reinterpret_cast<char*>(host),
                             UnsignedCast(hostLength),
                             reinterpret_cast<char*>(service),
                             UnsignedCast(serviceLength),
                             nativeFlags);
    }

    return ConvertGetAddrInfoAndGetNameInfoErrorsToPal(result);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetDomainName(uint8_t* name, int32_t nameLength)
{
    return SystemNative_GetDomainName(name, nameLength);
}

extern "C" int32_t SystemNative_GetDomainName(uint8_t* name, int32_t nameLength)
{
    assert(name != nullptr);
    assert(nameLength > 0);

#if HAVE_GETDOMAINNAME_SIZET
    size_t namelen = UnsignedCast(nameLength);
#else
    int namelen = nameLength;
#endif

    return getdomainname(reinterpret_cast<char*>(name), namelen);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetHostName(uint8_t* name, int32_t nameLength)
{
    return SystemNative_GetHostName(name, nameLength);
}

extern "C" int32_t SystemNative_GetHostName(uint8_t* name, int32_t nameLength)
{
    assert(name != nullptr);
    assert(nameLength > 0);

    size_t unsignedSize = UnsignedCast(nameLength);
    return gethostname(reinterpret_cast<char*>(name), unsignedSize);
}

template <typename TType, typename TField>
static bool IsInBounds(const TType* base, size_t len, const TField* value)
{
    auto* baseAddr = reinterpret_cast<const uint8_t*>(base);
    auto* valueAddr = reinterpret_cast<const uint8_t*>(value);
    return valueAddr >= baseAddr && (valueAddr + sizeof(TField)) <= (baseAddr + len);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetIPSocketAddressSizes(int32_t* ipv4SocketAddressSize, int32_t* ipv6SocketAddressSize)
{
    return SystemNative_GetIPSocketAddressSizes(ipv4SocketAddressSize, ipv6SocketAddressSize);
}

extern "C" Error SystemNative_GetIPSocketAddressSizes(int32_t* ipv4SocketAddressSize, int32_t* ipv6SocketAddressSize)
{
    if (ipv4SocketAddressSize == nullptr || ipv6SocketAddressSize == nullptr)
    {
        return PAL_EFAULT;
    }

    *ipv4SocketAddressSize = sizeof(sockaddr_in);
    *ipv6SocketAddressSize = sizeof(sockaddr_in6);
    return PAL_SUCCESS;
}

static bool TryConvertAddressFamilyPlatformToPal(sa_family_t platformAddressFamily, int32_t* palAddressFamily)
{
    assert(palAddressFamily != nullptr);

    switch (platformAddressFamily)
    {
        case AF_UNSPEC:
            *palAddressFamily = PAL_AF_UNSPEC;
            return true;

        case AF_UNIX:
            *palAddressFamily = PAL_AF_UNIX;
            return true;

        case AF_INET:
            *palAddressFamily = PAL_AF_INET;
            return true;

        case AF_INET6:
            *palAddressFamily = PAL_AF_INET6;
            return true;

        default:
            *palAddressFamily = platformAddressFamily;
            return false;
    }
}

static bool TryConvertAddressFamilyPalToPlatform(int32_t palAddressFamily, sa_family_t* platformAddressFamily)
{
    assert(platformAddressFamily != nullptr);

    switch (palAddressFamily)
    {
        case PAL_AF_UNSPEC:
            *platformAddressFamily = AF_UNSPEC;
            return true;

        case PAL_AF_UNIX:
            *platformAddressFamily = AF_UNIX;
            return true;

        case PAL_AF_INET:
            *platformAddressFamily = AF_INET;
            return true;

        case PAL_AF_INET6:
            *platformAddressFamily = AF_INET6;
            return true;

        default:
            *platformAddressFamily = static_cast<sa_family_t>(palAddressFamily);
            return false;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetAddressFamily(const uint8_t* socketAddress, int32_t socketAddressLen, int32_t* addressFamily)
{
    return SystemNative_GetAddressFamily(socketAddress, socketAddressLen, addressFamily);
}

extern "C" Error SystemNative_GetAddressFamily(const uint8_t* socketAddress, int32_t socketAddressLen, int32_t* addressFamily)
{
    if (socketAddress == nullptr || addressFamily == nullptr || socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<const sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (!TryConvertAddressFamilyPlatformToPal(sockAddr->sa_family, addressFamily))
    {
        return PAL_EAFNOSUPPORT;
    }

    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetAddressFamily(uint8_t* socketAddress, int32_t socketAddressLen, int32_t addressFamily)
{
    return SystemNative_SetAddressFamily(socketAddress, socketAddressLen, addressFamily);
}

extern "C" Error SystemNative_SetAddressFamily(uint8_t* socketAddress, int32_t socketAddressLen, int32_t addressFamily)
{
    auto* sockAddr = reinterpret_cast<sockaddr*>(socketAddress);
    if (sockAddr == nullptr || socketAddressLen < 0 ||
        !IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (!TryConvertAddressFamilyPalToPlatform(addressFamily, &sockAddr->sa_family))
    {
        return PAL_EAFNOSUPPORT;
    }

    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetPort(const uint8_t* socketAddress, int32_t socketAddressLen, uint16_t* port)
{
    return SystemNative_GetPort(socketAddress, socketAddressLen, port);
}

extern "C" Error SystemNative_GetPort(const uint8_t* socketAddress, int32_t socketAddressLen, uint16_t* port)
{
    if (socketAddress == nullptr)
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<const sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    switch (sockAddr->sa_family)
    {
        case AF_INET:
        {
            if (socketAddressLen < 0 || static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in))
            {
                return PAL_EFAULT;
            }

            *port = ntohs(reinterpret_cast<const sockaddr_in*>(socketAddress)->sin_port);
            return PAL_SUCCESS;
        }

        case AF_INET6:
        {
            if (socketAddressLen < 0 || static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in6))
            {
                return PAL_EFAULT;
            }

            *port = ntohs(reinterpret_cast<const sockaddr_in6*>(socketAddress)->sin6_port);
            return PAL_SUCCESS;
        }

        default:
            return PAL_EAFNOSUPPORT;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetPort(uint8_t* socketAddress, int32_t socketAddressLen, uint16_t port)
{
    return SystemNative_SetPort(socketAddress, socketAddressLen, port);
}

extern "C" Error SystemNative_SetPort(uint8_t* socketAddress, int32_t socketAddressLen, uint16_t port)
{
    if (socketAddress == nullptr)
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<const sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    switch (sockAddr->sa_family)
    {
        case AF_INET:
        {
            if (socketAddressLen < 0 || static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in))
            {
                return PAL_EFAULT;
            }

            reinterpret_cast<sockaddr_in*>(socketAddress)->sin_port = htons(port);
            return PAL_SUCCESS;
        }

        case AF_INET6:
        {
            if (socketAddressLen < 0 || static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in6))
            {
                return PAL_EFAULT;
            }

            reinterpret_cast<sockaddr_in6*>(socketAddress)->sin6_port = htons(port);
            return PAL_SUCCESS;
        }

        default:
            return PAL_EAFNOSUPPORT;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetIPv4Address(const uint8_t* socketAddress, int32_t socketAddressLen, uint32_t* address)
{
    return SystemNative_GetIPv4Address(socketAddress, socketAddressLen, address);
}

extern "C" Error SystemNative_GetIPv4Address(const uint8_t* socketAddress, int32_t socketAddressLen, uint32_t* address)
{
    if (socketAddress == nullptr || address == nullptr || socketAddressLen < 0 ||
        static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in))
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<const sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (sockAddr->sa_family != AF_INET)
    {
        return PAL_EINVAL;
    }

    *address = reinterpret_cast<const sockaddr_in*>(socketAddress)->sin_addr.s_addr;
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetIPv4Address(uint8_t* socketAddress, int32_t socketAddressLen, uint32_t address)
{
    return SystemNative_SetIPv4Address(socketAddress, socketAddressLen, address);
}

extern "C" Error SystemNative_SetIPv4Address(uint8_t* socketAddress, int32_t socketAddressLen, uint32_t address)
{
    if (socketAddress == nullptr || socketAddressLen < 0 || static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in))
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (sockAddr->sa_family != AF_INET)
    {
        return PAL_EINVAL;
    }

    auto* inetSockAddr = reinterpret_cast<sockaddr_in*>(sockAddr);

    inetSockAddr->sin_family = AF_INET;
    inetSockAddr->sin_addr.s_addr = address;
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetIPv6Address(
    const uint8_t* socketAddress, int32_t socketAddressLen, uint8_t* address, int32_t addressLen, uint32_t* scopeId)
{
    return SystemNative_GetIPv6Address(socketAddress, socketAddressLen, address, addressLen, scopeId);
}

extern "C" Error SystemNative_GetIPv6Address(
    const uint8_t* socketAddress, int32_t socketAddressLen, uint8_t* address, int32_t addressLen, uint32_t* scopeId)
{
    if (socketAddress == nullptr || address == nullptr || scopeId == nullptr || socketAddressLen < 0 ||
        static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in6) || addressLen < NUM_BYTES_IN_IPV6_ADDRESS)
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<const sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (sockAddr->sa_family != AF_INET6)
    {
        return PAL_EINVAL;
    }

    auto* inet6SockAddr = reinterpret_cast<const sockaddr_in6*>(sockAddr);
    ConvertIn6AddrToByteArray(address, addressLen, inet6SockAddr->sin6_addr);
    *scopeId = inet6SockAddr->sin6_scope_id;

    return PAL_SUCCESS;
}

extern "C" Error
SystemNative_SetIPv6Address(uint8_t* socketAddress, int32_t socketAddressLen, uint8_t* address, int32_t addressLen, uint32_t scopeId)
{
    if (socketAddress == nullptr || address == nullptr || socketAddressLen < 0 ||
        static_cast<size_t>(socketAddressLen) < sizeof(sockaddr_in6) || addressLen < NUM_BYTES_IN_IPV6_ADDRESS)
    {
        return PAL_EFAULT;
    }

    auto* sockAddr = reinterpret_cast<sockaddr*>(socketAddress);
    if (!IsInBounds(sockAddr, static_cast<size_t>(socketAddressLen), &sockAddr->sa_family))
    {
        return PAL_EFAULT;
    }

    if (sockAddr->sa_family != AF_INET6)
    {
        return PAL_EINVAL;
    }

    auto* inet6SockAddr = reinterpret_cast<sockaddr_in6*>(sockAddr);
    ConvertByteArrayToSockAddrIn6(*inet6SockAddr, address, addressLen);
    inet6SockAddr->sin6_family = AF_INET6;
    inet6SockAddr->sin6_flowinfo = 0;
    inet6SockAddr->sin6_scope_id = scopeId;

    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetIPv6Address(uint8_t* socketAddress, int32_t socketAddressLen, uint8_t* address, int32_t addressLen, uint32_t scopeId)
{
    return SystemNative_SetIPv6Address(socketAddress, socketAddressLen, address, addressLen, scopeId);
}

static void ConvertMessageHeaderToMsghdr(msghdr* header, const MessageHeader& messageHeader)
{
    *header = {
        .msg_name = messageHeader.SocketAddress,
        .msg_namelen = static_cast<unsigned int>(messageHeader.SocketAddressLen),
        .msg_iov = reinterpret_cast<iovec*>(messageHeader.IOVectors),
        .msg_iovlen = static_cast<decltype(header->msg_iovlen)>(messageHeader.IOVectorCount),
        .msg_control = messageHeader.ControlBuffer,
        .msg_controllen = static_cast<decltype(header->msg_controllen)>(messageHeader.ControlBufferLen),
    };
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetControlMessageBufferSize(int32_t isIPv4, int32_t isIPv6)
{
    return SystemNative_GetControlMessageBufferSize(isIPv4, isIPv6);
}

extern "C" int32_t SystemNative_GetControlMessageBufferSize(int32_t isIPv4, int32_t isIPv6)
{
    // Note: it is possible that the address family of the socket is neither
    //       AF_INET nor AF_INET6. In this case both inputs will be false and
    //       the controll message buffer size should be zero.
    return (isIPv4 != 0 ? CMSG_SPACE(sizeof(in_pktinfo)) : 0) + (isIPv6 != 0 ? CMSG_SPACE(sizeof(in6_pktinfo)) : 0);
}

static int32_t GetIPv4PacketInformation(cmsghdr* controlMessage, IPPacketInformation* packetInfo)
{
    assert(controlMessage != nullptr);
    assert(packetInfo != nullptr);

    if (controlMessage->cmsg_len < sizeof(in_pktinfo))
    {
        assert(false && "expected a control message large enough to hold an in_pktinfo value");
        return 0;
    }

    auto* pktinfo = reinterpret_cast<in_pktinfo*>(CMSG_DATA(controlMessage));
    ConvertInAddrToByteArray(&packetInfo->Address.Address[0], NUM_BYTES_IN_IPV4_ADDRESS, pktinfo->ipi_addr);
#if HAVE_IN_PKTINFO
    packetInfo->InterfaceIndex = static_cast<int32_t>(pktinfo->ipi_ifindex);
#else
    // TODO: Figure out how to get interface index with in_addr.
    // One option is http://www.unix.com/man-page/freebsd/3/if_nametoindex
    // which requires interface name to be known.
    // Meanwhile:
    packetInfo->InterfaceIndex = 0;
#endif

    return 1;
}

static int32_t GetIPv6PacketInformation(cmsghdr* controlMessage, IPPacketInformation* packetInfo)
{
    assert(controlMessage != nullptr);
    assert(packetInfo != nullptr);

    if (controlMessage->cmsg_len < sizeof(in6_pktinfo))
    {
        assert(false && "expected a control message large enough to hold an in6_pktinfo value");
        return 0;
    }

    auto* pktinfo = reinterpret_cast<in6_pktinfo*>(CMSG_DATA(controlMessage));
    ConvertIn6AddrToByteArray(&packetInfo->Address.Address[0], NUM_BYTES_IN_IPV6_ADDRESS, pktinfo->ipi6_addr);
    packetInfo->Address.IsIPv6 = 1;
    packetInfo->InterfaceIndex = static_cast<int32_t>(pktinfo->ipi6_ifindex);

    return 1;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t TryGetIPPacketInformation(MessageHeader* messageHeader, int32_t isIPv4, IPPacketInformation* packetInfo)
{
    return SystemNative_TryGetIPPacketInformation(messageHeader, isIPv4, packetInfo);
}

extern "C" int32_t
SystemNative_TryGetIPPacketInformation(MessageHeader* messageHeader, int32_t isIPv4, IPPacketInformation* packetInfo)
{
    if (messageHeader == nullptr || packetInfo == nullptr)
    {
        return 0;
    }

    msghdr header;
    ConvertMessageHeaderToMsghdr(&header, *messageHeader);

    cmsghdr* controlMessage = CMSG_FIRSTHDR(&header);
    if (isIPv4 != 0)
    {
        for (; controlMessage != nullptr && controlMessage->cmsg_len > 0;
             controlMessage = CMSG_NXTHDR(&header, controlMessage))
        {
            if (controlMessage->cmsg_level == IPPROTO_IP && controlMessage->cmsg_type == IP_PKTINFO)
            {
                return GetIPv4PacketInformation(controlMessage, packetInfo);
            }
        }
    }
    else
    {
        for (; controlMessage != nullptr && controlMessage->cmsg_len > 0;
             controlMessage = CMSG_NXTHDR(&header, controlMessage))
        {
            if (controlMessage->cmsg_level == IPPROTO_IPV6 && controlMessage->cmsg_type == IPV6_PKTINFO)
            {
                return GetIPv6PacketInformation(controlMessage, packetInfo);
            }
        }
    }

    return 0;
}

static bool GetMulticastOptionName(int32_t multicastOption, bool isIPv6, int& optionName)
{
    switch (multicastOption)
    {
        case PAL_MULTICAST_ADD:
            optionName = isIPv6 ? IPV6_ADD_MEMBERSHIP : IP_ADD_MEMBERSHIP;
            return true;

        case PAL_MULTICAST_DROP:
            optionName = isIPv6 ? IPV6_DROP_MEMBERSHIP : IP_DROP_MEMBERSHIP;
            return true;

        default:
            return false;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option)
{
    return SystemNative_GetIPv4MulticastOption(socket, multicastOption, option);
}

extern "C" Error SystemNative_GetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    int optionName;
    if (!GetMulticastOptionName(multicastOption, false, optionName))
    {
        return PAL_EINVAL;
    }

    ip_mreqn opt;
    socklen_t len = sizeof(opt);
    int err = getsockopt(socket, IPPROTO_IP, optionName, &opt, &len);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    *option = {.MulticastAddress = opt.imr_multiaddr.s_addr,
               .LocalAddress = opt.imr_address.s_addr,
               .InterfaceIndex = opt.imr_ifindex};
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option)
{
    return SystemNative_SetIPv4MulticastOption(socket, multicastOption, option);
}

extern "C" Error SystemNative_SetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    int optionName;
    if (!GetMulticastOptionName(multicastOption, false, optionName))
    {
        return PAL_EINVAL;
    }

    ip_mreqn opt = {.imr_multiaddr = {.s_addr = option->MulticastAddress},
                    .imr_address = {.s_addr = option->LocalAddress},
                    .imr_ifindex = option->InterfaceIndex};
    int err = setsockopt(socket, IPPROTO_IP, optionName, &opt, sizeof(opt));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option)
{
    return SystemNative_GetIPv6MulticastOption(socket, multicastOption, option);
}

extern "C" Error SystemNative_GetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    int optionName;
    if (!GetMulticastOptionName(multicastOption, false, optionName))
    {
        return PAL_EINVAL;
    }

    ipv6_mreq opt;
    socklen_t len = sizeof(opt);
    int err = getsockopt(socket, IPPROTO_IP, optionName, &opt, &len);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    ConvertIn6AddrToByteArray(&option->Address.Address[0], NUM_BYTES_IN_IPV6_ADDRESS, opt.ipv6mr_multiaddr);
    option->InterfaceIndex = static_cast<int32_t>(opt.ipv6mr_interface);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option)
{
    return SystemNative_SetIPv6MulticastOption(socket, multicastOption, option);
}

extern "C" Error SystemNative_SetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    int optionName;
    if (!GetMulticastOptionName(multicastOption, false, optionName))
    {
        return PAL_EINVAL;
    }

    ipv6_mreq opt = {.ipv6mr_interface = static_cast<unsigned int>(option->InterfaceIndex)};
    ConvertByteArrayToIn6Addr(opt.ipv6mr_multiaddr, &option->Address.Address[0], NUM_BYTES_IN_IPV6_ADDRESS);

    int err = setsockopt(socket, IPPROTO_IP, optionName, &opt, sizeof(opt));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

#if defined(__APPLE__) && __APPLE__
static int32_t GetMaxLingerTime()
{
    static volatile int32_t MaxLingerTime = -1;
    static_assert(sizeof(xsocket::so_linger) == 2, "");

    // OS X does not define the linger time in seconds by default, but in ticks.
    // Furthermore, when SO_LINGER_SEC is used, the value is simply scaled by
    // the number of ticks per second and then the result is used to set the
    // underlying linger time. Unfortunately, the underlying linger time is
    // stored as a `short` and out-of-range values are simply truncated to fit
    // within 16 bits and then reinterpreted as 2's complement signed integers.
    // This results in some *very* strange behavior and a rather low limit for
    // the linger time. Instead of admitting this behavior, we determine the
    // maximum linger time in seconds and return an error if the input is out
    // of range.
    int32_t maxLingerTime = MaxLingerTime;
    if (maxLingerTime == -1)
    {
        long ticksPerSecond = sysconf(_SC_CLK_TCK);
        maxLingerTime = static_cast<int32_t>(32767 / ticksPerSecond);
        MaxLingerTime = maxLingerTime;
    }

    return maxLingerTime;
}
#else
constexpr int32_t GetMaxLingerTime()
{
    // On other platforms, the maximum linger time is locked to the smaller of
    // 65535 (the maximum time for winsock) and the maximum signed value that
    // will fit in linger::l_linger.

    return Min(65535U, (1U << (sizeof(linger::l_linger) * 8 - 1)) - 1);
}
#endif

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetLingerOption(int32_t socket, LingerOption* option)
{
    return SystemNative_GetLingerOption(socket, option);
}

extern "C" Error SystemNative_GetLingerOption(int32_t socket, LingerOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    linger opt;
    socklen_t len = sizeof(opt);
    int err = getsockopt(socket, SOL_SOCKET, LINGER_OPTION_NAME, &opt, &len);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    *option = {.OnOff = opt.l_onoff, .Seconds = opt.l_linger};
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetLingerOption(int32_t socket, LingerOption* option)
{
    return SystemNative_SetLingerOption(socket, option);
}

extern "C" Error SystemNative_SetLingerOption(int32_t socket, LingerOption* option)
{
    if (option == nullptr)
    {
        return PAL_EFAULT;
    }

    if (option->OnOff != 0 && (option->Seconds < 0 || option->Seconds > GetMaxLingerTime()))
    {
        return PAL_EINVAL;
    }

    linger opt = {.l_onoff = option->OnOff, .l_linger = option->Seconds};
    int err = setsockopt(socket, SOL_SOCKET, LINGER_OPTION_NAME, &opt, sizeof(opt));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static bool ConvertSocketFlagsPalToPlatform(int32_t palFlags, int& platformFlags)
{
    const int32_t SupportedFlagsMask = PAL_MSG_OOB | PAL_MSG_PEEK | PAL_MSG_DONTROUTE | PAL_MSG_TRUNC | PAL_MSG_CTRUNC;

    if ((palFlags & ~SupportedFlagsMask) != 0)
    {
        // TODO: we may want to simply mask off unsupported flags.
        return false;
    }

    platformFlags = ((palFlags & PAL_MSG_OOB) == 0 ? 0 : MSG_OOB) | ((palFlags & PAL_MSG_PEEK) == 0 ? 0 : MSG_PEEK) |
                    ((palFlags & PAL_MSG_DONTROUTE) == 0 ? 0 : MSG_DONTROUTE) |
                    ((palFlags & PAL_MSG_TRUNC) == 0 ? 0 : MSG_TRUNC) |
                    ((palFlags & PAL_MSG_CTRUNC) == 0 ? 0 : MSG_CTRUNC);

    return true;
}

static int32_t ConvertSocketFlagsPlatformToPal(int platformFlags)
{
    const int SupportedFlagsMask = MSG_OOB | MSG_PEEK | MSG_DONTROUTE | MSG_TRUNC | MSG_CTRUNC;

    platformFlags &= SupportedFlagsMask;

    return ((platformFlags & MSG_OOB) == 0 ? 0 : PAL_MSG_OOB) | ((platformFlags & MSG_PEEK) == 0 ? 0 : PAL_MSG_PEEK) |
           ((platformFlags & MSG_DONTROUTE) == 0 ? 0 : PAL_MSG_DONTROUTE) |
           ((platformFlags & MSG_TRUNC) == 0 ? 0 : PAL_MSG_TRUNC) |
           ((platformFlags & MSG_CTRUNC) == 0 ? 0 : PAL_MSG_CTRUNC);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error ReceiveMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* received)
{
    return SystemNative_ReceiveMessage(socket, messageHeader, flags, received);
}

extern "C" Error SystemNative_ReceiveMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* received)
{
    if (messageHeader == nullptr || received == nullptr || messageHeader->SocketAddressLen < 0 ||
        messageHeader->ControlBufferLen < 0 || messageHeader->IOVectorCount < 0)
    {
        return PAL_EFAULT;
    }

    int socketFlags;
    if (!ConvertSocketFlagsPalToPlatform(flags, socketFlags))
    {
        return PAL_ENOTSUP;
    }

    msghdr header;
    ConvertMessageHeaderToMsghdr(&header, *messageHeader);

    ssize_t res = recvmsg(socket, &header, socketFlags);

    assert(static_cast<int32_t>(header.msg_namelen) <= messageHeader->SocketAddressLen);
    messageHeader->SocketAddressLen = Min(static_cast<int32_t>(header.msg_namelen), messageHeader->SocketAddressLen);
    memcpy(messageHeader->SocketAddress, header.msg_name, static_cast<size_t>(messageHeader->SocketAddressLen));

    assert(header.msg_controllen <= static_cast<size_t>(messageHeader->ControlBufferLen));
    messageHeader->ControlBufferLen = Min(static_cast<int32_t>(header.msg_controllen), messageHeader->ControlBufferLen);
    memcpy(messageHeader->ControlBuffer, header.msg_control, static_cast<size_t>(messageHeader->ControlBufferLen));

    messageHeader->Flags = ConvertSocketFlagsPlatformToPal(header.msg_flags);

    if (res != -1)
    {
        *received = res;
        return PAL_SUCCESS;
    }

    *received = 0;
    return SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SendMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* sent)
{
    return SystemNative_SendMessage(socket, messageHeader, flags, sent);
}

extern "C" Error SystemNative_SendMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* sent)
{
    if (messageHeader == nullptr || sent == nullptr || messageHeader->SocketAddressLen < 0 ||
        messageHeader->ControlBufferLen < 0 || messageHeader->IOVectorCount < 0)
    {
        return PAL_EFAULT;
    }

    int socketFlags;
    if (!ConvertSocketFlagsPalToPlatform(flags, socketFlags))
    {
        return PAL_ENOTSUP;
    }

    msghdr header;
    ConvertMessageHeaderToMsghdr(&header, *messageHeader);

    ssize_t res = sendmsg(socket, &header, flags);
    if (res != -1)
    {
        *sent = res;
        return PAL_SUCCESS;
    }

    *sent = 0;
    return SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Accept(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen, int32_t* acceptedSocket)
{
    return SystemNative_Accept(socket, socketAddress, socketAddressLen, acceptedSocket);
}

extern "C" Error SystemNative_Accept(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen, int32_t* acceptedSocket)
{
    if (socketAddress == nullptr || socketAddressLen == nullptr || acceptedSocket == nullptr || *socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    socklen_t addrLen = static_cast<socklen_t>(*socketAddressLen);
    int accepted = accept(socket, reinterpret_cast<sockaddr*>(socketAddress), &addrLen);
    if (accepted == -1)
    {
        *acceptedSocket = -1;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    assert(addrLen <= static_cast<socklen_t>(*socketAddressLen));
    *socketAddressLen = static_cast<int32_t>(addrLen);
    *acceptedSocket = accepted;
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Bind(int32_t socket, uint8_t* socketAddress, int32_t socketAddressLen)
{
    return SystemNative_Bind(socket, socketAddress, socketAddressLen);
}

extern "C" Error SystemNative_Bind(int32_t socket, uint8_t* socketAddress, int32_t socketAddressLen)
{
    if (socketAddress == nullptr || socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    int err = bind(socket, reinterpret_cast<sockaddr*>(socketAddress), static_cast<socklen_t>(socketAddressLen));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Connect(int32_t socket, uint8_t* socketAddress, int32_t socketAddressLen)
{
    return SystemNative_Connect(socket, socketAddress, socketAddressLen);
}

extern "C" Error SystemNative_Connect(int32_t socket, uint8_t* socketAddress, int32_t socketAddressLen)
{
    if (socketAddress == nullptr || socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    int err = connect(socket, reinterpret_cast<sockaddr*>(socketAddress), static_cast<socklen_t>(socketAddressLen));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetPeerName(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen)
{
    return SystemNative_GetPeerName(socket, socketAddress, socketAddressLen);
}

extern "C" Error SystemNative_GetPeerName(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen)
{
    if (socketAddress == nullptr || socketAddressLen == nullptr || *socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    socklen_t addrLen = static_cast<socklen_t>(*socketAddressLen);
    int err = getpeername(socket, reinterpret_cast<sockaddr*>(socketAddress), &addrLen);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    assert(addrLen <= static_cast<socklen_t>(*socketAddressLen));
    *socketAddressLen = static_cast<int32_t>(addrLen);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetSockName(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen)
{
    return SystemNative_GetSockName(socket, socketAddress, socketAddressLen);
}

extern "C" Error SystemNative_GetSockName(int32_t socket, uint8_t* socketAddress, int32_t* socketAddressLen)
{
    if (socketAddress == nullptr || socketAddressLen == nullptr || *socketAddressLen < 0)
    {
        return PAL_EFAULT;
    }

    socklen_t addrLen = static_cast<socklen_t>(*socketAddressLen);
    int err = getsockname(socket, reinterpret_cast<sockaddr*>(socketAddress), &addrLen);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    assert(addrLen <= static_cast<socklen_t>(*socketAddressLen));
    *socketAddressLen = static_cast<int32_t>(addrLen);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Listen(int32_t socket, int32_t backlog)
{
    return SystemNative_Listen(socket, backlog);
}

extern "C" Error SystemNative_Listen(int32_t socket, int32_t backlog)
{
    int err = listen(socket, backlog);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Shutdown(int32_t socket, int32_t socketShutdown)
{
    return SystemNative_Shutdown(socket, socketShutdown);
}

extern "C" Error SystemNative_Shutdown(int32_t socket, int32_t socketShutdown)
{
    int how;
    switch (socketShutdown)
    {
        case PAL_SHUT_READ:
            how = SHUT_RD;
            break;

        case PAL_SHUT_WRITE:
            how = SHUT_WR;
            break;

        case PAL_SHUT_BOTH:
            how = SHUT_RDWR;
            break;

        default:
            return PAL_EINVAL;
    }

    int err = shutdown(socket, how);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetSocketErrorOption(int32_t socket, Error* error)
{
    return SystemNative_GetSocketErrorOption(socket, error);
}

extern "C" Error SystemNative_GetSocketErrorOption(int32_t socket, Error* error)
{
    if (error == nullptr)
    {
        return PAL_EFAULT;
    }

    int socketErrno;
    socklen_t optLen = sizeof(socketErrno);
    int err = getsockopt(socket, SOL_SOCKET, SO_ERROR, &socketErrno, &optLen);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    assert(optLen == sizeof(socketErrno));
    *error = SystemNative_ConvertErrorPlatformToPal(socketErrno);
    return PAL_SUCCESS;
}

static bool TryGetPlatformSocketOption(int32_t socketOptionName, int32_t socketOptionLevel, int& optLevel, int& optName)
{
    switch (socketOptionName)
    {
        case PAL_SOL_SOCKET:
            optLevel = SOL_SOCKET;

            switch (socketOptionLevel)
            {
                case PAL_SO_DEBUG:
                    optName = SO_DEBUG;
                    return true;

                case PAL_SO_ACCEPTCONN:
                    optName = SO_ACCEPTCONN;
                    return true;

                case PAL_SO_REUSEADDR:
                    optName = SO_REUSEADDR;
                    return true;

                case PAL_SO_KEEPALIVE:
                    optName = SO_KEEPALIVE;
                    return true;

                case PAL_SO_DONTROUTE:
                    optName = SO_DONTROUTE;
                    return true;

                case PAL_SO_BROADCAST:
                    optName = SO_BROADCAST;
                    return true;

                // case PAL_SO_USELOOPBACK:

                case PAL_SO_LINGER:
                    optName = SO_LINGER;
                    return true;

                case PAL_SO_OOBINLINE:
                    optName = SO_OOBINLINE;
                    return true;

                // case PAL_SO_DONTLINGER:

                // case PAL_SO_EXCLUSIVEADDRUSE:

                case PAL_SO_SNDBUF:
                    optName = SO_SNDBUF;
                    return true;

                case PAL_SO_RCVBUF:
                    optName = SO_RCVBUF;
                    return true;

                case PAL_SO_SNDLOWAT:
                    optName = SO_SNDLOWAT;
                    return true;

                case PAL_SO_RCVLOWAT:
                    optName = SO_RCVLOWAT;
                    return true;

                case PAL_SO_SNDTIMEO:
                    optName = SO_SNDTIMEO;
                    return true;

                case PAL_SO_RCVTIMEO:
                    optName = SO_RCVTIMEO;
                    return true;

                case PAL_SO_ERROR:
                    optName = SO_ERROR;
                    return true;

                case PAL_SO_TYPE:
                    optName = SO_TYPE;
                    return true;

                // case PAL_SO_MAXCONN:

                default:
                    return false;
            }

        case PAL_SOL_IP:
            optLevel = IPPROTO_IP;

            switch (socketOptionLevel)
            {
                case PAL_SO_IP_OPTIONS:
                    optName = IP_OPTIONS;
                    return true;

                case PAL_SO_IP_HDRINCL:
                    optName = IP_HDRINCL;
                    return true;

                case PAL_SO_IP_TOS:
                    optName = IP_TOS;
                    return true;

                case PAL_SO_IP_TTL:
                    optName = IP_TTL;
                    return true;

                case PAL_SO_IP_MULTICAST_IF:
                    optName = IP_MULTICAST_IF;
                    return true;

                case PAL_SO_IP_MULTICAST_TTL:
                    optName = IP_MULTICAST_TTL;
                    return true;

                case PAL_SO_IP_MULTICAST_LOOP:
                    optName = IP_MULTICAST_LOOP;
                    return true;

                case PAL_SO_IP_ADD_MEMBERSHIP:
                    optName = IP_ADD_MEMBERSHIP;
                    return true;

                case PAL_SO_IP_DROP_MEMBERSHIP:
                    optName = IP_DROP_MEMBERSHIP;
                    return true;

                // case PAL_SO_IP_DONTFRAGMENT:

                case PAL_SO_IP_ADD_SOURCE_MEMBERSHIP:
                    optName = IP_ADD_SOURCE_MEMBERSHIP;
                    return true;

                case PAL_SO_IP_DROP_SOURCE_MEMBERSHIP:
                    optName = IP_DROP_SOURCE_MEMBERSHIP;
                    return true;

                case PAL_SO_IP_BLOCK_SOURCE:
                    optName = IP_BLOCK_SOURCE;
                    return true;

                case PAL_SO_IP_UNBLOCK_SOURCE:
                    optName = IP_UNBLOCK_SOURCE;
                    return true;

                case PAL_SO_IP_PKTINFO:
                    optName = IP_PKTINFO;
                    return true;

                default:
                    return false;
            }

        case PAL_SOL_IPV6:
            optLevel = IPPROTO_IPV6;

            switch (socketOptionLevel)
            {
                case PAL_SO_IPV6_HOPLIMIT:
                    optName = IPV6_HOPLIMIT;
                    return true;

                // case PAL_SO_IPV6_PROTECTION_LEVEL:

                case PAL_SO_IPV6_V6ONLY:
                    optName = IPV6_V6ONLY;
                    return true;

                case PAL_SO_IP_PKTINFO:
                    optName = IPV6_RECVPKTINFO;
                    return true;

                default:
                    return false;
            }

        case PAL_SOL_TCP:
            optLevel = IPPROTO_TCP;

            switch (socketOptionLevel)
            {
                case PAL_SO_TCP_NODELAY:
                    optName = TCP_NODELAY;
                    return true;

                // case PAL_SO_TCP_BSDURGENT:

                default:
                    return false;
            }

        case PAL_SOL_UDP:
            optLevel = IPPROTO_UDP;

            switch (socketOptionLevel)
            {
                // case PAL_SO_UDP_NOCHECKSUM:

                // case PAL_SO_UDP_CHECKSUM_COVERAGE:

                // case PAL_SO_UDP_UPDATEACCEPTCONTEXT:

                // case PAL_SO_UDP_UPDATECONNECTCONTEXT:

                default:
                    return false;
            }

        default:
            return false;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetSockOpt(int32_t socket, int32_t socketOptionLevel, int32_t socketOptionName, uint8_t* optionValue, int32_t* optionLen)
{
    return SystemNative_GetSockOpt(socket, socketOptionLevel, socketOptionName, optionValue, optionLen);
}

extern "C" Error SystemNative_GetSockOpt(
    int32_t socket, int32_t socketOptionLevel, int32_t socketOptionName, uint8_t* optionValue, int32_t* optionLen)
{
    if (optionLen == nullptr || *optionLen < 0)
    {
        return PAL_EFAULT;
    }

    int optLevel, optName;
    if (!TryGetPlatformSocketOption(socketOptionLevel, socketOptionName, optLevel, optName))
    {
        return PAL_ENOTSUP;
    }

    auto optLen = static_cast<socklen_t>(*optionLen);
    int err = getsockopt(socket, optLevel, optName, optionValue, &optLen);
    if (err != 0)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    assert(optLen <= static_cast<socklen_t>(*optionLen));
    *optionLen = static_cast<int32_t>(optLen);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error SetSockOpt(int32_t socket, int32_t socketOptionLevel, int32_t socketOptionName, uint8_t* optionValue, int32_t optionLen)
{
    return SystemNative_SetSockOpt(socket, socketOptionLevel, socketOptionName, optionValue, optionLen);
}

extern "C" Error
SystemNative_SetSockOpt(int32_t socket, int32_t socketOptionLevel, int32_t socketOptionName, uint8_t* optionValue, int32_t optionLen)
{
    if (optionLen < 0)
    {
        return PAL_EFAULT;
    }

    int optLevel, optName;
    if (!TryGetPlatformSocketOption(socketOptionLevel, socketOptionName, optLevel, optName))
    {
        return PAL_ENOTSUP;
    }

    int err = setsockopt(socket, optLevel, optName, optionValue, static_cast<socklen_t>(optionLen));
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static bool TryConvertSocketTypePalToPlatform(int32_t palSocketType, int* platformSocketType)
{
    assert(platformSocketType != nullptr);

    switch (palSocketType)
    {
        case PAL_SOCK_STREAM:
            *platformSocketType = SOCK_STREAM;
            return true;

        case PAL_SOCK_DGRAM:
            *platformSocketType = SOCK_DGRAM;
            return true;

        case PAL_SOCK_RAW:
            *platformSocketType = SOCK_RAW;
            return true;

        case PAL_SOCK_RDM:
            *platformSocketType = SOCK_RDM;
            return true;

        case PAL_SOCK_SEQPACKET:
            *platformSocketType = SOCK_SEQPACKET;
            return true;

        default:
            *platformSocketType = static_cast<int>(palSocketType);
            return false;
    }
}

static bool TryConvertProtocolTypePalToPlatform(int32_t palProtocolType, int* platformProtocolType)
{
    assert(platformProtocolType != nullptr);

    switch (palProtocolType)
    {
        case PAL_PT_UNSPECIFIED:
            *platformProtocolType = 0;
            return true;

        case PAL_PT_ICMP:
            *platformProtocolType = IPPROTO_ICMP;
            return true;

        case PAL_PT_TCP:
            *platformProtocolType = IPPROTO_TCP;
            return true;

        case PAL_PT_UDP:
            *platformProtocolType = IPPROTO_UDP;
            return true;

        case PAL_PT_ICMPV6:
            *platformProtocolType = IPPROTO_ICMPV6;
            return true;

        default:
            *platformProtocolType = static_cast<int>(palProtocolType);
            return false;
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Socket(int32_t addressFamily, int32_t socketType, int32_t protocolType, int32_t* createdSocket)
{
    return SystemNative_Socket(addressFamily, socketType, protocolType, createdSocket);
}

extern "C" Error SystemNative_Socket(int32_t addressFamily, int32_t socketType, int32_t protocolType, int32_t* createdSocket)
{
    if (createdSocket == nullptr)
    {
        return PAL_EFAULT;
    }

    sa_family_t platformAddressFamily;
    int platformSocketType, platformProtocolType;

    if (!TryConvertAddressFamilyPalToPlatform(addressFamily, &platformAddressFamily))
    {
        *createdSocket = -1;
        return PAL_EAFNOSUPPORT;
    }

    if (!TryConvertSocketTypePalToPlatform(socketType, &platformSocketType))
    {
        *createdSocket = -1;
        return PAL_EPROTOTYPE;
    }

    if (!TryConvertProtocolTypePalToPlatform(protocolType, &platformProtocolType))
    {
        *createdSocket = -1;
        return PAL_EPROTONOSUPPORT;
    }

    *createdSocket = socket(platformAddressFamily, platformSocketType, platformProtocolType);
    return *createdSocket != -1 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

const int FD_SETSIZE_BYTES = FD_SETSIZE / 8;

#if !HAVE_FDS_BITS && !HAVE_PRIVATE_FDS_BITS
const int FD_SETSIZE_UINTS = FD_SETSIZE_BYTES / sizeof(uint32_t);
#endif

static void ConvertFdSetPlatformToPal(uint32_t* palSet, fd_set& platformSet, int32_t fdCount)
{
    assert(fdCount >= 0);

    memset(palSet, 0, FD_SETSIZE_BYTES);

#if !HAVE_FDS_BITS && !HAVE_PRIVATE_FDS_BITS
    for (int i = 0; i < fdCount; i++)
    {
        uint32_t* word = &palSet[i / FD_SETSIZE_UINTS];
        uint32_t mask = 1 << (i % FD_SETSIZE_UINTS);

        if (FD_ISSET(i, &platformSet))
        {
            *word |= mask;
        }
        else
        {
            *word &= ~mask;
        }
    }
#else
    size_t bytesToCopy = static_cast<size_t>((fdCount / 8) + ((fdCount % 8) != 0 ? 1 : 0));

    uint8_t* source;
#if HAVE_FDS_BITS
    source = reinterpret_cast<uint8_t*>(&platformSet.fds_bits[0]);
#elif HAVE_PRIVATE_FDS_BITS
    source = reinterpret_cast<uint8_t*>(&platformSet.__fds_bits[0]);
#endif

    memcpy(palSet, source, bytesToCopy);
#endif
}

static void ConvertFdSetPalToPlatform(fd_set& platformSet, uint32_t* palSet, int32_t fdCount)
{
    assert(fdCount >= 0);

    memset(&platformSet, 0, sizeof(platformSet));

#if !HAVE_FDS_BITS && !HAVE_PRIVATE_FDS_BITS
    for (int i = 0; i < fdCount; i++)
    {
        int word = i / FD_SETSIZE_UINTS;
        int bit = i % FD_SETSIZE_UINTS;
        if ((palSet[word] & (1 << bit)) == 0)
        {
            FD_CLR(i, &platformSet);
        }
        else
        {
            FD_SET(i, &platformSet);
        }
    }
#else

    size_t bytesToCopy = static_cast<size_t>((fdCount / 8) + ((fdCount % 8) != 0 ? 1 : 0));

    uint8_t* dest;
#if HAVE_FDS_BITS
    dest = reinterpret_cast<uint8_t*>(&platformSet.fds_bits[0]);
#elif HAVE_PRIVATE_FDS_BITS
    dest = reinterpret_cast<uint8_t*>(&platformSet.__fds_bits[0]);
#endif

    memcpy(dest, palSet, bytesToCopy);
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FdSetSize()
{
    return SystemNative_FdSetSize();
}

extern "C" int32_t SystemNative_FdSetSize()
{
    return FD_SETSIZE;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Select(int32_t fdCount, uint32_t* readFdSet, uint32_t* writeFdSet, uint32_t* errorFdSet, int32_t microseconds, int32_t* selected)
{
    return SystemNative_Select(fdCount, readFdSet, writeFdSet, errorFdSet, microseconds, selected);
}

extern "C" Error
SystemNative_Select(int32_t fdCount, uint32_t* readFdSet, uint32_t* writeFdSet, uint32_t* errorFdSet, int32_t microseconds, int32_t* selected)
{
    if (selected == nullptr)
    {
        return PAL_EFAULT;
    }

    if (fdCount < 0 || static_cast<uint32_t>(fdCount) >= FD_SETSIZE || microseconds < -1)
    {
        return PAL_EINVAL;
    }

    fd_set* readFds = nullptr;
    fd_set* writeFds = nullptr;
    fd_set* errorFds = nullptr;
    timeval* timeout = nullptr;
    timeval tv;

    if (readFdSet != nullptr)
    {
        readFds = reinterpret_cast<fd_set*>(alloca(sizeof(fd_set)));
        ConvertFdSetPalToPlatform(*readFds, readFdSet, fdCount);
    }

    if (writeFdSet != nullptr)
    {
        writeFds = reinterpret_cast<fd_set*>(alloca(sizeof(fd_set)));
        ConvertFdSetPalToPlatform(*writeFds, writeFdSet, fdCount);
    }

    if (errorFdSet != nullptr)
    {
        errorFds = reinterpret_cast<fd_set*>(alloca(sizeof(fd_set)));
        ConvertFdSetPalToPlatform(*errorFds, errorFdSet, fdCount);
    }

    if (microseconds != -1)
    {
        tv.tv_sec = microseconds / 1000000;
        tv.tv_usec = microseconds % 1000000;
        timeout = &tv;
    }

    int rv = select(fdCount, readFds, writeFds, errorFds, timeout);
    if (rv == -1)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    if (readFdSet != nullptr)
    {
        ConvertFdSetPlatformToPal(readFdSet, *readFds, fdCount);
    }

    if (writeFdSet != nullptr)
    {
        ConvertFdSetPlatformToPal(writeFdSet, *writeFds, fdCount);
    }

    if (errorFdSet != nullptr)
    {
        ConvertFdSetPlatformToPal(errorFdSet, *errorFds, fdCount);
    }

    *selected = rv;
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error GetBytesAvailable(int32_t socket, int32_t* available)
{
    return SystemNative_GetBytesAvailable(socket, available);
}

extern "C" Error SystemNative_GetBytesAvailable(int32_t socket, int32_t* available)
{
    if (available == nullptr)
    {
        return PAL_EFAULT;
    }

    int avail;
    int err = ioctl(socket, FIONREAD, &avail);
    if (err == -1)
    {
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    *available = static_cast<int32_t>(avail);
    return PAL_SUCCESS;
}

#if HAVE_EPOLL

const size_t SocketEventBufferElementSize = Max(sizeof(epoll_event), sizeof(SocketEvent));

static SocketEvents GetSocketEvents(uint32_t events)
{
    int asyncEvents = (((events & EPOLLIN) != 0) ? PAL_SA_READ : 0) | (((events & EPOLLOUT) != 0) ? PAL_SA_WRITE : 0) |
                      (((events & EPOLLRDHUP) != 0) ? PAL_SA_READCLOSE : 0) |
                      (((events & EPOLLHUP) != 0) ? PAL_SA_CLOSE : 0) | (((events & EPOLLERR) != 0) ? PAL_SA_ERROR : 0);

    return static_cast<SocketEvents>(asyncEvents);
}

static uint32_t GetEPollEvents(SocketEvents events)
{
    return (((events & PAL_SA_READ) != 0) ? EPOLLIN : 0) | (((events & PAL_SA_WRITE) != 0) ? EPOLLOUT : 0) |
           (((events & PAL_SA_READCLOSE) != 0) ? EPOLLRDHUP : 0) | (((events & PAL_SA_CLOSE) != 0) ? EPOLLHUP : 0) |
           (((events & PAL_SA_ERROR) != 0) ? EPOLLERR : 0);
}

static Error CreateSocketEventPortInner(int32_t* port)
{
    assert(port != nullptr);

    int epollFd = epoll_create1(EPOLL_CLOEXEC);
    if (epollFd == -1)
    {
        *port = -1;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    *port = epollFd;
    return PAL_SUCCESS;
}

static Error CloseSocketEventPortInner(int32_t port)
{
    int err = close(port);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static Error TryChangeSocketEventRegistrationInner(
    int32_t port, int32_t socket, SocketEvents currentEvents, SocketEvents newEvents, uintptr_t data)
{
    assert(currentEvents != newEvents);

    int op = EPOLL_CTL_MOD;
    if (currentEvents == PAL_SA_NONE)
    {
        op = EPOLL_CTL_ADD;
    }
    else if (newEvents == PAL_SA_NONE)
    {
        op = EPOLL_CTL_DEL;
    }

    epoll_event evt = {.events = GetEPollEvents(newEvents) | EPOLLET, .data = {.ptr = reinterpret_cast<void*>(data)}};
    int err = epoll_ctl(port, op, socket, &evt);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static void ConvertEventEPollToSocketAsync(SocketEvent* sae, epoll_event* epoll)
{
    assert(sae != nullptr);
    assert(epoll != nullptr);

    // epoll does not play well with disconnected connection-oriented sockets, frequently
    // reporting spurious EPOLLHUP events. Fortunately, EPOLLHUP may be handled as an
    // EPOLLIN | EPOLLOUT event: the usual processing for these events will recognize and
    // handle the HUP condition.
    uint32_t events = epoll->events;
    if ((events & EPOLLHUP) != 0)
    {
        events = (events & ~EPOLLHUP) | EPOLLIN | EPOLLOUT;
    }

    *sae = {.Data = reinterpret_cast<uintptr_t>(epoll->data.ptr), .Events = GetSocketEvents(events)};
}

static Error WaitForSocketEventsInner(int32_t port, SocketEvent* buffer, int32_t* count)
{
    assert(buffer != nullptr);
    assert(count != nullptr);
    assert(*count >= 0);

    auto* events = reinterpret_cast<epoll_event*>(buffer);
    int numEvents = epoll_wait(port, events, *count, -1);
    if (numEvents == -1)
    {
        *count = 0;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    // We should never see 0 events. Given an infinite timeout, epoll_wait will never return
    // 0 events even if there are no file descriptors registered with the epoll fd. In
    // that case, the wait will block until a file descriptor is added and an event occurs
    // on the added file descriptor.
    assert(numEvents != 0);
    assert(numEvents < *count);

    if (sizeof(epoll_event) < sizeof(SocketEvent))
    {
        // Copy backwards to avoid overwriting earlier data.
        for (int i = numEvents - 1; i >= 0; i--)
        {
            // This copy is made deliberately to avoid overwriting data.
            epoll_event evt = events[i];
            ConvertEventEPollToSocketAsync(&buffer[i], &evt);
        }
    }
    else
    {
        // Copy forwards for better cache behavior
        for (int i = 0; i < numEvents; i++)
        {
            // This copy is made deliberately to avoid overwriting data.
            epoll_event evt = events[i];
            ConvertEventEPollToSocketAsync(&buffer[i], &evt);
        }
    }

    *count = numEvents;
    return PAL_SUCCESS;
}

#elif HAVE_KQUEUE

static_assert(sizeof(SocketEvent) <= sizeof(struct kevent), "");
const size_t SocketEventBufferElementSize = sizeof(struct kevent);

static SocketEvents GetSocketEvents(int16_t filter, uint16_t flags)
{
    int32_t events;
    switch (filter)
    {
        case EVFILT_READ:
            events = PAL_SA_READ;
            if ((flags & EV_EOF) != 0)
            {
                events |= PAL_SA_READCLOSE;
            }
            break;

        case EVFILT_WRITE:
            events = PAL_SA_WRITE;

            // kqueue does not play well with disconnected connection-oriented sockets, frequently
            // reporting spurious EOF events. Fortunately, EOF may be handled as an EVFILT_READ |
            // EVFILT_WRITE event: the usual processing for these events will recognize and
            // handle the EOF condition.
            if ((flags & EV_EOF) != 0)
            {
                events |= PAL_SA_READ;
            }
            break;

        default:
            assert(false && "unexpected kqueue filter type");
            return PAL_SA_NONE;
    }

    if ((flags & EV_ERROR) != 0)
    {
        events |= PAL_SA_ERROR;
    }

    return static_cast<SocketEvents>(events);
};

static Error CreateSocketEventPortInner(int32_t* port)
{
    assert(port != nullptr);

    int kqueueFd = kqueue();
    if (kqueueFd == -1)
    {
        *port = -1;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    *port = kqueueFd;
    return PAL_SUCCESS;
}

static Error CloseSocketEventPortInner(int32_t port)
{
    int err = close(port);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static Error TryChangeSocketEventRegistrationInner(
    int32_t port, int32_t socket, SocketEvents currentEvents, SocketEvents newEvents, uintptr_t data)
{
    const uint16_t AddFlags = EV_ADD | EV_CLEAR | EV_RECEIPT;
    const uint16_t RemoveFlags = EV_DELETE | EV_RECEIPT;

    assert(currentEvents != newEvents);

    int32_t changes = currentEvents ^ newEvents;
    bool readChanged = (changes & PAL_SA_READ) != 0;
    bool writeChanged = (changes & PAL_SA_WRITE) != 0;

    struct kevent events[2];

    int i = 0;
    if (readChanged)
    {
        EV_SET(&events[i++],
               static_cast<uint64_t>(socket),
               EVFILT_READ,
               (newEvents & PAL_SA_READ) == 0 ? RemoveFlags : AddFlags,
               0,
               0,
               reinterpret_cast<void*>(data));
    }

    if (writeChanged)
    {
        EV_SET(&events[i++],
               static_cast<uint64_t>(socket),
               EVFILT_WRITE,
               (newEvents & PAL_SA_WRITE) == 0 ? RemoveFlags : AddFlags,
               0,
               0,
               reinterpret_cast<void*>(data));
    }

    int err = kevent(port, events, i, nullptr, 0, nullptr);
    return err == 0 ? PAL_SUCCESS : SystemNative_ConvertErrorPlatformToPal(errno);
}

static Error WaitForSocketEventsInner(int32_t port, SocketEvent* buffer, int32_t* count)
{
    assert(buffer != nullptr);
    assert(count != nullptr);
    assert(*count >= 0);

    auto* events = reinterpret_cast<struct kevent*>(buffer);
    int numEvents = kevent(port, nullptr, 0, events, *count, nullptr);
    if (numEvents == -1)
    {
        *count = -1;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    // We should never see 0 events. Given an infinite timeout, kevent will never return
    // 0 events even if there are no file descriptors registered with the kqueue fd. In
    // that case, the wait will block until a file descriptor is added and an event occurs
    // on the added file descriptor.
    assert(numEvents != 0);
    assert(numEvents < *count);

    for (int i = 0; i < numEvents; i++)
    {
        // This copy is made deliberately to avoid overwriting data.
        struct kevent evt = events[i];
        buffer[i] = {.Data = reinterpret_cast<uintptr_t>(evt.udata), .Events = GetSocketEvents(evt.filter, evt.flags)};
    }

    *count = numEvents;
    return PAL_SUCCESS;
}

#else
#error Asynchronous sockets require epoll or kqueue support.
#endif

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error CreateSocketEventPort(int32_t* port)
{
    return SystemNative_CreateSocketEventPort(port);
}

extern "C" Error SystemNative_CreateSocketEventPort(int32_t* port)
{
    if (port == nullptr)
    {
        return PAL_EFAULT;
    }

    return CreateSocketEventPortInner(port);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error CloseSocketEventPort(int32_t port)
{
    return SystemNative_CloseSocketEventPort(port);
}

extern "C" Error SystemNative_CloseSocketEventPort(int32_t port)
{
    return CloseSocketEventPortInner(port);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error CreateSocketEventBuffer(int32_t count, SocketEvent** buffer)
{
    return SystemNative_CreateSocketEventBuffer(count, buffer);
}

extern "C" Error SystemNative_CreateSocketEventBuffer(int32_t count, SocketEvent** buffer)
{
    if (buffer == nullptr || count < 0)
    {
        return PAL_EFAULT;
    }

    void* b = malloc(SocketEventBufferElementSize * static_cast<size_t>(count));
    if (b == nullptr)
    {
        *buffer = nullptr;
        return PAL_ENOMEM;
    }

    *buffer = reinterpret_cast<SocketEvent*>(b);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error FreeSocketEventBuffer(SocketEvent* buffer)
{
    return SystemNative_FreeSocketEventBuffer(buffer);
}

extern "C" Error SystemNative_FreeSocketEventBuffer(SocketEvent* buffer)
{
    free(buffer);
    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error TryChangeSocketEventRegistration(int32_t port, int32_t socket, int32_t currentEvents, int32_t newEvents, uintptr_t data)
{
    return SystemNative_TryChangeSocketEventRegistration(port, socket, currentEvents, newEvents, data);
}

extern "C" Error
SystemNative_TryChangeSocketEventRegistration(int32_t port, int32_t socket, int32_t currentEvents, int32_t newEvents, uintptr_t data)
{
    const int32_t SupportedEvents = PAL_SA_READ | PAL_SA_WRITE | PAL_SA_READCLOSE | PAL_SA_CLOSE | PAL_SA_ERROR;

    if ((currentEvents & ~SupportedEvents) != 0 || (newEvents & ~SupportedEvents) != 0)
    {
        return PAL_EINVAL;
    }

    if (currentEvents == newEvents)
    {
        return PAL_SUCCESS;
    }

    return TryChangeSocketEventRegistrationInner(
        port, socket, static_cast<SocketEvents>(currentEvents), static_cast<SocketEvents>(newEvents), data);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error WaitForSocketEvents(int32_t port, SocketEvent* buffer, int32_t* count)
{
    return SystemNative_WaitForSocketEvents(port, buffer, count);
}

extern "C" Error SystemNative_WaitForSocketEvents(int32_t port, SocketEvent* buffer, int32_t* count)
{
    if (buffer == nullptr || count == nullptr || *count < 0)
    {
        return PAL_EFAULT;
    }

    return WaitForSocketEventsInner(port, buffer, count);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t PlatformSupportsMultipleConnectAttempts()
{
    return SystemNative_PlatformSupportsMultipleConnectAttempts();
}

extern "C" int32_t SystemNative_PlatformSupportsMultipleConnectAttempts()
{
#if HAVE_SUPPORT_FOR_MULTIPLE_CONNECT_ATTEMPTS
    return 1;
#else
    return 0;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t PlatformSupportsDualModeIPv4PacketInfo()
{
    return SystemNative_PlatformSupportsDualModeIPv4PacketInfo();
}

extern "C" int32_t SystemNative_PlatformSupportsDualModeIPv4PacketInfo()
{
#if HAVE_SUPPORT_FOR_DUAL_MODE_IPV4_PACKET_INFO
    return 1;
#else
    return 0;
#endif
}
