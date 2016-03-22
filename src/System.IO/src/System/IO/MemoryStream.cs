// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace System.IO
{
    // A MemoryStream represents a Stream in memory (ie, it has no backing store).
    // This stream may reduce the need for temporary buffers and files in 
    // an application.  
    // 
    // There are two ways to create a MemoryStream.  You can initialize one
    // from an unsigned byte array, or you can create an empty one.  Empty 
    // memory streams are resizable, while ones created with a byte array provide
    // a stream "view" of the data.
    public class MemoryStream : Stream
    {
        private byte[] _buffer;    // Either allocated internally or externally.
        private int _origin;       // For user-provided arrays, start at this origin
        private int _position;     // read/write head.
        [ContractPublicPropertyName("Length")]
        private int _length;       // Number of bytes within the memory stream
        private int _capacity;     // length of usable portion of buffer for stream
        // Note that _capacity == _buffer.Length for non-user-provided byte[]'s

        private bool _expandable;  // User-provided buffers aren't expandable.
        private bool _writable;    // Can user write to this stream?
        private bool _exposable;   // Whether the array can be returned to the user.
        private bool _isOpen;      // Is this stream open or closed?

        // <TODO>In V2, if we get support for arrays of more than 2 GB worth of elements,
        // consider removing this constraing, or setting it to Int64.MaxValue.</TODO>
        private const int MemStreamMaxLength = int.MaxValue;

        public MemoryStream()
            : this(0)
        {
        }

        public MemoryStream(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NegativeCapacity);
            }

            _buffer = new byte[capacity];
            _capacity = capacity;
            _expandable = true;
            _writable = true;
            _exposable = true;
            _origin = 0;      // Must be 0 for byte[]'s created by MemoryStream
            _isOpen = true;
        }

        public MemoryStream(byte[] buffer)
            : this(buffer, true)
        {
        }

        public MemoryStream(byte[] buffer, bool writable)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }

            _buffer = buffer;
            _length = _capacity = buffer.Length;
            _writable = writable;
            _exposable = false;
            _origin = 0;
            _isOpen = true;
        }

        public MemoryStream(byte[] buffer, int index, int count)
            : this(buffer, index, count, true, false)
        {
        }

        public MemoryStream(byte[] buffer, int index, int count, bool writable)
            : this(buffer, index, count, writable, false)
        {
        }

        public MemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            _buffer = buffer;
            _origin = _position = index;
            _length = _capacity = index + count;
            _writable = writable;
            _exposable = publiclyVisible;  // Can TryGetBuffer return the array?
            _expandable = false;
            _isOpen = true;
        }

        public override bool CanRead
        {
            [Pure]
            get
            { return _isOpen; }
        }

        public override bool CanSeek
        {
            [Pure]
            get
            { return _isOpen; }
        }

        public override bool CanWrite
        {
            [Pure]
            get
            { return _writable; }
        }

        private void EnsureWriteable()
        {
            if (!CanWrite)
            {
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _isOpen = false;
                    _writable = false;
                    _expandable = false;
                    // Don't set buffer to null - allow TryGetBuffer & ToArray to work.
                }
            }
            finally
            {
                // Call base.Close() to cleanup async IO resources
                base.Dispose(disposing);
            }
        }

        // returns a bool saying whether we allocated a new array.
        private bool EnsureCapacity(int value)
        {
            // Check for overflow
            if (value < 0)
            {
                throw new IOException(SR.IO_IO_StreamTooLong);
            }
            if (value > _capacity)
            {
                int newCapacity = value;
                if (newCapacity < 256)
                {
                    newCapacity = 256;
                }
                if (newCapacity < _capacity * 2)
                {
                    newCapacity = _capacity * 2;
                }

                Capacity = newCapacity;
                return true;
            }
            return false;
        }

        public override void Flush()
        {
        }

#pragma warning disable 1998 //async method with no await operators
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Flush();
        }
#pragma warning restore 1998

        public virtual bool TryGetBuffer(out ArraySegment<byte> buffer)
        {
            if (!_exposable)
            {
                buffer = default(ArraySegment<byte>);
                return false;
            }

            buffer = new ArraySegment<byte>(_buffer, offset: _origin, count: (_length - _origin));
            return true;
        }

        // -------------- PERF: Internal functions for fast direct access of MemoryStream buffer (cf. BinaryReader for usage) ---------------

        // PERF: Internal sibling of GetBuffer, always returns a buffer (cf. GetBuffer())
        internal byte[] InternalGetBuffer()
        {
            return _buffer;
        }

        // PERF: True cursor position, we don't need _origin for direct access
        internal int InternalGetPosition()
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
            return _position;
        }

        // PERF: Takes out Int32 as fast as possible
        internal int InternalReadInt32()
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }

            int pos = (_position += 4); // use temp to avoid race
            if (pos > _length)
            {
                _position = _length;
                throw new EndOfStreamException(SR.IO_EOF_ReadBeyondEOF);
            }
            return (int)(_buffer[pos - 4] | _buffer[pos - 3] << 8 | _buffer[pos - 2] << 16 | _buffer[pos - 1] << 24);
        }

        // PERF: Get actual length of bytes available for read; do sanity checks; shift position - i.e. everything except actual copying bytes
        internal int InternalEmulateRead(int count)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }

            int n = _length - _position;
            if (n > count)
            {
                n = count;
            }
            if (n < 0)
            {
                n = 0;
            }

            Debug.Assert(_position + n >= 0, "_position + n >= 0");  // len is less than 2^31 -1.
            _position += n;
            return n;
        }

        // Gets & sets the capacity (number of bytes allocated) for this stream.
        // The capacity cannot be set to a value less than the current length
        // of the stream.
        // 
        public virtual int Capacity
        {
            get
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
                }

                return _capacity - _origin;
            }
            set
            {
                // Only update the capacity if the MS is expandable and the value is different than the current capacity.
                // Special behavior if the MS isn't expandable: we don't throw if value is the same as the current capacity
                if (value < Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_SmallCapacity);
                }
                if (!_isOpen)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
                }
                if (!_expandable && (value != Capacity))
                {
                    throw new NotSupportedException(SR.NotSupported_MemStreamNotExpandable);
                }

                // MemoryStream has this invariant: _origin > 0 => !expandable (see ctors)
                if (_expandable && value != _capacity)
                {
                    if (value > 0)
                    {
                        byte[] newBuffer = new byte[value];
                        if (_length > 0)
                        {
                            Buffer.BlockCopy(_buffer, 0, newBuffer, 0, _length);
                        }

                        _buffer = newBuffer;
                    }
                    else
                    {
                        _buffer = null;
                    }
                    _capacity = value;
                }
            }
        }

        public override long Length
        {
            get
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
                }

                return _length - _origin;
            }
        }

        public override long Position
        {
            get
            {
                if (!_isOpen)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
                }

                return _position - _origin;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_NeedNonNegNum);
                }
                if (!_isOpen)
                {
                    throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
                }
                if (value > MemStreamMaxLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_StreamLength);
                }

                _position = _origin + (int)value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }

            int n = _length - _position;
            if (n > count)
            {
                n = count;
            }
            if (n <= 0)
            {
                return 0;
            }

            Debug.Assert(_position + n >= 0, "_position + n >= 0");  // len is less than 2^31 -1.

            if (n <= 8)
            {
                int byteCount = n;
                while (--byteCount >= 0)
                    buffer[offset + byteCount] = _buffer[_position + byteCount];
            }
            else
                Buffer.BlockCopy(_buffer, _position, buffer, offset, n);
            _position += n;

            return n;
        }

        public override Task<int> ReadAsync(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            return ReadAsyncImpl(buffer, offset, count, cancellationToken);
        }

#pragma warning disable 1998 //async method with no await operators
        private async Task<int> ReadAsyncImpl(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Read(buffer, offset, count);
        }
#pragma warning restore 1998


        public override int ReadByte()
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }

            if (_position >= _length)
            {
                return -1;
            }

            return _buffer[_position++];
        }


        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            // This implementation offers beter performance compared to the base class version.

            // The parameter checks must be in sync with the base version:
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), SR.ArgumentOutOfRange_NeedPosNum);
            }
            if (!CanRead && !CanWrite)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
            if (!destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException(nameof(destination), SR.ObjectDisposed_StreamClosed);
            }
            if (!CanRead)
            {
                throw new NotSupportedException(SR.NotSupported_UnreadableStream);
            }
            if (!destination.CanWrite)
            {
                throw new NotSupportedException(SR.NotSupported_UnwritableStream);
            }

            // If we have been inherited into a subclass, the following implementation could be incorrect
            // since it does not call through to Read() or Write() which a subclass might have overriden.  
            // To be safe we will only use this implementation in cases where we know it is safe to do so,
            // and delegate to our base class (which will call into Read/Write) when we are not sure.
            if (GetType() != typeof(MemoryStream))
            {
                return base.CopyToAsync(destination, bufferSize, cancellationToken);
            }

            return CopyToAsyncImpl(destination, bufferSize, cancellationToken);
        }

        private async Task CopyToAsyncImpl(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Avoid copying data from this buffer into a temp buffer:
            //   (require that InternalEmulateRead does not throw,
            //    otherwise it needs to be wrapped into try-catch-Task.FromException like memStrDest.Write below)

            int pos = _position;
            int n = InternalEmulateRead(_length - _position);

            // If destination is not a memory stream, write there asynchronously:
            MemoryStream memStrDest = destination as MemoryStream;
            if (memStrDest == null)
            {
                await destination.WriteAsync(_buffer, pos, n, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                memStrDest.Write(_buffer, pos, n);
            }
        }


        public override long Seek(long offset, SeekOrigin loc)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
            if (offset > MemStreamMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_StreamLength);
            }

            switch (loc)
            {
                case SeekOrigin.Begin:
                    {
                        int tempPosition = unchecked(_origin + (int)offset);
                        if (offset < 0 || tempPosition < _origin)
                        {
                            throw new IOException(SR.IO_IO_SeekBeforeBegin);
                        }

                        _position = tempPosition;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        int tempPosition = unchecked(_position + (int)offset);
                        if (unchecked(_position + offset) < _origin || tempPosition < _origin)
                        {
                            throw new IOException(SR.IO_IO_SeekBeforeBegin);
                        }

                        _position = tempPosition;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        int tempPosition = unchecked(_length + (int)offset);
                        if (unchecked(_length + offset) < _origin || tempPosition < _origin)
                        {
                            throw new IOException(SR.IO_IO_SeekBeforeBegin);
                        }

                        _position = tempPosition;
                        break;
                    }
                default:
                    throw new ArgumentException(SR.Argument_InvalidSeekOrigin);
            }

            Debug.Assert(_position >= 0, "_position >= 0");
            return _position;
        }

        // Sets the length of the stream to a given value.  The new
        // value must be nonnegative and less than the space remaining in
        // the array, Int32.MaxValue - origin
        // Origin is 0 in all cases other than a MemoryStream created on
        // top of an existing array and a specific starting offset was passed 
        // into the MemoryStream constructor.  The upper bounds prevents any 
        // situations where a stream may be created on top of an array then 
        // the stream is made longer than the maximum possible length of the 
        // array (Int32.MaxValue).
        // 
        public override void SetLength(long value)
        {
            if (value < 0 || value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_StreamLength);
            }
            EnsureWriteable();

            // Origin wasn't publicly exposed above.
            Debug.Assert(MemStreamMaxLength == int.MaxValue);  // Check parameter validation logic in this method if this fails.
            if (value > (int.MaxValue - _origin))
            {
                throw new ArgumentOutOfRangeException(nameof(value), SR.ArgumentOutOfRange_StreamLength);
            }

            int newLength = _origin + (int)value;
            bool allocatedNewArray = EnsureCapacity(newLength);
            if (!allocatedNewArray && newLength > _length)
            {
                Array.Clear(_buffer, _length, newLength - _length);
            }

            _length = newLength;
            if (_position > newLength)
            {
                _position = newLength;
            }
        }

        public virtual byte[] ToArray()
        {
            //BCLDebug.Perf(_exposable, "MemoryStream::GetBuffer will let you avoid a copy.");
            int count = _length - _origin;
            if (count == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] copy = new byte[count];
            Buffer.BlockCopy(_buffer, _origin, copy, 0, _length - _origin);
            return copy;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
            EnsureWriteable();

            int i = _position + count;
            // Check for overflow
            if (i < 0)
            {
                throw new IOException(SR.IO_IO_StreamTooLong);
            }

            if (i > _length)
            {
                bool mustZero = _position > _length;
                if (i > _capacity)
                {
                    bool allocatedNewArray = EnsureCapacity(i);
                    if (allocatedNewArray)
                    {
                        mustZero = false;
                    }
                }
                if (mustZero)
                {
                    Array.Clear(_buffer, _length, i - _length);
                }
                _length = i;
            }
            if ((count <= 8) && (buffer != _buffer))
            {
                int byteCount = count;
                while (--byteCount >= 0)
                {
                    _buffer[_position + byteCount] = buffer[offset + byteCount];
                }
            }
            else
            {
                Buffer.BlockCopy(buffer, offset, _buffer, _position, count);
            }
            _position = i;
        }

        public override Task WriteAsync(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), SR.ArgumentNull_Buffer);
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }

            return WriteAsyncImpl(buffer, offset, count, cancellationToken);
        }

#pragma warning disable 1998 //async method with no await operators
        private async Task WriteAsyncImpl(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Write(buffer, offset, count);
        }
#pragma warning restore 1998

        public override void WriteByte(byte value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }
            EnsureWriteable();

            if (_position >= _length)
            {
                int newLength = _position + 1;
                bool mustZero = _position > _length;
                if (newLength >= _capacity)
                {
                    bool allocatedNewArray = EnsureCapacity(newLength);
                    if (allocatedNewArray)
                    {
                        mustZero = false;
                    }
                }
                if (mustZero)
                {
                    Array.Clear(_buffer, _length, _position - _length);
                }
                _length = newLength;
            }
            _buffer[_position++] = value;
        }

        // Writes this MemoryStream to another stream.
        public virtual void WriteTo(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), SR.ArgumentNull_Stream);
            }
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_StreamClosed);
            }

            stream.Write(_buffer, _origin, _length - _origin);
        }
    }
}
