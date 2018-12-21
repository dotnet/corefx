using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Range<TResult> : ConsumerEnumerator<TResult>
    {
        private readonly int _end;
        private Chain<int> _chain = null;

        int _current;

        internal override Chain StartOfChain => _chain;

        public Range(int start, int count, ILink<int, TResult> factory)
        {
            Debug.Assert(count > 0);

            _current = start;
            _end = unchecked(start + count);

            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            base.ChainComplete();
            _chain = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (_current == _end || status.IsStopped())
            {
                Result = default;
                _chain.ChainComplete();
                return false;
            }

            status = _chain.ProcessNext(_current++);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
