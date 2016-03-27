// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class LastLastOrDefaultTests
    {
        public static IEnumerable<object[]> LastUnorderedData(int[] counts)
        {
            Func<int, IEnumerable<int>> positions = x => new[] { 1, x / 2 + 1, Math.Max(1, x - 1) }.Distinct();
            foreach (object[] results in UnorderedSources.Ranges(counts.Cast<int>(), positions)) yield return results;
        }

        public static IEnumerable<object[]> LastData(int[] counts)
        {
            Func<int, IEnumerable<int>> positions = x => new[] { 1, x / 2 + 1, Math.Max(1, x - 1) }.Distinct();
            foreach (object[] results in Sources.Ranges(counts.Cast<int>(), positions)) yield return results;
        }

        //
        // Last and LastOrDefault
        //
        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 1, 2, 16 })]
        [MemberData(nameof(LastData), new[] { 1, 2, 16 })]
        public static void Last(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            // For unordered collections, which element is chosen isn't actually guaranteed, but an effect of the implementation.
            // If this test starts failing it should be split, and possibly mentioned in release notes.
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count - 1, query.Last());
            Assert.Equal(position - 1, query.Last(x => x < position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(LastUnorderedData), new[] { 1024 * 4, 1024 * 1024 })]
        [MemberData(nameof(LastData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void Last_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            Last(labeled, count, position);
        }

        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 1, 2, 16 })]
        [MemberData(nameof(LastData), new[] { 1, 2, 16 })]
        public static void LastOrDefault(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            // For unordered collections, which element is chosen isn't actually guaranteed, but an effect of the implementation.
            // If this test starts failing it should be split, and possibly mentioned in release notes.
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count - 1, query.Last());
            Assert.Equal(position - 1, query.Last(x => x < position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(LastUnorderedData), new[] { 1024 * 4, 1024 * 1024 })]
        [MemberData(nameof(LastData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void LastOrDefault_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            LastOrDefault(labeled, count, position);
        }

        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 0 })]
        [MemberData(nameof(LastData), new[] { 0 })]
        public static void Last_Empty(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Throws<InvalidOperationException>(() => query.Last());
        }

        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 0 })]
        [MemberData(nameof(LastData), new[] { 0 })]
        public static void LastOrDefault_Empty(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(default(int), query.LastOrDefault());
        }

        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 1, 2, 16 })]
        [MemberData(nameof(LastData), new[] { 1, 2, 16 })]
        public static void Last_NoMatch(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Throws<InvalidOperationException>(() => query.Last(x => !seen.Add(x)));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(LastUnorderedData), new[] { 1024 * 4, 1024 * 1024 })]
        [MemberData(nameof(LastData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void Last_NoMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            Last_NoMatch(labeled, count, position);
        }

        [Theory]
        [MemberData(nameof(LastUnorderedData), new[] { 1, 2, 16 })]
        [MemberData(nameof(LastData), new[] { 1, 2, 16 })]
        public static void LastOrDefault_NoMatch(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Equal(default(int), query.LastOrDefault(x => !seen.Add(x)));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(LastUnorderedData), new[] { 1024 * 4, 1024 * 1024 })]
        [MemberData(nameof(LastData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void LastOrDefault_NoMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int position)
        {
            LastOrDefault_NoMatch(labeled, count, position);
        }

        [Fact]
        public static void Last_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.Last(x => { canceler(); return true; }));
            AssertThrows.EventuallyCanceled((source, canceler) => source.LastOrDefault(x => { canceler(); return true; }));
        }

        [Fact]
        public static void Last_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Last(x => { canceler(); return true; }));
            AssertThrows.OtherTokenCanceled((source, canceler) => source.LastOrDefault(x => { canceler(); return false; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Last(x => { canceler(); return true; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.LastOrDefault(x => { canceler(); return false; }));
        }

        [Fact]
        public static void Last_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.Last());
            AssertThrows.AlreadyCanceled(source => source.Last(x => true));

            AssertThrows.AlreadyCanceled(source => source.LastOrDefault());
            AssertThrows.AlreadyCanceled(source => source.LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1 }, MemberType = typeof(UnorderedSources))]
        public static void Last_AggregateException(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Last(x => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.LastOrDefault(x => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void Last_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).Last());
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).LastOrDefault());

            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<int>().Last(null));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<int>().LastOrDefault(null));
        }
    }
}
