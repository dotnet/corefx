// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Security
{
    //
    // This is a wrapping stream that does data encryption/decryption based on a successfully authenticated SSPI context.
    //
    internal class SslStreamInternal
    {
        private static readonly AsyncCallback s_writeCallback = new AsyncCallback(WriteCallback);
        private static readonly AsyncProtocolCallback s_resumeAsyncWriteCallback = new AsyncProtocolCallback(ResumeAsyncWriteCallback);
        private static readonly AsyncProtocolCallback s_resumeAsyncReadCallback = new AsyncProtocolCallback(ResumeAsyncReadCallback);
        private static readonly AsyncProtocolCallback s_readHeaderCallback = new AsyncProtocolCallback(ReadHeaderCallback);
        private static readonly AsyncProtocolCallback s_readFrameCallback = new AsyncProtocolCallback(ReadFrameCallback);
        private static readonly Action<Task, object> s_freeWriteBufferCallback = FreeWriteBuffer;

        private const int FrameOverhead = 32;
        private const int ReadBufferSize = 4096 * 4 + FrameOverhead;         // We read in 16K chunks + headers.

        private readonly SslState _sslState;
        private int _nestedWrite;
        private int _nestedRead;
        private AsyncProtocolRequest _readProtocolRequest; // cached, reusable AsyncProtocolRequest used for read operations
        private AsyncProtocolRequest _writeProtocolRequest; // cached, reusable AsyncProtocolRequest used for write operations

        // Never updated directly, special properties are used.  This is the read buffer.
        private byte[] _internalBuffer;

        private int _internalOffset;
        private int _internalBufferCount;

        private int _decryptedBytesOffset;
        private int _decryptedBytesCount;

        internal SslStreamInternal(SslState sslState)
        {
            _sslState = sslState;

            _decryptedBytesOffset = 0;
            _decryptedBytesCount = 0;
        }

        private static void FreeWriteBuffer(Task t, object buffer)
        {
            ArrayPool<byte>.Shared.Return((byte[])buffer);
            t.GetAwaiter().GetResult();
        }

        //We will only free the read buffer if it
        //actually contains no decrypted or encrypted bytes
        private void ReturnReadBufferIfEmpty()
        {
            if (_internalBuffer != null && _decryptedBytesCount == 0 && _internalBufferCount == 0)
            {
                ArrayPool<byte>.Shared.Return(_internalBuffer);
                _internalBuffer = null;
                _internalBufferCount = 0;
                _internalOffset = 0;
                _decryptedBytesCount = 0;
                _decryptedBytesOffset = 0;
            }
        }

        ~SslStreamInternal()
        {
            if (_internalBuffer != null)
            {
                ArrayPool<byte>.Shared.Return(_internalBuffer);
                _internalBuffer = null;
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
                if (_decryptedBytesCount > 0)
                {
                    int b = _internalBuffer[_decryptedBytesOffset++];
                    _decryptedBytesCount--;
                    ReturnReadBufferIfEmpty();
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
            int bytesRead = Read(oneByte, 0, 1);
            Debug.Assert(bytesRead == 0 || bytesRead == 1);
            return bytesRead == 1 ? oneByte[0] : -1;
        }

        internal int Read(byte[] buffer, int offset, int count)
        {
            return ProcessRead(buffer, offset, count, null);
        }

        internal void Write(byte[] buffer, int offset, int count)
        {
            ProcessWrite(buffer, offset, count, null);
        }

        internal IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            var bufferResult = new BufferAsyncResult(this, buffer, offset, count, asyncState, asyncCallback);
            ProcessRead(buffer, offset, count, bufferResult);
            return bufferResult;
        }

        internal int EndRead(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            BufferAsyncResult bufferResult = asyncResult as BufferAsyncResult;
            if (bufferResult == null)
            {
                throw new ArgumentException(SR.Format(SR.net_io_async_result, asyncResult.GetType().FullName), nameof(asyncResult));
            }

            if (Interlocked.Exchange(ref _nestedRead, 0) == 0)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndRead"));
            }

            // No "artificial" timeouts implemented so far, InnerStream controls timeout.
            bufferResult.InternalWaitForCompletion();

            if (bufferResult.Result is Exception)
            {
                if (bufferResult.Result is IOException)
                {
                    throw (Exception)bufferResult.Result;
                }

                throw new IOException(SR.net_io_read, (Exception)bufferResult.Result);
            }

            return bufferResult.Int32Result;
        }

        internal IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
        {
            var lazyResult = new LazyAsyncResult(this, asyncState, asyncCallback);
            ProcessWrite(buffer, offset, count, lazyResult);
            return lazyResult;
        }

        internal void EndWrite(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }

            LazyAsyncResult lazyResult = asyncResult as LazyAsyncResult;
            if (lazyResult == null)
            {
                throw new ArgumentException(SR.Format(SR.net_io_async_result, asyncResult.GetType().FullName), nameof(asyncResult));
            }

            if (Interlocked.Exchange(ref _nestedWrite, 0) == 0)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndWrite"));
            }

            // No "artificial" timeouts implemented so far, InnerStream controls timeout.
            lazyResult.InternalWaitForCompletion();

            if (lazyResult.Result is Exception e)
            {
                if (e is IOException)
                {
                    ExceptionDispatchInfo.Throw(e);
                }

                throw new IOException(SR.net_io_write, e);
            }
        }

        private void ResetReadBuffer()
        {
            Debug.Assert(_decryptedBytesCount == 0);
            Debug.Assert(_internalBuffer == null || _internalBufferCount > 0);

            if (_internalBuffer == null)
            {
                _internalBuffer = ArrayPool<byte>.Shared.Rent(ReadBufferSize);
            }
            else if (_internalOffset > 0)
            {
                // We have buffered data at a non-zero offset.
                // To maximize the buffer space available for the next read,
                // copy the existing data down to the beginning of the buffer.
                Buffer.BlockCopy(_internalBuffer, _internalOffset, _internalBuffer, 0, _internalBufferCount);
                _internalOffset = 0;
            }
        }

        //
        // Validates user parameters for all Read/Write methods.
        //
        private void ValidateParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.net_offset_plus_count);
            }
        }

        private AsyncProtocolRequest GetOrCreateProtocolRequest(ref AsyncProtocolRequest aprField, LazyAsyncResult asyncResult)
        {
            AsyncProtocolRequest request = null;
            if (asyncResult != null)
            {
                // SslStreamInternal supports only a single read and a single write operation at a time.
                // As such, we can cache and reuse the AsyncProtocolRequest object that's used throughout
                // the implementation.
                request = aprField;
                if (request != null)
                {
                    request.Reset(asyncResult);
                }
                else
                {
                    aprField = request = new AsyncProtocolRequest(asyncResult);
                }
            }
            return request;
        }

        //
        // Sync write method.
        //
        private void ProcessWrite(byte[] buffer, int offset, int count, LazyAsyncResult asyncResult)
        {
            _sslState.CheckThrow(authSuccessCheck: true, shutdownCheck: true);
            ValidateParameters(buffer, offset, count);

            if (Interlocked.Exchange(ref _nestedWrite, 1) == 1)
            {
                throw new NotSupportedException(SR.Format(SR.net_io_invalidnestedcall, "Write", "write"));
            }

            // If this is an async operation, get the AsyncProtocolRequest to use.
            // We do this only after we verify we're the sole write operation in flight.
            AsyncProtocolRequest asyncRequest = GetOrCreateProtocolRequest(ref _writeProtocolRequest, asyncResult);

            bool failed = false;

            try
            {
                StartWriting(buffer, offset, count, asyncRequest);
            }
            catch (Exception e)
            {
                _sslState.FinishWrite();

                failed = true;
                if (e is IOException)
                {
                    throw;
                }

                throw new IOException(SR.net_io_write, e);
            }
            finally
            {
                if (asyncRequest == null || failed)
                {
                    _nestedWrite = 0;
                }
            }
        }

        private void StartWriting(byte[] buffer, int offset, int count, AsyncProtocolRequest asyncRequest)
        {
            if (asyncRequest != null)
            {
                asyncRequest.SetNextRequest(buffer, offset, count, s_resumeAsyncWriteCallback);
            }

            // We loop to this method from the callback.
            // If the last chunk was just completed from async callback (count < 0), we complete user request.
            if (count >= 0)
            {
                do
                {
                    if (count == 0 && !SslStreamPal.CanEncryptEmptyMessage)
                    {
                        // If it's an empty message and the PAL doesn't support that,
                        // we're done.
                        break;
                    }

                    // Request a write IO slot.
                    if (_sslState.CheckEnqueueWrite(asyncRequest))
                    {
                        // Operation is async and has been queued, return.
                        return;
                    }

                    int chunkBytes = Math.Min(count, _sslState.MaxDataSize);
                    byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(chunkBytes + FrameOverhead);
                    byte[] outBuffer = rentedBuffer;
                    int encryptedBytes = 0;

                    SecurityStatusPal status = _sslState.EncryptData(buffer, offset, chunkBytes, ref outBuffer, out encryptedBytes);
                    if (status.ErrorCode != SecurityStatusPalErrorCode.OK)
                    {
                        // Re-handshake status is not supported.
                        ProtocolToken message = new ProtocolToken(null, status);
                        ArrayPool<byte>.Shared.Return(rentedBuffer);
                        throw new IOException(SR.net_io_encrypt, message.GetException());
                    }

                    if (asyncRequest != null)
                    {
                        // Prepare for the next request.
                        asyncRequest.SetNextRequest(buffer, offset + chunkBytes, count - chunkBytes, s_resumeAsyncWriteCallback);
                        Task t = _sslState.InnerStream.WriteAsync(outBuffer, 0, encryptedBytes);
                        if (t.IsCompleted)
                        {
                            ArrayPool<byte>.Shared.Return(rentedBuffer);
                            t.GetAwaiter().GetResult();
                        }
                        else
                        {
                            t = t.ContinueWith(s_freeWriteBufferCallback, rentedBuffer, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                            IAsyncResult ar = TaskToApm.Begin(t, s_writeCallback, asyncRequest);
                            if (!ar.CompletedSynchronously)
                            {
                                return;
                            }
                            TaskToApm.End(ar);
                        }
                    }
                    else
                    {
                        try
                        {
                            _sslState.InnerStream.Write(outBuffer, 0, encryptedBytes);
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(rentedBuffer);
                        }
                    }

                    offset += chunkBytes;
                    count -= chunkBytes;

                    // Release write IO slot.
                    _sslState.FinishWrite();

                } while (count != 0);
            }

            if (asyncRequest != null)
            {
                asyncRequest.CompleteUser();
            }
        }

        // Fill the buffer up to the minimum specified size (or more, if possible).
        // Returns 0 if EOF on initial read, otherwise throws on EOF.
        // Returns minSize on success.
        private int FillBuffer(int minSize)
        {
            Debug.Assert(_internalOffset == 0);
            Debug.Assert(minSize > _internalBufferCount);

            int initialCount = _internalBufferCount;
            do
            {
                int bytes = _sslState.InnerStream.Read(_internalBuffer, _internalBufferCount, _internalBuffer.Length - _internalBufferCount);
                if (bytes == 0)
                {
                    if (_internalBufferCount != initialCount)
                    {
                        // We read some bytes, but not as many as we expected, so throw.
                        throw new IOException(SR.net_io_eof);
                    }

                    return 0;
                }

                _internalBufferCount += bytes;
            } while (_internalBufferCount < minSize);

            return minSize;
        }

        // Fill the buffer up to the minimum specified size (or more, if possible).
        // Returns 0 if EOF on initial read, otherwise throws on EOF.
        // Returns minSize on success.
        public async Task<int> FillBufferAsync(int minSize)
        {
            Debug.Assert(_internalOffset == 0);
            Debug.Assert(minSize > _internalBufferCount);

            int initialCount = _internalBufferCount;
            do
            {
                int bytes = await _sslState.InnerStream.ReadAsync(_internalBuffer, _internalBufferCount, _internalBuffer.Length - _internalBufferCount, CancellationToken.None).ConfigureAwait(false);
                if (bytes == 0)
                {
                    if (_internalBufferCount != initialCount)
                    {
                        // We read some bytes, but not as many as we expected, so throw.
                        throw new IOException(SR.net_io_eof);
                    }

                    return 0;
                }

                _internalBufferCount += bytes;
            } while (_internalBufferCount < minSize);

            return minSize;
        }

        private static void CompleteFromCompletedTask(Task<int> task, AsyncProtocolRequest asyncRequest)
        {
            Debug.Assert(task.IsCompleted);
            if (task.IsCompletedSuccessfully)
            {
                asyncRequest.CompleteRequest(task.Result);
            }
            else if (task.IsFaulted)
            {
                asyncRequest.CompleteUserWithError(task.Exception.InnerException);
            }
            else
            {
                asyncRequest.CompleteUserWithError(new OperationCanceledException());
            }
        }

        // Returns true if pending, false if completed
        private static bool TaskToAsyncProtocolRequest(Task<int> task, AsyncProtocolRequest asyncRequest, AsyncProtocolCallback asyncCallback, out int result)
        {
            // Parameters other than asyncCallback are not used
            asyncRequest.SetNextRequest(null, 0, 0, asyncCallback);

            if (task.IsCompleted)
            {
                CompleteFromCompletedTask(task, asyncRequest);
            }
            else
            {
                task.ContinueWith((t, s) => CompleteFromCompletedTask(t, (AsyncProtocolRequest)s),
                    asyncRequest,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
                    TaskScheduler.Default);
            }

            if (asyncRequest.MustCompleteSynchronously)
            {
                result = asyncRequest.Result;
                return false;
            }

            result = 0;
            return true;
        }

        private int EnsureBufferedBytes(int minSize, AsyncProtocolRequest asyncRequest, AsyncProtocolCallback asyncCallback)
        {
            if (_internalBufferCount >= minSize)
            {
                return minSize;
            }
                        
            int bytesRead;
            if (asyncRequest != null)
            {
                if (TaskToAsyncProtocolRequest(
                        FillBufferAsync(minSize),
                        asyncRequest,
                        asyncCallback,
                        out bytesRead))
                {
                    return -1;
                }
            }
            else
            {
                bytesRead = FillBuffer(minSize);
            }

            Debug.Assert(bytesRead == 0 || bytesRead == minSize);
            return bytesRead;
        }

        private void ConsumeBufferedBytes(int byteCount)
        {
            Debug.Assert(byteCount >= 0);
            Debug.Assert(byteCount <= _internalBufferCount);

            _internalOffset += byteCount;
            _internalBufferCount -= byteCount;

            ReturnReadBufferIfEmpty();
        }

        private int CopyDecryptedData(byte[] buffer, int offset, int count)
        {
            Debug.Assert(_decryptedBytesCount > 0);

            int copyBytes = _decryptedBytesCount > count ? count : _decryptedBytesCount;
            if (copyBytes != 0)
            {
                Buffer.BlockCopy(_internalBuffer, _decryptedBytesOffset, buffer, offset, copyBytes);

                _decryptedBytesOffset += copyBytes;
                _decryptedBytesCount -= copyBytes;
            }
            ReturnReadBufferIfEmpty();
            return copyBytes;
        }

        //
        // Combined sync/async read method. For sync request asyncRequest==null.
        //
        private int ProcessRead(byte[] buffer, int offset, int count, BufferAsyncResult asyncResult)
        {
            ValidateParameters(buffer, offset, count);

            if (Interlocked.Exchange(ref _nestedRead, 1) == 1)
            {
                throw new NotSupportedException(SR.Format(SR.net_io_invalidnestedcall, (asyncResult != null ? "BeginRead" : "Read"), "read"));
            }

            // If this is an async operation, get the AsyncProtocolRequest to use.
            // We do this only after we verify we're the sole write operation in flight.
            AsyncProtocolRequest asyncRequest = GetOrCreateProtocolRequest(ref _readProtocolRequest, asyncResult);

            bool failed = false;

            try
            {
                if (_decryptedBytesCount != 0)
                {
                    int copyBytes = CopyDecryptedData(buffer, offset, count);

                    asyncRequest?.CompleteUser(copyBytes);

                    return copyBytes;
                }

                return StartReading(buffer, offset, count, asyncRequest);
            }
            catch (Exception e)
            {
                _sslState.FinishRead(null);
                failed = true;

                if (e is IOException)
                {
                    throw;
                }

                throw new IOException(SR.net_io_read, e);
            }
            finally
            {
                if (asyncRequest == null || failed)
                {
                    _nestedRead = 0;
                }
            }
        }

        //
        // To avoid recursion when decrypted 0 bytes this method will loop until a decrypted result at least 1 byte.
        //
        private int StartReading(byte[] buffer, int offset, int count, AsyncProtocolRequest asyncRequest)
        {
            int result = 0;

            if (_decryptedBytesCount != 0)
            {
                NetEventSource.Fail(this, $"Previous frame was not consumed. _decryptedBytesCount: {_decryptedBytesCount}");
            }

            do
            {
                if (asyncRequest != null)
                {
                    asyncRequest.SetNextRequest(buffer, offset, count, s_resumeAsyncReadCallback);
                }

                int copyBytes = _sslState.CheckEnqueueRead(buffer, offset, count, asyncRequest);
                if (copyBytes == 0)
                {
                    // Queued but not completed!
                    return 0;
                }

                if (copyBytes != -1)
                {
                    asyncRequest?.CompleteUser(copyBytes);

                    return copyBytes;
                }
            }

            // When we read -1 bytes means we have decrypted 0 bytes or rehandshaking, need looping.
            while ((result = StartFrameHeader(buffer, offset, count, asyncRequest)) == -1);

            return result;
        }

        private int StartFrameHeader(byte[] buffer, int offset, int count, AsyncProtocolRequest asyncRequest)
        {
            ResetReadBuffer();
            int readBytes = EnsureBufferedBytes(SecureChannel.ReadHeaderSize, asyncRequest, s_readHeaderCallback);
            if (readBytes == -1)
            {
                Debug.Assert(asyncRequest != null);
                return 0;
            }

            return StartFrameBody(readBytes, buffer, offset, count, asyncRequest);
        }

        private int StartFrameBody(int readBytes, byte[] buffer, int offset, int count, AsyncProtocolRequest asyncRequest)
        {
            if (readBytes == 0)
            {
                // EOF 
                asyncRequest?.CompleteUser(0);
                return 0;
            }

            Debug.Assert(readBytes == SecureChannel.ReadHeaderSize);

            int payloadBytes = _sslState.GetRemainingFrameSize(_internalBuffer, _internalOffset, readBytes);
            if (payloadBytes < 0)
            {
                throw new IOException(SR.net_frame_read_size);
            }

            readBytes = EnsureBufferedBytes(SecureChannel.ReadHeaderSize + payloadBytes, asyncRequest, s_readFrameCallback);
            if (readBytes == -1)
            {
                Debug.Assert(asyncRequest != null);
                return 0;
            }

            Debug.Assert(readBytes == 0 || readBytes == SecureChannel.ReadHeaderSize + payloadBytes);

            return ProcessFrameBody(readBytes, buffer, offset, count, asyncRequest);
        }

        //
        // readBytes == SSL Data Payload size on input or 0 on EOF.
        //
        private int ProcessFrameBody(int readBytes, byte[] buffer, int offset, int count, AsyncProtocolRequest asyncRequest)
        {
            if (readBytes == 0)
            {
                // EOF
                throw new IOException(SR.net_io_eof);
            }

            // At this point, readBytes contains the size of the header plus body.
            // Set _decrytpedBytesOffset/Count to the current frame we have (including header)
            // DecryptData will decrypt in-place and modify these to point to the actual decrypted data, which may be smaller.
            _decryptedBytesOffset = _internalOffset;
            _decryptedBytesCount = readBytes;
            SecurityStatusPal status = _sslState.DecryptData(_internalBuffer, ref _decryptedBytesOffset, ref _decryptedBytesCount);

            // Treat the bytes we just decrypted as consumed
            // Note, we won't do another buffer read until the decrypted bytes are processed
            ConsumeBufferedBytes(readBytes);

            if (status.ErrorCode != SecurityStatusPalErrorCode.OK)
            {
                byte[] extraBuffer = null;
                if (_decryptedBytesCount != 0)
                {
                    extraBuffer = new byte[_decryptedBytesCount];
                    Buffer.BlockCopy(_internalBuffer, _decryptedBytesOffset, extraBuffer, 0, _decryptedBytesCount);

                    _decryptedBytesCount = 0;
                }

                return ProcessReadErrorCode(status, asyncRequest, extraBuffer);
            }

            if (_decryptedBytesCount == 0)
            {
                // Read again since remote side has sent encrypted 0 bytes.
                return -1;
            }

            int copyBytes = CopyDecryptedData(buffer, offset, count);

            _sslState.FinishRead(null);
            asyncRequest?.CompleteUser(copyBytes);

            return copyBytes;
        }

        private int ProcessReadErrorCode(SecurityStatusPal status, AsyncProtocolRequest asyncRequest, byte[] extraBuffer)
        {
            ProtocolToken message = new ProtocolToken(null, status);
            if (NetEventSource.IsEnabled)
                NetEventSource.Info(null, $"***Processing an error Status = {message.Status}");

            if (message.Renegotiate)
            {
                _sslState.ReplyOnReAuthentication(extraBuffer);

                // Loop on read.
                return -1;
            }

            if (message.CloseConnection)
            {
                _sslState.FinishRead(null);
                asyncRequest?.CompleteUser(0);

                return 0;
            }

            throw new IOException(SR.net_io_decrypt, message.GetException());
        }

        private static void WriteCallback(IAsyncResult transportResult)
        {
            if (transportResult.CompletedSynchronously)
            {
                return;
            }

            if (!(transportResult.AsyncState is AsyncProtocolRequest))
            {
                NetEventSource.Fail(transportResult, "State type is wrong, expected AsyncProtocolRequest.");
            }

            AsyncProtocolRequest asyncRequest = (AsyncProtocolRequest)transportResult.AsyncState;

            var sslStream = (SslStreamInternal)asyncRequest.AsyncObject;

            try
            {
                TaskToApm.End(transportResult);
                sslStream._sslState.FinishWrite();

                if (asyncRequest.Count == 0)
                {
                    // This was the last chunk.
                    asyncRequest.Count = -1;
                }

                sslStream.StartWriting(asyncRequest.Buffer, asyncRequest.Offset, asyncRequest.Count, asyncRequest);
            }
            catch (Exception e)
            {
                if (asyncRequest.IsUserCompleted)
                {
                    // This will throw on a worker thread.
                    throw;
                }

                sslStream._sslState.FinishWrite();
                asyncRequest.CompleteUserWithError(e);
            }
        }

        //
        // This is used in a rare situation when async Read is resumed from completed handshake.
        //
        private static void ResumeAsyncReadCallback(AsyncProtocolRequest request)
        {
            try
            {
                ((SslStreamInternal)request.AsyncObject).StartReading(request.Buffer, request.Offset, request.Count, request);
            }
            catch (Exception e)
            {
                if (request.IsUserCompleted)
                {
                    // This will throw on a worker thread.
                    throw;
                }

                ((SslStreamInternal)request.AsyncObject)._sslState.FinishRead(null);
                request.CompleteUserWithError(e);
            }
        }

        //
        // This is used in a rare situation when async Write is resumed from completed handshake.
        //
        private static void ResumeAsyncWriteCallback(AsyncProtocolRequest asyncRequest)
        {
            try
            {
                ((SslStreamInternal)asyncRequest.AsyncObject).StartWriting(asyncRequest.Buffer, asyncRequest.Offset, asyncRequest.Count, asyncRequest);
            }
            catch (Exception e)
            {
                if (asyncRequest.IsUserCompleted)
                {
                    // This will throw on a worker thread.
                    throw;
                }

                ((SslStreamInternal)asyncRequest.AsyncObject)._sslState.FinishWrite();
                asyncRequest.CompleteUserWithError(e);
            }
        }

        private static void ReadHeaderCallback(AsyncProtocolRequest asyncRequest)
        {
            try
            {
                SslStreamInternal sslStream = (SslStreamInternal)asyncRequest.AsyncObject;
                BufferAsyncResult bufferResult = (BufferAsyncResult)asyncRequest.UserAsyncResult;
                if (-1 == sslStream.StartFrameBody(asyncRequest.Result, bufferResult.Buffer, bufferResult.Offset, bufferResult.Count, asyncRequest))
                {
                    // in case we decrypted 0 bytes start another reading.
                    sslStream.StartReading(bufferResult.Buffer, bufferResult.Offset, bufferResult.Count, asyncRequest);
                }
            }
            catch (Exception e)
            {
                if (asyncRequest.IsUserCompleted)
                {
                    // This will throw on a worker thread.
                    throw;
                }

                asyncRequest.CompleteUserWithError(e);
            }
        }

        private static void ReadFrameCallback(AsyncProtocolRequest asyncRequest)
        {
            try
            {
                SslStreamInternal sslStream = (SslStreamInternal)asyncRequest.AsyncObject;
                BufferAsyncResult bufferResult = (BufferAsyncResult)asyncRequest.UserAsyncResult;
                if (-1 == sslStream.ProcessFrameBody(asyncRequest.Result, bufferResult.Buffer, bufferResult.Offset, bufferResult.Count, asyncRequest))
                {
                    // in case we decrypted 0 bytes start another reading.
                    sslStream.StartReading(bufferResult.Buffer, bufferResult.Offset, bufferResult.Count, asyncRequest);
                }
            }
            catch (Exception e)
            {
                if (asyncRequest.IsUserCompleted)
                {
                    // This will throw on a worker thread.
                    throw;
                }

                asyncRequest.CompleteUserWithError(e);
            }
        }
    }
}
