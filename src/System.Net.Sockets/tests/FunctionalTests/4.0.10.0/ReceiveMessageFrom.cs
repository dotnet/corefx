namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;

    [TestClass]
    public class ReceiveMessageFrom
    {
        private const int TestPortBase = 8060;

        [TestMethod]
        public void Success()
        {
            if (Socket.OSSupportsIPv4)
            {
                using (Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    receiver.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase));
                    receiver.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sender.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    sender.SendTo(new byte[1024], new IPEndPoint(IPAddress.Loopback, TestPortBase));

                    IPPacketInformation packetInformation;
                    SocketFlags flags = SocketFlags.None;
                    EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                    int len = receiver.ReceiveMessageFrom(new byte[1024], 0, 1024, ref flags, ref remoteEP, out packetInformation);

                    Assert.AreEqual(1024, len, "Unexpected packet size");
                    Assert.AreEqual(sender.LocalEndPoint, remoteEP, "Unexpected sender");
                    Assert.AreEqual(((IPEndPoint)sender.LocalEndPoint).Address, packetInformation.Address, "Unexpected address in packetinfo");

                    sender.Dispose();
                }
            }
        }
    }
}
