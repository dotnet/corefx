using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    partial class List<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly List<T> _list;

        public List(List<T> array, ILink<T, V> first) : base(first) =>
            _list = array;

        public override Consumable<W> Create<W>(ILink<T, W> first) =>
            new List<T, W>(_list, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.List.Get(_list, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.List.Invoke(_list, Link, consumer);
    }
}
