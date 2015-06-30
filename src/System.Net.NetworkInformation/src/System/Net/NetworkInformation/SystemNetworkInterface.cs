

using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private AdapterFlags _adapterFlags;
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
            SocketAddress address = new SocketAddress(addr);
            int error = (int)UnsafeNetInfoNativeMethods.GetBestInterfaceEx(address.m_Buffer, out index);
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

        // Vista+
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods",
            Justification = "DangerousGetHandle is required for marshaling")]
        internal static NetworkInterface[] GetNetworkInterfaces()
        {
            Contract.Ensures(Contract.Result<NetworkInterface[]>() != null);
            AddressFamily family = AddressFamily.Unspecified;
            uint bufferSize = 0;
            SafeLocalFree buffer = null;
            FIXED_INFO fixedInfo = HostInformation.GetFixedInfo();
            List<SystemNetworkInterface> interfaceList = new List<SystemNetworkInterface>();

            GetAdaptersAddressesFlags flags =
                GetAdaptersAddressesFlags.IncludeGateways
                | GetAdaptersAddressesFlags.IncludeWins;

            // Figure out the right buffer size for the adapter information
            uint result = UnsafeNetInfoNativeMethods.GetAdaptersAddresses(
                family, (uint)flags, IntPtr.Zero, SafeLocalFree.Zero, ref bufferSize);

            while (result == IpHelperErrors.ErrorBufferOverflow)
            {
                try
                {
                    // Allocate the buffer and get the adapter info
                    buffer = SafeLocalFree.LocalAlloc((int)bufferSize);
                    result = UnsafeNetInfoNativeMethods.GetAdaptersAddresses(
                        family, (uint)flags, IntPtr.Zero, buffer, ref bufferSize);

                    // If succeeded, we're going to add each new interface
                    if (result == IpHelperErrors.Success)
                    {
                        // Linked list of interfaces
                        IntPtr ptr = buffer.DangerousGetHandle();
                        while (ptr != IntPtr.Zero)
                        {
                            // Traverse the list, marshal in the native structures, and create new NetworkInterfaces
                            IpAdapterAddresses adapterAddresses = Marshal.PtrToStructure<IpAdapterAddresses>(ptr);
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
            if (result == IpHelperErrors.ErrorNoData || result == IpHelperErrors.ErrorInvalidParameter)
            {
                return new SystemNetworkInterface[0];
            }

            // Otherwise we throw on an error
            if (result != IpHelperErrors.Success)
            {
                throw new NetworkInformationException((int)result);
            }

            return interfaceList.ToArray();
        }

        // Vista+
        internal SystemNetworkInterface(FIXED_INFO fixedInfo, IpAdapterAddresses ipAdapterAddresses)
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
                && ((_adapterFlags & AdapterFlags.IPv6Enabled) != 0))
            {
                return true;
            }
            if (networkInterfaceComponent == NetworkInterfaceComponent.IPv4
                && ((_adapterFlags & AdapterFlags.IPv4Enabled) != 0))
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
                return ((_adapterFlags & AdapterFlags.ReceiveOnly) > 0);
            }
        }
        /// <summary>The interface doesn't allow multicast.</summary>
        public override bool SupportsMulticast
        {
            get
            {
                return ((_adapterFlags & AdapterFlags.NoMulticast) == 0);
            }
        }
    }
}



