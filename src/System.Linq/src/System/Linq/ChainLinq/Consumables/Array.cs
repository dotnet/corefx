using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Array<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly T[] _array;

        public Array(T[] array, Link<T, V> first) : base(first) =>
            _array = array;

        public override Consumable<W> Create<W>(Link<T, W> first) =>
            new Array<T, W>(_array, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Array.Get(_array, Link);

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Array.Invoke(_array, Link, consumer);
    }
}
