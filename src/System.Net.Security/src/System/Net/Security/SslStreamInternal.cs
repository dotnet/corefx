// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Security.SslStream;

namespace System.Net.Security
{
    //
    // This is a wrapping stream that does data encryption/decryption based on a successfully authenticated SSPI context.
    //
    internal partial class SslStreamInternal : IDisposable
    {
        private readonly SslStream _sslState;
        private int _nestedWrite;
        private int _nestedRead;

        internal SslStreamInternal(SslStream sslState)
        {
            _sslState = sslState;

            _sslState._decryptedBytesOffset = 0;
            _sslState._decryptedBytesCount = 0;
        }
        
        ~SslStreamInternal()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);

            if (_sslState._internalBuffer == null)
            {
                // Suppress finalizer if the read buffer was returned.
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            // Ensure a Read operation is not in progress,
            // block potential reads since SslStream is disposing.
            // This leaves the _nestedRead = 1, but that's ok, since
            // subsequent Reads first check if the context is still available.
            if (Interlocked.CompareExchange(ref _nestedRead, 1, 0) == 0)
            {
                byte[] buffer = _sslState._internalBuffer;
                if (buffer != null)
                {
                    _sslState._internalBuffer = null;
                    _sslState._internalBufferCount = 0;
                    _sslState._internalOffset = 0;
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        internal int ReadByte()
        {
            if (Interlocked.Exchange(ref _nestedRead, 1) == 1)
            {
                throw new NotSupportedException(SR.Format(SR.net_io_invalidnestedcall, "ReadByte", "read"));
            }

            // If there's any data in the buffer, take one byte, and we're done.
            try
            {
                if (_sslState._decryptedBytesCount > 0)
                {
                    int b = _sslState._internalBuffer[_sslState._decryptedBytesOffset++];
                    _sslState._decryptedBytesCount--;
                    _sslState.ReturnReadBufferIfEmpty();
                    return b;
                }
            }
            finally
            {
                // Regardless of whether we were able to read a byte from the buffer,
                // reset the read tracking.  If we weren't able to read a byte, the
                // subsequent call to Read will set the flag again.
                _nestedRead = 0;
            }

            // Otherwise, fall back to reading a byte via Read, the same way Stream.ReadByte does.
            // This allocation is unfortunate but should be relatively rare, as it'll only occur once
            // per buffer fill internally by Read.
            byte[] oneByte = new byte[1];
            int bytesRead = _sslState.Read(oneByte, 0, 1);
            Debug.Assert(bytesRead == 0 || bytesRead == 1);
            return bytesRead == 1 ? oneByte[0] : -1;
        }
                
        internal async ValueTask<int> ReadAsyncInternal<TReadAdapter>(TReadAdapter adapter, Memory<byte> buffer)
            where TReadAdapter : ISslReadAdapter
        {
            if (Interlocked.Exchange(ref _nestedRead, 1) == 1)
            {
                throw new NotSupportedException(SR.Format(SR.net_io_invalidnestedcall, nameof(SslStream.ReadAsync), "read"));
            }

            try
            {
                while (true)
                {
                    int copyBytes;
                    if (_sslState._decryptedBytesCount != 0)
                    {
                        copyBytes = _sslState.CopyDecryptedData(buffer);

                        _sslState.FinishRead(null);

                        return copyBytes;
                    }

                    copyBytes = await adapter.LockAsync(buffer).ConfigureAwait(false);
                    if (copyBytes > 0)
                    {
                        return copyBytes;
                    }

                    _sslState.ResetReadBuffer();
                    int readBytes = await FillBufferAsync(adapter, SecureChannel.ReadHeaderSize).ConfigureAwait(false);
                    if (readBytes == 0)
                    {
                        return 0;
                    }

                    int payloadBytes = _sslState.GetRemainingFrameSize(_sslState._internalBuffer, _sslState._internalOffset, readBytes);
                    if (payloadBytes < 0)
                    {
                        throw new IOException(SR.net_frame_read_size);
                    }

                    readBytes = await FillBufferAsync(adapter, SecureChannel.ReadHeaderSize + payloadBytes).ConfigureAwait(false);
                    Debug.Assert(readBytes >= 0);
                    if (readBytes == 0)
                    {
                        throw new IOException(SR.net_io_eof);
                    }

                    // At this point, readBytes contains the size of the header plus body.
                    // Set _decrytpedBytesOffset/Count to the current frame we have (including header)
                    // DecryptData will decrypt in-place and modify these to point to the actual decrypted data, which may be smaller.
                    _sslState._decryptedBytesOffset = _sslState._internalOffset;
                    _sslState._decryptedBytesCount = readBytes;
                    SecurityStatusPal status = _sslState.DecryptData();

                    // Treat the bytes we just decrypted as consumed
                    // Note, we won't do another buffer read until the decrypted bytes are processed
                    _sslState.ConsumeBufferedBytes(readBytes);

                    if (status.ErrorCode != SecurityStatusPalErrorCode.OK)
                    {
                        byte[] extraBuffer = null;
                        if (_sslState._decryptedBytesCount != 0)
                        {
                            extraBuffer = new byte[_sslState._decryptedBytesCount];
                            Buffer.BlockCopy(_sslState._internalBuffer, _sslState._decryptedBytesOffset, extraBuffer, 0, _sslState._decryptedBytesCount);

                            _sslState._decryptedBytesCount = 0;
                        }

                        ProtocolToken message = new ProtocolToken(null, status);
                        if (NetEventSource.IsEnabled)
                            NetEventSource.Info(null, $"***Processing an error Status = {message.Status}");

                        if (message.Renegotiate)
                        {
                            if (!_sslState._sslAuthenticationOptions.AllowRenegotiation)
                            {
                                throw new IOException(SR.net_ssl_io_renego);
                            }

                            _sslState.ReplyOnReAuthentication(extraBuffer);

                            // Loop on read.
                            continue;
                        }

                        if (message.CloseConnection)
                        {
                            _sslState.FinishRead(null);
                            return 0;
                        }

                        throw new IOException(SR.net_io_decrypt, message.GetException());
                    }
                }
            }
            catch (Exception e)
            {
                _sslState.FinishRead(null);

                if (e is IOException)
                {
                    throw;
                }

                throw new IOException(SR.net_io_read, e);
            }
            finally
            {
                _nestedRead = 0;
            }
        }

        internal ValueTask WriteAsyncInternal<TWriteAdapter>(TWriteAdapter writeAdapter, ReadOnlyMemory<byte> buffer)
            where TWriteAdapter : struct, ISslWriteAdapter
        {
            _sslState.CheckThrow(authSuccessCheck: true, shutdownCheck: true);

            if (buffer.Length == 0 && !SslStreamPal.CanEncryptEmptyMessage)
            {
                // If it's an empty message and the PAL doesn't support that, we're done.
                return default;
            }

            if (Interlocked.Exchange(ref _nestedWrite, 1) == 1)
            {
                throw new NotSupportedException(SR.Format(SR.net_io_invalidnestedcall, nameof(SslStream.WriteAsync), "write"));
            }

            ValueTask t = buffer.Length < _sslState.MaxDataSize ?
                    _sslState.WriteSingleChunk(writeAdapter, buffer) :
                    new ValueTask(_sslState.WriteAsyncChunked(writeAdapter, buffer));

            if (t.IsCompletedSuccessfully)
            {
                _nestedWrite = 0;
                return t;
            }
            return new ValueTask(ExitWriteAsync(t));

            async Task ExitWriteAsync(ValueTask task)
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _sslState.FinishWrite();

                    if (e is IOException)
                    {
                        throw;
                    }

                    throw new IOException(SR.net_io_write, e);
                }
                finally
                {
                    _nestedWrite = 0;
                }
            }
        }

        private ValueTask<int> FillBufferAsync<TReadAdapter>(TReadAdapter adapter, int minSize)
            where TReadAdapter : ISslReadAdapter
        {
            if (_sslState._internalBufferCount >= minSize)
            {
                return new ValueTask<int>(minSize);
            }

            int initialCount = _sslState._internalBufferCount;
            do
            {
                ValueTask<int> t = adapter.ReadAsync(_sslState._internalBuffer, _sslState._internalBufferCount, _sslState._internalBuffer.Length - _sslState._internalBufferCount);
                if (!t.IsCompletedSuccessfully)
                {
                    return InternalFillBufferAsync(adapter, t, minSize, initialCount);
                }
                int bytes = t.Result;
                if (bytes == 0)
                {
                    if (_sslState._internalBufferCount != initialCount)
                    {
                        // We read some bytes, but not as many as we expected, so throw.
                        throw new IOException(SR.net_io_eof);
                    }

                    return new ValueTask<int>(0);
                }

                _sslState._internalBufferCount += bytes;
            } while (_sslState._internalBufferCount < minSize);

            return new ValueTask<int>(minSize);

            async ValueTask<int> InternalFillBufferAsync(TReadAdapter adap, ValueTask<int> task, int min, int initial)
            {
                while (true)
                {
                    int b = await task.ConfigureAwait(false);
                    if (b == 0)
                    {
                        if (_sslState._internalBufferCount != initial)
                        {
                            throw new IOException(SR.net_io_eof);
                        }

                        return 0;
                    }

                    _sslState._internalBufferCount += b;
                    if (_sslState._internalBufferCount >= min)
                    {
                        return min;
                    }

                    task = adap.ReadAsync(_sslState._internalBuffer, _sslState._internalBufferCount, _sslState._internalBuffer.Length - _sslState._internalBufferCount);
                }
            }
        }
    }
}
