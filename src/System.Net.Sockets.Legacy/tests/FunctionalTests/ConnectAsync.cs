namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    
    [TestClass]
    public class ConnectAsync
    {
        private const int TestPortBase = 8020;

        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();            
        }

        [TestMethod]
        public void Success()
        {
            AutoResetEvent completed = new AutoResetEvent(false);

            if (Socket.OSSupportsIPv4)
            {
                using (SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, TestPortBase)))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
                    args.Completed += OnConnectCompleted;
                    args.UserToken = completed;

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Assert.IsTrue(client.ConnectAsync(args));

                    Assert.IsTrue(completed.WaitOne(5000), "IPv4: Timed out while waiting for connection");

                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "Failed to Connect");

                    client.Dispose();
                }
            }

            if (Socket.OSSupportsIPv6)
            {
                using (SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase)))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase);
                    args.Completed += OnConnectCompleted;
                    args.UserToken = completed;

                    Socket client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    Assert.IsTrue(client.ConnectAsync(args));

                    Assert.IsTrue(completed.WaitOne(5000), "IPv6: Timed out while waiting for connection");

                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "Failed to Connect");

                    client.Dispose();
                }
            }
        }
    }
}
