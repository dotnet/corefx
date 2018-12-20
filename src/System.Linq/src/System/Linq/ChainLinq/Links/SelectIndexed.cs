namespace System.Linq.ChainLinq.Links
{
    sealed partial class SelectIndexed<T, U> : ILink<T, U>
    {
        readonly int _startIndex;
        readonly Func<T, int, U> _selector;

        private SelectIndexed(Func<T, int, U> selector, int startIndex) =>
            (_selector, _startIndex) = (selector, startIndex);

        public SelectIndexed(Func<T, int, U> selector) : this(selector, 0) { }

        public Chain<T, V> Compose<V>(Chain<U, V> activity) =>
            new Activity<V>(_selector, _startIndex, activity);

        sealed class Activity<V> : Activity<T, U, V>
        {
            private readonly Func<T, int, U> _selector;

            private int _index;

            public Activity(Func<T, int, U> selector, int startIndex, Chain<U, V> next) : base(next)
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
