// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [System.Runtime.CompilerServices.TypeForwardedFrom("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private SimpleMonitor _monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. Do not rename (binary serialization)

        //TODO should serialize?
        [NonSerialized]
        private DeferredEventsCollection _deferredEvents;

        [NonSerialized]
        private int _blockReentrancyCount;
        #endregion Private Fields

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors
        /// <summary>
        /// Initializes a new instance of ObservableCollection that is empty and has default initial capacity.
        /// </summary>
        public ObservableCollection() { }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public ObservableCollection(IEnumerable<T> collection) : base(CreateCopy(collection, nameof(collection))) { }

        /// <summary>
        /// Initializes a new instance of the ObservableCollection class
        /// that contains elements copied from the specified list
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public ObservableCollection(List<T> list) : base(CreateCopy(list, nameof(list))) { }

        // Note: If we're gonna introduce new constructors not using a List<T>, 
        // review InsertRange, ReplaceRange(index, collection) and RemoveRange(index, count).
        private static List<T> CreateCopy(IEnumerable<T> collection, string paramName)
        {
            if (collection == null)
                throw new ArgumentNullException(paramName);

            return new List<T>(collection);
        }

        #endregion Constructors


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }


        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(Count, collection);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List<T>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>                
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));      
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);     
            if (index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_ArgExceedsSize);

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    return;
                }
            }
            else if (!ContainsAny(collection))
            {
                return;
            }

            CheckReentrancy();

            //expand the following couple of lines when adding more constructors.
            var target = (List<T>)Items;
            target.InsertRange(index, collection);

            OnEssentialPropertiesChanged();

            if (!(collection is IList list))
                list = new List<T>(collection);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }


        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">The items to remove.</param>        
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (Count == 0)
            {
                return;
            }
            else if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                    return;
                else if (countable.Count == 1)
                    using (IEnumerator<T> enumerator = countable.GetEnumerator())
                    {
                        enumerator.MoveNext();
                        Remove(enumerator.Current);
                        return;
                    }
            }
            else if (!(ContainsAny(collection)))
            {
                return;
            }

            CheckReentrancy();

            var clusters = new Dictionary<int, List<T>>();
            var lastIndex = -1;
            List<T> lastCluster = null;
            foreach (T item in collection)
            {
                var index = IndexOf(item);
                if (index < 0)
                {
                    continue;
                }

                Items.RemoveAt(index);

                if (lastIndex == index && lastCluster != null)
                {
                    lastCluster.Add(item);
                }
                else
                {
                    clusters[lastIndex = index] = lastCluster = new List<T> { item };
                }
            }

            OnEssentialPropertiesChanged();

            if (Count == 0)
                OnCollectionReset();
            else
                foreach (KeyValuePair<int, List<T>> cluster in clusters)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));

        }

        /// <summary>
        /// Iterates over the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="match"></param>
        /// <returns>Returns the number of elements that where </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public int RemoveAll(Predicate<T> match)
        {
            return RemoveAll(0, Count, match);

            /*
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (Count == 0)
                return 0;

            List<T> cluster = null;
            var clusterIndex = -1;
            var removedCount = 0;
            using (BlockReentrancy())
            {
                for (int index = 0; index < Count; index++)
                {
                    T item = Items[index];
                    if (match(item))
                    {
                        Items.RemoveAt(index);
                        removedCount++;

                        if (clusterIndex == index)
                        {
                            Debug.Assert(cluster != null);
                            cluster.Add(item);
                        }
                        else
                        {
                            cluster = new List<T> { item };
                            clusterIndex = index;
                        }

                        index--;
                    }
                    else if (clusterIndex > -1)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                        clusterIndex = -1;
                        cluster = null;
                    }
                }

                if (clusterIndex > -1)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
            }

            return removedCount;    
            */
        }

        /// <summary>
        /// Iterates over the specified range within the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="index">The index of where to start performing the search.</param>
        /// <param name="count">The number of items to iterate on.</param>
        /// <param name="match"></param>
        /// <returns>Returns the number of elements that where </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public int RemoveAll(int index, int count, Predicate<T> match)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_ArgExceedsSize);
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (Count == 0)
                return 0;

            List<T> cluster = null;
            var clusterIndex = -1;
            var removedCount = 0;

            using (BlockReentrancy())
            using (new DeferredEventsCollection(this))
            {
                for (var i = 0; i < count; i++, index++)
                {
                    T item = Items[index];
                    if (match(item))
                    {
                        Items.RemoveAt(index);
                        removedCount++;

                        if (clusterIndex == index)
                        {
                            Debug.Assert(cluster != null);
                            cluster.Add(item);
                        }
                        else
                        {
                            cluster = new List<T> { item };
                            clusterIndex = index;
                        }

                        index--;
                    }
                    else if (clusterIndex > -1)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                        clusterIndex = -1;
                        cluster = null;
                    }
                }

                if (clusterIndex > -1)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
            }

            if (removedCount > 0)
                OnEssentialPropertiesChanged();

            return removedCount;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_ArgExceedsSize);

            if (count == 0)
                return;

            if (count == 1)
            {
                RemoveItem(index);
                return;
            }

            //Items will always be List<T>, see constructors
            var items = (List<T>)Items;
            List<T> removedItems = items.GetRange(index, count);

            CheckReentrancy();

            items.RemoveRange(index, count);

            OnEssentialPropertiesChanged();

            if (Count == 0)
                OnCollectionReset();
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection,
        /// using the default <see cref="EqualityComparer{T}"/>.
        /// </summary>             
        /// <param name="collection">The items to fill the collection with, after clearing it.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(IEnumerable<T> collection)
        {
            ReplaceRange(0, Count, collection, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Clears the current collection and replaces it with the specified collection,
        /// using the specified comparer to skip equal items.
        /// </summary>
        /// <param name="collection">The items to fill the collection with, after clearing it.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to be used
        /// to check whether an item in the same location already existed before,
        /// which in case it would not be added to the collection, and no event will be raised for it.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        public void ReplaceRange(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            ReplaceRange(0, Count, collection, comparer);
        }

        /// <summary>
        /// Removes the specified range and inserts the specified collection,
        /// ignoring equal items (using <see cref="EqualityComparer{T}.Default"/>).
        /// </summary>
        /// <param name="index">The index of where to start the replacement.</param>
        /// <param name="count">The number of items to be replaced.</param>
        /// <param name="collection">The collection to insert in that location.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(int index, int count, IEnumerable<T> collection)
        {
            ReplaceRange(index, count, collection, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Removes the specified range and inserts the specified collection in its position, leaving equal items in equal positions intact.
        /// </summary>
        /// <param name="index">The index of where to start the replacement.</param>
        /// <param name="count">The number of items to be replaced.</param>
        /// <param name="collection">The collection to insert in that location.</param>
        /// <param name="comparer">The comparer to use when checking for equal items.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        public void ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (index + count > Count)
                throw new ArgumentOutOfRangeException(SR.ArgumentOutOfRange_ArgExceedsSize);

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));  
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    RemoveRange(index, count);
                    return;
                }
            }
            else if (!ContainsAny(collection))
            {
                RemoveRange(index, count);
                return;
            }

            if (index + count == 0)
            {
                InsertRange(0, collection);
                return;
            }

            if (!(collection is IList<T> list))
                list = new List<T>(collection);

            using (BlockReentrancy())
            using (new DeferredEventsCollection(this))
            {
                var rangeCount = index + count;
                var addedCount = list.Count;

                var changesMade = false;
                List<T>
                    oldCluster = new List<T>(),
                    newCluster = new List<T>();


                int i = index;
                for (; i < rangeCount && i - index < addedCount; i++)
                {
                    //parallel position
                    T old = this[i], @new = list[i - index];
                    if (comparer.Equals(old, @new))
                    {
                        OnRangeReplaced(i, newCluster, oldCluster);
                        continue;
                    }
                    else
                    {
                        Items[i] = @new;
                        newCluster.Add(@new);
                        oldCluster.Add(old);

                        changesMade = true;
                    }
                }

                OnRangeReplaced(i, newCluster, oldCluster);

                //exceeding position
                if (count != addedCount)
                {     
                    var items = (List<T>)Items;
                    if (count > addedCount)
                    {
                        var removedCount = rangeCount - addedCount;
                        T[] removed = new T[removedCount];
                        items.CopyTo(i, removed, 0, removed.Length);
                        items.RemoveRange(i, removedCount);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i));
                    }
                    else
                    {
                        var k = i - index;
                        T[] added = new T[addedCount - k];
                        for (int j = k; j < addedCount; j++)
                        {
                            T @new = list[j];
                            added[j - k] = @new;
                        }
                        items.InsertRange(i, added);
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
                    }

                    OnEssentialPropertiesChanged();
                }
                else if(changesMade)
                {
                    OnIndexerPropertyChanged();
                }  
            }
        }

        #endregion Public Methods


        //------------------------------------------------------
        //
        //  Public Events
        //
        //------------------------------------------------------

        #region Public Events

        //------------------------------------------------------
        #region INotifyPropertyChanged implementation

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }
        #endregion INotifyPropertyChanged implementation


        //------------------------------------------------------
        /// <summary>
        /// Occurs when the collection changes, either by adding or removing an item.
        /// </summary>
        /// <remarks>
        /// see <seealso cref="INotifyCollectionChanged"/>
        /// </remarks>
        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion Public Events


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            if (Count == 0)
                return;

            CheckReentrancy();
            base.ClearItems();
            OnEssentialPropertiesChanged();
            OnCollectionReset();
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is removed from list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not in the collection range.
        /// </exception>
        protected override void RemoveItem(int index)
        {
            CheckReentrancy();
            T removedItem = this[index];

            base.RemoveItem(index);

            OnEssentialPropertiesChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is added to list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);

            OnEssentialPropertiesChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is set in list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            if (Equals(this[index], item))
                return;

            CheckReentrancy();
            T originalItem = this[index];
            base.SetItem(index, item);

            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <summary>
        /// Called by base class ObservableCollection&lt;T&gt; when an item is to be moved within the list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            T removedItem = this[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);

            OnIndexerPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }


        /// <summary>
        /// Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        [field: NonSerialized]
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
        /// </remarks>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_deferredEvents != null)
            {
                _deferredEvents.Add(e);
                return;
            }

            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null)
            {
                // Not calling BlockReentrancy() here to avoid the SimpleMonitor allocation.
                _blockReentrancyCount++;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    _blockReentrancyCount--;
                }
            }
        }

        /// <summary>
        /// Disallow reentrant attempts to change this collection. E.g. a event handler
        /// of the CollectionChanged event is not allowed to make changes to this collection.
        /// </summary>
        /// <remarks>
        /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
        /// <code>
        ///         using (BlockReentrancy())
        ///         {
        ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
        ///         }
        /// </code>
        /// </remarks>
        protected IDisposable BlockReentrancy()
        {
            _blockReentrancyCount++;
            return EnsureMonitorInitialized();
        }

        /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
        /// <exception cref="InvalidOperationException"> raised when changing the collection
        /// while another collection change is still being notified to other listeners </exception>
        protected void CheckReentrancy()
        {
            if (_blockReentrancyCount > 0)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if (CollectionChanged?.GetInvocationList().Length > 1)
                    throw new InvalidOperationException(SR.ObservableCollectionReentrancyNotAllowed);
            }
        }

        #endregion Protected Methods


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Helper function to determine if a collection contains any elements.
        /// </summary>
        /// <param name="collection">The collection to evaluate.</param>
        /// <returns></returns>
        private static bool ContainsAny(IEnumerable<T> collection)
        {
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
                return enumerator.MoveNext();
        }

        /// <summary>
        /// Helper to raise Count property and the Indexer property.
        /// </summary>
        private void OnEssentialPropertiesChanged()
        {
            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnIndexerPropertyChanged();
        }

        /// <summary>
        /// /// Helper to raise a PropertyChanged event for the Indexer property
        /// /// </summary>
        private void OnIndexerPropertyChanged()
        {
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary>
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners
        /// </summary>
        private void OnCollectionReset()
        {
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }

        /// <summary>
        /// Helper to raise event for clustered action and clear cluster.
        /// </summary>
        /// <param name="followingItemIndex">The index of the item following the replacement block.</param>
        /// <param name="newCluster"></param>
        /// <param name="oldCluster"></param>
        //TODO should have really been a local method inside ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer),
        //move when supported language version updated.
        private void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T> oldCluster)
        {
            if (oldCluster.Count == 0)
                return;

            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    new List<T>(newCluster),
                    new List<T>(oldCluster),
                    followingItemIndex - oldCluster.Count));

            oldCluster.Clear();
            newCluster.Clear();
        }


        private SimpleMonitor EnsureMonitorInitialized()
        {
            return _monitor ?? (_monitor = new SimpleMonitor(this));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            EnsureMonitorInitialized();
            _monitor._busyCount = _blockReentrancyCount;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_monitor != null)
            {
                _blockReentrancyCount = _monitor._busyCount;
                _monitor._collection = this;
            }
        }
        #endregion Private Methods

        //------------------------------------------------------
        //
        //  Private Types
        //
        //------------------------------------------------------

        #region Private Types

        // this class helps prevent reentrant calls
        [Serializable]
        [System.Runtime.CompilerServices.TypeForwardedFrom("WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
        private sealed class SimpleMonitor : IDisposable
        {
            internal int _busyCount; // Only used during (de)serialization to maintain compatibility with desktop. Do not rename (binary serialization)

            [NonSerialized]
            internal /*private readonly*/ ObservableCollection<T> _collection;

            public SimpleMonitor(ObservableCollection<T> collection)
            {
                Debug.Assert(collection != null);
                _collection = collection;
            }

            public void Dispose()
            {
                _collection._blockReentrancyCount--;
            }
        }

        //TODO serialization?
        private sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly ObservableCollection<T> _collection;
            public DeferredEventsCollection(ObservableCollection<T> collection)
            {
                Debug.Assert(collection != null);
                Debug.Assert(collection._deferredEvents == null);
                _collection = collection;
                _collection._deferredEvents = this;
            }

            public void Dispose()
            {
                _collection._deferredEvents = null;
                foreach (var args in this)
                    _collection.OnCollectionChanged(args);
            }
        }

        #endregion Private Types

    }

    /// <remarks>
    /// To be kept outside <see cref="ObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used.
    /// </remarks>
    internal static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
