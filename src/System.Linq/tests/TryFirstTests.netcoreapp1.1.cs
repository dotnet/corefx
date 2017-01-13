// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class TryFirstTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            IEnumerable<int> ieInt = Enumerable.Range(0, 0);
            var q = from x in ieInt
                    select x;

            int r1, r2;
            Assert.Equal(q.TryFirst(out r1), q.TryFirst(out r2));
            Assert.Equal(r1, r2);
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                    where !String.IsNullOrEmpty(x)
                    select x;

            string r1, r2;
            Assert.Equal(q.TryFirst(out r1), q.TryFirst(out r2));
            Assert.Equal(r1, r2);
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = { };
            T expected = default(T);
            
            Assert.IsAssignableFrom<IList<T>>(source);

            T r1;
            Assert.Equal(false, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<TryFirstTests>();
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = { 5 };
            int expected = 5;

            Assert.IsAssignableFrom<IList<int>>(source);

            int r1;
            Assert.Equal(true, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void IListTManyElementsFirstIsDefault()
        {
            int?[] source = { null, -10, 2, 4, 3, 0, 2 };
            int? expected = null;

            Assert.IsAssignableFrom<IList<int?>>(source);

            int? r1;
            Assert.Equal(true, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void IListTManyElementsFirstIsNotDefault()
        {
            int?[] source = { 19, null, -10, 2, 4, 3, 0, 2 };
            int? expected = 19;

            Assert.IsAssignableFrom<IList<int?>>(source);

            int? r1;
            Assert.Equal(true, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        private static IEnumerable<T> EmptySource<T>()
        {
            yield break;
        }

        private static void TestEmptyNotIList<T>()
        {
            var source = EmptySource<T>();
            T expected = default(T);
            
            Assert.Null(source as IList<T>);

            T r1;
            Assert.Equal(false, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyNotIListT()
        {
            TestEmptyNotIList<int>();
            TestEmptyNotIList<string>();
            TestEmptyNotIList<DateTime>();
            TestEmptyNotIList<TryFirstTests>();
        }

        [Fact]
        public void OneElementNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-5, 1);
            int expected = -5;

            Assert.Null(source as IList<int>);

            int r1;
            Assert.Equal(true, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(3, 10);
            int expected = 3;

            Assert.Null(source as IList<int>);

            int r1;
            Assert.Equal(true, source.TryFirst(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptySource()
        {
            int?[] source = { };

            int? r1;
            Assert.Equal(false, source.TryFirst(x => true, out r1));
            Assert.Null(r1);
            Assert.Equal(false, source.TryFirst(x => false, out r1));
            Assert.Null(r1);
        }

        [Fact]
        public void OneElementTruePredicate()
        {
            int[] source = { 4 };
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            int r1;
            Assert.Equal(true, source.TryFirst(predicate, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsPredicateFalseForAll()
        {
            int[] source = { 9, 5, 1, 3, 17, 21 };
            Func<int, bool> predicate = IsEven;
            int expected = default(int);

            int r1;
            Assert.Equal(false, source.TryFirst(predicate, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void PredicateTrueOnlyForLast()
        {
            int[] source = { 9, 5, 1, 3, 17, 21, 50 };
            Func<int, bool> predicate = IsEven;
            int expected = 50;

            int r1;
            Assert.Equal(true, source.TryFirst(predicate, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void PredicateTrueForSome()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 17, 13, 8 };
            Func<int, bool> predicate = IsEven;
            int expected = 10;

            int r1;
            Assert.Equal(true, source.TryFirst(predicate, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void NullSource()
        {
            int r1;
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryFirst(out r1));
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            int r1;
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryFirst(i => i != 2, out r1));
        }

        [Fact]
        public void NullPredicate()
        {
            Func<int, bool> predicate = null;

            int r1;
            Assert.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).TryFirst(predicate, out r1));
        }
    }
}
