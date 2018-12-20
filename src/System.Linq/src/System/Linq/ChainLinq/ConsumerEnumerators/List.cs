using System.Collections.Generic;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal class List<T, TResult> : ConsumerEnumerator<TResult>
    {
        private List<T> _list;
        private int _idx;
        private Chain<T> _chain = null;

        internal override Chain StartOfChain => _chain;

        public List(List<T> list, ILink<T, TResult> factory)
        {
            _list = list;
            _chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            _list = null;
            _chain = null;
        }

        public override bool MoveNext()
        {
        tryAgain:
            if (_idx >= _list.Count || status.IsStopped())
            {
                Result = default;
                _chain.ChainComplete();
                return false;
            }

            status = _chain.ProcessNext(_list[_idx++]);
            if (!status.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}
