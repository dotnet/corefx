﻿// Licensed to the .NET Foundation under one or more agreements.
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
            private long _contentBytesRemaining;

            public ContentLengthReadStream(HttpConnection connection, long contentLength)
                : base(connection)
            {
                if (contentLength == 0)
                {
                    _connection = null;
                    _contentBytesRemaining = 0;
                    connection.ReturnConnectionToPool();
                }
                else
                {
                    _contentBytesRemaining = contentLength;
                }
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                ValidateBufferArgs(buffer, offset, count);

                if (_connection == null || count == 0)
                {
                    // Response body fully consumed or the caller didn't ask for any data
                    return 0;
                }

                Debug.Assert(_contentBytesRemaining > 0);

                count = (int)Math.Min(count, _contentBytesRemaining);

                int bytesRead = await _connection.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    // Unexpected end of response stream
                    throw new IOException("Unexpected end of content stream");
                }

                Debug.Assert(bytesRead <= _contentBytesRemaining);
                _contentBytesRemaining -= bytesRead;

                if (_contentBytesRemaining == 0)
                {
                    // End of response body
                    _connection.ReturnConnectionToPool();
                    _connection = null;
                }

                return bytesRead;
            }

            public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                if (destination == null)
                {
                    throw new ArgumentNullException(nameof(destination));
                }

                if (_connection == null)
                {
                    // Response body fully consumed
                    return;
                }

                await _connection.CopyChunkToAsync(destination, _contentBytesRemaining, cancellationToken).ConfigureAwait(false);

                _contentBytesRemaining = 0;
                _connection.ReturnConnectionToPool();
                _connection = null;
            }
        }
    }
}
