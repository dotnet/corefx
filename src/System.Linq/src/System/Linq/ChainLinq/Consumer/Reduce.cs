namespace System.Linq.ChainLinq.Consumer
{
    sealed class Reduce<T> : Consumer<T, T>
    {
        readonly Func<T, T, T> _func;
        bool _first;

        public Reduce(Func<T, T, T> func) : base(default) =>
            (_func, _first) = (func, true);

        public override ChainStatus ProcessNext(T input)
        {
            if (_first)
            {
                _first = false;
                Result = input;
            }
            else
            {
                Result = _func(Result, input);
            }

            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
                throw Error.NoElements();

            base.ChainComplete();
        }
    }
}
