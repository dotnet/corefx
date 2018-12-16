namespace System.Linq.ChainLinq.Consumer
{
    sealed class Any<T> : Consumer<T, bool>
    {
        private Func<T, bool> _selector;

        public Any(Func<T, bool> selector) : base(false) =>
            _selector = selector;

        public override ChainStatus ProcessNext(T input)
        {
            if (_selector(input))
            {
                Result = true;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }
    }
}
