using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    internal partial class Select<T, U> 
        : Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>
        , Optimizations.IMergeSelect<U>
    {
        public virtual Consumable<V> MergeSelect<V>(ConsumableForMerging<U> consumable, Func<U, V> selector) =>
            consumable.ReplaceTailLink(new Select<T, U, V>(Selector, selector));

        public Link<T, U> Skip(int toSkip) => this;

        sealed partial class Activity<V>
            : Optimizations.IPipelineArray<T>
            , Optimizations.IPipelineList<T>
            , Optimizations.IPipelineEnumerable<T>
        {
            public void Pipeline(T[] array)
            {
                foreach (var item in array)
                {
                    var state = Next(_selector(item));
                    if (state.IsStopped())
                        break;
                }
            }

            public void Pipeline(IEnumerable<T> e)
            {
                foreach (var item in e)
                {
                    var state = Next(_selector(item));
                    if (state.IsStopped())
                        break;
                }
            }

            public void Pipeline(List<T> list)
            {
                foreach (var item in list)
                {
                    var state = Next(_selector(item));
                    if (state.IsStopped())
                        break;
                }
            }
        }
    }

    sealed class Select<T, U, V> : Select<T, V>
    {
        private readonly Func<T, U> _t2u;
        private readonly Func<U, V> _u2v;

        public Select(Func<T, U> t2u, Func<U, V> u2v) : base(t => u2v(t2u(t))) =>
            (_t2u, _u2v) = (t2u, u2v);

        public override Consumable<W> MergeSelect<W>(ConsumableForMerging<V> consumer, Func<V, W> v2w) =>
            consumer.ReplaceTailLink(new Select<T, U, V, W>(_t2u, _u2v, v2w));
    }

    sealed class Select<T, U, V, W> : Select<T, W>
    {
        private readonly Func<T, U> _t2u;
        private readonly Func<U, V> _u2v;
        private readonly Func<V, W> _v2w;

        public Select(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w) : base(t => v2w(u2v(t2u(t)))) =>
            (_t2u, _u2v, _v2w) = (t2u, u2v, v2w);

        public override Consumable<X> MergeSelect<X>(ConsumableForMerging<W> consumer, Func<W, X> w2x) =>
            consumer.ReplaceTailLink(new Select<T, U, V, W, X>(_t2u, _u2v, _v2w, w2x));
    }

    sealed class Select<T, U, V, W, X> : Select<T, X>
    {
        public Select(Func<T, U> t2u, Func<U, V> u2v, Func<V, W> v2w, Func<W, X> w2x)
            : base(t => w2x(v2w(u2v(t2u(t)))))
        { }
    }
}
