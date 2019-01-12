using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class SelectMany<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly Consumable<IEnumerable<T>> _selectMany;

        public SelectMany(Consumable<IEnumerable<T>> enumerable, Link<T, V> first) : base(first) =>
            _selectMany = enumerable;

        public override Consumable<V> Create   (Link<T, V> first) => new SelectMany<T, V>(_selectMany, first);
        public override Consumable<W> Create<W>(Link<T, W> first) => new SelectMany<T, W>(_selectMany, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, Link, consumer);
    }

    sealed partial class SelectMany<TSource, TCollection, T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly Consumable<(TSource, IEnumerable<TCollection>)> _selectMany;
        private readonly Func<TSource, TCollection, T> _resultSelector;

        public SelectMany(Consumable<(TSource, IEnumerable<TCollection>)> enumerable, Func<TSource, TCollection, T> resultSelector, Link<T, V> first) : base(first) =>
            (_selectMany, _resultSelector) = (enumerable, resultSelector);

        public override Consumable<V> Create   (Link<T, V> first) => new SelectMany<TSource, TCollection, T, V>(_selectMany, _resultSelector, first);
        public override Consumable<W> Create<W>(Link<T, W> first) => new SelectMany<TSource, TCollection, T, W>(_selectMany, _resultSelector, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.SelectMany.Get(_selectMany, _resultSelector, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.SelectMany.Invoke(_selectMany, _resultSelector, Link, consumer);
    }
}
