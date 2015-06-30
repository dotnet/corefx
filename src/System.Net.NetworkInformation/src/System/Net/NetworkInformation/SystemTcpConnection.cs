// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using System.Threading;

namespace System.Net.NetworkInformation
{
    /// <summary>
    /// Represents an active Tcp connection.</summary>
    internal class SystemTcpConnectionInformation : TcpConnectionInformation
    {
        private IPEndPoint _localEndPoint;
        private IPEndPoint _remoteEndPoint;
        private TcpState _state;

        internal SystemTcpConnectionInformation(MibTcpRow row)
        {
            _state = row.state;

            //port is returned in Big-Endian - most significant bit on left
            //unfortunately, its done at the word level and not the dword level.

            int localPort = row.localPort1 << 8 | row.localPort2;
            int remotePort = ((_state == TcpState.Listen) ? 0 : row.remotePort1 << 8 | row.remotePort2);

            _localEndPoint = new IPEndPoint(row.localAddr, (int)localPort);
            _remoteEndPoint = new IPEndPoint(row.remoteAddr, (int)remotePort);
        }

        // IPV6 version of the Tcp row 
        internal SystemTcpConnectionInformation(MibTcp6RowOwnerPid row)
        {
            _state = row.state;

            //port is returned in Big-Endian - most significant bit on left
            //unfortunately, its done at the word level and not the dword level.

            int localPort = row.localPort1 << 8 | row.localPort2;
            int remotePort = ((_state == TcpState.Listen) ? 0 : row.remotePort1 << 8 | row.remotePort2);

            _localEndPoint = new IPEndPoint(new IPAddress(row.localAddr, row.localScopeId), (int)localPort);
            _remoteEndPoint = new IPEndPoint(new IPAddress(row.remoteAddr, row.remoteScopeId), (int)remotePort);
        }


        public override TcpState State { get { return _state; } }
        public override IPEndPoint LocalEndPoint { get { return _localEndPoint; } }
        public override IPEndPoint RemoteEndPoint { get { return _remoteEndPoint; } }
    }
}

