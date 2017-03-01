// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static System.Linq.Enumerable;

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
            var hs = Range(0, 50).ToHashSet();
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Equal(EqualityComparer<int>.Default, hs.Comparer);
        }

        [Fact]
        public void ExplicitComparer()
        {
            var cmp = new CustomComparer<int>();
            var hs = Range(0, 50).ToHashSet(cmp);
            Assert.IsType<HashSet<int>>(hs);
            Assert.Equal(50, hs.Count);
            Assert.Same(cmp, hs.Comparer);
        }

        [Fact]
        public void RunOnce()
        {
            Range(0, 50).RunOnce().ToHashSet(new CustomComparer<int>());
        }

        [Fact]
        public void TolerateNullElements()
        {
            // Unlike the keys of a dictionary, HashSet tolerates null items.
            Assert.False(new HashSet<string>().Contains(null));
            var hs = new[] { "abc", null, "def" }.ToHashSet();
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
            var hs = Range(0, 50).Select(i => i / 5).ToHashSet();

            // The OrderBy isn't strictly necessary, but that depends upon an
            // implementation detail of HashSet, so explicitly force ordering.
            Assert.Equal(Range(0, 10), hs.OrderBy(i => i));
        }

        [Fact]
        public void ThrowOnNullSource()
        {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<object>)null).ToHashSet());
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void WorksAsExpected(IEnumerable<int> source, Func<IEnumerable<int>, IEnumerable<int>> action, IEqualityComparer<int> comparer)
        {
            var result = action(source);

            var expected = new HashSet<int>(result, comparer);

            var actual = result.ToHashSet(comparer);

            Assert.Equal(expected, actual); // Implies they have equal Counts too
            Assert.Equal(expected.Comparer, actual.Comparer);
        }

        [Theory]
        [MemberData(nameof(GetGroupingData))]
        public void WorksAsExpectedForGrouping(IEnumerable<int> source, Func<IEnumerable<int>, IEnumerable<IGrouping<int, int>>> action, IEqualityComparer<IGrouping<int, int>> comparer)
        {
            var result = action(source);

            var expected = new HashSet<IGrouping<int, int>>(result, comparer);

            var actual = result.ToHashSet(comparer);

            AssertHashSetGroupingEqual(expected, actual);
            Assert.Equal(expected.Comparer, actual.Comparer);
        }

        private static IEnumerable<object[]> GetData()
        {
            var testData = new WorkAsExpectedData<int>();

            testData.AddDefaultComparers();

            testData.AddActions(
                source => source.Concat(Range(40, 50)), // ConcatIterator
                source => source.DefaultIfEmpty(), // DefaultIfEmptyIterator
                source => source.Append(100), // AppendPrependIterator, AppendPrepend1Iterator, AppendPrependN
                source => source.Prepend(100), // AppendPrependIterator
                source => source.Distinct(), // DistinctIterator
                source => source.OrderBy(x => x), // OrderedEnumerable
                source => source.Take(1), // EmptyPartition, EnumerablePartition, ListPartition, OrderedPartition
                source => source.Reverse(), // ReverseIterator
                source => source.SelectMany(x => new[] { x, 0 }), // SelectManySingleSelectorIterator
                source => source.Union(Range(20, 50)), // UnionIterator
                source => source.Select(x => x), // SelectArrayIterator, SelectEnumerableIterator, SelectListIterator, SelectIListIterator
                source => source.Skip(1).Select(x => x), // SelectListPartitionIterator, SelectIPartitionIterator
                source => source.Where(x => x < 40), // WhereArrayIterator, WhereEnumerableIterator, WhereListIterator
                source => source.Where(x => x < 40).Select(x => x) // WhereSelectArrayIterator, WhereSelectEnumerableIterator, WhereSelectListIterator
            );

            testData.AddSources(GetSources());

            return testData.GetData();
        }

        private static IEnumerable<object[]> GetGroupingData()
        {
            var testData = new WorkAsExpectedData<IGrouping<int, int>>();

            testData.AddDefaultComparers();

            testData.AddActions(
                source => source.GroupBy(x => x), // GroupedEnumerable<TSource, TKey>, GroupedEnumerable<TSource, TKey, TElement>
                source => source.GroupBy(x => x, x => x), // GroupedResultEnumerable<TSource, TKey, TResult>, GroupedResultEnumerable<TSource, TKey, TElement, TResult>
                source => source.ToLookup(i => i) // Lookup
            );

            testData.AddSources(GetSources());

            return testData.GetData();
        }

        /// <summary>
        /// Assert.Equals -> HashSet.SetEquals not work for IGrouping
        /// </summary>
        /// <param name="expected">Expected set</param>
        /// <param name="actual">Actual set</param>
        private void AssertHashSetGroupingEqual(HashSet<IGrouping<int, int>> expected, HashSet<IGrouping<int, int>> actual)
        {
            // The Count should be equal
            Assert.Equal(expected.Count, actual.Count);

            // Get the group by key and check as IEnumerable
            foreach (var expectedGroup in expected)
            {
                var group = actual.FirstOrDefault(x => x.Key == expectedGroup.Key);

                Assert.NotNull(group);

                Assert.Equal(expectedGroup.AsEnumerable(), group.AsEnumerable());
            }
        }
        
        private static IEnumerable<IEnumerable<int>> GetSources()
        {
            // Create empty, distinct and repeated sources
            var sources = new[]
            {
                Array.Empty<int>(),
                Range(0, 50),
                Range(0, 50).Concat(Range(20, 50))
            };

            // Some methods have specific handling for Arrays, Lists and IEnumerables
            foreach (var source in sources)
            {
                yield return source.ToArray();
                yield return new List<int>(source);
                yield return new SortedSet<int>(source);
            }

            // Return RepeatIterator and RangeIterator
            yield return Repeat(0, 50);
            yield return Range(0, 50);
        }

        /// <summary>
        /// Responsible for combining Actions, Comparers and Sources
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        private class WorkAsExpectedData<T>
        {
            private readonly List<Func<IEnumerable<int>, IEnumerable<T>>> actions;
            private readonly List<IEqualityComparer<T>> comparers;
            private readonly List<IEnumerable<int>> sources;

            public WorkAsExpectedData()
            {
                actions = new List<Func<IEnumerable<int>, IEnumerable<T>>>();
                comparers = new List<IEqualityComparer<T>>();
                sources = new List<IEnumerable<int>>();
            }

            public void AddActions(params Func<IEnumerable<int>, IEnumerable<T>>[] actions)
            {
                this.actions.AddRange(actions);
            }

            public void AddDefaultComparers()
            {
                comparers.Add(null);
                comparers.Add(EqualityComparer<T>.Default);
                comparers.Add(new CustomComparer<T>());
            }

            public void AddSources(IEnumerable<IEnumerable<int>> sources)
            {
                this.sources.AddRange(sources);
            }

            public IEnumerable<object[]> GetData()
            {
                // Combine all and return one test of each combination
                // Currently: 17 actions * 11 Sources * 3 Comparers = 561 tests
                var tests = from action in actions
                            from comparer in comparers
                            from source in sources
                            select new { action, comparer, source };

                foreach (var test in tests)
                {
                    yield return new object[] { test.source, test.action, test.comparer };
                }
            }
        }
    }
}
