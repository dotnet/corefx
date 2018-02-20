// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal partial class HttpConnection
    {
        private sealed class ChunkedEncodingReadStream : HttpContentReadStream
        {
            /// <summary>How long a chunk indicator is allowed to be.</summary>
            /// <remarks>
            /// While most chunks indicators will contain no more than ulong.MaxValue.ToString("X").Length characters,
            /// "chunk extensions" are allowed. We place a limit on how long a line can be to avoid OOM issues if an
            /// infinite chunk length is sent.  This value is arbitrary and can be changed as needed.
            /// </remarks>
            private const int MaxChunkBytesAllowed = 16*1024;
            /// <summary>How long a trailing header can be.  This value is arbitrary and can be changed as needed.</summary>
            private const int MaxTrailingHeaderLength = 16*1024;
            /// <summary>The number of bytes remaining in the chunk.</summary>
            private ulong _chunkBytesRemaining;
            /// <summary>The current state of the parsing state machine for the chunked response.</summary>
            private ParsingState _state = ParsingState.ExpectChunkHeader;

            public ChunkedEncodingReadStream(HttpConnection connection) : base(connection) { }

            public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Cancellation requested.
                    return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
                }

                if (_connection == null || destination.Length == 0)
                {
                    // Response body fully consumed or the caller didn't ask for any data.
                    return new ValueTask<int>(0);
                }

                // Try to consume from data we already have in the buffer.
                int bytesRead = ReadChunksFromConnectionBuffer(destination.Span);
                if (bytesRead > 0)
                {
                    return new ValueTask<int>(bytesRead);
                }

                // Nothing available to consume.  Fall back to I/O.
                return ReadAsyncCore(destination, cancellationToken);
            }

            private async ValueTask<int> ReadAsyncCore(Memory<byte> destination, CancellationToken cancellationToken)
            {
                // Should only be called if ReadChunksFromConnectionBuffer returned 0.

                Debug.Assert(_connection != null);
                Debug.Assert(destination.Length > 0);

                CancellationTokenRegistration ctr = _connection.RegisterCancellation(cancellationToken);
                try
                {
                    while (true)
                    {
                        if (_state == ParsingState.Ending)
                        {
                            await ConsumeTrailingHeaders().ConfigureAwait(false);
                            Finish();
                            return 0;
                        }

                        // We're only here if we need more data to make forward progress.
                        await _connection.FillAsync();

                        // Now that we have more, see if we can get any response data, and if
                        // we can we're done.
                        int bytesCopied = ReadChunksFromConnectionBuffer(destination.Span);
                        if (bytesCopied > 0)
                        {
                            return bytesCopied;
                        }
                    }
                }
                catch (Exception exc) when (ShouldWrapInOperationCanceledException(exc, cancellationToken))
                {
                    throw new OperationCanceledException(s_cancellationMessage, exc, cancellationToken);
                }
                finally
                {
                    ctr.Dispose();
                }
            }

            public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            {
                ValidateCopyToArgs(this, destination, bufferSize);

                return
                    cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) :
                    _connection == null ? Task.CompletedTask :
                    CopyToAsyncCore(destination, cancellationToken);
            }

            private async Task CopyToAsyncCore(Stream destination, CancellationToken cancellationToken)
            {
                CancellationTokenRegistration ctr = _connection.RegisterCancellation(cancellationToken);
                try
                {
                    while (true)
                    {
                        while (true)
                        {
                            ReadOnlyMemory<byte> bytesRead = ReadChunkFromConnectionBuffer(int.MaxValue);
                            if (bytesRead.Length == 0)
                            {
                                break;
                            }
                            await destination.WriteAsync(bytesRead, cancellationToken).ConfigureAwait(false);
                        }

                        if (_state == ParsingState.Ending)
                        {
                            await ConsumeTrailingHeaders().ConfigureAwait(false);
                            Finish();
                            return;
                        }

                        await _connection.FillAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception exc) when (ShouldWrapInOperationCanceledException(exc, cancellationToken))
                {
                    throw new OperationCanceledException(s_cancellationMessage, exc, cancellationToken);
                }
                finally
                {
                    ctr.Dispose();
                }
            }

            private async Task ConsumeTrailingHeaders()
            {
                while (true)
                {
                    _connection._allowedReadLineBytes = MaxTrailingHeaderLength;
                    if (LineIsEmpty(await _connection.ReadNextLineAsync().ConfigureAwait(false)))
                    {
                        break;
                    }
                }
            }

            private void Finish()
            {
                _connection.ReturnConnectionToPool();
                _connection = null;
            }

            private int ReadChunksFromConnectionBuffer(Span<byte> destination)
            {
                int totalBytesRead = 0;
                while (destination.Length > 0)
                {
                    ReadOnlyMemory<byte> bytesRead = ReadChunkFromConnectionBuffer(destination.Length);
                    Debug.Assert(bytesRead.Length <= destination.Length);
                    if (bytesRead.Length == 0)
                    {
                        break;
                    }

                    totalBytesRead += bytesRead.Length;
                    bytesRead.Span.CopyTo(destination);
                    destination = destination.Slice(bytesRead.Length);
                }
                return totalBytesRead;
            }

            private ReadOnlyMemory<byte> ReadChunkFromConnectionBuffer(int maxBytesToRead)
            {
                ReadOnlySpan<byte> currentLine;
                switch (_state)
                {
                    case ParsingState.ExpectChunkHeader:
                        // Read the chunk header line.
                        _connection._allowedReadLineBytes = MaxChunkBytesAllowed;
                        if (!_connection.TryReadNextLine(out currentLine))
                        {
                            // Could not get a whole line, so we can't parse the chunk header.
                            return default;
                        }

                        // Parse the hex value from it.
                        if (!Utf8Parser.TryParse(currentLine, out ulong chunkSize, out int bytesConsumed, 'X'))
                        {
                            throw new IOException(SR.net_http_invalid_response);
                        }
                        _chunkBytesRemaining = chunkSize;

                        // If there's a chunk extension after the chunk size, validate it.
                        if (bytesConsumed != currentLine.Length)
                        {
                            ValidateChunkExtension(currentLine.Slice(bytesConsumed));
                        }

                        // Proceed to handle the chunk.  If there's data in it, go read it.
                        // Otherwise, finish handling the response.
                        if (chunkSize > 0)
                        {
                            _state = ParsingState.ExpectChunkData;
                            goto case ParsingState.ExpectChunkData;
                        }
                        else
                        {
                            _state = ParsingState.Ending;
                            return default;
                        }

                    case ParsingState.ExpectChunkData:
                        Debug.Assert(_chunkBytesRemaining > 0);
                        ReadOnlyMemory<byte> connectionBuffer = _connection.RemainingBuffer;
                        if (connectionBuffer.Length == 0 || maxBytesToRead == 0)
                        {
                            return default;
                        }

                        int bytesToConsume = Math.Min(maxBytesToRead, (int)Math.Min((ulong)connectionBuffer.Length, _chunkBytesRemaining));
                        Debug.Assert(bytesToConsume > 0);

                        _connection.ConsumeFromRemainingBuffer(bytesToConsume);
                        _chunkBytesRemaining -= (ulong)bytesToConsume;
                        if (_chunkBytesRemaining == 0)
                        {
                            _state = ParsingState.ExpectChunkTerminator;
                        }

                        return connectionBuffer.Slice(0, bytesToConsume);

                    case ParsingState.ExpectChunkTerminator:
                        _connection._allowedReadLineBytes = MaxChunkBytesAllowed;
                        if (!_connection.TryReadNextLine(out currentLine))
                        {
                            return default;
                        }

                        if (currentLine.Length != 0)
                        {
                            ThrowInvalidHttpResponse();
                        }

                        _state = ParsingState.ExpectChunkHeader;
                        goto case ParsingState.ExpectChunkHeader;

                    // Done processing all data.
                    default:
                    case ParsingState.Ending:
                        Debug.Assert(_state == ParsingState.Ending, $"Unknown state: {_state}");
                        return default;
                }
            }

            private static void ValidateChunkExtension(ReadOnlySpan<byte> lineAfterChunkSize)
            {
                // Until we see the ';' denoting the extension, the line after the chunk size
                // must contain only tabs and spaces.  After the ';', anything goes.
                for (int i = 0; i < lineAfterChunkSize.Length; i++)
                {
                    byte c = lineAfterChunkSize[i];
                    if (c == ';')
                    {
                        break;
                    }
                    else if (c != ' ' && c != '\t') // not called out in the RFC, but WinHTTP allows it
                    {
                        throw new IOException(SR.net_http_invalid_response);
                    }
                }
            }

            private enum ParsingState : byte
            {
                ExpectChunkHeader,
                ExpectChunkData,
                ExpectChunkTerminator,
                Ending
            }
        }
    }
}
