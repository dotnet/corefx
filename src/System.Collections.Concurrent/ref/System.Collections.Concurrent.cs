// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------


namespace System.Collections.Concurrent
{
    /// <summary>
    /// Provides blocking and bounding capabilities for thread-safe collections that implement
    /// <see cref="IProducerConsumerCollection{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public partial class BlockingCollection<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockingCollection{T}" />
        /// class without an upper-bound.
        /// </summary>
        public BlockingCollection() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockingCollection{T}" />
        /// class without an upper-bound and using the provided
        /// <see cref="IProducerConsumerCollection{T}" /> as its underlying data store.
        /// </summary>
        /// <param name="collection">The collection to use as the underlying data store.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
        public BlockingCollection(System.Collections.Concurrent.IProducerConsumerCollection<T> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockingCollection{T}" />
        /// class with the specified upper-bound and using the provided
        /// <see cref="IProducerConsumerCollection{T}" /> as its underlying data store.
        /// </summary>
        /// <param name="collection">The collection to use as the underlying data store.</param>
        /// <param name="boundedCapacity">The bounded size of the collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="boundedCapacity" /> is not a positive value.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The supplied <paramref name="collection" /> contains more values than is permitted by
        /// <paramref name="boundedCapacity" />.
        /// </exception>
        public BlockingCollection(System.Collections.Concurrent.IProducerConsumerCollection<T> collection, int boundedCapacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockingCollection{T}" />
        /// class with the specified upper-bound.
        /// </summary>
        /// <param name="boundedCapacity">The bounded size of the collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="boundedCapacity" /> is not a positive value.
        /// </exception>
        public BlockingCollection(int boundedCapacity) { }
        /// <summary>
        /// Gets the bounded capacity of this <see cref="BlockingCollection{T}" />
        /// instance.
        /// </summary>
        /// <returns>
        /// The bounded capacity of this collection, or int.MaxValue if no bound was supplied.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public int BoundedCapacity { get { return default(int); } }
        /// <summary>
        /// Gets the number of items contained in the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <returns>
        /// The number of items contained in the <see cref="BlockingCollection{T}" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets whether this <see cref="BlockingCollection{T}" /> has
        /// been marked as complete for adding.
        /// </summary>
        /// <returns>
        /// Whether this collection has been marked as complete for adding.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public bool IsAddingCompleted { get { return default(bool); } }
        /// <summary>
        /// Gets whether this <see cref="BlockingCollection{T}" /> has
        /// been marked as complete for adding and is empty.
        /// </summary>
        /// <returns>
        /// Whether this collection has been marked as complete for adding and is empty.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public bool IsCompleted { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Adds the item to the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public void Add(T item) { }
        /// <summary>
        /// Adds the item to the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed
        /// or the <see cref="Threading.CancellationTokenSource" /> that owns <paramref name="cancellationToken" />
        /// has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public void Add(T item, System.Threading.CancellationToken cancellationToken) { }
        /// <summary>
        /// Adds the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        public static int AddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item) { return default(int); }
        /// <summary>
        /// Adds the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed, or the <see cref="Threading.CancellationTokenSource" /> that created
        /// <paramref name="cancellationToken" /> has been disposed.
        /// </exception>
        public static int AddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item, System.Threading.CancellationToken cancellationToken) { return default(int); }
        /// <summary>
        /// Marks the <see cref="BlockingCollection{T}" /> instances as
        /// not accepting any more additions.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public void CompleteAdding() { }
        /// <summary>
        /// Copies all of the items in the <see cref="BlockingCollection{T}" />
        /// instance to a compatible one-dimensional array, starting at the specified index of the
        /// target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array that is the destination of the elements copied from the
        /// <see cref="BlockingCollection{T}" /> instance. The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="array" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index" /> argument is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="index" /> argument is equal to or greater than the length of the <paramref name="array" />.
        /// The destination array is too small to hold all of the BlockingCcollection
        /// elements.The array rank doesn't match.The array type is incompatible with the type of the
        /// BlockingCollection elements.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Releases all resources used by the current instance of the
        /// <see cref="BlockingCollection{T}" /> class.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Releases resources used by the <see cref="BlockingCollection{T}" />
        /// instance.
        /// </summary>
        /// <param name="disposing">Whether being disposed explicitly (true) or due to a finalizer (false).</param>
        protected virtual void Dispose(bool disposing) { }
        /// <summary>
        /// Provides a consuming <see cref="Generic.IEnumerator{T}" /> for items in
        /// the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="Generic.IEnumerable{T}" /> that removes and returns items
        /// from the collection.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public System.Collections.Generic.IEnumerable<T> GetConsumingEnumerable() { return default(System.Collections.Generic.IEnumerable<T>); }
        /// <summary>
        /// Provides a consuming <see cref="Generic.IEnumerable{T}" /> for items in
        /// the collection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// An <see cref="Generic.IEnumerable{T}" /> that removes and returns items
        /// from the collection.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed
        /// or the <see cref="Threading.CancellationTokenSource" /> that created <paramref name="cancellationToken" />
        /// has been disposed
        /// </exception>
        public System.Collections.Generic.IEnumerable<T> GetConsumingEnumerable(System.Threading.CancellationToken cancellationToken) { return default(System.Collections.Generic.IEnumerable<T>); }
        System.Collections.Generic.IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Removes  an item from the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <returns>
        /// The item removed from the collection.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside of this
        /// <see cref="BlockingCollection{T}" /> instance, or the <see cref="BlockingCollection{T}" /> is
        /// empty and the collection has been marked as complete for adding.
        /// </exception>
        public T Take() { return default(T); }
        /// <summary>
        /// Removes an item from the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="cancellationToken">Object that can be used to cancel the take operation.</param>
        /// <returns>
        /// The item removed from the collection.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// The <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed
        /// or the <see cref="Threading.CancellationTokenSource" /> that created the token was
        /// canceled.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside of this
        /// <see cref="BlockingCollection{T}" /> instance or the BlockingCollection is marked as complete for adding, or the
        /// <see cref="BlockingCollection{T}" /> is empty.
        /// </exception>
        public T Take(System.Threading.CancellationToken cancellationToken) { return default(T); }
        /// <summary>
        /// Takes an item from any one of the specified <see cref="BlockingCollection{T}" />
        /// instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element
        /// or <see cref="BlockingCollection`1.CompleteAdding" /> has
        /// been called on the collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public static int TakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item) { item = default(T); return default(int); }
        /// <summary>
        /// Takes an item from any one of the specified <see cref="BlockingCollection{T}" />
        /// instances while observing the specified cancellation token.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or <see cref="BlockingCollection`1.CompleteAdding" /> has
        /// been called on the collection.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        public static int TakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item, System.Threading.CancellationToken cancellationToken) { item = default(T); return default(int); }
        /// <summary>
        /// Copies the items from the <see cref="BlockingCollection{T}" />
        /// instance into a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the collection.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Tries to add the specified item to the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to be added to the collection.</param>
        /// <returns>
        /// true if <paramref name="item" /> could be added; otherwise false. If the item is a duplicate,
        /// and the underlying collection does not accept duplicate items, then an
        /// <see cref="InvalidOperationException" /> is thrown.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public bool TryAdd(T item) { return default(bool); }
        /// <summary>
        /// Tries to add the specified item to the <see cref="BlockingCollection{T}" />
        /// within the specified time period.
        /// </summary>
        /// <param name="item">The item to be added to the collection.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <returns>
        /// true if the <paramref name="item" /> could be added to the collection within the specified
        /// time; otherwise, false. If the item is a duplicate, and the underlying collection does not
        /// accept duplicate items, then an <see cref="InvalidOperationException" /> is thrown.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public bool TryAdd(T item, int millisecondsTimeout) { return default(bool); }
        /// <summary>
        /// Tries to add the specified item to the <see cref="BlockingCollection{T}" />
        /// within the specified time period, while observing a cancellation token.
        /// </summary>
        /// <param name="item">The item to be added to the collection.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// true if the <paramref name="item" /> could be added to the collection within the specified
        /// time; otherwise, false. If the item is a duplicate, and the underlying collection does not
        /// accept duplicate items, then an <see cref="InvalidOperationException" /> is thrown.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed
        /// or the underlying <see cref="Threading.CancellationTokenSource" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public bool TryAdd(T item, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { return default(bool); }
        /// <summary>
        /// Tries to add the specified item to the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to be added to the collection.</param>
        /// <param name="timeout">
        /// A <see cref="TimeSpan" /> that represents the number of milliseconds to wait, or
        /// a <see cref="TimeSpan" /> that represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        /// true if the <paramref name="item" /> could be added to the collection within the specified
        /// time span; otherwise, false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents
        /// an infinite time-out -or- timeout is greater than <see cref="Int32.MaxValue" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="BlockingCollection{T}" /> has been marked as
        /// complete with regards to additions.-or-The underlying collection didn't accept the item.
        /// </exception>
        public bool TryAdd(T item, System.TimeSpan timeout) { return default(bool); }
        /// <summary>
        /// Tries to add the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added, or -1 if the item could not be added.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        public static int TryAddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item) { return default(int); }
        /// <summary>
        /// Tries to add the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added, or -1 if the item could not be added.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.-or-The count of <paramref name="collections" /> is greater than the
        /// maximum size of 62 for STA and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        public static int TryAddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item, int millisecondsTimeout) { return default(int); }
        /// <summary>
        /// Tries to add the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added, or -1 if the item could not be added.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.-or-The count of <paramref name="collections" /> is greater than the
        /// maximum size of 62 for STA and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        public static int TryAddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { return default(int); }
        /// <summary>
        /// Tries to add the specified item to any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances while observing the specified cancellation token.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item to be added to one of the collections.</param>
        /// <param name="timeout">
        /// A <see cref="TimeSpan" /> that represents the number of milliseconds to wait, or
        /// a <see cref="TimeSpan" /> that represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array to which the item
        /// was added, or -1 if the item could not be added.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// or the <see cref="Threading.CancellationTokenSource" /> that created
        /// <paramref name="cancellationToken" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents
        /// an infinite time-out -or- timeout is greater than <see cref="Int32.MaxValue" />.-or-The
        /// count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and
        /// 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element,
        /// or at least one of collections has been marked as complete for adding.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one underlying collection didn't accept the item.
        /// </exception>
        public static int TryAddToAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, T item, System.TimeSpan timeout) { return default(int); }
        /// <summary>
        /// Tries to remove an item from the <see cref="BlockingCollection{T}" />.
        /// </summary>
        /// <param name="item">The item to be removed from the collection.</param>
        /// <returns>
        /// true if an item could be removed; otherwise, false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside of this
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public bool TryTake(out T item) { item = default(T); return default(bool); }
        /// <summary>
        /// Tries to remove an item from the <see cref="BlockingCollection{T}" />
        /// in the specified time period.
        /// </summary>
        /// <param name="item">The item to be removed from the collection.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <returns>
        /// true if an item could be removed from the collection within the specified  time; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside of this
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public bool TryTake(out T item, int millisecondsTimeout) { item = default(T); return default(bool); }
        /// <summary>
        /// Tries to remove an item from the <see cref="BlockingCollection{T}" />
        /// in the specified time period while observing a cancellation token.
        /// </summary>
        /// <param name="item">The item to be removed from the collection.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// true if an item could be removed from the collection within the specified  time; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// The <see cref="Threading.CancellationToken" /> has been canceled.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed
        /// or the underlying <see cref="Threading.CancellationTokenSource" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside this
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public bool TryTake(out T item, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { item = default(T); return default(bool); }
        /// <summary>
        /// Tries to remove an item from the <see cref="BlockingCollection{T}" />
        /// in the specified time period.
        /// </summary>
        /// <param name="item">The item to be removed from the collection.</param>
        /// <param name="timeout">
        /// An object that represents the number of milliseconds to wait, or an object that represents
        /// -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        /// true if an item could be removed from the collection within the specified  time; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="BlockingCollection{T}" /> has been disposed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents
        /// an infinite time-out.-or- <paramref name="timeout" /> is greater than <see cref="Int32.MaxValue" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The underlying collection was modified outside of this
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public bool TryTake(out T item, System.TimeSpan timeout) { item = default(T); return default(bool); }
        /// <summary>
        /// Tries to remove an item from any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed, or -1 if an item could not be removed.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The count of <paramref name="collections" /> is greater than the maximum size of 62 for STA
        /// and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public static int TryTakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item) { item = default(T); return default(int); }
        /// <summary>
        /// Tries to remove an item from any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed, or -1 if an item could not be removed.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.-or-The count of <paramref name="collections" /> is greater than the
        /// maximum size of 62 for STA and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public static int TryTakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item, int millisecondsTimeout) { item = default(T); return default(int); }
        /// <summary>
        /// Tries to remove an item from any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Threading.Timeout.Infinite" />
        /// (-1) to wait indefinitely.
        /// </param>
        /// <param name="cancellationToken">A cancellation token to observe.</param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed, or -1 if an item could not be removed.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// If the <see cref="Threading.CancellationToken" /> is canceled.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout" /> is a negative number other than -1, which represents
        /// an infinite time-out.-or-The count of <paramref name="collections" /> is greater than the
        /// maximum size of 62 for STA and 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        public static int TryTakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item, int millisecondsTimeout, System.Threading.CancellationToken cancellationToken) { item = default(T); return default(int); }
        /// <summary>
        /// Tries to remove an item from any one of the specified
        /// <see cref="BlockingCollection{T}" /> instances.
        /// </summary>
        /// <param name="collections">The array of collections.</param>
        /// <param name="item">The item removed from one of the collections.</param>
        /// <param name="timeout">
        /// A <see cref="TimeSpan" /> that represents the number of milliseconds to wait, or
        /// a <see cref="TimeSpan" /> that represents -1 milliseconds to wait indefinitely.
        /// </param>
        /// <returns>
        /// The index of the collection in the <paramref name="collections" /> array from which the item
        /// was removed, or -1 if an item could not be removed.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// At least one of the <see cref="BlockingCollection{T}" /> instances
        /// has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="collections" /> argument is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents
        /// an infinite time-out -or- timeout is greater than <see cref="Int32.MaxValue" />.-or-The
        /// count of <paramref name="collections" /> is greater than the maximum size of 62 for STA and
        /// 63 for MTA.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="collections" /> argument is a 0-length array or contains a null element.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// At least one of the underlying collections was modified outside of its
        /// <see cref="BlockingCollection{T}" /> instance.
        /// </exception>
        public static int TryTakeFromAny(System.Collections.Concurrent.BlockingCollection<T>[] collections, out T item, System.TimeSpan timeout) { item = default(T); return default(int); }
    }
    /// <summary>
    /// Represents a thread-safe, unordered collection of objects.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be stored in the collection.</typeparam>
    public partial class ConcurrentBag<T> : System.Collections.Concurrent.IProducerConsumerCollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentBag{T}" />
        /// class.
        /// </summary>
        public ConcurrentBag() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentBag{T}" />
        /// class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new
        /// <see cref="ConcurrentBag{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        public ConcurrentBag(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentBag{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ConcurrentBag{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentBag{T}" />
        /// is empty.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ConcurrentBag{T}" /> is empty; otherwise,
        /// false.
        /// </returns>
        public bool IsEmpty { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Adds an object to the <see cref="ConcurrentBag{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to be added to the <see cref="ConcurrentBag{T}" />.
        /// The value can be a null reference (Nothing in Visual Basic) for reference types.
        /// </param>
        public void Add(T item) { }
        /// <summary>
        /// Copies the <see cref="ConcurrentBag{T}" /> elements to an existing
        /// one-dimensional <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from the <see cref="ConcurrentBag{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" />
        /// -or- the number of elements in the source <see cref="ConcurrentBag{T}" />
        /// is greater than the available space from <paramref name="index" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentBag{T}" />.
        /// </summary>
        /// <returns>
        /// An enumerator for the contents of the <see cref="ConcurrentBag{T}" />.
        /// </returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        bool System.Collections.Concurrent.IProducerConsumerCollection<T>.TryAdd(T item) { return default(bool); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the <see cref="ConcurrentBag{T}" /> elements to a new
        /// array.
        /// </summary>
        /// <returns>
        /// A new array containing a snapshot of elements copied from the
        /// <see cref="ConcurrentBag{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Attempts to return an object from the <see cref="ConcurrentBag{T}" />
        /// without removing it.
        /// </summary>
        /// <param name="result">
        /// When this method returns, <paramref name="result" /> contains an object from the
        /// <see cref="ConcurrentBag{T}" /> or the default value of <paramref name="T" /> if the operation failed.
        /// </param>
        /// <returns>
        /// true if and object was returned successfully; otherwise, false.
        /// </returns>
        public bool TryPeek(out T result) { result = default(T); return default(bool); }
        /// <summary>
        /// Attempts to remove and return an object from the <see cref="ConcurrentBag{T}" />.
        /// </summary>
        /// <param name="result">
        /// When this method returns, <paramref name="result" /> contains the object removed from the
        /// <see cref="ConcurrentBag{T}" /> or the default value of <paramref name="T" />
        /// if the bag is empty.
        /// </param>
        /// <returns>
        /// true if an object was removed successfully; otherwise, false.
        /// </returns>
        public bool TryTake(out T result) { result = default(T); return default(bool); }
    }
    /// <summary>
    /// Represents a thread-safe collection of key/value pairs that can be accessed by multiple threads
    /// concurrently.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public partial class ConcurrentDictionary<TKey, TValue> : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>, System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey, TValue}" />
        /// class that is empty, has the default concurrency level, has the default initial capacity,
        /// and uses the default comparer for the key type.
        /// </summary>
        public ConcurrentDictionary() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified
        /// <see cref="Generic.IEnumerable{T}" />, has the default concurrency level, has the default initial capacity, and uses the default
        /// comparer for the key type.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="Generic.IEnumerable{T}" /> whose elements are copied to
        /// the new <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> or any of its keys is  null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection" /> contains one or more duplicate keys.
        /// </exception>
        public ConcurrentDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> collection) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IEnumerable" />
        /// has the default concurrency level, has the default initial capacity, and uses the specified
        /// <see cref="Generic.IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="Generic.IEnumerable{T}" /> whose elements are copied to
        /// the new <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="Generic.IEqualityComparer{T}" /> implementation to use
        /// when comparing keys.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> or <paramref name="comparer" /> is null.
        /// </exception>
        public ConcurrentDictionary(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> collection, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that is empty, has the default concurrency level and capacity, and uses the specified
        /// <see cref="Generic.IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is null.</exception>
        public ConcurrentDictionary(System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that contains elements copied from the specified <see cref="IEnumerable" />,
        /// and uses the specified <see cref="Generic.IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> concurrently.
        /// </param>
        /// <param name="collection">
        /// The <see cref="Generic.IEnumerable{T}" /> whose elements are copied to
        /// the new <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="Generic.IEqualityComparer{T}" /> implementation to use
        /// when comparing keys.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> or <paramref name="comparer" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel" /> is less than 1.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection" /> contains one or more duplicate keys.
        /// </exception>
        public ConcurrentDictionary(int concurrencyLevel, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>> collection, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that is empty, has the specified concurrency level and capacity, and uses the default
        /// comparer for the key type.
        /// </summary>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel" /> is less than 1.-or-<paramref name="capacity" /> is less
        /// than 0.
        /// </exception>
        public ConcurrentDictionary(int concurrencyLevel, int capacity) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// class that is empty, has the specified concurrency level, has the specified initial capacity,
        /// and uses the specified <see cref="Generic.IEqualityComparer{T}" />.
        /// </summary>
        /// <param name="concurrencyLevel">
        /// The estimated number of threads that will update the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> concurrently.
        /// </param>
        /// <param name="capacity">
        /// The initial number of elements that the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// can contain.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="Generic.IEqualityComparer{T}" /> implementation to use
        /// when comparing keys.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="concurrencyLevel" /> or <paramref name="capacity" /> is less than 1.
        /// </exception>
        public ConcurrentDictionary(int concurrencyLevel, int capacity, System.Collections.Generic.IEqualityComparer<TKey> comparer) { }
        /// <summary>
        /// Gets the number of key/value pairs contained in the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// The number of key/value pairs contained in the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </returns>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// is empty.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ConcurrentDictionary{TKey,TValue}" /> is empty;
        /// otherwise, false.
        /// </returns>
        public bool IsEmpty { get { return default(bool); } }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value of the key/value pair at the specified index.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is  null.</exception>
        /// <exception cref="Generic.KeyNotFoundException">
        /// The property is retrieved and <paramref name="key" /> does not exist in the collection.
        /// </exception>
        public TValue this[TKey key] { get { return default(TValue); } set { } }
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Generic.Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A collection of keys in the <see cref="Generic.Dictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.ICollection<TKey> Keys { get { return default(System.Collections.Generic.ICollection<TKey>); } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly { get { return default(bool); } }
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
        /// Gets a collection that contains the values in the <see cref="Generic.Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// A collection that contains the values in the <see cref="Generic.Dictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.ICollection<TValue> Values { get { return default(System.Collections.Generic.ICollection<TValue>); } }
        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// if the key does not already exist, or updates a key/value pair in the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> by using the specified function if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be addValue (if the key was absent) or the result
        /// of updateValueFactory (if the key was present).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> or <paramref name="updateValueFactory" /> is null.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public TValue AddOrUpdate(TKey key, TValue addValue, System.Func<TKey, TValue, TValue> updateValueFactory) { return default(TValue); }
        /// <summary>
        /// Uses the specified functions to add a key/value pair to the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> if the key does not already exist, or to update a key/value pair in the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be the result of addValueFactory (if the key
        /// was absent) or the result of updateValueFactory (if the key was present).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" />, <paramref name="addValueFactory" />, or <paramref name="updateValueFactory" />
        /// is null.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public TValue AddOrUpdate(TKey key, System.Func<TKey, TValue> addValueFactory, System.Func<TKey, TValue, TValue> updateValueFactory) { return default(TValue); }
        /// <summary>
        /// Removes all keys and values from the <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Determines whether the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </param>
        /// <returns>
        /// true if the <see cref="ConcurrentDictionary{TKey,TValue}" /> contains
        /// an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool ContainsKey(TKey key) { return default(bool); }
        /// <summary>
        /// Returns an enumerator that iterates through the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        /// <returns>
        /// An enumerator for the <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>>); }
        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">the value to be added, if the key does not already exist</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already
        /// in the dictionary, or the new value if the key was not in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public TValue GetOrAdd(TKey key, TValue value) { return default(TValue); }
        /// <summary>
        /// Adds a key/value pair to the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// by using the specified function, if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already
        /// in the dictionary, or the new value for the key as returned by valueFactory if the key was not
        /// in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key" /> or <paramref name="valueFactory" /> is null.
        /// </exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public TValue GetOrAdd(TKey key, System.Func<TKey, TValue> valueFactory) { return default(TValue); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo(System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int index) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove(System.Collections.Generic.KeyValuePair<TKey, TValue> keyValuePair) { return default(bool); }
        void System.Collections.Generic.IDictionary<TKey, TValue>.Add(TKey key, TValue value) { }
        bool System.Collections.Generic.IDictionary<TKey, TValue>.Remove(TKey key) { return default(bool); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        void System.Collections.IDictionary.Add(object key, object value) { }
        bool System.Collections.IDictionary.Contains(object key) { return default(bool); }
        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return default(System.Collections.IDictionaryEnumerator); }
        void System.Collections.IDictionary.Remove(object key) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the key and value pairs stored in the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing a snapshot of key and value pairs copied from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </returns>
        public System.Collections.Generic.KeyValuePair<TKey, TValue>[] ToArray() { return default(System.Collections.Generic.KeyValuePair<TKey, TValue>[]); }
        /// <summary>
        /// Attempts to add the specified key and value to the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be  null for reference types.</param>
        /// <returns>
        /// true if the key/value pair was added to the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> successfully; false if the key already exists.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is  null.</exception>
        /// <exception cref="OverflowException">
        /// The dictionary already contains the maximum number of elements (<see cref="Int32.MaxValue" />
        /// ).
        /// </exception>
        public bool TryAdd(TKey key, TValue value) { return default(bool); }
        /// <summary>
        /// Attempts to get the value associated with the specified key from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the object from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" /> that has the specified key, or the default value of the type if the operation failed.
        /// </param>
        /// <returns>
        /// true if the key was found in the <see cref="ConcurrentDictionary{TKey,TValue}" />
        /// ; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is  null.</exception>
        public bool TryGetValue(TKey key, out TValue value) { value = default(TValue); return default(bool); }
        /// <summary>
        /// Attempts to remove and return the value that has the specified key from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">
        /// When this method returns, contains the object removed from the
        /// <see cref="ConcurrentDictionary{TKey,TValue}" />, or the default value of  the TValue type if <paramref name="key" /> does not exist.
        /// </param>
        /// <returns>
        /// true if the object was removed successfully; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is  null.</exception>
        public bool TryRemove(TKey key, out TValue value) { value = default(TValue); return default(bool); }
        /// <summary>
        /// Compares the existing value for the specified key with a specified value, and if they are equal,
        /// updates the key with a third value.
        /// </summary>
        /// <param name="key">
        /// The key whose value is compared with <paramref name="comparisonValue" /> and possibly replaced.
        /// </param>
        /// <param name="newValue">
        /// The value that replaces the value of the element that has the specified <paramref name="key" />
        /// if the comparison results in equality.
        /// </param>
        /// <param name="comparisonValue">
        /// The value that is compared to the value of the element that has the specified <paramref name="key" />.
        /// </param>
        /// <returns>
        /// true if the value with <paramref name="key" /> was equal to <paramref name="comparisonValue" />
        /// and was replaced with <paramref name="newValue" />; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue) { return default(bool); }
    }
    /// <summary>
    /// Represents a thread-safe first in-first out (FIFO) collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the queue.</typeparam>
    public partial class ConcurrentQueue<T> : System.Collections.Concurrent.IProducerConsumerCollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentQueue{T}" />
        /// class.
        /// </summary>
        public ConcurrentQueue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentQueue{T}" />
        /// class that contains elements copied from the specified collection
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new
        /// <see cref="ConcurrentQueue{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
        public ConcurrentQueue(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentQueue{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ConcurrentQueue{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentQueue{T}" />
        /// is empty.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ConcurrentQueue{T}" /> is empty; otherwise,
        /// false.
        /// </returns>
        public bool IsEmpty { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Copies the <see cref="ConcurrentQueue{T}" /> elements to an
        /// existing one-dimensional <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from the <see cref="ConcurrentQueue{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" />
        /// -or- The number of elements in the source <see cref="ConcurrentQueue{T}" />
        /// is greater than the available space from <paramref name="index" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Adds an object to the end of the <see cref="ConcurrentQueue{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to add to the end of the <see cref="ConcurrentQueue{T}" />.
        /// The value can be a null reference (Nothing in Visual Basic) for reference types.
        /// </param>
        public void Enqueue(T item) { }
        /// <summary>
        /// Returns an enumerator that iterates through the
        /// <see cref="ConcurrentQueue{T}" />.
        /// </summary>
        /// <returns>
        /// An enumerator for the contents of the <see cref="ConcurrentQueue{T}" />.
        /// </returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        bool System.Collections.Concurrent.IProducerConsumerCollection<T>.TryAdd(T item) { return default(bool); }
        bool System.Collections.Concurrent.IProducerConsumerCollection<T>.TryTake(out T item) { item = default(T); return default(bool); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the elements stored in the <see cref="ConcurrentQueue{T}" />
        /// to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing a snapshot of elements copied from the
        /// <see cref="ConcurrentQueue{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Tries to remove and return the object at the beginning of the concurrent queue.
        /// </summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, <paramref name="result" /> contains
        /// the object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the beginning of the
        /// <see cref="ConcurrentQueue{T}" /> successfully; otherwise, false.
        /// </returns>
        public bool TryDequeue(out T result) { result = default(T); return default(bool); }
        /// <summary>
        /// Tries to return an object from the beginning of the
        /// <see cref="ConcurrentQueue{T}" /> without removing it.
        /// </summary>
        /// <param name="result">
        /// When this method returns, <paramref name="result" /> contains an object from the beginning
        /// of the <see cref="ConcurrentQueue{T}" /> or an unspecified
        /// value if the operation failed.
        /// </param>
        /// <returns>
        /// true if an object was returned successfully; otherwise, false.
        /// </returns>
        public bool TryPeek(out T result) { result = default(T); return default(bool); }
    }
    /// <summary>
    /// Represents a thread-safe last in-first out (LIFO) collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements contained in the stack.</typeparam>
    public partial class ConcurrentStack<T> : System.Collections.Concurrent.IProducerConsumerCollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IReadOnlyCollection<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentStack{T}" />
        /// class.
        /// </summary>
        public ConcurrentStack() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentStack{T}" />
        /// class that contains elements copied from the specified collection
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements are copied to the new
        /// <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> argument is null.</exception>
        public ConcurrentStack(System.Collections.Generic.IEnumerable<T> collection) { }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentStack{T}" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ConcurrentStack{T}" />.
        /// </returns>
        public int Count { get { return default(int); } }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="ConcurrentStack{T}" />
        /// is empty.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ConcurrentStack{T}" /> is empty; otherwise,
        /// false.
        /// </returns>
        public bool IsEmpty { get { return default(bool); } }
        bool System.Collections.ICollection.IsSynchronized { get { return default(bool); } }
        object System.Collections.ICollection.SyncRoot { get { return default(object); } }
        /// <summary>
        /// Removes all objects from the <see cref="ConcurrentStack{T}" />.
        /// </summary>
        public void Clear() { }
        /// <summary>
        /// Copies the <see cref="ConcurrentStack{T}" /> elements to an
        /// existing one-dimensional <see cref="Array" />, starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from the <see cref="ConcurrentStack{T}" />. The <see cref="Array" />
        /// must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" />
        /// -or- The number of elements in the source <see cref="ConcurrentStack{T}" />
        /// is greater than the available space from <paramref name="index" /> to the end of the destination
        /// <paramref name="array" />.
        /// </exception>
        public void CopyTo(T[] array, int index) { }
        /// <summary>
        /// Returns an enumerator that iterates through the
        /// <see cref="ConcurrentStack{T}" />.
        /// </summary>
        /// <returns>
        /// An enumerator for the <see cref="ConcurrentStack{T}" />.
        /// </returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<T>); }
        /// <summary>
        /// Inserts an object at the top of the <see cref="ConcurrentStack{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to push onto the <see cref="ConcurrentStack{T}" />.
        /// The value can be a null reference (Nothing in Visual Basic) for reference types.
        /// </param>
        public void Push(T item) { }
        /// <summary>
        /// Inserts multiple objects at the top of the <see cref="ConcurrentStack{T}" />
        /// atomically.
        /// </summary>
        /// <param name="items">
        /// The objects to push onto the <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        public void PushRange(T[] items) { }
        /// <summary>
        /// Inserts multiple objects at the top of the <see cref="ConcurrentStack{T}" />
        /// atomically.
        /// </summary>
        /// <param name="items">
        /// The objects to push onto the <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <param name="startIndex">
        /// The zero-based offset in <paramref name="items" /> at which to begin inserting elements onto
        /// the top of the <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <param name="count">
        /// The number of elements to be inserted onto the top of the
        /// <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> or <paramref name="count" /> is negative. Or <paramref name="startIndex" />
        /// is greater than or equal to the length of <paramref name="items" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of
        /// <paramref name="items" />.
        /// </exception>
        public void PushRange(T[] items, int startIndex, int count) { }
        bool System.Collections.Concurrent.IProducerConsumerCollection<T>.TryAdd(T item) { return default(bool); }
        bool System.Collections.Concurrent.IProducerConsumerCollection<T>.TryTake(out T item) { item = default(T); return default(bool); }
        void System.Collections.ICollection.CopyTo(System.Array array, int index) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        /// <summary>
        /// Copies the items stored in the <see cref="ConcurrentStack{T}" />
        /// to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing a snapshot of elements copied from the
        /// <see cref="ConcurrentStack{T}" />.
        /// </returns>
        public T[] ToArray() { return default(T[]); }
        /// <summary>
        /// Attempts to return an object from the top of the
        /// <see cref="ConcurrentStack{T}" /> without removing it.
        /// </summary>
        /// <param name="result">
        /// When this method returns, <paramref name="result" /> contains an object from the top of the
        /// <see cref="ConcurrentStack{T}" />or an unspecified value if
        /// the operation failed.
        /// </param>
        /// <returns>
        /// true if and object was returned successfully; otherwise, false.
        /// </returns>
        public bool TryPeek(out T result) { result = default(T); return default(bool); }
        /// <summary>
        /// Attempts to pop and return the object at the top of the
        /// <see cref="ConcurrentStack{T}" />.
        /// </summary>
        /// <param name="result">
        /// When this method returns, if the operation was successful, <paramref name="result" /> contains
        /// the object removed. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an element was removed and returned from the top of the
        /// <see cref="ConcurrentStack{T}" /> successfully; otherwise, false.
        /// </returns>
        public bool TryPop(out T result) { result = default(T); return default(bool); }
        /// <summary>
        /// Attempts to pop and return multiple objects from the top of the
        /// <see cref="ConcurrentStack{T}" /> atomically.
        /// </summary>
        /// <param name="items">
        /// The <see cref="Array" /> to which objects popped from the top of the
        /// <see cref="ConcurrentStack{T}" /> will be added.
        /// </param>
        /// <returns>
        /// The number of objects successfully popped from the top of the
        /// <see cref="ConcurrentStack{T}" /> and inserted in <paramref name="items" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is a null argument (Nothing in Visual Basic).
        /// </exception>
        public int TryPopRange(T[] items) { return default(int); }
        /// <summary>
        /// Attempts to pop and return multiple objects from the top of the
        /// <see cref="ConcurrentStack{T}" /> atomically.
        /// </summary>
        /// <param name="items">
        /// The <see cref="Array" /> to which objects popped from the top of the
        /// <see cref="ConcurrentStack{T}" /> will be added.
        /// </param>
        /// <param name="startIndex">
        /// The zero-based offset in <paramref name="items" /> at which to begin inserting elements from
        /// the top of the <see cref="ConcurrentStack{T}" />.
        /// </param>
        /// <param name="count">
        /// The number of elements to be popped from top of the
        /// <see cref="ConcurrentStack{T}" /> and inserted into <paramref name="items" />.
        /// </param>
        /// <returns>
        /// The number of objects successfully popped from the top of the stack and inserted in <paramref name="items" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex" /> or <paramref name="count" /> is negative. Or <paramref name="startIndex" />
        /// is greater than or equal to the length of <paramref name="items" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="startIndex" /> + <paramref name="count" /> is greater than the length of
        /// <paramref name="items" />.
        /// </exception>
        public int TryPopRange(T[] items, int startIndex, int count) { return default(int); }
    }
    /// <summary>
    /// Specifies options to control the buffering behavior of a partitioner
    /// </summary>
    [System.FlagsAttribute]
    public enum EnumerablePartitionerOptions
    {
        /// <summary>
        /// Create a partitioner that takes items from the source enumerable one at a time and does not
        /// use intermediate storage that can be accessed more efficiently by multiple threads. This option provides
        /// support for low latency (items will be processed as soon as they are available from the source)
        /// and provides partial support for dependencies between items (a thread cannot deadlock waiting for
        /// an item that the thread itself is responsible for processing).
        /// </summary>
        NoBuffering = 1,
        /// <summary>
        /// Use the default behavior, which is to use buffering to achieve optimal performance.
        /// </summary>
        None = 0,
    }
    /// <summary>
    /// Defines methods to manipulate thread-safe collections intended for producer/consumer usage.
    /// This interface provides a unified representation for producer/consumer collections so that
    /// higher level abstractions such as <see cref="BlockingCollection{T}" />
    /// can use the collection as the underlying storage mechanism.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
    public partial interface IProducerConsumerCollection<T> : System.Collections.Generic.IEnumerable<T>, System.Collections.ICollection, System.Collections.IEnumerable
    {
        /// <summary>
        /// Copies the elements of the <see cref="IProducerConsumerCollection{T}" />
        /// to an <see cref="Array" />, starting at a specified index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array" /> that is the destination of the elements
        /// copied from the <see cref="IProducerConsumerCollection{T}" />.
        /// The array must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array" /> is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="index" /> is equal to or greater than the length of the <paramref name="array" />
        /// -or- The number of elements in the collection is greater than the available space from
        /// <paramref name="index" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        void CopyTo(T[] array, int index);
        /// <summary>
        /// Copies the elements contained in the
        /// <see cref="IProducerConsumerCollection{T}" /> to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing the elements copied from the
        /// <see cref="IProducerConsumerCollection{T}" />.
        /// </returns>
        T[] ToArray();
        /// <summary>
        /// Attempts to add an object to the <see cref="IProducerConsumerCollection{T}" />.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="IProducerConsumerCollection{T}" />.
        /// </param>
        /// <returns>
        /// true if the object was added successfully; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="item" /> was invalid for this collection.
        /// </exception>
        bool TryAdd(T item);
        /// <summary>
        /// Attempts to remove and return an object from the
        /// <see cref="IProducerConsumerCollection{T}" />.
        /// </summary>
        /// <param name="item">
        /// When this method returns, if the object was removed and returned successfully, <paramref name="item" />
        /// contains the removed object. If no object was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>
        /// true if an object was removed and returned successfully; otherwise, false.
        /// </returns>
        bool TryTake(out T item);
    }
    /// <summary>
    /// Represents a particular manner of splitting an orderable data source into multiple partitions.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in the collection.</typeparam>
    public abstract partial class OrderablePartitioner<TSource> : System.Collections.Concurrent.Partitioner<TSource>
    {
        /// <summary>
        /// Called from constructors in derived classes to initialize the
        /// <see cref="OrderablePartitioner{T}" /> class with the specified constraints on the index keys.
        /// </summary>
        /// <param name="keysOrderedInEachPartition">
        /// Indicates whether the elements in each partition are yielded in the order of increasing keys.
        /// </param>
        /// <param name="keysOrderedAcrossPartitions">
        /// Indicates whether elements in an earlier partition always come before elements in a later partition.
        /// If true, each element in partition 0 has a smaller order key than any element in partition
        /// 1, each element in partition 1 has a smaller order key than any element in partition 2, and so on.
        /// </param>
        /// <param name="keysNormalized">
        /// Indicates whether keys are normalized. If true, all order keys are distinct integers in the
        /// range [0 .. numberOfElements-1]. If false, order keys must still be distinct, but only their relative
        /// order is considered, not their absolute values.
        /// </param>
        protected OrderablePartitioner(bool keysOrderedInEachPartition, bool keysOrderedAcrossPartitions, bool keysNormalized) { }
        /// <summary>
        /// Gets whether order keys are normalized.
        /// </summary>
        /// <returns>
        /// true if the keys are normalized; otherwise false.
        /// </returns>
        public bool KeysNormalized { get { return default(bool); } }
        /// <summary>
        /// Gets whether elements in an earlier partition always come before elements in a later partition.
        /// </summary>
        /// <returns>
        /// true if the elements in an earlier partition always come before elements in a later partition;
        /// otherwise false.
        /// </returns>
        public bool KeysOrderedAcrossPartitions { get { return default(bool); } }
        /// <summary>
        /// Gets whether elements in each partition are yielded in the order of increasing keys.
        /// </summary>
        /// <returns>
        /// true if the elements in each partition are yielded in the order of increasing keys; otherwise
        /// false.
        /// </returns>
        public bool KeysOrderedInEachPartition { get { return default(bool); } }
        /// <summary>
        /// Creates an object that can partition the underlying collection into a variable number of partitions.
        /// </summary>
        /// <returns>
        /// An object that can create partitions over the underlying data source.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Dynamic partitioning is not supported by the base class. It must be implemented in derived
        /// classes.
        /// </exception>
        public override System.Collections.Generic.IEnumerable<TSource> GetDynamicPartitions() { return default(System.Collections.Generic.IEnumerable<TSource>); }
        /// <summary>
        /// Creates an object that can partition the underlying collection into a variable number of partitions.
        /// </summary>
        /// <returns>
        /// An object that can create partitions over the underlying data source.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Dynamic partitioning is not supported by this partitioner.
        /// </exception>
        public virtual System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long, TSource>> GetOrderableDynamicPartitions() { return default(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long, TSource>>); }
        /// <summary>
        /// Partitions the underlying collection into the specified number of orderable partitions.
        /// </summary>
        /// <param name="partitionCount">The number of partitions to create.</param>
        /// <returns>
        /// A list containing <paramref name="partitionCount" /> enumerators.
        /// </returns>
        public abstract System.Collections.Generic.IList<System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount);
        /// <summary>
        /// Partitions the underlying collection into the given number of ordered partitions.
        /// </summary>
        /// <param name="partitionCount">The number of partitions to create.</param>
        /// <returns>
        /// A list containing <paramref name="partitionCount" /> enumerators.
        /// </returns>
        public override System.Collections.Generic.IList<System.Collections.Generic.IEnumerator<TSource>> GetPartitions(int partitionCount) { return default(System.Collections.Generic.IList<System.Collections.Generic.IEnumerator<TSource>>); }
    }
    /// <summary>
    /// Provides common partitioning strategies for arrays, lists, and enumerables.
    /// </summary>
    public static partial class Partitioner
    {
        /// <summary>
        /// Creates a partitioner that chunks the user-specified range.
        /// </summary>
        /// <param name="fromInclusive">The lower, inclusive bound of the range.</param>
        /// <param name="toExclusive">The upper, exclusive bound of the range.</param>
        /// <returns>
        /// A partitioner.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive" /> argument is less than or equal to the <paramref name="fromInclusive" />
        /// argument.
        /// </exception>
        public static System.Collections.Concurrent.OrderablePartitioner<System.Tuple<int, int>> Create(int fromInclusive, int toExclusive) { return default(System.Collections.Concurrent.OrderablePartitioner<System.Tuple<int, int>>); }
        /// <summary>
        /// Creates a partitioner that chunks the user-specified range.
        /// </summary>
        /// <param name="fromInclusive">The lower, inclusive bound of the range.</param>
        /// <param name="toExclusive">The upper, exclusive bound of the range.</param>
        /// <param name="rangeSize">The size of each subrange.</param>
        /// <returns>
        /// A partitioner.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive" /> argument is less than or equal to the <paramref name="fromInclusive" />
        /// argument.-or-The <paramref name="rangeSize" /> argument is less than or equal to 0.
        /// </exception>
        public static System.Collections.Concurrent.OrderablePartitioner<System.Tuple<int, int>> Create(int fromInclusive, int toExclusive, int rangeSize) { return default(System.Collections.Concurrent.OrderablePartitioner<System.Tuple<int, int>>); }
        /// <summary>
        /// Creates a partitioner that chunks the user-specified range.
        /// </summary>
        /// <param name="fromInclusive">The lower, inclusive bound of the range.</param>
        /// <param name="toExclusive">The upper, exclusive bound of the range.</param>
        /// <returns>
        /// A partitioner.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive" /> argument is less than or equal to the <paramref name="fromInclusive" />
        /// argument.
        /// </exception>
        public static System.Collections.Concurrent.OrderablePartitioner<System.Tuple<long, long>> Create(long fromInclusive, long toExclusive) { return default(System.Collections.Concurrent.OrderablePartitioner<System.Tuple<long, long>>); }
        /// <summary>
        /// Creates a partitioner that chunks the user-specified range.
        /// </summary>
        /// <param name="fromInclusive">The lower, inclusive bound of the range.</param>
        /// <param name="toExclusive">The upper, exclusive bound of the range.</param>
        /// <param name="rangeSize">The size of each subrange.</param>
        /// <returns>
        /// A partitioner.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="toExclusive" /> argument is less than or equal to the <paramref name="fromInclusive" />
        /// argument.-or-The <paramref name="rangeSize" /> argument is less than or equal to 0.
        /// </exception>
        public static System.Collections.Concurrent.OrderablePartitioner<System.Tuple<long, long>> Create(long fromInclusive, long toExclusive, long rangeSize) { return default(System.Collections.Concurrent.OrderablePartitioner<System.Tuple<long, long>>); }
        /// <summary>
        /// Creates an orderable partitioner from a <see cref="Array" /> instance.
        /// </summary>
        /// <param name="array">The array to be partitioned.</param>
        /// <param name="loadBalance">
        /// A Boolean value that indicates whether the created partitioner should dynamically load balance
        /// between partitions rather than statically partition.
        /// </param>
        /// <typeparam name="TSource">Type of the elements in source array.</typeparam>
        /// <returns>
        /// An orderable partitioner based on the input array.
        /// </returns>
        public static System.Collections.Concurrent.OrderablePartitioner<TSource> Create<TSource>(TSource[] array, bool loadBalance) { return default(System.Collections.Concurrent.OrderablePartitioner<TSource>); }
        /// <summary>
        /// Creates an orderable partitioner from a <see cref="Generic.IEnumerable{T}" />
        /// instance.
        /// </summary>
        /// <param name="source">The enumerable to be partitioned.</param>
        /// <typeparam name="TSource">Type of the elements in source enumerable.</typeparam>
        /// <returns>
        /// An orderable partitioner based on the input array.
        /// </returns>
        public static System.Collections.Concurrent.OrderablePartitioner<TSource> Create<TSource>(System.Collections.Generic.IEnumerable<TSource> source) { return default(System.Collections.Concurrent.OrderablePartitioner<TSource>); }
        /// <summary>
        /// Creates an orderable partitioner from a <see cref="Generic.IEnumerable{T}" />
        /// instance.
        /// </summary>
        /// <param name="source">The enumerable to be partitioned.</param>
        /// <param name="partitionerOptions">Options to control the buffering behavior of the partitioner.</param>
        /// <typeparam name="TSource">Type of the elements in source enumerable.</typeparam>
        /// <returns>
        /// An orderable partitioner based on the input array.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="partitionerOptions" /> argument specifies an invalid value for
        /// <see cref="EnumerablePartitionerOptions" />.
        /// </exception>
        public static System.Collections.Concurrent.OrderablePartitioner<TSource> Create<TSource>(System.Collections.Generic.IEnumerable<TSource> source, System.Collections.Concurrent.EnumerablePartitionerOptions partitionerOptions) { return default(System.Collections.Concurrent.OrderablePartitioner<TSource>); }
        /// <summary>
        /// Creates an orderable partitioner from an <see cref="Generic.IList{T}" />
        /// instance.
        /// </summary>
        /// <param name="list">The list to be partitioned.</param>
        /// <param name="loadBalance">
        /// A Boolean value that indicates whether the created partitioner should dynamically load balance
        /// between partitions rather than statically partition.
        /// </param>
        /// <typeparam name="TSource">Type of the elements in source list.</typeparam>
        /// <returns>
        /// An orderable partitioner based on the input list.
        /// </returns>
        public static System.Collections.Concurrent.OrderablePartitioner<TSource> Create<TSource>(System.Collections.Generic.IList<TSource> list, bool loadBalance) { return default(System.Collections.Concurrent.OrderablePartitioner<TSource>); }
    }
    /// <summary>
    /// Represents a particular manner of splitting a data source into multiple partitions.
    /// </summary>
    /// <typeparam name="TSource">Type of the elements in the collection.</typeparam>
    public abstract partial class Partitioner<TSource>
    {
        /// <summary>
        /// Creates a new partitioner instance.
        /// </summary>
        protected Partitioner() { }
        /// <summary>
        /// Gets whether additional partitions can be created dynamically.
        /// </summary>
        /// <returns>
        /// true if the <see cref="Partitioner{T}" /> can create partitions
        /// dynamically as they are requested; false if the <see cref="Partitioner{T}" />
        /// can only allocate partitions statically.
        /// </returns>
        public virtual bool SupportsDynamicPartitions { get { return default(bool); } }
        /// <summary>
        /// Creates an object that can partition the underlying collection into a variable number of partitions.
        /// </summary>
        /// <returns>
        /// An object that can create partitions over the underlying data source.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Dynamic partitioning is not supported by the base class. You must implement it in a derived
        /// class.
        /// </exception>
        public virtual System.Collections.Generic.IEnumerable<TSource> GetDynamicPartitions() { return default(System.Collections.Generic.IEnumerable<TSource>); }
        /// <summary>
        /// Partitions the underlying collection into the given number of partitions.
        /// </summary>
        /// <param name="partitionCount">The number of partitions to create.</param>
        /// <returns>
        /// A list containing <paramref name="partitionCount" /> enumerators.
        /// </returns>
        public abstract System.Collections.Generic.IList<System.Collections.Generic.IEnumerator<TSource>> GetPartitions(int partitionCount);
    }
}
