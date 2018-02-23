﻿using System.Collections.Specialized;
using System.ComponentModel;
using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    /// <summary>
    /// Helper class to test the CollectionChanged and PropertyChanged Events.
    /// </summary>
    public partial class CollectionAndPropertyChangedTester
    {
        #region Properties

        private const string COUNT = "Count";
        private const string ITEMARRAY = "Item[]";

        // Number of collection changed events that were ACTUALLY fired.
        public int NumCollectionChangedFired { get; private set; }
        // Number of collection changed events that are EXPECTED to be fired.
        public int ExpectedCollectionChangedFired { get; private set; }

        public int ExpectedNewStartingIndex { get; private set; }
        public NotifyCollectionChangedAction ExpectedAction { get; private set; }
        public IList ExpectedNewItems { get; private set; }
        public IList ExpectedOldItems { get; private set; }
        public int ExpectedOldStartingIndex { get; private set; }
        public bool SkipVerifyEventArgs { get; private set; }

        private PropertyNameExpected[] _expectedPropertyChanged;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Will perform an Add or Insert on the given Collection depending on whether the 
        /// insertIndex is null or not. If it is null, will Add, otherwise, will Insert.
        /// </summary>
        public void AddOrInsertItemTest(ObservableCollection<string> collection, string itemToAdd, int? insertIndex = null)
        {
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collection.CollectionChanged += Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            ExpectedCollectionChangedFired++;
            ExpectedAction = NotifyCollectionChangedAction.Add;
            ExpectedNewItems = new string[] { itemToAdd };
            if (insertIndex.HasValue)
                ExpectedNewStartingIndex = insertIndex.Value;
            else
                ExpectedNewStartingIndex = collection.Count;
            ExpectedOldItems = null;
            ExpectedOldStartingIndex = -1;

            int expectedCount = collection.Count + 1;

            if (insertIndex.HasValue)
            {
                collection.Insert(insertIndex.Value, itemToAdd);
                Assert.Equal(itemToAdd, collection[insertIndex.Value]);
            }
            else
            {
                collection.Add(itemToAdd);
                Assert.Equal(itemToAdd, collection[collection.Count - 1]);
            }

            Assert.Equal(expectedCount, collection.Count);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);


            foreach (var item in _expectedPropertyChanged)
                Assert.True(item.IsFound, "The propertychanged event should have fired for " + item.Name + ", since we just added an item");

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Clears the given Collection, and raises events and property change notifications if any items were in it.
        /// </summary>
        public void ClearTest(ObservableCollection<string> collection)
        {
            bool any = collection.Count > 0;
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            collection.CollectionChanged += Collection_CollectionChanged;
            if (any)
                ExpectedCollectionChangedFired++;

            ExpectedAction = NotifyCollectionChangedAction.Reset;
            ExpectedNewItems = null;
            ExpectedNewStartingIndex = -1;
            ExpectedOldItems = null;
            ExpectedOldStartingIndex = -1;

            collection.Clear();

            Assert.Equal(0, collection.Count);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            foreach (var item in _expectedPropertyChanged)                
                Assert.True(any == item.IsFound,
                    string.Format("The propertychanged event should have {0} been fired for", any ? "" : "not") + item.Name + 
                    ", since the collection " + (any ? "was just cleared" : "was already clear"));

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Given a collection, will move an item from the oldIndex to the newIndex.
        /// </summary>
        public void MoveItemTest(ObservableCollection<string> collection, int oldIndex, int newIndex)
        {
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[] { new PropertyNameExpected(ITEMARRAY) };

            collection.CollectionChanged += Collection_CollectionChanged;

            string itemAtOldIndex = collection[oldIndex];

            ExpectedCollectionChangedFired++;
            ExpectedAction = NotifyCollectionChangedAction.Move;
            ExpectedNewItems = new string[] { itemAtOldIndex };
            ExpectedNewStartingIndex = newIndex;
            ExpectedOldItems = new string[] { itemAtOldIndex };
            ExpectedOldStartingIndex = oldIndex;

            int expectedCount = collection.Count;

            collection.Move(oldIndex, newIndex);
            Assert.Equal(expectedCount, collection.Count);
            Assert.Equal(itemAtOldIndex, collection[newIndex]);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            foreach (var item in _expectedPropertyChanged)
                Assert.True(item.IsFound, "The propertychanged event should have fired for" + item.Name + ", since we just moved an item");

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Will set that new item at the specified index in the given collection.
        /// </summary>
        public void ReplaceItemTest(ObservableCollection<string> collection, int index, string newItem)
        {
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[] { new PropertyNameExpected(ITEMARRAY) };

            collection.CollectionChanged += Collection_CollectionChanged;

            string itemAtOldIndex = collection[index];

            ExpectedCollectionChangedFired++;
            ExpectedAction = NotifyCollectionChangedAction.Replace;
            ExpectedNewItems = new string[] { newItem };
            ExpectedNewStartingIndex = index;
            ExpectedOldItems = new string[] { itemAtOldIndex };
            ExpectedOldStartingIndex = index;

            int expectedCount = collection.Count;

            collection[index] = newItem;
            Assert.Equal(expectedCount, collection.Count);
            Assert.Equal(newItem, collection[index]);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            foreach (var item in _expectedPropertyChanged)
                Assert.True(item.IsFound, "The propertychanged event should have fired for" + item.Name + ", since we just replaced an item");

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Given a collection, index and item to remove, will try to remove that item
        /// from the index. If the item has duplicates, will verify that only the first
        /// instance was removed.
        /// </summary>
        public void RemoveItemTest(ObservableCollection<string> collection, int itemIndex, string itemToRemove, bool isSuccessfulRemove, bool hasDuplicates)
        {
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            collection.CollectionChanged += Collection_CollectionChanged;

            if (isSuccessfulRemove)
                ExpectedCollectionChangedFired++;

            ExpectedAction = NotifyCollectionChangedAction.Remove;
            ExpectedNewItems = null;
            ExpectedNewStartingIndex = -1;
            ExpectedOldItems = new string[] { itemToRemove };
            ExpectedOldStartingIndex = itemIndex;

            int expectedCount = isSuccessfulRemove ? collection.Count - 1 : collection.Count;

            bool removedItem = collection.Remove(itemToRemove);
            Assert.Equal(expectedCount, collection.Count);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            if (isSuccessfulRemove)
            {
                foreach (var item in _expectedPropertyChanged)
                    Assert.True(item.IsFound, "The propertychanged event should have fired for" + item.Name + ", since there were items removed.");

                Assert.True(removedItem, "Should have been successful in removing the item.");
            }
            else
            {
                foreach (var item in _expectedPropertyChanged)
                    Assert.False(item.IsFound, "The propertychanged event should have fired for" + item.Name + ", since there were no items removed.");

                Assert.False(removedItem, "Should not have been successful in removing the item.");
            }
            if (hasDuplicates)
                return;

            Assert.DoesNotContain(itemToRemove, collection);

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Verifies that the item is removed from a given index in the collection.
        /// </summary>
        public void RemoveItemAtTest(ObservableCollection<string> collection, int itemIndex)
        {
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            collection.CollectionChanged += Collection_CollectionChanged;

            string itemAtOldIndex = collection[itemIndex];

            ExpectedCollectionChangedFired++;
            ExpectedAction = NotifyCollectionChangedAction.Remove;
            ExpectedNewItems = null;
            ExpectedNewStartingIndex = -1;
            ExpectedOldItems = new string[] { itemAtOldIndex };
            ExpectedOldStartingIndex = itemIndex;

            int expectedCount = collection.Count - 1;

            collection.RemoveAt(itemIndex);
            Assert.Equal(expectedCount, collection.Count);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            foreach (var item in _expectedPropertyChanged)
                Assert.True(item.IsFound, "The propertychanged event should have fired for" + item.Name + ", since we just removed an item");

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }


        /// <summary>
        /// Verifies that the eventargs fired matches the expected results.
        /// </summary>
        private void VerifyEventArgs(NotifyCollectionChangedEventArgs e)
        {
            Assert.Equal(ExpectedNewStartingIndex, e.NewStartingIndex);
            Assert.Equal(ExpectedOldStartingIndex, e.OldStartingIndex);

            if (ExpectedNewItems != null)
            {
                foreach (var newItem in e.NewItems)
                    Assert.True(ExpectedNewItems.Contains(newItem), "newItem was not in the ExpectedNewItems. newItem: " + newItem);
                foreach (var expectedItem in ExpectedNewItems)
                    Assert.True(e.NewItems.Contains(expectedItem), "expectedItem was not in e.NewItems. expectedItem: " + expectedItem);
            }
            else
            {
                Assert.Null(e.NewItems);
            }

            if (ExpectedOldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                    Assert.True(ExpectedOldItems.Contains(oldItem), "oldItem was not in the ExpectedOldItems. oldItem: " + oldItem);
                foreach (var expectedItem in ExpectedOldItems)
                    Assert.True(e.OldItems.Contains(expectedItem), "expectedItem was not in e.OldItems. expectedItem: " + expectedItem);
            }
            else
            {
                Assert.Null(e.OldItems);
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NumCollectionChangedFired++;
            Assert.Equal(ExpectedAction, e.Action);
            switch (ExpectedAction)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Replace:
                    if (!SkipVerifyEventArgs)
                        VerifyEventArgs(e);
                    break;
                default:
                    throw new NotSupportedException("Does not support this action yet.");
            }
        }

        private void Collection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var item in _expectedPropertyChanged)
            {
                if (item.Name == e.PropertyName)
                    item.IsFound = true;
            }
        }

        /// <summary>
        /// Helper class to keep track of what propertychanges we expect and whether they were found or not.
        /// </summary>
        private class PropertyNameExpected
        {
            internal PropertyNameExpected(string name)
            {
                Name = name;
            }

            internal string Name { get; private set; }
            internal bool IsFound { get; set; }
        }
        #endregion
    }
}
