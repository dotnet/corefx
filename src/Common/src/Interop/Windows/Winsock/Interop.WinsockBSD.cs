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
        // IO-Control operations are not directly exposed.
        // blocking is controlled by "Blocking" property on socket (FIONBIO)
        // amount of data available is queried by "Available" property (FIONREAD)
        // The other flags are not exposed currently
        //
        internal static class IoctlSocketConstants
        {
            public const int FIONREAD = 0x4004667F;
            public const int FIONBIO = unchecked((int)0x8004667E);
            public const int FIOASYNC = unchecked((int)0x8004667D);
            public const int SIOGETEXTENSIONFUNCTIONPOINTER = unchecked((int)0xC8000006);
            //
            // not likely to block (sync IO ok)
            //
            // FIONBIO
            // FIONREAD
            // SIOCATMARK
            // SIO_RCVALL
            // SIO_RCVALL_MCAST
            // SIO_RCVALL_IGMPMCAST
            // SIO_KEEPALIVE_VALS
            // SIO_ASSOCIATE_HANDLE (opcode setting: I, T==1)
            // SIO_ENABLE_CIRCULAR_QUEUEING (opcode setting: V, T==1)
            // SIO_GET_BROADCAST_ADDRESS (opcode setting: O, T==1)
            // SIO_GET_EXTENSION_FUNCTION_POINTER (opcode setting: O, I, T==1)
            // SIO_MULTIPOINT_LOOPBACK (opcode setting: I, T==1)
            // SIO_MULTICAST_SCOPE (opcode setting: I, T==1)
            // SIO_TRANSLATE_HANDLE (opcode setting: I, O, T==1)
            // SIO_ROUTING_INTERFACE_QUERY (opcode setting: I, O, T==1)
            //
            // likely to block (reccommended for async IO)
            //
            // SIO_FIND_ROUTE (opcode setting: O, T==1)
            // SIO_FLUSH (opcode setting: V, T==1)
            // SIO_GET_QOS (opcode setting: O, T==1)
            // SIO_GET_GROUP_QOS (opcode setting: O, I, T==1)
            // SIO_SET_QOS (opcode setting: I, T==1)
            // SIO_SET_GROUP_QOS (opcode setting: I, T==1)
            // SIO_ROUTING_INTERFACE_CHANGE (opcode setting: I, T==1)
            // SIO_ADDRESS_LIST_CHANGE (opcode setting: T==1)
        }
        
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
        internal static extern SocketError ioctlsocket(
                                            [In] IntPtr handle,
                                            [In] int cmd,
                                            [In, Out] ref int argp
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError ioctlsocket(
                                            [In] SafeCloseSocket socketHandle,
                                            [In] int cmd,
                                            [In, Out] ref int argp
                                            );
    }
}
