// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv4InterfaceProperties : IPv4InterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly bool _isForwardingEnabled;
        private readonly int _mtu;

        public LinuxIPv4InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
            _isForwardingEnabled = GetIsForwardingEnabled();
            _mtu = GetMtu();
        }

        public override int Index
        {
            get
            {
                return _linuxNetworkInterface.Index;
            }
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

        public override bool IsForwardingEnabled { get { return _isForwardingEnabled; } }

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
                // TODO: There is a configuration option in /etc/samba.smb.conf:
                // # wins support = no (default value)
                throw new NotImplementedException();
            }
        }

        private bool GetIsForwardingEnabled()
        {
            // /proc/sys/net/ipv4/conf/<name>/forwarding
            string path = Path.Combine(LinuxNetworkFiles.Ipv4ConfigFolder, _linuxNetworkInterface.Name, LinuxNetworkFiles.ForwardingFileName);
            return int.Parse(File.ReadAllText(path)) == 1;
        }

        private int GetMtu()
        {
            string path = path = Path.Combine(LinuxNetworkFiles.SysClassNetFolder, _linuxNetworkInterface.Name, LinuxNetworkFiles.MtuFileName);
            return int.Parse(File.ReadAllText(path));
        }
    }
}
