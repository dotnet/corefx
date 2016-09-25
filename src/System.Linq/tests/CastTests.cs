// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace System.Linq.Tests
{
    public class CastTests : EnumerableTests
    {
        private static List<Func<IEnumerable<T>, IEnumerable<T>>> IdentityTransforms<T>()
        {
            return new List<Func<IEnumerable<T>, IEnumerable<T>>>
            {
                e => e,
                e => e.ToArray(), // well-known ICollection
                e => e.ToList(), // well-known ICollection
                e => new Queue<T>(e), // not-well-known ICollection
                e => e.Select(i => i), // IPartition
                e => !e.Any() ? e : e.Skip(1).Prepend(e.First()) // IIListProvider, non-IPartition
            };
        }

        [Fact]
        public void CastIntToLongThrows()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > Int32.MinValue
                    select x;

            var rst = q.Cast<long>();

            Assert.Throws<InvalidCastException>(() => { foreach (var t in rst) ; });
        }

        [Fact]
        public void CastByteToUShortThrows()
        {
            var q = from x in new byte[] { 0, 255, 127, 128, 1, 33, 99 }
                    select x;

            var rst = q.Cast<ushort>();
            Assert.Throws<InvalidCastException>(() => { foreach (var t in rst) ; });
        }

        [Fact]
        public void EmptySource()
        {
            object[] source = { };
            Assert.Empty(source.Cast<int>());
        }

        [Fact]
        public void NullableIntFromAppropriateObjects()
        {
            int? i = 10;
            object[] source = { -4, 1, 2, 3, 9, i };
            int?[] expected = { -4, 1, 2, 3, 9, i };

            Assert.Equal(expected, source.Cast<int?>());
        }
        
        [Fact]
        public void LongFromNullableIntInObjectsThrows()
        {
            int? i = 10;
            object[] source = { -4, 1, 2, 3, 9, i };

            IEnumerable<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void LongFromNullableIntInObjectsIncludingNullThrows()
        {
            int? i = 10;
            object[] source = { -4, 1, 2, 3, 9, null, i };

            IEnumerable<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void NullableIntFromAppropriateObjectsIncludingNull()
        {
            int? i = 10;
            object[] source = { -4, 1, 2, 3, 9, null, i };
            int?[] expected = { -4, 1, 2, 3, 9, null, i };

            Assert.Equal(expected, source.Cast<int?>());
        }

        [Fact]
        public void ThrowOnUncastableItem()
        {
            object[] source = { -4, 1, 2, 3, 9, "45" };
            int[] expectedBeginning = { -4, 1, 2, 3, 9 };

            IEnumerable<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
            Assert.Equal(expectedBeginning, cast.Take(5));
            Assert.Throws<InvalidCastException>(() => cast.ElementAt(5));
        }

        [Fact]
        public void ThrowCastingIntToDouble()
        {
            int[] source = new int[] { -4, 1, 2, 9 };

            IEnumerable<double> cast = source.Cast<double>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }
        
        private static void TestCastThrow<T>(object o)
        {
            byte? i = 10;
            object[] source = { -1, 0, o, i };

            IEnumerable<T> cast = source.Cast<T>();

            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowOnHeterogenousSource()
        {
            TestCastThrow<long?>(null);
            TestCastThrow<long>(9L);
        }

        [Fact]
        public void CastToString()
        {
            object[] source = { "Test1", "4.5", null, "Test2" };
            string[] expected = { "Test1", "4.5", null, "Test2" };

            Assert.Equal(expected, source.Cast<string>());
        }

        [Fact]
        public void ArrayConversionThrows()
        {
            Assert.Throws<InvalidCastException>(() => new[] { -4 }.Cast<long>().ToList());
        }

        [Fact]
        public void FirstElementInvalidForCast()
        {
            object[] source = { "Test", 3, 5, 10 };

            IEnumerable<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void LastElementInvalidForCast()
        {
            object[] source = { -5, 9, 0, 5, 9, "Test" };

            IEnumerable<int> cast = source.Cast<int>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void NullableIntFromNullsAndInts()
        {
            object[] source = { 3, null, 5, -4, 0, null, 9 };
            int?[] expected = { 3, null, 5, -4, 0, null, 9 };

            Assert.Equal(expected, source.Cast<int?>());
        }

        [Fact]
        public void ThrowCastingIntToLong()
        {
            int[] source = new int[] { -4, 1, 2, 3, 9 };

            IEnumerable<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingIntToNullableLong()
        {
            int[] source = new int[] { -4, 1, 2, 3, 9 };

            IEnumerable<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingNullableIntToLong()
        {
            int?[] source = new int?[] { -4, 1, 2, 3, 9 };

            IEnumerable<long> cast = source.Cast<long>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void ThrowCastingNullableIntToNullableLong()
        {
            int?[] source = new int?[] { -4, 1, 2, 3, 9, null };

            IEnumerable<long?> cast = source.Cast<long?>();
            Assert.Throws<InvalidCastException>(() => cast.ToList());
        }

        [Fact]
        public void CastingNullToNonnullableIsNullReferenceException()
        {
            int?[] source = new int?[] { -4, 1, null, 3 };
            IEnumerable<int> cast = source.Cast<int>();
            Assert.Throws<NullReferenceException>(() => cast.ToList());
        }

        [Fact]
        public void NullSource()
        {
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<object>)null).Cast<string>());
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var iterator = new object[0].Where(i => i != null).Cast<string>();
            // Don't insist on this behaviour, but check it's correct if it happens
            var en = iterator as IEnumerator<string>;
            Assert.False(en != null && en.MoveNext());
        }

        [Fact]
        public void CastDisposeWorksForDisposableAndNonDisposableNonGenericIEnumerators()
        {
            int state1 = 0;
            var disposable = new DisposableDelegateBasedNonGenericEnumerator
            {
                MoveNextWorker = () => false, // Enumerator represents a collection w/ 0 elements.
                DisposeWorker = () => state1++
            };

            var nonDisposable = new DelegateBasedNonGenericEnumerator();

            var enumerable = new DelegateBasedNonGenericEnumerable { GetEnumeratorWorker = () => disposable };

            using (var e = enumerable.Cast<object>().GetEnumerator())
            {
                Assert.Equal(0, state1);

                // Once we reach the end of the sequence, we should call Dispose if
                // the non-generic IEnumerator implements IDisposable.
                e.MoveNext();
                Assert.Equal(1, state1);

                // We shouldn't call it twice.
                e.MoveNext();
                Assert.Equal(1, state1);
            }

            enumerable.GetEnumeratorWorker = () => nonDisposable;

            using (var e = enumerable.Cast<object>().GetEnumerator())
            {
                // If the IEnumerator does not implement IDisposable, things should still work correctly.
                e.MoveNext();
                e.MoveNext();
            }
        }

        [Theory]
        [MemberData(nameof(InvalidCastOptimizationWithAtLeastOneNonStringData))]
        public void CountIsNotInvalidlyOptimized(IEnumerable source)
        {
            // If not all of the items in a collection are of type T, then
            // optimizing collection.Cast<T>().Count() would be invalid.

            Assert.Throws<InvalidCastException>(() => source.Cast<string>().Count());
        }

        [Theory]
        [MemberData(nameof(InvalidCastOptimizationWithAtLeastOneNonStringData))]
        public void SkipIsNotInvalidlyOptimized(IEnumerable source)
        {
            // Can't skip over non-Ts.

            object[] sourceArray = source.Cast<object>().ToArray();
            int lastNonStringIndex = Array.FindLastIndex(sourceArray, obj => !(obj is string));

            Assert.Throws<InvalidCastException>(() =>
            {
                foreach (string item in source.Cast<string>().Skip(lastNonStringIndex + 1)) { }
            });
        }

        [Theory]
        [MemberData(nameof(InvalidCastOptimizationWithAtLeastOneNonStringData))]
        public void ElementAtIsNotInvalidlyOptimized(IEnumerable source)
        {
            object[] sourceArray = source.Cast<object>().ToArray();
            int lastNonStringIndex = Array.FindLastIndex(sourceArray, obj => !(obj is string));
            
            if (lastNonStringIndex == source.Cast<object>().Count() - 1)
            {
                // Add a string at the end.
                source = source.Cast<object>().Append(string.Empty);
            }

            Assert.Throws<InvalidCastException>(() => source.Cast<string>().ElementAt(lastNonStringIndex + 1));
        }

        [Theory]
        [MemberData(nameof(InvalidCastOptimizationWithAtLeastOneNonStringData))]
        public void LastIsNotInvalidlyOptimized(IEnumerable source)
        {
            if (source.Cast<object>().Last() is string)
            {
                Assert.Throws<InvalidCastException>(() => source.Cast<string>().Last());
            }
            else
            {
                source = source.Cast<object>().Prepend(3).Append(string.Empty);

                Assert.Throws<InvalidCastException>(() => source.Cast<string>().Last());
            }
        }

        [Theory]
        [MemberData(nameof(InvalidCastOptimizationWithAtLeastOneNonStringData))]
        public void InvalidCastToArrayToList(IEnumerable source)
        {
            Assert.Throws<InvalidCastException>(() => source.Cast<string>().ToArray());
            Assert.Throws<InvalidCastException>(() => source.Cast<string>().ToList());
        }

        public static IEnumerable<object[]> InvalidCastOptimizationWithAtLeastOneNonStringData()
        {
            var sources = new List<IEnumerable<object>>
            {
                new object[] { 3, "foo", new object() },
                Enumerable.Repeat<object>("foo", 37).Append(3),
                Enumerable.Range(1, 3).Cast<object>(),
                Enumerable.Repeat(new object(), 1010),
                Enumerable.Repeat<object>(1, 1).Append("foo").Append("foo"),
                new object[] { 3, null, null, "foo" },
                new object[] { null, 3 },
                new object[] { null, 3, null, 3 }.Reverse()
            };

            foreach (var equivalentSource in IdentityTransforms<object>().SelectMany(t => sources.Select(s => t(s))))
            {
                yield return new object[] { equivalentSource };
            }
        }

        [Theory]
        [MemberData(nameof(UntypedCollectionWithAllStringsData))]
        public void CountToArrayAndToListShouldWorkAsExpected(IEnumerable source)
        {
            Assert.Equal(NaiveCast<string>(source).Count(), source.Cast<string>().Count());
            Assert.Equal(NaiveCast<string>(source), source.Cast<string>().ToArray());
            Assert.Equal(NaiveCast<string>(source), source.Cast<string>().ToList());
        }

        public static IEnumerable<object[]> UntypedCollectionWithAllStringsData()
        {
            var sources = new List<IEnumerable<object>>
            {
                Enumerable.Repeat<object>("foo", 37),
                Enumerable.Repeat<object>(null, 123),
                new object[] { null, "strstr", "bar" },
                Array.Empty<object>()
            };

            foreach (var equivalentSource in IdentityTransforms<object>().SelectMany(t => sources.Select(s => t(s))))
            {
                yield return new object[] { equivalentSource };
            }
        }

        private static IEnumerable<T> NaiveCast<T>(IEnumerable source)
        {
            Debug.Assert(source != null);

            foreach (object obj in source)
            {
                yield return (T)obj;
            }
        }
    }
}
