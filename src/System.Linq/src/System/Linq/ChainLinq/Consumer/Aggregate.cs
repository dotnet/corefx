namespace System.Linq.ChainLinq.Consumer
{
    sealed class Aggregate<T, TAccumulate, TResult> : Consumer<T, TResult>
    {
        readonly Func<TAccumulate, T, TAccumulate> _func;
        readonly Func<TAccumulate, TResult> _resultSelector;
        
        TAccumulate _accumulate;

        public Aggregate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) : base(default) =>
            (_accumulate, _func, _resultSelector) = (seed, func, resultSelector);

        public override ChainStatus ProcessNext(T input)
        {
            _accumulate = _func(_accumulate, input);
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = _resultSelector(_accumulate);
            base.ChainComplete();
        }
    }
}
