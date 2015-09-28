﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Tests
{
    public class ToListTests : EnumerableTests
    {
        [Fact]
        public void ToList_AlwaysCreateACopy()
        {
            List<int> sourceList = new List<int>() { 1, 2, 3, 4, 5 };
            List<int> resultList = sourceList.ToList();

            Assert.NotSame(sourceList, resultList);
            Assert.Equal(sourceList, resultList);
        }


        private void RunToListOnAllCollectionTypes<T>(T[] items, Action<List<T>> validation)
        {
            validation(Enumerable.ToList(items));
            validation(Enumerable.ToList(new List<T>(items)));
            validation(new TestEnumerable<T>(items).ToList());
            validation(new TestReadOnlyCollection<T>(items).ToList());
            validation(new TestCollection<T>(items).ToList());
        }


        [Fact]
        public void ToList_WorkWithEmptyCollection()
        {
            RunToListOnAllCollectionTypes(new int[0],
                resultList =>
                {
                    Assert.NotNull(resultList);
                    Assert.Equal(0, resultList.Count);
                });
        }

        [Fact]
        public void ToList_ProduceCorrectList()
        {
            int[] sourceArray = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            RunToListOnAllCollectionTypes(sourceArray,
                resultList =>
                {
                    Assert.Equal(sourceArray.Length, resultList.Count);
                    Assert.Equal(sourceArray, resultList);
                });

            string[] sourceStringArray = new string[] { "1", "2", "3", "4", "5", "6", "7", "8" };
            RunToListOnAllCollectionTypes(sourceStringArray,
                resultStringList =>
                {
                    Assert.Equal(sourceStringArray.Length, resultStringList.Count);
                    for (int i = 0; i < sourceStringArray.Length; i++)
                        Assert.Same(sourceStringArray[i], resultStringList[i]);
                });
        }


        [Fact]
        public void ToList_TouchCountWithICollection()
        {
            TestCollection<int> source = new TestCollection<int>(new int[] { 1, 2, 3, 4 });
            var resultList = source.ToList();

            Assert.Equal(source, resultList);
            Assert.Equal(1, source.CountTouched);
        }


        [Fact]
        public void ToList_ThrowArgumentNullExceptionWhenSourceIsNull()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.ToList());
        }


        // Later this behaviour can be changed
        [Fact]
        [ActiveIssue(1561)]
        public void ToList_UseCopyToWithICollection()
        {
            TestCollection<int> source = new TestCollection<int>(new int[] { 1, 2, 3, 4 });
            var resultList = source.ToList();

            Assert.Equal(source, resultList);
            Assert.Equal(1, source.CopyToTouched);
        }


        [Fact]
        [ActiveIssue(1561)]
        public void ToList_WorkWhenCountChangedAsynchronously()
        {
            GrowingAfterCountReadCollection source = new GrowingAfterCountReadCollection(new int[] { 1, 2, 3, 4 });
            var resultList = source.ToList();

            Assert.True(resultList.Count >= 4);
            Assert.Equal(1, resultList[0]);
            Assert.Equal(2, resultList[0]);
            Assert.Equal(3, resultList[0]);
            Assert.Equal(4, resultList[0]);
        }

        [Theory]
        [InlineData(new int[] { }, new string[] { })]
        [InlineData(new int[] { 1 }, new string[] { "1" })]
        [InlineData(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ToList_ArrayWhereSelect(int[] sourceIntegers, string[] convertedStrings)
        {
            var sourceList = new List<int>(sourceIntegers);
            var convertedList = new List<string>(convertedStrings);

            var emptyIntegersList = new List<int>();
            var emptyStringsList = new List<string>();

            Assert.Equal(convertedList, sourceIntegers.Select(i => i.ToString()).ToList());

            Assert.Equal(sourceList, sourceIntegers.Where(i => true).ToList());
            Assert.Equal(emptyIntegersList, sourceIntegers.Where(i => false).ToList());

            Assert.Equal(convertedList, sourceIntegers.Where(i => true).Select(i => i.ToString()).ToList());
            Assert.Equal(emptyStringsList, sourceIntegers.Where(i => false).Select(i => i.ToString()).ToList());

            Assert.Equal(convertedList, sourceIntegers.Select(i => i.ToString()).Where(s => s != null).ToList());
            Assert.Equal(emptyStringsList, sourceIntegers.Select(i => i.ToString()).Where(s => s == null).ToList());
        }

        [Theory]
        [InlineData(new int[] { }, new string[] { })]
        [InlineData(new int[] { 1 }, new string[] { "1" })]
        [InlineData(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ToList_ListWhereSelect(int[] sourceIntegers, string[] convertedStrings)
        {
            var sourceList = new List<int>(sourceIntegers);
            var convertedList = new List<string>(convertedStrings);

            var emptyIntegersList = new List<int>();
            var emptyStringsList = new List<string>();

            Assert.Equal(convertedList, sourceList.Select(i => i.ToString()).ToList());

            Assert.Equal(sourceList, sourceList.Where(i => true).ToList());
            Assert.Equal(emptyIntegersList, sourceList.Where(i => false).ToList());

            Assert.Equal(convertedList, sourceList.Where(i => true).Select(i => i.ToString()).ToList());
            Assert.Equal(emptyStringsList, sourceList.Where(i => false).Select(i => i.ToString()).ToList());

            Assert.Equal(convertedList, sourceList.Select(i => i.ToString()).Where(s => s != null).ToList());
            Assert.Equal(emptyStringsList, sourceList.Select(i => i.ToString()).Where(s => s == null).ToList());
        }

        [Theory]
        [InlineData(new int[] { }, new string[] { })]
        [InlineData(new int[] { 1 }, new string[] { "1" })]
        [InlineData(new int[] { 1, 2, 3 }, new string[] { "1", "2", "3" })]
        public void ToList_IListWhereSelect(int[] sourceIntegers, string[] convertedStrings)
        {
            var sourceList = new ReadOnlyCollection<int>(sourceIntegers);
            var convertedList = new ReadOnlyCollection<string>(convertedStrings);

            var emptyIntegersList = new ReadOnlyCollection<int>(Array.Empty<int>());
            var emptyStringsList = new ReadOnlyCollection<string>(Array.Empty<string>());

            Assert.Equal(convertedList, sourceList.Select(i => i.ToString()).ToList());

            Assert.Equal(sourceList, sourceList.Where(i => true).ToList());
            Assert.Equal(emptyIntegersList, sourceList.Where(i => false).ToList());

            Assert.Equal(convertedList, sourceList.Where(i => true).Select(i => i.ToString()).ToList());
            Assert.Equal(emptyStringsList, sourceList.Where(i => false).Select(i => i.ToString()).ToList());

            Assert.Equal(convertedList, sourceList.Select(i => i.ToString()).Where(s => s != null).ToList());
            Assert.Equal(emptyStringsList, sourceList.Select(i => i.ToString()).Where(s => s == null).ToList());
        }

        [Fact]
        public void SameResultsRepeatCallsFromWhereOnIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > Int32.MinValue
                    select x;

            Assert.Equal(q.ToList(), q.ToList());
        }
        
        [Fact]
        public void SameResultsRepeatCallsFromWhereOnStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                        where !String.IsNullOrEmpty(x)
                        select x;

            Assert.Equal(q.ToList(), q.ToList());
        }

        [Fact]
        public void SourceIsEmptyICollectionT()
        {
            int[] source = { };

            ICollection<int> collection = source as ICollection<int>;

            Assert.Empty(source.ToList());
            Assert.Empty(collection.ToList());
        }

        [Fact]
        public void SourceIsICollectionTWithFewElements()
        {
            int?[] source = { -5, null, 0, 10, 3, -1, null, 4, 9 };
            int?[] expected = { -5, null, 0, 10, 3, -1, null, 4, 9 };

            ICollection<int?> collection = source as ICollection<int?>;

            Assert.Equal(expected, source.ToList());
            Assert.Equal(expected, collection.ToList());
        }

        [Fact]
        public void SourceNotICollectionAndIsEmpty()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-4, 0);
            Assert.Empty(source.ToList());
        }

        [Fact]
        public void SourceNotICollectionAndHasElements()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-4, 10);
            int[] expected = { -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };

            Assert.Null(source as ICollection<int>);

            Assert.Equal(expected, source.ToList());
        }

        [Fact]
        public void SourceNotICollectionAndAllNull()
        {
            IEnumerable<int?> source = RepeatedNullableNumberGuaranteedNotCollectionType(null, 5);
            int?[] expected = { null, null, null, null, null };

            Assert.Null(source as ICollection<int>);
    
            Assert.Equal(expected, source.ToList());
        }
    }
}
