// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    internal static class ValueList
    {
        public static ValueList<T> Create<T>()
        {
            return new ValueList<T> { _array = Array.Empty<T>() };
        }

        public static ValueList<T> Create<T>(int capacity)
        {
            Debug.Assert(capacity >= 0);

            T[] array = capacity == 0 ? Array.Empty<T>() : new T[capacity];
            return new ValueList<T> { _array = array };
        }
    }

    internal struct ValueList<T>
    {
        private const int DefaultCapacity = 4;

        // Not possible since currently, this is being used by System.Private.Uri.
        // [EditorBrowsable(EditorBrowsableState.Never)]
        internal T[] _array;
        private int _count;

        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                Debug.Assert(value > Capacity);
                Array.Resize(ref _array, value);
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
            if (_count == _array.Length)
            {
                EnsureCapacity(_count + 1);
            }

            _array[_count++] = item;
        }

        public T[] Extract(out int count)
        {
            count = Count;
            return _array;
        }

        public T[] ExtractOrToArray()
        {
            return Count == Capacity ? _array : ToArray();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_array, _count);
        }

        public T[] ToArray()
        {
            var result = new T[Count];
            Array.Copy(_array, 0, result, 0, Count);
            return result;
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
                Debug.Assert(array != null);
                Debug.Assert(count >= 0 && count <= array.Length);

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
