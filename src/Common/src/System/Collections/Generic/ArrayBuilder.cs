// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    internal struct ArrayBuilder<T>
    {
        private const int DefaultCapacity = 4;

        private T[] _array; // Starts out null, initialized on first Add.
        private int _count; // Number of items into _array we're using.

        public int Capacity
        {
            get { return _array?.Length ?? 0; }
            set
            {
                // Although it has almost exactly what we're looking for,
                // Array.Resize is not used here since it unnecessarily copies
                // the indices between Count and Capacity, which will all be
                // default-initialized.

                Debug.Assert(value > Capacity);

                var next = new T[value];
                if (_array != null)
                {
                    Array.Copy(_array, 0, next, 0, _count);
                }
                _array = next;
            }
        }

        public int Count => _count;

        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < _count);
                return _array[index];
            }
            set
            {
                Debug.Assert(index >= 0 && index < _count);
                _array[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_count == Capacity)
            {
                EnsureCapacity(_count + 1);
            }

            _array[_count++] = item;
        }

        public void AddRange(T[] items)
        {
            Debug.Assert(items != null);

            if (items.Length != 0)
            {
                int endCount = _count + items.Length;
                Debug.Assert(endCount > 0); // Check for overflow

                if (endCount > Capacity)
                {
                    EnsureCapacity(endCount);
                }

                Debug.Assert(Capacity > 0); // At least 1 item is being added
                Array.Copy(items, 0, _array, _count, items.Length);
                _count = endCount;
            }
        }

        public T[] AsArray() => _array ?? Array.Empty<T>();

        public T[] AsArray(out int count)
        {
            count = _count;
            return AsArray();
        }

        public T[] AsOrToArray()
        {
            return _count == Capacity ? AsArray() : ToArray();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_array, _count);
        }

        public T[] ToArray()
        {
            if (_count == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[_count];
            Array.Copy(_array, 0, result, 0, _count);
            return result;
        }

        public void ZeroExtend(int count)
        {
            Debug.Assert(count >= 0);
            EnsureCapacity(_count + count);
            _count += count;
        }

        private void EnsureCapacity(int minimum)
        {
            Debug.Assert(minimum > Capacity);

            int nextCapacity = Capacity == 0 ? DefaultCapacity : 2 * Capacity;

            Debug.Assert(nextCapacity > 0); // Check for overflow.
            nextCapacity = Math.Max(nextCapacity, minimum);

            Capacity = minimum;
        }

        public struct Enumerator
        {
            private readonly T[] _array;
            private readonly int _count;
            private int _index;
            
            internal Enumerator(T[] array, int count)
            {
                Debug.Assert(count >= 0);
                Debug.Assert(array == null || count <= array.Length);

                _array = array;
                _count = count;
                _index = -1;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                return ++_index < _count;
            }

            public T Current => _array[_index];
        }
    }
}
