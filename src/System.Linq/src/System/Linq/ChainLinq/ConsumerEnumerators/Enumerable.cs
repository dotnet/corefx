using System.Collections.Generic;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal class Enumerable<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> _enumerable;
        private IEnumerator<T> _enumerator;
        private Chain<T> _chain = null;

        internal override Chain StartOfChain => _chain;

        public Enumerable(IEnumerable<T> enumerable, ILink<T, TResult> factory)
        {
            _enumerable = enumerable;
            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            base.ChainComplete();

            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
            _enumerable = null;
            _chain = null;
        }

        public override bool MoveNext()
        {
            if (_enumerable != null)
            {
                _enumerator = _enumerable.GetEnumerator();
                _enumerable = null;
            }

        tryAgain:
            if (status.IsStopped() || !_enumerator.MoveNext())
            {
                Result = default;
                _chain.ChainComplete();
                return false;
            }

            status = _chain.ProcessNext(_enumerator.Current);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
