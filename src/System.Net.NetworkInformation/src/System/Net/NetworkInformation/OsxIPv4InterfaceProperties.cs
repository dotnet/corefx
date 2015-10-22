﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxIPv4InterfaceProperties : UnixIPv4InterfaceProperties
    {
        private readonly int _mtu;

        public OsxIPv4InterfaceProperties(OsxNetworkInterface oni, int mtu)
            : base(oni)
        {
            _mtu = mtu;
        }

        public override bool IsAutomaticPrivateAddressingActive
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsAutomaticPrivateAddressingEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsDhcpEnabled
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public override bool IsForwardingEnabled
        {
            get
            {
                // Doesn't seem to be exposed on a per-interface basis.
                throw new PlatformNotSupportedException();
            }
        }

        public override int Mtu
        {
            get
            {
                return _mtu;
            }
        }

        public override bool UsesWins
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}