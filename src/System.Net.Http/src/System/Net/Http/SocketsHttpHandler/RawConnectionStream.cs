﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal partial class HttpConnection : IDisposable
    {
        private sealed class RawConnectionStream : HttpContentDuplexStream
        {
            public RawConnectionStream(HttpConnection connection) : base(connection)
            {
            }

            public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_connection == null || destination.Length == 0)
                {
                    // Response body fully consumed or the caller didn't ask for any data
                    return 0;
                }

                ValueTask<int> readTask = _connection.ReadAsync(destination);
                int bytesRead;
                if (readTask.IsCompletedSuccessfully)
                {
                    bytesRead = readTask.Result;
                }
                else
                {
                    CancellationTokenRegistration ctr = _connection.RegisterCancellation(cancellationToken);
                    try
                    {
                        bytesRead = await readTask.ConfigureAwait(false);
                    }
                    catch (Exception exc) when (ShouldWrapInOperationCanceledException(exc, cancellationToken))
                    {
                        throw CreateOperationCanceledException(exc, cancellationToken);
                    }
                    finally
                    {
                        ctr.Dispose();
                    }
                }

                if (bytesRead == 0)
                {
                    // A cancellation request may have caused the EOF.
                    cancellationToken.ThrowIfCancellationRequested();

                    // We cannot reuse this connection, so close it.
                    _connection.Dispose();
                    _connection = null;
                    return 0;
                }

                return bytesRead;
            }

            public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                ValidateCopyToArgs(this, destination, bufferSize);

                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromCanceled(cancellationToken);
                }

                if (_connection == null)
                {
                    // null if response body fully consumed
                    return Task.CompletedTask;
                }

                Task copyTask = _connection.CopyToUntilEofAsync(destination, bufferSize, cancellationToken);
                if (copyTask.IsCompletedSuccessfully)
                {
                    Finish();
                    return Task.CompletedTask;
                }

                return CompleteCopyToAsync(copyTask, cancellationToken);
            }

            private async Task CompleteCopyToAsync(Task copyTask, CancellationToken cancellationToken)
            {
                CancellationTokenRegistration ctr = _connection.RegisterCancellation(cancellationToken);
                try
                {
                    await copyTask.ConfigureAwait(false);
                }
                catch (Exception exc) when (ShouldWrapInOperationCanceledException(exc, cancellationToken))
                {
                    throw CreateOperationCanceledException(exc, cancellationToken);
                }
                finally
                {
                    ctr.Dispose();
                }

                // If cancellation is requested and tears down the connection, it could cause the copy
                // to end early but think it ended successfully. So we prioritize cancellation in this
                // race condition, and if we find after the copy has completed that cancellation has
                // been requested, we assume the copy completed due to cancellation and throw.
                cancellationToken.ThrowIfCancellationRequested();

                Finish();
            }

            private void Finish()
            {
                // We cannot reuse this connection, so close it.
                _connection.Dispose();
                _connection = null;
            }

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new ValueTask(Task.FromCanceled(cancellationToken));
                }

                if (_connection == null)
                {
                    return new ValueTask(Task.FromException(new IOException(SR.net_http_io_write)));
                }

                if (source.Length == 0)
                {
                    return default;
                }

                ValueTask writeTask = _connection.WriteWithoutBufferingAsync(source);
                return writeTask.IsCompleted ?
                    writeTask :
                    new ValueTask(WaitWithConnectionCancellationAsync(writeTask, cancellationToken));
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return Task.FromCanceled(cancellationToken);
                }

                if (_connection == null)
                {
                    return Task.CompletedTask;
                }

                ValueTask flushTask = _connection.FlushAsync();
                return flushTask.IsCompleted ?
                    flushTask.AsTask() :
                    WaitWithConnectionCancellationAsync(flushTask, cancellationToken);
            }

            private async Task WaitWithConnectionCancellationAsync(ValueTask task, CancellationToken cancellationToken)
            {
                CancellationTokenRegistration ctr = _connection.RegisterCancellation(cancellationToken);
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception exc) when (ShouldWrapInOperationCanceledException(exc, cancellationToken))
                {
                    throw CreateOperationCanceledException(exc, cancellationToken);
                }
                finally
                {
                    ctr.Dispose();
                }
            }
        }
    }
}
