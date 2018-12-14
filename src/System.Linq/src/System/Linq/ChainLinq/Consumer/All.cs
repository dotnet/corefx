namespace System.Linq.ChainLinq.Consumer
{
    sealed class All<T> : Consumer<T, bool>
    {
        private Func<T, bool> _selector;

        public All(Func<T, bool> selector) : base(true) =>
            _selector = selector;

        public override ChainStatus ProcessNext(T input)
        {
            if (!_selector(input))
            {
                Result = false;
                return ChainStatus.StoppedConsumer;
            }
            return ChainStatus.Flow;
        }
    }
}
