using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class List<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        internal List<T> Underlying { get; }

        public List(List<T> array, Link<T, V> first) : base(first) =>
            Underlying = array;

        public override Consumable<V> Create   (Link<T, V> first) => new List<T, V>(Underlying, first);
        public override Consumable<W> Create<W>(Link<T, W> first) => new List<T, W>(Underlying, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.List.Get(this);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.List.Invoke(Underlying, Link, consumer);
    }
}
