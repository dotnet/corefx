// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv6InterfaceProperties : IPv6InterfaceProperties
    {
        private LinuxNetworkInterface _linuxNetworkInterface;

        public LinuxIPv6InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
        }

        public override int Index
        {
            get
            {
                return _linuxNetworkInterface.Index;
            }
        }

        public override int Mtu
        {
            get
            {
                // /proc/sys/net/ipv6/conf/<name>/mtu
                string path = Path.Combine(LinuxNetworkFiles.Ipv6ConfigFolder, _linuxNetworkInterface.Name, LinuxNetworkFiles.MtuFileName);
                return int.Parse(File.ReadAllText(path));
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
