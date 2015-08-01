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
