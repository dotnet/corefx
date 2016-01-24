// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace System.IO
{
    /// <summary>
    /// One of the design goals here is to prevent the buffer from getting in the way and slowing
    /// down underlying stream accesses when it is not needed. If you always read &amp; write for sizes
    /// greater than the internal buffer size, then this class may not even allocate the internal buffer.
    /// See a large comment in Write for the details of the write buffer heuristic.
    /// 
    /// This class buffers reads &amp; writes in a shared buffer.
    /// (If you maintained two buffers separately, one operation would always trash the other buffer
    /// anyways, so we might as well use one buffer.) 
    /// The assumption here is you will almost always be doing a series of reads or writes, but rarely
    /// alternate between the two of them on the same stream.
    ///
    /// Class Invariants:
    /// The class has one buffer, shared for reading &amp; writing.
    /// It can only be used for one or the other at any point in time - not both.
    /// The following should be true:
    /// <![CDATA[
    ///   * 0 <= _readPos <= _readLen < _bufferSize
    ///   * 0 <= _writePos < _bufferSize
    ///   * _readPos == _readLen && _readPos > 0 implies the read buffer is valid, but we're at the end of the buffer.
    ///   * _readPos == _readLen == 0 means the read buffer contains garbage.
    ///   * Either _writePos can be greater than 0, or _readLen & _readPos can be greater than zero,
    ///     but neither can be greater than zero at the same time.
    ///  ]]>
    /// This class will never cache more bytes than the max specified buffer size.
    /// However, it may use a temporary buffer of up to twice the size in order to combine several IO operations on
    /// the underlying stream into a single operation. This is because we assume that memory copies are significantly
    /// faster than IO operations on the underlying stream (if this was not true, using buffering is never appropriate).
    /// The max size of this "shadow" buffer is limited as to not allocate it on the LOH.
    /// Shadowing is always transient. Even when using this technique, this class still guarantees that the number of
    /// bytes cached (not yet written to the target stream or not yet consumed by the user) is never larger than the 
    /// actual specified buffer size.
    /// </summary>
    public sealed class BufferedStream : Stream
    {
        /// <summary><code>MaxShadowBufferSize</code> is chosen such that shadow buffers are not allocated on the Large Object Heap.
        /// Currently, an object is allocated on the LOH if it is larger than 85000 bytes. See LARGE_OBJECT_SIZE in src/gc/gc.h
        /// We will go with exactly 80 Kbytes, although this is somewhat arbitrary.</summary>
        private const int MaxShadowBufferSize = 81920;  // Make sure not to get to the Large Object Heap.
        private const int DefaultBufferSize = 4096;

        private Stream _stream;                             // Underlying stream.  Close sets _stream to null.
        private byte[] _buffer;                             // Shared read/write buffer.  Alloc on first use.
        private readonly int _bufferSize;                   // Length of internal buffer (not counting the shadow buffer).
        private int _readPos;                               // Read pointer within shared buffer.
        private int _readLen;                               // Number of bytes read in buffer from _stream.
        private int _writePos;                              // Write pointer within shared buffer.
        private Task<int> _lastSyncCompletedReadTask;       // The last successful Task returned from ReadAsync
                                                            // (perf optimization for successive reads of the same size)
                                                            // Removing a private default constructor is a breaking change for the DataDebugSerializer.
                                                            // Because this ctor was here previously we need to keep it around.
        private SemaphoreSlim _asyncActiveSemaphore;

        internal SemaphoreSlim LazyEnsureAsyncActiveSemaphoreInitialized()
        {
            // Lazily-initialize _asyncActiveSemaphore.  As we're never accessing the SemaphoreSlim's
            // WaitHandle, we don't need to worry about Disposing it.
            return LazyInitializer.EnsureInitialized(ref _asyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
        }

        private BufferedStream() { }

        public BufferedStream(Stream stream)
            : this(stream, DefaultBufferSize)
        {
        }

        public BufferedStream(Stream stream, int bufferSize)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", SR.Format(SR.ArgumentOutOfRange_MustBePositive, "bufferSize"));

            _stream = stream;
            _bufferSize = bufferSize;

            // Allocate _buffer on its first use - it will not be used if all reads
            // & writes are greater than or equal to buffer size.

            if (!_stream.CanRead && !_stream.CanWrite)
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
        }

        private void EnsureNotClosed()
        {
            if (_stream == null)
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
        }

        private void EnsureCanSeek()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanSeek)
                throw new NotSupportedException(SR.NotSupported_UnseekableStream);
        }

        private void EnsureCanRead()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanRead)
                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
        }

        private void EnsureCanWrite()
        {
            Debug.Assert(_stream != null);

            if (!_stream.CanWrite)
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
        }

        private void EnsureShadowBufferAllocated()
        {
            Debug.Assert(_buffer != null);
            Debug.Assert(_bufferSize > 0);

            // Already have a shadow buffer? 
            // Or is the user-specified buffer size already so large that we don't want to create one?
            if (_buffer.Length != _bufferSize || _bufferSize >= MaxShadowBufferSize)
                return;

            byte[] shadowBuffer = new byte[Math.Min(_bufferSize + _bufferSize, MaxShadowBufferSize)];
            Array.Copy(_buffer, 0, shadowBuffer, 0, _writePos);
            _buffer = shadowBuffer;
        }

        private void EnsureBufferAllocated()
        {
            Debug.Assert(_bufferSize > 0);

            // BufferedStream is not intended for multi-threaded use, so no worries about the get/set race on _buffer.
            if (_buffer == null)
                _buffer = new byte[_bufferSize];
        }

        public override bool CanRead
        {
            [Pure]
            get
            {
                return _stream != null && _stream.CanRead;
            }
        }

        public override bool CanWrite
        {
            [Pure]
            get
            {
                return _stream != null && _stream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            [Pure]
            get
            {
                return _stream != null && _stream.CanSeek;
            }
        }

        public override long Length
        {
            get
            {
                EnsureNotClosed();

                if (_writePos > 0)
                    FlushWrite();

                return _stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                EnsureNotClosed();
                EnsureCanSeek();

                Debug.Assert(!(_writePos > 0 && _readPos != _readLen), "Read and Write buffers cannot both have data in them at the same time.");
                return _stream.Position + (_readPos - _readLen + _writePos);
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", SR.ArgumentOutOfRange_NeedNonNegNum);

                EnsureNotClosed();
                EnsureCanSeek();

                if (_writePos > 0)
                    FlushWrite();

                _readPos = 0;
                _readLen = 0;
                _stream.Seek(value, SeekOrigin.Begin);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _stream != null)
                {
                    try
                    {
                        Flush();
                    }
                    finally
                    {
                        _stream.Dispose();
                    }
                }
            }
            finally
            {
                _stream = null;
                _buffer = null;

                // Call base.Dispose(bool) to cleanup async IO resources
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            EnsureNotClosed();

            // Has write data in the buffer:
            if (_writePos > 0)
            {
                FlushWrite();
                Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                return;
            }

            // Has read data in the buffer:
            if (_readPos < _readLen)
            {
                // If the underlying stream is not seekable AND we have something in the read buffer, then FlushRead would throw.
                // We can either throw away the buffer resulting in data loss (!) or ignore the Flush.
                // (We cannot throw becasue it would be a breaking change.) We opt into ignoring the Flush in that situation.
                if (!_stream.CanSeek)
                    return;

                FlushRead();

                // User streams may have opted to throw from Flush if CanWrite is false (although the abstract Stream does not do so).
                // However, if we do not forward the Flush to the underlying stream, we may have problems when chaining several streams.
                // Let us make a best effort attempt:
                if (_stream.CanWrite)
                    _stream.Flush();

                Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                return;
            }

            // We had no data in the buffer, but we still need to tell the underlying stream to flush.
            if (_stream.CanWrite)
                _stream.Flush();

            _writePos = _readPos = _readLen = 0;
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();

            return FlushAsyncInternal(cancellationToken);
        }

        private async Task FlushAsyncInternal(CancellationToken cancellationToken)
        {
            Debug.Assert(_stream != null);

            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            await sem.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_writePos > 0)
                {
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);
                    Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                    return;
                }

                if (_readPos < _readLen)
                {
                    // If the underlying stream is not seekable AND we have something in the read buffer, then FlushRead would throw.
                    // We can either throw away the buffer resulting in date loss (!) or ignore the Flush. (We cannot throw becasue it
                    // would be a breaking change.) We opt into ignoring the Flush in that situation.
                    if (!_stream.CanSeek)
                        return;

                    FlushRead();  // not async; it uses Seek, but there's no SeekAsync

                    // User streams may have opted to throw from Flush if CanWrite is false (although the abstract Stream does not do so).
                    // However, if we do not forward the Flush to the underlying stream, we may have problems when chaining several streams.
                    // Let us make a best effort attempt:
                    if (_stream.CanRead)
                        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                    Debug.Assert(_writePos == 0 && _readPos == 0 && _readLen == 0);
                    return;
                }

                // We had no data in the buffer, but we still need to tell the underlying stream to flush.
                if (_stream.CanWrite)
                    await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                // There was nothing in the buffer:
                Debug.Assert(_writePos == 0 && _readPos == _readLen);

            }
            finally
            {
                sem.Release();
            }
        }

        // Reading is done in blocks, but someone could read 1 byte from the buffer then write. 
        // At that point, the underlying stream's pointer is out of sync with this stream's position. 
        // All write functions should call this function to ensure that the buffered data is not lost.
        private void FlushRead()
        {
            Debug.Assert(_writePos == 0, "BufferedStream: Write buffer must be empty in FlushRead!");

            if (_readPos - _readLen != 0)
                _stream.Seek(_readPos - _readLen, SeekOrigin.Current);

            _readPos = 0;
            _readLen = 0;
        }

        /// <summary>
        /// Called by Write methods to clear the Read Buffer
        /// </summary>
        private void ClearReadBufferBeforeWrite()
        {
            Debug.Assert(_readPos <= _readLen, "_readPos <= _readLen [" + _readPos + " <= " + _readLen + "]");

            // No read data in the buffer:
            if (_readPos == _readLen)
            {
                _readPos = _readLen = 0;
                return;
            }

            // Must have read data.
            Debug.Assert(_readPos < _readLen);

            // If the underlying stream cannot seek, FlushRead would end up throwing NotSupported.
            // However, since the user did not call a method that is intuitively expected to seek, a better message is in order.
            // Ideally, we would throw an InvalidOperation here, but for backward compat we have to stick with NotSupported.
            if (!_stream.CanSeek)
                throw new NotSupportedException(SR.NotSupported_CannotWriteToBufferedStreamIfReadBufferCannotBeFlushed);

            FlushRead();
        }

        private void FlushWrite()
        {
            Debug.Assert(_readPos == 0 && _readLen == 0,
                            "BufferedStream: Read buffer must be empty in FlushWrite!");
            Debug.Assert(_buffer != null && _bufferSize >= _writePos,
                            "BufferedStream: Write buffer must be allocated and write position must be in the bounds of the buffer in FlushWrite!");

            _stream.Write(_buffer, 0, _writePos);
            _writePos = 0;
            _stream.Flush();
        }

        private async Task FlushWriteAsync(CancellationToken cancellationToken)
        {

            Debug.Assert(_readPos == 0 && _readLen == 0,
                            "BufferedStream: Read buffer must be empty in FlushWrite!");
            Debug.Assert(_buffer != null && _bufferSize >= _writePos,
                            "BufferedStream: Write buffer must be allocated and write position must be in the bounds of the buffer in FlushWrite!");

            await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
            _writePos = 0;
            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        private int ReadFromBuffer(byte[] array, int offset, int count)
        {
            int readbytes = _readLen - _readPos;
            Debug.Assert(readbytes >= 0);

            if (readbytes == 0)
                return 0;

            Debug.Assert(readbytes > 0);

            if (readbytes > count)
                readbytes = count;
            Array.Copy(_buffer, _readPos, array, offset, readbytes);
            _readPos += readbytes;

            return readbytes;
        }

        private int ReadFromBuffer(Byte[] array, int offset, int count, out Exception error)
        {
            try
            {
                error = null;
                return ReadFromBuffer(array, offset, count);
            }
            catch (Exception ex)
            {
                error = ex;
                return 0;
            }
        }

        public override int Read([In, Out] byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array", SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream Debug.

            // Reading again for more data may cause us to block if we're using a device with no clear end of file,
            // such as a serial port or pipe. If we blocked here and this code was used with redirected pipes for a
            // process's standard output, this can lead to deadlocks involving two processes.              
            // BUT - this is a breaking change. 
            // So: If we could not read all bytes the user asked for from the buffer, we will try once from the underlying
            // stream thus ensuring the same blocking behaviour as if the underlying stream was not wrapped in this BufferedStream.
            if (bytesFromBuffer == count)
                return bytesFromBuffer;

            int alreadySatisfied = bytesFromBuffer;
            if (bytesFromBuffer > 0)
            {
                count -= bytesFromBuffer;
                offset += bytesFromBuffer;
            }

            // So the read buffer is empty.
            Debug.Assert(_readLen == _readPos);
            _readPos = _readLen = 0;

            // If there was anything in the write buffer, clear it.
            if (_writePos > 0)
                FlushWrite();

            // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
            if (count >= _bufferSize)
            {
                return _stream.Read(array, offset, count) + alreadySatisfied;
            }

            // Ok. We can fill the buffer:
            EnsureBufferAllocated();
            _readLen = _stream.Read(_buffer, 0, _bufferSize);

            bytesFromBuffer = ReadFromBuffer(array, offset, count);

            // We may have read less than the number of bytes the user asked for, but that is part of the Stream Debug.
            // Reading again for more data may cause us to block if we're using a device with no clear end of stream,
            // such as a serial port or pipe.  If we blocked here & this code was used with redirected pipes for a process's
            // standard output, this can lead to deadlocks involving two processes. Additionally, translating one read on the
            // BufferedStream to more than one read on the underlying Stream may defeat the whole purpose of buffering of the
            // underlying reads are significantly more expensive.

            return bytesFromBuffer + alreadySatisfied;
        }

        private Task<int> LastSyncCompletedReadTask(int val)
        {

            Task<int> t = _lastSyncCompletedReadTask;
            Debug.Assert(t == null || t.Status == TaskStatus.RanToCompletion);

            if (t != null && t.Result == val)
                return t;

            t = Task.FromResult<int>(val);
            _lastSyncCompletedReadTask = t;
            return t;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {

            if (buffer == null)
                throw new ArgumentNullException("buffer", SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();
            EnsureCanRead();

            int bytesFromBuffer = 0;
            // Try to satisfy the request from the buffer synchronously. But still need a sem-lock in case that another
            // Async IO Task accesses the buffer concurrently. If we fail to acquire the lock without waiting, make this 
            // an Async operation.
            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            Task semaphoreLockTask = sem.WaitAsync();
            if (semaphoreLockTask.Status == TaskStatus.RanToCompletion)
            {

                bool completeSynchronously = true;
                try
                {
                    Exception error;
                    bytesFromBuffer = ReadFromBuffer(buffer, offset, count, out error);

                    // If we satistied enough data from the buffer, we can complete synchronously.
                    // Reading again for more data may cause us to block if we're using a device with no clear end of file,
                    // such as a serial port or pipe. If we blocked here and this code was used with redirected pipes for a
                    // process's standard output, this can lead to deadlocks involving two processes.              
                    // BUT - this is a breaking change. 
                    // So: If we could not read all bytes the user asked for from the buffer, we will try once from the underlying
                    // stream thus ensuring the same blocking behaviour as if the underlying stream was not wrapped in this BufferedStream.
                    completeSynchronously = (bytesFromBuffer == count || error != null);

                    if (completeSynchronously)
                    {

                        return (error == null)
                                    ? LastSyncCompletedReadTask(bytesFromBuffer)
                                    : Task.FromException<int>(error);
                    }
                }
                finally
                {
                    if (completeSynchronously)  // if this is FALSE, we will be entering ReadFromUnderlyingStreamAsync and releasing there.
                        sem.Release();
                }
            }

            // Delegate to the async implementation.
            return ReadFromUnderlyingStreamAsync(buffer, offset + bytesFromBuffer, count - bytesFromBuffer, cancellationToken,
                                                 bytesFromBuffer, semaphoreLockTask);
        }

        /// <summary>BufferedStream should be as thin a wrapper as possible. We want ReadAsync to delegate to
        /// ReadAsync of the underlying _stream rather than calling the base Stream which implements the one in terms of the other.
        /// This allows BufferedStream to affect the semantics of the stream it wraps as little as possible. </summary>
        /// <returns>-2 if _bufferSize was set to 0 while waiting on the semaphore; otherwise num of bytes read.</returns>
        private async Task<int> ReadFromUnderlyingStreamAsync(byte[] array, int offset, int count,
                                                                CancellationToken cancellationToken,
                                                                int bytesAlreadySatisfied,
                                                                Task semaphoreLockTask)
        {

            // Same conditions validated with exceptions in ReadAsync:
            // (These should be Debug.Requires(..) but that method had some issues in async methods; using Assert(..) for now.)
            Debug.Assert(array != null);
            Debug.Assert(offset >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(array.Length - offset >= count);
            Debug.Assert(_stream != null);
            Debug.Assert(_stream.CanRead);
            Debug.Assert(_bufferSize > 0);
            Debug.Assert(semaphoreLockTask != null);

            // Employ async waiting based on the same synchronization used in BeginRead of the abstract Stream.        
            await semaphoreLockTask.ConfigureAwait(false);
            try
            {

                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // Check it now again.            
                int bytesFromBuffer = ReadFromBuffer(array, offset, count);
                if (bytesFromBuffer == count)
                    return bytesAlreadySatisfied + bytesFromBuffer;

                if (bytesFromBuffer > 0)
                {
                    count -= bytesFromBuffer;
                    offset += bytesFromBuffer;
                    bytesAlreadySatisfied += bytesFromBuffer;
                }

                Debug.Assert(_readLen == _readPos);
                _readPos = _readLen = 0;

                // If there was anything in the write buffer, clear it.
                if (_writePos > 0)
                    await FlushWriteAsync(cancellationToken).ConfigureAwait(false);  // no Begin-End read version for Flush. Use Async.            

                // If the requested read is larger than buffer size, avoid the buffer and still use a single read:
                if (count >= _bufferSize)
                {
                    return bytesAlreadySatisfied + await _stream.ReadAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
                }

                // Ok. We can fill the buffer:
                EnsureBufferAllocated();
                _readLen = await _stream.ReadAsync(_buffer, 0, _bufferSize, cancellationToken).ConfigureAwait(false);

                bytesFromBuffer = ReadFromBuffer(array, offset, count);
                return bytesAlreadySatisfied + bytesFromBuffer;

            }
            finally
            {
                SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
                sem.Release();
            }
        }

        public override int ReadByte()
        {
            return _readPos != _readLen ?
                _buffer[_readPos++] :
                ReadByteSlow();
        }

        private int ReadByteSlow()
        {
            Debug.Assert(_readPos == _readLen);

            // We want to check for whether the underlying stream has been closed and whether
            // it's readable, but we only need to do so if we don't have data in our buffer,
            // as any data we have came from reading it from an open stream, and we don't
            // care if the stream has been closed or become unreadable since. Further, if
            // the stream is closed, its read buffer is flushed, so we'll take this slow path.
            EnsureNotClosed();
            EnsureCanRead();

            if (_writePos > 0)
                FlushWrite();

            EnsureBufferAllocated();
            _readLen = _stream.Read(_buffer, 0, _bufferSize);
            if (_readLen == 0)
                return -1;

            _readPos = 0;
            return _buffer[_readPos++];
        }

        private void WriteToBuffer(byte[] array, ref int offset, ref int count)
        {
            int bytesToWrite = Math.Min(_bufferSize - _writePos, count);

            if (bytesToWrite <= 0)
                return;

            EnsureBufferAllocated();
            Array.Copy(array, offset, _buffer, _writePos, bytesToWrite);

            _writePos += bytesToWrite;
            count -= bytesToWrite;
            offset += bytesToWrite;
        }

        private void WriteToBuffer(byte[] array, ref int offset, ref int count, out Exception error)
        {
            try
            {
                error = null;
                WriteToBuffer(array, ref offset, ref count);
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }


        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array", SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            EnsureNotClosed();
            EnsureCanWrite();

            if (_writePos == 0)
                ClearReadBufferBeforeWrite();

            #region Write algorithm comment
            // We need to use the buffer, while avoiding unnecessary buffer usage / memory copies.
            // We ASSUME that memory copies are much cheaper than writes to the underlying stream, so if an extra copy is
            // guaranteed to reduce the number of writes, we prefer it.
            // We pick a simple strategy that makes degenerate cases rare if our assumptions are right.
            //
            // For ever write, we use a simple heuristic (below) to decide whether to use the buffer.
            // The heuristic has the desirable property (*) that if the specified user data can fit into the currently available
            // buffer space without filling it up completely, the heuristic will always tell us to use the buffer. It will also
            // tell us to use the buffer in cases where the current write would fill the buffer, but the remaining data is small
            // enough such that subsequent operations can use the buffer again.
            // 
            // Algorithm:
            // Determine whether or not to buffer according to the heuristic (below).
            // If we decided to use the buffer:
            //     Copy as much user data as we can into the buffer.
            //     If we consumed all data: We are finished.
            //     Otherwise, write the buffer out.
            //     Copy the rest of user data into the now cleared buffer (no need to write out the buffer again as the heuristic
            //     will prevent it from being filled twice).
            // If we decided not to use the buffer:
            //     Can the data already in the buffer and current user data be combines to a single write
            //     by allocating a "shadow" buffer of up to twice the size of _bufferSize (up to a limit to avoid LOH)?
            //     Yes, it can:
            //         Allocate a larger "shadow" buffer and ensure the buffered  data is moved there.
            //         Copy user data to the shadow buffer.
            //         Write shadow buffer to the underlying stream in a single operation.
            //     No, it cannot (amount of data is still too large):
            //         Write out any data possibly in the buffer.
            //         Write out user data directly.
            //
            // Heuristic:
            // If the subsequent write operation that follows the current write operation will result in a write to the
            // underlying stream in case that we use the buffer in the current write, while it would not have if we avoided
            // using the buffer in the current write (by writing current user data to the underlying stream directly), then we
            // prefer to avoid using the buffer since the corresponding memory copy is wasted (it will not reduce the number
            // of writes to the underlying stream, which is what we are optimising for).
            // ASSUME that the next write will be for the same amount of bytes as the current write (most common case) and
            // determine if it will cause a write to the underlying stream. If the next write is actually larger, our heuristic
            // still yields the right behaviour, if the next write is actually smaller, we may making an unnecessary write to
            // the underlying stream. However, this can only occur if the current write is larger than half the buffer size and
            // we will recover after one iteration.
            // We have:
            //     useBuffer = (_writePos + count + count < _bufferSize + _bufferSize)
            //
            // Example with _bufferSize = 20, _writePos = 6, count = 10:
            //
            //     +---------------------------------------+---------------------------------------+
            //     |             current buffer            | next iteration's "future" buffer      |
            //     +---------------------------------------+---------------------------------------+ 
            //     |0| | | | | | | | | |1| | | | | | | | | |2| | | | | | | | | |3| | | | | | | | | |
            //     |0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8|9|
            //     +-----------+-------------------+-------------------+---------------------------+
            //     | _writePos |  current count    | assumed next count|avail buff after next write|
            //     +-----------+-------------------+-------------------+---------------------------+
            //
            // A nice property (*) of this heuristic is that it will always succeed if the user data completely fits into the
            // available buffer, i.e. if count < (_bufferSize - _writePos).
            #endregion Write algorithm comment

            Debug.Assert(_writePos < _bufferSize);

            int totalUserbytes;
            bool useBuffer;
            checked
            {  // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                totalUserbytes = _writePos + count;
                useBuffer = (totalUserbytes + count < (_bufferSize + _bufferSize));
            }

            if (useBuffer)
            {
                WriteToBuffer(array, ref offset, ref count);

                if (_writePos < _bufferSize)
                {
                    Debug.Assert(count == 0);
                    return;
                }

                Debug.Assert(count >= 0);
                Debug.Assert(_writePos == _bufferSize);
                Debug.Assert(_buffer != null);

                _stream.Write(_buffer, 0, _writePos);
                _writePos = 0;

                WriteToBuffer(array, ref offset, ref count);

                Debug.Assert(count == 0);
                Debug.Assert(_writePos < _bufferSize);
            }
            else
            {  // if (!useBuffer)
               // Write out the buffer if necessary.
                if (_writePos > 0)
                {
                    Debug.Assert(_buffer != null);
                    Debug.Assert(totalUserbytes >= _bufferSize);

                    // Try avoiding extra write to underlying stream by combining previously buffered data with current user data:
                    if (totalUserbytes <= (_bufferSize + _bufferSize) && totalUserbytes <= MaxShadowBufferSize)
                    {
                        EnsureShadowBufferAllocated();
                        Array.Copy(array, offset, _buffer, _writePos, count);
                        _stream.Write(_buffer, 0, totalUserbytes);
                        _writePos = 0;
                        return;
                    }

                    _stream.Write(_buffer, 0, _writePos);
                    _writePos = 0;
                }

                // Write out user data.
                _stream.Write(array, offset, count);
            }
        }


        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {

            if (buffer == null)
                throw new ArgumentNullException("buffer", SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            // Fast path check for cancellation already requested
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<int>(cancellationToken);

            EnsureNotClosed();
            EnsureCanWrite();

            // Try to satisfy the request from the buffer synchronously. But still need a sem-lock in case that another
            // Async IO Task accesses the buffer concurrently. If we fail to acquire the lock without waiting, make this 
            // an Async operation.
            SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
            Task semaphoreLockTask = sem.WaitAsync();
            if (semaphoreLockTask.Status == TaskStatus.RanToCompletion)
            {
                bool completeSynchronously = true;
                try
                {

                    if (_writePos == 0)
                        ClearReadBufferBeforeWrite();

                    Debug.Assert(_writePos < _bufferSize);

                    // If the write completely fits into the buffer, we can complete synchronously:
                    completeSynchronously = (count < _bufferSize - _writePos);

                    if (completeSynchronously)
                    {

                        Exception error;
                        WriteToBuffer(buffer, ref offset, ref count, out error);
                        Debug.Assert(count == 0);

                        return (error == null)
                                    ? Task.CompletedTask
                                    : Task.FromException(error);
                    }
                }
                finally
                {
                    if (completeSynchronously)  // if this is FALSE, we will be entering WriteToUnderlyingStreamAsync and releasing there.
                        sem.Release();
                }
            }

            // Delegate to the async implementation.
            return WriteToUnderlyingStreamAsync(buffer, offset, count, cancellationToken, semaphoreLockTask);
        }


        /// <summary>BufferedStream should be as thin a wrapper as possible. We want WriteAsync to delegate to
        /// WriteAsync of the underlying _stream rather than calling the base Stream which implements the one 
        /// in terms of the other. This allows BufferedStream to affect the semantics of the stream it wraps as 
        /// little as possible.
        /// </summary>
        private async Task WriteToUnderlyingStreamAsync(byte[] array, int offset, int count,
                                                        CancellationToken cancellationToken,
                                                        Task semaphoreLockTask)
        {
            Debug.Assert(array != null);
            Debug.Assert(offset >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(array.Length - offset >= count);
            Debug.Assert(_stream != null);
            Debug.Assert(_stream.CanWrite);
            Debug.Assert(_bufferSize > 0);
            Debug.Assert(semaphoreLockTask != null);

            // See the LARGE COMMENT in Write(..) for the explanation of the write buffer algorithm.

            await semaphoreLockTask.ConfigureAwait(false);
            try
            {

                // The buffer might have been changed by another async task while we were waiting on the semaphore.
                // However, note that if we recalculate the sync completion condition to TRUE, then useBuffer will also be TRUE.

                if (_writePos == 0)
                    ClearReadBufferBeforeWrite();

                int totalUserBytes;
                bool useBuffer;
                checked
                {  // We do not expect buffer sizes big enough for an overflow, but if it happens, lets fail early:
                    totalUserBytes = _writePos + count;
                    useBuffer = (totalUserBytes + count < (_bufferSize + _bufferSize));
                }

                if (useBuffer)
                {
                    WriteToBuffer(array, ref offset, ref count);

                    if (_writePos < _bufferSize)
                    {
                        Debug.Assert(count == 0);
                        return;
                    }

                    Debug.Assert(count >= 0);
                    Debug.Assert(_writePos == _bufferSize);
                    Debug.Assert(_buffer != null);

                   
                    await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
                    _writePos = 0;

                    WriteToBuffer(array, ref offset, ref count);

                    Debug.Assert(count == 0);
                    Debug.Assert(_writePos < _bufferSize);

                }
                else
                {  // if (!useBuffer)
                    // Write out the buffer if necessary.
                    if (_writePos > 0)
                    {
                        Debug.Assert(_buffer != null);
                        Debug.Assert(totalUserBytes >= _bufferSize);

                        // Try avoiding extra write to underlying stream by combining previously buffered data with current user data:
                        if (totalUserBytes <= (_bufferSize + _bufferSize) && totalUserBytes <= MaxShadowBufferSize)
                        {
                            EnsureShadowBufferAllocated();
                            Array.Copy(array, offset, _buffer, _writePos, count);

                            await _stream.WriteAsync(_buffer, 0, totalUserBytes, cancellationToken).ConfigureAwait(false);
                            _writePos = 0;
                            return;
                        }

                        await _stream.WriteAsync(_buffer, 0, _writePos, cancellationToken).ConfigureAwait(false);
                        _writePos = 0;
                    }

                    // Write out user data.
                    await _stream.WriteAsync(array, offset, count, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                SemaphoreSlim sem = LazyEnsureAsyncActiveSemaphoreInitialized();
                sem.Release();
            }
        }

        public override void WriteByte(byte value)
        {
            EnsureNotClosed();

            if (_writePos == 0)
            {
                EnsureCanWrite();
                ClearReadBufferBeforeWrite();
                EnsureBufferAllocated();
            }

            // We should not be flushing here, but only writing to the underlying stream, but previous version flushed, so we keep this.
            if (_writePos >= _bufferSize - 1)
                FlushWrite();

            _buffer[_writePos++] = value;

            Debug.Assert(_writePos < _bufferSize);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureNotClosed();
            EnsureCanSeek();

            // If we have bytes in the write buffer, flush them out, seek and be done.
            if (_writePos > 0)
            {
                // We should be only writing the buffer and not flushing,
                // but the previous version did flush and we stick to it for back-compat reasons.
                FlushWrite();
                return _stream.Seek(offset, origin);
            }

            // The buffer is either empty or we have a buffered read.

            if (_readLen - _readPos > 0 && origin == SeekOrigin.Current)
            {
                // If we have bytes in the read buffer, adjust the seek offset to account for the resulting difference
                // between this stream's position and the underlying stream's position.            
                offset -= (_readLen - _readPos);
            }

            long oldPos = Position;
            Debug.Assert(oldPos == _stream.Position + (_readPos - _readLen));

            long newPos = _stream.Seek(offset, origin);

            // If the seek destination is still within the data currently in the buffer, we want to keep the buffer data and continue using it.
            // Otherwise we will throw away the buffer. This can only happen on read, as we flushed write data above.

            // The offset of the new/updated seek pointer within _buffer:
            _readPos = (int)(newPos - (oldPos - _readPos));

            // If the offset of the updated seek pointer in the buffer is still legal, then we can keep using the buffer:
            if (0 <= _readPos && _readPos < _readLen)
            {
                // Adjust the seek pointer of the underlying stream to reflect the amount of useful bytes in the read buffer:
                _stream.Seek(_readLen - _readPos, SeekOrigin.Current);
            }
            else
            {  // The offset of the updated seek pointer is not a legal offset. Loose the buffer.
                _readPos = _readLen = 0;
            }

            Debug.Assert(newPos == Position, "newPos (=" + newPos + ") == Position (=" + Position + ")");
            return newPos;
        }

        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", SR.ArgumentOutOfRange_NeedNonNegNum);

            EnsureNotClosed();
            EnsureCanSeek();
            EnsureCanWrite();

            Flush();
            _stream.SetLength(value);
        }
    }  // class BufferedStream
}  // namespace
