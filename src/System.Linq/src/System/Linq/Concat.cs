// Licensed to the .NET Foundation under one or more agreements.
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

            Debug.Assert(!(first is ICollection<TSource> && first is ConcatIterator<TSource>), "Didn't expect enumerable to be both a collection and a concat iterator.");

            var firstCollection = first as ICollection<TSource>;
            if (firstCollection != null)
            {
                var secondCollection = second as ICollection<TSource>;
                if (secondCollection != null)
                {
                    return new Concat2CollectionIterator<TSource>(firstCollection, secondCollection);
                }
                // We know first is an ICollection, second isn't.
            }
            else
            {
                // We know first isn't an ICollection.
                var firstConcat = first as ConcatIterator<TSource>;
                if (firstConcat != null)
                {
                    return firstConcat.ConcatAfter(second);
                }
                // We know first is neither an ICollection nor a ConcatIterator.
            }

            var secondConcat = second as ConcatIterator<TSource>;
            if (secondConcat != null)
            {
                // We know first is not a ConcatIterator and second is a ConcatIterator.
                Debug.Assert(!(first is ConcatIterator<TSource>));
                return secondConcat.ConcatBefore(first);
            }

            return new Concat2EnumerableIterator<TSource>(first, second);
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

            internal override ConcatIterator<TSource> ConcatAfter(IEnumerable<TSource> next)
            {
                return new ConcatNEnumerableIterator<TSource>(this, next, 2);
            }

            internal override ConcatIterator<TSource> ConcatBefore(IEnumerable<TSource> previous)
            {
                return new ConcatNEnumerableIterator<TSource>(this, previous, 2 | int.MinValue);
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
        // GetEnumerable to fetch enumerables from the other sources.  This means that rather
        // than each MoveNext/Current calls having to traverse all of the other sources, we
        // only have to traverse all of the other sources once per chained enumerable.  An
        // alternative would be to use an array to store all of the enumerables, but this has
        // a much better memory profile and without much additional run-time cost.
        private sealed class ConcatNEnumerableIterator<TSource> : ConcatIterator<TSource>
        {
            private readonly ConcatIterator<TSource> _linked; // Singly-inked list of other enumerables being concatenated.
            private readonly IEnumerable<TSource> _concatee; // The enumerable represented by this node.
            private readonly int _index; // Sign bit is set if we're adding _concatee before other enumerables (prepending),
                                         // otherwise we're adding it after other enumerables (appending).
                                         // The rest of the int (_index & int.MaxValue) represents how many Concat calls
                                         // have been chained, e.g. for a.Concat(b).Concat(c) or a.Concat(b.Concat(c))
                                         // this number will be 2.
                                         // For appended nodes which are at the head of the linked list, this will also
                                         // be equal to their index in the list (the last index).

            internal ConcatNEnumerableIterator(ConcatIterator<TSource> linked, IEnumerable<TSource> concatee, int index)
            {
                Debug.Assert(linked != null);
                Debug.Assert(concatee != null);
                Debug.Assert((index & int.MaxValue) >= 2);

                _linked = linked;
                _concatee = concatee;
                _index = index;
            }

            private bool IsAppended => _index >= 0;

            public override Iterator<TSource> Clone()
            {
                return new ConcatNEnumerableIterator<TSource>(_linked, _concatee, _index);
            }

            internal override ConcatIterator<TSource> ConcatAfter(IEnumerable<TSource> next)
            {
                if (_index == int.MaxValue - 2)
                {
                    // In the unlikely case of this many concatenations, if we produced a ConcatNEnumerableIterator
                    // with int.MaxValue then state would overflow before it matched its index.
                    // So we use the naïve approach of just having a left and right sequence.
                    return new Concat2EnumerableIterator<TSource>(this, next);
                }

                return new ConcatNEnumerableIterator<TSource>(this, next, (_index & int.MaxValue) + 1);
            }

            internal override ConcatIterator<TSource> ConcatBefore(IEnumerable<TSource> previous)
            {
                return new ConcatNEnumerableIterator<TSource>(this, previous, (_index | int.MinValue) + 1);
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                // We need to walk back through the chain of ConcatNIterators to
                // look for the iterator that corresponds to a certain index.
                // The rules for this are as follows:
                // - If the current node is an appended node, meaning this concatee
                //   comes last, then the result should be _concatee if index == _index.
                //   Otherwise, it should be the same as _linked.GetEnumerable(index).
                // - If the current node is a prepended node, meaning this concatee
                //   comes first, then the result should be _concatee if index == 0.
                //   Otherwise, it should be the same as _linked.GetEnumerable(index - 1).

                // Another thing we have to do is stop at the first appended node we
                // encounter (including this), which will hold the last concatee.
                // If index is greater than that node's index at that point, then we
                // need to return null to indicate that there are no more concatees.

                ConcatNEnumerableIterator<TSource> current = this;
                while (true)
                {
                    Debug.Assert(index >= 0);

                    if (current.IsAppended)
                    {
                        if (index >= current._index)
                        {
                            return index > current._index ? null : current._concatee;
                        }
                    }
                    else if (index-- == 0)
                    {
                        return current._concatee;
                    }

                    ConcatIterator<TSource> linked = current._linked;

                    var linkedEnumerables = linked as ConcatNEnumerableIterator<TSource>;
                    if (linkedEnumerables != null)
                    {
                        current = linkedEnumerables;
                        continue;
                    }

                    var linkedCollections = linked as ConcatNCollectionIterator<TSource>;
                    if (linkedCollections != null)
                    {
                        // Since ConcatNCollectionIterator.GetEnumerable does not call into this method,
                        // it is safe to call GetEnumerable on it here. It also makes things faster, since
                        // the above type-cast will only ever be run once per call of this method.
                        return linkedCollections.GetEnumerable(index);
                    }

                    // We've reached the tail of the linked list, which contains the first 2 enumerables.
                    Debug.Assert(linked is Concat2EnumerableIterator<TSource> || linked is Concat2CollectionIterator<TSource>);
                    Debug.Assert(index >= 0 && index <= 2); // index can be 2 if we prepend to a Concat2Iterator.
                    return linked.GetEnumerable(index);
                }
            }
        }

        private sealed class Concat2CollectionIterator<TSource> : ConcatIterator<TSource>
        {
            private readonly ICollection<TSource> _first;
            private readonly ICollection<TSource> _second;

            internal Concat2CollectionIterator(ICollection<TSource> first, ICollection<TSource> second)
            {
                Debug.Assert(first != null && second != null);
                _first = first;
                _second = second;
            }

            internal int Count => checked(_first.Count + _second.Count);

            public override Iterator<TSource> Clone()
            {
                return new Concat2CollectionIterator<TSource>(_first, _second);
            }

            internal override ConcatIterator<TSource> ConcatAfter(IEnumerable<TSource> next)
            {
                var nextCollection = next as ICollection<TSource>;
                if (nextCollection != null)
                {
                    return new ConcatNCollectionIterator<TSource>(this, nextCollection, 2);
                }
                return new ConcatNEnumerableIterator<TSource>(this, next, 2);
            }

            internal override ConcatIterator<TSource> ConcatBefore(IEnumerable<TSource> previous)
            {
                var previousCollection = previous as ICollection<TSource>;
                if (previousCollection != null)
                {
                    return new ConcatNCollectionIterator<TSource>(this, previousCollection, 2 | int.MinValue);
                }
                return new ConcatNEnumerableIterator<TSource>(this, previous, 2 | int.MinValue);
            }

            internal void CopyTo(TSource[] array, int arrayIndex)
            {
                Debug.Assert(array != null);
                Debug.Assert(arrayIndex >= 0);
                Debug.Assert(array.Length - arrayIndex >= Count);

                _first.CopyTo(array, arrayIndex);
                _second.CopyTo(array, arrayIndex + _first.Count);
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
                int firstCount = _first.Count; // Cache an interface method call
                int totalCount = checked(firstCount + _second.Count);

                if (totalCount == 0)
                {
                    return Array.Empty<TSource>();
                }

                var result = new TSource[totalCount];

                _first.CopyTo(result, 0);
                _second.CopyTo(result, firstCount);

                return result;
            }

            public override int GetCount(bool onlyIfCheap) => Count; // Getting the count is always cheap.
        }

        private sealed class ConcatNCollectionIterator<TSource> : ConcatIterator<TSource>
        {
            private readonly ConcatIterator<TSource> _linked;
            private readonly ICollection<TSource> _concatee;
            private readonly int _index;

            internal ConcatNCollectionIterator(ConcatIterator<TSource> linked, ICollection<TSource> concatee, int index)
            {
                Debug.Assert(linked != null);
                // A collection iterator should only be linked to other collection iterators
                Debug.Assert(linked is Concat2CollectionIterator<TSource> || linked is ConcatNCollectionIterator<TSource>);
                Debug.Assert(concatee != null);
                Debug.Assert((index & int.MaxValue) >= 2);

                _linked = linked;
                _concatee = concatee;
                _index = index;
            }

            private int Count
            {
                get
                {
                    // Walk the linked list of Concat{2,N}CollectionIterators and call .Count
                    // on each of the collections.
                    // It's possible that we may not start from the first nor end at the last collection,
                    // but the cumulative count will be the same either way.
                    
                    int totalCount = _concatee.Count;
                    ConcatIterator<TSource> linked = _linked;

                    ConcatNCollectionIterator<TSource> linkedN;
                    while ((linkedN = linked as ConcatNCollectionIterator<TSource>) != null)
                    {
                        checked
                        {
                            totalCount += linkedN._concatee.Count;
                        }
                        linked = linkedN._linked;
                    }

                    var linked2 = (Concat2CollectionIterator<TSource>)linked;
                    return checked(totalCount + linked2.Count);
                }
            }

            private bool IsAppended => _index >= 0;

            public override Iterator<TSource> Clone()
            {
                return new ConcatNCollectionIterator<TSource>(_linked, _concatee, _index);
            }

            internal override ConcatIterator<TSource> ConcatAfter(IEnumerable<TSource> next)
            {
                var nextCollection = next as ICollection<TSource>;
                if (nextCollection != null)
                {
                    if (_index == int.MaxValue - 2)
                    {
                        // In the unlikely case of this many concatenations, if we produced a ConcatNCollectionIterator
                        // with int.MaxValue then state would overflow before it matched its index.
                        // So we use the naïve approach of just having a left and right sequence.
                        return new Concat2EnumerableIterator<TSource>(this, next);
                    }

                    return new ConcatNCollectionIterator<TSource>(this, nextCollection, (_index & int.MaxValue) + 1);
                }
                
                // If we encounter a non-ICollection then getting .Count and performing .ToArray()
                // will no longer be cheap, due to enumerables' lazy nature. So, fall back to using
                // enumerable-based iterators for any further chaining.
                return new ConcatNEnumerableIterator<TSource>(this, next, (_index & int.MaxValue) + 1);
            }

            internal override ConcatIterator<TSource> ConcatBefore(IEnumerable<TSource> previous)
            {
                var previousCollection = previous as ICollection<TSource>;
                if (previousCollection != null)
                {
                    return new ConcatNCollectionIterator<TSource>(this, previousCollection, (_index | int.MinValue) + 1);
                }
                return new ConcatNEnumerableIterator<TSource>(this, previous, (_index | int.MinValue) + 1);
            }

            // Copy all of the elements in the iterator to the specified array.
            // Collections from prepended nodes are copied starting at startIndex.
            // Collections from appended nodes are copied ending at startIndex + count - 1.
            private void CopyTo(TSource[] array, int startIndex, int count)
            {
                Debug.Assert(array != null);
                Debug.Assert(startIndex >= 0);
                Debug.Assert(count >= 0);
                Debug.Assert(array.Length - startIndex >= count);
                Debug.Assert(count >= Count);

                int lowerBound = startIndex;
                int upperBound = startIndex + count;

                ConcatIterator<TSource> current;
                ConcatNCollectionIterator<TSource> currentN = this;

                do
                {
                    ICollection<TSource> concatee = currentN._concatee;
                    int toCopy = concatee.Count;

                    if (toCopy > 0)
                    {
                        checked
                        {
                            if (currentN.IsAppended)
                            {
                                // Copy towards the end of the array.
                                upperBound -= toCopy;
                                concatee.CopyTo(array, upperBound);
                            }
                            else
                            {
                                // Copy towards the beginning of the array.
                                concatee.CopyTo(array, lowerBound);
                                lowerBound += toCopy;
                            }
                        }
                    }
                    
                    current = currentN._linked;
                }
                while ((currentN = current as ConcatNCollectionIterator<TSource>) != null);

                // We've reached the first 2 collections that were concatenated
                var current2 = (Concat2CollectionIterator<TSource>)current;
                int currentCount = current2.Count;

                if (currentCount > 0)
                {
                    checked
                    {
                        upperBound -= currentCount; // We'll treat this node as an appended node
                    }
                    current2.CopyTo(array, upperBound);
                }

#if DEBUG
                int itemsPrepended = lowerBound - startIndex;
                int itemsAppended = (startIndex + count) - upperBound;
                Debug.Assert(itemsPrepended + itemsAppended == Count); // We should have copied all the elements
#endif
            }

            internal override IEnumerable<TSource> GetEnumerable(int index)
            {
                ConcatNCollectionIterator<TSource> current = this;
                while (true)
                {
                    Debug.Assert(index >= 0);

                    if (current.IsAppended)
                    {
                        if (index >= current._index)
                        {
                            return index > current._index ? null : current._concatee;
                        }
                    }
                    else if (index-- == 0)
                    {
                        return current._concatee;
                    }

                    var linkedN = current._linked as ConcatNCollectionIterator<TSource>;
                    if (linkedN != null)
                    {
                        current = linkedN;
                        continue;
                    }

                    var linked2 = (Concat2CollectionIterator<TSource>)current._linked;
                    Debug.Assert(index >= 0 && index <= 2); // index can be 2 if we prepend to a Concat2Iterator.
                    return linked2.GetEnumerable(index);
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
                CopyTo(result, startIndex: 0, count: result.Length);
                return result;
            }

            public override int GetCount(bool onlyIfCheap) => Count; // Getting the count is always cheap relative to manually iterating.
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

            internal abstract ConcatIterator<TSource> ConcatAfter(IEnumerable<TSource> next);

            internal abstract ConcatIterator<TSource> ConcatBefore(IEnumerable<TSource> previous);

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

            public List<TSource> ToList()
            {
                int count = GetCount(onlyIfCheap: true);
                var list = count != -1 ? new List<TSource>(count) : new List<TSource>();

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
        }
    }
}
