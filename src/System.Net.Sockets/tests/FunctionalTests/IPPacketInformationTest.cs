// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class IPPacketInformationTest
    {
        [Fact]
        public void Equals_DefaultValues_Success()
        {
            Assert.Equal(default(IPPacketInformation), default(IPPacketInformation));
            Assert.True(default(IPPacketInformation) == default(IPPacketInformation));
            Assert.False(default(IPPacketInformation) != default(IPPacketInformation));
        }

        [Fact]
        public void GetHashCode_DefaultValues_Success()
        {
            Assert.Equal(default(IPPacketInformation).GetHashCode(), default(IPPacketInformation).GetHashCode());
        }

        [Fact]
        public void Equals_NonDefaultValue_Success()
        {
            IPPacketInformation packetInfo = GetNonDefaultIPPacketInformation();
            IPPacketInformation packetInfoCopy = packetInfo;

            Assert.Equal(packetInfo, packetInfoCopy);
            Assert.True(packetInfo == packetInfoCopy);
            Assert.False(packetInfo != packetInfoCopy);

            Assert.NotEqual(packetInfo, default(IPPacketInformation));
            Assert.False(packetInfo == default(IPPacketInformation));
            Assert.True(packetInfo != default(IPPacketInformation));
        }

        [Fact]
        public void GetHashCode_NonDefaultValue_Succes()
        {
            IPPacketInformation packetInfo = GetNonDefaultIPPacketInformation();

            Assert.Equal(packetInfo.GetHashCode(), packetInfo.GetHashCode());
        }

        private IPPacketInformation GetNonDefaultIPPacketInformation()
        {
            const int ReceiveTimeout = 5000;

            using (var receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                int port = receiver.BindToAnonymousPort(IPAddress.Loopback);

                var waitHandle = new ManualResetEvent(false);

                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs {
                    RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, port),
                    UserToken = waitHandle
                };

                receiveArgs.SetBuffer(new byte[1], 0, 1);
                receiveArgs.Completed += (_, args) => ((ManualResetEvent)args.UserToken).Set();

                Assert.True(receiver.ReceiveMessageFromAsync(receiveArgs));

                sender.SendTo(new byte[1], new IPEndPoint(IPAddress.Loopback, port));

                Assert.True(waitHandle.WaitOne(ReceiveTimeout));

                return receiveArgs.ReceiveMessageFromPacketInfo;
            }
        }
    }
}
