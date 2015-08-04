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
    }
}
