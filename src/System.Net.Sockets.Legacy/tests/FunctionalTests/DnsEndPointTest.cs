namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using NCLTest.Common;
    
    [TestClass]
    public class DnsEndPointTest
    {
        [TestMethod]
        public void Socket_ConnectDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(new DnsEndPoint("localhost", 8080));

            sock.Dispose();
            server.Dispose();
        }

        [TestMethod]
        public void Socket_ConnectDnsEndPoint_Failure()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sock.Connect(new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080));
                Assert.Fail("Connecting to invalid dns name did not fail");
            }
            catch (SocketException ex)
            {
                SocketError errorCode = ex.SocketErrorCode;
                Assert.IsTrue((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                    "SocketErrorCode: {0}", errorCode);
            }

            try
            {
                sock.Connect(new DnsEndPoint("localhost", 8081));
                Assert.Fail("Connecting to an invalid port did not fail");
            }
            catch (SocketException ex)
            {
                Assert.AreEqual(SocketError.ConnectionRefused, ex.SocketErrorCode);
            }

            sock.Dispose();
        }

        [TestMethod]
        public void Socket_SendToDnsEndPoint_ArgumentException()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                sock.SendTo(new byte[10], new DnsEndPoint("localhost", 8080));
                Assert.Fail("SendTo a DnsEndPoint did not fail");
            }
            catch (ArgumentException)
            {
            }

            sock.Dispose();
        }

        [TestMethod]
        public void Socket_ReceiveFromDnsEndPoint_ArgumentException()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.Bind(new IPEndPoint(IPAddress.Loopback, 8080));

            try
            {
                EndPoint endpoint = new DnsEndPoint("localhost", 8080);
                sock.ReceiveFrom(new byte[10], ref endpoint);
                Assert.Fail("ReceiveFrom a DnsEndPoint did not fail");
            }
            catch (ArgumentException)
            {
            }

            sock.Dispose();
        }

        [TestMethod]
        public void Socket_BeginConnectDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8080), null, null);
            sock.EndConnect(result);

            sock.Dispose();
            server.Dispose();
        }

        [TestMethod]
        public void Socket_BeginConnectDnsEndPoint_Failure()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IAsyncResult result = sock.BeginConnect(new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080), null, null);
                sock.EndConnect(result);
                Assert.Fail("BeginConnect to an invalid hostname did not fail");
            }
            catch (SocketException ex)
            {
                SocketError errorCode = ex.SocketErrorCode;
                Assert.IsTrue((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                    "SocketErrorCode: {0}", errorCode);
            }

            try
            {
                IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8080), null, null);
                sock.EndConnect(result);
                Assert.Fail("BeginConnect to an invalid port did not fail");
            }
            catch (SocketException ex)
            {
                Assert.AreEqual(SocketError.ConnectionRefused, ex.SocketErrorCode);
            }

            sock.Dispose();
        }

        [TestMethod]
        public void Socket_BeginSendToDnsEndPoint_ArgumentException()
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                sock.BeginSendTo(new byte[10], 0, 0, SocketFlags.None, new DnsEndPoint("localhost", 8080), null, null);
                Assert.Fail("SendTo a DnsEndPoint did not fail");
            }
            catch (ArgumentException)
            {
            }

            sock.Dispose();
        }

        private void OnConnectAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            ManualResetEvent complete = (ManualResetEvent)args.UserToken;
            complete.Set();
        }

        [TestMethod]
        public void Socket_ConnectAsyncDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;
            
            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.IsTrue(sock.ConnectAsync(args));

            complete.WaitOne();

            Assert.AreEqual(SocketError.Success, args.SocketError);
            Assert.IsNull(args.ConnectByNameError, "ConnectByNameError");

            complete.Dispose();
            sock.Dispose();
            server.Dispose();
        }

        [TestMethod]
        public void Socket_ConnectAsyncDnsEndPoint_HostNotFound()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.IsTrue(sock.ConnectAsync(args));

            complete.WaitOne();

            AssertHostNotFoundOrNoData(args);

            complete.Dispose();
            sock.Dispose();
        }

        [TestMethod]
        public void Socket_ConnectAsyncDnsEndPoint_ConnectionRefused()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.IsTrue(sock.ConnectAsync(args));

            complete.WaitOne();

            Assert.AreEqual(SocketError.ConnectionRefused, args.SocketError);
            Assert.IsTrue(args.ConnectByNameError is SocketException);
            Assert.AreEqual(SocketError.ConnectionRefused, ((SocketException)args.ConnectByNameError).SocketErrorCode);

            complete.Dispose();
            sock.Dispose();
        }

        [TestMethod]
        public void Socket_StaticConnectAsync_Success()
        {
            TestRequirements.CheckIPv6Support();
            TestRequirements.CheckIPv4Support();

            SocketTestServer server4 = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));
            SocketTestServer server6 = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.IPv6Loopback, 8081));

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.IsTrue(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.AreEqual(SocketError.Success, args.SocketError);
            Assert.IsNull(args.ConnectByNameError);
            Assert.IsNotNull(args.ConnectSocket);
            Assert.IsTrue(args.ConnectSocket.AddressFamily == AddressFamily.InterNetwork);
            Assert.IsTrue(args.ConnectSocket.Connected);

            args.ConnectSocket.Dispose();

            args.RemoteEndPoint = new DnsEndPoint("localhost", 8081);
            complete.Reset();

            Assert.IsTrue(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.AreEqual(SocketError.Success, args.SocketError);
            Assert.IsNull(args.ConnectByNameError);
            Assert.IsNotNull(args.ConnectSocket);
            Assert.IsTrue(args.ConnectSocket.AddressFamily == AddressFamily.InterNetworkV6);
            Assert.IsTrue(args.ConnectSocket.Connected);

            args.ConnectSocket.Dispose();

            server4.Dispose();
            server6.Dispose();
        }

        [TestMethod]
        public void Socket_StaticConnectAsync_HostNotFound()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.IsTrue(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            AssertHostNotFoundOrNoData(args);

            Assert.IsNull(args.ConnectSocket);

            complete.Dispose();
        }

        [TestMethod]
        public void Socket_StaticConnectAsync_ConnectionRefused()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.IsTrue(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.AreEqual(SocketError.ConnectionRefused, args.SocketError);
            Assert.IsTrue(args.ConnectByNameError is SocketException);
            Assert.AreEqual(SocketError.ConnectionRefused, ((SocketException)args.ConnectByNameError).SocketErrorCode);
            Assert.IsNull(args.ConnectSocket);

            complete.Dispose();
        }

        public void CallbackThatShouldNotBeCalled(object sender, SocketAsyncEventArgs args)
        {
            Assert.Fail("This Callback should not be called");
        }

        [TestMethod]
        public void Socket_StaticConnectAsync_SyncFailure()
        {
            TestRequirements.CheckIPv6Support(); // IPv6 required because we use AF.InterNetworkV6

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("127.0.0.1", 8080, AddressFamily.InterNetworkV6);
            args.Completed += CallbackThatShouldNotBeCalled;

            Assert.IsFalse(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            Assert.AreEqual(SocketError.NoData, args.SocketError);
            Assert.IsNull(args.ConnectSocket);
        }

        private static void AssertHostNotFoundOrNoData(SocketAsyncEventArgs args)
        {
            SocketError errorCode = args.SocketError;
            Assert.IsTrue((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                "SocketError: {0}", errorCode);

            Assert.IsTrue(args.ConnectByNameError is SocketException);
            errorCode = ((SocketException)args.ConnectByNameError).SocketErrorCode;
            Assert.IsTrue((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                "SocketError: {0}", errorCode);
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
