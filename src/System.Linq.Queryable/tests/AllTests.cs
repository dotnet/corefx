// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Tests
{
    public class AllTests : EnumerableBasedTests
    {
        [Fact]
        public void PredicateTrueAllExceptLast()
        {
            int[] source = { 4, 2, 10, 12, 8, 6, 3 };

            Assert.False(source.AsQueryable().All(i => i % 2 == 0));
        }

        [Fact]
        public void NullSource()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IQueryable<int>)null).All(i => i != 0));
        }

        [Fact]
        public void NullPredicateUsed()
        {
            Expression<Func<int, bool>> predicate = null;
            Assert.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).AsQueryable().All(predicate));
        }

        [Fact]
        public void All()
        {
            var val = (new int[] { 0, 2, 1 }).AsQueryable().All(n => n > 1);
            Assert.False(val);
        }
    }
}
