namespace System.Linq.ChainLinq.Consumer
{
    class SetResult<T> : Consumer<T, T>
    {
        public SetResult() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result = input;
            return ChainStatus.Flow;
        }
    }
}
