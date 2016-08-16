// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Collections
{
    /// <summary>
    /// Manages a compact array of bit values, which are represented as Booleans, where true indicates
    /// that the bit is on (1) and false indicates the bit is off (0).
    /// </summary>
    public sealed partial class BitArray : System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// contains bit values copied from the specified array of Booleans.
        /// </summary>
        /// <param name="values">An array of Booleans to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values" /> is null.</exception>
        public BitArray(bool[] values) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// contains bit values copied from the specified array of bytes.
        /// </summary>
        /// <param name="bytes">
        /// An array of bytes containing the values to copy, where each byte represents eight consecutive
        /// bits.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The length of <paramref name="bytes" /> is greater than <see cref="Int32.MaxValue" />.
        /// </exception>
        public BitArray(byte[] bytes) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// contains bit values copied from the specified <see cref="BitArray" />.
        /// </summary>
        /// <param name="bits">The <see cref="BitArray" /> to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bits" /> is null.</exception>
        public BitArray(System.Collections.BitArray bits) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// can hold the specified number of bit values, which are initially set to false.
        /// </summary>
        /// <param name="length">The number of bit values in the new <see cref="BitArray" />.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length" /> is less than zero.
        /// </exception>
        public BitArray(int length) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// can hold the specified number of bit values, which are initially set to the specified value.
        /// </summary>
        /// <param name="length">The number of bit values in the new <see cref="BitArray" />.</param>
        /// <param name="defaultValue">The Boolean value to assign to each bit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length" /> is less than zero.
        /// </exception>
        public BitArray(int length, bool defaultValue) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BitArray" /> class that
        /// contains bit values copied from the specified array of 32-bit integers.
        /// </summary>
        /// <param name="values">
        /// An array of integers containing the values to copy, where each integer represents 32 consecutive
        /// bits.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="values" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The length of <paramref name="values" /> is greater than <see cref="Int32.MaxValue" />
        /// </exception>
        public BitArray(int[] values) { }
        /// <summary>
        /// Gets or sets the value of the bit at a specific position in the <see cref="BitArray" />.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <returns>
        /// The value of the bit at position <paramref name="index" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is equal to or
        /// greater than <see cref="BitArray.Count" />.
        /// </exception>
        public bool this[int index] { get { return default(bool); } set { } }
        /// <summary>
        /// Gets or sets the number of elements in the <see cref="BitArray" />.
        /// </summary>
        /// <returns>
        /// The number of elements in the <see cref="BitArray" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The property is set to a value that is less than zero.
        /// </exception>
        public int Length { get { return default(int); } set { } }
        int System.Collections.ICollection.Count { get { return default(int); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Performs the bitwise AND operation between the elements of the current
        /// <see cref="BitArray" /> object and the corresponding elements in the specified array. The current
        /// <see cref="BitArray" /> object will be modified to store the result of the bitwise AND operation.
        /// </summary>
        /// <param name="value">The array with which to perform the bitwise AND operation.</param>
        /// <returns>
        /// An array containing the result of the bitwise AND operation, which is a reference to the current
        /// <see cref="BitArray" /> object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value" /> and the current <see cref="BitArray" /> do
        /// not have the same number of elements.
        /// </exception>
        public System.Collections.BitArray And(System.Collections.BitArray value) { return default(System.Collections.BitArray); }
        /// <summary>
        /// Gets the value of the bit at a specific position in the <see cref="BitArray" />.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>
        /// The value of the bit at position <paramref name="index" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is greater than
        /// or equal to the number of elements in the <see cref="BitArray" />.
        /// </exception>
        public bool Get(int index) { return default(bool); }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="BitArray" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the entire <see cref="BitArray" />.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Inverts all the bit values in the current <see cref="BitArray" />, so
        /// that elements set to true are changed to false, and elements set to false are changed to true.
        /// </summary>
        /// <returns>
        /// The current instance with inverted bit values.
        /// </returns>
        public System.Collections.BitArray Not() { return default(System.Collections.BitArray); }
        /// <summary>
        /// Performs the bitwise OR operation between the elements of the current
        /// <see cref="BitArray" /> object and the corresponding elements in the specified array. The current
        /// <see cref="BitArray" /> object will be modified to store the result of the bitwise OR operation.
        /// </summary>
        /// <param name="value">The array with which to perform the bitwise OR operation.</param>
        /// <returns>
        /// An array containing the result of the bitwise OR operation, which is a reference to the current
        /// <see cref="BitArray" /> object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value" /> and the current <see cref="BitArray" /> do
        /// not have the same number of elements.
        /// </exception>
        public System.Collections.BitArray Or(System.Collections.BitArray value) { return default(System.Collections.BitArray); }
        /// <summary>
        /// Sets the bit at a specific position in the <see cref="BitArray" /> to
        /// the specified value.
        /// </summary>
        /// <param name="index">The zero-based index of the bit to set.</param>
        /// <param name="value">The Boolean value to assign to the bit.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is greater than
        /// or equal to the number of elements in the <see cref="BitArray" />.
        /// </exception>
        public void Set(int index, bool value) { }
        /// <summary>
        /// Sets all bits in the <see cref="BitArray" /> to the specified value.
        /// </summary>
        /// <param name="value">The Boolean value to assign to all bits.</param>
        public void SetAll(bool value) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Performs the bitwise exclusive OR operation between the elements of the current
        /// <see cref="BitArray" /> object against the corresponding elements in the specified array. The current
        /// <see cref="BitArray" /> object will be modified to store the result of the bitwise exclusive OR operation.
        /// </summary>
        /// <param name="value">The array with which to perform the bitwise exclusive OR operation.</param>
        /// <returns>
        /// An array containing the result of the bitwise exclusive OR operation, which is a reference
        /// to the current <see cref="BitArray" /> object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value" /> and the current <see cref="BitArray" /> do
        /// not have the same number of elements.
        /// </exception>
        public System.Collections.BitArray Xor(System.Collections.BitArray value) { return default(System.Collections.BitArray); }
    }
    /// <summary>
    /// Provides objects for performing a structural comparison of two collection objects.
    /// </summary>
    public static partial class StructuralComparisons
    {
        /// <summary>
        /// Gets a predefined object that performs a structural comparison of two objects.
        /// </summary>
        /// <returns>
        /// A predefined object that is used to perform a structural comparison of two collection objects.
        /// </returns>
        public static System.Collections.IComparer StructuralComparer { get { return default(System.Collections.IComparer); } }
        /// <summary>
        /// Gets a predefined object that compares two objects for structural equality.
        /// </summary>
        /// <returns>
        /// A predefined object that is used to compare two collection objects for structural equality.
        /// </returns>
        public static System.Collections.IEqualityComparer StructuralEqualityComparer { get { return default(System.Collections.IEqualityComparer); } }
    }
}
namespace System.Collections.Generic
{
    /// <summary>
    /// Provides a base class for implementations of the <see cref="IComparer{T}" />
    /// generic interface.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public abstract partial class Comparer<T> : System.Collections.Generic.IComparer<T>, System.Collections.IComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer{T}" /> class.
        /// </summary>
        protected Comparer() { }
        /// <summary>
        /// Returns a default sort order comparer for the type specified by the generic argument.
        /// </summary>
        /// <returns>
        /// An object that inherits <see cref="Comparer{T}" /> and serves
        /// as a sort order comparer for type <paramref name="T" />.
        /// </returns>
        public static System.Collections.Generic.Comparer<T> Default { get { return default(System.Collections.Generic.Comparer<T>); } }
        /// <summary>
        /// When overridden in a derived class, performs a comparison of two objects of the same type and
        /// returns a value indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />,
        /// as shown in the following table.Value Meaning Less than zero <paramref name="x" />
        /// is less than <paramref name="y" />.Zero <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Type <paramref name="T" /> does not implement either the <see cref="IComparable{T}" />
        /// generic interface or the <see cref="IComparable" /> interface.
        /// </exception>
        public abstract int Compare(T x, T y);
        /// <summary>
        /// Creates a comparer by using the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison to use.</param>
        /// <returns>
        /// The new comparer.
        /// </returns>
        public static System.Collections.Generic.Comparer<T> Create(System.Comparison<T> comparison) { return default(System.Collections.Generic.Comparer<T>); }
        int System.Collections.IComparer.Compare(object x, object y) { return default(int); }
    }
    /// <summary>
    /// Represents a collection of keys and values.To browse the .NET Framework source code for this
    /// type, see the Reference Source.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public partial class Dictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that is empty, has the default initial capacity, and uses the default equality comparer
        /// for the key type.
        /// </summary>
        public Dictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />
        /// and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="Dictionary{TKey,TValue}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public Dictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />
        /// and uses the specified <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="Dictionary{TKey,TValue}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}" /> implementation to use
        /// when comparing keys, or null to use the default <see cref="EqualityComparer{T}" />
        /// for the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public Dictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}" /> implementation to use
        /// when comparing keys, or null to use the default <see cref="EqualityComparer{T}" />
        /// for the type of the key.
        /// </param>
        public Dictionary(System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that is empty, has the specified initial capacity, and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Dictionary{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0.
        /// </exception>
        public Dictionary(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}" />
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Dictionary{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}" /> implementation to use
        /// when comparing keys, or null to use the default <see cref="EqualityComparer{T}" />
        /// for the type of the key.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0.
        /// </exception>
        public Dictionary(int capacity, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}" /> that is used to determine
        /// equality of keys for the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="IEqualityComparer{T}" /> generic interface implementation
        /// that is used to determine equality of keys for the current
        /// <see cref="Dictionary{TKey,TValue}" /> and to provide hash values for the keys.
        /// </returns>
        public System.Collections.Generic.IEqualityComparer<TKey> Comparer { get { return default(System.Collections.Generic.IEqualityComparer<TKey>); } }
        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="Dictionary{TKey,TValue}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation
        /// throws a <see cref="KeyNotFoundException" />, and a set operation
        /// creates a new element with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and <paramref name="key" /> does not exist in the collection.
        /// </exception>
        public TValue this[TKey key] { get { return default(TValue); } set { } }
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="KeyCollection" /> containing the keys
        /// in the <see cref="Dictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection Keys { get { return default(System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection); } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly { get { return default(bool); } }
        System.Collections.Generic.ICollection<TKey> System.Collections.Generic.IDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.ICollection<TKey>); } }
        System.Collections.Generic.ICollection<TValue> System.Collections.Generic.IDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.ICollection<TValue>); } }
        System.Collections.Generic.IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.IEnumerable<TKey>); } }
        System.Collections.Generic.IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.IEnumerable<TValue>); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IDictionary.IsReadOnly { get { return default(bool); } }
        object System.Collections.IDictionary.this[object key] { get { return default(object); } set { } }
        System.Collections.ICollection System.Collections.IDictionary.Keys { get { return default(System.Collections.ICollection); } }
        System.Collections.ICollection System.Collections.IDictionary.Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets a collection containing the values in the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="ValueCollection" /> containing the
        /// values in the <see cref="Dictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection Values { get { return default(System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection); } }
        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="Dictionary{TKey,TValue}" />.
        /// </exception>
        public void Add(TKey key, TValue value) { }
        /// <summary>
        /// Removes all keys and values from the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey,TValue}" /> contains the
        /// specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Dictionary{TKey,TValue}" />.</param>
        /// <returns>
        /// true if the <see cref="Dictionary{TKey,TValue}" /> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key) { return default(bool); }
        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey,TValue}" /> contains a
        /// specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="Dictionary{TKey,TValue}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if the <see cref="Dictionary{TKey,TValue}" /> contains an element with
        /// the specified value; otherwise, false.
        /// </returns>
        public bool ContainsValue(TValue value) { return default(bool); }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator" /> structure for the
        /// <see cref="Dictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.Enumerator); }
        /// <summary>
        /// Removes the value with the specified key from the <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.  This method returns
        /// false if <paramref name="key" /> is not found in the <see cref="Dictionary{TKey,TValue}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Remove(TKey key) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        void System.Collections.IDictionary.Add(object key, object value) { }
        bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        void System.Collections.IDictionary.Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, if the key
        /// is found; otherwise, the default value for the type of the <paramref name="value" /> parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the <see cref="Dictionary{TKey,TValue}" /> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
        /// <summary>
        /// Enumerates the elements of a <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="Dictionary{TKey,TValue}" /> at the current
            /// position of the enumerator.
            /// </returns>
            public System.Collections.Generic.KeyValuePair<TKey, TValue> Current { get { return default(System.Collections.Generic.KeyValuePair<TKey, TValue>); } }
            System.Collections.DictionaryEntry System.Collections.IDictionaryEnumerator.Entry { get { return default(System.Collections.DictionaryEntry); } }
            object System.Collections.IDictionaryEnumerator.Key { get { return default(object); } }
            object System.Collections.IDictionaryEnumerator.Value { get { return default(object); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="Dictionary{TKey,TValue}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
        /// <summary>
        /// Represents the collection of keys in a <see cref="Dictionary{TKey,TValue}" />.
        /// This class cannot be inherited.
        /// </summary>
        public sealed partial class KeyCollection : System.Collections.Generic.ICollection<TKey>, System.Collections.Generic.IEnumerable<TKey>, System.Collections.Generic.IReadOnlyCollection<TKey>, System.Collections.ICollection, System.Collections.IEnumerable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection" />
            /// class that reflects the keys in the specified <see cref="Dictionary{TKey,TValue}" />.
            /// </summary>
            /// <param name="dictionary">
            /// The <see cref="Dictionary{TKey,TValue}" /> whose keys are reflected in the
            /// new <see cref="KeyCollection" />.
            /// </param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
            public KeyCollection(System.Collections.Generic.Dictionary<TKey, TValue> dictionary) { }
            /// <summary>
            /// Gets the number of elements contained in the
            /// <see cref="KeyCollection" />.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the <see cref="KeyCollection" />.
            /// Retrieving the value of this property is an O(1) operation.
            /// </returns>
            public int Count { get { return default(int); } }
            bool System.Collections.Generic.ICollection<TKey>.IsReadOnly { get { return default(bool); } }
            bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
            object System.Collections.ICollection.SyncRoot { get { return default(object); } }
            /// <summary>
            /// Copies the <see cref="KeyCollection" /> elements
            /// to an existing one-dimensional <see cref="Array" />, starting at the specified array
            /// index.
            /// </summary>
            /// <param name="array">
            /// The one-dimensional <see cref="Array" /> that is the destination of the elements
            /// copied from <see cref="KeyCollection" />. The <see cref="Array" />
            /// must have zero-based indexing.
            /// </param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// The number of elements in the source <see cref="KeyCollection" />
            /// is greater than the available space from <paramref name="index" /> to the end of the destination
            /// <paramref name="array" />.
            /// </exception>
            public void CopyTo(TKey[] array, int index) { }
            /// <summary>
            /// Returns an enumerator that iterates through the
            /// <see cref="KeyCollection" />.
            /// </summary>
            /// <returns>
            /// A <see cref="Enumerator" /> for the
            /// <see cref="KeyCollection" />.
            /// </returns>
            public System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.KeyCollection.Enumerator); }
            void System.Collections.Generic.ICollection<TKey>.Add(TKey item) { }
            void System.Collections.Generic.ICollection<TKey>.Clear() { }
            bool System.Collections.Generic.ICollection<TKey>.Contains(TKey item) { return default(bool); }
            bool System.Collections.Generic.ICollection<TKey>.Remove(TKey item) { return default(bool); }
            System.Collections.Generic.IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TKey>); }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
            /// <summary>
            /// Enumerates the elements of a <see cref="KeyCollection" />.
            /// </summary>
            [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public partial struct Enumerator : System.Collections.Generic.IEnumerator<TKey>, System.Collections.IEnumerator, System.IDisposable
            {
                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the <see cref="KeyCollection" />
                /// at the current position of the enumerator.
                /// </returns>
                public TKey Current { get { return default(TKey); } }
                object System.Collections.IEnumerator.Current { get { return default(object); } }
                /// <summary>
                /// Releases all resources used by the
                /// <see cref="Enumerator" />.
                /// </summary>
                public void Dispose() { }
                /// <summary>
                /// Advances the enumerator to the next element of the
                /// <see cref="KeyCollection" />.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator
                /// has passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">
                /// The collection was modified after the enumerator was created.
                /// </exception>
                public bool MoveNext() { return default(bool); }
                void System.Collections.IEnumerator.Reset() { }
            }
        }
        /// <summary>
        /// Represents the collection of values in a <see cref="Dictionary{TKey,TValue}" />.
        /// This class cannot be inherited.
        /// </summary>
        public sealed partial class ValueCollection : System.Collections.Generic.ICollection<TValue>, System.Collections.Generic.IEnumerable<TValue>, System.Collections.Generic.IReadOnlyCollection<TValue>, System.Collections.ICollection, System.Collections.IEnumerable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection" />
            /// class that reflects the values in the specified <see cref="Dictionary{TKey,TValue}" />.
            /// </summary>
            /// <param name="dictionary">
            /// The <see cref="Dictionary{TKey,TValue}" /> whose values are reflected in
            /// the new <see cref="ValueCollection" />.
            /// </param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
            public ValueCollection(System.Collections.Generic.Dictionary<TKey, TValue> dictionary) { }
            /// <summary>
            /// Gets the number of elements contained in the
            /// <see cref="ValueCollection" />.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the
            /// <see cref="ValueCollection" />.
            /// </returns>
            public int Count { get { return default(int); } }
            bool System.Collections.Generic.ICollection<TValue>.IsReadOnly { get { return default(bool); } }
            bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
            object System.Collections.ICollection.SyncRoot { get { return default(object); } }
            /// <summary>
            /// Copies the <see cref="ValueCollection" /> elements
            /// to an existing one-dimensional <see cref="Array" />, starting at the specified array
            /// index.
            /// </summary>
            /// <param name="array">
            /// The one-dimensional <see cref="Array" /> that is the destination of the elements
            /// copied from <see cref="ValueCollection" />. The
            /// <see cref="Array" /> must have zero-based indexing.
            /// </param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// <paramref name="index" /> is less than zero.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// The number of elements in the source <see cref="ValueCollection" />
            /// is greater than the available space from <paramref name="index" /> to the end of the destination
            /// <paramref name="array" />.
            /// </exception>
            public void CopyTo(TValue[] array, int index) { }
            /// <summary>
            /// Returns an enumerator that iterates through the
            /// <see cref="ValueCollection" />.
            /// </summary>
            /// <returns>
            /// A <see cref="Enumerator" /> for
            /// the <see cref="ValueCollection" />.
            /// </returns>
            public System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.Dictionary<TKey, TValue>.ValueCollection.Enumerator); }
            void System.Collections.Generic.ICollection<TValue>.Add(TValue item) { }
            void System.Collections.Generic.ICollection<TValue>.Clear() { }
            bool System.Collections.Generic.ICollection<TValue>.Contains(TValue item) { return default(bool); }
            bool System.Collections.Generic.ICollection<TValue>.Remove(TValue item) { return default(bool); }
            System.Collections.Generic.IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TValue>); }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
            /// <summary>
            /// Enumerates the elements of a <see cref="ValueCollection" />.
            /// </summary>
            [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public partial struct Enumerator : System.Collections.Generic.IEnumerator<TValue>, System.Collections.IEnumerator, System.IDisposable
            {
                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the <see cref="ValueCollection" />
                /// at the current position of the enumerator.
                /// </returns>
                public TValue Current { get { return default(TValue); } }
                object System.Collections.IEnumerator.Current { get { return default(object); } }
                /// <summary>
                /// Releases all resources used by the
                /// <see cref="Enumerator" />.
                /// </summary>
                public void Dispose() { }
                /// <summary>
                /// Advances the enumerator to the next element of the
                /// <see cref="ValueCollection" />.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator
                /// has passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">
                /// The collection was modified after the enumerator was created.
                /// </exception>
                public bool MoveNext() { return default(bool); }
                void System.Collections.IEnumerator.Reset() { }
            }
        }
    }
    /// <summary>
    /// Provides a base class for implementations of the
    /// <see cref="IEqualityComparer{T}" /> generic interface.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public abstract partial class EqualityComparer<T> : System.Collections.Generic.IEqualityComparer<T>, System.Collections.IEqualityComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparer{T}" />
        /// class.
        /// </summary>
        protected EqualityComparer() { }
        /// <summary>
        /// Returns a default equality comparer for the type specified by the generic argument.
        /// </summary>
        /// <returns>
        /// The default instance of the <see cref="EqualityComparer{T}" />
        /// class for type <paramref name="T" />.
        /// </returns>
        public static System.Collections.Generic.EqualityComparer<T> Default { get { return default(System.Collections.Generic.EqualityComparer<T>); } }
        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <paramref name="T" />
        /// are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public abstract bool Equals(T x, T y);
        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for
        /// hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <param name="obj">The object for which to get a hash code.</param>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public abstract int GetHashCode(T obj);
        bool System.Collections.IEqualityComparer.Equals(object x, object y) { return default(bool); }
        int System.Collections.IEqualityComparer.GetHashCode(object obj) { return default(int); }
    }
    /// <summary>
    /// Represents a set of values.To browse the .NET Framework source code for this type, see the
    /// Reference Source.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    public partial class HashSet<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.ISet<T>, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}" /> class
        /// that is empty and uses the default equality comparer for the set type.
        /// </summary>
        public HashSet() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}" /> class
        /// that uses the default equality comparer for the set type, contains elements copied from the
        /// specified collection, and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public HashSet(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}" /> class
        /// that uses the specified equality comparer for the set type, contains elements copied from
        /// the specified collection, and has sufficient capacity to accommodate the number of elements
        /// copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}" /> implementation to use
        /// when comparing values in the set, or null to use the default
        /// <see cref="EqualityComparer{T}" /> implementation for the set type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public HashSet(System.Collections.Generic.IEnumerable<T> collection, System.Collections.Generic.IEqualityComparer<T> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}" /> class
        /// that is empty and uses the specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}" /> implementation to use
        /// when comparing values in the set, or null to use the default
        /// <see cref="EqualityComparer{T}" /> implementation for the set type.
        /// </param>
        public HashSet(System.Collections.Generic.IEqualityComparer<T> comparer) { }
        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}" /> object that is used
        /// to determine equality for the values in the set.
        /// </summary>
        /// <returns>
        /// The <see cref="IEqualityComparer{T}" /> object that is used to
        /// determine equality for the values in the set.
        /// </returns>
        public System.Collections.Generic.IEqualityComparer<T> Comparer { get { return default(System.Collections.Generic.IEqualityComparer<T>); } }
        /// <summary>
        /// Gets the number of elements that are contained in a set.
        /// </summary>
        /// <returns>
        /// The number of elements that are contained in the set.
        /// </returns>
        public int Count { get { return default(int); } }
        bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>
        /// true if the element is added to the <see cref="HashSet{T}" />
        /// object; false if the element is already present.
        /// </returns>
        public bool Add(T item) { return default(bool); }
        /// <summary>
        /// Removes all elements from a <see cref="HashSet{T}" /> object.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object contains
        /// the specified element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="HashSet{T}" /> object.</param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object contains the specified
        /// element; otherwise, false.
        /// </returns>
        public bool Contains(T item) { return default(bool); }
        /// <summary>
        /// Copies the elements of a <see cref="HashSet{T}" /> object to an
        /// array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="HashSet{T}" /> object. The array must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        public void CopyTo(T[] array) { }
        /// <summary>
        /// Copies the elements of a <see cref="HashSet{T}" /> object to an
        /// array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="HashSet{T}" /> object. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex" /> is greater than the length of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) { }
        /// <summary>
        /// Copies the specified number of elements of a <see cref="HashSet{T}" />
        /// object to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="HashSet{T}" /> object. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy to <paramref name="array" />.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="arrayIndex" /> is greater than the length of the destination <paramref name="array" />.
        /// -or-<paramref name="count" /> is greater than the available space from the <paramref name="index" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex, int count) { }
        /// <summary>
        /// Removes all elements in the specified collection from the current
        /// <see cref="HashSet{T}" /> object.
        /// </summary>
        /// <param name="other">
        /// The collection of items to remove from the <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void ExceptWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="HashSet{T}" />
        /// object.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator" /> object for the
        /// <see cref="HashSet{T}" /> object.
        /// </returns>
        public System.Collections.Generic.HashSet<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.HashSet<T>.Enumerator); }
        /// <summary>
        /// Modifies the current <see cref="HashSet{T}" /> object to contain
        /// only elements that are present in that object and in the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void IntersectWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object is a proper
        /// subset of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object is a proper subset
        /// of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSubsetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object is a proper
        /// superset of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object is a proper superset
        /// of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSupersetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object is a subset
        /// of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object is a subset of
        /// <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSubsetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object is a superset
        /// of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object is a superset of
        /// <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSupersetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether the current <see cref="HashSet{T}" /> object
        /// and a specified collection share common elements.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object and <paramref name="other" />
        /// share at least one common element; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool Overlaps(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Removes the specified element from a <see cref="HashSet{T}" />
        /// object.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.  This method returns
        /// false if <paramref name="item" /> is not found in the <see cref="HashSet{T}" />
        /// object.
        /// </returns>
        public bool Remove(T item) { return default(bool); }
        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from a
        /// <see cref="HashSet{T}" /> collection.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements
        /// to remove.
        /// </param>
        /// <returns>
        /// The number of elements that were removed from the <see cref="HashSet{T}" />
        /// collection.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int RemoveWhere(System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Determines whether a <see cref="HashSet{T}" /> object and the
        /// specified collection contain the same elements.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="HashSet{T}" /> object is equal to <paramref name="other" />
        /// ; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool SetEquals(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Modifies the current <see cref="HashSet{T}" /> object to contain
        /// only elements that are present either in that object or in the specified collection, but not
        /// both.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void SymmetricExceptWith(System.Collections.Generic.IEnumerable<T> other) { }
        void System.Collections.Generic.ICollection<T>.Add(T item) { }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Sets the capacity of a <see cref="HashSet{T}" /> object to the
        /// actual number of elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess() { }
        /// <summary>
        /// Modifies the current <see cref="HashSet{T}" /> object to contain
        /// all elements that are present in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="HashSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void UnionWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Enumerates the elements of a <see cref="HashSet{T}" /> object.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="HashSet{T}" /> collection at the
            /// current position of the enumerator.
            /// </returns>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by a <see cref="Enumerator" />
            /// object.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="HashSet{T}" />
            /// collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
    /// <summary>
    /// Represents a doubly linked list.
    /// </summary>
    /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
    public partial class LinkedList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList{T}" />
        /// class that is empty.
        /// </summary>
        public LinkedList() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedList{T}" />
        /// class that contains elements copied from the specified <see cref="IEnumerable" />
        /// and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IEnumerable" /> whose elements are copied to the new
        /// <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public LinkedList(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Gets the number of nodes actually contained in the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// The number of nodes actually contained in the <see cref="LinkedList{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets the first node of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// The first <see cref="LinkedListNode{T}" /> of the
        /// <see cref="LinkedList{T}" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> First { get { return default(System.Collections.Generic.LinkedListNode<T>); } }
        /// <summary>
        /// Gets the last node of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// The last <see cref="LinkedListNode{T}" /> of the
        /// <see cref="LinkedList{T}" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> Last { get { return default(System.Collections.Generic.LinkedListNode<T>); } }
        bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Adds a new node containing the specified value after the specified existing node in the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="LinkedListNode{T}" /> after which to insert a new
        /// <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </param>
        /// <param name="value">The value to add to the <see cref="LinkedList{T}" />.</param>
        /// <returns>
        /// The new <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> is not in the current <see cref="LinkedList{T}" />.
        /// </exception>
        public System.Collections.Generic.LinkedListNode<T> AddAfter(System.Collections.Generic.LinkedListNode<T> node, T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Adds the specified new node after the specified existing node in the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="LinkedListNode{T}" /> after which to insert
        /// <paramref name="newNode" />.
        /// </param>
        /// <param name="newNode">
        /// The new <see cref="LinkedListNode{T}" /> to add to the
        /// <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node" /> is null.-or-<paramref name="newNode" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> is not in the current <see cref="LinkedList{T}" />.
        /// -or-<paramref name="newNode" /> belongs to another <see cref="LinkedList{T}" />.
        /// </exception>
        public void AddAfter(System.Collections.Generic.LinkedListNode<T> node, System.Collections.Generic.LinkedListNode<T> newNode) { }
        /// <summary>
        /// Adds a new node containing the specified value before the specified existing node in the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="LinkedListNode{T}" /> before which to insert a
        /// new <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </param>
        /// <param name="value">The value to add to the <see cref="LinkedList{T}" />.</param>
        /// <returns>
        /// The new <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> is not in the current <see cref="LinkedList{T}" />.
        /// </exception>
        public System.Collections.Generic.LinkedListNode<T> AddBefore(System.Collections.Generic.LinkedListNode<T> node, T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Adds the specified new node before the specified existing node in the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="LinkedListNode{T}" /> before which to insert
        /// <paramref name="newNode" />.
        /// </param>
        /// <param name="newNode">
        /// The new <see cref="LinkedListNode{T}" /> to add to the
        /// <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="node" /> is null.-or-<paramref name="newNode" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> is not in the current <see cref="LinkedList{T}" />.
        /// -or-<paramref name="newNode" /> belongs to another <see cref="LinkedList{T}" />.
        /// </exception>
        public void AddBefore(System.Collections.Generic.LinkedListNode<T> node, System.Collections.Generic.LinkedListNode<T> newNode) { }
        /// <summary>
        /// Adds a new node containing the specified value at the start of the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="value">
        /// The value to add at the start of the <see cref="LinkedList{T}" />.
        /// </param>
        /// <returns>
        /// The new <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> AddFirst(T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Adds the specified new node at the start of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The new <see cref="LinkedListNode{T}" /> to add at the start of
        /// the <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> belongs to another <see cref="LinkedList{T}" />.
        /// </exception>
        public void AddFirst(System.Collections.Generic.LinkedListNode<T> node) { }
        /// <summary>
        /// Adds a new node containing the specified value at the end of the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="value">
        /// The value to add at the end of the <see cref="LinkedList{T}" />.
        /// </param>
        /// <returns>
        /// The new <see cref="LinkedListNode{T}" /> containing <paramref name="value" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> AddLast(T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Adds the specified new node at the end of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The new <see cref="LinkedListNode{T}" /> to add at the end of
        /// the <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> belongs to another <see cref="LinkedList{T}" />.
        /// </exception>
        public void AddLast(System.Collections.Generic.LinkedListNode<T> node) { }
        /// <summary>
        /// Removes all nodes from the <see cref="LinkedList{T}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether a value is in the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="LinkedList{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if <paramref name="value" /> is found in the <see cref="LinkedList{T}" />
        /// ; otherwise, false.
        /// </returns>
        public bool Contains(T value) { return default(bool); }
        /// <summary>
        /// Copies the entire <see cref="LinkedList{T}" /> to a compatible
        /// one-dimensional <see cref="Array" />, starting at the specified index of the target
        /// array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="LinkedList{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="LinkedList{T}" />
        /// is greater than the available space from <paramref name="index" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Finds the first node that contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LinkedList{T}" />.</param>
        /// <returns>
        /// The first <see cref="LinkedListNode{T}" /> that contains the specified
        /// value, if found; otherwise, null.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> Find(T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Finds the last node that contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="LinkedList{T}" />.</param>
        /// <returns>
        /// The last <see cref="LinkedListNode{T}" /> that contains the specified
        /// value, if found; otherwise, null.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> FindLast(T value) { return default(System.Collections.Generic.LinkedListNode<T>); }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="Enumerator" /> for the
        /// <see cref="LinkedList{T}" />.
        /// </returns>
        public System.Collections.Generic.LinkedList<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.LinkedList<T>.Enumerator); }
        /// <summary>
        /// Removes the first occurrence of the specified value from the
        /// <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="value">The value to remove from the <see cref="LinkedList{T}" />.</param>
        /// <returns>
        /// true if the element containing <paramref name="value" /> is successfully removed; otherwise,
        /// false.  This method also returns false if <paramref name="value" /> was not found in the original
        /// <see cref="LinkedList{T}" />.
        /// </returns>
        public bool Remove(T value) { return default(bool); }
        /// <summary>
        /// Removes the specified node from the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <param name="node">
        /// The <see cref="LinkedListNode{T}" /> to remove from the
        /// <see cref="LinkedList{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="node" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="node" /> is not in the current <see cref="LinkedList{T}" />.
        /// </exception>
        public void Remove(System.Collections.Generic.LinkedListNode<T> node) { }
        /// <summary>
        /// Removes the node at the start of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="LinkedList{T}" /> is empty.
        /// </exception>
        public void RemoveFirst() { }
        /// <summary>
        /// Removes the node at the end of the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="LinkedList{T}" /> is empty.
        /// </exception>
        public void RemoveLast() { }
        void System.Collections.Generic.ICollection<T>.Add(T value) { }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Enumerates the elements of a <see cref="LinkedList{T}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="LinkedList{T}" /> at the current
            /// position of the enumerator.
            /// </returns>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="LinkedList{T}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
    /// <summary>
    /// Represents a node in a <see cref="LinkedList{T}" />. This class
    /// cannot be inherited.
    /// </summary>
    /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
    public sealed partial class LinkedListNode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedListNode{T}" />
        /// class, containing the specified value.
        /// </summary>
        /// <param name="value">The value to contain in the <see cref="LinkedListNode{T}" />.</param>
        public LinkedListNode(T value) { }
        /// <summary>
        /// Gets the <see cref="LinkedList{T}" /> that the
        /// <see cref="LinkedListNode{T}" /> belongs to.
        /// </summary>
        /// <returns>
        /// A reference to the <see cref="LinkedList{T}" /> that the
        /// <see cref="LinkedListNode{T}" /> belongs to, or null if the
        /// <see cref="LinkedListNode{T}" /> is not linked.
        /// </returns>
        public System.Collections.Generic.LinkedList<T> List { get { return default(System.Collections.Generic.LinkedList<T>); } }
        /// <summary>
        /// Gets the next node in the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// A reference to the next node in the <see cref="LinkedList{T}" />,
        /// or null if the current node is the last element (<see cref="LinkedList{T}.Last" />
        /// ) of the <see cref="LinkedList{T}" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> Next { get { return default(System.Collections.Generic.LinkedListNode<T>); } }
        /// <summary>
        /// Gets the previous node in the <see cref="LinkedList{T}" />.
        /// </summary>
        /// <returns>
        /// A reference to the previous node in the <see cref="LinkedList{T}" />,
        /// or null if the current node is the first element (<see cref="LinkedList{T}.First" />
        /// ) of the <see cref="LinkedList{T}" />.
        /// </returns>
        public System.Collections.Generic.LinkedListNode<T> Previous { get { return default(System.Collections.Generic.LinkedListNode<T>); } }
        /// <summary>
        /// Gets the value contained in the node.
        /// </summary>
        /// <returns>
        /// The value contained in the node.
        /// </returns>
        public T Value { get { return default(T); } set { } }
    }
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods
    /// to search, sort, and manipulate lists.To browse the .NET Framework source code for this type, see the
    /// Reference Source.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public partial class List<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.IReadOnlyList<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}" /> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        public List() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}" /> class
        /// that contains elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public List(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}" /> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than 0.
        /// </exception>
        public List(int capacity) { }
        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="List{T}" /> can contain
        /// before resizing is required.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="List{T}.Capacity" /> is set to a value that is less
        /// than <see cref="Count"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public int Capacity { get { return default(int); } set { } }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="List{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="List{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is equal to or greater
        /// than <see cref="Count"/>.
        /// </exception>
        public T this[int index] { get { return default(T); } set { } }
        bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Adds an object to the end of the <see cref="List{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the <see cref="List{T}" />.
        /// The value can be null for reference types.
        /// </param>
        public void Add(T item) { }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the
        /// <see cref="List{T}" />.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the
        /// <see cref="List{T}" />. The collection itself cannot be null, but it can contain elements that are null, if type
        /// <paramref name="T" /> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public void AddRange(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Returns a read-only <see cref="ObjectModel.ReadOnlyCollection{T}" /> wrapper
        /// for the current collection.
        /// </summary>
        /// <returns>
        /// An object that acts as a read-only wrapper around the current
        /// <see cref="List{T}" />.
        /// </returns>
        public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly() { return default(System.Collections.ObjectModel.ReadOnlyCollection<T>); }
        /// <summary>
        /// Searches the entire sorted <see cref="List{T}" /> for an element
        /// using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item" /> in the sorted
        /// <see cref="List{T}" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The default comparer <see cref="Comparer{T}.Default" /> cannot
        /// find an implementation of the <see cref="IComparable{T}" /> generic interface or the
        /// <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        public int BinarySearch(T item) { return default(int); }
        /// <summary>
        /// Searches the entire sorted <see cref="List{T}" /> for an element
        /// using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// elements.-or-null to use the default comparer <see cref="Comparer{T}.Default" />.
        /// </param>
        /// <returns>
        /// The zero-based index of <paramref name="item" /> in the sorted
        /// <see cref="List{T}" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null, and the default comparer
        /// <see cref="Comparer{T}.Default" /> cannot find an implementation of the <see cref="IComparable{T}" /> generic interface
        /// or the <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        public int BinarySearch(T item, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
        /// <summary>
        /// Searches a range of elements in the sorted <see cref="List{T}" />
        /// for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// elements, or null to use the default comparer <see cref="Comparer{T}.Default" />.
        /// </param>
        /// <returns>
        /// The zero-based index of <paramref name="item" /> in the sorted
        /// <see cref="List{T}" />, if <paramref name="item" /> is found; otherwise, a negative number that is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="item" /> or,
        /// if there is no larger element, the bitwise complement of <see cref="Count"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in the
        /// <see cref="List{T}" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null, and the default comparer
        /// <see cref="Comparer{T}.Default" /> cannot find an implementation of the <see cref="IComparable{T}" /> generic interface
        /// or the <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        public int BinarySearch(int index, int count, T item, System.Collections.Generic.IComparer<T> comparer) { return default(int); }
        /// <summary>
        /// Removes all elements from the <see cref="List{T}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether an element is in the <see cref="List{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="List{T}" />
        /// ; otherwise, false.
        /// </returns>
        public bool Contains(T item) { return default(bool); }
        /// <summary>
        /// Copies the entire <see cref="List{T}" /> to a compatible one-dimensional
        /// array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="List{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="List{T}" /> is
        /// greater than the number of elements that the destination <paramref name="array" /> can contain.
        /// </exception>
        public void CopyTo(T[] array) { }
        /// <summary>
        /// Copies the entire <see cref="List{T}" /> to a compatible one-dimensional
        /// array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="List{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="List{T}" /> is
        /// greater than the available space from <paramref name="arrayIndex" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) { }
        /// <summary>
        /// Copies a range of elements from the <see cref="List{T}" /> to
        /// a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">
        /// The zero-based index in the source <see cref="List{T}" /> at which
        /// copying begins.
        /// </param>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="List{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="arrayIndex" /> is less than 0.-or-
        /// <paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the
        /// <see cref="Count"/> of the source <see cref="List{T}" />.-or-The number of elements
        /// from <paramref name="index" /> to the end of the source
        /// <see cref="List{T}" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the
        /// destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count) { }
        /// <summary>
        /// Determines whether the <see cref="List{T}" /> contains elements
        /// that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements
        /// to search for.
        /// </param>
        /// <returns>
        /// true if the <see cref="List{T}" /> contains one or more elements
        /// that match the conditions defined by the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public bool Exists(System.Predicate<T> match) { return default(bool); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the first occurrence within the entire <see cref="List{T}" />.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate, if found;
        /// otherwise, the default value for type <paramref name="T" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public T Find(System.Predicate<T> match) { return default(T); }
        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements
        /// to search for.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}" /> containing all the elements that match
        /// the conditions defined by the specified predicate, if found; otherwise, an empty
        /// <see cref="List{T}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public System.Collections.Generic.List<T> FindAll(System.Predicate<T> match) { return default(System.Collections.Generic.List<T>); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the first occurrence within the range of elements in the
        /// <see cref="List{T}" /> that starts at the specified index and contains
        /// the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.-or-<paramref name="count" /> is less than 0.-or-<paramref name="startIndex" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="List{T}" />.
        /// </exception>
        public int FindIndex(int startIndex, int count, System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the first occurrence within the range of elements in the
        /// <see cref="List{T}" /> that extends from the specified index to the
        /// last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.
        /// </exception>
        public int FindIndex(int startIndex, System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the first occurrence within the entire
        /// <see cref="List{T}" />.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int FindIndex(System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the last occurrence within the entire <see cref="List{T}" />.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate, if found;
        /// otherwise, the default value for type <paramref name="T" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public T FindLast(System.Predicate<T> match) { return default(T); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the last occurrence within the range of elements in the
        /// <see cref="List{T}" /> that contains the specified number of elements
        /// and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.-or-<paramref name="count" /> is less than 0.-or-<paramref name="startIndex" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="List{T}" />.
        /// </exception>
        public int FindLastIndex(int startIndex, int count, System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the last occurrence within the range of elements in the
        /// <see cref="List{T}" /> that extends from the first element to the specified
        /// index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.
        /// </exception>
        public int FindLastIndex(int startIndex, System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and
        /// returns the zero-based index of the last occurrence within the entire
        /// <see cref="List{T}" />.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the element
        /// to search for.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined
        /// by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int FindLastIndex(System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}" />.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action{T}" /> delegate to perform on each element of the
        /// <see cref="List{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// An element in the collection has been modified. This exception is thrown starting with the
        /// .NET Framework 4.5.
        /// </exception>
        public void ForEach(System.Action<T> action) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List{T}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator" /> for the
        /// <see cref="List{T}" />.
        /// </returns>
        public System.Collections.Generic.List<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.List<T>.Enumerator); }
        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="List{T}" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based <see cref="List{T}" /> index at which the range
        /// starts.
        /// </param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// A shallow copy of a range of elements in the source <see cref="List{T}" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="List{T}" />.
        /// </exception>
        public System.Collections.Generic.List<T> GetRange(int index, int count) { return default(System.Collections.Generic.List<T>); }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the entire <see cref="List{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" /> within the entire
        /// <see cref="List{T}" />, if found; otherwise, –1.
        /// </returns>
        public int IndexOf(T item) { return default(int); }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the range of elements in the <see cref="List{T}" /> that
        /// extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" /> within the range
        /// of elements in the <see cref="List{T}" /> that extends from <paramref name="index" />
        /// to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.
        /// </exception>
        public int IndexOf(T item, int index) { return default(int); }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the range of elements in the <see cref="List{T}" /> that
        /// starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" /> within the range
        /// of elements in the <see cref="List{T}" /> that starts at <paramref name="index" />
        /// and contains <paramref name="count" /> number of elements, if found; otherwise,
        /// –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.-or-<paramref name="count" /> is less than 0.-or-<paramref name="index" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="List{T}" />.
        /// </exception>
        public int IndexOf(T item, int index, int count) { return default(int); }
        /// <summary>
        /// Inserts an element into the <see cref="List{T}" /> at the specified
        /// index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is greater than
        /// <see cref="Count"/>.
        /// </exception>
        public void Insert(int index, T item) { }
        /// <summary>
        /// Inserts the elements of a collection into the <see cref="List{T}" />
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the <see cref="List{T}" />.
        /// The collection itself cannot be null, but it can contain elements that are null, if type
        /// <paramref name="T" /> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is greater than
        /// <see cref="Count"/>.
        /// </exception>
        public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence
        /// within the entire <see cref="List{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item" /> within the entire
        /// the <see cref="List{T}" />, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf(T item) { return default(int); }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence
        /// within the range of elements in the <see cref="List{T}" /> that
        /// extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item" /> within the range of
        /// elements in the <see cref="List{T}" /> that extends from the first
        /// element to <paramref name="index" />, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.
        /// </exception>
        public int LastIndexOf(T item, int index) { return default(int); }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence
        /// within the range of elements in the <see cref="List{T}" /> that
        /// contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item" /> within the range of
        /// elements in the <see cref="List{T}" /> that contains <paramref name="count" />
        /// number of elements and ends at <paramref name="index" />, if found; otherwise,
        /// –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="List{T}" />.-or-<paramref name="count" /> is less than 0.-or-<paramref name="index" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="List{T}" />.
        /// </exception>
        public int LastIndexOf(T item, int index, int count) { return default(int); }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="List{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="List{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> is successfully removed; otherwise, false.  This method also
        /// returns false if <paramref name="item" /> was not found in the <see cref="List{T}" />.
        /// </returns>
        public bool Remove(T item) { return default(bool); }
        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions of the elements
        /// to remove.
        /// </param>
        /// <returns>
        /// The number of elements removed from the <see cref="List{T}" />
        /// .
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int RemoveAll(System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Removes the element at the specified index of the <see cref="List{T}" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="index" /> is equal to or greater
        /// than <see cref="Count"/>.
        /// </exception>
        public void RemoveAt(int index) { }
        /// <summary>
        /// Removes a range of elements from the <see cref="List{T}" />.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="List{T}" />.
        /// </exception>
        public void RemoveRange(int index, int count) { }
        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="List{T}" />.
        /// </summary>
        public void Reverse() { }
        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="List{T}" />.
        /// </exception>
        public void Reverse(int index, int count) { }
        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}" /> using
        /// the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The default comparer <see cref="Comparer{T}.Default" /> cannot
        /// find an implementation of the <see cref="IComparable{T}" /> generic interface or the
        /// <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        public void Sort() { }
        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}" /> using
        /// the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// elements, or null to use the default comparer <see cref="Comparer{T}.Default" />.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null, and the default comparer
        /// <see cref="Comparer{T}.Default" /> cannot find implementation of the <see cref="IComparable{T}" /> generic interface
        /// or the <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The implementation of <paramref name="comparer" /> caused an error during the sort. For example,
        /// <paramref name="comparer" /> might not return 0 when comparing an item with itself.
        /// </exception>
        public void Sort(System.Collections.Generic.IComparer<T> comparer) { }
        /// <summary>
        /// Sorts the elements in the entire <see cref="List{T}" /> using
        /// the specified <see cref="Comparison{T}" />.
        /// </summary>
        /// <param name="comparison">The <see cref="Comparison{T}" /> to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// The implementation of <paramref name="comparison" /> caused an error during the sort. For
        /// example, <paramref name="comparison" /> might not return 0 when comparing an item with itself.
        /// </exception>
        public void Sort(System.Comparison<T> comparison) { }
        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="List{T}" />
        /// using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// elements, or null to use the default comparer <see cref="Comparer{T}.Default" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than 0.-or-<paramref name="count" /> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid range in the
        /// <see cref="List{T}" />.-or-      The implementation of
        /// <paramref name="comparer" /> caused an error during the sort. For example, <paramref name="comparer" />
        /// might not return 0 when comparing an item with itself.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null, and the default comparer
        /// <see cref="Comparer{T}.Default" /> cannot find implementation of the <see cref="IComparable{T}" /> generic interface
        /// or the <see cref="IComparable" /> interface for type <paramref name="T" />.
        /// </exception>
        public void Sort(int index, int count, System.Collections.Generic.IComparer<T> comparer) { }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        int System.Collections.IList.Add(object item) { return default(int); }
        bool System.Collections.IList.Contains(object item) { return default(bool); }
        int System.Collections.IList.IndexOf(object item) { return default(int); }
        void System.Collections.IList.Insert(int index, object item) { }
        void System.Collections.IList.Remove(object item) { }
        /// <summary>
        /// Copies the elements of the <see cref="List{T}" /> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the <see cref="List{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="List{T}" />,
        /// if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess() { }
        /// <summary>
        /// Determines whether every element in the <see cref="List{T}" />
        /// matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">
        /// The <see cref="Predicate{T}" /> delegate that defines the conditions to check against
        /// the elements.
        /// </param>
        /// <returns>
        /// true if every element in the <see cref="List{T}" /> matches the
        /// conditions defined by the specified predicate; otherwise, false. If the list has no elements,
        /// the return value is true.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public bool TrueForAll(System.Predicate<T> match) { return default(bool); }
        /// <summary>
        /// Enumerates the elements of a <see cref="List{T}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="List{T}" /> at the current position
            /// of the enumerator.
            /// </returns>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="List{T}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
    /// <summary>
    /// Represents a first-in, first-out collection of objects.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    public partial class Queue<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T}" /> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        public Queue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T}" /> class
        /// that contains elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new <see cref="Queue{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public Queue(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T}" /> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Queue{T}" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public Queue(int capacity) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="Queue{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Queue{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Removes all objects from the <see cref="Queue{T}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether an element is in the <see cref="Queue{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="Queue{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="Queue{T}" />
        /// ; otherwise, false.
        /// </returns>
        public bool Contains(T item) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="Queue{T}" /> elements to an existing one-dimensional
        /// <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="Queue{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="Queue{T}" /> is
        /// greater than the available space from <paramref name="arrayIndex" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) { }
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T}" />.
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the <see cref="Queue{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Queue{T}" /> is empty.
        /// </exception>
        public T Dequeue() { return default(T); }
        /// <summary>
        /// Adds an object to the end of the <see cref="Queue{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="Queue{T}" />. The value can
        /// be null for reference types.
        /// </param>
        public void Enqueue(T item) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Queue{T}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="Enumerator" /> for the
        /// <see cref="Queue{T}" />.
        /// </returns>
        public System.Collections.Generic.Queue<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.Queue<T>.Enumerator); }
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T}" />
        /// without removing it.
        /// </summary>
        /// <returns>
        /// The object at the beginning of the <see cref="Queue{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Queue{T}" /> is empty.
        /// </exception>
        public T Peek() { return default(T); }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the <see cref="Queue{T}" /> elements to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing elements copied from the <see cref="Queue{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="Queue{T}" />,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess() { }
        /// <summary>
        /// Enumerates the elements of a <see cref="Queue{T}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="Queue{T}" /> at the current position
            /// of the enumerator.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The enumerator is positioned before the first element of the collection or after the last element.
            /// </exception>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="Queue{T}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted on the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public partial class SortedDictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey,TValue}" />
        /// class that is empty and uses the default <see cref="IComparer{T}" />
        /// implementation for the key type.
        /// </summary>
        public SortedDictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey,TValue}" />
        /// class that is empty and uses the specified <see cref="IComparer{T}" />
        /// implementation to compare keys.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// keys, or null to use the default <see cref="Comparer{T}" /> for
        /// the type of the key.
        /// </param>
        public SortedDictionary(System.Collections.Generic.IComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />
        /// and uses the default <see cref="IComparer{T}" /> implementation
        /// for the key type.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="SortedDictionary{TKey,TValue}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SortedDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />
        /// and uses the specified <see cref="IComparer{T}" /> implementation
        /// to compare keys.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="SortedDictionary{TKey,TValue}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// keys, or null to use the default <see cref="Comparer{T}" /> for
        /// the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SortedDictionary(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IComparer<TKey> comparer) { }
        /// <summary>
        /// Gets the <see cref="IComparer{T}" /> used to order the elements
        /// of the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// The <see cref="IComparer{T}" /> used to order the elements of
        /// the <see cref="SortedDictionary{TKey,TValue}" />
        /// </returns>
        public System.Collections.Generic.IComparer<TKey> Comparer { get { return default(System.Collections.Generic.IComparer<TKey>); } }
        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation
        /// throws a <see cref="KeyNotFoundException" />, and a set operation
        /// creates a new element with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and <paramref name="key" /> does not exist in the collection.
        /// </exception>
        public TValue this[TKey key] { get { return default(TValue); } set { } }
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="KeyCollection" /> containing
        /// the keys in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.SortedDictionary<TKey, TValue>.KeyCollection Keys { get { return default(System.Collections.Generic.SortedDictionary<TKey, TValue>.KeyCollection); } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly { get { return default(bool); } }
        System.Collections.Generic.ICollection<TKey> System.Collections.Generic.IDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.ICollection<TKey>); } }
        System.Collections.Generic.ICollection<TValue> System.Collections.Generic.IDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.ICollection<TValue>); } }
        System.Collections.Generic.IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.IEnumerable<TKey>); } }
        System.Collections.Generic.IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.IEnumerable<TValue>); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IDictionary.IsReadOnly { get { return default(bool); } }
        object System.Collections.IDictionary.this[object key] { get { return default(object); } set { } }
        System.Collections.ICollection System.Collections.IDictionary.Keys { get { return default(System.Collections.ICollection); } }
        System.Collections.ICollection System.Collections.IDictionary.Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="ValueCollection" /> containing
        /// the values in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.SortedDictionary<TKey, TValue>.ValueCollection Values { get { return default(System.Collections.Generic.SortedDictionary<TKey, TValue>.ValueCollection); } }
        /// <summary>
        /// Adds an element with the specified key and value into the
        /// <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the
        /// <see cref="SortedDictionary{TKey,TValue}" />.
        /// </exception>
        public void Add(TKey key, TValue value) { }
        /// <summary>
        /// Removes all elements from the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="SortedDictionary{TKey,TValue}" /> contains
        /// an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedDictionary{TKey,TValue}" />.</param>
        /// <returns>
        /// true if the <see cref="SortedDictionary{TKey,TValue}" /> contains an element
        /// with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key) { return default(bool); }
        /// <summary>
        /// Determines whether the <see cref="SortedDictionary{TKey,TValue}" /> contains
        /// an element with the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// The value can be null for reference types.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedDictionary{TKey,TValue}" /> contains an element
        /// with the specified value; otherwise, false.
        /// </returns>
        public bool ContainsValue(TValue value) { return default(bool); }
        /// <summary>
        /// Copies the elements of the <see cref="SortedDictionary{TKey,TValue}" />
        /// to the specified array of <see cref="KeyValuePair{TKey,TValue}" /> structures,
        /// starting at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array of <see cref="KeyValuePair{TKey,TValue}" /> structures
        /// that is the destination of the elements copied from the current
        /// <see cref="SortedDictionary{TKey,TValue}" /> The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="SortedDictionary{TKey,TValue}" />
        /// is greater than the available space from <paramref name="index" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="Enumerator" /> for the
        /// <see cref="SortedDictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.SortedDictionary<TKey, TValue>.Enumerator GetEnumerator() { return default(System.Collections.Generic.SortedDictionary<TKey, TValue>.Enumerator); }
        /// <summary>
        /// Removes the element with the specified key from the
        /// <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false
        /// if <paramref name="key" /> is not found in the <see cref="SortedDictionary{TKey,TValue}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Remove(TKey key) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        void System.Collections.IDictionary.Add(object key, object value) { }
        bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        void System.Collections.IDictionary.Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the <paramref name="value" /> parameter.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedDictionary{TKey,TValue}" /> contains an element
        /// with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
        /// <summary>
        /// Enumerates the elements of a <see cref="SortedDictionary{TKey,TValue}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.IDictionaryEnumerator, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="SortedDictionary{TKey,TValue}" /> at the current
            /// position of the enumerator.
            /// </returns>
            public System.Collections.Generic.KeyValuePair<TKey, TValue> Current { get { return default(System.Collections.Generic.KeyValuePair<TKey, TValue>); } }
            System.Collections.DictionaryEntry System.Collections.IDictionaryEnumerator.Entry { get { return default(System.Collections.DictionaryEntry); } }
            object System.Collections.IDictionaryEnumerator.Key { get { return default(object); } }
            object System.Collections.IDictionaryEnumerator.Value { get { return default(object); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the
            /// <see cref="SortedDictionary{TKey,TValue}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
        /// <summary>
        /// Represents the collection of keys in a <see cref="SortedDictionary{TKey,TValue}" />.
        /// This class cannot be inherited.
        /// </summary>
        public sealed partial class KeyCollection : System.Collections.Generic.ICollection<TKey>, System.Collections.Generic.IEnumerable<TKey>, System.Collections.Generic.IReadOnlyCollection<TKey>, System.Collections.ICollection, System.Collections.IEnumerable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection" />
            /// class that reflects the keys in the specified
            /// <see cref="SortedDictionary{TKey,TValue}" />.
            /// </summary>
            /// <param name="dictionary">
            /// The <see cref="SortedDictionary{TKey,TValue}" /> whose keys are reflected
            /// in the new <see cref="KeyCollection" />.
            /// </param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
            public KeyCollection(System.Collections.Generic.SortedDictionary<TKey, TValue> dictionary) { }
            /// <summary>
            /// Gets the number of elements contained in the
            /// <see cref="KeyCollection" />.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the
            /// <see cref="KeyCollection" />.
            /// </returns>
            public int Count { get { return default(int); } }
            bool System.Collections.Generic.ICollection<TKey>.IsReadOnly { get { return default(bool); } }
            bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
            object System.Collections.ICollection.SyncRoot { get { return default(object); } }
            /// <summary>
            /// Copies the <see cref="KeyCollection" /> elements
            /// to an existing one-dimensional array, starting at the specified array index.
            /// </summary>
            /// <param name="array">
            /// The one-dimensional array that is the destination of the elements copied from the
            /// <see cref="KeyCollection" />. The array must have zero-based indexing.
            /// </param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than 0.</exception>
            /// <exception cref="ArgumentException">
            /// The number of elements in the source
            /// <see cref="KeyCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination
            /// <paramref name="array" />.
            /// </exception>
            public void CopyTo(TKey[] array, int index) { }
            /// <summary>
            /// Returns an enumerator that iterates through the
            /// <see cref="KeyCollection" />.
            /// </summary>
            /// <returns>
            /// A <see cref="Enumerator" />
            /// structure for the <see cref="KeyCollection" />.
            /// </returns>
            public System.Collections.Generic.SortedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.SortedDictionary<TKey, TValue>.KeyCollection.Enumerator); }
            void System.Collections.Generic.ICollection<TKey>.Add(TKey item) { }
            void System.Collections.Generic.ICollection<TKey>.Clear() { }
            bool System.Collections.Generic.ICollection<TKey>.Contains(TKey item) { return default(bool); }
            bool System.Collections.Generic.ICollection<TKey>.Remove(TKey item) { return default(bool); }
            System.Collections.Generic.IEnumerator<TKey> System.Collections.Generic.IEnumerable<TKey>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TKey>); }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
            /// <summary>
            /// Enumerates the elements of a <see cref="KeyCollection" />.
            /// </summary>
            [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public partial struct Enumerator : System.Collections.Generic.IEnumerator<TKey>, System.Collections.IEnumerator, System.IDisposable
            {
                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the <see cref="KeyCollection" />
                /// at the current position of the enumerator.
                /// </returns>
                public TKey Current { get { return default(TKey); } }
                object System.Collections.IEnumerator.Current { get { return default(object); } }
                /// <summary>
                /// Releases all resources used by the
                /// <see cref="Enumerator" />.
                /// </summary>
                public void Dispose() { }
                /// <summary>
                /// Advances the enumerator to the next element of the
                /// <see cref="KeyCollection" />.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator
                /// has passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">
                /// The collection was modified after the enumerator was created.
                /// </exception>
                public bool MoveNext() { return default(bool); }
                void System.Collections.IEnumerator.Reset() { }
            }
        }
        /// <summary>
        /// Represents the collection of values in a <see cref="SortedDictionary{TKey,TValue}" />.
        /// This class cannot be inherited
        /// </summary>
        public sealed partial class ValueCollection : System.Collections.Generic.ICollection<TValue>, System.Collections.Generic.IEnumerable<TValue>, System.Collections.Generic.IReadOnlyCollection<TValue>, System.Collections.ICollection, System.Collections.IEnumerable
        {
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="ValueCollection" /> class that reflects the values in the specified
            /// <see cref="SortedDictionary{TKey,TValue}" />.
            /// </summary>
            /// <param name="dictionary">
            /// The <see cref="SortedDictionary{TKey,TValue}" /> whose values are reflected
            /// in the new <see cref="ValueCollection" />.
            /// </param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
            public ValueCollection(System.Collections.Generic.SortedDictionary<TKey, TValue> dictionary) { }
            /// <summary>
            /// Gets the number of elements contained in the
            /// <see cref="ValueCollection" />.
            /// </summary>
            /// <returns>
            /// The number of elements contained in the
            /// <see cref="ValueCollection" />.
            /// </returns>
            public int Count { get { return default(int); } }
            bool System.Collections.Generic.ICollection<TValue>.IsReadOnly { get { return default(bool); } }
            bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
            object System.Collections.ICollection.SyncRoot { get { return default(object); } }
            /// <summary>
            /// Copies the <see cref="ValueCollection" />
            /// elements to an existing one-dimensional array, starting at the specified array index.
            /// </summary>
            /// <param name="array">
            /// The one-dimensional array that is the destination of the elements copied from the
            /// <see cref="ValueCollection" />. The array must have zero-based indexing.
            /// </param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is less than 0.</exception>
            /// <exception cref="ArgumentException">
            /// The number of elements in the source
            /// <see cref="ValueCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination
            /// <paramref name="array" />.
            /// </exception>
            public void CopyTo(TValue[] array, int index) { }
            /// <summary>
            /// Returns an enumerator that iterates through the
            /// <see cref="ValueCollection" />.
            /// </summary>
            /// <returns>
            /// A <see cref="Enumerator" />
            /// structure for the <see cref="ValueCollection" />.
            /// </returns>
            public System.Collections.Generic.SortedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator() { return default(System.Collections.Generic.SortedDictionary<TKey, TValue>.ValueCollection.Enumerator); }
            void System.Collections.Generic.ICollection<TValue>.Add(TValue item) { }
            void System.Collections.Generic.ICollection<TValue>.Clear() { }
            bool System.Collections.Generic.ICollection<TValue>.Contains(TValue item) { return default(bool); }
            bool System.Collections.Generic.ICollection<TValue>.Remove(TValue item) { return default(bool); }
            System.Collections.Generic.IEnumerator<TValue> System.Collections.Generic.IEnumerable<TValue>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<TValue>); }
            void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
            /// <summary>
            /// Enumerates the elements of a <see cref="ValueCollection" />.
            /// </summary>
            [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
            public partial struct Enumerator : System.Collections.Generic.IEnumerator<TValue>, System.Collections.IEnumerator, System.IDisposable
            {
                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <returns>
                /// The element in the <see cref="ValueCollection" />
                /// at the current position of the enumerator.
                /// </returns>
                public TValue Current { get { return default(TValue); } }
                object System.Collections.IEnumerator.Current { get { return default(object); } }
                /// <summary>
                /// Releases all resources used by the
                /// <see cref="Enumerator" />.
                /// </summary>
                public void Dispose() { }
                /// <summary>
                /// Advances the enumerator to the next element of the
                /// <see cref="ValueCollection" />.
                /// </summary>
                /// <returns>
                /// true if the enumerator was successfully advanced to the next element; false if the enumerator
                /// has passed the end of the collection.
                /// </returns>
                /// <exception cref="InvalidOperationException">
                /// The collection was modified after the enumerator was created.
                /// </exception>
                public bool MoveNext() { return default(bool); }
                void System.Collections.IEnumerator.Reset() { }
            }
        }
    }
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted by key based on the associated
    /// <see cref="IComparer{T}" /> implementation.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the collection.</typeparam>
    /// <typeparam name="TValue">The type of values in the collection.</typeparam>
    public partial class SortedList<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that is empty, has the default initial capacity, and uses the default
        /// <see cref="IComparer{T}" />.
        /// </summary>
        public SortedList() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that is empty, has the default initial capacity, and uses the specified
        /// <see cref="IComparer{T}" />.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="Comparer{T}" /> for
        /// the type of the key.
        /// </param>
        public SortedList(System.Collections.Generic.IComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />,
        /// has sufficient capacity to accommodate the number of elements copied, and uses the default
        /// <see cref="IComparer{T}" />.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="SortedList{TKey,TValue}" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SortedList(System.Collections.Generic.IDictionary<TKey, TValue> dictionary) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}" />,
        /// has sufficient capacity to accommodate the number of elements copied, and uses the specified
        /// <see cref="IComparer{T}" />.
        /// </summary>
        /// <param name="dictionary">
        /// The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to
        /// the new <see cref="SortedList{TKey,TValue}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="Comparer{T}" /> for
        /// the type of the key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="dictionary" /> contains one or more duplicate keys.
        /// </exception>
        public SortedList(System.Collections.Generic.IDictionary<TKey, TValue> dictionary, System.Collections.Generic.IComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that is empty, has the specified initial capacity, and uses the default
        /// <see cref="IComparer{T}" />.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="SortedList{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public SortedList(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList{TKey,TValue}" />
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// <see cref="IComparer{T}" />.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="SortedList{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}" /> implementation to use when comparing
        /// keys.-or-null to use the default <see cref="Comparer{T}" /> for
        /// the type of the key.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public SortedList(int capacity, System.Collections.Generic.IComparer<TKey> comparer) { }
        /// <summary>
        /// Gets or sets the number of elements that the <see cref="SortedList{TKey,TValue}" />
        /// can contain.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="SortedList{TKey,TValue}" /> can
        /// contain.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Capacity" /> is set to a value that is
        /// less than <see cref="Count" />.
        /// </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public int Capacity { get { return default(int); } set { } }
        /// <summary>
        /// Gets the <see cref="IComparer{T}" /> for the sorted list.
        /// </summary>
        /// <returns>
        /// The <see cref="IComparable{T}" /> for the current
        /// <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.IComparer<TKey> Comparer { get { return default(System.Collections.Generic.IComparer<TKey>); } }
        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, a get operation
        /// throws a <see cref="KeyNotFoundException" /> and a set operation
        /// creates a new element using the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and <paramref name="key" /> does not exist in the collection.
        /// </exception>
        public TValue this[TKey key] { get { return default(TValue); } set { } }
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="SortedList{TKey,TValue}" />,
        /// in sorted order.
        /// </summary>
        /// <returns>
        /// A <see cref="IList{T}" /> containing the keys in the
        /// <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.IList<TKey> Keys { get { return default(System.Collections.Generic.IList<TKey>); } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly { get { return default(bool); } }
        System.Collections.Generic.ICollection<TKey> System.Collections.Generic.IDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.ICollection<TKey>); } }
        System.Collections.Generic.ICollection<TValue> System.Collections.Generic.IDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.ICollection<TValue>); } }
        System.Collections.Generic.IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys { get { return default(System.Collections.Generic.IEnumerable<TKey>); } }
        System.Collections.Generic.IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values { get { return default(System.Collections.Generic.IEnumerable<TValue>); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IDictionary.IsReadOnly { get { return default(bool); } }
        object System.Collections.IDictionary.this[object key] { get { return default(object); } set { } }
        System.Collections.ICollection System.Collections.IDictionary.Keys { get { return default(System.Collections.ICollection); } }
        System.Collections.ICollection System.Collections.IDictionary.Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets a collection containing the values in the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A <see cref="IList{T}" /> containing the values in the
        /// <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.IList<TValue> Values { get { return default(System.Collections.Generic.IList<TValue>); } }
        /// <summary>
        /// Adds an element with the specified key and value into the
        /// <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="SortedList{TKey,TValue}" />.
        /// </exception>
        public void Add(TKey key, TValue value) { }
        /// <summary>
        /// Removes all elements from the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="SortedList{TKey,TValue}" /> contains a
        /// specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedList{TKey,TValue}" />.</param>
        /// <returns>
        /// true if the <see cref="SortedList{TKey,TValue}" /> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key) { return default(bool); }
        /// <summary>
        /// Determines whether the <see cref="SortedList{TKey,TValue}" /> contains a
        /// specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="SortedList{TKey,TValue}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedList{TKey,TValue}" /> contains an element with
        /// the specified value; otherwise, false.
        /// </returns>
        public bool ContainsValue(TValue value) { return default(bool); }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{T}" /> of type
        /// <see cref="KeyValuePair{TKey,TValue}" /> for the <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
        /// <summary>
        /// Searches for the specified key and returns the zero-based index within the entire
        /// <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedList{TKey,TValue}" />.</param>
        /// <returns>
        /// The zero-based index of <paramref name="key" /> within the entire
        /// <see cref="SortedList{TKey,TValue}" />, if found; otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public int IndexOfKey(TKey key) { return default(int); }
        /// <summary>
        /// Searches for the specified value and returns the zero-based index of the first occurrence
        /// within the entire <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="SortedList{TKey,TValue}" />.  The
        /// value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value" /> within the entire
        /// <see cref="SortedList{TKey,TValue}" />, if found; otherwise, -1.
        /// </returns>
        public int IndexOfValue(TValue value) { return default(int); }
        /// <summary>
        /// Removes the element with the specified key from the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false
        /// if <paramref name="key" /> was not found in the original <see cref="SortedList{TKey,TValue}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Remove(TKey key) { return default(bool); }
        /// <summary>
        /// Removes the element at the specified index of the <see cref="SortedList{TKey,TValue}" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater
        /// than <see cref="Count" />.
        /// </exception>
        public void RemoveAt(int index) { }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        void System.Collections.IDictionary.Add(object key, object value) { }
        bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        void System.Collections.IDictionary.Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the
        /// <see cref="SortedList{TKey,TValue}" />, if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess() { }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found;
        /// otherwise, the default value for the type of the <paramref name="value" /> parameter. This
        /// parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedList{TKey,TValue}" /> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
    }
    /// <summary>
    /// Represents a collection of objects that is maintained in sorted order.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public partial class SortedSet<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.Generic.ISet<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}" />
        /// class.
        /// </summary>
        public SortedSet() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}" />
        /// class that uses a specified comparer.
        /// </summary>
        /// <param name="comparer">The default comparer to use for comparing objects.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is null.</exception>
        public SortedSet(System.Collections.Generic.IComparer<T> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}" />
        /// class that contains elements copied from a specified enumerable collection.
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        public SortedSet(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedSet{T}" />
        /// class that contains elements copied from a specified enumerable collection and that uses a
        /// specified comparer.
        /// </summary>
        /// <param name="collection">The enumerable collection to be copied.</param>
        /// <param name="comparer">The default comparer to use for comparing objects.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public SortedSet(System.Collections.Generic.IEnumerable<T> collection, System.Collections.Generic.IComparer<T> comparer) { }
        /// <summary>
        /// Gets the <see cref="IComparer{T}" /> object that is used to order
        /// the values in the <see cref="SortedSet{T}" />.
        /// </summary>
        /// <returns>
        /// The comparer that is used to order the values in the <see cref="SortedSet{T}" />.
        /// </returns>
        public System.Collections.Generic.IComparer<T> Comparer { get { return default(System.Collections.Generic.IComparer<T>); } }
        /// <summary>
        /// Gets the number of elements in the <see cref="SortedSet{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements in the <see cref="SortedSet{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets the maximum value in the <see cref="SortedSet{T}" />, as
        /// defined by the comparer.
        /// </summary>
        /// <returns>
        /// The maximum value in the set.
        /// </returns>
        public T Max { get { return default(T); } }
        /// <summary>
        /// Gets the minimum value in the <see cref="SortedSet{T}" />, as
        /// defined by the comparer.
        /// </summary>
        /// <returns>
        /// The minimum value in the set.
        /// </returns>
        public T Min { get { return default(T); } }
        bool System.Collections.Generic.ICollection<T>.IsReadOnly { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Adds an element to the set and returns a value that indicates if it was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>
        /// true if <paramref name="item" /> is added to the set; otherwise, false.
        /// </returns>
        public bool Add(T item) { return default(bool); }
        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public virtual void Clear() { }
        /// <summary>
        /// Determines whether the set contains a specific element.
        /// </summary>
        /// <param name="item">The element to locate in the set.</param>
        /// <returns>
        /// true if the set contains <paramref name="item" />; otherwise, false.
        /// </returns>
        public virtual bool Contains(T item) { return default(bool); }
        /// <summary>
        /// Copies the complete <see cref="SortedSet{T}" /> to a compatible
        /// one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// A one-dimensional array that is the destination of the elements copied from the
        /// <see cref="SortedSet{T}" />.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="SortedSet{T}" />
        /// exceeds the number of elements that the destination array can contain.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        public void CopyTo(T[] array) { }
        /// <summary>
        /// Copies the complete <see cref="SortedSet{T}" /> to a compatible
        /// one-dimensional array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// A one-dimensional array that is the destination of the elements copied from the
        /// <see cref="SortedSet{T}" />. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source array is greater than the available space from <paramref name="index" />
        /// to the end of the destination array.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Copies a specified number of elements from <see cref="SortedSet{T}" />
        /// to a compatible one-dimensional array, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// A one-dimensional array that is the destination of the elements copied from the
        /// <see cref="SortedSet{T}" />. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source array is greater than the available space from <paramref name="index" />
        /// to the end of the destination array.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or-<paramref name="count" /> is less than zero.
        /// </exception>
        public void CopyTo(T[] array, int index, int count) { }
        /// <summary>
        /// Removes all elements that are in a specified collection from the current
        /// <see cref="SortedSet{T}" /> object.
        /// </summary>
        /// <param name="other">
        /// The collection of items to remove from the <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void ExceptWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="SortedSet{T}" />.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the <see cref="SortedSet{T}" />
        /// in sorted order.
        /// </returns>
        public System.Collections.Generic.SortedSet<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.SortedSet<T>.Enumerator); }
        /// <summary>
        /// Returns a view of a subset in a <see cref="SortedSet{T}" />.
        /// </summary>
        /// <param name="lowerValue">The lowest desired value in the view.</param>
        /// <param name="upperValue">The highest desired value in the view.</param>
        /// <returns>
        /// A subset view that contains only the values in the specified range.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="lowerValue" /> is more than <paramref name="upperValue" /> according to the
        /// comparer.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// A tried operation on the view was outside the range specified by <paramref name="lowerValue" />
        /// and <paramref name="upperValue" />.
        /// </exception>
        public virtual System.Collections.Generic.SortedSet<T> GetViewBetween(T lowerValue, T upperValue) { return default(System.Collections.Generic.SortedSet<T>); }
        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}" /> object so that
        /// it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public virtual void IntersectWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}" /> object is a proper
        /// subset of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedSet{T}" /> object is a proper subset
        /// of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSubsetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}" /> object is a proper
        /// superset of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedSet{T}" /> object is a proper superset
        /// of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsProperSupersetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}" /> object is a subset
        /// of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the current <see cref="SortedSet{T}" /> object is a subset
        /// of <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSubsetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="SortedSet{T}" /> object is a superset
        /// of the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedSet{T}" /> object is a superset of
        /// <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool IsSupersetOf(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Determines whether the current <see cref="SortedSet{T}" /> object
        /// and a specified collection share common elements.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedSet{T}" /> object and <paramref name="other" />
        /// share at least one common element; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool Overlaps(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Removes a specified item from the <see cref="SortedSet{T}" />.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>
        /// true if the element is found and successfully removed; otherwise, false.
        /// </returns>
        public bool Remove(T item) { return default(bool); }
        /// <summary>
        /// Removes all elements that match the conditions defined by the specified predicate from a
        /// <see cref="SortedSet{T}" />.
        /// </summary>
        /// <param name="match">The delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements that were removed from the <see cref="SortedSet{T}" />
        /// collection..
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        public int RemoveWhere(System.Predicate<T> match) { return default(int); }
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}" /> that iterates over the
        /// <see cref="SortedSet{T}" /> in reverse order.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates over the <see cref="SortedSet{T}" />
        /// in reverse order.
        /// </returns>
        public System.Collections.Generic.IEnumerable<T> Reverse() { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Determines whether the current <see cref="SortedSet{T}" /> object
        /// and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <returns>
        /// true if the current <see cref="SortedSet{T}" /> object is equal
        /// to <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public bool SetEquals(System.Collections.Generic.IEnumerable<T> other) { return default(bool); }
        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}" /> object so that
        /// it contains only elements that are present either in the current object or in the specified
        /// collection, but not both.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void SymmetricExceptWith(System.Collections.Generic.IEnumerable<T> other) { }
        void System.Collections.Generic.ICollection<T>.Add(T item) { }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Modifies the current <see cref="SortedSet{T}" /> object so that
        /// it contains all elements that are present in either the current object or the specified collection.
        /// </summary>
        /// <param name="other">
        /// The collection to compare to the current <see cref="SortedSet{T}" />
        /// object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
        public void UnionWith(System.Collections.Generic.IEnumerable<T> other) { }
        /// <summary>
        /// Enumerates the elements of a <see cref="SortedSet{T}" /> object.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="SortedSet{T}" />
            /// collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
    /// <summary>
    /// Represents a variable size last-in-first-out (LIFO) collection of instances of the same specified
    /// type.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    public partial class Stack<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack{T}" /> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        public Stack() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack{T}" /> class
        /// that contains elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public Stack(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack{T}" /> class
        /// that is empty and has the specified initial capacity or the default initial capacity, whichever
        /// is greater.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Stack{T}" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public Stack(int capacity) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="Stack{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Stack{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Removes all objects from the <see cref="Stack{T}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether an element is in the <see cref="Stack{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="Stack{T}" />. The value
        /// can be null for reference types.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="Stack{T}" />
        /// ; otherwise, false.
        /// </returns>
        public bool Contains(T item) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="Stack{T}" /> to an existing one-dimensional
        /// <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="Stack{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of elements in the source <see cref="Stack{T}" /> is
        /// greater than the available space from <paramref name="arrayIndex" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int arrayIndex) { }
        /// <summary>
        /// Returns an enumerator for the <see cref="Stack{T}" />.
        /// </summary>
        /// <returns>
        /// An <see cref="Enumerator" /> for the
        /// <see cref="Stack{T}" />.
        /// </returns>
        public System.Collections.Generic.Stack<T>.Enumerator GetEnumerator() { return default(System.Collections.Generic.Stack<T>.Enumerator); }
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T}" /> without
        /// removing it.
        /// </summary>
        /// <returns>
        /// The object at the top of the <see cref="Stack{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Stack{T}" /> is empty.
        /// </exception>
        public T Peek() { return default(T); }
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T}" />.
        /// </summary>
        /// <returns>
        /// The object removed from the top of the <see cref="Stack{T}" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Stack{T}" /> is empty.
        /// </exception>
        public T Pop() { return default(T); }
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to push onto the <see cref="Stack{T}" />. The value
        /// can be null for reference types.
        /// </param>
        public void Push(T item) { }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the <see cref="Stack{T}" /> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing copies of the elements of the <see cref="Stack{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="Stack{T}" />,
        /// if that number is less than 90 percent of current capacity.
        /// </summary>
        public void TrimExcess() { }
        /// <summary>
        /// Enumerates the elements of a <see cref="Stack{T}" />.
        /// </summary>
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public partial struct Enumerator : System.Collections.Generic.IEnumerator<T>, System.Collections.IEnumerator, System.IDisposable
        {
            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the <see cref="Stack{T}" /> at the current position
            /// of the enumerator.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The enumerator is positioned before the first element of the collection or after the last element.
            /// </exception>
            public T Current { get { return default(T); } }
            object System.Collections.IEnumerator.Current { get { return default(object); } }
            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator" />.
            /// </summary>
            public void Dispose() { }
            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="Stack{T}" />.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator
            /// has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext() { return default(bool); }
            void System.Collections.IEnumerator.Reset() { }
        }
    }
}
