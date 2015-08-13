namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [TestClass]
    public class ReceiveMessageFromAsync
    {
        private const int TestPortBase = 8090;

        public void OnCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [TestMethod]
        public void Success()
        {
            ManualResetEvent completed = new ManualResetEvent(false);

            if (Socket.OSSupportsIPv4)
            {
                using (Socket receiver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    receiver.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase));
                    receiver.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                    Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sender.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    sender.SendTo(new byte[1024], new IPEndPoint(IPAddress.Loopback, TestPortBase));
                    
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    args.SetBuffer(new byte[1024], 0, 1024);
                    args.Completed += OnCompleted;
                    args.UserToken = completed;

                    Assert.IsTrue(receiver.ReceiveMessageFromAsync(args));

                    Assert.IsTrue(completed.WaitOne(5000), "Timeout while waiting for connection");

                    Assert.AreEqual(1024, args.BytesTransferred, "Unexpected packet size");
                    Assert.AreEqual(sender.LocalEndPoint, args.RemoteEndPoint, "Unexpected sender");
                    Assert.AreEqual(((IPEndPoint)sender.LocalEndPoint).Address, args.ReceiveMessageFromPacketInfo.Address, "Unexpected address in packetinfo");

                    sender.Dispose();
                }
            }
        }
    }
}
