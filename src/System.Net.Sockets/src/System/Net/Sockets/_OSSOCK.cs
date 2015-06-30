// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    //
    // Argument structure for IP_ADD_MEMBERSHIP and IP_DROP_MEMBERSHIP.
    //
    [StructLayout(LayoutKind.Sequential)]
    internal struct IPMulticastRequest
    {
        internal int MulticastAddress; // IP multicast address of group
        internal int InterfaceAddress; // local IP address of interface

        internal static readonly int Size = Marshal.SizeOf<IPMulticastRequest>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Linger
    {
        internal ushort OnOff; // option on/off
        internal ushort Time; // linger time
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WSABuffer
    {
        internal int Length; // Length of Buffer
        internal IntPtr Pointer;// Pointer to Buffer
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class TransmitFileBuffers
    {
        internal IntPtr preBuffer;// Pointer to Buffer
        internal int preBufferLength; // Length of Buffer
        internal IntPtr postBuffer;// Pointer to Buffer
        internal int postBufferLength; // Length of Buffer
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WSAData
    {
        internal short wVersion;
        internal short wHighVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
        internal string szDescription;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        internal string szSystemStatus;
        internal short iMaxSockets;
        internal short iMaxUdpDg;
        internal IntPtr lpVendorInfo;
    }

    // data structures and types needed for getaddrinfo calls.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal unsafe struct AddressInfo
    {
        internal AddressInfoHints ai_flags;
        internal AddressFamily ai_family;
        internal SocketType ai_socktype;
        internal ProtocolFamily ai_protocol;
        internal int ai_addrlen;
        internal sbyte* ai_canonname;   // Ptr to the cannonical name - check for NULL
        internal byte* ai_addr;         // Ptr to the sockaddr structure
        internal AddressInfo* ai_next;  // Ptr to the next AddressInfo structure
    }

    [Flags]
    internal enum AddressInfoHints
    {
        AI_PASSIVE = 0x01, /* Socket address will be used in bind() call */
        AI_CANONNAME = 0x02, /* Return canonical name in first ai_canonname */
        AI_NUMERICHOST = 0x04, /* Nodename must be a numeric address string */
        AI_FQDN = 0x20000, /* Return the FQDN in ai_canonname. This is different than AI_CANONNAME bit flag that
                                   * returns the canonical name registered in DNS which may be different than the fully
                                   * qualified domain name that the flat name resolved to. Only one of the AI_FQDN and 
                                   * AI_CANONNAME bits can be set.  Win7+ */
    }

    [Flags]
    internal enum NameInfoFlags
    {
        NI_NOFQDN = 0x01, /* Only return nodename portion for local hosts */
        NI_NUMERICHOST = 0x02, /* Return numeric form of the host's address */
        NI_NAMEREQD = 0x04, /* Error if the host's name not in DNS */
        NI_NUMERICSERV = 0x08, /* Return numeric form of the service (port #) */
        NI_DGRAM = 0x10, /* Service is a datagram service */
    }

    // Argument structure for IPV6_ADD_MEMBERSHIP and IPV6_DROP_MEMBERSHIP.
    [StructLayout(LayoutKind.Sequential)]
    internal struct IPv6MulticastRequest
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] MulticastAddress; // IP address of group
        internal int InterfaceIndex;   // local interface index

        internal static readonly int Size = Marshal.SizeOf<IPv6MulticastRequest>();
    }

    //
    // used as last parameter to WSASocket call
    //
    [Flags]
    internal enum SocketConstructorFlags
    {
        WSA_FLAG_OVERLAPPED = 0x01,
        WSA_FLAG_MULTIPOINT_C_ROOT = 0x02,
        WSA_FLAG_MULTIPOINT_C_LEAF = 0x04,
        WSA_FLAG_MULTIPOINT_D_ROOT = 0x08,
        WSA_FLAG_MULTIPOINT_D_LEAF = 0x10,
    }
} // namespace System.Net
