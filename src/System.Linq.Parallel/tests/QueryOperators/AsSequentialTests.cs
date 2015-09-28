// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class AsSequentialTests
    {
        [Theory]
        [MemberData("Ranges", (object)(new int[] { 0, 1, 2, 16 }), MemberType = typeof(UnorderedSources))]
        public static void AsSequential_Unordered(Labeled<ParallelQuery<int>> labeled, int count)
        {
            IntegerRangeSet seen = new IntegerRangeSet(0, count);
            IEnumerable<int> seq = labeled.Item.AsSequential();
            Assert.All(seq, x => seen.Add(x));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData("Ranges", (object)(new int[] { 1024 * 1024, 1024 * 1024 * 4 }), MemberType = typeof(UnorderedSources))]
        public static void AsSequential_Unordered_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            AsSequential_Unordered(labeled, count);
        }

        [Theory]
        [MemberData("Ranges", (object)(new int[] { 0, 1, 2, 16 }), MemberType = typeof(Sources))]
        public static void AsSequential(Labeled<ParallelQuery<int>> labeled, int count)
        {
            int seen = 0;
            IEnumerable<int> seq = labeled.Item.AsSequential();
            Assert.All(seq, x => Assert.Equal(seen++, x));
            Assert.Equal(count, seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData("Ranges", (object)(new int[] { 1024 * 4, 1024 * 128 }), MemberType = typeof(Sources))]
        public static void AsSequential_Longrunning(Labeled<ParallelQuery<int>> labeled, int count)
        {
            AsSequential(labeled, count);
        }

        [Theory]
        [ActiveIssue("AsSequential.Cast<T>() retains origin type")]
        [MemberData("Ranges", (object)(new int[] { 0, 16 }), MemberType = typeof(Sources))]
        public static void AsSequential_LinqBinding(Labeled<ParallelQuery<int>> labeled, int count)
        {
            IEnumerable<int> seq = labeled.Item.AsSequential();

            // The LINQ Cast<T>() retains origin type for ParallelEnumerable  and Partitioner when unordered,
            // (and for all sources when ordered, due to the extra wrapper)
            // although aliased as IEnumerable<T>, so further LINQ calls work as expected.
            // If this test starts failing, update this test, and maybe mention it in release notes.
            Assert.IsNotType<ParallelQuery<int>>(seq.Cast<int>());
            Assert.True(seq.Cast<int>() is ParallelQuery<int>);

            Assert.False(seq.Concat(Enumerable.Range(0, count)) is ParallelQuery<int>);
            Assert.False(seq.DefaultIfEmpty() is ParallelQuery<int>);
            Assert.False(seq.Distinct() is ParallelQuery<int>);
            Assert.False(seq.Except(Enumerable.Range(0, count)) is ParallelQuery<int>);
            Assert.False(seq.GroupBy(x => x) is ParallelQuery<int>);
            Assert.False(seq.GroupJoin(Enumerable.Range(0, count), x => x, y => y, (x, g) => x) is ParallelQuery<int>);
            Assert.False(seq.Intersect(Enumerable.Range(0, count)) is ParallelQuery<int>);
            Assert.False(seq.Join(Enumerable.Range(0, count), x => x, y => y, (x, y) => x) is ParallelQuery<int>);
            Assert.False(seq.OfType<int>() is ParallelQuery<int>);
            Assert.False(seq.OrderBy(x => x) is ParallelQuery<int>);
            Assert.False(seq.OrderByDescending(x => x) is ParallelQuery<int>);
            Assert.False(seq.Reverse() is ParallelQuery<int>);
            Assert.False(seq.Select(x => x) is ParallelQuery<int>);
            Assert.False(seq.SelectMany(x => new[] { x }) is ParallelQuery<int>);
            Assert.False(seq.Skip(count / 2) is ParallelQuery<int>);
            Assert.False(seq.SkipWhile(x => true) is ParallelQuery<int>);
            Assert.False(seq.Take(count / 2) is ParallelQuery<int>);
            Assert.False(seq.TakeWhile(x => true) is ParallelQuery<int>);
            Assert.False(seq.Union(Enumerable.Range(0, count)) is ParallelQuery<int>);
            Assert.False(seq.Where(x => true) is ParallelQuery<int>);
            Assert.False(seq.Zip(Enumerable.Range(0, count), (x, y) => x) is ParallelQuery<int>);
        }

        [Fact]
        public static void AsSequential_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).AsSequential());
        }
    }
}
