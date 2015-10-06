// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
    /// <summary>
    /// Implements a NetworkInterface on Linux.
    /// </summary>
    internal class LinuxNetworkInterface : UnixNetworkInterface
    {
        internal LinuxNetworkInterface(string name) : base(name) { }

        public unsafe static NetworkInterface[] GetLinuxNetworkInterfaces()
        {
            Dictionary<string, LinuxNetworkInterface> interfacesByName = new Dictionary<string, LinuxNetworkInterface>();
            Interop.Sys.EnumerateInterfaceAddresses(
                (name, ipAddr, maskAddr) =>
                {
                    LinuxNetworkInterface lni = GetOrCreate(interfacesByName, name);
                    ProcessIpv4Address(lni, ipAddr, maskAddr);
                },
                (name, ipAddr, scopeId) =>
                {
                    LinuxNetworkInterface lni = GetOrCreate(interfacesByName, name);
                    ProcessIpv6Address(lni, ipAddr, *scopeId);
                },
                (name, llAddr) => 
                {
                    LinuxNetworkInterface lni = GetOrCreate(interfacesByName, name);
                    ProcessLinkLevelAddress(lni, llAddr);
                });

            return interfacesByName.Values.ToArray();
        }

        private static unsafe void ProcessIpv4Address(LinuxNetworkInterface lni,
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

            lni.AddAddress(ipAddress);
            lni._netMasks[ipAddress] = netMaskAddress;
        }

        private static unsafe void ProcessIpv6Address(LinuxNetworkInterface lni, 
                                                        Interop.Sys.IpAddressInfo* addressInfo,
                                                        uint scopeId)
        {
            byte[] ipBytes = new byte[addressInfo->NumAddressBytes];
            fixed (byte* ipArrayPtr = ipBytes)
            {
                Buffer.MemoryCopy(addressInfo->AddressBytes, ipArrayPtr, ipBytes.Length, ipBytes.Length);
            }
            IPAddress address = new IPAddress(ipBytes);

            lni.AddAddress(address);
            lni._ipv6ScopeId = scopeId;
        }

        private static unsafe void ProcessLinkLevelAddress(LinuxNetworkInterface lni, Interop.Sys.LinkLayerAddressInfo* llAddr)
        {
            byte[] macAddress = new byte[llAddr->NumAddressBytes];
            fixed (byte* macAddressPtr = macAddress)
            {
                Buffer.MemoryCopy(llAddr->AddressBytes, macAddressPtr, llAddr->NumAddressBytes, llAddr->NumAddressBytes);
            }
            PhysicalAddress physicalAddress = new PhysicalAddress(macAddress);

            lni._index = llAddr->InterfaceIndex;
            lni._physicalAddress = physicalAddress;
            lni._networkInterfaceType = MapArpHardwareType(llAddr->HardwareType);
        }

        // Adds any IPAddress to this interface's List of addresses.
        private void AddAddress(IPAddress ipAddress)
        {
            _addresses.Add(ipAddress);
        }

        /// <summary>
        /// Gets or creates a LinuxNetworkInterface, based on whether it already exists in the given Dictionary.
        /// If created, it is added to the Dictionary.
        /// </summary>
        /// <param name="interfaces">The Dictionary of existing interfaces.</param>
        /// <param name="name">The name of the interface.</param>
        /// <returns>The cached or new LinuxNetworkInterface with the given name.</returns>
        private static LinuxNetworkInterface GetOrCreate(Dictionary<string, LinuxNetworkInterface> interfaces, string name)
        {
            LinuxNetworkInterface lni;
            if (!interfaces.TryGetValue(name, out lni))
            {
                lni = new LinuxNetworkInterface(name);
                interfaces.Add(name, lni);
            }

            return lni;
        }

        public override string Name { get { return _name; } }

        public override NetworkInterfaceType NetworkInterfaceType { get { return _networkInterfaceType; } }

        public override bool SupportsMulticast { get { return GetSupportsMulticast(); } }

        private bool GetSupportsMulticast()
        {
            string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "flags");
            string fileContents = File.ReadAllText(path).Trim();
            LinuxNetDeviceFlags flags = (LinuxNetDeviceFlags)Convert.ToInt32(fileContents, 16);
            return (flags & LinuxNetDeviceFlags.IFF_MULTICAST) == LinuxNetDeviceFlags.IFF_MULTICAST;
        }

        public override IPInterfaceProperties GetIPProperties()
        {
            return new LinuxIPInterfaceProperties(this);
        }

        public override IPInterfaceStatistics GetIPStatistics()
        {
            return new LinuxIPInterfaceStatistics(_name);
        }

        public override IPv4InterfaceStatistics GetIPv4Statistics()
        {
            return new LinuxIpv4InterfaceStatisticsWrapper(_name);
        }

        public override PhysicalAddress GetPhysicalAddress()
        {
            Debug.Assert(_physicalAddress != null, "_physicalAddress was never initialized. This means no address with type AF_PACKET was discovered.");
            return _physicalAddress;
        }

        public override OperationalStatus OperationalStatus
        {
            get
            {
                // /sys/class/net/<name>/operstate
                string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "operstate");
                string state = File.ReadAllText(path).Trim();
                return MapState(state);
            }
        }

        // Maps values from /sys/class/net/<interface>/operstate to OperationStatus values.
        private OperationalStatus MapState(string state)
        {
            // TODO: Figure out the possible values that Linux might return.
            switch (state)
            {
                case "up":
                    return OperationalStatus.Up;
                case "down":
                    return OperationalStatus.Down;
                default:
                    return OperationalStatus.Unknown;
            }
        }

        public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            Sockets.AddressFamily family =
                (networkInterfaceComponent == NetworkInterfaceComponent.IPv4)
                ? Sockets.AddressFamily.InterNetwork
                : Sockets.AddressFamily.InterNetworkV6;

            return _addresses.Any(addr => addr.AddressFamily == family);
        }

        public override string Id { get { throw new PlatformNotSupportedException(); } }

        public override string Description { get { throw new PlatformNotSupportedException(); } }

        public override long Speed
        {
            get
            {
                try
                {
                    string path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _name, "speed");
                    string contents = File.ReadAllText(path);
                    long val;
                    if (long.TryParse(contents, out val))
                    {
                        return val;
                    }
                    else
                    {
                        throw new PlatformNotSupportedException();
                    }
                }
                catch (IOException) // Some interfaces may give an "Invalid argument" error when opening this file.
                {
                    throw new PlatformNotSupportedException();
                }
            }
        }

        public override bool IsReceiveOnly { get { throw new PlatformNotSupportedException(); } }

        public IPAddress GetNetMaskForIPv4Address(IPAddress address)
        {
            Debug.Assert(address.AddressFamily == Sockets.AddressFamily.InterNetwork);
            return _netMasks[address];
        }
    }
}
