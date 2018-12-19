namespace System.Linq.ChainLinq.Links
{
    internal class Skip<T> : ILink<T, T>
    {
        private int _count;

        public Skip(int count) =>
            _count = count;

        public Chain<T, V> Compose<V>(Chain<T, V> activity) =>
            new Activity<V>(_count, activity);

        sealed class Activity<V> : Activity<T, T, V>
        {
            private readonly int count;

            private int index;

            public Activity(int count, Chain<T, V> next) : base(next) =>
                (this.count, index) = (count, 0);

            public override ChainStatus ProcessNext(T input)
            {
                checked
                {
                    index++;
                }

                if (index <= count)
                {
                    return ChainStatus.Filter;
                }
                return Next(input);
            }
        }
    }
}
