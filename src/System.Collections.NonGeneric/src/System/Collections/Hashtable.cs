// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*============================================================
**
** Class:  Hashtable
**
** Purpose: Represents a collection of key/value pairs
**          that are organized based on the hash code
**          of the key.
**
===========================================================*/

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace System.Collections
{
    // The Hashtable class represents a dictionary of associated keys and values
    // with constant lookup time.
    // 
    // Objects used as keys in a hashtable must implement the GetHashCode
    // and Equals methods (or they can rely on the default implementations
    // inherited from Object if key equality is simply reference
    // equality). Furthermore, the GetHashCode and Equals methods of
    // a key object must produce the same results given the same parameters for the
    // entire time the key is present in the hashtable. In practical terms, this
    // means that key objects should be immutable, at least for the time they are
    // used as keys in a hashtable.
    // 
    // When entries are added to a hashtable, they are placed into
    // buckets based on the hashcode of their keys. Subsequent lookups of
    // keys will use the hashcode of the keys to only search a particular bucket,
    // thus substantially reducing the number of key comparisons required to find
    // an entry. A hashtable's maximum load factor, which can be specified
    // when the hashtable is instantiated, determines the maximum ratio of
    // hashtable entries to hashtable buckets. Smaller load factors cause faster
    // average lookup times at the cost of increased memory consumption. The
    // default maximum load factor of 1.0 generally provides the best balance
    // between speed and size. As entries are added to a hashtable, the hashtable's
    // actual load factor increases, and when the actual load factor reaches the
    // maximum load factor value, the number of buckets in the hashtable is
    // automatically increased by approximately a factor of two (to be precise, the
    // number of hashtable buckets is increased to the smallest prime number that
    // is larger than twice the current number of hashtable buckets).
    // 
    // Each object provides their own hash function, accessed by calling
    // GetHashCode().  However, one can write their own object 
    // implementing IEqualityComparer and pass it to a constructor on
    // the Hashtable.  That hash function (and the equals method on the 
    // IEqualityComparer) would be used for all objects in the table.
    //
    [DebuggerTypeProxy(typeof(System.Collections.Hashtable.HashtableDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    public class Hashtable : IDictionary
    {
        /*
          This Hashtable uses double hashing.  There are hashsize buckets in the 
          table, and each bucket can contain 0 or 1 element.  We assign a bit to mark
          whether there's been a collision when we inserted multiple elements
          (ie, an inserted item was hashed at least a second time and we probed 
          this bucket, but it was already in use).  Using the collision bit, we
          can terminate lookups & removes for elements that aren't in the hash
          table more quickly.  We steal the most significant bit from the hash code
          to store the collision bit.

          Our hash function is of the following form:
    
          h(key, n) = h1(key) + n*h2(key)
    
          where n is the number of times we've hit a collided bucket and rehashed
          (on this particular lookup).  Here are our hash functions:
    
          h1(key) = GetHash(key);  // default implementation calls key.GetHashCode();
          h2(key) = 1 + (((h1(key) >> 5) + 1) % (hashsize - 1));
    
          The h1 can return any number.  h2 must return a number between 1 and
          hashsize - 1 that is relatively prime to hashsize (not a problem if 
          hashsize is prime).  (Knuth's Art of Computer Programming, Vol. 3, p. 528-9)
          If this is true, then we are guaranteed to visit every bucket in exactly
          hashsize probes, since the least common multiple of hashsize and h2(key)
          will be hashsize * h2(key).  (This is the first number where adding h2 to
          h1 mod hashsize will be 0 and we will search the same bucket twice).
          
          We previously used a different h2(key, n) that was not constant.  That is a 
          horrifically bad idea, unless you can prove that series will never produce
          any identical numbers that overlap when you mod them by hashsize, for all
          subranges from i to i+hashsize, for all i.  It's not worth investigating,
          since there was no clear benefit from using that hash function, and it was
          broken.
    
          For efficiency reasons, we've implemented this by storing h1 and h2 in a 
          temporary, and setting a variable called seed equal to h1.  We do a probe,
          and if we collided, we simply add h2 to seed each time through the loop.
    
          A good test for h2() is to subclass Hashtable, provide your own implementation
          of GetHash() that returns a constant, then add many items to the hash table.
          Make sure Count equals the number of items you inserted.

          Note that when we remove an item from the hash table, we set the key
          equal to buckets, if there was a collision in this bucket.  Otherwise
          we'd either wipe out the collision bit, or we'd still have an item in
          the hash table.

           -- 
        */

        internal const Int32 HashPrime = 101;
        private const Int32 InitialSize = 3;
        private const String LoadFactorName = "LoadFactor";
        private const String VersionName = "Version";
        private const String ComparerName = "Comparer";
        private const String HashCodeProviderName = "HashCodeProvider";
        private const String HashSizeName = "HashSize";  // Must save buckets.Length
        private const String KeysName = "Keys";
        private const String ValuesName = "Values";
        private const String KeyComparerName = "KeyComparer";

        // Deleted entries have their key set to buckets

        // The hash table data.
        // This cannot be serialized
        private struct bucket
        {
            public Object key;
            public Object val;
            public int hash_coll;   // Store hash code; sign bit means there was a collision.
        }

        private bucket[] _buckets;

        // The total number of entries in the hash table.
        private int _count;

        // The total number of collision bits set in the hashtable
        private int _occupancy;

        private int _loadsize;
        private float _loadFactor;

        private volatile int _version;
        private volatile bool _isWriterInProgress;

        private ICollection _keys;
        private ICollection _values;

        private IEqualityComparer _keycomparer;
        private Object _syncRoot;

        protected IEqualityComparer EqualityComparer
        {
            get
            {
                return _keycomparer;
            }
        }

        // Note: this constructor is a bogus constructor that does nothing
        // and is for use only with SyncHashtable.
        internal Hashtable(bool trash)
        {
        }

        // Constructs a new hashtable. The hashtable is created with an initial
        // capacity of zero and a load factor of 1.0.
        public Hashtable() : this(0, 1.0f)
        {
        }

        // Constructs a new hashtable with the given initial capacity and a load
        // factor of 1.0. The capacity argument serves as an indication of
        // the number of entries the hashtable will contain. When this number (or
        // an approximation) is known, specifying it in the constructor can
        // eliminate a number of resizing operations that would otherwise be
        // performed when elements are added to the hashtable.
        // 
        public Hashtable(int capacity) : this(capacity, 1.0f)
        {
        }

        // Constructs a new hashtable with the given initial capacity and load
        // factor. The capacity argument serves as an indication of the
        // number of entries the hashtable will contain. When this number (or an
        // approximation) is known, specifying it in the constructor can eliminate
        // a number of resizing operations that would otherwise be performed when
        // elements are added to the hashtable. The loadFactor argument
        // indicates the maximum ratio of hashtable entries to hashtable buckets.
        // Smaller load factors cause faster average lookup times at the cost of
        // increased memory consumption. A load factor of 1.0 generally provides
        // the best balance between speed and size.
        // 
        public Hashtable(int capacity, float loadFactor)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (!(loadFactor >= 0.1f && loadFactor <= 1.0f))
                throw new ArgumentOutOfRangeException(nameof(loadFactor), SR.Format(SR.ArgumentOutOfRange_HashtableLoadFactor, .1, 1.0));
            Contract.EndContractBlock();

            // Based on perf work, .72 is the optimal load factor for this table.  
            _loadFactor = 0.72f * loadFactor;

            double rawsize = capacity / _loadFactor;
            if (rawsize > Int32.MaxValue)
                throw new ArgumentException(SR.Arg_HTCapacityOverflow);

            // Avoid awfully small sizes
            int hashsize = (rawsize > InitialSize) ? HashHelpers.GetPrime((int)rawsize) : InitialSize;
            _buckets = new bucket[hashsize];

            _loadsize = (int)(_loadFactor * hashsize);
            _isWriterInProgress = false;
            // Based on the current algorithm, loadsize must be less than hashsize.
            Debug.Assert(_loadsize < hashsize, "Invalid hashtable loadsize!");
        }

        public Hashtable(int capacity, float loadFactor, IEqualityComparer equalityComparer) : this(capacity, loadFactor)
        {
            _keycomparer = equalityComparer;
        }

        public Hashtable(IEqualityComparer equalityComparer) : this(0, 1.0f, equalityComparer)
        {
        }

        public Hashtable(int capacity, IEqualityComparer equalityComparer)
            : this(capacity, 1.0f, equalityComparer)
        {
        }

        // Constructs a new hashtable containing a copy of the entries in the given
        // dictionary. The hashtable is created with a load factor of 1.0.
        // 
        public Hashtable(IDictionary d) : this(d, 1.0f)
        {
        }

        // Constructs a new hashtable containing a copy of the entries in the given
        // dictionary. The hashtable is created with the given load factor.
        // 
        public Hashtable(IDictionary d, float loadFactor)
            : this(d, loadFactor, (IEqualityComparer)null)
        {
        }

        public Hashtable(IDictionary d, IEqualityComparer equalityComparer)
            : this(d, 1.0f, equalityComparer)
        {
        }

        public Hashtable(IDictionary d, float loadFactor, IEqualityComparer equalityComparer)
            : this((d != null ? d.Count : 0), loadFactor, equalityComparer)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d), SR.ArgumentNull_Dictionary);
            Contract.EndContractBlock();

            IDictionaryEnumerator e = d.GetEnumerator();
            while (e.MoveNext()) Add(e.Key, e.Value);
        }

        // ?InitHash? is basically an implementation of classic DoubleHashing (see http://en.wikipedia.org/wiki/Double_hashing)  
        //
        // 1) The only ?correctness? requirement is that the ?increment? used to probe 
        //    a. Be non-zero
        //    b. Be relatively prime to the table size ?hashSize?. (This is needed to insure you probe all entries in the table before you ?wrap? and visit entries already probed)
        // 2) Because we choose table sizes to be primes, we just need to insure that the increment is 0 < incr < hashSize
        //
        // Thus this function would work: Incr = 1 + (seed % (hashSize-1))
        // 
        // While this works well for ?uniformly distributed? keys, in practice, non-uniformity is common. 
        // In particular in practice we can see ?mostly sequential? where you get long clusters of keys that ?pack?. 
        // To avoid bad behavior you want it to be the case that the increment is ?large? even for ?small? values (because small 
        // values tend to happen more in practice). Thus we multiply ?seed? by a number that will make these small values
        // bigger (and not hurt large values). We picked HashPrime (101) because it was prime, and if ?hashSize-1? is not a multiple of HashPrime
        // (enforced in GetPrime), then incr has the potential of being every value from 1 to hashSize-1. The choice was largely arbitrary.
        // 
        // Computes the hash function:  H(key, i) = h1(key) + i*h2(key, hashSize).
        // The out parameter seed is h1(key), while the out parameter 
        // incr is h2(key, hashSize).  Callers of this function should 
        // add incr each time through a loop.
        private uint InitHash(Object key, int hashsize, out uint seed, out uint incr)
        {
            // Hashcode must be positive.  Also, we must not use the sign bit, since
            // that is used for the collision bit.
            uint hashcode = (uint)GetHash(key) & 0x7FFFFFFF;
            seed = (uint)hashcode;
            // Restriction: incr MUST be between 1 and hashsize - 1, inclusive for
            // the modular arithmetic to work correctly.  This guarantees you'll
            // visit every bucket in the table exactly once within hashsize 
            // iterations.  Violate this and it'll cause obscure bugs forever.
            // If you change this calculation for h2(key), update putEntry too!
            incr = (uint)(1 + ((seed * HashPrime) % ((uint)hashsize - 1)));
            return hashcode;
        }

        // Adds an entry with the given key and value to this hashtable. An
        // ArgumentException is thrown if the key is null or if the key is already
        // present in the hashtable.
        // 
        public virtual void Add(Object key, Object value)
        {
            Insert(key, value, true);
        }

        // Removes all entries from this hashtable.
        public virtual void Clear()
        {
            Debug.Assert(!_isWriterInProgress, "Race condition detected in usages of Hashtable - multiple threads appear to be writing to a Hashtable instance simultaneously!  Don't do that - use Hashtable.Synchronized.");

            if (_count == 0 && _occupancy == 0)
                return;

            _isWriterInProgress = true;
            for (int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i].hash_coll = 0;
                _buckets[i].key = null;
                _buckets[i].val = null;
            }

            _count = 0;
            _occupancy = 0;
            UpdateVersion();
            _isWriterInProgress = false;
        }

        // Clone returns a virtually identical copy of this hash table.  This does
        // a shallow copy - the Objects in the table aren't cloned, only the references
        // to those Objects.
        public virtual Object Clone()
        {
            bucket[] lbuckets = _buckets;
            Hashtable ht = new Hashtable(_count, _keycomparer);
            ht._version = _version;
            ht._loadFactor = _loadFactor;
            ht._count = 0;

            int bucket = lbuckets.Length;
            while (bucket > 0)
            {
                bucket--;
                Object keyv = lbuckets[bucket].key;
                if ((keyv != null) && (keyv != lbuckets))
                {
                    ht[keyv] = lbuckets[bucket].val;
                }
            }

            return ht;
        }

        // Checks if this hashtable contains the given key.
        public virtual bool Contains(Object key)
        {
            return ContainsKey(key);
        }

        // Checks if this hashtable contains an entry with the given key.  This is
        // an O(1) operation.
        // 
        public virtual bool ContainsKey(Object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
            }
            Contract.EndContractBlock();

            uint seed;
            uint incr;
            // Take a snapshot of buckets, in case another thread resizes table
            bucket[] lbuckets = _buckets;
            uint hashcode = InitHash(key, lbuckets.Length, out seed, out incr);
            int ntry = 0;

            bucket b;
            int bucketNumber = (int)(seed % (uint)lbuckets.Length);
            do
            {
                b = lbuckets[bucketNumber];
                if (b.key == null)
                {
                    return false;
                }
                if (((b.hash_coll & 0x7FFFFFFF) == hashcode) &&
                    KeyEquals(b.key, key))
                    return true;
                bucketNumber = (int)(((long)bucketNumber + incr) % (uint)lbuckets.Length);
            } while (b.hash_coll < 0 && ++ntry < lbuckets.Length);
            return false;
        }



        // Checks if this hashtable contains an entry with the given value. The
        // values of the entries of the hashtable are compared to the given value
        // using the Object.Equals method. This method performs a linear
        // search and is thus be substantially slower than the ContainsKey
        // method.
        // 
        public virtual bool ContainsValue(Object value)
        {
            if (value == null)
            {
                for (int i = _buckets.Length; --i >= 0;)
                {
                    if (_buckets[i].key != null && _buckets[i].key != _buckets && _buckets[i].val == null)
                        return true;
                }
            }
            else
            {
                for (int i = _buckets.Length; --i >= 0;)
                {
                    Object val = _buckets[i].val;
                    if (val != null && val.Equals(value)) return true;
                }
            }
            return false;
        }

        // Copies the keys of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the KeyCollection class.
        private void CopyKeys(Array array, int arrayIndex)
        {
            Contract.Requires(array != null);
            Contract.Requires(array.Rank == 1);

            bucket[] lbuckets = _buckets;
            for (int i = lbuckets.Length; --i >= 0;)
            {
                Object keyv = lbuckets[i].key;
                if ((keyv != null) && (keyv != _buckets))
                {
                    array.SetValue(keyv, arrayIndex++);
                }
            }
        }

        // Copies the keys of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the KeyCollection class.
        private void CopyEntries(Array array, int arrayIndex)
        {
            Contract.Requires(array != null);
            Contract.Requires(array.Rank == 1);

            bucket[] lbuckets = _buckets;
            for (int i = lbuckets.Length; --i >= 0;)
            {
                Object keyv = lbuckets[i].key;
                if ((keyv != null) && (keyv != _buckets))
                {
                    DictionaryEntry entry = new DictionaryEntry(keyv, lbuckets[i].val);
                    array.SetValue(entry, arrayIndex++);
                }
            }
        }

        // Copies the values in this hash table to an array at
        // a given index.  Note that this only copies values, and not keys.
        public virtual void CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), SR.ArgumentNull_Array);
            if (array.Rank != 1)
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            Contract.EndContractBlock();
            CopyEntries(array, arrayIndex);
        }

        // Copies the values in this Hashtable to an KeyValuePairs array.
        // KeyValuePairs is different from Dictionary Entry in that it has special
        // debugger attributes on its fields.

        internal virtual KeyValuePairs[] ToKeyValuePairsArray()
        {
            KeyValuePairs[] array = new KeyValuePairs[_count];
            int index = 0;
            bucket[] lbuckets = _buckets;
            for (int i = lbuckets.Length; --i >= 0;)
            {
                Object keyv = lbuckets[i].key;
                if ((keyv != null) && (keyv != _buckets))
                {
                    array[index++] = new KeyValuePairs(keyv, lbuckets[i].val);
                }
            }

            return array;
        }


        // Copies the values of this hashtable to a given array starting at a given
        // index. This method is used by the implementation of the CopyTo method in
        // the ValueCollection class.
        private void CopyValues(Array array, int arrayIndex)
        {
            Contract.Requires(array != null);
            Contract.Requires(array.Rank == 1);

            bucket[] lbuckets = _buckets;
            for (int i = lbuckets.Length; --i >= 0;)
            {
                Object keyv = lbuckets[i].key;
                if ((keyv != null) && (keyv != _buckets))
                {
                    array.SetValue(lbuckets[i].val, arrayIndex++);
                }
            }
        }

        // Returns the value associated with the given key. If an entry with the
        // given key is not found, the returned value is null.
        // 
        public virtual Object this[Object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
                }
                Contract.EndContractBlock();

                uint seed;
                uint incr;


                // Take a snapshot of buckets, in case another thread does a resize
                bucket[] lbuckets = _buckets;
                uint hashcode = InitHash(key, lbuckets.Length, out seed, out incr);
                int ntry = 0;

                bucket b;
                int bucketNumber = (int)(seed % (uint)lbuckets.Length);
                do
                {
                    int currentversion;

                    //     A read operation on hashtable has three steps:
                    //        (1) calculate the hash and find the slot number.
                    //        (2) compare the hashcode, if equal, go to step 3. Otherwise end.
                    //        (3) compare the key, if equal, go to step 4. Otherwise end.
                    //        (4) return the value contained in the bucket.
                    //     After step 3 and before step 4. A writer can kick in a remove the old item and add a new one 
                    //     in the same bucket. So in the reader we need to check if the hash table is modified during above steps.
                    //
                    // Writers (Insert, Remove, Clear) will set 'isWriterInProgress' flag before it starts modifying 
                    // the hashtable and will ckear the flag when it is done.  When the flag is cleared, the 'version'
                    // will be increased.  We will repeat the reading if a writer is in progress or done with the modification 
                    // during the read.
                    //
                    // Our memory model guarantee if we pick up the change in bucket from another processor, 
                    // we will see the 'isWriterProgress' flag to be true or 'version' is changed in the reader.
                    //                    
                    SpinWait spin = new SpinWait();
                    while (true)
                    {
                        // this is volatile read, following memory accesses can not be moved ahead of it.
                        currentversion = _version;
                        b = lbuckets[bucketNumber];

                        if (!_isWriterInProgress && (currentversion == _version))
                            break;

                        spin.SpinOnce();
                    }

                    if (b.key == null)
                    {
                        return null;
                    }
                    if (((b.hash_coll & 0x7FFFFFFF) == hashcode) &&
                        KeyEquals(b.key, key))
                        return b.val;
                    bucketNumber = (int)(((long)bucketNumber + incr) % (uint)lbuckets.Length);
                } while (b.hash_coll < 0 && ++ntry < lbuckets.Length);
                return null;
            }

            set
            {
                Insert(key, value, false);
            }
        }

        // Increases the bucket count of this hashtable. This method is called from
        // the Insert method when the actual load factor of the hashtable reaches
        // the upper limit specified when the hashtable was constructed. The number
        // of buckets in the hashtable is increased to the smallest prime number
        // that is larger than twice the current number of buckets, and the entries
        // in the hashtable are redistributed into the new buckets using the cached
        // hashcodes.
        private void expand()
        {
            int rawsize = HashHelpers.ExpandPrime(_buckets.Length);
            rehash(rawsize, false);
        }

        // We occasionally need to rehash the table to clean up the collision bits.
        private void rehash()
        {
            rehash(_buckets.Length, false);
        }

        private void UpdateVersion()
        {
            // Version might become negative when version is Int32.MaxValue, but the oddity will be still be correct. 
            // So we don't need to special case this. 
            _version++;
        }

        private void rehash(int newsize, bool forceNewHashCode)
        {
            // reset occupancy
            _occupancy = 0;

            // Don't replace any internal state until we've finished adding to the 
            // new bucket[].  This serves two purposes: 
            //   1) Allow concurrent readers to see valid hashtable contents 
            //      at all times
            //   2) Protect against an OutOfMemoryException while allocating this 
            //      new bucket[].
            bucket[] newBuckets = new bucket[newsize];

            // rehash table into new buckets
            int nb;
            for (nb = 0; nb < _buckets.Length; nb++)
            {
                bucket oldb = _buckets[nb];
                if ((oldb.key != null) && (oldb.key != _buckets))
                {
                    int hashcode = ((forceNewHashCode ? GetHash(oldb.key) : oldb.hash_coll) & 0x7FFFFFFF);
                    putEntry(newBuckets, oldb.key, oldb.val, hashcode);
                }
            }

            // New bucket[] is good to go - replace buckets and other internal state.
            _isWriterInProgress = true;
            _buckets = newBuckets;
            _loadsize = (int)(_loadFactor * newsize);
            UpdateVersion();
            _isWriterInProgress = false;
            // minimum size of hashtable is 3 now and maximum loadFactor is 0.72 now.
            Debug.Assert(_loadsize < newsize, "Our current implementation means this is not possible.");
            return;
        }

        // Returns an enumerator for this hashtable.
        // If modifications made to the hashtable while an enumeration is
        // in progress, the MoveNext and Current methods of the
        // enumerator will throw an exception.
        //
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new HashtableEnumerator(this, HashtableEnumerator.DictEntry);
        }

        // Returns a dictionary enumerator for this hashtable.
        // If modifications made to the hashtable while an enumeration is
        // in progress, the MoveNext and Current methods of the
        // enumerator will throw an exception.
        //
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            return new HashtableEnumerator(this, HashtableEnumerator.DictEntry);
        }

        // Internal method to get the hash code for an Object.  This will call
        // GetHashCode() on each object if you haven't provided an IHashCodeProvider
        // instance.  Otherwise, it calls hcp.GetHashCode(obj).
        protected virtual int GetHash(Object key)
        {
            if (_keycomparer != null)
                return _keycomparer.GetHashCode(key);
            return key.GetHashCode();
        }

        // Is this Hashtable read-only?
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        // Is this Hashtable synchronized?  See SyncRoot property
        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        // Internal method to compare two keys.  If you have provided an IComparer
        // instance in the constructor, this method will call comparer.Compare(item, key).
        // Otherwise, it will call item.Equals(key).
        // 
        protected virtual bool KeyEquals(Object item, Object key)
        {
            Debug.Assert(key != null, "key can't be null here!");
            if (Object.ReferenceEquals(_buckets, item))
            {
                return false;
            }

            if (Object.ReferenceEquals(item, key))
                return true;

            if (_keycomparer != null)
                return _keycomparer.Equals(item, key);
            return item == null ? false : item.Equals(key);
        }

        // Returns a collection representing the keys of this hashtable. The order
        // in which the returned collection represents the keys is unspecified, but
        // it is guaranteed to be          buckets = newBuckets; the same order in which a collection returned by
        // GetValues represents the values of the hashtable.
        // 
        // The returned collection is live in the sense that any changes
        // to the hash table are reflected in this collection.  It is not
        // a static copy of all the keys in the hash table.
        // 
        public virtual ICollection Keys
        {
            get
            {
                if (_keys == null) _keys = new KeyCollection(this);
                return _keys;
            }
        }

        // Returns a collection representing the values of this hashtable. The
        // order in which the returned collection represents the values is
        // unspecified, but it is guaranteed to be the same order in which a
        // collection returned by GetKeys represents the keys of the
        // hashtable.
        // 
        // The returned collection is live in the sense that any changes
        // to the hash table are reflected in this collection.  It is not
        // a static copy of all the keys in the hash table.
        // 
        public virtual ICollection Values
        {
            get
            {
                if (_values == null) _values = new ValueCollection(this);
                return _values;
            }
        }

        // Inserts an entry into this hashtable. This method is called from the Set
        // and Add methods. If the add parameter is true and the given key already
        // exists in the hashtable, an exception is thrown.
        private void Insert(Object key, Object nvalue, bool add)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
            }
            Contract.EndContractBlock();
            if (_count >= _loadsize)
            {
                expand();
            }
            else if (_occupancy > _loadsize && _count > 100)
            {
                rehash();
            }

            uint seed;
            uint incr;
            // Assume we only have one thread writing concurrently.  Modify
            // buckets to contain new data, as long as we insert in the right order.
            uint hashcode = InitHash(key, _buckets.Length, out seed, out incr);
            int ntry = 0;
            int emptySlotNumber = -1; // We use the empty slot number to cache the first empty slot. We chose to reuse slots
            // create by remove that have the collision bit set over using up new slots.
            int bucketNumber = (int)(seed % (uint)_buckets.Length);
            do
            {
                // Set emptySlot number to current bucket if it is the first available bucket that we have seen
                // that once contained an entry and also has had a collision.
                // We need to search this entire collision chain because we have to ensure that there are no 
                // duplicate entries in the table.
                if (emptySlotNumber == -1 && (_buckets[bucketNumber].key == _buckets) && (_buckets[bucketNumber].hash_coll < 0))//(((buckets[bucketNumber].hash_coll & unchecked(0x80000000))!=0)))
                    emptySlotNumber = bucketNumber;

                // Insert the key/value pair into this bucket if this bucket is empty and has never contained an entry
                // OR
                // This bucket once contained an entry but there has never been a collision
                if ((_buckets[bucketNumber].key == null) ||
                    (_buckets[bucketNumber].key == _buckets && ((_buckets[bucketNumber].hash_coll & unchecked(0x80000000)) == 0)))
                {
                    // If we have found an available bucket that has never had a collision, but we've seen an available
                    // bucket in the past that has the collision bit set, use the previous bucket instead
                    if (emptySlotNumber != -1) // Reuse slot
                        bucketNumber = emptySlotNumber;

                    // We pretty much have to insert in this order.  Don't set hash
                    // code until the value & key are set appropriately.
                    _isWriterInProgress = true;
                    _buckets[bucketNumber].val = nvalue;
                    _buckets[bucketNumber].key = key;
                    _buckets[bucketNumber].hash_coll |= (int)hashcode;
                    _count++;
                    UpdateVersion();
                    _isWriterInProgress = false;
#if FEATURE_RANDOMIZED_STRING_HASHING
                    if (ntry > HashHelpers.HashCollisionThreshold && HashHelpers.IsWellKnownEqualityComparer(_keycomparer))
                    {
                        // PERF: We don't want to rehash if _keycomparer is already a RandomizedObjectEqualityComparer since in some
                        // cases there may not be any strings in the hashtable and we wouldn't get any mixing.
                        if (_keycomparer == null || !(_keycomparer is System.Collections.Generic.RandomizedObjectEqualityComparer))
                        {
                            _keycomparer = HashHelpers.GetRandomizedEqualityComparer(_keycomparer);
                            rehash(buckets.Length, true);
                        }
                    }
#endif
                    return;
                }

                // The current bucket is in use
                // OR
                // it is available, but has had the collision bit set and we have already found an available bucket
                if (((_buckets[bucketNumber].hash_coll & 0x7FFFFFFF) == hashcode) &&
                    KeyEquals(_buckets[bucketNumber].key, key))
                {
                    if (add)
                    {
                        throw new ArgumentException(SR.Format(SR.Argument_AddingDuplicate__, _buckets[bucketNumber].key, key));
                    }
                    _isWriterInProgress = true;
                    _buckets[bucketNumber].val = nvalue;
                    UpdateVersion();
                    _isWriterInProgress = false;

#if FEATURE_RANDOMIZED_STRING_HASHING
                    if (ntry > HashHelpers.HashCollisionThreshold && HashHelpers.IsWellKnownEqualityComparer(_keycomparer))
                    {
                        // PERF: We don't want to rehash if _keycomparer is already a RandomizedObjectEqualityComparer since in some
                        // cases there may not be any strings in the hashtable and we wouldn't get any mixing.
                        if (_keycomparer == null || !(_keycomparer is System.Collections.Generic.RandomizedObjectEqualityComparer))
                        {
                            _keycomparer = HashHelpers.GetRandomizedEqualityComparer(_keycomparer);
                            rehash(buckets.Length, true);
                        }
                    }
#endif
                    return;
                }

                // The current bucket is full, and we have therefore collided.  We need to set the collision bit
                // unless we have remembered an available slot previously.
                if (emptySlotNumber == -1)
                {// We don't need to set the collision bit here since we already have an empty slot
                    if (_buckets[bucketNumber].hash_coll >= 0)
                    {
                        _buckets[bucketNumber].hash_coll |= unchecked((int)0x80000000);
                        _occupancy++;
                    }
                }

                bucketNumber = (int)(((long)bucketNumber + incr) % (uint)_buckets.Length);
            } while (++ntry < _buckets.Length);

            // This code is here if and only if there were no buckets without a collision bit set in the entire table
            if (emptySlotNumber != -1)
            {
                // We pretty much have to insert in this order.  Don't set hash
                // code until the value & key are set appropriately.
                _isWriterInProgress = true;
                _buckets[emptySlotNumber].val = nvalue;
                _buckets[emptySlotNumber].key = key;
                _buckets[emptySlotNumber].hash_coll |= (int)hashcode;
                _count++;
                UpdateVersion();
                _isWriterInProgress = false;

#if FEATURE_RANDOMIZED_STRING_HASHING
                if (buckets.Length > HashHelpers.HashCollisionThreshold && HashHelpers.IsWellKnownEqualityComparer(_keycomparer))
                {
                    // PERF: We don't want to rehash if _keycomparer is already a RandomizedObjectEqualityComparer since in some
                    // cases there may not be any strings in the hashtable and we wouldn't get any mixing.
                    if (_keycomparer == null || !(_keycomparer is System.Collections.Generic.RandomizedObjectEqualityComparer))
                    {
                        _keycomparer = HashHelpers.GetRandomizedEqualityComparer(_keycomparer);
                        rehash(buckets.Length, true);
                    }
                }
#endif
                return;
            }

            // If you see this assert, make sure load factor & count are reasonable.
            // Then verify that our double hash function (h2, described at top of file)
            // meets the requirements described above. You should never see this assert.
            Debug.Fail("hash table insert failed!  Load factor too high, or our double hashing function is incorrect.");
            throw new InvalidOperationException(SR.InvalidOperation_HashInsertFailed);
        }

        private void putEntry(bucket[] newBuckets, Object key, Object nvalue, int hashcode)
        {
            Debug.Assert(hashcode >= 0, "hashcode >= 0");  // make sure collision bit (sign bit) wasn't set.

            uint seed = (uint)hashcode;
            uint incr = (uint)(1 + ((seed * HashPrime) % ((uint)newBuckets.Length - 1)));
            int bucketNumber = (int)(seed % (uint)newBuckets.Length);
            do
            {
                if ((newBuckets[bucketNumber].key == null) || (newBuckets[bucketNumber].key == _buckets))
                {
                    newBuckets[bucketNumber].val = nvalue;
                    newBuckets[bucketNumber].key = key;
                    newBuckets[bucketNumber].hash_coll |= hashcode;
                    return;
                }

                if (newBuckets[bucketNumber].hash_coll >= 0)
                {
                    newBuckets[bucketNumber].hash_coll |= unchecked((int)0x80000000);
                    _occupancy++;
                }
                bucketNumber = (int)(((long)bucketNumber + incr) % (uint)newBuckets.Length);
            } while (true);
        }

        // Removes an entry from this hashtable. If an entry with the specified
        // key exists in the hashtable, it is removed. An ArgumentException is
        // thrown if the key is null.
        // 
        public virtual void Remove(Object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
            }
            Contract.EndContractBlock();
            Debug.Assert(!_isWriterInProgress, "Race condition detected in usages of Hashtable - multiple threads appear to be writing to a Hashtable instance simultaneously!  Don't do that - use Hashtable.Synchronized.");

            uint seed;
            uint incr;
            // Assuming only one concurrent writer, write directly into buckets.
            uint hashcode = InitHash(key, _buckets.Length, out seed, out incr);
            int ntry = 0;

            bucket b;
            int bn = (int)(seed % (uint)_buckets.Length);  // bucketNumber
            do
            {
                b = _buckets[bn];
                if (((b.hash_coll & 0x7FFFFFFF) == hashcode) &&
                    KeyEquals(b.key, key))
                {
                    _isWriterInProgress = true;
                    // Clear hash_coll field, then key, then value
                    _buckets[bn].hash_coll &= unchecked((int)0x80000000);
                    if (_buckets[bn].hash_coll != 0)
                    {
                        _buckets[bn].key = _buckets;
                    }
                    else
                    {
                        _buckets[bn].key = null;
                    }
                    _buckets[bn].val = null;  // Free object references sooner & simplify ContainsValue.
                    _count--;
                    UpdateVersion();
                    _isWriterInProgress = false;
                    return;
                }
                bn = (int)(((long)bn + incr) % (uint)_buckets.Length);
            } while (b.hash_coll < 0 && ++ntry < _buckets.Length);
        }

        // Returns the object to synchronize on for this hash table.
        public virtual Object SyncRoot
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

        // Returns the number of associations in this hashtable.
        // 
        public virtual int Count
        {
            get { return _count; }
        }

        // Returns a thread-safe wrapper for a Hashtable.
        //
        public static Hashtable Synchronized(Hashtable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            Contract.EndContractBlock();
            return new SyncHashtable(table);
        }

        // Implements a Collection for the keys of a hashtable. An instance of this
        // class is created by the GetKeys method of a hashtable.
        private class KeyCollection : ICollection
        {
            private Hashtable _hashtable;

            internal KeyCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public virtual void CopyTo(Array array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
                Contract.EndContractBlock();
                if (array.Length - arrayIndex < _hashtable._count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                _hashtable.CopyKeys(array, arrayIndex);
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new HashtableEnumerator(_hashtable, HashtableEnumerator.Keys);
            }

            public virtual bool IsSynchronized
            {
                get { return _hashtable.IsSynchronized; }
            }

            public virtual Object SyncRoot
            {
                get { return _hashtable.SyncRoot; }
            }

            public virtual int Count
            {
                get { return _hashtable._count; }
            }
        }

        // Implements a Collection for the values of a hashtable. An instance of
        // this class is created by the GetValues method of a hashtable.
        private class ValueCollection : ICollection
        {
            private Hashtable _hashtable;

            internal ValueCollection(Hashtable hashtable)
            {
                _hashtable = hashtable;
            }

            public virtual void CopyTo(Array array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (array.Rank != 1)
                    throw new ArgumentException(SR.Arg_RankMultiDimNotSupported);
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), SR.ArgumentOutOfRange_NeedNonNegNum);
                Contract.EndContractBlock();
                if (array.Length - arrayIndex < _hashtable._count)
                    throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
                _hashtable.CopyValues(array, arrayIndex);
            }

            public virtual IEnumerator GetEnumerator()
            {
                return new HashtableEnumerator(_hashtable, HashtableEnumerator.Values);
            }

            public virtual bool IsSynchronized
            {
                get { return _hashtable.IsSynchronized; }
            }

            public virtual Object SyncRoot
            {
                get { return _hashtable.SyncRoot; }
            }

            public virtual int Count
            {
                get { return _hashtable._count; }
            }
        }

        // Synchronized wrapper for hashtable
        private class SyncHashtable : Hashtable, IEnumerable
        {
            protected Hashtable _table;

            internal SyncHashtable(Hashtable table) : base(false)
            {
                _table = table;
            }

            public override int Count
            {
                get { return _table.Count; }
            }

            public override bool IsReadOnly
            {
                get { return _table.IsReadOnly; }
            }

            public override bool IsFixedSize
            {
                get { return _table.IsFixedSize; }
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override Object this[Object key]
            {
                get
                {
                    return _table[key];
                }
                set
                {
                    lock (_table.SyncRoot)
                    {
                        _table[key] = value;
                    }
                }
            }

            public override Object SyncRoot
            {
                get { return _table.SyncRoot; }
            }

            public override void Add(Object key, Object value)
            {
                lock (_table.SyncRoot)
                {
                    _table.Add(key, value);
                }
            }

            public override void Clear()
            {
                lock (_table.SyncRoot)
                {
                    _table.Clear();
                }
            }

            public override bool Contains(Object key)
            {
                return _table.Contains(key);
            }

            public override bool ContainsKey(Object key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), SR.ArgumentNull_Key);
                }
                Contract.EndContractBlock();
                return _table.ContainsKey(key);
            }

            public override bool ContainsValue(Object key)
            {
                lock (_table.SyncRoot)
                {
                    return _table.ContainsValue(key);
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_table.SyncRoot)
                {
                    _table.CopyTo(array, arrayIndex);
                }
            }

            public override Object Clone()
            {
                lock (_table.SyncRoot)
                {
                    return Hashtable.Synchronized((Hashtable)_table.Clone());
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _table.GetEnumerator();
            }

            public override IDictionaryEnumerator GetEnumerator()
            {
                return _table.GetEnumerator();
            }

            public override ICollection Keys
            {
                get
                {
                    lock (_table.SyncRoot)
                    {
                        return _table.Keys;
                    }
                }
            }

            public override ICollection Values
            {
                get
                {
                    lock (_table.SyncRoot)
                    {
                        return _table.Values;
                    }
                }
            }

            public override void Remove(Object key)
            {
                lock (_table.SyncRoot)
                {
                    _table.Remove(key);
                }
            }

            internal override KeyValuePairs[] ToKeyValuePairsArray()
            {
                return _table.ToKeyValuePairsArray();
            }
        }


        // Implements an enumerator for a hashtable. The enumerator uses the
        // internal version number of the hashtable to ensure that no modifications
        // are made to the hashtable while an enumeration is in progress.
        private class HashtableEnumerator : IDictionaryEnumerator
        {
            private Hashtable _hashtable;
            private int _bucket;
            private int _version;
            private bool _current;
            private int _getObjectRetType;   // What should GetObject return?
            private Object _currentKey;
            private Object _currentValue;

            internal const int Keys = 1;
            internal const int Values = 2;
            internal const int DictEntry = 3;

            internal HashtableEnumerator(Hashtable hashtable, int getObjRetType)
            {
                _hashtable = hashtable;
                _bucket = hashtable._buckets.Length;
                _version = hashtable._version;
                _current = false;
                _getObjectRetType = getObjRetType;
            }

            public virtual Object Key
            {
                get
                {
                    if (_current == false) throw new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
                    return _currentKey;
                }
            }

            public virtual bool MoveNext()
            {
                if (_version != _hashtable._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                while (_bucket > 0)
                {
                    _bucket--;
                    Object keyv = _hashtable._buckets[_bucket].key;
                    if ((keyv != null) && (keyv != _hashtable._buckets))
                    {
                        _currentKey = keyv;
                        _currentValue = _hashtable._buckets[_bucket].val;
                        _current = true;
                        return true;
                    }
                }
                _current = false;
                return false;
            }

            public virtual DictionaryEntry Entry
            {
                get
                {
                    if (_current == false) throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    return new DictionaryEntry(_currentKey, _currentValue);
                }
            }


            public virtual Object Current
            {
                get
                {
                    if (_current == false) throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);

                    if (_getObjectRetType == Keys)
                        return _currentKey;
                    else if (_getObjectRetType == Values)
                        return _currentValue;
                    else
                        return new DictionaryEntry(_currentKey, _currentValue);
                }
            }

            public virtual Object Value
            {
                get
                {
                    if (_current == false) throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    return _currentValue;
                }
            }

            public virtual void Reset()
            {
                if (_version != _hashtable._version) throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                _current = false;
                _bucket = _hashtable._buckets.Length;
                _currentKey = null;
                _currentValue = null;
            }
        }

        // internal debug view class for hashtable
        internal class HashtableDebugView
        {
            private Hashtable _hashtable;

            public HashtableDebugView(Hashtable hashtable)
            {
                if (hashtable == null)
                {
                    throw new ArgumentNullException(nameof(hashtable));
                }
                Contract.EndContractBlock();

                _hashtable = hashtable;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePairs[] Items
            {
                get
                {
                    return _hashtable.ToKeyValuePairsArray();
                }
            }
        }
    }

    internal static class HashHelpers
    {
#if FEATURE_RANDOMIZED_STRING_HASHING
        public const int HashCollisionThreshold = 100;
        public static bool s_UseRandomizedStringHashing = String.UseRandomizedHashing();
#endif
        // Table of prime numbers to use as hash table sizes. 
        // A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity. 
        // Suppose our Hashtable currently has capacity x and enough elements are added 
        // such that a resize needs to occur. Resizing first computes 2x then finds the 
        // first prime in the table greater than 2x, i.e. if primes are ordered 
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
        // Doubling is important for preserving the asymptotic complexity of the 
        // hashtable operations such as add.  Having a prime guarantees that double 
        // hashing does not lead to infinite loops.  IE, your hash function will be 
        // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
        public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }
                return true;
            }
            return (candidate == 2);
        }

        public static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException(SR.Arg_HTCapacityOverflow);
            Contract.EndContractBlock();

            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min) return prime;
            }

            //outside of our predefined table. 
            //compute the hard way. 
            for (int i = (min | 1); i < Int32.MaxValue; i += 2)
            {
                if (IsPrime(i) && ((i - 1) % Hashtable.HashPrime != 0))
                    return i;
            }
            return min;
        }

        // Returns size of hashtable to grow to.
        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encountering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                Debug.Assert(MaxPrimeArrayLength == GetPrime(MaxPrimeArrayLength), "Invalid MaxPrimeArrayLength");
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }


        // This is the maximum prime smaller than Array.MaxArrayLength
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;

#if FEATURE_RANDOMIZED_STRING_HASHING
        public static bool IsWellKnownEqualityComparer(object comparer)
        {
            return (comparer == null || comparer == System.Collections.Generic.EqualityComparer<string>.Default || comparer is IWellKnownStringEqualityComparer);
        }

        public static IEqualityComparer GetRandomizedEqualityComparer(object comparer)
        {
            Debug.Assert(comparer == null || comparer == System.Collections.Generic.EqualityComparer<string>.Default || comparer is IWellKnownStringEqualityComparer);

            if (comparer == null)
            {
                return new System.Collections.Generic.RandomizedObjectEqualityComparer();
            }

            if (comparer == System.Collections.Generic.EqualityComparer<string>.Default)
            {
                return new System.Collections.Generic.RandomizedStringEqualityComparer();
            }

            IWellKnownStringEqualityComparer cmp = comparer as IWellKnownStringEqualityComparer;

            if (cmp != null)
            {
                return cmp.GetRandomizedEqualityComparer();
            }

            Debug.Fail("Missing case in GetRandomizedEqualityComparer!");

            return null;
        }

        public static object GetEqualityComparerForSerialization(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }

            IWellKnownStringEqualityComparer cmp = comparer as IWellKnownStringEqualityComparer;

            if (cmp != null)
            {
                return cmp.GetEqualityComparerForSerialization();
            }

            return comparer;
        }

        private const int bufferSize = 1024;
        private static RandomNumberGenerator rng;
        private static byte[] data;
        private static int currentIndex = bufferSize;
        private static readonly object lockObj = new Object();

        internal static long GetEntropy()
        {
            lock (lockObj)
            {
                long ret;

                if (currentIndex == bufferSize)
                {
                    if (null == rng)
                    {
                        rng = RandomNumberGenerator.Create();
                        data = new byte[bufferSize];
                        Debug.Assert(bufferSize % 8 == 0, "We increment our current index by 8, so our buffer size must be a multiple of 8");
                    }

                    rng.GetBytes(data);
                    currentIndex = 0;
                }

                ret = BitConverter.ToInt64(data, currentIndex);
                currentIndex += 8;

                return ret;
            }
        }
#endif // FEATURE_RANDOMIZED_STRING_HASHING
    }
}
