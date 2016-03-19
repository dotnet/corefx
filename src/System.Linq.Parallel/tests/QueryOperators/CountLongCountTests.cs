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
            foreach (int count in counts)
            {
                foreach (int position in new[] { 0, count / 2, Math.Max(0, count - 1) }.Distinct())
                {
                    yield return new object[] { count, position };
                }
            }
        }

        //
        // Count and LongCount
        //
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Count_All(int count)
        {
            Assert.Equal(count, ParallelEnumerable.Range(0, count).Count());
            Assert.Equal(count, ParallelEnumerable.Range(0, count).Count(i => i < count));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Count_All_Longrunning(int count)
        {
            Count_All(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void LongCount_All(int count)
        {
            Assert.Equal(count, ParallelEnumerable.Range(0, count).LongCount());
            Assert.Equal(count, ParallelEnumerable.Range(0, count).LongCount(i => i < count));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void LongCount_All_Longrunning(int count)
        {
            LongCount_All(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void Count_None(int count)
        {
            Assert.Equal(0, ParallelEnumerable.Range(0, count).Count(i => i == -1));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void Count_None_Longrunning(int count)
        {
            Count_None(count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(16)]
        public static void LongCount_None(int count)
        {
            Assert.Equal(0, ParallelEnumerable.Range(0, count).LongCount(i => i == -1));
        }

        [Theory]
        [OuterLoop]
        [InlineData(1024 * 1024)]
        [InlineData(1024 * 1024 * 4)]
        public static void LongCount_None_Longrunning(int count)
        {
            LongCount_None(count);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 0, 1, 2, 16 })]
        public static void Count_One(int count, int position)
        {
            Assert.Equal(Math.Min(1, count), ParallelEnumerable.Range(0, count).Count(i => i == position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void Count_One_Longrunning(int count, int position)
        {
            Count_One(count, position);
        }

        [Theory]
        [MemberData(nameof(OnlyOneData), new[] { 0, 1, 2, 16 })]
        public static void LongCount_One(int count, long position)
        {
            Assert.Equal(Math.Min(1, count), ParallelEnumerable.Range(0, count).LongCount(i => i == position));
        }

        [Theory]
        [OuterLoop]
        [MemberData(nameof(OnlyOneData), new[] { 1024 * 1024, 1024 * 1024 * 4 })]
        public static void LongCount_One_Longrunning(int count, long position)
        {
            LongCount_One(count, position);
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

        [Fact]
        public static void CountLongCount_AggregateException()
        {
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 1).Count(x => { throw new DeliberateTestException(); }));
            AssertThrows.Wrapped<DeliberateTestException>(() => ParallelEnumerable.Range(0, 1).LongCount(x => { throw new DeliberateTestException(); }));
        }

        [Fact]
        public static void CountLongCount_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).Count());
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).Count(x => x));
            Assert.Throws<ArgumentNullException>("predicate", () => ParallelEnumerable.Empty<bool>().Count(null));

            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).LongCount());
            Assert.Throws<ArgumentNullException>("source", () => ((ParallelQuery<bool>)null).LongCount(x => x));
            Assert.Throws<ArgumentNullException>("predicate", () => ParallelEnumerable.Empty<bool>().LongCount(null));
        }
    }
}
