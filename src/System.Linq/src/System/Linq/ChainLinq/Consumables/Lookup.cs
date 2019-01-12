﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.Consumables
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SystemLinq_ConsumablesLookupDebugView<,>))]
    internal partial class Lookup<TKey, TElement> 
        : ConsumableForAddition<IGrouping<TKey, TElement>>
        , ILookup<TKey, TElement>
        , IConsumableInternal
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private GroupingInternal<TKey, TElement>[] _groupings;
        private GroupingInternal<TKey, TElement> _lastGrouping;

        internal Lookup(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _groupings = new GroupingInternal<TKey, TElement>[7];
        }

        public int Count { get; private set; }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                Grouping<TKey, TElement> grouping = GetGrouping(key, create: false);
                if (grouping != null)
                {
                    return grouping;
                }

                return Empty<TElement>.Instance;
            }
        }

        public bool Contains(TKey key) => GetGrouping(key, create: false) != null;

        internal ConsumableForAddition<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
            new LookupResultsSelector<TKey, TElement, TResult>(_lastGrouping, resultSelector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int InternalGetHashCode(TKey key)
        {
            // Handle comparer implementations that throw when passed null
            return (key == null) ? 0 : _comparer.GetHashCode(key) & 0x7FFFFFFF;
        }

        internal GroupingInternal<TKey, TElement> GetGrouping(TKey key, bool create)
        {
            int hashCode = InternalGetHashCode(key);
            for (GroupingInternal<TKey, TElement> g = _groupings[hashCode % _groupings.Length]; g != null; g = g._hashNext)
            {
                if (g._hashCode == hashCode && _comparer.Equals(g._key, key))
                {
                    return g;
                }
            }

            if (create)
            {
                if (Count == _groupings.Length)
                {
                    Resize();
                }

                int index = hashCode % _groupings.Length;
                GroupingInternal<TKey, TElement> g = new GroupingInternal<TKey, TElement>();
                g._key = key;
                g._hashCode = hashCode;
                g._elements = new TElement[1];
                g._hashNext = _groupings[index];
                _groupings[index] = g;
                if (_lastGrouping == null)
                {
                    g._next = g;
                }
                else
                {
                    g._next = _lastGrouping._next;
                    _lastGrouping._next = g;
                }

                _lastGrouping = g;
                Count++;
                return g;
            }

            return null;
        }

        private void Resize()
        {
            int newSize = checked((Count * 2) + 1);
            GroupingInternal<TKey, TElement>[] newGroupings = new GroupingInternal<TKey, TElement>[newSize];
            GroupingInternal<TKey, TElement> g = _lastGrouping;
            do
            {
                g = g._next;
                int index = g._hashCode % newSize;
                g._hashNext = newGroupings[index];
                newGroupings[index] = g;
            }
            while (g != _lastGrouping);

            _groupings = newGroupings;
        }

        public override Consumable<IGrouping<TKey, TElement>> AddTail(Link<IGrouping<TKey, TElement>, IGrouping<TKey, TElement>> transform) =>
            new Lookup<TKey, TElement, IGrouping<TKey, TElement>>(_lastGrouping, transform);

        public override Consumable<U> AddTail<U>(Link<IGrouping<TKey, TElement>, U> transform) =>
            new Lookup<TKey, TElement, U>(_lastGrouping, transform);

        public override IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() =>
            ChainLinq.GetEnumerator.Lookup.Get(_lastGrouping, Links.Identity<IGrouping<TKey, TElement>>.Instance);

        public override Result Consume<Result>(Consumer<IGrouping<TKey, TElement>, Result> consumer) =>
            ChainLinq.Consume.Lookup.Invoke(_lastGrouping, Links.Identity<IGrouping<TKey,TElement>>.Instance, consumer);
    }

    sealed partial class Lookup<TKey, TValue, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, IGrouping<TKey, TValue>>
    {
        private readonly Grouping<TKey, TValue> _lastGrouping;

        public Lookup(Grouping<TKey, TValue> lastGrouping, Link<IGrouping<TKey, TValue>, V> first) : base(first) =>
            _lastGrouping = lastGrouping;

        public override Consumable<V> Create(Link<IGrouping<TKey, TValue>, V> first) =>
            new Lookup<TKey, TValue, V>(_lastGrouping, first);
        public override Consumable<W> Create<W>(Link<IGrouping<TKey, TValue>, W> first) =>
            new Lookup<TKey, TValue, W>(_lastGrouping, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Lookup.Get(_lastGrouping, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Lookup.Invoke(_lastGrouping, Link, consumer);
    }

    class LookupResultsSelector<TKey, TElement, TResult>
        : ConsumableForAddition<TResult>
        , IConsumableInternal
    {
        private readonly Grouping<TKey, TElement> _lastGrouping;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public LookupResultsSelector(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector) =>
            (_lastGrouping, _resultSelector) = (lastGrouping, resultSelector);

        public override Consumable<TResult> AddTail(Link<TResult, TResult> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, TResult>(_lastGrouping, _resultSelector, first);

        public override Consumable<W> AddTail<W>(Link<TResult, W> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, W>(_lastGrouping, _resultSelector, first);

        public override IEnumerator<TResult> GetEnumerator() =>
            ChainLinq.GetEnumerator.Lookup.Get(_lastGrouping, _resultSelector, Links.Identity<TResult>.Instance);

        public override Result Consume<Result>(Consumer<TResult, Result> consumer) =>
            ChainLinq.Consume.Lookup.Invoke(_lastGrouping, _resultSelector, Links.Identity<TResult>.Instance, consumer);
    }

    sealed partial class LookupResultsSelector<TKey, TElement, TResult, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, TResult>
    {
        private readonly Grouping<TKey, TElement> _lastGrouping;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public LookupResultsSelector(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Link<TResult, V> first) : base(first) =>
            (_lastGrouping, _resultSelector) = (lastGrouping, resultSelector);

        public override Consumable<V> Create(Link<TResult, V> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, V>(_lastGrouping, _resultSelector, first);
        public override Consumable<W> Create<W>(Link<TResult, W> first) =>
            new LookupResultsSelector<TKey, TElement, TResult, W>(_lastGrouping, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Lookup.Get(_lastGrouping, _resultSelector, Link);

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            ChainLinq.Consume.Lookup.Invoke(_lastGrouping, _resultSelector, Link, consumer);
    }


}
