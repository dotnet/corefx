namespace System.Linq.ChainLinq.Links
{
    sealed class WhereSelect<T, U>
        : ILink<T, U>
        , Optimizations.IMergeSelect<U>
    {
        public Func<T, bool> Predicate { get; }
        public Func<T, U> Selector { get; }

        public WhereSelect(Func<T, bool> predicate, Func<T, U> selector) =>
            (Predicate, Selector) = (predicate, selector);

        public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(Predicate, Selector, activity);

        public Consumable<V> MergeSelect<V>(ConsumableForMerging<U> consumable, Func<U, V> u2v) =>
            consumable.ReplaceTailLink(new WhereSelect<T, V>(Predicate, t => u2v(Selector(t))));

        sealed class Activity<V> : Activity<T, U, V>
        {
            private readonly Func<T, bool> _predicate;
            private readonly Func<T, U> _selector; 

            public Activity(Func<T, bool> predicate, Func<T, U> selector, Chain<U, V> next) : base(next) =>
                (_predicate, _selector) = (predicate, selector);

            public override ChainStatus ProcessNext(T input) =>
                _predicate(input) ? Next(_selector(input)) : ChainStatus.Filter;
        }
    }
}
