// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
    {
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
    }
}
