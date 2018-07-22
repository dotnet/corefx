// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member", Target = "System.ComponentModel.PropertyDescriptorCollection.System.Collections.IDictionary.Add(System.Object,System.Object):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member", Target = "System.ComponentModel.PropertyDescriptorCollection.System.Collections.IList.set_Item(System.Int32,System.Object):System.Void")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Scope = "member", Target = "System.ComponentModel.PropertyDescriptorCollection.System.Collections.IDictionary.set_Item(System.Object,System.Object):System.Void")]
[assembly: SuppressMessage("Microsoft.Security", "CA2112:SecuredTypesShouldNotExposeFields", Scope = "type", Target = "System.ComponentModel.PropertyDescriptorCollection")]

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a collection of properties.
    /// </summary>
    public class PropertyDescriptorCollection : ICollection, IList, IDictionary
    {
        /// <summary>
        /// An empty PropertyDescriptorCollection that can used instead of creating a new one with no items.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2112:SecuredTypesShouldNotExposeFields")]
        public static readonly PropertyDescriptorCollection Empty = new PropertyDescriptorCollection(null, true);

        private IDictionary _cachedFoundProperties;
        private bool _cachedIgnoreCase;
        private PropertyDescriptor[] _properties;
        private readonly string[] _namedSort;
        private readonly IComparer _comparer;
        private bool _propsOwned;
        private bool _needSort;
        private readonly bool _readOnly;

        private readonly object _internalSyncObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref='System.ComponentModel.PropertyDescriptorCollection'/>
        /// class.
        /// </summary>
        public PropertyDescriptorCollection(PropertyDescriptor[] properties)
        {
            if (properties == null)
            {
                _properties = Array.Empty<PropertyDescriptor>();
                Count = 0;
            }
            else
            {
                _properties = properties;
                Count = properties.Length;
            }
            _propsOwned = true;
        }

        /// <summary>
        /// Initializes a new instance of a property descriptor collection, and allows you to mark the
        /// collection as read-only so it cannot be modified.
        /// </summary>
        public PropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly) : this(properties)
        {
            _readOnly = readOnly;
        }

        private PropertyDescriptorCollection(PropertyDescriptor[] properties, int propCount, string[] namedSort, IComparer comparer)
        {
            _propsOwned = false;
            if (namedSort != null)
            {
                _namedSort = (string[])namedSort.Clone();
            }
            _comparer = comparer;
            _properties = properties;
            Count = propCount;
            _needSort = true;
        }

        /// <summary>
        /// Gets the number of property descriptors in the  collection.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the property with the specified index number.
        /// </summary>
        public virtual PropertyDescriptor this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                EnsurePropsOwned();
                return _properties[index];
            }
        }

        /// <summary>
        /// Gets the property with the specified name.
        /// </summary>
        public virtual PropertyDescriptor this[string name] => Find(name, false);

        public int Add(PropertyDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            EnsureSize(Count + 1);
            _properties[Count++] = value;
            return Count - 1;
        }

        public void Clear()
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            Count = 0;
            _cachedFoundProperties = null;
        }

        public bool Contains(PropertyDescriptor value) => IndexOf(value) >= 0;

        public void CopyTo(Array array, int index)
        {
            EnsurePropsOwned();
            Array.Copy(_properties, 0, array, index, Count);
        }

        private void EnsurePropsOwned()
        {
            if (!_propsOwned)
            {
                _propsOwned = true;
                if (_properties != null)
                {
                    PropertyDescriptor[] newProps = new PropertyDescriptor[Count];
                    Array.Copy(_properties, 0, newProps, 0, Count);
                    _properties = newProps;
                }
            }

            if (_needSort)
            {
                _needSort = false;
                InternalSort(_namedSort);
            }
        }

        private void EnsureSize(int sizeNeeded)
        {
            if (sizeNeeded <= _properties.Length)
            {
                return;
            }

            if (_properties.Length == 0)
            {
                Count = 0;
                _properties = new PropertyDescriptor[sizeNeeded];
                return;
            }

            EnsurePropsOwned();

            int newSize = Math.Max(sizeNeeded, _properties.Length * 2);
            PropertyDescriptor[] newProps = new PropertyDescriptor[newSize];
            Array.Copy(_properties, 0, newProps, 0, Count);
            _properties = newProps;
        }

        /// <summary>
        /// Gets the description of the property with the specified name.
        /// </summary>
        public virtual PropertyDescriptor Find(string name, bool ignoreCase)
        {
            lock (_internalSyncObject)
            {
                PropertyDescriptor p = null;

                if (_cachedFoundProperties == null || _cachedIgnoreCase != ignoreCase)
                {
                    _cachedIgnoreCase = ignoreCase;
                    if (ignoreCase)
                    {
                        _cachedFoundProperties = new Hashtable(StringComparer.OrdinalIgnoreCase);
                    }
                    else
                    {
                        _cachedFoundProperties = new Hashtable();
                    }
                }

                // first try to find it in the cache
                //
                object cached = _cachedFoundProperties[name];

                if (cached != null)
                {
                    return (PropertyDescriptor)cached;
                }

                // Now start walking from where we last left off, filling
                // the cache as we go.
                //
                for (int i = 0; i < Count; i++)
                {
                    if (ignoreCase)
                    {
                        if (string.Equals(_properties[i].Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            _cachedFoundProperties[name] = _properties[i];
                            p = _properties[i];
                            break;
                        }
                    }
                    else
                    {
                        if (_properties[i].Name.Equals(name))
                        {
                            _cachedFoundProperties[name] = _properties[i];
                            p = _properties[i];
                            break;
                        }
                    }
                }

                return p;
            }
        }

        public int IndexOf(PropertyDescriptor value) => Array.IndexOf(_properties, value, 0, Count);

        public void Insert(int index, PropertyDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            EnsureSize(Count + 1);
            if (index < Count)
            {
                Array.Copy(_properties, index, _properties, index + 1, Count - index);
            }
            _properties[index] = value;
            Count++;
        }

        public void Remove(PropertyDescriptor value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            int index = IndexOf(value);

            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            if (_readOnly)
            {
                throw new NotSupportedException();
            }

            if (index < Count - 1)
            {
                Array.Copy(_properties, index + 1, _properties, index, Count - index - 1);
            }
            _properties[Count - 1] = null;
            Count--;
        }

        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection, using the default sort for this collection, 
        /// which is usually alphabetical.
        /// </summary>
        public virtual PropertyDescriptorCollection Sort()
        {
            return new PropertyDescriptorCollection(_properties, Count, _namedSort, _comparer);
        }


        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection. Any specified NamedSort arguments will 
        /// be applied first, followed by sort using the specified IComparer.
        /// </summary>
        public virtual PropertyDescriptorCollection Sort(string[] names)
        {
            return new PropertyDescriptorCollection(_properties, Count, names, _comparer);
        }

        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection. Any specified NamedSort arguments will 
        /// be applied first, followed by sort using the specified IComparer.
        /// </summary>
        public virtual PropertyDescriptorCollection Sort(string[] names, IComparer comparer)
        {
            return new PropertyDescriptorCollection(_properties, Count, names, comparer);
        }

        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection, using the specified IComparer to compare, 
        /// the PropertyDescriptors contained in the collection.
        /// </summary>
        public virtual PropertyDescriptorCollection Sort(IComparer comparer)
        {
            return new PropertyDescriptorCollection(_properties, Count, _namedSort, comparer);
        }

        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection. Any specified NamedSort arguments will 
        /// be applied first, followed by sort using the specified IComparer.
        /// </summary>
        protected void InternalSort(string[] names)
        {
            if (_properties.Length == 0)
            {
                return;
            }

            InternalSort(_comparer);

            if (names != null && names.Length > 0)
            {
                List<PropertyDescriptor> propList = new List<PropertyDescriptor>(_properties);
                int foundCount = 0;
                int propCount = _properties.Length;

                for (int i = 0; i < names.Length; i++)
                {
                    for (int j = 0; j < propCount; j++)
                    {
                        PropertyDescriptor currentProp = propList[j];

                        // Found a matching property. Here, we add it to our array. We also
                        // mark it as null in our array list so we don't add it twice later.
                        //
                        if (currentProp != null && currentProp.Name.Equals(names[i]))
                        {
                            _properties[foundCount++] = currentProp;
                            propList[j] = null;
                            break;
                        }
                    }
                }

                // At this point we have filled in the first "foundCount" number of propeties, one for each
                // name in our name array. If a name didn't match, then it is ignored. Next, we must fill
                // in the rest of the properties. We now have a sparse array containing the remainder, so
                // it's easy.
                //
                for (int i = 0; i < propCount; i++)
                {
                    if (propList[i] != null)
                    {
                        _properties[foundCount++] = propList[i];
                    }
                }

                Debug.Assert(foundCount == propCount, "We did not completely fill our property array");
            }
        }

        /// <summary>
        /// Sorts the members of this PropertyDescriptorCollection using the specified IComparer.
        /// </summary>
        protected void InternalSort(IComparer sorter)
        {
            if (sorter == null)
            {
                TypeDescriptor.SortDescriptorArray(this);
            }
            else
            {
                Array.Sort(_properties, sorter);
            }
        }

        /// <summary>
        /// Gets an enumerator for this <see cref='System.ComponentModel.PropertyDescriptorCollection'/>.
        /// </summary>
        public virtual IEnumerator GetEnumerator()
        {
            EnsurePropsOwned();
            // we can only return an enumerator on the props we actually have...
            if (_properties.Length != Count)
            {
                PropertyDescriptor[] enumProps = new PropertyDescriptor[Count];
                Array.Copy(_properties, 0, enumProps, 0, Count);
                return enumProps.GetEnumerator();
            }
            return _properties.GetEnumerator();
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        int ICollection.Count => Count;

        void IList.Clear() => Clear();

        void IDictionary.Clear() => Clear();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void IList.RemoveAt(int index) => RemoveAt(index);

        void IDictionary.Add(object key, object value)
        {
            if (!(value is PropertyDescriptor newProp))
            {
                throw new ArgumentException(nameof(value));
            }
            Add(newProp);
        }

        bool IDictionary.Contains(object key)
        {
            if (key is string)
            {
                return this[(string)key] != null;
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => new PropertyDescriptorEnumerator(this);

        bool IDictionary.IsFixedSize => _readOnly;

        bool IDictionary.IsReadOnly => _readOnly;

        object IDictionary.this[object key]
        {
            get
            {
                if (key is string)
                {
                    return this[(string)key];
                }
                return null;
            }

            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException();
                }

                if (value != null && !(value is PropertyDescriptor))
                {
                    throw new ArgumentException(nameof(value));
                }

                int index = -1;

                if (key is int)
                {
                    index = (int)key;

                    if (index < 0 || index >= Count)
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
                else if (key is string)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (_properties[i].Name.Equals((string)key))
                        {
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(nameof(key));
                }

                if (index == -1)
                {
                    Add((PropertyDescriptor)value);
                }
                else
                {
                    EnsurePropsOwned();
                    _properties[index] = (PropertyDescriptor)value;
                    if (_cachedFoundProperties != null && key is string)
                    {
                        _cachedFoundProperties[key] = value;
                    }
                }
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                string[] keys = new string[Count];
                for (int i = 0; i < Count; i++)
                {
                    keys[i] = _properties[i].Name;
                }
                return keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                // We can only return an enumerator on the props we actually have.
                if (_properties.Length != Count)
                {
                    PropertyDescriptor[] newProps = new PropertyDescriptor[Count];
                    Array.Copy(_properties, 0, newProps, 0, Count);
                    return newProps;
                }
                else
                {
                    return (ICollection)_properties.Clone();
                }
            }
        }

        void IDictionary.Remove(object key)
        {
            if (key is string)
            {
                PropertyDescriptor pd = this[(string)key];
                if (pd != null)
                {
                    ((IList)this).Remove(pd);
                }
            }
        }

        int IList.Add(object value) => Add((PropertyDescriptor)value);

        bool IList.Contains(object value) => Contains((PropertyDescriptor)value);

        int IList.IndexOf(object value) => IndexOf((PropertyDescriptor)value);

        void IList.Insert(int index, object value) => Insert(index, (PropertyDescriptor)value);

        bool IList.IsReadOnly => _readOnly;

        bool IList.IsFixedSize => _readOnly;

        void IList.Remove(object value) => Remove((PropertyDescriptor)value);

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException();
                }

                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }


                if (value != null && !(value is PropertyDescriptor))
                {
                    throw new ArgumentException(nameof(value));
                }

                EnsurePropsOwned();
                _properties[index] = (PropertyDescriptor)value;
            }
        }

        private class PropertyDescriptorEnumerator : IDictionaryEnumerator
        {
            private PropertyDescriptorCollection _owner;
            private int _index = -1;

            public PropertyDescriptorEnumerator(PropertyDescriptorCollection owner)
            {
                _owner = owner;
            }

            public object Current => Entry;

            public DictionaryEntry Entry
            {
                get
                {
                    PropertyDescriptor curProp = _owner[_index];
                    return new DictionaryEntry(curProp.Name, curProp);
                }
            }

            public object Key => _owner[_index].Name;

            public object Value => _owner[_index].Name;

            public bool MoveNext()
            {
                if (_index < (_owner.Count - 1))
                {
                    _index++;
                    return true;
                }
                return false;
            }

            public void Reset() => _index = -1;
        }
    }
}
