﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Test.Common;
using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class UdpClientTest
    {
        // Port 8 is unassigned as per https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.txt
        private const int UnusedPort = 8;

        private ManualResetEvent _waitHandle = new ManualResetEvent(false);

        private void AsyncCompleted(IAsyncResult ar)
        {
            UdpClient udpService = (UdpClient)ar.AsyncState;
            udpService.EndSend(ar);
            _waitHandle.Set();
        }

        [Fact]
        public void BeginSend_NegativeBytes_Throws()
        {
            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, UnusedPort);

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () =>
            {
                udpClient.BeginSend(sendBytes, -1, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);
            });
        }

        [Fact]
        public void BeginSend_BytesMoreThanArrayLength_Throws()
        {
            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, UnusedPort);

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () =>
            {
                udpClient.BeginSend(sendBytes, sendBytes.Length + 1, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);
            });
        }

        [Fact]
        public void BeginSend_Success()
        {
            UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork);
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, UnusedPort);
            _waitHandle.Reset();
            udpClient.BeginSend(sendBytes, sendBytes.Length, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);

            Assert.True(_waitHandle.WaitOne(5000), "Timed out while waiting for connection");
        }
    }
}
