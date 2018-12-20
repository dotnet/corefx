using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    class Appender<T>
        : Consumable<T>
        , IConsumableInternal
        , Optimizations.ICountOnConsumable
    {
        readonly T _element;
        readonly Appender<T> _previous;

        private Appender(Appender<T> previous, T element) =>
            (_previous, _element) = (previous, element);

        public Appender(T element) : this(null, element) { }

        public Appender<T> Add(T element) =>
            new Appender<T>(this, element);

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

        public override Result Consume<Result>(Consumer<T, Result> consumer)
        {
            var reversed = Reverse();
            return reversed.Consume(consumer);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            var reversed = Reverse();
            return reversed.GetEnumerator();
        }

        public int GetCount(bool onlyIfCheap)
        {
            throw new NotImplementedException();
        }
    }
}
