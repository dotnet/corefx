// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Tests
{
    public abstract partial class HashSet_Generic_Tests<T> : ISet_Generic_Tests<T>
    {
        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void HashSet_Generic_Constructor_int(int capacity)
        {
            HashSet<T> set = new HashSet<T>(capacity);
            Assert.Equal(0, set.Count);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void HashSet_Generic_Constructor_int_AddUpToAndBeyondCapacity(int capacity)
        {
            HashSet<T> set = new HashSet<T>(capacity);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void HashSet_Generic_Constructor_int_Negative_ThrowsArgumentOutOfRangeException()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new HashSet<T>(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new HashSet<T>(int.MinValue));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void HashSet_Generic_Constructor_int_IEqualityComparer(int capacity)
        {
            IEqualityComparer<T> comparer = GetIEqualityComparer();
            HashSet<T> set = new HashSet<T>(capacity, comparer);
            Assert.Equal(0, set.Count);
            if (comparer == null)
                Assert.Equal(EqualityComparer<T>.Default, set.Comparer);
            else
                Assert.Equal(comparer, set.Comparer);
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void HashSet_Generic_Constructor_int_IEqualityComparer_AddUpToAndBeyondCapacity(int capacity)
        {
            IEqualityComparer<T> comparer = GetIEqualityComparer();
            HashSet<T> set = new HashSet<T>(capacity, comparer);

            AddToCollection(set, capacity);
            Assert.Equal(capacity, set.Count);

            AddToCollection(set, capacity + 1);
            Assert.Equal(capacity + 1, set.Count);
        }

        [Fact]
        public void HashSet_Generic_Constructor_int_IEqualityComparer_Negative_ThrowsArgumentOutOfRangeException()
        {
            IEqualityComparer<T> comparer = GetIEqualityComparer();
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new HashSet<T>(-1, comparer));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("capacity", () => new HashSet<T>(int.MinValue, comparer));
        }

        #region TryGetValue

        [Fact]
        public void HashSet_Generic_TryGetValue_Contains()
        {
            T value = CreateT(1);
            HashSet<T> set = new HashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue;
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).IsValueType)
            {
                Assert.Same(value, actualValue);
            }
        }

        [Fact]
        public void HashSet_Generic_TryGetValue_Contains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            HashSet<T> set = new HashSet<T> { value };
            T equalValue = CreateT(1);
            T actualValue = CreateT(2);
            Assert.True(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(value, actualValue);
            if (!typeof(T).IsValueType)
            {
                Assert.Same(value, actualValue);
            }
        }

        [Fact]
        public void HashSet_Generic_TryGetValue_NotContains()
        {
            T value = CreateT(1);
            HashSet<T> set = new HashSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        [Fact]
        public void HashSet_Generic_TryGetValue_NotContains_OverwriteOutputParam()
        {
            T value = CreateT(1);
            HashSet<T> set = new HashSet<T> { value };
            T equalValue = CreateT(2);
            T actualValue = equalValue;
            Assert.False(set.TryGetValue(equalValue, out actualValue));
            Assert.Equal(default(T), actualValue);
        }

        #endregion

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSelf(int setLength)
        {
            HashSet<T> set = (HashSet<T>)GenericISetFactory(setLength);
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(set.Comparer);
            Assert.True(comparer.Equals(set, set));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSimilar(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>();
            foreach (T item in x)
            {
                y.Add(item);
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(x.Comparer);
            Assert.True(comparer.Equals(x, y));
            Assert.True(comparer.Equals(y, x));
            Assert.Equal(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSimilarAddedDifferentOrder(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>();
            foreach (T item in x.Reverse())
            {
                y.Add(item);
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(x.Comparer);
            Assert.True(comparer.Equals(x, y));
            Assert.True(comparer.Equals(y, x));
            Assert.Equal(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSameButDifferentComparer(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>(new DefaultCopyCatComparer<T>());
            foreach (T item in x)
            {
                y.Add(item);
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(x.Comparer);
            Assert.True(comparer.Equals(x, y));
            Assert.True(comparer.Equals(y, x));
            Assert.Equal(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSameButDifferentToSourceCmp(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>();
            foreach (T item in x)
            {
                y.Add(item);
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>());
            Assert.True(comparer.Equals(x, y));
            Assert.True(comparer.Equals(y, x));
            Assert.Equal(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithLarger(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>();
            foreach (T item in x)
            {
                y.Add(item);
            }

            int seed = setLength;
            while (y.Count == setLength)
            {
                y.Add(CreateT(seed++));
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(x.Comparer);
            Assert.False(comparer.Equals(x, y));
            Assert.False(comparer.Equals(y, x));
            // Valid for the following to fail, but very unlikely so failure is probably a problem and should be investigated
            Assert.NotEqual(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithLargerDifferentSourceCmp(int setLength)
        {
            HashSet<T> x = (HashSet<T>)GenericISetFactory(setLength);
            HashSet<T> y = new HashSet<T>();
            foreach (T item in x)
            {
                y.Add(item);
            }

            int seed = setLength;
            while (y.Count == setLength)
            {
                y.Add(CreateT(seed++));
            }
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>());
            Assert.False(comparer.Equals(x, y));
            Assert.False(comparer.Equals(y, x));
            // Valid for the following to fail, but very unlikely so failure is probably a problem and should be investigated
            Assert.NotEqual(comparer.GetHashCode(x), comparer.GetHashCode(y));
        }

        [Theory]
        [MemberData(nameof(ValidCollectionSizes))]
        public void SetComparerCompareWithSelfCustomComparer(int setLength)
        {
            HashSet<T> set = new HashSet<T>(new DefaultCopyCatComparer<T>());
            AddToCollection(set, setLength);
            IEqualityComparer<HashSet<T>> comparer = HashSet<T>.CreateSetComparer(set.Comparer);
            Assert.True(comparer.Equals(set, set));
        }

        [Fact]
        public void SetComparerNullHashCode()
        {
            Assert.Equal(0, HashSet<T>.CreateSetComparer(null).GetHashCode(null));
            Assert.Equal(0, HashSet<T>.CreateSetComparer().GetHashCode(null));
            Assert.Equal(0, HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>()).GetHashCode(null));
        }

        [Fact]
        public void SetComparerNullEqualsNull()
        {
            Assert.True(HashSet<T>.CreateSetComparer(null).Equals(null, null));
            Assert.True(HashSet<T>.CreateSetComparer().Equals(null, null));
            Assert.True(HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>()).Equals(null, null));
        }

        [Fact]
        public void SetComparerNullNotEqualsEmpty()
        {
            HashSet<T> empty = new HashSet<T>();
            Assert.False(HashSet<T>.CreateSetComparer(null).Equals(empty, null));
            Assert.False(HashSet<T>.CreateSetComparer().Equals(empty, null));
            Assert.False(HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>()).Equals(empty, null));
            Assert.False(HashSet<T>.CreateSetComparer(null).Equals(null, empty));
            Assert.False(HashSet<T>.CreateSetComparer().Equals(null, empty));
            Assert.False(HashSet<T>.CreateSetComparer(new DefaultCopyCatComparer<T>()).Equals(null, empty));
        }

        [Fact]
        public void SetComparerSelfComparison()
        {
            var x = HashSet<T>.CreateSetComparer(null);
            var y = HashSet<T>.CreateSetComparer(null);
            Assert.NotSame(x, y);
            Assert.Equal(x, y);
            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void NullarySetComparerSelfComparison()
        {
            var x = HashSet<T>.CreateSetComparer();
            var y = HashSet<T>.CreateSetComparer();
            Assert.NotSame(x, y);
            Assert.Equal(x, y);
            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void UnaryNullarySetComparerSelfComparison()
        {
            var singular = HashSet<T>.CreateSetComparer(null);
            var nullar = HashSet<T>.CreateSetComparer();
            Assert.NotEqual(singular, nullar);
            // Valid for the following to fail, but very unlikely so failure is probably a problem and should be investigated
            Assert.NotEqual(singular.GetHashCode(), nullar.GetHashCode());
        }
    }

    public class HashSetComparisonsWithDifferentComparersTests
    {
        private static IEnumerable<object[]> StringSetData()
        {
            yield return new object[]
            {
                new HashSet<string> {"a", "b", "c", "d", "d", "f", "g", "h", "A"},
                new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"a"},
                null, false, true, false
            };
            yield return new object[]
            {
                new HashSet<string>(StringComparer.OrdinalIgnoreCase) {"a"},
                new HashSet<string> {"a", "b", "c", "d", "d", "f", "g", "h", "A"},
                null, false, false, false
            };
            yield return new object[]
            {
                new HashSet<string> {"Case", "case", "CASE", "cASE", "cAsE"},
                new HashSet<string> {"Case", "case", "CASE", "cASE"},
                StringComparer.OrdinalIgnoreCase, true, false, false
            };
            yield return new object[]
            {
                new HashSet<string> {"Case", "case", "CASE", "cASE"},
                new HashSet<string> {"Case", "case", "CASE", "cASE", "cAsE"},
                StringComparer.OrdinalIgnoreCase, true, false, false
            };
            yield return new object[]
            {
                new HashSet<string>(StringComparer.OrdinalIgnoreCase){"a", "b", "c"},
                new HashSet<string>(StringComparer.OrdinalIgnoreCase){"A", "B", "C"},
                null, false, true, false
            };
            yield return new object[]
            {
                new HashSet<string>(new ReferenceEqualityComparer<string>()) { "a", "b", "c" },
                new HashSet<string>(new ReferenceEqualityComparer<string>()) { new string('a', 1), "b", "c" },
                null, true, false, true
            };
            yield return new object[]
            {
                new HashSet<string>(new ReferenceEqualityComparer<string>()) { "a", "b", "c" },
                new HashSet<string>(new ReferenceEqualityComparer<string>()) { new string('a', 1), "b", "c" },
                new ReferenceEqualityComparer<string>(), false, false, true
            };
        }

        [Theory, MemberData(nameof(StringSetData))]
        public void CompareSets(
            HashSet<string> x, HashSet<string> y, IEqualityComparer<string> comparer, bool equal, bool? legacyEqual, bool? legacyHashEqual)
        {
            var setComparer = HashSet<string>.CreateSetComparer(comparer);
            var nullaryComparer = HashSet<string>.CreateSetComparer();
            Assert.Equal(equal, setComparer.Equals(x, y));
            if (equal)
            {
                Assert.Equal(setComparer.GetHashCode(x), setComparer.GetHashCode(y));
            }
            else
            {
                // Valid for the following to fail, but very unlikely so failure is probably a problem and should be investigated
                Assert.NotEqual(setComparer.GetHashCode(x), setComparer.GetHashCode(y));
            }

            if (legacyEqual.HasValue)
            {
                Assert.Equal(legacyEqual, nullaryComparer.Equals(x, y));
            }
            if (legacyHashEqual.HasValue)
            {
                if (legacyHashEqual.GetValueOrDefault())
                {
                    Assert.Equal(nullaryComparer.GetHashCode(x), nullaryComparer.GetHashCode(y));
                }
                else
                {
                    Assert.NotEqual(nullaryComparer.GetHashCode(x), nullaryComparer.GetHashCode(y));
                }
            }
        }

        [Fact]
        public void SetsOfSets()
        {
            IEqualityComparer<HashSet<string>> stringSetCmp = HashSet<string>.CreateSetComparer(null);
            HashSet<HashSet<string>> l1 = new HashSet<HashSet<string>>(stringSetCmp);
            HashSet<HashSet<string>> l2 = new HashSet<HashSet<string>>(stringSetCmp);

            HashSet<string> set1 = new HashSet<string> { "a" };
            HashSet<string> set2 = new HashSet<string> { "a" };
            l1.Add(set1);
            l2.Add(set2);

            IEqualityComparer<HashSet<HashSet<string>>> matchingSetCmp = HashSet<HashSet<string>>.CreateSetComparer(stringSetCmp);
            Assert.True(matchingSetCmp.Equals(l1, l2));
            Assert.Equal(matchingSetCmp.GetHashCode(l1), matchingSetCmp.GetHashCode(l2));

            IEqualityComparer<HashSet<HashSet<string>>> notMtchingSetCmp = HashSet<HashSet<string>>.CreateSetComparer(null);
            Assert.False(notMtchingSetCmp.Equals(l1, l2));
            Assert.NotEqual(notMtchingSetCmp.GetHashCode(l1), matchingSetCmp.GetHashCode(l2));

            IEqualityComparer<HashSet<HashSet<string>>> legacySetCmp = HashSet<HashSet<string>>.CreateSetComparer();
            Assert.True(legacySetCmp.Equals(l1, l2));
            // https://github.com/dotnet/corefx/issues/12560
            Assert.NotEqual(legacySetCmp.GetHashCode(l1), matchingSetCmp.GetHashCode(l2));
        }
    }
}
