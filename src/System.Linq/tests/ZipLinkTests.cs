// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ZipLinkTests : EnumerableTests
    {
        [Fact]
        public void ImplicitTypeParameters()
        {
            IEnumerable<int> source = new int[] { 1, 2, 3, 4 };
            IEnumerable<int> expected = new int[] { 3, 5, 7 };

            Assert.Equal(expected, source.ZipLink((x, y) => x + y));
        }
        
        [Fact]
        public void ExplicitTypeParameters()
        {
            IEnumerable<int> source = new int[] { 1, 2, 3, 4 };
            IEnumerable<int> expected = new int[] { 3, 5, 7 };

            Assert.Equal(expected, source.ZipLink<int, int>((x, y) => x + y));
        }

        [Fact]
        public void SourceIsNull()
        {
            IEnumerable<int> source = null;
            
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.ZipLink<int, int>((x, y) => x + y));
        }
                
        [Fact]
        public void FuncIsNull()
        {
            IEnumerable<int> source = new int[] { 1, 2, 3, 4 };
            Func<int, int, int> func = null;
            
            AssertExtensions.Throws<ArgumentNullException>("resultSelector", () => source.ZipLink(func));
        }

        [Fact]
        public void ExceptionThrownFromEnumerator()
        {
            ThrowsOnMatchEnumerable<int> source = new ThrowsOnMatchEnumerable<int>(new int[] { 1, 3, 3, 4 }, 2);
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { 4, 6, 7 };
            
            Assert.Equal(expected, source.ZipLink(func));

            source = new ThrowsOnMatchEnumerable<int>(new int[] { 1, 2, 3, 4 }, 2);
            
            var zipLink = source.ZipLink(func);
            
            Assert.Throws<Exception>(() => zipLink.ToList());
        }

        [Fact]
        public void SourceIsEmpty()
        {
            IEnumerable<int> source = new int[] { };
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { };

            Assert.Equal(expected, source.ZipLink(second, func));
        }

        [Fact]
        public void SourceIsSingle()
        {
            IEnumerable<int> source = new int[] { 1 };
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { };

            Assert.Equal(expected, source.ZipLink(second, func));
        }

        [Fact]
        public void SourceHasTwoElements()
        {
            IEnumerable<int> source = new int[] { 1, 2 };
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { 3 };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void SourceMany()
        {
            IEnumerable<int> source = new int[] { 1, 2, 3, 4 };
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { 3, 5, 7 };

            Assert.Equal(expected, source.ZipLink(func));
        }
        
        [Fact]
        public void DelegateFuncChanged()
        {
            IEnumerable<int> source = new int[] { 1, 2, 4, 8 };
            Func<int, int, int> func = (x, y) => x + y;
            IEnumerable<int> expected = new int[] { 3, 6, 12 };

            Assert.Equal(expected, source.ZipLink(func));

            func = (x, y) => x - y;
            expected = new int[] { -1, -2, -4 };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void LambdaFuncChanged()
        {
            IEnumerable<int> source = new int[] { 1, 2, 4, 8 };
            IEnumerable<int> expected = new int[] { 3, 6, 12 };

            Assert.Equal(expected, source.ZipLink((x, y) => x + y));

            expected = new int[] { -1, -2, -4 };

            Assert.Equal(expected, source.ZipLink((x, y) => x - y));
        }

        [Fact]
        public void SourceHasFirstElementNull()
        {
            IEnumerable<int?> source = new[] { (int?)null, 2, 3, 4, 5 };
            Func<int?, int?, int?> func = (x, y) => x + y;
            IEnumerable<int?> expected = new int?[] { null, 5, 7, 9 };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void SourceHasMiddleNullValue()
        {
            IEnumerable<int?> source = new[] { 1, 2, (int?)null, 4, 5 };
            Func<int?, int?, int?> func = (x, y) => x + y;
            IEnumerable<int?> expected = new int?[] { 3, null, null, 9 };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void SourceHasLastElementNull()
        {
            IEnumerable<int?> source = new[] { 1, 2, 3, 4, (int?)null };
            Func<int?, int?, int?> func = (x, y) => x + y;
            IEnumerable<int?> expected = new int?[] { 3, 5, 7, null };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void SourceAllElementsNull()
        {
            IEnumerable<int?> source = new int?[] { null, null, null, null };
            Func<int?, int?, int?> func = (x, y) => x + y;
            IEnumerable<int?> expected = new int?[] { null, null, null };

            Assert.Equal(expected, source.ZipLink(func));
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var iterator = NumberRangeGuaranteedNotCollectionType(0, 3).ZipLink((x, y) => x + y);
            // Don't insist on this behaviour, but check it's correct if it happens
            var en = iterator as IEnumerator<int>;
            Assert.False(en != null && en.MoveNext());
        }

        [Fact]
        public void RunOnce()
        {
            IEnumerable<int?> source = new[] { 1, 2, (int?)null, 4, 5 };
            Func<int?, int?, int?> func = (x, y) => x + y;
            IEnumerable<int?> expected = new int?[] { 3, null, null, 9 };

            Assert.Equal(expected, source.RunOnce().ZipLink(func));
        }
    }
}
