using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class AcceptAsync
    {
        private readonly ITestOutputHelper _output;

        public AcceptAsync(ITestOutputHelper output)
        {
            _output = output;
        }

        private const int TestPortBase = 8000;

        public void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            _output.WriteLine("OnAcceptCompleted event handler");
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }
        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            _output.WriteLine("OnConnectCompleted event handler");
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [Fact]
        public void Success()
        {
            AutoResetEvent completed = new AutoResetEvent(false);
            AutoResetEvent completedClient = new AutoResetEvent(false);
            if (Socket.OSSupportsIPv4)
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase));
                    sock.Listen(1);

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnAcceptCompleted;
                    args.UserToken = completed;

                    Assert.True(sock.AcceptAsync(args));
                    _output.WriteLine("IPv4 Server: Waiting for clients.");

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    SocketAsyncEventArgs argsClient = new SocketAsyncEventArgs();
                    argsClient.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
                    argsClient.Completed += OnConnectCompleted;
                    argsClient.UserToken = completedClient;
                    client.ConnectAsync(argsClient);

                    _output.WriteLine("IPv4 Client: Connecting.");
                    Assert.True(completed.WaitOne(5000), "IPv4: Timed out while waiting for connection");

                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);
                    Assert.NotNull(args.AcceptSocket);
                    Assert.True(args.AcceptSocket.Connected, "IPv4 Accept Socket was not connected");
                    Assert.NotNull(args.AcceptSocket.RemoteEndPoint);
                    Assert.Equal(client.LocalEndPoint, args.AcceptSocket.RemoteEndPoint);

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

                    Assert.True(sock.AcceptAsync(args));
                    _output.WriteLine("IPv6 Server: Waiting for clients.");

                    Socket client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    SocketAsyncEventArgs argsClient = new SocketAsyncEventArgs();
                    argsClient.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase);
                    argsClient.Completed += OnConnectCompleted;
                    argsClient.UserToken = completedClient;
                    client.ConnectAsync(argsClient);

                    _output.WriteLine("IPv6 Client: Connecting.");
                    Assert.True(completed.WaitOne(5000), "IPv6: Timed out while waiting for connection");

                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);
                    Assert.NotNull(args.AcceptSocket);
                    Assert.True(args.AcceptSocket.Connected, "IPv6 Accept Socket was not connected");
                    Assert.NotNull(args.AcceptSocket.RemoteEndPoint);
                    Assert.Equal(client.LocalEndPoint, args.AcceptSocket.RemoteEndPoint);

                    client.Dispose();
                }
            }
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
