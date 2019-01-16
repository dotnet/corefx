using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed class Enumerable<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        internal IEnumerable<T> Underlying { get; }

        public Enumerable(IEnumerable<T> enumerable, Link<T, V> link) : base(link) =>
            Underlying = enumerable;

        public override Consumable<V> Create   (Link<T, V> first) => new Enumerable<T, V>(Underlying, first);
        public override Consumable<W> Create<W>(Link<T, W> first) => new Enumerable<T, W>(Underlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Enumerable.Get(this);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(Underlying, Link, consumer);
    }
}
