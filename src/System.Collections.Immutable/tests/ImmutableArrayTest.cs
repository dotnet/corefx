// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Collections.Immutable.Test
{
    public class ImmutableArrayTest : SimpleElementImmutablesTestBase
    {
        private static readonly ImmutableArray<int> _emptyDefault;
        private static readonly ImmutableArray<int> _empty = ImmutableArray.Create<int>();
        private static readonly ImmutableArray<int> _oneElement = ImmutableArray.Create(1);
        private static readonly ImmutableArray<int> _manyElements = ImmutableArray.Create(1, 2, 3);
        private static readonly ImmutableArray<GenericParameterHelper> _oneElementRefType = ImmutableArray.Create(new GenericParameterHelper(1));
        private static readonly ImmutableArray<string> _twoElementRefTypeWithNull = ImmutableArray.Create("1", null);

        [Fact]
        public void CreateEmpty()
        {
            Assert.Equal(ImmutableArray.Create<int>(), ImmutableArray<int>.Empty);
        }

        [Fact]
        public void CreateFromEnumerable()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange((IEnumerable<int>)null));

            IEnumerable<int> source = new[] { 1, 2, 3 };
            var array = ImmutableArray.CreateRange(source);
            Assert.Equal(3, array.Length);
        }

        [Fact]
        public void CreateFromEmptyEnumerableReturnsSingleton()
        {
            IEnumerable<int> emptySource1 = new int[0];
            var immutable = ImmutableArray.CreateRange(emptySource1);

            // This equality check returns true if the underlying arrays are the same instance.
            Assert.Equal(_empty, immutable);
        }

        [Fact]
        public void CreateRangeFromImmutableArrayWithSelector()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);

            var copy1 = ImmutableArray.CreateRange(array, i => i + 0.5);
            Assert.Equal(new[] { 4.5, 5.5, 6.5, 7.5 }, copy1);

            var copy2 = ImmutableArray.CreateRange(array, i => i + 1);
            Assert.Equal(new[] { 5, 6, 7, 8 }, copy2);

            Assert.Equal(new int[] { }, ImmutableArray.CreateRange(_empty, i => i));

            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(array, (Func<int, int>)null));
        }

        [Fact]
        public void CreateRangeFromImmutableArrayWithSelectorAndArgument()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);

            var copy1 = ImmutableArray.CreateRange(array, (i, j) => i + j, 0.5);
            Assert.Equal(new[] { 4.5, 5.5, 6.5, 7.5 }, copy1);

            var copy2 = ImmutableArray.CreateRange(array, (i, j) => i + j, 1);
            Assert.Equal(new[] { 5, 6, 7, 8 }, copy2);

            var copy3 = ImmutableArray.CreateRange(array, (int i, object j) => i, null);
            Assert.Equal(new[] { 4, 5, 6, 7 }, copy3);

            Assert.Equal(new int[] { }, ImmutableArray.CreateRange(_empty, (i, j) => i + j, 0));

            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(array, (Func<int, int, int>)null, 0));
        }

        [Fact]
        public void CreateRangeSliceFromImmutableArrayWithSelector()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);

            var copy1 = ImmutableArray.CreateRange(array, 0, 0, i => i + 0.5);
            Assert.Equal(new double[] { }, copy1);

            var copy2 = ImmutableArray.CreateRange(array, 0, 0, i => i);
            Assert.Equal(new int[] { }, copy2);

            var copy3 = ImmutableArray.CreateRange(array, 0, 1, i => i * 2);
            Assert.Equal(new int[] { 8 }, copy3);

            var copy4 = ImmutableArray.CreateRange(array, 0, 2, i => i + 1);
            Assert.Equal(new int[] { 5, 6 }, copy4);

            var copy5 = ImmutableArray.CreateRange(array, 0, 4, i => i);
            Assert.Equal(new int[] { 4, 5, 6, 7 }, copy5);

            var copy6 = ImmutableArray.CreateRange(array, 3, 1, i => i);
            Assert.Equal(new int[] { 7 }, copy6);

            var copy7 = ImmutableArray.CreateRange(array, 3, 0, i => i);
            Assert.Equal(new int[] { }, copy7);

            var copy8 = ImmutableArray.CreateRange(array, 4, 0, i => i);
            Assert.Equal(new int[] { }, copy8);

            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(array, 0, 0, (Func<int, int>)null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(_empty, 0, 0, (Func<int, int>)null));

            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, -1, 1, (Func<int, int>)null));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, -1, 1, i => i));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 0, 5, i => i));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 4, 1, i => i));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 3, 2, i => i));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 1, -1, i => i));
        }

        [Fact]
        public void CreateRangeSliceFromImmutableArrayWithSelectorAndArgument()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);

            var copy1 = ImmutableArray.CreateRange(array, 0, 0, (i, j) => i + j, 0.5);
            Assert.Equal(new double[] { }, copy1);

            var copy2 = ImmutableArray.CreateRange(array, 0, 0, (i, j) => i + j, 0);
            Assert.Equal(new int[] { }, copy2);

            var copy3 = ImmutableArray.CreateRange(array, 0, 1, (i, j) => i * j, 2);
            Assert.Equal(new int[] { 8 }, copy3);

            var copy4 = ImmutableArray.CreateRange(array, 0, 2, (i, j) => i + j, 1);
            Assert.Equal(new int[] { 5, 6 }, copy4);

            var copy5 = ImmutableArray.CreateRange(array, 0, 4, (i, j) => i + j, 0);
            Assert.Equal(new int[] { 4, 5, 6, 7 }, copy5);

            var copy6 = ImmutableArray.CreateRange(array, 3, 1, (i, j) => i + j, 0);
            Assert.Equal(new int[] { 7 }, copy6);

            var copy7 = ImmutableArray.CreateRange(array, 3, 0, (i, j) => i + j, 0);
            Assert.Equal(new int[] { }, copy7);

            var copy8 = ImmutableArray.CreateRange(array, 4, 0, (i, j) => i + j, 0);
            Assert.Equal(new int[] { }, copy8);

            var copy9 = ImmutableArray.CreateRange(array, 0, 1, (int i, object j) => i, null);
            Assert.Equal(new int[] { 4 }, copy9);

            Assert.Equal(new int[] { }, ImmutableArray.CreateRange(_empty, 0, 0, (i, j) => i + j, 0));

            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(array, 0, 0, (Func<int, int, int>)null, 0));
            Assert.Throws<ArgumentNullException>(() => ImmutableArray.CreateRange(_empty, 0, 0, (Func<int, int, int>)null, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(_empty, -1, 1, (Func<int, int, int>)null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, -1, 1, (i, j) => i + j, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 0, 5, (i, j) => i + j, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 4, 1, (i, j) => i + j, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 3, 2, (i, j) => i + j, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.CreateRange(array, 1, -1, (i, j) => i + j, 0));
        }

        [Fact]
        public void CreateFromSliceOfImmutableArray()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);
            Assert.Equal(new[] { 4, 5 }, ImmutableArray.Create(array, 0, 2));
            Assert.Equal(new[] { 5, 6 }, ImmutableArray.Create(array, 1, 2));
            Assert.Equal(new[] { 6, 7 }, ImmutableArray.Create(array, 2, 2));
            Assert.Equal(new[] { 7 }, ImmutableArray.Create(array, 3, 1));
            Assert.Equal(new int[0], ImmutableArray.Create(array, 4, 0));

            Assert.Equal(new int[] { }, ImmutableArray.Create(_empty, 0, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(_empty, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 0, array.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 1, array.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, array.Length + 1, 0));
        }

        [Fact]
        public void CreateFromSliceOfImmutableArrayOptimizations()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);
            var slice = ImmutableArray.Create(array, 0, array.Length);
            Assert.Equal(array, slice); // array instance actually shared between the two
        }

        [Fact]
        public void CreateFromSliceOfImmutableArrayEmptyReturnsSingleton()
        {
            var array = ImmutableArray.Create(4, 5, 6, 7);
            var slice = ImmutableArray.Create(array, 1, 0);
            Assert.Equal(_empty, slice);
        }

        [Fact]
        public void CreateFromSliceOfArray()
        {
            var array = new int[] { 4, 5, 6, 7 };
            Assert.Equal(new[] { 4, 5 }, ImmutableArray.Create(array, 0, 2));
            Assert.Equal(new[] { 5, 6 }, ImmutableArray.Create(array, 1, 2));
            Assert.Equal(new[] { 6, 7 }, ImmutableArray.Create(array, 2, 2));
            Assert.Equal(new[] { 7 }, ImmutableArray.Create(array, 3, 1));
            Assert.Equal(new int[0], ImmutableArray.Create(array, 4, 0));

            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 0, array.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, 1, array.Length));
            Assert.Throws<ArgumentOutOfRangeException>(() => ImmutableArray.Create(array, array.Length + 1, 0));
        }

        [Fact]
        public void CreateFromSliceOfArrayEmptyReturnsSingleton()
        {
            var array = new int[] { 4, 5, 6, 7 };
            var slice = ImmutableArray.Create(array, 1, 0);
            Assert.Equal(_empty, slice);
            slice = ImmutableArray.Create(array, array.Length, 0);
            Assert.Equal(_empty, slice);
        }

        [Fact]
        public void CreateFromArray()
        {
            var source = new[] { 1, 2, 3 };
            var immutable = ImmutableArray.Create(source);
            Assert.Equal(source, immutable);
        }

        [Fact]
        public void CreateFromNullArray()
        {
            int[] nullArray = null;
            ImmutableArray<int> immutable = ImmutableArray.Create(nullArray);
            Assert.False(immutable.IsDefault);
            Assert.Equal(0, immutable.Length);
        }

        [Fact]
        public void Covariance()
        {
            ImmutableArray<string> derivedImmutable = ImmutableArray.Create("a", "b", "c");
            ImmutableArray<object> baseImmutable = derivedImmutable.As<object>();
            Assert.False(baseImmutable.IsDefault);
            Assert.Equal(derivedImmutable, baseImmutable);

            // Make sure we can reverse that, as a means to verify the underlying array is the same instance.
            ImmutableArray<string> derivedImmutable2 = baseImmutable.As<string>();
            Assert.False(derivedImmutable2.IsDefault);
            Assert.Equal(derivedImmutable, derivedImmutable2);

            // Try a cast that would fail.
            Assert.True(baseImmutable.As<Encoder>().IsDefault);
        }

        [Fact]
        public void DowncastOfDefaultStructs()
        {
            ImmutableArray<string> derivedImmutable = default(ImmutableArray<string>);
            ImmutableArray<object> baseImmutable = derivedImmutable.As<object>();
            Assert.True(baseImmutable.IsDefault);
            Assert.True(derivedImmutable.IsDefault);

            // Make sure we can reverse that, as a means to verify the underlying array is the same instance.
            ImmutableArray<string> derivedImmutable2 = baseImmutable.As<string>();
            Assert.True(derivedImmutable2.IsDefault);
            Assert.True(derivedImmutable == derivedImmutable2);
        }

        /// <summary>
        /// Verifies that using an ordinary Create factory method is smart enough to reuse
        /// an underlying array when possible.
        /// </summary>
        [Fact]
        public void CovarianceImplicit()
        {
            ImmutableArray<string> derivedImmutable = ImmutableArray.Create("a", "b", "c");
            ImmutableArray<object> baseImmutable = ImmutableArray.CreateRange<object>(derivedImmutable);
            Assert.Equal(derivedImmutable, baseImmutable);

            // Make sure we can reverse that, as a means to verify the underlying array is the same instance.
            ImmutableArray<string> derivedImmutable2 = baseImmutable.As<string>();
            Assert.Equal(derivedImmutable, derivedImmutable2);
        }

        [Fact]
        public void CreateByCovariantStaticCast()
        {
            ImmutableArray<string> derivedImmutable = ImmutableArray.Create("a", "b", "c");
            ImmutableArray<object> baseImmutable = ImmutableArray.Create<object, string>(derivedImmutable);
            Assert.Equal(derivedImmutable, baseImmutable);

            // Make sure we can reverse that, as a means to verify the underlying array is the same instance.
            ImmutableArray<string> derivedImmutable2 = baseImmutable.As<string>();
            Assert.Equal(derivedImmutable, derivedImmutable2);
        }

        [Fact]
        public void CreateByCovariantStaticCastDefault()
        {
            ImmutableArray<string> derivedImmutable = default(ImmutableArray<string>);
            ImmutableArray<object> baseImmutable = ImmutableArray.Create<object, string>(derivedImmutable);
            Assert.True(baseImmutable.IsDefault);
            Assert.True(derivedImmutable.IsDefault);

            // Make sure we can reverse that, as a means to verify the underlying array is the same instance.
            ImmutableArray<string> derivedImmutable2 = baseImmutable.As<string>();
            Assert.True(derivedImmutable2.IsDefault);
            Assert.True(derivedImmutable == derivedImmutable2);
        }

        [Fact]
        public void ToImmutableArray()
        {
            IEnumerable<int> source = new[] { 1, 2, 3 };
            ImmutableArray<int> immutable = source.ToImmutableArray();
            Assert.Equal(source, immutable);

            ImmutableArray<int> immutable2 = immutable.ToImmutableArray();
            Assert.Equal(immutable, immutable2); // this will compare array reference equality.
        }

        [Fact]
        public void Count()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Length);
            Assert.Throws<InvalidOperationException>(() => ((ICollection)_emptyDefault).Count);
            Assert.Throws<InvalidOperationException>(() => ((ICollection<int>)_emptyDefault).Count);
            Assert.Throws<InvalidOperationException>(() => ((IReadOnlyCollection<int>)_emptyDefault).Count);

            Assert.Equal(0, _empty.Length);
            Assert.Equal(0, ((ICollection)_empty).Count);
            Assert.Equal(0, ((ICollection<int>)_empty).Count);
            Assert.Equal(0, ((IReadOnlyCollection<int>)_empty).Count);

            Assert.Equal(1, _oneElement.Length);
            Assert.Equal(1, ((ICollection)_oneElement).Count);
            Assert.Equal(1, ((ICollection<int>)_oneElement).Count);
            Assert.Equal(1, ((IReadOnlyCollection<int>)_oneElement).Count);
        }

        [Fact]
        public void IsEmpty()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.IsEmpty);
            Assert.True(_empty.IsEmpty);
            Assert.False(_oneElement.IsEmpty);
        }

        [Fact]
        public void IndexOfDefault()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.IndexOf(5));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.IndexOf(5, 0));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.IndexOf(5, 0, 0));
        }

        [Fact]
        public void LastIndexOfDefault()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.LastIndexOf(5));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.LastIndexOf(5, 0));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.LastIndexOf(5, 0, 0));
        }

        [Fact]
        public void IndexOf()
        {
            IndexOfTests.IndexOfTest(
                seq => ImmutableArray.CreateRange(seq),
                (b, v) => b.IndexOf(v),
                (b, v, i) => b.IndexOf(v, i),
                (b, v, i, c) => b.IndexOf(v, i, c),
                (b, v, i, c, eq) => b.IndexOf(v, i, c, eq));
        }

        [Fact]
        public void LastIndexOf()
        {
            IndexOfTests.LastIndexOfTest(
                seq => ImmutableArray.CreateRange(seq),
                (b, v) => b.LastIndexOf(v),
                (b, v, eq) => b.LastIndexOf(v, eq),
                (b, v, i) => b.LastIndexOf(v, i),
                (b, v, i, c) => b.LastIndexOf(v, i, c),
                (b, v, i, c, eq) => b.LastIndexOf(v, i, c, eq));
        }

        [Fact]
        public void Contains()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Contains(0));
            Assert.False(_empty.Contains(0));
            Assert.True(_oneElement.Contains(1));
            Assert.False(_oneElement.Contains(2));
            Assert.True(_manyElements.Contains(3));
            Assert.False(_oneElementRefType.Contains(null));
            Assert.True(_twoElementRefTypeWithNull.Contains(null));
        }

        [Fact]
        public void ContainsEqualityComparer()
        {
            var array = ImmutableArray.Create("a", "B");
            Assert.False(array.Contains("A", StringComparer.Ordinal));
            Assert.True(array.Contains("A", StringComparer.OrdinalIgnoreCase));
            Assert.False(array.Contains("b", StringComparer.Ordinal));
            Assert.True(array.Contains("b", StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void Enumerator()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.GetEnumerator());

            ImmutableArray<int>.Enumerator enumerator = default(ImmutableArray<int>.Enumerator);
            Assert.Throws<NullReferenceException>(() => enumerator.Current);
            Assert.Throws<NullReferenceException>(() => enumerator.MoveNext());

            enumerator = _empty.GetEnumerator();
            Assert.Throws<IndexOutOfRangeException>(() => enumerator.Current);
            Assert.False(enumerator.MoveNext());

            enumerator = _manyElements.GetEnumerator();
            Assert.Throws<IndexOutOfRangeException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<IndexOutOfRangeException>(() => enumerator.Current);
        }

        [Fact]
        public void ObjectEnumerator()
        {
            Assert.Throws<InvalidOperationException>(() => ((IEnumerable<int>)_emptyDefault).GetEnumerator());

            IEnumerator<int> enumerator = ((IEnumerable<int>)_empty).GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            Assert.False(enumerator.MoveNext());

            enumerator = ((IEnumerable<int>)_manyElements).GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }

        [Fact]
        public void EnumeratorWithNullValues()
        {
            var enumerationResult = System.Linq.Enumerable.ToArray(_twoElementRefTypeWithNull);
            Assert.Equal("1", enumerationResult[0]);
            Assert.Null(enumerationResult[1]);
        }

        [Fact]
        public void EqualityCheckComparesInternalArrayByReference()
        {
            var immutable1 = ImmutableArray.Create(1);
            var immutable2 = ImmutableArray.Create(1);
            Assert.NotEqual(immutable1, immutable2);

            Assert.True(immutable1.Equals(immutable1));
            Assert.True(immutable1.Equals((object)immutable1));
        }

        [Fact]
        public void EqualsObjectNull()
        {
            Assert.False(_empty.Equals((object)null));
        }

        [Fact]
        public void OperatorsAndEquality()
        {
            Assert.True(_empty.Equals(_empty));
            var emptySame = _empty;
            Assert.True(_empty == emptySame);
            Assert.False(_empty != emptySame);

            // empty and default should not be seen as equal
            Assert.False(_empty.Equals(_emptyDefault));
            Assert.False(_empty == _emptyDefault);
            Assert.True(_empty != _emptyDefault);
            Assert.False(_emptyDefault == _empty);
            Assert.True(_emptyDefault != _empty);

            Assert.False(_empty.Equals(_oneElement));
            Assert.False(_empty == _oneElement);
            Assert.True(_empty != _oneElement);
            Assert.False(_oneElement == _empty);
            Assert.True(_oneElement != _empty);
        }

        [Fact]
        public void NullableOperators()
        {
            ImmutableArray<int>? nullArray = null;
            ImmutableArray<int>? nonNullDefault = _emptyDefault;
            ImmutableArray<int>? nonNullEmpty = _empty;

            Assert.True(nullArray == nonNullDefault);
            Assert.False(nullArray != nonNullDefault);
            Assert.True(nonNullDefault == nullArray);
            Assert.False(nonNullDefault != nullArray);

            Assert.False(nullArray == nonNullEmpty);
            Assert.True(nullArray != nonNullEmpty);
            Assert.False(nonNullEmpty == nullArray);
            Assert.True(nonNullEmpty != nullArray);
        }

        [Fact]
        public void GetHashCodeTest()
        {
            Assert.Equal(0, _emptyDefault.GetHashCode());
            Assert.NotEqual(0, _empty.GetHashCode());
            Assert.NotEqual(0, _oneElement.GetHashCode());
        }

        [Fact]
        public void Add()
        {
            var source = new[] { 1, 2 };
            var array1 = ImmutableArray.Create(source);
            var array2 = array1.Add(3);
            Assert.Equal(source, array1);
            Assert.Equal(new[] { 1, 2, 3 }, array2);
            Assert.Equal(new[] { 1 }, _empty.Add(1));
        }

        [Fact]
        public void AddRange()
        {
            var nothingToEmpty = _empty.AddRange(Enumerable.Empty<int>());
            Assert.False(nothingToEmpty.IsDefault);
            Assert.True(nothingToEmpty.IsEmpty);

            Assert.Equal(new[] { 1, 2 }, _empty.AddRange(Enumerable.Range(1, 2)));
            Assert.Equal(new[] { 1, 2 }, _empty.AddRange(new[] { 1, 2 }));

            Assert.Equal(new[] { 1, 2, 3, 4 }, _manyElements.AddRange(new[] { 4 }));
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, _manyElements.AddRange(new[] { 4, 5 }));
        }

        [Fact]
        public void AddRangeDefaultEnumerable()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(Enumerable.Empty<int>()));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(Enumerable.Range(1, 2)));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(new[] { 1, 2 }));
        }

        [Fact]
        public void AddRangeDefaultStruct()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(_empty));
            Assert.Throws<NullReferenceException>(() => _empty.AddRange(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(_oneElement));
            Assert.Throws<NullReferenceException>(() => _oneElement.AddRange(_emptyDefault));

            IEnumerable<int> emptyBoxed = _empty;
            IEnumerable<int> emptyDefaultBoxed = _emptyDefault;
            IEnumerable<int> oneElementBoxed = _oneElement;
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(emptyBoxed));
            Assert.Throws<InvalidOperationException>(() => _empty.AddRange(emptyDefaultBoxed));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.AddRange(oneElementBoxed));
            Assert.Throws<InvalidOperationException>(() => _oneElement.AddRange(emptyDefaultBoxed));
        }

        [Fact]
        public void AddRangeNoOpIdentity()
        {
            Assert.Equal(_empty, _empty.AddRange(_empty));
            Assert.Equal(_oneElement, _empty.AddRange(_oneElement)); // struct overload
            Assert.Equal(_oneElement, _empty.AddRange((IEnumerable<int>)_oneElement)); // enumerable overload
            Assert.Equal(_oneElement, _oneElement.AddRange(_empty));
        }

        [Fact]
        public void Insert()
        {
            var array1 = ImmutableArray.Create<char>();
            Assert.Throws<ArgumentOutOfRangeException>(() => array1.Insert(-1, 'a'));
            Assert.Throws<ArgumentOutOfRangeException>(() => array1.Insert(1, 'a'));

            var insertFirst = array1.Insert(0, 'c');
            Assert.Equal(new[] { 'c' }, insertFirst);

            var insertLeft = insertFirst.Insert(0, 'a');
            Assert.Equal(new[] { 'a', 'c' }, insertLeft);

            var insertRight = insertFirst.Insert(1, 'e');
            Assert.Equal(new[] { 'c', 'e' }, insertRight);

            var insertBetween = insertLeft.Insert(1, 'b');
            Assert.Equal(new[] { 'a', 'b', 'c' }, insertBetween);
        }

        [Fact]
        public void InsertDefault()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Insert(-1, 10));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Insert(1, 10));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Insert(0, 10));
        }

        [Fact]
        public void InsertRangeNoOpIdentity()
        {
            Assert.Equal(_empty, _empty.InsertRange(0, _empty));
            Assert.Equal(_oneElement, _empty.InsertRange(0, _oneElement)); // struct overload
            Assert.Equal(_oneElement, _empty.InsertRange(0, (IEnumerable<int>)_oneElement)); // enumerable overload
            Assert.Equal(_oneElement, _oneElement.InsertRange(0, _empty));
        }

        [Fact]
        public void InsertRangeEmpty()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Insert(-1, 10));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Insert(1, 10));
            Assert.Equal(new int[0], _empty.InsertRange(0, Enumerable.Empty<int>()));
            Assert.Equal(_empty, _empty.InsertRange(0, Enumerable.Empty<int>()));
            Assert.Equal(new[] { 1 }, _empty.InsertRange(0, new[] { 1 }));
            Assert.Equal(new[] { 2, 3, 4 }, _empty.InsertRange(0, new[] { 2, 3, 4 }));
            Assert.Equal(new[] { 2, 3, 4 }, _empty.InsertRange(0, Enumerable.Range(2, 3)));
            Assert.Equal(_manyElements, _manyElements.InsertRange(0, Enumerable.Empty<int>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.InsertRange(1, _oneElement));
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.InsertRange(-1, _oneElement));
        }

        [Fact]
        public void InsertRangeDefault()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(1, Enumerable.Empty<int>()));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(-1, Enumerable.Empty<int>()));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, Enumerable.Empty<int>()));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, new[] { 1 }));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, new[] { 2, 3, 4 }));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, Enumerable.Range(2, 3)));
        }

        /// <summary>
        /// Validates that a fixed bug in the inappropriate adding of the
        /// Empty singleton enumerator to the reusable instances bag does not regress.
        /// </summary>
        [Fact]
        public void EmptyEnumeratorReuseRegressionTest()
        {
            IEnumerable<int> oneElementBoxed = _oneElement;
            IEnumerable<int> emptyBoxed = _empty;
            IEnumerable<int> emptyDefaultBoxed = _emptyDefault;

            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(emptyBoxed));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(emptyDefaultBoxed));
            Assert.Throws<InvalidOperationException>(() => _empty.RemoveRange(emptyDefaultBoxed));
            Assert.Equal(oneElementBoxed, oneElementBoxed);
        }

        [Fact]
        public void InsertRangeDefaultStruct()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, _empty));
            Assert.Throws<NullReferenceException>(() => _empty.InsertRange(0, _emptyDefault));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, _oneElement));
            Assert.Throws<NullReferenceException>(() => _oneElement.InsertRange(0, _emptyDefault));

            IEnumerable<int> emptyBoxed = _empty;
            IEnumerable<int> emptyDefaultBoxed = _emptyDefault;
            IEnumerable<int> oneElementBoxed = _oneElement;
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, emptyBoxed));
            Assert.Throws<InvalidOperationException>(() => _empty.InsertRange(0, emptyDefaultBoxed));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.InsertRange(0, oneElementBoxed));
            Assert.Throws<InvalidOperationException>(() => _oneElement.InsertRange(0, emptyDefaultBoxed));
        }

        [Fact]
        public void InsertRangeLeft()
        {
            Assert.Equal(new[] { 7, 1, 2, 3 }, _manyElements.InsertRange(0, new[] { 7 }));
            Assert.Equal(new[] { 7, 8, 1, 2, 3 }, _manyElements.InsertRange(0, new[] { 7, 8 }));
        }

        [Fact]
        public void InsertRangeMid()
        {
            Assert.Equal(new[] { 1, 7, 2, 3 }, _manyElements.InsertRange(1, new[] { 7 }));
            Assert.Equal(new[] { 1, 7, 8, 2, 3 }, _manyElements.InsertRange(1, new[] { 7, 8 }));
        }

        [Fact]
        public void InsertRangeRight()
        {
            Assert.Equal(new[] { 1, 2, 3, 7 }, _manyElements.InsertRange(3, new[] { 7 }));
            Assert.Equal(new[] { 1, 2, 3, 7, 8 }, _manyElements.InsertRange(3, new[] { 7, 8 }));
        }

        [Fact]
        public void RemoveAt()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.RemoveAt(0));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _oneElement.RemoveAt(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.RemoveAt(-1));

            Assert.Equal(new int[0], _oneElement.RemoveAt(0));
            Assert.Equal(new[] { 2, 3 }, _manyElements.RemoveAt(0));
            Assert.Equal(new[] { 1, 3 }, _manyElements.RemoveAt(1));
            Assert.Equal(new[] { 1, 2 }, _manyElements.RemoveAt(2));
        }

        [Fact]
        public void Remove()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.Remove(5));
            Assert.False(_empty.Remove(5).IsDefault);

            Assert.True(_oneElement.Remove(1).IsEmpty);
            Assert.Equal(new[] { 2, 3 }, _manyElements.Remove(1));
            Assert.Equal(new[] { 1, 3 }, _manyElements.Remove(2));
            Assert.Equal(new[] { 1, 2 }, _manyElements.Remove(3));
        }

        [Fact]
        public void RemoveRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.RemoveRange(0, 0));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _oneElement.RemoveRange(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.RemoveRange(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _oneElement.RemoveRange(0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => _oneElement.RemoveRange(0, -1));

            var fourElements = ImmutableArray.Create(1, 2, 3, 4);
            Assert.Equal(new int[0], _oneElement.RemoveRange(0, 1));
            Assert.Equal(_oneElement.ToArray(), _oneElement.RemoveRange(0, 0));
            Assert.Equal(new[] { 3, 4 }, fourElements.RemoveRange(0, 2));
            Assert.Equal(new[] { 1, 4 }, fourElements.RemoveRange(1, 2));
            Assert.Equal(new[] { 1, 2 }, fourElements.RemoveRange(2, 2));
        }

        [Fact]
        public void RemoveRangeDefaultStruct()
        {
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(_empty));
            Assert.Throws<ArgumentNullException>(() => Assert.Equal(_empty, _empty.RemoveRange(_emptyDefault)));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(_oneElement));
            Assert.Throws<ArgumentNullException>(() => Assert.Equal(_oneElement, _oneElement.RemoveRange(_emptyDefault)));

            IEnumerable<int> emptyBoxed = _empty;
            IEnumerable<int> emptyDefaultBoxed = _emptyDefault;
            IEnumerable<int> oneElementBoxed = _oneElement;
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(emptyBoxed));
            Assert.Throws<InvalidOperationException>(() => _empty.RemoveRange(emptyDefaultBoxed));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(oneElementBoxed));
            Assert.Throws<InvalidOperationException>(() => _oneElement.RemoveRange(emptyDefaultBoxed));
        }

        [Fact]
        public void RemoveRangeNoOpIdentity()
        {
            Assert.Equal(_empty, _empty.RemoveRange(_empty));
            Assert.Equal(_empty, _empty.RemoveRange(_oneElement)); // struct overload
            Assert.Equal(_empty, _empty.RemoveRange((IEnumerable<int>)_oneElement)); // enumerable overload
            Assert.Equal(_oneElement, _oneElement.RemoveRange(_empty));
        }

        [Fact]
        public void RemoveAll()
        {
            Assert.Throws<ArgumentNullException>(() => _oneElement.RemoveAll(null));

            var array = ImmutableArray.CreateRange(Enumerable.Range(1, 10));
            var removedEvens = array.RemoveAll(n => n % 2 == 0);
            var removedOdds = array.RemoveAll(n => n % 2 == 1);
            var removedAll = array.RemoveAll(n => true);
            var removedNone = array.RemoveAll(n => false);

            Assert.Equal(new[] { 1, 3, 5, 7, 9 }, removedEvens);
            Assert.Equal(new[] { 2, 4, 6, 8, 10 }, removedOdds);
            Assert.True(removedAll.IsEmpty);
            Assert.Equal(Enumerable.Range(1, 10), removedNone);

            Assert.False(_empty.RemoveAll(n => false).IsDefault);
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveAll(n => false));
        }

        [Fact]
        public void RemoveRangeEnumerableTest()
        {
            var list = ImmutableArray.Create(1, 2, 3);
            Assert.Throws<ArgumentNullException>(() => list.RemoveRange(null));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.RemoveRange(new int[0]).IsDefault);
            Assert.False(_empty.RemoveRange(new int[0]).IsDefault);

            ImmutableArray<int> removed2 = list.RemoveRange(new[] { 2 });
            Assert.Equal(2, removed2.Length);
            Assert.Equal(new[] { 1, 3 }, removed2);

            ImmutableArray<int> removed13 = list.RemoveRange(new[] { 1, 3, 5 });
            Assert.Equal(1, removed13.Length);
            Assert.Equal(new[] { 2 }, removed13);

            Assert.Equal(new[] { 1, 3, 6, 8, 9 }, ImmutableArray.CreateRange(Enumerable.Range(1, 10)).RemoveRange(new[] { 2, 4, 5, 7, 10 }));
            Assert.Equal(new[] { 3, 6, 8, 9 }, ImmutableArray.CreateRange(Enumerable.Range(1, 10)).RemoveRange(new[] { 1, 2, 4, 5, 7, 10 }));

            Assert.Equal(list, list.RemoveRange(new[] { 5 }));
            Assert.Equal(ImmutableArray.Create<int>(), ImmutableArray.Create<int>().RemoveRange(new[] { 1 }));

            var listWithDuplicates = ImmutableArray.Create(1, 2, 2, 3);
            Assert.Equal(new[] { 1, 2, 3 }, listWithDuplicates.RemoveRange(new[] { 2 }));
            Assert.Equal(new[] { 1, 3 }, listWithDuplicates.RemoveRange(new[] { 2, 2 }));
            Assert.Equal(new[] { 1, 3 }, listWithDuplicates.RemoveRange(new[] { 2, 2, 2 }));
        }

        [Fact]
        public void Replace()
        {
            Assert.Equal(new[] { 5 }, _oneElement.Replace(1, 5));

            Assert.Equal(new[] { 6, 2, 3 }, _manyElements.Replace(1, 6));
            Assert.Equal(new[] { 1, 6, 3 }, _manyElements.Replace(2, 6));
            Assert.Equal(new[] { 1, 2, 6 }, _manyElements.Replace(3, 6));

            Assert.Equal(new[] { 1, 2, 3, 4 }, ImmutableArray.Create(1, 3, 3, 4).Replace(3, 2));
        }

        [Fact]
        public void ReplaceMissingThrowsTest()
        {
            Assert.Throws<ArgumentException>(() => _empty.Replace(5, 3));
        }

        [Fact]
        public void SetItem()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.SetItem(0, 10));
            Assert.Throws<NullReferenceException>(() => _emptyDefault.SetItem(0, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => _oneElement.SetItem(1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => _empty.SetItem(-1, 10));

            Assert.Equal(new[] { 12345 }, _oneElement.SetItem(0, 12345));
            Assert.Equal(new[] { 12345, 2, 3 }, _manyElements.SetItem(0, 12345));
            Assert.Equal(new[] { 1, 12345, 3 }, _manyElements.SetItem(1, 12345));
            Assert.Equal(new[] { 1, 2, 12345 }, _manyElements.SetItem(2, 12345));
        }

        [Fact]
        public void CopyToArray()
        {
            {
                var target = new int[_manyElements.Length];
                _manyElements.CopyTo(target);
                Assert.Equal(target, _manyElements);
            }

            {
                var target = new int[0];
                Assert.Throws<NullReferenceException>(() => _emptyDefault.CopyTo(target));
            }
        }

        [Fact]
        public void CopyToIntArrayIntInt()
        {
            var source = ImmutableArray.Create(1, 2, 3);
            var target = new int[4];
            source.CopyTo(1, target, 3, 1);
            Assert.Equal(new[] { 0, 0, 0, 2 }, target);
        }

        [Fact]
        public void Concat()
        {
            var array1 = ImmutableArray.Create(1, 2, 3);
            var array2 = ImmutableArray.Create(4, 5, 6);

            var concat = array1.Concat(array2);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, concat);
        }

        /// <summary>
        /// Verifies reuse of the original array when concatenated to an empty array.
        /// </summary>
        [Fact]
        public void ConcatEdgeCases()
        {
            // empty arrays
            Assert.Equal(_manyElements, _manyElements.Concat(_empty));
            Assert.Equal(_manyElements, _empty.Concat(_manyElements));

            // default arrays
            _manyElements.Concat(_emptyDefault);
            Assert.Throws<InvalidOperationException>(() => _manyElements.Concat(_emptyDefault).Count());
            Assert.Throws<InvalidOperationException>(() => _emptyDefault.Concat(_manyElements).Count());
        }

        [Fact]
        public void IsDefault()
        {
            Assert.True(_emptyDefault.IsDefault);
            Assert.False(_empty.IsDefault);
            Assert.False(_oneElement.IsDefault);
        }

        [Fact]
        public void IsDefaultOrEmpty()
        {
            Assert.True(_empty.IsDefaultOrEmpty);
            Assert.True(_emptyDefault.IsDefaultOrEmpty);
            Assert.False(_oneElement.IsDefaultOrEmpty);
        }

        [Fact]
        public void IndexGetter()
        {
            Assert.Equal(1, _oneElement[0]);
            Assert.Equal(1, ((IList)_oneElement)[0]);
            Assert.Equal(1, ((IList<int>)_oneElement)[0]);
            Assert.Equal(1, ((IReadOnlyList<int>)_oneElement)[0]);

            Assert.Throws<IndexOutOfRangeException>(() => _oneElement[1]);
            Assert.Throws<IndexOutOfRangeException>(() => _oneElement[-1]);

            Assert.Throws<NullReferenceException>(() => _emptyDefault[0]);
            Assert.Throws<InvalidOperationException>(() => ((IList)_emptyDefault)[0]);
            Assert.Throws<InvalidOperationException>(() => ((IList<int>)_emptyDefault)[0]);
            Assert.Throws<InvalidOperationException>(() => ((IReadOnlyList<int>)_emptyDefault)[0]);
        }

        [Fact]
        public void ExplicitMethods()
        {
            IList<int> c = _oneElement;
            Assert.Throws<NotSupportedException>(() => c.Add(3));
            Assert.Throws<NotSupportedException>(() => c.Clear());
            Assert.Throws<NotSupportedException>(() => c.Remove(3));
            Assert.True(c.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => c.Insert(0, 2));
            Assert.Throws<NotSupportedException>(() => c.RemoveAt(0));
            Assert.Equal(_oneElement[0], c[0]);
            Assert.Throws<NotSupportedException>(() => c[0] = 8);

            var enumerator = c.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            Assert.Equal(_oneElement[0], enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }

        [Fact]
        public void Sort()
        {
            var array = ImmutableArray.Create(2, 4, 1, 3);
            Assert.Equal(new[] { 1, 2, 3, 4 }, array.Sort());
            Assert.Equal(new[] { 2, 4, 1, 3 }, array); // original array uneffected.
        }

        [Fact]
        public void SortRange()
        {
            var array = ImmutableArray.Create(2, 4, 1, 3);
            Assert.Throws<ArgumentOutOfRangeException>(() => array.Sort(-1, 2, Comparer<int>.Default));
            Assert.Throws<ArgumentOutOfRangeException>(() => array.Sort(1, 4, Comparer<int>.Default));
            Assert.Equal(new int[] { 2, 4, 1, 3 }, array.Sort(array.Length, 0, Comparer<int>.Default));
            Assert.Throws<ArgumentNullException>(() => array.Sort(1, 2, null));
            Assert.Equal(new[] { 2, 1, 4, 3 }, array.Sort(1, 2, Comparer<int>.Default));
        }

        [Fact]
        public void SortComparer()
        {
            var array = ImmutableArray.Create("c", "B", "a");
            Assert.Equal(new[] { "a", "B", "c" }, array.Sort(StringComparer.OrdinalIgnoreCase));
            Assert.Equal(new[] { "B", "a", "c" }, array.Sort(StringComparer.Ordinal));
        }

        [Fact]
        public void SortPreservesArrayWhenAlreadySorted()
        {
            var sortedArray = ImmutableArray.Create(1, 2, 3, 4);
            Assert.Equal(sortedArray, sortedArray.Sort());

            var mostlySorted = ImmutableArray.Create(1, 2, 3, 4, 6, 5, 7, 8, 9, 10);
            Assert.Equal(mostlySorted, mostlySorted.Sort(0, 5, Comparer<int>.Default));
            Assert.Equal(mostlySorted, mostlySorted.Sort(5, 5, Comparer<int>.Default));
            Assert.Equal(Enumerable.Range(1, 10), mostlySorted.Sort(4, 2, Comparer<int>.Default));
        }

        [Fact]
        public void ToBuilder()
        {
            Assert.Equal(0, _empty.ToBuilder().Count);
            Assert.Throws<NullReferenceException>(() => _emptyDefault.ToBuilder().Count);

            var builder = _oneElement.ToBuilder();
            Assert.Equal(_oneElement.ToArray(), builder);

            builder = _manyElements.ToBuilder();
            Assert.Equal(_manyElements.ToArray(), builder);

            // Make sure that changing the builder doesn't change the original immutable array.
            int expected = _manyElements[0];
            builder[0] = expected + 1;
            Assert.Equal(expected, _manyElements[0]);
            Assert.Equal(expected + 1, builder[0]);
        }

        [Fact]
        public void StructuralEquatableEqualsDefault()
        {
            IStructuralEquatable eq = _emptyDefault;

            Assert.True(eq.Equals(_emptyDefault, EqualityComparer<int>.Default));
            Assert.False(eq.Equals(_empty, EqualityComparer<int>.Default));
            Assert.False(eq.Equals(_oneElement, EqualityComparer<int>.Default));
        }

        [Fact]
        public void StructuralEquatableEquals()
        {
            IStructuralEquatable array = new int[3] { 1, 2, 3 };
            IStructuralEquatable immArray = ImmutableArray.Create(1, 2, 3);

            var otherArray = new object[] { 1, 2, 3 };
            var otherImmArray = ImmutableArray.Create(otherArray);
            var unequalArray = new int[] { 1, 2, 4 };
            var unequalImmArray = ImmutableArray.Create(unequalArray);
            var unrelatedArray = new string[3];
            var unrelatedImmArray = ImmutableArray.Create(unrelatedArray);
            var otherList = new List<int> { 1, 2, 3 };
            Assert.Equal(array.Equals(otherArray, EqualityComparer<int>.Default), immArray.Equals(otherImmArray, EqualityComparer<int>.Default));
            Assert.Equal(array.Equals(otherList, EqualityComparer<int>.Default), immArray.Equals(otherList, EqualityComparer<int>.Default));
            Assert.Equal(array.Equals(unrelatedArray, EverythingEqual<object>.Default), immArray.Equals(unrelatedImmArray, EverythingEqual<object>.Default));
            Assert.Equal(array.Equals(new object(), EqualityComparer<int>.Default), immArray.Equals(new object(), EqualityComparer<int>.Default));
            Assert.Equal(array.Equals(null, EqualityComparer<int>.Default), immArray.Equals(null, EqualityComparer<int>.Default));
            Assert.Equal(array.Equals(unequalArray, EqualityComparer<int>.Default), immArray.Equals(unequalImmArray, EqualityComparer<int>.Default));
        }

        [Fact]
        public void StructuralEquatableEqualsArrayInterop()
        {
            IStructuralEquatable array = new int[3] { 1, 2, 3 };
            IStructuralEquatable immArray = ImmutableArray.Create(1, 2, 3);
            var unequalArray = new int[] { 1, 2, 4 };

            Assert.True(immArray.Equals(array, EqualityComparer<int>.Default));
            Assert.False(immArray.Equals(unequalArray, EqualityComparer<int>.Default));
        }

        [Fact]
        public void StructuralEquatableGetHashCodeDefault()
        {
            IStructuralEquatable defaultImmArray = _emptyDefault;
            Assert.Equal(0, defaultImmArray.GetHashCode(EqualityComparer<int>.Default));
        }

        [Fact]
        public void StructuralEquatableGetHashCode()
        {
            IStructuralEquatable emptyArray = new int[0];
            IStructuralEquatable emptyImmArray = _empty;
            IStructuralEquatable array = new int[3] { 1, 2, 3 };
            IStructuralEquatable immArray = ImmutableArray.Create(1, 2, 3);

            Assert.Equal(emptyArray.GetHashCode(EqualityComparer<int>.Default), emptyImmArray.GetHashCode(EqualityComparer<int>.Default));
            Assert.Equal(array.GetHashCode(EqualityComparer<int>.Default), immArray.GetHashCode(EqualityComparer<int>.Default));
            Assert.Equal(array.GetHashCode(EverythingEqual<int>.Default), immArray.GetHashCode(EverythingEqual<int>.Default));
        }

        [Fact]
        public void StructuralComparableDefault()
        {
            IStructuralComparable def = _emptyDefault;
            IStructuralComparable mt = _empty;

            // default to default is fine, and should be seen as equal.
            Assert.Equal(0, def.CompareTo(_emptyDefault, Comparer<int>.Default));

            // default to empty and vice versa should throw, on the basis that 
            // arrays compared that are of different lengths throw. Empty vs. default aren't really compatible.
            Assert.Throws<ArgumentException>(() => def.CompareTo(_empty, Comparer<int>.Default));
            Assert.Throws<ArgumentException>(() => mt.CompareTo(_emptyDefault, Comparer<int>.Default));
        }

        [Fact]
        public void StructuralComparable()
        {
            IStructuralComparable array = new int[3] { 1, 2, 3 };
            IStructuralComparable equalArray = new int[3] { 1, 2, 3 };
            IStructuralComparable immArray = ImmutableArray.Create((int[])array);
            IStructuralComparable equalImmArray = ImmutableArray.Create((int[])equalArray);

            IStructuralComparable longerArray = new int[] { 1, 2, 3, 4 };
            IStructuralComparable longerImmArray = ImmutableArray.Create((int[])longerArray);

            Assert.Equal(array.CompareTo(equalArray, Comparer<int>.Default), immArray.CompareTo(equalImmArray, Comparer<int>.Default));

            Assert.Throws<ArgumentException>(() => array.CompareTo(longerArray, Comparer<int>.Default));
            Assert.Throws<ArgumentException>(() => immArray.CompareTo(longerImmArray, Comparer<int>.Default));

            var list = new List<int> { 1, 2, 3 };
            Assert.Throws<ArgumentException>(() => array.CompareTo(list, Comparer<int>.Default));
            Assert.Throws<ArgumentException>(() => immArray.CompareTo(list, Comparer<int>.Default));
        }

        [Fact]
        public void StructuralComparableArrayInterop()
        {
            IStructuralComparable array = new int[3] { 1, 2, 3 };
            IStructuralComparable equalArray = new int[3] { 1, 2, 3 };
            IStructuralComparable immArray = ImmutableArray.Create((int[])array);
            IStructuralComparable equalImmArray = ImmutableArray.Create((int[])equalArray);

            Assert.Equal(array.CompareTo(equalArray, Comparer<int>.Default), immArray.CompareTo(equalArray, Comparer<int>.Default));
        }

        [Fact]
        public void BinarySearch()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.Equal(Array.BinarySearch(new int[0], 5), ImmutableArray.BinarySearch(default(ImmutableArray<int>), 5)));
            Assert.Equal(Array.BinarySearch(new int[0], 5), ImmutableArray.BinarySearch(ImmutableArray.Create<int>(), 5));
            Assert.Equal(Array.BinarySearch(new int[] { 3 }, 5), ImmutableArray.BinarySearch(ImmutableArray.Create(3), 5));
            Assert.Equal(Array.BinarySearch(new int[] { 5 }, 5), ImmutableArray.BinarySearch(ImmutableArray.Create(5), 5));
        }

        [Fact]
        public void OfType()
        {
            Assert.Equal(0, _emptyDefault.OfType<int>().Count());
            Assert.Equal(0, _empty.OfType<int>().Count());
            Assert.Equal(1, _oneElement.OfType<int>().Count());
            Assert.Equal(1, _twoElementRefTypeWithNull.OfType<string>().Count());
        }

        protected override IEnumerable<T> GetEnumerableOf<T>(params T[] contents)
        {
            return ImmutableArray.Create(contents);
        }

        /// <summary>
        /// A structure that takes exactly 3 bytes of memory.
        /// </summary>
        private struct ThreeByteStruct : IEquatable<ThreeByteStruct>
        {
            public ThreeByteStruct(byte first, byte second, byte third)
            {
                this.Field1 = first;
                this.Field2 = second;
                this.Field3 = third;
            }

            public byte Field1;
            public byte Field2;
            public byte Field3;

            public bool Equals(ThreeByteStruct other)
            {
                return this.Field1 == other.Field1
                    && this.Field2 == other.Field2
                    && this.Field3 == other.Field3;
            }

            public override bool Equals(object obj)
            {
                if (obj is ThreeByteStruct)
                {
                    return this.Equals((ThreeByteStruct)obj);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.Field1;
            }
        }

        /// <summary>
        /// A structure that takes exactly 9 bytes of memory.
        /// </summary>
        private struct NineByteStruct : IEquatable<NineByteStruct>
        {
            public NineByteStruct(int first, int second, int third, int fourth, int fifth, int sixth, int seventh, int eighth, int ninth)
            {
                this.Field1 = (byte)first;
                this.Field2 = (byte)second;
                this.Field3 = (byte)third;
                this.Field4 = (byte)fourth;
                this.Field5 = (byte)fifth;
                this.Field6 = (byte)sixth;
                this.Field7 = (byte)seventh;
                this.Field8 = (byte)eighth;
                this.Field9 = (byte)ninth;
            }

            public byte Field1;
            public byte Field2;
            public byte Field3;
            public byte Field4;
            public byte Field5;
            public byte Field6;
            public byte Field7;
            public byte Field8;
            public byte Field9;

            public bool Equals(NineByteStruct other)
            {
                return this.Field1 == other.Field1
                    && this.Field2 == other.Field2
                    && this.Field3 == other.Field3
                    && this.Field4 == other.Field4
                    && this.Field5 == other.Field5
                    && this.Field6 == other.Field6
                    && this.Field7 == other.Field7
                    && this.Field8 == other.Field8
                    && this.Field9 == other.Field9;
            }

            public override bool Equals(object obj)
            {
                if (obj is NineByteStruct)
                {
                    return this.Equals((NineByteStruct)obj);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.Field1;
            }
        }

        /// <summary>
        /// A structure that requires 9 bytes of memory but occupies 12 because of memory alignment.
        /// </summary>
        private struct TwelveByteStruct : IEquatable<TwelveByteStruct>
        {
            public TwelveByteStruct(int first, int second, byte third)
            {
                this.Field1 = first;
                this.Field2 = second;
                this.Field3 = third;
            }

            public int Field1;
            public int Field2;
            public byte Field3;

            public bool Equals(TwelveByteStruct other)
            {
                return this.Field1 == other.Field1
                    && this.Field2 == other.Field2
                    && this.Field3 == other.Field3;
            }

            public override bool Equals(object obj)
            {
                if (obj is TwelveByteStruct)
                {
                    return this.Equals((TwelveByteStruct)obj);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.Field1;
            }
        }

        private struct StructWithReferenceTypeField
        {
            public string foo;

            public StructWithReferenceTypeField(string foo)
            {
                this.foo = foo;
            }
        }
    }
}
