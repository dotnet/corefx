// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class AnyTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > Int32.MinValue
                    select x;

            Func<int, bool> predicate = IsEven; 

            Assert.Equal(q.Any(predicate), q.Any(predicate));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                    select x;

            Func<string, bool> predicate = String.IsNullOrEmpty;

            Assert.Equal(q.Any(predicate), q.Any(predicate));
        }
        
        [Fact]
        public void NoPredicateEmptySource()
        {
            int[] source = { };
            
            Assert.False(source.Any());
        }

        [Fact]
        public void NoPredicateSingleElement()
        {
            int[] source = { 3 };

            Assert.True(source.Any());
        }

        [Fact]
        public void NoPredicateNullElements()
        {
            int?[] source = { null, null, null, null };

            Assert.True(source.Any());
        }

        [Fact]
        public void PredicateEmptySource()
        {
            int[] source = { };
            
            Assert.False(source.Any(IsEven));
        }

        [Fact]
        public void OneElementPredicateTrue()
        {
            int[] source = { 4 };
            
            Assert.True(source.Any(IsEven));
        }

        [Fact]
        public void OneElementPredicateFalse()
        {
            int[] source = { 5 };

            Assert.False(source.Any(IsEven));
        }

        [Fact]
        public void OnlyLastTrue()
        {
            int[] source = { 5, 9, 3, 7, 4 };
            
            Assert.True(source.Any(IsEven));
        }

        [Fact]
        public void OnlyOneTrue()
        {
            int[] source = { 5, 8, 9, 3, 7, 11 };
            
            Assert.True(source.Any(IsEven));
        }

        [Fact]
        public void RangeWithinRange()
        {
            var array = Enumerable.Range(1, 10).ToArray();
            for (var j = 0; j <= 9; j++)
                Assert.True(array.Any(i => i > j));
            Assert.False(array.Any(i => i > 10));
        }

        [Fact]
        public void NullSource()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).Any());
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).Any(i => i != 0));
        }

        [Fact]
        public void NullPredicateUsed()
        {
            Func<int, bool> predicate = null;
            Assert.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).Any(predicate));
        }
    }
}
