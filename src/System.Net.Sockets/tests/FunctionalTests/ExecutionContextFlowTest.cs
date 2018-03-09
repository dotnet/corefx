// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public partial class ExecutionContextFlowTest : FileCleanupTestBase
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SocketAsyncEventArgs_ExecutionContextFlowsAcrossAcceptAsyncOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var saea = new SocketAsyncEventArgs())
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                var asyncLocal = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();
                saea.Completed += (s, e) =>
                {
                    e.AcceptSocket.Dispose();
                    tcs.SetResult(asyncLocal.Value);
                };

                asyncLocal.Value = 42;
                if (suppressContext) ExecutionContext.SuppressFlow();
                try
                {
                    Assert.True(listener.AcceptAsync(saea));
                }
                finally
                {
                    if (suppressContext) ExecutionContext.RestoreFlow();
                }
                asyncLocal.Value = 0;

                client.Connect(listener.LocalEndPoint);

                Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginAcceptOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                var asyncLocal = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();

                asyncLocal.Value = 42;
                if (suppressContext) ExecutionContext.SuppressFlow();
                try
                {
                    listener.BeginAccept(iar =>
                    {
                        listener.EndAccept(iar).Dispose();
                        tcs.SetResult(asyncLocal.Value);
                    }, null);
                }
                finally
                {
                    if (suppressContext) ExecutionContext.RestoreFlow();
                }
                asyncLocal.Value = 0;

                client.Connect(listener.LocalEndPoint);

                Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SocketAsyncEventArgs_ExecutionContextFlowsAcrossConnectAsyncOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var saea = new SocketAsyncEventArgs())
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                var asyncLocal = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();
                saea.Completed += (s, e) => tcs.SetResult(asyncLocal.Value);
                saea.RemoteEndPoint = listener.LocalEndPoint;

                bool pending;
                asyncLocal.Value = 42;
                if (suppressContext) ExecutionContext.SuppressFlow();
                try
                {
                    pending = client.ConnectAsync(saea);
                }
                finally
                {
                    if (suppressContext) ExecutionContext.RestoreFlow();
                }
                asyncLocal.Value = 0;

                if (pending)
                {
                    Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginConnectOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                var asyncLocal = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();

                bool pending;
                asyncLocal.Value = 42;
                if (suppressContext) ExecutionContext.SuppressFlow();
                try
                {
                    pending = !client.BeginConnect(listener.LocalEndPoint, iar =>
                    {
                        client.EndConnect(iar);
                        tcs.SetResult(asyncLocal.Value);
                    }, null).CompletedSynchronously;
                }
                finally
                {
                    if (suppressContext) ExecutionContext.RestoreFlow();
                }
                asyncLocal.Value = 0;

                if (pending)
                {
                    Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SocketAsyncEventArgs_ExecutionContextFlowsAcrossDisconnectAsyncOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var saea = new SocketAsyncEventArgs())
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();
                    saea.Completed += (s, e) => tcs.SetResult(asyncLocal.Value);

                    bool pending;
                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        pending = client.DisconnectAsync(saea);
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    if (pending)
                    {
                        Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginDisconnectOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();

                    bool pending;
                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        pending = !client.BeginDisconnect(reuseSocket: false, iar =>
                        {
                            client.EndDisconnect(iar);
                            tcs.SetResult(asyncLocal.Value);
                        }, null).CompletedSynchronously;
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    if (pending)
                    {
                        Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task SocketAsyncEventArgs_ExecutionContextFlowsAcrossReceiveAsyncOperation(bool suppressContext, bool receiveFrom)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var saea = new SocketAsyncEventArgs())
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();
                    saea.Completed += (s, e) => tcs.SetResult(asyncLocal.Value);
                    saea.SetBuffer(new byte[1], 0, 1);
                    saea.RemoteEndPoint = server.LocalEndPoint;

                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        Assert.True(receiveFrom ?
                            client.ReceiveFromAsync(saea) :
                            client.ReceiveAsync(saea));
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    server.Send(new byte[] { 18 });
                    Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                }
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginReceiveOperation(bool suppressContext, bool receiveFrom)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();

                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        EndPoint ep = server.LocalEndPoint;
                        Assert.False(receiveFrom ?
                            client.BeginReceiveFrom(new byte[1], 0, 1, SocketFlags.None, ref ep, iar =>
                            {
                                client.EndReceiveFrom(iar, ref ep);
                                tcs.SetResult(asyncLocal.Value);
                            }, null).CompletedSynchronously :
                            client.BeginReceive(new byte[1], 0, 1, SocketFlags.None, iar =>
                            {
                                client.EndReceive(iar);
                                tcs.SetResult(asyncLocal.Value);
                            }, null).CompletedSynchronously);
                    }
                    finally
                    {
                        if (suppressContext)
                            ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    server.Send(new byte[] { 18 });
                    Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                }
            }
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        [InlineData(true, 1)]
        [InlineData(false, 2)]
        [InlineData(true, 2)]
        public async Task SocketAsyncEventArgs_ExecutionContextFlowsAcrossSendAsyncOperation(bool suppressContext, int sendMode)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var saea = new SocketAsyncEventArgs())
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    byte[] buffer = new byte[10_000_000];

                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();
                    saea.Completed += (s, e) => tcs.SetResult(asyncLocal.Value);
                    saea.SetBuffer(buffer, 0, buffer.Length);
                    saea.RemoteEndPoint = server.LocalEndPoint;
                    saea.SendPacketsElements = new[] { new SendPacketsElement(buffer) };
                    
                    bool pending;
                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        pending =
                            sendMode == 0 ? client.SendAsync(saea) :
                            sendMode == 1 ? client.SendToAsync(saea) :
                            client.SendPacketsAsync(saea);
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    int totalReceived = 0;
                    while (totalReceived < buffer.Length)
                    {
                        totalReceived += server.Receive(buffer);
                    }

                    if (pending)
                    {
                        Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginSendOperation(bool suppressContext, bool sendTo)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    byte[] buffer = new byte[10_000_000];

                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();

                    bool pending;
                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        pending = sendTo ?
                            !client.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, server.LocalEndPoint, iar =>
                            {
                                client.EndSendTo(iar);
                                tcs.SetResult(asyncLocal.Value);
                            }, null).CompletedSynchronously :
                            !client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, iar =>
                            {
                                client.EndSend(iar);
                                tcs.SetResult(asyncLocal.Value);
                            }, null).CompletedSynchronously;
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    int totalReceived = 0;
                    while (totalReceived < buffer.Length)
                    {
                        totalReceived += server.Receive(buffer);
                    }

                    if (pending)
                    {
                        Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task APM_ExecutionContextFlowsAcrossBeginSendFileOperation(bool suppressContext)
        {
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                listener.Listen(1);

                client.Connect(listener.LocalEndPoint);
                using (Socket server = listener.Accept())
                {
                    string filePath = GetTestFilePath();
                    using (FileStream fs = File.Create(filePath))
                    {
                        fs.WriteByte(18);
                    }

                    var asyncLocal = new AsyncLocal<int>();
                    var tcs = new TaskCompletionSource<int>();

                    bool pending;
                    asyncLocal.Value = 42;
                    if (suppressContext) ExecutionContext.SuppressFlow();
                    try
                    {
                        pending = !client.BeginSendFile(filePath, iar =>
                        {
                            client.EndSendFile(iar);
                            tcs.SetResult(asyncLocal.Value);
                        }, null).CompletedSynchronously;
                    }
                    finally
                    {
                        if (suppressContext) ExecutionContext.RestoreFlow();
                    }
                    asyncLocal.Value = 0;

                    if (pending)
                    {
                        Assert.Equal(suppressContext ? 0 : 42, await tcs.Task);
                    }
                }
            }
        }
    }
}
