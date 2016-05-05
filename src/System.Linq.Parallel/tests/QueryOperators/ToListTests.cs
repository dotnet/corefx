// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class ToListTests
    {
        [Theory]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(UnorderedSources))]
        public static void ToList_Unordered(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            Assert.All(query.ToList(), x => seen.Add(x));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 4, 1024 * 1024 }, MemberType = typeof(UnorderedSources))]
        public static void ToList_Unordered_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ToList_Unordered(labeled, count);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 0, 1, 2, 16 }, MemberType = typeof(Sources))]
        public static void ToList(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            int seen = 0;
            Assert.All(query.ToList(), x => Assert.Equal(seen++, x));
            Assert.Equal(count, seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 4, 1024 * 1024 }, MemberType = typeof(Sources))]
        public static void ToList_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ToList(labeled, count);
        }

        [Fact]
        public static void ToList_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.ToList());
        }

        [Fact]
        public static void ToList_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<bool>)null).ToList());
        }
    }
}
