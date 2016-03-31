// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace System.Net.NetworkInformation
{
    internal class OsxNetworkInterface : UnixNetworkInterface
    {
        private readonly OsxIpInterfaceProperties _ipProperties;
        private readonly long _speed;

        protected unsafe OsxNetworkInterface(string name) : base(name)
        {
            Interop.Sys.NativeIPInterfaceStatistics nativeStats;
            if (Interop.Sys.GetNativeIPInterfaceStatistics(name, out nativeStats) == -1)
            {
                throw new NetworkInformationException(SR.net_PInvokeError);
            }

            _speed = (long)nativeStats.Speed;
            _ipProperties = new OsxIpInterfaceProperties(this, (int)nativeStats.Mtu);
        }

        public unsafe static NetworkInterface[] GetOsxNetworkInterfaces()
        {
            Dictionary<string, OsxNetworkInterface> interfacesByName = new Dictionary<string, OsxNetworkInterface>();
            if (Interop.Sys.EnumerateInterfaceAddresses(
                (name, ipAddr, maskAddr) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    oni.ProcessIpv4Address(ipAddr, maskAddr);
                },
                (name, ipAddr, scopeId) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    oni.ProcessIpv6Address(ipAddr, *scopeId);
                },
                (name, llAddr) =>
                {
                    OsxNetworkInterface oni = GetOrCreate(interfacesByName, name);
                    oni.ProcessLinkLayerAddress(llAddr);
                }) != 0)
            {
                throw new NetworkInformationException(SR.net_PInvokeError);
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
            return _ipProperties;
        }

        public override IPInterfaceStatistics GetIPStatistics()
        {
            return new OsxIpInterfaceStatistics(Name);
        }

        public override OperationalStatus OperationalStatus
        {
            get
            {
                // TODO: This is a crude approximation, but does allow us to determine
                // whether an interface is operational or not. The OS exposes more information
                // (see ifconfig and the "Status" label), but it's unclear how closely
                // that information maps to the OperationalStatus enum we expose here.
                return Addresses.Count > 0 ? OperationalStatus.Up : OperationalStatus.Unknown;
            }
        }

        public override long Speed { get { return _speed; } }

        public override string Description { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }

        public override bool SupportsMulticast { get { return _ipProperties.MulticastAddresses.Count > 0; } }

        public override bool IsReceiveOnly { get { throw new PlatformNotSupportedException(SR.net_InformationUnavailableOnPlatform); } }
    }
}
