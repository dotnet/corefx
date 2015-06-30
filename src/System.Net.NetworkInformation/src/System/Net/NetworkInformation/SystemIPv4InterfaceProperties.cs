
using System.Net;
using System;
using System.Runtime.InteropServices;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


namespace System.Net.NetworkInformation
{
    internal class SystemIPv4InterfaceProperties : IPv4InterfaceProperties
    {
        //these are only valid for ipv4 interfaces
        private bool _haveWins = false;
        private bool _dhcpEnabled = false;
        private bool _routingEnabled = false;
        private bool _autoConfigEnabled = false;
        private bool _autoConfigActive = false;
        private uint _index = 0;
        private uint _mtu = 0;

        // Vista+
        internal SystemIPv4InterfaceProperties(FIXED_INFO fixedInfo, IpAdapterAddresses ipAdapterAddresses)
        {
            _index = ipAdapterAddresses.index;
            _routingEnabled = fixedInfo.enableRouting;
            _dhcpEnabled = ((ipAdapterAddresses.flags & AdapterFlags.DhcpEnabled) != 0);
            _haveWins = (ipAdapterAddresses.firstWinsServerAddress != IntPtr.Zero);

            _mtu = ipAdapterAddresses.mtu;

            GetPerAdapterInfo(ipAdapterAddresses.index);
        }

        /// <summary>Only valid for Ipv4 Uses WINS for name resolution.</summary>
        public override bool UsesWins { get { return _haveWins; } }


        public override bool IsDhcpEnabled
        {
            get { return _dhcpEnabled; }
        }

        public override bool IsForwardingEnabled { get { return _routingEnabled; } }      //proto



        /// <summary>Auto configuration of an ipv4 address for a client
        /// on a network where a DHCP server
        /// isn't available.</summary>
        public override bool IsAutomaticPrivateAddressingEnabled
        {
            get
            {
                return _autoConfigEnabled;
            }
        } // proto

        public override bool IsAutomaticPrivateAddressingActive
        {
            get
            {
                return _autoConfigActive;
            }
        }


        /// <summary>Specifies the Maximum transmission unit in bytes. Uses GetIFEntry.</summary>
        //We cache this to be consistent across all platforms
        public override int Mtu
        {
            get
            {
                return (int)_mtu;
            }
        }

        public override int Index
        {
            get
            {
                return (int)_index;
            }
        }

        private void GetPerAdapterInfo(uint index)
        {
            if (index != 0)
            {
                uint size = 0;
                SafeLocalFree buffer = null;

                uint result = UnsafeNetInfoNativeMethods.GetPerAdapterInfo(index, SafeLocalFree.Zero, ref size);
                while (result == IpHelperErrors.ErrorBufferOverflow)
                {
                    try
                    {
                        //now we allocate the buffer and read the network parameters.
                        buffer = SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetPerAdapterInfo(index, buffer, ref size);
                        if (result == IpHelperErrors.Success)
                        {
                            IpPerAdapterInfo ipPerAdapterInfo = Marshal.PtrToStructure<IpPerAdapterInfo>(buffer.DangerousGetHandle());

                            _autoConfigEnabled = ipPerAdapterInfo.autoconfigEnabled;
                            _autoConfigActive = ipPerAdapterInfo.autoconfigActive;
                        }
                    }
                    finally
                    {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }

                if (result != IpHelperErrors.Success)
                {
                    throw new NetworkInformationException((int)result);
                }
            }
        }
    }
}
