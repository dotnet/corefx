// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net.NetworkInformation
{
    internal abstract class UnixNetworkInterface : NetworkInterface
    {
        protected string _name;
        protected int _index;
        protected NetworkInterfaceType _networkInterfaceType = NetworkInterfaceType.Unknown;
        protected PhysicalAddress _physicalAddress;
        protected List<IPAddress> _addresses = new List<IPAddress>();
        protected Dictionary<IPAddress, IPAddress> _netMasks = new Dictionary<IPAddress, IPAddress>();

        protected UnixNetworkInterface(string name)
        {
            _name = name;
        }

        // Maps ARPHRD_* values to analogous NetworkInterfaceType values, as closely as possible.
        public static NetworkInterfaceType MapArpHardwareType(ushort arpHardwareType)
        {
            switch (arpHardwareType)
            {
                case Interop.libc.ARPHRD_ETHER:
                case Interop.libc.ARPHRD_EETHER:
                    return NetworkInterfaceType.Ethernet;

                case Interop.libc.ARPHRD_PRONET:
                    return NetworkInterfaceType.TokenRing;

                case Interop.libc.ARPHRD_ATM:
                    return NetworkInterfaceType.Atm;

                case Interop.libc.ARPHRD_SLIP:
                case Interop.libc.ARPHRD_CSLIP:
                case Interop.libc.ARPHRD_SLIP6:
                case Interop.libc.ARPHRD_CSLIP6:
                    return NetworkInterfaceType.Slip;

                case Interop.libc.ARPHRD_PPP:
                    return NetworkInterfaceType.Ppp;

                case Interop.libc.ARPHRD_TUNNEL:
                case Interop.libc.ARPHRD_TUNNEL6:
                    return NetworkInterfaceType.Tunnel;

                case Interop.libc.ARPHRD_LOOPBACK:
                    return NetworkInterfaceType.Loopback;

                case Interop.libc.ARPHRD_FDDI:
                    return NetworkInterfaceType.Fddi;

                case Interop.libc.ARPHRD_IEEE80211:
                case Interop.libc.ARPHRD_IEEE80211_PRISM:
                case Interop.libc.ARPHRD_IEEE80211_RADIOTAP:
                    return NetworkInterfaceType.Wireless80211;

                default:
                    Debug.WriteLine("Unmapped ARP Hardware type: " + arpHardwareType);
                    return NetworkInterfaceType.Unknown;
            }
        }

        // If this is an ipv6 device, contains the Scope ID.
        protected uint? _ipv6ScopeId = null;

        /// <summary>
        /// The system's index for this network device.
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// Returns a list of all of the interface's IP Addresses.
        /// </summary>
        public List<IPAddress> Addresses { get { return _addresses; } }
    }
}