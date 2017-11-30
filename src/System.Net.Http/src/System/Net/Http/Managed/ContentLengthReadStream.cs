// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed partial class HttpConnection : IDisposable
    {
        private sealed class ContentLengthReadStream : HttpContentReadStream
        {
            private ulong _contentBytesRemaining;
            private object _asyncReadLock = new object();

            public ContentLengthReadStream(HttpConnection connection, ulong contentLength)
                : base(connection)
            {
                Debug.Assert(contentLength > 0, "Caller should have checked for 0.");
                _contentBytesRemaining = contentLength;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                ValidateBufferArgs(buffer, offset, count);
                return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
            }

            public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
            {
                if (_connection == null || destination.Length == 0 || cancellationToken.IsCancellationRequested)
                {
                    // Response body fully consumed or the caller didn't ask for any data or cancellation requested
                    return 0;
                }

                try
                {
                    using (cancellationToken.Register(action => ((ContentLengthReadStream)action).CancelPendingResponseStreamReadOperation(), this))
                    {
                        Debug.Assert(_contentBytesRemaining > 0);

                        if ((ulong)destination.Length > _contentBytesRemaining)
                        {
                            destination = destination.Slice(0, (int)_contentBytesRemaining);
                        }

                        int bytesRead = await _connection.ReadAsync(destination, cancellationToken).ConfigureAwait(false);

                        if (bytesRead <= 0)
                        {
                            // Unexpected end of response stream
                            throw new IOException(SR.net_http_invalid_response);
                        }

                        Debug.Assert((ulong)bytesRead <= _contentBytesRemaining);
                        _contentBytesRemaining -= (ulong)bytesRead;

                        if (_contentBytesRemaining == 0)
                        {
                            lock (_asyncReadLock)
                            {
                                // End of response body
                                _connection.ReturnConnectionToPool();
                                _connection = null;
                            }
                        }

                        return bytesRead;
                    }
                }
                catch (IOException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw;
                }
            }

            public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                if (destination == null)
                {
                    throw new ArgumentNullException(nameof(destination));
                }

                if (_connection == null || cancellationToken.IsCancellationRequested)
                {
                    // Response body fully consumed or cancellation requested
                    return;
                }

                try
                {
                    using (cancellationToken.Register(action => ((ContentLengthReadStream)action).CancelPendingResponseStreamReadOperation(), this))
                    {
                        await _connection.CopyToAsync(destination, _contentBytesRemaining, cancellationToken).ConfigureAwait(false);

                        _contentBytesRemaining = 0;
                        lock (_asyncReadLock)
                        {
                            _connection.ReturnConnectionToPool();
                            _connection = null;
                        }
                    }

                }
                catch (IOException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw;
                }
            }

            private void CancelPendingResponseStreamReadOperation()
            {
                lock (_asyncReadLock)
                {
                    _connection?.Dispose();
                }
            }
        }
    }
}
