namespace System.Linq.ChainLinq.Links
{
    internal partial class Where<T> : ILink<T, T>
    {
        public Func<T, bool> Predicate { get; }

        public Where(Func<T, bool> predicate) =>
            Predicate = predicate;

        public Chain<T, U> Compose<U>(Chain<T, U> activity) =>
            new Activity<U>(Predicate, activity);

        sealed class Activity<U> : Activity<T, T, U>
        {
            private readonly Func<T, bool> _predicate;

            public Activity(Func<T, bool> predicate, Chain<T, U> next) : base(next) =>
                _predicate = predicate;

            public override ChainStatus ProcessNext(T input) =>
                _predicate(input) ? Next(input) : ChainStatus.Filter;
        }
    }
}
