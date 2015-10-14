// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv6InterfaceProperties : UnixIPv6InterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly int _mtu;

        public LinuxIPv6InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
            : base(linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
            _mtu = GetMtu();
        }

        public override int Mtu
        {
            get
            {
                return _mtu;
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            throw new PlatformNotSupportedException();
        }

        private int GetMtu()
        {
            // /proc/sys/net/ipv6/conf/<name>/mtu
            string path = Path.Combine(LinuxNetworkFiles.Ipv6ConfigFolder, _linuxNetworkInterface.Name, LinuxNetworkFiles.MtuFileName);
            return int.Parse(File.ReadAllText(path));
        }
    }
}
