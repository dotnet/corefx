// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>
    /// Helper type for avoiding allocations while building arrays.
    /// </summary>
    internal struct ArrayBuilder<T>
    {
        private T[] _array; // Starts out null, initialized on first Add.
        private int _count; // Number of items into _array we're using.

        /// <summary>
        /// Initializes the <see cref="ArrayBuilder{T}"/> with a specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity of the array to allocate.</param>
        public ArrayBuilder(int capacity) : this()
        {
            Debug.Assert(capacity >= 0);
            if (capacity > 0)
            {
                _array = new T[capacity];
            }
        }

        /// <summary>
        /// Gets the number of items this instance can store without re-allocating,
        /// or 0 if the backing array is <c>null</c>.
        /// </summary>
        public int Capacity => _array?.Length ?? 0;

        /// <summary>
        /// Gets the number of items in the array currently in use.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets or sets the item at a certain index in the array.
        /// </summary>
        /// <param name="index">The index into the array.</param>
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

        /// <summary>
        /// Adds an item to the backing array, resizing it if necessary.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            if (_count == Capacity)
            {
                EnsureCapacity(_count + 1);
            }

            UncheckedAdd(item);
        }

        /// <summary>
        /// Gets an enumerator which enumerates the contents of this builder.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_array, _count);
        }

        /// <summary>
        /// Returns an array with equivalent contents as this builder.
        /// </summary>
        /// <remarks>
        /// Do not call this method twice on the same builder.
        /// </remarks>
        public T[] ToArray()
        {
            if (_count == 0)
            {
                return Array.Empty<T>();
            }

            Debug.Assert(_array != null); // Nonzero _count should imply this

            T[] result = _array;
            if (_count < result.Length)
            {
                // Avoid a bit of overhead (method call, some branches, extra codegen)
                // which would be incurred by using Array.Resize
                result = new T[_count];
                Array.Copy(_array, 0, result, 0, _count);
            }

#if DEBUG
            // Try to prevent callers from using the ArrayBuilder after ToArray, if _count != 0.
            _count = -1;
            _array = null;
#endif

            return result;
        }

        /// <summary>
        /// Adds an item to the backing array, without checking if there is room.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Use this method if you know there is enough space in the <see cref="ArrayBuilder{T}"/>
        /// for another item, and you are writing performance-sensitive code.
        /// </remarks>
        public void UncheckedAdd(T item)
        {
            Debug.Assert(_count < Capacity);

            _array[_count++] = item;
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
