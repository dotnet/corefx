// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ToHashSetTests
    {
        private class CustomComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

            public int GetHashCode(T obj) => EqualityComparer<T>.Default.GetHashCode(obj);
        }

        [Fact]
        public void NoExplicitComparer()
        {
            var hs = Enumerable.Range(0, 50).ToHashSet();
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Equal(EqualityComparer<int>.Default, hs.Comparer);
        }

        [Fact]
        public void ExplicitComparer()
        {
            var cmp = new CustomComparer<int>();
            var hs = Enumerable.Range(0, 50).ToHashSet(cmp);
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Same(cmp, hs.Comparer);
        }

        [Fact]
        public void RunOnce()
        {
            Enumerable.Range(0, 50).RunOnce().ToHashSet(new CustomComparer<int>());
        }

        [Fact]
        public void TolerateNullElements()
        {
            // Unlike the keys of a dictionary, HashSet tolerates null items.
            Assert.False(new HashSet<string>().Contains(null));
            var hs = new [] {"abc", null, "def"}.ToHashSet();
            Assert.True(hs.Contains(null));
        }

        [Fact]
        public void TolerateDuplicates()
        {
            // ToDictionary throws on duplicates, because that is the normal behaviour
            // of Dictionary<TKey, TValue>.Add().
            // By the same token, since the normal behaviour of HashSet<T>.Add()
            // is to signal duplicates without an exception ToHashSet should
            // tolerate duplicates.
            var hs = Enumerable.Range(0, 50).Select(i => i / 5).ToHashSet();

            // The OrderBy isn't strictly necessary, but that depends upon an
            // implementation detail of HashSet, so explicitly force ordering.
            Assert.Equal(Enumerable.Range(0, 10), hs.OrderBy(i => i));
        }

        [Fact]
        public void ThrowOnNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<object>)null).ToHashSet());
        }

        [Fact]
        public void WithConcat()
        {
            var first = Enumerable.Range(0, 50);
            var second = Enumerable.Range(20, 50);

            HashSet<int> result = first.Concat(second).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(70, result.Count);
        }

        [Fact]
        public void WithDefaultIfEmpty()
        {
            IEnumerable<int> source = Enumerable.Range(0, 0);

            HashSet<int> result = source.DefaultIfEmpty().ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.True(result.Contains(0));
        }

        [Fact]
        public void WithAppend()
        {
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.Append(100).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(51, result.Count);
            Assert.True(result.Contains(100));
        }

        [Fact]
        public void WithPrepend()
        {
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.Prepend(100).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(51, result.Count);
            Assert.True(result.Contains(100));
        }

        [Fact]
        public void WithDistinct()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<int> result = source.Distinct().ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithGroupBy()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<IGrouping<int, int>> result = source.GroupBy(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithGroupByAndSelector()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<IGrouping<int, string>> result = source.GroupBy(x => x, x => x.ToString()).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithGroupByAndElementSelector()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<IGrouping<int, string>> result = source.GroupBy(x => x, x => x.ToString()).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithGroupByAndElementAndResultSelector()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<IEnumerable<int>> result = source.GroupBy(x => x, x => x, (x, group) => group).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithToLookup()
        {
            var source = new[] { 1, 2, 3, 1, 2, 3 };

            HashSet<IGrouping<int, int>> result = source.ToLookup(i => i).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithTake_EmptyPartition()
        {
            var source = new int[0];

            // Empty Partition
            HashSet<int> result = source.Take(1).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void WithTake_ListPartition()
        {
            var source = new[] { 1, 2, 3 }.ToList();

            // List Partition
            HashSet<int> result = source.Take(1).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void WithTake_EnumerablePartition()
        {
            var source = new[] { 1, 2, 3 };

            // Enumerable Partition
            HashSet<int> result = source.Take(1).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void WithSkipThenTake_Partition()
        {
            var source = new[] { 1, 2, 3 };

            // Ordered Partition
            HashSet<int> result = source.Skip(1).Take(1).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void WithRange()
        {
            HashSet<int> result = Enumerable.Range(0, 50).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithRange_CustomComparer()
        {
            CustomComparer<int> customComparer = new CustomComparer<int>();

            HashSet<int> result = Enumerable.Range(0, 50).ToHashSet(customComparer);

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithRepeat()
        {
            HashSet<int> result = Enumerable.Repeat(1, 50).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void WithReverse()
        {
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.Reverse().ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithSelectMany()
        {
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.SelectMany(x => new[] { x, 0 }).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithUnion()
        {
            var first = Enumerable.Range(0, 50);
            var second = Enumerable.Range(20, 50);

            HashSet<int> result = first.Union(second).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(70, result.Count);
        }

        [Fact]
        public void WithSelect_Array()
        {
            int[] source = { 1, 2, 3 };

            HashSet<int> result = source.Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithSelect_List()
        {
            List<int> source = new List<int> { 1, 2, 3 };

            HashSet<int> result = source.Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithSelect_Iterator()
        {
            // RangeIterator
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithSelect_ListPartition()
        {
            List<int> source = new List<int> { 1, 2, 3 };

            HashSet<int> result = source.Skip(1).Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithSelect_Partition()
        {
            var source = Enumerable.Range(0, 50);

            HashSet<int> result = source.Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(50, result.Count);
        }

        [Fact]
        public void WithSelect_IEnumerable()
        {
            IEnumerable<int> source = new HashSet<int> { 1, 2, 3 };

            HashSet<int> result = source.Select(x => x).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void WithWhere_Array()
        {
            int[] source = { 1, 2, 3 };

            HashSet<int> result = source.Where(x => x < 3).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithWhere_Enumerable()
        {
            HashSet<int> source = new HashSet<int> { 1, 2, 3, 2 };

            HashSet<int> result = source.Where(x => x < 3).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithWhere_List()
        {
            List<int> source = new List<int> { 1, 2, 3 };

            HashSet<int> result = source.Where(x => x < 3).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithWhereThenSelect_Array()
        {
            int[] source = { 1, 2, 3 };

            HashSet<string> result = source.Where(x => x < 3).Select(x => x.ToString()).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithWhereThenSelect_Enumerable()
        {
            HashSet<int> source = new HashSet<int> { 1, 2, 3, 2 };

            HashSet<string> result = source.Where(x => x < 3).Select(x => x.ToString()).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void WithWhereThenSelect_List()
        {
            List<int> source = new List<int> { 1, 2, 3 };

            HashSet<string> result = source.Where(x => x < 3).Select(x => x.ToString()).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}
