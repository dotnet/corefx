// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed partial class HttpConnection : IDisposable
    {
        private sealed class ContentLengthWriteStream : HttpContentWriteStream
        {
            public ContentLengthWriteStream(HttpConnection connection, CancellationToken cancellationToken) :
                base(connection, cancellationToken)
            {
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ignored)
            {
                ValidateBufferArgs(buffer, offset, count);
                return _connection._currentRequest != null ?
                    _connection.WriteAsync(buffer, offset, count, _cancellationToken) :
                    Task.CompletedTask;
            }

            public override Task FlushAsync(CancellationToken ignored)
            {
                return _connection.FlushAsync(_cancellationToken);
            }

            public override Task FinishAsync()
            {
                _connection = null;
                return Task.CompletedTask;
            }
        }
    }
}
