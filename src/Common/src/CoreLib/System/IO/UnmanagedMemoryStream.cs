// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    /*
     * This class is used to access a contiguous block of memory, likely outside 
     * the GC heap (or pinned in place in the GC heap, but a MemoryStream may 
     * make more sense in those cases).  It's great if you have a pointer and
     * a length for a section of memory mapped in by someone else and you don't
     * want to copy this into the GC heap.  UnmanagedMemoryStream assumes these 
     * two things:
     *
     * 1) All the memory in the specified block is readable or writable,
     *    depending on the values you pass to the constructor.
     * 2) The lifetime of the block of memory is at least as long as the lifetime
     *    of the UnmanagedMemoryStream.
     * 3) You clean up the memory when appropriate.  The UnmanagedMemoryStream 
     *    currently will do NOTHING to free this memory.
     * 4) All calls to Write and WriteByte may not be threadsafe currently.
     *
     * It may become necessary to add in some sort of 
     * DeallocationMode enum, specifying whether we unmap a section of memory, 
     * call free, run a user-provided delegate to free the memory, etc.  
     * We'll suggest user write a subclass of UnmanagedMemoryStream that uses
     * a SafeHandle subclass to hold onto the memory.
     * 
     */

    /// <summary>
    /// Stream over a memory pointer or over a SafeBuffer
    /// </summary>
    public class UnmanagedMemoryStream : Stream
    {
        private SafeBuffer _buffer;
        private unsafe byte* _mem;
        private long _length;
        private long _capacity;
        private long _position;
        private long _offset;
        private FileAccess _access;
        private bool _isOpen;
        private Task<Int32> _lastReadTask; // The last successful task returned from ReadAsync 

        /// <summary>
        /// Creates a closed stream.
        /// </summary>
        // Needed for subclasses that need to map a file, etc.
        protected UnmanagedMemoryStream()
        {
            unsafe
            {
                _mem = null;
            }
            _isOpen = false;
        }

        /// <summary>
        /// Creates a stream over a SafeBuffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public UnmanagedMemoryStream(SafeBuffer buffer, long offset, long length)
        {
            Initialize(buffer, offset, length, FileAccess.Read);
        }

        /// <summary>
        /// Creates a stream over a SafeBuffer.
        /// </summary>
        public UnmanagedMemoryStream(SafeBuffer buffer, long offset, long length, FileAccess access)
        {
            Initialize(buffer, offset, length, access);
        }

        /// <summary>
        /// Subclasses must call this method (or the other overload) to properly initialize all instance fields.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="access"></param>
        protected void Initialize(SafeBuffer buffer, long offset, long length, FileAccess access)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.ByteLength < (ulong)(offset + length))
            {
                throw new ArgumentException(SR.Argument_InvalidSafeBufferOffLen);
            }
            if (access < FileAccess.Read || access > FileAccess.ReadWrite)
            {
                throw new ArgumentOutOfRangeException(nameof(access));
            }

            if (_isOpen)
            {
                throw new InvalidOperationException(SR.InvalidOperation_CalledTwice);
            }

            // check for wraparound
            unsafe
            {
                byte* pointer = null;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    buffer.AcquirePointer(ref pointer);
                    if ((pointer + offset + length) < pointer)
                    {
                        throw new ArgumentException(SR.ArgumentOutOfRange_UnmanagedMemStreamWrapAround);
                    }
                }
                finally
                {
                    if (pointer != null)
                    {
                        buffer.ReleasePointer();
                    }
                }
            }

            _offset = offset;
            _buffer = buffer;
            _length = length;
            _capacity = length;
            _access = access;
            _isOpen = true;
        }

        /// <summary>
        /// Creates a stream over a byte*.
        /// </summary>
        [CLSCompliant(false)]
        public unsafe UnmanagedMemoryStream(byte* pointer, long length)
        {
            Initialize(pointer, length, length, FileAccess.Read);
        }

        /// <summary>
        /// Creates a stream over a byte*.
        /// </summary>
        [CLSCompliant(false)]
        public unsafe UnmanagedMemoryStream(byte* pointer, long length, long capacity, FileAccess access)
        {
            Initialize(pointer, length, capacity, access);
        }

        /// <summary>
        /// Subclasses must call this method (or the other overload) to properly initialize all instance fields.
        /// </summary>
        [CLSCompliant(false)]
        protected unsafe void Initialize(byte* pointer, long length, long capacity, FileAccess access)
        {
            if (pointer == null)
                throw new ArgumentNullException(nameof(pointer));
            if (length < 0 || capacity < 0)
                throw new ArgumentOutOfRangeException((length < 0) ? nameof(length) : nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (length > capacity)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_LengthGreaterThanCapacity);
            // Check for wraparound.
            if (((byte*)((long)pointer + capacity)) < pointer)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_UnmanagedMemStreamWrapAround);
            if (access < FileAccess.Read || access > FileAccess.ReadWrite)
                throw new ArgumentOutOfRangeException(nameof(access), SR.ArgumentOutOfRange_Enum);
            if (_isOpen)
                throw new InvalidOperationException(SR.InvalidOperation_CalledTwice);

            _mem = pointer;
            _offset = 0;
            _length = length;
            _capacity = capacity;
            _access = access;
            _isOpen = true;
        }

        /// <summary>
        /// Returns true if the stream can be read; otherwise returns false.
        /// </summary>
        public override bool CanRead
        {
            get { return _isOpen && (_access & FileAccess.Read) != 0; }
        }

        /// <summary>
        /// Returns true if the stream can seek; otherwise returns false.
        /// </summary>
        public override bool CanSeek
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Returns true if the stream can be written to; otherwise returns false.
        /// </summary>
        public override bool CanWrite
        {
            get { return _isOpen && (_access & FileAccess.Write) != 0; }
        }

        /// <summary>
        /// Closes the stream. The stream's memory needs to be dealt with separately.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _isOpen = false;
            unsafe { _mem = null; }

            // Stream allocates WaitHandles for async calls. So for correctness 
            // call base.Dispose(disposing) for better perf, avoiding waiting
            // for the finalizers to run on those types.
            base.Dispose(disposing);
        }

        private void EnsureNotClosed()
        {
            if (!_isOpen)
                throw Error.GetStreamIsClosed();
        }

        private void EnsureReadable()
        {
            if (!CanRead)
                throw Error.GetReadNotSupported();
        }

        private void EnsureWriteable()
        {
            if (!CanWrite)
                throw Error.GetWriteNotSupported();
        }

        /// <summary>
        /// Since it's a memory stream, this method does nothing.
        /// </summary>
        public override void Flush()
        {
            EnsureNotClosed();
        }

        /// <summary>
        /// Since it's a memory stream, this method does nothing specific.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            try
            {
                Flush();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        /// <summary>
        /// Number of bytes in the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                EnsureNotClosed();
                return Interlocked.Read(ref _length);
            }
        }

        /// <summary>
        /// Number of bytes that can be written to the stream.
        /// </summary>
        public long Capacity
        {
            get
            {
                EnsureNotClosed();
                return _capacity;
            }
        }

        /// <summary>
        /// ReadByte will read byte at the Position in the stream
        /// </summary>
        public override long Position
        {
            get
            {
                if (!CanSeek) throw Error.GetStreamIsClosed();
                return Interlocked.Read(ref _position);
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);
                if (!CanSeek) throw Error.GetStreamIsClosed();

                Interlocked.Exchange(ref _position, value);
            }
        }

        /// <summary>
        /// Pointer to memory at the current Position in the stream. 
        /// </summary>
        [CLSCompliant(false)]
        public unsafe byte* PositionPointer
        {
            get
            {
                if (_buffer != null)
                    throw new NotSupportedException(SR.NotSupported_UmsSafeBuffer);

                EnsureNotClosed();

                // Use a temp to avoid a race
                long pos = Interlocked.Read(ref _position);
                if (pos > _capacity)
                    throw new IndexOutOfRangeException(SR.IndexOutOfRange_UMSPosition);
                byte* ptr = _mem + pos;
                return ptr;
            }
            set
            {
                if (_buffer != null)
                    throw new NotSupportedException(SR.NotSupported_UmsSafeBuffer);

                EnsureNotClosed();

                if (value < _mem)
                    throw new IOException(SR.IO_SeekBeforeBegin);
                long newPosition = (long)value - (long)_mem;
                if (newPosition < 0)
                    throw new ArgumentOutOfRangeException("offset", SR.ArgumentOutOfRange_UnmanagedMemStreamLength);

                Interlocked.Exchange(ref _position, newPosition);
            }
        }

        /// <summary>
        /// Reads bytes from stream and puts them into the buffer
        /// </summary>
        /// <param name="buffer">Buffer to read the bytes to.</param>
        /// <param name="offset">Starting index in the buffer.</param>
        /// <param name="count">Maximum number of bytes to read.</param>
        /// <returns>Number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            return ReadCore(new Span<byte>(buffer, offset, count));
        }

        public override int Read(Span<byte> destination)
        {
            if (GetType() == typeof(UnmanagedMemoryStream))
            {
                return ReadCore(destination);
            }
            else
            {
                // UnmanagedMemoryStream is not sealed, and a derived type may have overridden Read(byte[], int, int) prior
                // to this Read(Span<byte>) overload being introduced.  In that case, this Read(Span<byte>) overload
                // should use the behavior of Read(byte[],int,int) overload.
                return base.Read(destination);
            }
        }

        internal int ReadCore(Span<byte> destination)
        {
            EnsureNotClosed();
            EnsureReadable();

            // Use a local variable to avoid a race where another thread 
            // changes our position after we decide we can read some bytes.
            long pos = Interlocked.Read(ref _position);
            long len = Interlocked.Read(ref _length);
            long n = Math.Min(len - pos, destination.Length);
            if (n <= 0)
            {
                return 0;
            }

            int nInt = (int)n; // Safe because n <= count, which is an Int32
            if (nInt < 0)
            {
                return 0;  // _position could be beyond EOF
            }
            Debug.Assert(pos + nInt >= 0, "_position + n >= 0");  // len is less than 2^63 -1.

            unsafe
            {
                fixed (byte* pBuffer = &MemoryMarshal.GetReference(destination))
                {
                    if (_buffer != null)
                    {
                        byte* pointer = null;

                        RuntimeHelpers.PrepareConstrainedRegions();
                        try
                        {
                            _buffer.AcquirePointer(ref pointer);
                            Buffer.Memcpy(pBuffer, pointer + pos + _offset, nInt);
                        }
                        finally
                        {
                            if (pointer != null)
                            {
                                _buffer.ReleasePointer();
                            }
                        }
                    }
                    else
                    {
                        Buffer.Memcpy(pBuffer, _mem + pos, nInt);
                    }
                }
            }
            Interlocked.Exchange(ref _position, pos + n);
            return nInt;
        }

        /// <summary>
        /// Reads bytes from stream and puts them into the buffer
        /// </summary>
        /// <param name="buffer">Buffer to read the bytes to.</param>
        /// <param name="offset">Starting index in the buffer.</param>
        /// <param name="count">Maximum number of bytes to read.</param>       
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        /// <returns>Task that can be used to access the number of bytes actually read.</returns>
        public override Task<Int32> ReadAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<Int32>(cancellationToken);

            try
            {
                Int32 n = Read(buffer, offset, count);
                Task<Int32> t = _lastReadTask;
                return (t != null && t.Result == n) ? t : (_lastReadTask = Task.FromResult<Int32>(n));
            }
            catch (Exception ex)
            {
                Debug.Assert(!(ex is OperationCanceledException));
                return Task.FromException<Int32>(ex);
            }
        }

        /// <summary>
        /// Reads bytes from stream and puts them into the buffer
        /// </summary>
        /// <param name="destination">Buffer to read the bytes to.</param>
        /// <param name="cancellationToken">Token that can be used to cancel this operation.</param>
        public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
            }

            try
            {
                // ReadAsync(Memory<byte>,...) needs to delegate to an existing virtual to do the work, in case an existing derived type
                // has changed or augmented the logic associated with reads.  If the Memory wraps an array, we could delegate to
                // ReadAsync(byte[], ...), but that would defeat part of the purpose, as ReadAsync(byte[], ...) often needs to allocate
                // a Task<int> for the return value, so we want to delegate to one of the synchronous methods.  We could always
                // delegate to the Read(Span<byte>) method, and that's the most efficient solution when dealing with a concrete
                // UnmanagedMemoryStream, but if we're dealing with a type derived from UnmanagedMemoryStream, Read(Span<byte>) will end up delegating
                // to Read(byte[], ...), which requires it to get a byte[] from ArrayPool and copy the data.  So, we special-case the
                // very common case of the Memory<byte> wrapping an array: if it does, we delegate to Read(byte[], ...) with it,
                // as that will be efficient in both cases, and we fall back to Read(Span<byte>) if the Memory<byte> wrapped something
                // else; if this is a concrete UnmanagedMemoryStream, that'll be efficient, and only in the case where the Memory<byte> wrapped
                // something other than an array and this is an UnmanagedMemoryStream-derived type that doesn't override Read(Span<byte>) will
                // it then fall back to doing the ArrayPool/copy behavior.
                return new ValueTask<int>(
                    MemoryMarshal.TryGetArray(destination, out ArraySegment<byte> destinationArray) ?
                        Read(destinationArray.Array, destinationArray.Offset, destinationArray.Count) :
                        Read(destination.Span));
            }
            catch (Exception ex)
            {
                return new ValueTask<int>(Task.FromException<int>(ex));
            }
        }

        /// <summary>
        /// Returns the byte at the stream current Position and advances the Position.
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            EnsureNotClosed();
            EnsureReadable();

            long pos = Interlocked.Read(ref _position);  // Use a local to avoid a race condition
            long len = Interlocked.Read(ref _length);
            if (pos >= len)
                return -1;
            Interlocked.Exchange(ref _position, pos + 1);
            int result;
            if (_buffer != null)
            {
                unsafe
                {
                    byte* pointer = null;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        _buffer.AcquirePointer(ref pointer);
                        result = *(pointer + pos + _offset);
                    }
                    finally
                    {
                        if (pointer != null)
                        {
                            _buffer.ReleasePointer();
                        }
                    }
                }
            }
            else
            {
                unsafe
                {
                    result = _mem[pos];
                }
            }
            return result;
        }

        /// <summary>
        /// Advanced the Position to specific location in the stream.
        /// </summary>
        /// <param name="offset">Offset from the loc parameter.</param>
        /// <param name="loc">Origin for the offset parameter.</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin loc)
        {
            EnsureNotClosed();

            switch (loc)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                        throw new IOException(SR.IO_SeekBeforeBegin);
                    Interlocked.Exchange(ref _position, offset);
                    break;

                case SeekOrigin.Current:
                    long pos = Interlocked.Read(ref _position);
                    if (offset + pos < 0)
                        throw new IOException(SR.IO_SeekBeforeBegin);
                    Interlocked.Exchange(ref _position, offset + pos);
                    break;

                case SeekOrigin.End:
                    long len = Interlocked.Read(ref _length);
                    if (len + offset < 0)
                        throw new IOException(SR.IO_SeekBeforeBegin);
                    Interlocked.Exchange(ref _position, len + offset);
                    break;

                default:
                    throw new ArgumentException(SR.Argument_InvalidSeekOrigin);
            }

            long finalPos = Interlocked.Read(ref _position);
            Debug.Assert(finalPos >= 0, "_position >= 0");
            return finalPos;
        }

        /// <summary>
        /// Sets the Length of the stream.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (_buffer != null)
                throw new NotSupportedException(SR.NotSupported_UmsSafeBuffer);

            EnsureNotClosed();
            EnsureWriteable();

            if (value > _capacity)
                throw new IOException(SR.IO_FixedCapacity);

            long pos = Interlocked.Read(ref _position);
            long len = Interlocked.Read(ref _length);
            if (value > len)
            {
                unsafe
                {
                    Buffer.ZeroMemory(_mem + len, value - len);
                }
            }
            Interlocked.Exchange(ref _length, value);
            if (pos > value)
            {
                Interlocked.Exchange(ref _position, value);
            }
        }

        /// <summary>
        /// Writes buffer into the stream
        /// </summary>
        /// <param name="buffer">Buffer that will be written.</param>
        /// <param name="offset">Starting index in the buffer.</param>
        /// <param name="count">Number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            WriteCore(new Span<byte>(buffer, offset, count));
        }

        public override void Write(ReadOnlySpan<byte> source)
        {
            if (GetType() == typeof(UnmanagedMemoryStream))
            {
                WriteCore(source);
            }
            else
            {
                // UnmanagedMemoryStream is not sealed, and a derived type may have overridden Write(byte[], int, int) prior
                // to this Write(Span<byte>) overload being introduced.  In that case, this Write(Span<byte>) overload
                // should use the behavior of Write(byte[],int,int) overload.
                base.Write(source);
            }
        }

        internal unsafe void WriteCore(ReadOnlySpan<byte> source)
        {
            EnsureNotClosed();
            EnsureWriteable();

            long pos = Interlocked.Read(ref _position);  // Use a local to avoid a race condition
            long len = Interlocked.Read(ref _length);
            long n = pos + source.Length;
            // Check for overflow
            if (n < 0)
            {
                throw new IOException(SR.IO_StreamTooLong);
            }

            if (n > _capacity)
            {
                throw new NotSupportedException(SR.IO_FixedCapacity);
            }

            if (_buffer == null)
            {
                // Check to see whether we are now expanding the stream and must 
                // zero any memory in the middle.
                if (pos > len)
                {
                    Buffer.ZeroMemory(_mem + len, pos - len);
                }

                // set length after zeroing memory to avoid race condition of accessing unzeroed memory
                if (n > len)
                {
                    Interlocked.Exchange(ref _length, n);
                }
            }

            fixed (byte* pBuffer = &MemoryMarshal.GetReference(source))
            {
                if (_buffer != null)
                {
                    long bytesLeft = _capacity - pos;
                    if (bytesLeft < source.Length)
                    {
                        throw new ArgumentException(SR.Arg_BufferTooSmall);
                    }

                    byte* pointer = null;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        _buffer.AcquirePointer(ref pointer);
                        Buffer.Memcpy(pointer + pos + _offset, pBuffer, source.Length);
                    }
                    finally
                    {
                        if (pointer != null)
                        {
                            _buffer.ReleasePointer();
                        }
                    }
                }
                else
                {
                    Buffer.Memcpy(_mem + pos, pBuffer, source.Length);
                }
            }

            Interlocked.Exchange(ref _position, n);
            return;
        }

        /// <summary>
        /// Writes buffer into the stream. The operation completes synchronously.
        /// </summary>
        /// <param name="buffer">Buffer that will be written.</param>
        /// <param name="offset">Starting index in the buffer.</param>
        /// <param name="count">Number of bytes to write.</param>
        /// <param name="cancellationToken">Token that can be used to cancel the operation.</param>
        /// <returns>Task that can be awaited </returns>
        public override Task WriteAsync(Byte[] buffer, Int32 offset, Int32 count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (buffer.Length - offset < count)
                throw new ArgumentException(SR.Argument_InvalidOffLen);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            try
            {
                Write(buffer, offset, count);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.Assert(!(ex is OperationCanceledException));
                return Task.FromException(ex);
            }
        }

        /// <summary>
        /// Writes buffer into the stream. The operation completes synchronously.
        /// </summary>
        /// <param name="buffer">Buffer that will be written.</param>
        /// <param name="cancellationToken">Token that can be used to cancel the operation.</param>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.FromCanceled(cancellationToken));
            }

            try
            {
                // See corresponding comment in ReadAsync for why we don't just always use Write(ReadOnlySpan<byte>).
                // Unlike ReadAsync, we could delegate to WriteAsync(byte[], ...) here, but we don't for consistency.
                if (MemoryMarshal.TryGetArray(source, out ArraySegment<byte> sourceArray))
                {
                    Write(sourceArray.Array, sourceArray.Offset, sourceArray.Count);
                }
                else
                {
                    Write(source.Span);
                }
                return default;
            }
            catch (Exception ex)
            {
                return new ValueTask(Task.FromException(ex));
            }
        }

        /// <summary>
        /// Writes a byte to the stream and advances the current Position.
        /// </summary>
        /// <param name="value"></param>
        public override void WriteByte(byte value)
        {
            EnsureNotClosed();
            EnsureWriteable();

            long pos = Interlocked.Read(ref _position);  // Use a local to avoid a race condition
            long len = Interlocked.Read(ref _length);
            long n = pos + 1;
            if (pos >= len)
            {
                // Check for overflow
                if (n < 0)
                    throw new IOException(SR.IO_StreamTooLong);

                if (n > _capacity)
                    throw new NotSupportedException(SR.IO_FixedCapacity);

                // Check to see whether we are now expanding the stream and must 
                // zero any memory in the middle.
                // don't do if created from SafeBuffer
                if (_buffer == null)
                {
                    if (pos > len)
                    {
                        unsafe
                        {
                            Buffer.ZeroMemory(_mem + len, pos - len);
                        }
                    }

                    // set length after zeroing memory to avoid race condition of accessing unzeroed memory
                    Interlocked.Exchange(ref _length, n);
                }
            }

            if (_buffer != null)
            {
                unsafe
                {
                    byte* pointer = null;
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        _buffer.AcquirePointer(ref pointer);
                        *(pointer + pos + _offset) = value;
                    }
                    finally
                    {
                        if (pointer != null)
                        {
                            _buffer.ReleasePointer();
                        }
                    }
                }
            }
            else
            {
                unsafe
                {
                    _mem[pos] = value;
                }
            }
            Interlocked.Exchange(ref _position, n);
        }
    }
}
