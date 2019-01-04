namespace System.Linq.ChainLinq.Consumer
{
    sealed class Last<T> : Consumer<T, T>
    {
        private bool _found;
        private bool _orDefault;

        public Last(bool orDefault) : base(default) =>
            (_orDefault, _found) = (orDefault, false);

        public override ChainStatus ProcessNext(T input)
        {
            _found = true;
            Result = input;
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (!_orDefault && !_found)
            {
                throw Error.NoElements();
            }
        }
    }

    sealed class LastWithPredicate<T> : Consumer<T, T>
    {
        private Func<T, bool> _selector;
        private bool _found;
        private bool _orDefault;

        public LastWithPredicate(bool orDefault, Func<T, bool> selector) : base(default) =>
            (_orDefault, _selector) = (orDefault, selector);

        public override ChainStatus ProcessNext(T input)
        {
            if (_selector(input))
            {
                _found = true;
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (!_orDefault && !_found)
            {
                throw Error.NoElements();
            }
        }
    }
}
