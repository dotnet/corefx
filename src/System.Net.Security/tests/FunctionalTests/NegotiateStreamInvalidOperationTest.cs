// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Net.Test.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace System.Net.Security.Tests
{
    [PlatformSpecific(TestPlatforms.Windows)] // NegotiateStream only supports client-side functionality on Unix
    public class NegotiateStreamInvalidOperationTest
    {
        private readonly byte[] _sampleMsg = Encoding.UTF8.GetBytes("Sample Test Message");

        [Fact]
        public async Task NegotiateStream_StreamContractTest_Success()
        {
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Assert.False(client.CanSeek);
                Assert.False(client.CanRead);
                Assert.False(client.CanTimeout);
                Assert.False(client.CanWrite);
                Assert.False(server.CanSeek);
                Assert.False(server.CanRead);
                Assert.False(server.CanTimeout);
                Assert.False(server.CanWrite);

                Assert.Throws<InvalidOperationException>(() => client.ReadTimeout);
                Assert.Throws<InvalidOperationException>(() => client.WriteTimeout);
                Assert.Throws<NotImplementedException>(() => client.Length);
                Assert.Throws<NotImplementedException>(() => client.Position);
                Assert.Throws<NotSupportedException>(() => client.Seek(0, new SeekOrigin()));

                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync();
                auth[1] = server.AuthenticateAsServerAsync();

                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                Assert.True(client.CanRead);
                Assert.True(client.CanWrite);
                Assert.True(server.CanRead);
                Assert.True(server.CanWrite);
            }
        }

        [Fact]
        public async Task NegotiateStream_EndReadEndWriteInvalidParameter_Throws()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, string.Empty);
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                auth[0] = Task.Factory.FromAsync(client.BeginWrite, CustomEndWrite, _sampleMsg, 0, _sampleMsg.Length, client);
                auth[1] = Task.Factory.FromAsync(server.BeginRead, CustomEndRead, recvBuf, 0, _sampleMsg.Length, server);
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);
            }
        }

        [Fact]
        public async Task NegotiateStream_ConcurrentAsyncReadOrWrite_ThrowsNotSupportedException()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, string.Empty);
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                // CustomEndWrite/Read will not reset the variable which monitors concurrent write/read.
                auth[0] = Task.Factory.FromAsync(client.BeginWrite, CustomEndWrite, _sampleMsg, 0, _sampleMsg.Length, client);
                auth[1] = Task.Factory.FromAsync(server.BeginRead, CustomEndRead, recvBuf, 0, _sampleMsg.Length, server);
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                Assert.Throws<NotSupportedException>(() => client.BeginWrite(_sampleMsg, 0, _sampleMsg.Length, CustomEndWrite, null));
                Assert.Throws<NotSupportedException>(() => server.BeginRead(recvBuf, 0, _sampleMsg.Length, CustomEndRead, null));
            }
        }

        [Fact]
        public async Task NegotiateStream_ConcurrentSyncReadOrWrite_ThrowsNotSupportedException()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, string.Empty);
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                // CustomEndWrite/Read will not reset the variable which monitors concurrent write/read.
                auth[0] = Task.Factory.FromAsync(client.BeginWrite, CustomEndWrite, _sampleMsg, 0, _sampleMsg.Length, client);
                auth[1] = Task.Factory.FromAsync(server.BeginRead, CustomEndRead, recvBuf, 0, _sampleMsg.Length, server);
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                Assert.Throws<NotSupportedException>(() => client.Write(_sampleMsg, 0, _sampleMsg.Length));
                Assert.Throws<NotSupportedException>(() => server.Read(recvBuf, 0, _sampleMsg.Length));
            }
        }

        [Fact]
        public async Task NegotiateStream_DoubleEndCall_Throws()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = Task.Run(() => client.AuthenticateAsClient());
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                IAsyncResult clientResult = client.BeginWrite(_sampleMsg, 0, _sampleMsg.Length, client.EndWrite, client);
                IAsyncResult serverResult = server.BeginRead(recvBuf, 0, _sampleMsg.Length, ValidCustomEndRead, server);
                clientResult.AsyncWaitHandle.WaitOne();
                serverResult.AsyncWaitHandle.WaitOne();
                Assert.Throws<InvalidOperationException>(() => client.EndWrite(clientResult));
                Assert.Throws<InvalidOperationException>(() => server.EndRead(serverResult));
            }
        }

        [Fact]
        public async Task NegotiateStream_DisposeTooEarly_Throws()
        {
            byte[] recvBuf = new byte[_sampleMsg.Length];
            VirtualNetwork network = new VirtualNetwork();

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync();
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                server.Dispose();
                Assert.Throws<IOException>(() => client.Write(_sampleMsg, 0, _sampleMsg.Length));
                Assert.Throws<IOException>(() => client.Read(recvBuf, 0, _sampleMsg.Length));
            }
        }

        [Fact]
        public async Task NegotiateStream_InvalidParametersForReadWrite_Throws()
        {
            VirtualNetwork network = new VirtualNetwork();
            byte[] buffer = _sampleMsg;
            int offset = 0;
            int count = _sampleMsg.Length;

            using (var clientStream = new VirtualNetworkStream(network, isServer: false))
            using (var serverStream = new VirtualNetworkStream(network, isServer: true))
            using (var client = new NegotiateStream(clientStream))
            using (var server = new NegotiateStream(serverStream))
            {
                // Need to do authentication first, because Read/Write operation
                // is only allowed using a successfully authenticated context.
                Task[] auth = new Task[2];
                auth[0] = client.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, string.Empty);
                auth[1] = server.AuthenticateAsServerAsync();
                await TestConfiguration.WhenAllOrAnyFailedWithTimeout(auth);

                // Null buffer.
                Assert.Throws<ArgumentNullException>(() => client.Write(null, offset, count));

                // Negative offset.
                Assert.Throws<ArgumentOutOfRangeException>(() => client.Write(buffer, -1, count));

                // Negative count.
                Assert.Throws<ArgumentOutOfRangeException>(() => client.Write(buffer, offset, -1));

                // Invalid offset and count combination.
                Assert.Throws<ArgumentOutOfRangeException>(() => client.Write(buffer, offset, count + count));

                // Null buffer.
                Assert.Throws<ArgumentNullException>(() => server.Read(null, offset, count));

                // Negative offset.
                Assert.Throws<ArgumentOutOfRangeException>(() => server.Read(buffer, -1, count));

                // Negative count.
                Assert.Throws<ArgumentOutOfRangeException>(() => server.Read(buffer, offset, -1));

                // Invalid offset and count combination.
                Assert.Throws<ArgumentOutOfRangeException>(() => server.Read(buffer, offset, count + count));
            }
        }

        private void CustomEndWrite(IAsyncResult ar)
        {
            NegotiateStream authStream = (NegotiateStream)ar.AsyncState;

            Assert.Throws<ArgumentNullException>(() => authStream.EndWrite(null));

            IAsyncResult result = new MyAsyncResult();
            Assert.Throws<ArgumentException>(() => authStream.EndWrite(result));
        }

        private void CustomEndRead(IAsyncResult ar)
        {
            NegotiateStream authStream = (NegotiateStream)ar.AsyncState;

            Assert.Throws<ArgumentNullException>(() => authStream.EndRead(null));

            IAsyncResult result = new MyAsyncResult();
            Assert.Throws<ArgumentException>(() => authStream.EndRead(result));
        }

        private void ValidCustomEndRead(IAsyncResult ar)
        {
            NegotiateStream authStream = (NegotiateStream)ar.AsyncState;
            authStream.EndRead(ar);
        }

        private class MyAsyncResult : IAsyncResult
        {
            public bool IsCompleted
            {
                get { return true; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { throw new NotImplementedException(); }
            }

            public object AsyncState
            {
                get { return null; }
            }

            public bool CompletedSynchronously
            {
                get { return true; }
            }
        }
    }
}
