namespace System.Linq.ChainLinq.Links
{
    partial class Select<T, U> : Link<T, U>
    {
        public Select(Func<T, U> selector) : base(LinkType.Select) =>
            Selector = selector;

        public Func<T, U> Selector { get; }

        public override Chain<T, ChainEnd> Compose(Chain<U, ChainEnd> activity) =>
            new Activity(Selector, activity);

        sealed partial class Activity : Activity<T, U, ChainEnd>
        {
            private readonly Func<T, U> _selector;

            public Activity(Func<T, U> selector, Chain<U, ChainEnd> next) : base(next) =>
                _selector = selector;

            public override ChainStatus ProcessNext(T input) =>
                Next(_selector(input));
        }
    }
}
