// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    /*============================================================
    **
    ** Class:  LowLevelDictionary<TKey, TValue>
    **
    ** Private version of Dictionary<> for internal System.Private.CoreLib use. This
    ** permits sharing more source between BCL and System.Private.CoreLib (as well as the
    ** fact that Dictionary<> is just a useful class in general.)
    **
    ** This does not strive to implement the full api surface area
    ** (but any portion it does implement should match the real Dictionary<>'s
    ** behavior.)
    ** 
    ===========================================================*/
#if TYPE_LOADER_IMPLEMENTATION
    [System.Runtime.CompilerServices.ForceDictionaryLookups]
#endif
    internal class LowLevelDictionary<TKey, TValue>
    {
        private const int DefaultSize = 17;

        public LowLevelDictionary()
            : this(DefaultSize, DefaultComparer)
        {
        }

        public LowLevelDictionary(int capacity)
            : this(capacity, DefaultComparer)
        {
        }

        public LowLevelDictionary(IEqualityComparer<TKey> comparer) : this(DefaultSize, comparer)
        {
        }

        public LowLevelDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer;
            Clear(capacity);
        }

        public int Count
        {
            get
            {
                return _numEntries;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                Entry entry = Find(key);
                if (entry == null)
                    throw new KeyNotFoundException();
                return entry.m_value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

#if DEBUG
                _version++;
#endif
                Entry entry = Find(key);
                if (entry != null)
                    entry.m_value = value;
                else
                    UncheckedAdd(key, value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            if (key == null)
                throw new ArgumentNullException("key");
            Entry entry = Find(key);
            if (entry != null)
            {
                value = entry.m_value;
                return true;
            }
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            Entry entry = Find(key);
            if (entry != null)
                throw new ArgumentException(SR.Argument_AddingDuplicate);
#if DEBUG
            _version++;
#endif
            UncheckedAdd(key, value);
        }

        public void Clear(int capacity = DefaultSize)
        {
#if DEBUG
            _version++;
#endif
            _buckets = new Entry[capacity];
            _numEntries = 0;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            int bucket = GetBucket(key);
            Entry prev = null;
            Entry entry = _buckets[bucket];
            while (entry != null)
            {
                if (_comparer.Equals(key, entry.m_key))
                {
                    if (prev == null)
                    {
                        _buckets[bucket] = entry.m_next;
                    }
                    else
                    {
                        prev.m_next = entry.m_next;
                    }
#if DEBUG
                    _version++;
#endif
                    _numEntries--;
                    return true;
                }

                prev = entry;
                entry = entry.m_next;
            }
            return false;
        }

        internal TValue LookupOrAdd(TKey key, TValue value)
        {
            Entry entry = Find(key);
            if (entry != null)
                return entry.m_value;
            UncheckedAdd(key, value);
            return value;
        }

        private Entry Find(TKey key)
        {
            int bucket = GetBucket(key);
            Entry entry = _buckets[bucket];
            while (entry != null)
            {
                if (_comparer.Equals(key, entry.m_key))
                    return entry;

                entry = entry.m_next;
            }
            return null;
        }

        private Entry UncheckedAdd(TKey key, TValue value)
        {
            Entry entry = new Entry();
            entry.m_key = key;
            entry.m_value = value;

            int bucket = GetBucket(key);
            entry.m_next = _buckets[bucket];
            _buckets[bucket] = entry;

            _numEntries++;
            if (_numEntries > (_buckets.Length * 2))
                ExpandBuckets();

            return entry;
        }


        private void ExpandBuckets()
        {
            try
            {
                int newNumBuckets = _buckets.Length * 2 + 1;
                Entry[] newBuckets = new Entry[newNumBuckets];
                for (int i = 0; i < _buckets.Length; i++)
                {
                    Entry entry = _buckets[i];
                    while (entry != null)
                    {
                        Entry nextEntry = entry.m_next;

                        int bucket = GetBucket(entry.m_key, newNumBuckets);
                        entry.m_next = newBuckets[bucket];
                        newBuckets[bucket] = entry;

                        entry = nextEntry;
                    }
                }
                _buckets = newBuckets;
            }
            catch (OutOfMemoryException)
            {
            }
        }

        private int GetBucket(TKey key, int numBuckets = 0)
        {
            int h = _comparer.GetHashCode(key);
            h &= 0x7fffffff;
            return (h % (numBuckets == 0 ? _buckets.Length : numBuckets));
        }


#if TYPE_LOADER_IMPLEMENTATION
        [System.Runtime.CompilerServices.ForceDictionaryLookups]
#endif
        private sealed class Entry
        {
            public TKey m_key;
            public TValue m_value;
            public Entry m_next;
        }

        private Entry[] _buckets;
        private int _numEntries;
#if DEBUG
        private int _version;
#endif
        private IEqualityComparer<TKey> _comparer;

        // This comparator is used if no comparator is supplied. It emulates the behavior of EqualityComparer<T>.Default
        // when T implements IEquatable<T>.
#if TYPE_LOADER_IMPLEMENTATION
        [System.Runtime.CompilerServices.ForceDictionaryLookups]
#endif
        private sealed class DefaultComparerForIEquatable : IEqualityComparer<TKey>
        {
            public bool Equals(TKey x, TKey y)
            {
                return x == null ? y == null : ((IEquatable<TKey>)x).Equals(y);
            }

            public int GetHashCode(TKey obj)
            {
                return obj.GetHashCode();
            }
        }
        // This comparator is used if no comparator is supplied. It emulates the behavior of EqualityComparer<T>.Default
        // when T does not implement IEquatable<T>.
#if TYPE_LOADER_IMPLEMENTATION
        [System.Runtime.CompilerServices.ForceDictionaryLookups]
#endif
        private sealed class DefaultComparerForNotIEquatable : IEqualityComparer<TKey>
        {
            public bool Equals(TKey x, TKey y)
            {
                return x == null ? y == null : x.Equals(y);
            }

            public int GetHashCode(TKey obj)
            {
                return obj.GetHashCode();
            }
        }
        
        private static readonly IEqualityComparer<TKey> DefaultComparer = typeof(IEquatable<TKey>).IsAssignableFrom(typeof(TKey)) ? (IEqualityComparer<TKey>)new DefaultComparerForIEquatable() : new DefaultComparerForNotIEquatable();
        

#if TYPE_LOADER_IMPLEMENTATION
        [System.Runtime.CompilerServices.ForceDictionaryLookups]
#endif
        protected sealed class LowLevelDictEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            public LowLevelDictEnumerator(LowLevelDictionary<TKey, TValue> dict)
            {
#if DEBUG
                _dict = dict;
                _version = _dict._version;
#endif
                Entry[] entries = new Entry[dict._numEntries];
                int dst = 0;
                for (int bucket = 0; bucket < dict._buckets.Length; bucket++)
                {
                    Entry entry = dict._buckets[bucket];
                    while (entry != null)
                    {
                        entries[dst++] = entry;
                        entry = entry.m_next;
                    }
                }
                _entries = entries;
                Reset();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
#if DEBUG
                    if (_version != _dict._version)
                        throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
#endif
                    if (_curPosition == -1 || _curPosition == _entries.Length)
                        throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
                    Entry entry = _entries[_curPosition];
                    return new KeyValuePair<TKey, TValue>(entry.m_key, entry.m_value);
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
#if DEBUG
                if (_version != _dict._version)
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
#endif
                if (_curPosition != _entries.Length)
                    _curPosition++;
                return _curPosition != _entries.Length;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Reset()
            {
#if DEBUG
                if (_version != _dict._version)
                    throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
#endif
                _curPosition = -1;
            }

            private Entry[] _entries;
            private int _curPosition;
#if DEBUG
            private LowLevelDictionary<TKey, TValue> _dict;
            private int _version;
#endif
        }
    }

    /// <summary>
    /// LowLevelDictionary when enumeration is needed
    /// </summary>
    internal sealed class LowLevelDictionaryWithIEnumerable<TKey, TValue> : LowLevelDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new LowLevelDictEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
