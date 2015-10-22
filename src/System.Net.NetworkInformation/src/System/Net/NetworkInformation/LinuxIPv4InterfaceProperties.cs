﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace System.Net.NetworkInformation
{
    internal class LinuxIPv4InterfaceProperties : UnixIPv4InterfaceProperties
    {
        private readonly LinuxNetworkInterface _linuxNetworkInterface;
        private readonly bool _isForwardingEnabled;
        private readonly int _mtu;

        public LinuxIPv4InterfaceProperties(LinuxNetworkInterface linuxNetworkInterface)
            : base(linuxNetworkInterface)
        {
            _linuxNetworkInterface = linuxNetworkInterface;
            _isForwardingEnabled = GetIsForwardingEnabled();
            _mtu = GetMtu();
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
                return _linuxNetworkInterface.GetIPProperties().WinsServersAddresses.Count > 0;
            }
        }

        private bool GetIsForwardingEnabled()
        {
            // /proc/sys/net/ipv4/conf/<name>/forwarding
            string path = Path.Combine(NetworkFiles.Ipv4ConfigFolder, _linuxNetworkInterface.Name, NetworkFiles.ForwardingFileName);
            return int.Parse(File.ReadAllText(path)) == 1;
        }

        private int GetMtu()
        {
            string path = path = Path.Combine(NetworkFiles.SysClassNetFolder, _linuxNetworkInterface.Name, NetworkFiles.MtuFileName);
            return int.Parse(File.ReadAllText(path));
        }
    }
}
