// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class OrderByTests : EnumerableTests
    {
        private class BadComparer1 : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return 1;
            }
        }

        private class BadComparer2 : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return -1;
            }
        }

        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x1 in new int[] { 1, 6, 0, -1, 3 }
                             from x2 in new int[] { 55, 49, 9, -100, 24, 25 }
                             select new { a1 = x1, a2 = x2 };

            Assert.Equal(q.OrderBy(e => e.a1).ThenBy(f => f.a2), q.OrderBy(e => e.a1).ThenBy(f => f.a2));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x1 in new[] { 55, 49, 9, -100, 24, 25, -1, 0 }
                             from x2 in new[] { "!@#$%^", "C", "AAA", "", null, "Calling Twice", "SoS", String.Empty }
                             where !String.IsNullOrEmpty(x2)
                             select new { a1 = x1, a2 = x2 };

            Assert.Equal(q.OrderBy(e => e.a1), q.OrderBy(e => e.a1));
        }

        [Fact]
        public void SourceEmpty()
        {
            int[] source = { };
            Assert.Empty(source.OrderBy(e => e));
        }

        [Fact]
        public void OrderedCount()
        {
            var source = Enumerable.Range(0, 20).Shuffle();
            Assert.Equal(20, source.OrderBy(i => i).Count());
        }

        //FIXME: This will hang with a larger source. Do we want to deal with that case?
        [Fact]
        public void SurviveBadComparerAlwaysReturnsNegative()
        {
            int[] source = { 1 };
            int[] expected = { 1 };

            Assert.Equal(expected, source.OrderBy(e => e, new BadComparer2()));
        }

        [Fact]
        public void KeySelectorReturnsNull()
        {
            int?[] source = { null, null, null };
            int?[] expected = { null, null, null };

            Assert.Equal(expected, source.OrderBy(e => e));
        }

        [Fact]
        public void ElementsAllSameKey()
        {
            int?[] source = { 9, 9, 9, 9, 9, 9 };
            int?[] expected = { 9, 9, 9, 9, 9, 9 };

            Assert.Equal(expected, source.OrderBy(e => e));
        }

        [Fact]
        public void KeySelectorCalled()
        {
            var source = new[]
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 45 },
                new { Name = "Prakash", Score = 99 }
            };
            var expected = new[]
            {
                new { Name = "Prakash", Score = 99 },
                new { Name = "Robert", Score = 45 },
                new { Name = "Tim", Score = 90 }
            };

            Assert.Equal(expected, source.OrderBy(e => e.Name, null));
        }

        [Fact]
        public void FirstAndLastAreDuplicatesCustomComparer()
        {
            string[] source = { "Prakash", "Alpha", "dan", "DAN", "Prakash" };
            string[] expected = { "Alpha", "dan", "DAN", "Prakash", "Prakash" };

            Assert.Equal(expected, source.OrderBy(e => e, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void RunOnce()
        {
            string[] source = { "Prakash", "Alpha", "dan", "DAN", "Prakash" };
            string[] expected = { "Alpha", "dan", "DAN", "Prakash", "Prakash" };

            Assert.Equal(expected, source.RunOnce().OrderBy(e => e, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void FirstAndLastAreDuplicatesNullPassedAsComparer()
        {
            int[] source = { 5, 1, 3, 2, 5 };
            int[] expected = { 1, 2, 3, 5, 5 };

            Assert.Equal(expected, source.OrderBy(e => e, null));
        }

        [Fact]
        public void SourceReverseOfResultNullPassedAsComparer()
        {
            int?[] source = { 100, 30, 9, 5, 0, -50, -75, null };
            int?[] expected = { null, -75, -50, 0, 5, 9, 30, 100 };

            Assert.Equal(expected, source.OrderBy(e => e, null));
        }

        [Fact]
        public void SameKeysVerifySortStable()
        {
            var source = new[]
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };
            var expected = new[]
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };

            Assert.Equal(expected, source.OrderBy(e => e.Score));
        }

        [Fact]
        public void OrderedToArray()
        {
            var source = new []
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };
            var expected = new []
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };

            Assert.Equal(expected, source.OrderBy(e => e.Score).ToArray());
        }

        [Fact]
        public void EmptyOrderedToArray()
        {
            Assert.Empty(Enumerable.Empty<int>().OrderBy(e => e).ToArray());
        }

        [Fact]
        public void OrderedToList()
        {
            var source = new[]
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };
            var expected = new[]
            {
                new { Name = "Tim", Score = 90 },
                new { Name = "Robert", Score = 90 },
                new { Name = "Prakash", Score = 90 },
                new { Name = "Jim", Score = 90 },
                new { Name = "John", Score = 90 },
                new { Name = "Albert", Score = 90 },
            };

            Assert.Equal(expected, source.OrderBy(e => e.Score).ToList());
        }

        [Fact]
        public void EmptyOrderedToList()
        {
            Assert.Empty(Enumerable.Empty<int>().OrderBy(e => e).ToList());
        }

        //FIXME: This will hang with a larger source. Do we want to deal with that case?
        [Fact]
        public void SurviveBadComparerAlwaysReturnsPositive()
        {
            int[] source = { 1 };
            int[] expected = { 1 };

            Assert.Equal(expected, source.OrderBy(e => e, new BadComparer1()));
        }

        private class ExtremeComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x == y)
                    return 0;
                if (x < y)
                    return int.MinValue;
                return int.MaxValue;
            }
        }

        [Fact]
        public void OrderByExtremeComparer()
        {
            var outOfOrder = new[] { 7, 1, 0, 9, 3, 5, 4, 2, 8, 6 };
            Assert.Equal(Enumerable.Range(0, 10), outOfOrder.OrderBy(i => i, new ExtremeComparer()));
        }

        [Fact]
        public void NullSource()
        {
            IEnumerable<int> source = null;
            AssertExtensions.Throws<ArgumentNullException>("source", () => source.OrderBy(i => i));
        }

        [Fact]
        public void NullKeySelector()
        {
            Func<DateTime, int> keySelector = null;
            AssertExtensions.Throws<ArgumentNullException>("keySelector", () => Enumerable.Empty<DateTime>().OrderBy(keySelector));
        }

        [Fact]
        public void FirstOnOrdered()
        {
            Assert.Equal(0, Enumerable.Range(0, 10).Shuffle().OrderBy(i => i).First());
            Assert.Equal(9, Enumerable.Range(0, 10).Shuffle().OrderByDescending(i => i).First());
            Assert.Equal(10, Enumerable.Range(0, 100).Shuffle().OrderByDescending(i => i.ToString().Length).ThenBy(i => i).First());
        }

        [Fact]
        public void FirstOnEmptyOrderedThrows()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().OrderBy(i => i).First());
        }

        [Fact]
        public void FirstOrDefaultOnOrdered()
        {
            Assert.Equal(0, Enumerable.Range(0, 10).Shuffle().OrderBy(i => i).FirstOrDefault());
            Assert.Equal(9, Enumerable.Range(0, 10).Shuffle().OrderByDescending(i => i).FirstOrDefault());
            Assert.Equal(10, Enumerable.Range(0, 100).Shuffle().OrderByDescending(i => i.ToString().Length).ThenBy(i => i).FirstOrDefault());
            Assert.Equal(0, Enumerable.Empty<int>().OrderBy(i => i).FirstOrDefault());
        }

        [Fact]
        public void LastOnOrdered()
        {
            Assert.Equal(9, Enumerable.Range(0, 10).Shuffle().OrderBy(i => i).Last());
            Assert.Equal(0, Enumerable.Range(0, 10).Shuffle().OrderByDescending(i => i).Last());
            Assert.Equal(10, Enumerable.Range(0, 100).Shuffle().OrderBy(i => i.ToString().Length).ThenByDescending(i => i).Last());
        }

        [Fact]
        public void LastOnOrderedMatchingCases()
        {
            object[] boxedInts = new object[] {0, 1, 2, 9, 1, 2, 3, 9, 4, 5, 7, 8, 9, 0, 1};
            Assert.Same(boxedInts[12], boxedInts.OrderBy(o => (int)o).Last());
            Assert.Same(boxedInts[12], boxedInts.OrderBy(o => (int)o).LastOrDefault());
            Assert.Same(boxedInts[12], boxedInts.OrderBy(o => (int)o).Last(o => (int)o % 2 == 1));
            Assert.Same(boxedInts[12], boxedInts.OrderBy(o => (int)o).LastOrDefault(o => (int)o % 2 == 1));
        }

        [Fact]
        public void LastOnEmptyOrderedThrows()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().OrderBy(i => i).Last());
        }

        [Fact]
        public void LastOrDefaultOnOrdered()
        {
            Assert.Equal(9, Enumerable.Range(0, 10).Shuffle().OrderBy(i => i).LastOrDefault());
            Assert.Equal(0, Enumerable.Range(0, 10).Shuffle().OrderByDescending(i => i).LastOrDefault());
            Assert.Equal(10, Enumerable.Range(0, 100).Shuffle().OrderBy(i => i.ToString().Length).ThenByDescending(i => i).LastOrDefault());
            Assert.Equal(0, Enumerable.Empty<int>().OrderBy(i => i).LastOrDefault());
        }

        [Fact]
        public void EnumeratorDoesntContinue()
        {
            var enumerator = NumberRangeGuaranteedNotCollectionType(0, 3).Shuffle().OrderBy(i => i).GetEnumerator();
            while (enumerator.MoveNext()) { }
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void OrderByIsCovariantTestWithCast()
        {
            var ordered = Enumerable.Range(0, 100).Select(i => i.ToString()).OrderBy(i => i.Length);
            IOrderedEnumerable<IComparable> covariantOrdered = ordered;
            covariantOrdered = covariantOrdered.ThenBy(i => i);
            string[] expected =
                Enumerable.Range(0, 100).Select(i => i.ToString()).OrderBy(i => i.Length).ThenBy(i => i).ToArray();
            Assert.Equal(expected, covariantOrdered);
        }

        [Fact]
        public void OrderByIsCovariantTestWithAssignToArgument()
        {
            var ordered = Enumerable.Range(0, 100).Select(i => i.ToString()).OrderBy(i => i.Length);
            IOrderedEnumerable<IComparable> covariantOrdered = ordered.ThenByDescending<IComparable, IComparable>(i => i);
            string[] expected = Enumerable.Range(0, 100)
                .Select(i => i.ToString())
                .OrderBy(i => i.Length)
                .ThenByDescending(i => i)
                .ToArray();
            Assert.Equal(expected, covariantOrdered);
        }

        [Fact]
        public void CanObtainFromCovariantIOrderedQueryable()
        {
            // If an ordered queryable is cast covariantly and then has ThenBy() called on it,
            // it depends on IOrderedEnumerable<TElement> also being covariant to allow for
            // that ThenBy() to be processed within Linq-to-objects, as otherwise there is no
            // equivalent ThenBy() overload to translate the call to.

            IOrderedQueryable<IComparable> ordered =
                Enumerable.Range(0, 100).AsQueryable().Select(i => i.ToString()).OrderBy(i => i.Length);
            ordered = ordered.ThenBy(i => i);
            string[] expected =
                Enumerable.Range(0, 100).Select(i => i.ToString()).OrderBy(i => i.Length).ThenBy(i => i).ToArray();
            Assert.Equal(expected, ordered);
        }
    }
}
