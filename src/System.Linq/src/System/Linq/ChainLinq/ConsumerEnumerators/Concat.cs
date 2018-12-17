using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal class Concat<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> _first;
        private IEnumerable<T> _second;
        private IEnumerator<T> _enumerator;

        ILink<T, TResult> _factory;
        private Chain<T> _chain = null;

        int _state;

        internal override Chain StartOfChain => _chain;

        public Concat(IEnumerable<T> first, IEnumerable<T> second, ILink<T, TResult> factory)
        {
            _state = Initialization;
            _first = first;
            _second = second;
            _factory = factory;
        }

        public override void ChainDispose()
        {
            base.ChainComplete();

            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
            _first = null;
            _second = null;
            _chain = null;
        }

        const int Initialization = 0;
        const int ReadFirstEnumerator = 1;
        const int ReadSecondEnumerator = 2;
        const int Finished = 3;
        const int PostFinished = 4;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _factory.Compose(this);
                    _enumerator = _first.GetEnumerator();
                    _first = null;
                    _state = ReadFirstEnumerator;
                    goto case ReadFirstEnumerator;

                case ReadFirstEnumerator:
                    if (status.IsStopped())
                    {
                        _state = Finished;
                        goto case Finished;
                    }

                    if (!_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        _enumerator = _second.GetEnumerator();
                        _second = null;
                        _state = ReadSecondEnumerator;
                        goto case ReadSecondEnumerator;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadFirstEnumerator);
                    goto case ReadFirstEnumerator;

                case ReadSecondEnumerator:
                    if (!_enumerator.MoveNext() || status.IsStopped())
                    {
                        _state = Finished;
                        goto case Finished;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadSecondEnumerator);
                    goto case ReadSecondEnumerator;

                case Finished:
                    Result = default;
                    _chain.ChainComplete();
                    _state = 4;
                    return false;

                default:
                    return false;

            }
        }
    }
}
