using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Enumerable<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> _enumerable;
        private IEnumerator<T> _enumerator;
        private Chain<T> _chain = null;
        int _state;

        ILink<T, TResult> _factory;
        internal override Chain StartOfChain => _chain;

        public Enumerable(IEnumerable<T> enumerable, ILink<T, TResult> factory) =>
            (_enumerable, _factory, _state) = (enumerable, factory, Initialization);

        public override void ChainDispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
            _enumerable = null;
            _factory = null;
            _chain = null;
        }

        const int Initialization = 0;
        const int ReadEnumerator = 1;
        const int Finished = 2;
        const int PostFinished = 3;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _chain ?? _factory.Compose(this);
                    _factory = null;
                    _enumerator = _enumerable.GetEnumerator();
                    _enumerable = null;
                    _state = ReadEnumerator;
                    goto case ReadEnumerator;

                case ReadEnumerator:
                    if (status.IsStopped() || !_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        _enumerator = null;
                        _state = Finished;
                        goto case Finished;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadEnumerator);
                    goto case ReadEnumerator;

                case Finished:
                    Result = default;
                    _chain.ChainComplete();
                    _state = PostFinished;
                    return false;

                default:
                    return false;
            }
        }
    }
}
