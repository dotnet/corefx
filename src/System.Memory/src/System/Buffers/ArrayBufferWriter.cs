// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Buffers
{
    public sealed class ArrayBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private T[] _rentedBuffer;
        private int _index;

        private const int MinimumBufferSize = 256;

        public ArrayBufferWriter()
        {
            _rentedBuffer = ArrayPool<T>.Shared.Rent(MinimumBufferSize);
            _index = 0;
        }

        public ArrayBufferWriter(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException(nameof(initialCapacity));

            _rentedBuffer = ArrayPool<T>.Shared.Rent(initialCapacity);
            _index = 0;
        }

        public ReadOnlyMemory<T> OutputAsMemory
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.AsMemory(0, _index);
            }
        }

        public ReadOnlySpan<T> OutputAsSpan
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.AsSpan(0, _index);
            }
        }

        public int CurrentIndex
        {
            get
            {
                CheckIfDisposed();

                return _index;
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

        public int AvailableSpace
        {
            get
            {
                CheckIfDisposed();

                return _rentedBuffer.Length - _index;
            }
        }

        public void Clear()
        {
            CheckIfDisposed();

            ClearHelper();
        }

        private void ClearHelper()
        {
            _rentedBuffer.AsSpan(0, _index).Clear();
            _index = 0;
        }

        // Returns the rented buffer back to the pool
        public void Dispose()
        {
            if (_rentedBuffer == null)
            {
                return;
            }

            ArrayPool<T>.Shared.Return(_rentedBuffer, clearArray: true);
            _rentedBuffer = null;
            _index = 0;
        }

        private void CheckIfDisposed()
        {
            if (_rentedBuffer == null)
                throw new ObjectDisposedException(nameof(ArrayBufferWriter<T>));
        }

        public void Advance(int count)
        {
            CheckIfDisposed();

            if (count < 0)
                throw new ArgumentException(nameof(count));

            if (_index > _rentedBuffer.Length - count)
                ThrowHelper.ThrowInvalidOperationException(_rentedBuffer.Length);

            _index += count;
        }

        public Memory<T> GetMemory(int sizeHint)
        {
            CheckIfDisposed();

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsMemory(_index);
        }

        public Span<T> GetSpan(int sizeHint)
        {
            CheckIfDisposed();

            CheckAndResizeBuffer(sizeHint);
            return _rentedBuffer.AsSpan(_index);
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
                throw new ArgumentException(nameof(sizeHint));

            if (sizeHint == 0)
            {
                sizeHint = MinimumBufferSize;
            }

            int availableSpace = _rentedBuffer.Length - _index;

            if (sizeHint > availableSpace)
            {
                int growBy = Math.Max(sizeHint, _rentedBuffer.Length);

                int newSize = checked(_rentedBuffer.Length + growBy);

                T[] oldBuffer = _rentedBuffer;

                _rentedBuffer = ArrayPool<T>.Shared.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= _index);
                Debug.Assert(_rentedBuffer.Length >= _index);

                Span<T> previousBuffer = oldBuffer.AsSpan(0, _index);
                previousBuffer.CopyTo(_rentedBuffer);
                previousBuffer.Clear();
                ArrayPool<T>.Shared.Return(oldBuffer);
            }

            Debug.Assert(_rentedBuffer.Length - _index > 0);
            Debug.Assert(_rentedBuffer.Length - _index >= sizeHint);
        }
    }
}
