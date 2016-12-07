// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class TryLastTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                             where x > Int32.MinValue
                             select x;

            int r1, r2;
            Assert.Equal(q.TryLast(out r1), q.TryLast(out r2));
            Assert.Equal(r1, r2);
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", String.Empty }
                             where !String.IsNullOrEmpty(x)
                             select x;

            string r1, r2;
            Assert.Equal(q.TryLast(out r1), q.TryLast(out r2));
            Assert.Equal(r1, r2);
        }

        private static void TestEmptyIList<T>()
        {
            T[] source = { };
            T expected = default(T);
            
            Assert.IsAssignableFrom<IList<T>>(source);

            T r1;
            Assert.Equal(false, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyIListT()
        {
            TestEmptyIList<int>();
            TestEmptyIList<string>();
            TestEmptyIList<DateTime>();
            TestEmptyIList<TryLastTests>();
        }

        [Fact]
        public void IListTOneElement()
        {
            int[] source = { 5 };
            int expected = 5;

            Assert.IsAssignableFrom<IList<int>>(source);

            int r1;
            Assert.Equal(true, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }


        [Fact]
        public void IListTManyElementsLastIsDefault()
        {
            int?[] source = { -10, 2, 4, 3, 0, 2, null };
            int? expected = null;

            Assert.IsAssignableFrom<IList<int?>>(source);

            int? r1;
            Assert.Equal(true, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void IListTManyElementsLastIsNotDefault()
        {
            int?[] source = { -10, 2, 4, 3, 0, 2, null, 19 };
            int? expected = 19;

            Assert.IsAssignableFrom<IList<int?>>(source);

            int? r1;
            Assert.Equal(true, source.TryLast(out r1));
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
            Assert.Equal(false, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyNotIListT()
        {
            TestEmptyNotIList<int>();
            TestEmptyNotIList<string>();
            TestEmptyNotIList<DateTime>();
            TestEmptyNotIList<TryLastTests>();
        }

        [Fact]
        public void OneElementNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(-5, 1);
            int expected = -5;

            Assert.Null(source as IList<int>);

            int r1;
            Assert.Equal(true, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsNotIListT()
        {
            IEnumerable<int> source = NumberRangeGuaranteedNotCollectionType(3, 10);
            int expected = 12;

            Assert.Null(source as IList<int>);

            int r1;
            Assert.Equal(true, source.TryLast(out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyIListSource()
        {
            int?[] source = { };

            int? r1;
            Assert.Equal(false, source.TryLast(x => true, out r1));
            Assert.Null(r1);
            Assert.Equal(false, source.TryLast(x => false, out r1));
            Assert.Null(r1);
        }

        [Fact]
        public void OneElementIListTruePredicate()
        {
            int[] source = { 4 };
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsIListPredicateFalseForAll()
        {
            int[] source = { 9, 5, 1, 3, 17, 21 };
            Func<int, bool> predicate = IsEven;
            int expected = default(int);

            int r1;
            Assert.Equal(false, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void IListPredicateTrueOnlyForLast()
        {
            int[] source = { 9, 5, 1, 3, 17, 21, 50 };
            Func<int, bool> predicate = IsEven;
            int expected = 50;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void IListPredicateTrueForSome()
        {
            int[] source = { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 };
            Func<int, bool> predicate = IsEven;
            int expected = 18;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void EmptyNotIListSource()
        {
            IEnumerable<int?> source = Enumerable.Repeat((int?)4, 0);

            int? r1;
            Assert.Equal(false, source.TryLast(x => true, out r1));
            Assert.Null(r1);
            Assert.Equal(false, source.TryLast(x => false, out r1));
            Assert.Null(r1);
        }

        [Fact]
        public void OneElementNotIListTruePredicate()
        {
            IEnumerable<int> source = ForceNotCollection(new[] { 4 });
            Func<int, bool> predicate = IsEven;
            int expected = 4;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void ManyElementsNotIListPredicateFalseForAll()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 9, 5, 1, 3, 17, 21 });
            Func<int, bool> predicate = IsEven;
            int expected = default(int);

            int r1;
            Assert.Equal(false, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void NotIListPredicateTrueOnlyForLast()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 9, 5, 1, 3, 17, 21, 50 });
            Func<int, bool> predicate = IsEven;
            int expected = 50;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void NotIListPredicateTrueForSome()
        {
            IEnumerable<int> source = ForceNotCollection(new int[] { 3, 7, 10, 7, 9, 2, 11, 18, 13, 9 });
            Func<int, bool> predicate = IsEven;
            int expected = 18;

            int r1;
            Assert.Equal(true, source.TryLast(IsEven, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void NullSource()
        {
            int r1;
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryLast(out r1));
        }

        [Fact]
        public void NullSourcePredicateUsed()
        {
            int r1;
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryLast(i => i != 2, out r1));
        }

        [Fact]
        public void NullPredicate()
        {
            Func<int, bool> predicate = null;

            int r1;
            Assert.Throws<ArgumentNullException>("predicate", () => Enumerable.Range(0, 3).TryLast(predicate, out r1));
        }
    }
}
