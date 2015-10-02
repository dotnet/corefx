﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class ReceiveMessageFrom
    {
        // This is a stand-in for an issue to be filed when this code is merged into corefx.
        private const int DummyOSXPacketInfoIssue = 123456;

        [Fact]
        [ActiveIssue(DummyOSXPacketInfoIssue, PlatformID.OSX)]
        public void Success()
        {
            if (Socket.OSSupportsIPv4)
            {
                using (Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    int port = receiver.BindToAnonymousPort(IPAddress.Loopback);
                    receiver.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sender.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    sender.SendTo(new byte[1024], new IPEndPoint(IPAddress.Loopback, port));

                    IPPacketInformation packetInformation;
                    SocketFlags flags = SocketFlags.None;
                    EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                    int len = receiver.ReceiveMessageFrom(new byte[1024], 0, 1024, ref flags, ref remoteEP, out packetInformation);

                    Assert.Equal(1024, len);
                    Assert.Equal(sender.LocalEndPoint, remoteEP);
                    Assert.Equal(((IPEndPoint)sender.LocalEndPoint).Address, packetInformation.Address);

                    sender.Dispose();
                }
            }
        }
    }
}
