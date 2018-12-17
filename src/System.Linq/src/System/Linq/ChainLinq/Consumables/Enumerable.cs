using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    internal class Enumerable<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly IEnumerable<T> _enumerable;

        public Enumerable(IEnumerable<T> enumerable, ILink<T, V> link) : base(link) =>
            _enumerable = enumerable;

        public override Consumable<W> Create<W>(ILink<T, W> first) =>
            new Enumerable<T, W>(_enumerable, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Enumerable.Get(_enumerable, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(_enumerable, Link, consumer);
    }
}
