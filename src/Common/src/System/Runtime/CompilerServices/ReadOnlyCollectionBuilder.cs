// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// The builder for read only collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection element.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    internal sealed class ReadOnlyCollectionBuilder<T> : IList<T>, System.Collections.IList
    {
        private const int DefaultCapacity = 4;

        private T[] _items;
        private int _size;
        private int _version;

        private Object _syncRoot;

        /// <summary>
        /// Constructs a ReadOnlyCollectionBuilder.
        /// </summary>
        public ReadOnlyCollectionBuilder()
        {
            _items = Array.Empty<T>();
        }

        /// <summary>
        /// Constructs a ReadOnlyCollectionBuilder with a given initial capacity.
        /// The contents are empty but builder will have reserved room for the given
        /// number of elements before any reallocations are required.
        /// </summary> 
        public ReadOnlyCollectionBuilder(int capacity)
        {
            ContractUtils.Requires(capacity >= 0, nameof(capacity));
            _items = new T[capacity];
        }

        /// <summary>
        /// Constructs a ReadOnlyCollectionBuilder, copying contents of the given collection.
        /// </summary>
        /// <param name="collection"></param>
        public ReadOnlyCollectionBuilder(IEnumerable<T> collection)
        {
            ContractUtils.Requires(collection != null, nameof(collection));

            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                _items = new T[count];
                c.CopyTo(_items, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                _items = new T[DefaultCapacity];

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Add(en.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Gets and sets the capacity of this ReadOnlyCollectionBuilder
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
            set
            {
                ContractUtils.Requires(value >= _size, nameof(value));

                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, _size);
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
        /// Returns number of elements in the ReadOnlyCollectionBuilder.
        /// </summary>
        public int Count
        {
            get { return _size; }
        }

        #region IList<T> Members

        /// <summary>
        /// Returns the index of the first occurrence of a given value in the builder.
        /// </summary>
        /// <param name="item">An item to search for.</param>
        /// <returns>The index of the first occurrence of an item.</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        /// <summary>
        /// Inserts an item to the <see cref="ReadOnlyCollectionBuilder{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        public void Insert(int index, T item)
        {
            ContractUtils.Requires(index <= _size, nameof(index));

            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        /// <summary>
        /// Removes the <see cref="ReadOnlyCollectionBuilder{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            ContractUtils.Requires(index >= 0 && index < _size, nameof(index));

            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default(T);
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
                ContractUtils.Requires(index < _size, nameof(index));
                return _items[index];
            }
            set
            {
                ContractUtils.Requires(index < _size, nameof(index));
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
            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            _items[_size++] = item;
            _version++;
        }

        /// <summary>
        /// Removes all items from the <see cref="ReadOnlyCollectionBuilder{T}"/>.
        /// </summary>
        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
            _version++;
        }

        /// <summary>
        /// Determines whether the <see cref="ReadOnlyCollectionBuilder{T}"/> contains a specific value
        /// </summary>
        /// <param name="item">the object to locate in the <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        /// <returns>true if item is found in the <see cref="ReadOnlyCollectionBuilder{T}"/>; otherwise, false.</returns>
        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < _size; i++)
                {
                    if ((Object)_items[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < _size; i++)
                {
                    if (c.Equals(_items[i], item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/> to an <see cref="Array"/>,
        /// starting at particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="ReadOnlyCollectionBuilder{T}"/>.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

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
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList Members

        bool System.Collections.IList.IsReadOnly
        {
            get { return false; }
        }

        int System.Collections.IList.Add(object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Add((T)value);
            }
            catch (InvalidCastException)
            {
                ThrowInvalidTypeException(value, nameof(value));
            }
            return Count - 1;
        }

        bool System.Collections.IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value);
            }
            else return false;
        }

        int System.Collections.IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }
            return -1;
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            ValidateNullValue(value, nameof(value));
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                ThrowInvalidTypeException(value, nameof(value));
            }
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return false; }
        }

        void System.Collections.IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        object System.Collections.IList.this[int index]
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
                    ThrowInvalidTypeException(value, nameof(value));
                }
            }
        }

        #endregion

        #region ICollection Members

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            ContractUtils.RequiresNotNull(array, nameof(array));
            ContractUtils.Requires(array.Rank == 1, nameof(array));
            Array.Copy(_items, 0, array, index, _size);
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

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
            ContractUtils.Requires(index >= 0, nameof(index));
            ContractUtils.Requires(count >= 0, nameof(count));

            Array.Reverse(_items, index, count);
            _version++;
        }

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/>.</returns>
        public T[] ToArray()
        {
            T[] array = new T[_size];
            Array.Copy(_items, 0, array, 0, _size);
            return array;
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlyCollection{T}"/> containing all of the the elements of the <see cref="ReadOnlyCollectionBuilder{T}"/>,
        /// avoiding copying the elements to the new array if possible. Resets the <see cref="ReadOnlyCollectionBuilder{T}"/> after the
        /// <see cref="ReadOnlyCollection{T}"/> has been created.
        /// </summary>
        /// <returns>A new instance of <see cref="ReadOnlyCollection{T}"/>.</returns>
        public ReadOnlyCollection<T> ToReadOnlyCollection()
        {
            // Can we use the stored array?
            T[] items;
            if (_size == _items.Length)
            {
                items = _items;
            }
            else
            {
                items = ToArray();
            }
            _items = Array.Empty<T>();
            _size = 0;
            _version++;

            return new TrueReadOnlyCollection<T>(items);
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = DefaultCapacity;
                if (_items.Length > 0)
                {
                    newCapacity = _items.Length * 2;
                }
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

        private static void ThrowInvalidTypeException(object value, string argument)
        {
            throw new ArgumentException(Strings.InvalidObjectType(value != null ? value.GetType() : (object)"null", typeof(T)), argument);
        }

        private class Enumerator : IEnumerator<T>, System.Collections.IEnumerator
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

            public T Current
            {
                get { return _current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index > _builder._size)
                    {
                        throw Error.EnumerationIsDone();
                    }
                    return _current;
                }
            }

            public bool MoveNext()
            {
                if (_version == _builder._version)
                {
                    if (_index < _builder._size)
                    {
                        _current = _builder._items[_index++];
                        return true;
                    }
                    else
                    {
                        _index = _builder._size + 1;
                        _current = default(T);
                        return false;
                    }
                }
                else
                {
                    throw Error.CollectionModifiedWhileEnumerating();
                }
            }

            #endregion

            #region IEnumerator Members

            void System.Collections.IEnumerator.Reset()
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
