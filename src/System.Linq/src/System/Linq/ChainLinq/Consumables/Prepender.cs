using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    class Prepender<T> : Consumable<T>
    {
        readonly T _element;
        readonly Prepender<T> _previous;

        private Prepender(Prepender<T> previous, T element) =>
            (_previous, _element) = (previous, element);

        public Prepender(T element) : this(null, element) { }

        public override Consumable<U> AddTail<U>(ILink<T, U> transform) =>
            new Enumerable<T, U>(this, transform);

        public Prepender<T> Push(T element) =>
            new Prepender<T>(this, element);

        public override Result Consume<Result>(Consumer<T, Result> consumer)
        {
            try
            {
                var next = this;
                do
                {
                    consumer.ProcessNext(next._element);
                    next = next._previous;
                } while (next != null);
                consumer.ChainComplete();

                return consumer.Result;
            }
            finally
            {
                consumer.ChainDispose();
            }
        }

        public override IEnumerator<T> GetEnumerator()
        {
            var next = this;
            do
            {
                yield return next._element;
                next = next._previous;
            } while (next != null);
        }
    }
}
