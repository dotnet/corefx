﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }

            var firstCol = first as ICollection<TSource>;
            if (firstCol != null)
            {
                var secondCol = second as ICollection<TSource>;
                if (secondCol != null)
                {
                    return new Concat2CollectionIterator<TSource>(firstCol, secondCol);
                }
            }

            var concatFirst = first as ConcatIterator<TSource>;
            return concatFirst != null ?
                concatFirst.Concat(second) :
                new Concat2EnumerableIterator<TSource>(first, second);
        }

        private sealed class Concat2EnumerableIterator<TSource> : ConcatIterator<TSource>
        {
            private readonly IEnumerable<TSource> _first;
            private readonly IEnumerable<TSource> _second;

            internal Concat2EnumerableIterator(IEnumerable<TSource> first, IEnumerable<TSource> second)
            {
                Debug.Assert(first != null && second != null);
                _first = first;
                _second = second;
            }

            public override Iterator<TSource> Clone()
            {
                return new Concat2EnumerableIterator<TSource>(_first, _second);
            }

            internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
            {
                return new ConcatNEnumerableIterator<TSource>(this, next, 2);
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                switch (index)
                {
                    case 0: return _first;
                    case 1: return _second;
                    default: return null;
                }
            }
        }

        // To handle chains of >= 3 sources, we chain the concat iterators together and allow
        // GetEnumerable to fetch enumerables from the previous sources.  This means that rather
        // than each MoveNext/Current calls having to traverse all of the previous sources, we
        // only have to traverse all of the previous sources once per chained enumerable.  An
        // alternative would be to use an array to store all of the enumerables, but this has
        // a much better memory profile and without much additional run-time cost.
        private sealed class ConcatNEnumerableIterator<TSource> : ConcatIterator<TSource>
        {
            private readonly ConcatIterator<TSource> _previousConcat;
            private readonly IEnumerable<TSource> _next;
            private readonly int _nextIndex;

            internal ConcatNEnumerableIterator(ConcatIterator<TSource> previousConcat, IEnumerable<TSource> next, int nextIndex)
            {
                Debug.Assert(previousConcat != null);
                Debug.Assert(next != null);
                Debug.Assert(nextIndex >= 2);
                _previousConcat = previousConcat;
                _next = next;
                _nextIndex = nextIndex;
            }

            public override Iterator<TSource> Clone()
            {
                return new ConcatNEnumerableIterator<TSource>(_previousConcat, _next, _nextIndex);
            }

            internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
            {
                if (_nextIndex == int.MaxValue - 2)
                {
                    // In the unlikely case of this many concatenations, if we produced a ConcatNEnumerableIterator
                    // with int.MaxValue then state would overflow before it matched it's index.
                    // So we use the naïve approach of just having a left and right sequence.
                    return new Concat2EnumerableIterator<TSource>(this, next);
                }

                return new ConcatNEnumerableIterator<TSource>(this, next, _nextIndex + 1);
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                if (index > _nextIndex)
                {
                    return null;
                }

                // Walk back through the chain of ConcatNEnumerableIterators looking for the one
                // that has its _nextIndex equal to index.  If we don't find one, then it
                // must be prior to any of them, so call GetEnumerable on the previous
                // Concat2EnumerableIterator.  This avoids a deep recursive call chain.
                ConcatNEnumerableIterator<TSource> current = this;
                while (true)
                {
                    if (index == current._nextIndex)
                    {
                        return current._next;
                    }

                    var prevN = current._previousConcat as ConcatNEnumerableIterator<TSource>;
                    if (prevN != null)
                    {
                        current = prevN;
                        continue;
                    }

                    ConcatIterator<TSource> prev2 = current._previousConcat;
                    Debug.Assert(prev2 is Concat2EnumerableIterator<TSource> || prev2 is Concat2CollectionIterator<TSource>);
                    Debug.Assert(index == 0 || index == 1);
                    return prev2.GetEnumerable(index);
                }
            }
        }

        private sealed class Concat2CollectionIterator<TSource> : ConcatCollectionIterator<TSource>
        {
            private readonly ICollection<TSource> _first;
            private readonly ICollection<TSource> _second;

            internal Concat2CollectionIterator(ICollection<TSource> first, ICollection<TSource> second)
            {
                Debug.Assert(first != null && second != null);
                _first = first;
                _second = second;
            }

            internal override int Count => checked(_first.Count + _second.Count);

            public override Iterator<TSource> Clone()
            {
                return new Concat2CollectionIterator<TSource>(_first, _second);
            }

            internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
            {
                var nextCol = next as ICollection<TSource>;
                if (nextCol != null)
                {
                    return new ConcatNCollectionIterator<TSource>(this, nextCol, 2);
                }
                return new ConcatNEnumerableIterator<TSource>(this, next, 2);
            }

            internal override void CopyToUntil(TSource[] array, int lastIndex)
            {
                Debug.Assert(array != null && array.Length != 0);
                Debug.Assert(lastIndex >= 0 && lastIndex < array.Length);

                int totalCount = lastIndex + 1;
                Debug.Assert(totalCount >= Count);

                int firstCount = _first.Count;
                int secondCount = _second.Count;

                _second.CopyTo(array, totalCount - secondCount);
                _first.CopyTo(array, totalCount - secondCount - firstCount);
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                switch (index)
                {
                    case 0: return _first;
                    case 1: return _second;
                    default: return null;
                }
            }

            public override TSource[] ToArray()
            {
                int firstCount = _first.Count;
                int secondCount = _second.Count;
                int totalCount = firstCount + secondCount;

                if (totalCount == 0)
                {
                    return Array.Empty<TSource>();
                }

                var result = new TSource[totalCount];

                _first.CopyTo(result, 0);
                _second.CopyTo(result, firstCount);

                return result;
            }

            public override List<TSource> ToList()
            {
                int firstCount = _first.Count;
                int secondCount = _second.Count;
                var result = new List<TSource>(firstCount + secondCount);
                
                if (firstCount != 0)
                {
                    result.AddRange(_first);
                }
                if (secondCount != 0)
                {
                    result.AddRange(_second);
                }

                return result;
            }
        }

        private sealed class ConcatNCollectionIterator<TSource> : ConcatCollectionIterator<TSource>
        {
            private readonly ConcatCollectionIterator<TSource> _previous;
            private readonly ICollection<TSource> _next;
            private readonly int _nextIndex;

            internal ConcatNCollectionIterator(ConcatCollectionIterator<TSource> previous, ICollection<TSource> next, int nextIndex)
            {
                Debug.Assert(previous != null);
                Debug.Assert(next != null);
                Debug.Assert(nextIndex >= 2);

                _previous = previous;
                _next = next;
                _nextIndex = nextIndex;
            }

            internal override int Count
            {
                get
                {
                    // Walk the linked list of Concat{2,N}CollectionIterators and call .Count
                    // on each of the collections.
                    // Note that we start from the last collection and make our way to the first,
                    // but the cumulative count will be the same either way.
                    // Though tempting, it is unwise to use recursion here since we could end up
                    // with a stack overflow.
                    
                    int totalCount = _next.Count;
                    ConcatCollectionIterator<TSource> previous = _previous;

                    while (true)
                    {
                        var previousN = previous as ConcatNCollectionIterator<TSource>;
                        if (previousN != null)
                        {
                            checked
                            {
                                totalCount += previousN._next.Count;
                            }
                            previous = previousN._previous;
                            continue;
                        }

                        Debug.Assert(previous is Concat2CollectionIterator<TSource>);
                        return checked(totalCount + previous.Count);
                    }
                }
            }

            public override Iterator<TSource> Clone()
            {
                return new ConcatNCollectionIterator<TSource>(_previous, _next, _nextIndex);
            }

            internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
            {
                var nextCol = next as ICollection<TSource>;
                if (nextCol != null)
                {
                    return new ConcatNCollectionIterator<TSource>(this, nextCol, _nextIndex + 1);
                }
                return new ConcatNEnumerableIterator<TSource>(this, next, _nextIndex + 1);
            }

            internal override void CopyToUntil(TSource[] array, int lastIndex)
            {
                Debug.Assert(array != null && array.Length != 0);
                Debug.Assert(lastIndex >= 0 && lastIndex < array.Length);

                int totalCount = lastIndex + 1;
                Debug.Assert(totalCount >= Count);

                // Copy the items from this collection, which is the last
                // one that was concatenated
                int copied = _next.Count;
                _next.CopyTo(array, totalCount - copied);

                ConcatCollectionIterator<TSource> previous = _previous;

                while (true)
                {
                    var previousN = previous as ConcatNCollectionIterator<TSource>;
                    if (previousN != null)
                    {
                        checked
                        {
                            copied += previousN._next.Count;
                        }
                        previousN._next.CopyTo(array, totalCount - copied);
                        previous = previousN._previous;
                        continue;
                    }

                    // We've reached the first 2 collections that were concatenated
                    Debug.Assert(previous is Concat2CollectionIterator<TSource>);
                    Debug.Assert(previous.Count == Count - copied);

                    // We just called CopyTo *starting from* totalCount - copied, so this will copy all of
                    // the previous elements *until* totalCount - copied - 1. 
                    previous.CopyToUntil(array, totalCount - copied - 1);
                    break;
                }
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                if (index > _nextIndex)
                {
                    return null;
                }

                ConcatNCollectionIterator<TSource> current = this;
                while (true)
                {
                    if (index == current._nextIndex)
                    {
                        return current._next;
                    }

                    var previousN = current._previous as ConcatNCollectionIterator<TSource>;
                    if (previousN != null)
                    {
                        current = previousN;
                        continue;
                    }

                    ConcatCollectionIterator<TSource> previous2 = current._previous;
                    Debug.Assert(previous2 is Concat2CollectionIterator<TSource>);
                    Debug.Assert(index == 0 || index == 1);
                    return previous2.GetEnumerable(index);
                }
            }

            public override TSource[] ToArray()
            {
                int totalCount = Count;
                if (totalCount == 0)
                {
                    return Array.Empty<TSource>();
                }

                var result = new TSource[totalCount];
                CopyToUntil(result, result.Length - 1);
                return result;
            }

            public override List<TSource> ToList()
            {
                return PopulateList(new List<TSource>(Count));
            }
        }

        private abstract class ConcatIterator<TSource> : Iterator<TSource>, IIListProvider<TSource>
        {
            private IEnumerator<TSource> _enumerator;

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                    _enumerator = null;
                }

                base.Dispose();
            }

            internal abstract IEnumerable<TSource> GetEnumerable(int index);

            internal abstract ConcatIterator<TSource> Concat(IEnumerable<TSource> next);

            public override bool MoveNext()
            {
                if (_state == 1)
                {
                    _enumerator = GetEnumerable(0).GetEnumerator();
                    _state = 2;
                }

                if (_state > 1)
                {
                    while (true)
                    {
                        if (_enumerator.MoveNext())
                        {
                            _current = _enumerator.Current;
                            return true;
                        }

                        IEnumerable<TSource> next = GetEnumerable(_state++ - 1);
                        if (next != null)
                        {
                            _enumerator.Dispose();
                            _enumerator = next.GetEnumerator();
                            continue;
                        }

                        Dispose();
                        break;
                    }
                }

                return false;
            }

            public virtual TSource[] ToArray()
            {
                return EnumerableHelpers.ToArray(this);
            }

            public virtual List<TSource> ToList()
            {
                return PopulateList(new List<TSource>());
            }

            public virtual int GetCount(bool onlyIfCheap)
            {
                if (onlyIfCheap)
                {
                    return -1;
                }

                int count = 0;
                for (int i = 0; ; i++)
                {
                    IEnumerable<TSource> source = GetEnumerable(i);
                    if (source == null)
                    {
                        break;
                    }

                    checked
                    {
                        count += source.Count();
                    }
                }

                return count;
            }

            protected List<TSource> PopulateList(List<TSource> list)
            {
                for (int i = 0; ; i++)
                {
                    IEnumerable<TSource> source = GetEnumerable(i);
                    if (source == null)
                    {
                        break;
                    }

                    list.AddRange(source);
                }

                return list;
            }
        }

        private abstract class ConcatCollectionIterator<TSource> : ConcatIterator<TSource>
        {
            internal abstract int Count { get; }

            internal abstract void CopyToUntil(TSource[] array, int lastIndex);

            public override int GetCount(bool onlyIfCheap) => Count; // Should always be cheap
        }
    }
}
