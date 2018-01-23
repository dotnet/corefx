using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    public static partial class PublicMethodsTests
    {

        /// <summary>
        /// Verifies that no events are raised when clearing an empty collection.
        /// </summary>
        [Fact]
        public static void ClearTestEmpty()
        {
            ObservableCollection<string> col = new ObservableCollection<string>();

            //tests that nothing is raised or changed if collection already empty.
            var helper = new CollectionAndPropertyChangedTester();
            helper.ClearTest(col);
        }


        /// <summary>
        /// Tests that it's possible to add a range to the end of a collection. Consists of:
        /// - Empty collection
        /// - Collection already containing elements
        /// - Trying to add an empty collection should not raise any events
        /// </summary>
        [Fact]
        public static void AddOrInsertRangeTest()
        {
            string[] anArray = { "one", "two", "three" };
            var col = new ObservableCollection<string>();

            //inserting to new collection
            var helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, anArray, 0);

            //adding to inistialized collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, anArray, null);

            //inserting collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, anArray, 0);

            //adding single-item collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, new[] { "single item" }, null);

            //inserting single-item collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, new[] { "single item" }, 0);

            //adding empty collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, new string[] { }, null);

            //inserting empty collection
            helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertRangeTest(col, new string[] { }, 0);
        }

        [Fact]
        public static void RemoveRangeTest_Items()
        {
            string[] items = { "one", "two", "three" };
            ObservableCollection<string> col;

            //remove first two
            resetCollection();
            var toRemove = items.Take(2).ToArray();
            var helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (0, toRemove));

            //remove last two
            resetCollection();
            toRemove = items.Skip(1).ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (1, toRemove));

            //remove first and last
            resetCollection();
            toRemove = items.Where((i) => i != items[1]).ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (0, items.Take(1)), (1, items.Skip(2)));

            //remove last and first
            resetCollection();
            toRemove = items.Where((i) => i != items[1]).Reverse().ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (2, items.Skip(2)), (0, items.Take(1)));

            //remove single item
            resetCollection();
            toRemove = items.Skip(1).Take(1).ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (1, toRemove));

            //remove empty collection
            resetCollection();
            toRemove = new string[0];
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove);

            //remove all items
            resetCollection();
            toRemove = items;
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove);

            //remove items in reversed order
            //TODO: implement clustering items regardless to their removal order.
            resetCollection();
            toRemove = items.Skip(1).Reverse().ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (2, toRemove.Take(1)), (1, toRemove.Skip(1)));

            //remove non existing items
            resetCollection();
            toRemove = new[] { "zero" }.Concat(items.Skip(1)).Concat(new[] { "four", "five" }).ToArray();
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, toRemove, (1, items.Skip(1)));

            void resetCollection() => col = new ObservableCollection<string>(items);
        }

        [Fact]
        public static void RemoveRangeTest_IndexCount()
        {
            string[] items = { "one", "two", "three", "four", "five", "six" };
            var col = new ObservableCollection<string>(items);
            var helper = new CollectionAndPropertyChangedTester();

            //remove first items
            helper.RemoveRangeTest(col, 0, 2);

            //remove subsequent items
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, 0, 2);

            //remove single item
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, 0, 1);

            //clear collection
            col = new ObservableCollection<string>(items);
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveRangeTest(col, 0, col.Count);

            col.Clear();
            Assert.Throws<ArgumentOutOfRangeException>(() => { col.RemoveRange(0, 1); });
        }

        /// <summary>
        /// Verifies that all matches are removed from the collection.
        /// </summary>
        [Fact]
        public static void RemoveAllTest()
        {
            var items = new[] { "alpha", "bravo", "charlie", "delta", "echo" };
            ObservableCollection<string> col;
            CollectionAndPropertyChangedTester tester;
            void reset()
            {
                col = new ObservableCollection<string>(items);
                tester = new CollectionAndPropertyChangedTester();
            }

            reset();
            col.Clear();
            Assert.Throws<ArgumentNullException>("match", () => col.RemoveAll(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.RemoveAll(0, 1, i => true));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.RemoveAll(1, 0, i => true));

            //remove single item
            reset();
            tester.RemoveAllTest(col, i => i.StartsWith("c"), (2, items.Skip(2).Take(1)));

            //remove multiple items
            reset();
            tester.RemoveAllTest(col, i => i.First() > 'b', (2, items.Skip(2)));

            //remove non-existing items
            reset();
            tester.RemoveAllTest(col, i => i == "foxtrot");

            //remove 1st and 4th elements
            reset();
            tester.RemoveAllTest(col, i => i.EndsWith('a'), (0, items.Take(1)), (2, items.Skip(3).Take(1)));

            //remove 1st, 3rd and 5th
            reset();
            tester.RemoveAllTest(col, (i) => Array.IndexOf(items, i) % 2 == 0, (0, items.Take(1)), (1, items.Skip(2).Take(1)), (2, items.TakeLast(1)));

            //remove all
            reset();
            tester.RemoveAllTest(col, i => true, (0, items));

            //remove all within boundary 1+3
            reset();
            tester.RemoveAllTest(col, 1, 3, i => true, (1, items.Skip(1).Take(3)));

            reset();
            tester.RemoveAllTest(col, 1, 3, i => i.EndsWith('a'), (3, new[] { items[3] }));

            tester.RemoveAllTest(col, 0, 0, i => true);
        }


        /// <summary>
        /// Tests that the collecion is cleared and the specified items are added.
        /// Additionally, tests that the minimal items to be refreshed will be refreshed.
        /// </summary>
        [Fact]
        public static void RaplaceRangeTest()
        {
            string[] allItems, oldItems, newItems;
            ObservableCollection<string> col;
            CollectionAndPropertyChangedTester tester;
            void reset(params string[] itemsAddedToCollection)
            {
                allItems = new[] { "alpha", "bravo", "charlie", "delta", "echo", "foxtrot", "golf" };
                oldItems = allItems.Take(3).ToArray();
                newItems = allItems.Skip(3).Take(3).ToArray();
                col = new ObservableCollection<string>(itemsAddedToCollection ?? Enumerable.Empty<string>());
                tester = new CollectionAndPropertyChangedTester();
            }

            reset();
            Assert.Throws<ArgumentOutOfRangeException>(() => col.ReplaceRange(1, 0, Enumerable.Empty<string>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.ReplaceRange(0, 1, Enumerable.Empty<string>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.ReplaceRange(-1, 0, Enumerable.Empty<string>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => col.ReplaceRange(0, -1, Enumerable.Empty<string>()));
            //assert does not throw
            col.ReplaceRange(0, 0, Enumerable.Empty<string>());

            Assert.Throws<ArgumentNullException>("collection", () => col.ReplaceRange(null));
            Assert.Throws<ArgumentNullException>("collection", () => col.ReplaceRange(null, EqualityComparer<string>.Default));
            Assert.Throws<ArgumentNullException>("comparer", () => col.ReplaceRange(oldItems, null));

            //replace 3 items with 3 other items
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                newItems,
                (0, NotifyCollectionChangedAction.Replace, newItems, oldItems));

            //replace 3 items with 3 similar items (do nothing)
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                oldItems);

            //replace 2 equal items and remove last
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                oldItems.Take(2),
                (2, NotifyCollectionChangedAction.Remove, null, oldItems.Last()));

            //replace one item and leave last two (by similarity)
            //the ObservableCollection is only affording to check for equality in similar position
            //not search for each item for equality.
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                oldItems.Skip(1),
                (0, NotifyCollectionChangedAction.Replace, oldItems.Skip(1), oldItems.Take(2)),
                (2, NotifyCollectionChangedAction.Remove, null, oldItems.Skip(2)));

            //skip one similar, replace one, skip another equal one, then add three.
            reset(oldItems);
            allItems[1] = "_bravo";
            tester.ReplaceRangeTest(col,
                allItems,
                (1, NotifyCollectionChangedAction.Replace, allItems.Skip(1).Take(1), oldItems.Skip(1).Take(1)),
                (3, NotifyCollectionChangedAction.Add, allItems.Skip(3), null));

            //replace 3 items with empty collection
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                Enumerable.Empty<string>(),
                (0, NotifyCollectionChangedAction.Reset, Enumerable.Empty<string>(), oldItems));

            //replace 3 items with new 3 equal + added items
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                oldItems.Concat(newItems),
                (3, NotifyCollectionChangedAction.Add, newItems, Enumerable.Empty<string>()));

            //replace 3 items with new 2 equal + added items
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                oldItems.Take(2).Concat(newItems),
                (2, NotifyCollectionChangedAction.Replace, newItems.Take(1), oldItems.Skip(2)),
                (3, NotifyCollectionChangedAction.Add, newItems.Skip(1), Enumerable.Empty<string>()));

            reset(allItems);
            var newItem = '_' + allItems[3];
            newItems = allItems.Take(3).Concat(new[] { newItem }).Concat(allItems.Skip(4)).ToArray();
            tester.ReplaceRangeTest(col,
                newItems,
                (3, NotifyCollectionChangedAction.Replace, newItem, allItems[3]));

            //replace empty collection with empty collection
            reset();
            tester.ReplaceRangeTest(col,
                Enumerable.Empty<string>());

            //replace empty collection with added collection
            reset();
            tester.ReplaceRangeTest(col,
                allItems, (0, NotifyCollectionChangedAction.Add, allItems, null));

            /* Tests using index and count */

            //replace
            // alpha bravo charlie
            //with 
            // bravo delta echo foxtrot golf
            // starting from 2nd position
            reset(oldItems);
            tester.ReplaceRangeTest(col,
                allItems.Skip(1).Take(1).Concat(allItems.Skip(3)), 1, col.Count - 1, null,
                (2, NotifyCollectionChangedAction.Replace, allItems.Skip(3).Take(1), allItems.Skip(2).Take(1)),
                (3, NotifyCollectionChangedAction.Add, allItems.Skip(4), null));

            reset(oldItems);
            tester.ReplaceRangeTest(col,
                newItems.Take(1), 0, 1, null,
                (0, NotifyCollectionChangedAction.Replace, newItems.Take(1), oldItems.Take(1)));

            reset();
            tester.ReplaceRangeTest(col,
                allItems, 0, 0, null, (0, NotifyCollectionChangedAction.Add, allItems, null));

            //replace empty collection with empty collection by index.
            reset();
            tester.ReplaceRangeTest(col,
                Enumerable.Empty<string>(), 0, 0, null);

            //TODO write more tests, and also such including comparer
        }


    }

    public partial class CollectionAndPropertyChangedTester
    {
        /// <summary>
        /// Will perform an AddRange or InsertRange on the given Collection depending on whether the 
        /// <paramref name="insertIndex"/> is null or not. If it is null, will Add, otherwise, will Insert.
        /// </summary>
        public void AddOrInsertRangeTest(ObservableCollection<string> collection, IEnumerable<string> itemsToAdd, int? insertIndex = null)
        {
            var count = itemsToAdd.Count();
            INotifyPropertyChanged collectionPropertyChanged = collection;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            collection.CollectionChanged += Collection_CollectionChanged;

            var expectedCollection = new List<string>(collection);

            if (count > 0)
            {
                ExpectedAction = NotifyCollectionChangedAction.Add;
                ExpectedCollectionChangedFired++;
                ExpectedNewItems = itemsToAdd.ToArray();
                ExpectedNewStartingIndex = insertIndex ?? collection.Count;
                ExpectedOldItems = null;
                ExpectedOldStartingIndex = -1;

                _expectedPropertyChanged = new[]
                {
                    new PropertyNameExpected(COUNT),
                    new PropertyNameExpected(ITEMARRAY)
                };
            }

            if (insertIndex.HasValue)
            {
                var index = insertIndex.Value;
                expectedCollection.InsertRange(index, itemsToAdd);
                collection.InsertRange(index, itemsToAdd);
            }
            else
            {
                expectedCollection.AddRange(itemsToAdd);
                collection.AddRange(itemsToAdd);
            }

            Assert.Equal(expectedCollection, collection);
            Assert.Equal(ExpectedCollectionChangedFired, NumCollectionChangedFired);

            if (_expectedPropertyChanged != null)
                foreach (var item in _expectedPropertyChanged)
                    Assert.True(item.IsFound, "The propertychanged event should have fired for " + item.Name + ", since we just added an item");

            collection.CollectionChanged -= Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
        }

        /// <summary>
        /// Verifies that first appearance of each of the items is remvoved from the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="itemsToRemove"></param>
        public void RemoveRangeTest(ObservableCollection<string> collection, IEnumerable<string> itemsToRemove, params (int StartingIndex, IEnumerable<string> Items)[] clusters)
        {
            // prepare
            INotifyPropertyChanged collectionPropertyChanged = collection;
            var eventArgsCollection = new List<NotifyCollectionChangedEventArgs>();
            void logEventArgs(object sender, NotifyCollectionChangedEventArgs nccea) => eventArgsCollection.Add(nccea);

            collection.CollectionChanged += Collection_CollectionChanged;
            collectionPropertyChanged.PropertyChanged += Collection_PropertyChanged;
            collection.CollectionChanged += logEventArgs;

            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            var expectedCollection = new List<string>(collection);
            foreach (var item in itemsToRemove)
                expectedCollection.Remove(item);

            ExpectedNewItems = null;
            ExpectedNewStartingIndex = -1;

            ExpectedAction = expectedCollection.Count == 0 ? NotifyCollectionChangedAction.Reset : NotifyCollectionChangedAction.Remove;
            ExpectedCollectionChangedFired = clusters == null ? 1 : clusters.Length;

            SkipVerifyEventArgs = true;

            // action
            collection.RemoveRange(itemsToRemove);

            // assert
            Assert.Equal(expectedCollection, collection);
            if (clusters != null)
            {
                for (int i = 0; i < clusters.Length; i++)
                {
                    var cluster = clusters[i];
                    var args = eventArgsCollection[i];

                    Assert.Equal(null, args.NewItems);
                    Assert.Equal(-1, args.NewStartingIndex);

                    Assert.Equal(cluster.StartingIndex, args.OldStartingIndex);
                    Assert.Equal(cluster.Items, args.OldItems.Cast<string>());
                }
            }

            collectionPropertyChanged.PropertyChanged -= Collection_PropertyChanged;
            collection.CollectionChanged -= Collection_CollectionChanged;
            collection.CollectionChanged -= logEventArgs;
        }


        /// <summary>
        /// Verifies that the items in the specified range are removed.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void RemoveRangeTest(ObservableCollection<string> collection, int index, int count)
        {
            // prepare
            INotifyPropertyChanged inpc = collection;
            inpc.PropertyChanged += Collection_PropertyChanged;
            collection.CollectionChanged += Collection_CollectionChanged;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };

            var expectedCollection = new List<string>(collection);
            expectedCollection.RemoveRange(index, count);

            ExpectedNewItems = null;
            ExpectedNewStartingIndex = -1;

            ExpectedCollectionChangedFired = 1;
            var isReset = count > 1 && expectedCollection.Count == 0;
            ExpectedAction = isReset ? NotifyCollectionChangedAction.Reset : NotifyCollectionChangedAction.Remove;

            if (!isReset)
            {
                ExpectedOldStartingIndex = index;
                ExpectedOldItems = collection.Skip(index).Take(count).ToArray();
            }
            else
                ExpectedOldStartingIndex = -1;

            // act
            collection.RemoveRange(index, count);

            // cleanup
            collection.CollectionChanged -= Collection_CollectionChanged;
            inpc.PropertyChanged -= Collection_PropertyChanged;

            // assert
            Assert.Equal(expectedCollection, collection);
        }

        public void RemoveAllTest(ObservableCollection<string> collection, Predicate<string> match, params (int StartingIndex, IEnumerable<string> Items)[] clusters) =>
            RemoveAllTest(collection, 0, collection.Count, match, clusters);

        public void RemoveAllTest(ObservableCollection<string> collection, int index, int count, Predicate<string> match, params (int StartingIndex, IEnumerable<string> Items)[] clusters)
        {
            // prepare
            List<NotifyCollectionChangedEventArgs> eventArgsCollection = new List<NotifyCollectionChangedEventArgs>();
            void logEventArgs(object sender, NotifyCollectionChangedEventArgs e) => eventArgsCollection.Add(e);
            INotifyPropertyChanged inpc = collection;
            inpc.PropertyChanged += Collection_PropertyChanged;
            collection.CollectionChanged += Collection_CollectionChanged;
            collection.CollectionChanged += logEventArgs;
            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };
            ExpectedAction = NotifyCollectionChangedAction.Remove;

            var expected = collection.ToList();
            int expectedRemoveCount;
            if (index == 0 && count == collection.Count)
                expectedRemoveCount = expected.RemoveAll(match);
            else
            {
                var range = expected.GetRange(index, count);
                expected.RemoveRange(index, count);
                expectedRemoveCount = range.RemoveAll(match);
                expected.InsertRange(index, range);
            }

            if (clusters == null)
                clusters = new(int, IEnumerable<string>)[] { };

            if (clusters.Any())
                ExpectedCollectionChangedFired = clusters.Length;
            SkipVerifyEventArgs = true;

            // act                  
            var removeCount = collection.RemoveAll(index, count, match);

            // assert
            Assert.Equal(expected, collection);
            Assert.Equal(expectedRemoveCount, removeCount);

            foreach (var prop in _expectedPropertyChanged)
                Assert.Equal(removeCount > 0, prop.IsFound);

            for (int i = 0; i < clusters.Length; i++)
            {
                var cluster = clusters[i];
                var args = eventArgsCollection[i];

                Assert.Equal(null, args.NewItems);
                Assert.Equal(-1, args.NewStartingIndex);
                Assert.Equal(cluster.StartingIndex, args.OldStartingIndex);
                Assert.Equal(cluster.Items, args.OldItems.Cast<string>());
            }

            collection.CollectionChanged -= logEventArgs;
            collection.CollectionChanged -= Collection_CollectionChanged;
            inpc.PropertyChanged -= Collection_PropertyChanged;
        }



        /// <summary>
        /// Use when no events are expected.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newItems"></param>
        public void ReplaceRangeTest(ObservableCollection<string> collection, IEnumerable<string> newItems) =>
            ReplaceRangeTest(collection, newItems, Enumerable.Empty<(int, NotifyCollectionChangedAction, IEnumerable<string>, IEnumerable<string>)>().ToArray());

        public void ReplaceRangeTest(ObservableCollection<string> collection, IEnumerable<string> newItems,
            params (int StartingIndex, NotifyCollectionChangedAction Action, string NewItem, string oldItem)[] expectedEvents) =>
            ReplaceRangeTest(collection, newItems, expectedEvents.Select(ev =>
                (ev.StartingIndex, ev.Action, Wrap(ev.NewItem), Wrap(ev.oldItem))).ToArray());

        public void ReplaceRangeTest(ObservableCollection<string> collection, IEnumerable<string> newItems,
            params (int StartingIndex, NotifyCollectionChangedAction Action, IEnumerable<string> NewItems, IEnumerable<string> OldItems)[] expectedvents) =>
            ReplaceRangeTest(collection, newItems, 0, collection.Count, null, expectedvents);

        public void ReplaceRangeTest(ObservableCollection<string> collection, IEnumerable<string> newItems, IEqualityComparer<string> comparer,
            params (int StartingIndex, NotifyCollectionChangedAction Action, string NewItem, string OldItem)[] expectedEvents) =>
            ReplaceRangeTest(collection, newItems, 0, collection.Count, comparer,
                expectedEvents.Select(ev =>
                    (ev.StartingIndex, ev.Action, Wrap(ev.NewItem), Wrap(ev.OldItem))).ToArray());

        public void ReplaceRangeTest(ObservableCollection<string> collection, IEnumerable<string> newItems, int index, int count, IEqualityComparer<string> comparer,
            params (int StartingIndex, NotifyCollectionChangedAction Action, IEnumerable<string> NewItems, IEnumerable<string> OldItems)[] expectedEvents)
        {
            Debug.Assert(expectedEvents != null);

            // prepare
            SkipVerifyEventArgs = true;
            INotifyPropertyChanged inpc = collection;

            _expectedPropertyChanged = new[]
            {
                new PropertyNameExpected(COUNT),
                new PropertyNameExpected(ITEMARRAY)
            };
            var eventargsCollection = new List<NotifyCollectionChangedEventArgs>();
            void logCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) => eventargsCollection.Add(args);

            inpc.PropertyChanged += Collection_PropertyChanged;
            collection.CollectionChanged += logCollectionChanged;

            IEnumerable<string> expectedCollection;
            if (index == 0 && count == collection.Count)
            {
                expectedCollection = newItems.ToArray();
            }
            else
            {
                expectedCollection = collection
                    .Take(index)
                    .Concat(newItems)
                    .Concat(collection.Skip(index + count)).ToArray();
            }

            // act
            if (comparer == null)
            {
                collection.ReplaceRange(index, count, newItems);
                //for later on
                comparer = EqualityComparer<string>.Default;
            }
            else
                collection.ReplaceRange(index, count, newItems, comparer);

            // cleanup
            inpc.PropertyChanged -= Collection_PropertyChanged;
            collection.CollectionChanged -= logCollectionChanged;

            //assert            
            comparer = EqualityComparer<string>.Default;

            Assert.Equal(expectedCollection, collection, comparer);
            Assert.Equal(expectedEvents.Length, eventargsCollection.Count);

            if (expectedEvents.Length == 0)
                Assert.False(_expectedPropertyChanged.Any(p => p.IsFound), "Expected no property change.");
            else
            {
                var expectedCount = !expectedEvents.All(e => e.Action == NotifyCollectionChangedAction.Replace);
                Assert.Equal(expectedCount, _expectedPropertyChanged.Single(p => p.Name == COUNT).IsFound);

                Assert.True(_expectedPropertyChanged.Single(p => p.Name == ITEMARRAY).IsFound);
            }

            for (int i = 0; i < expectedEvents.Length; i++)
            {
                var expectedEvent = expectedEvents[i];
                var actualEvent = eventargsCollection[i];
                var action = actualEvent.Action;

                Assert.Equal(expectedEvent.Action, action);

                switch (action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Assert.Equal(-1, actualEvent.OldStartingIndex);
                        Assert.Equal(null, actualEvent.OldItems);

                        Assert.Equal(expectedEvent.StartingIndex, actualEvent.NewStartingIndex);
                        Assert.Equal(expectedEvent.NewItems, actualEvent.NewItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Assert.Equal(-1, actualEvent.NewStartingIndex);
                        Assert.Equal(null, actualEvent.NewItems);

                        Assert.Equal(expectedEvent.StartingIndex, actualEvent.OldStartingIndex);
                        Assert.Equal(expectedEvent.OldItems, actualEvent.OldItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        Assert.Equal(expectedEvent.StartingIndex, actualEvent.OldStartingIndex);
                        Assert.Equal(expectedEvent.StartingIndex, actualEvent.NewStartingIndex);

                        Assert.Equal(expectedEvent.NewItems, actualEvent.NewItems.Cast<string>());
                        Assert.Equal(expectedEvent.OldItems, actualEvent.OldItems.Cast<string>());
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Assert.Equal(0, collection.Count);
                        Assert.Equal(-1, actualEvent.NewStartingIndex);
                        Assert.Equal(-1, actualEvent.OldStartingIndex);
                        Assert.Equal(null, actualEvent.NewItems);
                        Assert.Equal(null, actualEvent.OldItems);

                        break;
                    default:
                        Assert.False(true, $"Action '{action}' not expected in '{nameof(ObservableCollection<string>.ReplaceRange)}'.");
                        break;
                }
            }
        }
        private static IEnumerable<string> Wrap(string item)
        {
            return item == null
                ? Enumerable.Empty<string>()
                : Enumerable.Repeat(item, 1);
        }
    }
}
