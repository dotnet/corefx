using System.Threading;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class Disconnect
    {
        private const int TestPortBase = 8040;

        public void OnCompleted(object sender, SocketAsyncEventArgs args)
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
                using (SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, TestPortBase)))
                using (SocketTestServer.SocketTestServerFactory(new IPEndPoint(IPAddress.Loopback, TestPortBase + 1)))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnCompleted;
                    args.UserToken = completed;
                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase);
                    args.DisconnectReuseSocket = true;

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

                    client.Disconnect(true);

                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 1);

                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

                    client.Dispose();
                }
            }
        }
    }
}
