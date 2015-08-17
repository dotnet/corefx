// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using Xunit.Extensions;

namespace System.Linq.Tests
{
    public class SelectTests
    {
        private class ListWrapping<T> : IList<T>
        {
            private readonly List<T> _source;
            
            public ListWrapping(IEnumerable<T> source)
            {
                _source = new List<T>(source);
            }
            
            public int IndexOf(T item)
            {
                return _source.IndexOf(item);
            }
            public void Insert(int index, T item)
            {
                _source.Insert(index, item);
            }
            public void RemoveAt(int index)
            {
                _source.RemoveAt(index);
            }
            public T this[int index]
            {
                get { return _source[index]; }
                set { _source[index] = value; }
            }
            public void Add(T item)
            {
                _source.Add(item);
            }
            public void Clear()
            {
                _source.Clear();
            }
            public bool Contains(T item)
            {
                return _source.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                _source.CopyTo(array, arrayIndex);
            }
            public bool Remove(T item)
            {
                return _source.Remove(item);
            }
            public int Count
            {
                get { return _source.Count; }
            }
            public bool IsReadOnly
            {
                get { return false; }
            }
            public IEnumerator<T> GetEnumerator()
            {
                return _source.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #region Null arguments

        [Fact]
        public void Select_SourceIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> source = null;
            Func<int, int> selector = i => i + 1;

            Assert.Throws<ArgumentNullException>(() => source.Select(selector));
            Assert.Throws<ArgumentNullException>(() => source.Select((x, i) => x + i));
        }

        [Fact]
        public void Select_SelectorIsNull_ArgumentNullExceptionThrown()
        {
            IEnumerable<int> source = Enumerable.Range(1, 10);
            Func<int, int> selector = null;
            Func<int, int, int> indexSelector = null;

            Assert.Throws<ArgumentNullException>(() => source.Select(selector));
            Assert.Throws<ArgumentNullException>(() => source.Select(indexSelector));
        }

        #endregion

        #region Deferred execution

        [Fact]
        public void Select_SourceIsAnArray_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            Func<int>[] source = new Func<int>[] { () => { funcCalled = true; return 1; } };

            IEnumerable<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsAList_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            List<Func<int>> source = new List<Func<int>>() { () => { funcCalled = true; return 1; } };

            IEnumerable<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyCollection<Func<int>> source = new ReadOnlyCollection<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            IEnumerable<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsICollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            ICollection<Func<int>> source = new LinkedList<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            IEnumerable<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void Select_SourceIsIEnumerable_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IEnumerable<Func<int>> source = Enumerable.Repeat((Func<int>)(() => { funcCalled = true; return 1; }), 1);

            IEnumerable<int> query = source.Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ExecutionIsDefered()
        {
            bool funcCalled = false;
            Func<int>[] source = new Func<int>[] { () => { funcCalled = true; return 1; } };

            IEnumerable<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ExecutionIsDefered()
        {
            bool funcCalled = false;
            List<Func<int>> source = new List<Func<int>>() { () => { funcCalled = true; return 1; } };

            IEnumerable<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyCollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            IReadOnlyCollection<Func<int>> source = new ReadOnlyCollection<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            IEnumerable<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsICollection_ExecutionIsDeferred()
        {
            bool funcCalled = false;
            ICollection<Func<int>> source = new LinkedList<Func<int>>(new List<Func<int>>() { () => { funcCalled = true; return 1; } });

            IEnumerable<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        [Fact]
        public void SelectSelect_SourceIsIEnumerable_ExecutionIsDefered()
        {
            bool funcCalled = false;
            IEnumerable<Func<int>> source = Enumerable.Repeat((Func<int>)(() => { funcCalled = true; return 1; }), 1);

            IEnumerable<int> query = source.Select(d => d).Select(d => d());
            Assert.False(funcCalled);
        }

        #endregion

        #region Expected return value

        [Fact]
        public void Select_SourceIsAnArray_ReturnsExpectedValues()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;
            Func<int, int, int> indexSelector = (x, i) => x + i;

            IEnumerable<int> query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);

            query = source.Select(indexSelector);

            index = 0;
            foreach (var item in query)
            {
                var expected = indexSelector(source[index], index);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
            
            index = 0;
            foreach (var item in query.ToArray())
            {
                var expected = indexSelector(source[index], index);
                Assert.Equal(expected, item);
                index++;
            }
        }

        [Fact]
        public void Select_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = new List<int> { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;
            Func<int, int, int> indexSelector = (x, i) => x + i;

            IEnumerable<int> query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(source[index]);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
            
            index = 0;
            foreach (var item in query.ToArray())
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            query = source.Select(indexSelector);

            index = 0;
            foreach (var item in query)
            {
                var expected = indexSelector(source[index], index);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
            
            index = 0;
            foreach (var item in query.ToArray())
            {
                var expected = indexSelector(source[index], index);
                Assert.Equal(expected, item);
                index++;
            }
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_ReturnsExpectedValues()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
            
            index = 0;
            foreach (var item in query.ToArray())
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }
        }

        [Fact]
        public void Select_SourceIsICollection_ReturnsExpectedValues()
        {
            ICollection<int> source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
            
            index = 0;
            foreach (var item in query.ToArray())
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }
        }

        [Fact]
        public void Select_SourceIsIEnumerable_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IEnumerable<int> source = Enumerable.Range(1, nbOfItems);
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(index);
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void Select_SourceIsAnArray_CurrentIsDefaultOfTAfterEnumeration()
        {
            int[] source = new[] { 1 };
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsAList_CurrentIsDefaultOfTAfterEnumeration()
        {
            List<int> source = new List<int>() { 1 };
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsIReadOnlyCollection_CurrentIsDefaultOfTAfterEnumeration()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int>() { 1 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsICollection_CurrentIsDefaultOfTAfterEnumeration()
        {
            ICollection<int> source = new LinkedList<int>(new List<int>() { 1 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void Select_SourceIsIEnumerable_CurrentIsDefaultOfTAfterEnumeration()
        {
            IEnumerable<int> source = Enumerable.Repeat(1, 1);
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector);

            var enumerator = query.GetEnumerator();
            while (enumerator.MoveNext()) ;

            Assert.Equal(default(int), enumerator.Current);
        }

        [Fact]
        public void SelectSelect_SourceIsAnArray_ReturnsExpectedValues()
        {
            Func<int, int> selector = i => i + 1;
            int[] source = new[] { 1, 2, 3, 4, 5 };

            IEnumerable<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Length, index);
        }

        [Fact]
        public void SelectSelect_SourceIsAList_ReturnsExpectedValues()
        {
            List<int> source = new List<int> { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                var expected = selector(selector(source[index]));
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsIReadOnlyCollection_ReturnsExpectedValues()
        {
            IReadOnlyCollection<int> source = new ReadOnlyCollection<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        [Fact]
        public void SelectSelect_SourceIsICollection_ReturnsExpectedValues()
        {
            ICollection<int> source = new LinkedList<int>(new List<int> { 1, 2, 3, 4, 5 });
            Func<int, int> selector = i => i + 1;

            IEnumerable<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(source.Count, index);
        }

        private static IEnumerable<int> OneToFive()
        {
            for(int i = 1; i != 6; ++i)
                yield return i;
        }

        [Fact]
        public void SelectSelect_SourceIsIEnumerable_ReturnsExpectedValues()
        {
            int nbOfItems = 5;
            IEnumerable<int> source = OneToFive();
            Func<int, int> selector = i => i + 1;
            Func<int, int, int> indexSelector = (x, i) => x + i;

            IEnumerable<int> query = source.Select(selector).Select(selector);

            int index = 0;
            foreach (var item in query)
            {
                index++;
                var expected = selector(selector(index));
                Assert.Equal(expected, item);
            }

            Assert.Equal(nbOfItems, index);
            
            query = source.Select(selector).Select(indexSelector);

            index = 0;
            foreach (var item in query)
            {
                var expected = indexSelector(selector(index + 1), index);
                Assert.Equal(expected, item);
                index++;
            }

            Assert.Equal(nbOfItems, index);
        }

        [Fact]
        public void Select_SourceIsEmptyEnumerable_ReturnedCollectionHasNoElements()
        {
            IEnumerable<int> source = Enumerable.Empty<int>();
            bool wasSelectorCalled = false;

            IEnumerable<int> result = source.Select(i => { wasSelectorCalled = true; return i + 1; });

            bool hadItems = false;
            foreach (var item in result)
            {
                hadItems = true;
            }

            Assert.False(hadItems);
            Assert.False(wasSelectorCalled);
        }

        #endregion

        #region Exceptions

        [Fact]
        public void Select_ExceptionThrownFromSelector_ExceptionPropagatedToTheCaller()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => { throw new InvalidOperationException(); };

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void Select_ExceptionThrownFromSelector_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i =>
            {
                if (i == 1)
                    throw new InvalidOperationException();
                return i + 1;
            };

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        /// <summary>
        /// Test enumerator - returns int values from 1 to 5 included.
        /// </summary>
        private class TestEnumerator : IEnumerable<int>, IEnumerator<int>
        {
            private int _current = 0;

            public virtual int Current { get { return _current; } }

            object IEnumerator.Current { get { return Current; } }

            public void Dispose() { }

            public virtual IEnumerator<int> GetEnumerator()
            {
                return this;
            }

            public virtual bool MoveNext()
            {
                return _current++ < 5;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void Select_ExceptionThrownFromCurrentOfSourceIterator_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnCurrent();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void Select_ExceptionThrownFromCurrentOfSourceIterator_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnCurrent();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        /// <summary>
        /// Test enumerator - throws InvalidOperationException from Current after MoveNext called once.
        /// </summary>
        private class ThrowsOnCurrent : TestEnumerator
        {
            public override int Current
            {
                get
                {
                    var current = base.Current;
                    if (current == 1)
                        throw new InvalidOperationException();
                    return current;
                }
            }
        }

        [Fact]
        public void Select_ExceptionThrownFromMoveNextOfSourceIterator_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnMoveNext();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void Select_ExceptionThrownFromMoveNextOfSourceIterator_IteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnMoveNext();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            enumerator.MoveNext();
            Assert.Equal(3 /* 2 + 1 */, enumerator.Current);
        }

        /// <summary>
        /// Test enumerator - throws InvalidOperationException on first call to MoveNext.
        /// </summary>
        private class ThrowsOnMoveNext : TestEnumerator
        {
            public override bool MoveNext()
            {
                bool baseReturn = base.MoveNext();
                if (base.Current == 1)
                    throw new InvalidOperationException();

                return baseReturn;
            }
        }

        [Fact]
        public void Select_ExceptionThrownFromGetEnumeratorOnSource_ExceptionPropagatedToTheCaller()
        {
            IEnumerable<int> source = new ThrowsOnGetEnumerator();
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        [Fact]
        public void Select_ExceptionThrownFromGetEnumeratorOnSource_CurrentIsSetToDefaultOfItemTypeAndIteratorCanBeUsedAfterExceptionIsCaught()
        {
            IEnumerable<int> source = new ThrowsOnGetEnumerator();
            Func<int, string> selector = i => i.ToString();

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
            string currentValue = enumerator.Current;
            Assert.Equal(default(string), currentValue);

            Assert.True(enumerator.MoveNext());
            Assert.Equal("1", enumerator.Current);
        }

        /// <summary>
        /// Test enumerator - throws InvalidOperationException from GetEnumerator when called for the first time.
        /// </summary>
        private class ThrowsOnGetEnumerator : TestEnumerator
        {
            private int getEnumeratorCallCount;
            public override IEnumerator<int> GetEnumerator()
            {
                if (getEnumeratorCallCount++ == 0)
                    throw new InvalidOperationException();

                return base.GetEnumerator();
            }
        }

        [Fact]
        public void Select_SourceListGetsModifiedDuringIteration_ExceptionIsPropagated()
        {
            List<int> source = new List<int>() { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2 /* 1 + 1 */, enumerator.Current);

            source.Add(6);
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
        }

        #endregion

        [Fact]
        public void Select_GetEnumeratorCalledTwice_DifferentInstancesReturned()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };
            var query = source.Select(i => i + 1);

            var enumerator1 = query.GetEnumerator();
            var enumerator2 = query.GetEnumerator();

            Assert.Same(query, enumerator1);
            Assert.NotSame(enumerator1, enumerator2);

            enumerator1.Dispose();
            enumerator2.Dispose();
        }

        [Fact]
        public void Select_ResetCalledOnEnumerator_ExceptionThrown()
        {
            int[] source = new[] { 1, 2, 3, 4, 5 };
            Func<int, int> selector = i => i + 1;

            var result = source.Select(selector);
            var enumerator = result.GetEnumerator();

            Assert.Throws<NotImplementedException>(() => enumerator.Reset());
        }
        
        public static IEnumerable<object[]> SelectedLists
        {
            get
            {
                var arr = new [] { 1, 2 };
                yield return new [] { arr.Select(i => i * 2) } ;
                yield return new [] { arr.Select((x, i) => i + 1 + x) } ;
                yield return new [] { arr.ToList().Select(i => i * 2) } ;
                yield return new [] { arr.ToList().Select((x, i) => i + 1 + x) } ;
                yield return new [] { new ListWrapping<int>(arr).Select(i => i * 2) } ;
            }
        }

        [Theory]
        [MemberData("SelectedLists")]
        public void Select_ResultsReadonly(IList<int> list)
        {
            Assert.True(list.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => list.Add(0));
            Assert.Throws<NotSupportedException>(() => list[0] = 0);
            Assert.Throws<NotSupportedException>(() => list.Remove(0));
            Assert.Throws<NotSupportedException>(() => list.Remove(0));
            Assert.Throws<NotSupportedException>(() => list.Clear());
            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.Throws<NotSupportedException>(() => list.Insert(0, 0));
        }
        
        [Theory]
        [MemberData("SelectedLists")]
        private static void TestIList(IList<int> list)
        {
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0]);
            Assert.Equal(4, list[1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[-1].ToString());
            Assert.Throws<ArgumentOutOfRangeException>(() => list[2].ToString());
            var targetArray = new int[4];
            list.CopyTo(targetArray, 1);
            Assert.Equal(new int[]{ 0, 2, 4, 0 }, targetArray);
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(targetArray, -1));
            Assert.Throws<ArgumentException>(() => list.CopyTo(targetArray, 3));
            Assert.Equal(-1, list.IndexOf(3));
            Assert.Equal(1, list.IndexOf(4));
            Assert.False(list.Contains(3));
            Assert.True(list.Contains(4));
        }
    }
}
