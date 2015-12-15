// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Net.Test.Common;
using System.Text;
using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class UnixDomainSocketTest : IDisposable
    {
        private readonly ITestOutputHelper _log;
        private readonly string _path;
        private readonly UnixDomainSocketEndPoint _endPoint;

        public UnixDomainSocketTest(ITestOutputHelper output)
        {
            _log = TestLogging.GetInstance();
            _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            _endPoint = new UnixDomainSocketEndPoint(_path);
        }

        private void OnConnectAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            ManualResetEvent complete = (ManualResetEvent)args.UserToken;
            complete.Set();
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void Socket_CreateUnixDomainSocket_Throws_OnWindows()
        {
            Assert.False(Capability.UnixDomainSocketsSupport());
            Assert.Throws<SocketException>(() => new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Linux | PlatformID.OSX)]
        public void Socket_ConnectAsyncUnixDomainSocketEndPoint_Success()
        {
            Assert.True(Capability.UnixDomainSocketsSupport());

            SocketTestServer server = SocketTestServer.SocketTestServerFactory(_endPoint, ProtocolType.Unspecified);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = _endPoint;
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            Assert.True(sock.ConnectAsync(args));

            complete.WaitOne();

            Assert.Equal(SocketError.Success, args.SocketError);
            Assert.Null(args.ConnectByNameError);

            complete.Dispose();
            sock.Dispose();
            server.Dispose();
        }

        [Fact]
        [PlatformSpecific(PlatformID.Linux | PlatformID.OSX)]
        public void Socket_ConnectAsyncUnixDomainSocketEndPoint_NotServer()
        {
            Assert.True(Capability.UnixDomainSocketsSupport());

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = _endPoint;
            args.Completed += OnConnectAsyncCompleted;

            ManualResetEvent complete = new ManualResetEvent(false);
            args.UserToken = complete;

            Socket sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            bool willRaiseEvent = sock.ConnectAsync(args);
            if (willRaiseEvent)
            {
                complete.WaitOne();
            }

            Assert.Equal(SocketError.SocketError, args.SocketError);

            complete.Dispose();
            sock.Dispose();
        }

        public void Dispose()
        {
            if (File.Exists(_path))
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
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

        private class UnixDomainSocketEndPoint : EndPoint
        {
            private static readonly Encoding PathEncoding = Encoding.UTF8;

            private const int MaxPathLength = 108;  // See sockaddr_un. It's the same on Linux and OSX
            private const int PathOffset = 2;       // = offsetof(struct sockaddr_un, sun_path). It's the same on Linux and OSX
            private const int MaxSocketAddressSize = PathOffset + MaxPathLength;
            private const int MinSocketAddressSize = PathOffset + 2; // +1 for one character and +1 for \0 ending
            private const AddressFamily EndPointAddressFamily = AddressFamily.Unix;

            private readonly string _path;
            private readonly byte[] _encodedPath;

            public UnixDomainSocketEndPoint(string path)
            {
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }

                if (path.Length == 0 || PathEncoding.GetByteCount(path) >= MaxPathLength)
                {
                    throw new ArgumentOutOfRangeException("path");
                }
                
                _path = path;
                _encodedPath = PathEncoding.GetBytes(_path);
            }

            internal UnixDomainSocketEndPoint(SocketAddress socketAddress)
            {
                if (socketAddress == null)
                {
                    throw new ArgumentNullException("socketAddress");
                }

                if (socketAddress.Family != EndPointAddressFamily || socketAddress.Size < MinSocketAddressSize || socketAddress.Size > MaxSocketAddressSize)
                {
                    throw new ArgumentException("socketAddress");
                }

                _encodedPath = new byte[socketAddress.Size - PathOffset];
                for (int index = 0; index < socketAddress.Size - PathOffset; index++)
                {
                    _encodedPath[index] = socketAddress[PathOffset + index];
                }

                _path = PathEncoding.GetString(_encodedPath);
            }

            public string Path
            {
                get
                {
                    return _path;
                }
            }

            public override AddressFamily AddressFamily
            {
                get
                {
                    return EndPointAddressFamily;
                }
            }

            public override SocketAddress Serialize()
            {
                SocketAddress result = new SocketAddress(AddressFamily.Unix, MaxSocketAddressSize);
                for (int index = 0; index < MaxPathLength; index++)
                {
                    result[PathOffset + index] = index < _encodedPath.Length ? _encodedPath[index] : (byte)0;
                }

                return result;
            }

            public override EndPoint Create(SocketAddress socketAddress)
            {
                return new UnixDomainSocketEndPoint(socketAddress);
            }

            public override string ToString()
            {
                return Path;
            }
        }
    }
}
