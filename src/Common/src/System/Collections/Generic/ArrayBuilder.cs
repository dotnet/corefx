// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    internal struct ArrayBuilder<T>
    {
        private T[] _array; // Starts out null, initialized on first Add.
        private int _count; // Number of items into _array we're using.

        public ArrayBuilder(int capacity)
        {
            Debug.Assert(capacity >= 0);
            if (capacity != 0)
            {
                _array = new T[capacity];
            }
        }

        public int Capacity => _array?.Length ?? 0;

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

            UncheckedAdd(item);
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

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_array, _count);
        }

        public T[] ToArray()
        {
#if DEBUG
            // Try to prevent callers from using the ArrayBuilder after ToArray.
            _count = -1;
#endif

            if (_array == null)
            {
                return Array.Empty<T>();
            }

            if (_count < _array.Length)
            {
                Array.Resize(ref _array, _count);
            }

            return _array;
        }

        public void UncheckedAdd(T item)
        {
            Debug.Assert(_count < Capacity);

            _array[_count++] = item;
        }

        public void ZeroExtend(int count)
        {
            Debug.Assert(count >= 0);

            if (count > 0)
            {
                EnsureCapacity(_count + count);
            }
            _count += count;
        }

        private void EnsureCapacity(int minimum)
        {
            Debug.Assert(minimum > Capacity);

            int nextCapacity = 2 * Capacity + 1;

            Debug.Assert(nextCapacity > 0); // Check for overflow.
            nextCapacity = Math.Max(nextCapacity, minimum);
            
            // Array.Resize will unnecessarily copy the slots @ _count
            // and after which are all default-initialized.
            T[] next = new T[nextCapacity];
            if (_count > 0)
            {
                Array.Copy(_array, 0, next, 0, _count);
            }
            _array = next;
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

            public bool MoveNext()
            {
                return ++_index < _count;
            }

            public T Current => _array[_index];
        }
    }
}
