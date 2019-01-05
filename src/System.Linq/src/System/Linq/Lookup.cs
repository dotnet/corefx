// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            ToLookup(source, keySelector, null);

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (keySelector == null)
            {
                throw Error.ArgumentNull(nameof(keySelector));
            }

            return ChainLinq.Utils.AsConsumable(source).Consume(new ChainLinq.Consumer.Lookup<TSource, TKey>(keySelector, comparer));
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
            ToLookup(source, keySelector, elementSelector, null);

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (keySelector == null)
            {
                throw Error.ArgumentNull(nameof(keySelector));
            }

            if (elementSelector == null)
            {
                throw Error.ArgumentNull(nameof(elementSelector));
            }

            return ChainLinq.Utils.AsConsumable(source).Consume(new ChainLinq.Consumer.LookupSplit<TSource, TKey, TElement>(keySelector, elementSelector, comparer));
        }
    }

    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        int Count { get; }

        IEnumerable<TElement> this[TKey key] { get; }

        bool Contains(TKey key);
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(SystemLinq_LookupDebugView<,>))]
    public partial class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private Grouping<TKey, TElement>[] _groupings;
        private Grouping<TKey, TElement> _lastGrouping;
        private int _count;

        internal Lookup(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
            _groupings = new Grouping<TKey, TElement>[7];
        }

        public int Count => _count;

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                Grouping<TKey, TElement> grouping = GetGrouping(key, create: false);
                if (grouping != null)
                {
                    return grouping;
                }

                return ChainLinq.Consumables.Empty<TElement>.Instance;
            }
        }

        public bool Contains(TKey key) => GetGrouping(key, create: false) != null;

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            Grouping<TKey, TElement> g = _lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    yield return g;
                }
                while (g != _lastGrouping);
            }
        }

        //internal List<TResult> ToList<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        //{
        //    List<TResult> list = new List<TResult>(_count);
        //    Grouping<TKey, TElement> g = _lastGrouping;
        //    if (g != null)
        //    {
        //        do
        //        {
        //            g = g._next;
        //            g.Trim();
        //            list.Add(resultSelector(g._key, g._elements));
        //        }
        //        while (g != _lastGrouping);
        //    }

        //    return list;
        //}

        public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            Grouping<TKey, TElement> g = _lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    g.Trim();
                    yield return resultSelector(g._key, g._elements);
                }
                while (g != _lastGrouping);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int InternalGetHashCode(TKey key)
        {
            // Handle comparer implementations that throw when passed null
            return (key == null) ? 0 : _comparer.GetHashCode(key) & 0x7FFFFFFF;
        }

        internal Grouping<TKey, TElement> GetGrouping(TKey key, bool create)
        {
            int hashCode = InternalGetHashCode(key);
            for (Grouping<TKey, TElement> g = _groupings[hashCode % _groupings.Length]; g != null; g = g._hashNext)
            {
                if (g._hashCode == hashCode && _comparer.Equals(g._key, key))
                {
                    return g;
                }
            }

            if (create)
            {
                if (_count == _groupings.Length)
                {
                    Resize();
                }

                int index = hashCode % _groupings.Length;
                Grouping<TKey, TElement> g = new Grouping<TKey, TElement>();
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
                _count++;
                return g;
            }

            return null;
        }

        private void Resize()
        {
            int newSize = checked((_count * 2) + 1);
            Grouping<TKey, TElement>[] newGroupings = new Grouping<TKey, TElement>[newSize];
            Grouping<TKey, TElement> g = _lastGrouping;
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
    }
}
