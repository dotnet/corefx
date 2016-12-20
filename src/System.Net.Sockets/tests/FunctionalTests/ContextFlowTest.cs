// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class ContextFlowTest
    {
        private static class ECHelper
        {
            private static readonly AsyncLocal<string> s_asyncLocal = new AsyncLocal<string>();

            public static void RunInCustomEC(string name, Action action)
            {
                var ec = ExecutionContext.Capture();

                // We never modify the existing EC, instead we do the following
                // to create a custom EC and run the action in it
                ExecutionContext.Run(ec, (_) =>
                {
                    s_asyncLocal.Value = name;
                    action();
                }, null);
            }

            // Note, null == default EC
            public static string GetCurrentECName()
            {
                return s_asyncLocal.Value;
            }

        }

        public void DoAPMContextFlow(Socket listenSocket)
        {
            string ecName = ECHelper.GetCurrentECName();

            AutoResetEvent completed = new AutoResetEvent(false);

            // Start async operation
            var asyncResult = listenSocket.BeginAccept(ar =>
            {
                var acceptSocket = listenSocket.EndAccept(ar);
                Assert.NotNull(acceptSocket);
                acceptSocket.Close();

                // Same EC as caller
                Assert.Equal(ecName, ECHelper.GetCurrentECName());

                completed.Set();
            }, null);

            Assert.False(asyncResult.CompletedSynchronously);

            // Connect, causing the async op above to complete
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(listenSocket.LocalEndPoint);

                // Wait for the completion routine to finish
                completed.WaitOne();
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void ContextFlow_APM_Success()
        {
            using (Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                int port = listenSocket.BindToAnonymousPort(IPAddress.Loopback);
                listenSocket.Listen(1);

                ECHelper.RunInCustomEC("ec1", () => DoAPMContextFlow(listenSocket));
                ECHelper.RunInCustomEC("ec2", () => DoAPMContextFlow(listenSocket));
                ECHelper.RunInCustomEC("ec3", () => DoAPMContextFlow(listenSocket));
            }
        }

        public void DoSAEAContextFlow(Socket listenSocket, SocketAsyncEventArgs e)
        {
            string ecName = ECHelper.GetCurrentECName();

            AutoResetEvent completed = new AutoResetEvent(false);

            EventHandler<SocketAsyncEventArgs> handler = null;
            handler = (_, args) =>
            {
                Assert.Equal(SocketError.Success, args.SocketError);
                Assert.NotNull(args.AcceptSocket);
                args.AcceptSocket.Close();
                args.AcceptSocket = null;
                args.Completed -= handler;

                // Same EC as caller
                Assert.Equal(ecName, ECHelper.GetCurrentECName());

                completed.Set();
            };

            e.Completed += handler;
            bool pending = listenSocket.AcceptAsync(e);

            Assert.True(pending);

            // Connect, causing the async op above to complete
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(listenSocket.LocalEndPoint);

                // Wait for the completion routine to finish
                completed.WaitOne();
            }
        }

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void ContextFlow_SAEA_Success()
        {
            using (Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                int port = listenSocket.BindToAnonymousPort(IPAddress.Loopback);
                listenSocket.Listen(1);

                SocketAsyncEventArgs e1 = new SocketAsyncEventArgs();
                ECHelper.RunInCustomEC("ec1", () => DoSAEAContextFlow(listenSocket, e1));
                ECHelper.RunInCustomEC("ec2", () => DoSAEAContextFlow(listenSocket, e1));

                SocketAsyncEventArgs e2 = new SocketAsyncEventArgs();
                ECHelper.RunInCustomEC("ec3", () => DoSAEAContextFlow(listenSocket, e2));
                ECHelper.RunInCustomEC("ec4", () => DoSAEAContextFlow(listenSocket, e2));

                ECHelper.RunInCustomEC("ec5", () => DoSAEAContextFlow(listenSocket, e1));
                ECHelper.RunInCustomEC("ec6", () => DoSAEAContextFlow(listenSocket, e2));
            }
        }
    }
}
