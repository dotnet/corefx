using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Array<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        internal T[] Underlying { get; }

        private readonly int _start;
        private readonly int _length;

        public Array(T[] array, int start, int length, Link<T, V> first) : base(first) =>
            (Underlying, _start, _length) = (array, start, length);

        public override Consumable<V> Create   (Link<T, V> first) => new Array<T, V>(Underlying, _start, _length, first);
        public override Consumable<W> Create<W>(Link<T, W> first) => new Array<T, W>(Underlying, _start, _length, first);

        public override IEnumerator<V> GetEnumerator() =>
            ChainLinq.GetEnumerator.Array.Get(Underlying, _start, _length, Link);

        public override void Consume(Consumer<V> consumer) =>
            ChainLinq.Consume.ReadOnlyMemory.Invoke(new ReadOnlyMemory<T>(Underlying, _start, _length), Link, consumer);
    }
}
