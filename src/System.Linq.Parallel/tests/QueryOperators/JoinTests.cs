// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public static class JoinTests
    {
        private const int KeyFactor = 8;

        public static IEnumerable<object[]> JoinUnorderedData(int[] counts)
        {
            foreach (int leftCount in counts)
            {
                foreach (int rightCount in counts)
                {
                    yield return new object[] { leftCount, rightCount };
                }
            }
        }

        public static IEnumerable<object[]> JoinData(int[] counts)
        {
            // When dealing with joins, if there aren't multiple matches the ordering of the second operand is immaterial.
            foreach (object[] parms in Sources.Ranges(counts, i => counts))
            {
                yield return parms;
            }
        }

        public static IEnumerable<object[]> JoinMultipleData(int[] counts)
        {
            foreach (object[] parms in UnorderedSources.BinaryRanges(counts, counts))
            {
                yield return new object[] { ((Labeled<ParallelQuery<int>>)parms[0]).Order(), parms[1], ((Labeled<ParallelQuery<int>>)parms[2]).Order(), parms[3] };
            }
        }

        public static IEnumerable<object[]> JoinOrderedLeftUnorderedRightData(int[] counts)
        {
            foreach (object[] parms in UnorderedSources.BinaryRanges(counts, counts))
            {
                yield return new object[] { ((Labeled<ParallelQuery<int>>)parms[0]).Order(), parms[1], ((Labeled<ParallelQuery<int>>)parms[2]), parms[3] };
            }
        }

        //
        // Join
        //
        [Theory]
        [MemberData(nameof(JoinUnorderedData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_Unordered(int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = UnorderedSources.Default(leftCount);
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            IntegerRangeSet seen = new IntegerRangeSet(0, Math.Min(leftCount, (rightCount + (KeyFactor - 1)) / KeyFactor));
            foreach (var p in leftQuery.Join(rightQuery, x => x * KeyFactor, y => y, (x, y) => KeyValuePair.Create(x, y)))
            {
                Assert.Equal(p.Key * KeyFactor, p.Value);
                seen.Add(p.Key);
            }
            seen.AssertComplete();
        }

        [Fact]
        [OuterLoop]
        public static void Join_Unordered_Longrunning()
        {
            Join_Unordered(Sources.OuterLoopCount / 64, Sources.OuterLoopCount / 64);
        }

        [Theory]
        [MemberData(nameof(JoinData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            int seen = 0;
            foreach (var p in leftQuery.Join(rightQuery, x => x * KeyFactor, y => y, (x, y) => KeyValuePair.Create(x, y)))
            {
                Assert.Equal(seen++, p.Key);
                Assert.Equal(p.Key * KeyFactor, p.Value);
            }
            Assert.Equal(Math.Min(leftCount, (rightCount + (KeyFactor - 1)) / KeyFactor), seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinData), new[] { 512, 1024 })]
        public static void Join_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            Join(left, leftCount, rightCount);
        }

        [Theory]
        [MemberData(nameof(JoinUnorderedData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_Unordered_NotPipelined(int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = UnorderedSources.Default(leftCount);
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            IntegerRangeSet seen = new IntegerRangeSet(0, Math.Min(leftCount, (rightCount + (KeyFactor - 1)) / KeyFactor));
            Assert.All(leftQuery.Join(rightQuery, x => x * KeyFactor, y => y, (x, y) => KeyValuePair.Create(x, y)).ToList(),
                p => { Assert.Equal(p.Key * KeyFactor, p.Value); seen.Add(p.Key); });
            seen.AssertComplete();
        }

        [Fact]
        [OuterLoop]
        public static void Join_Unordered_NotPipelined_Longrunning()
        {
            Join_Unordered_NotPipelined(Sources.OuterLoopCount / 64, Sources.OuterLoopCount / 64);
        }

        [Theory]
        [MemberData(nameof(JoinData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_NotPipelined(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            int seen = 0;
            Assert.All(leftQuery.Join(rightQuery, x => x * KeyFactor, y => y, (x, y) => KeyValuePair.Create(x, y)).ToList(),
                p => { Assert.Equal(seen++, p.Key); Assert.Equal(p.Key * KeyFactor, p.Value); });
            Assert.Equal(Math.Min(leftCount, (rightCount + (KeyFactor - 1)) / KeyFactor), seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinData), new[] { 512, 1024 })]
        public static void Join_NotPipelined_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            Join_NotPipelined(left, leftCount, rightCount);
        }

        [Theory]
        [MemberData(nameof(JoinUnorderedData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_Unordered_Multiple(int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = UnorderedSources.Default(leftCount);
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            IntegerRangeSet seenOuter = new IntegerRangeSet(0, Math.Min(leftCount, (rightCount + (KeyFactor - 1)) / KeyFactor));
            IntegerRangeSet seenInner = new IntegerRangeSet(0, Math.Min(leftCount * KeyFactor, rightCount));
            Assert.All(leftQuery.Join(rightQuery, x => x, y => y / KeyFactor, (x, y) => KeyValuePair.Create(x, y)),
                p =>
                {
                    Assert.Equal(p.Key, p.Value / KeyFactor);
                    seenInner.Add(p.Value);
                    if (p.Value % KeyFactor == 0) seenOuter.Add(p.Key);
                });
            seenOuter.AssertComplete();
            seenInner.AssertComplete();
        }

        [Fact]
        [OuterLoop]
        public static void Join_Unordered_Multiple_Longrunning()
        {
            Join_Unordered_Multiple(Sources.OuterLoopCount / 64, Sources.OuterLoopCount / 64);
        }

        [Theory]
        [MemberData(nameof(JoinMultipleData), new[] { 0, 1, 2, KeyFactor * 2 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_Multiple(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            int seen = 0;
            Assert.All(leftQuery.Join(rightQuery, x => x, y => y / KeyFactor, (x, y) => KeyValuePair.Create(x, y)),
                p =>
                {
                    Assert.Equal(p.Key, p.Value / KeyFactor);
                    Assert.Equal(seen++, p.Value);
                });
            Assert.Equal(Math.Min(leftCount * KeyFactor, rightCount), seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinMultipleData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_Multiple_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_Multiple(left, leftCount, right, rightCount);
        }

        private class LeftOrderingCollisionTestWithOrderedRight : LeftOrderingCollisionTest
        {
            protected override ParallelQuery<KeyValuePair<int, int>> Join(ParallelQuery<int> left, ParallelQuery<int> right)
            {
                return ReorderLeft(left).Join(right, x => x, y => y % KeyFactor, (x, y) => KeyValuePair.Create(x, y));
            }

            protected override void ValidateRightValue(int left, int right, int seenRightCount)
            {
                Assert.Equal(left, right % KeyFactor);
                Assert.Equal(left + seenRightCount * KeyFactor, right);
            }

            protected override int GetExpectedSeenLeftCount(int leftCount, int rightCount)
            {
                return Math.Min(KeyFactor, Math.Min(rightCount, leftCount));
            }
        }

        [Theory]
        [MemberData(nameof(JoinMultipleData), new[] { 2, KeyFactor - 1, KeyFactor, KeyFactor + 1, KeyFactor * 2 - 1, KeyFactor * 2, KeyFactor * 2 + 1 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_Multiple_LeftWithOrderingColisions(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            LeftOrderingCollisionTestWithOrderedRight validator = new LeftOrderingCollisionTestWithOrderedRight();

            validator.Validate(left.Item, leftCount, right.Item, rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinMultipleData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_Multiple_LeftWithOrderingColisions_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_Multiple_LeftWithOrderingColisions(left, leftCount, right, rightCount);
        }

        private class LeftOrderingCollisionTestWithUnorderedRight : LeftOrderingCollisionTest
        {
            protected override ParallelQuery<KeyValuePair<int, int>> Join(ParallelQuery<int> left, ParallelQuery<int> right)
            {
                return ReorderLeft(left).Join(right, x => x, y => y % KeyFactor, (x, y) => KeyValuePair.Create(x, y)).Distinct();
            }

            protected override void ValidateRightValue(int left, int right, int seenRightCount)
            {
                Assert.Equal(left, right % KeyFactor);
            }

            protected override int GetExpectedSeenLeftCount(int leftCount, int rightCount)
            {
                return Math.Min(KeyFactor, Math.Min(rightCount, leftCount));
            }
        }

        [Theory]
        [MemberData(nameof(JoinOrderedLeftUnorderedRightData), new[] { 2, KeyFactor - 1, KeyFactor, KeyFactor + 1, KeyFactor * 2 - 1, KeyFactor * 2, KeyFactor * 2 + 1 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve grouping of order-identical left keys through repartitioning (see https://github.com/dotnet/corefx/pull/27930#issuecomment-372084741)")]
        public static void Join_Multiple_LeftWithOrderingColisions_UnorderedRight(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            LeftOrderingCollisionTestWithUnorderedRight validator = new LeftOrderingCollisionTestWithUnorderedRight();

            validator.Validate(left.Item, leftCount, right.Item, rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinOrderedLeftUnorderedRightData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve grouping of order-identical left keys through repartitioning (see https://github.com/dotnet/corefx/pull/27930#issuecomment-372084741)")]
        public static void Join_Multiple_LeftWithOrderingColisions_UnorderedRight_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_Multiple_LeftWithOrderingColisions_UnorderedRight(left, leftCount, right, rightCount);
        }

        [Theory]
        [MemberData(nameof(JoinUnorderedData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_Unordered_CustomComparator(int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = UnorderedSources.Default(leftCount);
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            IntegerRangeCounter seenOuter = new IntegerRangeCounter(0, leftCount);
            IntegerRangeCounter seenInner = new IntegerRangeCounter(0, rightCount);
            Assert.All(leftQuery.Join(rightQuery, x => x, y => y,
                (x, y) => KeyValuePair.Create(x, y), new ModularCongruenceComparer(KeyFactor)),
                p =>
                {
                    Assert.Equal(p.Key % KeyFactor, p.Value % KeyFactor);
                    seenOuter.Add(p.Key);
                    seenInner.Add(p.Value);
                });
            if (leftCount == 0 || rightCount == 0)
            {
                seenOuter.AssertEncountered(0);
                seenInner.AssertEncountered(0);
            }
            else
            {
                Func<int, int, int> cartesian = (key, other) => (other + (KeyFactor - 1) - key % KeyFactor) / KeyFactor;
                Assert.All(seenOuter, kv => Assert.Equal(cartesian(kv.Key, rightCount), kv.Value));
                Assert.All(seenInner, kv => Assert.Equal(cartesian(kv.Key, leftCount), kv.Value));
            }
        }

        [Fact]
        [OuterLoop]
        public static void Join_Unordered_CustomComparator_Longrunning()
        {
            Join_Unordered_CustomComparator(Sources.OuterLoopCount / 64, Sources.OuterLoopCount / 64);
        }

        [Theory]
        [MemberData(nameof(JoinMultipleData), new[] { 0, 1, 2, KeyFactor * 2 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_CustomComparator(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = right.Item;
            int seenOuter = 0;
            int previousOuter = -1;
            int seenInner = Math.Max(previousOuter % KeyFactor, rightCount - KeyFactor + previousOuter % KeyFactor);

            Assert.All(leftQuery.Join(rightQuery, x => x, y => y,
                (x, y) => KeyValuePair.Create(x, y), new ModularCongruenceComparer(KeyFactor)),
                p =>
                {
                    if (p.Key != previousOuter)
                    {
                        Assert.Equal(seenOuter, p.Key);
                        Assert.Equal(p.Key % 8, p.Value);
                    // If there aren't sufficient elements in the RHS (< 8), the LHS skips an entry at the end of the mod cycle.
                    seenOuter = Math.Min(leftCount, seenOuter + (p.Key % KeyFactor + 1 == rightCount ? KeyFactor - p.Key % KeyFactor : 1));
                        Assert.Equal(Math.Max(previousOuter % KeyFactor, rightCount - KeyFactor + previousOuter % KeyFactor), seenInner);
                        previousOuter = p.Key;
                        seenInner = (p.Key % KeyFactor) - KeyFactor;
                    }
                    Assert.Equal(p.Key % KeyFactor, p.Value % KeyFactor);
                    Assert.Equal(seenInner += KeyFactor, p.Value);
                });
            Assert.Equal(rightCount == 0 ? 0 : leftCount, seenOuter);
            Assert.Equal(Math.Max(previousOuter % KeyFactor, rightCount - KeyFactor + previousOuter % KeyFactor), seenInner);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinMultipleData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_CustomComparator_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_CustomComparator(left, leftCount, right, rightCount);
        }

        private class LeftOrderingCollisionTestWithOrderedRightAndCustomComparator : LeftOrderingCollisionTest
        {
            protected override ParallelQuery<KeyValuePair<int, int>> Join(ParallelQuery<int> left, ParallelQuery<int> right)
            {
                return ReorderLeft(left).Join(right, x => x, y => y, (x, y) => KeyValuePair.Create(x, y), new ModularCongruenceComparer(KeyFactor));
            }

            protected override void ValidateRightValue(int left, int right, int seenRightCount)
            {
                Assert.Equal(left % KeyFactor, right % KeyFactor);
                Assert.Equal(left % KeyFactor + seenRightCount * KeyFactor, right);
            }

            protected override int GetExpectedSeenLeftCount(int leftCount, int rightCount)
            {
                if (rightCount >= KeyFactor) 
                {
                    return leftCount;
                }
                else {
                    return leftCount / KeyFactor * rightCount + Math.Min(leftCount % KeyFactor, rightCount);
                }
            }
        }

        [Theory]
        [MemberData(nameof(JoinMultipleData), new[] { 2, KeyFactor - 1, KeyFactor, KeyFactor + 1, KeyFactor * 2 - 1, KeyFactor * 2, KeyFactor * 2 + 1 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_CustomComparator_LeftWithOrderingColisions(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            LeftOrderingCollisionTestWithOrderedRightAndCustomComparator validator = new LeftOrderingCollisionTestWithOrderedRightAndCustomComparator();

            validator.Validate(left.Item, leftCount, right.Item, rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinMultipleData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve the right collection order (.Net core bug fix https://github.com/dotnet/corefx/pull/27930)")]
        public static void Join_CustomComparator_LeftWithOrderingColisions_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_CustomComparator_LeftWithOrderingColisions(left, leftCount, right, rightCount);
        }

        private class LeftOrderingCollisionTestWithUnorderedRightAndCustomComparator : LeftOrderingCollisionTest
        {
            protected override ParallelQuery<KeyValuePair<int, int>> Join(ParallelQuery<int> left, ParallelQuery<int> right)
            {
                return ReorderLeft(left).Join(right, x => x, y => y, (x, y) => KeyValuePair.Create(x, y), new ModularCongruenceComparer(KeyFactor)).Distinct();
            }

            protected override void ValidateRightValue(int left, int right, int seenRightCount)
            {
                Assert.Equal(left % KeyFactor, right % KeyFactor);
            }

            protected override int GetExpectedSeenLeftCount(int leftCount, int rightCount)
            {
                if (rightCount >= KeyFactor)
                {
                    return leftCount;
                }
                else
                {
                    return leftCount / KeyFactor * rightCount + Math.Min(leftCount % KeyFactor, rightCount);
                }
            }
        }

        [Theory]
        [MemberData(nameof(JoinOrderedLeftUnorderedRightData), new[] { 2, KeyFactor - 1, KeyFactor, KeyFactor + 1, KeyFactor * 2 - 1, KeyFactor * 2, KeyFactor * 2 + 1 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve grouping of order-identical left keys through repartitioning (see https://github.com/dotnet/corefx/pull/27930#issuecomment-372084741)")]
        public static void Join_CustomComparator_LeftWithOrderingColisions_UnorderedRight(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            LeftOrderingCollisionTestWithUnorderedRightAndCustomComparator validator = new LeftOrderingCollisionTestWithUnorderedRightAndCustomComparator();

            validator.Validate(left.Item, leftCount, right.Item, rightCount);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinOrderedLeftUnorderedRightData), new[] { 512, 1024 })]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full framework doesn't preserve grouping of order-identical left keys through repartitioning (see https://github.com/dotnet/corefx/pull/27930#issuecomment-372084741)")]
        public static void Join_CustomComparator_LeftWithOrderingColisions_UnorderedRight_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, Labeled<ParallelQuery<int>> right, int rightCount)
        {
            Join_CustomComparator_LeftWithOrderingColisions_UnorderedRight(left, leftCount, right, rightCount);
        }

        [Theory]
        [MemberData(nameof(JoinData), new[] { 0, 1, 2, KeyFactor * 2 })]
        public static void Join_InnerJoin_Ordered(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            ParallelQuery<int> leftQuery = left.Item;
            ParallelQuery<int> rightQuery = UnorderedSources.Default(rightCount);
            ParallelQuery<int> middleQuery = ParallelEnumerable.Range(0, leftCount).AsOrdered();

            int seen = 0;
            Assert.All(leftQuery.Join(middleQuery.Join(rightQuery, x => x * KeyFactor / 2, y => y, (x, y) => KeyValuePair.Create(x, y)),
                z => z * 2, p => p.Key, (x, p) => KeyValuePair.Create(x, p)),
                pOuter =>
                {
                    KeyValuePair<int, int> pInner = pOuter.Value;
                    Assert.Equal(seen++, pOuter.Key);
                    Assert.Equal(pOuter.Key * 2, pInner.Key);
                    Assert.Equal(pOuter.Key * KeyFactor, pInner.Value);
                });
            Assert.Equal(Math.Min((leftCount + 1) / 2, (rightCount + (KeyFactor - 1)) / KeyFactor), seen);
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(JoinData), new[] { 512, 1024 })]
        public static void Join_InnerJoin_Ordered_Longrunning(Labeled<ParallelQuery<int>> left, int leftCount, int rightCount)
        {
            Join_InnerJoin_Ordered(left, leftCount, rightCount);
        }

        [Fact]
        public static void Join_NotSupportedException()
        {
#pragma warning disable 618
            Assert.Throws<NotSupportedException>(() => ParallelEnumerable.Range(0, 1).Join(Enumerable.Range(0, 1), i => i, i => i, (i, j) => i));
            Assert.Throws<NotSupportedException>(() => ParallelEnumerable.Range(0, 1).Join(Enumerable.Range(0, 1), i => i, i => i, (i, j) => i, null));
#pragma warning restore 618
        }

        [Fact]
        // Should not get the same setting from both operands.
        public static void Join_NoDuplicateSettings()
        {
            CancellationToken t = new CancellationTokenSource().Token;
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithCancellation(t).Join(ParallelEnumerable.Range(0, 1).WithCancellation(t), x => x, y => y, (l, r) => l));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithDegreeOfParallelism(1).Join(ParallelEnumerable.Range(0, 1).WithDegreeOfParallelism(1), x => x, y => y, (l, r) => l));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithExecutionMode(ParallelExecutionMode.Default).Join(ParallelEnumerable.Range(0, 1).WithExecutionMode(ParallelExecutionMode.Default), x => x, y => y, (l, r) => l));
            Assert.Throws<InvalidOperationException>(() => ParallelEnumerable.Range(0, 1).WithMergeOptions(ParallelMergeOptions.Default).Join(ParallelEnumerable.Range(0, 1).WithMergeOptions(ParallelMergeOptions.Default), x => x, y => y, (l, r) => l));
        }

        [Fact]
        public static void Join_ArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("outer", () => ((ParallelQuery<int>)null).Join(ParallelEnumerable.Range(0, 1), i => i, i => i, (i, j) => i));
            AssertExtensions.Throws<ArgumentNullException>("inner", () => ParallelEnumerable.Range(0, 1).Join((ParallelQuery<int>)null, i => i, i => i, (i, j) => i));
            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), (Func<int, int>)null, i => i, (i, j) => i));
            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), i => i, (Func<int, int>)null, (i, j) => i));
            AssertExtensions.Throws<ArgumentNullException>("resultSelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), i => i, i => i, (Func<int, int, int>)null));
            AssertExtensions.Throws<ArgumentNullException>("outer", () => ((ParallelQuery<int>)null).Join(ParallelEnumerable.Range(0, 1), i => i, i => i, (i, j) => i, EqualityComparer<int>.Default));
            AssertExtensions.Throws<ArgumentNullException>("inner", () => ParallelEnumerable.Range(0, 1).Join((ParallelQuery<int>)null, i => i, i => i, (i, j) => i, EqualityComparer<int>.Default));
            AssertExtensions.Throws<ArgumentNullException>("outerKeySelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), (Func<int, int>)null, i => i, (i, j) => i, EqualityComparer<int>.Default));
            AssertExtensions.Throws<ArgumentNullException>("innerKeySelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), i => i, (Func<int, int>)null, (i, j) => i, EqualityComparer<int>.Default));
            AssertExtensions.Throws<ArgumentNullException>("resultSelector", () => ParallelEnumerable.Range(0, 1).Join(ParallelEnumerable.Range(0, 1), i => i, i => i, (Func<int, int, int>)null, EqualityComparer<int>.Default));
        }

        private abstract class LeftOrderingCollisionTest
        {
            protected ParallelQuery<int> ReorderLeft(ParallelQuery<int> left)
            {
                return left.AsUnordered().OrderBy(x => x % 2);
            }

            protected abstract ParallelQuery<KeyValuePair<int, int>> Join(ParallelQuery<int> left, ParallelQuery<int> right);
            protected abstract void ValidateRightValue(int left, int right, int seenRightCount);
            protected abstract int GetExpectedSeenLeftCount(int leftCount, int rightCount);

            public void Validate(ParallelQuery<int> left, int leftCount, ParallelQuery<int> right, int rightCount)
            {
                HashSet<int> seenLeft = new HashSet<int>();
                HashSet<int> seenRight = new HashSet<int>();

                int currentLeft = -1;
                bool seenOdd = false;

                Assert.All(Join(left, right),
                    p =>
                    {
                        try
                        {
                            if (currentLeft != p.Key)
                            {
                                try
                                {
                                    if (p.Key % 2 == 1)
                                    {
                                        seenOdd = true;
                                    }
                                    else
                                    {
                                        Assert.False(seenOdd, "Key out of order! " + p.Key.ToString());
                                    }
                                    Assert.True(seenLeft.Add(p.Key), "Key already seen! " + p.Key.ToString());
                                    if (currentLeft != -1)
                                    {
                                        Assert.Equal((rightCount / KeyFactor) + (((rightCount % KeyFactor) > (currentLeft % KeyFactor)) ? 1 : 0), seenRight.Count);
                                    }
                                }
                                finally
                                {
                                    currentLeft = p.Key;
                                    seenRight.Clear();
                                }
                            }
                            ValidateRightValue(p.Key, p.Value, seenRight.Count);
                            Assert.True(seenRight.Add(p.Value), "Value already seen! " + p.Value.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Key: {0}, Value: {1}", p.Key, p.Value), ex);
                        }
                        finally
                        {
                            seenRight.Add(p.Value);
                        }
                    });
                Assert.Equal((rightCount / KeyFactor) + (((rightCount % KeyFactor) > (currentLeft % KeyFactor)) ? 1 : 0), seenRight.Count);
                Assert.Equal(GetExpectedSeenLeftCount(leftCount, rightCount), seenLeft.Count);
            }
        }
    }
}
