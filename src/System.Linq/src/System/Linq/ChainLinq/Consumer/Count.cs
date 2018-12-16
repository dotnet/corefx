namespace System.Linq.ChainLinq.Consumer
{
    sealed class Count<T> : Consumer<T, int>
    {
        public Count() : base(0) {}

        public override ChainStatus ProcessNext(T input)
        {
            checked
            {
                Result++;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class CountConditional<T> : Consumer<T, int>
    {
        private Func<T, bool> _selector;

        public CountConditional(Func<T, bool> selector) : base(0) =>
            _selector = selector;

        public override ChainStatus ProcessNext(T input)
        {
            if (_selector(input))
            {
                checked
                {
                    ++Result;
                }
            }
            return ChainStatus.Flow;
        }
    }

    sealed class LongCount<T> : Consumer<T, long>
    {
        public LongCount() : base(0L) { }

        public override ChainStatus ProcessNext(T input)
        {
            checked
            {
                Result++;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class LongCountConditional<T> : Consumer<T, long>
    {
        private Func<T, bool> _selector;

        public LongCountConditional(Func<T, bool> selector) : base(0L) =>
            _selector = selector;

        public override ChainStatus ProcessNext(T input)
        {
            if (_selector(input))
            {
                checked
                {
                    ++Result;
                }
            }
            return ChainStatus.Flow;
        }
    }
}
