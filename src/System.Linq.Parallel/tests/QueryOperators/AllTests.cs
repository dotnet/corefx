// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class AllTests
    {
        public static IEnumerable<object[]> OnlyOneData(int[] counts)
        {
            Func<int, IEnumerable<int>> positions = x => new[] { 0, x / 2, Math.Max(0, x - 1) }.Distinct();
            foreach (object[] results in UnorderedSources.Ranges(counts.Cast<int>(), positions)) yield return results;
        }

        //
        // All
        //
        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void All_AllFalse(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count == 0, query.All(x => x < 0));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void All_AllFalse_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            All_AllFalse(labeled, count);
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void All_AllTrue(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.True(query.All(x => seen.Add(x)));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 1024, 1024 * 1024 * 4 }, MemberType = typeof(UnorderedSources))]
        public static void All_AllTrue_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            All_AllTrue(labeled, count);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 2, 16 })]
        public static void All_OneFalse(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.False(query.All(x => !(x == position)));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void All_OneFalse_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            All_OneFalse(labeled, count, position);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 2, 16 })]
        public static void All_OneTrue(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.False(query.All(x => x == position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void All_OneTrue_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            All_OneTrue(labeled, count, position);
        }

        [Fact]
        public static void All_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.All(x => { canceler(); return true; }));
        }

        [Fact]
        public static void All_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.All(x => { canceler(); return true; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.All(x => { canceler(); return true; }));
        }

        [Fact]
        public static void All_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.All(x => true));
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1 }, MemberType = typeof(UnorderedSources))]
        public static void All_AggregateException(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.All(x => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void All_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).All(x => x));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<bool>().All(null));
        }
    }
}
