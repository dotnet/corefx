using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Repeat<T, U> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T>
    {
        private readonly T _element;
        private readonly int _count;

        public Repeat(T element, int count, Link<T, U> first) : base(first) =>
            (_element, _count) = (element, count);

        public override Consumable<U> Create   (Link<T, U> first) => new Repeat<T, U>(_element, _count, first);
        public override Consumable<V> Create<V>(Link<T, V> first) => new Repeat<T, V>(_element, _count, first);

        public override IEnumerator<U> GetEnumerator() =>
            ChainLinq.GetEnumerator.Repeat.Get(_element, _count, Link);

        public override TResult Consume<TResult>(Consumer<U, TResult> consumer) =>
            ChainLinq.Consume.Repeat.Invoke(_element, _count, Link, consumer);
    }
}
