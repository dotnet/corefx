// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Collections.Immutable.Test
{
    public class ImmutableArrayExtensionsTest
    {
        private static readonly ImmutableArray<int> _emptyDefault = default(ImmutableArray<int>);
        private static readonly ImmutableArray<int> _empty = ImmutableArray.Create<int>();
        private static readonly ImmutableArray<int> _oneElement = ImmutableArray.Create(1);
        private static readonly ImmutableArray<int> _manyElements = ImmutableArray.Create(1, 2, 3);
        private static readonly ImmutableArray<GenericParameterHelper> _oneElementRefType = ImmutableArray.Create(new GenericParameterHelper(1));
        private static readonly ImmutableArray<string> _twoElementRefTypeWithNull = ImmutableArray.Create("1", null);

        private static readonly ImmutableArray<int>.Builder _emptyBuilder = ImmutableArray.Create<int>().ToBuilder();
        private static readonly ImmutableArray<int>.Builder _oneElementBuilder = ImmutableArray.Create<int>(1).ToBuilder();
        private static readonly ImmutableArray<int>.Builder _manyElementsBuilder = ImmutableArray.Create<int>(1, 2, 3).ToBuilder();

        [Fact]
        public void Select()
        {
            Assert.Equal(new[] { 4, 5, 6 }, ImmutableArrayExtensions.Select(_manyElements, n => n + 3));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Select<int, bool>(_manyElements, null));
        }

        [Fact]
        public void SelectEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Select<int, bool>(_emptyDefault, null));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Select(_emptyDefault, n => true));
        }

        [Fact]
        public void SelectEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Select<int, bool>(_empty, null));
            Assert.False(ImmutableArrayExtensions.Select(_empty, n => true).Any());
        }

        [Fact]
        public void SelectMany()
        {
            Func<int, IEnumerable<int>> collectionSelector = i => Enumerable.Range(i, 10);
            Func<int, int, int> resultSelector = (i, e) => e * 2;
            foreach (var arr in new[] { _empty, _oneElement, _manyElements })
            {
                Assert.Equal(
                    Enumerable.SelectMany(arr, collectionSelector, resultSelector),
                    ImmutableArrayExtensions.SelectMany(arr, collectionSelector, resultSelector));
            }

            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SelectMany<int, int, int>(_emptyDefault, null, null));
            Assert.Throws<ArgumentNullException>(() =>
                ImmutableArrayExtensions.SelectMany<int, int, int>(_manyElements, null, (i, e) => e));
            Assert.Throws<ArgumentNullException>(() =>
                ImmutableArrayExtensions.SelectMany<int, int, int>(_manyElements, i => new[] { i }, null));
        }

        [Fact]
        public void Where()
        {
            Assert.Equal(new[] { 2, 3 }, ImmutableArrayExtensions.Where(_manyElements, n => n > 1));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Where(_manyElements, null));
        }

        [Fact]
        public void WhereEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Where(_emptyDefault, null));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Where(_emptyDefault, n => true));
        }

        [Fact]
        public void WhereEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Where(_empty, null));
            Assert.False(ImmutableArrayExtensions.Where(_empty, n => true).Any());
        }

        [Fact]
        public void Any()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Any(_oneElement, null));
            Assert.True(ImmutableArrayExtensions.Any(_oneElement));
            Assert.True(ImmutableArrayExtensions.Any(_manyElements, n => n == 2));
            Assert.False(ImmutableArrayExtensions.Any(_manyElements, n => n == 4));
            Assert.True(ImmutableArrayExtensions.Any(_oneElementBuilder));
        }

        [Fact]
        public void AnyEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Any(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Any(_emptyDefault, n => true));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Any(_emptyDefault, null));
        }

        [Fact]
        public void AnyEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Any(_empty, null));
            Assert.False(ImmutableArrayExtensions.Any(_empty));
            Assert.False(ImmutableArrayExtensions.Any(_empty, n => true));
        }

        [Fact]
        public void All()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.All(_oneElement, null));
            Assert.False(ImmutableArrayExtensions.All(_manyElements, n => n == 2));
            Assert.True(ImmutableArrayExtensions.All(_manyElements, n => n > 0));
        }

        [Fact]
        public void AllEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.All(_emptyDefault, n => true));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.All(_emptyDefault, null));
        }

        [Fact]
        public void AllEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.All(_empty, null));
            Assert.True(ImmutableArrayExtensions.All(_empty, n => { Assert.True(false); return false; })); // predicate should never be invoked.
        }

        [Fact]
        public void SequenceEqual()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SequenceEqual(_oneElement, (IEnumerable<int>)null));

            foreach (IEqualityComparer<int> comparer in new[] { null, EqualityComparer<int>.Default })
            {
                Assert.True(ImmutableArrayExtensions.SequenceEqual(_manyElements, _manyElements, comparer));
                Assert.True(ImmutableArrayExtensions.SequenceEqual(_manyElements, (IEnumerable<int>)_manyElements.ToArray(), comparer));
                Assert.True(ImmutableArrayExtensions.SequenceEqual(_manyElements, ImmutableArray.Create(_manyElements.ToArray()), comparer));

                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, _oneElement, comparer));
                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, (IEnumerable<int>)_oneElement.ToArray(), comparer));
                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, ImmutableArray.Create(_oneElement.ToArray()), comparer));
                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, (IEnumerable<int>)_manyElements.Add(1).ToArray(), comparer));
                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements.Add(1), _manyElements.Add(2).ToArray(), comparer));
                Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements.Add(1), (IEnumerable<int>)_manyElements.Add(2).ToArray(), comparer));
            }

            Assert.True(ImmutableArrayExtensions.SequenceEqual(_manyElements, _manyElements, (a, b) => true));

            Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, _oneElement, (a, b) => a == b));
            Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements.Add(1), _manyElements.Add(2), (a, b) => a == b));
            Assert.True(ImmutableArrayExtensions.SequenceEqual(_manyElements.Add(1), _manyElements.Add(1), (a, b) => a == b));

            Assert.False(ImmutableArrayExtensions.SequenceEqual(_manyElements, ImmutableArray.Create(_manyElements.ToArray()), (a, b) => false));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SequenceEqual(_oneElement, _oneElement, (Func<int, int, bool>)null));
        }

        [Fact]
        public void SequenceEqualEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SequenceEqual(_oneElement, _emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SequenceEqual(_emptyDefault, _empty));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SequenceEqual(_emptyDefault, _emptyDefault));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SequenceEqual(_emptyDefault, _emptyDefault, (Func<int, int, bool>)null));
        }

        [Fact]
        public void SequenceEqualEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SequenceEqual(_empty, (IEnumerable<int>)null));
            Assert.True(ImmutableArrayExtensions.SequenceEqual(_empty, _empty));
            Assert.True(ImmutableArrayExtensions.SequenceEqual(_empty, _empty.ToArray()));
            Assert.True(ImmutableArrayExtensions.SequenceEqual(_empty, _empty, (a, b) => true));
            Assert.True(ImmutableArrayExtensions.SequenceEqual(_empty, _empty, (a, b) => false));
        }

        [Fact]
        public void Aggregate()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Aggregate(_oneElement, null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Aggregate(_oneElement, 1, null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Aggregate<int, int, int>(_oneElement, 1, null, null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Aggregate<int, int, int>(_oneElement, 1, (a, b) => a + b, null));

            Assert.Equal(Enumerable.Aggregate(_manyElements, (a, b) => a * b), ImmutableArrayExtensions.Aggregate(_manyElements, (a, b) => a * b));
            Assert.Equal(Enumerable.Aggregate(_manyElements, 5, (a, b) => a * b), ImmutableArrayExtensions.Aggregate(_manyElements, 5, (a, b) => a * b));
            Assert.Equal(Enumerable.Aggregate(_manyElements, 5, (a, b) => a * b, a => -a), ImmutableArrayExtensions.Aggregate(_manyElements, 5, (a, b) => a * b, a => -a));
        }

        [Fact]
        public void AggregateEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Aggregate(_emptyDefault, (a, b) => a + b));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Aggregate(_emptyDefault, 1, (a, b) => a + b));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Aggregate<int, int, int>(_emptyDefault, 1, (a, b) => a + b, a => a));
        }

        [Fact]
        public void AggregateEmpty()
        {
            Assert.Equal(0, ImmutableArrayExtensions.Aggregate(_empty, (a, b) => a + b));
            Assert.Equal(1, ImmutableArrayExtensions.Aggregate(_empty, 1, (a, b) => a + b));
            Assert.Equal(1, ImmutableArrayExtensions.Aggregate<int, int, int>(_empty, 1, (a, b) => a + b, a => a));
        }

        [Fact]
        public void ElementAt()
        {
            // Basis for some assertions that follow
            Assert.Throws<IndexOutOfRangeException>(() => Enumerable.ElementAt(_empty, 0));
            Assert.Throws<IndexOutOfRangeException>(() => Enumerable.ElementAt(_manyElements, -1));

            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ElementAt(_emptyDefault, 0));
            Assert.Throws<IndexOutOfRangeException>(() => ImmutableArrayExtensions.ElementAt(_empty, 0));
            Assert.Throws<IndexOutOfRangeException>(() => ImmutableArrayExtensions.ElementAt(_manyElements, -1));
            Assert.Equal(1, ImmutableArrayExtensions.ElementAt(_oneElement, 0));
            Assert.Equal(3, ImmutableArrayExtensions.ElementAt(_manyElements, 2));
        }

        [Fact]
        public void ElementAtOrDefault()
        {
            Assert.Equal(Enumerable.ElementAtOrDefault(_manyElements, -1), ImmutableArrayExtensions.ElementAtOrDefault(_manyElements, -1));
            Assert.Equal(Enumerable.ElementAtOrDefault(_manyElements, 3), ImmutableArrayExtensions.ElementAtOrDefault(_manyElements, 3));

            Assert.Throws<InvalidOperationException>(() => Enumerable.ElementAtOrDefault(_emptyDefault, 0));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ElementAtOrDefault(_emptyDefault, 0));

            Assert.Equal(0, ImmutableArrayExtensions.ElementAtOrDefault(_empty, 0));
            Assert.Equal(0, ImmutableArrayExtensions.ElementAtOrDefault(_empty, 1));
            Assert.Equal(1, ImmutableArrayExtensions.ElementAtOrDefault(_oneElement, 0));
            Assert.Equal(3, ImmutableArrayExtensions.ElementAtOrDefault(_manyElements, 2));
        }

        [Fact]
        public void First()
        {
            Assert.Equal(Enumerable.First(_oneElement), ImmutableArrayExtensions.First(_oneElement));
            Assert.Equal(Enumerable.First(_oneElement, i => true), ImmutableArrayExtensions.First(_oneElement, i => true));

            Assert.Equal(Enumerable.First(_manyElements), ImmutableArrayExtensions.First(_manyElements));
            Assert.Equal(Enumerable.First(_manyElements, i => true), ImmutableArrayExtensions.First(_manyElements, i => true));

            Assert.Equal(Enumerable.First(_oneElementBuilder), ImmutableArrayExtensions.First(_oneElementBuilder));
            Assert.Equal(Enumerable.First(_manyElementsBuilder), ImmutableArrayExtensions.First(_manyElementsBuilder));

            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_empty));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_empty, i => true));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_manyElements, i => false));
        }

        [Fact]
        public void FirstEmpty()
        {
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_empty));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.First(_empty, null));

            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.First(_emptyBuilder));
        }

        [Fact]
        public void FirstEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.First(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.First(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.First(_emptyDefault, null));
        }

        [Fact]
        public void FirstOrDefault()
        {
            Assert.Equal(Enumerable.FirstOrDefault(_oneElement), ImmutableArrayExtensions.FirstOrDefault(_oneElement));
            Assert.Equal(Enumerable.FirstOrDefault(_manyElements), ImmutableArrayExtensions.FirstOrDefault(_manyElements));

            foreach (bool result in new[] { true, false })
            {
                Assert.Equal(Enumerable.FirstOrDefault(_oneElement, i => result), ImmutableArrayExtensions.FirstOrDefault(_oneElement, i => result));
                Assert.Equal(Enumerable.FirstOrDefault(_manyElements, i => result), ImmutableArrayExtensions.FirstOrDefault(_manyElements, i => result));
            }
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.FirstOrDefault(_oneElement, null));

            Assert.Equal(Enumerable.FirstOrDefault(_oneElementBuilder), ImmutableArrayExtensions.FirstOrDefault(_oneElementBuilder));
            Assert.Equal(Enumerable.FirstOrDefault(_manyElementsBuilder), ImmutableArrayExtensions.FirstOrDefault(_manyElementsBuilder));
        }

        [Fact]
        public void FirstOrDefaultEmpty()
        {
            Assert.Equal(0, ImmutableArrayExtensions.FirstOrDefault(_empty));
            Assert.Equal(0, ImmutableArrayExtensions.FirstOrDefault(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.FirstOrDefault(_empty, null));

            Assert.Equal(0, ImmutableArrayExtensions.FirstOrDefault(_emptyBuilder));
        }

        [Fact]
        public void FirstOrDefaultEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.FirstOrDefault(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.FirstOrDefault(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.FirstOrDefault(_emptyDefault, null));
        }

        [Fact]
        public void Last()
        {
            Assert.Equal(Enumerable.Last(_oneElement), ImmutableArrayExtensions.Last(_oneElement));
            Assert.Equal(Enumerable.Last(_oneElement, i => true), ImmutableArrayExtensions.Last(_oneElement, i => true));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Last(_oneElement, i => false));

            Assert.Equal(Enumerable.Last(_manyElements), ImmutableArrayExtensions.Last(_manyElements));
            Assert.Equal(Enumerable.Last(_manyElements, i => true), ImmutableArrayExtensions.Last(_manyElements, i => true));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Last(_manyElements, i => false));

            Assert.Equal(Enumerable.Last(_oneElementBuilder), ImmutableArrayExtensions.Last(_oneElementBuilder));
            Assert.Equal(Enumerable.Last(_manyElementsBuilder), ImmutableArrayExtensions.Last(_manyElementsBuilder));
        }


        [Fact]
        public void LastEmpty()
        {
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Last(_empty));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Last(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Last(_empty, null));

            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Last(_emptyBuilder));
        }


        [Fact]
        public void LastEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Last(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Last(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Last(_emptyDefault, null));
        }

        [Fact]
        public void LastOrDefault()
        {
            Assert.Equal(Enumerable.LastOrDefault(_oneElement), ImmutableArrayExtensions.LastOrDefault(_oneElement));
            Assert.Equal(Enumerable.LastOrDefault(_manyElements), ImmutableArrayExtensions.LastOrDefault(_manyElements));

            foreach (bool result in new[] { true, false })
            {
                Assert.Equal(Enumerable.LastOrDefault(_oneElement, i => result), ImmutableArrayExtensions.LastOrDefault(_oneElement, i => result));
                Assert.Equal(Enumerable.LastOrDefault(_manyElements, i => result), ImmutableArrayExtensions.LastOrDefault(_manyElements, i => result));
            }
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.LastOrDefault(_oneElement, null));

            Assert.Equal(Enumerable.LastOrDefault(_oneElementBuilder), ImmutableArrayExtensions.LastOrDefault(_oneElementBuilder));
            Assert.Equal(Enumerable.LastOrDefault(_manyElementsBuilder), ImmutableArrayExtensions.LastOrDefault(_manyElementsBuilder));
        }

        [Fact]
        public void LastOrDefaultEmpty()
        {
            Assert.Equal(0, ImmutableArrayExtensions.LastOrDefault(_empty));
            Assert.Equal(0, ImmutableArrayExtensions.LastOrDefault(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.LastOrDefault(_empty, null));

            Assert.Equal(0, ImmutableArrayExtensions.LastOrDefault(_emptyBuilder));
        }

        [Fact]
        public void LastOrDefaultEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.LastOrDefault(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.LastOrDefault(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.LastOrDefault(_emptyDefault, null));
        }

        [Fact]
        public void Single()
        {
            Assert.Equal(Enumerable.Single(_oneElement), ImmutableArrayExtensions.Single(_oneElement));
            Assert.Equal(Enumerable.Single(_oneElement), ImmutableArrayExtensions.Single(_oneElement, i => true));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_manyElements));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_manyElements, i => true));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_manyElements, i => false));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_oneElement, i => false));
        }

        [Fact]
        public void SingleEmpty()
        {
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_empty));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.Single(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Single(_empty, null));
        }

        [Fact]
        public void SingleEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Single(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.Single(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.Single(_emptyDefault, null));
        }

        [Fact]
        public void SingleOrDefault()
        {
            Assert.Equal(Enumerable.SingleOrDefault(_oneElement), ImmutableArrayExtensions.SingleOrDefault(_oneElement));
            Assert.Equal(Enumerable.SingleOrDefault(_oneElement), ImmutableArrayExtensions.SingleOrDefault(_oneElement, i => true));
            Assert.Equal(Enumerable.SingleOrDefault(_oneElement, i => false), ImmutableArrayExtensions.SingleOrDefault(_oneElement, i => false));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.SingleOrDefault(_manyElements));
            Assert.Throws<InvalidOperationException>(() => ImmutableArrayExtensions.SingleOrDefault(_manyElements, i => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SingleOrDefault(_oneElement, null));
        }

        [Fact]
        public void SingleOrDefaultEmpty()
        {
            Assert.Equal(0, ImmutableArrayExtensions.SingleOrDefault(_empty));
            Assert.Equal(0, ImmutableArrayExtensions.SingleOrDefault(_empty, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SingleOrDefault(_empty, null));
        }

        [Fact]
        public void SingleOrDefaultEmptyDefault()
        {
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SingleOrDefault(_emptyDefault));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.SingleOrDefault(_emptyDefault, n => true));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.SingleOrDefault(_emptyDefault, null));
        }

        [Fact]
        public void ToDictionary()
        {
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.ToDictionary(_manyElements, (Func<int, int>)null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.ToDictionary(_manyElements, (Func<int, int>)null, n => n));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.ToDictionary(_manyElements, (Func<int, int>)null, n => n, EqualityComparer<int>.Default));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.ToDictionary(_manyElements, n => n, (Func<int, string>)null));
            Assert.Throws<ArgumentNullException>(() => ImmutableArrayExtensions.ToDictionary(_manyElements, n => n, (Func<int, string>)null, EqualityComparer<int>.Default));

            var stringToString = ImmutableArrayExtensions.ToDictionary(_manyElements, n => n.ToString(), n => (n * 2).ToString());
            Assert.Equal(stringToString.Count, _manyElements.Length);
            Assert.Equal("2", stringToString["1"]);
            Assert.Equal("4", stringToString["2"]);
            Assert.Equal("6", stringToString["3"]);

            var stringToInt = ImmutableArrayExtensions.ToDictionary(_manyElements, n => n.ToString());
            Assert.Equal(stringToString.Count, _manyElements.Length);
            Assert.Equal(1, stringToInt["1"]);
            Assert.Equal(2, stringToInt["2"]);
            Assert.Equal(3, stringToInt["3"]);

            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ToDictionary(_emptyDefault, n => n));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ToDictionary(_emptyDefault, n => n, n => n));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ToDictionary(_emptyDefault, n => n, EqualityComparer<int>.Default));
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ToDictionary(_emptyDefault, n => n, n => n, EqualityComparer<int>.Default));
        }

        [Fact]
        public void ToArray()
        {
            Assert.Equal(0, ImmutableArrayExtensions.ToArray(_empty).Length);
            Assert.Throws<NullReferenceException>(() => ImmutableArrayExtensions.ToArray(_emptyDefault));
            Assert.Equal(_manyElements.ToArray(), ImmutableArrayExtensions.ToArray(_manyElements));
        }
    }
}
