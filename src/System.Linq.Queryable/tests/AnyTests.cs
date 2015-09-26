// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Tests
{
    public class AnyTests : EnumerableBasedTests
    {
        [Fact]
        public void NoPredicateNullElements()
        {
            int?[] source = { null, null, null, null };

            Assert.True(source.AsQueryable().Any());
        }

        [Fact]
        public void OnlyLastTrue()
        {
            int[] source = { 5, 9, 3, 7, 4 };
            
            Assert.True(source.AsQueryable().Any(i => i % 2 == 0));
        }

        [Fact]
        public void RangeWithinRange()
        {
            var array = Enumerable.Range(1, 10).ToArray();
            for (var j = 0; j <= 9; j++)
                Assert.True(array.AsQueryable().Any(i => i > j));
            Assert.False(array.AsQueryable().Any(i => i > 10));
        }

        [Fact]
        public void NullSource()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IQueryable<int>)null).Any());
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IQueryable<int>)null).Any(i => i != 0));
        }

        [Fact]
        public void NullPredicateUsed()
        {
            Expression<Func<int, bool>> predicate = null;
            Assert.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).AsQueryable().Any(predicate));
        }

        [Fact]
        public void Any1()
        {
            var val = (new int[] { 0, 2, 1 }).AsQueryable().Any();
            Assert.True(val);
        }

        [Fact]
        public void Any2()
        {
            var val = (new int[] { 0, 2, 1 }).AsQueryable().Any(n => n > 1);
            Assert.True(val);
        }
    }
}
