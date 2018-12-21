using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Concat<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> _firstOrNull;
        private IEnumerable<T> _second;
        private IEnumerable<T> _thirdOrNull;
        private IEnumerator<T> _enumerator;

        ILink<T, TResult> _factory;
        private Chain<T> _chain = null;

        int _state;

        internal override Chain StartOfChain => _chain;

        public Concat(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, ILink<T, TResult> factory)
        {
            _state = Initialization;
            _firstOrNull = firstOrNull;
            _second = second;
            _thirdOrNull = thirdOrNull;
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
            _firstOrNull = null;
            _second = null;
            _thirdOrNull = null;
            _chain = null;
        }

        const int Initialization = 0;
        const int ReadFirstEnumerator = 1;
        const int ReadSecondEnumerator = 2;
        const int ReadThirdEnumerator = 3;
        const int Finished = 4;
        const int PostFinished = 5;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _factory.Compose(this);
                    if (_firstOrNull == null)
                    {
                        _enumerator = _second.GetEnumerator();
                        _second = null;
                        _state = ReadSecondEnumerator;
                        goto case ReadSecondEnumerator;
                    }
                    else
                    {
                        _enumerator = _firstOrNull.GetEnumerator();
                        _firstOrNull = null;
                        _state = ReadFirstEnumerator;
                        goto case ReadFirstEnumerator;
                    }

                case ReadFirstEnumerator:
                    if (status.IsStopped())
                    {
                        _enumerator.Dispose();
                        _enumerator = null;
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
                    if (status.IsStopped())
                    {
                        _enumerator.Dispose();
                        _enumerator = null;
                        _state = Finished;
                        goto case Finished;
                    }

                    if (!_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
                        if (_thirdOrNull == null)
                        {
                            _enumerator = null;
                            _state = Finished;
                            goto case Finished;
                        }
                        _enumerator = _thirdOrNull.GetEnumerator();
                        _thirdOrNull = null;
                        _state = ReadThirdEnumerator;
                        goto case ReadThirdEnumerator;
                    }

                    status = _chain.ProcessNext(_enumerator.Current);
                    if (status.IsFlowing())
                    {
                        return true;
                    }

                    Debug.Assert(_state == ReadSecondEnumerator);
                    goto case ReadSecondEnumerator;

                case ReadThirdEnumerator:
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

                    Debug.Assert(_state == ReadThirdEnumerator);
                    goto case ReadThirdEnumerator;

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
