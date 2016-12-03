// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Collections.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public class List_Generic_Tests_Insert
    {
        internal class Driver<T>
        {
            #region Insert

            public void BasicInsert(T[] items, T item, int index, int repeat)
            {
                List<T> list = new List<T>(items);

                for (int i = 0; i < repeat; i++)
                {
                    list.Insert(index, item);
                }

                Assert.True(list.Contains(item)); 
                Assert.Equal(list.Count, items.Length + repeat);


                for (int i = 0; i < index; i++)
                {
                    Assert.Equal(list[i], items[i]);
                }

                for (int i = index; i < index + repeat; i++)
                {
                    Assert.Equal(list[i], item);
                }


                for (int i = index + repeat; i < list.Count; i++)
                {
                    Assert.Equal(list[i], items[i - repeat]);
                }
            }

            public void InsertValidations(T[] items)
            {
                List<T> list = new List<T>(items);
                int[] bad = new int[] { items.Length + 1, items.Length + 2, int.MaxValue, -1, -2, int.MinValue };
                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(bad[i], items[0]));
                }
            }

            public void NonGenericIListBasicInsert(T[] items, T item, int index, int repeat)
            {
                List<T> list = new List<T>(items);
                IList iList = list;

                for (int i = 0; i < repeat; i++)
                {
                    iList.Insert(index, item);
                }

                Assert.True(list.Contains(item));
                Assert.Equal(list.Count, items.Length + repeat);

                for (int i = 0; i < index; i++)
                {
                    Assert.Equal(list[i], items[i]);
                }

                for (int i = index; i < index + repeat; i++)
                {
                    Assert.Equal((object)list[i], item);
                }


                for (int i = index + repeat; i < list.Count; i++)
                {
                    Assert.Equal(list[i], items[i - repeat]);
                }
            }

            public void NonGenericIListInsertValidations(T[] items)
            {
                List<T> list = new List<T>(items);
                IList iList = list;
                int[] bad = new int[] { items.Length + 1, items.Length + 2, int.MaxValue, -1, -2, int.MinValue };
                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => iList.Insert(bad[i], items[0]));
                }

                Assert.Throws<ArgumentException>(() => iList.Insert(0, new LinkedListNode<string>("blargh")));
            }

            #endregion

            #region InsertRange

            public void InsertRangeICollection(T[] itemsX, T[] itemsY, int index, int repeat, Func<T[], IEnumerable<T>> constructIEnumerable)
            {
                List<T> list = new List<T>(constructIEnumerable(itemsX));

                for (int i = 0; i < repeat; i++)
                {
                    list.InsertRange(index, constructIEnumerable(itemsY));
                }

                foreach (T item in itemsY)
                {
                    Assert.True(list.Contains(item));
                }
                Assert.Equal(list.Count, itemsX.Length + (itemsY.Length * repeat));

                for (int i = 0; i < index; i++)
                {
                    Assert.Equal(list[i], itemsX[i]);
                }

                for (int i = index; i < index + (itemsY.Length * repeat); i++)
                {
                    Assert.Equal(list[i], itemsY[(i - index) % itemsY.Length]);
                }

                for (int i = index + (itemsY.Length * repeat); i < list.Count; i++)
                {
                    Assert.Equal(list[i], itemsX[i - (itemsY.Length * repeat)]);
                }

                //InsertRange into itself
                list = new List<T>(constructIEnumerable(itemsX));
                list.InsertRange(index, list);

                foreach (T item in itemsX)
                {
                    Assert.True(list.Contains(item));
                }
                Assert.Equal(list.Count, itemsX.Length + (itemsX.Length));

                for (int i = 0; i < index; i++)
                {
                    Assert.Equal(list[i], itemsX[i]);
                }

                for (int i = index; i < index + (itemsX.Length); i++)
                {
                    Assert.Equal(list[i], itemsX[(i - index) % itemsX.Length]);
                }

                for (int i = index + (itemsX.Length); i < list.Count; i++)
                {
                    Assert.Equal(list[i], itemsX[i - (itemsX.Length)]);
                }
            }

            public void InsertRangeList(T[] itemsX, T[] itemsY, int index, int repeat, Func<T[], IEnumerable<T>> constructIEnumerable)
            {
                List<T> list = new List<T>(constructIEnumerable(itemsX));

                for (int i = 0; i < repeat; i++)
                {
                    list.InsertRange(index, new List<T>(constructIEnumerable(itemsY)));
                }

                foreach (T item in itemsY)
                {
                    Assert.True(list.Contains(item));
                }
                Assert.Equal(list.Count, itemsX.Length + (itemsY.Length * repeat));

                for (int i = 0; i < index; i++)
                {
                    Assert.Equal(list[i], itemsX[i]);
                }

                for (int i = index; i < index + (itemsY.Length * repeat); i++)
                {
                    Assert.Equal(list[i], itemsY[(i - index) % itemsY.Length]);
                }

                for (int i = index + (itemsY.Length * repeat); i < list.Count; i++)
                {
                    Assert.Equal(list[i], itemsX[i - (itemsY.Length * repeat)]);
                }
            }

            public void InsertRangeValidations(T[] items, Func<T[], IEnumerable<T>> constructIEnumerable)
            {
                List<T> list = new List<T>(constructIEnumerable(items));
                int[] bad = new int[] { items.Length + 1, items.Length + 2, int.MaxValue, -1, -2, int.MinValue };
                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => list.InsertRange(bad[i], constructIEnumerable(items)));
                }

                Assert.Throws<ArgumentNullException>(() => list.InsertRange(0, null));
            }

            public IEnumerable<T> ConstructTestCollection(T[] items)
            {
                return items;
            }

            #endregion

            #region GetRange

            public void BasicGetRange(T[] items, int index, int count)
            {
                List<T> list = new List<T>(items);
                List<T> range = list.GetRange(index, count);

                //ensure range is good
                for (int i = 0; i < count; i++)
                {
                    Assert.Equal(range[i], items[i + index]); //String.Format("Err_170178aqhbpa Expected item: {0} at: {1} actual: {2}", items[i + index], i, range[i])
                }

                //ensure no side effects
                for (int i = 0; i < items.Length; i++)
                {
                    Assert.Equal(list[i], items[i]); //String.Format("Err_00125698ahpap Expected item: {0} at: {1} actual: {2}", items[i], i, list[i])
                }
            }

            public void EnsureRangeIsReference(T[] items, T item, int index, int count)
            {
                List<T> list = new List<T>(items);
                List<T> range = list.GetRange(index, count);
                T tempItem = list[index];
                range[0] = item;
                Assert.Equal(list[index], tempItem); //String.Format("Err_707811hapba Expected item: {0} at: {1} actual: {2}", tempItem, index, list[index])
            }

            public void EnsureThrowsAfterModification(T[] items, T item, int index, int count)
            {
                List<T> list = new List<T>(items);
                List<T> range = list.GetRange(index, count);
                T tempItem = list[index];
                list[index] = item;

                Assert.Equal(range[0], tempItem); //String.Format("Err_1221589ajpa Expected item: {0} at: {1} actual: {2}", tempItem, 0, range[0])
            }

            public void GetRangeValidations(T[] items)
            {
                //
                //Always send items.Length is even
                //
                List<T> list = new List<T>(items);
                int[] bad = new int[] {  /**/items.Length,1,
                    /**/
                                    items.Length+1,0,
                    /**/
                                    items.Length+1,1,
                    /**/
                                    items.Length,2,
                    /**/
                                    items.Length/2,items.Length/2+1,
                    /**/
                                    items.Length-1,2,
                    /**/
                                    items.Length-2,3,
                    /**/
                                    1,items.Length,
                    /**/
                                    0,items.Length+1,
                    /**/
                                    1,items.Length+1,
                    /**/
                                    2,items.Length,
                    /**/
                                    items.Length/2+1,items.Length/2,
                    /**/
                                    2,items.Length-1,
                    /**/
                                    3,items.Length-2
                                };

                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentException>(() => list.GetRange(bad[i], bad[++i]));
                }

                bad = new int[] {
                    /**/
                                    -1,-1,
                    /**/
                                    -1,0,
                    /**/
                                    -1,1,
                    /**/
                                    -1,2,
                    /**/
                                    0,-1,
                    /**/
                                    1,-1,
                    /**/
                                    2,-1
                                };

                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => list.GetRange(bad[i], bad[++i]));
                }
            }

            #endregion

            #region Exists(Pred<T>)

            public void Exists_Verify(T[] items)
            {
                Exists_VerifyVanilla(items);
                Exists_VerifyDuplicates(items);
            }

            public void Exists_VerifyExceptions(T[] items)
            {
                List<T> list = new List<T>();
                Predicate<T> predicate = (T item) => { return true; };

                for (int i = 0; i < items.Length; ++i)
                    list.Add(items[i]);

                //[] Verify Null match
                Assert.Throws<ArgumentNullException>(() => list.Exists(null));
            }

            private void Exists_VerifyVanilla(T[] items)
            {
                T expectedItem = default(T);
                List<T> list = new List<T>();
                Predicate<T> expectedItemDelegate = (T item) => { return expectedItem == null ? item == null : expectedItem.Equals(item); };
                bool typeNullable = default(T) == null;

                for (int i = 0; i < items.Length; ++i)
                    list.Add(items[i]);

                //[] Verify Exists returns the correct index
                for (int i = 0; i < items.Length; ++i)
                {
                    expectedItem = items[i];

                    Assert.True(list.Exists(expectedItemDelegate),
                        "Err_282308ahid Verifying Nullable returned FAILED\n");
                }

                //[] Verify Exists returns true if the match returns true on every item
                Assert.True((0 < items.Length) == list.Exists((T item) => { return true; }),
                        "Err_548ahid Verify Exists returns 0 if the match returns true on every item FAILED\n");

                //[] Verify Exists returns false if the match returns false on every item
                Assert.True(!list.Exists((T item) => { return false; }),
                        "Err_30848ahidi Verify Exists returns -1 if the match returns false on every item FAILED\n");

                //[] Verify with default(T)
                list.Add(default(T));
                Assert.True(list.Exists((T item) => { return item == null ? default(T) == null : item.Equals(default(T)); }),
                        "Err_541848ajodi Verify with default(T) FAILED\n");
                list.RemoveAt(list.Count - 1);
            }

            private void Exists_VerifyDuplicates(T[] items)
            {
                T expectedItem = default(T);
                List<T> list = new List<T>();
                Predicate<T> expectedItemDelegate = (T item) => { return expectedItem == null ? item == null : expectedItem.Equals(item); };

                if (0 < items.Length)
                {
                    for (int i = 0; i < items.Length; ++i)
                        list.Add(items[i]);

                    for (int i = 0; i < items.Length && i < 2; ++i)
                        list.Add(items[i]);

                    //[] Verify first item is duplicated
                    expectedItem = items[0];
                    Assert.True(list.Exists(expectedItemDelegate),
                            "Err_2879072qaiadf  Verify first item is duplicated FAILED\n");
                }

                if (1 < items.Length)
                {
                    //[] Verify second item is duplicated
                    expectedItem = items[1];
                    Assert.True(list.Exists(expectedItemDelegate),
                            "Err_4588ajdia Verify second item is duplicated FAILED\n");

                    //[] Verify with match that matches more then one item
                    Assert.True(list.Exists((T item) => { return item != null && (item.Equals(items[0]) || item.Equals(items[1])); }),
                            "Err_4489ajodoi Verify with match that matches more then one item FAILED\n");
                }
            }

            #endregion

            #region Contains

            public void BasicContains(T[] items)
            {
                List<T> list = new List<T>(items);

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.True(list.Contains(items[i]));
                }
            }

            public void NonExistingValues(T[] itemsX, T[] itemsY)
            {
                List<T> list = new List<T>(itemsX);

                for (int i = 0; i < itemsY.Length; i++)
                {
                    Assert.False(list.Contains(itemsY[i]));
                }
            }

            public void RemovedValues(T[] items)
            {
                List<T> list = new List<T>(items);
                for (int i = 0; i < items.Length; i++)
                {
                    list.Remove(items[i]);
                    Assert.False(list.Contains(items[i]));
                }
            }

            public void AddRemoveValues(T[] items)
            {
                List<T> list = new List<T>(items);
                for (int i = 0; i < items.Length; i++)
                {
                    list.Add(items[i]);
                    list.Remove(items[i]);
                    list.Add(items[i]);
                    Assert.True(list.Contains(items[i]));
                }
            }

            public void MultipleValues(T[] items, int times)
            {
                List<T> list = new List<T>(items);

                for (int i = 0; i < times; i++)
                {
                    list.Add(items[items.Length / 2]);
                }

                for (int i = 0; i < times + 1; i++)
                {
                    Assert.True(list.Contains(items[items.Length / 2]));
                    list.Remove(items[items.Length / 2]);
                }
                Assert.False(list.Contains(items[items.Length / 2]));
            }
            public void ContainsNullWhenReference(T[] items, T value)
            {
                if ((object)value != null)
                {
                    throw new ArgumentException("invalid argument passed to testcase");
                }

                List<T> list = new List<T>(items);
                list.Add(value);
                Assert.True(list.Contains(value));
            }

            public void NonGenericIListBasicContains(T[] items)
            {
                List<T> list = new List<T>(items);
                IList iList = list;

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.True(iList.Contains(items[i]));
                }
            }

            public void NonGenericIListNonExistingValues(T[] itemsX, T[] itemsY)
            {
                List<T> list = new List<T>(itemsX);
                IList iList = list;

                for (int i = 0; i < itemsY.Length; i++)
                {
                    Assert.False(iList.Contains(itemsY[i]));
                }
            }

            public void NonGenericIListRemovedValues(T[] items)
            {
                List<T> list = new List<T>(items);
                IList iList = list;
                for (int i = 0; i < items.Length; i++)
                {
                    list.Remove(items[i]);
                    Assert.False(iList.Contains(items[i]));
                }
            }

            public void NonGenericIListAddRemoveValues(T[] items)
            {
                List<T> list = new List<T>(items);
                IList iList = list;
                for (int i = 0; i < items.Length; i++)
                {
                    list.Add(items[i]);
                    list.Remove(items[i]);
                    list.Add(items[i]);
                    Assert.True(iList.Contains(items[i]));
                }
            }

            public void NonGenericIListMultipleValues(T[] items, int times)
            {
                List<T> list = new List<T>(items);
                IList iList = list;

                for (int i = 0; i < times; i++)
                {
                    list.Add(items[items.Length / 2]);
                }

                for (int i = 0; i < times + 1; i++)
                {
                    Assert.True(iList.Contains(items[items.Length / 2]));
                    list.Remove(items[items.Length / 2]);
                }
                Assert.False(iList.Contains(items[items.Length / 2]));
            }

            public void NonGenericIListContainsNullWhenReference(T[] items, T value)
            {
                if ((object)value != null)
                {
                    throw new ArgumentException("invalid argument passed to testcase");
                }

                List<T> list = new List<T>(items);
                IList iList = list;
                list.Add(value);
                Assert.True(iList.Contains(value));
            }

            public void NonGenericIListContainsTestParams()
            {
                List<T> list = new List<T>();
                IList iList = list;

                Assert.False(iList.Contains(new LinkedListNode<string>("rah"))); //"Err_68850ahiuedpz Expected Contains to return false with invalid type");
            }

            #endregion

            #region Clear

            public void ClearEmptyList()
            {
                List<T> list = new List<T>();
                Assert.Equal(list.Count, 0);
                list.Clear();
                Assert.Equal(list.Count, 0);
            }
            public void ClearMultipleTimesEmptyList(int times)
            {
                List<T> list = new List<T>();
                Assert.Equal(list.Count, 0);
                for (int i = 0; i < times; i++)
                {
                    list.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }
            public void ClearNonEmptyList(T[] items)
            {
                List<T> list = new List<T>(items);
                list.Clear();
                Assert.Equal(list.Count, 0);
            }

            public void ClearMultipleTimesNonEmptyList(T[] items, int times)
            {
                List<T> list = new List<T>(items);
                for (int i = 0; i < times; i++)
                {
                    list.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }

            public void NonGenericIListClearEmptyList()
            {
                List<T> list = new List<T>();
                IList iList = list;
                Assert.Equal(list.Count, 0);
                iList.Clear();
                Assert.Equal(list.Count, 0);
            }
            public void NonGenericIListClearMultipleTimesEmptyList(int times)
            {
                List<T> list = new List<T>();
                IList iList = list;
                Assert.Equal(list.Count, 0);
                for (int i = 0; i < times; i++)
                {
                    iList.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }
            public void NonGenericIListClearNonEmptyList(T[] items)
            {
                List<T> list = new List<T>(items);
                IList iList = list;
                iList.Clear();
                Assert.Equal(list.Count, 0);
            }

            public void NonGenericIListClearMultipleTimesNonEmptyList(T[] items, int times)
            {
                List<T> list = new List<T>(items);
                IList iList = list;
                for (int i = 0; i < times; i++)
                {
                    iList.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }

            #endregion

            #region TrueForAll

            public void TrueForAll_VerifyVanilla(T[] items)
            {
                T expectedItem = default(T);
                List<T> list = new List<T>();
                Predicate<T> expectedItemDelegate = delegate (T item) { return expectedItem == null ? item != null : !expectedItem.Equals(item); };
                bool typeNullable = default(T) == null;

                for (int i = 0; i < items.Length; ++i)
                    list.Add(items[i]);

                //[] Verify TrueForAll looks at every item
                for (int i = 0; i < items.Length; ++i)
                {
                    expectedItem = items[i];
                    Assert.False(list.TrueForAll(expectedItemDelegate)); //"Err_282308ahid Verify TrueForAll looks at every item FAILED\n"
                }

                //[] Verify TrueForAll returns true if the match returns true on every item
                Assert.True(list.TrueForAll(delegate (T item) { return true; }),
                        "Err_548ahid Verify TrueForAll returns true if the match returns true on every item FAILED\n");

                //[] Verify TrueForAll returns false if the match returns false on every item
                Assert.True((0 == items.Length) == list.TrueForAll(delegate (T item) { return false; }),
                        "Err_30848ahidi Verify TrueForAll returns " + (0 == items.Length) + " if the match returns false on every item FAILED\n");
            }

            public void TrueForAll_VerifyExceptions(T[] items)
            {
                List<T> list = new List<T>();
                Predicate<T> predicate = delegate (T item) { return true; };
                for (int i = 0; i < items.Length; ++i)
                    list.Add(items[i]);

                //[] Verify Null match
                Assert.Throws<ArgumentNullException>(() => list.TrueForAll(null)); //"Err_858ahia Expected null match to throw ArgumentNullException"
            }

            #endregion

            #region ToArray

            public void BasicToArray(T[] items)
            {
                List<T> list = new List<T>(items);

                T[] arr = list.ToArray();

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.Equal(((object)arr[i]), items[i]);
                }
            }

            public void EnsureNotUnderlyingToArray(T[] items, T item)
            {
                List<T> list = new List<T>(items);
                T[] arr = list.ToArray();
                list[0] = item;
                if (((object)arr[0]) == null)
                    Assert.NotNull(list[0]);
                else
                    Assert.NotEqual(((object)arr[0]), list[0]);
            }

            #endregion
        }

        [Fact]
        public static void InsertTests()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;

            int[] intArr2 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr2[i] = i + 100;

            intDriver.BasicInsert(new int[0], 1, 0, 3);
            intDriver.BasicInsert(intArr1, 101, 50, 4);
            intDriver.BasicInsert(intArr1, 100, 100, 5);
            intDriver.BasicInsert(intArr1, 100, 99, 6);
            intDriver.BasicInsert(intArr1, 50, 0, 7);
            intDriver.BasicInsert(intArr1, 50, 1, 8);
            intDriver.BasicInsert(intArr1, 100, 50, 50);

            intDriver.NonGenericIListBasicInsert(new int[0], 1, 0, 3);
            intDriver.NonGenericIListBasicInsert(intArr1, 101, 50, 4);
            intDriver.NonGenericIListBasicInsert(intArr1, 100, 100, 5);
            intDriver.NonGenericIListBasicInsert(intArr1, 100, 99, 6);
            intDriver.NonGenericIListBasicInsert(intArr1, 50, 0, 7);
            intDriver.NonGenericIListBasicInsert(intArr1, 50, 1, 8);
            intDriver.NonGenericIListBasicInsert(intArr1, 100, 50, 50);

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();
            string[] stringArr2 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr2[i] = "SomeTestString" + (i + 100).ToString();

            stringDriver.BasicInsert(stringArr1, "strobia", 99, 2);
            stringDriver.BasicInsert(stringArr1, "strobia", 100, 3);
            stringDriver.BasicInsert(stringArr1, "strobia", 0, 4);
            stringDriver.BasicInsert(stringArr1, "strobia", 1, 5);
            stringDriver.BasicInsert(stringArr1, "strobia", 50, 51);
            stringDriver.BasicInsert(stringArr1, "strobia", 0, 100);
            stringDriver.BasicInsert(new string[] { null, null, null, "strobia", null }, null, 2, 3);
            stringDriver.BasicInsert(new string[] { null, null, null, null, null }, "strobia", 0, 5);
            stringDriver.BasicInsert(new string[] { null, null, null, null, null }, "strobia", 5, 1);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 99, 2);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 100, 3);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 0, 4);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 1, 5);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 50, 51);
            stringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 0, 100);
            stringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, "strobia", null }, null, 2, 3);
            stringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, null, null }, "strobia", 0, 5);
            stringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, null, null }, "strobia", 5, 1);
        }

        [Fact]
        public static void InsertTests_negative()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;
            intDriver.InsertValidations(intArr1);
            intDriver.NonGenericIListInsertValidations(intArr1);

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();
            stringDriver.InsertValidations(stringArr1);
            stringDriver.NonGenericIListInsertValidations(stringArr1);
        }

        [Fact]
        public static void InsertRangeTests()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;

            int[] intArr2 = new int[10];
            for (int i = 0; i < 10; i++)
            {
                intArr2[i] = i + 100;
            }

            intDriver.InsertRangeICollection(new int[0], intArr1, 0, 1, intDriver.ConstructTestCollection);
            intDriver.InsertRangeICollection(intArr1, intArr2, 0, 1, intDriver.ConstructTestCollection);
            intDriver.InsertRangeICollection(intArr1, intArr2, 1, 1, intDriver.ConstructTestCollection);
            intDriver.InsertRangeICollection(intArr1, intArr2, 99, 1, intDriver.ConstructTestCollection);
            intDriver.InsertRangeICollection(intArr1, intArr2, 100, 1, intDriver.ConstructTestCollection);
            intDriver.InsertRangeICollection(intArr1, intArr2, 50, 50, intDriver.ConstructTestCollection);
            intDriver.InsertRangeList(intArr1, intArr2, 0, 1, intDriver.ConstructTestCollection);

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();
            string[] stringArr2 = new string[10];
            for (int i = 0; i < 10; i++)
                stringArr2[i] = "SomeTestString" + (i + 100).ToString();

            stringDriver.InsertRangeICollection(new string[0], stringArr1, 0, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(stringArr1, stringArr2, 0, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(stringArr1, stringArr2, 1, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(stringArr1, stringArr2, 99, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(stringArr1, stringArr2, 100, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(stringArr1, stringArr2, 50, 50, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(new string[] { null, null, null, null }, stringArr2, 0, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(new string[] { null, null, null, null }, stringArr2, 4, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(new string[] { null, null, null, null }, new string[] { null, null, null, null }, 0, 1, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeICollection(new string[] { null, null, null, null }, new string[] { null, null, null, null }, 4, 50, stringDriver.ConstructTestCollection);
            stringDriver.InsertRangeList(stringArr1, stringArr2, 0, 1, stringDriver.ConstructTestCollection);
        }

        [Fact]
        public static void InsertRangeTests_Negative()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;
            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();

            intDriver.InsertRangeValidations(intArr1, intDriver.ConstructTestCollection);
            stringDriver.InsertRangeValidations(stringArr1, stringDriver.ConstructTestCollection);
        }

        [Fact]
        public static void GetRangeTests()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;

            intDriver.BasicGetRange(intArr1, 50, 50);
            intDriver.BasicGetRange(intArr1, 0, 50);
            intDriver.BasicGetRange(intArr1, 50, 25);
            intDriver.BasicGetRange(intArr1, 0, 25);
            intDriver.BasicGetRange(intArr1, 75, 25);
            intDriver.BasicGetRange(intArr1, 0, 100);
            intDriver.BasicGetRange(intArr1, 0, 99);
            intDriver.BasicGetRange(intArr1, 1, 1);
            intDriver.BasicGetRange(intArr1, 99, 1);
            intDriver.EnsureRangeIsReference(intArr1, 101, 0, 10);
            intDriver.EnsureThrowsAfterModification(intArr1, 10, 10, 10);

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();

            stringDriver.BasicGetRange(stringArr1, 50, 50);
            stringDriver.BasicGetRange(stringArr1, 0, 50);
            stringDriver.BasicGetRange(stringArr1, 50, 25);
            stringDriver.BasicGetRange(stringArr1, 0, 25);
            stringDriver.BasicGetRange(stringArr1, 75, 25);
            stringDriver.BasicGetRange(stringArr1, 0, 100);
            stringDriver.BasicGetRange(stringArr1, 0, 99);
            stringDriver.BasicGetRange(stringArr1, 1, 1);
            stringDriver.BasicGetRange(stringArr1, 99, 1);
            stringDriver.EnsureRangeIsReference(stringArr1, "SometestString101", 0, 10);
            stringDriver.EnsureThrowsAfterModification(stringArr1, "str", 10, 10);
        }

        [Fact]
        public static void GetRangeTests_Negative()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[100];
            for (int i = 0; i < 100; i++)
                intArr1[i] = i;

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();

            stringDriver.GetRangeValidations(stringArr1);
            intDriver.GetRangeValidations(intArr1);
        }

        [Fact]
        public static void ExistsTests()
        {
            Driver<int> intDriver = new Driver<int>();
            Driver<string> stringDriver = new Driver<string>();
            int[] intArray;
            string[] stringArray;
            int arraySize = 16;

            intArray = new int[arraySize];
            stringArray = new string[arraySize];

            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i + 1;
                stringArray[i] = (i + 1).ToString();
            }

            intDriver.Exists_Verify(new int[0]);
            intDriver.Exists_Verify(new int[] { 1 });
            intDriver.Exists_Verify(intArray);

            stringDriver.Exists_Verify(new string[0]);
            stringDriver.Exists_Verify(new string[] { "1" });
            stringDriver.Exists_Verify(stringArray);
        }

        [Fact]
        public static void ExistsTests_Negative()
        {
            Driver<int> intDriver = new Driver<int>();
            Driver<string> stringDriver = new Driver<string>();
            int[] intArray;
            string[] stringArray;
            int arraySize = 16;

            intArray = new int[arraySize];
            stringArray = new string[arraySize];

            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i + 1;
                stringArray[i] = (i + 1).ToString();
            }

            intDriver.Exists_VerifyExceptions(intArray);
            stringDriver.Exists_VerifyExceptions(stringArray);
        }

        [Fact]
        public static void ContainsTests()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr1 = new int[10];
            for (int i = 0; i < 10; i++)
            {
                intArr1[i] = i;
            }

            int[] intArr2 = new int[10];
            for (int i = 0; i < 10; i++)
            {
                intArr2[i] = i + 10;
            }

            intDriver.BasicContains(intArr1);
            intDriver.NonExistingValues(intArr1, intArr2);
            intDriver.RemovedValues(intArr1);
            intDriver.AddRemoveValues(intArr1);
            intDriver.MultipleValues(intArr1, 3);
            intDriver.MultipleValues(intArr1, 5);
            intDriver.MultipleValues(intArr1, 17);
            intDriver.NonGenericIListBasicContains(intArr1);
            intDriver.NonGenericIListNonExistingValues(intArr1, intArr2);
            intDriver.NonGenericIListRemovedValues(intArr1);
            intDriver.NonGenericIListAddRemoveValues(intArr1);
            intDriver.NonGenericIListMultipleValues(intArr1, 3);
            intDriver.NonGenericIListMultipleValues(intArr1, 5);
            intDriver.NonGenericIListMultipleValues(intArr1, 17);
            intDriver.NonGenericIListContainsTestParams();


            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr1 = new string[10];
            for (int i = 0; i < 10; i++)
            {
                stringArr1[i] = "SomeTestString" + i.ToString();
            }
            string[] stringArr2 = new string[10];
            for (int i = 0; i < 10; i++)
            {
                stringArr2[i] = "SomeTestString" + (i + 10).ToString();
            }

            stringDriver.BasicContains(stringArr1);
            stringDriver.NonExistingValues(stringArr1, stringArr2);
            stringDriver.RemovedValues(stringArr1);
            stringDriver.AddRemoveValues(stringArr1);
            stringDriver.MultipleValues(stringArr1, 3);
            stringDriver.MultipleValues(stringArr1, 5);
            stringDriver.MultipleValues(stringArr1, 17);
            stringDriver.ContainsNullWhenReference(stringArr1, null);
            stringDriver.NonGenericIListBasicContains(stringArr1);
            stringDriver.NonGenericIListNonExistingValues(stringArr1, stringArr2);
            stringDriver.NonGenericIListRemovedValues(stringArr1);
            stringDriver.NonGenericIListAddRemoveValues(stringArr1);
            stringDriver.NonGenericIListMultipleValues(stringArr1, 3);
            stringDriver.NonGenericIListMultipleValues(stringArr1, 5);
            stringDriver.NonGenericIListMultipleValues(stringArr1, 17);
            stringDriver.NonGenericIListContainsNullWhenReference(stringArr1, null);
            stringDriver.NonGenericIListContainsTestParams();
        }

        [Fact]
        public static void ClearTests()
        {
            Driver<int> intDriver = new Driver<int>();
            int[] intArr = new int[10];
            for (int i = 0; i < 10; i++)
            {
                intArr[i] = i;
            }

            intDriver.ClearEmptyList();
            intDriver.ClearMultipleTimesEmptyList(1);
            intDriver.ClearMultipleTimesEmptyList(10);
            intDriver.ClearMultipleTimesEmptyList(100);
            intDriver.ClearNonEmptyList(intArr);
            intDriver.ClearMultipleTimesNonEmptyList(intArr, 2);
            intDriver.ClearMultipleTimesNonEmptyList(intArr, 7);
            intDriver.ClearMultipleTimesNonEmptyList(intArr, 31);
            intDriver.NonGenericIListClearEmptyList();
            intDriver.NonGenericIListClearMultipleTimesEmptyList(1);
            intDriver.NonGenericIListClearMultipleTimesEmptyList(10);
            intDriver.NonGenericIListClearMultipleTimesEmptyList(100);
            intDriver.NonGenericIListClearNonEmptyList(intArr);
            intDriver.NonGenericIListClearMultipleTimesNonEmptyList(intArr, 2);
            intDriver.NonGenericIListClearMultipleTimesNonEmptyList(intArr, 7);
            intDriver.NonGenericIListClearMultipleTimesNonEmptyList(intArr, 31);

            Driver<string> stringDriver = new Driver<string>();
            string[] stringArr = new string[10];
            for (int i = 0; i < 10; i++)
            {
                stringArr[i] = "SomeTestString" + i.ToString();
            }

            stringDriver.ClearEmptyList();
            stringDriver.ClearMultipleTimesEmptyList(1);
            stringDriver.ClearMultipleTimesEmptyList(10);
            stringDriver.ClearMultipleTimesEmptyList(100);
            stringDriver.ClearNonEmptyList(stringArr);
            stringDriver.ClearMultipleTimesNonEmptyList(stringArr, 2);
            stringDriver.ClearMultipleTimesNonEmptyList(stringArr, 7);
            stringDriver.ClearMultipleTimesNonEmptyList(stringArr, 31);
            stringDriver.NonGenericIListClearEmptyList();
            stringDriver.NonGenericIListClearMultipleTimesEmptyList(1);
            stringDriver.NonGenericIListClearMultipleTimesEmptyList(10);
            stringDriver.NonGenericIListClearMultipleTimesEmptyList(100);
            stringDriver.NonGenericIListClearNonEmptyList(stringArr);
            stringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 2);
            stringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 7);
            stringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 31);
        }

        [Fact]
        public static void TrueForAllTests()
        {
            Driver<int> intDriver = new Driver<int>();
            Driver<string> stringDriver = new Driver<string>();
            int[] intArray;
            string[] stringArray;
            int arraySize = 16;

            intArray = new int[arraySize];
            stringArray = new string[arraySize];

            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i + 1;
                stringArray[i] = (i + 1).ToString();
            }

            intDriver.TrueForAll_VerifyVanilla(new int[0]);
            intDriver.TrueForAll_VerifyVanilla(new int[] { 1 });
            intDriver.TrueForAll_VerifyVanilla(intArray);

            stringDriver.TrueForAll_VerifyVanilla(new string[0]);
            stringDriver.TrueForAll_VerifyVanilla(new string[] { "1" });
            stringDriver.TrueForAll_VerifyVanilla(stringArray);
        }

        [Fact]
        public static void TrueForAllTests_Negative()
        {
            Driver<int> intDriver = new Driver<int>();
            Driver<string> stringDriver = new Driver<string>();
            int[] intArray;
            string[] stringArray;
            int arraySize = 16;

            intArray = new int[arraySize];
            stringArray = new string[arraySize];

            for (int i = 0; i < arraySize; ++i)
            {
                intArray[i] = i + 1;
                stringArray[i] = (i + 1).ToString();
            }
            intDriver.TrueForAll_VerifyExceptions(intArray);
            stringDriver.TrueForAll_VerifyExceptions(stringArray);
        }
    }
}
