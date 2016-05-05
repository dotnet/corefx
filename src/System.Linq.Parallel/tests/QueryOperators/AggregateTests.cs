// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class AggregateTests
    {
        private const int ResultFuncModifier = 17;

        public static IEnumerable<object[]> AggregateExceptionData(int[] counts)
        {
            foreach (object[] results in UnorderedSources.Ranges(counts.Cast<int>()))
            {
                Labeled<ParallelQuery<int>> query = (Labeled<ParallelQuery<int>>)results[0];
                if (query.ToString().StartsWith("Partitioner"))
                {
                    yield return new object[] { Labeled.Label(query.ToString(), Partitioner.Create(UnorderedSources.GetRangeArray(0, (int)results[1]), false).AsParallel()), results[1] };
                }
                else if (query.ToString().StartsWith("Enumerable.Range"))
                {
                    yield return new object[] { Labeled.Label(query.ToString(), new StrictPartitioner<int>(Partitioner.Create(Enumerable.Range(0, (int)results[1]), EnumerablePartitionerOptions.None), (int)results[1]).AsParallel()), results[1] };
                }
                else
                {
                    yield return results;
                }
            }
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            if (count == 0)
            {
                Assert.Throws<InvalidOperationException>(() => query.Aggregate((x, y) => x + y));
            }
            else
            {
                // The operation will overflow for long-running sizes, but that's okay:
                // The helper is overflowing too!
                Assert.Equal(Functions.SumRange(0, count), query.Aggregate((x, y) => x + y));
            }
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Sum(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Seed(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Functions.SumRange(0, count), query.Aggregate(0, (x, y) => x + y));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Seed_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Sum_Seed(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Seed(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            ParallelQuery<int> query = labeled.Item;
            // The operation will overflow for long-running sizes, but that's okay:
            // The helper is overflowing too!
            Assert.Equal(Functions.ProductRange(start, count), query.Aggregate(1L, (x, y) => x * y));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Seed_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            Aggregate_Product_Seed(labeled, count, start);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Seed(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Enumerable.Range(0, count), query.Aggregate((IList<int>)new List<int>(), (l, x) => l.AddToCopy(x)).OrderBy(x => x));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 512, 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Seed_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Collection_Seed(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Result(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, query.Aggregate(0, (x, y) => x + y, result => result + ResultFuncModifier));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Result_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Sum_Result(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Result(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Functions.ProductRange(start, count) + ResultFuncModifier, query.Aggregate(1L, (x, y) => x * y, result => result + ResultFuncModifier));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Results_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            Aggregate_Product_Result(labeled, count, start);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Results(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Enumerable.Range(0, count), query.Aggregate((IList<int>)new List<int>(), (l, x) => l.AddToCopy(x), l => l.OrderBy(x => x)));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 512, 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Results_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Collection_Results(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Accumulator(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            int actual = query.Aggregate(
                0,
                (accumulator, x) => accumulator + x,
                (left, right) => left + right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_Accumulator_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Sum_Accumulator(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Accumulator(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            ParallelQuery<int> query = labeled.Item;
            long actual = query.Aggregate(
               1L,
                (accumulator, x) => accumulator * x,
                (left, right) => left * right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.ProductRange(start, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_Accumulator_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            Aggregate_Product_Accumulator(labeled, count, start);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Accumulator(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            IList<int> actual = query.Aggregate(
                (IList<int>)new List<int>(),
                (accumulator, x) => accumulator.AddToCopy(x),
                (left, right) => left.ConcatCopy(right),
                result => result.OrderBy(x => x).ToList());
            Assert.Equal(Enumerable.Range(0, count), actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 512, 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_Accumulator_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Collection_Accumulator(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_SeedFunction(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            int actual = query.Aggregate(
                () => 0,
                (accumulator, x) => accumulator + x,
                (left, right) => left + right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Sum_SeedFunction_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Sum_SeedFunction(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_SeedFunction(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            ParallelQuery<int> query = labeled.Item;
            long actual = query.Aggregate(
                () => 1L,
                (accumulator, x) => accumulator * x,
                (left, right) => left * right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.ProductRange(start, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), 1, new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Product_SeedFunction_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int start)
        {
            Aggregate_Product_SeedFunction(labeled, count, start);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_SeedFunction(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            IList<int> actual = query.Aggregate(
                () => (IList<int>)new List<int>(),
                (accumulator, x) => accumulator.AddToCopy(x),
                (left, right) => left.ConcatCopy(right),
                result => result.OrderBy(x => x).ToList());
            Assert.Equal(Enumerable.Range(0, count), actual);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 512, 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void Aggregate_Collection_SeedFunction_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Aggregate_Collection_SeedFunction(labeled, count);
        }

        [Fact]
        public static void Aggregate_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Empty<int>().Aggregate((i, j) => i));
            // All other invocations return the seed value.
            Assert.Equal(-1, ParallelEnumerable.Empty<int>().Aggregate(-1, (i, j) => i + j));
            Assert.Equal(-1, ParallelEnumerable.Empty<int>().Aggregate(-1, (i, j) => i + j, i => i));
            Assert.Equal(-1, ParallelEnumerable.Empty<int>().Aggregate(-1, (i, j) => i + j, (i, j) => i + j, i => i));
            Assert.Equal(-1, ParallelEnumerable.Empty<int>().Aggregate(() => -1, (i, j) => i + j, (i, j) => i + j, i => i));
        }

        [Fact]
        public static void Aggregate_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.Aggregate((i, j) => { canceler(); return j; }));
            AssertThrows.EventuallyCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }));
            AssertThrows.EventuallyCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, i => i));
            AssertThrows.EventuallyCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, (i, j) => i, i => i));
            AssertThrows.EventuallyCanceled((source, canceler) => source.Aggregate(() => 0, (i, j) => { canceler(); ; return j; }, (i, j) => i, i => i));
        }

        [Fact]
        public static void Aggregate_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Aggregate((i, j) => { canceler(); return j; }));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, i => i));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, (i, j) => i, i => i));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Aggregate(() => 0, (i, j) => { canceler(); ; return j; }, (i, j) => i, i => i));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Aggregate((i, j) => { canceler(); return j; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, i => i));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Aggregate(0, (i, j) => { canceler(); return j; }, (i, j) => i, i => i));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Aggregate(() => 0, (i, j) => { canceler(); ; return j; }, (i, j) => i, i => i));
        }

        [Fact]
        public static void Aggregate_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.Aggregate((i, j) => i));
            AssertThrows.AlreadyCanceled(source => source.Aggregate(0, (i, j) => i));
            AssertThrows.AlreadyCanceled(source => source.Aggregate(0, (i, j) => i, i => i));
            AssertThrows.AlreadyCanceled(source => source.Aggregate(0, (i, j) => i, (i, j) => i, i => i));
            AssertThrows.AlreadyCanceled(source => source.Aggregate(() => 0, (i, j) => i, (i, j) => i, i => i));
        }

        [Theory]
        [MemberData(nameof(AggregateExceptionData), new[] { 2 })]
        public static void Aggregate_AggregateException(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate((i, j) => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(0, (i, j) => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(0, (i, j) => { throw new DeliberateTestException(); }, i => i));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate<int, int, int>(0, (i, j) => i, i => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(0, (i, j) => { throw new DeliberateTestException(); }, (i, j) => i, i => i));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate<int, int, int>(0, (i, j) => i, (i, j) => i, i => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate<int, int, int>(() => { throw new DeliberateTestException(); }, (i, j) => i, (i, j) => i, i => i));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(() => 0, (i, j) => { throw new DeliberateTestException(); }, (i, j) => i, i => i));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate<int, int, int>(() => 0, (i, j) => i, (i, j) => i, i => { throw new DeliberateTestException(); }));
            if (Environment.ProcessorCount >= 2)
            {
                Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(0, (i, j) => i, (i, j) => { throw new DeliberateTestException(); }, i => i));
                Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Aggregate(() => 0, (i, j) => i, (i, j) => { throw new DeliberateTestException(); }, i => i));
            }
        }

        [Fact]
        public static void Aggregate_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Aggregate((i, j) => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(null));

            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(0, null));

            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(0, null, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(0, (i, j) => i, null));

            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(0, null, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(0, (i, j) => i, null, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(0, (i, j) => i, (i, j) => i, null));

            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Aggregate(() => 0, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(null, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(() => 0, null, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate(() => 0, (i, j) => i, null, i => i));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(() => 0, (i, j) => i, (i, j) => i, null));
        }
    }

    internal static class ListHelper
    {
        // System.Collections.Immutable.ImmutableList wasn't available.
        public static IList<int> AddToCopy(this IList<int> collection, int element)
        {
            collection = new List<int>(collection);
            collection.Add(element);
            return collection;
        }

        public static IList<int> ConcatCopy(this IList<int> left, IList<int> right)
        {
            List<int> results = new List<int>(left);
            results.AddRange(right);
            return results;
        }
    }
}
