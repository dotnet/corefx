using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Range<T> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<T, int>
    {
        private readonly int _start;
        private readonly int _count;

        public Range(int start, int count, ILink<int, T> first) : base(first) =>
            (_start, _count) = (start, count);

        public override Consumable<U> Create<U>(ILink<int, U> first) =>
            new Range<U>(_start, _count, first);

        public override IEnumerator<T> GetEnumerator() =>
            ChainLinq.GetEnumerator.Range.Get(_start, _count, Link);

        public override TResult Consume<TResult>(Consumer<T, TResult> consumer) =>
            ChainLinq.Consume.Range.Invoke(_start, _count, Link, consumer);
    }
}
