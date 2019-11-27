﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Test.Common;

namespace System.Net.Security.Tests
{
    public static class TestHelper
    {
        public static (Stream, Stream) GetConnectedStreams()
        {
            if (Capability.SecurityForceSocketStreams())
            {
                return GetConnectedTcpStreams();
            }

            return GetConnectedVirtualStreams();
        }

        internal static (NetworkStream, NetworkStream) GetConnectedTcpStreams()
        {
            using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(listener.LocalEndPoint);
                Socket serverSocket = listener.Accept();

                return (new NetworkStream(serverSocket, ownsSocket: true), new NetworkStream(clientSocket, ownsSocket: true));
            }

        }

        internal static (VirtualNetworkStream, VirtualNetworkStream) GetConnectedVirtualStreams()
        {
            VirtualNetwork vn = new VirtualNetwork();

            return (new VirtualNetworkStream(vn, isServer: false), new VirtualNetworkStream(vn, isServer: true));
        }
    }
}
