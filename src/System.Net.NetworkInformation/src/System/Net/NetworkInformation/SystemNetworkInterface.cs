

using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Win32.SafeHandles;

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


namespace System.Net.NetworkInformation
{
    internal class SystemNetworkInterface : NetworkInterface
    {
        //common properties
        private string _name;
        private string _id;
        private string _description;
        private byte[] _physicalAddress;
        private uint _addressLength;
        private NetworkInterfaceType _type;
        private OperationalStatus _operStatus;
        private long _speed;
        //Unfortunately, any interface can
        //have two completely different valid indexes for ipv4 and ipv6
        private uint _index = 0;
        private uint _ipv6Index = 0;
        private Interop.IpHlpApi.AdapterFlags _adapterFlags;
        private SystemIPInterfaceProperties _interfaceProperties = null;

        internal static int InternalLoopbackInterfaceIndex
        {
            get
            {
                return GetBestInterfaceForAddress(IPAddress.Loopback);
            }
        }

        internal static int InternalIPv6LoopbackInterfaceIndex
        {
            get
            {
                return GetBestInterfaceForAddress(IPAddress.IPv6Loopback);
            }
        }

        private static int GetBestInterfaceForAddress(IPAddress addr)
        {
            int index;
            Internals.SocketAddress address = new Internals.SocketAddress(addr);
            int error = (int)Interop.IpHlpApi.GetBestInterfaceEx(address.Buffer, out index);
            if (error != 0)
            {
                throw new NetworkInformationException(error);
            }

            return index;
        }

        internal static bool InternalGetIsNetworkAvailable()
        {
            try
            {
                NetworkInterface[] networkInterfaces = GetNetworkInterfaces();
                foreach (NetworkInterface netInterface in networkInterfaces)
                {
                    if (netInterface.OperationalStatus == OperationalStatus.Up && netInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                        && netInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        return true;
                    }
                }
            }
            catch (NetworkInformationException nie)
            {
                if (Logging.On) Logging.Exception(Logging.Web, "SystemNetworkInterface", "InternalGetIsNetworkAvailable", nie);
            }

            return false;
        }

        internal static NetworkInterface[] GetNetworkInterfaces()
        {
            Contract.Ensures(Contract.Result<NetworkInterface[]>() != null);
            AddressFamily family = AddressFamily.Unspecified;
            uint bufferSize = 0;
            SafeLocalAllocHandle buffer = null;

            // TODO: #2485: This will probably require 
            Interop.IpHlpApi.FIXED_INFO fixedInfo = HostInformationPal.GetFixedInfo();
            List<SystemNetworkInterface> interfaceList = new List<SystemNetworkInterface>();

            Interop.IpHlpApi.GetAdaptersAddressesFlags flags =
                Interop.IpHlpApi.GetAdaptersAddressesFlags.IncludeGateways
                | Interop.IpHlpApi.GetAdaptersAddressesFlags.IncludeWins;

            // Figure out the right buffer size for the adapter information
            uint result = Interop.IpHlpApi.GetAdaptersAddresses(
                family, (uint)flags, IntPtr.Zero, SafeLocalAllocHandle.Zero, ref bufferSize);

            while (result == Interop.IpHlpApi.ERROR_BUFFER_OVERFLOW)
            {
                try
                {
                    // Allocate the buffer and get the adapter info
                    buffer = SafeLocalAllocHandle.LocalAlloc((int)bufferSize);
                    result = Interop.IpHlpApi.GetAdaptersAddresses(
                        family, (uint)flags, IntPtr.Zero, buffer, ref bufferSize);

                    // If succeeded, we're going to add each new interface
                    if (result == Interop.IpHlpApi.ERROR_SUCCESS)
                    {
                        // Linked list of interfaces
                        IntPtr ptr = buffer.DangerousGetHandle();
                        while (ptr != IntPtr.Zero)
                        {
                            // Traverse the list, marshal in the native structures, and create new NetworkInterfaces
                            Interop.IpHlpApi.IpAdapterAddresses adapterAddresses = Marshal.PtrToStructure<Interop.IpHlpApi.IpAdapterAddresses>(ptr);
                            interfaceList.Add(new SystemNetworkInterface(fixedInfo, adapterAddresses));

                            ptr = adapterAddresses.next;
                        }
                    }
                }
                finally
                {
                    if (buffer != null)
                    {
                        buffer.Dispose();
                    }
                    buffer = null;
                }
            }

            // if we don't have any interfaces detected, return empty.
            if (result == Interop.IpHlpApi.ERROR_NO_DATA || result == Interop.IpHlpApi.ERROR_INVALID_PARAMETER)
            {
                return new SystemNetworkInterface[0];
            }

            // Otherwise we throw on an error
            if (result != Interop.IpHlpApi.ERROR_SUCCESS)
            {
                throw new NetworkInformationException((int)result);
            }

            return interfaceList.ToArray();
        }

        // Vista+
        internal SystemNetworkInterface(Interop.IpHlpApi.FIXED_INFO fixedInfo, Interop.IpHlpApi.IpAdapterAddresses ipAdapterAddresses)
        {
            //store the common api information
            _id = ipAdapterAddresses.AdapterName;
            _name = ipAdapterAddresses.friendlyName;
            _description = ipAdapterAddresses.description;
            _index = ipAdapterAddresses.index;

            _physicalAddress = ipAdapterAddresses.address;
            _addressLength = ipAdapterAddresses.addressLength;

            _type = ipAdapterAddresses.type;
            _operStatus = ipAdapterAddresses.operStatus;
            _speed = (long)ipAdapterAddresses.receiveLinkSpeed;

            //api specific info
            _ipv6Index = ipAdapterAddresses.ipv6Index;

            _adapterFlags = ipAdapterAddresses.flags;
            _interfaceProperties = new SystemIPInterfaceProperties(fixedInfo, ipAdapterAddresses);
        }

        /// Basic Properties

        public override string Id { get { return _id; } }
        public override string Name { get { return _name; } }
        public override string Description { get { return _description; } }

        public override PhysicalAddress GetPhysicalAddress()
        {
            byte[] newAddr = new byte[_addressLength];

            // Array.Copy only supports int and long while addressLength is uint (see IpAdapterAddresses).
            // Will throw OverflowException if addressLength > Int32.MaxValue
            Array.Copy(_physicalAddress, newAddr, checked((int)_addressLength));
            return new PhysicalAddress(newAddr);
        }
        public override NetworkInterfaceType NetworkInterfaceType { get { return _type; } }

        public override IPInterfaceProperties GetIPProperties()
        {
            return _interfaceProperties;
        }

        /// Despite the naming, the results are not IPv4 specific.
        /// Do not use this method, use GetIPStatistics instead.
        /// <include file='doc\NetworkInterface.uex' path='docs/doc[@for="NetworkInterface.GetInterfaceStatistics"]/*' />
        public override IPv4InterfaceStatistics GetIPv4Statistics()
        {
            return new SystemIPv4InterfaceStatistics(_index);
        }

        public override IPInterfaceStatistics GetIPStatistics()
        {
            return new SystemIPInterfaceStatistics(_index);
        }

        public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            if (networkInterfaceComponent == NetworkInterfaceComponent.IPv6
                && ((_adapterFlags & Interop.IpHlpApi.AdapterFlags.IPv6Enabled) != 0))
            {
                return true;
            }
            if (networkInterfaceComponent == NetworkInterfaceComponent.IPv4
                && ((_adapterFlags & Interop.IpHlpApi.AdapterFlags.IPv4Enabled) != 0))
            {
                return true;
            }
            return false;
        }

        //We cache this to be consistent across all platforms
        public override OperationalStatus OperationalStatus
        {
            get
            {
                return _operStatus;
            }
        }

        public override long Speed
        {
            get
            {
                return _speed;
            }
        }

        public override bool IsReceiveOnly
        {
            get
            {
                return ((_adapterFlags & Interop.IpHlpApi.AdapterFlags.ReceiveOnly) > 0);
            }
        }
        /// <summary>The interface doesn't allow multicast.</summary>
        public override bool SupportsMulticast
        {
            get
            {
                return ((_adapterFlags & Interop.IpHlpApi.AdapterFlags.NoMulticast) == 0);
            }
        }
    }
}



