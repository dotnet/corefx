// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class SingleSingleOrDefaultTests
    {
        public static IEnumerable<object[]> SingleSpecificData(int[] counts)
        {
            Func<int, IEnumerable<int>> positions = x => new[] { 0, x / 2, Math.Max(0, x - 1) }.Distinct();
            foreach (object[] results in UnorderedSources.Ranges(counts.Cast<int>(), positions)) yield return results;
            foreach (object[] results in Sources.Ranges(counts.Cast<int>(), positions)) yield return results;
        }

        public static IEnumerable<object[]> SingleData(int[] elements, int[] counts)
        {
            foreach (int element in elements)
            {
                foreach (object[] results in UnorderedSources.Ranges(element, counts.Cast<int>()))
                {
                    yield return new object[] { results[0], results[1], element };
                }
                foreach (object[] results in Sources.Ranges(element, counts.Cast<int>()))
                {
                    yield return new object[] { results[0], results[1], element };
                }
            }
        }

        //
        // Single and SingleOrDefault
        //
        [Theory]
        [MemberData(nameof(SingleData), new[] { 0, 2, 16, 1024 * 1024 }, new[] { 1 })]
        public static void Single(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(element, query.Single());
            Assert.Equal(element, query.Single(x => true));
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0, 2, 16, 1024 * 1024 }, new[] { 0, 1 })]
        public static void SingleOrDefault(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Equal(count >= 1 ? element : default(int), query.SingleOrDefault());
            Assert.Equal(count >= 1 ? element : default(int), query.SingleOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0, 1024 * 1024 }, new[] { 0 })]
        public static void Single_Empty(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Throws<InvalidOperationException>(() => query.Single());
            Assert.Throws<InvalidOperationException>(() => query.Single(x => true));
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 0, 1, 2, 16 })]
        public static void Single_NoMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Throws<InvalidOperationException>(() => query.Single(x => !seen.Add(x)));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 1024 * 4, 1024 * 1024 })]
        public static void Single_NoMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            Single_NoMatch(labeled, count, element);
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 0, 1, 2, 16 })]
        public static void SingleOrDefault_NoMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Equal(default(int), query.SingleOrDefault(x => !seen.Add(x)));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 1024 * 4, 1024 * 1024 })]
        public static void SingleOrDefault_NoMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            SingleOrDefault_NoMatch(labeled, count, element);
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 2, 16 })]
        public static void Single_AllMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Throws<InvalidOperationException>(() => query.Single(x => true));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 1024 * 4, 1024 * 1024 })]
        public static void Single_AllMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            Single_AllMatch(labeled, count, element);
        }

        [Theory]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 2, 16 })]
        public static void SingleOrDefault_AllMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            Assert.Throws<InvalidOperationException>(() => query.SingleOrDefault(x => true));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleData), new[] { 0 }, new[] { 1024 * 4, 1024 * 1024 })]
        public static void SingleOrDefault_AllMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            SingleOrDefault_AllMatch(labeled, count, element);
        }

        [Theory]
        [MemberData(nameof(SingleSpecificData), new[] { 1, 2, 16 })]
        public static void Single_OneMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Equal(element, query.Single(x => seen.Add(x) && x == element));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleSpecificData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void Single_OneMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            Single_OneMatch(labeled, count, element);
        }

        [Theory]
        [MemberData(nameof(SingleSpecificData), new[] { 0, 1, 2, 16 })]
        public static void SingleOrDefault_OneMatch(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.Equal(element, query.SingleOrDefault(x => seen.Add(x) && x == element));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(SingleSpecificData), new[] { 1024 * 4, 1024 * 1024 })]
        public static void SingleOrDefault_OneMatch_Longrunning(Labeled<ParallelQuery<int>> labeled, int count, int element)
        {
            SingleOrDefault_OneMatch(labeled, count, element);
        }

        [Fact]
        public static void Single_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.Single(x => { canceler(); return false; }));
        }

        [Fact]
        public static void SingleOrDefault_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.SingleOrDefault(x => { canceler(); return false; }));
        }

        [Fact]
        public static void Single_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Single(x => { canceler(); return false; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Single(x => { canceler(); return false; }));
        }

        [Fact]
        public static void SingleOrDefault_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.SingleOrDefault(x => { canceler(); return false; }));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.SingleOrDefault(x => { canceler(); return false; }));
        }

        [Fact]
        public static void Single_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.Single());
            AssertThrows.AlreadyCanceled(source => source.Single(x => true));
        }

        [Fact]
        public static void SingleOrDefault_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.SingleOrDefault());
            AssertThrows.AlreadyCanceled(source => source.SingleOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1 }, MemberType = typeof(UnorderedSources))]
        public static void Single_AggregateException(Labeled<ParallelQuery<int>> labeled, int count)
        {
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.Single(x => { throw new DeliberateTestException(); }));
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => labeled.Item.SingleOrDefault(x => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void Single_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).Single());
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).SingleOrDefault());

            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<int>().Single(null));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Empty<int>().SingleOrDefault(null));
        }
    }
}
