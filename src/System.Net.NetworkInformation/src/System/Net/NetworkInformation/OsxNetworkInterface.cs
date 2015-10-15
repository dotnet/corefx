// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace System.Net.NetworkInformation
{
    internal class OsxNetworkInterface : UnixNetworkInterface
    {
        protected OsxNetworkInterface(string name) : base(name) { }

        public unsafe static NetworkInterface[] GetOsxNetworkInterfaces()
        {
            Dictionary<string, OsxNetworkInterface> interfacesByName = new Dictionary<string, OsxNetworkInterface>();
            if (Interop.Sys.EnumerateInterfaceAddresses(
                (name, ipAddr, maskAddr) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    ProcessIpv4Address(oni, ipAddr, maskAddr);
                },
                (name, ipAddr, scopeId) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    ProcessIpv6Address(oni, ipAddr, *scopeId);
                },
                (name, llAddr) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    ProcessLinkLayerAddress(oni, llAddr);
                }) != 0)
            {
                throw new NetworkInformationException((int)Interop.Sys.GetLastError());
            }

            return interfacesByName.Values.ToArray();
        }

        /// <summary>
        /// Gets or creates an OsxNetworkInterface, based on whether it already exists in the given Dictionary.
        /// If created, it is added to the Dictionary.
        /// </summary>
        /// <param name="interfaces">The Dictionary of existing interfaces.</param>
        /// <param name="name">The name of the interface.</param>
        /// <returns>The cached or new OsxNetworkInterface with the given name.</returns>
        private static OsxNetworkInterface GetOrCreate(Dictionary<string, OsxNetworkInterface> interfaces, string name)
        {
            OsxNetworkInterface oni;
            if (!interfaces.TryGetValue(name, out oni))
            {
                oni = new OsxNetworkInterface(name);
                interfaces.Add(name, oni);
            }

            return oni;
        }

        public override IPInterfaceProperties GetIPProperties()
        {
            return new OsxIpInterfaceProperties(this);
        }

        public override IPInterfaceStatistics GetIPStatistics()
        {
            return new OsxIpInterfaceStatistics(_name);
        }

        public override OperationalStatus OperationalStatus
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Description { get { throw new PlatformNotSupportedException(); } }

        public override bool SupportsMulticast
        {
            get
            {
                return base.SupportsMulticast;
            }
        }

        public override bool IsReceiveOnly { get { throw new PlatformNotSupportedException(); } }
    }
}
