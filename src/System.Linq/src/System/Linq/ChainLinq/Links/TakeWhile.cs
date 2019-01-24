namespace System.Linq.ChainLinq.Links
{
    sealed partial class TakeWhile<T> : Link<T, T>
    {
        private readonly Func<T, bool> _predicate;

        public TakeWhile(Func<T, bool> predicate) : base(LinkType.TakeWhile) =>
            _predicate = predicate;

        public override Chain<T> Compose(Chain<T> activity) =>
            new Activity(_predicate, activity);

        sealed class Activity : Activity<T, T>
        {
            private readonly Func<T, bool> _predicate;

            public Activity(Func<T, bool> predicate, Chain<T> next) : base(next) =>
                _predicate = predicate;

            public override ChainStatus ProcessNext(T input)
            {
                if (_predicate(input))
                {
                    return Next(input);
                }
                return ChainStatus.Stop;
            }
        }
    }
}
