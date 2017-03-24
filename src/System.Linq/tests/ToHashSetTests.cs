// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
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

        public class Record
        {
            public string Name { get; set; }
            public int Score { get; set; }

            private sealed class RecordComparer : IEqualityComparer<Record>
            {
                public bool Equals(Record x, Record y) => string.Equals(x.Name, y.Name) && x.Score == y.Score;

                public int GetHashCode(Record obj)
                {
                    unchecked
                    {
                        return (obj.Name.GetHashCode() * 397) ^ obj.Score;
                    }
                }
            }

            public static IEqualityComparer<Record> Comparer { get; } = new RecordComparer();
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

        [Fact]
        public void DistinctShouldConsiderIteratorComparerRegression()
        {
            Record[] source = { new Record { Name = "A", Score = 1 }, new Record { Name = "A", Score = 1 } };

            HashSet<Record> result = source.Distinct(Record.Comparer).ToHashSet();

            Assert.NotNull(result);
            Assert.Equal(EqualityComparer<Record>.Default, result.Comparer);
            Assert.Equal(1, result.Count);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void WorksAsExpected(IEnumerable<int> source, Func<IEnumerable<int>, IEnumerable<int>> action, IEqualityComparer<int> comparer)
        {
            var iterator = action(source);

            var expected = new HashSet<int>(iterator, comparer);

            var actual = iterator.ToHashSet(comparer);

            Assert.Equal(expected, actual); // Implies they have equal Counts too
            Assert.Equal(expected.Comparer, actual.Comparer);
        }

        [Theory]
        [MemberData(nameof(GetGroupingData))]
        public void WorksAsExpectedForGrouping(IEnumerable<int> source, Func<IEnumerable<int>, IEnumerable<IGrouping<int, int>>> action, IEqualityComparer<IGrouping<int, int>> comparer)
        {
            var iterator = action(source);

            var expected = new HashSet<IGrouping<int, int>>(iterator, comparer);

            var actual = iterator.ToHashSet(comparer);

            AssertHashSetGroupingEqual(expected, actual);
        }

        private static IEnumerable<object[]> GetData()
        {
            var builder = new TestDataBuilder<int>();

            builder.AddDefaultComparers();

            builder.AddActions(
                source => source.Concat(Range(40, 50)),
                source => source.DefaultIfEmpty(),
                source => source.Append(100),
                source => source.Prepend(100).Append(200),
                source => source.Distinct(),
                source => source.Skip(1),
                source => source.Take(1),
                source => source.OrderBy(x => x),
                source => source.OrderBy(x => x).Take(1),
                source => source.OrderBy(x => x).Take(1000),
                source => source.Reverse(),
                source => source.SelectMany(x => new[] { x, 0 }),
                source => source.Union(Range(20, 50)),
                source => source.Select(x => x),
                source => source.Skip(1).Select(x => x),
                source => source.Where(x => x < 40),
                source => source.Where(x => x < 40).Select(x => x),

                source => source.GroupBy(i => i, (i, ints) => i),
                source => source.GroupBy(i => i, i => i, (i, ints) => i)
            );

            builder.AddSources(GetSources());

            return builder.GetData();
        }

        private static IEnumerable<object[]> GetGroupingData()
        {
            var builder = new TestDataBuilder<IGrouping<int, int>>();

            builder.AddDefaultComparers();

            builder.AddActions(
                source => source.GroupBy(x => x),
                source => source.GroupBy(x => x, x => x),
                source => source.ToLookup(i => i)
            );

            builder.AddSources(GetSources());

            return builder.GetData();
        }

        /// <summary>
        /// Assert.Equals -> HashSet.SetEquals not work for IGrouping
        /// </summary>
        /// <param name="expected">Expected set</param>
        /// <param name="actual">Actual set</param>
        private void AssertHashSetGroupingEqual(HashSet<IGrouping<int, int>> expected, HashSet<IGrouping<int, int>> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected.Comparer, actual.Comparer);

            // Get the group by key and check as IEnumerable
            foreach (var expectedGroup in expected)
            {
                var group = actual.Single(x => x.Key == expectedGroup.Key);

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
                yield return source;

                // Implements IList<T>, but isn't a type we're likely to explicitly optimize for.
                yield return new LinkedList<int>(source);

                yield return source.ToArray();
                yield return source.ToList();
                yield return source.ToImmutableArray();

                // Is not an ICollection<T>, but does implement ICollection and IReadOnlyCollection<T>.
                yield return new Queue<int>(source);
            }

            yield return Repeat(0, 50);
            yield return Range(0, 50);
        }

        /// <summary>
        /// Responsible for combining Actions, Comparers and Sources
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        private class TestDataBuilder<T>
        {
            private readonly List<Func<IEnumerable<int>, IEnumerable<T>>> _actions;
            private readonly List<IEqualityComparer<T>> _comparers;
            private readonly List<IEnumerable<int>> _sources;

            public TestDataBuilder()
            {
                _actions = new List<Func<IEnumerable<int>, IEnumerable<T>>>();
                _comparers = new List<IEqualityComparer<T>>();
                _sources = new List<IEnumerable<int>>();
            }

            public void AddActions(params Func<IEnumerable<int>, IEnumerable<T>>[] actions)
            {
                _actions.AddRange(actions);
            }

            public void AddDefaultComparers()
            {
                _comparers.Add(null);
                _comparers.Add(EqualityComparer<T>.Default);
                _comparers.Add(new CustomComparer<T>());
            }

            public void AddSources(IEnumerable<IEnumerable<int>> sources)
            {
                _sources.AddRange(sources);
            }

            public IEnumerable<object[]> GetData()
            {
                var tests = from action in _actions
                            from comparer in _comparers
                            from source in _sources
                            select new { action, comparer, source };

                foreach (var test in tests)
                {
                    yield return new object[] { test.source, test.action, test.comparer };
                }
            }
        }
    }
}