using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class ConnectAsync
    {
        private readonly ITestOutputHelper _output;
        private readonly VerboseLog _verboseLog;

        public ConnectAsync(ITestOutputHelper output)
        {
            _output = output;
            _verboseLog = new VerboseLog(_output);
        }

        private const int TestPortBase = 8020;
        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        [Fact]
        public void Success()
        {
            AutoResetEvent completed = new AutoResetEvent(false);

            if (Socket.OSSupportsIPv4)
            {
                using (SocketTestServer.SocketTestServerFactory(
                                            _verboseLog, 
                                            new IPEndPoint(IPAddress.Loopback, TestPortBase)))
                {

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
                    args.Completed += OnConnectCompleted;
                    args.UserToken = completed;

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Assert.True(client.ConnectAsync(args));

                    Assert.True(completed.WaitOne(5000), "IPv4: Timed out while waiting for connection");

                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

                    client.Dispose();
                }

            }

            if (Socket.OSSupportsIPv6)
            {
                using (SocketTestServer.SocketTestServerFactory(
                                            _verboseLog,
                                            new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase)))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase);
                    args.Completed += OnConnectCompleted;
                    args.UserToken = completed;

                    Socket client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "IPv6: Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

                    client.Dispose();
                }
            }
        }
    }
}