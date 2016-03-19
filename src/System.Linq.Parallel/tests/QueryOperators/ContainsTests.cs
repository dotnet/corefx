// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public class ContainsTests
    {
        public static IEnumerable<object[]> OnlyOneData(int[] counts)
        {
            foreach (int count in counts)
            {
                foreach (int position in new[] { 0, count / 2, Math.Max(0, count - 1) }.Distinct())
                {
                    yield return new object[] { count, position };
                }
            }
        }

        //
        // Contains
        //
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Contains_NoMatching(int count)
        {
            Assert.False(ParallelEnumerable.Range(0, count).Contains(count));
            Assert.False(ParallelEnumerable.Range(0, count).Contains(count, null));
            Assert.False(ParallelEnumerable.Range(0, count).Contains(count, new ModularCongruenceComparer(count + 1)));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Contains_NoMatching_Longrunning(int count)
        {
            Contains_NoMatching(count);
        }

        [Theory]
        [InlineData(16)]
        public static void Contains_MultipleMatching(int count)
        {
            Assert.True(ParallelEnumerable.Range(0, count).Contains(count, new ModularCongruenceComparer(2)));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Contains_MultipleMatching_Longrunning(int count)
        {
            Contains_MultipleMatching(count);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 2, 16 })]
        public static void Contains_OneMatching(int count, int position)
        {
            Assert.True(ParallelEnumerable.Range(0, count).Contains(position));
            Assert.True(ParallelEnumerable.Range(0, count).Contains(position, null));
            Assert.True(ParallelEnumerable.Range(0, count).Contains(position, new ModularCongruenceComparer(count)));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void Contains_OneMatching_Longrunning(int count, int position)
        {
            Contains_OneMatching(count, position);
        }

        [Fact]
        public static void Contains_OperationCanceledException()
        {
            AssertThrows.EventuallyCanceled((source, canceler) => source.Contains(-1, new CancelingEqualityComparer<int>(canceler)));
        }

        [Fact]
        public static void Contains_AggregateException_Wraps_OperationCanceledException()
        {
            AssertThrows.OtherTokenCanceled((source, canceler) => source.Contains(-1, new CancelingEqualityComparer<int>(canceler)));
            AssertThrows.SameTokenNotCanceled((source, canceler) => source.Contains(-1, new CancelingEqualityComparer<int>(canceler)));
        }

        [Fact]
        public static void Contains_OperationCanceledException_PreCanceled()
        {
            AssertThrows.AlreadyCanceled(source => source.Contains(0));
            AssertThrows.AlreadyCanceled(source => source.Contains(0, EqualityComparer<int>.Default));
        }

        [Fact]
        public static void Contains_AggregateException()
        {
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 1).Contains(1, new FailingEqualityComparer<int>()));
        }

        [Fact]
        public static void Contains_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).Contains(false));
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).Contains(false, EqualityComparer<bool>.Default));
        }
    }
}
