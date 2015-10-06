// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LinkLayerAddressInfo
        {
            public int InterfaceIndex;
            public fixed byte AddressBytes[8];
            public byte NumAddressBytes;
            private byte __pading;
            public ushort HardwareType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IpAddressInfo
        {
            public int InterfaceIndex;
            public fixed byte AddressBytes[16];
            public byte NumAddressBytes;
            private fixed byte __padding[3];
        }

        public unsafe delegate void IPv4AddressDiscoveredCallback(string ifaceName, IpAddressInfo* ipAddressInfo, IpAddressInfo* netMaskInfo);
        public unsafe delegate void IPv6AddressDiscoveredCallback(string ifaceName, IpAddressInfo* ipAddressInfo, uint* scopeId);
        public unsafe delegate void LinkLayerAddressDiscoveredCallback(string ifaceName, LinkLayerAddressInfo* llAddress);

        [DllImport("System.Native")]
        public static extern void EnumerateInterfaceAddresses(IPv4AddressDiscoveredCallback ipv4Found,
                                                                IPv6AddressDiscoveredCallback ipv6Found,
                                                                LinkLayerAddressDiscoveredCallback linkLayerFound);

        // From /usr/include/linux/if_arp.h
        // These should be the valid values of sll_hatype
        // denoting the type of hardware link.

        /* ARP protocol HARDWARE identifiers. */
        public const ushort ARPHRD_ETHER = 1;                   /* Ethernet 10Mbps              */
        public const ushort ARPHRD_EETHER = 2;                  /* Experimental Ethernet        */
        public const ushort ARPHRD_PRONET = 4;                  /* PROnet token ring            */
        public const ushort ARPHRD_ATM = 19;                    /* ATM                          */

        public const ushort ARPHRD_SLIP = 256;
        public const ushort ARPHRD_CSLIP = 257;
        public const ushort ARPHRD_SLIP6 = 258;
        public const ushort ARPHRD_CSLIP6 = 259;

        public const ushort ARPHRD_PPP = 512;

        public const ushort ARPHRD_TUNNEL = 768;                /* IPIP tunnel                  */
        public const ushort ARPHRD_TUNNEL6 = 769;               /* IP6IP6 tunnel                */

        public const ushort ARPHRD_LOOPBACK = 772;              /* Loopback device              */

        public const ushort ARPHRD_FDDI = 774;                  /* Fiber Distributed Data Interface */

        public const ushort ARPHRD_IEEE80211 = 801;             /* IEEE 802.11                  */
        public const ushort ARPHRD_IEEE80211_PRISM = 802;       /* IEEE 802.11 + Prism2 header  */
        public const ushort ARPHRD_IEEE80211_RADIOTAP = 803;    /* IEEE 802.11 + radiotap header    */
    }
}