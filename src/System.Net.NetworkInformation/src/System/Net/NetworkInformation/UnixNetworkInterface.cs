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
        // If this is an ipv6 device, contains the Scope ID.
        protected uint? _ipv6ScopeId = null;

        protected UnixNetworkInterface(string name)
        {
            _name = name;
        }

        public override string Name { get { return _name; } }

        public override NetworkInterfaceType NetworkInterfaceType { get { return _networkInterfaceType; } }

        public override PhysicalAddress GetPhysicalAddress()
        {
            Debug.Assert(_physicalAddress != null, "_physicalAddress was never initialized. This means no address with type AF_PACKET was discovered.");
            return _physicalAddress;
        }

        /// <summary>
        /// The system's index for this network device.
        /// </summary>
        public int Index { get { return _index; } }

        /// <summary>
        /// Returns a list of all of the interface's IP Addresses.
        /// </summary>
        public List<IPAddress> Addresses { get { return _addresses; } }

        // Adds any IPAddress to this interface's List of addresses.
        protected void AddAddress(IPAddress ipAddress)
        {
            _addresses.Add(ipAddress);
        }

        public IPAddress GetNetMaskForIPv4Address(IPAddress address)
        {
            Debug.Assert(address.AddressFamily == Sockets.AddressFamily.InterNetwork);
            return _netMasks[address];
        }

        protected static unsafe void ProcessIpv4Address(UnixNetworkInterface uni,
                                                Interop.Sys.IpAddressInfo* addressInfo,
                                                Interop.Sys.IpAddressInfo* netMask)
        {
            byte[] ipBytes = new byte[addressInfo->NumAddressBytes];
            fixed (byte* ipArrayPtr = ipBytes)
            {
                Buffer.MemoryCopy(addressInfo->AddressBytes, ipArrayPtr, ipBytes.Length, ipBytes.Length);
            }
            IPAddress ipAddress = new IPAddress(ipBytes);

            byte[] ipBytes2 = new byte[netMask->NumAddressBytes];
            fixed (byte* ipArrayPtr = ipBytes2)
            {
                Buffer.MemoryCopy(netMask->AddressBytes, ipArrayPtr, ipBytes2.Length, ipBytes2.Length);
            }
            IPAddress netMaskAddress = new IPAddress(ipBytes2);

            uni.AddAddress(ipAddress);
            uni._netMasks[ipAddress] = netMaskAddress;
        }

        protected static unsafe void ProcessIpv6Address(UnixNetworkInterface uni,
                                                        Interop.Sys.IpAddressInfo* addressInfo,
                                                        uint scopeId)
        {
            byte[] ipBytes = new byte[addressInfo->NumAddressBytes];
            fixed (byte* ipArrayPtr = ipBytes)
            {
                Buffer.MemoryCopy(addressInfo->AddressBytes, ipArrayPtr, ipBytes.Length, ipBytes.Length);
            }
            IPAddress address = new IPAddress(ipBytes);

            uni.AddAddress(address);
            uni._ipv6ScopeId = scopeId;
        }

        protected static unsafe void ProcessLinkLevelAddress(UnixNetworkInterface uni, Interop.Sys.LinkLayerAddressInfo* llAddr)
        {
            byte[] macAddress = new byte[llAddr->NumAddressBytes];
            fixed (byte* macAddressPtr = macAddress)
            {
                Buffer.MemoryCopy(llAddr->AddressBytes, macAddressPtr, llAddr->NumAddressBytes, llAddr->NumAddressBytes);
            }
            PhysicalAddress physicalAddress = new PhysicalAddress(macAddress);

            uni._index = llAddr->InterfaceIndex;
            uni._physicalAddress = physicalAddress;
            uni._networkInterfaceType = (NetworkInterfaceType)llAddr->HardwareType;
        }
    }
}
