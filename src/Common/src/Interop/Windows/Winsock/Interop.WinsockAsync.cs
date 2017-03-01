// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Winsock
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct ControlData
        {
            internal UIntPtr length;
            internal uint level;
            internal uint type;
            internal uint address;
            internal uint index;
        }

        internal const int IPv6AddressLength = 16;

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct ControlDataIPv6
        {
            internal UIntPtr length;
            internal uint level;
            internal uint type;
            internal fixed byte address[IPv6AddressLength];
            internal uint index;
        }

        // Flags equivalent to winsock TRANSMIT_PACKETS_ELEMENT flags
        //    #define TP_ELEMENT_MEMORY   1
        //    #define TP_ELEMENT_FILE     2
        //    #define TP_ELEMENT_EOP      4
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
        [StructLayout(LayoutKind.Explicit)]
        internal unsafe struct TransmitPacketsElement
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            internal TransmitPacketsElementFlags flags;
            [System.Runtime.InteropServices.FieldOffset(4)]
            internal uint length;
            [System.Runtime.InteropServices.FieldOffset(8)]
            internal Int64 fileOffset;
            [System.Runtime.InteropServices.FieldOffset(8)]
            internal byte* buffer;
            [System.Runtime.InteropServices.FieldOffset(16)]
            internal IntPtr fileHandle;

            // This controls how many TransmitPacketsElement structs we will allocate on the stack.
            // Beyond this, we need to alloc on the heap and pin.
            internal const int StackAllocLimit = 128;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct WSAMsg
        {
            internal byte* socketAddress;
            internal uint addressLength;
            internal WSABuffer* buffers;
            internal uint count;
            internal WSABuffer controlBuffer;
            internal SocketFlags flags;
            internal int pad;

            // NOTE: Even though Win32 docs say the flags arg is a DWORD, and thus 4 bytes long, it seems to actually write 4 bytes past this.
            // The "pad" field above deals with this.

            internal unsafe WSAMsg(byte* socketAddressBuffer, int socketAddressLen, WSABuffer* wsaBuffers, int bufferCount, WSABuffer controlBuffer, SocketFlags flags)
            {
                this.socketAddress = socketAddressBuffer;
                this.addressLength = (uint)socketAddressLen;
                this.buffers = wsaBuffers;
                this.count = (uint)bufferCount;
                this.controlBuffer = controlBuffer;
                this.flags = flags;
                this.pad = 0;
            }
        }
    }
}
