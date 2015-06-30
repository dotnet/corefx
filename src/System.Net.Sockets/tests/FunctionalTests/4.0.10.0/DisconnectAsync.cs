namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [TestClass]
    public class DisconnectAsync
    {
        private const int TestPortBase = 8050;

        public void OnCompleted(object sender, SocketAsyncEventArgs args)
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
                using (SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, TestPortBase + 1)))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnCompleted;
                    args.UserToken = completed;
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
                    args.DisconnectReuseSocket = true;

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    Assert.IsTrue(client.ConnectAsync(args));
                    Assert.IsTrue(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "Initial Connect attempt fails with " + args.SocketError);

                    Assert.IsTrue(client.DisconnectAsync(args));
                    Assert.IsTrue(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "Disconnect fails with " + args.SocketError);

                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 1);

                    Assert.IsTrue(client.ConnectAsync(args));
                    Assert.IsTrue(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "reconnect failed with " + args.SocketError);

                    client.Dispose();
                }
            }
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
