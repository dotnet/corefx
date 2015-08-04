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
    }
}
