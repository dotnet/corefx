namespace System.Linq.ChainLinq.Links
{
    internal sealed class SelectWhere<T, U>
        : ILink<T, U>
        , Optimizations.IMergeWhere<U>
    {
        public Func<T, U> Selector { get; }
        public Func<U, bool> Predicate { get; }

        public SelectWhere(Func<T, U> selector, Func<U, bool> predicate) =>
            (Selector, Predicate) = (selector, predicate);

        public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(Selector, Predicate, activity);

        public Consumable<U> MergeWhere(ConsumableForMerging<U> consumable, Func<U, bool> second) =>
            consumable.ReplaceTailLink(new SelectWhere<T, U>(Selector, t => Predicate(t) && second(t)));

        sealed class Activity<V> : Activity<T, U, V>
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
        }
    }
}
