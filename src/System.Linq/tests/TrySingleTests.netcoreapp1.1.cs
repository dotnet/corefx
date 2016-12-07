// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class TrySingleTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 0.12335f }
                    select x;

            int r1, r2;
            Assert.Equal(q.TrySingle(out r1), q.TrySingle(out r2));
            Assert.Equal(r1, r2);
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "" }
                    select x;

            string r1, r2;
            Assert.Equal(q.TrySingle(out r1), q.TrySingle(out r2));
            Assert.Equal(r1, r2);
        }

        [Fact]
        public void EmptyIList()
        {
            int?[] source = { };
            int? expected = null;

            int? r1;
            Assert.Equal(false, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void SingleElementIList()
        {
            int[] source = { 4 };
            int expected = 4;

            int? r1;
            Assert.Equal(true, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementIList()
        {
            int[] source = { 4, 4, 4, 4, 4 };
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyNotIList()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(0, 0);
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void SingleElementNotIList()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(-5, 1);
            int expected = -5;

            int r1;
            Assert.Equal(true, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementNotIList()
        {
            IEnumerable<int> source = RepeatedNumberGuaranteedNotCollectionType(3, 5);
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptySourceWithPredicate()
        {
            int[] source = { };
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void SingleElementPredicateTrue()
        {
            int[] source = { 4 };
            int expected = 4;

            int r1;
            Assert.Equal(true, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void SingleElementPredicateFalse()
        {
            int[] source = { 3 };
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsPredicateFalseForAll()
        {
            int[] source = { 3, 1, 7, 9, 13, 19 };
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsPredicateTrueForLast()
        {
            int[] source = { 3, 1, 7, 9, 13, 19, 20 };
            int expected = 20;

            int r1;
            Assert.Equal(true, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsPredicateTrueForFirstAndFifth()
        {
            int[] source = { 2, 3, 1, 7, 10, 13, 19, 9 };
            int expected = default(int);

            int r1;
            Assert.Equal(false, q.TrySingle(i => i % 2 == 0, out r1));
            Assert.Equal(expected, r1);
        }

        [Theory]
        [InlineData(true, 1, 100)]
        [InlineData(true, 42, 100)]
        public void FindSingleMatch(bool found, int target, int range)
        {
            int r1;
            Assert.Equal(found, Enumerable.Range(0, range).TrySingle(i => i == target, out r1));
            Assert.Equal(target, r1);
        }

        [Fact]
        public void ThrowsOnNullSource()
        {
            int[] source = null;

            int r1;
            Assert.Throws<ArgumentNullException>("source", () => source.TrySingle(out r1));
            Assert.Throws<ArgumentNullException>("source", () => source.TrySingle(i => i % 2 == 0, out r1));
        }

        [Fact]
        public void ThrowsOnNullPredicate()
        {
            int[] source = { };
            Func<int, bool> nullPredicate = null;

            int r1;
            Assert.Throws<ArgumentNullException>("predicate", () => source.TrySingle(nullPredicate, out r1));
        }
    }
}
