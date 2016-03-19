// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class AggregateTests
    {
        private const int ResultFuncModifier = 17;

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Sum(int count)
        {
            // The operation will overflow for long-running sizes, but that's okay:
            // The helper is overflowing too!
            Assert.Equal(Functions.SumRange(0, count), ParallelEnumerable.Range(0, count).Aggregate((x, y) => x + y));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Sum_Longrunning(int count)
        {
            Aggregate_Sum(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Sum_Seed(int count)
        {
            Assert.Equal(Functions.SumRange(0, count), ParallelEnumerable.Range(0, count).Aggregate(0, (x, y) => x + y));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Sum_Seed_Longrunning(int count)
        {
            Aggregate_Sum_Seed(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Product_Seed(int count)
        {
            // The operation will overflow for long-running sizes, but that's okay:
            // The helper is overflowing too!
            Assert.Equal(Functions.ProductRange(1, count), ParallelEnumerable.Range(1, count).Aggregate(1L, (x, y) => x * y));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Product_Seed_Longrunning(int count)
        {
            Aggregate_Product_Seed(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Collection_Seed(int count)
        {
            Assert.Equal(Enumerable.Range(0, count), ParallelEnumerable.Range(0, count).Aggregate(ImmutableList<int>.Empty, (l, x) => l.Add(x)).OrderBy(x => x));
        }

        [Theory]
        [OuterLoop]
        [InlineData(512)]
        [InlineData(1024 * 16)]
        public static void Aggregate_Collection_Seed_Longrunning(int count)
        {
            Aggregate_Collection_Seed(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Sum_Result(int count)
        {
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, ParallelEnumerable.Range(0, count).Aggregate(0, (x, y) => x + y, result => result + ResultFuncModifier));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Sum_Result_Longrunning(int count)
        {
            Aggregate_Sum_Result(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Product_Result(int count)
        {
            Assert.Equal(Functions.ProductRange(1, count) + ResultFuncModifier, ParallelEnumerable.Range(1, count).Aggregate(1L, (x, y) => x * y, result => result + ResultFuncModifier));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Product_Results_Longrunning(int count)
        {
            Aggregate_Product_Result(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Collection_Results(int count)
        {
            Assert.Equal(Enumerable.Range(0, count), ParallelEnumerable.Range(0, count).Aggregate(ImmutableList<int>.Empty, (l, x) => l.Add(x), l => l.OrderBy(x => x)));
        }

        [Theory]
        [OuterLoop]
        [InlineData(512)]
        [InlineData(1024 * 16)]
        public static void Aggregate_Collection_Results_Longrunning(int count)
        {
            Aggregate_Collection_Results(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Sum_Accumulator(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(0, count);
            int actual = query.Aggregate(
                0,
                (accumulator, x) => accumulator + x,
                (left, right) => left + right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Sum_Accumulator_Longrunning(int count)
        {
            Aggregate_Sum_Accumulator(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Product_Accumulator(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(1, count);
            long actual = query.Aggregate(
                1L,
                (accumulator, x) => accumulator * x,
                (left, right) => left * right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.ProductRange(1, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Product_Accumulator_Longrunning(int count)
        {
            Aggregate_Product_Accumulator(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Collection_Accumulator(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(0, count);
            IList<int> actual = query.Aggregate(
                ImmutableList<int>.Empty,
                (accumulator, x) => accumulator.Add(x),
                (left, right) => left.AddRange(right),
                result => result.OrderBy(x => x).ToList());
            Assert.Equal(Enumerable.Range(0, count), actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(512)]
        [InlineData(1024 * 16)]
        public static void Aggregate_Collection_Accumulator_Longrunning(int count)
        {
            Aggregate_Collection_Accumulator(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Sum_SeedFunction(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(0, count);
            int actual = query.Aggregate(
                () => 0,
                (accumulator, x) => accumulator + x,
                (left, right) => left + right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.SumRange(0, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Sum_SeedFunction_Longrunning(int count)
        {
            Aggregate_Sum_SeedFunction(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Product_SeedFunction(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(1, count);
            long actual = query.Aggregate(
                () => 1L,
                (accumulator, x) => accumulator * x,
                (left, right) => left * right,
                result => result + ResultFuncModifier);
            Assert.Equal(Functions.ProductRange(1, count) + ResultFuncModifier, actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Aggregate_Product_SeedFunction_Longrunning(int count)
        {
            Aggregate_Product_SeedFunction(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Aggregate_Collection_SeedFunction(int count)
        {
            ParallelQuery<int> query = ParallelEnumerable.Range(0, count);
            IList<int> actual = query.Aggregate(
                () => ImmutableList<int>.Empty,
                (accumulator, x) => accumulator.Add(x),
                (left, right) => left.AddRange(right),
                result => result.OrderBy(x => x).ToList());
            Assert.Equal(Enumerable.Range(0, count), actual);
        }

        [Theory]
        [OuterLoop]
        [InlineData(512)]
        [InlineData(1024 * 16)]
        public static void Aggregate_Collection_SeedFunction_Longrunning(int count)
        {
            Aggregate_Collection_SeedFunction(count);
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

        [Fact]
        public static void Aggregate_AggregateException()
        {
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate((i, j) => { throw new DeliberateTestException(); }));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(0, (i, j) => { throw new DeliberateTestException(); }));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(0, (i, j) => { throw new DeliberateTestException(); }, i => i));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate<int, int, int>(0, (i, j) => i, i => { throw new DeliberateTestException(); }));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(0, (i, j) => { throw new DeliberateTestException(); }, (i, j) => i, i => i));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate<int, int, int>(0, (i, j) => i, (i, j) => i, i => { throw new DeliberateTestException(); }));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate<int, int, int>(() => { throw new DeliberateTestException(); }, (i, j) => i, (i, j) => i, i => i));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(() => 0, (i, j) => { throw new DeliberateTestException(); }, (i, j) => i, i => i));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate<int, int, int>(() => 0, (i, j) => i, (i, j) => i, i => { throw new DeliberateTestException(); }));
            if (Environment.ProcessorCount >= 2)
            {
                AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(0, (i, j) => i, (i, j) => { throw new DeliberateTestException(); }, i => i));
                AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 2).Aggregate(() => 0, (i, j) => i, (i, j) => { throw new DeliberateTestException(); }, i => i));
            }
        }

        [Fact]
        public static void Aggregate_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<int>)null).Aggregate((i, j) => i));
            Assert.Throws<ArgumentNullException>("func", () => ParallelEnumerable.Range(0, 1).Aggregate(null));

            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i));
            Assert.Throws<ArgumentNullException>("func", () => ParallelEnumerable.Range(0, 1).Aggregate(0, null));

            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("func", () => ParallelEnumerable.Range(0, 1).Aggregate(0, null, i => i));
            Assert.Throws<ArgumentNullException>("resultSelector", () => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(0, (i, j) => i, null));

            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<int>)null).Aggregate(0, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("updateAccumulatorFunc", () => ParallelEnumerable.Range(0, 1).Aggregate(0, null, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("combineAccumulatorsFunc", () => ParallelEnumerable.Range(0, 1).Aggregate(0, (i, j) => i, null, i => i));
            Assert.Throws<ArgumentNullException>("resultSelector", () => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(0, (i, j) => i, (i, j) => i, null));

            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<int>)null).Aggregate(() => 0, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("seedFactory", () => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(null, (i, j) => i, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("updateAccumulatorFunc", () => ParallelEnumerable.Range(0, 1).Aggregate(() => 0, null, (i, j) => i, i => i));
            Assert.Throws<ArgumentNullException>("combineAccumulatorsFunc", () => ParallelEnumerable.Range(0, 1).Aggregate(() => 0, (i, j) => i, null, i => i));
            Assert.Throws<ArgumentNullException>("resultSelector", () => ParallelEnumerable.Range(0, 1).Aggregate<int, int, int>(() => 0, (i, j) => i, (i, j) => i, null));
        }
    }
}
