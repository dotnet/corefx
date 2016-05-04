// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Collections.Specialized
{
    /// <summary>
    /// Provides a simple structure that stores Boolean values and small integers in 32 bits of memory.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public partial struct BitVector32
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitVector32" />
        /// structure containing the data represented in an existing <see cref="BitVector32" />
        /// structure.
        /// </summary>
        /// <param name="value">
        /// A <see cref="BitVector32" /> structure that contains the
        /// data to copy.
        /// </param>
        public BitVector32(System.Collections.Specialized.BitVector32 value) { throw new System.NotImplementedException(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitVector32" />
        /// structure containing the data represented in an integer.
        /// </summary>
        /// <param name="data">
        /// An integer representing the data of the new <see cref="BitVector32" />.
        /// </param>
        public BitVector32(int data) { throw new System.NotImplementedException(); }
        /// <summary>
        /// Gets the value of the <see cref="BitVector32" /> as an integer.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="BitVector32" /> as an integer.
        /// </returns>
        public int Data { get { return default(int); } }
        /// <summary>
        /// Gets or sets the value stored in the specified
        /// <see cref="Section" />.
        /// </summary>
        /// <param name="section">
        /// A <see cref="Section" /> that contains the value
        /// to get or set.
        /// </param>
        /// <returns>
        /// The value stored in the specified <see cref="Section" />.
        /// </returns>
        public int this[System.Collections.Specialized.BitVector32.Section section] { get { return default(int); } set { } }
        /// <summary>
        /// Gets or sets the state of the bit flag indicated by the specified mask.
        /// </summary>
        /// <param name="bit">A mask that indicates the bit to get or set.</param>
        /// <returns>
        /// true if the specified bit flag is on (1); otherwise, false.
        /// </returns>
        public bool this[int bit] { get { return default(bool); } set { } }
        /// <summary>
        /// Creates the first mask in a series of masks that can be used to retrieve individual bits in
        /// a <see cref="BitVector32" /> that is set up as bit flags.
        /// </summary>
        /// <returns>
        /// A mask that isolates the first bit flag in the <see cref="BitVector32" />.
        /// </returns>
        public static int CreateMask() { return default(int); }
        /// <summary>
        /// Creates an additional mask following the specified mask in a series of masks that can be used
        /// to retrieve individual bits in a <see cref="BitVector32" />
        /// that is set up as bit flags.
        /// </summary>
        /// <param name="previous">The mask that indicates the previous bit flag.</param>
        /// <returns>
        /// A mask that isolates the bit flag following the one that <paramref name="previous" /> points
        /// to in <see cref="BitVector32" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="previous" /> indicates the last bit flag in the
        /// <see cref="BitVector32" />.
        /// </exception>
        public static int CreateMask(int previous) { return default(int); }
        /// <summary>
        /// Creates the first <see cref="Section" /> in a
        /// series of sections that contain small integers.
        /// </summary>
        /// <param name="maxValue">
        /// A 16-bit signed integer that specifies the maximum value for the new
        /// <see cref="Section" />.
        /// </param>
        /// <returns>
        /// A <see cref="Section" /> that can hold a number
        /// from zero to <paramref name="maxValue" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="maxValue" /> is less than 1.</exception>
        public static System.Collections.Specialized.BitVector32.Section CreateSection(short maxValue) { return default(System.Collections.Specialized.BitVector32.Section); }
        /// <summary>
        /// Creates a new <see cref="Section" /> following
        /// the specified <see cref="Section" /> in a series
        /// of sections that contain small integers.
        /// </summary>
        /// <param name="maxValue">
        /// A 16-bit signed integer that specifies the maximum value for the new
        /// <see cref="Section" />.
        /// </param>
        /// <param name="previous">
        /// The previous <see cref="Section" /> in the
        /// <see cref="BitVector32" />.
        /// </param>
        /// <returns>
        /// A <see cref="Section" /> that can hold a number
        /// from zero to <paramref name="maxValue" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="maxValue" /> is less than 1.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="previous" /> includes the final bit in the
        /// <see cref="BitVector32" />.-or- <paramref name="maxValue" /> is greater than the highest value that can be represented
        /// by the number of bits after <paramref name="previous" />.
        /// </exception>
        public static System.Collections.Specialized.BitVector32.Section CreateSection(short maxValue, System.Collections.Specialized.BitVector32.Section previous) { return default(System.Collections.Specialized.BitVector32.Section); }
        /// <summary>
        /// Determines whether the specified object is equal to the
        /// <see cref="BitVector32" />.
        /// </summary>
        /// <param name="o">
        /// The object to compare with the current <see cref="BitVector32" />.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the <see cref="BitVector32" />
        /// ; otherwise, false.
        /// </returns>
        public override bool Equals(object o) { return default(bool); }
        /// <summary>
        /// Serves as a hash function for the <see cref="BitVector32" />.
        /// </summary>
        /// <returns>
        /// A hash code for the <see cref="BitVector32" />.
        /// </returns>
        public override int GetHashCode() { return default(int); }
        /// <summary>
        /// Returns a string that represents the current <see cref="BitVector32" />.
        /// </summary>
        /// <returns>
        /// A string that represents the current <see cref="BitVector32" />.
        /// </returns>
        public override string ToString() { return default(string); }
        /// <summary>
        /// Returns a string that represents the specified <see cref="BitVector32" />.
        /// </summary>
        /// <param name="value">The <see cref="BitVector32" /> to represent.</param>
        /// <returns>
        /// A string that represents the specified <see cref="BitVector32" />.
        /// </returns>
        public static string ToString(System.Collections.Specialized.BitVector32 value) { return default(string); }
        /// <summary>
        /// Represents a section of the vector that can contain an integer number.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Section
        {
            /// <summary>
            /// Gets a mask that isolates this section within the <see cref="BitVector32" />.
            /// </summary>
            /// <returns>
            /// A mask that isolates this section within the <see cref="BitVector32" />.
            /// </returns>
            public short Mask { get { return default(short); } }
            /// <summary>
            /// Gets the offset of this section from the start of the
            /// <see cref="BitVector32" />.
            /// </summary>
            /// <returns>
            /// The offset of this section from the start of the <see cref="BitVector32" />.
            /// </returns>
            public short Offset { get { return default(short); } }
            /// <summary>
            /// Determines whether the specified <see cref="Section" />
            /// object is the same as the current <see cref="Section" />
            /// object.
            /// </summary>
            /// <param name="obj">
            /// The <see cref="Section" /> object to compare
            /// with the current <see cref="Section" /> object.
            /// </param>
            /// <returns>
            /// true if the <paramref name="obj" /> parameter is the same as the current
            /// <see cref="Section" /> object; otherwise false.
            /// </returns>
            public bool Equals(System.Collections.Specialized.BitVector32.Section obj) { return default(bool); }
            /// <summary>
            /// Determines whether the specified object is the same as the current
            /// <see cref="Section" /> object.
            /// </summary>
            /// <param name="o">
            /// The object to compare with the current <see cref="Section" />.
            /// </param>
            /// <returns>
            /// true if the specified object is the same as the current
            /// <see cref="Section" /> object; otherwise, false.
            /// </returns>
            public override bool Equals(object o) { return default(bool); }
            /// <summary>
            /// Serves as a hash function for the current <see cref="Section" />,
            /// suitable for hashing algorithms and data structures, such as a hash table.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="Section" />.
            /// </returns>
            public override int GetHashCode() { return default(int); }
            /// <summary>
            /// Determines whether two specified <see cref="Section" />
            /// objects are equal.
            /// </summary>
            /// <param name="a">A <see cref="Section" /> object.</param>
            /// <param name="b">A <see cref="Section" /> object.</param>
            /// <returns>
            /// true if the <paramref name="a" /> and <paramref name="b" /> parameters represent the same
            /// <see cref="Section" /> object, otherwise, false.
            /// </returns>
            public static bool operator ==(System.Collections.Specialized.BitVector32.Section a, System.Collections.Specialized.BitVector32.Section b) { return default(bool); }
            /// <summary>
            /// Determines whether two <see cref="Section" />
            /// objects have different values.
            /// </summary>
            /// <param name="a">A <see cref="Section" /> object.</param>
            /// <param name="b">A <see cref="Section" /> object.</param>
            /// <returns>
            /// true if the <paramref name="a" /> and <paramref name="b" /> parameters represent different
            /// <see cref="Section" /> objects; otherwise, false.
            /// </returns>
            public static bool operator !=(System.Collections.Specialized.BitVector32.Section a, System.Collections.Specialized.BitVector32.Section b) { return default(bool); }
            /// <summary>
            /// Returns a string that represents the current
            /// <see cref="Section" />.
            /// </summary>
            /// <returns>
            /// A string that represents the current <see cref="Section" />.
            /// </returns>
            public override string ToString() { return default(string); }
            /// <summary>
            /// Returns a string that represents the specified
            /// <see cref="Section" />.
            /// </summary>
            /// <param name="value">The <see cref="Section" /> to represent.</param>
            /// <returns>
            /// A string that represents the specified <see cref="Section" />.
            /// </returns>
            public static string ToString(System.Collections.Specialized.BitVector32.Section value) { return default(string); }
        }
    }
    /// <summary>
    /// Implements IDictionary by using a <see cref="ListDictionary" />
    /// while the collection is small, and then switching to a <see cref="Hashtable" />
    /// when the collection gets large.
    /// </summary>
    public partial class HybridDictionary : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Creates an empty case-sensitive <see cref="HybridDictionary" />.
        /// </summary>
        public HybridDictionary() { }
        /// <summary>
        /// Creates an empty <see cref="HybridDictionary" /> with the
        /// specified case sensitivity.
        /// </summary>
        /// <param name="caseInsensitive">
        /// A Boolean that denotes whether the <see cref="HybridDictionary" />
        /// is case-insensitive.
        /// </param>
        public HybridDictionary(bool caseInsensitive) { }
        /// <summary>
        /// Creates a case-sensitive <see cref="HybridDictionary" />
        /// with the specified initial size.
        /// </summary>
        /// <param name="initialSize">
        /// The approximate number of entries that the <see cref="HybridDictionary" />
        /// can initially contain.
        /// </param>
        public HybridDictionary(int initialSize) { }
        /// <summary>
        /// Creates a <see cref="HybridDictionary" /> with the specified
        /// initial size and case sensitivity.
        /// </summary>
        /// <param name="initialSize">
        /// The approximate number of entries that the <see cref="HybridDictionary" />
        /// can initially contain.
        /// </param>
        /// <param name="caseInsensitive">
        /// A Boolean that denotes whether the <see cref="HybridDictionary" />
        /// is case-insensitive.
        /// </param>
        public HybridDictionary(int initialSize, bool caseInsensitive) { }
        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="HybridDictionary" />.
        /// Retrieving the value of this property is an O(1) operation.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="HybridDictionary" />
        /// has a fixed size.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="HybridDictionary" />
        /// is read-only.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="HybridDictionary" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, attempting
        /// to get it returns null, and attempting to set it creates a new entry using the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public object this[object key] { get { return default(object); } set { } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the keys in the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the keys in the
        /// <see cref="HybridDictionary" />.
        /// </returns>
        public System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the
        /// <see cref="HybridDictionary" />.
        /// </returns>
        public object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the values in the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the values in the
        /// <see cref="HybridDictionary" />.
        /// </returns>
        public System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An entry with the same key already exists in the
        /// <see cref="HybridDictionary" />.
        /// </exception>
        public void Add(object key, object value) { }
        /// <summary>
        /// Removes all entries from the <see cref="HybridDictionary" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="HybridDictionary" /> contains
        /// a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="HybridDictionary" />.</param>
        /// <returns>
        /// true if the <see cref="HybridDictionary" /> contains an entry
        /// with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Contains(object key) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="HybridDictionary" /> entries to a one-dimensional
        /// <see cref="Array" /> instance at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from <see cref="HybridDictionary" />. The
        /// <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="HybridDictionary" /> is greater than the available
        /// space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="HybridDictionary" /> cannot
        /// be cast automatically to the type of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that iterates through
        /// the <see cref="HybridDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for the
        /// <see cref="HybridDictionary" />.
        /// </returns>
        public System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Removes the entry with the specified key from the
        /// <see cref="HybridDictionary" />.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public void Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Represents an indexed collection of key/value pairs.
    /// </summary>
    public partial interface IOrderedDictionary : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or- <paramref name="index" /> is equal to or greater
        /// than <see cref="ICollection.Count" />.
        /// </exception>
        object this[int index] { get; set; }
        /// <summary>
        /// Returns an enumerator that iterates through the
        /// <see cref="IOrderedDictionary" /> collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for the entire
        /// <see cref="IOrderedDictionary" /> collection.
        /// </returns>
        new System.Collections.IDictionaryEnumerator GetEnumerator();
        /// <summary>
        /// Inserts a key/value pair into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the key/value pair should be inserted.</param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.  The value can be null.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is greater than
        /// <see cref="ICollection.Count" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the
        /// <see cref="IOrderedDictionary" /> collection.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IOrderedDictionary" /> collection is read-only.-or-The
        /// <see cref="IOrderedDictionary" /> collection has a fixed
        /// size.
        /// </exception>
        void Insert(int index, object key, object value);
        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or- <paramref name="index" /> is equal to or greater
        /// than <see cref="ICollection.Count" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="IOrderedDictionary" /> collection is read-only.-or-
        /// The <see cref="IOrderedDictionary" /> collection has a fixed
        /// size.
        /// </exception>
        void RemoveAt(int index);
    }
    /// <summary>
    /// Implements IDictionary using a singly linked list. Recommended for collections that typically
    /// include fewer than 10 items.
    /// </summary>
    public partial class ListDictionary : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Creates an empty <see cref="ListDictionary" /> using the
        /// default comparer.
        /// </summary>
        public ListDictionary() { }
        /// <summary>
        /// Creates an empty <see cref="ListDictionary" /> using the
        /// specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> to use to determine whether two keys are
        /// equal.-or- null to use the default comparer, which is each key's implementation of
        /// <see cref="Object.Equals(Object)" />.
        /// </param>
        public ListDictionary(System.Collections.IComparer comparer) { }
        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="ListDictionary" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ListDictionary" />
        /// has a fixed size.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ListDictionary" />
        /// is read-only.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ListDictionary" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, attempting
        /// to get it returns null, and attempting to set it creates a new entry using the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public object this[object key] { get { return default(object); } set { } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the keys in the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the keys in the
        /// <see cref="ListDictionary" />.
        /// </returns>
        public System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the
        /// <see cref="ListDictionary" />.
        /// </returns>
        public object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the values in the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the values in the
        /// <see cref="ListDictionary" />.
        /// </returns>
        public System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An entry with the same key already exists in the <see cref="ListDictionary" />.
        /// </exception>
        public void Add(object key, object value) { }
        /// <summary>
        /// Removes all entries from the <see cref="ListDictionary" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="ListDictionary" /> contains
        /// a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="ListDictionary" />.</param>
        /// <returns>
        /// true if the <see cref="ListDictionary" /> contains an entry
        /// with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Contains(object key) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="ListDictionary" /> entries to a one-dimensional
        /// <see cref="Array" /> instance at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from <see cref="ListDictionary" />. The
        /// <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="ListDictionary" /> is greater than the available space
        /// from <paramref name="index" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ListDictionary" /> cannot
        /// be cast automatically to the type of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that iterates through
        /// the <see cref="ListDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for the
        /// <see cref="ListDictionary" />.
        /// </returns>
        public System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Removes the entry with the specified key from the
        /// <see cref="ListDictionary" />.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public void Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Provides the abstract base class for a collection of associated <see cref="String" />
    /// keys and <see cref="Object" /> values that can be accessed either with the key
    /// or with the index.
    /// </summary>
    public abstract partial class NameObjectCollectionBase : System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameObjectCollectionBase" />
        /// class that is empty.
        /// </summary>
        protected NameObjectCollectionBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameObjectCollectionBase" />
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object to use to determine whether
        /// two keys are equal and to generate hash codes for the keys in the collection.
        /// </param>
        protected NameObjectCollectionBase(System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameObjectCollectionBase" />
        /// class that is empty, has the specified initial capacity, and uses the default hash code
        /// provider and the default comparer.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of entries that the
        /// <see cref="NameObjectCollectionBase" /> instance can initially contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        protected NameObjectCollectionBase(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameObjectCollectionBase" />
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of entries that the
        /// <see cref="NameObjectCollectionBase" /> object can initially contain.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object to use to determine whether
        /// two keys are equal and to generate hash codes for the keys in the collection.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        protected NameObjectCollectionBase(int capacity, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="NameObjectCollectionBase" /> instance is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="NameObjectCollectionBase" /> instance
        /// is read-only; otherwise, false.
        /// </returns>
        protected bool IsReadOnly { get { return default(bool); } set { } }
        /// <summary>
        /// Gets a <see cref="KeysCollection" />
        /// instance that contains all the keys in the <see cref="NameObjectCollectionBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// A <see cref="KeysCollection" />
        /// instance that contains all the keys in the <see cref="NameObjectCollectionBase" />
        /// instance.
        /// </returns>
        public virtual System.Collections.Specialized.NameObjectCollectionBase.KeysCollection Keys { get { return default(System.Collections.Specialized.NameObjectCollectionBase.KeysCollection); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to add. The key can be null.</param>
        /// <param name="value">The <see cref="Object" /> value of the entry to add. The value can be null.</param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        protected void BaseAdd(string name, object value) { }
        /// <summary>
        /// Removes all entries from the <see cref="NameObjectCollectionBase" />
        /// instance.
        /// </summary>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        protected void BaseClear() { }
        /// <summary>
        /// Gets the value of the entry at the specified index of the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the value of the entry at the specified
        /// index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        protected object BaseGet(int index) { return default(object); }
        /// <summary>
        /// Gets the value of the first entry with the specified key from the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to get. The key can be null.</param>
        /// <returns>
        /// An <see cref="Object" /> that represents the value of the first entry with the specified
        /// key, if found; otherwise, null.
        /// </returns>
        protected object BaseGet(string name) { return default(object); }
        /// <summary>
        /// Returns a <see cref="String" /> array that contains all the keys in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> array that contains all the keys in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </returns>
        protected string[] BaseGetAllKeys() { return default(string[]); }
        /// <summary>
        /// Returns an <see cref="Object" /> array that contains all the values in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> array that contains all the values in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </returns>
        protected object[] BaseGetAllValues() { return default(object[]); }
        /// <summary>
        /// Returns an array of the specified type that contains all the values in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="type">A <see cref="Type" /> that represents the type of array to return.</param>
        /// <returns>
        /// An array of the specified type that contains all the values in the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type" /> is not a valid <see cref="Type" />.
        /// </exception>
        protected object[] BaseGetAllValues(System.Type type) { return default(object[]); }
        /// <summary>
        /// Gets the key of the entry at the specified index of the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>
        /// A <see cref="String" /> that represents the key of the entry at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        protected string BaseGetKey(int index) { return default(string); }
        /// <summary>
        /// Gets a value indicating whether the <see cref="NameObjectCollectionBase" />
        /// instance contains entries whose keys are not null.
        /// </summary>
        /// <returns>
        /// true if the <see cref="NameObjectCollectionBase" /> instance
        /// contains entries whose keys are not null; otherwise, false.
        /// </returns>
        protected bool BaseHasKeys() { return default(bool); }
        /// <summary>
        /// Removes the entries with the specified key from the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entries to remove. The key can be null.</param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        protected void BaseRemove(string name) { }
        /// <summary>
        /// Removes the entry at the specified index of the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        protected void BaseRemoveAt(int index) { }
        /// <summary>
        /// Sets the value of the entry at the specified index of the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to set.</param>
        /// <param name="value">
        /// The <see cref="Object" /> that represents the new value of the entry to set. The
        /// value can be null.
        /// </param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        protected void BaseSet(int index, object value) { }
        /// <summary>
        /// Sets the value of the first entry with the specified key in the
        /// <see cref="NameObjectCollectionBase" /> instance, if found; otherwise, adds an entry with the specified key and value into the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to set. The key can be null.</param>
        /// <param name="value">
        /// The <see cref="Object" /> that represents the new value of the entry to set. The
        /// value can be null.
        /// </param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        protected void BaseSet(string name, object value) { }
        /// <summary>
        /// Returns an enumerator that iterates through the
        /// <see cref="NameObjectCollectionBase" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Represents a collection of the <see cref="String" /> keys of a collection.
        /// </summary>
        public partial class KeysCollection : System.Collections.ICollection, System.Collections.IEnumerable
        {
            internal KeysCollection() { }
            /// <summary>
            /// Gets the number of keys in the
            /// <see cref="KeysCollection" />.
            /// </summary>
            /// <returns>
            /// The number of keys in the
            /// <see cref="KeysCollection" />.
            /// </returns>
            public int Count { get { return default(int); } }
            /// <summary>
            /// Gets the entry at the specified index of the collection.
            /// </summary>
            /// <param name="index">The zero-based index of the entry to locate in the collection.</param>
            /// <returns>
            /// The <see cref="String" /> key of the entry at the specified index of the collection.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="index" /> is outside the valid range of indexes for the collection.
            /// </exception>
            public string this[int index] { get { return default(string); } }
            bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
            object System.Collections.ICollection.SyncRoot { get { return default(object); } }
            /// <summary>
            /// Gets the key at the specified index of the collection.
            /// </summary>
            /// <param name="index">The zero-based index of the key to get from the collection.</param>
            /// <returns>
            /// A <see cref="String" /> that contains the key at the specified index of the collection.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="index" /> is outside the valid range of indexes for the collection.
            /// </exception>
            public virtual string Get(int index) { return default(string); }
            /// <summary>
            /// Returns an enumerator that iterates through the
            /// <see cref="KeysCollection" />.
            /// </summary>
            /// <returns>
            /// An <see cref="IEnumerator" /> for the
            /// <see cref="KeysCollection" />.
            /// </returns>
            public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        }
    }
    /// <summary>
    /// Represents a collection of associated <see cref="String" /> keys and <see cref="String" />
    /// values that can be accessed either with the key or with the index.
    /// </summary>
    public partial class NameValueCollection : System.Collections.Specialized.NameObjectCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection" />
        /// class that is empty, has the default initial capacity and uses the default case-insensitive
        /// hash code provider and the default case-insensitive comparer.
        /// </summary>
        public NameValueCollection() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection" />
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object to use to determine whether
        /// two keys are equal and to generate hash codes for the keys in the collection.
        /// </param>
        public NameValueCollection(System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Copies the entries from the specified <see cref="NameValueCollection" />
        /// to a new <see cref="NameValueCollection" /> with the same
        /// initial capacity as the number of entries copied and using the same hash code provider and
        /// the same comparer as the source collection.
        /// </summary>
        /// <param name="col">
        /// The <see cref="NameValueCollection" /> to copy to the new
        /// <see cref="NameValueCollection" /> instance.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="col" /> is null.</exception>
        public NameValueCollection(System.Collections.Specialized.NameValueCollection col) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection" />
        /// class that is empty, has the specified initial capacity and uses the default case-insensitive
        /// hash code provider and the default case-insensitive comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public NameValueCollection(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameValueCollection" />
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection" />
        /// object can contain.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object to use to determine whether
        /// two keys are equal and to generate hash codes for the keys in the collection.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public NameValueCollection(int capacity, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Copies the entries from the specified <see cref="NameValueCollection" />
        /// to a new <see cref="NameValueCollection" /> with the specified
        /// initial capacity or the same initial capacity as the number of entries copied, whichever is
        /// greater, and using the default case-insensitive hash code provider and the default case-insensitive
        /// comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of entries that the <see cref="NameValueCollection" />
        /// can contain.
        /// </param>
        /// <param name="col">
        /// The <see cref="NameValueCollection" /> to copy to the new
        /// <see cref="NameValueCollection" /> instance.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="col" /> is null.</exception>
        public NameValueCollection(int capacity, System.Collections.Specialized.NameValueCollection col) { }
        /// <summary>
        /// Gets all the keys in the <see cref="NameValueCollection" />.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> array that contains all the keys of the
        /// <see cref="NameValueCollection" />.
        /// </returns>
        public virtual string[] AllKeys { get { return default(string[]); } }
        /// <summary>
        /// Gets the entry at the specified index of the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to locate in the collection.</param>
        /// <returns>
        /// A <see cref="String" /> that contains the comma-separated list of values at the specified
        /// index of the collection.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public string this[int index] { get { return default(string); } }
        /// <summary>
        /// Gets or sets the entry with the specified key in the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to locate. The key can be null.</param>
        /// <returns>
        /// A <see cref="String" /> that contains the comma-separated list of values associated
        /// with the specified key, if found; otherwise, null.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The collection is read-only and the operation attempts to modify the collection.
        /// </exception>
        public string this[string name] { get { return default(string); } set { } }
        /// <summary>
        /// Copies the entries in the specified <see cref="NameValueCollection" />
        /// to the current <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="c">
        /// The <see cref="NameValueCollection" /> to copy to the current
        /// <see cref="NameValueCollection" />.
        /// </param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="c" /> is null.</exception>
        public void Add(System.Collections.Specialized.NameValueCollection c) { }
        /// <summary>
        /// Adds an entry with the specified name and value to the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to add. The key can be null.</param>
        /// <param name="value">The <see cref="String" /> value of the entry to add. The value can be null.</param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        public virtual void Add(string name, string value) { }
        /// <summary>
        /// Invalidates the cached arrays and removes all entries from the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        public virtual void Clear() { }
        /// <summary>
        /// Copies the entire <see cref="NameValueCollection" /> to a
        /// compatible one-dimensional <see cref="Array" />, starting at the specified index
        /// of the target array.
        /// </summary>
        /// <param name="dest">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="NameValueCollection" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="dest" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dest" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dest" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="NameValueCollection" /> is greater than the available
        /// space from <paramref name="index" /> to the end of the destination <paramref name="dest" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="NameValueCollection" />
        /// cannot be cast automatically to the type of the destination <paramref name="dest" />.
        /// </exception>
        public void CopyTo(System.Array dest, int index) { }
        /// <summary>
        /// Gets the values at the specified index of the
        /// <see cref="NameValueCollection" /> combined into one comma-separated list.
        /// </summary>
        /// <param name="index">The zero-based index of the entry that contains the values to get from the collection.</param>
        /// <returns>
        /// A <see cref="String" /> that contains a comma-separated list of the values at the
        /// specified index of the <see cref="NameValueCollection" />,
        /// if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public virtual string Get(int index) { return default(string); }
        /// <summary>
        /// Gets the values associated with the specified key from the
        /// <see cref="NameValueCollection" /> combined into one comma-separated list.
        /// </summary>
        /// <param name="name">
        /// The <see cref="String" /> key of the entry that contains the values to get. The key
        /// can be null.
        /// </param>
        /// <returns>
        /// A <see cref="String" /> that contains a comma-separated list of the values associated
        /// with the specified key from the <see cref="NameValueCollection" />,
        /// if found; otherwise, null.
        /// </returns>
        public virtual string Get(string name) { return default(string); }
        /// <summary>
        /// Gets the key at the specified index of the <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get from the collection.</param>
        /// <returns>
        /// A <see cref="String" /> that contains the key at the specified index of the
        /// <see cref="NameValueCollection" />, if found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public virtual string GetKey(int index) { return default(string); }
        /// <summary>
        /// Gets the values at the specified index of the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="index">The zero-based index of the entry that contains the values to get from the collection.</param>
        /// <returns>
        /// A <see cref="String" /> array that contains the values at the specified index of
        /// the <see cref="NameValueCollection" />, if found; otherwise,
        /// null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the valid range of indexes for the collection.
        /// </exception>
        public virtual string[] GetValues(int index) { return default(string[]); }
        /// <summary>
        /// Gets the values associated with the specified key from the
        /// <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="name">
        /// The <see cref="String" /> key of the entry that contains the values to get. The key
        /// can be null.
        /// </param>
        /// <returns>
        /// A <see cref="String" /> array that contains the values associated with the specified
        /// key from the <see cref="NameValueCollection" />, if found;
        /// otherwise, null.
        /// </returns>
        public virtual string[] GetValues(string name) { return default(string[]); }
        /// <summary>
        /// Gets a value indicating whether the <see cref="NameValueCollection" />
        /// contains keys that are not null.
        /// </summary>
        /// <returns>
        /// true if the <see cref="NameValueCollection" /> contains keys
        /// that are not null; otherwise, false.
        /// </returns>
        public bool HasKeys() { return default(bool); }
        /// <summary>
        /// Resets the cached arrays of the collection to null.
        /// </summary>
        protected void InvalidateCachedArrays() { }
        /// <summary>
        /// Removes the entries with the specified key from the
        /// <see cref="NameObjectCollectionBase" /> instance.
        /// </summary>
        /// <param name="name">The <see cref="String" /> key of the entry to remove. The key can be null.</param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        public virtual void Remove(string name) { }
        /// <summary>
        /// Sets the value of an entry in the <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="name">
        /// The <see cref="String" /> key of the entry to add the new value to. The key can be
        /// null.
        /// </param>
        /// <param name="value">
        /// The <see cref="Object" /> that represents the new value to add to the specified entry.
        /// The value can be null.
        /// </param>
        /// <exception cref="NotSupportedException">The collection is read-only.</exception>
        public virtual void Set(string name, string value) { }
    }
    /// <summary>
    /// Represents a collection of key/value pairs that are accessible by the key or index.
    /// </summary>
    public partial class OrderedDictionary : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable, System.Collections.Specialized.IOrderedDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary" />
        /// class.
        /// </summary>
        public OrderedDictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary" />
        /// class using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> to use to determine whether two keys are
        /// equal.-or- null to use the default comparer, which is each key's implementation of
        /// <see cref="Object.Equals(Object)" />.
        /// </param>
        public OrderedDictionary(System.Collections.IEqualityComparer comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary" />
        /// class using the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="OrderedDictionary" />
        /// collection can contain.
        /// </param>
        public OrderedDictionary(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDictionary" />
        /// class using the specified initial capacity and comparer.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="OrderedDictionary" />
        /// collection can contain.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> to use to determine whether two keys are
        /// equal.-or- null to use the default comparer, which is each key's implementation of
        /// <see cref="Object.Equals(Object)" />.
        /// </param>
        public OrderedDictionary(int capacity, System.Collections.IEqualityComparer comparer) { }
        /// <summary>
        /// Gets the number of key/values pairs contained in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="OrderedDictionary" />
        /// collection is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="OrderedDictionary" /> collection is
        /// read-only; otherwise, false. The default is false.
        /// </returns>
        public bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <returns>
        /// The value of the item at the specified index.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The property is being set and the <see cref="OrderedDictionary" />
        /// collection is read-only.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater
        /// than <see cref="Count" />.
        /// </exception>
        public object this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, attempting
        /// to get it returns null, and attempting to set it creates a new element using the specified key.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The property is being set and the <see cref="OrderedDictionary" />
        /// collection is read-only.
        /// </exception>
        public object this[object key] { get { return default(object); } set { } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> object containing the keys in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> object containing the keys in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </returns>
        public System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> object containing the values in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> object containing the values in the
        /// <see cref="OrderedDictionary" /> collection.
        /// </returns>
        public System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="OrderedDictionary" /> collection with the lowest available index.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. This value can be null.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="OrderedDictionary" /> collection is read-only.
        /// </exception>
        public void Add(object key, object value) { }
        /// <summary>
        /// Returns a read-only copy of the current <see cref="OrderedDictionary" />
        /// collection.
        /// </summary>
        /// <returns>
        /// A read-only copy of the current <see cref="OrderedDictionary" />
        /// collection.
        /// </returns>
        public System.Collections.Specialized.OrderedDictionary AsReadOnly() { return default(System.Collections.Specialized.OrderedDictionary); }
        /// <summary>
        /// Removes all elements from the <see cref="OrderedDictionary" />
        /// collection.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="OrderedDictionary" /> collection is read-only.
        /// </exception>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="OrderedDictionary" /> collection
        /// contains a specific key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="OrderedDictionary" />
        /// collection.
        /// </param>
        /// <returns>
        /// true if the <see cref="OrderedDictionary" /> collection contains
        /// an element with the specified key; otherwise, false.
        /// </returns>
        public bool Contains(object key) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="OrderedDictionary" /> elements to a
        /// one-dimensional <see cref="Array" /> object at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> object that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from
        /// <see cref="OrderedDictionary" /> collection. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> object that iterates
        /// through the <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> object for the
        /// <see cref="OrderedDictionary" /> collection.
        /// </returns>
        public virtual System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Inserts a new entry into the <see cref="OrderedDictionary" />
        /// collection with the specified key and value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be null.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is out of range.</exception>
        /// <exception cref="NotSupportedException">This collection is read-only.</exception>
        public void Insert(int index, object key, object value) { }
        /// <summary>
        /// Removes the entry with the specified key from the
        /// <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="OrderedDictionary" /> collection is read-only.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public void Remove(object key) { }
        /// <summary>
        /// Removes the entry at the specified index from the
        /// <see cref="OrderedDictionary" /> collection.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to remove.</param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="OrderedDictionary" /> collection is read-only.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.- or -<paramref name="index" /> is equal to or
        /// greater than <see cref="Count" />.
        /// </exception>
        public void RemoveAt(int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Represents a collection of strings.
    /// </summary>
    public partial class StringCollection : System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringCollection" />
        /// class.
        /// </summary>
        public StringCollection() { }
        /// <summary>
        /// Gets the number of strings contained in the <see cref="StringCollection" />.
        /// </summary>
        /// <returns>
        /// The number of strings contained in the <see cref="StringCollection" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="StringCollection" />
        /// is read-only.
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="StringCollection" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// This property always returns false.
        /// </returns>
        public bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is equal to or
        /// greater than <see cref="Count" />.
        /// </exception>
        public string this[int index] { get { return default(string); } set { } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="StringCollection" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the
        /// <see cref="StringCollection" />.
        /// </returns>
        public object SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Adds a string to the end of the <see cref="StringCollection" />.
        /// </summary>
        /// <param name="value">
        /// The string to add to the end of the <see cref="StringCollection" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// The zero-based index at which the new element is inserted.
        /// </returns>
        public int Add(string value) { return default(int); }
        /// <summary>
        /// Copies the elements of a string array to the end of the
        /// <see cref="StringCollection" />.
        /// </summary>
        /// <param name="value">
        /// An array of strings to add to the end of the <see cref="StringCollection" />.
        /// The array itself can not be null but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        public void AddRange(string[] value) { }
        /// <summary>
        /// Removes all the strings from the <see cref="StringCollection" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the specified string is in the
        /// <see cref="StringCollection" />.
        /// </summary>
        /// <param name="value">
        /// The string to locate in the <see cref="StringCollection" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// true if <paramref name="value" /> is found in the
        /// <see cref="StringCollection" />; otherwise, false.
        /// </returns>
        public bool Contains(string value) { return default(bool); }
        /// <summary>
        /// Copies the entire <see cref="StringCollection" /> values
        /// to a one-dimensional array of strings, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array of strings that is the destination of the elements copied from
        /// <see cref="StringCollection" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="StringCollection" /> is greater than the available
        /// space from <paramref name="index" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="StringCollection" /> cannot
        /// be cast automatically to the type of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(string[] array, int index) { }
        /// <summary>
        /// Returns a <see cref="StringEnumerator" /> that iterates through
        /// the <see cref="StringCollection" />.
        /// </summary>
        /// <returns>
        /// A <see cref="StringEnumerator" /> for the
        /// <see cref="StringCollection" />.
        /// </returns>
        public System.Collections.Specialized.StringEnumerator GetEnumerator() { return default(System.Collections.Specialized.StringEnumerator); }
        /// <summary>
        /// Searches for the specified string and returns the zero-based index of the first occurrence
        /// within the <see cref="StringCollection" />.
        /// </summary>
        /// <param name="value">The string to locate. The value can be null.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value" /> in the
        /// <see cref="StringCollection" />, if found; otherwise, -1.
        /// </returns>
        public int IndexOf(string value) { return default(int); }
        /// <summary>
        /// Inserts a string into the <see cref="StringCollection" />
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> is inserted.</param>
        /// <param name="value">The string to insert. The value can be null.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> greater than
        /// <see cref="Count" />.
        /// </exception>
        public void Insert(int index, string value) { }
        /// <summary>
        /// Removes the first occurrence of a specific string from the
        /// <see cref="StringCollection" />.
        /// </summary>
        /// <param name="value">
        /// The string to remove from the <see cref="StringCollection" />.
        /// The value can be null.
        /// </param>
        public void Remove(string value) { }
        /// <summary>
        /// Removes the string at the specified index of the
        /// <see cref="StringCollection" />.
        /// </summary>
        /// <param name="index">The zero-based index of the string to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is equal to or
        /// greater than <see cref="Count" />.
        /// </exception>
        public void RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        int System.Collections.IList.Add(object value) { return default(int); }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
    }
    /// <summary>
    /// Implements a hash table with the key and the value strongly typed to be strings rather than
    /// objects.
    /// </summary>
    public partial class StringDictionary : System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringDictionary" />
        /// class.
        /// </summary>
        public StringDictionary() { }
        /// <summary>
        /// Gets the number of key/value pairs in the <see cref="StringDictionary" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs in the <see cref="StringDictionary" />.
        /// Retrieving the value of this property is an O(1) operation.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="StringDictionary" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="StringDictionary" /> is
        /// synchronized (thread safe); otherwise, false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, Get returns
        /// null, and Set creates a new entry with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public virtual string this[string key] { get { return default(string); } set { } }
        /// <summary>
        /// Gets a collection of keys in the <see cref="StringDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> that provides the keys in the
        /// <see cref="StringDictionary" />.
        /// </returns>
        public virtual System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="StringDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> that can be used to synchronize access to the
        /// <see cref="StringDictionary" />.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Gets a collection of values in the <see cref="StringDictionary" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> that provides the values in the
        /// <see cref="StringDictionary" />.
        /// </returns>
        public virtual System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an entry with the specified key and value into the
        /// <see cref="StringDictionary" />.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value of the entry to add. The value can be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An entry with the same key already exists in the
        /// <see cref="StringDictionary" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="StringDictionary" /> is read-only.
        /// </exception>
        public virtual void Add(string key, string value) { }
        /// <summary>
        /// Removes all entries from the <see cref="StringDictionary" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="StringDictionary" /> is read-only.
        /// </exception>
        public virtual void Clear() { }
        /// <summary>
        /// Determines if the <see cref="StringDictionary" /> contains
        /// a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="StringDictionary" />.</param>
        /// <returns>
        /// true if the <see cref="StringDictionary" /> contains an entry
        /// with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">The key is null.</exception>
        public virtual bool ContainsKey(string key) { return default(bool); }
        /// <summary>
        /// Determines if the <see cref="StringDictionary" /> contains
        /// a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="StringDictionary" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// true if the <see cref="StringDictionary" /> contains an element
        /// with the specified value; otherwise, false.
        /// </returns>
        public virtual bool ContainsValue(string value) { return default(bool); }
        /// <summary>
        /// Copies the string dictionary values to a one-dimensional <see cref="Array" /> instance
        /// at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the values copied
        /// from the <see cref="StringDictionary" />.
        /// </param>
        /// <param name="index">The index in the array where copying begins.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the
        /// <see cref="StringDictionary" /> is greater than the available space from <paramref name="index" /> to the end of
        /// <paramref name="array" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than the lower bound of <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an enumerator that iterates through the string dictionary.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> that iterates through the string dictionary.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Removes the entry with the specified key from the string dictionary.
        /// </summary>
        /// <param name="key">The key of the entry to remove.</param>
        /// <exception cref="ArgumentNullException">The key is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="StringDictionary" /> is read-only.
        /// </exception>
        public virtual void Remove(string key) { }
    }
    /// <summary>
    /// Supports a simple iteration over a <see cref="StringCollection" />.
    /// </summary>
    public partial class StringEnumerator
    {
        internal StringEnumerator() { }
        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The enumerator is positioned before the first element of the collection or after the last element.
        /// </exception>
        public string Current { get { return default(string); } }
        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator
        /// has passed the end of the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public bool MoveNext() { return default(bool); }
        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public void Reset() { }
    }
}
