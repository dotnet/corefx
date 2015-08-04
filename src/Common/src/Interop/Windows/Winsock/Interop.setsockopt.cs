// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
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

        // Argument structure for IPV6_ADD_MEMBERSHIP and IPV6_DROP_MEMBERSHIP.
        [StructLayout(LayoutKind.Sequential)]
        internal struct IPv6MulticastRequest
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            internal byte[] MulticastAddress; // IP address of group
            internal int InterfaceIndex;   // local interface index
    
            internal static readonly int Size = Marshal.SizeOf<IPv6MulticastRequest>();
        }
    
        [StructLayout(LayoutKind.Sequential)]
        internal struct Linger
        {
            internal ushort OnOff; // option on/off
            internal ushort Time; // linger time
        }

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] IntPtr handle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref Linger linger,
                                           [In] int optionLength
                                           );
        
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref int optionValue,
                                           [In] int optionLength
                                           );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] byte[] optionValue,
                                           [In] int optionLength
                                           );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref IntPtr pointer,
                                           [In] int optionLength
                                           );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref Linger linger,
                                           [In] int optionLength
                                           );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref IPMulticastRequest mreq,
                                           [In] int optionLength
                                           );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref IPv6MulticastRequest mreq,
                                           [In] int optionLength
                                           );
    }
}
