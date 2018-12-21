namespace System.Linq.ChainLinq.Links
{
    sealed class WhereIndexed<T> : ILink<T, T>
    {
        public Func<T, int, bool> Predicate { get; }

        public WhereIndexed(Func<T, int, bool> predicate) =>
            Predicate = predicate;

        public Chain<T, U> Compose<U>(Chain<T, U> activity) =>
            new Activity<U>(Predicate, activity);

        sealed class Activity<U> : Activity<T, T, U>
        {
            private readonly Func<T, int, bool> _predicate;
            private int _index;

            public Activity(Func<T, int, bool> predicate, Chain<T, U> next) : base(next) =>
                (_predicate, _index) = (predicate, -1);

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    _index++;
                }

                return _predicate(input, _index) ? Next(input) : ChainStatus.Filter;
            }
                
        }
    }
}
