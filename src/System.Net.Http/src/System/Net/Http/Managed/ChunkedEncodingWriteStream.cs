// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed partial class HttpConnection : IDisposable
    {
        private sealed class ChunkedEncodingWriteStream : HttpContentWriteStream
        {
            private static readonly byte[] s_finalChunkBytes = { (byte)'0', (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };

            public ChunkedEncodingWriteStream(HttpConnection connection, CancellationToken cancellationToken) :
                base(connection, cancellationToken)
            {
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ignored)
            {
                ValidateBufferArgs(buffer, offset, count);

                // Don't write if nothing was given, especially since we don't want to accidentally send a 0 chunk,
                // which would indicate end of body.  We also avoid sending if the response has already completed,
                // in which case there's no point sending further data (this might happen, for example, on a redirect.)
                return count != 0 && _connection._currentRequest != null ?
                    WriteAsyncCore(buffer, offset, count) :
                    Task.CompletedTask;
            }

            private async Task WriteAsyncCore(byte[] buffer, int offset, int count)
            {
                // Write chunk length -- hex representation of count
                bool digitWritten = false;
                for (int i = 7; i >= 0; i--)
                {
                    int shift = i * 4;
                    int mask = 0xF << shift;
                    int digit = (count & mask) >> shift;
                    if (digitWritten || digit != 0)
                    {
                        await _connection.WriteByteAsync((byte)(digit < 10 ? '0' + digit : 'A' + digit - 10), _cancellationToken).ConfigureAwait(false);
                        digitWritten = true;
                    }
                }

                // End chunk length
                await _connection.WriteTwoBytesAsync((byte)'\r', (byte)'\n', _cancellationToken).ConfigureAwait(false);

                // Write chunk contents
                await _connection.WriteAsync(buffer, offset, count, _cancellationToken).ConfigureAwait(false);
                await _connection.WriteTwoBytesAsync((byte)'\r', (byte)'\n', _cancellationToken).ConfigureAwait(false);

                // Flush the chunk.  This is reasonable from the standpoint of having just written a standalone piece
                // of data, but is also necessary to support duplex communication, where a CopyToAsync is taking the
                // data from content and writing it here; if there was no flush, we might not send the data until the
                // source was empty, and it might be kept open to enable subsequent communication.
                await _connection.FlushAsync(_cancellationToken);
            }

            public override Task FlushAsync(CancellationToken ignored)
            {
                return _connection.FlushAsync(_cancellationToken);
            }
            
            public override async Task FinishAsync()
            {
                // Send 0 byte chunk to indicate end, then final CrLf
                await _connection.WriteBytesAsync(s_finalChunkBytes, _cancellationToken).ConfigureAwait(false);
                _connection = null;
            }
        }
    }
}
