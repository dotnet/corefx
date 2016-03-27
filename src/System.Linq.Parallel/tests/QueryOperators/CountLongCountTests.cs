// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class CountLongCountTests
    {
        public static IEnumerable<object[]> OnlyOneData(int[] counts)
        {
            Func<int, IEnumerable<int>> positions = x => new[] { 0, x / 2, Math.Max(0, x - 1) }.Distinct();
            foreach (object[] results in UnorderedSources.Ranges(counts.Cast<int>(), positions)) yield return results;
        }

        //
        // Count and LongCount
        //
        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Count_All(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count, query.Count());
            Assert.Equal(count, query.Count(i => i < count));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Count_All_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Count_All(labeled, count);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void LongCount_All(Labeled<ParallelQuery<int>> labeled, long count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count, query.LongCount());
            Assert.Equal(count, query.LongCount(i => i < count));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void LongCount_All_Longrunning(Labeled<ParallelQuery<int>> labeled, long count)
        {
            LongCount_All(labeled, count);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void Count_None(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(0, query.Count(i => i == -1));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void Count_None_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Count_None(labeled, count);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void LongCount_None(Labeled<ParallelQuery<int>> labeled, long count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(0, query.LongCount(i => i == -1));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void LongCount_None_Longrunning(Labeled<ParallelQuery<int>> labeled, long count)
        {
            LongCount_None(labeled, count);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 0, 1, 2, 16 })]
        public static void Count_One(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Math.Min(1, count), query.Count(i => i == position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void Count_One_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            Count_One(labeled, count, position);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 0, 1, 2, 16 })]
        public static void LongCount_One(Labeled<ParallelQuery<int>> labeled, int count, long position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(Math.Min(1, count), query.LongCount(i => i == position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void LongCount_One_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, long position)
        {
            LongCount_One(labeled, count, position);
        }

        [Fact]
        public static void Count_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.Count(x => { canceler(); return true; }));
            AssertThrows.EventuallyCanceled((source, canceler) => source.LongCount(x => { canceler(); return true; }));
        }

        [Fact]
        public static void Count_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Count(x => { canceler(); return true; }));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.LongCount(x => { canceler(); return true; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Count(x => { canceler(); return true; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.LongCount(x => { canceler(); return true; }));
        }

        [Fact]
        public static void CountLongCount_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.Count());
            AssertThrows.AlreadyCanceled(source => source.Count(x => true));

            AssertThrows.AlreadyCanceled(source => source.LongCount());
            AssertThrows.AlreadyCanceled(source => source.LongCount(x => true));
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1 }, MemberType = typeof(UnorderedSources))]
        public static void CountLongCount_AggregateException(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Count(x => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.LongCount(x => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void CountLongCount_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).Count());
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).Count(x => x));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<bool>().Count(null));

            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).LongCount());
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).LongCount(x => x));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<bool>().LongCount(null));
        }
    }
}
