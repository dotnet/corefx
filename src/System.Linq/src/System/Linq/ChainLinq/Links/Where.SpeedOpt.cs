using System.Collections.Generic;

namespace System.Linq.ChainLinq.Links
{
    internal partial class Where<T>
        : Optimizations.IMergeSelect<T>
        , Optimizations.IMergeWhere<T>
    {
        public Consumable<U> MergeSelect<U>(ConsumableForMerging<T> consumable, Func<T, U> selector) =>
            consumable.ReplaceTailLink(new WhereSelect<T, U>(Predicate, selector));

        public virtual Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> second) =>
            consumable.ReplaceTailLink(new Where2<T>(Predicate, second));

        sealed partial class Activity<U>
            : Optimizations.IPipelineArray<T>
            , Optimizations.IPipelineList<T>
            , Optimizations.IPipelineEnumerable<T>
        {
            public void Pipeline(T[] array)
            {
                foreach (var item in array)
                {
                    if (_predicate(item))
                    {
                        var state = Next(item);
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            public void Pipeline(List<T> list)
            {
                foreach (var item in list)
                {
                    if (_predicate(item))
                    {
                        var state = Next(item);
                        if (state.IsStopped())
                            break;
                    }
                }
            }

            public void Pipeline(IEnumerable<T> enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (_predicate(item))
                    {
                        var state = Next(item);
                        if (state.IsStopped())
                            break;
                    }
                }
            }
        }
    }

    sealed class Where2<T> : Where<T>
    {
        private readonly Func<T, bool> _first;
        private readonly Func<T, bool> _second;

        public Where2(Func<T, bool> first, Func<T, bool> second) : base(t => first(t) && second(t)) =>
            (_first, _second) = (first, second);

        public override Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> third) =>
            consumable.ReplaceTailLink(new Where3<T>(_first, _second, third));
    }

    sealed class Where3<T> : Where<T>
    {
        private readonly Func<T, bool> _first;
        private readonly Func<T, bool> _second;
        private readonly Func<T, bool> _third;

        public Where3(Func<T, bool> first, Func<T, bool> second, Func<T, bool> third) : base(t => first(t) && second(t) && third(t)) =>
            (_first, _second, _third) = (first, second, third);

        public override Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> forth) =>
            consumable.ReplaceTailLink(new Where4<T>(_first, _second, _third, forth));
    }

    sealed class Where4<T> : Where<T>
    {
        public Where4(Func<T, bool> first, Func<T, bool> second, Func<T, bool> third, Func<T, bool> forth)
            : base(t => first(t) && second(t) && third(t) && forth(t)) { }
    }
}
