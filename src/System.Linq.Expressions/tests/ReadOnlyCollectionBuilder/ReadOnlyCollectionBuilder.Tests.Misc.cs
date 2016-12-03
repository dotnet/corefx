// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Linq.Expressions.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness of the List class.
    /// </summary>
    public class ReadOnlyCollectionBuilder_Generic_Tests_Insert
    {
        internal class Driver<T>
        {
            #region Insert

            public void BasicInsert(T[] items, T item, int index, int repeat)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);

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
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                int[] bad = new int[] { items.Length + 1, items.Length + 2, int.MaxValue, -1, -2, int.MinValue };
                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(bad[i], items[0]));
                }
            }

            public void NonGenericIListBasicInsert(T[] items, T item, int index, int repeat)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
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
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;
                int[] bad = new int[] { items.Length + 1, items.Length + 2, int.MaxValue, -1, -2, int.MinValue };
                for (int i = 0; i < bad.Length; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => iList.Insert(bad[i], items[0]));
                }

                Assert.Throws<ArgumentException>(() => iList.Insert(0, new LinkedListNode<string>("blargh")));
            }

            #endregion

            #region Contains

            public void BasicContains(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.True(list.Contains(items[i]));
                }
            }

            public void NonExistingValues(T[] itemsX, T[] itemsY)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(itemsX);

                for (int i = 0; i < itemsY.Length; i++)
                {
                    Assert.False(list.Contains(itemsY[i]));
                }
            }

            public void RemovedValues(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                for (int i = 0; i < items.Length; i++)
                {
                    list.Remove(items[i]);
                    Assert.False(list.Contains(items[i]));
                }
            }

            public void AddRemoveValues(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
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
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);

                for (int i = 0; i < times; i++)
                {
                    list.Add(items[items.Length / 2]);
                }

                for (int i = 0; i < times + 1; i++)
                {
                    Assert.True(list.Contains(items[items.Length / 2]));
                    Assert.True(list.Remove(items[items.Length / 2]));
                }
                Assert.False(list.Contains(items[items.Length / 2]));
            }
            public void ContainsNullWhenReference(T[] items, T value)
            {
                if ((object)value != null)
                {
                    throw new ArgumentException("invalid argument passed to testcase");
                }

                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                list.Add(value);
                Assert.True(list.Contains(value));
            }

            public void NonGenericIListBasicContains(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.True(iList.Contains(items[i]));
                }
            }

            public void NonGenericIListNonExistingValues(T[] itemsX, T[] itemsY)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(itemsX);
                IList iList = list;

                for (int i = 0; i < itemsY.Length; i++)
                {
                    Assert.False(iList.Contains(itemsY[i]));
                }
            }

            public void NonGenericIListRemovedValues(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;
                for (int i = 0; i < items.Length; i++)
                {
                    list.Remove(items[i]);
                    Assert.False(iList.Contains(items[i]));
                }
            }

            public void NonGenericIListAddRemoveValues(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
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
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
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

                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;
                list.Add(value);
                Assert.True(iList.Contains(value));
            }

            public void NonGenericIListContainsTestParams()
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>();
                IList iList = list;

                Assert.False(iList.Contains(new LinkedListNode<string>("rah")));//"Err_68850ahiuedpz Expected Contains to return false with invalid type");
            }

            #endregion

            #region Clear

            public void ClearEmptyList()
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>();
                Assert.Equal(list.Count, 0);
                list.Clear();
                Assert.Equal(list.Count, 0);
            }
            public void ClearMultipleTimesEmptyList(int times)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>();
                Assert.Equal(list.Count, 0);
                for (int i = 0; i < times; i++)
                {
                    list.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }
            public void ClearNonEmptyList(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                list.Clear();
                Assert.Equal(list.Count, 0);
            }

            public void ClearMultipleTimesNonEmptyList(T[] items, int times)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                for (int i = 0; i < times; i++)
                {
                    list.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }

            public void NonGenericIListClearEmptyList()
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>();
                IList iList = list;
                Assert.Equal(list.Count, 0);
                iList.Clear();
                Assert.Equal(list.Count, 0);
            }
            public void NonGenericIListClearMultipleTimesEmptyList(int times)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>();
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
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;
                iList.Clear();
                Assert.Equal(list.Count, 0);
            }

            public void NonGenericIListClearMultipleTimesNonEmptyList(T[] items, int times)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
                IList iList = list;
                for (int i = 0; i < times; i++)
                {
                    iList.Clear();
                    Assert.Equal(list.Count, 0);
                }
            }

            #endregion

            #region ToArray

            public void BasicToArray(T[] items)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);

                T[] arr = list.ToArray();

                for (int i = 0; i < items.Length; i++)
                {
                    Assert.Equal(((object)arr[i]), items[i]);
                }
            }

            public void EnsureNotUnderlyingToArray(T[] items, T item)
            {
                ReadOnlyCollectionBuilder<T> list = new ReadOnlyCollectionBuilder<T>(items);
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

            Driver<string> StringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();
            string[] stringArr2 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr2[i] = "SomeTestString" + (i + 100).ToString();

            StringDriver.BasicInsert(stringArr1, "strobia", 99, 2);
            StringDriver.BasicInsert(stringArr1, "strobia", 100, 3);
            StringDriver.BasicInsert(stringArr1, "strobia", 0, 4);
            StringDriver.BasicInsert(stringArr1, "strobia", 1, 5);
            StringDriver.BasicInsert(stringArr1, "strobia", 50, 51);
            StringDriver.BasicInsert(stringArr1, "strobia", 0, 100);
            StringDriver.BasicInsert(new string[] { null, null, null, "strobia", null }, null, 2, 3);
            StringDriver.BasicInsert(new string[] { null, null, null, null, null }, "strobia", 0, 5);
            StringDriver.BasicInsert(new string[] { null, null, null, null, null }, "strobia", 5, 1);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 99, 2);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 100, 3);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 0, 4);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 1, 5);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 50, 51);
            StringDriver.NonGenericIListBasicInsert(stringArr1, "strobia", 0, 100);
            StringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, "strobia", null }, null, 2, 3);
            StringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, null, null }, "strobia", 0, 5);
            StringDriver.NonGenericIListBasicInsert(new string[] { null, null, null, null, null }, "strobia", 5, 1);
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

            Driver<string> StringDriver = new Driver<string>();
            string[] stringArr1 = new string[100];
            for (int i = 0; i < 100; i++)
                stringArr1[i] = "SomeTestString" + i.ToString();
            StringDriver.InsertValidations(stringArr1);
            StringDriver.NonGenericIListInsertValidations(stringArr1);
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


            Driver<string> StringDriver = new Driver<string>();
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

            StringDriver.BasicContains(stringArr1);
            StringDriver.NonExistingValues(stringArr1, stringArr2);
            StringDriver.RemovedValues(stringArr1);
            StringDriver.AddRemoveValues(stringArr1);
            StringDriver.MultipleValues(stringArr1, 3);
            StringDriver.MultipleValues(stringArr1, 5);
            StringDriver.MultipleValues(stringArr1, 17);
            StringDriver.ContainsNullWhenReference(stringArr1, null);
            StringDriver.NonGenericIListBasicContains(stringArr1);
            StringDriver.NonGenericIListNonExistingValues(stringArr1, stringArr2);
            StringDriver.NonGenericIListRemovedValues(stringArr1);
            StringDriver.NonGenericIListAddRemoveValues(stringArr1);
            StringDriver.NonGenericIListMultipleValues(stringArr1, 3);
            StringDriver.NonGenericIListMultipleValues(stringArr1, 5);
            StringDriver.NonGenericIListMultipleValues(stringArr1, 17);
            StringDriver.NonGenericIListContainsNullWhenReference(stringArr1, null);
            StringDriver.NonGenericIListContainsTestParams();
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

            Driver<string> StringDriver = new Driver<string>();
            string[] stringArr = new string[10];
            for (int i = 0; i < 10; i++)
            {
                stringArr[i] = "SomeTestString" + i.ToString();
            }

            StringDriver.ClearEmptyList();
            StringDriver.ClearMultipleTimesEmptyList(1);
            StringDriver.ClearMultipleTimesEmptyList(10);
            StringDriver.ClearMultipleTimesEmptyList(100);
            StringDriver.ClearNonEmptyList(stringArr);
            StringDriver.ClearMultipleTimesNonEmptyList(stringArr, 2);
            StringDriver.ClearMultipleTimesNonEmptyList(stringArr, 7);
            StringDriver.ClearMultipleTimesNonEmptyList(stringArr, 31);
            StringDriver.NonGenericIListClearEmptyList();
            StringDriver.NonGenericIListClearMultipleTimesEmptyList(1);
            StringDriver.NonGenericIListClearMultipleTimesEmptyList(10);
            StringDriver.NonGenericIListClearMultipleTimesEmptyList(100);
            StringDriver.NonGenericIListClearNonEmptyList(stringArr);
            StringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 2);
            StringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 7);
            StringDriver.NonGenericIListClearMultipleTimesNonEmptyList(stringArr, 31);
        }
    }
}
