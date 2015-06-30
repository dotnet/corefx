// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/// <summary><para>
///    Provides support for ip configuation information and statistics.
///</para></summary>
///


namespace System.Net.NetworkInformation
{
    internal class SystemIPv6InterfaceProperties : IPv6InterfaceProperties
    {
        uint index = 0;
        uint mtu = 0;
        uint[] zoneIndices;

        internal SystemIPv6InterfaceProperties(uint index, uint mtu, uint[] zoneIndices)
        {
            this.index = index;
            this.mtu = mtu;
            this.zoneIndices = zoneIndices;
        }
        /// <summary>Specifies the Maximum transmission unit in bytes. Uses GetIFEntry.</summary>
        //We cache this to be consistent across all platforms
        public override int Index
        {
            get
            {
                return (int)index;
            }
        }
        /// <summary>Specifies the Maximum transmission unit in bytes. Uses GetIFEntry.</summary>
        //We cache this to be consistent across all platforms
        public override int Mtu
        {
            get
            {
                return (int)mtu;
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            if ((scopeLevel < 0) || ((int)scopeLevel >= zoneIndices.Length))
            {
                throw new ArgumentOutOfRangeException("scopeLevel");
            }

            return zoneIndices[(int)scopeLevel];
        }
    }
}
