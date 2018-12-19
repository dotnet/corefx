﻿using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.Consumables
{
    internal class Concat<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly IEnumerable<T> _firstOrNull;
        private readonly IEnumerable<T> _second;
        private readonly IEnumerable<T> _thirdOrNull;

        public Concat(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, ILink<T, V> link) : base(link) =>
            (_firstOrNull, _second, _thirdOrNull) = (firstOrNull, second, thirdOrNull);

        public override Consumable<W> Create<W>(ILink<T, W> link) =>
            new Concat<T, W>(_firstOrNull, _second, _thirdOrNull, link);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Concat.Get(_firstOrNull, _second, _thirdOrNull, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Concat.Invoke(_firstOrNull, _second, _thirdOrNull, Link, consumer);

        public Consumable<V> Append(IEnumerable<V> next)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull == null)
                {
                    Debug.Assert(_firstOrNull != null);
                    return new Concat<T, V>(_firstOrNull, _second, (IEnumerable<T>)next, Link);
                }

                if (_firstOrNull == null)
                {
                    Debug.Assert(_thirdOrNull != null);
                    return new Concat<T, V>(_second, _thirdOrNull, (IEnumerable<T>)next, Link);
                }
            }

            return new Concat<V, V>(this, next, null, Links.Identity<V>.Instance);
        }

        public Consumable<V> Prepend(IEnumerable<V> prior)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull == null)
                {
                    Debug.Assert(_firstOrNull != null);
                    return new Concat<T, V>((IEnumerable<T>)prior, _firstOrNull, _second, Link);
                }

                if (_firstOrNull == null)
                {
                    Debug.Assert(_thirdOrNull != null);
                    return new Concat<T, V>((IEnumerable<T>)prior, _second, _thirdOrNull, Link);
                }
            }

            return new Concat<V, V>(null, prior, this, Links.Identity<V>.Instance);
        }

        public Consumable<V> Append(V element)
        {
            if (IsIdentity)
            {
                if (_thirdOrNull is Appender<V> appender)
                {
                    return new Concat<T, V>(_firstOrNull, _second, (IEnumerable<T>)appender.Add(element), Link);
                }
            }
            return Append(new Appender<V>(element));
        }

        public Consumable<V> Prepend(V element)
        {
            if (IsIdentity)
            {
                if (_firstOrNull is Prepender<V> prepender)
                {
                    return new Concat<T, V>((IEnumerable<T>)prepender.Push(element), _second, _thirdOrNull, Link);
                }
            }
            return Prepend(new Prepender<V>(element));
        }
    }
}
