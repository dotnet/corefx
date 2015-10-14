// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv6InterfaceProperties : IPv6InterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly int _mtu;

        public LinuxIPv6InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
            _mtu = GetMtu();
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
                return _mtu;
            }
        }

        public override long GetScopeId(ScopeLevel scopeLevel)
        {
            throw new PlatformNotSupportedException();
        }

        private int GetMtu()
        {
            string path = path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _linuxNetworkInterface.Name, LinuxNetworkFiles.MtuFileName);
            return int.Parse(File.ReadAllText(path));
        }
    }
}
