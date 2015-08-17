using Xunit;

namespace System.Net.Sockets.Tests
{
    public class ReceiveMessageFrom
    {
        private const int TestPortBase = 8060;

        [Fact]
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

                    Assert.Equal(1024, len);
                    Assert.Equal(sender.LocalEndPoint, remoteEP);
                    Assert.Equal(((IPEndPoint)sender.LocalEndPoint).Address, packetInformation.Address);

                    sender.Dispose();
                }
            }
        }
    }
}
