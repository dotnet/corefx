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
        
        // This function is always potentially blocking so it uses an IntPtr.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSAConnect(
                                          [In] IntPtr socketHandle,
                                          [In] byte[] socketAddress,
                                          [In] int socketAddressSize,
                                          [In] IntPtr inBuffer,
                                          [In] IntPtr outBuffer,
                                          [In] IntPtr sQOS,
                                          [In] IntPtr gQOS
                                          );


        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSASend(
                                          [In] SafeCloseSocket socketHandle,
                                          [In] ref WSABuffer buffer,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In] SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSASend(
                                          [In] SafeCloseSocket socketHandle,
                                          [In] WSABuffer[] buffersArray,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In] SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true, EntryPoint = "WSASend")]
        internal static extern SocketError WSASend_Blocking(
                                          [In] IntPtr socketHandle,
                                          [In] WSABuffer[] buffersArray,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In] SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSASendTo(
                                            [In] SafeCloseSocket socketHandle,
                                            [In] ref WSABuffer buffer,
                                            [In] int bufferCount,
                                            [Out] out int bytesTransferred,
                                            [In] SocketFlags socketFlags,
                                            [In] IntPtr socketAddress,
                                            [In] int socketAddressSize,
                                            [In] SafeHandle overlapped,
                                            [In] IntPtr completionRoutine
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSASendTo(
                                            [In] SafeCloseSocket socketHandle,
                                            [In] WSABuffer[] buffersArray,
                                            [In] int bufferCount,
                                            [Out] out int bytesTransferred,
                                            [In] SocketFlags socketFlags,
                                            [In] IntPtr socketAddress,
                                            [In] int socketAddressSize,
                                            [In] SafeNativeOverlapped overlapped,
                                            [In] IntPtr completionRoutine
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecv(
                                          [In] SafeCloseSocket socketHandle,
                                          [In] ref WSABuffer buffer,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In, Out] ref SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecv(
                                          [In] SafeCloseSocket socketHandle,
                                          [In, Out] WSABuffer[] buffers,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In, Out] ref SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true, EntryPoint = "WSARecv")]
        internal static extern SocketError WSARecv_Blocking(
                                          [In] IntPtr socketHandle,
                                          [In, Out] WSABuffer[] buffers,
                                          [In] int bufferCount,
                                          [Out] out int bytesTransferred,
                                          [In, Out] ref SocketFlags socketFlags,
                                          [In] SafeHandle overlapped,
                                          [In] IntPtr completionRoutine
                                          );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecvFrom(
                                              [In] SafeCloseSocket socketHandle,
                                              [In] ref WSABuffer buffer,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In, Out] ref SocketFlags socketFlags,
                                              [In] IntPtr socketAddressPointer,
                                              [In] IntPtr socketAddressSizePointer,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSARecvFrom(
                                              [In] SafeCloseSocket socketHandle,
                                              [In, Out] WSABuffer[] buffers,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In, Out] ref SocketFlags socketFlags,
                                              [In] IntPtr socketAddressPointer,
                                              [In] IntPtr socketAddressSizePointer,
                                              [In] SafeNativeOverlapped overlapped,
                                              [In] IntPtr completionRoutine
                                              );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSAEventSelect(
                                                 [In] SafeCloseSocket socketHandle,
                                                 [In] SafeHandle Event,
                                                 [In] AsyncEventBits NetworkEvents
                                                 );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSAEventSelect(
                                     [In] SafeCloseSocket socketHandle,
                                     [In] IntPtr Event,
                                     [In] AsyncEventBits NetworkEvents
                                     );

        [DllImport(Interop.Libraries.Ws2_32, ExactSpelling = true, SetLastError = true)]
        internal static extern SocketError WSAEventSelect(
                                                 [In] IntPtr handle,
                                                 [In] IntPtr Event,
                                                 [In] AsyncEventBits NetworkEvents
                                                 );

        // Used with SIOGETEXTENSIONFUNCTIONPOINTER - we're assuming that will never block.
        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSAIoctl(
                                            [In] SafeCloseSocket socketHandle,
                                            [In] int ioControlCode,
                                            [In, Out] ref Guid guid,
                                            [In] int guidSize,
                                            [Out] out IntPtr funcPtr,
                                            [In]  int funcPtrSize,
                                            [Out] out int bytesTransferred,
                                            [In] IntPtr shouldBeNull,
                                            [In] IntPtr shouldBeNull2
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true, EntryPoint = "WSAIoctl")]
        internal static extern SocketError WSAIoctl_Blocking(
                                            [In] IntPtr socketHandle,
                                            [In] int ioControlCode,
                                            [In] byte[] inBuffer,
                                            [In] int inBufferSize,
                                            [Out] byte[] outBuffer,
                                            [In] int outBufferSize,
                                            [Out] out int bytesTransferred,
                                            [In] SafeHandle overlapped,
                                            [In] IntPtr completionRoutine
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true, EntryPoint = "WSAIoctl")]
        internal static extern SocketError WSAIoctl_Blocking_Internal(
                                            [In]  IntPtr socketHandle,
                                            [In]  uint ioControlCode,
                                            [In]  IntPtr inBuffer,
                                            [In]  int inBufferSize,
                                            [Out] IntPtr outBuffer,
                                            [In]  int outBufferSize,
                                            [Out] out int bytesTransferred,
                                            [In]  SafeHandle overlapped,
                                            [In]  IntPtr completionRoutine
                                            );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern SocketError WSAEnumNetworkEvents(
                                                 [In] SafeCloseSocket socketHandle,
                                                 [In] SafeWaitHandle Event,
                                                 [In, Out] ref NetworkEvents networkEvents
                                                 );

        [DllImport(Interop.Libraries.Ws2_32, SetLastError = true)]
        internal static extern bool WSAGetOverlappedResult(
                                                 [In] SafeCloseSocket socketHandle,
                                                 [In] SafeHandle overlapped,
                                                 [Out] out uint bytesTransferred,
                                                 [In] bool wait,
                                                 [Out] out SocketFlags socketFlags
                                                 );
    }
}
