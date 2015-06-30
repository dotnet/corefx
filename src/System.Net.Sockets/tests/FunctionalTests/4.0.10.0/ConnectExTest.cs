namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [TestClass]
    public class ConnectExTest
    {
        private const int TestPortBase = 8030;

        private static void OnConnectAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            ManualResetEvent complete = (ManualResetEvent)args.UserToken;
            complete.Set();
        }

        [TestMethod]
        public void Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, TestPortBase));
            SocketTestServer server6 = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase));

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.IsTrue(sock.ConnectAsync(args));

            Assert.IsTrue(complete.WaitOne(5000), "IPv4: Timed out while waiting for connection");

            Assert.IsTrue(args.SocketError == SocketError.Success);

            sock.Dispose();

            sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            args.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase);
            complete.Reset();

            Assert.IsTrue(sock.ConnectAsync(args));

            Assert.IsTrue(complete.WaitOne(5000), "IPv6: Timed out while waiting for connection");

            Assert.IsTrue(args.SocketError == SocketError.Success);

            sock.Dispose();

            server.Dispose();
            server6.Dispose();
        }

        #region GC Finalizer test
        // This test assumes sequential execution of tests and that it is going to be executed after other tests
        // that used Sockets. 
        [TestMethod]
        public void TestFinalizers()
        {
            // Making several passes through the FReachable list.
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        #endregion 
    }
}
