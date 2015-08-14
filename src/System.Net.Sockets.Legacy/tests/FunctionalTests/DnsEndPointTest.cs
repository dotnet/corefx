using System.Net.Test.Common;
using System.Threading;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class DnsEndPointTest
    {
        [Fact]
        public void Socket_ConnectDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(new DnsEndPoint("localhost", 8080));

            sock.Dispose();
            server.Dispose();
        }

        [Fact]
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
                Assert.True((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                    "SocketErrorCode: {0}", errorCode);
            }

            try
            {
                sock.Connect(new DnsEndPoint("localhost", 8081));
                Assert.Fail("Connecting to an invalid port did not fail");
            }
            catch (SocketException ex)
            {
                Assert.Equal(SocketError.ConnectionRefused, ex.SocketErrorCode);
            }

            sock.Dispose();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void Socket_BeginConnectDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8080), null, null);
            sock.EndConnect(result);

            sock.Dispose();
            server.Dispose();
        }

        [Fact]
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
                Assert.True((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
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
                Assert.Equal(SocketError.ConnectionRefused, ex.SocketErrorCode);
            }

            sock.Dispose();
        }

        [Fact]
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

        [Fact]
        public void Socket_ConnectAsyncDnsEndPoint_Success()
        {
            SocketTestServer server = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;
            
            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.True(sock.ConnectAsync(args));

            complete.WaitOne();

            Assert.Equal(SocketError.Success, args.SocketError);
            Assert.Null(args.ConnectByNameError);

            complete.Dispose();
            sock.Dispose();
            server.Dispose();
        }

        [Fact]
        public void Socket_ConnectAsyncDnsEndPoint_HostNotFound()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.True(sock.ConnectAsync(args));

            complete.WaitOne();

            AssertHostNotFoundOrNoData(args);

            complete.Dispose();
            sock.Dispose();
        }

        [Fact]
        public void Socket_ConnectAsyncDnsEndPoint_ConnectionRefused()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Assert.True(sock.ConnectAsync(args));

            complete.WaitOne();

            Assert.Equal(SocketError.ConnectionRefused, args.SocketError);
            Assert.True(args.ConnectByNameError is SocketException);
            Assert.Equal(SocketError.ConnectionRefused, ((SocketException)args.ConnectByNameError).SocketErrorCode);

            complete.Dispose();
            sock.Dispose();
        }

        [Fact]
        public void Socket_StaticConnectAsync_Success()
        {

            Assert.True(Capability.IPv6Support() && Capability.IPv4Support());

            SocketTestServer server4 = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, 8080));
            SocketTestServer server6 = SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.IPv6Loopback, 8081));

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.True(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.Equal(SocketError.Success, args.SocketError);
            Assert.Null(args.ConnectByNameError);
            Assert.NotNull(args.ConnectSocket);
            Assert.True(args.ConnectSocket.AddressFamily == AddressFamily.InterNetwork);
            Assert.True(args.ConnectSocket.Connected);

            args.ConnectSocket.Dispose();

            args.RemoteEndPoint = new DnsEndPoint("localhost", 8081);
            complete.Reset();

            Assert.True(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.Equal(SocketError.Success, args.SocketError);
            Assert.Null(args.ConnectByNameError);
            Assert.NotNull(args.ConnectSocket);
            Assert.True(args.ConnectSocket.AddressFamily == AddressFamily.InterNetworkV6);
            Assert.True(args.ConnectSocket.Connected);

            args.ConnectSocket.Dispose();

            server4.Dispose();
            server6.Dispose();
        }

        [Fact]
        public void Socket_StaticConnectAsync_HostNotFound()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("notahostname.invalid.corp.microsoft.com", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.True(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            AssertHostNotFoundOrNoData(args);

            Assert.Null(args.ConnectSocket);

            complete.Dispose();
        }

        [Fact]
        public void Socket_StaticConnectAsync_ConnectionRefused()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", 8080);
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Assert.True(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            complete.WaitOne();

            Assert.Equal(SocketError.ConnectionRefused, args.SocketError);
            Assert.True(args.ConnectByNameError is SocketException);
            Assert.Equal(SocketError.ConnectionRefused, ((SocketException)args.ConnectByNameError).SocketErrorCode);
            Assert.Null(args.ConnectSocket);

            complete.Dispose();
        }

        public void CallbackThatShouldNotBeCalled(object sender, SocketAsyncEventArgs args)
        {
            Assert.Fail("This Callback should not be called");
        }

        [Fact]
        public void Socket_StaticConnectAsync_SyncFailure()
        {
            Assert.True(Capability.IPv6Support()); // IPv6 required because we use AF.InterNetworkV6

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("127.0.0.1", 8080, AddressFamily.InterNetworkV6);
            args.Completed += CallbackThatShouldNotBeCalled;

            Assert.False(Socket.ConnectAsync(SocketType.Stream, ProtocolType.Tcp, args));

            Assert.Equal(SocketError.NoData, args.SocketError);
            Assert.Null(args.ConnectSocket);
        }

        private static void AssertHostNotFoundOrNoData(SocketAsyncEventArgs args)
        {
            SocketError errorCode = args.SocketError;
            Assert.True((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                "SocketError: " + errorCode);

            Assert.True(args.ConnectByNameError is SocketException);
            errorCode = ((SocketException)args.ConnectByNameError).SocketErrorCode;
            Assert.True((errorCode == SocketError.HostNotFound) || (errorCode == SocketError.NoData),
                "SocketError: " + errorCode);
        }

        #region GC Finalizer test
        // This test assumes sequential execution of tests and that it is going to be executed after other tests
        // that used Sockets. 
        [Fact]
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
