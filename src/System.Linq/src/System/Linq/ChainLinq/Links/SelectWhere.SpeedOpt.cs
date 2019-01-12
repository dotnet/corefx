using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    internal sealed class SelectWhere<T, U>
        : Link<T, U>
        , Optimizations.IMergeWhere<U>
    {
        public Func<T, U> Selector { get; }
        public Func<U, bool> Predicate { get; }

        public SelectWhere(Func<T, U> selector, Func<U, bool> predicate) : base(LinkType.SelectWhere) =>
            (Selector, Predicate) = (selector, predicate);

        public override Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(Selector, Predicate, activity);

        public Consumable<U> MergeWhere(ConsumableForMerging<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectWhere<T, U>(Selector, t => Predicate(t) && second(t)));

        sealed class Activity<V>
            : Activity<T, U, V>
            , Optimizations.IPipelineArray<T>
            , Optimizations.IPipelineList<T>
            , Optimizations.IPipelineEnumerable<T>
        {
            private readonly Func<T, U> _selector;
            private readonly Func<U, bool> _predicate;

            public Activity(Func<T, U> selector, Func<U, bool> predicate, Chain<U, V> next) : base(next) =>
                (_selector, _predicate) = (selector, predicate);

            public override ChainStatus ProcessNext(T input)
            {
                var item = _selector(input);
                return _predicate(item) ? Next(item) : ChainStatus.Filter;
            }

            public void Pipeline(T[] array)
            {
                foreach (var t in array)
                {
                    var u = _selector(t);
                    if (_predicate(u))
                    {
                        var state = Next(u);
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            public void Pipeline(List<T> list)
            {
                foreach (var t in list)
                {
                    var u = _selector(t);
                    if (_predicate(u))
                    {
                        var state = Next(u);
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            public void Pipeline(IEnumerable<T> enumerable)
            {
                foreach (var t in enumerable)
                {
                    var u = _selector(t);
                    if (_predicate(u))
                    {
                        var state = Next(u);
                        if (state.IsStopped())
                            break;
                    }
                }
            }
        }
    }
}
