using System.Threading;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class DisconnectAsync
    {
        private const int TestPortBase = 8050;

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

                    Assert.True(client.DisconnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

                    args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 1);

                    Assert.True(client.ConnectAsync(args));
                    Assert.True(completed.WaitOne(5000), "Timed out while waiting for connection");
                    Assert.Equal<SocketError>(SocketError.Success, args.SocketError);

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
