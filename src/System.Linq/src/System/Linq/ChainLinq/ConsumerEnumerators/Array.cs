namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Array<T, TResult> : ConsumerEnumerator<TResult>
    {
        private T[] _array;
        private int _idx;
        private Chain<T> _chain = null;

        internal override Chain StartOfChain => _chain;

        public Array(T[] array, ILink<T, TResult> factory)
        {
            _array = array;
            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            _array = null;
            _chain = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (_idx >= _array.Length || status.IsStopped())
            {
                Result = default;
                _chain.ChainComplete();
                return false;
            }

            status = _chain.ProcessNext(_array[_idx++]);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
