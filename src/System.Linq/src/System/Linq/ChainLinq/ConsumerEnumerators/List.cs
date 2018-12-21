using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class List<T, TResult> : ConsumerEnumerator<TResult>
    {
        private List<T> _list;
        private List<T>.Enumerator _enumerator;
        private Chain<T> _chain = null;
        int _state;

        ILink<T, TResult> _factory;
        internal override Chain StartOfChain => _chain;

        public List(List<T> enumerable, ILink<T, TResult> factory) =>
            (_list, _factory, _state) = (enumerable, factory, Initialization);

        public override void ChainDispose()
        {
            _enumerator.Dispose();
            _list = null;
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
                    _enumerator = _list.GetEnumerator();
                    _list = null;
                    _state = ReadEnumerator;
                    goto case ReadEnumerator;

                case ReadEnumerator:
                    if (status.IsStopped() || !_enumerator.MoveNext())
                    {
                        _enumerator.Dispose();
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
