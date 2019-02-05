// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Buffers
{
    public sealed class ArrayBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private byte[] _rentedBuffer;
        private int _written;
        private long _totalWritten;

        private const int MinimumBufferSize = 256;

        public ArrayBufferWriter()
        {
            _rentedBuffer = ArrayPool<byte>.Shared.Rent(MinimumBufferSize);
            _written = 0;
            _totalWritten = 0;
        }

        public ArrayBufferWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException(nameof(initialCapacity));

            _rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
            _written = 0;
            _totalWritten = 0;
        }

        public ReadOnlyMemory<byte> OutputAsMemory
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.AsMemory(0, _written);
            }
        }

        public ReadOnlySpan<byte> OutputAsSpan
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.AsSpan(0, _written);
            }
        }

        public int BytesWritten
        {
            get
            {
                CheckIfDisposed();

                return _written;
            }
        }

        public int Capacity
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.Length;
            }
        }

        public int BytesAvailable
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.Length - _written;
            }
        }

        public long TotalBytesWritten
        {
            get
            {
                CheckIfDisposed();

                return _totalWritten;
            }
        }

        public void Reset()
        {
            CheckIfDisposed();

            ResetHelper();
        }

        private void ResetHelper()
        {
            _rentedBuffer.AsSpan(0, _written).Clear();
            _written = 0;
        }

        public async Task CopyToAndResetAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            CheckIfDisposed();

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            await stream.WriteAsync(_rentedBuffer, 0, _written, cancellationToken).ConfigureAwait(false);
            _totalWritten += _written;

            ResetHelper();
        }

        public void CopyToAndReset(Stream stream)
        {
            CheckIfDisposed();

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(_rentedBuffer, 0, _written);
            _totalWritten += _written;

            ResetHelper();
        }

        // Returns the rented buffer back to the pool
        public void Dispose()
        {
            if (_rentedBuffer == null)
            {
                return;
            }

            ArrayPool<byte>.Shared.Return(_rentedBuffer, clearArray: true);
            _rentedBuffer = null;
            _written = 0;
        }

        private void CheckIfDisposed()
        {
            if (_rentedBuffer == null)
                throw new ObjectDisposedException(nameof(ArrayBufferWriter));
        }

        void IBufferWriter<byte>.Advance(int count)
        {
            CheckIfDisposed();

            if (count < 0)
                throw new ArgumentException(nameof(count));

            if (_written > _rentedBuffer.Length - count)
                throw new InvalidOperationException("Cannot advance past the end of the buffer.");

            _written += count;
        }

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        {
            CheckIfDisposed();

            if (sizeHint < 0)
                throw new ArgumentException(nameof(sizeHint));

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsMemory(_written);
        }

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
        {
            CheckIfDisposed();

            if (sizeHint < 0)
                throw new ArgumentException(nameof(sizeHint));

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsSpan(_written);
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            Debug.Assert(sizeHint >= 0);

            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            int availableSpace = _rentedBuffer.Length - _written;

            if (sizeHint > availableSpace)
            {
                int growBy = sizeHint > _rentedBuffer.Length ? sizeHint : _rentedBuffer.Length;

                int newSize = checked(_rentedBuffer.Length + growBy);

                byte[] oldBuffer = _rentedBuffer;

                _rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= _written);
                Debug.Assert(_rentedBuffer.Length >= _written);

                oldBuffer.AsSpan(0, _written).CopyTo(_rentedBuffer);
                ArrayPool<byte>.Shared.Return(oldBuffer, clearArray: true);
            }

            Debug.Assert(_rentedBuffer.Length - _written > 0);
            Debug.Assert(_rentedBuffer.Length - _written >= sizeHint);
        }
    }
}
