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
                // We could call if_nametoindex(name), if we wanted.
                return _linuxNetworkInterface.Index;
            }
        }

        public override int Mtu
        {
            get
            {
                // /proc/sys/net/ipv6/conf/<name>/mtu
                string path = Path.Combine(LinuxNetworkFiles.ProcSysNetFolder, "ipv6", "conf", _linuxNetworkInterface.Name, "mtu");
                return int.Parse(File.ReadAllText(path));
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
