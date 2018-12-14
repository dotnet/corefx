namespace System.Linq.ChainLinq.Links
{
    internal class SelectLink<T, U> : ILink<T, U>
    {
        public SelectLink(Func<T, U> selector) =>
            Selector = selector;

        public Func<T, U> Selector { get; }

        public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(Selector, activity);

        sealed class Activity<V> : Activity<T, U, V>
        {
            private readonly Func<T, U> _selector;

            public Activity(Func<T, U> selector, Chain<U, V> next) : base(next) =>
                _selector = selector;

            public override ChainStatus ProcessNext(T input) =>
                Next(_selector(input));
        }
    }
}
