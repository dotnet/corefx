using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq.ChainLinq.Consumables
{
    internal class SelectMany<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly Consumable<IEnumerable<T>> _selectMany;

        public SelectMany(Consumable<IEnumerable<T>> enumerable, ILink<T, V> first) : base(first) =>
            _selectMany = enumerable;

        public override Consumable<W> Create<W>(ILink<T, W> first) =>
            new SelectMany<T, W>(_selectMany, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, Link, consumer);
    }

    internal class SelectMany<TSource, TCollection, T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly Consumable<(TSource, IEnumerable<TCollection>)> _selectMany;
        private readonly Func<TSource, TCollection, T> _resultSelector;

        public SelectMany(Consumable<(TSource, IEnumerable<TCollection>)> enumerable, Func<TSource, TCollection, T> resultSelector, ILink<T, V> first) : base(first) =>
            (_selectMany, _resultSelector) = (enumerable, resultSelector);

        public override Consumable<W> Create<W>(ILink<T, W> first) =>
            new SelectMany<TSource, TCollection, T, W>(_selectMany, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, _resultSelector, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, _resultSelector, Link, consumer);
    }
}
