// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        // This method is always blocking, so it uses an IntPtr.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal unsafe static extern int send(
                                     [In] IntPtr socketHandle,
                                     [In] byte* pinnedBuffer,
                                     [In] int len,
                                     [In] SocketFlags socketFlags
                                     );

        // This method is always blocking, so it uses an IntPtr.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal unsafe static extern int recv(
                                     [In] IntPtr socketHandle,
                                     [In] byte* pinnedBuffer,
                                     [In] int len,
                                     [In] SocketFlags socketFlags
                                     );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError listen(
                                       [In] SafeCloseSocket socketHandle,
                                       [In] int backlog
                                       );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError bind(
                                     [In] SafeCloseSocket socketHandle,
                                     [In] byte[] socketAddress,
                                     [In] int socketAddressSize
                                     );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError shutdown(
                                         [In] SafeCloseSocket socketHandle,
                                         [In] int how
                                         );

        // This method is always blocking, so it uses an IntPtr.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal unsafe static extern int sendto(
                                       [In] IntPtr socketHandle,
                                       [In] byte* pinnedBuffer,
                                       [In] int len,
                                       [In] SocketFlags socketFlags,
                                       [In] byte[] socketAddress,
                                       [In] int socketAddressSize
                                       );

        // This method is always blocking, so it uses an IntPtr.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal unsafe static extern int recvfrom(
                                         [In] IntPtr socketHandle,
                                         [In] byte* pinnedBuffer,
                                         [In] int len,
                                         [In] SocketFlags socketFlags,
                                         [Out] byte[] socketAddress,
                                         [In, Out] ref int socketAddressSize
                                         );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError getsockname(
                                            [In] SafeCloseSocket socketHandle,
                                            [Out] byte[] socketAddress,
                                            [In, Out] ref int socketAddressSize
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern int select(
                                       [In] int ignoredParameter,
                                       [In, Out] IntPtr[] readfds,
                                       [In, Out] IntPtr[] writefds,
                                       [In, Out] IntPtr[] exceptfds,
                                       [In] ref TimeValue timeout
                                       );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern int select(
                                       [In] int ignoredParameter,
                                       [In, Out] IntPtr[] readfds,
                                       [In, Out] IntPtr[] writefds,
                                       [In, Out] IntPtr[] exceptfds,
                                       [In] IntPtr nullTimeout
                                       );

        // Blocking call - requires IntPtr instead of SafeCloseSocket.
        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SafeCloseSocket.InnerSafeCloseSocket accept(
                                              [In] IntPtr socketHandle,
                                              [Out] byte[] socketAddress,
                                              [In, Out] ref int socketAddressSize
                                              );

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SocketError closesocket(
                                              [In] IntPtr socketHandle
                                              );

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SocketError ioctlsocket(
                                            [In] IntPtr handle,
                                            [In] int cmd,
                                            [In, Out] ref int argp
                                            );

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

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SocketError setsockopt(
                                           [In] IntPtr handle,
                                           [In] SocketOptionLevel optionLevel,
                                           [In] SocketOptionName optionName,
                                           [In] ref Linger linger,
                                           [In] int optionLength
                                           );
    }
}
