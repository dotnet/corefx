// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libc
    {
        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern unsafe int getifaddrs(out IntPtr ifap);

        [DllImport(Libraries.Libc, SetLastError = true)]
        public static extern unsafe void freeifaddrs(IntPtr ifa);

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct ifaddrs
        {
            public IntPtr ifa_next;
            public string ifa_name;
            public UIntPtr ifa_flags;
            public sockaddr* ifa_addr;
            public sockaddr* ifa_netmask;
            public ifa_ifu ifa_ifu; // Overlapped field

            public sockaddr_in GetIPv4Address()
            {
                return *((sockaddr_in*)ifa_addr);
            }

            public sockaddr_in6 GetIPv6Address()
            {
                return *((sockaddr_in6*)ifa_addr);
            }

            public sockaddr_ll GetLinkLevelAddress()
            {
                return *((sockaddr_ll*)ifa_addr);
            }

            public sockaddr_in GetNetMask()
            {
                return *((sockaddr_in*)ifa_netmask);
            }
        }

        /*
        union {
            struct sockaddr *ifu_broadaddr;
                    /-* Broadcast address of interface *-/
            struct sockaddr *ifu_dstaddr;
                    /-* Point-to-point destination address *-/
            } ifa_ifu;
        */
        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct ifa_ifu
        {
            [FieldOffset(0)]
            public sockaddr* ifu_broaddr;
            [FieldOffset(0)]
            public sockaddr* ifu_dstaddr;
        }
    }
}
