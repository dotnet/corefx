// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Dynamic.Utils;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Builder for read only collections.
    /// </summary>
    /// <typeparam name="T">The type of the collection element.</typeparam>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class ReadOnlyCollectionBuilder<T> : IList<T>, IList
    {
        private const int DefaultCapacity = 4;

        private T[] _items;
        private int _version;

        /// <summary>
        /// Constructs a <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public ReadOnlyCollectionBuilder()
        {
            _items = Array.Empty<T>();
        }

        /// <summary>
        /// Constructs a <see cref="ReadOnlyCollectionBuilder{T}"/> with a given initial capacity.
        /// The contents are empty but builder will have reserved room for the given
        /// number of elements before any reallocations are required.
        /// </summary>
        /// <param name="capacity">Initial capacity of the builder.</param>
        public ReadOnlyCollectionBuilder(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            //Contract.EndContractBlock();

            if (capacity == 0)
                _items = Array.Empty<T>();
            else
                _items = new T[capacity];
        }

        /// <summary>
        /// Constructs a <see cref="ReadOnlyCollectionBuilder{T}"/>, copying contents of the given collection.
        /// </summary>
        /// <param name="collection">The collection whose elements to copy to the builder.</param>
        public ReadOnlyCollectionBuilder(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            //Contract.EndContractBlock();

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = Array.Empty<T>();
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    Count = count;
                }
            }
            else
            {
                Count = 0;
                _items = Array.Empty<T>();
                // This enumerable could be empty.  Let Add allocate a new array, if needed.
                // Note it will also go to _defaultCapacity first, not 1, then 2, etc.

                foreach (T item in collection)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Gets and sets the capacity of this <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public int Capacity
        {
            get {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _items.Length;
            }
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                //Contract.EndContractBlock();

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (Count > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, Count);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = Array.Empty<T>();
                    }
                }
            }
        }

        /// <summary>
        /// Returns number of elements in the <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public int Count { get; private set; }

        #region IList<T> Members

        /// <summary>
        /// Returns the index of the first occurrence of a given value in the builder.
        /// </summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the first occurrence of an item.</returns>
        public int IndexOf(T item)
        {
            Contract.Ensures(Contract.Result<int>() >= -1);
            Contract.Ensures(Contract.Result<int>() < Count);
            return Array.IndexOf(_items, item, 0, Count);
        }

        /// <summary>
        /// Inserts an item to the <see cref="ReadOnlyCollectionBuilder{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        public void Insert(int index, T item)
        {
            // Note that insertions at the end are legal.
            // Following trick can reduce the range check by one
            if ((uint)index > (uint)Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            //Contract.EndContractBlock();

            if (Count == _items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            if (index < Count)
            {
                Array.Copy(_items, index, _items, index + 1, Count - index);
            }
            _items[index] = item;
            Count++;
            _version++;
        }

        /// <summary>
        /// Removes the <see cref="ReadOnlyCollectionBuilder{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            // Following trick can reduce the range check by one
            if ((uint)index >= (uint)Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            //Contract.EndContractBlock();

            Count--;
            if (index < Count)
            {
                Array.Copy(_items, index + 1, _items, index, Count - index);
            }
            _items[Count] = default(T);
            _version++;
        }

        /// <summary>
        ///  Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                //Contract.EndContractBlock();

                return _items[index];
            }
            set
            {
                // Following trick can reduce the range check by one
                if ((uint)index >= (uint)Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                //Contract.EndContractBlock();

                _items[index] = value;
                _version++;
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        public void Add(T item)
        {
            if (Count == _items.Length)
            {
                EnsureCapacity(Count + 1);
            }
            _items[Count++] = item;
            _version++;
        }

        /// <summary>
        /// Removes all items from the <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                Array.Clear(_items, 0, Count); // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
                Count = 0;
            }
            _version++;
        }

        /// <summary>
        /// Determines whether the <see cref="ReadOnlyCollectionBuilder{T}"/> contains a specific value
        /// </summary>
        /// <param name="item">the object to locate in the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        /// <returns>true if item is found in the <see cref="ReadOnlyCollectionBuilder{T}"/>; otherwise, false.</returns>
        public bool Contains(T item) => Count != 0 && IndexOf(item) != -1;

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/> to an <see cref="Array"/>,
        /// starting at particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(_items, 0, array, arrayIndex, Count);
        }

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        /// <returns>true if item was successfully removed from the <see cref="ReadOnlyCollectionBuilder{T}"/>;
        /// otherwise, false. This method also returns false if item is not found in the original <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #endregion

        #region IList Members

        bool IList.IsReadOnly => false;

        int IList.Add(object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Add((T)value);
            }
            catch (InvalidCastException)
            {
                throw InvalidTypeException(value, nameof(value));
            }
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value);
            }
            else return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                throw InvalidTypeException(value, nameof(value));
            }
        }

        bool IList.IsFixedSize => false;

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                ValidateNullValue(value, nameof(value));

                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw InvalidTypeException(value, nameof(value));
                }
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            if ((array != null) && (array.Rank != 1))
            {
                throw new ArgumentException(); // Arg_RankMultiDimNotSupported
            }
            //Contract.EndContractBlock();

            try
            {
                // Array.Copy will check for null.
                Array.Copy(_items, 0, array, index, Count);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(); // Argument_InvalidArrayType
            }
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        #endregion

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, Count);
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (Count - index < count)
            {
                throw new ArgumentException(); // Argument_InvalidOffLen
            }
            //Contract.EndContractBlock();

            Array.Reverse(_items, index, count);
            _version++;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/>.</returns>
        public T[] ToArray()
        {
            Contract.Ensures(Contract.Result<T[]>() != null);
            Contract.Ensures(Contract.Result<T[]>().Length == Count);

            if (Count == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[Count];
            Array.Copy(_items, 0, array, 0, Count);
            return array;
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlyCollection{T}"/> containing all of the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/>,
        /// avoiding copying the elements to the new array if possible. Resets the <see cref="ReadOnlyCollectionBuilder{T}"/> after the
        /// <see cref="ReadOnlyCollection{T}"/> has been created.
        /// </summary>
        /// <returns>A new instance of <see cref="ReadOnlyCollection{T}"/>.</returns>
        public ReadOnlyCollection<T> ToReadOnlyCollection()
        {
            // Can we use the stored array?
            T[] items;
            if (Count == _items.Length)
            {
                items = _items;
            }
            else
            {
                items = ToArray();
            }
            _items = Array.Empty<T>();
            Count = 0;
            _version++;

            return new TrueReadOnlyCollection<T>(items);
        }

        // Ensures that the capacity of this list is at least the given minimum
        // value. If the currect capacity of the list is less than min, the
        // capacity is increased to twice the current capacity or to min,
        // whichever is larger.
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length > 0 ? _items.Length * 2 : DefaultCapacity;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                newCapacity = (int)Math.Min((uint)newCapacity, (uint)0X7FEFFFFF);
                if (newCapacity < min)
                {
                    newCapacity = min;
                }
                Capacity = newCapacity;
            }
        }

        private static bool IsCompatibleObject(object value)
        {
            return ((value is T) || (value == null && default(T) == null));
        }

        private static void ValidateNullValue(object value, string argument)
        {
            if (value == null && !(default(T) == null))
            {
                throw new ArgumentException(Strings.InvalidNullValue(typeof(T)), argument);
            }
        }

        private static Exception InvalidTypeException(object value, string argument)
        {
            return new ArgumentException(Strings.InvalidObjectType(value != null ? value.GetType() : (object)"null", typeof(T)), argument);
        }

        [Serializable]
        private class Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly ReadOnlyCollectionBuilder<T> _builder;
            private readonly int _version;

            private int _index;
            private T _current;

            internal Enumerator(ReadOnlyCollectionBuilder<T> builder)
            {
                _builder = builder;
                _version = builder._version;
                _index = 0;
                _current = default(T);
            }

            #region IEnumerator<T> Members

            public T Current => _current;

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get
                {
                    if (_index <= 0)
                    {
                        throw Error.EnumerationIsDone();
                    }
                    return _current;
                }
            }

            public bool MoveNext()
            {
                ReadOnlyCollectionBuilder<T> localBuilder = _builder;

                // Following trick can reduce the range check by one
                if (_version == localBuilder._version && ((uint)_index < (uint)localBuilder.Count))
                {
                    _current = localBuilder._items[_index++];
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (_version != _builder._version)
                {
                    throw Error.CollectionModifiedWhileEnumerating();
                }

                _index = -1;
                _current = default(T);
                return false;
            }

            #endregion

            #region IEnumerator Members

            void IEnumerator.Reset()
            {
                if (_version != _builder._version)
                {
                    throw Error.CollectionModifiedWhileEnumerating();
                }
                _index = 0;
                _current = default(T);
            }

            #endregion
        }
    }
}
