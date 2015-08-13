namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    [TestClass]
    public class AcceptAsync
    {
        private const int TestPortBase = 8000;

        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            Console.WriteLine("OnAcceptCompleted event handler");
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [TestMethod]
        public void Success()
        {
            AutoResetEvent completed = new AutoResetEvent(false);

            if (Socket.OSSupportsIPv4)
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase));
                    sock.Listen(1);
                    
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnAcceptCompleted;
                    args.UserToken = completed;

                    // Not expecting the operation to finish synchronously as no client should be trying to connect.
                    Assert.IsTrue(sock.AcceptAsync(args));
                    Console.WriteLine("IPv4 Server: Waiting for clients.");

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(new IPEndPoint(IPAddress.Loopback, TestPortBase));

                    Console.WriteLine("IPv4 Client: Connecting.");
                    Assert.IsTrue(completed.WaitOne(5000), "IPv4: Timed out while waiting for connection");

                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "IPv4 Accept failed with " + args.SocketError);
                    Assert.IsNotNull(args.AcceptSocket, "IPv4 Accept socket was null");
                    Assert.IsTrue(args.AcceptSocket.Connected, "IPv4 Accept Socket was not connected");

                    client.Dispose();
                }
            }

            if (Socket.OSSupportsIPv6)
            {
                using (Socket sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase));
                    sock.Listen(1);

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnAcceptCompleted;
                    args.UserToken = completed;

                    Assert.IsTrue(sock.AcceptAsync(args));
                    Console.WriteLine("IPv6 Server: Waiting for clients.");

                    Socket client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase));
                    
                    Console.WriteLine("IPv6 Client: Connecting.");
                    Assert.IsTrue(completed.WaitOne(5000), "IPv6: Timed out while waiting for connection");

                    Assert.AreEqual<SocketError>(SocketError.Success, args.SocketError, "IPv6 Accept failed with " + args.SocketError);
                    Assert.IsNotNull(args.AcceptSocket, "IPv6 Accept socket was null");
                    Assert.IsTrue(args.AcceptSocket.Connected, "IPv6 Accept Socket was not connected");
                    //Assert.IsNotNull(args.AcceptSocket.m_RightEndPoint, "m_RightEndPoint was not set");
                    //Assert.AreEqual(client.LocalEndPoint, args.AcceptSocket.m_RemoteEndPoint, "m_RemoteEndPoint is wrong!");

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
