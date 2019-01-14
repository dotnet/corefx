using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Appender<T>
        : Consumable<T>
        , IConsumableInternal
    {
        readonly T _element;
        readonly int _count;
        readonly Appender<T> _previous;

        private int AddCount() =>
            _count < 0 ? _count : Math.Max(-1, _count + 1);

        private Appender(Appender<T> previous, T element, int count) =>
            (_previous, _element, _count) = (previous, element, count);

        public Appender(T element) : this(null, element, 1) { }

        public Appender<T> Add(T element) =>
            new Appender<T>(this, element, AddCount());

        private Prepender<T> Reverse()
        {
            var p = new Prepender<T>(_element);
            var next = _previous;
            while (next != null)
            {
                p = p.Push(next._element);
                next = next._previous;
            }
            return p;
        }

        public override void Consume(Chain<T> consumer)
        {
            var reversed = Reverse();
            reversed.Consume(consumer);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            var reversed = Reverse();
            return reversed.GetEnumerator();
        }
    }
}
