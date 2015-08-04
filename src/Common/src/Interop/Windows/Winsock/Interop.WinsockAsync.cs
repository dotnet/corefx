// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WSAPROTOCOLCHAIN
        {
            internal int ChainLen;                                 /* the length of the chain,     */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            internal uint[] ChainEntries;       /* a list of dwCatalogEntryIds */
        }
       
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WSAPROTOCOL_INFO
        {
            internal uint dwServiceFlags1;
            internal uint dwServiceFlags2;
            internal uint dwServiceFlags3;
            internal uint dwServiceFlags4;
            internal uint dwProviderFlags;
            private Guid _providerId;
            internal uint dwCatalogEntryId;
            private WSAPROTOCOLCHAIN _protocolChain;
            internal int iVersion;
            internal AddressFamily iAddressFamily;
            internal int iMaxSockAddr;
            internal int iMinSockAddr;
            internal int iSocketType;
            internal int iProtocol;
            internal int iProtocolMaxOffset;
            internal int iNetworkByteOrder;
            internal int iSecurityScheme;
            internal uint dwMessageSize;
            internal uint dwProviderReserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szProtocol;
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
        
        //
        // Flags equivalent to winsock TRANSMIT_PACKETS_ELEMENT flags
        //    #define TP_ELEMENT_MEMORY   1
        //    #define TP_ELEMENT_FILE     2
        //    #define TP_ELEMENT_EOP      4
        //
        [Flags]
        internal enum TransmitPacketsElementFlags : uint
        {
            None = 0x00,
            Memory = 0x01,
            File = 0x02,
            EndOfPacket = 0x04
        }

        // Structure equivalent to TRANSMIT_PACKETS_ELEMENT
        //
        // typedef struct _TRANSMIT_PACKETS_ELEMENT {
        //     ULONG dwElFlags;  
        //     ULONG cLength;  
        //     union {    
        //         struct {      
        //             LARGE_INTEGER nFileOffset;      
        //             HANDLE hFile;
        //         };    
        //         PVOID pBuffer;  
        //     }
        //  };
        // } TRANSMIT_PACKETS_ELEMENT;
        //
        [StructLayout(LayoutKind.Explicit)]
        internal struct TransmitPacketsElement
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal TransmitPacketsElementFlags flags;
            [System.Runtime.InteropServices.FieldOffset(4)]
            internal uint length;
            [System.Runtime.InteropServices.FieldOffset(8)]
            internal Int64 fileOffset;
            [System.Runtime.InteropServices.FieldOffset(8)]
            internal IntPtr buffer;
            [System.Runtime.InteropServices.FieldOffset(16)]
            internal IntPtr fileHandle;
        }

        //
        // WinSock 2 extension -- bit values and indices for FD_XXX network events
        //
        [Flags]
        internal enum AsyncEventBits
        {
            FdNone = 0,
            FdRead = 1 << 0,
            FdWrite = 1 << 1,
            FdOob = 1 << 2,
            FdAccept = 1 << 3,
            FdConnect = 1 << 4,
            FdClose = 1 << 5,
            FdQos = 1 << 6,
            FdGroupQos = 1 << 7,
            FdRoutingInterfaceChange = 1 << 8,
            FdAddressListChange = 1 << 9,
            FdAllEvents = (1 << 10) - 1,
        }
    
        // Array position in NetworkEvents (WSAEnumNetworkEvents).
        internal enum AsyncEventBitsPos
        {
            FdReadBit = 0,
            FdWriteBit = 1,
            FdOobBit = 2,
            FdAcceptBit = 3,
            FdConnectBit = 4,
            FdCloseBit = 5,
            FdQosBit = 6,
            FdGroupQosBit = 7,
            FdRoutingInterfaceChangeBit = 8,
            FdAddressListChangeBit = 9,
            FdMaxEvents = 10,
        }
    
        [StructLayout(LayoutKind.Sequential)]
        internal struct NetworkEvents
        {
            //
            // Indicates which of the FD_XXX network events have occurred.
            //
            public AsyncEventBits Events;
    
            //
            // An array that contains any associated error codes, with an array index that corresponds to the position of event bits in lNetworkEvents. The identifiers FD_READ_BIT, FD_WRITE_BIT and other can be used to index the iErrorCode array.
            //
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AsyncEventBitsPos.FdMaxEvents)]
            public int[] ErrorCodes;
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
