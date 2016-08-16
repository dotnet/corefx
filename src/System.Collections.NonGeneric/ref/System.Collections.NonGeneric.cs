// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Collections
{
    /// <summary>
    /// Implements the <see cref="IList" /> interface using an array whose size
    /// is dynamically increased as required.To browse the .NET Framework source code for this type,
    /// see the Reference Source.
    /// </summary>
    public partial class ArrayList : System.Collections.IEnumerable, System.Collections.IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayList" /> class that
        /// is empty and has the default initial capacity.
        /// </summary>
        public ArrayList() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayList" /> class that
        /// contains elements copied from the specified collection and that has the same initial capacity
        /// as the number of elements copied.
        /// </summary>
        /// <param name="c">
        /// The <see cref="ICollection" /> whose elements are copied to the new list.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="c" /> is null.</exception>
        public ArrayList(System.Collections.ICollection c) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayList" /> class that
        /// is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public ArrayList(int capacity) { }
        /// <summary>
        /// Gets or sets the number of elements that the <see cref="ArrayList" />
        /// can contain.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="ArrayList" /> can contain.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Capacity" /> is set to a value that is less than
        /// <see cref="Count" />.
        /// </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public virtual int Capacity { get { return default(int); } set { } }
        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="ArrayList" />.
        /// </summary>
        /// <returns>
        /// The number of elements actually contained in the <see cref="ArrayList" />.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayList" /> has a fixed
        /// size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ArrayList" /> has a fixed size; otherwise, false.
        /// The default is false.
        /// </returns>
        public virtual bool IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="ArrayList" /> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ArrayList" /> is read-only; otherwise, false.
        /// The default is false.
        /// </returns>
        public virtual bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ArrayList" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="ArrayList" /> is synchronized (thread
        /// safe); otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is equal to or
        /// greater than <see cref="Count" />.
        /// </exception>
        public virtual object this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ArrayList" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="ArrayList" />.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Creates an <see cref="ArrayList" /> wrapper for a specific
        /// <see cref="IList" />.
        /// </summary>
        /// <param name="list">The <see cref="IList" /> to wrap.</param>
        /// <returns>
        /// The <see cref="ArrayList" /> wrapper around the <see cref="IList" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.ArrayList Adapter(System.Collections.IList list) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Adds an object to the end of the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to be added to the end of the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// The <see cref="ArrayList" /> index at which the <paramref name="value" />
        /// has been added.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual int Add(object value) { return default(int); }
        /// <summary>
        /// Adds the elements of an <see cref="ICollection" /> to the end of the
        /// <see cref="ArrayList" />.
        /// </summary>
        /// <param name="c">
        /// The <see cref="ICollection" /> whose elements should be added to the
        /// end of the <see cref="ArrayList" />. The collection itself cannot be
        /// null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="c" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void AddRange(System.Collections.ICollection c) { }
        /// <summary>
        /// Searches a range of elements in the sorted <see cref="ArrayList" /> for
        /// an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="value">The <see cref="Object" /> to locate. The value can be null.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing elements.-or-
        /// null to use the default comparer that is the <see cref="IComparable" /> implementation
        /// of each element.
        /// </param>
        /// <returns>
        /// The zero-based index of <paramref name="value" /> in the sorted <see cref="ArrayList" />,
        /// if <paramref name="value" /> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="value" />
        /// or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in the
        /// <see cref="ArrayList" />.-or- <paramref name="comparer" /> is null and
        /// neither <paramref name="value" /> nor the elements of <see cref="ArrayList" />
        /// implement the <see cref="IComparable" /> interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null and <paramref name="value" /> is not of the same type
        /// as the elements of the <see cref="ArrayList" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        public virtual int BinarySearch(int index, int count, object value, System.Collections.IComparer comparer) { return default(int); }
        /// <summary>
        /// Searches the entire sorted <see cref="ArrayList" /> for an element using
        /// the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to locate. The value can be null.</param>
        /// <returns>
        /// The zero-based index of <paramref name="value" /> in the sorted <see cref="ArrayList" />,
        /// if <paramref name="value" /> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="value" />
        /// or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Neither <paramref name="value" /> nor the elements of <see cref="ArrayList" />
        /// implement the <see cref="IComparable" /> interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="value" /> is not of the same type as the elements of the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual int BinarySearch(object value) { return default(int); }
        /// <summary>
        /// Searches the entire sorted <see cref="ArrayList" /> for an element using
        /// the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="value">The <see cref="Object" /> to locate. The value can be null.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing elements.-or-
        /// null to use the default comparer that is the <see cref="IComparable" /> implementation
        /// of each element.
        /// </param>
        /// <returns>
        /// The zero-based index of <paramref name="value" /> in the sorted <see cref="ArrayList" />,
        /// if <paramref name="value" /> is found; otherwise, a negative number, which is the bitwise
        /// complement of the index of the next element that is larger than <paramref name="value" />
        /// or, if there is no larger element, the bitwise complement of <see cref="Count" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparer" /> is null and neither <paramref name="value" /> nor the elements
        /// of <see cref="ArrayList" /> implement the <see cref="IComparable" />
        /// interface.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="comparer" /> is null and <paramref name="value" /> is not of the same type
        /// as the elements of the <see cref="ArrayList" />.
        /// </exception>
        public virtual int BinarySearch(object value, System.Collections.IComparer comparer) { return default(int); }
        /// <summary>
        /// Removes all elements from the <see cref="ArrayList" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void Clear() { }
        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayList" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="ArrayList" />.
        /// </returns>
        public virtual object Clone() { return default(object); }
        /// <summary>
        /// Determines whether an element is in the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="item">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="ArrayList" />
        /// ; otherwise, false.
        /// </returns>
        public virtual bool Contains(object item) { return default(bool); }
        /// <summary>
        /// Copies the entire <see cref="ArrayList" /> to a compatible one-dimensional
        /// <see cref="Array" />, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="ArrayList" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="ArrayList" /> is greater than the number of elements that the destination
        /// <paramref name="array" /> can contain.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ArrayList" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array) { }
        /// <summary>
        /// Copies the entire <see cref="ArrayList" /> to a compatible one-dimensional
        /// <see cref="Array" />, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="ArrayList" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="ArrayList" /> is greater than the available space from <paramref name="arrayIndex" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ArrayList" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int arrayIndex) { }
        /// <summary>
        /// Copies a range of elements from the <see cref="ArrayList" /> to a compatible
        /// one-dimensional <see cref="Array" />, starting at the specified index of the target
        /// array.
        /// </summary>
        /// <param name="index">
        /// The zero-based index in the source <see cref="ArrayList" /> at which
        /// copying begins.
        /// </param>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="ArrayList" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="arrayIndex" /> is less than
        /// zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- <paramref name="index" /> is equal to or
        /// greater than the <see cref="Count" /> of the source
        /// <see cref="ArrayList" />.-or- The number of elements from <paramref name="index" /> to the end of the source
        /// <see cref="ArrayList" /> is greater than the available space from
        /// <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ArrayList" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(int index, System.Array array, int arrayIndex, int count) { }
        /// <summary>
        /// Returns an <see cref="ArrayList" /> wrapper with a fixed size.
        /// </summary>
        /// <param name="list">The <see cref="ArrayList" /> to wrap.</param>
        /// <returns>
        /// An <see cref="ArrayList" /> wrapper with a fixed size.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.ArrayList FixedSize(System.Collections.ArrayList list) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Returns an <see cref="IList" /> wrapper with a fixed size.
        /// </summary>
        /// <param name="list">The <see cref="IList" /> to wrap.</param>
        /// <returns>
        /// An <see cref="IList" /> wrapper with a fixed size.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.IList FixedSize(System.Collections.IList list) { return default(System.Collections.IList); }
        /// <summary>
        /// Returns an enumerator for the entire <see cref="ArrayList" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the entire <see cref="ArrayList" />.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Returns an enumerator for a range of elements in the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the <see cref="ArrayList" /> section
        /// that the enumerator should refer to.
        /// </param>
        /// <param name="count">
        /// The number of elements in the <see cref="ArrayList" /> section that the
        /// enumerator should refer to.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the specified range of elements in
        /// the <see cref="ArrayList" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid range in the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual System.Collections.IEnumerator GetEnumerator(int index, int count) { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Returns an <see cref="ArrayList" /> which represents a subset of the
        /// elements in the source <see cref="ArrayList" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based <see cref="ArrayList" /> index at which the range starts.
        /// </param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// An <see cref="ArrayList" /> which represents a subset of the elements
        /// in the source <see cref="ArrayList" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="ArrayList" />.
        /// </exception>
        public virtual System.Collections.ArrayList GetRange(int index, int count) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the first occurrence within the entire <see cref="ArrayList" />.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value" /> within the entire
        /// <see cref="ArrayList" />, if found; otherwise, -1.
        /// </returns>
        public virtual int IndexOf(object value) { return default(int); }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the first occurrence within the range of elements in the <see cref="ArrayList" />
        /// that extends from the specified index to the last element.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value" /> within the range
        /// of elements in the <see cref="ArrayList" /> that extends from <paramref name="startIndex" />
        /// to the last element, if found; otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual int IndexOf(object value, int startIndex) { return default(int); }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the first occurrence within the range of elements in the <see cref="ArrayList" />
        /// that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value" /> within the range
        /// of elements in the <see cref="ArrayList" /> that starts at <paramref name="startIndex" />
        /// and contains <paramref name="count" /> number of elements, if found;
        /// otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="ArrayList" />.-or- <paramref name="count" /> is less than zero.-or- <paramref name="startIndex" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual int IndexOf(object value, int startIndex, int count) { return default(int); }
        /// <summary>
        /// Inserts an element into the <see cref="ArrayList" /> at the specified
        /// index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The <see cref="Object" /> to insert. The value can be null.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is greater than
        /// <see cref="Count" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void Insert(int index, object value) { }
        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ArrayList" />
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="c">
        /// The <see cref="ICollection" /> whose elements should be inserted into
        /// the <see cref="ArrayList" />. The collection itself cannot be null, but
        /// it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="c" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is greater than
        /// <see cref="Count" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void InsertRange(int index, System.Collections.ICollection c) { }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the last occurrence within the entire <see cref="ArrayList" />.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="value" /> within the entire
        /// the <see cref="ArrayList" />, if found; otherwise, -1.
        /// </returns>
        public virtual int LastIndexOf(object value) { return default(int); }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the last occurrence within the range of elements in the <see cref="ArrayList" />
        /// that extends from the first element to the specified index.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="value" /> within the range
        /// of elements in the <see cref="ArrayList" /> that extends from the first
        /// element to <paramref name="startIndex" />, if found; otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual int LastIndexOf(object value, int startIndex) { return default(int); }
        /// <summary>
        /// Searches for the specified <see cref="Object" /> and returns the zero-based index
        /// of the last occurrence within the range of elements in the <see cref="ArrayList" />
        /// that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to locate in the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="value" /> within the range
        /// of elements in the <see cref="ArrayList" /> that contains <paramref name="count" />
        /// number of elements and ends at <paramref name="startIndex" />, if found; otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> is outside the range of valid indexes for the
        /// <see cref="ArrayList" />.-or- <paramref name="count" /> is less than zero.-or- <paramref name="startIndex" /> and
        /// <paramref name="count" /> do not specify a valid section in the
        /// <see cref="ArrayList" />.
        /// </exception>
        public virtual int LastIndexOf(object value, int startIndex, int count) { return default(int); }
        /// <summary>
        /// Returns a read-only <see cref="ArrayList" /> wrapper.
        /// </summary>
        /// <param name="list">The <see cref="ArrayList" /> to wrap.</param>
        /// <returns>
        /// A read-only <see cref="ArrayList" /> wrapper around <paramref name="list" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.ArrayList ReadOnly(System.Collections.ArrayList list) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Returns a read-only <see cref="IList" /> wrapper.
        /// </summary>
        /// <param name="list">The <see cref="IList" /> to wrap.</param>
        /// <returns>
        /// A read-only <see cref="IList" /> wrapper around <paramref name="list" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.IList ReadOnly(System.Collections.IList list) { return default(System.Collections.IList); }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="Object" /> to remove from the <see cref="ArrayList" />.
        /// The value can be null.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void Remove(object obj) { }
        /// <summary>
        /// Removes the element at the specified index of the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> is equal to or
        /// greater than <see cref="Count" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void RemoveAt(int index) { }
        /// <summary>
        /// Removes a range of elements from the <see cref="ArrayList" />.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="ArrayList" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void RemoveRange(int index, int count) { }
        /// <summary>
        /// Returns an <see cref="ArrayList" /> whose elements are copies of the
        /// specified value.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Object" /> to copy multiple times in the new
        /// <see cref="ArrayList" />. The value can be null.
        /// </param>
        /// <param name="count">The number of times <paramref name="value" /> should be copied.</param>
        /// <returns>
        /// An <see cref="ArrayList" /> with <paramref name="count" /> number of
        /// elements, all of which are copies of <paramref name="value" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count" /> is less than zero.
        /// </exception>
        public static System.Collections.ArrayList Repeat(object value, int count) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="ArrayList" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        public virtual void Reverse() { }
        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range of elements
        /// in the <see cref="ArrayList" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        public virtual void Reverse(int index, int count) { }
        /// <summary>
        /// Copies the elements of a collection over a range of elements in the
        /// <see cref="ArrayList" />.
        /// </summary>
        /// <param name="index">
        /// The zero-based <see cref="ArrayList" /> index at which to start copying
        /// the elements of <paramref name="c" />.
        /// </param>
        /// <param name="c">
        /// The <see cref="ICollection" /> whose elements to copy to the
        /// <see cref="ArrayList" />. The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="index" /> plus the number
        /// of elements in <paramref name="c" /> is greater than <see cref="Count" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="c" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        public virtual void SetRange(int index, System.Collections.ICollection c) { }
        /// <summary>
        /// Sorts the elements in the entire <see cref="ArrayList" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        public virtual void Sort() { }
        /// <summary>
        /// Sorts the elements in the entire <see cref="ArrayList" /> using the specified
        /// comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing elements.-or-
        /// A null reference (Nothing in Visual Basic) to use the <see cref="IComparable" />
        /// implementation of each element.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        /// <exception cref="InvalidOperationException">An error occurred while comparing two elements.</exception>
        /// <exception cref="ArgumentException">
        /// null is passed for <paramref name="comparer" />, and the elements in the list do not implement
        /// <see cref="IComparable" />.
        /// </exception>
        public virtual void Sort(System.Collections.IComparer comparer) { }
        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="ArrayList" />
        /// using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing elements.-or-
        /// A null reference (Nothing in Visual Basic) to use the <see cref="IComparable" />
        /// implementation of each element.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or- <paramref name="count" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> and <paramref name="count" /> do not specify a valid range in the
        /// <see cref="ArrayList" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.
        /// </exception>
        /// <exception cref="InvalidOperationException">An error occurred while comparing two elements.</exception>
        public virtual void Sort(int index, int count, System.Collections.IComparer comparer) { }
        /// <summary>
        /// Returns an <see cref="ArrayList" /> wrapper that is synchronized (thread
        /// safe).
        /// </summary>
        /// <param name="list">The <see cref="ArrayList" /> to synchronize.</param>
        /// <returns>
        /// An <see cref="ArrayList" /> wrapper that is synchronized (thread safe).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.ArrayList Synchronized(System.Collections.ArrayList list) { return default(System.Collections.ArrayList); }
        /// <summary>
        /// Returns an <see cref="IList" /> wrapper that is synchronized (thread
        /// safe).
        /// </summary>
        /// <param name="list">The <see cref="IList" /> to synchronize.</param>
        /// <returns>
        /// An <see cref="IList" /> wrapper that is synchronized (thread safe).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.IList Synchronized(System.Collections.IList list) { return default(System.Collections.IList); }
        /// <summary>
        /// Copies the elements of the <see cref="ArrayList" /> to a new
        /// <see cref="Object" /> array.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> array containing copies of the elements of the
        /// <see cref="ArrayList" />.
        /// </returns>
        public virtual object[] ToArray() { return default(object[]); }
        /// <summary>
        /// Copies the elements of the <see cref="ArrayList" /> to a new array of
        /// the specified element type.
        /// </summary>
        /// <param name="type">
        /// The element <see cref="Type" /> of the destination array to create and copy elements
        /// to.
        /// </param>
        /// <returns>
        /// An array of the specified element type containing copies of the elements of the
        /// <see cref="ArrayList" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="ArrayList" /> cannot be cast automatically
        /// to the specified type.
        /// </exception>
        public virtual System.Array ToArray(System.Type type) { return default(System.Array); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="ArrayList" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="ArrayList" /> is read-only.-or- The
        /// <see cref="ArrayList" /> has a fixed size.
        /// </exception>
        public virtual void TrimToSize() { }
    }
    /// <summary>
    /// Compares two objects for equivalence, ignoring the case of strings.
    /// </summary>
    public partial class CaseInsensitiveComparer : System.Collections.IComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseInsensitiveComparer" />
        /// class using the <see cref="Threading.Thread.CurrentCulture" /> of the current
        /// thread.
        /// </summary>
        public CaseInsensitiveComparer() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseInsensitiveComparer" />
        /// class using the specified <see cref="Globalization.CultureInfo" />.
        /// </summary>
        /// <param name="culture">
        /// The <see cref="Globalization.CultureInfo" /> to use for the new
        /// <see cref="CaseInsensitiveComparer" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="culture" /> is null.</exception>
        public CaseInsensitiveComparer(System.Globalization.CultureInfo culture) { }
        /// <summary>
        /// Gets an instance of <see cref="CaseInsensitiveComparer" /> that is associated
        /// with the <see cref="Threading.Thread.CurrentCulture" /> of the current thread and
        /// that is always available.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CaseInsensitiveComparer" /> that is associated
        /// with the <see cref="Threading.Thread.CurrentCulture" /> of the current thread.
        /// </returns>
        public static System.Collections.CaseInsensitiveComparer Default { get { return default(System.Collections.CaseInsensitiveComparer); } }
        /// <summary>
        /// Gets an instance of <see cref="CaseInsensitiveComparer" /> that is associated
        /// with <see cref="Globalization.CultureInfo.InvariantCulture" /> and that is always
        /// available.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CaseInsensitiveComparer" /> that is associated
        /// with <see cref="Globalization.CultureInfo.InvariantCulture" />.
        /// </returns>
        public static System.Collections.CaseInsensitiveComparer DefaultInvariant { get { return default(System.Collections.CaseInsensitiveComparer); } }
        /// <summary>
        /// Performs a case-insensitive comparison of two objects of the same type and returns a value
        /// indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="a" /> and <paramref name="b" />,
        /// as shown in the following table.Value Meaning Less than zero <paramref name="a" />
        /// is less than <paramref name="b" />, with casing ignored. Zero <paramref name="a" /> equals
        /// <paramref name="b" />, with casing ignored. Greater than zero <paramref name="a" /> is greater
        /// than <paramref name="b" />, with casing ignored.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Neither <paramref name="a" /> nor <paramref name="b" /> implements the <see cref="IComparable" />
        /// interface.-or- <paramref name="a" /> and <paramref name="b" /> are of different types.
        /// </exception>
        public int Compare(object a, object b) { return default(int); }
    }
    /// <summary>
    /// Provides the abstract base class for a strongly typed collection.
    /// </summary>
    public abstract partial class CollectionBase : System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionBase" /> class
        /// with the default initial capacity.
        /// </summary>
        protected CollectionBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionBase" /> class
        /// with the specified capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        protected CollectionBase(int capacity) { }
        /// <summary>
        /// Gets or sets the number of elements that the <see cref="CollectionBase" />
        /// can contain.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="CollectionBase" /> can contain.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="Capacity" /> is set to a value that is less
        /// than <see cref="Count" />.
        /// </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public int Capacity { get { return default(int); } set { } }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="CollectionBase" />
        /// instance. This property cannot be overridden.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="CollectionBase" />
        /// instance.Retrieving the value of this property is an O(1) operation.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets an <see cref="ArrayList" /> containing the list of elements in the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="ArrayList" /> representing the
        /// <see cref="CollectionBase" /> instance itself.Retrieving the value of this property is an O(1) operation.
        /// </returns>
        protected System.Collections.ArrayList InnerList { get { return default(System.Collections.ArrayList); } }
        /// <summary>
        /// Gets an <see cref="IList" /> containing the list of elements in the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IList" /> representing the
        /// <see cref="CollectionBase" /> instance itself.
        /// </returns>
        protected System.Collections.IList List { get { return default(System.Collections.IList); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IList.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IList.IsReadOnly { get { return default(bool); } }
        object System.Collections.IList.this[int index] { get { return default(object); } set { } }
        /// <summary>
        /// Removes all objects from the <see cref="CollectionBase" /> instance.
        /// This method cannot be overridden.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CollectionBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the <see cref="CollectionBase" />
        /// instance.
        /// </returns>
        public System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Performs additional custom processes when clearing the contents of the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        protected virtual void OnClear() { }
        /// <summary>
        /// Performs additional custom processes after clearing the contents of the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        protected virtual void OnClearComplete() { }
        /// <summary>
        /// Performs additional custom processes before inserting a new element into the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value" />.</param>
        /// <param name="value">The new value of the element at <paramref name="index" />.</param>
        protected virtual void OnInsert(int index, object value) { }
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value" />.</param>
        /// <param name="value">The new value of the element at <paramref name="index" />.</param>
        protected virtual void OnInsertComplete(int index, object value) { }
        /// <summary>
        /// Performs additional custom processes when removing an element from the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> can be found.</param>
        /// <param name="value">The value of the element to remove from <paramref name="index" />.</param>
        protected virtual void OnRemove(int index, object value) { }
        /// <summary>
        /// Performs additional custom processes after removing an element from the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> can be found.</param>
        /// <param name="value">The value of the element to remove from <paramref name="index" />.</param>
        protected virtual void OnRemoveComplete(int index, object value) { }
        /// <summary>
        /// Performs additional custom processes before setting a value in the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue" /> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue" />.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index" />.</param>
        protected virtual void OnSet(int index, object oldValue, object newValue) { }
        /// <summary>
        /// Performs additional custom processes after setting a value in the
        /// <see cref="CollectionBase" /> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue" /> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue" />.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index" />.</param>
        protected virtual void OnSetComplete(int index, object oldValue, object newValue) { }
        /// <summary>
        /// Performs additional custom processes when validating a value.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null.</exception>
        protected virtual void OnValidate(object value) { }
        /// <summary>
        /// Removes the element at the specified index of the <see cref="CollectionBase" />
        /// instance. This method is not overridable.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.-or-<paramref name="index" /> is equal to or greater
        /// than <see cref="Count" />.
        /// </exception>
        public void RemoveAt(int index) { }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        int System.Collections.IList.Add(object value) { return default(int); }
        bool System.Collections.IList.Contains(object value) { return default(bool); }
        int System.Collections.IList.IndexOf(object value) { return default(int); }
        void System.Collections.IList.Insert(int index, object value) { }
        void System.Collections.IList.Remove(object value) { }
    }
    /// <summary>
    /// Compares two objects for equivalence, where string comparisons are case-sensitive.
    /// </summary>
    public sealed partial class Comparer : System.Collections.IComparer
    {
        /// <summary>
        /// Represents an instance of <see cref="Comparer" /> that is associated
        /// with the <see cref="Threading.Thread.CurrentCulture" /> of the current thread. This
        /// field is read-only.
        /// </summary>
        public static readonly System.Collections.Comparer Default;
        /// <summary>
        /// Represents an instance of <see cref="Comparer" /> that is associated
        /// with <see cref="Globalization.CultureInfo.InvariantCulture" />. This field is read-only.
        /// </summary>
        public static readonly System.Collections.Comparer DefaultInvariant;
        /// <summary>
        /// Initializes a new instance of the <see cref="Comparer" /> class using
        /// the specified <see cref="Globalization.CultureInfo" />.
        /// </summary>
        /// <param name="culture">
        /// The <see cref="Globalization.CultureInfo" /> to use for the new
        /// <see cref="Comparer" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="culture" /> is null.</exception>
        public Comparer(System.Globalization.CultureInfo culture) { }
        /// <summary>
        /// Performs a case-sensitive comparison of two objects of the same type and returns a value indicating
        /// whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="a" /> and <paramref name="b" />,
        /// as shown in the following table.Value Meaning Less than zero <paramref name="a" />
        /// is less than <paramref name="b" />. Zero <paramref name="a" /> equals <paramref name="b" />.
        /// Greater than zero <paramref name="a" /> is greater than <paramref name="b" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Neither <paramref name="a" /> nor <paramref name="b" /> implements the <see cref="IComparable" />
        /// interface.-or- <paramref name="a" /> and <paramref name="b" /> are of different types and
        /// neither one can handle comparisons with the other.
        /// </exception>
        public int Compare(object a, object b) { return default(int); }
    }
    /// <summary>
    /// Provides the abstract base class for a strongly typed collection of key/value pairs.
    /// </summary>
    public abstract partial class DictionaryBase : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBase" /> class.
        /// </summary>
        protected DictionaryBase() { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="DictionaryBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="DictionaryBase" />
        /// instance.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets the list of elements contained in the <see cref="DictionaryBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionary" /> representing the
        /// <see cref="DictionaryBase" /> instance itself.
        /// </returns>
        protected System.Collections.IDictionary Dictionary { get { return default(System.Collections.IDictionary); } }
        /// <summary>
        /// Gets the list of elements contained in the <see cref="DictionaryBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// A <see cref="Hashtable" /> representing the
        /// <see cref="DictionaryBase" /> instance itself.
        /// </returns>
        protected System.Collections.Hashtable InnerHashtable { get { return default(System.Collections.Hashtable); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        bool System.Collections.IDictionary.IsFixedSize { get { return default(bool); } }
        bool System.Collections.IDictionary.IsReadOnly { get { return default(bool); } }
        object System.Collections.IDictionary.this[object key] { get { return default(object); } set { } }
        System.Collections.ICollection System.Collections.IDictionary.Keys { get { return default(System.Collections.ICollection); } }
        System.Collections.ICollection System.Collections.IDictionary.Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Clears the contents of the <see cref="DictionaryBase" /> instance.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Copies the <see cref="DictionaryBase" /> elements to a one-dimensional
        /// <see cref="Array" /> at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from the <see cref="DictionaryBase" /> instance. The
        /// <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="DictionaryBase" /> is greater than the available space from <paramref name="index" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="DictionaryBase" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that iterates through
        /// the <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for the
        /// <see cref="DictionaryBase" /> instance.
        /// </returns>
        public System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Performs additional custom processes before clearing the contents of the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        protected virtual void OnClear() { }
        /// <summary>
        /// Performs additional custom processes after clearing the contents of the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        protected virtual void OnClearComplete() { }
        /// <summary>
        /// Gets the element with the specified key and value in the <see cref="DictionaryBase" />
        /// instance.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <param name="currentValue">The current value of the element associated with <paramref name="key" />.</param>
        /// <returns>
        /// An <see cref="Object" /> containing the element with the specified key and value.
        /// </returns>
        protected virtual object OnGet(object key, object currentValue) { return default(object); }
        /// <summary>
        /// Performs additional custom processes before inserting a new element into the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        protected virtual void OnInsert(object key, object value) { }
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to insert.</param>
        /// <param name="value">The value of the element to insert.</param>
        protected virtual void OnInsertComplete(object key, object value) { }
        /// <summary>
        /// Performs additional custom processes before removing an element from the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element to remove.</param>
        protected virtual void OnRemove(object key, object value) { }
        /// <summary>
        /// Performs additional custom processes after removing an element from the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element to remove.</param>
        protected virtual void OnRemoveComplete(object key, object value) { }
        /// <summary>
        /// Performs additional custom processes before setting a value in the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to locate.</param>
        /// <param name="oldValue">The old value of the element associated with <paramref name="key" />.</param>
        /// <param name="newValue">The new value of the element associated with <paramref name="key" />.</param>
        protected virtual void OnSet(object key, object oldValue, object newValue) { }
        /// <summary>
        /// Performs additional custom processes after setting a value in the
        /// <see cref="DictionaryBase" /> instance.
        /// </summary>
        /// <param name="key">The key of the element to locate.</param>
        /// <param name="oldValue">The old value of the element associated with <paramref name="key" />.</param>
        /// <param name="newValue">The new value of the element associated with <paramref name="key" />.</param>
        protected virtual void OnSetComplete(object key, object oldValue, object newValue) { }
        /// <summary>
        /// Performs additional custom processes when validating the element with the specified key and
        /// value.
        /// </summary>
        /// <param name="key">The key of the element to validate.</param>
        /// <param name="value">The value of the element to validate.</param>
        protected virtual void OnValidate(object key, object value) { }
        void System.Collections.IDictionary.Add(object key, object value) { }
        bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
        void System.Collections.IDictionary.Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Represents a collection of key/value pairs that are organized based on the hash code of the
    /// key.To browse the .NET Framework source code for this type, see the Reference Source.
    /// </summary>
    public partial class Hashtable : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the default initial capacity, load factor, hash code provider, and comparer.
        /// </summary>
        public Hashtable() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Hashtable" /> class by copying
        /// the elements from the specified dictionary to the new <see cref="Hashtable" />
        /// object. The new <see cref="Hashtable" /> object has an initial capacity
        /// equal to the number of elements copied, and uses the default load factor, hash code provider,
        /// and comparer.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> object to copy to a new
        /// <see cref="Hashtable" /> object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        public Hashtable(System.Collections.IDictionary d) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Hashtable" /> class by copying
        /// the elements from the specified dictionary to a new <see cref="Hashtable" />
        /// object. The new <see cref="Hashtable" /> object has an initial capacity
        /// equal to the number of elements copied, and uses the default load factor and the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> object to copy to a new
        /// <see cref="Hashtable" /> object.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object that defines the hash code
        /// provider and the comparer to use with the <see cref="Hashtable" />.-or-
        /// null to use the default hash code provider and the default comparer. The default hash code
        /// provider is each key's implementation of <see cref="Object.GetHashCode" /> and the
        /// default comparer is each key's implementation of <see cref="Object.Equals(Object)" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        public Hashtable(System.Collections.IDictionary d, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Hashtable" /> class by copying
        /// the elements from the specified dictionary to the new <see cref="Hashtable" />
        /// object. The new <see cref="Hashtable" /> object has an initial capacity
        /// equal to the number of elements copied, and uses the specified load factor, and the default
        /// hash code provider and comparer.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> object to copy to a new
        /// <see cref="Hashtable" /> object.
        /// </param>
        /// <param name="loadFactor">
        /// A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides
        /// the best performance. The result is the maximum ratio of elements to buckets.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="loadFactor" /> is less than 0.1.-or- <paramref name="loadFactor" /> is greater
        /// than 1.0.
        /// </exception>
        public Hashtable(System.Collections.IDictionary d, float loadFactor) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Hashtable" /> class by copying
        /// the elements from the specified dictionary to the new <see cref="Hashtable" />
        /// object. The new <see cref="Hashtable" /> object has an initial capacity
        /// equal to the number of elements copied, and uses the specified load factor and
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> object to copy to a new
        /// <see cref="Hashtable" /> object.
        /// </param>
        /// <param name="loadFactor">
        /// A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides
        /// the best performance. The result is the maximum ratio of elements to buckets.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object that defines the hash code
        /// provider and the comparer to use with the <see cref="Hashtable" />.-or-
        /// null to use the default hash code provider and the default comparer. The default hash code
        /// provider is each key's implementation of <see cref="Object.GetHashCode" /> and the
        /// default comparer is each key's implementation of <see cref="Object.Equals(Object)" />.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="loadFactor" /> is less than 0.1.-or- <paramref name="loadFactor" /> is greater
        /// than 1.0.
        /// </exception>
        public Hashtable(System.Collections.IDictionary d, float loadFactor, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the default initial capacity and load factor, and the specified
        /// <see cref="IEqualityComparer" /> object.
        /// </summary>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object that defines the hash code
        /// provider and the comparer to use with the <see cref="Hashtable" /> object.-or-
        /// null to use the default hash code provider and the default comparer. The default hash code
        /// provider is each key's implementation of <see cref="Object.GetHashCode" /> and the
        /// default comparer is each key's implementation of <see cref="Object.Equals(Object)" />.
        /// </param>
        public Hashtable(System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the specified initial capacity, and the default load factor, hash code provider, and
        /// comparer.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of elements that the <see cref="Hashtable" />
        /// object can initially contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public Hashtable(int capacity) { }
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the specified initial capacity and <see cref="IEqualityComparer" />,
        /// and the default load factor.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of elements that the <see cref="Hashtable" />
        /// object can initially contain.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object that defines the hash code
        /// provider and the comparer to use with the <see cref="Hashtable" />.-or-
        /// null to use the default hash code provider and the default comparer. The default hash code
        /// provider is each key's implementation of <see cref="Object.GetHashCode" /> and the
        /// default comparer is each key's implementation of <see cref="Object.Equals(Object)" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public Hashtable(int capacity, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the specified initial capacity and load factor, and the default hash code provider and
        /// comparer.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of elements that the <see cref="Hashtable" />
        /// object can initially contain.
        /// </param>
        /// <param name="loadFactor">
        /// A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides
        /// the best performance. The result is the maximum ratio of elements to buckets.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.-or- <paramref name="loadFactor" /> is less
        /// than 0.1.-or- <paramref name="loadFactor" /> is greater than 1.0.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="capacity" /> is causing an overflow.</exception>
        public Hashtable(int capacity, float loadFactor) { }
        /// <summary>
        /// Initializes a new, empty instance of the <see cref="Hashtable" /> class
        /// using the specified initial capacity, load factor, and <see cref="IEqualityComparer" />
        /// object.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of elements that the <see cref="Hashtable" />
        /// object can initially contain.
        /// </param>
        /// <param name="loadFactor">
        /// A number in the range from 0.1 through 1.0 that is multiplied by the default value which provides
        /// the best performance. The result is the maximum ratio of elements to buckets.
        /// </param>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer" /> object that defines the hash code
        /// provider and the comparer to use with the <see cref="Hashtable" />.-or-
        /// null to use the default hash code provider and the default comparer. The default hash code
        /// provider is each key's implementation of <see cref="Object.GetHashCode" /> and the
        /// default comparer is each key's implementation of <see cref="Object.Equals(Object)" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.-or- <paramref name="loadFactor" /> is less
        /// than 0.1.-or- <paramref name="loadFactor" /> is greater than 1.0.
        /// </exception>
        public Hashtable(int capacity, float loadFactor, System.Collections.IEqualityComparer equalityComparer) { }
        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="Hashtable" />.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets the <see cref="IEqualityComparer" /> to use for the
        /// <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// The <see cref="IEqualityComparer" /> to use for the
        /// <see cref="Hashtable" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The property is set to a value, but the hash table was created using an
        /// <see cref="IHashCodeProvider" /> and an <see cref="IComparer" />.
        /// </exception>
        protected System.Collections.IEqualityComparer EqualityComparer { get { return default(System.Collections.IEqualityComparer); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="Hashtable" /> has a fixed
        /// size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Hashtable" /> has a fixed size; otherwise, false.
        /// The default is false.
        /// </returns>
        public virtual bool IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether the <see cref="Hashtable" /> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Hashtable" /> is read-only; otherwise, false.
        /// The default is false.
        /// </returns>
        public virtual bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="Hashtable" />
        /// is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="Hashtable" /> is synchronized (thread
        /// safe); otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found, attempting
        /// to get it returns null, and attempting to set it creates a new element using the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="Hashtable" /> is read-only.-or-
        /// The property is set, <paramref name="key" /> does not exist in the collection, and the
        /// <see cref="Hashtable" /> has a fixed size.
        /// </exception>
        public virtual object this[object key] { get { return default(object); } set { } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the keys in the
        /// <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the keys in the
        /// <see cref="Hashtable" />.
        /// </returns>
        public virtual System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="Hashtable" />.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Gets an <see cref="ICollection" /> containing the values in the
        /// <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> containing the values in the
        /// <see cref="Hashtable" />.
        /// </returns>
        public virtual System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="Hashtable" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="Hashtable" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Hashtable" /> is read-only.-or- The
        /// <see cref="Hashtable" /> has a fixed size.
        /// </exception>
        public virtual void Add(object key, object value) { }
        /// <summary>
        /// Removes all elements from the <see cref="Hashtable" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Hashtable" /> is read-only.
        /// </exception>
        public virtual void Clear() { }
        /// <summary>
        /// Creates a shallow copy of the <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Hashtable" />.
        /// </returns>
        public virtual object Clone() { return default(object); }
        /// <summary>
        /// Determines whether the <see cref="Hashtable" /> contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Hashtable" />.</param>
        /// <returns>
        /// true if the <see cref="Hashtable" /> contains an element with the specified
        /// key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public virtual bool Contains(object key) { return default(bool); }
        /// <summary>
        /// Determines whether the <see cref="Hashtable" /> contains a specific key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Hashtable" />.</param>
        /// <returns>
        /// true if the <see cref="Hashtable" /> contains an element with the specified
        /// key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public virtual bool ContainsKey(object key) { return default(bool); }
        /// <summary>
        /// Determines whether the <see cref="Hashtable" /> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="Hashtable" />. The value can be
        /// null.
        /// </param>
        /// <returns>
        /// true if the <see cref="Hashtable" /> contains an element with the specified
        /// <paramref name="value" />; otherwise, false.
        /// </returns>
        public virtual bool ContainsValue(object value) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="Hashtable" /> elements to a one-dimensional
        /// <see cref="Array" /> instance at the specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from <see cref="Hashtable" />. The
        /// <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="Hashtable" /> is greater than the available space from <paramref name="arrayIndex" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="Hashtable" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int arrayIndex) { }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> that iterates through
        /// the <see cref="Hashtable" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> for the
        /// <see cref="Hashtable" />.
        /// </returns>
        public virtual System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Returns the hash code for the specified key.
        /// </summary>
        /// <param name="key">The <see cref="Object" /> for which a hash code is to be returned.</param>
        /// <returns>
        /// The hash code for <paramref name="key" />.
        /// </returns>
        /// <exception cref="NullReferenceException"><paramref name="key" /> is null.</exception>
        protected virtual int GetHash(object key) { return default(int); }
        /// <summary>
        /// Compares a specific <see cref="Object" /> with a specific key in the
        /// <see cref="Hashtable" />.
        /// </summary>
        /// <param name="item">The <see cref="Object" /> to compare with <paramref name="key" />.</param>
        /// <param name="key">
        /// The key in the <see cref="Hashtable" /> to compare with <paramref name="item" />.
        /// </param>
        /// <returns>
        /// true if <paramref name="item" /> and <paramref name="key" /> are equal; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is null.-or- <paramref name="key" /> is null.
        /// </exception>
        protected virtual bool KeyEquals(object item, object key) { return default(bool); }
        /// <summary>
        /// Removes the element with the specified key from the <see cref="Hashtable" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Hashtable" /> is read-only.-or- The
        /// <see cref="Hashtable" /> has a fixed size.
        /// </exception>
        public virtual void Remove(object key) { }
        /// <summary>
        /// Returns a synchronized (thread-safe) wrapper for the <see cref="Hashtable" />.
        /// </summary>
        /// <param name="table">The <see cref="Hashtable" /> to synchronize.</param>
        /// <returns>
        /// A synchronized (thread-safe) wrapper for the <see cref="Hashtable" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="table" /> is null.</exception>
        public static System.Collections.Hashtable Synchronized(System.Collections.Hashtable table) { return default(System.Collections.Hashtable); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
    }
    /// <summary>
    /// Represents a first-in, first-out collection of objects.
    /// </summary>
    public partial class Queue : System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue" /> class that is
        /// empty, has the default initial capacity, and uses the default growth factor.
        /// </summary>
        public Queue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue" /> class that contains
        /// elements copied from the specified collection, has the same initial capacity as the number
        /// of elements copied, and uses the default growth factor.
        /// </summary>
        /// <param name="col">The <see cref="ICollection" /> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="col" /> is null.</exception>
        public Queue(System.Collections.ICollection col) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue" /> class that is
        /// empty, has the specified initial capacity, and uses the default growth factor.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Queue" /> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public Queue(int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue" /> class that is
        /// empty, has the specified initial capacity, and uses the specified growth factor.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="Queue" /> can contain.
        /// </param>
        /// <param name="growFactor">
        /// The factor by which the capacity of the <see cref="Queue" /> is expanded.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.-or- <paramref name="growFactor" /> is less
        /// than 1.0 or greater than 10.0.
        /// </exception>
        public Queue(int capacity, float growFactor) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="Queue" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Queue" />.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="Queue" /> is
        /// synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="Queue" /> is synchronized (thread safe);
        /// otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Queue" />.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="Queue" />.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Removes all objects from the <see cref="Queue" />.
        /// </summary>
        public virtual void Clear() { }
        /// <summary>
        /// Creates a shallow copy of the <see cref="Queue" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Queue" />.
        /// </returns>
        public virtual object Clone() { return default(object); }
        /// <summary>
        /// Determines whether an element is in the <see cref="Queue" />.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="Object" /> to locate in the <see cref="Queue" />.
        /// The value can be null.
        /// </param>
        /// <returns>
        /// true if <paramref name="obj" /> is found in the <see cref="Queue" />;
        /// otherwise, false.
        /// </returns>
        public virtual bool Contains(object obj) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="Queue" /> elements to an existing one-dimensional
        /// <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="Queue" />. The <see cref="Array" /> must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="Queue" /> is greater than the available space from <paramref name="index" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="ArrayTypeMismatchException">
        /// The type of the source <see cref="Queue" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue" />.
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the <see cref="Queue" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Queue" /> is empty.
        /// </exception>
        public virtual object Dequeue() { return default(object); }
        /// <summary>
        /// Adds an object to the end of the <see cref="Queue" />.
        /// </summary>
        /// <param name="obj">The object to add to the <see cref="Queue" />. The value can be null.</param>
        public virtual void Enqueue(object obj) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Queue" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the <see cref="Queue" />.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue" /> without
        /// removing it.
        /// </summary>
        /// <returns>
        /// The object at the beginning of the <see cref="Queue" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Queue" /> is empty.
        /// </exception>
        public virtual object Peek() { return default(object); }
        /// <summary>
        /// Returns a new <see cref="Queue" /> that wraps the original queue, and
        /// is thread safe.
        /// </summary>
        /// <param name="queue">The <see cref="Queue" /> to synchronize.</param>
        /// <returns>
        /// A <see cref="Queue" /> wrapper that is synchronized (thread safe).
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="queue" /> is null.</exception>
        public static System.Collections.Queue Synchronized(System.Collections.Queue queue) { return default(System.Collections.Queue); }
        /// <summary>
        /// Copies the <see cref="Queue" /> elements to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing elements copied from the <see cref="Queue" />.
        /// </returns>
        public virtual object[] ToArray() { return default(object[]); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="Queue" />.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="Queue" /> is read-only.
        /// </exception>
        public virtual void TrimToSize() { }
    }
    /// <summary>
    /// Provides the abstract base class for a strongly typed non-generic read-only collection.
    /// </summary>
    public abstract partial class ReadOnlyCollectionBase : System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionBase" />
        /// class.
        /// </summary>
        protected ReadOnlyCollectionBase() { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ReadOnlyCollectionBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ReadOnlyCollectionBase" />
        /// instance.Retrieving the value of this property is an O(1) operation.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets the list of elements contained in the <see cref="ReadOnlyCollectionBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// An <see cref="ArrayList" /> representing the
        /// <see cref="ReadOnlyCollectionBase" /> instance itself.
        /// </returns>
        protected System.Collections.ArrayList InnerList { get { return default(System.Collections.ArrayList); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyCollectionBase" />
        /// instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the
        /// <see cref="ReadOnlyCollectionBase" /> instance.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
    }
    /// <summary>
    /// Represents a collection of key/value pairs that are sorted by the keys and are accessible by
    /// key and by index.
    /// </summary>
    public partial class SortedList : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// is empty, has the default initial capacity, and is sorted according to the <see cref="IComparable" />
        /// interface implemented by each key added to the <see cref="SortedList" />
        /// object.
        /// </summary>
        public SortedList() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// is empty, has the default initial capacity, and is sorted according to the specified
        /// <see cref="IComparer" /> interface.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing keys.-or-
        /// null to use the <see cref="IComparable" /> implementation of each key.
        /// </param>
        public SortedList(System.Collections.IComparer comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// is empty, has the specified initial capacity, and is sorted according to the specified
        /// <see cref="IComparer" /> interface.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing keys.-or-
        /// null to use the <see cref="IComparable" /> implementation of each key.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="SortedList" /> object
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// There is not enough available memory to create a <see cref="SortedList" />
        /// object with the specified <paramref name="capacity" />.
        /// </exception>
        public SortedList(System.Collections.IComparer comparer, int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// contains elements copied from the specified dictionary, has the same initial capacity as the
        /// number of elements copied, and is sorted according to the <see cref="IComparable" />
        /// interface implemented by each key.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> implementation to copy to a new
        /// <see cref="SortedList" /> object.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        /// One or more elements in <paramref name="d" /> do not implement the <see cref="IComparable" />
        /// interface.
        /// </exception>
        public SortedList(System.Collections.IDictionary d) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// contains elements copied from the specified dictionary, has the same initial capacity as the
        /// number of elements copied, and is sorted according to the specified <see cref="IComparer" />
        /// interface.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> implementation to copy to a new
        /// <see cref="SortedList" /> object.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer" /> implementation to use when comparing keys.-or-
        /// null to use the <see cref="IComparable" /> implementation of each key.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="comparer" /> is null, and one or more elements in <paramref name="d" /> do
        /// not implement the <see cref="IComparable" /> interface.
        /// </exception>
        public SortedList(System.Collections.IDictionary d, System.Collections.IComparer comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedList" /> class that
        /// is empty, has the specified initial capacity, and is sorted according to the <see cref="IComparable" />
        /// interface implemented by each key added to the <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="initialCapacity">
        /// The initial number of elements that the <see cref="SortedList" /> object
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="initialCapacity" /> is less than zero.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// There is not enough available memory to create a <see cref="SortedList" />
        /// object with the specified <paramref name="initialCapacity" />.
        /// </exception>
        public SortedList(int initialCapacity) { }
        /// <summary>
        /// Gets or sets the capacity of a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="SortedList" /> object can
        /// contain.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value assigned is less than the current number of elements in the
        /// <see cref="SortedList" /> object.
        /// </exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public virtual int Capacity { get { return default(int); } set { } }
        /// <summary>
        /// Gets the number of elements contained in a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="SortedList" /> object.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether a <see cref="SortedList" /> object has
        /// a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="SortedList" /> object has a fixed size; otherwise,
        /// false. The default is false.
        /// </returns>
        public virtual bool IsFixedSize { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether a <see cref="SortedList" /> object is
        /// read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="SortedList" /> object is read-only; otherwise,
        /// false. The default is false.
        /// </returns>
        public virtual bool IsReadOnly { get { return default(bool); } }
        /// <summary>
        /// Gets a value indicating whether access to a <see cref="SortedList" />
        /// object is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="SortedList" /> object is synchronized
        /// (thread safe); otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets and sets the value associated with a specific key in a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="key">The key associated with the value to get or set.</param>
        /// <returns>
        /// The value associated with the <paramref name="key" /> parameter in the
        /// <see cref="SortedList" /> object, if <paramref name="key" /> is found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The property is set and the <see cref="SortedList" /> object is read-only.-or-
        /// The property is set, <paramref name="key" /> does not exist in the collection, and the
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// There is not enough available memory to add the element to the <see cref="SortedList" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">The comparer throws an exception.</exception>
        public virtual object this[object key] { get { return default(object); } set { } }
        /// <summary>
        /// Gets the keys in a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> object containing the keys in the
        /// <see cref="SortedList" /> object.
        /// </returns>
        public virtual System.Collections.ICollection Keys { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="SortedList" />
        /// object.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Gets the values in a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// An <see cref="ICollection" /> object containing the values in the
        /// <see cref="SortedList" /> object.
        /// </returns>
        public virtual System.Collections.ICollection Values { get { return default(System.Collections.ICollection); } }
        /// <summary>
        /// Adds an element with the specified key and value to a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="ArgumentException">
        /// An element with the specified <paramref name="key" /> already exists in the
        /// <see cref="SortedList" /> object.-or- The <see cref="SortedList" /> is set to use the
        /// <see cref="IComparable" /> interface, and <paramref name="key" /> does not implement the
        /// <see cref="IComparable" /> interface.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedList" /> is read-only.-or- The
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// There is not enough available memory to add the element to the <see cref="SortedList" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">The comparer throws an exception.</exception>
        public virtual void Add(object key, object value) { }
        /// <summary>
        /// Removes all elements from a <see cref="SortedList" /> object.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedList" /> object is read-only.-or- The
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        public virtual void Clear() { }
        /// <summary>
        /// Creates a shallow copy of a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="SortedList" /> object.
        /// </returns>
        public virtual object Clone() { return default(object); }
        /// <summary>
        /// Determines whether a <see cref="SortedList" /> object contains a specific
        /// key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedList" /> object.</param>
        /// <returns>
        /// true if the <see cref="SortedList" /> object contains an element with
        /// the specified <paramref name="key" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The comparer throws an exception.</exception>
        public virtual bool Contains(object key) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="SortedList" /> object contains a specific
        /// key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedList" /> object.</param>
        /// <returns>
        /// true if the <see cref="SortedList" /> object contains an element with
        /// the specified <paramref name="key" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The comparer throws an exception.</exception>
        public virtual bool ContainsKey(object key) { return default(bool); }
        /// <summary>
        /// Determines whether a <see cref="SortedList" /> object contains a specific
        /// value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="SortedList" /> object. The value
        /// can be null.
        /// </param>
        /// <returns>
        /// true if the <see cref="SortedList" /> object contains an element with
        /// the specified <paramref name="value" />; otherwise, false.
        /// </returns>
        public virtual bool ContainsValue(object value) { return default(bool); }
        /// <summary>
        /// Copies <see cref="SortedList" /> elements to a one-dimensional
        /// <see cref="Array" /> object, starting at the specified index in the array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> object that is the destination of the
        /// <see cref="DictionaryEntry" /> objects copied from <see cref="SortedList" />.
        /// The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="SortedList" /> object is greater than the available space from
        /// <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="SortedList" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int arrayIndex) { }
        /// <summary>
        /// Gets the value at the specified index of a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>
        /// The value at the specified index of the <see cref="SortedList" /> object.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="SortedList" /> object.
        /// </exception>
        public virtual object GetByIndex(int index) { return default(object); }
        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator" /> object that iterates
        /// through a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// An <see cref="IDictionaryEnumerator" /> object for the
        /// <see cref="SortedList" /> object.
        /// </returns>
        public virtual System.Collections.IDictionaryEnumerator GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        /// <summary>
        /// Gets the key at the specified index of a <see cref="SortedList" /> object.
        /// </summary>
        /// <param name="index">The zero-based index of the key to get.</param>
        /// <returns>
        /// The key at the specified index of the <see cref="SortedList" /> object.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="SortedList" /> object.
        /// </exception>
        public virtual object GetKey(int index) { return default(object); }
        /// <summary>
        /// Gets the keys in a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// An <see cref="IList" /> object containing the keys in the
        /// <see cref="SortedList" /> object.
        /// </returns>
        public virtual System.Collections.IList GetKeyList() { return default(System.Collections.IList); }
        /// <summary>
        /// Gets the values in a <see cref="SortedList" /> object.
        /// </summary>
        /// <returns>
        /// An <see cref="IList" /> object containing the values in the
        /// <see cref="SortedList" /> object.
        /// </returns>
        public virtual System.Collections.IList GetValueList() { return default(System.Collections.IList); }
        /// <summary>
        /// Returns the zero-based index of the specified key in a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="SortedList" /> object.</param>
        /// <returns>
        /// The zero-based index of the <paramref name="key" /> parameter, if <paramref name="key" />
        /// is found in the <see cref="SortedList" /> object; otherwise, -1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="InvalidOperationException">The comparer throws an exception.</exception>
        public virtual int IndexOfKey(object key) { return default(int); }
        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified value in a
        /// <see cref="SortedList" /> object.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="SortedList" /> object. The value
        /// can be null.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the <paramref name="value" /> parameter, if
        /// <paramref name="value" /> is found in the <see cref="SortedList" /> object;
        /// otherwise, -1.
        /// </returns>
        public virtual int IndexOfValue(object value) { return default(int); }
        /// <summary>
        /// Removes the element with the specified key from a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedList" /> object is read-only.-or- The
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        public virtual void Remove(object key) { }
        /// <summary>
        /// Removes the element at the specified index of a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="SortedList" /> object.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedList" /> is read-only.-or- The
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        public virtual void RemoveAt(int index) { }
        /// <summary>
        /// Replaces the value at a specific index in a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="index">The zero-based index at which to save <paramref name="value" />.</param>
        /// <param name="value">
        /// The <see cref="Object" /> to save into the <see cref="SortedList" />
        /// object. The value can be null.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is outside the range of valid indexes for the
        /// <see cref="SortedList" /> object.
        /// </exception>
        public virtual void SetByIndex(int index, object value) { }
        /// <summary>
        /// Returns a synchronized (thread-safe) wrapper for a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <param name="list">The <see cref="SortedList" /> object to synchronize.</param>
        /// <returns>
        /// A synchronized (thread-safe) wrapper for the <see cref="SortedList" />
        /// object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="list" /> is null.</exception>
        public static System.Collections.SortedList Synchronized(System.Collections.SortedList list) { return default(System.Collections.SortedList); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Sets the capacity to the actual number of elements in a <see cref="SortedList" />
        /// object.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// The <see cref="SortedList" /> object is read-only.-or- The
        /// <see cref="SortedList" /> has a fixed size.
        /// </exception>
        public virtual void TrimToSize() { }
    }
    /// <summary>
    /// Represents a simple last-in-first-out (LIFO) non-generic collection of objects.
    /// </summary>
    public partial class Stack : System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack" /> class that is
        /// empty and has the default initial capacity.
        /// </summary>
        public Stack() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack" /> class that contains
        /// elements copied from the specified collection and has the same initial capacity as the number
        /// of elements copied.
        /// </summary>
        /// <param name="col">The <see cref="ICollection" /> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="col" /> is null.</exception>
        public Stack(System.Collections.ICollection col) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Stack" /> class that is
        /// empty and has the specified initial capacity or the default initial capacity, whichever is
        /// greater.
        /// </summary>
        /// <param name="initialCapacity">
        /// The initial number of elements that the <see cref="Stack" /> can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="initialCapacity" /> is less than zero.
        /// </exception>
        public Stack(int initialCapacity) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="Stack" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Stack" />.
        /// </returns>
        public virtual int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="Stack" /> is
        /// synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true, if access to the <see cref="Stack" /> is synchronized (thread safe);
        /// otherwise, false. The default is false.
        /// </returns>
        public virtual bool IsSynchronized { get { return default(bool); } }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Stack" />.
        /// </summary>
        /// <returns>
        /// An <see cref="Object" /> that can be used to synchronize access to the
        /// <see cref="Stack" />.
        /// </returns>
        public virtual object SyncRoot { get { return default(object); } }
        /// <summary>
        /// Removes all objects from the <see cref="Stack" />.
        /// </summary>
        public virtual void Clear() { }
        /// <summary>
        /// Creates a shallow copy of the <see cref="Stack" />.
        /// </summary>
        /// <returns>
        /// A shallow copy of the <see cref="Stack" />.
        /// </returns>
        public virtual object Clone() { return default(object); }
        /// <summary>
        /// Determines whether an element is in the <see cref="Stack" />.
        /// </summary>
        /// <param name="obj">
        /// The object to locate in the <see cref="Stack" />. The value can be null.
        /// </param>
        /// <returns>
        /// true, if <paramref name="obj" /> is found in the <see cref="Stack" />;
        /// otherwise, false.
        /// </returns>
        public virtual bool Contains(object obj) { return default(bool); }
        /// <summary>
        /// Copies the <see cref="Stack" /> to an existing one-dimensional
        /// <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from <see cref="Stack" />. The <see cref="Array" /> must
        /// have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array" /> is multidimensional.-or- The number of elements in the source
        /// <see cref="Stack" /> is greater than the available space from <paramref name="index" />
        /// to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The type of the source <see cref="Stack" /> cannot be cast automatically
        /// to the type of the destination <paramref name="array" />.
        /// </exception>
        public virtual void CopyTo(System.Array array, int index) { }
        /// <summary>
        /// Returns an <see cref="IEnumerator" /> for the <see cref="Stack" />.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator" /> for the <see cref="Stack" />.
        /// </returns>
        public virtual System.Collections.IEnumerator GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack" /> without removing
        /// it.
        /// </summary>
        /// <returns>
        /// The <see cref="Object" /> at the top of the <see cref="Stack" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Stack" /> is empty.
        /// </exception>
        public virtual object Peek() { return default(object); }
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack" />.
        /// </summary>
        /// <returns>
        /// The <see cref="Object" /> removed from the top of the <see cref="Stack" />.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="Stack" /> is empty.
        /// </exception>
        public virtual object Pop() { return default(object); }
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack" />.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="Object" /> to push onto the <see cref="Stack" />.
        /// The value can be null.
        /// </param>
        public virtual void Push(object obj) { }
        /// <summary>
        /// Returns a synchronized (thread safe) wrapper for the <see cref="Stack" />.
        /// </summary>
        /// <param name="stack">The <see cref="Stack" /> to synchronize.</param>
        /// <returns>
        /// A synchronized wrapper around the <see cref="Stack" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="stack" /> is null.</exception>
        public static System.Collections.Stack Synchronized(System.Collections.Stack stack) { return default(System.Collections.Stack); }
        /// <summary>
        /// Copies the <see cref="Stack" /> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing copies of the elements of the <see cref="Stack" />.
        /// </returns>
        public virtual object[] ToArray() { return default(object[]); }
    }
}
namespace System.Collections.Specialized
{
    /// <summary>
    /// Creates collections that ignore the case in strings.
    /// </summary>
    public partial class CollectionsUtil
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionsUtil" />
        /// class.
        /// </summary>
        public CollectionsUtil() { }
        /// <summary>
        /// Creates a new case-insensitive instance of the <see cref="Hashtable" />
        /// class with the default initial capacity.
        /// </summary>
        /// <returns>
        /// A new case-insensitive instance of the <see cref="Hashtable" /> class
        /// with the default initial capacity.
        /// </returns>
        public static System.Collections.Hashtable CreateCaseInsensitiveHashtable() { return default(System.Collections.Hashtable); }
        /// <summary>
        /// Copies the entries from the specified dictionary to a new case-insensitive instance of the
        /// <see cref="Hashtable" /> class with the same initial capacity as the
        /// number of entries copied.
        /// </summary>
        /// <param name="d">
        /// The <see cref="IDictionary" /> to copy to a new case-insensitive
        /// <see cref="Hashtable" />.
        /// </param>
        /// <returns>
        /// A new case-insensitive instance of the <see cref="Hashtable" /> class
        /// containing the entries from the specified <see cref="IDictionary" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="d" /> is null.</exception>
        public static System.Collections.Hashtable CreateCaseInsensitiveHashtable(System.Collections.IDictionary d) { return default(System.Collections.Hashtable); }
        /// <summary>
        /// Creates a new case-insensitive instance of the <see cref="Hashtable" />
        /// class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">
        /// The approximate number of entries that the <see cref="Hashtable" /> can
        /// initially contain.
        /// </param>
        /// <returns>
        /// A new case-insensitive instance of the <see cref="Hashtable" /> class
        /// with the specified initial capacity.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity" /> is less than zero.
        /// </exception>
        public static System.Collections.Hashtable CreateCaseInsensitiveHashtable(int capacity) { return default(System.Collections.Hashtable); }
        /// <summary>
        /// Creates a new instance of the <see cref="SortedList" /> class that ignores
        /// the case of strings.
        /// </summary>
        /// <returns>
        /// A new instance of the <see cref="SortedList" /> class that ignores the
        /// case of strings.
        /// </returns>
        public static System.Collections.SortedList CreateCaseInsensitiveSortedList() { return default(System.Collections.SortedList); }
    }
}
