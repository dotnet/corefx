// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;

namespace System.Collections.Test
{
    public class IListBinarySearch
    {
        private static string[] _dota =
        {
            "Abaddon","Alchemist","Ancient Apparition","Anti-Mage","Arc Warden","Axe","Bane","Batrider","Beastmaster","Bloodseeker","Bounty Hunter","Brewmaster","Bristleback","Broodmother","Centaur Warrunner","Chaos Knight","Chen","Clinkz"
        };

        [Fact]
        public static void BinarySearchTest()
        {
            IList list = new List<string>(_dota);

            //search for each item
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(i, list.BinarySearch(list[i], new StringComparer())); //"Binary Search should have returned the same index."

            //ensure no side effects
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(list[i], list[i]); //"Should not have changed the items in the array/list."

            //search for each item starting from last item
            for (int i = list.Count - 1; i >= 0; i--)
                Assert.Equal(i, list.BinarySearch(list[i], new StringComparer())); //"Binary Search should have returned the same index starting from the tend."

            //ensure no side effects
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(list[i], list[i]); //"Should not have changed the items in the array/list."
        }

        [Fact]
        public static void BinarySearchSubsetTest()
        {
            IList list = new List<string>(_dota);
            int index = 5;
            int count = list.Count - 5;
            //search for each item
            for (int i = index; i < index + count; i++)
            {
                Assert.Equal(i, list.BinarySearch(index, count, list[i], new StringComparer())); //"Binary search should have returned the same index starting search from the beginning"
            }

            //ensure no side effects
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(list[i], list[i]); //"Should not have changed the items in the array/list."

            //search for each item starting from last item
            for (int i = index + count - 1; i >= index; i--)
            {
                Assert.Equal(i, list.BinarySearch(index, count, list[i], new StringComparer())); //"Binary search should have returned the same index starting search from the end"
            }

            //ensure no side effects
            for (int i = 0; i < list.Count; i++)
                Assert.Equal(list[i], list[i]); //"Should not have changed the items in the array/list."
        }

        [Fact]
        public static void BinarySearchNegativeTest()
        {
            IList listX = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                listX.Add(i);
            }
            IList listY = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                listY.Add(100 + i);
            }
            //search for each item
            for (int i = 0; i < listY.Count; i++)
                Assert.True(-1 >= listX.BinarySearch(listY[i], new IntComparer())); //"Should not have found item with BinarySearch."

            //ensure no side effects
            for (int i = 0; i < listX.Count; i++)
                Assert.Equal(listX[i], listX[i]); //"Should not have changed the items in the array/list."
        }

        [Fact]
        public static void BinarySearchNullIListThowsNullReferenceExceptionTest()
        {
            IList list = null;
            Assert.Throws<NullReferenceException>(() => list.BinarySearch(1, new IntComparer())); //"Should have thrown NullReferenceException with null IList"
        }

        [Fact]
        public static void BinarySearchValidationsTest()
        {
            IList list = new List<string>(_dota);
            Assert.Throws<ArgumentNullException>(()=> list.BinarySearch(1,null));//"ArgumentNullException should be thrown when IComparer is null."
            Assert.Throws<ArgumentException>(() => list.BinarySearch(0, list.Count + 1, list[0], new StringComparer())); //"Finding items longer than array should throw ArgumentException"
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(-1, list.Count, list[0], new StringComparer())); //"ArgumentOutOfRangeException should be thrown on negative index."
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(0, -1, list[0], new StringComparer())); //"ArgumentOutOfRangeException should be thrown on negative count."
            Assert.Throws<ArgumentException>(() => list.BinarySearch(list.Count + 1, list.Count, list[0], new StringComparer())); //"ArgumentException should be thrown on index greater than length of array."
        }

        private class StringComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (null == x)
                    if (null == y)
                        return 0;
                    else
                        return -1;
                if (null == y)
                    return 1;
                if (null == x as string)
                    if (null == y as string)
                        return 0;
                    else
                        return -1;
                return (x as string).CompareTo((string)y);
            }
        }

        private class IntComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                int xInt;
                int yInt;
                if (null == x)
                    if (null == y)
                        return 0;
                    else
                        return -1;
                if (null == y)
                    return 1;

                xInt = Convert.ToInt32(x);
                yInt = Convert.ToInt32(y);

                return xInt.CompareTo(yInt);
            }
        }
    }
}

