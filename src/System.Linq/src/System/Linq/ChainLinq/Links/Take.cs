namespace System.Linq.ChainLinq.Links
{
    sealed partial class Take<T> : Link<T, T>
    {
        private int _count;

        public Take(int count) : base(LinkType.Take) =>
            _count = count;

        public override Chain<T, V> Compose<V>(Chain<T, V> activity) =>
            new Activity<V>(_count, activity);

        sealed class Activity<V> : Activity<T, T, V>
        {
            private readonly int count;

            private int index;

            public Activity(int count, Chain<T, V> next) : base(next) =>
                (this.count, index) = (count, 0);

            public override ChainStatus ProcessNext(T input)
            {
                if (index >= count)
                {
                    return ChainStatus.Stop;
                }

                checked
                {
                    index++;
                }

                if (index >= count)
                    return ChainStatus.Stop | Next(input);
                else
                    return Next(input);
            }
        }
    }
}
