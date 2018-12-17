using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    internal class Concat<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly IEnumerable<T> _first;
        private readonly IEnumerable<T> _second;

        public Concat(IEnumerable<T> first, IEnumerable<T> second, ILink<T, V> link) : base(link) =>
            (_first, _second) = (first, second);

        public override Consumable<W> Create<W>(ILink<T, W> link) =>
            new Concat<T, W>(_first, _second, link);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Concat.Get(_first, _second, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Concat.Invoke(_first, _second, Link, consumer);
    }
}
