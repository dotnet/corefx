// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Tests
{
    public class TakeWhileTests : EnumerableBasedTests
    {
        [Fact]
        public void SourceNonEmptyPredicateTrueSomeFalseSecond()
        {
            int[] source = { 8, 3, 12, 4, 6, 10 };
            int[] expected = { 8 };

            Assert.Equal(expected, source.AsQueryable().TakeWhile(x => x % 2 == 0));
        }

        [Fact]
        public void SourceNonEmptyPredicateTrueSomeFalseSecondWithIndex()
        {
            int[] source = { 8, 3, 12, 4, 6, 10 };
            int[] expected = { 8 };

            Assert.Equal(expected, source.AsQueryable().TakeWhile((x, i) => x % 2 == 0));
        }

        [Fact]
        public void ThrowsOnNullSource()
        {
            IQueryable<int> source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.TakeWhile(x => true));
        }

        [Fact]
        public void ThrowsOnNullPredicate()
        {
            IQueryable<int> source = new[] { 1, 2, 3 }.AsQueryable();
            Expression<Func<int, bool>> nullPredicate = null;
            Assert.Throws<ArgumentNullException>("predicate", () => source.TakeWhile(nullPredicate));
        }

        [Fact]
        public void ThrowsOnNullSourceIndexed()
        {
            IQueryable<int> source = null;
            Assert.Throws<ArgumentNullException>("source", () => source.TakeWhile((x, i) => true));
        }

        [Fact]
        public void ThrowsOnNullPredicateIndexed()
        {
            IQueryable<int> source = new[] { 1, 2, 3 }.AsQueryable();
            Expression<Func<int, int, bool>> nullPredicate = null;
            Assert.Throws<ArgumentNullException>("predicate", () => source.TakeWhile(nullPredicate));
        }

        [Fact]
        public void TakeWhile1()
        {
            var count = (new int[] { 0, 1, 2 }).AsQueryable().TakeWhile(n => n < 2).Count();
            Assert.Equal(2, count);
        }

        [Fact]
        public void TakeWhile2()
        {
            var count = (new int[] { 0, 1, 2 }).AsQueryable().TakeWhile((n, i) => n + i < 4).Count();
            Assert.Equal(2, count);
        }
    }
}