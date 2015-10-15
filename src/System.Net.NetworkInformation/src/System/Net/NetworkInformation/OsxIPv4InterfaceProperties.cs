// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class OsxIPv4InterfaceProperties : UnixIPv4InterfaceProperties
    {
        public OsxIPv4InterfaceProperties(OsxNetworkInterface oni)
            : base(oni)
        {
        }

        public override bool IsAutomaticPrivateAddressingActive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsAutomaticPrivateAddressingEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsDhcpEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsForwardingEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Mtu
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool UsesWins
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}