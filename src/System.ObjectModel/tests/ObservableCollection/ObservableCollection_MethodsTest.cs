// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Collections.ObjectModel.Tests
{
    /// <summary>
    /// Tests the public methods in ObservableCollection<T> as well as verifies
    /// that the CollectionChanged events and eventargs are fired and populated
    /// properly.
    /// </summary>
    public static partial class PublicMethodsTest
    {
        /// <summary>
        /// Tests that is possible to Add an item to the collection.
        /// </summary>
        [Fact]
        public static void AddTest()
        {
            string[] anArray = { "one", "two", "three" };
            ObservableCollection<string> col = new ObservableCollection<string>(anArray);
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertItemTest(col, "four");
        }

        /// <summary>
        /// Tests that it is possible to remove an item from the collection.
        /// - Removing an item from the collection results in a false.
        /// - Removing null from collection returns a false.
        /// - Removing an item that has duplicates only takes out the first instance.
        /// </summary>
        [Fact]
        public static void RemoveTest()
        {
            // trying to remove item in collection.
            string[] anArray = { "one", "two", "three", "four" };
            ObservableCollection<string> col = new ObservableCollection<string>(anArray);
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            helper.RemoveItemTest(col, 2, "three", true, hasDuplicates: false);

            // trying to remove item not in collection.
            anArray = new string[] { "one", "two", "three", "four" };
            col = new ObservableCollection<string>(anArray);
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveItemTest(col, -1, "three2", false, hasDuplicates: false);

            // removing null
            anArray = new string[] { "one", "two", "three", "four" };
            col = new ObservableCollection<string>(anArray);
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveItemTest(col, -1, null, false, hasDuplicates: false);

            // trying to remove item in collection that has duplicates.
            anArray = new string[] { "one", "three", "two", "three", "four" };
            col = new ObservableCollection<string>(anArray);
            helper = new CollectionAndPropertyChangedTester();
            helper.RemoveItemTest(col, 1, "three", true, hasDuplicates: true);
            // want to ensure that there is one "three" left in collection and not both were removed.
            int occurrencesThree = 0;
            foreach (var item in col)
            {
                if (item.Equals("three"))
                    occurrencesThree++;
            }
            Assert.Equal(1, occurrencesThree);
        }

        /// <summary>
        /// Tests that a collection can be cleared.
        /// Verifies that no events are raised if the collection was already empty.
        /// </summary>
        [Fact]
        public static void ClearTest()
        {
            string[] anArray = { "one", "two", "three", "four" };
            ObservableCollection<string> col = new ObservableCollection<string>(anArray);

            col.Clear();
            Assert.Equal(0, col.Count);
            Assert.Empty(col);

            Assert.Throws<ArgumentOutOfRangeException>(() => col[1]);

            //tests that the collectionChanged events are fired.
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            col = new ObservableCollection<string>(anArray);
            helper.ClearTest(col);

            //tests that nothing is raised or changed if collection already empty.
            helper = new CollectionAndPropertyChangedTester();
            helper.ClearTest(col);
        }

        /// <summary>
        /// Tests that we can remove items at a specific index, at the middle beginning and end.
        /// </summary>
        [Fact]
        public static void RemoveAtTest()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> col0 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col1 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col2 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);

            col0.RemoveAt(0);
            string collectionString = "";
            foreach (var item in col1)
                collectionString += item + ", ";
            Assert.False(col0.Contains(anArray[0]), "Collection0 should no longer contain the item: " + anArray[0] + " Collection: " + collectionString);

            col1.RemoveAt(1);
            collectionString = "";
            foreach (var item in col1)
                collectionString += item + ", ";
            Assert.False(col1.Contains(anArray[1]), "Collection1 should no longer contain the item: " + anArray[1] + " Collection: " + collectionString);

            col2.RemoveAt(2);
            collectionString = "";
            foreach (var item in col2)
                collectionString += item + ", ";
            Assert.False(col2.Contains(anArray[2]), "Collection2 should no longer contain the item: " + anArray[2] + " Collection: " + collectionString);

            string[] anArrayString = { "one", "two", "three", "four" };
            ObservableCollection<string> col = new ObservableCollection<string>(anArrayString);
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            helper.RemoveItemAtTest(col, 1);
        }

        /// <summary>
        /// Tests that exceptions are thrown:
        /// ArgumentOutOfRangeException when index < 0 or index >= collection.Count.
        /// And that the collection does not change.
        /// </summary>
        [Fact]
        public static void RemoveAtTest_Negative()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> collection = new ObservableCollection<Guid>(anArray);
            collection.CollectionChanged += (o, e) => { throw new ShouldNotBeInvokedException(); };
            int[] iArrInvalidValues = new Int32[] { -1, -2, -100, -1000, -10000, -100000, -1000000, -10000000, -100000000, -1000000000, Int32.MinValue };
            foreach (var index in iArrInvalidValues)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt(index));
                Assert.Equal(anArray.Length, collection.Count);
            }

            int[] iArrLargeValues = new Int32[] { collection.Count, Int32.MaxValue, Int32.MaxValue / 2, Int32.MaxValue / 10 };
            foreach (var index in iArrLargeValues)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt(index));
                Assert.Equal(anArray.Length, collection.Count);
            }
        }

        /// <summary>
        /// Tests that items can be moved throughout a collection whether from 
        /// beginning to end, etc.
        /// </summary>
        [Fact]
        public static void MoveTest()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> col01 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col10 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col12 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col21 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col20 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);

            col01.Move(0, 1);
            Assert.Equal(anArray[0], col01[1]);

            col10.Move(1, 0);
            Assert.Equal(anArray[1], col10[0]);

            col12.Move(1, 2);
            Assert.Equal(anArray[1], col12[2]);

            col21.Move(2, 1);
            Assert.Equal(anArray[2], col21[1]);

            col20.Move(2, 0);
            Assert.Equal(anArray[2], col20[0]);

            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            string[] anArrayString = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>(anArrayString);
            helper.MoveItemTest(collection, 0, 2);
            helper.MoveItemTest(collection, 3, 0);
            helper.MoveItemTest(collection, 1, 2);
        }

        /// <summary>
        /// Tests that:
        /// ArgumentOutOfRangeException is thrown when the source or destination 
        /// Index is >= collection.Count or Index < 0.
        /// </summary>
        /// <remarks>
        /// When the sourceIndex is valid, the item actually is removed from the list.
        /// </remarks>
        [Fact]
        public static void MoveTest_Negative()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = null;

            int validIndex = 2;
            int[] iArrInvalidValues = new Int32[] { -1, -2, -100, -1000, -10000, -100000, -1000000, -10000000, -100000000, -1000000000, Int32.MinValue };
            int[] iArrLargeValues = new Int32[] { anArray.Length, Int32.MaxValue, Int32.MaxValue / 2, Int32.MaxValue / 10 };

            foreach (var index in iArrInvalidValues)
            {
                collection = new ObservableCollection<string>(anArray);
                collection.CollectionChanged += (o, e) => { throw new ShouldNotBeInvokedException(); };

                // invalid startIndex, valid destination index.
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Move(index, validIndex));
                Assert.Equal(anArray.Length, collection.Count);

                // valid startIndex, invalid destIndex.
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Move(validIndex, index));
                //NOTE: It actually moves the item right out of the collection.So the count is one less.
                //Assert.Equal(anArray.Length, collection.Count, "Collection should not have changed. index: " + index);
            }

            foreach (var index in iArrLargeValues)
            {
                collection = new ObservableCollection<string>(anArray);
                collection.CollectionChanged += (o, e) => { throw new ShouldNotBeInvokedException(); };

                // invalid startIndex, valid destination index.
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Move(index, validIndex));
                Assert.Equal(anArray.Length, collection.Count);

                // valid startIndex, invalid destIndex.
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Move(validIndex, index));
                //NOTE: It actually moves the item right out of the collection. So the count is one less.
                //Assert.Equal(anArray.Length, collection.Count, "Collection should not have changed.");
            }
        }

        /// <summary>
        /// Tests that an item can be inserted throughout the collection.
        /// </summary>
        [Fact]
        public static void InsertTest()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> col0 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col1 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);
            ObservableCollection<Guid> col3 = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);

            //inserting item at the beginning.
            Guid g0 = Guid.NewGuid();
            col0.Insert(0, g0);
            Assert.Equal(g0, col0[0]);

            // inserting item in the middle
            Guid g1 = Guid.NewGuid();
            col1.Insert(1, g1);
            Assert.Equal(g1, col1[1]);

            // inserting item at the end.
            Guid g3 = Guid.NewGuid();
            col3.Insert(col3.Count, g3);
            Assert.Equal(g3, col3[col3.Count - 1]);

            string[] anArrayString = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>((IEnumerable<string>)anArrayString);
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            helper.AddOrInsertItemTest(collection, "seven", 2);
            helper.AddOrInsertItemTest(collection, "zero", 0);
            helper.AddOrInsertItemTest(collection, "eight", collection.Count);
        }

        /// <summary>
        /// Tests that:
        /// ArgumentOutOfRangeException is thrown when the Index is >= collection.Count
        /// or Index < 0.  And ensures that the collection does not change.
        /// </summary>
        [Fact]
        public static void InsertTest_Negative()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> collection = new ObservableCollection<Guid>(anArray);
            collection.CollectionChanged += (o, e) => { throw new ShouldNotBeInvokedException(); };

            Guid itemToInsert = Guid.NewGuid();
            int[] iArrInvalidValues = new Int32[] { -1, -2, -100, -1000, -10000, -100000, -1000000, -10000000, -100000000, -1000000000, Int32.MinValue };
            foreach (var index in iArrInvalidValues)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(index, itemToInsert));
                Assert.Equal(anArray.Length, collection.Count);
            }

            int[] iArrLargeValues = new Int32[] { collection.Count + 1, Int32.MaxValue, Int32.MaxValue / 2, Int32.MaxValue / 10 };
            foreach (var index in iArrLargeValues)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(index, itemToInsert));
                Assert.Equal(anArray.Length, collection.Count);
            }
        }

        /// <summary>
        /// Tests that the appropriate collectionchanged event is fired when
        /// an item is replaced in the collection.
        /// </summary>
        [Fact]
        public static void ReplaceItemTest()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>((IEnumerable<string>)anArray);
            CollectionAndPropertyChangedTester helper = new CollectionAndPropertyChangedTester();
            helper.ReplaceItemTest(collection, 1, "seven");
            helper.ReplaceItemTest(collection, 3, "zero");
        }


        /// <summary>
        /// Tests that contains returns true when the item is in the collection
        /// and false otherwise.
        /// </summary>
        [Fact]
        public static void ContainsTest()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>((IEnumerable<string>)anArray);
            string collectionString = "";

            foreach (var item in collection)
                collectionString += item + ", ";

            for (int i = 0; i < collection.Count; ++i)
                Assert.True(collection.Contains(anArray[i]), "ObservableCollection did not contain the item: " + anArray[i] + " Collection: " + collectionString);

            string g = "six";
            Assert.False(collection.Contains(g), "Collection contained an item that should not have been there. guid: " + g + " Collection: " + collectionString);
            Assert.False(collection.Contains(null), "Collection should not have contained null. Collection: " + collectionString);
        }

        /// <summary>
        /// Tests that the index of an item can be retrieved when the item is
        /// in the collection and -1 otherwise.
        /// </summary>
        [Fact]
        public static void IndexOfTest()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>((IEnumerable<string>)anArray);

            for (int i = 0; i < anArray.Length; ++i)
                Assert.Equal(i, collection.IndexOf(anArray[i]));

            Assert.Equal(-1, collection.IndexOf("seven"));
            Assert.Equal(-1, collection.IndexOf(null));

            // testing that the first occurrence is the index returned.
            ObservableCollection<int> intCol = new ObservableCollection<int>();
            for (int i = 0; i < 4; ++i)
                intCol.Add(i % 2);

            Assert.Equal(0, intCol.IndexOf(0));
            Assert.Equal(1, intCol.IndexOf(1));

            IList colAsIList = (IList)intCol;
            var index = colAsIList.IndexOf("stringObj");
            Assert.Equal(-1, index);
        }

        /// <summary>
        /// Tests that the collection can be copied into a destination array.
        /// </summary>
        [Fact]
        public static void CopyToTest()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>((IEnumerable<string>)anArray);

            string[] aCopy = new string[collection.Count];
            collection.CopyTo(aCopy, 0);
            for (int i = 0; i < anArray.Length; ++i)
                Assert.Equal(anArray[i], aCopy[i]);

            // copy observable collection starting in middle, where array is larger than source.
            aCopy = new string[collection.Count + 2];
            int offsetIndex = 1;
            collection.CopyTo(aCopy, offsetIndex);
            for (int i = 0; i < aCopy.Length; i++)
            {
                string value = aCopy[i];
                if (i == 0)
                    Assert.True(null == value, "Should not have a value since we did not start copying there.");
                else if (i == (aCopy.Length - 1))
                    Assert.True(null == value, "Should not have a value since the collection is shorter than the copy array..");
                else
                {
                    int indexInCollection = i - offsetIndex;
                    Assert.Equal(collection[indexInCollection], aCopy[i]);
                }
            }
        }

        /// <summary>
        /// Tests that:
        /// ArgumentOutOfRangeException is thrown when the Index is >= collection.Count
        /// or Index < 0.
        /// ArgumentException when the destination array does not have enough space to
        /// contain the source Collection.
        /// ArgumentNullException when the destination array is null.
        /// </summary>
        [Fact]
        public static void CopyToTest_Negative()
        {
            string[] anArray = new string[] { "one", "two", "three", "four" };
            ObservableCollection<string> collection = new ObservableCollection<string>(anArray);

            int[] iArrInvalidValues = new Int32[] { -1, -2, -100, -1000, -10000, -100000, -1000000, -10000000, -100000000, -1000000000, Int32.MinValue };
            foreach (var index in iArrInvalidValues)
            {
                string[] aCopy = new string[collection.Count];
                Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(aCopy, index));
            }

            int[] iArrLargeValues = new Int32[] { collection.Count, Int32.MaxValue, Int32.MaxValue / 2, Int32.MaxValue / 10 };
            foreach (var index in iArrLargeValues)
            {
                string[] aCopy = new string[collection.Count];
                AssertExtensions.Throws<ArgumentException>("destinationArray", null, () => collection.CopyTo(aCopy, index));
            }

            Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null, 1));

            string[] copy = new string[collection.Count - 1];
            AssertExtensions.Throws<ArgumentException>("destinationArray", "", () => collection.CopyTo(copy, 0));

            copy = new string[0];
            AssertExtensions.Throws<ArgumentException>("destinationArray", "", () => collection.CopyTo(copy, 0));
        }

        /// <summary>
        /// Tests that it is possible to iterate through the collection with an
        /// Enumerator.
        /// </summary>
        [Fact]
        public static void GetEnumeratorTest()
        {
            Guid[] anArray = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            ObservableCollection<Guid> col = new ObservableCollection<Guid>((IEnumerable<Guid>)anArray);

            int i = 0;
            IEnumerator<Guid> e;
            for (e = col.GetEnumerator(); e.MoveNext(); ++i)
            {
                Assert.Equal(anArray[i], e.Current);
            }
            Assert.Equal(col.Count, i);
            e.Dispose();
        }
    }
}
