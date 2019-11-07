// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Quic.Implementations.Mock
{
    internal sealed class MockStream : QuicStreamProvider
    {
        private bool _disposed = false;
        private readonly long _streamId;
        private bool _canRead;
        private bool _canWrite;

        private MockConnection _connection;

        private Socket _socket = null;

        // Constructor for outbound streams
        internal MockStream(MockConnection connection, long streamId, bool bidirectional)
        {
            _connection = connection;
            _streamId = streamId;
            _canRead = bidirectional;
            _canWrite = true;
        }

        // Constructor for inbound streams
        internal MockStream(Socket socket, long streamId, bool bidirectional)
        {
            _socket = socket;
            _streamId = streamId;
            _canRead = true;
            _canWrite = bidirectional;
        }

        private async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(_connection != null, "Stream not connected but no connection???");

            _socket = await _connection.CreateOutboundMockStreamAsync(_streamId).ConfigureAwait(false);

            // Don't need to hold on to the connection any longer.
            _connection = null;
        }

        internal override long StreamId
        {
            get
            {
                CheckDisposed();
                return _streamId;
            }
        }

        internal override bool CanRead => _canRead;

        internal override int Read(Span<byte> buffer)
        {
            CheckDisposed();

            if (!_canRead)
            {
                throw new NotSupportedException();
            }

            return _socket.Receive(buffer);
        }

        internal override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (!_canRead)
            {
                throw new NotSupportedException();
            }

            if (_socket == null)
            {
                await ConnectAsync(cancellationToken).ConfigureAwait(false);
            }

            return await _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
        }

        internal override bool CanWrite => _canWrite;

        internal override void Write(ReadOnlySpan<byte> buffer)
        {
            CheckDisposed();

            if (!_canWrite)
            {
                throw new NotSupportedException();
            }

            _socket.Send(buffer);
        }

        internal override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (!_canWrite)
            {
                throw new NotSupportedException();
            }

            if (_socket == null)
            {
                await ConnectAsync(cancellationToken).ConfigureAwait(false);
            }

            await _socket.SendAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
        }

        internal override void Flush()
        {
            CheckDisposed();
        }

        internal override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();

            return Task.CompletedTask;
        }

        internal override void ShutdownRead()
        {
            throw new NotImplementedException();
        }

        internal override void ShutdownWrite()
        {
            CheckDisposed();

            _socket.Shutdown(SocketShutdown.Send);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(QuicStream));
            }
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _socket?.Dispose();
                _socket = null;
            }
        }
    }
}
