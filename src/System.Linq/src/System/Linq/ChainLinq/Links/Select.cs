namespace System.Linq.ChainLinq.Links
{
    partial class Select<T, U> : Link<T, U>
    {
        public Select(Func<T, U> selector) : base(LinkType.Select) =>
            Selector = selector;

        public Func<T, U> Selector { get; }

        public override Chain<T> Compose(Chain<U> activity) =>
            new Activity(Selector, activity);

        sealed partial class Activity : Activity<T, U>
        {
            private readonly Func<T, U> _selector;

            public Activity(Func<T, U> selector, Chain<U> next) : base(next) =>
                _selector = selector;

            public override ChainStatus ProcessNext(T input) =>
                Next(_selector(input));
        }
    }
}
