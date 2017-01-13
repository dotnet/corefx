// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Test.Common;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class ContextFlowTest
    {
        private static AsyncLocal<string> s_asyncLocal = new AsyncLocal<string>();

        [OuterLoop] // TODO: Issue #11345
        [Fact]
        public void ContextFlow_Success()
        {
            AutoResetEvent completed = new AutoResetEvent(false);

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int port = sock.BindToAnonymousPort(IPAddress.Loopback);
            sock.Listen(1);

            // Get default EC 
            ExecutionContext defaultEC = ExecutionContext.Capture();

            // Create a non-default EC by setting the AsyncLocal
            s_asyncLocal.Value = "caller EC";

            // Get new EC 
            ExecutionContext callerEC = ExecutionContext.Capture();
            Assert.NotEqual(defaultEC, callerEC);

            // Start async operation
            var asyncResult = sock.BeginAccept(ar =>
            {
                // Same EC as caller
                ExecutionContext callbackEC = ExecutionContext.Capture();
                Assert.Equal(callbackEC, callerEC);

                sock.EndAccept(ar);
                sock.Close();
                completed.Set();
            }, null);

            Assert.False(asyncResult.CompletedSynchronously);

            // Change EC again
            s_asyncLocal.Value = null;

            // Get new EC 
            ExecutionContext newEC = ExecutionContext.Capture();
            Assert.NotEqual(callerEC, newEC);

            // Connect, causing the async op above to complete
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, port));

                // Wait for the completion routine to finish
                completed.WaitOne();
            }
        }
    }
}
