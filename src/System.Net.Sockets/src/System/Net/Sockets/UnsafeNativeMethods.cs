// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//------------------------------------------------------------------------------
// </copyright>
//------------------------------------------------------------------------------

namespace System.Net.Sockets
{
    using Microsoft.Win32.SafeHandles;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class UnsafeSocketsNativeMethods
    {
        //
        // UnsafeSocketsNativeMethods.OSSOCK class contains all Unsafe() calls and should all be protected
        // by the appropriate SocketPermission() to connect/accept to/from remote
        // peers over the network and to perform name resolution.
        // te following calls deal mainly with:
        // 1) socket calls
        // 2) DNS calls
        //

        //
        // here's a brief explanation of all possible decorations we use for PInvoke.
        // these are used in such a way that we hope to gain maximum performance from the
        // unmanaged/managed/unmanaged transition we need to undergo when calling into winsock:
        //
        // [In] (Note: this is similar to what msdn will show)
        // the managed data will be marshalled so that the unmanaged function can read it but even
        // if it is changed in unmanaged world, the changes won't be propagated to the managed data
        //
        // [Out] (Note: this is similar to what msdn will show)
        // the managed data will not be marshalled so that the unmanaged function will not see the
        // managed data, if the data changes in unmanaged world, these changes will be propagated by
        // the marshaller to the managed data
        //
        // objects are marshalled differently if they're:
        //
        // 1) structs
        // for structs, by default, the whole layout is pushed on the stack as it is.
        // in order to pass a pointer to the managed layout, we need to specify either the ref or out keyword.
        //
        //      a) for IN and OUT:
        //      [In, Out] ref Struct ([In, Out] is optional here)
        //
        //      b) for IN only (the managed data will be marshalled so that the unmanaged
        //      function can read it but even if it changes it the change won't be propagated
        //      to the managed struct)
        //      [In] ref Struct
        //
        //      c) for OUT only (the managed data will not be marshalled so that the
        //      unmanaged function cannot read, the changes done in unmanaged code will be
        //      propagated to the managed struct)
        //      [Out] out Struct ([Out] is optional here)
        //
        // 2) array or classes
        // for array or classes, by default, a pointer to the managed layout is passed.
        // we don't need to specify neither the ref nor the out keyword.
        //
        //      a) for IN and OUT:
        //      [In, Out] byte[]
        //
        //      b) for IN only (the managed data will be marshalled so that the unmanaged
        //      function can read it but even if it changes it the change won't be propagated
        //      to the managed struct)
        //      [In] byte[] ([In] is optional here)
        //
        //      c) for OUT only (the managed data will not be marshalled so that the
        //      unmanaged function cannot read, the changes done in unmanaged code will be
        //      propagated to the managed struct)
        //      [Out] byte[]
        //
        internal static class OSSOCK
        {
            //
            // IPv6 Changes: These are initialized in InitializeSockets - don't set them here or
            //               there will be an ordering problem with the call above that will
            //               result in both being set to false !
            //

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

            [StructLayout(LayoutKind.Sequential)]
            internal struct ControlData
            {
                internal UIntPtr length;
                internal uint level;
                internal uint type;
                internal uint address;
                internal uint index;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct ControlDataIPv6
            {
                internal UIntPtr length;
                internal uint level;
                internal uint type;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                internal byte[] address;
                internal uint index;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct WSAMsg
            {
                internal IntPtr socketAddress;
                internal uint addressLength;
                internal IntPtr buffers;
                internal uint count;
                internal WSABuffer controlBuffer;
                internal SocketFlags flags;
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

            /*
               typedef struct _SOCKET_ADDRESS {  
                   PSOCKADDR lpSockaddr;  
                   INT iSockaddrLength;
               } SOCKET_ADDRESS, *PSOCKET_ADDRESS;			
            */
            [StructLayout(LayoutKind.Sequential)]
            internal struct SOCKET_ADDRESS
            {
                internal IntPtr lpSockAddr;
                internal int iSockaddrLength;
            }

            /*
               typedef struct _SOCKET_ADDRESS_LIST {
                   INT             iAddressCount;
                   SOCKET_ADDRESS  Address[1];
               } SOCKET_ADDRESS_LIST, *PSOCKET_ADDRESS_LIST, FAR *LPSOCKET_ADDRESS_LIST;
            */
            [StructLayout(LayoutKind.Sequential)]
            internal struct SOCKET_ADDRESS_LIST
            {
                internal int iAddressCount;
                internal SOCKET_ADDRESS Addresses;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct TransmitFileBuffersStruct
            {
                internal IntPtr preBuffer;// Pointer to Buffer
                internal int preBufferLength; // Length of Buffer
                internal IntPtr postBuffer;// Pointer to Buffer
                internal int postBufferLength; // Length of Buffer
            }

            // TODO: The MCG compiler or Test infrastructure is currently broken and cannot properly interpret CharSet.Unicode.
            //       The code has been changed from WSASocket to WSASocketW to avoid an EXE loader issue.
            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern SafeCloseSocket.InnerSafeCloseSocket WSASocketW(
                                                    [In] AddressFamily addressFamily,
                                                    [In] SocketType socketType,
                                                    [In] ProtocolType protocolType,
                                                    [In] IntPtr protocolInfo, // will be WSAProtcolInfo protocolInfo once we include QOS APIs
                                                    [In] uint group,
                                                    [In] SocketConstructorFlags flags
                                                    );

            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Unicode, SetLastError = true)]
            internal unsafe static extern SafeCloseSocket.InnerSafeCloseSocket WSASocketW(
                                        [In] AddressFamily addressFamily,
                                        [In] SocketType socketType,
                                        [In] ProtocolType protocolType,
                                        [In] byte* pinnedBuffer, // will be WSAProtcolInfo protocolInfo once we include QOS APIs
                                        [In] uint group,
                                        [In] SocketConstructorFlags flags
                                        );


            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            internal static extern SocketError WSAStartup(
                                               [In] short wVersionRequested,
                                               [Out] out WSAData lpWSAData
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError ioctlsocket(
                                                [In] SafeCloseSocket socketHandle,
                                                [In] int cmd,
                                                [In, Out] ref int argp
                                                );

            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            internal static extern IntPtr gethostbyname(
                                                  [In] string host
                                                  );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern IntPtr gethostbyaddr(
                                                  [In] ref int addr,
                                                  [In] int len,
                                                  [In] ProtocolFamily type
                                                  );

            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            internal static extern SocketError gethostname(
                                                [Out] StringBuilder hostName,
                                                [In] int bufferLength
                                                );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getpeername(
                                                [In] SafeCloseSocket socketHandle,
                                                [Out] byte[] socketAddress,
                                                [In, Out] ref int socketAddressSize
                                                );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [Out] out int optionValue,
                                               [In, Out] ref int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [Out] byte[] optionValue,
                                               [In, Out] ref int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [Out] out Linger optionValue,
                                               [In, Out] ref int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [Out] out IPMulticastRequest optionValue,
                                               [In, Out] ref int optionLength
                                               );

            //
            // IPv6 Changes: need to receive and IPv6MulticastRequest from getsockopt
            //
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [Out] out IPv6MulticastRequest optionValue,
                                               [In, Out] ref int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref int optionValue,
                                               [In] int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] byte[] optionValue,
                                               [In] int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref IntPtr pointer,
                                               [In] int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref Linger linger,
                                               [In] int optionLength
                                               );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref IPMulticastRequest mreq,
                                               [In] int optionLength
                                               );

            //
            // IPv6 Changes: need to pass an IPv6MulticastRequest to setsockopt
            //
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] SafeCloseSocket socketHandle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref IPv6MulticastRequest mreq,
                                               [In] int optionLength
                                               );

#if !FEATURE_PAL

            [DllImport(ExternDll.MSWSOCK, SetLastError = true)]
            internal static extern bool TransmitFile(
                                      [In] SafeCloseSocket socket,
                                      [In] SafeHandle fileHandle,
                                      [In] int numberOfBytesToWrite,
                                      [In] int numberOfBytesPerSend,
                                      [In] SafeHandle overlapped,
                                      [In] TransmitFileBuffers buffers,
                                      [In] TransmitFileOptions flags
                                      );

            [DllImport(ExternDll.MSWSOCK, SetLastError = true, EntryPoint = "TransmitFile")]
            internal static extern bool TransmitFile2(
                                      [In] SafeCloseSocket socket,
                                      [In] IntPtr fileHandle,
                                      [In] int numberOfBytesToWrite,
                                      [In] int numberOfBytesPerSend,
                                      [In] SafeHandle overlapped,
                                      [In] TransmitFileBuffers buffers,
                                      [In] TransmitFileOptions flags
                                      );


            [DllImport(ExternDll.MSWSOCK, SetLastError = true, EntryPoint = "TransmitFile")]
            internal static extern bool TransmitFile_Blocking(
                                      [In] IntPtr socket,
                                      [In] SafeHandle fileHandle,
                                      [In] int numberOfBytesToWrite,
                                      [In] int numberOfBytesPerSend,
                                      [In] SafeHandle overlapped,
                                      [In] TransmitFileBuffers buffers,
                                      [In] TransmitFileOptions flags
                                      );

            [DllImport(ExternDll.MSWSOCK, SetLastError = true, EntryPoint = "TransmitFile")]
            internal static extern bool TransmitFile_Blocking2(
                                      [In] IntPtr socket,
                                      [In] IntPtr fileHandle,
                                      [In] int numberOfBytesToWrite,
                                      [In] int numberOfBytesPerSend,
                                      [In] SafeHandle overlapped,
                                      [In] TransmitFileBuffers buffers,
                                      [In] TransmitFileOptions flags
                                      );

#endif // !FEATURE_PAL

            // This method is always blocking, so it uses an IntPtr.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal unsafe static extern int send(
                                         [In] IntPtr socketHandle,
                                         [In] byte* pinnedBuffer,
                                         [In] int len,
                                         [In] SocketFlags socketFlags
                                         );

            // This method is always blocking, so it uses an IntPtr.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal unsafe static extern int recv(
                                         [In] IntPtr socketHandle,
                                         [In] byte* pinnedBuffer,
                                         [In] int len,
                                         [In] SocketFlags socketFlags
                                         );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError listen(
                                           [In] SafeCloseSocket socketHandle,
                                           [In] int backlog
                                           );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError bind(
                                         [In] SafeCloseSocket socketHandle,
                                         [In] byte[] socketAddress,
                                         [In] int socketAddressSize
                                         );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError shutdown(
                                             [In] SafeCloseSocket socketHandle,
                                             [In] int how
                                             );

            // This method is always blocking, so it uses an IntPtr.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal unsafe static extern int sendto(
                                           [In] IntPtr socketHandle,
                                           [In] byte* pinnedBuffer,
                                           [In] int len,
                                           [In] SocketFlags socketFlags,
                                           [In] byte[] socketAddress,
                                           [In] int socketAddressSize
                                           );

            // This method is always blocking, so it uses an IntPtr.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal unsafe static extern int recvfrom(
                                             [In] IntPtr socketHandle,
                                             [In] byte* pinnedBuffer,
                                             [In] int len,
                                             [In] SocketFlags socketFlags,
                                             [Out] byte[] socketAddress,
                                             [In, Out] ref int socketAddressSize
                                             );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError getsockname(
                                                [In] SafeCloseSocket socketHandle,
                                                [Out] byte[] socketAddress,
                                                [In, Out] ref int socketAddressSize
                                                );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern int select(
                                           [In] int ignoredParameter,
                                           [In, Out] IntPtr[] readfds,
                                           [In, Out] IntPtr[] writefds,
                                           [In, Out] IntPtr[] exceptfds,
                                           [In] ref TimeValue timeout
                                           );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern int select(
                                           [In] int ignoredParameter,
                                           [In, Out] IntPtr[] readfds,
                                           [In, Out] IntPtr[] writefds,
                                           [In, Out] IntPtr[] exceptfds,
                                           [In] IntPtr nullTimeout
                                           );

            // This function is always potentially blocking so it uses an IntPtr.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSAConnect(
                                              [In] IntPtr socketHandle,
                                              [In] byte[] socketAddress,
                                              [In] int socketAddressSize,
                                              [In] IntPtr inBuffer,
                                              [In] IntPtr outBuffer,
                                              [In] IntPtr sQOS,
                                              [In] IntPtr gQOS
                                              );


            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSASend(
                                              [In] SafeCloseSocket socketHandle,
                                              [In] ref WSABuffer buffer,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In] SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSASend(
                                              [In] SafeCloseSocket socketHandle,
                                              [In] WSABuffer[] buffersArray,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In] SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true, EntryPoint = "WSASend")]
            internal static extern SocketError WSASend_Blocking(
                                              [In] IntPtr socketHandle,
                                              [In] WSABuffer[] buffersArray,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In] SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSARecv(
                                              [In] SafeCloseSocket socketHandle,
                                              [In] ref WSABuffer buffer,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In, Out] ref SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSARecv(
                                              [In] SafeCloseSocket socketHandle,
                                              [In, Out] WSABuffer[] buffers,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In, Out] ref SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true, EntryPoint = "WSARecv")]
            internal static extern SocketError WSARecv_Blocking(
                                              [In] IntPtr socketHandle,
                                              [In, Out] WSABuffer[] buffers,
                                              [In] int bufferCount,
                                              [Out] out int bytesTransferred,
                                              [In, Out] ref SocketFlags socketFlags,
                                              [In] SafeHandle overlapped,
                                              [In] IntPtr completionRoutine
                                              );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSAEventSelect(
                                                     [In] SafeCloseSocket socketHandle,
                                                     [In] SafeHandle Event,
                                                     [In] AsyncEventBits NetworkEvents
                                                     );

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSAEventSelect(
                                         [In] SafeCloseSocket socketHandle,
                                         [In] IntPtr Event,
                                         [In] AsyncEventBits NetworkEvents
                                         );


            // Used with SIOGETEXTENSIONFUNCTIONPOINTER - we're assuming that will never block.
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true, EntryPoint = "WSAIoctl")]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true, EntryPoint = "WSAIoctl")]
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

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern SocketError WSAEnumNetworkEvents(
                                                     [In] SafeCloseSocket socketHandle,
                                                     [In] SafeWaitHandle Event,
                                                     [In, Out] ref NetworkEvents networkEvents
                                                     );

#if !FEATURE_PAL
            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal unsafe static extern int WSADuplicateSocket(
                [In] SafeCloseSocket socketHandle,
                [In] uint targetProcessID,
                [In] byte* pinnedBuffer
            );
#endif // !FEATURE_PAL

            [DllImport(ExternDll.WS2_32, SetLastError = true)]
            internal static extern bool WSAGetOverlappedResult(
                                                     [In] SafeCloseSocket socketHandle,
                                                     [In] SafeHandle overlapped,
                                                     [Out] out uint bytesTransferred,
                                                     [In] bool wait,
                                                     [Out] out SocketFlags socketFlags
                                                     );
            [DllImport(ExternDll.WS2_32, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            internal static extern SocketError GetNameInfoW(
                [In]         byte[] sa,
                [In]         int salen,
                [In, Out]     StringBuilder host,
                [In]         int hostlen,
                [In, Out]     StringBuilder serv,
                [In]         int servlen,
                [In]         int flags);
        } // class UnsafeSocketsNativeMethods.OSSOCK

        // Because the regular SafeNetHandles tries to bind this MustRun method on type initialization, failing
        // on legacy platforms.
        internal static class SafeNetHandlesXPOrLater
        {
            [DllImport(ExternDll.WS2_32, ExactSpelling = true, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
            internal static extern int GetAddrInfoW(
                [In] string nodename,
                [In] string servicename,
                [In] ref AddressInfo hints,
                [Out] out SafeFreeAddrInfo handle
                );

            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern void freeaddrinfo([In] IntPtr info);
        }

        internal static class SafeNetHandles
        {
            // Blocking call - requires IntPtr instead of SafeCloseSocket.
            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern SafeCloseSocket.InnerSafeCloseSocket accept(
                                                  [In] IntPtr socketHandle,
                                                  [Out] byte[] socketAddress,
                                                  [In, Out] ref int socketAddressSize
                                                  );

            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern SocketError closesocket(
                                                  [In] IntPtr socketHandle
                                                  );

            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern SocketError ioctlsocket(
                                                [In] IntPtr handle,
                                                [In] int cmd,
                                                [In, Out] ref int argp
                                                );

            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern SocketError WSAEventSelect(
                                                     [In] IntPtr handle,
                                                     [In] IntPtr Event,
                                                     [In] AsyncEventBits NetworkEvents
                                                     );

            [DllImport(ExternDll.WS2_32, ExactSpelling = true, SetLastError = true)]
            internal static extern SocketError setsockopt(
                                               [In] IntPtr handle,
                                               [In] SocketOptionLevel optionLevel,
                                               [In] SocketOptionName optionName,
                                               [In] ref Linger linger,
                                               [In] int optionLength
                                               );
        }
    }
}

