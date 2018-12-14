namespace System.Linq.ChainLinq.Links
{
    sealed class SelectIndexed<T, U> : ILink<T, U>
    {
        internal readonly Func<T, int, U> _selector;

        public SelectIndexed(Func<T, int, U> selector) =>
            _selector = selector;

        public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(_selector, activity);

        sealed class Activity<V> : Activity<T, U, V>
        {
            private readonly Func<T, int, U> _selector;
            private int _index;

            public Activity(Func<T, int, U> selector, Chain<U, V> next) : base(next) =>
                (_selector, _index) = (selector, -1);

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    _index++;
                }

                return Next(_selector(input, _index));
            }
        }
    }

}
