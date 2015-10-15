// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include "pal_types.h"
#include "pal_errno.h"

/**
 * These error values are different on every platform so make a
 * platform-agnostic version that we convert to and send to managed
 */
enum GetAddrInfoErrorFlags
{
    PAL_EAI_SUCCESS = 0,  // Success
    PAL_EAI_AGAIN = 1,    // Temporary failure in name resolution.
    PAL_EAI_BADFLAGS = 2, // Invalid value for `ai_flags' field.
    PAL_EAI_FAIL = 3,     // Non-recoverable failure in name resolution.
    PAL_EAI_FAMILY = 4,   // 'ai_family' not supported.
    PAL_EAI_NONAME = 5,   // NAME or SERVICE is unknown.
    PAL_EAI_BADARG = 6,   // One or more input arguments were invalid.
    PAL_EAI_NOMORE = 7,   // No more entries are present in the list.
};

/**
 * Flags to pass to GetNameInfo. These do not match
 * from platform to platform and must be converted
 */
enum GetNameInfoFlags
{
    PAL_NI_NAMEREQD = 0x1,
    PAL_NI_NUMERICHOST = 0x2,
};

/**
 * Error codes from GetHostByName and GetHostByAddr
 */
enum GetHostErrorCodes
{
    PAL_HOST_NOT_FOUND = 1,
    PAL_TRY_AGAIN = 2,
    PAL_NO_RECOVERY = 3,
    PAL_NO_DATA = 4,
    PAL_NO_ADDRESS = PAL_NO_DATA,
};

/**
 * Address families recognized by {Get,Set}AddressFamily.
 *
 * NOTE: these values are taken from System.Net.AddressFamily. If you add
 *       new entries, be sure that the values are chosen accordingly.
 */
enum AddressFamily : int32_t
{
    PAL_AF_UNSPEC = 0, // System.Net.AddressFamily.Unspecified
    PAL_AF_UNIX = 1,   // System.Net.AddressFamily.Unix
    PAL_AF_INET = 2,   // System.Net.AddressFamily.InterNetwork
    PAL_AF_INET6 = 23, // System.Net.AddressFamily.InterNetworkV6
};

enum MulticastOption : int32_t
{
    PAL_MULTICAST_ADD = 0, // IP{,V6}_ADD_MEMBERSHIP
    PAL_MULTICAST_DROP = 1 // IP{,V6}_DROP_MEMBERSHIP
};

/**
 * IP address sizes.
 */
enum
{
    NUM_BYTES_IN_IPV4_ADDRESS = 4,
    NUM_BYTES_IN_IPV6_ADDRESS = 16,
    MAX_IP_ADDRESS_BYTES = 16,
};

struct IPAddress
{
    uint8_t Address[MAX_IP_ADDRESS_BYTES]; // Buffer to fit IPv4 or IPv6 address
    uint32_t IsIPv6;                       // Non-zero if this is an IPv6 endpoint; zero for IPv4.
    uint32_t ScopeId;                      // Scope ID (IPv6 only)
};

struct HostEntry
{
    uint8_t* CanonicalName;  // Canonical Name of the Host
    void* AddressListHandle; // Handle for host socket addresses
    int32_t IPAddressCount;  // Number of IP end points in the list
    int32_t Padding;         // Pad out to 8-byte alignment
};

struct IPPacketInformation
{
    IPAddress Address;      // Destination IP address
    int32_t InterfaceIndex; // Interface index
    int32_t Padding;        // Pad out to 8-byte alignment
};

struct IPv4MulticastOption
{
    uint32_t MulticastAddress; // Multicast address
    uint32_t LocalAddress;     // Local address
    int32_t InterfaceIndex;    // Interface index
    int32_t Padding;           // Pad out to 8-byte alignment
};

struct IPv6MulticastOption
{
    IPAddress Address;      // Multicast address
    int32_t InterfaceIndex; // Interface index
    int32_t Padding;        // Pad out to 8-byte alignment
};

struct LingerOption
{
    int32_t OnOff;   // Non-zero to enable linger
    int32_t Seconds; // Number of seconds to linger for
};

// NOTE: the layout of this type is intended to exactly  match the layout of a `struct iovec`. There are
//       assertions in pal_networking.cpp that validate this.
struct IOVector
{
    uint8_t* Base;
    uintptr_t Count;
};

struct MessageHeader
{
    uint8_t* SocketAddress;
    IOVector* IOVectors;
    uint8_t* ControlBuffer;
    int32_t SocketAddressLen;
    int32_t IOVectorCount;
    int32_t ControlBufferLen;
    int32_t Flags;
};

/**
 * Converts string-representations of IP Addresses to
 */
extern "C" int32_t IPv6StringToAddress(
    const uint8_t* address, const uint8_t* port, uint8_t* buffer, int32_t bufferLength, uint32_t* scope);

extern "C" int32_t IPv4StringToAddress(const uint8_t* address, uint8_t* buffer, int32_t bufferLength, uint16_t* port);

extern "C" int32_t IPAddressToString(const uint8_t* address,
                                     int32_t addressLength,
                                     bool isIPv6,
                                     uint8_t* string,
                                     int32_t stringLength,
                                     uint32_t scope = 0);

extern "C" int32_t GetHostEntryForName(const uint8_t* address, HostEntry* entry);

extern "C" int32_t GetNextIPAddress(void** addressListHandle, IPAddress* endPoint);

extern "C" void FreeHostEntry(HostEntry* entry);

extern "C" int32_t GetNameInfo(const uint8_t* address,
                               int32_t addressLength,
                               bool isIPv6,
                               uint8_t* host,
                               int32_t hostLength,
                               uint8_t* service,
                               int32_t serviceLength,
                               int32_t flags);

extern "C" int32_t GetHostName(uint8_t* name, int32_t nameLength);

extern "C" Error GetIPSocketAddressSizes(int32_t* ipv4SocketAddressSize, int32_t* ipv6SocketAddressSize);

extern "C" Error GetAddressFamily(const uint8_t* socketAddress, int32_t socketAddressLen, int32_t* addressFamily);

extern "C" Error SetAddressFamily(uint8_t* socketAddress, int32_t socketAddressLen, int32_t addressFamily);

extern "C" Error GetPort(const uint8_t* socketAddress, int32_t socketAddressLen, uint16_t* port);

extern "C" Error SetPort(uint8_t* socketAddress, int32_t socketAddressLen, uint16_t port);

extern "C" Error GetIPv4Address(const uint8_t* socketAddress, int32_t socketAddressLen, uint32_t* address);

extern "C" Error SetIPv4Address(uint8_t* socketAddress, int32_t socketAddressLen, uint32_t address);

extern "C" Error GetIPv6Address(
    const uint8_t* socketAddress, int32_t socketAddressLen, uint8_t* address, int32_t addressLen, uint32_t* scopeId);

// Managed interface types
enum NetworkInterfaceType : uint16_t
{
    Unknown = 1,
    Ethernet = 6,
    TokenRing = 9,
    Fddi = 15,
    BasicIsdn = 20,
    PrimaryIsdn = 21,
    Ppp = 23,
    Loopback = 24,
    Ethernet3Megabit = 26,
    Slip = 28, // GenericSlip
    Atm = 37,
    GenericModem = 48, // GenericModem
    FastEthernetT = 62, // FastEthernet(100BaseT)
    Isdn = 63, // ISDNandX.25
    FastEthernetFx = 69, // FastEthernet(100BaseFX)
    Wireless80211 = 71, // IEEE80211
    AsymmetricDsl = 94, // AsymmetricDigitalSubscrbrLoop
    RateAdaptDsl = 95, // Rate-AdaptDigitalSubscrbrLoop
    SymmetricDsl = 96, // SymmetricDigitalSubscriberLoop
    VeryHighSpeedDsl = 97, // VeryH-SpeedDigitalSubscrbLoop
    IPOverAtm = 114,
    GigabitEthernet = 117,
    Tunnel = 131,
    MultiRateSymmetricDsl = 143, // Multi-rate Symmetric DSL
    HighPerformanceSerialBus = 144, // IEEE1394
    Wman = 237, // IF_TYPE_IEEE80216_WMAN WIMAX
    Wwanpp = 243, // IF_TYPE_WWANPP Mobile Broadband devices based on GSM technology
    Wwanpp2 = 244, // IF_TYPE_WWANPP2 Mobile Broadband devices based on CDMA technology
};

struct LinkLayerAddressInfo
{
    uint32_t InterfaceIndex; // The index of the interface to which this address belongs.
    uint8_t AddressBytes[8]; // A pointer to the bytes containing the address.
    uint8_t NumAddressBytes; // The number of bytes actually stored in the address.
    uint8_t __padding;
    NetworkInterfaceType HardwareType;
};

struct IpAddressInfo
{
    uint32_t InterfaceIndex;
    uint8_t AddressBytes[16];
    uint8_t NumAddressBytes;
    uint8_t __padding[3];
};

typedef void (*IPv4AddressFound)(const char* interfaceName, IpAddressInfo* addressInfo, IpAddressInfo* netMaskInfo);
typedef void (*IPv6AddressFound)(const char* interfaceName, IpAddressInfo* info, uint32_t* scopeId);
typedef void (*LinkLayerAddressFound)(const char* interfaceName, LinkLayerAddressInfo* llAddress);

extern "C" int32_t GetControlMessageBufferSize(int32_t isIPv4, int32_t isIPv6);

extern "C" int32_t
TryGetIPPacketInformation(MessageHeader* messageHeader, int32_t isIPv4, IPPacketInformation* packetInfo);

extern "C" Error GetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option);

extern "C" Error SetIPv4MulticastOption(int32_t socket, int32_t multicastOption, IPv4MulticastOption* option);

extern "C" Error GetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option);

extern "C" Error SetIPv6MulticastOption(int32_t socket, int32_t multicastOption, IPv6MulticastOption* option);

extern "C" Error GetLingerOption(int32_t socket, LingerOption* option);

extern "C" Error SetLingerOption(int32_t socket, LingerOption* option);

extern "C" Error ReceiveMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* received);

extern "C" Error SendMessage(int32_t socket, MessageHeader* messageHeader, int32_t flags, int64_t* sent);
