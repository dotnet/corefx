namespace System.Linq.ChainLinq.Links
{
    sealed partial class SelectIndexed<T, U> : Link<T, U>
    {
        readonly int _startIndex;
        readonly Func<T, int, U> _selector;

        private SelectIndexed(Func<T, int, U> selector, int startIndex) : base(LinkType.SelectIndexed) =>
            (_selector, _startIndex) = (selector, startIndex);

        public SelectIndexed(Func<T, int, U> selector) : this(selector, 0) { }

        public override Chain<T> Compose(Chain<U> activity) =>
            new Activity(_selector, _startIndex, activity);

        sealed class Activity : Activity<T, U>
        {
            private readonly Func<T, int, U> _selector;

            private int _index;

            public Activity(Func<T, int, U> selector, int startIndex, Chain<U> next) : base(next)
            {
                _selector = selector;
                checked
                {
                    _index = startIndex - 1;
                }
            }

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
