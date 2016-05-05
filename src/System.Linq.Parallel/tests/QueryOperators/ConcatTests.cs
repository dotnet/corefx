// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class ConcatTests
    {
        public static IEnumerable<object[]> ConcatUnorderedData(int[] counts)
        {
            foreach (object[] parms in UnorderedSources.BinaryRanges(counts.Cast<int>(), (left, right) => left, counts.Cast<int>())) yield return parms.Take(4).ToArray();
        }

        public static IEnumerable<object[]> ConcatData(int[] counts)
        {
            foreach (object[] parms in ConcatUnorderedData(counts))
            {
                yield return new object[] { ((Labeled<ParallelQuery<int>>)parms[0]).Order(), parms[1], ((Labeled<ParallelQuery<int>>)parms[2]).Order(), parms[3] };
                yield return new object[] { ((Labeled<ParallelQuery<int>>)parms[0]).Order(), parms[1], parms[2], parms[3] };
                yield return new object[] { parms[0], parms[1], ((Labeled<ParallelQuery<int>>)parms[2]).Order(), parms[3] };
            }
        }

        //
        // Concat
        //
        [Theory]
        [MemberData(nameof(ConcatUnorderedData), new[] { 0, 1, 2, 16 })]
        public static void Concat_Unordered(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, leftCount + rightCount);
            foreach (int i in leftQuery.Concat(rightQuery))
            {
                seen.Add(i);
            }
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(ConcatUnorderedData), new[] { 1024, 1024 * 16 })]
        public static void Concat_Unordered_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Concat_Unordered(left, leftCount, right, rightCount);
        }

        [Theory]
        [MemberData(nameof(ConcatData), new[] { 0, 1, 2, 16 })]
        public static void Concat(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            int seen = 0;
            foreach (int i in leftQuery.Concat(rightQuery))
            {
                Assert.Equal(seen++, i);
            }
            Assert.Equal(seen, leftCount + rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(ConcatData), new[] { 1024, 1024 * 16 })]
        public static void Concat_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Concat(left, leftCount, right, rightCount);
        }

        [Theory]
        [MemberData(nameof(ConcatUnorderedData), new[] { 0, 1, 2, 16 })]
        public static void Concat_Unordered_NotPipelined(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            IntegerRangeSet seen = new IntegerRangeSet(0, leftCount + rightCount);
            Assert.All(leftQuery.Concat(rightQuery).ToList(), x => seen.Add(x));
            seen.AssertComplete();
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(ConcatUnorderedData), new[] { 1024, 1024 * 16 })]
        public static void Concat_Unordered_NotPipelined_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Concat_Unordered_NotPipelined(left, leftCount, right, rightCount);
        }

        [Theory]
        [MemberData(nameof(ConcatData), new[] { 0, 1, 2, 16 })]
        public static void Concat_NotPipelined(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            int seen = 0;
            Assert.All(leftQuery.Concat(rightQuery).ToList(), x => Assert.Equal(seen++, x));
            Assert.Equal(seen, leftCount + rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(ConcatData), new[] { 1024, 1024 * 16 })]
        public static void Concat_NotPipelined_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Concat_NotPipelined(left, leftCount, right, rightCount);
        }

        [Fact]
        public static void Concat_NotSupportedException()
        {
#pragma warning disable 618
            Assert.Throws<NotSupportedException>(() => ParallelEnumerable.Range(0, 1).Concat(Enumerable.Range(0, 1)));
#pragma warning restore 618
        }

        [Fact]
        // Should not get the same setting from both operands.
        public static void Concat_NoDuplicateSettings()
        {
            CancellationToken t = new CancellationTokenSource().Token;
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithCancellation(t).Concat(ParallelEnumerable.Range(0, 1).WithCancellation(t)));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithDegreeOfParallelism(1).Concat(ParallelEnumerable.Range(0, 1).WithDegreeOfParallelism(1)));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithExecutionMode(ParallelExecutionMode.Default).Concat(ParallelEnumerable.Range(0, 1).WithExecutionMode(ParallelExecutionMode.Default)));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithMergeOptions(ParallelMergeOptions.Default).Concat(ParallelEnumerable.Range(0, 1).WithMergeOptions(ParallelMergeOptions.Default)));
        }

        [Fact]
        public static void Concat_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((ParallelQuery<int>)null).Concat(ParallelEnumerable.Range(0, 1)));
            Assert.Throws<ArgumentNullException>(() => ParallelEnumerable.Range(0, 1).Concat(null));
        }

        [Fact]
        public static void Concat_UnionSources_PrematureMerges()
        {
            const int ElementCount = 2048;
            ParallelQuery<int> leftQuery = ParallelEnumerable.Range(0, ElementCount / 4).Union(ParallelEnumerable.Range(ElementCount / 4, ElementCount / 4));
            ParallelQuery<int> rightQuery = ParallelEnumerable.Range(2 * ElementCount / 4, ElementCount / 4).Union(ParallelEnumerable.Range(3 * ElementCount / 4, ElementCount / 4));

            var results = new HashSet<int>(leftQuery.Concat(rightQuery));
            Assert.Equal(ElementCount, results.Count);
        }
    }
}
