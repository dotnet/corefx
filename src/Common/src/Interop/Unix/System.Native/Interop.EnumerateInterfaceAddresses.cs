// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LinkLayerAddress
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
        public unsafe delegate void LinkLayerAddressDiscoveredCallback(string ifaceName, LinkLayerAddress* llAddress);

        [DllImport("System.Native")]
        public static extern void EnumerateInterfaceAddresses(  IPv4AddressDiscoveredCallback ipv4Found,
                                                                IPv6AddressDiscoveredCallback ipv6Found,
                                                                LinkLayerAddressDiscoveredCallback linkLayerFound);
    }
}