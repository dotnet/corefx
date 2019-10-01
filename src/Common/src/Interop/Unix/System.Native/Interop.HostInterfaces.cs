// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        internal const int NI_MAXHOST = 1025;
        internal const int NI_MAXSERV = 32;

        internal enum GetAddrInfoErrorFlags : int
        {
            EAI_AGAIN = 1,      // Temporary failure in name resolution.
            EAI_BADFLAGS = 2,   // Invalid value for `ai_flags' field.
            EAI_FAIL = 3,       // Non-recoverable failure in name resolution.
            EAI_FAMILY = 4,     // 'ai_family' not supported.
            EAI_NONAME = 5,     // NAME or SERVICE is unknown.
            EAI_BADARG = 6,     // One or more input arguments were invalid.
            EAI_NOMORE = 7,     // No more entries are present in the list.
        }

        //opaque structure to maintain consistency with native function signature
        internal unsafe struct ifaddrs
        {

        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct HostInterfaces
        {
            internal ifaddrs* AddressListHandle; // Handle for socket address list
            internal int IPAddressCount;      // Number of IP addresses in the list
        }

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetHostInterfaces")]
        internal static extern unsafe int GetHostInterfaces(HostEntry* entry);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetNextIPAddress_IfAddrs")]
        internal static extern unsafe int GetNextIPAddress(HostEntry* entry, addrinfo** addressListHandle, IPAddress* endPoint);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_FreeHostInterfaces")]
        internal static extern unsafe void FreeHostInterfaces(HostEntry* entry);
    }
}
