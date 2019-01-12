using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.ConsumerEnumerators
{
    internal sealed class Lookup<TKey, TElement, TResult> : ConsumerEnumerator<TResult>
    {
        private Grouping<TKey, TElement> _lastGrouping;
        private Grouping<TKey, TElement> _g;
        private Chain<IGrouping<TKey, TElement>> _chain = null;
        int _state;

        Link<IGrouping<TKey, TElement>, TResult> _factory;
        internal override Chain StartOfChain => _chain;

        public Lookup(Grouping<TKey, TElement> lastGrouping, Link<IGrouping<TKey, TElement>, TResult> factory) =>
            (_lastGrouping, _factory, _state) = (lastGrouping, factory, Initialization);

        public override void ChainDispose()
        {
            _lastGrouping = null;
            _g = null;
            _factory = null;
            _chain = null;
        }

        const int Initialization = 0;
        const int ProcessGrouping = 1;
        const int Finished = 2;
        const int PostFinished = 3;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _chain ?? _factory.Compose(this);
                    _factory = null;

                    if (_lastGrouping == null)
                    {
                        _state = Finished;
                        goto case Finished;
                    }
                    _g = _lastGrouping;

                    _state = ProcessGrouping;
                    goto case ProcessGrouping;

                case ProcessGrouping:
                    if (status.IsStopped())
                    {
                        _lastGrouping = null;
                        _g = null;
                        _state = Finished;
                        goto case Finished;
                    }

                    _g = _g._next;
                    status = _chain.ProcessNext(_g);
                    var flowing = status.IsFlowing();
                    if (_g == _lastGrouping)
                        status = ChainStatus.Stop;

                    if (flowing)
                    {
                        return true;
                    }

                    Debug.Assert(_state == ProcessGrouping);
                    goto case ProcessGrouping;

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

    internal sealed class Lookup<TKey, TElement, TResult, Result> : ConsumerEnumerator<Result>
    {
        private Grouping<TKey, TElement> _lastGrouping;
        private Grouping<TKey, TElement> _g;
        private Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;
        private Chain<TResult> _chain = null;
        int _state;

        Link<TResult, Result> _factory;
        internal override Chain StartOfChain => _chain;

        public Lookup(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Link<TResult, Result> factory) =>
            (_lastGrouping, _resultSelector, _factory, _state) = (lastGrouping, resultSelector, factory, Initialization);

        public override void ChainDispose()
        {
            _lastGrouping = null;
            _g = null;
            _factory = null;
            _chain = null;
        }

        const int Initialization = 0;
        const int ProcessGrouping = 1;
        const int Finished = 2;
        const int PostFinished = 3;

        public override bool MoveNext()
        {
            switch (_state)
            {
                case Initialization:
                    _chain = _chain ?? _factory.Compose(this);
                    _factory = null;

                    if (_lastGrouping == null)
                    {
                        _state = Finished;
                        goto case Finished;
                    }
                    _g = _lastGrouping;

                    _state = ProcessGrouping;
                    goto case ProcessGrouping;

                case ProcessGrouping:
                    if (status.IsStopped())
                    {
                        _lastGrouping = null;
                        _g = null;
                        _state = Finished;
                        goto case Finished;
                    }

                    _g = _g._next;
                    _g.Trim();
                    status = _chain.ProcessNext(_resultSelector(_g.Key, _g._elements));
                    var flowing = status.IsFlowing();
                    if (_g == _lastGrouping)
                        status = ChainStatus.Stop;

                    if (flowing)
                    {
                        return true;
                    }

                    Debug.Assert(_state == ProcessGrouping);
                    goto case ProcessGrouping;

                case Finished:
                    base.Result = default;
                    _chain.ChainComplete();
                    _state = PostFinished;
                    return false;

                default:
                    return false;
            }
        }
    }
}
