// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
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
        internal static extern unsafe int GetHostInterfaces(HostInterfaces* interfaces);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetNextIPAddress_IfAddrs")]
        internal static extern unsafe int GetNextIPAddress_IfAddrs(HostInterfaces* interfaces, ifaddrs** addressListHandle, IPAddress* endPoint);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_FreeHostInterfaces")]
        internal static extern unsafe void FreeHostInterfaces(HostInterfaces* interfaces);
    }
}
